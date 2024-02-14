using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

namespace DanceTools.Commands
{
    internal class ItemCommand : ICommand
    {
        public string Name => "item";

        public string Desc => "Spawns items on you\nUsage: item itemID/itemName amount value weight\nType just the command without arguments \nto see list of items";

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
                string itemPrint = $"\nItem List (ID | Name) Total items: {DanceTools.spawnableItems.Count}";

                for (int i = 0; i < DanceTools.spawnableItems.Count; i++)
                {
                    itemPrint += $"\n{DanceTools.spawnableItems[i].id} | {DanceTools.spawnableItems[i].name}";
                }
                //mls.LogInfo($"{itemPrint}"); itemList.itemsList.Count
                DTConsole.Instance.PushTextToOutput($"{itemPrint}", DanceTools.consoleSuccessColor);
                DTConsole.Instance.PushTextToOutput("Command usage: item itemID/itemName amount value weight", DanceTools.consoleInfoColor);
                return;
            }

            int value = 1;
            int amount = 1;
            int index = 0;
            float weight = -1f;

            //both id and name search support
            if (int.TryParse(args[0], out int val))
            {
                //if it is id
                index = val;
            }
            else
            {
                //if it is item name
                if(!DanceTools.spawnableItems.Any((x) => x.name.ToLower().Contains(args[0])))
                {
                    DTConsole.Instance.PushTextToOutput($"Cannot find item by the name: {args[0]}", DanceTools.consoleErrorColor);
                    return;
                }

                index = DanceTools.spawnableItems.Find((x) => x.name.ToLower().Contains(args[0])).id;
            }

            //StartOfRound.Instance.allItemsList.itemsList.Find((x) => x.name.ToLower().Contains(itemName);

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

            //item modifiers

            //set cost for item
            if (args.Length > 2)
            {
                value = DanceTools.CheckInt(args[2]);
                //fix for invalid args
                if (value == -1)
                {
                    value = 1;
                }
            }

            //set weight if applicable
            if (args.Length > 3)
            {
                weight = DanceTools.CheckFloat(args[3]);
                //fix for invalid args
                if (weight == -1f)
                {
                    weight = -1f;
                }
            }


            //spawn multiple items
            for (int i = 0; i < amount; i++)
            {
                
                GameObject obj = UnityEngine.Object.Instantiate(StartOfRound.Instance.allItemsList.itemsList[index].spawnPrefab, spawnPos, Quaternion.identity);
                //.itemProperties.creditsWorth
                
                ScanNodeProperties scan = obj.GetComponent<ScanNodeProperties>();
                if (scan == null)
                {
                    scan = obj.AddComponent<ScanNodeProperties>(); //attach scanning node for value
                    scan.scrapValue = value;
                    scan.subText = $"Value: ${value}";
                }

                //GrabbableObject
                //idk which one actually sets the value properly, do all of them
                obj.GetComponent<GrabbableObject>().fallTime = 0f;
                obj.GetComponent<GrabbableObject>().scrapValue = value;
                if(weight != -1f)
                {
                    obj.GetComponent<GrabbableObject>().itemProperties.weight = weight;
                }
                obj.GetComponent<GrabbableObject>().itemProperties.creditsWorth = value;
                obj.GetComponent<GrabbableObject>().SetScrapValue(value); //give value to it
                //item.weight = 6969f;

                //spawn it after giving it values
                obj.GetComponent<NetworkObject>().Spawn();

            }
            DTConsole.Instance.PushTextToOutput($"Spawned {amount}x item {StartOfRound.Instance.allItemsList.itemsList[index].itemName}({index}) valued at {value} (weight: {weight} (buggy))", DanceTools.consoleSuccessColor);

        }
    }
}
