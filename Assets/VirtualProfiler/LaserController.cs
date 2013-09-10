using System.Diagnostics;
using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class LaserController : MonoBehaviour
    {
        private Stopwatch _timer = new Stopwatch();
        private bool _isOn;

        public int MillisecondsOn = 500;
        public int MillisecondsOff = 500;

        public void OnTriggerEnter()
        {
            var serialPortAdapter = Global.Launcher.SerialPortAdapter;
            if (serialPortAdapter == null)
            {
                Logger.Debug("Trigger failed: could not create a connection to the laser!");
                return;
            }

            _isOn = true;
            serialPortAdapter.Write("z1");
            _timer.Reset();
            _timer.Start();
        }

        public void OnTriggerStay()
        {
            var pulseTime = _isOn ? MillisecondsOn : MillisecondsOff;

            if (_timer.ElapsedMilliseconds >= pulseTime)
            {
                _isOn = !_isOn;
                _timer.Reset();
                _timer.Start();
            }
        }

        public void OnTriggerExit()
        {
            var serialPortAdapter = Global.Launcher.SerialPortAdapter;
            if (serialPortAdapter == null)
            {
                Logger.Debug("Trigger failed: could not create a connection to the laser!");
                return;
            }

            serialPortAdapter.Write("z0");
        }

    }
}
