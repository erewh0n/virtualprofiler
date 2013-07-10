using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class ReplayAdapter : IStreamAdapter
    {
        public Stack<Event> Events { get; set; }
        
        public ReplayAdapter(string streamFileName)
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
            var lines = File.ReadAllLines(streamFileName);
            Events = new Stack<Event>((from line in lines select Event.Parse(line)).Where(x => x != null).Reverse());
        }

        public int WriteToStream(MemoryStream stream)
        {
            var numWritten = 0;
            var count = 0;
            // TODO KPH: revisit this.  Allow for real time seeking (e.g. play, pause, ff, rw, etc)
            while (Events.Count > 0 && count++ < 100)// Events.Peek().DeltaTime < Time.time
            {
                var @event = Events.Pop();
                var buffer = Encoding.UTF8.GetBytes((string) @event.Payload);
                stream.Write(buffer, 0, buffer.Length);
                numWritten += buffer.Length + 1;
            }

            return numWritten;
        }

        public void Dispose()
        {
            // NOP.
        }
    }
}