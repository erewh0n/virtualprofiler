using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Object = UnityEngine.Object;

namespace Assets.VirtualProfiler
{
    public class VirtualProfiler
    {
        private readonly SubjectController _controller;
        private IStreamAdapter _movementStreamAdapter;

        public VirtualProfiler()
        {
            _controller = Object.FindObjectOfType(typeof(SubjectController)) as SubjectController;
            if (_controller == null)
                throw new ArgumentException("Could not find the subject controller.  Please attach the 'SubjectController' script to a Unity object.");
        }

        public void StartReplay(string replayFile)
        {
            Stop();
            _movementStreamAdapter = new ReplayAdapter(replayFile);
            StartProfiling();
        }

        public void EnableStreamAdapter()
        {
            try
            {
                DisableStreamAdapter();
                _movementStreamAdapter = new SerialPortAdapter(Global.Config.SerialPortMovementInput, Global.Config.SerialPortBaud);
            }
            catch (Exception e)
            {
                Logger.Error("Failed while starting up the stream adapter.", e);
                throw;
            }
        }

        public void DisableStreamAdapter()
        {
            if (_movementStreamAdapter == null) return;
            try
            {
                _movementStreamAdapter.Dispose();
                _movementStreamAdapter = null;
            }
            catch (Exception e)
            {
                Logger.Error("Failed while shutting down the stream adapter.", e);
                throw;
            }
        }

        public void Start(string runFolder, string notes)
        {
            Stop();
            EnableStreamAdapter();

            var config = new VirtualProfilerRunConfiguration(runFolder, notes);
            SaveProfilerConfiguration(config, config.SaveStatePath);
            
            if (Global.Config.EnableSubjectLogging)
                _controller.SubjectLogger = new SubjectLogger(config.SubjectPositionPath);

            StartProfiling(new EventStreamWriter(config.MovementLogPath));
        }

        public void StopAndFinalizeRun()
        {
            _controller.SubjectLogger.Save();
        }

        public void Stop()
        {
            _controller.DetachDriver();
            DisableStreamAdapter();
        }

        public bool IsStreamAdapterReceivingData()
        {
            if (_movementStreamAdapter == null)
                return false;

            var ms = new MemoryStream();
            _movementStreamAdapter.WriteToStream(ms);

            return ms.Length > 0;
        }

        private void SaveProfilerConfiguration(VirtualProfilerRunConfiguration config, string path)
        {
            var saveState = new VirtualProfilerSaveState
            {
                GlobalSettings = Global.Config,
                RunSettings = config
            };

            var saveStateFileContents = "";
            var serializer = new XmlSerializer(typeof(VirtualProfilerSaveState));
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, saveState);
                saveStateFileContents = Encoding.UTF8.GetString(ms.ToArray());
            }
            File.WriteAllText(path, saveStateFileContents);
        }

        private void StartProfiling(EventStreamWriter eventStreamWriter = null)
        {
            try
            {
                _controller.AttachDriver(new UnityMovementDriver(_movementStreamAdapter, eventStreamWriter));
            }
            catch (Exception e)
            {
                Logger.Error("Failed while starting the virtual profiler.", e);
                throw;
            }
        }

        public void LoadGlobalConfiguration()
        {
            var serializer = new XmlSerializer(typeof (VirtualProfilerSaveState));
            try
            {
                var globalConfigText = File.ReadAllText("VirtualProfilerGlobal.cfg");
                var config = serializer.Deserialize(new StringReader(globalConfigText)) as VirtualProfilerSaveState;

                if (config == null)
                    SaveGlobalConfiguration();
                else
                    Global.Config = config.GlobalSettings;
            }
            catch (Exception e)
            {
                // The file doesn't exist, or is corrupt.  Just write out fresh.
                SaveGlobalConfiguration();
            }
        }

        public void SaveGlobalConfiguration()
        {
            SaveProfilerConfiguration(null, "VirtualProfilerGlobal.cfg");
        }

        public void OnApplicationQuit()
        {
            Stop();
        }

    }
}