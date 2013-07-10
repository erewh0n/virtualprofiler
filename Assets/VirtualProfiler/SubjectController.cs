using System;
using System.Collections;
using System.Collections.Generic;
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

        public SubjectLogger SubjectLogger { get; set; }

        public void AttachDriver(UnityMovementDriver driver)
        {
            if (driver == null) throw new ArgumentNullException("driver");

            DetachDriver();
            _driver = driver;
            _moving = false;
            _delta = Vector3.zero;
        }

        public void DetachDriver()
        {
            if (_driver != null)
                _driver.Dispose();

            _moving = false;
            _delta = Vector3.zero;
            _driver = null;
        }

        public void Start()
        {
            _subject = transform;
            _driver = null;
        }

        private Vector3 Scale(Vector3 vector)
        {
            // Not sure the delta makes sense here.  These are controlled by user anyway.
            vector.x *= Global.Config.ScaleX*Time.deltaTime;
            vector.y *= Global.Config.ScaleY*Time.deltaTime;
            vector.z *= Global.Config.ScaleZ*Time.deltaTime;

            return vector;
        }

        private IEnumerator Move()
        {
            _moving = true;

            var xzVector = _delta;
            xzVector.y = 0;
            var curPos = _subject.transform.position;
            xzVector = _subject.TransformDirection(xzVector);
            var endPos = curPos + xzVector;
            var curRotation = _subject.rotation;
            var endRotation = _subject.rotation*Quaternion.Euler(new Vector3(0, _delta.y, 0));

            // Logger.Debug(string.Format("Moving subject: {0}", _delta));
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
                if (_driver == null)
                {
                    return;
                }
                var vectors = _driver.GetVectors().ToList();
                // Evaluate the overall change in position since last read from driver.
                // Could try avoiding the aggregation, but might be expensive.
                var summedVector = vectors.Aggregate(Vector3.zero, (current, vector) => current + vector);

                if (summedVector == Vector3.zero) return;

                _delta += Scale(summedVector);
                if (_moving || _delta == Vector3.zero)
                    return;

                StartCoroutine(Move());
    
                if (SubjectLogger != null)
                    SubjectLogger.AddVector(_subject.position, _subject.rotation);
            }
            catch (Exception e)
            {
                // Do something nice here?  Prompt user with menu?
                Logger.Error("Something bad happened while trying to read movement data.", e);
            }
        }

    }

}
