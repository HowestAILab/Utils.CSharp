/*
Original author: Dieter Vandroemme, dev at Sizing Servers Lab (https://www.sizingservers.be) @ University College of West-Flanders, Department GKG
Written in 2015

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Windows.Forms;

namespace SizingServers.Util.Examples {
    public partial class CompilerUnitExample : Form {
        private CompilerUnit _compilerUnit = new CompilerUnit();
        public CompilerUnitExample() {
            InitializeComponent();
        }

        private void btnCompile_Click(object sender, EventArgs e) {
            CompilerResults results;
            _compilerUnit.Compile(richTextBox1.Text, false, out results);

            string warnings = string.Empty;
            string errors = string.Empty;
            foreach (CompilerError ce in results.Errors) {
                if (ce.IsWarning) warnings += ce.ErrorText + " ";
                else errors += ce.ErrorText + " ";
            }

            if (warnings.Length == 0 && errors.Length == 0) {
                MessageBox.Show("Compiled without errors or warnings.");

                try {
                    Assembly assembly = results.CompiledAssembly;
                    var foo = (IFoo)assembly.CreateInstance("SizingServers.Util.Examples.Foo");
                    foo.Bar();
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }

            } else {
                MessageBox.Show("Errors: " + errors + "\nWarnings: " + warnings);
            }
        }

        public interface IFoo { void Bar(); }
    }
}
