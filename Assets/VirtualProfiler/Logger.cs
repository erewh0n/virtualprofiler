using System;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public static class Logger
    {

        public static void Debug(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void Warning(string message, Exception e)
        {
            UnityEngine.Debug.LogWarning(message);
            UnityEngine.Debug.LogException(e);
        }

        public static void Error(string message, Exception e)
        {
            UnityEngine.Debug.LogError(message);
            UnityEngine.Debug.LogException(e);
        }

    }
}
