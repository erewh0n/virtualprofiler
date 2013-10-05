using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class SubjectLogger
    {
        private readonly string _path;
        private readonly StringWriter _buffer = new StringWriter();

        public SubjectLogger(string path)
        {
            _path = path;
            File.WriteAllText(_path, "");
            File.Delete(_path);
        }

        public void AddVector(Vector3 positionVector, Quaternion rotation)
        {
            _buffer.WriteLine("{0},{1},{2},{3},{4}", DateTime.UtcNow.ToString("o"), positionVector.x, positionVector.y, positionVector.z, rotation.eulerAngles.y);
        }

        public void Save()
        {
            File.WriteAllText(_path, _buffer.ToString());
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
