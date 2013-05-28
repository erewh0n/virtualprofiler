using System;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public static class Logger
    {

        public static void Debug(string message)
        {
            if (Global.Config.DebugLoggingEnabled)
                UnityEngine.Debug.Log(message);
        }

        public static void Warning(string message, Exception e)
        {
            UnityEngine.Debug.LogWarning(string.Format("{0}: {1}", message, e.Message));
        }

        public static void Error(string message, Exception e)
        {
            UnityEngine.Debug.LogError(message);
            UnityEngine.Debug.LogException(e);
        }

    }
}
