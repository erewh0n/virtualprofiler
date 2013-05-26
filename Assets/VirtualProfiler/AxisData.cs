using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.VirtualProfiler
{

    public class ArduinoAxisMovementProtocol
    {
        private const byte BoundaryByte = (byte) 'e';
        private const long MaximumVectorSize = 30;

        public static byte[] ParseFromStream(MemoryStream stream)
        {
            var startPosition = stream.Position;
            var bytesRead = 1;
            var data = stream.ReadByte();
            while ((data != -1) && (data != BoundaryByte))
            {
                if ((bytesRead) > MaximumVectorSize)
                    throw new InvalidDataException(
                        string.Format("The stream data length exceeded the maximum vector size."));
                data = stream.ReadByte();
                bytesRead++;
            }
            if (data == -1)
            {
                stream.Position = startPosition;
            }
            else if ((data == BoundaryByte) && (bytesRead > 1))
            {
                var axialDataSegment = new byte[bytesRead];
                stream.Position = startPosition;
                if (stream.Read(axialDataSegment, 0, axialDataSegment.Length) != axialDataSegment.Length)
                    throw new InvalidDataException("Failed to read the expected data from the stream.");

                return axialDataSegment;
            }

            return null;
        }

        public static Vector3 ToVector(string axisData)
        {
            var axes = (from axisDelta in axisData.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        select int.Parse(Regex.Replace(axisDelta, "[a-zA-Z]*", "")))
                .ToArray();
            if (axes.Length != 6)
                throw new InvalidDataException(string.Format(
                    "The movement data could not be converted to a vector: {0}", axisData));

            return new Vector3(axes[2] - axes[3], axes[4] - axes[5], axes[0] - axes[1]);
        }

    }

}
