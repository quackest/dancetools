using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTools.Commands
{
    internal class CreditsCommands : ICommand
    {
        public string Name => "setcredits";

        public string Desc => "Set credits to a value\nUsage: setcredits amount";

        public void DisplayCommandDesc()
        {
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
        }

        public void ExecCommand(string[] args)
        {
            if (!DanceTools.CheckHost()) return;

            if (args.Length < 1)
            {
                DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
                return;
            }


            int creditsVal = 0;
            creditsVal = DanceTools.CheckInt(args[0]);
            if (creditsVal == -1) return;

            if(creditsVal < 0)
            {
                DTConsole.Instance.PushTextToOutput($"Can't set credits bellow 0", DanceTools.consoleErrorColor);
                return;
            }

            Terminal terminal = (Terminal)UnityEngine.Object.FindObjectOfType(typeof(Terminal));

            terminal.SyncGroupCreditsServerRpc(creditsVal, terminal.numberOfItemsInDropship);

            DTConsole.Instance.PushTextToOutput($"Set group credits to {creditsVal}", DanceTools.consoleSuccessColor);
        }
    }
}
