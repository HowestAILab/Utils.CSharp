/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2015

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SizingServers.Util.WinForms {
    /// <summary>
    /// A GUI wrapper for FindAndReplace. 
    /// </summary>
    public partial class FindAndReplaceControl : UserControl {
        /// <summary>
        /// Occures when the find text is changed.
        /// </summary>
        [Category("FindAndReplace")]
        public event EventHandler FindTextChanged;
        /// <summary>
        /// Occures when the replace text is changed.
        /// </summary>
        [Category("FindAndReplace")]
        public event EventHandler ReplaceTextChanged;
        /// <summary>
        /// Occures when the find button is clicked.
        /// </summary>
        [Category("FindAndReplace")]
        public event EventHandler FindClicked;
        /// <summary>
        /// Occures when the replace button is clicked.
        /// </summary>
        [Category("FindAndReplace")]
        public event EventHandler ReplaceClicked;
        /// <summary>
        /// Occures when whole words is (un)checked.
        /// </summary>
        [Category("FindAndReplace")]
        public event EventHandler WholeWordsChanged;
        /// <summary>
        /// Occures when match case is (un)checked.
        /// </summary>
        [Category("FindAndReplace")]
        public event EventHandler MatchCaseChanged;
        /// <summary>
        /// Occures when replace all is (un)checked.
        /// </summary>
        [Category("FindAndReplace")]
        public event EventHandler ReplaceAllChanged;

        /// <summary>
        /// A GUI wrapper for FindAndReplace. 
        /// </summary>
        public FindAndReplaceControl() { InitializeComponent(); }

        /// <summary>
        /// </summary>
        [Category("FindAndReplace")]
        public string Find {
            get { return txtFind.Text; }
            set { txtFind.Text = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        [Category("FindAndReplace")]
        public bool FindWholeWords {
            get { return chkWholeWords.Checked; }
            set { chkWholeWords.Checked = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        [Category("FindAndReplace")]
        public bool FindMatchCase {
            get { return chkMatchCase.Checked; }
            set { chkMatchCase.Checked = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        [Category("FindAndReplace")]
        public string ReplaceWith {
            get { return txtReplace.Text; }
            set { txtReplace.Text = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        [Category("FindAndReplace")]
        public bool ReplaceAll {
            get { return chkReplaceAll.Checked; }
            set { chkReplaceAll.Checked = value; }
        }

        private void btnFind_Click(object sender, EventArgs e) {
            if (FindClicked != null)
                FindClicked(this, new EventArgs());
        }
        private void txtFind_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && btnFind.Enabled)
                btnFind.PerformClick();
        }

        private void chkWholeWords_CheckedChanged(object sender, EventArgs e) {
            if (WholeWordsChanged != null)
                WholeWordsChanged(this, new EventArgs());
        }
        private void chkMatchCase_CheckedChanged(object sender, EventArgs e) {
            if (MatchCaseChanged != null)
                MatchCaseChanged(this, new EventArgs());
        }
        private void chkReplaceAll_CheckedChanged(object sender, EventArgs e) {
            if (ReplaceAllChanged != null)
                ReplaceAllChanged(this, null);
        }

        private void btnReplaceWith_Click(object sender, EventArgs e) {
            if (ReplaceClicked != null)
                ReplaceClicked(this, new EventArgs());
        }
        private void txtFind_TextChanged(object sender, EventArgs e) {
            btnFind.Enabled = txtFind.Text.Length != 0;
            btnReplaceWith.Enabled = txtFind.Text.Length != 0 && txtFind.Text != txtReplace.Text;
            if (FindTextChanged != null)
                FindTextChanged(this, new EventArgs());
        }
        private void txtReplace_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && btnReplaceWith.Enabled)
                btnReplaceWith.PerformClick();
        }

        private void txtReplace_TextChanged(object sender, EventArgs e) {
            btnReplaceWith.Enabled = txtFind.Text.Length != 0 && txtFind.Text != txtReplace.Text;
            if (ReplaceTextChanged != null)
                ReplaceTextChanged(this, new EventArgs());
        }
    }
}
