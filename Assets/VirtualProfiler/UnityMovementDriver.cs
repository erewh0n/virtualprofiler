using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class UnityMovementDriver : IDisposable
    {
        private readonly IStreamAdapter _adapter;
        private readonly EventStreamWriter _logger;
        private MemoryStream _buffer;


        public UnityMovementDriver(IStreamAdapter adapter, EventStreamWriter logger)
        {
            _adapter = adapter;
            _logger = logger;
            _buffer = new MemoryStream();
        }

        public bool SyncWithStream()
        {
            // TODO KPH: use maximum time for sync.
            byte[] data = null;
            if (data == null)
                return false;

            return true;
        }

        public IEnumerable<Vector3> GetVectors()
        {
            var timestamp = Time.time;

            var currentPosition = _buffer.Position;
            _buffer.Seek(0, SeekOrigin.End);
            var bytesWritten = _adapter.WriteToStream(_buffer);
            _buffer.Position = currentPosition;

            if (bytesWritten == 0)
                return new List<Vector3>();
                
            byte[] axisData;
            var vectors = new List<Vector3>();
            while ((axisData = ArduinoAxisMovementProtocol.ParseFromStream(_buffer)) != null)
            {
                currentPosition = _buffer.Position;
                var axisStringData = Encoding.UTF8.GetString(axisData);

                if (_logger != null) _logger.Write(new Event(timestamp, axisStringData));
                try
                {
                    vectors.Add(ArduinoAxisMovementProtocol.ToVector(axisStringData));
                }
                catch (Exception e)
                {
                    Logger.Warning(string.Format("Ignoring bad data: {0}", axisStringData), e);
                }
            }

            var bytesRemaining = _buffer.Length - currentPosition;
            if (bytesRemaining > 0)
            {
                var remaining = new byte[bytesRemaining];
                _buffer.Position = currentPosition;
                _buffer.Read(remaining, 0, (int) bytesRemaining);
                _buffer.Dispose();
                _buffer = new MemoryStream();
                _buffer.Write(remaining, 0, remaining.Length);
                _buffer.Position = 0;
            }

            return vectors;
        }

        public void Dispose()
        {
            _adapter.Dispose();
        }

    }
}
