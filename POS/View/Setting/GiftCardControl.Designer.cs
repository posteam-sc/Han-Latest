namespace POS
{
    partial class GiftCardControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GiftCardControl));
            this.dgvGiftCardList = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.colViewDetail = new System.Windows.Forms.DataGridViewLinkColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCardNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdo6000 = new System.Windows.Forms.RadioButton();
            this.rdo21000 = new System.Windows.Forms.RadioButton();
            this.rdo35000 = new System.Windows.Forms.RadioButton();
            this.rdo70000 = new System.Windows.Forms.RadioButton();
            this.rdo50000 = new System.Windows.Forms.RadioButton();
            this.rdo5000 = new System.Windows.Forms.RadioButton();
            this.lblPhoneNo = new System.Windows.Forms.Label();
            this.lblCustomersName = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblMember = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.txtSearchCardNo = new System.Windows.Forms.TextBox();
            this.lblNRIC = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.btnNewCustomer = new System.Windows.Forms.Button();
            this.cboCustomer = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rdo28000 = new System.Windows.Forms.RadioButton();
            this.rdo7000 = new System.Windows.Forms.RadioButton();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdoFilter6000 = new System.Windows.Forms.RadioButton();
            this.rdoFilter21000 = new System.Windows.Forms.RadioButton();
            this.rdoFilter28000 = new System.Windows.Forms.RadioButton();
            this.rdoFilter35000 = new System.Windows.Forms.RadioButton();
            this.rdoFilter70000 = new System.Windows.Forms.RadioButton();
            this.rdoFilter5000 = new System.Windows.Forms.RadioButton();
            this.cboSearchCustomer = new System.Windows.Forms.ComboBox();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.rdoFilter50000 = new System.Windows.Forms.RadioButton();
            this.rdoFilter7000 = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.rdo18000 = new System.Windows.Forms.RadioButton();
            this.rdoFilter18000 = new System.Windows.Forms.RadioButton();
            this.rdo30000 = new System.Windows.Forms.RadioButton();
            this.rdoFilter30000 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGiftCardList)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvGiftCardList
            // 
            this.dgvGiftCardList.AllowUserToAddRows = false;
            this.dgvGiftCardList.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGiftCardList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvGiftCardList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGiftCardList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column6,
            this.Column2,
            this.Column4,
            this.Column5,
            this.Column3,
            this.colViewDetail});
            this.dgvGiftCardList.Location = new System.Drawing.Point(6, 23);
            this.dgvGiftCardList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgvGiftCardList.Name = "dgvGiftCardList";
            this.dgvGiftCardList.RowHeadersVisible = false;
            this.dgvGiftCardList.RowHeadersWidth = 51;
            this.dgvGiftCardList.Size = new System.Drawing.Size(774, 269);
            this.dgvGiftCardList.TabIndex = 0;
            this.dgvGiftCardList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGiftCardList_CellClick);
            this.dgvGiftCardList.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvGiftCardList_DataBindingComplete);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Id";
            this.Column1.HeaderText = "Id";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.Visible = false;
            this.Column1.Width = 125;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Customer Name";
            this.Column6.MinimumWidth = 6;
            this.Column6.Name = "Column6";
            this.Column6.Width = 200;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "CardNumber";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column2.HeaderText = "Card Number";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.Width = 150;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "Amount";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column4.HeaderText = "Amount";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.Width = 125;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "";
            this.Column5.MinimumWidth = 6;
            this.Column5.Name = "Column5";
            this.Column5.Text = "Top Up";
            this.Column5.UseColumnTextForLinkValue = true;
            this.Column5.Visible = false;
            this.Column5.Width = 125;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.Text = "Delete";
            this.Column3.UseColumnTextForLinkValue = true;
            this.Column3.Width = 125;
            // 
            // colViewDetail
            // 
            this.colViewDetail.HeaderText = "";
            this.colViewDetail.MinimumWidth = 6;
            this.colViewDetail.Name = "colViewDetail";
            this.colViewDetail.Text = "ViewDetail";
            this.colViewDetail.UseColumnTextForLinkValue = true;
            this.colViewDetail.Width = 125;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(90, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 18);
            this.label2.TabIndex = 18;
            this.label2.Text = "Amount :";
            // 
            // txtCardNumber
            // 
            this.txtCardNumber.Location = new System.Drawing.Point(232, 147);
            this.txtCardNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCardNumber.Name = "txtCardNumber";
            this.txtCardNumber.Size = new System.Drawing.Size(220, 24);
            this.txtCardNumber.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(141, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 18);
            this.label1.TabIndex = 13;
            this.label1.Text = "Card No :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdo30000);
            this.groupBox1.Controls.Add(this.rdo18000);
            this.groupBox1.Controls.Add(this.rdo6000);
            this.groupBox1.Controls.Add(this.rdo21000);
            this.groupBox1.Controls.Add(this.rdo35000);
            this.groupBox1.Controls.Add(this.rdo70000);
            this.groupBox1.Controls.Add(this.rdo50000);
            this.groupBox1.Controls.Add(this.rdo5000);
            this.groupBox1.Controls.Add(this.lblPhoneNo);
            this.groupBox1.Controls.Add(this.lblCustomersName);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Controls.Add(this.lblMember);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtSearchCardNo);
            this.groupBox1.Controls.Add(this.lblNRIC);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.lblPhoneNumber);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblCustomerName);
            this.groupBox1.Controls.Add(this.btnNewCustomer);
            this.groupBox1.Controls.Add(this.cboCustomer);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.rdo28000);
            this.groupBox1.Controls.Add(this.rdo7000);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtCardNumber);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(802, 283);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add Gift Card";
            // 
            // rdo6000
            // 
            this.rdo6000.AutoSize = true;
            this.rdo6000.Checked = true;
            this.rdo6000.Location = new System.Drawing.Point(170, 190);
            this.rdo6000.Name = "rdo6000";
            this.rdo6000.Size = new System.Drawing.Size(61, 22);
            this.rdo6000.TabIndex = 23;
            this.rdo6000.TabStop = true;
            this.rdo6000.Text = "6000";
            this.rdo6000.UseVisualStyleBackColor = true;
            // 
            // rdo21000
            // 
            this.rdo21000.AutoSize = true;
            this.rdo21000.Location = new System.Drawing.Point(410, 190);
            this.rdo21000.Name = "rdo21000";
            this.rdo21000.Size = new System.Drawing.Size(69, 22);
            this.rdo21000.TabIndex = 22;
            this.rdo21000.Text = "21000";
            this.rdo21000.UseVisualStyleBackColor = true;
            // 
            // rdo35000
            // 
            this.rdo35000.AutoSize = true;
            this.rdo35000.Location = new System.Drawing.Point(570, 190);
            this.rdo35000.Name = "rdo35000";
            this.rdo35000.Size = new System.Drawing.Size(69, 22);
            this.rdo35000.TabIndex = 21;
            this.rdo35000.Text = "35000";
            this.rdo35000.UseVisualStyleBackColor = true;
            // 
            // rdo70000
            // 
            this.rdo70000.AutoSize = true;
            this.rdo70000.Location = new System.Drawing.Point(487, 235);
            this.rdo70000.Name = "rdo70000";
            this.rdo70000.Size = new System.Drawing.Size(69, 22);
            this.rdo70000.TabIndex = 20;
            this.rdo70000.Text = "70000";
            this.rdo70000.UseVisualStyleBackColor = true;
            this.rdo70000.Visible = false;
            // 
            // rdo50000
            // 
            this.rdo50000.AutoSize = true;
            this.rdo50000.Location = new System.Drawing.Point(401, 235);
            this.rdo50000.Name = "rdo50000";
            this.rdo50000.Size = new System.Drawing.Size(69, 22);
            this.rdo50000.TabIndex = 19;
            this.rdo50000.Text = "50000";
            this.rdo50000.UseVisualStyleBackColor = true;
            this.rdo50000.Visible = false;
            // 
            // rdo5000
            // 
            this.rdo5000.AutoSize = true;
            this.rdo5000.Checked = true;
            this.rdo5000.Location = new System.Drawing.Point(159, 190);
            this.rdo5000.Name = "rdo5000";
            this.rdo5000.Size = new System.Drawing.Size(61, 22);
            this.rdo5000.TabIndex = 3;
            this.rdo5000.TabStop = true;
            this.rdo5000.Text = "5000";
            this.rdo5000.UseVisualStyleBackColor = true;
            this.rdo5000.Visible = false;
            // 
            // lblPhoneNo
            // 
            this.lblPhoneNo.AutoSize = true;
            this.lblPhoneNo.BackColor = System.Drawing.Color.Transparent;
            this.lblPhoneNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.lblPhoneNo.Location = new System.Drawing.Point(229, 109);
            this.lblPhoneNo.Name = "lblPhoneNo";
            this.lblPhoneNo.Size = new System.Drawing.Size(13, 18);
            this.lblPhoneNo.TabIndex = 10;
            this.lblPhoneNo.Text = "-";
            // 
            // lblCustomersName
            // 
            this.lblCustomersName.AutoSize = true;
            this.lblCustomersName.BackColor = System.Drawing.Color.Transparent;
            this.lblCustomersName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.lblCustomersName.Location = new System.Drawing.Point(229, 77);
            this.lblCustomersName.Name = "lblCustomersName";
            this.lblCustomersName.Size = new System.Drawing.Size(13, 18);
            this.lblCustomersName.TabIndex = 5;
            this.lblCustomersName.Text = "-";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.label11.Location = new System.Drawing.Point(428, 109);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 18);
            this.label11.TabIndex = 11;
            this.label11.Text = "Member ID :";
            // 
            // btnRefresh
            // 
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Image = global::POS.Properties.Resources.refresh_small;
            this.btnRefresh.Location = new System.Drawing.Point(711, 141);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(85, 36);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblMember
            // 
            this.lblMember.AutoSize = true;
            this.lblMember.BackColor = System.Drawing.Color.Transparent;
            this.lblMember.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.lblMember.Location = new System.Drawing.Point(528, 109);
            this.lblMember.Name = "lblMember";
            this.lblMember.Size = new System.Drawing.Size(13, 18);
            this.lblMember.TabIndex = 12;
            this.lblMember.Text = "-";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Image = global::POS.Properties.Resources.search_small;
            this.btnSearch.Location = new System.Drawing.Point(620, 141);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(85, 36);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Visible = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.label13.Location = new System.Drawing.Point(465, 77);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 18);
            this.label13.TabIndex = 6;
            this.label13.Text = "NRIC :";
            // 
            // txtSearchCardNo
            // 
            this.txtSearchCardNo.Location = new System.Drawing.Point(467, 147);
            this.txtSearchCardNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSearchCardNo.Name = "txtSearchCardNo";
            this.txtSearchCardNo.Size = new System.Drawing.Size(147, 24);
            this.txtSearchCardNo.TabIndex = 7;
            this.txtSearchCardNo.Visible = false;
            // 
            // lblNRIC
            // 
            this.lblNRIC.AutoSize = true;
            this.lblNRIC.BackColor = System.Drawing.Color.Transparent;
            this.lblNRIC.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.lblNRIC.Location = new System.Drawing.Point(528, 77);
            this.lblNRIC.Name = "lblNRIC";
            this.lblNRIC.Size = new System.Drawing.Size(13, 18);
            this.lblNRIC.TabIndex = 7;
            this.lblNRIC.Text = "-";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.label10.Location = new System.Drawing.Point(89, 109);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(124, 18);
            this.label10.TabIndex = 8;
            this.label10.Text = "Phone Number   :";
            // 
            // lblPhoneNumber
            // 
            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.BackColor = System.Drawing.Color.Transparent;
            this.lblPhoneNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.lblPhoneNumber.Location = new System.Drawing.Point(179, 109);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(13, 18);
            this.lblPhoneNumber.TabIndex = 9;
            this.lblPhoneNumber.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.label5.Location = new System.Drawing.Point(87, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 18);
            this.label5.TabIndex = 3;
            this.label5.Text = "Customer Name :";
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.BackColor = System.Drawing.Color.Transparent;
            this.lblCustomerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.lblCustomerName.Location = new System.Drawing.Point(179, 77);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(13, 18);
            this.lblCustomerName.TabIndex = 4;
            this.lblCustomerName.Text = "-";
            // 
            // btnNewCustomer
            // 
            this.btnNewCustomer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnNewCustomer.FlatAppearance.BorderSize = 0;
            this.btnNewCustomer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnNewCustomer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnNewCustomer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewCustomer.Image = global::POS.Properties.Resources.add_small;
            this.btnNewCustomer.Location = new System.Drawing.Point(487, 36);
            this.btnNewCustomer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnNewCustomer.Name = "btnNewCustomer";
            this.btnNewCustomer.Size = new System.Drawing.Size(103, 30);
            this.btnNewCustomer.TabIndex = 1;
            this.btnNewCustomer.UseVisualStyleBackColor = true;
            this.btnNewCustomer.Click += new System.EventHandler(this.btnNewCustomer_Click);
            // 
            // cboCustomer
            // 
            this.cboCustomer.FormattingEnabled = true;
            this.cboCustomer.Location = new System.Drawing.Point(232, 39);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(220, 26);
            this.cboCustomer.TabIndex = 0;
            this.cboCustomer.SelectedIndexChanged += new System.EventHandler(this.cboCustomer_SelectedIndexChanged);
            this.cboCustomer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboCustomer_KeyDown);
            this.cboCustomer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboCustomer_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(86, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "Select Customer :";
            // 
            // rdo28000
            // 
            this.rdo28000.AutoSize = true;
            this.rdo28000.Location = new System.Drawing.Point(326, 235);
            this.rdo28000.Name = "rdo28000";
            this.rdo28000.Size = new System.Drawing.Size(69, 22);
            this.rdo28000.TabIndex = 5;
            this.rdo28000.Text = "28000";
            this.rdo28000.UseVisualStyleBackColor = true;
            this.rdo28000.Visible = false;
            // 
            // rdo7000
            // 
            this.rdo7000.AutoSize = true;
            this.rdo7000.Location = new System.Drawing.Point(250, 190);
            this.rdo7000.Name = "rdo7000";
            this.rdo7000.Size = new System.Drawing.Size(61, 22);
            this.rdo7000.TabIndex = 4;
            this.rdo7000.Text = "7000";
            this.rdo7000.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Image = global::POS.Properties.Resources.add_small;
            this.btnAdd.Location = new System.Drawing.Point(232, 231);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(103, 30);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvGiftCardList);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 430);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(802, 300);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Gift Card List";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdoFilter30000);
            this.groupBox3.Controls.Add(this.rdoFilter18000);
            this.groupBox3.Controls.Add(this.rdoFilter6000);
            this.groupBox3.Controls.Add(this.rdoFilter21000);
            this.groupBox3.Controls.Add(this.rdoFilter28000);
            this.groupBox3.Controls.Add(this.rdoFilter35000);
            this.groupBox3.Controls.Add(this.rdoFilter70000);
            this.groupBox3.Controls.Add(this.rdoFilter5000);
            this.groupBox3.Controls.Add(this.cboSearchCustomer);
            this.groupBox3.Controls.Add(this.rdoAll);
            this.groupBox3.Controls.Add(this.rdoFilter50000);
            this.groupBox3.Controls.Add(this.rdoFilter7000);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 303);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(802, 100);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search";
            // 
            // rdoFilter6000
            // 
            this.rdoFilter6000.AutoSize = true;
            this.rdoFilter6000.Location = new System.Drawing.Point(180, 28);
            this.rdoFilter6000.Name = "rdoFilter6000";
            this.rdoFilter6000.Size = new System.Drawing.Size(61, 22);
            this.rdoFilter6000.TabIndex = 27;
            this.rdoFilter6000.Text = "6000";
            this.rdoFilter6000.UseVisualStyleBackColor = true;
            this.rdoFilter6000.CheckedChanged += new System.EventHandler(this.rdoFilter6000_CheckedChanged);
            // 
            // rdoFilter21000
            // 
            this.rdoFilter21000.AutoSize = true;
            this.rdoFilter21000.Location = new System.Drawing.Point(180, 60);
            this.rdoFilter21000.Name = "rdoFilter21000";
            this.rdoFilter21000.Size = new System.Drawing.Size(69, 22);
            this.rdoFilter21000.TabIndex = 23;
            this.rdoFilter21000.Text = "21000";
            this.rdoFilter21000.UseVisualStyleBackColor = true;
            this.rdoFilter21000.CheckedChanged += new System.EventHandler(this.rdoFilter21000_CheckedChanged);
            // 
            // rdoFilter28000
            // 
            this.rdoFilter28000.AutoSize = true;
            this.rdoFilter28000.Location = new System.Drawing.Point(456, 60);
            this.rdoFilter28000.Name = "rdoFilter28000";
            this.rdoFilter28000.Size = new System.Drawing.Size(69, 22);
            this.rdoFilter28000.TabIndex = 24;
            this.rdoFilter28000.Text = "28000";
            this.rdoFilter28000.UseVisualStyleBackColor = true;
            this.rdoFilter28000.Visible = false;
            this.rdoFilter28000.CheckedChanged += new System.EventHandler(this.rdoFilter28000_CheckedChanged);
            // 
            // rdoFilter35000
            // 
            this.rdoFilter35000.AutoSize = true;
            this.rdoFilter35000.Location = new System.Drawing.Point(340, 60);
            this.rdoFilter35000.Name = "rdoFilter35000";
            this.rdoFilter35000.Size = new System.Drawing.Size(69, 22);
            this.rdoFilter35000.TabIndex = 25;
            this.rdoFilter35000.Text = "35000";
            this.rdoFilter35000.UseVisualStyleBackColor = true;
            this.rdoFilter35000.CheckedChanged += new System.EventHandler(this.rdoFilter35000_CheckedChanged);
            // 
            // rdoFilter70000
            // 
            this.rdoFilter70000.AutoSize = true;
            this.rdoFilter70000.Location = new System.Drawing.Point(620, 60);
            this.rdoFilter70000.Name = "rdoFilter70000";
            this.rdoFilter70000.Size = new System.Drawing.Size(69, 22);
            this.rdoFilter70000.TabIndex = 26;
            this.rdoFilter70000.Text = "70000";
            this.rdoFilter70000.UseVisualStyleBackColor = true;
            this.rdoFilter70000.Visible = false;
            this.rdoFilter70000.CheckedChanged += new System.EventHandler(this.rdoFilter70000_CheckedChanged);
            // 
            // rdoFilter5000
            // 
            this.rdoFilter5000.AutoSize = true;
            this.rdoFilter5000.Location = new System.Drawing.Point(201, 28);
            this.rdoFilter5000.Name = "rdoFilter5000";
            this.rdoFilter5000.Size = new System.Drawing.Size(61, 22);
            this.rdoFilter5000.TabIndex = 19;
            this.rdoFilter5000.Text = "5000";
            this.rdoFilter5000.UseVisualStyleBackColor = true;
            this.rdoFilter5000.Visible = false;
            this.rdoFilter5000.CheckedChanged += new System.EventHandler(this.rdoFilter5000_CheckedChanged);
            // 
            // cboSearchCustomer
            // 
            this.cboSearchCustomer.FormattingEnabled = true;
            this.cboSearchCustomer.Location = new System.Drawing.Point(456, 24);
            this.cboSearchCustomer.Name = "cboSearchCustomer";
            this.cboSearchCustomer.Size = new System.Drawing.Size(220, 26);
            this.cboSearchCustomer.TabIndex = 0;
            this.cboSearchCustomer.SelectedIndexChanged += new System.EventHandler(this.cboSearchCustomer_SelectedIndexChanged);
            // 
            // rdoAll
            // 
            this.rdoAll.AutoSize = true;
            this.rdoAll.Checked = true;
            this.rdoAll.Location = new System.Drawing.Point(113, 28);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(44, 22);
            this.rdoAll.TabIndex = 1;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "All";
            this.rdoAll.UseVisualStyleBackColor = true;
            this.rdoAll.CheckedChanged += new System.EventHandler(this.rdoAll_CheckedChanged_1);
            // 
            // rdoFilter50000
            // 
            this.rdoFilter50000.AutoSize = true;
            this.rdoFilter50000.Location = new System.Drawing.Point(531, 60);
            this.rdoFilter50000.Name = "rdoFilter50000";
            this.rdoFilter50000.Size = new System.Drawing.Size(69, 22);
            this.rdoFilter50000.TabIndex = 3;
            this.rdoFilter50000.Text = "50000";
            this.rdoFilter50000.UseVisualStyleBackColor = true;
            this.rdoFilter50000.Visible = false;
            this.rdoFilter50000.CheckedChanged += new System.EventHandler(this.rdoFilter50000_CheckedChanged);
            // 
            // rdoFilter7000
            // 
            this.rdoFilter7000.AutoSize = true;
            this.rdoFilter7000.Location = new System.Drawing.Point(260, 28);
            this.rdoFilter7000.Name = "rdoFilter7000";
            this.rdoFilter7000.Size = new System.Drawing.Size(61, 22);
            this.rdoFilter7000.TabIndex = 2;
            this.rdoFilter7000.Text = "7000";
            this.rdoFilter7000.UseVisualStyleBackColor = true;
            this.rdoFilter7000.CheckedChanged += new System.EventHandler(this.rdoFilter7000_CheckedChanged_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Filter By: ";
            // 
            // rdo18000
            // 
            this.rdo18000.AutoSize = true;
            this.rdo18000.Location = new System.Drawing.Point(330, 190);
            this.rdo18000.Name = "rdo18000";
            this.rdo18000.Size = new System.Drawing.Size(69, 22);
            this.rdo18000.TabIndex = 24;
            this.rdo18000.Text = "18000";
            this.rdo18000.UseVisualStyleBackColor = true;
            // 
            // rdoFilter18000
            // 
            this.rdoFilter18000.AutoSize = true;
            this.rdoFilter18000.Location = new System.Drawing.Point(340, 28);
            this.rdoFilter18000.Name = "rdoFilter18000";
            this.rdoFilter18000.Size = new System.Drawing.Size(69, 22);
            this.rdoFilter18000.TabIndex = 28;
            this.rdoFilter18000.Text = "18000";
            this.rdoFilter18000.UseVisualStyleBackColor = true;
            this.rdoFilter18000.CheckedChanged += new System.EventHandler(this.rdoFilter18000_CheckedChanged);
            // 
            // rdo30000
            // 
            this.rdo30000.AutoSize = true;
            this.rdo30000.Location = new System.Drawing.Point(490, 190);
            this.rdo30000.Name = "rdo30000";
            this.rdo30000.Size = new System.Drawing.Size(69, 22);
            this.rdo30000.TabIndex = 25;
            this.rdo30000.Text = "30000";
            this.rdo30000.UseVisualStyleBackColor = true;
            // 
            // rdoFilter30000
            // 
            this.rdoFilter30000.AutoSize = true;
            this.rdoFilter30000.Location = new System.Drawing.Point(260, 60);
            this.rdoFilter30000.Name = "rdoFilter30000";
            this.rdoFilter30000.Size = new System.Drawing.Size(69, 22);
            this.rdoFilter30000.TabIndex = 29;
            this.rdoFilter30000.Text = "30000";
            this.rdoFilter30000.UseVisualStyleBackColor = true;
            this.rdoFilter30000.CheckedChanged += new System.EventHandler(this.rdoFilter30000_CheckedChanged);
            // 
            // GiftCardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(829, 732);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "GiftCardControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add New GiftCard";
            this.Activated += new System.EventHandler(this.GiftCardControl_Activated);
            this.Load += new System.EventHandler(this.GiftCardControl_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GiftCardControl_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGiftCardList)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvGiftCardList;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtCardNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtSearchCardNo;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cboCustomer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rdo28000;
        private System.Windows.Forms.RadioButton rdo7000;
        private System.Windows.Forms.Button btnNewCustomer;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblPhoneNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblMember;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblNRIC;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewLinkColumn Column5;
        private System.Windows.Forms.DataGridViewLinkColumn Column3;
        private System.Windows.Forms.DataGridViewLinkColumn colViewDetail;
        private System.Windows.Forms.Label lblPhoneNo;
        private System.Windows.Forms.Label lblCustomersName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cboSearchCustomer;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.RadioButton rdoFilter50000;
        private System.Windows.Forms.RadioButton rdoFilter7000;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rdo5000;
        private System.Windows.Forms.RadioButton rdoFilter5000;
        private System.Windows.Forms.RadioButton rdo21000;
        private System.Windows.Forms.RadioButton rdo35000;
        private System.Windows.Forms.RadioButton rdo70000;
        private System.Windows.Forms.RadioButton rdo50000;
        private System.Windows.Forms.RadioButton rdoFilter21000;
        private System.Windows.Forms.RadioButton rdoFilter28000;
        private System.Windows.Forms.RadioButton rdoFilter35000;
        private System.Windows.Forms.RadioButton rdoFilter70000;
        private System.Windows.Forms.RadioButton rdoFilter6000;
        private System.Windows.Forms.RadioButton rdo6000;
        private System.Windows.Forms.RadioButton rdo18000;
        private System.Windows.Forms.RadioButton rdoFilter18000;
        private System.Windows.Forms.RadioButton rdo30000;
        private System.Windows.Forms.RadioButton rdoFilter30000;
    }
}