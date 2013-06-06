using System;
using System.IO;
using Object = UnityEngine.Object;

namespace Assets.VirtualProfiler
{
    public class VirtualProfileLauncher
    {
        private SubjectController _controller;

        public void Start()
        {
            Stop();
            _controller = Object.FindObjectOfType(typeof (SubjectController)) as SubjectController;

            InitializeProfiling();
        }

        public void Stop()
        {
            if (_controller != null)
                _controller.DetachDriver();
        }

        private void InitializeProfiling()
        {
            var path = "";
            try
            {
                if (!Directory.Exists(Global.Config.MovementLogDirectory))
                    Directory.CreateDirectory(Global.Config.MovementLogDirectory);
                path = Path.Combine(Global.Config.MovementLogDirectory,
                                    string.Format("MovementStream.{0}.log",
                                                  DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")));
            }
            catch (Exception e)
            {
                Logger.Error("Failed while initializing the event logger.", e);
                throw;
            }
            try
            {
                _controller.AttachDriver(new UnityMovementDriver(new SerialPortAdapter(), new EventStreamWriter(path)));
            }
            catch (Exception e)
            {
                Logger.Error("Failed while initializing the movement driver.", e);
                throw;
            }
        }
    }
}