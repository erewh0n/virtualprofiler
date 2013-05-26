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
                Global.Instance.SerialPortMovementInput,
                Global.Instance.SerialPortBaud,
                Parity.None,
                8,
                StopBits.One);

        public SerialPortAdapter()
        {
            _serialPort.Open();
            _serialPort.ReadTimeout = 5; // in milliseconds
            DataStream = new MemoryStream();
            Debug.Log("Serial port opened.");
        }

        public MemoryStream WriteToStream(MemoryStream buffer)
        {
            try
            {
                var sb = (byte) _serialPort.ReadByte();
                while (sb != '\0')
                {
                    buffer.WriteByte(sb);
                    sb = (byte) _serialPort.ReadByte();
                }
            }
            catch (TimeoutException)
            {
                // We can safely ignore.
            }

            return buffer;
        }

        public void Dispose()
        {
            _serialPort.Close();
        }
    }
}