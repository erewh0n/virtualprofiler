using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets.VirtualProfiler
{
    public static class StringExtensions
    {
        public static string ValidateStringAsFilePath(this string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException("filepath", "The filename cannot be empty.");
            }
            var dir = Path.GetDirectoryName(filepath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                throw new ArgumentException(string.Format("The specified directory '{0}' does not exist.", dir));
            }

            return filepath;
        }

    }

    public class EventStreamWriter : IDisposable
    {
        private const string Version = "1.0";
        private StringWriter _buffer;
        private string _eventFilePath;
        
        public EventStreamWriter(string eventFilePath)
        {
            _buffer = new StringWriter();
            _eventFilePath = eventFilePath;

            WriteHeader();
        }

        public void Write(Event @event)
        {
            if (@event != null) this.Write(new List<Event>{@event});
        }

        public void Write(IEnumerable<Event> events)
        {
            if (events == null) return;

            foreach (var @event in events)
                _buffer.Write(@event.ToString());
        }

        private void WriteHeader()
        {
            // TODO KPH: this should be done by the Event class.
            _buffer.Write(string.Format("[EventLog({0}) Timestamp:{1}]{2}", Version, DateTime.UtcNow, Environment.NewLine));
        }

        public void Dispose()
        {
            File.WriteAllText(_eventFilePath, _buffer.ToString());
        }

    }

}
