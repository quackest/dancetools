﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace DanceTools.Commands
{
    internal class ItemCommand : ICommand
    {
        public string Name => "item";

        public string Desc => "Spawns items on you\nUsage: item itemID amount value\nType just the command without arguments \nto see list of items";

        public void DisplayCommandDesc()
        {
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
        }

        //args string array doesn't include the original command
        //ie: if you type "item 50 3 1"
        //you will only receive "50 3 1" in args
        public void ExecCommand(string[] args)
        {
            //check if host
            if (!DanceTools.CheckHost()) return;

            if (args.Length < 1)
            {

                AllItemsList itemList = StartOfRound.Instance.allItemsList;
                string itemPrint = $"\nItem List (ID | Name) Total items: {itemList.itemsList.Count}";

                for (int i = 0; i < itemList.itemsList.Count; i++)
                {
                    itemPrint += $"\n{i} | {itemList.itemsList[i].itemName}";
                }
                //mls.LogInfo($"{itemPrint}"); itemList.itemsList.Count
                DTConsole.Instance.PushTextToOutput($"{itemPrint}", DanceTools.consoleSuccessColor);
                DTConsole.Instance.PushTextToOutput("Command usage: item itemID amount value weight", DanceTools.consoleInfoColor);
                return;
            }

            int value = 1;
            int amount = 1;
            int index = 0;
            float weight = 1f;

            //fix for invalid args
            index = DanceTools.CheckInt(args[0]);
            if (index == -1) return;

            //check if item is in the AllItemsList, if not, ignore it
            if (index > StartOfRound.Instance.allItemsList.itemsList.Count || index < 0)
            {
                DTConsole.Instance.PushTextToOutput($"Invalid Item ID: {index}", DanceTools.consoleErrorColor);
                return;
            }
            //item (id amount value)
            if (args.Length > 1)
            {
                //fix for invalid args
                amount = DanceTools.CheckInt(args[1]);
                if (amount == -1) return;

                if (amount <= 0)
                {
                    DTConsole.Instance.PushTextToOutput($"Amount cannot be 0 or less than 0", DanceTools.consoleErrorColor);
                    return;
                }

            }
            //GameNetworkManager.Instance.localPlayerController.transform.position
            
            Vector3 spawnPos = GameNetworkManager.Instance.localPlayerController.transform.position;

            if(GameNetworkManager.Instance.localPlayerController.isPlayerDead)
            {
                spawnPos = GameNetworkManager.Instance.localPlayerController.spectatedPlayerScript.transform.position;
                DTConsole.Instance.PushTextToOutput($"Spawning item on {GameNetworkManager.Instance.localPlayerController.spectatedPlayerScript.playerUsername}", DanceTools.consoleInfoColor);
            }

            //spawn multiple items
            for (int i = 0; i < amount; i++)
            {
                
                GameObject obj = UnityEngine.Object.Instantiate(StartOfRound.Instance.allItemsList.itemsList[index].spawnPrefab, spawnPos, Quaternion.identity);
                obj.GetComponent<GrabbableObject>().fallTime = 0f;
                
                // set cost for item
                if (args.Length > 2)
                {
                    value = DanceTools.CheckInt(args[2]);
                    //fix for invalid args
                    if (value == -1) 
                    {
                        value = 1;
                    }

                    // set weight for item
                    if (args.Length > 3)
                    {
                        weight = DanceTools.CheckFloat(args[3]);
                        if (weight == -1f)
                        {
                            weight = 1f;
                        }
                    }
                }
                
                
                obj.AddComponent<ScanNodeProperties>().scrapValue = value; //attach scanning node for value
                obj.GetComponent<GrabbableObject>().SetScrapValue(value); //give value to it
                //spawn it after giving it a value
                obj.GetComponent<NetworkObject>().Spawn();
                obj.GetComponent<GrabbableObject>().itemProperties.weight = weight;
            }
            
            DTConsole.Instance.PushTextToOutput($"Spawned {amount}x item {StartOfRound.Instance.allItemsList.itemsList[index].itemName}({index}) valued at {value}, with weight {weight} lbs", DanceTools.consoleSuccessColor);
        }
    }
}
