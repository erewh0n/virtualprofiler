using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class SerialPortAdapter : IStreamAdapter
    {
        private readonly SerialPort _serialPort =
            new SerialPort(
                Global.Config.SerialPortMovementInput,
                Global.Config.SerialPortBaud,
                Parity.None,
                8,
                StopBits.One);

        public SerialPortAdapter()
        {
            _serialPort.ReadTimeout = 5; // In milliseconds.
            // TODO KPH: probably want this config'd.
            _serialPort.DtrEnable = true;
            _serialPort.RtsEnable = true;

            Thread.Sleep(500); // Play nice.
            _serialPort.Open();

            Logger.Debug("Serial port opened.");
        }

        public int WriteToStream(MemoryStream buffer)
        {
            var bytesWritten = 0;
            try
            {
                var sb = (byte) _serialPort.ReadByte();
                while (sb != '\0')
                {
                    bytesWritten++;
                    buffer.WriteByte(sb);
                    sb = (byte) _serialPort.ReadByte();
                }
            }
            catch (TimeoutException)
            {
                // We can safely ignore.
            }

            return bytesWritten;
        }

        public void Dispose()
        {
            _serialPort.Close();
        }
    }
}