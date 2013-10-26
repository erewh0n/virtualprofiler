using System;
using System.Linq;
using Assets.Histogram;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.VirtualProfiler
{
    [RequireComponent(typeof(CharacterController))]
    public class SubjectController : MonoBehaviour
    {
        [SerializeField]
        private float scaleX = 1;
        
        [SerializeField]
        private float scaleY = 1;
        
        [SerializeField]
        private float scaleZ = 1;

        [SerializeField]
        private bool useDeltaTime = true;
        
        [SerializeField]
        private Transform _chaser;
        
        [SerializeField]
        private bool keyboardControls = false;
        
        private Vector3 _lastPos;
        //
        private Transform _subject;
        private UnityMovementDriver _driver;
        private Vector3 _delta;
        private bool _moving;
        private bool _isSimpleMovement = true;
        private DateTime _laserOnTime;
        private bool _laserIsOn;


        public int LaserDuration = 500; // Sane default

        public List<AbstractHistogramTracker> HistogramTrackers = new List<AbstractHistogramTracker>();

        public SubjectLogger SubjectLogger { get; set; }

        public Transform ChaserTransform
        {
            get { return _chaser; }
        }

        public bool IsSimpleMovement
        {
            get { return _isSimpleMovement; }
            set { _isSimpleMovement = value; }
        }

        private CharacterController _controller;
        private bool _trackingActive = false;

        public void AttachDriver(UnityMovementDriver driver)
        {
            StopAllCoroutines();
            _trackingActive = false;

            if (driver == null) throw new ArgumentNullException("driver");

            DetachDriver();
            _driver = driver;
            _moving = false;
            _delta = Vector3.zero;

            foreach (var tracker in HistogramTrackers)
            {
                tracker.HistogramRenderer.RenderSize = new Vector2(Global.Config.HistogramWidth, Global.Config.HistogramHeight);
            }
        }

        public void DetachDriver()
        {
            StopAllCoroutines();
            _trackingActive = false;

            if (_driver != null)
                _driver.Dispose();

            _moving = false;
            _delta = Vector3.zero;
            _driver = null;


            if (_chaser != null)
            {
                _chaser.gameObject.SetActive(false);
            }
        }

        public void Start()
        {
            if (rigidbody != null)
            {
                Destroy(rigidbody);
            }

            if (collider != null)
            {
                Destroy(collider);
            }

            _subject = transform;
            _driver = null;
            _controller = gameObject.GetComponent<CharacterController>();

            if (_chaser != null)
            {
                _chaser.gameObject.SetActive(false);
            }
        }

        private Vector3 Scale(Vector3 vector)
        {
            if (useDeltaTime)
            {
                // Not sure the delta makes sense here.  These are controlled by user anyway.
                vector.x *= Global.Config.ScaleX * Time.deltaTime * scaleX;
                vector.y *= Global.Config.ScaleY * Time.deltaTime * scaleY;
                vector.z *= Global.Config.ScaleZ * Time.deltaTime * scaleZ;
            }
            else
            {
                vector.x *= Global.Config.ScaleX * scaleX;
                vector.y *= Global.Config.ScaleY * scaleY;
                vector.z *= Global.Config.ScaleZ * scaleZ;
            }

            return vector;
        }

        private void MoveSubject()
        {
            var xzVector = _delta;
            xzVector.y = 0;

            _controller.Move(_subject.TransformDirection(xzVector));
            _controller.transform.rotation = _subject.rotation * Quaternion.Euler(new Vector3(0, _delta.y, 0));
			
            _delta = Vector3.zero;
        }

        public void OnGUI()
        {
            if (Global.Config.RenderHistogram)
            {
                foreach (var tracker in HistogramTrackers)
                {
                    tracker.HistogramRenderer.OnGUI();
                }
            }
        }

        public void OnApplicationQuit()
        {
            foreach (var tracker in HistogramTrackers)
            {
                tracker.HistogramRenderer.Save();
            }
        }

        public void Update()
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    if (_laserIsOn)
                    {
                        _driver.SendData("z0");
                        _laserIsOn = false;
                    }
                    else
                    {
                        _driver.SendData("z1");
                        _laserIsOn = true;
                    }
                }

                if (_driver == null || _moving)
                {
                    return;
                }

                if (_chaser != null)
                {
                    if (_chaser.gameObject.activeSelf == false)
                    {
                        _chaser.gameObject.SetActive(true);
                    }
                }

                if (!_trackingActive)
                {
                    _trackingActive = true;

                    StopAllCoroutines();
                    foreach (var tracker in HistogramTrackers)
                    {
                        StartCoroutine(tracker.StartTracking());
                        StartCoroutine(tracker.TrackUpdate());
                    }
                }

                var vectors = _driver.GetMovementVectors().ToList();

                // Evaluate the overall change in position since last read from driver.
                // Could try avoiding the aggregation, but might be expensive.
                var summedVector = vectors.Aggregate(Vector3.zero, (current, vector) => current + vector);

                if (keyboardControls)
                {
                    summedVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * 20;
                }

                if (summedVector == Vector3.zero)
                {
                    return;
                }

                _delta += Scale(summedVector);

                if (_delta == Vector3.zero)
                {
                    return;
                }

                MoveSubject();

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
