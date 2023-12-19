using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTools
{
    internal interface ICommand
    {
        string Name { get; }
        string Desc { get; }
        void ExecCommand(string[] args);
        void DisplayCommandDesc();

        //todo: add a check for if it's a host command
        //todo: add a check if command requires you to be in a server
    }
}
