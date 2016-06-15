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
    public partial class SynchronizationContextWrapperExample : Form {
        public SynchronizationContextWrapperExample() {
            InitializeComponent();
            this.HandleCreated += SynchronizationContextWrapperExample_HandleCreated;
        }

        private void SynchronizationContextWrapperExample_HandleCreated(object sender, EventArgs e) {
            this.HandleCreated -= SynchronizationContextWrapperExample_HandleCreated;
            SynchronizationContextWrapper.SynchronizationContext = SynchronizationContext.Current;
        }

        private void button1_Click(object sender, EventArgs e) {
            ThreadPool.QueueUserWorkItem((state) => {
                SynchronizationContextWrapper.SynchronizationContext.Send(o => button1.Text = "Updated!", null); //SynchronizationContext.Send is threadsafe.
            });
        }
    }
}
