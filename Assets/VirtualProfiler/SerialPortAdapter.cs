using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;


namespace Assets.VirtualProfiler
{
    public class SerialPortAdapter
    {
        private readonly SerialPort _serialPort;

        public SerialPortAdapter(string comPort, int baud)
        {
            _serialPort =
                        new SerialPort(
                            comPort,
                            baud,
                            Parity.None,
                            8,
                            StopBits.One)
                            {
                                ReadBufferSize = 50, // No effect?
                                ReadTimeout = 5,
                                DtrEnable = true,
                                RtsEnable = true
                            };

            Thread.Sleep(500); // Play nice.
            _serialPort.Open();
        }

        public IEnumerable<byte> SerialStream
        {
            get
            {
                var sb = ReadFromStream();
                while (sb != 0)
                {
                    yield return sb;
                    sb = ReadFromStream();
                }
            }
        }

        // TODO KPH: this seems to be broken in Unity. :(
        public int NumBytesInBuffer
        {
            get
            {
                return _serialPort.BytesToRead;
            }
        }

        public void Write(string data)
        {
            _serialPort.Write(data);
        }

        public void FastForward()
        {
            _serialPort.DiscardInBuffer();
        }

        public void Close()
        {
            _serialPort.Close();
            _serialPort.Dispose();
        }

        private byte ReadFromStream()
        {
            try
            {
                return (byte)_serialPort.ReadByte();
            }
            catch (TimeoutException)
            {
            }
            return 0;
        }

    }

}
