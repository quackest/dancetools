using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTools.Commands
{
    internal class CloseConsole : ICommand
    {
        public string Name => "close";

        public string Desc => "Closes the console in case of bug/can't close it";

        public void DisplayCommandDesc()
        {
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
        }

        public void ExecCommand(string[] args)
        {
            DTConsole.Instance.ToggleUI();
        }
    }
}
