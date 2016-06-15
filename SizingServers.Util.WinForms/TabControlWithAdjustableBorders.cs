/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2011

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SizingServers.Util.WinForms {
    /// <summary>
    /// When using multiple (encapsulated) tab controls a form can look very messy. Removing borders makes the form look cleaner.
    /// </summary>
    public class TabControlWithAdjustableBorders : TabControl {

        private const int TCM_ADJUSTRECT = 0X1328;

        /// <summary> </summary>
        public bool BottomVisible { get; set; }
        /// <summary> </summary>
        public bool LeftVisible { get; set; }
        /// <summary> </summary>
        public bool RightVisible { get; set; }
        /// <summary> </summary>
        public bool TopVisible { get; set; }

        /// <summary></summary>
        protected override void WndProc(ref Message m) {
            try {
                if (m.Msg == TCM_ADJUSTRECT) {
                    var rc = (RECT)m.GetLParam(typeof(RECT));

                    if (!LeftVisible) rc.Left -= 4;
                    if (!RightVisible) rc.Right += 3;
                    if (!TopVisible) rc.Top -= 3;
                    if (!BottomVisible) rc.Bottom += 3;

                    Marshal.StructureToPtr(rc, m.LParam, true);
                }
                base.WndProc(ref m);
            } catch {
                //Whatever.
            }
        }

        private struct RECT { public int Left, Top, Right, Bottom;  }
    }
}