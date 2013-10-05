using System.Collections.Generic;
using Assets.GridLayout;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
    public class MainMenuView : ISmartMenu
    {
        private Grid _grid;
        private ISmartMenu _nextMenu;

        public ISmartMenu CreateMenu()
        {
            _nextMenu = this;
            GUI.skin.button.fontSize = 10;
            GUI.skin.label.fontSize = 10;
            GUI.skin.textArea.fontSize = 11;
            GUI.skin.textField.fontSize = 11;

            if (_grid == null)
            {
                _grid = new Grid(5, 5, 135, 140);
                _grid.Add(new[]
                {
                    new GridColumn(new List<IGridRenderable>
                        {
                            new GridButton("Start New Run...", () => _nextMenu = new NewRunMenu()),
                            new GridButton("Configuration...", () => _nextMenu = new ConfigurationMenu()),
                            new GridButton("Replay...",        () => _nextMenu = new ReplayMenu()),
                            new GridButton("Exit", Application.Quit),
                        }, new Padding(5, 5)),
                });
            }
            _grid.Render();
            //GUI.BeginGroup(new Rect(5, 5, 135, 140));

            //GUI.Box(new Rect(0, 0, 110, 125), "");

            //GUI.skin.button.fontSize = 10;
            //GUI.skin.label.fontSize = 10;
            //GUI.skin.textArea.fontSize = 11;
            //GUI.skin.textField.fontSize = 11;
            //if (GUI.Button(new Rect(5, 5, 100, 25), "Start New Run..."))
            //{
            //    return new NewRunMenu();
            //}
            //if (GUI.Button(new Rect(5, 35, 100, 25), "Configuration..."))
            //{
            //    return new ConfigurationMenu();
            //}
            //if (GUI.Button(new Rect(5, 65, 100, 25), "Replay..."))
            //{
            //    return new ReplayMenu();
            //}
            //if (GUI.Button(new Rect(5, 95, 100, 25), "Exit"))
            //{
            //    Application.Quit();
            //}

            //GUI.EndGroup();

            return _nextMenu;
        }
    }
}
