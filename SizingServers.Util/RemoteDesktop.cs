/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2015

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Diagnostics;

namespace SizingServers.Util {
    /// <summary>
    /// Uses the default Windows RDP client.
    /// </summary>
    public static class RemoteDesktop {
        /// <summary>
        ///Stupid credentials workaround. Do not forget to remove the credentials (RemoveCredentials(string ip)). Do not do this too fast, otherwise login will not work.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        public static void Show(string host, string username, string password, string domain = null) {
            StoreCredentials(host, username, password, domain);

            var p = new Process();
            p.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
            p.StartInfo.Arguments = "/v " + host;
            p.Start();
        }

        private static void StoreCredentials(string host, string username, string password, string domain = null) {
            if (!string.IsNullOrWhiteSpace(domain)) username = domain + "\\" + username;

            var p = new Process();
            p.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
            p.StartInfo.Arguments = "/generic:TERMSRV/" + host + " /user:" + username + " /pass:" + password;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
        }

        /// <summary>
        /// I know, this sucks. Why can't credentials be passed to mstsc.exe as arguments?...
        /// </summary>
        /// <param name="host"></param>
        public static void RemoveCredentials(string host) {
            var p = new Process();
            p.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
            p.StartInfo.Arguments = "/delete:TERMSRV/" + host;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
        }
    }
}
