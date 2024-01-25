using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace DanceTools.Commands
{
    internal class EnemySpawn : ICommand
    {
        public string Name => "enemy";

        public string Desc => "Spawns enemies\nUsage: enemy name amount (onme)\nType just the command without arguments \nto see list of enemies";

        public void DisplayCommandDesc()
        {
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
        }

        public void ExecCommand(string[] args)
        {

            //check if host
            if (!DanceTools.CheckHost()) return;

            //create a list of spawnable enemies
            //DanceTools.

            if (args.Length < 1)
            {
                string consoleInfo = "\nSpawnable Enemies (Name | Inside/Outside)\n<color=red>Warning. Inside enemies break game when spawned outside.\nSpawn at your own risk.</color>";

                if (DanceTools.currentRound.currentLevel.Enemies.Count <= 0)
                {
                    DTConsole.Instance.PushTextToOutput($"No enemies to spawn in this level", DanceTools.consoleErrorColor);
                    return;
                }

                for (int i = 0; i < DanceTools.spawnableEnemies.Count; i++)
                {
                    consoleInfo += $"\n{DanceTools.spawnableEnemies[i].name} | {(DanceTools.spawnableEnemies[i].isOutside ? "outside" : "inside")}";
                }
                //DanceTools.currentRound.currentLevel.OutsideEnemies
                //DanceTools.currentRound.outsideAINodes <- spawnplace
                DTConsole.Instance.PushTextToOutput($"{consoleInfo}", DanceTools.consoleSuccessColor);
                DTConsole.Instance.PushTextToOutput("Command usage: enemy name amount (onme)", DanceTools.consoleInfoColor);
                return;
            }
            try
            {
                string enemyName = args[0].ToLower();
                DanceTools.SpawnableEnemy enemyToSpawn;
                int amount = 1;
                string message = "";
                string outsideInsideText = "";
                bool onMeSpawn = false;

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

                if (!DanceTools.spawnableEnemies.Any((x) => x.name.ToLower().Contains(enemyName)))
                {
                    //enemy doesn't exist in list
                    DTConsole.Instance.PushTextToOutput($"Enemy {enemyName} doesn't exist in current list.\nSometimes you need to load a certain map to load an enemy reference.", DanceTools.consoleErrorColor);
                    return;
                }
                //get enemy to spawn
                enemyToSpawn = DanceTools.spawnableEnemies.Find((x) => x.name.ToLower().Contains(enemyName));

                DTConsole.Instance.PushTextToOutput($"{enemyToSpawn.name} <- you chose this debug asdas", DanceTools.consoleErrorColor);

                //check if inside or outside text
                outsideInsideText = enemyToSpawn.isOutside ? "outside" : "inside a random vent";

                
                //spawn it in a random vent
                if (args.Length > 2)
                {
                    if (args[2] == "onme")
                    {
                        //spawnPos = GameNetworkManager.Instance.localPlayerController.transform.position;
                        //message = $"Spawned {amount}x {DanceTools.currentRound.currentLevel.Enemies[index].enemyType.enemyName} on top of you";
                        onMeSpawn = true;
                        outsideInsideText = "on top of you..";
                    }
                }
                int randomIndex = 0;
                for (int i = 0; i < amount; i++)
                {
                    //random vent for each enemy unless specified otherwise
                    //im so fucking unique bro.
                    if(onMeSpawn)
                    {
                        Vector3 spawnPos = GameNetworkManager.Instance.localPlayerController.transform.position;

                        //special case for when player is dead and spectating someone else
                        if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
                        {
                            spawnPos = GameNetworkManager.Instance.localPlayerController.spectatedPlayerScript.transform.position;
                        }
                        SpawnEnemy(enemyToSpawn, spawnPos, i);
                    } else
                    {
                        randomIndex = UnityEngine.Random.Range(0, DanceTools.currentRound.allEnemyVents.Length);
                        if(enemyToSpawn.isOutside)
                        {
                            //outside enemy
                            SpawnEnemy(enemyToSpawn, DanceTools.currentRound.outsideAINodes[randomIndex].transform.position, i);
                        } else
                        {
                            //inside enemy
                            SpawnEnemy(enemyToSpawn, DanceTools.currentRound.allEnemyVents[randomIndex].floorNode.position, i);
                        }
                        
                    }
                }
                //spawn message
                message = $"Spawned {amount}x {enemyToSpawn.name} {outsideInsideText}";
                DTConsole.Instance.PushTextToOutput(message, DanceTools.consoleSuccessColor);
            }
            catch (Exception e)
            {
                DTConsole.Instance.PushTextToOutput("Can't spawn enemies when not landed.", DanceTools.consoleErrorColor);
                DanceTools.mls.LogError($"error: {e.Message}");
            }
        }

        //needs testing to see if it actually spawns the enemies
        private void SpawnEnemy(DanceTools.SpawnableEnemy enemy, Vector3 spawnPos, int count = 0)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(enemy.prefab, spawnPos, Quaternion.identity);
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(destroyWithScene: true);

            //sanity checker
            if (DanceTools.consoleDebug)
            {
                DTConsole.Instance.PushTextToOutput($"{enemy.name}{count} pos: {spawnPos}", "white");
            }
        }
    }
}
