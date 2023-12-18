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

        public string Desc => "Description of the command!";

        public void DisplayCommandDesc()
        {
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.consoleInfoColor);
        }

        public void ExecCommand(string[] args)
        {
            if (args.Length < 1)
            { 
                DisplayCommandDesc();
                return;
            }
            DTConsole.Instance.PushTextToOutput("Executed tester cmd", DanceTools.consoleSuccessColor);
        }
    }
}
