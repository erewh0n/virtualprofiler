using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class Global
    {
        private static Configuration _configInstance;
        public static Configuration Config { get { return _configInstance ?? (_configInstance = Object.FindObjectOfType(typeof(Configuration)) as Configuration); } }
    }
}
