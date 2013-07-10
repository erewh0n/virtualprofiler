using System;
using System.IO;
using System.IO.Ports;
using System.Threading;


namespace Assets.VirtualProfiler
{

    public class SerialPortAdapter : IStreamAdapter
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
                                ReadTimeout = 5,
                                DtrEnable = true,
                                RtsEnable = true
                            };

            Thread.Sleep(500); // Play nice.
            _serialPort.Open();

            Logger.Debug(string.Format("Serial port opened on {0}.", comPort));
        }

        // TODO KPH: So currently this just reads everything available on the port
        // TODO KPH: for performance, we may want to try limiting the read time/size.
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
