using UnityEngine;

namespace Assets.SmartMenu
{
    public class MainMenuView : ISmartMenu
    {
        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(5, 5, 135, 140));

            GUI.Box(new Rect(0, 0, 110, 125), "");

            GUI.skin.button.fontSize = 10;
            GUI.skin.label.fontSize = 10;
            GUI.skin.textArea.fontSize = 11;
            GUI.skin.textField.fontSize = 11;
            if (GUI.Button(new Rect(5, 5, 100, 25), "Start New Run..."))
            {
                return new NewRunMenu();
            }
            if (GUI.Button(new Rect(5, 35, 100, 25), "Arduino Setup..."))
            {
                return new ArduinoMenu();
            }
            if (GUI.Button(new Rect(5, 65, 100, 25), "Replay..."))
            {
                return new ReplayMenu();
            }
            if (GUI.Button(new Rect(5, 95, 100, 25), "Exit"))
            {
                Application.Quit();
            }

            GUI.EndGroup();

            return null;
        }
    }
}
