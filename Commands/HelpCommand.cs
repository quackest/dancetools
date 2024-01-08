using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTools.Commands
{
    internal class HelpCommand : ICommand
    {
        //Command that shows list of commands and also description of commands if used as "help cmd"
        public string Name => "help";
        public string Desc => "Shows list of commands and what each command does";

        public void DisplayCommandDesc()
        {
            string output = "";

            for (int i = 0; i < DanceTools.commands.Count; i++)
            {
                output += $"\n{DanceTools.commands[i].Name}";
            }

            DTConsole.Instance.PushTextToOutput($"List Of Commands: \n{output}", DanceTools.consoleSuccessColor);
        }

        public void ExecCommand(string[] args)
        {
            if (args.Length < 1)
            {
                DisplayCommandDesc();
                return;
            }
            bool cmdFound = false;


            for (int i = 0; i < DanceTools.commands.Count; i++)
            {
                if (DanceTools.commands[i].Name.ToLower() == args[0].ToLower())
                {
                    cmdFound = true;
                    DTConsole.Instance.PushTextToOutput(DanceTools.commands[i].Desc, DanceTools.consoleSuccessColor);
                    break;
                }
            }

            if(!cmdFound)
            {
                DTConsole.Instance.PushTextToOutput($"Command not found", DanceTools.consoleErrorColor);
            }
        }
    }
}
