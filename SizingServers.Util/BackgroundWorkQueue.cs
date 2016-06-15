/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2010

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SizingServers.Util {
    /// <summary>
    /// To offload work to a background thread and keeping that work in a synchronized order.
    /// </summary>
    public sealed class BackgroundWorkQueue : IDisposable {
        /// <summary>
        /// Invoked if an enqueued work item (function) was invoked successfully or unsuccessfully.
        /// </summary>
        public event EventHandler<OnWorkItemProcessedEventArgs> OnWorkItemProcessed;

        #region Fields
        /// <summary>
        /// If this is set all items in the work queue will be processed, except if this processing is paused.
        /// </summary>
        private readonly AutoResetEvent _processWorkQueueWaitHandle = new AutoResetEvent(false);
        /// <summary>
        /// Used for pausing and continuing processing the work queue.
        /// </summary>
        private readonly ManualResetEvent _continueWaitHandle = new ManualResetEvent(true), _idleWaitHandle = new ManualResetEvent(true);

        private bool _isDisposed;
        private ConcurrentQueue<KeyValuePair<Delegate, object[]>> _workQueue = new ConcurrentQueue<KeyValuePair<Delegate, object[]>>();
        private Thread _processWorkQueueThread;

        private bool _pausedProcessingWorkQueue;
        #endregion

        /// <summary> </summary>
        public bool IsDisposed { get { return _isDisposed; } }

        /// <summary>
        /// To offload work to a background thread and keeping that work in a synchronized order.
        /// </summary>
        public BackgroundWorkQueue() {
            _processWorkQueueThread = new Thread(HandleWorkQueue);
            _processWorkQueueThread.IsBackground = true;
            _processWorkQueueThread.Start();
        }

        /// <summary></summary>
        ~BackgroundWorkQueue() { Dispose(); }

        #region Functions

        /// <summary>
        /// Releases all resources used.
        /// </summary>
        public void Dispose() { Dispose(0); }
        /// <summary>
        /// <para>Releases all resources used.</para>
        /// <para>Waits the given timeout before disposing (0 is acceptable, , smaller than 0 is indefinetly), if the work is not done it will be aborted.</para>
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public void Dispose(int millisecondsTimeout) {
            if (!_isDisposed) {
                _isDisposed = true;
                _processWorkQueueWaitHandle.Set();
                _continueWaitHandle.Set();
                _idleWaitHandle.Set();

                if (millisecondsTimeout > 0)
                    _processWorkQueueThread.Join(millisecondsTimeout);
                else if (millisecondsTimeout == -1)
                    _processWorkQueueThread.Join();

                _processWorkQueueWaitHandle.Dispose();
                _continueWaitHandle.Dispose();
                _idleWaitHandle.Dispose();

                try { _processWorkQueueThread.Abort(); } catch { }
                try { _processWorkQueueThread.Join(); } catch { }
                _processWorkQueueThread = null;
                _workQueue = null;
            }
        }

        /// <summary>
        /// <para>Pauses the processing of the work queue and blocks until the processing is effectively paused.</para>
        /// <para>Continue this processing calling 'ContinueProcessingWorkQueue()'.</para>
        /// </summary>
        public void PauseProcessingWorkQueue() {
            //Untested, but I see no errors in the code, so should work.
            if (!_pausedProcessingWorkQueue) {
                _continueWaitHandle.Reset();
                _idleWaitHandle.WaitOne();
                _pausedProcessingWorkQueue = true;
            }
        }
        /// <summary>
        /// Resumes the processing of the work queue.
        /// </summary>
        public void ContinueProcessingWorkQueue() {
            if (_pausedProcessingWorkQueue) {
                _pausedProcessingWorkQueue = false;
                _continueWaitHandle.Set();
            }
        }

        /// <summary>
        /// <para>Adds a delegate on the work queue, that work queue is processed by a background thread, threadsafe.</para>
        /// <para>The delegates on that queue are invoked in a synchronized order.</para>
        /// <para></para>
        /// <para>Define your delegate like so:</para>
        /// <para>  <code>delegate T del(T, out T);</code> (Action or Func is also possible.)</para>
        /// <para>then pass your function using this signature:</para>
        /// <para>  <code>BackgroundWorkQueue.Send(new del(function), value, value);</code></para>
        /// <para>The return type may be void and args are not obligatory.</para>
        /// <para>Make sure you provide the right amount of args, even if it are out parameters (yes that is possible too).</para>
        /// </summary>
        /// <param name="del"></param>
        /// <param name="parameters"></param>
        public void EnqueueWorkItem(Delegate del, params object[] parameters) {
            try {
                _workQueue.Enqueue(new KeyValuePair<Delegate, object[]>(del, parameters));
                _processWorkQueueWaitHandle.Set();
            } catch (ObjectDisposedException) {
                throw new ObjectDisposedException(this.ToString());
            }
        }
        private void HandleWorkQueue() {
            try {
                while (!_isDisposed) {
                    while (_workQueue.Count != 0) {
                        _continueWaitHandle.WaitOne();
                        _idleWaitHandle.Reset();

                        KeyValuePair<Delegate, object[]> kvp;
                        if (!_workQueue.TryDequeue(out kvp)) continue;

                        object returned = null;
                        Exception exception = null;
                        try {
                            returned = kvp.Key.DynamicInvoke(kvp.Value);
                        } catch (Exception ex) {
                            exception = ex;
                        } finally {
                            if (OnWorkItemProcessed != null) {
                                var invocationList = OnWorkItemProcessed.GetInvocationList();
                                Parallel.For(0, invocationList.Length, (i) => {
                                    (invocationList[i] as EventHandler<OnWorkItemProcessedEventArgs>).Invoke(this, new OnWorkItemProcessedEventArgs(kvp.Key, kvp.Value, returned, exception));
                                });
                            }
                        }
                        _idleWaitHandle.Set();
                    }
                    _processWorkQueueWaitHandle.WaitOne();
                }
            } catch {
                //Exception on dispose possible.
            }
        }

        /// <summary>
        /// Wait untill all work queue items are processed.
        /// </summary>
        public void Flush() {
            while (_workQueue.Count != 0)
                _idleWaitHandle.WaitOne();
        }
        #endregion

        #region EventArgs
        /// <summary>
        /// The result of a called function or the exception thrown.
        /// </summary>
        public class OnWorkItemProcessedEventArgs : EventArgs {
            /// <summary></summary>
            public readonly Delegate Delegate;
            /// <summary>
            ///     Out parameters are stored here too.
            /// </summary>
            public readonly object[] Parameters;
            /// <summary></summary>
            public readonly object ReturnValue;
            /// <summary></summary>
            public readonly Exception Exception;

            /// <summary>
            /// The result of a called function or the exception thrown.
            /// </summary>
            /// <param name="del"></param>
            /// <param name="parameters"></param>
            /// <param name="returnValue"></param>
            /// <param name="exception"></param>
            public OnWorkItemProcessedEventArgs(Delegate del, object[] parameters, object returnValue, Exception exception) {
                Delegate = del;
                Parameters = parameters;
                ReturnValue = returnValue;
                Exception = exception;
            }
        }
        #endregion
    }
}