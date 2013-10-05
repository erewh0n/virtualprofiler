using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class MovementProtocolAdapter
    {
        private StringBuilder _buffer = new StringBuilder();

        public IEnumerable<Vector3> GetVectors(IEnumerable<byte> bytes)
        {
            var enumerator = bytes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                WriteDataIfValid(_buffer, enumerator.Current);

                if (enumerator.Current == 'e')
                {
                    var vector = ToVector(_buffer.ToString());
                    _buffer = new StringBuilder();

                    if (vector.HasValue) yield return vector.Value;
                }
            }
        }

        private static void WriteDataIfValid(StringBuilder buffer, byte data)
        {
            const string validCharacters = "0123456789,";

            if (validCharacters.Contains((char) data))
            {
                buffer.Append((char) data);
            }
        }

        private static Vector3? ToVector(string buffer)
        {
            // Debug.Log("buffer: " + buffer);
            try
            {
                var axes =
                    (from axis in buffer.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                     select int.Parse(axis)).ToArray();

                if (axes.Length != 6) return null;

                return new Vector3(axes[2] - axes[3],  // X axis (planar)
                                   axes[4] - axes[5],  // Y axis (rotation)
                                   axes[0] - axes[1]); // Z axis (planar)
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}