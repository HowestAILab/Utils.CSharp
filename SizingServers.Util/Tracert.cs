/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2012

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace SizingServers.Util {
    /// <summary>
    /// Determines the trace route for a given destination host or IP.
    /// </summary>
    public class Tracert : IDisposable {
        /// <summary>
        /// </summary>
        public event EventHandler<HopEventArgs> Hop;
        /// <summary>
        /// </summary>
        public event EventHandler Done;

        #region Fields
        private static byte[] _buffer;
        private PingOptions _options;
        private Ping _ping;
        #endregion

        #region Properties
        /// <summary>
        /// The IP to trace route.
        /// </summary>
        public IPAddress IP { get; private set; }
        /// <summary>
        /// The IP status for the last hop.
        /// </summary>
        public IPStatus LastStatus { get; private set; }
        /// <summary>
        /// The maximum hops that may occur.
        /// </summary>
        public int MaxHops { get; private set; }
        /// <summary>
        /// The ping timeout for a hop in ms.
        /// </summary>
        public int Timeout { get; private set; }
        /// <summary>
        /// The numbers of hops done.
        /// </summary>
        public int Hops { get; private set; }
        /// <summary>
        /// </summary>
        public bool IsDone { get; private set; }
        #endregion

        private static byte[] Buffer {
            get {
                if (_buffer == null) {
                    _buffer = new byte[32];
                    for (int i = 0; i < _buffer.Length; i++)
                        _buffer[i] = 0x65;
                }
                return _buffer;
            }
        }

        #region Functions
        /// <summary>
        /// Determines the trace route for a given destination host or IP.
        /// </summary>
        /// <param name="hostNameOrIP">The host name or IP to trace route.</param>
        /// <param name="maxHops">The maximum hops that may occur.</param>
        /// <param name="timeout">The ping timeout for a hop in ms.</param>
        public void Trace(string hostNameOrIP, int maxHops = 100, int timeout = 5000) {
            if (_ping != null)
                throw new Exception("Another trace is still in route!");

            try {
                //Set the fields --> asynchonically handled
                IP = Dns.GetHostEntry(hostNameOrIP).AddressList[0];
                Hops = 0;
                MaxHops = maxHops;
                Timeout = timeout;

                IsDone = false;

                if (IPAddress.IsLoopback(IP)) {
                    ProcessHop(IP, IPStatus.Success);
                } else {
                    _ping = new Ping();

                    _ping.PingCompleted += OnPingCompleted;
                    _options = new PingOptions(1, true);
                    _ping.SendAsync(IP, Timeout, Buffer, _options, null);
                }
            } catch {
                ProcessHop(IP, IPStatus.Unknown);
            }
        }

        private void OnPingCompleted(object sender, PingCompletedEventArgs e) {
            ThreadPool.QueueUserWorkItem(delegate {
                if (!IsDone)
                    try {
                        _options.Ttl += 1;
                        ProcessHop(e.Reply.Address, e.Reply.Status);
                        _ping.SendAsync(IP, Timeout, Buffer, _options, null);
                    } catch {
                        //Handled elsewhere.
                    }
            });
        }

        private void ProcessHop(IPAddress ip, IPStatus status) {
            long roundTripTime = 0;
            if (status == IPStatus.TtlExpired || status == IPStatus.Success) {
                var pingIntermediate = new Ping();
                try {
                    //Compute roundtrip time to the address by pinging it
                    PingReply reply = pingIntermediate.Send(ip, Timeout);
                    roundTripTime = reply.RoundtripTime;
                    status = reply.Status;
                } catch (PingException e) {
                    //Do nothing
                    System.Diagnostics.Trace.WriteLine(e);
                }
                pingIntermediate.Dispose();
            }

            ++Hops;

            if (ip == null) {
                IsDone = true;
            } else {
                FireHop(ip, roundTripTime, status);
                IsDone = ip.Equals(IP) || Hops == MaxHops;
            }
            if (IsDone)
                FireDone();
        }

        private void FireHop(IPAddress ip, long roundTripTime, IPStatus status) {
            if (Hop != null) {
                string hostName = string.Empty;
                try {
                    hostName = Dns.GetHostEntry(ip).HostName;
                } catch {
                    //Some things don't have a (known) host name.
                }
                Hop(this, new HopEventArgs(ip.ToString(), hostName, roundTripTime, status));
            }
        }

        private void FireDone() {
            if (Done != null) Done(this, null);
        }

        /// <summary>
        /// </summary>
        public void Dispose() {
            IsDone = true;
            if (_ping != null)
                try {
                    _ping.Dispose();
                } catch {
                    //Don't care.
                }
            _ping = null;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public class HopEventArgs : EventArgs {
            /// <summary>
            /// </summary>
            public readonly string HostName;
            /// <summary>
            /// </summary>
            public readonly string IP;

            /// <summary>
            /// Round trip time in ms
            /// </summary>
            public readonly long RoundTripTime;

            /// <summary>
            /// The status of sending an Internet Control Message Protocol (ICMP) echo message to a computer.
            /// </summary>
            public readonly IPStatus Status;

            /// <summary>
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="hostName"></param>
            /// <param name="roundTripTime">in ms</param>
            /// <param name="status">The status of sending an Internet Control Message Protocol (ICMP) echo message to a computer.</param>
            public HopEventArgs(string ip, string hostName, long roundTripTime, IPStatus status) {
                IP = ip;
                HostName = hostName;
                RoundTripTime = roundTripTime;
                Status = status;
            }
        }
    }
}