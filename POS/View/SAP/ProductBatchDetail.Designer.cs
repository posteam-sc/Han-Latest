
namespace POS
{
    partial class ProductBatchDetial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductBatchDetial));
            this.dgvBatchDetails = new System.Windows.Forms.DataGridView();
            this.ColBatchNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColTotalQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColAvailableQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColExpireDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblAvailableQty = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotalQty = new System.Windows.Forms.Label();
            this.lblProductName = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatchDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvBatchDetails
            // 
            this.dgvBatchDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBatchDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColBatchNo,
            this.ColTotalQty,
            this.ColAvailableQty,
            this.ColExpireDate});
            this.dgvBatchDetails.Location = new System.Drawing.Point(27, 116);
            this.dgvBatchDetails.Name = "dgvBatchDetails";
            this.dgvBatchDetails.RowTemplate.ReadOnly = true;
            this.dgvBatchDetails.Size = new System.Drawing.Size(495, 210);
            this.dgvBatchDetails.TabIndex = 6;
            this.dgvBatchDetails.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvBatchDetails_DataBindingComplete);
            // 
            // ColBatchNo
            // 
            this.ColBatchNo.HeaderText = "Batch No.";
            this.ColBatchNo.Name = "ColBatchNo";
            // 
            // ColTotalQty
            // 
            this.ColTotalQty.HeaderText = "Total Quantity";
            this.ColTotalQty.Name = "ColTotalQty";
            // 
            // ColAvailableQty
            // 
            this.ColAvailableQty.HeaderText = "Available Quantity";
            this.ColAvailableQty.Name = "ColAvailableQty";
            this.ColAvailableQty.Width = 150;
            // 
            // ColExpireDate
            // 
            this.ColExpireDate.HeaderText = "Expire Date";
            this.ColExpireDate.Name = "ColExpireDate";
            // 
            // lblAvailableQty
            // 
            this.lblAvailableQty.AutoSize = true;
            this.lblAvailableQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAvailableQty.Location = new System.Drawing.Point(132, 88);
            this.lblAvailableQty.Name = "lblAvailableQty";
            this.lblAvailableQty.Size = new System.Drawing.Size(11, 13);
            this.lblAvailableQty.TabIndex = 20;
            this.lblAvailableQty.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Available Quanties:";
            // 
            // lblTotalQty
            // 
            this.lblTotalQty.AutoSize = true;
            this.lblTotalQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalQty.Location = new System.Drawing.Point(128, 59);
            this.lblTotalQty.Name = "lblTotalQty";
            this.lblTotalQty.Size = new System.Drawing.Size(11, 13);
            this.lblTotalQty.TabIndex = 15;
            this.lblTotalQty.Text = "-";
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductName.Location = new System.Drawing.Point(114, 28);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(11, 13);
            this.lblProductName.TabIndex = 16;
            this.lblProductName.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Product Quanties:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Product Name:";
            // 
            // ProductBatchDetial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 354);
            this.Controls.Add(this.lblAvailableQty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblTotalQty);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvBatchDetails);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProductBatchDetial";
            this.Text = "Product Batch Details";
            this.Load += new System.EventHandler(this.ProductBatchDetial_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatchDetails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBatchDetails;
        private System.Windows.Forms.Label lblAvailableQty;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalQty;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColBatchNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTotalQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColAvailableQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColExpireDate;
    }
}