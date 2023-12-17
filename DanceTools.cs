﻿using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEngine;
using HarmonyLib;
using Unity;
using Unity.Netcode;
using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;

namespace danceTools
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class DanceTools : BaseUnityPlugin
    {
        //plugin info
        public const string pluginGUID = "dancemoon.lethalcompany.dancetools";
        public const string pluginName = "DanceTools";
        public const string pluginVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(pluginGUID);//harmony
        public static ManualLogSource mls; //logging
        internal static DanceTools Instance;
        internal static RoundManager currentRound;
        internal static SelectableLevel currentLevel;

        internal static bool isHost;


        public static string prefix = "."; //default
        public void Awake()
        {
            Instance = this;
            Logger.LogInfo("Hello guys!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            
            mls = BepInEx.Logging.Logger.CreateLogSource("ItemSpawner");
            // Plugin startup logic
            harmony.PatchAll(typeof(DanceTools));
        }

        //set host of the lobby
        [HarmonyPatch(typeof(RoundManager), "Start")]
        [HarmonyPrefix]
        static void setHost()
        {
            isHost = RoundManager.Instance.NetworkManager.IsHost;
            currentRound = RoundManager.Instance;
            //AllItemsList
        }

        //on chat message sent
        [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
        [HarmonyPrefix]
        static void ChatMessageSent(HUDManager __instance)
        {
            if (!isHost) return; //if not host, ignore 

            currentRound = RoundManager.Instance;
            string text = __instance.chatTextField.text;

            //mls.LogInfo($"{text}"); //debug
            //check if prefix is used
            if (!text.ToLower().StartsWith(prefix.ToLower())) return;

            //get specific command
            if (text.ToLower().StartsWith(prefix + "item"))
            {
                string[] msg = text.Split(' ');

                if(msg.Length < 2)
                {
                    DMNotice("Item Spawner", "Usage: .item itemID amount value\nCheck Bepin Console");

                    string itemPrint = "\nItem List (ID | Name)";

                    AllItemsList itemList = StartOfRound.Instance.allItemsList;
                    mls.LogInfo(itemList.itemsList.Count);

                    for (int i = 0; i < itemList.itemsList.Count; i++)
                    {
                        itemPrint += $"\n{i} | {itemList.itemsList[i].itemName}";
                    }
                    mls.LogInfo($"{itemPrint}");
                    CommandSent(__instance);
                    return;
                }
                int value = 0;
                int amount = 1;
                int index = int.Parse(msg[1]);

                //check if item is in the AllItemsList, if not, ignore it
                if(index > StartOfRound.Instance.allItemsList.itemsList.Count || index < 0)
                {
                    DMNotice($"Invalid Item ID ({index})", "Item doesn't exist in master item list");
                    CommandSent(__instance);
                    return;
                }
                //.item id amount value
                if(msg.Length > 2)
                {
                    amount = int.Parse(msg[2]);
                }
                
                //spawn multiple items
                for(int i  = 0; i < amount; i++)
                {
                    GameObject obj = Instantiate(StartOfRound.Instance.allItemsList.itemsList[index].spawnPrefab, GameNetworkManager.Instance.localPlayerController.transform.position, Quaternion.identity);
                    obj.GetComponent<GrabbableObject>().fallTime = 0f;
                    obj.GetComponent<NetworkObject>().Spawn();

                    //set cost for item
                    if(msg.Length > 3)
                    {
                        value = int.Parse(msg[3]);
                    }
                    obj.AddComponent<ScanNodeProperties>().scrapValue = value; //attach scanning node for value
                    obj.GetComponent<GrabbableObject>().SetScrapValue(value); //give value to it
                    //item 65 tragedy 
                }
                DMNotice($"Item ({index})", $"{StartOfRound.Instance.allItemsList.itemsList[index].itemName} at value {value}");
                CommandSent(__instance);
            }

            //
            if (text.ToLower().StartsWith(prefix + "enemy"))
            {
                string[] msg = text.Split(' ');
                //info message:
                if(msg.Length < 2)
                {
                    string consoleInfo = "\n";
                    string spawnableEnemies = "";

                    if(currentRound.currentLevel.Enemies.Count <= 0)
                    {
                        DMNotice("No Enemies", "No enemies to spawn in this level");
                        CommandSent(__instance);
                        return;
                    }

                    for(int i = 0; i < currentRound.currentLevel.Enemies.Count; i++)
                    {
                        consoleInfo += $"\n{i} | {currentRound.currentLevel.Enemies[i].enemyType.enemyName}";
                        spawnableEnemies += $"({i}) {currentRound.currentLevel.Enemies[i].enemyType.enemyName}, ";
                    }
                    mls.LogInfo(consoleInfo);
                    DMNotice("Enemy Spawn Command", $".enemy id \n{spawnableEnemies}");
                    CommandSent(__instance);
                    return;
                }
                try
                {
                
                    int index = int.Parse(msg[1]);

                //default = in vents
                Vector3 spawnPos = currentRound.allEnemyVents[UnityEngine.Random.Range(0, currentRound.allEnemyVents.Length)].floorNode.position;
                string noticeBody = "Spawned in a random vent inside";

                //spawn it in a random vent
                if(msg.Length > 2)
                {
                    if (msg[2] == "onme")
                    {
                        spawnPos = GameNetworkManager.Instance.localPlayerController.transform.position;
                        noticeBody = "Spawned on top of you";
                    }
                }
                currentRound.SpawnEnemyOnServer(spawnPos, 0f, index);

                DMNotice($"{currentRound.currentLevel.Enemies[index].enemyType.enemyName} Spawned!", noticeBody);
                CommandSent(__instance);
                } 
                catch(Exception e) 
                {
                    DMNotice("Couldn't spawn enemy", "Can't spawn enemies when not landed. Check console for more info");
                    mls.LogInfo($"error: {e.Message}");
                    CommandSent(__instance);
                }
            }
        }


            //do something when command is sent
            static void CommandSent(HUDManager ins)
        {
            ins.chatTextField.text = ""; //hide the command message
        }

        //to display messages
        static void DMNotice(string title, string msg)
        {
            HUDManager.Instance.DisplayTip(title, msg);
        }
    }

    //notes 
    //p.usernameBillboardText; sets player text above head?
    //GameNetcodeStuff.PlayerControllerB
    //idea shut someone up.
    //p.voiceMuffledByEnemy = true
}
