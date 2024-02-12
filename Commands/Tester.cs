using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTools.Commands
{
    internal class Tester : ICommand
    {
        public string Name => "tester";

        public string Desc => "command for testing stuff. disabled in current build";

        public void DisplayCommandDesc()
        {
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
        }

        public void ExecCommand(string[] args)
        {

            DisplayCommandDesc();
            return; //for testing purposes
            /*if (args.Length < 1)
            { 
                DisplayCommandDesc();
                return;
            }
            DTConsole.Instance.PushTextToOutput("Executed tester cmd", DanceTools.consoleSuccessColor);
            string temp = "";
            PlayerControllerB[] playerList = DanceTools.currentRound.playersManager.allPlayerScripts;
            //DanceTools.currentRound.playersManager.PlayerHasRevivedServerRpc();
            //DanceTools.currentRound.currentLevel.
            for (int i = 0; i < playerList.Length; i++)
            {
                temp += playerList[i].playerUsername;
            }

            DTConsole.Instance.PushTextToOutput(temp, DanceTools.consoleInfoColor);
            DanceTools.currentRound.currentLevel.overrideWeatherType = LevelWeatherType.Eclipsed;
             
             */
        }
    }
}
