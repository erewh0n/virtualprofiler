using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
    public class MainMenuView : ISmartMenu
    {
        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 400));

            GUI.Box(new Rect(0, 0, 400, 400), "");

            if (GUI.Button(new Rect(5, 5, 80, 25), "Start New Run..."))
            {
                Global.Launcher.Start();
            }
            if (GUI.Button(new Rect(90, 5, 80, 25), "Arduino Setup..."))
            {
                
            }
            if (GUI.Button(new Rect(175, 5, 80, 25), "Exit"))
            {
                Application.Quit();
            }

            GUI.EndGroup();

            return null;
        }
    }
}
