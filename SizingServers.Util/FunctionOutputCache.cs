/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2014

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace SizingServers.Util {
    /// <summary>
    /// <para>A thread-safe way to cache the calling function's return value / output arguments.</para>
    /// <para>This should only be used for output that always stays the same, but takes long to calculate.</para>
    /// <para>You can use the instance if you want one general FunctionOutputCache.</para>
    /// <para>I made this class before I knew you can do more or less the same with System.Web.Caching.Cache (HttpContext.Current.Cache).</para>
    /// </summary>
    public class FunctionOutputCache : IDisposable {
        private static FunctionOutputCache _instance;

        private bool _isDisposed;
        private ConcurrentBag<CacheEntry> _cache = new ConcurrentBag<CacheEntry>();

        /// <summary>
        /// 
        /// </summary>
        public bool IsDisposed { get { return _isDisposed; } }

        /// <summary>
        /// The current size of the cache.
        /// </summary>
        public int Size { get { return _cache.Count; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FunctionOutputCache GetInstance() {
            if (_instance == null || _instance.IsDisposed) _instance = new FunctionOutputCache();
            return _instance;
        }

        /// <summary>
        /// <para>Returns the entry if it was found in cache. Otherwise a new one is added to the cache and returned.</para>
        /// <para>Store the return value and/or output arguments for the calling function in that new entry.</para>
        /// <para>This is thread-safe.</para>
        /// </summary>
        /// <param name="method">e.g. MethodInfo.GetCurrentMethod()</param>
        /// <param name="inputArguments">
        /// <para>Primitives, objects that are not a collection of any sort, arrays and ILists are supported. Array / IList members are compared for equality.</para>
        /// <para>Override InspectArgumentEquality(object, object) if you want to for instance support dictionaries.</para>
        /// </param>
        /// <returns></returns>
        public CacheEntry GetOrAdd(MethodBase method, params object[] inputArguments) {
            foreach (var entry in _cache)
                if (entry.Method == method && InspectArgumentEquality(entry.InputArguments, inputArguments))
                    return entry;

            var newEntry = new CacheEntry(method, inputArguments);
            _cache.Add(newEntry);
            return newEntry;
        }
        /// <summary>
        /// <para>Primitives, objects that are not a collection of any sort, arrays and ILists are supported. Array / IList members are compared for equality.</para>
        /// <para>Override this if you want to for instance support dictionaries.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual bool InspectArgumentEquality(object x, object y) {
            if ((x == null && y != null) || (x != null && y == null)) return false;
            if ((x == null && y == null) || x.Equals(y)) return true;
            if (x is Array && y is Array) {
                var xArr = x as Array;
                var yArr = y as Array;

                if (xArr.Length == yArr.Length) {
                    for (int i = 0; i != xArr.Length; i++)
                        if (!InspectArgumentEquality(xArr.GetValue(i), yArr.GetValue(i)))
                            return false;

                    return true;
                }
                return false;
            }
            if (x is IList && y is IList) {
                var xL = x as IList;
                var yL = y as IList;

                if (xL.Count == yL.Count) {
                    for (int i = 0; i != xL.Count; i++)
                        if (!InspectArgumentEquality(xL[i], yL[i]))
                            return false;

                    return true;
                }
                return false;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;
                _cache = null;
            }
        }
        /// <summary>
        /// Holds the MethodBase, de input and output arguments if any and the return value.
        /// </summary>
        public class CacheEntry {
            /// <summary>
            /// </summary>
            public MethodBase Method { get; set; }
            /// <summary>
            /// </summary>
            public object[] InputArguments { get; set; }
            /// <summary>
            /// </summary>
            public object[] OutputArguments { get; set; }
            /// <summary>
            /// </summary>
            public object ReturnValue { get; set; }

            /// <summary>
            /// </summary>
            /// <param name="method"></param>
            /// <param name="inputArguments"></param>
            public CacheEntry(MethodBase method, object[] inputArguments) {
                Method = method;
                InputArguments = inputArguments;
            }
        }
    }
}
