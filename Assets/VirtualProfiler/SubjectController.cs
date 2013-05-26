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

        public void Start()
        {
            _delta = Vector3.zero;
            _subject = transform;
            _driver = new UnityMovementDriver(new ReplayAdapter(GC.Instance.ReplayFile), new EventLogger(GC.Instance.MovementLogFile));
            _moving = false;
        }

        public void OnApplicationQuit()
        {
            _driver.Dispose();
        }

        private IEnumerator Move()
        {
            var timeScale = GC.Instance.UniversalScaling * Time.deltaTime;

            _moving = true;
            var xzVector = _delta;
            xzVector.y = 0;
            var curPos = _subject.transform.position;
            var newPos = curPos + (xzVector * timeScale);
            for (var t = 0f; t < 1; t += (Time.fixedDeltaTime / 0.5f))
            {
                _subject.transform.position = Vector3.Lerp(curPos, newPos, t);
                yield return 0;
            }

            _delta = Vector3.zero;
            _moving = false;
        }

        public void Update()
        {
            try
            {
                Debug.Log(string.Format("Time: {0}", Time.time));
                var vectors = _driver.GetVectors().ToList();
                var summedVector = vectors.Aggregate(Vector3.zero, (current, vector) => current + vector);
                _delta += summedVector;

                if (_moving || _delta == Vector3.zero)
                    return;
                StartCoroutine(Move());

            }
            catch (Exception e)
            {
                Debug.LogError("Something bad happened while trying to read movement data.");
                Debug.LogException(e);
            }
        }

    }

}
