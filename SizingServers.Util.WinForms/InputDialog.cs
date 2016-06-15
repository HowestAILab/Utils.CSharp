/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2008

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SizingServers.Util.WinForms {
    /// <summary>
    /// An implementation of the old VB6 input dialog.
    /// </summary>
    public partial class InputDialog : Form {

        #region Fields
        private DialogResult _cancel = DialogResult.Cancel;
        private MessageBoxButtons _messageBoxButtons = MessageBoxButtons.OKCancel;
        private int _minimumInputLength;
        private DialogResult _ok = DialogResult.OK;
        #endregion

        #region Properties

        /// <summary>
        /// The minimum allowed length in the textbox. Call SetInputLength(...) to set this.
        /// </summary>
        public int MinimumInputLength {   get { return _minimumInputLength; }    }

        /// <summary>
        /// The minimum allowed length in the textbox. Call SetInputLength(...) to set this.
        /// </summary>
        public int MaximumInputLength {  get { return txtInput.MaxLength; }   }

        /// <summary>
        /// Set or get the input text.
        /// </summary>
        public string Input {
            get { return txtInput.Text; }
            set { txtInput.Text = value; }
        }

        /// <summary>
        /// Default: MessageBoxButtons.OKCancel
        /// </summary>
        public MessageBoxButtons MessageBoxButtons {
            set {
                if (value != _messageBoxButtons) {
                    _messageBoxButtons = value;
                    switch (_messageBoxButtons) {
                        case MessageBoxButtons.OK:
                            btnOK.Text = "OK";
                            btnCancel.Text = "Cancel";
                            _ok = DialogResult.OK;
                            _cancel = DialogResult.Cancel;
                            btnCancel.Enabled = false;
                            break;
                        case MessageBoxButtons.OKCancel:
                            btnOK.Text = "OK";
                            btnCancel.Text = "Cancel";
                            _ok = DialogResult.OK;
                            _cancel = DialogResult.Cancel;
                            btnCancel.Enabled = true;
                            break;
                        case MessageBoxButtons.YesNo:
                        case MessageBoxButtons.YesNoCancel:
                            btnOK.Text = "Yes";
                            btnCancel.Text = "No";
                            _ok = DialogResult.Yes;
                            _cancel = DialogResult.No;
                            btnCancel.Enabled = true;
                            break;
                    }
                }
            }
            get { return _messageBoxButtons; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// An implementation of the old VB6 input dialog.
        /// </summary>
        /// <param name="question">The question asked to the user.</param>
        /// <param name="caption"></param>
        /// <param name="defaultInput">The default value for the input textbox.</param>
        public InputDialog(string question, string caption = "", string defaultInput = "") {
            InitializeComponent();
            lblQuestion.Text = question;
            Text = caption;
            txtInput.Text = defaultInput;
            if (IsHandleCreated)
                SetGui();
            else
                HandleCreated += InputDialog_HandleCreated;
        }

        #endregion

        #region Functions

        private void InputDialog_HandleCreated(object sender, EventArgs e) { SetGui(); }

        private void SetGui() {
            Graphics g = lblQuestion.CreateGraphics();
            int difference = txtInput.Height - (int)g.MeasureString(lblQuestion.Text, lblQuestion.Font).Height;
            if (Height - difference > MinimumSize.Height)
                Height -= difference;
        }

        /// <summary>
        /// Set the input length for the textbox.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetInputLength(int min, int max = int.MaxValue) {
            if (min > max)
                throw new ArgumentException("min cannot be larger than max.");
            _minimumInputLength = min;
            txtInput.MaxLength = max;

            btnOK.Enabled = txtInput.Text.Length >= _minimumInputLength;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            DialogResult = _ok;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = _cancel;
            Close();
        }

        private void txtInput_TextChanged(object sender, EventArgs e) {   btnOK.Enabled = txtInput.Text.Length >= _minimumInputLength;   }

        /// <summary></summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            txtInput.Focus();
        }

        #endregion
    }
}