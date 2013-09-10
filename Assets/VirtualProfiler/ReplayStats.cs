using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.VirtualProfiler
{
    public class ReplayStats
    {
        private readonly List<TimeVector> _vectors;
        private readonly IReplay _replayer;

        public ReplayStats(IReplay replayer)
        {
            _replayer = replayer;
            _vectors  = replayer.Vectors.ToList();

            TotalSegments = _vectors.Count();
            StartTime     = _vectors[0].Time;
            EndTime       = _vectors[TotalSegments - 1].Time;
            TotalTime     = EndTime - StartTime;

            Velocities = new List<double>();
            for (var i = 1; i < TotalSegments; i++)
            {
                Velocities.Add(_vectors[i].Vector.magnitude/((_vectors[i].Time - _vectors[i - 1].Time).TotalMilliseconds));
            }

            var sortedVelocities = Velocities.OrderBy(x => x);
            MaxVelocity = sortedVelocities.Last();
            MinVelocity = sortedVelocities.First();
        }

        public int          TotalSegments { get; protected set; }
        public DateTime     StartTime     { get; protected set; }
        public DateTime     EndTime       { get; protected set; }
        public TimeSpan     TotalTime     { get; protected set; }
        public List<double> Velocities    { get; protected set; }
        public double       MaxVelocity   { get; protected set; }
        public double       MinVelocity   { get; protected set; }

        public List<TimeVector> Vectors { get { return _vectors; } }
        public int      CurrentSegment  { get { return _replayer.SegmentIndex; } }
        public DateTime CurrentTime     { get { return _vectors[CurrentSegment].Time; } }
        public TimeSpan Elapsed         { get { return CurrentTime - StartTime; } }
        public double   CurrentVelocity { get { return Velocities[CurrentSegment]; } }

        public double TotalDistance
        {
            get { return _vectors.Aggregate(0f, (x, y) => y.Vector.magnitude); }
        }


    }
}