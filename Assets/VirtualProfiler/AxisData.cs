using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class AxisData
    {
        public int[] Axes { get; private set; }
        public string RawInput { get; private set; }
        public static string Boundary { get { return "e"; } }

        protected AxisData(string rawInput)
        {
            Axes = (from axisDelta in rawInput.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries)
                    select int.Parse(Regex.Replace(axisDelta, "[a-zA-Z]*", "")))
                .ToArray();
            if (Axes.Length != 6)
                throw new ArgumentOutOfRangeException(string.Format("Failed to parse the movement data '{0}'.", rawInput));
            RawInput = rawInput;
        }

        public static List<AxisData> Create(string rawInput)
        {
            var deltas = rawInput.Split(new [] {'e'}, StringSplitOptions.RemoveEmptyEntries);

            var axisDataItems = new List<AxisData>();
            foreach(var delta in deltas)
            {
                try
                {
                    axisDataItems.Add(new AxisData(delta));
                }
                catch
                {
                    // Ignore.  Try to recover rather than fail hard.
                    Debug.Log(string.Format("Failed to parse delta line {0}", delta));
                }
            }

            return axisDataItems;
        }

        public Vector3 ToVector()
        {
            return new Vector3(Axes[2]-Axes[3], Axes[4]-Axes[5], Axes[0]-Axes[1]);
        }

        public override string ToString()
        {
            return RawInput;
        }
    }
}