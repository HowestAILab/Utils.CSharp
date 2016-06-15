/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2013

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SizingServers.Util {
    /// <summary>
    /// <para>A helper class to easily find and replace in a given piece of text. Multiline find patterns are not supported.</para>
    /// <para>Use this class if you want something 'reasonably' fast and thread safe.</para>
    /// <para>If you want more practical functionality for real user input, use FindAndReplaceHelperObject.</para>
    /// </summary>
    public static class FindAndReplace {
        /// <summary>
        /// </summary>
        /// <param name="find">You can use *, +, - "" like before in Google. Multiline find patterns are not supported.</param>
        /// <param name="inText"></param>
        /// <param name="rows">The rows where a match is found.</param>
        /// <param name="columns">The columns where a match is found.</param>
        /// <param name="starts">The start index of each find, for making text selections easier.</param>
        /// <param name="matchLengths">The lengths of the match</param>
        /// <param name="wholeWords"></param>
        /// <param name="matchCase"></param>
        public static void Find(string find, string inText, out List<int> rows, out List<int> columns, out List<int> starts, out List<int> matchLengths, bool wholeWords = false, bool matchCase = false) {
            if (find.Contains("\n") || find.Contains("\r")) throw new ArgumentException("Multiline find patterns are not supported");

            string[] lines = inText.Split(new char[] { '\n' }, StringSplitOptions.None); //We match regexes single line.

            string mustHaveRegex, canHaveRegex, cannotHaveRegex;

            //+ and - are allowed. (Must, cannot have)
            DetermineFindRegexes(find, wholeWords, out mustHaveRegex, out canHaveRegex, out cannotHaveRegex);

            RegexOptions options = matchCase ? RegexOptions.Singleline : RegexOptions.Singleline | RegexOptions.IgnoreCase;

            rows = new List<int>();
            columns = new List<int>();
            starts = new List<int>();
            matchLengths = new List<int>();

            int rowsProcessedLength = 0;

            for (int row = 0; row != lines.Length; row++) {
                string line = lines[row];
                //Exclude all rows that do not match.
                if (cannotHaveRegex.Length == 0 || !Regex.IsMatch(line, cannotHaveRegex, options)) {
                    //Include the rows that must match.
                    MatchCollection matches = null;
                    if (mustHaveRegex.Length != 0) {
                        matches = Regex.Matches(line, mustHaveRegex, options);
                        foreach (Match match in matches) {
                            rows.Add(row);
                            columns.Add(match.Index);
                            starts.Add(rowsProcessedLength + match.Index);
                            matchLengths.Add(match.Value.Length);
                        }
                    }
                    //Include the rows that "can" match. Strange check, but it works.
                    if (mustHaveRegex.Length == 0 || matches.Count != 0) {
                        if (canHaveRegex.Length == 0) {
                            rows.Add(row);
                            columns.Add(0);
                            starts.Add(rowsProcessedLength);
                            matchLengths.Add(line.Length);
                        } else {
                            matches = Regex.Matches(line, canHaveRegex, options);
                            foreach (Match match in matches) {
                                rows.Add(row);
                                columns.Add(match.Index);
                                starts.Add(rowsProcessedLength + match.Index);
                                matchLengths.Add(match.Value.Length);
                            }
                        }
                    }
                }
                rowsProcessedLength += line.Length + 1; //+ 1 for the missing new line character.
            }
        }

        /// <summary>
        /// Always call Find(...) first.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="matchLength"></param>
        /// <param name="inText"></param>
        /// <param name="with"></param>
        /// <returns></returns>
        public static string Replace(int row, int column, int matchLength, string inText, string with) {
            var rows = new List<int>(1);
            var columns = new List<int>(1);
            var matchLengths = new List<int>(1);

            rows.Add(row);
            columns.Add(column);
            matchLengths.Add(matchLength);

            return Replace(rows, columns, matchLengths, inText, with);
        }

        /// <summary>
        /// Always call Find(...) first.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="matchLengths"></param>
        /// <param name="inText"></param>
        /// <param name="with"></param>
        /// <returns></returns>
        public static string Replace(List<int> rows, List<int> columns, List<int> matchLengths, string inText, string with) {
            string[] lines = inText.Split(new char[] { '\n' }, StringSplitOptions.None); //We match regexes single line.

            var newLines = new string[lines.Length];
            int rowIndex = 0;
            for (int i = 0; i != lines.Length; i++) {
                var line = lines[i];

                //These will be reverserd, so if the text changes the indies will still be right.
                var partColumns = new List<int>();
                var partMatchLengths = new List<int>();

                while (rowIndex != rows.Count && rows[rowIndex] == i) {
                    partColumns.Add(columns[rowIndex]);
                    partMatchLengths.Add(matchLengths[rowIndex]);
                    ++rowIndex;
                }

                if (partColumns.Count != 0) {
                    partColumns.Sort();
                    partColumns.Reverse();

                    partMatchLengths.Sort();
                    partMatchLengths.Reverse();

                    for (int j = 0; j != partColumns.Count; j++) {
                        int column = partColumns[j];
                        line = line.Substring(0, column) + with + line.Substring(column + partMatchLengths[j]);
                    }
                }
                newLines[i] = line;
            }

            return Combine(newLines, Environment.NewLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="find"></param>
        /// <param name="wholeWords"></param>
        /// <param name="mustHaveRegex"></param>
        /// <param name="canHaveRegex"></param>
        /// <param name="cannotHaveRegex"></param>
        private static void DetermineFindRegexes(string find, bool wholeWords, out string mustHaveRegex, out string canHaveRegex, out string cannotHaveRegex) {
            string wordDelimiter = wholeWords ? "\\b" : string.Empty;

            var mustHave = new List<string>(); //And relation (+ in the text)
            var canHave = new List<string>();
            var cannotHave = new List<string>(); //Or relation (- in the text)

            string[] sentences = GetSentences(find);
            for (int i = 0; i != sentences.Length; i++) {
                string sentence = sentences[i];
                if (!(sentences.Length == 1 && string.IsNullOrWhiteSpace(sentence)))  //Allow whitespace find.
                    sentence = sentence.TrimStart();

                bool addToMustHave = false;
                bool addToCannotHave = false;
                if (sentence.StartsWith("+")) {
                    sentence = (sentence.Length == 1) ? string.Empty : sentence.Substring(1);
                    addToMustHave = true;
                } else if (sentence.StartsWith("-")) {
                    sentence = (sentence.Length == 1) ? string.Empty : sentence.Substring(1);
                    addToCannotHave = true;
                }

                if (sentence.Length == 0) {
                    continue;
                } else {
                    sentence = Regex.Escape(sentence);
                    //All characters except white space.
                    sentence = sentence.Replace("\\*", "([\\S{6,}])*");

                    if (addToMustHave) {
                        if (canHave.Contains(sentence) || cannotHave.Contains(sentence)) {
                            continue;
                        } else {
                            sentence = "(" + wordDelimiter + ".*" + sentence + wordDelimiter + ")";
                            if (!mustHave.Contains(sentence)) mustHave.Add(sentence);
                        }
                    } else if (addToCannotHave) {
                        if (canHave.Contains(sentence) | mustHave.Contains(sentence)) {
                            continue;
                        } else {
                            sentence = "(" + wordDelimiter + sentence + wordDelimiter + ")";
                            if (!cannotHave.Contains(sentence)) cannotHave.Add(sentence);
                        }
                    } else {
                        if (mustHave.Contains(sentence) || cannotHave.Contains(sentence)) {
                            continue;
                        } else {
                            sentence = "(" + wordDelimiter + sentence + wordDelimiter + ")";
                            if (!canHave.Contains(sentence)) canHave.Add(sentence);
                        }
                    }
                }
            }
            mustHaveRegex = Combine(mustHave, string.Empty);
            canHaveRegex = Combine(canHave, "|");
            cannotHaveRegex = Combine(cannotHave, "|");
        }
       
        /// <summary>
        /// This takes double quotes into account
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string[] GetSentences(string s) {
            string[] args = s.Split(' ');
            var argsCorrectSentenced = new List<string>();
            //Check if the array does not contain '"', if so make new sentences.
            bool quote = false;
            foreach (string tokenCandidate in args) {
                if (tokenCandidate.StartsWith("\"") && !quote) {
                    if (tokenCandidate.Length > 1)
                        argsCorrectSentenced.Add(tokenCandidate.Substring(1));
                    else
                        argsCorrectSentenced.Add("");
                    quote = true;
                } else if (tokenCandidate.EndsWith("\"") && quote) {
                    if (tokenCandidate.Length > 1)
                        argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " " + tokenCandidate.Substring(0, tokenCandidate.Length - 1);
                    else
                        argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " ";
                    quote = false;
                } else {
                    if (quote)
                        argsCorrectSentenced[argsCorrectSentenced.Count - 1] += " " + tokenCandidate;
                    else
                        argsCorrectSentenced.Add(tokenCandidate);
                }
            }
            return argsCorrectSentenced.ToArray();
        }

        private static string Combine<T>(List<T> list, string separator) {
            if (list.Count == 0) return string.Empty;

            var sb = new StringBuilder();
            object value;
            for (int i = 0; i != list.Count - 1; i++) {
                value = list[i];
                sb.Append(value);
                sb.Append(separator);
            }
            value = list[list.Count - 1];
            sb.Append(value);

            return sb.ToString();
        }
        private static string Combine(Array array, string separator) {
            lock (array.SyncRoot) {
                if (array.Length == 0) return string.Empty;

                var sb = new StringBuilder();
                object value;
                for (int i = 0; i != array.Length - 1; i++) {
                    value = array.GetValue(i);
                    sb.Append(value);
                    sb.Append(separator);
                }
                value = array.GetValue(array.Length - 1);
                sb.Append(value);

                return sb.ToString();
            }
        }

    }
}
