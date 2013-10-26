using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Assets.Histogram;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.VirtualProfiler
{
    public class ReplayManager
    {
        private static readonly VirtualObjects Objects = new VirtualObjects();
        private IReplay _activeReplayer;

        public void EnableReplayView()
        {
            Objects.ReplayCamera.enabled = true;
            Objects.RuntimeCamera.enabled = false;
        }

        public void RealTimeReplay(string saveStateFile)
        {
            var savedState = VirtualProfilerSaveState.LoadFromFile(saveStateFile);
            Global.Launcher.SaveState = savedState;
            if (Objects.ReplayController == null) throw new ApplicationException("No replay adapter found.");
            _activeReplayer = new RealTimeReplayer(Objects.ReplayController.LineRenderer, SubjectLogger.Load(savedState.RunSettings.SubjectPositionPath));
            Objects.ReplayController.StartReplay(_activeReplayer);
        }

        public void InstantReplay(string saveStateFile)
        {
            var savedState = VirtualProfilerSaveState.LoadFromFile(saveStateFile);
            Global.Launcher.SaveState = savedState;
            if (Objects.ReplayController == null) throw new ApplicationException("No replay adapter found.");
            _activeReplayer = new InstantReplayer(Objects.ReplayController.LineRenderer, SubjectLogger.Load(savedState.RunSettings.SubjectPositionPath));
            Objects.ReplayController.StartReplay(_activeReplayer);
        }

        public ReplayStats GetStatsEngine()
        {
            if (_activeReplayer == null)
            {
                return null;
            }

            return new ReplayStats(_activeReplayer);
        }

        public void StopReplay()
        {
            if (Objects.ReplayController != null)
                Objects.ReplayController.StopReplay();

            Objects.ReplayCamera.enabled = false;
            Objects.RuntimeCamera.enabled = true;

            _activeReplayer = null;
        }

        public void PlayPause()
        {
            if (_activeReplayer == null) return;

            _activeReplayer.PlayPause();
        }

        public void SetPosition(int segmentPosition)
        {
            _activeReplayer.SetPosition(segmentPosition);
        }
    }

    public class VirtualObjects
    {
        public Camera RuntimeCamera
        {
            get { return GameObject.FindGameObjectWithTag(Global.Config.RuntimeCameraTag).camera; }
        }

        public Camera ReplayCamera
        {
            get { return GameObject.FindGameObjectWithTag(Global.Config.ReplayCameraTag).camera; }
        }

        private ReplayController _replayController;
        public ReplayController ReplayController
        {
            get
            {
                return _replayController ??
                       (_replayController =
                        BuildReplayController());
            }
        }

        private ReplayController BuildReplayController()
        {
            var replayObject = GameObject.FindGameObjectWithTag(Global.Config.LineRendererTag);
            if (replayObject == null)
            {
                // Create a default if tag not found.
                replayObject = new GameObject("vp_replayObject", typeof (LineRenderer));
                var renderer = replayObject.GetComponent<LineRenderer>();
                renderer.SetWidth(1, 1);
                renderer.SetColors(new Color(0, 0, 1), new Color(0, 0, 1));
            }
            replayObject.AddComponent("ReplayController");

            return replayObject.GetComponent<ReplayController>();
        }

        public ParticleRenderer ParticleRenderer
        {
            get { return GameObject.FindWithTag(Global.Config.LineRendererTag).renderer as ParticleRenderer; }
        }

        public LaserController[] LaserControllers
        {
            get { return (from gameObject in Object.FindObjectsOfType(typeof (LaserController)) where (gameObject as LaserController) != null select (LaserController) gameObject).ToArray(); }
        }

    }

    public class VirtualProfiler
    {
        private readonly SubjectController _controller;
        private SerialPortAdapter _movementStreamAdapter;
        private VirtualProfilerSaveState _saveState;

        public SerialPortAdapter SerialPortAdapter { get { return _movementStreamAdapter; } }
        public VirtualProfilerSaveState SaveState { get { return _saveState; } set { _saveState = value; } }
        
        public VirtualProfiler()
        {
            _controller = Object.FindObjectOfType(typeof(SubjectController)) as SubjectController;
            if (_controller == null)
                throw new ArgumentException("Could not find the subject controller.  Please attach the 'SubjectController' script to a Unity object.");
        }

        public void Initialize()
        {
            if (Global.Objects.ReplayCamera != null)
                Global.Objects.ReplayCamera.enabled = false;
            if (Global.Objects.RuntimeCamera != null)
                Global.Objects.RuntimeCamera.enabled = true;
            else
                (Object.FindObjectOfType(typeof (Camera)) as Camera).enabled = true;
			
			Application.runInBackground = true;
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
                _movementStreamAdapter.Close();
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

            _saveState = new VirtualProfilerSaveState()
                {
                    RunSettings = new VirtualProfilerRunConfiguration(runFolder, notes),
                    GlobalSettings = Global.Config,
                    RuntimeResults = null
                };
            SaveProfilerConfiguration(_saveState, _saveState.RunSettings.SaveStatePath);
            
            if (Global.Config.EnableSubjectLogging)
                _controller.SubjectLogger = new SubjectLogger(_saveState.RunSettings.SubjectPositionPath);

            _controller.HistogramTrackers.Clear();
            _controller.HistogramTrackers.Add(new HistogramTracker(
                _controller.transform,
                _saveState.RunSettings.RunFolder,
                _saveState.GlobalSettings));

            _controller.HistogramTrackers.Add(new HistogramDistanceTracker(
                _saveState.RunSettings.RunFolder,
                _saveState.GlobalSettings,
                _controller.transform,
                _controller.ChaserTransform));

            _controller.HistogramTrackers.Add(new HistogramOrientationTracker(
                _saveState.RunSettings.RunFolder,
                _saveState.GlobalSettings,
                _controller.transform,
                _controller.ChaserTransform));

            StartProfiling(new EventStreamWriter(_saveState.RunSettings.MovementLogPath));
        }

        public void StopAndFinalizeRun()
        {
            if (_controller.SubjectLogger != null)
                _controller.SubjectLogger.Save();

            var totalTime = Global.Objects.LaserControllers.Aggregate(0f, (x, y) => y.TotalTimeOn);
            _saveState.RuntimeResults = new RuntimeResults
                {
                    TotalLaserTime = totalTime,
                };
            SaveProfilerConfiguration(_saveState, _saveState.RunSettings.SaveStatePath);

            Stop();
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

            return _movementStreamAdapter.SerialStream.Count() != 0;
        }

        private void SaveProfilerConfiguration(VirtualProfilerSaveState saveState, string path)
        {
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
                _controller.AttachDriver(new UnityMovementDriver(_movementStreamAdapter, new MovementProtocolAdapter(Global.Config.MinMotionFilter)));
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
            SaveProfilerConfiguration(new VirtualProfilerSaveState
            {
                GlobalSettings = Global.Config,
            }, "VirtualProfilerGlobal.cfg");
        }

        public void OnApplicationQuit()
        {
            Stop();
        }

    }
}