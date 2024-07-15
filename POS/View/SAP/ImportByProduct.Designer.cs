
namespace POS
{
    partial class ImportByProduct
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportByProduct));
            this.dgvProductList = new System.Windows.Forms.DataGridView();
            this.colProductCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewLinkColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.cboProductName = new System.Windows.Forms.ComboBox();
            this.btnGetDataByProduct = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvProductList
            // 
            this.dgvProductList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProductCode,
            this.colProductName,
            this.colDelete});
            this.dgvProductList.Location = new System.Drawing.Point(22, 71);
            this.dgvProductList.Name = "dgvProductList";
            this.dgvProductList.Size = new System.Drawing.Size(595, 184);
            this.dgvProductList.TabIndex = 9;
            // 
            // colProductCode
            // 
            this.colProductCode.HeaderText = "Product Code";
            this.colProductCode.Name = "colProductCode";
            this.colProductCode.Width = 150;
            // 
            // colProductName
            // 
            this.colProductName.HeaderText = "Product Name";
            this.colProductName.Name = "colProductName";
            this.colProductName.Width = 300;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "";
            this.colDelete.Name = "colDelete";
            this.colDelete.Text = "Delete";
            this.colDelete.UseColumnTextForLinkValue = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Product Name:";
            // 
            // cboProductName
            // 
            this.cboProductName.FormattingEnabled = true;
            this.cboProductName.Location = new System.Drawing.Point(145, 29);
            this.cboProductName.Name = "cboProductName";
            this.cboProductName.Size = new System.Drawing.Size(121, 21);
            this.cboProductName.TabIndex = 6;
            this.cboProductName.SelectedIndexChanged += new System.EventHandler(this.cboProductName_SelectedIndexChanged);
            // 
            // btnGetDataByProduct
            // 
            this.btnGetDataByProduct.BackColor = System.Drawing.SystemColors.Control;
            this.btnGetDataByProduct.FlatAppearance.BorderSize = 0;
            this.btnGetDataByProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetDataByProduct.ForeColor = System.Drawing.Color.Black;
            this.btnGetDataByProduct.Image = global::POS.Properties.Resources.imelc;
            this.btnGetDataByProduct.Location = new System.Drawing.Point(288, 275);
            this.btnGetDataByProduct.Name = "btnGetDataByProduct";
            this.btnGetDataByProduct.Size = new System.Drawing.Size(115, 37);
            this.btnGetDataByProduct.TabIndex = 8;
            this.btnGetDataByProduct.UseVisualStyleBackColor = false;
            this.btnGetDataByProduct.Click += new System.EventHandler(this.btnGetDataByProduct_Click);
            // 
            // ImportByProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 344);
            this.Controls.Add(this.dgvProductList);
            this.Controls.Add(this.btnGetDataByProduct);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboProductName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImportByProduct";
            this.Text = "Import Data By Product";
            this.Load += new System.EventHandler(this.ImportByProduct_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvProductList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboProductName;
        private System.Windows.Forms.Button btnGetDataByProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductName;
        private System.Windows.Forms.DataGridViewLinkColumn colDelete;
    }
}