using System.Collections;
using UnityEngine;

namespace Assets.Histogram
{
    public abstract class AbstractHistogramTracker
    {
        protected Assets.VirtualProfiler.GlobalConfiguration _config;
        protected HistogramRenderer _histogramRenderer;

        public HistogramRenderer HistogramRenderer {
            get { return _histogramRenderer; }
        }
	
        public AbstractHistogramTracker (string path, Assets.VirtualProfiler.GlobalConfiguration config, Color color)
        {
            _config = config;
            _histogramRenderer = new HistogramRenderer (path, config, color);		
        }
		
        public void Save ()
        {
            _histogramRenderer.Save ();
        }
	
        public abstract IEnumerator StartTracking ();
	
        public abstract IEnumerator TrackUpdate ();
    }
}
