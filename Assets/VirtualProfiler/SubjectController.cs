using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class SubjectController : MonoBehaviour
    {
        private Transform _subject;
        private UnityMovementDriver _driver;
        private Vector3 _delta;
        private bool _moving;

        public void Start()
        {
            _delta = Vector3.zero;
            _subject = transform;

            try
            {
                if (!Directory.Exists(Global.Config.MovementLogDirectory))
                    Directory.CreateDirectory(Global.Config.MovementLogDirectory);
                var path = Path.Combine(Global.Config.MovementLogDirectory,
                                        string.Format("MovementStream.{0}.log",
                                                      DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")));
                _driver = new UnityMovementDriver(new SerialPortAdapter(), new EventStreamWriter(path));
                _moving = false;
            }
            catch (Exception e)
            {
                Logger.Error("Failed while setting up profiler event logger.", e);
            }
        }

        public void OnApplicationQuit()
        {
            _driver.Dispose();
        }

        private void Scale(Vector3 vector)
        {
            vector.x = Global.Config.ScaleX*Time.deltaTime;
            vector.y = Global.Config.ScaleY*Time.deltaTime;
            vector.z = Global.Config.ScaleZ*Time.deltaTime;
        }

        private IEnumerator Move()
        {

            _moving = true;
            var xzVector = _delta;
            xzVector.y = 0;
            xzVector.x *= (Global.Config.ScaleX*Time.deltaTime);
            xzVector.z *= (Global.Config.ScaleZ*Time.deltaTime);
            var curPos = _subject.transform.position;
            var endPos = curPos + xzVector;
            var curRotation = _subject.rotation;
            var endRotation = _subject.rotation*Quaternion.Euler(new Vector3(0, _delta.y, 0));
            Logger.Debug(string.Format("Moving subject: {0}", _delta));
            for (var t = 0f; t < 1; t += (Time.deltaTime / Global.Config.Smoothing))
            {
                _subject.transform.position = Vector3.Lerp(curPos, endPos, t);
                _subject.rotation = Quaternion.Slerp(curRotation, endRotation, t);

                yield return 0;
            }

            _delta = Vector3.zero;
            _moving = false;
        }

        public void Update()
        {
            try
            {
                var vectors = _driver.GetVectors().ToList();
                var summedVector = vectors.Aggregate(Vector3.zero, (current, vector) => current + vector);
                _delta += summedVector;

                if (_moving || _delta == Vector3.zero)
                    return;
                Scale(_delta);
                StartCoroutine(Move());
            }
            catch (Exception e)
            {
                Logger.Error("Something bad happened while trying to read movement data.", e);
            }
        }

    }

}
