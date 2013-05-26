using System;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class Event
    {
        public float DeltaTime { get; private set; }
        public object Payload { get; private set; }

        public Event(float deltaTime, object payload)
        {
            DeltaTime = deltaTime;
            Payload = payload;
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}]{2}", DeltaTime, Payload, Environment.NewLine);
        }

        public static Event Parse(string data)
        {
            var result = data.Trim(new[] {'[', ']'}).Split(':');
            if (result.Length != 2)
                return null;
            try
            {
                var deltaTime = float.Parse(result[0]);
                return new Event(deltaTime, result[1]);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
