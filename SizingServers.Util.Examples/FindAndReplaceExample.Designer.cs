namespace SizingServers.Util.Examples {
    partial class FindAndReplaceExample {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindAndReplaceExample));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.findAndReplaceControl1 = new WinForms.FindAndReplaceControl();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ShowSelectionMargin = true;
            this.richTextBox1.Size = new System.Drawing.Size(760, 453);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // findAndReplaceControl1
            // 
            this.findAndReplaceControl1.Find = "forms";
            this.findAndReplaceControl1.FindMatchCase = false;
            this.findAndReplaceControl1.FindWholeWords = true;
            this.findAndReplaceControl1.Location = new System.Drawing.Point(12, 471);
            this.findAndReplaceControl1.Name = "findAndReplaceControl1";
            this.findAndReplaceControl1.ReplaceAll = true;
            this.findAndReplaceControl1.ReplaceWith = "WORMS";
            this.findAndReplaceControl1.Size = new System.Drawing.Size(760, 78);
            this.findAndReplaceControl1.TabIndex = 5;
            this.findAndReplaceControl1.FindClicked += new System.EventHandler(this.findAndReplaceControl1_FindClicked);
            this.findAndReplaceControl1.ReplaceClicked += new System.EventHandler(this.findAndReplaceControl1_ReplaceClicked);
            // 
            // FindAndReplaceExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.findAndReplaceControl1);
            this.Controls.Add(this.richTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindAndReplaceExample";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FindAndReplace Example";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private WinForms.FindAndReplaceControl findAndReplaceControl1;
    }
}

