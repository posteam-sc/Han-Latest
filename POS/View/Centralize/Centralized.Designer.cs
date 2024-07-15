namespace POS
{
    partial class Centralized
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Progressbar1 = new System.Windows.Forms.ProgressBar();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ofdImportFile = new System.Windows.Forms.OpenFileDialog();
            this.txtmonth = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Progressbar1
            // 
            this.Progressbar1.Location = new System.Drawing.Point(12, 69);
            this.Progressbar1.Name = "Progressbar1";
            this.Progressbar1.Size = new System.Drawing.Size(523, 23);
            this.Progressbar1.TabIndex = 9;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(317, 17);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 8;
            this.btnImport.Text = "Read XML";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(147, 17);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 7;
            this.btnExport.Text = "Export XML";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Zawgyi-One", 9F);
            this.label1.Location = new System.Drawing.Point(54, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 20);
            this.label1.TabIndex = 10;
            // 
            // ofdImportFile
            // 
            this.ofdImportFile.FileName = "openFileDialog1";
            // 
            // txtmonth
            // 
            this.txtmonth.Location = new System.Drawing.Point(21, 19);
            this.txtmonth.Name = "txtmonth";
            this.txtmonth.Size = new System.Drawing.Size(100, 20);
            this.txtmonth.TabIndex = 11;
            this.txtmonth.TextChanged += new System.EventHandler(this.txtmonth_TextChanged);
            // 
            // Centralized
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 158);
            this.Controls.Add(this.txtmonth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Progressbar1);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Name = "Centralized";
            this.Text = "Centralized";
            this.Load += new System.EventHandler(this.Centralized_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar Progressbar1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog ofdImportFile;
        private System.Windows.Forms.TextBox txtmonth;
    }
}