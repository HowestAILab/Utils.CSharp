/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

namespace SizingServers.Util.Examples {
    public partial class FastObjectCreatorExample : Form {
        public FastObjectCreatorExample() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            MeasureObjectCreation(() => FastObjectCreator.CreateInstance<Control>(typeof(Control)));
        }

        private void button2_Click(object sender, EventArgs e) {
            MeasureObjectCreation(() => Activator.CreateInstance<Control>());
        }

        private void MeasureObjectCreation(Action creationMethod) {
            var sw = new Stopwatch();

            var millis = new long[10];

            for (int i = 0; i != millis.Length; i++) {
                sw.Start();
                for (int j = 0; j != 100000; j++) 
                    creationMethod.Invoke();
                
                sw.Stop();
                millis[i] = sw.ElapsedMilliseconds;

                sw.Reset();
            }

            lblElapsed.Text = millis.Average() + " ms";
        }
    }
}
