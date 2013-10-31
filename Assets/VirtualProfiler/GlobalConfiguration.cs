using System;

namespace Assets.VirtualProfiler
{

    [Serializable]
    public class GlobalConfiguration
    {
        public string MovementLogDirectory = "C:\\virtualprofiler";

        public float ScaleX = -.028f;
        public float ScaleZ = -.102f;
        public float ScaleY = -1f;
        public float Smoothing = 0f;

        public string SerialPortMovementInput = "COM5";
        public int SerialPortBaud = 9600;

        public bool DebugLoggingEnabled = true;
        public bool EnableSubjectLogging = true;

        public string RuntimeCameraTag = "MainCamera";
        public string ReplayCameraTag  = "LineRenderCam";
        public string SurfaceLayerTag  = "SurfaceLayer";
        public string LineRendererTag  = "LineRenderer";
        public string ParticleRendererTag = "ParticleRenderer";

        public int   MinMotionFilter     = 2;
        public float HistogramSampleRate = 1f;
        public float HistogramMinY       = 0f;
        public float HistogramMaxY       = 5f;
        public int   HistogramGranularity = 128;
        public int   HistogramHeight     = 128;
        public int   HistogramWidth      = 256;

        public bool RenderHistogram = true;
    }
}
