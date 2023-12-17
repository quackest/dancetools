using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DanceTools.UI
{
    internal class DTWidget : MonoBehaviour
    {
        //sanity check
        private void Awake()
        {
            DanceTools.mls.LogInfo("UI Loaded");
        }
        private void OnEnable()
        {
            DanceTools.mls.LogInfo("IM HERE!!!!!");
        }
    }


    //we use this to basically control the actual ui widget
    internal class DTUIManager : MonoBehaviour
    {
        //ui things
        internal static KeyboardShortcut UIShortcut = new KeyboardShortcut(KeyCode.BackQuote); //ui key
        internal static bool isUIOpen = false;
        public GameObject holder;
        public TMP_InputField input;
        public TextMeshProUGUI output;
        private string oldOutput = "";

        private void Awake()
        {
            //get ui elements from asset bundle
            holder = transform.Find("Holder").gameObject;
            input = transform.Find("Holder/InputBackground/InputField").GetComponent<TMP_InputField>();
            output = transform.Find("Holder/OutputBackground/Scroll/Viewport/OutputField").GetComponent<TextMeshProUGUI>();
            DanceTools.mls.LogInfo($"Setup holder: {holder.name}");
            DanceTools.mls.LogInfo($"Setup input: {input.name}");
            DanceTools.mls.LogInfo($"Setup output: {output.name}");

            input.onSubmit.AddListener(text => { OnEditEnd(text); }); ; //worky :^]

            output.text = "";

            PushTextToOutput($"Hey there!\nDanceTools V{DanceTools.pluginVersion}\n", "yellow");

            holder.SetActive(false);
            //ToggleUI();
        }

        //User input
        public void OnEditEnd(string txt)
        {
            PushTextToOutput($"> {input.text}"); 

            //do stuff with input.text

            //...
            
            input.text = " ";
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
            if (UIShortcut.IsDown())
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
                //auto focus
                input.text = " ";
                input.ActivateInputField();

            }
        }
    }
}
