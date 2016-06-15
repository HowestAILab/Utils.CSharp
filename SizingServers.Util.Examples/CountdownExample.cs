/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Threading;
using System.Windows.Forms;

namespace SizingServers.Util.Examples {
    public partial class CountdownExample : Form {
        private Countdown _countdown = new Countdown();
        public CountdownExample() {
            InitializeComponent();
            _countdown.Tick += _countdown_Tick;
            _countdown.Stopped += _countdown_Stopped;
        }

        private void _countdown_Tick(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                button1.Text = (_countdown.CountdownTime / 1000).ToString();
            }, null);
        }

        private void _countdown_Stopped(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send((state) => {
                button1.Text = _countdown.CountdownTime == 0 ? "Happy New Year!" : "Countdown";
            }, null);
        }

        private void button1_Click(object sender, EventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
            if (_countdown.IsStarted) {
                _countdown.Stop();
            } else {
                button1.Text = "5";
                _countdown.Start(5000); //Countdown from 5 with a 1 second interval.
            }
        }
    }
}
