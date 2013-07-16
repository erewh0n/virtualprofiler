using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.VirtualProfiler
{

    public class UnityMovementDriver : IDisposable
    {
        private readonly MovementProtocolAdapter _vectorAdapter;
        private readonly SerialPortAdapter _movementStream;

        public UnityMovementDriver(SerialPortAdapter movementStream, MovementProtocolAdapter vectorAdapter)
        {
            _movementStream = movementStream;
            _vectorAdapter = vectorAdapter;
        }

        public IEnumerable<Vector3> GetMovementVectors()
        {
            FastForwardIfNecessary();

            return _vectorAdapter.GetVectors(_movementStream.SerialStream).Take(3);
        }

        public void SendData(string data)
        {
            _movementStream.Write(data);
        }

        // TODO KPH: need to figure out if SerialPort is really broken.
        public void FastForwardIfNecessary()
        {
            // Can't do anything about this...
        }

        public void Dispose()
        {
            _movementStream.Close();
        }
    }

}
