/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2010

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace SizingServers.Util.WinForms {
    /// <summary>
    /// A simple control to vizualize key value pairs. 
    /// </summary>
    public partial class KeyValuePairControl : UserControl {
        #region Properties
        /// <summary>
        /// </summary>
        public string Key {
            get { return lblKey.Text; }
            set { lblKey.Text = value; }
        }
        /// <summary>
        /// </summary>
        public string Value {
            get { return lblValue.Text; }
            set { lblValue.Text = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string Tooltip {
            set {
                value = value.Replace("\\n", "\n").Replace("\\r", "\r");
                toolTip.SetToolTip(this, value);
                toolTip.SetToolTip(lblKey, value);
                toolTip.SetToolTip(lblValue, value);
            }
            get { return toolTip.GetToolTip(this); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// A simple control to vizualize key value pairs. 
        /// </summary>
        public KeyValuePairControl() {
            InitializeComponent();
        }
        /// <summary>
        /// A simple control to vizualize key value pairs. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public KeyValuePairControl(string key, string value)
            : this() {
            lblKey.Text = key;
            lblValue.Text = value;
        }
        #endregion

        #region Functions
        private void lbl_SizeChanged(object sender, EventArgs e) {
            SizeChanged -= KeyValuePairControl_SizeChanged;

            lblValue.Left = lblKey.Right;
            Width = lblValue.Right + 3;

            if (lblKey.Height > Height) Height = lblKey.Height + 3;

            KeyValuePairControl_SizeChanged(null, null);

            SizeChanged += KeyValuePairControl_SizeChanged;
        }

        private void KeyValuePairControl_SizeChanged(object sender, EventArgs e) {
            lblValue.SizeChanged -= lbl_SizeChanged;
            if (MaximumSize.Width == 0) {
                lblValue.MaximumSize = new Size(int.MaxValue, lblValue.Height);
            } else {
                int difference = lblValue.Right - Width;
                int newWidth = lblValue.Width - difference;
                lblValue.MaximumSize = new Size(newWidth > 0 ? newWidth : int.MaxValue, lblValue.Height);
            }
            lblValue.SizeChanged += lbl_SizeChanged;
        }

        private void lbl_MouseDown(object sender, MouseEventArgs e) {
            OnMouseDown(e);
        }
        #endregion
    }
}