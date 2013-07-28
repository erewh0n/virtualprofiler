using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class ReplayAdapter : MonoBehaviour
    {
        private LineRenderer _renderer;
        private bool _isRealTime;
        private TimeVector[] _vectors;
        private int _index;
        private bool _isStarted;

        public void Start()
        {
            _renderer = transform.renderer as LineRenderer;
            if (_renderer == null)
                throw new ArgumentException("In order for replay to work properly a LineRenderer component must be attached to the game object!");
            _index = 0;
            _isStarted = false;
        }

        public void StartReplay(string replayFile, bool isRealTime)
        {
            Logger.Debug("Start replay");
            var positions = File.ReadAllLines(replayFile);
            _isRealTime = isRealTime;
            _vectors = (from position in positions
                        let coordinates = position.Split(',')
                        where coordinates.Length == 5
                        select
                            new TimeVector
                                {
                                    Vector = new Vector3(float.Parse(coordinates[1]), float.Parse(coordinates[2]),
                                                         float.Parse(coordinates[3])),
                                    Time = float.Parse(coordinates[0]),
                                }).ToArray();

            _isStarted = true;
            Logger.Debug("Replay started.");
            RenderPath();
        }

        public void RenderPath()
        {
            if (!_isRealTime)
            {
                Logger.Debug("Rendering: " + _vectors.Count());
                _renderer.SetVertexCount(_vectors.Count());

                for (var i = 0; i < _vectors.Count(); i++)
                {
                    _renderer.SetPosition(i, _vectors[i].Vector);
                }
            }
        }

        public void StopReplay()
        {
            _renderer.SetVertexCount(0);
            _isStarted = false;
        }

        public void Update()
        {
            if (!_isRealTime || !_isStarted)
                return;

            while ((_index < _vectors.Length) && (Time.time > _vectors[_index++].Time))
            {
            }

            _renderer.SetVertexCount(_index);
            for (var i = 0; i < _index; i++)
            {
                _renderer.SetPosition(i, _vectors[i].Vector);
            }
        }

        public float PercentDone()
        {
            if (!_isRealTime)
                return 100;

            return ((float) _index) / _vectors.Length * 100.0f;
        }
    }

}