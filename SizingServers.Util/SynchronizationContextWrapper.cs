/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2009

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Threading;

namespace SizingServers.Util {
    /// <summary>
    /// <para>A wrapper for SynchronizationContext, so SynchronizationContext is available throughout the entire application.</para>
    /// <para>A SynchronizationContext is for synchronizing data to another thread, for instance to the main thread.</para>
    /// <para>Handy if you want to update a System.Windows.Forms.Label from another thread. It is the most reliable way to do something like that IMHO.</para>
    /// </summary>
    public static class SynchronizationContextWrapper {
        private static SynchronizationContext _synchronizationContext;

        /// <summary>
        /// <para>Set this to System.Threading.SynchronizationContext.Current in the constructor below 'InitializeComponent();' of a WPF main form or when the handle is created for a WinForms main form.</para>
        /// <para>e.g. If you want to update a System.Windows.Forms.Label form another thread: SynchronizationContextWrapper.SynchronizationContext.Current.Send((state) => { lblFoo.Text = "bar"; }, null)</para>
        /// </summary>
        public static SynchronizationContext SynchronizationContext {
            get {
                return _synchronizationContext;
            }
            set {
                if (value == null) throw new NullReferenceException("SynchronizationContext");
                _synchronizationContext = value;
            }
        }
    }
}