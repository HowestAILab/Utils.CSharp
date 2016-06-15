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
    public partial class BackgroundWorkQueueExample : Form {
        private BackgroundWorkQueue _backgroundWorkQueue = new BackgroundWorkQueue();
        private int _jobsProcessed;
        public BackgroundWorkQueueExample() {
            InitializeComponent();
            this.HandleCreated += SynchronizationContextWrapperExample_HandleCreated;
            _backgroundWorkQueue.OnWorkItemProcessed += _backgroundWorkQueue_OnWorkItemProcessed;
        }

        private void SynchronizationContextWrapperExample_HandleCreated(object sender, EventArgs e) {
            this.HandleCreated -= SynchronizationContextWrapperExample_HandleCreated;
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
        }

        private void button1_Click(object sender, EventArgs e) {
            _backgroundWorkQueue.EnqueueWorkItem(new Action(() => Thread.Sleep(2000)));
        }

        private void _backgroundWorkQueue_OnWorkItemProcessed(object sender, BackgroundWorkQueue.OnWorkItemProcessedEventArgs e) {
            SynchronizationContextWrapper.SynchronizationContext.Send(o => label1.Text = (++_jobsProcessed) + " jobs done.", null);
        }
    }
}
