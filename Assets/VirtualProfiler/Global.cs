using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class Global
    {
        private static Configuration _instance;
        public static Configuration Instance { get { return _instance ?? (_instance = GameObject.FindObjectOfType(typeof(Configuration)) as Configuration); } }
    }
}
