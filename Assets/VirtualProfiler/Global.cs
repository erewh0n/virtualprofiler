using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class Global
    {
        private static Configuration _config;
        public static Configuration Config
        {
            get
            {
                return _config ??
                       (_config = Object.FindObjectOfType(typeof (Configuration)) as Configuration);
            }
        }

        private static VirtualProfileLauncher _launcher;
        public static VirtualProfileLauncher Launcher
        {
            get
            {
                return _launcher ??
                       (_launcher = new VirtualProfileLauncher());
            }
        }
    }
}
