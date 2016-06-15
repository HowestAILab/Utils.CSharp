/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2015

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Windows.Forms;

namespace SizingServers.Util.Examples {
    public partial class FindAndReplaceExample : Form {
        FindAndReplaceHelperObject _findAndReplaceHelperObject = new FindAndReplaceHelperObject();

        public FindAndReplaceExample() {
            InitializeComponent();
        }

        private void Find(string find) {
            int row, column, start, matchLength;
            _findAndReplaceHelperObject.FindNext(find, richTextBox1.Text, out row, out column, out start, out matchLength, findAndReplaceControl1.FindWholeWords, findAndReplaceControl1.FindMatchCase);

            if (row != -1) {
                richTextBox1.Focus();
                richTextBox1.Select(start, matchLength);
            }
        }

        private void Replace() {
            Find(findAndReplaceControl1.Find);
            richTextBox1.Text = _findAndReplaceHelperObject.Replace(findAndReplaceControl1.ReplaceWith, findAndReplaceControl1.ReplaceAll);            
            Find(findAndReplaceControl1.ReplaceWith);
        }

        private void findAndReplaceControl1_FindClicked(object sender, EventArgs e) { Find(findAndReplaceControl1.Find); }
        private void findAndReplaceControl1_ReplaceClicked(object sender, EventArgs e) { Replace(); }
    }
}
