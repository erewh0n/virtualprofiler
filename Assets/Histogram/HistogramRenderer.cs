using System;
using System.IO;
using UnityEngine;

namespace Assets.Histogram
{
    public class HistogramRenderer
    {
        public Vector2 RenderSize = new Vector2 (1024, 256);
        public string FilePrefix ="";
        private Texture2D _histogramBuffer;

        private int _histogramPart = 0;
        private string _path;
        private Assets.VirtualProfiler.GlobalConfiguration _config;
        private Color _color;
	
        public HistogramRenderer (string path, Assets.VirtualProfiler.GlobalConfiguration config, Color color)
        {
            _color = color;
            _config = config;
            _path = path + "/Histograms";
            if (!Directory.Exists (_path)) {
                Directory.CreateDirectory (_path);
            }
		
            CreateBuffer ();
        }
	
        public void Save ()
        {
            if (_histogramBuffer != null) {
                var fileName = _path + "/" + FilePrefix + "_" + DateTime.Now.ToString ("yyyyMMdd-HHmmss") + "_" + _histogramPart + ".png";
                var bytes = _histogramBuffer.EncodeToPNG ();
                File.WriteAllBytes (fileName, bytes);
                GameObject.Destroy (_histogramBuffer);			
            }
        }
	
        public void OnGUI ()
        {
            if (_histogramBuffer != null) {
				
                var oldColor = GUI.color;
                GUI.color = new Color(1, 1, 1, 0.25f);
                GUI.DrawTexture (new Rect (0, Screen.height - RenderSize.y, RenderSize.x, RenderSize.y), _histogramBuffer);
                GUI.color = oldColor;
            }
        }
	
        public void PlotPoint (float x, float y)
        {
            var range = _config.HistogramMaxY - _config.HistogramMinY;
            RenderLine (Mathf.FloorToInt (x), Mathf.CeilToInt (Mathf.Lerp (0, _histogramBuffer.height, y / range)));
        }
	
        private void RenderLine (int xPos, int height)
        {
            xPos -= _histogramBuffer.width * _histogramPart;
		
            if (xPos >= _histogramBuffer.width) {
                Save ();
                CreateBuffer ();
                _histogramPart++;
            }
		
            xPos = Mathf.Clamp (xPos, 0, _histogramBuffer.width);
            height = Mathf.Clamp (height, 0, _histogramBuffer.height);
		
            for (int i = 0; i < height; i ++) {
                _histogramBuffer.SetPixel (xPos, i, _color);
            }
		
            _histogramBuffer.Apply ();
        }
	
        private void CreateBuffer ()
        {
            _histogramBuffer = new Texture2D (1024, _config.HistogramGranularity, TextureFormat.ARGB32, false, false);
            _histogramBuffer.filterMode = FilterMode.Point;
		
            var totalPixels = 1024 * _config.HistogramGranularity;
            var colorBuffer = new Color[totalPixels];
            for(int i = 0; i < totalPixels; i++)
            {
                colorBuffer[i] = Color.clear;
            }
		
            _histogramBuffer.SetPixels(colorBuffer);
        }
			
    }
}
