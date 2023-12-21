using DanceTools;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using Unity;
using Unity.Netcode;
using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using BepInEx.Configuration;
using System.IO;
using System.Reflection;
using BepInEx;
using System;

namespace TestingThings
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class Class1 : BaseUnityPlugin
    {
        //plugin info
        public const string pluginGUID = "dancemoon.Testing";
        public const string pluginName = "DanceTesting";
        public const string pluginVersion = "1.0.0";
        private readonly Harmony harmony = new Harmony(pluginGUID);//harmony

        //create an object of your custom command
        public TestExternalCommand ExternalCmdClass = new TestExternalCommand();

        public void Awake()
        {
            //your plugin init
            Logger.LogInfo("DanceTesting loaded !!!!!!!!!!!!!!!!!!!!!!!!");
            harmony.PatchAll(typeof(Class1));

            //add your command to the DanceTools commands list to be usable
            DanceTools.DanceTools.AddToCommandsList(ExternalCmdClass);
        }
    }
    public class TestExternalCommand : ICommand
    {
        //the command name that will be used to call it
        public string Name => "testcommand";

        //small description of what the command does
        public string Desc => "external command"; 

        public void DisplayCommandDesc()
        {
            //help description usage
            //example: help testcommand <- will run DisplayCommandDesc()
            //recommended to do as the example desc and only print text to the console
            DTConsole.Instance.PushTextToOutput(Desc, DanceTools.DanceTools.consoleInfoColor);
        }

        //ExecCommand is called when the command is used in the console.
        //args is a list of paramaters that comes after the actual command.
        //Example: testcommand arg0 arg1 arg2 ...
        //"testcommand" portion of it gets stripped
        public void ExecCommand(string[] args)
        {

            //Example code snippets you can use to help you:

            //line of code you can use to check if the player is host or not.
            //it will return true if host. If false, it will also send a console command
            //telling the user they are not the host.
            if (!DanceTools.DanceTools.CheckHost()) return;

            //no args
            if(args.Length < 1)
            {
                //command was written without arguments
                DTConsole.Instance.PushTextToOutput("Haii!! No arguments were present in the command", "white");
                return;
            }

            //this will check if an arg is a proper int or not.
            //if it's not, then it will give a "invalid argument" resposne on console
            int someInt = 0;
            someInt = DanceTools.DanceTools.CheckInt(args[0]);
            if (someInt == -1) return;
            //someInt is now valid int

            //clears the console log
            DTConsole.Instance.ClearConsole();

            //This is how you can send stuff in the console
            //for color, you can use a hex code for color. 
            //rich text is supported, so if you want certain parts to be different color you can do so
            //Example string: <color=red>Hello</color> <color=yellow">World!</color> <- will print the text in said colors
            //leaving color blank, will use default player color specified in DanceTools config
            DTConsole.Instance.PushTextToOutput("Hello there! I'm in a different plugin", "white");



            //do what you want here. 
            //...
        }
    }
}