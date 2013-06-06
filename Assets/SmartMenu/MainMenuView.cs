using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{

    public class RunningMenu : ISmartMenu
    {
        public ISmartMenu CreateMenu()
        {
            GUILayout.BeginVertical();

            if (GUI.Button(new Rect(5, 5, 80, 25), "Stop"))
            {
                Global.Launcher.Stop();
                return new MainMenuView();
            }

            GUILayout.EndVertical();

            return this;
        }
    }

    public class ArduinoMenu : ISmartMenu
    {
        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 160, 400, 400));

            GUI.Box(new Rect(0, 0, 400, 400), "");

            if (GUI.Button(new Rect(145, 370, 110, 25), "OK"))
            {
                return new MainMenuView();
            }

            GUI.Label(new Rect(5, 50, 130, 25), "Movement COM Port: ");
            Global.Config.SerialPortMovementInput = GUI.TextField(new Rect(150, 50, 130, 25), Global.Config.SerialPortMovementInput, 4);
            GUI.Label(new Rect(5, 80, 130, 25), "Movement COM Baud: ");
            Global.Config.SerialPortBaud = int.Parse(GUI.TextField(new Rect(150, 80, 130, 25), Global.Config.SerialPortBaud.ToString(), 6));
            GUI.Label(new Rect(5, 110, 130, 25), "Reporting Folder: ");
            Global.Config.MovementLogDirectory = GUI.TextField(new Rect(150, 110, 130, 25), Global.Config.MovementLogDirectory, 500);
            GUI.Label(new Rect(5, 140, 130, 25), "X Scaling: ");
            Global.Config.ScaleX = int.Parse(GUI.TextField(new Rect(150, 140, 130, 25), Global.Config.ScaleX.ToString(), 6));
            GUI.Label(new Rect(5, 170, 130, 25), "Y Scaling: ");
            Global.Config.ScaleY = int.Parse(GUI.TextField(new Rect(150, 170, 130, 25), Global.Config.ScaleY.ToString(), 6));
            GUI.Label(new Rect(5, 200, 130, 25), "Z Scaling: ");
            Global.Config.ScaleZ = int.Parse(GUI.TextField(new Rect(150, 200, 130, 25), Global.Config.ScaleZ.ToString(), 6));
            GUI.Label(new Rect(5, 230, 130, 25), "Smoothing: ");
            Global.Config.Smoothing = int.Parse(GUI.TextField(new Rect(150, 230, 130, 25), Global.Config.Smoothing.ToString(), 6));

            GUI.EndGroup();

            return this;
        }
    }

    public class MainMenuView : ISmartMenu
    {
        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 35));

            GUI.Box(new Rect(0, 0, 400, 35), "");

            if (GUI.Button(new Rect(5, 5, 110, 25), "Start New Run..."))
            {
                Global.Launcher.Start();
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
