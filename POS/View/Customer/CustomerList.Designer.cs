﻿namespace POS
{
    partial class CustomerList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerList));
            this.btnAddNewCustomer = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRevoke = new System.Windows.Forms.Button();
            this.dtpBirthday = new System.Windows.Forms.DateTimePicker();
            this.rdoBirthday = new System.Windows.Forms.RadioButton();
            this.lblSearchTitle = new System.Windows.Forms.Label();
            this.rdoMemberCardNo = new System.Windows.Forms.RadioButton();
            this.rdoCustomerName = new System.Windows.Forms.RadioButton();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgvCustomerList = new System.Windows.Forms.DataGridView();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.rdoAllCustomers = new System.Windows.Forms.RadioButton();
            this.rdoNonMemberCustomers = new System.Windows.Forms.RadioButton();
            this.rdoMemberCustomers = new System.Windows.Forms.RadioButton();
            this.gbMemberType = new System.Windows.Forms.GroupBox();
            this.cboMemberType = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupvipstarted = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.createdshop = new System.Windows.Forms.ComboBox();
            this.btn_fix = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerList)).BeginInit();
            this.gbMemberType.SuspendLayout();
            this.groupvipstarted.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddNewCustomer
            // 
            this.btnAddNewCustomer.BackColor = System.Drawing.Color.Transparent;
            this.btnAddNewCustomer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnAddNewCustomer.FlatAppearance.BorderSize = 0;
            this.btnAddNewCustomer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnAddNewCustomer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnAddNewCustomer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNewCustomer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAddNewCustomer.Image = global::POS.Properties.Resources.addnewcustomer;
            this.btnAddNewCustomer.Location = new System.Drawing.Point(791, 14);
            this.btnAddNewCustomer.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnAddNewCustomer.Name = "btnAddNewCustomer";
            this.btnAddNewCustomer.Size = new System.Drawing.Size(215, 79);
            this.btnAddNewCustomer.TabIndex = 1;
            this.btnAddNewCustomer.UseVisualStyleBackColor = false;
            this.btnAddNewCustomer.Visible = false;
            this.btnAddNewCustomer.Click += new System.EventHandler(this.btnAddNewCustomer_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRevoke);
            this.groupBox1.Controls.Add(this.dtpBirthday);
            this.groupBox1.Controls.Add(this.rdoBirthday);
            this.groupBox1.Controls.Add(this.lblSearchTitle);
            this.groupBox1.Controls.Add(this.rdoMemberCardNo);
            this.groupBox1.Controls.Add(this.rdoCustomerName);
            this.groupBox1.Controls.Add(this.btnClearSearch);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.btnAddNewCustomer);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.groupBox1.Location = new System.Drawing.Point(7, 59);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(1014, 106);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search By";
            // 
            // btnRevoke
            // 
            this.btnRevoke.BackColor = System.Drawing.Color.Transparent;
            this.btnRevoke.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnRevoke.FlatAppearance.BorderSize = 0;
            this.btnRevoke.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnRevoke.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnRevoke.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRevoke.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnRevoke.Image = global::POS.Properties.Resources.revokeAllHan1;
            this.btnRevoke.Location = new System.Drawing.Point(630, 17);
            this.btnRevoke.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnRevoke.Name = "btnRevoke";
            this.btnRevoke.Size = new System.Drawing.Size(150, 80);
            this.btnRevoke.TabIndex = 7;
            this.btnRevoke.UseVisualStyleBackColor = false;
            this.btnRevoke.Click += new System.EventHandler(this.btnRevoke_Click);
            // 
            // dtpBirthday
            // 
            this.dtpBirthday.Location = new System.Drawing.Point(130, 66);
            this.dtpBirthday.Name = "dtpBirthday";
            this.dtpBirthday.Size = new System.Drawing.Size(262, 24);
            this.dtpBirthday.TabIndex = 4;
            // 
            // rdoBirthday
            // 
            this.rdoBirthday.AutoSize = true;
            this.rdoBirthday.Location = new System.Drawing.Point(326, 25);
            this.rdoBirthday.Name = "rdoBirthday";
            this.rdoBirthday.Size = new System.Drawing.Size(82, 22);
            this.rdoBirthday.TabIndex = 2;
            this.rdoBirthday.Text = "Birthday";
            this.rdoBirthday.UseVisualStyleBackColor = true;
            this.rdoBirthday.CheckedChanged += new System.EventHandler(this.rdoBirthday_CheckedChanged);
            // 
            // lblSearchTitle
            // 
            this.lblSearchTitle.AutoSize = true;
            this.lblSearchTitle.Location = new System.Drawing.Point(15, 69);
            this.lblSearchTitle.Name = "lblSearchTitle";
            this.lblSearchTitle.Size = new System.Drawing.Size(114, 18);
            this.lblSearchTitle.TabIndex = 3;
            this.lblSearchTitle.Text = "Member Card Id";
            // 
            // rdoMemberCardNo
            // 
            this.rdoMemberCardNo.AutoSize = true;
            this.rdoMemberCardNo.Checked = true;
            this.rdoMemberCardNo.Location = new System.Drawing.Point(18, 25);
            this.rdoMemberCardNo.Name = "rdoMemberCardNo";
            this.rdoMemberCardNo.Size = new System.Drawing.Size(135, 22);
            this.rdoMemberCardNo.TabIndex = 0;
            this.rdoMemberCardNo.TabStop = true;
            this.rdoMemberCardNo.Text = "Member Card Id";
            this.rdoMemberCardNo.UseVisualStyleBackColor = true;
            this.rdoMemberCardNo.CheckedChanged += new System.EventHandler(this.rdoMemberCardNo_CheckedChanged);
            // 
            // rdoCustomerName
            // 
            this.rdoCustomerName.AutoSize = true;
            this.rdoCustomerName.Location = new System.Drawing.Point(174, 25);
            this.rdoCustomerName.Name = "rdoCustomerName";
            this.rdoCustomerName.Size = new System.Drawing.Size(139, 22);
            this.rdoCustomerName.TabIndex = 1;
            this.rdoCustomerName.Text = "Customer Name";
            this.rdoCustomerName.UseVisualStyleBackColor = true;
            this.rdoCustomerName.CheckedChanged += new System.EventHandler(this.rdoCustomerName_CheckedChanged);
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnClearSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnClearSearch.FlatAppearance.BorderSize = 0;
            this.btnClearSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnClearSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnClearSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearSearch.Image = global::POS.Properties.Resources.refresh_small;
            this.btnClearSearch.Location = new System.Drawing.Point(512, 55);
            this.btnClearSearch.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(75, 46);
            this.btnClearSearch.TabIndex = 6;
            this.btnClearSearch.UseVisualStyleBackColor = false;
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
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
            this.btnSearch.Location = new System.Drawing.Point(414, 55);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 46);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(130, 66);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(262, 24);
            this.txtSearch.TabIndex = 5;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dgvCustomerList);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.groupBox3.Location = new System.Drawing.Point(7, 172);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1014, 460);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Customer List";
            // 
            // dgvCustomerList
            // 
            this.dgvCustomerList.AllowUserToAddRows = false;
            this.dgvCustomerList.AllowUserToDeleteRows = false;
            this.dgvCustomerList.AllowUserToResizeColumns = false;
            this.dgvCustomerList.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomerList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomerList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomerList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column6,
            this.colMemId,
            this.Column1,
            this.Column2,
            this.Column5,
            this.Column8,
            this.Column3,
            this.Column9,
            this.Column4});
            this.dgvCustomerList.Location = new System.Drawing.Point(6, 20);
            this.dgvCustomerList.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.dgvCustomerList.Name = "dgvCustomerList";
            this.dgvCustomerList.ReadOnly = true;
            this.dgvCustomerList.Size = new System.Drawing.Size(1002, 431);
            this.dgvCustomerList.TabIndex = 4;
            this.dgvCustomerList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomerList_CellClick);
            this.dgvCustomerList.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvCustomerList_DataBindingComplete);
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "Id";
            this.Column6.HeaderText = "ID";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Visible = false;
            // 
            // colMemId
            // 
            this.colMemId.DataPropertyName = "VIPMemberId";
            this.colMemId.HeaderText = "Member Card Id";
            this.colMemId.Name = "colMemId";
            this.colMemId.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Name";
            this.Column1.HeaderText = "Customer Name";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 210;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "PhoneNumber";
            this.Column2.HeaderText = "Phone Number";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 120;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Member Type";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 150;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "PromoteDate";
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Column8.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column8.HeaderText = "Member Registration Date";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 120;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Text = "Detail";
            this.Column3.UseColumnTextForLinkValue = true;
            this.Column3.VisitedLinkColor = System.Drawing.Color.Blue;
            this.Column3.Width = 80;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.Text = "Edit";
            this.Column9.UseColumnTextForLinkValue = true;
            this.Column9.Width = 80;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Text = "Delete";
            this.Column4.UseColumnTextForLinkValue = true;
            this.Column4.Width = 80;
            // 
            // rdoAllCustomers
            // 
            this.rdoAllCustomers.AutoSize = true;
            this.rdoAllCustomers.Checked = true;
            this.rdoAllCustomers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoAllCustomers.Location = new System.Drawing.Point(12, 18);
            this.rdoAllCustomers.Name = "rdoAllCustomers";
            this.rdoAllCustomers.Size = new System.Drawing.Size(44, 22);
            this.rdoAllCustomers.TabIndex = 5;
            this.rdoAllCustomers.TabStop = true;
            this.rdoAllCustomers.Text = "All";
            this.rdoAllCustomers.UseVisualStyleBackColor = true;
            this.rdoAllCustomers.CheckedChanged += new System.EventHandler(this.rdoAllCustomers_CheckedChanged);
            // 
            // rdoNonMemberCustomers
            // 
            this.rdoNonMemberCustomers.AutoSize = true;
            this.rdoNonMemberCustomers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoNonMemberCustomers.Location = new System.Drawing.Point(68, 18);
            this.rdoNonMemberCustomers.Name = "rdoNonMemberCustomers";
            this.rdoNonMemberCustomers.Size = new System.Drawing.Size(117, 22);
            this.rdoNonMemberCustomers.TabIndex = 8;
            this.rdoNonMemberCustomers.Text = "Non-Member";
            this.rdoNonMemberCustomers.UseVisualStyleBackColor = true;
            this.rdoNonMemberCustomers.CheckedChanged += new System.EventHandler(this.rdoNonMemberCustomers_CheckedChanged);
            // 
            // rdoMemberCustomers
            // 
            this.rdoMemberCustomers.AutoSize = true;
            this.rdoMemberCustomers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoMemberCustomers.Location = new System.Drawing.Point(181, 18);
            this.rdoMemberCustomers.Name = "rdoMemberCustomers";
            this.rdoMemberCustomers.Size = new System.Drawing.Size(88, 22);
            this.rdoMemberCustomers.TabIndex = 7;
            this.rdoMemberCustomers.Text = "Member ";
            this.rdoMemberCustomers.UseVisualStyleBackColor = true;
            this.rdoMemberCustomers.CheckedChanged += new System.EventHandler(this.rdoMemberCustomers_CheckedChanged);
            // 
            // gbMemberType
            // 
            this.gbMemberType.Controls.Add(this.cboMemberType);
            this.gbMemberType.Controls.Add(this.label10);
            this.gbMemberType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.gbMemberType.Location = new System.Drawing.Point(274, -1);
            this.gbMemberType.Name = "gbMemberType";
            this.gbMemberType.Size = new System.Drawing.Size(334, 56);
            this.gbMemberType.TabIndex = 9;
            this.gbMemberType.TabStop = false;
            this.gbMemberType.Visible = false;
            // 
            // cboMemberType
            // 
            this.cboMemberType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMemberType.FormattingEnabled = true;
            this.cboMemberType.Location = new System.Drawing.Point(126, 18);
            this.cboMemberType.Name = "cboMemberType";
            this.cboMemberType.Size = new System.Drawing.Size(181, 26);
            this.cboMemberType.TabIndex = 1;
            this.cboMemberType.SelectedIndexChanged += new System.EventHandler(this.cboMemberType_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(99, 18);
            this.label10.TabIndex = 0;
            this.label10.Text = "Member Type";
            // 
            // groupvipstarted
            // 
            this.groupvipstarted.Controls.Add(this.label1);
            this.groupvipstarted.Controls.Add(this.createdshop);
            this.groupvipstarted.Location = new System.Drawing.Point(614, 0);
            this.groupvipstarted.Name = "groupvipstarted";
            this.groupvipstarted.Size = new System.Drawing.Size(325, 56);
            this.groupvipstarted.TabIndex = 19;
            this.groupvipstarted.TabStop = false;
            this.groupvipstarted.Text = "select shop";
            this.groupvipstarted.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 17);
            this.label1.TabIndex = 16;
            this.label1.Text = "VipStartedIn";
            // 
            // createdshop
            // 
            this.createdshop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createdshop.FormattingEnabled = true;
            this.createdshop.Location = new System.Drawing.Point(95, 19);
            this.createdshop.Name = "createdshop";
            this.createdshop.Size = new System.Drawing.Size(204, 25);
            this.createdshop.TabIndex = 9;
            this.createdshop.SelectedIndexChanged += new System.EventHandler(this.createdshop_SelectedIndexChanged);
            // 
            // btn_fix
            // 
            this.btn_fix.Location = new System.Drawing.Point(947, 19);
            this.btn_fix.Name = "btn_fix";
            this.btn_fix.Size = new System.Drawing.Size(66, 31);
            this.btn_fix.TabIndex = 20;
            this.btn_fix.Text = "Fix Data";
            this.btn_fix.UseVisualStyleBackColor = true;
            this.btn_fix.Click += new System.EventHandler(this.btn_fix_Click);
            // 
            // CustomerList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1025, 632);
            this.Controls.Add(this.groupvipstarted);
            this.Controls.Add(this.btn_fix);
            this.Controls.Add(this.gbMemberType);
            this.Controls.Add(this.rdoNonMemberCustomers);
            this.Controls.Add(this.rdoMemberCustomers);
            this.Controls.Add(this.rdoAllCustomers);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CustomerList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer List";
            this.Load += new System.EventHandler(this.CustomerList_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerList)).EndInit();
            this.gbMemberType.ResumeLayout(false);
            this.gbMemberType.PerformLayout();
            this.groupvipstarted.ResumeLayout(false);
            this.groupvipstarted.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddNewCustomer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.RadioButton rdoMemberCardNo;
        private System.Windows.Forms.RadioButton rdoCustomerName;
        private System.Windows.Forms.Label lblSearchTitle;
        private System.Windows.Forms.RadioButton rdoBirthday;
        private System.Windows.Forms.DateTimePicker dtpBirthday;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dgvCustomerList;
        private System.Windows.Forms.RadioButton rdoAllCustomers;
        private System.Windows.Forms.RadioButton rdoNonMemberCustomers;
        private System.Windows.Forms.RadioButton rdoMemberCustomers;
        private System.Windows.Forms.GroupBox gbMemberType;
        private System.Windows.Forms.ComboBox cboMemberType;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewLinkColumn Column3;
        private System.Windows.Forms.DataGridViewLinkColumn Column9;
        private System.Windows.Forms.DataGridViewLinkColumn Column4;
        private System.Windows.Forms.GroupBox groupvipstarted;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox createdshop;
        private System.Windows.Forms.Button btn_fix;
        private System.Windows.Forms.Button btnRevoke;
    }
}