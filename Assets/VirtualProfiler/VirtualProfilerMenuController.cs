using Assets.SmartMenu;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class VirtualProfilerMenuController : MonoBehaviour
    {
        private readonly SmartMenuController _menuController = new SmartMenuController(new MainMenuView());

        public void OnGUI()
        {
            _menuController.OnGUI();
        }

    }
}
