namespace POS
{
    partial class frmPointDeductionPercentage_History
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPercentage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PDPActive = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvPointDeductionList = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DistcountRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.User = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Counter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Active = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPointDeductionList)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPercentage);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(92, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(499, 107);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Create Deduction Percentage Rule";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(327, 43);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 27);
            this.button1.TabIndex = 3;
            this.button1.Text = "create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(266, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "%";
            // 
            // txtPercentage
            // 
            this.txtPercentage.Location = new System.Drawing.Point(183, 44);
            this.txtPercentage.Name = "txtPercentage";
            this.txtPercentage.Size = new System.Drawing.Size(76, 20);
            this.txtPercentage.TabIndex = 1;
            this.txtPercentage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPercentage_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Discount Percentage    >";
            // 
            // PDPActive
            // 
            this.PDPActive.AutoSize = true;
            this.PDPActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PDPActive.ForeColor = System.Drawing.Color.Black;
            this.PDPActive.Location = new System.Drawing.Point(30, 12);
            this.PDPActive.Name = "PDPActive";
            this.PDPActive.Size = new System.Drawing.Size(133, 21);
            this.PDPActive.TabIndex = 1;
            this.PDPActive.Text = "Active Deduction";
            this.PDPActive.UseVisualStyleBackColor = true;
            this.PDPActive.CheckedChanged += new System.EventHandler(this.PDPActive_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvPointDeductionList);
            this.groupBox2.Location = new System.Drawing.Point(12, 178);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(675, 309);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "History list";
            // 
            // dgvPointDeductionList
            // 
            this.dgvPointDeductionList.AllowUserToAddRows = false;
            this.dgvPointDeductionList.AllowUserToDeleteRows = false;
            this.dgvPointDeductionList.AllowUserToResizeColumns = false;
            this.dgvPointDeductionList.AllowUserToResizeRows = false;
            this.dgvPointDeductionList.BackgroundColor = System.Drawing.Color.White;
            this.dgvPointDeductionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPointDeductionList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.DistcountRate,
            this.StartDate,
            this.EndDate,
            this.User,
            this.Counter,
            this.Active});
            this.dgvPointDeductionList.Location = new System.Drawing.Point(15, 19);
            this.dgvPointDeductionList.Name = "dgvPointDeductionList";
            this.dgvPointDeductionList.ReadOnly = true;
            this.dgvPointDeductionList.Size = new System.Drawing.Size(643, 274);
            this.dgvPointDeductionList.TabIndex = 0;
            this.dgvPointDeductionList.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvPointDeductionList_DataBindingComplte);
            // 
            // id
            // 
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Visible = false;
            // 
            // DistcountRate
            // 
            this.DistcountRate.HeaderText = "DistcountRate";
            this.DistcountRate.Name = "DistcountRate";
            this.DistcountRate.ReadOnly = true;
            this.DistcountRate.Width = 85;
            // 
            // StartDate
            // 
            this.StartDate.HeaderText = "StartDate";
            this.StartDate.Name = "StartDate";
            this.StartDate.ReadOnly = true;
            // 
            // EndDate
            // 
            this.EndDate.HeaderText = "EndDate";
            this.EndDate.Name = "EndDate";
            this.EndDate.ReadOnly = true;
            // 
            // User
            // 
            this.User.HeaderText = "User";
            this.User.Name = "User";
            this.User.ReadOnly = true;
            // 
            // Counter
            // 
            this.Counter.HeaderText = "Counter";
            this.Counter.Name = "Counter";
            this.Counter.ReadOnly = true;
            this.Counter.Width = 150;
            // 
            // Active
            // 
            this.Active.HeaderText = "Status";
            this.Active.Name = "Active";
            this.Active.ReadOnly = true;
            this.Active.Width = 65;
            // 
            // frmPointDeductionPercentage_History
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Azure;
            this.ClientSize = new System.Drawing.Size(703, 492);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.PDPActive);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmPointDeductionPercentage_History";
            this.Text = "Default VIP Double Promotion Rule";
            this.Load += new System.EventHandler(this.PointDeductionPercentage_History_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPointDeductionList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPercentage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox PDPActive;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvPointDeductionList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn DistcountRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn StartDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn User;
        private System.Windows.Forms.DataGridViewTextBoxColumn Counter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Active;
    }
}