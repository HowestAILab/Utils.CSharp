namespace SizingServers.Util.Examples {
    partial class TracertExample {
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
            this.tracertControl1 = new WinForms.TracertControl();
            this.txt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tracertControl1
            // 
            this.tracertControl1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tracertControl1.Location = new System.Drawing.Point(270, 261);
            this.tracertControl1.MaximumSize = new System.Drawing.Size(9999, 35);
            this.tracertControl1.MinimumSize = new System.Drawing.Size(0, 35);
            this.tracertControl1.Name = "tracertControl1";
            this.tracertControl1.Size = new System.Drawing.Size(500, 35);
            this.tracertControl1.TabIndex = 1;
            this.tracertControl1.BeforeTrace += new System.EventHandler(this.tracertControl1_BeforeTrace);
            this.tracertControl1.Done += new System.EventHandler(this.tracertControl1_Done);
            // 
            // txt
            // 
            this.txt.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txt.Location = new System.Drawing.Point(164, 268);
            this.txt.Name = "txt";
            this.txt.Size = new System.Drawing.Size(100, 20);
            this.txt.TabIndex = 0;
            this.txt.Text = "www.google.be";
            // 
            // TracertExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.txt);
            this.Controls.Add(this.tracertControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TracertExample";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tracert Example";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WinForms.TracertControl tracertControl1;
        private System.Windows.Forms.TextBox txt;
    }
}

