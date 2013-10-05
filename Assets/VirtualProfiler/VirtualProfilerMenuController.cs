using System;
using Assets.SmartMenu;
using UnityEngine;

namespace Assets.VirtualProfiler
{

    public class VirtualProfilerMenuController : MonoBehaviour
    {
        public void Start()
        {
            Global.Launcher.LoadGlobalConfiguration();
            Global.Launcher.Initialize();
        }

        private readonly SmartMenuController _menuController = new SmartMenuController(new MainMenuView());

        public void OnGUI()
        {
            try
            {
                _menuController.OnGUI();
            }
            catch (Exception e)
            {
                Logger.Error("An exception occurred while navigating the menu.", e);
            }
        }

    }
}
