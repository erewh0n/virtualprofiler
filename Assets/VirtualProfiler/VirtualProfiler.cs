using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.VirtualProfiler
{
    public class ReplayAdapter
    {
        private readonly List<string> _positions = new List<string>();
        private readonly LineRenderer _renderer = null;

        public ReplayAdapter(LineRenderer renderer, string replayFile)
        {
            if (renderer == null)
                throw new ArgumentException("In order for replay to work properly a LineRenderer component must be attached to a game object!");
            _positions.AddRange(File.ReadAllLines(replayFile));
            _renderer = renderer;
        }

        public void RenderPath()
        {
            var vectors = (from position in _positions
                          let coordinates = position.Split(',')
                          where coordinates.Length == 4
                          select
                              new Vector3(float.Parse(coordinates[0]), float.Parse(coordinates[1]),
                                          float.Parse(coordinates[2]))).ToArray();

            _renderer.SetVertexCount(vectors.Count());

            for (var i = 0; i < vectors.Count(); i++)
            {
                _renderer.SetPosition(i, vectors[i]);
            }
        }

        public void Clear()
        {
            _renderer.SetVertexCount(0);
        }
    }

    public class VirtualProfiler
    {
        private readonly SubjectController _controller;
        private SerialPortAdapter _movementStreamAdapter;

        public VirtualProfiler()
        {
            _controller = Object.FindObjectOfType(typeof(SubjectController)) as SubjectController;
            if (_controller == null)
                throw new ArgumentException("Could not find the subject controller.  Please attach the 'SubjectController' script to a Unity object.");
        }

        private LineRenderer LineRenderer
        {
            get { return GameObject.FindWithTag(Global.Config.LineRendererTag).renderer as LineRenderer; }
        }

        private ParticleRenderer ParticleRenderer
        {
            get { return GameObject.FindWithTag(Global.Config.LineRendererTag).renderer as ParticleRenderer; }
        }

        private Camera RuntimeCamera
        {
            get { return GameObject.FindGameObjectWithTag(Global.Config.RuntimeCameraTag).camera; }
        }

        private Camera ReplayCamera
        {
            get { return GameObject.FindGameObjectWithTag(Global.Config.ReplayCameraTag).camera; }
        }

        private GameObject SurfaceLayer
        {
            get { return GameObject.FindWithTag(Global.Config.SurfaceLayerTag); }
        }

        public void StartReplay()
        {
            ReplayCamera.enabled = true;
            RuntimeCamera.enabled = false;
            SurfaceLayer.renderer.enabled = false;
            Logger.Debug(string.Format("surface info: {0}, {1}", SurfaceLayer.tag, SurfaceLayer.name));
        }

        public void RenderReplay(string replayFile)
        {
            var replayRenderer = new ReplayAdapter(LineRenderer, replayFile);
            
            replayRenderer.RenderPath();
        }

        public void StopReplay()
        {
            if (LineRenderer != null)
                LineRenderer.SetVertexCount(0);

            ReplayCamera.enabled = false;
            RuntimeCamera.enabled = true;
            SurfaceLayer.renderer.enabled = true;
        }

        public void Initialize()
        {
            if (ReplayCamera != null)
                ReplayCamera.enabled = false;
            if (RuntimeCamera != null)
                RuntimeCamera.enabled = true;
            else
                (Object.FindObjectOfType(typeof (Camera)) as Camera).enabled = true;
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

            var config = new VirtualProfilerRunConfiguration(runFolder, notes);
            SaveProfilerConfiguration(config, config.SaveStatePath);
            
            if (Global.Config.EnableSubjectLogging)
                _controller.SubjectLogger = new SubjectLogger(config.SubjectPositionPath);

            StartProfiling(new EventStreamWriter(config.MovementLogPath));
        }

        public void StopAndFinalizeRun()
        {
            if (_controller.SubjectLogger != null)
                _controller.SubjectLogger.Save();

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
                _controller.AttachDriver(new UnityMovementDriver(_movementStreamAdapter, new MovementProtocolAdapter()));
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