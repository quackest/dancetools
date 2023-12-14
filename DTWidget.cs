using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DanceTools.UI
{
    internal class DTWidget : MonoBehaviour
    {

        private bool isUIOpen = false;

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
        internal static KeyboardShortcut UIShortcut = new KeyboardShortcut(KeyCode.RightBracket); //ui key
        internal static bool isUIOpen = false;

        //ui things
        public void Update()
        {
            if (!DanceTools.isHost) return; //ignore if not host
            if (UIShortcut.IsDown())
            {
                if (!isUIOpen)
                {
                    //open UI
                    DanceTools.mls.LogInfo("UI Open");
                    isUIOpen = true;
                    ToggleUI();
                }
                else
                {
                    //close UI
                    DanceTools.mls.LogInfo("UI Closed");
                    isUIOpen = false;
                    ToggleUI();
                }
            }
        }

        public void ToggleUI()
        {
            //toggle ui;
            if (isUIOpen)
            {
                DanceTools.DTUI.gameObject.SetActive(false);
            }
            else
            {
                DanceTools.DTUI.gameObject.SetActive(true);
            }
        }
    }
}
