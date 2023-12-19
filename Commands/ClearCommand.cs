using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTools.Commands
{
    internal class ClearCommand : ICommand
    {
        public string Name => "clear";

        public string Desc => "Clears the console log";

        public void DisplayCommandDesc()
        {
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
        }

        public void ExecCommand(string[] args)
        {
            DTConsole.Instance.ClearConsole();
        }
    }
}
