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
            set { _config = value; }
        }

        private static VirtualObjects _objects;
        public static VirtualObjects Objects
        {
            get
            {
                return _objects ??
                       (_objects = new VirtualObjects());
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

        private static ReplayManager _replayManager;
        public static ReplayManager ReplayManager
        {
            get
            {
                return _replayManager ??
                       (_replayManager = new ReplayManager());
            }
        }
    }
}
