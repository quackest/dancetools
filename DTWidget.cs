﻿using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DanceTools
{
    //logic of the console
    public class DTCmdHandler : MonoBehaviour
    {
        public static DTCmdHandler Instance;

        //sanity check
        private void Awake()
        {
            Instance = this;
            DanceTools.mls.LogInfo("Loading Commands..");

            var interfaceType = typeof(ICommand);
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var cmdTypes = types.Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass);

            foreach (var cmd in cmdTypes)
            {
                var inst = (ICommand)Activator.CreateInstance(cmd);
                DanceTools.commands.Add(inst);
                DanceTools.mls.LogInfo($"Loaded {inst.Name} command!");
            }
            DanceTools.mls.LogInfo("Commands Loaded!");
        }

        //check if we have the command
        public void CheckCommand(string input)
        {
            //it's a console, we dont need to check for prefixes
            string[] args = input.Split(' ');

            //if nothing, ignore it
            if (args.Length == 0) return;

            bool cmdFound = false;

            for (int i = 0; i < DanceTools.commands.Count; i++) 
            {
                if (DanceTools.commands[i].Name.ToLower() == args[0].ToLower())
                {
                    cmdFound = true;
                    args = args.Skip(1).ToArray(); //get rid of command part
                    TriggerCommand(DanceTools.commands[i], args);
                    break;
                }
            }
            if(!cmdFound)
            {
                DTConsole.Instance.PushTextToOutput($"Invalid Command", DanceTools.consoleErrorColor);
            }
            //if (commands.Contains(msg[0].ToLower()))
        }
        public void TriggerCommand(ICommand cmd, string[] args)
        {
            cmd.ExecCommand(args);
        }

    }

    //handles inputs, outputs and keybind to the console
    public class DTConsole : MonoBehaviour
    {
        //ui things
        internal static bool isUIOpen = true; //
        public GameObject holder;
        public TMP_InputField input;
        public TextMeshProUGUI output;
        private string oldOutput = "";
        public static DTConsole Instance;
        internal static string[] sillyMessages = 
            { 
            "Hey there!",
            "Console colors are customizable in the config o.o",
            "Haiii! >.<",
            "Dancing on the moon or something.."};

        private void Awake()
        {
            Instance = this;
            //get ui elements from asset bundle
            holder = transform.Find("Holder").gameObject;
            input = transform.Find("Holder/InputBackground/InputField").GetComponent<TMP_InputField>();
            output = transform.Find("Holder/OutputBackground/Scroll/Viewport/OutputField").GetComponent<TextMeshProUGUI>();
            DanceTools.mls.LogInfo($"Setup holder: {holder.name}");
            DanceTools.mls.LogInfo($"Setup input: {input.name}");
            DanceTools.mls.LogInfo($"Setup output: {output.name}");

            input.onSubmit.AddListener(text => { OnEditEnd(text); }); ; //worky :^]
            //set default starting command to help
            input.text = "help";
            //clear console
            output.text = "";
            //intro message
            PushTextToOutput($"\n{sillyMessages[UnityEngine.Random.Range(0, sillyMessages.Length)]}\nDanceTools v{DanceTools.pluginVersion}\n", "#FF00FF");

            //hide the console on startup
            holder.SetActive(false); //uncomment
        }

        //User input
        public void OnEditEnd(string txt)
        {
            PushTextToOutput($"> {input.text}", DanceTools.consolePlayerColor); 
            //do stuff with input.text
            DTCmdHandler.Instance.CheckCommand(input.text);
            //...
            input.text = "";
            input.ActivateInputField();
        }
        //Every response sent back
        public void PushTextToOutput(string text, string color = "#00FFF3")
        {
            output.text = $"<color={color}>{text}</color>\n{oldOutput}";
            oldOutput = output.text;
        }

        //ui key
        public void Update()
        {
            if (!DanceTools.isHost) return; //ignore if not host
            if (DanceTools.keyboardShortcut.IsDown())
            {
                ToggleUI();
            }
        }

        //toggle ui;
        public void ToggleUI()
        {
            isUIOpen = !isUIOpen;
            
            if (isUIOpen)
            {
                holder.gameObject.SetActive(false);
                GameNetworkManager.Instance.localPlayerController.quickMenuManager.isMenuOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                holder.gameObject.SetActive(true);
                GameNetworkManager.Instance.localPlayerController.quickMenuManager.isMenuOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                //auto focus and reset text to nothing
                input.text = "";
                input.ActivateInputField();

            }
        }
        public void ClearConsole()
        {
            output.text = "";
            oldOutput = "";
        }
    }
}
