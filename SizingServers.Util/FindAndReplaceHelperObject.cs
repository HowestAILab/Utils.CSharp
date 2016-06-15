/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2015

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections.Generic;

namespace SizingServers.Util {
    /// <summary>
    /// <para>This class contains extra functionality upon FindAndReplace for searching a given text. For instance: A find next is implemented in it.</para>
    /// <para>It caches the found entries in the text (rows, columns, starts, match lengths) and invalidate it's cache when needed. For instance: A new find pattern is given.</para>
    /// <para>This also means that, unlike FindAndReplace, the functionality in this class is not thread safe.</para>
    /// </summary>
    public class FindAndReplaceHelperObject {
        private List<int> _rows, _columns, _starts, _matchLengths;

        private string _find, _inText;
        private bool _wholeWords, _matchCase;
        private int _findIndex;

        /// <summary>
        /// Find the next occurance of a given find pattern. When one of the given arguments is different then the last time this function was called, the find cache is invalidated.
        /// </summary>
        /// <param name="find">You can use *, +, - "" like before in Google. Multiline find patterns are not supported.</param>
        /// <param name="inText"></param>
        /// <param name="row">The row where a match is found.</param>
        /// <param name="column">The column where a match is found.</param>
        /// <param name="start">The start index of a find, for making text selections easier.</param>
        /// <param name="matchLength">The length of the match</param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        public void FindNext(string find, string inText, out int row, out int column, out int start, out int matchLength, bool wholeWords = false, bool matchCase = false) {
            row = column = start = matchLength = -1;
            if (_find != find || _inText != inText || _wholeWords != wholeWords || _matchCase != matchCase) {
                FindAndReplace.Find(find, inText, out _rows, out _columns, out _starts, out _matchLengths, wholeWords, matchCase);

                _findIndex = 0;
                _find = find;
                _inText = inText;
                _wholeWords = wholeWords;
                _matchCase = matchCase;
            }

            if (_rows.Count != 0) {
                row = _rows[_findIndex];
                column = _columns[_findIndex];
                start = _starts[_findIndex];
                matchLength = _matchLengths[_findIndex];
                ++_findIndex;
                if (_findIndex == _rows.Count) _findIndex = 0;
            }
        }
        /// <summary>
        /// Find all occurances of a given find pattern. When one of the given arguments is different then the last time this function was called, the find cache is invalidated.
        /// </summary>
        /// <param name="find">You can use *, +, - "" like before in Google. Multiline find patterns are not supported.</param>
        /// <param name="inText"></param>
        /// <param name="rows">The rows where a match is found.</param>
        /// <param name="columns">The columns where a match is found.</param>
        /// <param name="starts">The start index of each find, for making text selections easier.</param>
        /// <param name="matchLengths">The lengths of the match</param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        public void FindAll(string find, string inText, out List<int> rows, out List<int> columns, out List<int> starts, out List<int> matchLengths, bool wholeWords = false, bool matchCase = false) {
            if (_find != find || _inText != inText || _wholeWords != wholeWords || _matchCase != matchCase) {
                FindAndReplace.Find(find, inText, out _rows, out _columns, out _starts, out _matchLengths, wholeWords, matchCase);

                _find = find;
                _inText = inText;
                _wholeWords = wholeWords;
                _matchCase = matchCase;
            }

            _findIndex = 0;
            rows = _rows;
            columns = _columns;
            starts = _starts;
            matchLengths = _matchLengths;
        }
        /// <summary>
        /// Always call FindNext(...) or FindAll(...) first.
        /// </summary>
        /// <param name="with"></param>
        /// <param name="all">True for replacing all occurances.</param>
        /// <returns></returns>
        public string Replace(string with, bool all) {
            if (_findIndex == -1) return _inText;

            string inText;
            if (all) {
                inText = FindAndReplace.Replace(_rows, _columns, _matchLengths, _inText, with);
            } else {
                int row = _rows[_findIndex];
                int column = _columns[_findIndex];
                int start = _starts[_findIndex];
                int matchLength = _matchLengths[_findIndex];

                inText = FindAndReplace.Replace(row, column, matchLength, _inText, with);
            }

            List<int> r, c, s, m;
            FindAll(_find, inText, out r, out c, out s, out m, _wholeWords, _matchCase);
            return _inText;
        }
    }
}
