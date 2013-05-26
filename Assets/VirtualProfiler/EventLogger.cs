using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class EventLogger
    {
        private const string Version = "1.0";
        private readonly FileStream _movementStream;
        
        public EventLogger(string streamFileName)
        {
            if (string.IsNullOrEmpty(streamFileName))
            {
                throw new ArgumentNullException("streamFileName", "The filename for the movement stream cannot be empty.");
            }
            var dir = Path.GetDirectoryName(streamFileName);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                throw new ArgumentException("The specified file '{0}' does not exist.");
            }
            _movementStream = new FileStream(streamFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            WriteHeader();
            Debug.Log("Event log started." + Environment.NewLine);
        }

        public void Write(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                // Debug.Log(string.Format("Writing event: {0}", @event.ToString()));
                var bytes = Encoding.UTF8.GetBytes(@event.ToString() + Environment.NewLine);
                _movementStream.Write(bytes, 0, bytes.Length);
                _movementStream.Flush();
            }
        }

        private void WriteHeader()
        {
            var bytes = Encoding.UTF8.GetBytes(string.Format("[EventLog({0}) Timestamp:{1}]{2}", Version, DateTime.UtcNow, Environment.NewLine));
            _movementStream.Write(bytes, 0, bytes.Length);
            _movementStream.Flush();
        }

    }
}