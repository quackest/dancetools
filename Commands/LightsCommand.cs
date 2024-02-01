using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceTools.Commands
{
    internal class LightsCommand : ICommand
    {
        public string Name => "lights";

        public string Desc => "Toggles lights inside the facility\nUsage: lights on/off";

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

            string text = "";
            try
            {
                switch(args[0])
                {
                    case "on":
                        DanceTools.currentRound.TurnOnAllLights(true);
                        text = "Indoor lights turned on";
                        break;
                    case "off":
                        DanceTools.currentRound.TurnOnAllLights(false);
                        text = "Indoor lights turned off";
                        break;
                }
            } catch (Exception)
            {
                text = "Failed to toggle lights";
            }

            DTConsole.Instance.PushTextToOutput(text, DanceTools.consoleInfoColor);

        }
    }
}
