using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Histogram
{
    public class HistogramOrientationTracker : AbstractHistogramTracker
    {
        private List<float> _recordedDotProducts;
        private Transform _targetA;
        private Transform _targetB;
	
        public HistogramOrientationTracker (
            string path,
            Assets.VirtualProfiler.GlobalConfiguration config,
            Transform targetA, 
            Transform targetB)
            : base(path, config, new Color(0, 0, 1, 1))
        {
            _targetA = targetA;
            _targetB = targetB;	
            HistogramRenderer.FilePrefix = "ORIENTATION";
        }
	
        public override IEnumerator StartTracking ()
        {
            _recordedDotProducts = new List<float> ();
		
            int xPos = 0;
            while (true) {
                yield return new WaitForSeconds(_config.HistogramSampleRate);
			
                var totalSpeed = 0f;
                foreach (var velocity in _recordedDotProducts) {
                    totalSpeed += velocity;
                }
			
                var avgVelocity = totalSpeed / (float)_recordedDotProducts.Count;
                avgVelocity /= 2;
			
                _recordedDotProducts.Clear ();
			
                _histogramRenderer.PlotPoint (xPos++, (avgVelocity * _config.HistogramMaxY));
            }
        }
	
        public override IEnumerator TrackUpdate ()
        {
            while (true) {
			
                var dotProduct = Vector3.Dot(_targetA.forward, _targetB.forward) + 1;
			
                _recordedDotProducts.Add (dotProduct);
			
                yield return null;
            }
        }
    }
}
