using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    /// <summary>
    /// TODO KPH: this is kind of a hack.  We should encapsulate the format of a replay "record"
    /// TODO KHP: and rename/conceptualize this logger as a "RuntimeRecorder".
    /// </summary>
    public class SubjectLogger
    {
        private int _bufferSize;
        private readonly FileStream _fs;

        public SubjectLogger(string path)
        {
            _fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
        }

        public void AddVector(Vector3 positionVector, Quaternion rotation)
        {
            var bytesToWrite  = Encoding.UTF8.GetBytes(
                                string.Format("{0},{1},{2},{3},{4}{5}",
                                                DateTime.UtcNow.ToString("o"),
                                                positionVector.x,
                                                positionVector.y,
                                                positionVector.z,
                                                rotation.eulerAngles.y,
                                                Environment.NewLine));
            _fs.Write(bytesToWrite, 0, bytesToWrite.Count());
        }

        public void Save()
        {
            _fs.Flush();
            _fs.Close();
        }

        public static IEnumerable<TimeVector> Load(string path)
        {
            var now = DateTime.UtcNow - TimeSpan.FromDays(2);
            var positions = File.ReadAllLines(path);
            return from position in positions
                   let coordinates = position.Split(',')
                   where coordinates.Length == 5
                   select
                       new TimeVector
                           {
                               Time = DateTime.Parse(coordinates[0]), // now + TimeSpan.FromSeconds(float.Parse(coordinates[0])),
                               Vector = new Vector3(float.Parse(coordinates[1]),
                                                    float.Parse(coordinates[2]),
                                                    float.Parse(coordinates[3])),
                           };
        }
    }
}
