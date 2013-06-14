using UnityEngine;

namespace Assets.SmartMenu
{
    public class ConfirmationDialogMenu : ISmartMenu
    {
        private readonly string _fieldName;
        private readonly string _fieldDescription;
        private readonly ISmartMenu _lastMenu;

        public ConfirmationDialogMenu(string fieldName, string fieldDescription, ISmartMenu lastMenu)
        {
            _fieldName = fieldName;
            _fieldDescription = fieldDescription;
            _lastMenu = lastMenu;
        }

        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100));

            GUI.Box(new Rect(0, 0, 200, 100), "");

            GUI.Label(new Rect(5, 5, 190, 60),
                      new GUIContent(string.Format("{0}: {1}", _fieldName, _fieldDescription)), new GUIStyle
                          {
                              fontSize = 11,
                              wordWrap = true,
                          });
            if (GUI.Button(new Rect(50, 70, 100, 25), "OK"))
            {
                return _lastMenu;
            }

            GUI.EndGroup();

            return this;
        }
    }
}