namespace POS
{
    partial class NewCustomerReport
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cboActiveNonActive = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboMemberType = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblpurchase = new System.Windows.Forms.Label();
            this.lblage = new System.Windows.Forms.Label();
            this.rdoPurchase = new System.Windows.Forms.RadioButton();
            this.txtEnterValue = new System.Windows.Forms.TextBox();
            this.cboAgeOP = new System.Windows.Forms.ComboBox();
            this.rdoAge = new System.Windows.Forms.RadioButton();
            this.lblCustomername = new System.Windows.Forms.Label();
            this.rdoCustomerName = new System.Windows.Forms.RadioButton();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cboActiveNonActive);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.cboMemberType);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Font = new System.Drawing.Font("Zawgyi-One", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(30, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(591, 70);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search By";
            // 
            // cboActiveNonActive
            // 
            this.cboActiveNonActive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboActiveNonActive.FormattingEnabled = true;
            this.cboActiveNonActive.Items.AddRange(new object[] {
            "All",
            "Active",
            "NonActive"});
            this.cboActiveNonActive.Location = new System.Drawing.Point(373, 29);
            this.cboActiveNonActive.Name = "cboActiveNonActive";
            this.cboActiveNonActive.Size = new System.Drawing.Size(181, 28);
            this.cboActiveNonActive.TabIndex = 3;
            this.cboActiveNonActive.SelectedIndexChanged += new System.EventHandler(this.cboActiveNonActive_SelectedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(328, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "State";
            // 
            // cboMemberType
            // 
            this.cboMemberType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMemberType.FormattingEnabled = true;
            this.cboMemberType.Location = new System.Drawing.Point(116, 26);
            this.cboMemberType.Name = "cboMemberType";
            this.cboMemberType.Size = new System.Drawing.Size(181, 28);
            this.cboMemberType.TabIndex = 1;
            this.cboMemberType.SelectedIndexChanged += new System.EventHandler(this.cboMemberType_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 20);
            this.label10.TabIndex = 0;
            this.label10.Text = "Member Type";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblpurchase);
            this.groupBox1.Controls.Add(this.lblage);
            this.groupBox1.Controls.Add(this.rdoPurchase);
            this.groupBox1.Controls.Add(this.txtEnterValue);
            this.groupBox1.Controls.Add(this.cboAgeOP);
            this.groupBox1.Controls.Add(this.rdoAge);
            this.groupBox1.Controls.Add(this.lblCustomername);
            this.groupBox1.Controls.Add(this.rdoCustomerName);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Font = new System.Drawing.Font("Zawgyi-One", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.groupBox1.Location = new System.Drawing.Point(30, 89);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(882, 106);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search By";
            // 
            // lblpurchase
            // 
            this.lblpurchase.AutoSize = true;
            this.lblpurchase.Location = new System.Drawing.Point(17, 71);
            this.lblpurchase.Name = "lblpurchase";
            this.lblpurchase.Size = new System.Drawing.Size(106, 20);
            this.lblpurchase.TabIndex = 8;
            this.lblpurchase.Text = "Purchase Amount";
            // 
            // lblage
            // 
            this.lblage.AutoSize = true;
            this.lblage.Location = new System.Drawing.Point(15, 69);
            this.lblage.Name = "lblage";
            this.lblage.Size = new System.Drawing.Size(31, 20);
            this.lblage.TabIndex = 7;
            this.lblage.Text = "Age";
            // 
            // rdoPurchase
            // 
            this.rdoPurchase.AutoSize = true;
            this.rdoPurchase.Location = new System.Drawing.Point(261, 27);
            this.rdoPurchase.Name = "rdoPurchase";
            this.rdoPurchase.Size = new System.Drawing.Size(124, 24);
            this.rdoPurchase.TabIndex = 6;
            this.rdoPurchase.Text = "Purchase Amount";
            this.rdoPurchase.UseVisualStyleBackColor = true;
            this.rdoPurchase.CheckedChanged += new System.EventHandler(this.rdoPurchase_CheckedChanged);
            // 
            // txtEnterValue
            // 
            this.txtEnterValue.Location = new System.Drawing.Point(235, 68);
            this.txtEnterValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtEnterValue.Name = "txtEnterValue";
            this.txtEnterValue.Size = new System.Drawing.Size(150, 27);
            this.txtEnterValue.TabIndex = 5;
            this.txtEnterValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEnterValue_KeyPress);
            // 
            // cboAgeOP
            // 
            this.cboAgeOP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboAgeOP.Font = new System.Drawing.Font("Zawgyi-One", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboAgeOP.FormattingEnabled = true;
            this.cboAgeOP.Items.AddRange(new object[] {
            "=",
            ">",
            "<",
            ">=",
            "<="});
            this.cboAgeOP.Location = new System.Drawing.Point(138, 68);
            this.cboAgeOP.Name = "cboAgeOP";
            this.cboAgeOP.Size = new System.Drawing.Size(70, 28);
            this.cboAgeOP.TabIndex = 4;
            // 
            // rdoAge
            // 
            this.rdoAge.AutoSize = true;
            this.rdoAge.Location = new System.Drawing.Point(176, 27);
            this.rdoAge.Name = "rdoAge";
            this.rdoAge.Size = new System.Drawing.Size(49, 24);
            this.rdoAge.TabIndex = 2;
            this.rdoAge.Text = "Age";
            this.rdoAge.UseVisualStyleBackColor = true;
            this.rdoAge.CheckedChanged += new System.EventHandler(this.rdoAge_CheckedChanged);
            // 
            // lblCustomername
            // 
            this.lblCustomername.AutoSize = true;
            this.lblCustomername.Location = new System.Drawing.Point(15, 69);
            this.lblCustomername.Name = "lblCustomername";
            this.lblCustomername.Size = new System.Drawing.Size(96, 20);
            this.lblCustomername.TabIndex = 3;
            this.lblCustomername.Text = "Customer Name";
            // 
            // rdoCustomerName
            // 
            this.rdoCustomerName.AutoSize = true;
            this.rdoCustomerName.Location = new System.Drawing.Point(19, 27);
            this.rdoCustomerName.Name = "rdoCustomerName";
            this.rdoCustomerName.Size = new System.Drawing.Size(114, 24);
            this.rdoCustomerName.TabIndex = 1;
            this.rdoCustomerName.Text = "Customer Name";
            this.rdoCustomerName.UseVisualStyleBackColor = true;
            this.rdoCustomerName.CheckedChanged += new System.EventHandler(this.rdoCustomerName_CheckedChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Image = global::POS.Properties.Resources.search_small;
            this.btnSearch.Location = new System.Drawing.Point(413, 55);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 46);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(129, 66);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(262, 27);
            this.txtSearch.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.reportViewer1);
            this.groupBox2.Font = new System.Drawing.Font("Zawgyi-One", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 200);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(1232, 463);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Customer List";
            // 
            // reportViewer1
            // 
            this.reportViewer1.Font = new System.Drawing.Font("Zawgyi-One", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reportViewer1.Location = new System.Drawing.Point(6, 28);
            this.reportViewer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ShowPrintButton = false;
            this.reportViewer1.ShowRefreshButton = false;
            this.reportViewer1.ShowStopButton = false;
            this.reportViewer1.ShowZoomControl = false;
            this.reportViewer1.Size = new System.Drawing.Size(1215, 427);
            this.reportViewer1.TabIndex = 0;
            // 
            // NewCustomerReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1251, 676);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "NewCustomerReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Customer Report";
            this.Load += new System.EventHandler(this.NewCustomerReport_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cboMemberType;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cboActiveNonActive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboAgeOP;
        private System.Windows.Forms.RadioButton rdoAge;
        private System.Windows.Forms.Label lblCustomername;
        private System.Windows.Forms.RadioButton rdoCustomerName;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.RadioButton rdoPurchase;
        private System.Windows.Forms.TextBox txtEnterValue;
        private System.Windows.Forms.GroupBox groupBox2;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.Label lblpurchase;
        private System.Windows.Forms.Label lblage;
    }
}