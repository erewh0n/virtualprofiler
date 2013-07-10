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
                Global.Launcher.StopAndFinalizeRun();
                return new MainMenuView();
            }

            GUILayout.EndVertical();

            return this;
        }
    }
}