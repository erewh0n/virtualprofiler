using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Histogram
{
    public class HistogramDistanceTracker : AbstractHistogramTracker
    {
        private List<float> _recordedDistances;
        private Transform _targetA;
        private Transform _targetB;
	
        public HistogramDistanceTracker (
            string path, 
            Assets.VirtualProfiler.GlobalConfiguration config, 
            Transform targetA, 
            Transform targetB)
            : base(path, config, new Color(0, 1, 0, 1))
        {
            _targetA = targetA;
            _targetB = targetB;
            HistogramRenderer.FilePrefix = "DISTANCE";
        }

        public override IEnumerator StartTracking ()
        {
            _recordedDistances = new List<float> ();

            int xPos = 0;
            while (true) {
                yield return new WaitForSeconds(_config.HistogramSampleRate);

                var totalSpeed = 0f;
                foreach (var velocity in _recordedDistances) {
                    totalSpeed += velocity;
                }

                var avgVelocity = totalSpeed / (float)_recordedDistances.Count;

                _recordedDistances.Clear ();

                _histogramRenderer.PlotPoint (xPos++, avgVelocity);
            }
        }

        public override IEnumerator TrackUpdate ()
        {
            while (true) {

                var distance = Vector3.Distance(_targetA.position, _targetB.position) / 10f;

                _recordedDistances.Add (distance);

                yield return null;
            }
        }
    }
}
