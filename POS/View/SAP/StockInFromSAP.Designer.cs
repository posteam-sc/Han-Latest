
namespace POS
{
    partial class StockInFromSAP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StockInFromSAP));
            this.rdbAll = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbActive = new System.Windows.Forms.RadioButton();
            this.grpStockInDate = new System.Windows.Forms.GroupBox();
            this.btnStockSearch = new System.Windows.Forms.Button();
            this.EndDatedateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.StartDatedateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ColActive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColAvailableQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpPCode = new System.Windows.Forms.GroupBox();
            this.btnProductCodeSearch = new System.Windows.Forms.Button();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.grpPName = new System.Windows.Forms.GroupBox();
            this.btnProductNameSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvStockInFromSAP = new System.Windows.Forms.DataGridView();
            this.groupBox1.SuspendLayout();
            this.grpStockInDate.SuspendLayout();
            this.grpPCode.SuspendLayout();
            this.grpPName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockInFromSAP)).BeginInit();
            this.SuspendLayout();
            // 
            // rdbAll
            // 
            this.rdbAll.AutoSize = true;
            this.rdbAll.Checked = true;
            this.rdbAll.Location = new System.Drawing.Point(19, 27);
            this.rdbAll.Name = "rdbAll";
            this.rdbAll.Size = new System.Drawing.Size(36, 17);
            this.rdbAll.TabIndex = 0;
            this.rdbAll.TabStop = true;
            this.rdbAll.Text = "All";
            this.rdbAll.UseVisualStyleBackColor = true;
            this.rdbAll.CheckedChanged += new System.EventHandler(this.rdbAll_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbActive);
            this.groupBox1.Controls.Add(this.rdbAll);
            this.groupBox1.Location = new System.Drawing.Point(754, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(166, 69);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter By";
            // 
            // rdbActive
            // 
            this.rdbActive.AutoSize = true;
            this.rdbActive.Location = new System.Drawing.Point(103, 27);
            this.rdbActive.Name = "rdbActive";
            this.rdbActive.Size = new System.Drawing.Size(55, 17);
            this.rdbActive.TabIndex = 1;
            this.rdbActive.Text = "Active";
            this.rdbActive.UseVisualStyleBackColor = true;
            this.rdbActive.CheckedChanged += new System.EventHandler(this.rdbActive_CheckedChanged);
            // 
            // grpStockInDate
            // 
            this.grpStockInDate.Controls.Add(this.btnStockSearch);
            this.grpStockInDate.Controls.Add(this.EndDatedateTimePicker);
            this.grpStockInDate.Controls.Add(this.StartDatedateTimePicker);
            this.grpStockInDate.Controls.Add(this.label2);
            this.grpStockInDate.Controls.Add(this.label1);
            this.grpStockInDate.Location = new System.Drawing.Point(11, 30);
            this.grpStockInDate.Name = "grpStockInDate";
            this.grpStockInDate.Size = new System.Drawing.Size(720, 69);
            this.grpStockInDate.TabIndex = 21;
            this.grpStockInDate.TabStop = false;
            this.grpStockInDate.Text = "Stock In Date";
            // 
            // btnStockSearch
            // 
            this.btnStockSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnStockSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnStockSearch.FlatAppearance.BorderSize = 0;
            this.btnStockSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnStockSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnStockSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStockSearch.Image = global::POS.Properties.Resources.search_small;
            this.btnStockSearch.Location = new System.Drawing.Point(598, 6);
            this.btnStockSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStockSearch.Name = "btnStockSearch";
            this.btnStockSearch.Size = new System.Drawing.Size(89, 62);
            this.btnStockSearch.TabIndex = 11;
            this.btnStockSearch.UseVisualStyleBackColor = false;
            this.btnStockSearch.Click += new System.EventHandler(this.btnStockSearch_Click);
            // 
            // EndDatedateTimePicker
            // 
            this.EndDatedateTimePicker.Location = new System.Drawing.Point(356, 29);
            this.EndDatedateTimePicker.Name = "EndDatedateTimePicker";
            this.EndDatedateTimePicker.Size = new System.Drawing.Size(220, 20);
            this.EndDatedateTimePicker.TabIndex = 1;
            // 
            // StartDatedateTimePicker
            // 
            this.StartDatedateTimePicker.Location = new System.Drawing.Point(69, 29);
            this.StartDatedateTimePicker.Name = "StartDatedateTimePicker";
            this.StartDatedateTimePicker.Size = new System.Drawing.Size(220, 20);
            this.StartDatedateTimePicker.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(303, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "To Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "From Date";
            // 
            // ColActive
            // 
            this.ColActive.DataPropertyName = "IsActive";
            this.ColActive.HeaderText = "Active";
            this.ColActive.Name = "ColActive";
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "CreatedDate";
            dataGridViewCellStyle1.Format = "d";
            dataGridViewCellStyle1.NullValue = null;
            this.Column6.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column6.HeaderText = "Stock In Date";
            this.Column6.Name = "Column6";
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "ExpireDate";
            dataGridViewCellStyle2.Format = "d";
            dataGridViewCellStyle2.NullValue = null;
            this.Column5.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column5.HeaderText = "Expire Date";
            this.Column5.Name = "Column5";
            // 
            // ColAvailableQty
            // 
            this.ColAvailableQty.DataPropertyName = "AvailableQty";
            this.ColAvailableQty.HeaderText = "Available Qty";
            this.ColAvailableQty.Name = "ColAvailableQty";
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "ProductQty";
            this.Column4.HeaderText = "StockIn Qty";
            this.Column4.Name = "Column4";
            // 
            // ColPrice
            // 
            this.ColPrice.DataPropertyName = "Price";
            this.ColPrice.HeaderText = "Price";
            this.ColPrice.Name = "ColPrice";
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "BatchNo";
            this.Column1.HeaderText = "Batch No.";
            this.Column1.Name = "Column1";
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "Name";
            this.Column3.HeaderText = "Product Name";
            this.Column3.Name = "Column3";
            this.Column3.Width = 250;
            // 
            // grpPCode
            // 
            this.grpPCode.Controls.Add(this.btnProductCodeSearch);
            this.grpPCode.Controls.Add(this.txtProductCode);
            this.grpPCode.Controls.Add(this.label4);
            this.grpPCode.Location = new System.Drawing.Point(10, 115);
            this.grpPCode.Name = "grpPCode";
            this.grpPCode.Size = new System.Drawing.Size(357, 69);
            this.grpPCode.TabIndex = 23;
            this.grpPCode.TabStop = false;
            this.grpPCode.Text = "Product Code";
            // 
            // btnProductCodeSearch
            // 
            this.btnProductCodeSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnProductCodeSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnProductCodeSearch.FlatAppearance.BorderSize = 0;
            this.btnProductCodeSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnProductCodeSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnProductCodeSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProductCodeSearch.Image = global::POS.Properties.Resources.search_small;
            this.btnProductCodeSearch.Location = new System.Drawing.Point(263, 7);
            this.btnProductCodeSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnProductCodeSearch.Name = "btnProductCodeSearch";
            this.btnProductCodeSearch.Size = new System.Drawing.Size(89, 62);
            this.btnProductCodeSearch.TabIndex = 12;
            this.btnProductCodeSearch.UseVisualStyleBackColor = false;
            this.btnProductCodeSearch.Click += new System.EventHandler(this.btnProductCodeSearch_Click);
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(88, 29);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(170, 20);
            this.txtProductCode.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Product Code:";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(88, 29);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(170, 20);
            this.txtProductName.TabIndex = 12;
            // 
            // grpPName
            // 
            this.grpPName.Controls.Add(this.btnProductNameSearch);
            this.grpPName.Controls.Add(this.txtProductName);
            this.grpPName.Controls.Add(this.label3);
            this.grpPName.Location = new System.Drawing.Point(374, 116);
            this.grpPName.Name = "grpPName";
            this.grpPName.Size = new System.Drawing.Size(357, 69);
            this.grpPName.TabIndex = 25;
            this.grpPName.TabStop = false;
            this.grpPName.Text = "Product Name";
            // 
            // btnProductNameSearch
            // 
            this.btnProductNameSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnProductNameSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnProductNameSearch.FlatAppearance.BorderSize = 0;
            this.btnProductNameSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnProductNameSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnProductNameSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProductNameSearch.Image = global::POS.Properties.Resources.search_small;
            this.btnProductNameSearch.Location = new System.Drawing.Point(263, 7);
            this.btnProductNameSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnProductNameSearch.Name = "btnProductNameSearch";
            this.btnProductNameSearch.Size = new System.Drawing.Size(89, 62);
            this.btnProductNameSearch.TabIndex = 12;
            this.btnProductNameSearch.UseVisualStyleBackColor = false;
            this.btnProductNameSearch.Click += new System.EventHandler(this.btnProductNameSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Product Name:";
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "ProductCode";
            this.Column2.HeaderText = "Product Code";
            this.Column2.Name = "Column2";
            // 
            // dgvStockInFromSAP
            // 
            this.dgvStockInFromSAP.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockInFromSAP.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.Column3,
            this.Column1,
            this.ColPrice,
            this.Column4,
            this.ColAvailableQty,
            this.Column5,
            this.Column6,
            this.ColActive});
            this.dgvStockInFromSAP.Location = new System.Drawing.Point(2, 205);
            this.dgvStockInFromSAP.Name = "dgvStockInFromSAP";
            this.dgvStockInFromSAP.RowTemplate.ReadOnly = true;
            this.dgvStockInFromSAP.Size = new System.Drawing.Size(1095, 326);
            this.dgvStockInFromSAP.TabIndex = 22;
            this.dgvStockInFromSAP.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvStockInFromSAP_DataBindingComplete);
            // 
            // StockInFromSAP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1116, 561);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpStockInDate);
            this.Controls.Add(this.grpPCode);
            this.Controls.Add(this.grpPName);
            this.Controls.Add(this.dgvStockInFromSAP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StockInFromSAP";
            this.Text = "Stock In From SAP";
            this.Load += new System.EventHandler(this.StockInFromSAP_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpStockInDate.ResumeLayout(false);
            this.grpStockInDate.PerformLayout();
            this.grpPCode.ResumeLayout(false);
            this.grpPCode.PerformLayout();
            this.grpPName.ResumeLayout(false);
            this.grpPName.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockInFromSAP)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbActive;
        private System.Windows.Forms.GroupBox grpStockInDate;
        private System.Windows.Forms.Button btnStockSearch;
        private System.Windows.Forms.DateTimePicker EndDatedateTimePicker;
        private System.Windows.Forms.DateTimePicker StartDatedateTimePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColAvailableQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.GroupBox grpPCode;
        private System.Windows.Forms.Button btnProductCodeSearch;
        private System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.GroupBox grpPName;
        private System.Windows.Forms.Button btnProductNameSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridView dgvStockInFromSAP;
    }
}