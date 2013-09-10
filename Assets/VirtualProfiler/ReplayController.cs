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
    }

    public class RealTimeReplayer : IReplay
    {
        private readonly LineRenderer _renderer;
        private readonly Stack<TimeVector> _inactiveVectors;
        private readonly List<TimeVector> _activeVectors;
        private readonly List<TimeVector> _allVectors;
        private readonly DateTime _runStartTime;
        private DateTime? _replayStartTime;
        private bool _isPaused;
        private Stopwatch _elapsed;

        public int SegmentIndex { get { return _activeVectors.Count(); } }
        public List<TimeVector> Vectors { get { return _allVectors; } }

        public void PlayPause()
        {
            _isPaused = !_isPaused;
            if (_isPaused) _elapsed.Stop();
            else _elapsed.Start();
        }

        public RealTimeReplayer(LineRenderer renderer, IEnumerable<TimeVector> vectors)
        {
            _renderer = renderer;
            _inactiveVectors = new Stack<TimeVector>(vectors.Reverse());
            _activeVectors = new List<TimeVector>();
            _allVectors = new List<TimeVector>(vectors);
            _runStartTime = _inactiveVectors.Peek().Time;
            _elapsed = new Stopwatch();
        }

        public void Render()
        {
            if (_isPaused) return;
            if (!_elapsed.IsRunning) _elapsed.Start();

            while ((_inactiveVectors.Count > 0) && ((_elapsed.Elapsed) >= (_inactiveVectors.Peek().Time - _runStartTime)))
            {
                _activeVectors.Add(_inactiveVectors.Pop());
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

        public int SegmentIndex { get { return _vectors.Count() - 1; } }
        public List<TimeVector> Vectors { get { return _vectors; } }

        public void PlayPause()
        { }

        public InstantReplayer(LineRenderer renderer, IEnumerable<TimeVector> vectors)
        {
            _renderer = renderer;
            _vectors = vectors.ToList();
        }

        public void Render()
        {
            _renderer.SetVertexCount(_vectors.Count());
            for (var i = 0; i < _vectors.Count(); i++)
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
