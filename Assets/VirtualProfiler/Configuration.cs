using UnityEngine;

namespace Assets.VirtualProfiler
{

    public class Configuration : MonoBehaviour
    {
        public string ReplayFile = "ReplayStream.log";
        public string MovementLogFile = "MovementStream.log";
        public float UniversalScaling = 100.0f;

        public float DistanceAboveSubject = 10;

        public string SerialPortMovementInput = "COM5";
        public int SerialPortBaud = 9600;
    }
}
