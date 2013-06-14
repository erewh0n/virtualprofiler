using System;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
    public class MainMenuView : ISmartMenu
    {
        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 35));

            GUI.Box(new Rect(0, 0, 400, 35), "");

            if (GUI.Button(new Rect(5, 5, 110, 25), "Start New Run..."))
            {
                try
                {
                    Global.Launcher.Start();
                }
                catch (Exception e)
                {
                    try
                    {
                        Global.Launcher.Stop();
                    }
                    catch
                    {
                    }
                    return new ConfirmationDialogMenu("Profiler", string.Format("There was a problem while starting the profiler: {0}", e.Message), this);
                }

                return new RunningMenu();
            }
            if (GUI.Button(new Rect(120, 5, 110, 25), "Arduino Setup..."))
            {
                return new ArduinoMenu();
            }
            if (GUI.Button(new Rect(325, 5, 70, 25), "Exit"))
            {
                Application.Quit();
            }

            GUI.EndGroup();

            return null;
        }
    }
}
