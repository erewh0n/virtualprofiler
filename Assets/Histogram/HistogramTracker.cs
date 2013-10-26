using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Histogram
{
    public class HistogramTracker : AbstractHistogramTracker
    {
        private Vector3 _lastPos;
        private List<Vector3> _recordedVelocities;
        private Transform _target;
	
        public HistogramTracker(Transform target, string path, Assets.VirtualProfiler.GlobalConfiguration config) 
            : base(path, config, new Color(1, 0, 0, 1))
        {		
            _target = target;
            HistogramRenderer.FilePrefix = "VELOCITY";
        }
	
        public override IEnumerator StartTracking ()
        {
            _recordedVelocities = new List<Vector3> ();
            _lastPos = _target.position;
		
            int xPos = 0;
            while (true) 
            {
                yield return new WaitForSeconds(_config.HistogramSampleRate);
			
                var totalSpeed = Vector3.zero;
                foreach (var velocity in _recordedVelocities) {
                    totalSpeed += velocity;
                }
			
                var avgVelocity = totalSpeed / (float)_recordedVelocities.Count;
			
                _recordedVelocities.Clear();
			
                _histogramRenderer.PlotPoint (xPos++, avgVelocity.magnitude);
            }
        }
	
        public override IEnumerator TrackUpdate ()
        {
            while (true) {
                var delta = _target.position - _lastPos;
                _lastPos = _target.position;
			
                _recordedVelocities.Add (delta);
			
                yield return null;
            }
        }
    }
}
