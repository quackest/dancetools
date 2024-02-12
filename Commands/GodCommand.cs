using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTools.Commands
{
    internal class GodCommand : ICommand
    {
        public string Name => "god";

        public string Desc => "Toggles godmode for the host";

        public void DisplayCommandDesc()
        {
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
        }

        public void ExecCommand(string[] args)
        {
            
            if (!DanceTools.CheckHost()) return;

            //flip flop
            DanceTools.playerGodMode = !DanceTools.playerGodMode;

            string text = DanceTools.playerGodMode ? "God mode enabled" : "God mode disabled";

            DTConsole.Instance.PushTextToOutput(text, DanceTools.consoleInfoColor);
        }
    }
}
