using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Assets.VirtualProfiler
{
    [Serializable]
    public class VirtualProfilerSaveState
    {
        public VirtualProfilerRunConfiguration RunSettings;
        public GlobalConfiguration GlobalSettings;
    }

    [Serializable]
    public class VirtualProfilerRunConfiguration
    {
        protected VirtualProfilerRunConfiguration()
        {
        }

        public VirtualProfilerRunConfiguration(string runFolder, string notes)
        {
            Timestamp = DateTime.UtcNow;
            RunFolder = Path.Combine(Global.Config.MovementLogDirectory, runFolder);
            if (!Directory.Exists(RunFolder))
                Directory.CreateDirectory(RunFolder);
            MovementLogPath = Path.Combine(RunFolder,
                                           string.Format("MovementStream.{0}.log",
                                                         Timestamp.ToString("yyyyMMdd-HHmmss")));
            SaveStatePath = Path.Combine(RunFolder,
                                           string.Format("VirtualProfileRun.{0}.cfg",
                                                         Timestamp.ToString("yyyyMMdd-HHmmss")));
            Notes = notes;
        }

        public string MovementLogPath { get; private set; }
        public string SaveStatePath { get; private set; }
        public string RunFolder { get; private set; }
        public string Notes { get; private set; }

        public bool IsPathRendererEnabled { get; set; }

        public DateTime Timestamp { get; private set; }

        public static VirtualProfilerRunConfiguration LoadFromFile(string filePath)
        {
            var configText = File.ReadAllText(filePath);
            var serializer = new XmlSerializer(typeof(VirtualProfilerSaveState));

            return (VirtualProfilerRunConfiguration) serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(configText)));
        }
    }
}
