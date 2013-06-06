
namespace Assets.SmartMenu
{

    public class SmartMenuController
    {
        private readonly ISmartMenu _defaultMenu;
        private ISmartMenu _nextMenu;

        public SmartMenuController(ISmartMenu defaultMenu)
        {
            _defaultMenu = defaultMenu;
            _nextMenu = defaultMenu;
        }

        public void OnGUI()
        {
            _nextMenu = _nextMenu == null ? _defaultMenu.CreateMenu() : _nextMenu.CreateMenu();
        }
    }

}
