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
        private LineRenderer _lineRenderer;
        private List<Vector3> _lineSegments = new List<Vector3>();

        public void AttachDriver(UnityMovementDriver driver)
        {
            _driver = driver;
            _moving = false;
            _delta = Vector3.zero;
            if (_driver == null)
                throw new ArgumentNullException("driver");
        }

        public void DetachDriver()
        {
            _moving = false;
            _delta = Vector3.zero;
            _driver = null;
        }

        public void Start()
        {
            _subject = transform;
            _driver = null;
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private Vector3 Scale(Vector3 vector)
        {
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


        private void RenderVirtualPath(Vector3 oldPosition, List<Vector3> movementDeltas)
        {
            if (_lineRenderer == null) return;
            _lineSegments.AddRange(from delta in movementDeltas select (oldPosition += new Vector3(delta.x, 0, delta.z)));
            _lineRenderer.SetVertexCount(_lineSegments.Count);
            for (var i = 0; i < _lineSegments.Count; i++)
            {
                _lineRenderer.SetPosition(i, _lineSegments[i]);
            }
        }

        public void Update()
        {
            try
            {
                if (_driver == null)
                {
                    return;
                }
                var oldPosition = _subject.position;
                var vectors = _driver.GetVectors().ToList();
                // vectors.Add(new Vector3(0, 10, 0.25f));
                var summedVector = vectors.Aggregate(Vector3.zero, (current, vector) => current + vector);

                if (summedVector == Vector3.zero) return;

                _delta += Scale(summedVector);
                if (_moving || _delta == Vector3.zero)
                    return;

                StartCoroutine(Move());
    
                if (Global.Config.EnablePathRenderer)
                    RenderVirtualPath(oldPosition, vectors.ToList());
            }
            catch (Exception e)
            {
                Logger.Error("Something bad happened while trying to read movement data.", e);
            }
        }

    }

}
