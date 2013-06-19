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
        private readonly FileStream _stream;
        
        public EventStreamWriter(string streamFileName)
        {
            _stream = new FileStream(streamFileName.ValidateStringAsFilePath(), FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            WriteHeader();
        }

        public void Write(Event @event)
        {
            if (@event != null) this.Write(new List<Event>{@event});
        }

        public void Write(IEnumerable<Event> events)
        {
            if (events == null) return;
            foreach (var bytes in events.Select(@event => Encoding.UTF8.GetBytes(@event.ToString())))
            {
                _stream.Write(bytes, 0, bytes.Length);
                _stream.Flush();
            }
        }

        private void WriteHeader()
        {
            // TODO KPH: this should be done by the Event class.
            var bytes = Encoding.UTF8.GetBytes(string.Format("[EventLog({0}) Timestamp:{1}]{2}", Version, DateTime.UtcNow, Environment.NewLine));
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Flush();
        }

        public void Dispose()
        {
            _stream.Close();
            System.GC.SuppressFinalize(_stream);
        }

    }

}
