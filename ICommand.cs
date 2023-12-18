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
    }
}
