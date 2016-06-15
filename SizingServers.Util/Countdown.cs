/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2012

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Threading.Tasks;
using System.Timers;

namespace SizingServers.Util {
    /// <summary>
    /// A countdown class, can report progress and when the countdown has become 0.
    /// </summary>
    public class Countdown : IDisposable {

        /// <summary>
        /// Occurs when started;
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when stopped;
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Occurs when the time period has elapsed. Get the current countdown from the Countdown field.
        /// </summary>
        public event EventHandler Tick;


        private bool _isdisposed;
        private Timer _tmr;

        /// <summary>
        /// In ms.
        /// </summary>
        public int CountdownTime { get; private set; }

        /// <summary>
        /// In ms.
        /// </summary>
        public int ReportProgressTime { get; private set; }

        /// <summary></summary>
        public bool IsStarted { get { return _tmr != null; } }

        /// <summary>
        /// A countdown class, can report progress and when the countdown has become 0.
        /// </summary>
        public Countdown() {
        }

        /// <summary>
        /// Start or restarts the countdown.
        /// </summary>
        /// <param name="countdownTime">In ms.</param>
        /// <param name="reportProgressTime">In ms. Min 100</param>
        public void Start(int countdownTime, int reportProgressTime = 1000) {
            CountdownTime = countdownTime;
            ReportProgressTime = reportProgressTime;

            if (_tmr != null) Stop();

            _tmr = new Timer(reportProgressTime);
            _tmr.Elapsed += _tmr_Elapsed;
            _tmr.Start();

            if (Started != null) {
                var invocationList = Started.GetInvocationList();
                Parallel.For(0, invocationList.Length, (i) => { (invocationList[i] as EventHandler).Invoke(this, null); });
            }
        }

        private void _tmr_Elapsed(object sender, ElapsedEventArgs e) {
            CountdownTime -= ReportProgressTime;
            if (CountdownTime <= 0) {
                _tmr.Stop();
                CountdownTime = 0;
            }

            if (Tick != null) {
                var invocationList = Tick.GetInvocationList();
                Parallel.For(0, invocationList.Length, (i) => { (invocationList[i] as EventHandler).Invoke(this, e); });
            }

            if (CountdownTime == 0) Stop();
        }

        /// <summary></summary>
        public void Stop() {
            if (_tmr != null) {
                _tmr.Stop();
                _tmr.Dispose();
                _tmr = null;

                if (Stopped != null) {
                    var invocationList = Stopped.GetInvocationList();
                    Parallel.For(0, invocationList.Length, (i) => { (invocationList[i] as EventHandler).Invoke(this, null); });
                }
            }
        }

        /// <summary></summary>
        public void Dispose() {
            if (!_isdisposed) {
                _isdisposed = true;
                if (_tmr != null) {
                    _tmr.Dispose();
                    _tmr = null;
                }
            }
        }
    }
}