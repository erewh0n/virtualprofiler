using System;
using System.IO;
using System.IO.Ports;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class SerialPortAdapter : IStreamAdapter
    {
        public MemoryStream DataStream { get; private set; }
        private readonly SerialPort _serialPort =
            new SerialPort(
                Global.Config.SerialPortMovementInput,
                Global.Config.SerialPortBaud,
                Parity.None,
                8,
                StopBits.One);

        public SerialPortAdapter()
        {
            _serialPort.Open();
            _serialPort.ReadTimeout = 5; // in milliseconds
            DataStream = new MemoryStream();
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