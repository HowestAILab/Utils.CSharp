/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2012

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Windows.Forms;

namespace SizingServers.Util.WinForms {
    /// <summary>
    /// 
    /// </summary>
    public partial class TracertDialog : Form {
        /// <summary>
        /// Fires when the cancel button is pressed.
        /// </summary>
        public event EventHandler TraceRouteCancelled;
        /// <summary>
        /// Initiale calling SetStarted(). Finalize calling SetCompleted().
        /// </summary>
        public TracertDialog() { InitializeComponent(); }
        /// <summary>
        /// </summary>
        public void SetStarted() {
            btnCancelTraceRoute.Enabled = true;
            lvw.Items.Clear();
        }

        /// <summary>
        /// Add a hop to the list view.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="hostName"></param>
        /// <param name="roundtripTime">Already formatted</param>
        public void AddHop(string ip, string hostName, string roundtripTime) {
            btnCancelTraceRoute.Enabled = true;
            lvw.Items.Add(new ListViewItem(new[] { (lvw.Items.Count + 1).ToString(), ip, hostName, roundtripTime }));
        }
        /// <summary>
        /// </summary>
        public void SetCompleted() { btnCancelTraceRoute.Enabled = false; }

        private void btnCancelTraceRoute_Click(object sender, EventArgs e) {
            btnCancelTraceRoute.Enabled = false;
            if (TraceRouteCancelled != null)
                TraceRouteCancelled(this, null);
        }
    }
}