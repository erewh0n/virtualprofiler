using System;
using System.Collections;
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
        private bool _isSimpleMovement = true;
        private DateTime _laserOnTime;
        private bool _laserIsOn;

        public int LaserDuration = 500; // Sane default

        public SubjectLogger SubjectLogger { get; set; }
        public bool IsSimpleMovement
        {
            get { return _isSimpleMovement; }
            set { _isSimpleMovement = value; }
        }

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
            vector.x *= Global.Config.ScaleX * Time.deltaTime;
            vector.y *= Global.Config.ScaleY * Time.deltaTime;
            vector.z *= Global.Config.ScaleZ * Time.deltaTime;

            return vector;
        }

        private void SimpleMove()
        {
            var xzVector = _delta;
            xzVector.y = 0;

            _subject.transform.rigidbody.MovePosition(_subject.transform.position + _subject.TransformDirection(xzVector));
            _subject.rotation = _subject.rotation * Quaternion.Euler(new Vector3(0, _delta.y, 0));

            _delta = Vector3.zero;
        }

        private IEnumerator Move()
        {
            var xzVector = _delta;
            xzVector.y = 0;
            var curPos = _subject.transform.position;
            xzVector = _subject.TransformDirection(xzVector);
            var endPos = curPos + xzVector;
            var curRotation = _subject.rotation;
            var endRotation = _subject.rotation * Quaternion.Euler(new Vector3(0, _delta.y, 0));

            // Logger.Debug(string.Format("Moving subject: {0}", _delta));
            for (var t = 0f; t < 1; t += (Time.deltaTime / Global.Config.Smoothing))
            {
                _subject.transform.rigidbody.MovePosition(Vector3.Lerp(curPos, endPos, t));
                _subject.rotation = Quaternion.Slerp(curRotation, endRotation, t);

                yield return 0;
            }

            _delta = Vector3.zero;
        }


        public void Update()
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    // Debug.Log("Firing laser!");
                    _driver.SendData("z1");
                    _laserIsOn = true;
                    _laserOnTime = DateTime.UtcNow;
                }
                if (_laserIsOn && (DateTime.UtcNow - _laserOnTime > TimeSpan.FromMilliseconds(LaserDuration)))
                {
                    _driver.SendData("z0");
                    _laserIsOn = false;
                }
                if (_driver == null || _moving)
                {
                    return;
                }

                var vectors = _driver.GetMovementVectors().ToList();
                // Evaluate the overall change in position since last read from driver.
                // Could try avoiding the aggregation, but might be expensive.
                var summedVector = vectors.Aggregate(Vector3.zero, (current, vector) => current + vector);

                if (summedVector == Vector3.zero) return;

                _delta += Scale(summedVector);
                if (_delta == Vector3.zero)
                    return;

                _moving = true;
                if (IsSimpleMovement)
                    SimpleMove();
                else
                    StartCoroutine(Move());
                _moving = false;

                if (SubjectLogger != null)
                    SubjectLogger.AddVector(Time.time, _subject.position, _subject.rotation);
            }
            catch (Exception e)
            {
                // Do something nice here?  Prompt user with menu?
                Logger.Error("Something bad happened while trying to read movement data.", e);
            }
        }
    }
}
