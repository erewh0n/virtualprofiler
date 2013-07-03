using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class Global
    {
        private static GlobalConfiguration _config;
        public static GlobalConfiguration Config
        {
            get
            {
                return _config ??
                       (_config = new GlobalConfiguration());
            }
        }

        private static VirtualProfiler _launcher;
        public static VirtualProfiler Launcher
        {
            get
            {
                return _launcher ??
                       (_launcher = new VirtualProfiler());
            }
        }
    }
}
