using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using Unity;
using Unity.Netcode;
using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using BepInEx.Configuration;
using System.IO;
using System.Reflection;
using DanceTools.Commands;
using static UnityEngine.EventSystems.EventTrigger;

namespace DanceTools
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class DanceTools : BaseUnityPlugin
    {
        //plugin info
        public const string pluginGUID = "dancemoon.lethalcompany.dancetools";
        public const string pluginName = "DanceTools";
        public const string pluginVersion = "1.1.3";

        private readonly Harmony harmony = new Harmony(pluginGUID); // harmony
        public static ManualLogSource mls; // logging
        internal static DanceTools Instance;
        internal static RoundManager currentRound;
        internal static SelectableLevel currentLevel;
        internal static bool isIngame = false;

        //Console things
        internal static GameObject consoleRef;
        internal static GameObject console; // obj manager
        internal static GameObject consoleHolder; // obj
        internal static KeyboardShortcut keyboardShortcut = new KeyboardShortcut(KeyCode.BackQuote); // ui key;
        public static bool consoleDebug;

        //commands
        public static List<ICommand> commands = new List<ICommand>();

        //enemy spawn command
        public static List<SpawnableEnemy> spawnableEnemies;

        //console colors
        public static string consolePlayerColor;
        public static string consoleSuccessColor;
        public static string consoleInfoColor;
        public static string consoleErrorColor;

        //host
        private static bool isHost;

        //player
        internal static bool playerGodMode = false;

        public void Awake()
        {
            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource("DanceTools");

            //load assetbundles

            //if dancetoolsconsole is not present in the DanceTools folder
            //do a check inside the plugins folder
            //Mod managers for some reason don't extract the folder correctly and 
            //dump the assetbundle dirrectly into the plugins folder.
            AssetBundle assets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DanceTools/dancetoolsconsole"));
            if(assets != null)
            {
                consoleRef = assets.LoadAsset<GameObject>("assets/prefabs/dancetoolsconsole.prefab"); //dancetoolsconsole
                mls.LogInfo("Loaded DanceTools AssetBundle");
            } else
            {
                mls.LogWarning("Failed to load DanceTools AssetBundle, trying fallback..");
                assets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "dancetoolsconsole"));
                if(assets != null)
                {
                    consoleRef = assets.LoadAsset<GameObject>("assets/prefabs/dancetoolsconsole.prefab");
                    mls.LogInfo("Loaded DanceTools AssetBundle from fallback");
                } else
                {
                    mls.LogFatal("Failed to load DanceTools AssetBundle");
                    return;
                }
            }
            //debug
            /*string temp = "";
            foreach(string asset in assets.GetAllAssetNames())
            {
                temp += asset;
            }
            mls.LogInfo(temp);
            */
            spawnableEnemies = new List<SpawnableEnemy>();
            //adding ui elements in game when game starts.
            if(consoleRef != null)
            {
                console = Instantiate(consoleRef);
                console.AddComponent<DTConsole>();
                console.AddComponent<DTCmdHandler>();
                console.hideFlags = HideFlags.HideAndDontSave; //important!!!
                DontDestroyOnLoad(console);
                InitConfig();
                mls.LogInfo("DanceTools Loaded :^]");
                harmony.PatchAll(typeof(DanceTools));
            } else
            {
                mls.LogFatal("No console assets present!!!!\nPlease check that you've installed everything correctly!!");
            }
        }

        private void InitConfig()
        {
            //console customization
            consolePlayerColor = Config.Bind("Console Customization", "Console Player Color", "#00FFF3", "Set player console color").Value;
            consoleSuccessColor = Config.Bind("Console Customization", "Console Success Color", "green", "Set success message console color").Value;
            consoleInfoColor = Config.Bind("Console Customization", "Console Info Color", "yellow", "Set info message console color").Value;
            consoleErrorColor = Config.Bind("Console Customization", "Console Error Color", "red", "Set error/fail message console color").Value;
            keyboardShortcut = Config.Bind("Console Customization", "Console Keybind", new KeyboardShortcut(KeyCode.BackQuote), "Set the shortcut key to open the console. Avaiable keys: https://docs.unity3d.com/ScriptReference/KeyCode.html").Value;
            consoleDebug = Config.Bind("Console Customization", "Console Debug", false, "Print debug text to console").Value;
            //add more settings here
        }

        // set host of the lobby
        [HarmonyPatch(typeof(RoundManager), "Start")]
        [HarmonyPrefix]
        private static void setHost()
        {
            isHost = RoundManager.Instance.NetworkManager.IsHost;
            currentRound = RoundManager.Instance;
            isIngame = true;
        }

        [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
        [HarmonyPrefix]
        private static void Disconnect()
        {
            isIngame = false;
        }

        // do once terminal starts (aka player in ship)
        // get a list of enemies from each moon/level. (Ty to MaskedEnemyOverhaul for this bit of code)
        [HarmonyPatch(typeof(Terminal), "Start")]
        [HarmonyPostfix]
        private static void GetAllEnemies(ref SelectableLevel[] ___moonsCatalogueList)
        {
            DTConsole.Instance.PushTextToOutput($"Ran get enemies", "white");
            SelectableLevel[] array = ___moonsCatalogueList;
            string temp = "Spawnable Enemy List Updated:\n";

            // FindObjectOfType<Terminal>().moonsCatalogueList

            foreach (EnemyType enemyType in Resources.FindObjectsOfTypeAll<EnemyType>())
            {
                if (enemyType.enemyPrefab == null)
                {
                    // Log a warning if the enemy prefab is missing, uncomment the code if you want 
                    /*
                    if (consoleDebug)
                    {
                        DTConsole.Instance.PushTextToOutput($"Enemy type {enemyType.name} is missing prefab", "red");
                    }
                    */
                    continue;
                }
                
                if (spawnableEnemies.Any((x) => x.name == enemyType.enemyName))
                {
                    // enemy exists in master list
                    if (consoleDebug)
                    {
                        DTConsole.Instance.PushTextToOutput($"{enemyType.enemyName} exists", "red");
                    }

                    continue;
                }

                SpawnableEnemy enemy = new SpawnableEnemy(enemyType.enemyPrefab, enemyType.enemyName, enemyType.isOutsideEnemy);
                spawnableEnemies.Add(enemy);
                temp += $"\n{enemy.name} | {(enemy.isOutside ? "outside" : "inside")}";
            }

            if (consoleDebug)
            {
                DTConsole.Instance.PushTextToOutput(temp, "white");
            }
        }

        public struct SpawnableEnemy
        {
            public GameObject prefab;
            public string name;
            public bool isOutside;
            //public int id; // using names to spawn enemies
            
            public SpawnableEnemy(GameObject prefab = null, string name = default, bool isOutside = false)
            {
                this.prefab = prefab;
                this.name = name;
                this.isOutside = isOutside;
            }
        }

        public static int CheckInt(string input)
        {
            // fix for invalid args
            if (int.TryParse(input, out int val))
            {
                return val;
            }
            
            else
            {
                DTConsole.Instance.PushTextToOutput($"Invalid Argument", consoleErrorColor);
                return -1;
            }
        }

        public static float CheckFloat(string input)
        {
            if (float.TryParse(input, out float val))
            {
                return val;
            }

            else
            {
                DTConsole.Instance.PushTextToOutput($"Invalid Argument", consoleErrorColor);
                return -1f;
            }
        }

        public static bool CheckHost()
        {
            // check if host
            if (isHost && isIngame) return true;
            DTConsole.Instance.PushTextToOutput($"You must be host to use this command", DanceTools.consoleErrorColor);
            return false;
        }

        // to display messages on hud (like warning message)
        public static void DMNotice(string title, string msg)
        {
            HUDManager.Instance.DisplayTip(title, msg);
        }
        
        // external commands (WIP)

        //thanks to GameMaster plugin
        [HarmonyPatch(typeof(PlayerControllerB), "AllowPlayerDeath")]
        [HarmonyPrefix]
        public static bool PreventPlayerDeath()
        {
            if (CheckHost()) {
                return true;
            }
            return !playerGodMode;
        }
 

        //external commands (WIP)
        public static void AddToCommandsList(ICommand cmd)
        {
            // var inst = (ICommand)Activator.CreateInstance(cmd);
            try
            { 
                commands.Add(cmd);
                mls.LogInfo($"Loaded external command {cmd.Name}!");
            } catch(Exception e)
            {
                mls.LogError(e.ToString());
            }
        }
    }

    //notes 
    //p.usernameBillboardText; sets player text above head?
    //GameNetcodeStuff.PlayerControllerB
    //idea shut someone up.
    //p.voiceMuffledByEnemy = true
}
