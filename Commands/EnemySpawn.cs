using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DanceTools.Commands
{
    internal class EnemySpawn : ICommand
    {
        public string Name => "enemy";

        public string Desc => "Spawns enemies\nUsage: enemy enemyID amount\nType just the command without arguments \nto see list of enemies";

        public void DisplayCommandDesc()
        {
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
        }

        public void ExecCommand(string[] args)
        {

            //check if host
            if (!DanceTools.isHost)
            {
                DTConsole.Instance.PushTextToOutput($"You must be host to use this command", DanceTools.consoleErrorColor);
                return;
            }

            if (args.Length < 1)
            {
                string consoleInfo = "\nSpawnable Enemies (ID | Name)";

                if (DanceTools.currentRound.currentLevel.Enemies.Count <= 0)
                {
                    DTConsole.Instance.PushTextToOutput($"No enemies to spawn in this level", DanceTools.consoleErrorColor);
                    return;
                }

                for (int i = 0; i < DanceTools.currentRound.currentLevel.Enemies.Count; i++)
                {
                    consoleInfo += $"\n{i} | {DanceTools.currentRound.currentLevel.Enemies[i].enemyType.enemyName}";
                }
                DTConsole.Instance.PushTextToOutput($"{consoleInfo}", DanceTools.consoleSuccessColor);
                DTConsole.Instance.PushTextToOutput("Command usage: enemy id (onme)", DanceTools.consoleInfoColor);
                return;
            }
            try
            {

                int index = 0;
                int amount = 1;
                //fix for invalid args
                index = DanceTools.CheckInt(args[0]);
                if (index == -1) return;

                if (args.Length > 1)
                {
                    amount = DanceTools.CheckInt(args[1]);
                    if (amount == -1) return;

                    if (amount <= 0)
                    {
                        DTConsole.Instance.PushTextToOutput($"Amount cannot be 0 or less than 0", DanceTools.consoleErrorColor);
                        return;
                    }
                }

                //default = in vents
                Vector3 spawnPos = DanceTools.currentRound.allEnemyVents[UnityEngine.Random.Range(0, DanceTools.currentRound.allEnemyVents.Length)].floorNode.position;
                string message = $"Spawned {amount}x {DanceTools.currentRound.currentLevel.Enemies[index].enemyType.enemyName} in a random vent inside";

                //spawn it in a random vent
                if (args.Length > 2)
                {
                    if (args[2] == "onme")
                    {
                        spawnPos = GameNetworkManager.Instance.localPlayerController.transform.position;
                        message = $"Spawned {amount}x {DanceTools.currentRound.currentLevel.Enemies[index].enemyType.enemyName} on top of you";
                    }
                }
                int randomIndex= 0;
                for (int i = 0; i < amount; i++)
                {
                    //random vent for each enemy
                    //im so fucking unique bro.
                    randomIndex = UnityEngine.Random.Range(0, DanceTools.currentRound.allEnemyVents.Length);
                    DanceTools.currentRound.SpawnEnemyOnServer(DanceTools.currentRound.allEnemyVents[randomIndex].floorNode.position, 0f, index);

                    if(DanceTools.consoleDebug)
                    {
                        DTConsole.Instance.PushTextToOutput($"Enemy{i} pos: {DanceTools.currentRound.allEnemyVents[randomIndex].floorNode.position}", "white");
                    }
                }
                

                DTConsole.Instance.PushTextToOutput(message, DanceTools.consoleSuccessColor);
            }
            catch (Exception e)
            {
                DTConsole.Instance.PushTextToOutput("Can't spawn enemies when not landed.", DanceTools.consoleErrorColor);
                DanceTools.mls.LogError($"error: {e.Message}");
            }
        }
    }
}
