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
        public const string pluginVersion = "1.1.4";

        private readonly Harmony harmony = new Harmony(pluginGUID);//harmony
        public static ManualLogSource mls; //logging
        internal static DanceTools Instance;
        internal static RoundManager currentRound;
        internal static bool isIngame = false;

        //Console things
        internal static GameObject consoleRef;
        internal static GameObject console; //obj manager
        internal static GameObject consoleHolder; //obj
        internal static KeyboardShortcut keyboardShortcut = new KeyboardShortcut(KeyCode.BackQuote); //ui key;
        public static bool consoleDebug = false;

        //commands
        public static List<ICommand> commands = new List<ICommand>();

        //enemy spawn command
        public static List<SpawnableEnemy> spawnableEnemies = new List<SpawnableEnemy>();

        //items
        public static List<DTItem> spawnableItems = new List<DTItem>();

        //console customization
        public static string consolePlayerColor;
        public static string consoleSuccessColor;
        public static string consoleInfoColor;
        public static string consoleErrorColor;
        public static Color consoleOutputFieldColor = new Color(0, 0, 0, 0.78f);
        public static Color consoleInputFieldColor = new Color(0, 0, 0, 0.78f);
        public static bool consoleClearAfterOpening = false;

        //host
        internal static bool isHost;

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
            //text colors
            consolePlayerColor = Config.Bind("Console Customization", "Console Player Color", "#00FFF3", "Set player console color").Value;
            consoleSuccessColor = Config.Bind("Console Customization", "Console Success Color", "green", "Set success message console color").Value;
            consoleInfoColor = Config.Bind("Console Customization", "Console Info Color", "yellow", "Set info message console color").Value;
            consoleErrorColor = Config.Bind("Console Customization", "Console Error Color", "red", "Set error/fail message console color").Value;

            //console colors
            consoleOutputFieldColor = Config.Bind("Console Customization", "Console Output Field Color (Hex)", new Color(0, 0, 0, 0.78f), "Sets the color and opacity of the OUTPUT field background\nUse this tool to get a hex value with alpha: https://rgbacolorpicker.com/rgba-to-hex").Value;
            consoleInputFieldColor = Config.Bind("Console Customization", "Console Input Field Color (Hex)", new Color(0, 0, 0, 0.78f), "Sets the color and opacity of the INPUT field background\nUse this tool to get a hex value with alpha: https://rgbacolorpicker.com/rgba-to-hex").Value;

            //other console settings
            keyboardShortcut = Config.Bind("Console Customization", "Console Keybind", new KeyboardShortcut(KeyCode.BackQuote), "Set the shortcut key to open the console. Avaiable keys: https://docs.unity3d.com/ScriptReference/KeyCode.html").Value;
            consoleDebug = Config.Bind("Console Customization", "Console Debug", false, "Print debug text to console").Value;
            consoleClearAfterOpening = Config.Bind("Console Customization", "Auto-Clear console when opening", false, "Clears the console output window each time it is opened").Value;

            //load the settings if needed after hand
            DTConsole.Instance.SetCustomizationSettings();

        }

        

        //set host of the lobby
        [HarmonyPatch(typeof(RoundManager), "Start")]
        [HarmonyPrefix]
        static void setHost()
        {
            isHost = RoundManager.Instance.NetworkManager.IsHost;
            currentRound = RoundManager.Instance;
            isIngame = true;
        }

        [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
        [HarmonyPrefix]
        static void Disconnect()
        {
            isIngame = false;
        }

        //do once terminal starts (aka player in ship)
        //get a list of enemies from each moon/level. (Ty to MaskedEnemyOverhaul for this bit of code)
        [HarmonyPatch(typeof(Terminal), "Start")]
        [HarmonyPostfix]
        private static void GetAllEnemiesAndItems(ref SelectableLevel[] ___moonsCatalogueList)
        {
            GetAllItems();
            DTConsole.Instance.PushTextToOutput($"Ran get enemies", "white");
            SelectableLevel[] array = ___moonsCatalogueList;
            SpawnableEnemy enemy = new SpawnableEnemy();
            string temp = "Spawnable Enemy List Updated:\n";

            //FindObjectOfType<Terminal>().moonsCatalogueList
            //each moon
            for (int i = 0; i < array.Length; i++)
            {
                SelectableLevel level = array[i];
                //inside enemies begin
                for (int j =  0; j < level.Enemies.Count; j++)
                {
                    //each inside enemy
                    if (spawnableEnemies.Any((x) => x.name == level.Enemies[j].enemyType.enemyName))
                    {
                        //enemy exists in master list
                        if (consoleDebug)
                        {
                            DTConsole.Instance.PushTextToOutput($"{level.PlanetName} | {level.Enemies[j].enemyType.enemyName} exists", "red");
                        }
                        continue;
                    }
                    enemy.name = level.Enemies[j].enemyType.enemyName;
                    enemy.isOutside = level.Enemies[j].enemyType.isOutsideEnemy;
                    enemy.prefab = level.Enemies[j].enemyType.enemyPrefab;
                    spawnableEnemies.Add(enemy);
                    temp += $"\n{enemy.name} | {(enemy.isOutside ? "outside" : "inside")}";
                }//inside enemies end

                //outside enemies begin
                for (int j = 0; j < level.OutsideEnemies.Count; j++)
                {
                    //each outside enemy
                    if (spawnableEnemies.Any((x) => x.name == level.OutsideEnemies[j].enemyType.enemyName))
                    {
                        //enemy exists in master list debug
                        if (consoleDebug)
                        {
                            DTConsole.Instance.PushTextToOutput($"{level.PlanetName} | {level.OutsideEnemies[j].enemyType.enemyName} exists", "red");
                        }
                        continue;
                    }
                    enemy.name = level.OutsideEnemies[j].enemyType.enemyName;
                    enemy.isOutside = level.OutsideEnemies[j].enemyType.isOutsideEnemy;
                    enemy.prefab = level.OutsideEnemies[j].enemyType.enemyPrefab;
                    spawnableEnemies.Add(enemy);
                    temp += $"\n{enemy.name} | {(enemy.isOutside ? "outside" : "inside")}";
                } //outside enemies end

                if (consoleDebug)
                {
                    DTConsole.Instance.PushTextToOutput(temp, "white");
                }

            }//moonsList end

        }
        //why no work
        private static void GetAllItems()
        {
            //items
            DTItem DTitem = new DTItem();
            string itemTemp = "Started Items:";

            AllItemsList itemList = StartOfRound.Instance.allItemsList;

            itemTemp += $"{itemList.itemsList.Count} <- item 0";
            //items begin
            for (int i = 0; i < itemList.itemsList.Count; i++)
            {
                DTitem.name = itemList.itemsList[i].itemName;
                DTitem.id = i;
                DTitem.prefab = itemList.itemsList[i].spawnPrefab;

                spawnableItems.Add(DTitem);
                //itemTemp += $"\n{DTitem.name} | {DTitem.id}";
                itemTemp += $"\n{i} | {DTitem.name}";
            }
            //items end
            if (consoleDebug)
            {
                DTConsole.Instance.PushTextToOutput(itemTemp, "white");
                DTConsole.Instance.PushTextToOutput($"Ran get all items", "white");
            }
        }

        public struct SpawnableEnemy
        {
            public GameObject prefab;
            public string name;
            //public int id; //using names to spawn enemies
            public bool isOutside;
        }

        public struct DTItem
        {
            public GameObject prefab;
            public string name;
            public int id;
        }



        public static int CheckInt(string input)
        {
            //fix for invalid args
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
            //fix for invalid args
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
            //check if host
            if (!isHost || !isIngame)
            {
                DTConsole.Instance.PushTextToOutput($"You must be host to use this command", DanceTools.consoleErrorColor);
                return false;
            } else
            {
                return true;
            }
        }


        //to display messages on hud (like warning message)
        public static void DMNotice(string title, string msg)
        {
            HUDManager.Instance.DisplayTip(title, msg);
        }

        //thanks to GameMaster plugin
        [HarmonyPatch(typeof(PlayerControllerB), "AllowPlayerDeath")]
        [HarmonyPrefix]
        public static bool AllowPlayerDeath()
        {
            return !playerGodMode;
        }


        //external commands (WIP)
        public static void AddToCommandsList(ICommand cmd)
        {
            //var inst = (ICommand)Activator.CreateInstance(cmd);
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
