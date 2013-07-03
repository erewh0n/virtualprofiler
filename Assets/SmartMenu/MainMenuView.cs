using UnityEngine;

namespace Assets.SmartMenu
{
    public class MainMenuView : ISmartMenu
    {
        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 250, 500, 35));

            GUI.Box(new Rect(0, 0, 500, 35), "");

            if (GUI.Button(new Rect(5, 5, 115, 25), "Start New Run..."))
            {
                return new NewRunMenu();
            }
            if (GUI.Button(new Rect(125, 5, 115, 25), "Arduino Setup..."))
            {
                return new ArduinoMenu();
            }
            if (GUI.Button(new Rect(245, 5, 115, 25), "Replay..."))
            {
                return new ReplayMenu();
            }
            if (GUI.Button(new Rect(425, 5, 70, 25), "Exit"))
            {
                Application.Quit();
            }

            GUI.EndGroup();

            return null;
        }
    }
}
