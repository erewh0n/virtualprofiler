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

        public Stream GetData()
        {
            // Debug.Log(string.Format("{0}: Reading data at time: {1}", Time.time, Events.Peek().DeltaTime));
            var stream = new MemoryStream();
            
            while (Events.Count > 0 && Events.Peek().DeltaTime < Time.time)
            {
                var @event = Events.Pop();
                // Debug.Log(string.Format("event time: {0}, data: {1}", @event.DeltaTime, @event.Payload));
                var buffer = Encoding.UTF8.GetBytes((string) @event.Payload);
                stream.Write(buffer, 0, buffer.Length);
                stream.WriteByte((byte) 'e');
            }
    
            stream.Position = 0;
            return stream;
        }

        public void Dispose()
        {
            // NOP.
        }
    }

    public interface IStreamAdapter : IDisposable
    {
        Stream GetData();
    }

    public class UnityMovementDriver : IDisposable
    {
        private readonly IStreamAdapter _adapter;
        private readonly EventLogger _logger;

        private StringBuilder _buffer;

        public UnityMovementDriver(IStreamAdapter adapter, EventLogger logger)
        {
            _adapter = adapter;
            _logger = logger;
            _buffer = new StringBuilder();
        }

        public IEnumerable<Vector3> GetVectors()
        {
            // Debug.Log("Getting vectors...");
            var data = _adapter.GetData();
            var timestamp = Time.time;

            if (data.Length <= 0)
                return new List<Vector3>();

            _buffer.Append(new StreamReader(data).ReadToEnd());
            var stringValue = _buffer.ToString();

            //Debug.Log(string.Format("Raw serial data: '{0}'", stringValue));

            var i0 = stringValue.IndexOf(AxisData.Boundary);
            var i1 = stringValue.LastIndexOf(AxisData.Boundary);

            if ((i0 == -1) || (i1 == -1))
                return new List<Vector3>();

            //Debug.Log(string.Format("length: {0} offsets: {1}, {2}", stringValue.Length, i0, i1));
            //return new List<Vector3>();
            _buffer = new StringBuilder(stringValue.Substring(i1));
            stringValue = stringValue.Substring(i0, i1-i0);


            var vectors = new List<Vector3>();
            var events = new List<Event>();
            foreach (var delta in AxisData.Create(stringValue))
            {
                events.Add(new Event(timestamp, delta));
                vectors.Add(delta.ToVector());
            }
            if (events.Count > 0) _logger.Write(events);

            return vectors;
        }

        public void Dispose()
        {
            _adapter.Dispose();
        }

    }
}