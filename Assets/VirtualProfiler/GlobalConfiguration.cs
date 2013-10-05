using System;

namespace Assets.VirtualProfiler
{

    [Serializable]
    public class GlobalConfiguration
    {
        public string MovementLogDirectory = "C:\\virtualprofiler";

        public float ScaleX = 100.0f;
        public float ScaleZ = 100.0f;
        public float ScaleY = 100.0f;
        public float Smoothing = 0.5f;

        public string SerialPortMovementInput = "COM5";
        public int SerialPortBaud = 9600;

        public bool DebugLoggingEnabled = true;
        public bool EnableSubjectLogging = true;

        public string RuntimeCameraTag = "RuntimeCamera";
        public string ReplayCameraTag  = "ReplayCamera";
        public string SurfaceLayerTag  = "SurfaceLayer";
        public string LineRendererTag  = "LineRenderer";
        public string ParticleRendererTag = "ParticleRenderer";
    }
}
