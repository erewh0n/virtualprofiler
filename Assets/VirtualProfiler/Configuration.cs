using UnityEngine;

namespace Assets.VirtualProfiler
{

    public class Configuration : MonoBehaviour
    {
        public string ReplayFile = "ReplayStream.log";
        public string MovementLogDirectory = "C:\\virtualprofiler";

        public float ScaleX = 100.0f;
        public float ScaleZ = 100.0f;
        public float ScaleY = 100.0f;
        public float Smoothing = 0.5f;

        public float DistanceAboveSubject = 10;

        public string SerialPortMovementInput = "COM5";
        public int SerialPortBaud = 9600;

        public bool DebugLoggingEnabled = true;
    }
}
