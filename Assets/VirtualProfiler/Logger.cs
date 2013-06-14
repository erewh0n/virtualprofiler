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
            var loggedMessage = e == null ? message : string.Format("{0}: {1}", message, e.Message);

            UnityEngine.Debug.LogWarning(loggedMessage);
        }

        public static void Error(string message, Exception e)
        {
            Warning(message, e);
        }

    }
}
