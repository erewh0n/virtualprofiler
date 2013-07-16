using System.IO;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class SubjectLogger
    {
        private readonly string _path;
        private StringWriter _buffer = new StringWriter();

        public SubjectLogger(string path)
        {
            _path = path;
            File.WriteAllText(_path, "");
            File.Delete(_path);
        }

        public void AddVector(Vector3 positionVector, Quaternion rotation)
        {
            _buffer.WriteLine("{0},{1},{2},{3}", positionVector.x, positionVector.y, positionVector.z, rotation.eulerAngles.y);
        }

        public void Save()
        {
            File.WriteAllText(_path, _buffer.ToString());
        }
    }
}