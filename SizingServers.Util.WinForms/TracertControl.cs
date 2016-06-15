/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2012

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SizingServers.Util.WinForms {
    /// <summary>
    /// <para>A control around Tracert --> Determines the trace route for a given destination host or IP.</para>
    /// <para>Suscribe to BeforeTrace and call SetToTrace(...).</para>
    /// </summary>
    public partial class TracertControl : UserControl {
        /// <summary>
        /// You can call SetToTrace here last minute.
        /// </summary>
        public event EventHandler BeforeTrace;
        /// <summary>
        /// </summary>
        public event EventHandler Done;

        #region Fields
        private readonly TracertDialog _tracertDialog = new TracertDialog();

        /// <summary>
        /// The host name or IP to trace route.
        /// </summary>
        public string HostNameOrIP { get; private set; }
        /// <summary>
        /// The IP status for the last hop.
        /// </summary>
        public IPStatus LastStatus { get { return _tracert == null ? IPStatus.Unknown : _tracert.LastStatus; } }
        /// <summary>
        /// The maximum hops that may occur.
        /// </summary>
        public int MaxHops { get; private set; }
        /// <summary>
        /// The ping timeout for a hop in ms.
        /// </summary>
        public int Timeout { get; private set; }
        /// <summary>
        /// The numbers of hops done.
        /// </summary>
        public int Hops { get { return _tracert == null ? 0 : _tracert.Hops; } }
        /// <summary>
        /// </summary>
        public bool IsDone { get { return _tracert == null ? false : _tracert.IsDone; } }

        private Tracert _tracert;
        #endregion

        #region Constructor
        /// <summary>
        /// <para>A control around Tracert --> Determines the trace route for a given destination host or IP.</para>
        /// <para>Suscribe to BeforeTrace and call SetToTrace(...).</para>
        /// </summary>
        public TracertControl() {
            InitializeComponent();
            _tracertDialog.TraceRouteCancelled += _tracertDialog_CancelTraceRoute;
        }
        #endregion

        #region Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostNameOrIP"></param>
        /// <param name="maxHops"></param>
        /// <param name="timeout"></param>
        public void SetToTrace(string hostNameOrIP, int maxHops = 100, int timeout = 5000) {
            HostNameOrIP = hostNameOrIP;
            MaxHops = maxHops;
            Timeout = timeout;
        }

        private void _tracertDialog_CancelTraceRoute(object sender, EventArgs e) {
            btnStatus.Text = "Idle";
            btnStatus.BackColor = Color.Transparent;
            btnTraceRoute.Enabled = true;

            _tracert.Dispose();
            _tracert = null;

            if (Done != null)
                Done(this, null);
        }

        private void Trace() {
            kvpHops.Key = "0 hops";
            kvpRoundtripTime.Key = "? roundtrip time";
            btnStatus.Text = "Tracing...";
            btnStatus.BackColor = Color.LightBlue;

            _tracert = new Tracert();
            _tracert.Hop += _tracert_Hop;
            _tracert.Done += _tracert_Done;

            _tracert.Trace(HostNameOrIP, MaxHops, Timeout);
        }

        private void btnTraceRoute_MouseDown(object sender, MouseEventArgs e) {
            if (BeforeTrace != null)
                BeforeTrace(this, null);
        }

        private void btnTraceRoute_Click(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;

            _tracertDialog.SetStarted();
            btnTraceRoute.Enabled = false;
            Trace();

            _tracertDialog.ShowDialog();
        }

        private void _tracert_Hop(object sender, Tracert.HopEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                kvpHops.Key = (Hops == 1 ? "1 hop" : Hops + " hops");
                string roundtripTime = "N/A";
                if (e.Status == IPStatus.Success) {
                    var ts = TimeSpan.FromMilliseconds(e.RoundTripTime);
                    roundtripTime = ToShortFormattedString(ts, "N/A");
                    kvpRoundtripTime.Key = roundtripTime + " roundtrip time";
                } else {
                    kvpRoundtripTime.Key = "? roundtrip time";
                }

                _tracertDialog.AddHop(e.IP, e.HostName, roundtripTime);
            }, null);
        }

        private string ToShortFormattedString(TimeSpan timeSpan, string returnOnZero = "--") {
            if (timeSpan.TotalMilliseconds == 0d) return returnOnZero;

            var sb = new StringBuilder();
            if (timeSpan.Days != 0) {
                sb.Append(timeSpan.Days);
                sb.Append(" d");
            }
            if (timeSpan.Hours != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Hours);
                sb.Append(" h");
            }
            if (timeSpan.Minutes != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Minutes);
                sb.Append(" m");
            }
            if (timeSpan.Seconds != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Seconds);
                sb.Append(" s");
            }
            if (timeSpan.Milliseconds != 0) {
                if (sb.ToString().Length != 0) sb.Append(", ");
                sb.Append(timeSpan.Milliseconds);
                sb.Append(" ms");
            }
            if (sb.ToString().Length == 0)
                return returnOnZero;

            return sb.ToString();
        }

        private void _tracert_Done(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(delegate {
                if (LastStatus == IPStatus.Success) {
                    btnStatus.Text = "Success...";
                    btnStatus.BackColor = Color.LightGreen;
                } else {
                    btnStatus.Text = "Failed...";
                    btnStatus.BackColor = Color.Orange;
                }
                btnTraceRoute.Enabled = true;

                _tracert.Dispose();
                _tracert = null;

                _tracertDialog.SetCompleted();

                if (Done != null)
                    Done(this, null);
            }, null);
        }

        private void btnStatus_Click(object sender, EventArgs e) {
            _tracertDialog.ShowDialog();
        }
        #endregion
    }
}