/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2013

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Windows.Forms;

namespace SizingServers.Util.WinForms {
    /// <summary>
    /// Draws a blank image for replacing the ugly, unneeded 'no image' image if no image is set.
    /// Put an instance of this in the CellTemplate of a DataGridViewImageColumn.
    /// </summary>
    public class DataGridViewImageCellBlank : DataGridViewImageCell {
        /// <summary>
        /// Draws a blank image for replacing the ugly, unneeded 'no image' image if no image is set.
        /// Put an instance of this in the CellTemplate of a DataGridViewImageColumn.
        /// </summary>
        public DataGridViewImageCellBlank() : base() { }
        /// <summary>
        /// Draws a blank image for replacing the ugly, unneeded 'no image' image if no image is set.
        /// Put an instance of this in the CellTemplate of a DataGridViewImageColumn.
        /// </summary>
        /// <param name="valueIsIcon"></param>
        public DataGridViewImageCellBlank(bool valueIsIcon) : base() { }
        /// <summary> </summary>
        public override object DefaultNewRowValue { get { return null; } }
    }
}
