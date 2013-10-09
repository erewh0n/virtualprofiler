using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Assets.VirtualProfiler
{

    public interface IReplay
    {
        void Render();
        int SegmentIndex { get; }
        List<TimeVector> Vectors { get; }
        void PlayPause();
        void SetPosition(int segmentIndex);
    }

    public class RealTimeReplayer : IReplay
    {
        private readonly LineRenderer _renderer;
        private readonly List<TimeVector> _vectors;
		private readonly List<TimeVector> _activeVectors;
        private int _index;
        private readonly DateTime _runStartTime;
        private bool _isPaused;
        private Stopwatch _elapsed;

        public int SegmentIndex { get { return _index >= _vectors.Count ? _vectors.Count - 1 : _index; } }
        public List<TimeVector> Vectors { get { return _vectors; } }

        public void PlayPause()
        {
            _isPaused = !_isPaused;
            if (_isPaused) _elapsed.Stop();
            else _elapsed.Start();
        }

        public void SetPosition(int segmentIndex)
        {
            _index = segmentIndex;
            if (_index >= _vectors.Count) _index = _vectors.Count - 1;
            if (_index < 0) _index = 0;
        }

        public RealTimeReplayer(LineRenderer renderer, IEnumerable<TimeVector> vectors)
        {
            _renderer = renderer;
            _index = 0;
            _vectors = new List<TimeVector>(vectors);
			_activeVectors = new List<TimeVector>();
            _runStartTime = _vectors[_index].Time;
            _elapsed = new Stopwatch();
        }

        public void Render()
        {
            if (_isPaused) return;
            if (!_elapsed.IsRunning) _elapsed.Start();

            while ((_index < _vectors.Count) && ((_elapsed.Elapsed) >= (_vectors[_index].Time - _runStartTime)))
            {
                _activeVectors.Add(_vectors[_index++]);
            }

            _renderer.SetVertexCount(_activeVectors.Count());
            for (var i = 0; i < _activeVectors.Count(); i++)
            {
                _renderer.SetPosition(i, _activeVectors[i].Vector);
            }
        }
    }

    public class InstantReplayer : IReplay
    {
        private readonly LineRenderer _renderer;
        private readonly List<TimeVector> _vectors;
        private int _index;

        public int SegmentIndex { get { return _index; } }
        public List<TimeVector> Vectors { get { return _vectors; } }

        public void PlayPause()
        { }

        public void SetPosition(int index)
        {
            _index = index;
            if (_index >= _vectors.Count) _index = _vectors.Count - 1;
            if (_index < 0) _index = 0;
        }

        public InstantReplayer(LineRenderer renderer, IEnumerable<TimeVector> vectors)
        {
            _renderer = renderer;
            _vectors = vectors.ToList();
            _index = _vectors.Count() - 1;
        }

        public void Render()
        {
            _renderer.SetVertexCount(_index);
            for (var i = 0; i < _index; i++)
            {
                _renderer.SetPosition(i, _vectors[i].Vector);
            }
        }

    }

    public class ReplayController : MonoBehaviour
    {
        private IReplay _replayer;
        private bool _isStarted;

        public LineRenderer LineRenderer { get { return transform.renderer as LineRenderer; } }

        public void Start()
        {
            if (LineRenderer == null)
                throw new ArgumentException("In order for replay to work properly a LineRenderer component must be attached to the game object!");
        }

        public void StartReplay(IReplay replayer)
        {
            _replayer = replayer;
            _isStarted = true;
        }

        public void StopReplay()
        {
            LineRenderer.SetVertexCount(0);
            _isStarted = false;
        }

        public void Update()
        {
            if (!_isStarted) return;

            _replayer.Render();
        }

    }

}
