namespace POS
{
    partial class ImportExportMemberTransLogDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportExportMemberTransLogDetails));
            this.lblBatchLable = new System.Windows.Forms.Label();
            this.lblBatchNo = new System.Windows.Forms.Label();
            this.grpLogDetails = new System.Windows.Forms.GroupBox();
            this.dgvImportExportMemberTransDetail = new System.Windows.Forms.DataGridView();
            this.colAPIName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColJson = new System.Windows.Forms.DataGridViewLinkColumn();
            this.ColJsonText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpLogDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvImportExportMemberTransDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBatchLable
            // 
            this.lblBatchLable.AutoSize = true;
            this.lblBatchLable.Location = new System.Drawing.Point(31, 36);
            this.lblBatchLable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBatchLable.Name = "lblBatchLable";
            this.lblBatchLable.Size = new System.Drawing.Size(91, 17);
            this.lblBatchLable.TabIndex = 19;
            this.lblBatchLable.Text = "Import Batch:";
            // 
            // lblBatchNo
            // 
            this.lblBatchNo.AutoSize = true;
            this.lblBatchNo.Location = new System.Drawing.Point(148, 36);
            this.lblBatchNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBatchNo.Name = "lblBatchNo";
            this.lblBatchNo.Size = new System.Drawing.Size(62, 17);
            this.lblBatchNo.TabIndex = 20;
            this.lblBatchNo.Text = "BatchNo";
            // 
            // grpLogDetails
            // 
            this.grpLogDetails.Controls.Add(this.dgvImportExportMemberTransDetail);
            this.grpLogDetails.Location = new System.Drawing.Point(16, 80);
            this.grpLogDetails.Margin = new System.Windows.Forms.Padding(4);
            this.grpLogDetails.Name = "grpLogDetails";
            this.grpLogDetails.Padding = new System.Windows.Forms.Padding(4);
            this.grpLogDetails.Size = new System.Drawing.Size(995, 252);
            this.grpLogDetails.TabIndex = 21;
            this.grpLogDetails.TabStop = false;
            this.grpLogDetails.Text = "Export Log Details:";
            // 
            // dgvImportExportMemberTransDetail
            // 
            this.dgvImportExportMemberTransDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvImportExportMemberTransDetail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAPIName,
            this.colStatus,
            this.colMessage,
            this.ColJson,
            this.ColJsonText});
            this.dgvImportExportMemberTransDetail.Location = new System.Drawing.Point(19, 31);
            this.dgvImportExportMemberTransDetail.Margin = new System.Windows.Forms.Padding(4);
            this.dgvImportExportMemberTransDetail.Name = "dgvImportExportMemberTransDetail";
            this.dgvImportExportMemberTransDetail.Size = new System.Drawing.Size(967, 196);
            this.dgvImportExportMemberTransDetail.TabIndex = 11;
            this.dgvImportExportMemberTransDetail.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvImportExportMemberTransDetail_CellClick);
            this.dgvImportExportMemberTransDetail.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvImportExportMemberTransDetail_DataBindingComplete);
            // 
            // colAPIName
            // 
            this.colAPIName.DataPropertyName = "ProcessName";
            this.colAPIName.HeaderText = "API Name";
            this.colAPIName.Name = "colAPIName";
            this.colAPIName.Width = 150;
            // 
            // colStatus
            // 
            this.colStatus.DataPropertyName = "DetailStatus";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 80;
            // 
            // colMessage
            // 
            this.colMessage.DataPropertyName = "ResponseMessageFromPiti";
            this.colMessage.HeaderText = "Response Message From Piti";
            this.colMessage.Name = "colMessage";
            this.colMessage.Width = 350;
            // 
            // ColJson
            // 
            this.ColJson.HeaderText = "";
            this.ColJson.Name = "ColJson";
            this.ColJson.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColJson.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColJson.Text = "View Json";
            this.ColJson.UseColumnTextForLinkValue = true;
            // 
            // ColJsonText
            // 
            this.ColJsonText.DataPropertyName = "PostJson";
            this.ColJsonText.HeaderText = "Json";
            this.ColJsonText.Name = "ColJsonText";
            this.ColJsonText.Visible = false;
            // 
            // ImportExportMemberTransLogDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1027, 394);
            this.Controls.Add(this.grpLogDetails);
            this.Controls.Add(this.lblBatchNo);
            this.Controls.Add(this.lblBatchLable);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImportExportMemberTransLogDetails";
            this.Text = " Export Transactions Log Details";
            this.Load += new System.EventHandler(this.ImportExportMemberTransLogDetails_Load);
            this.grpLogDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImportExportMemberTransDetail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBatchLable;
        private System.Windows.Forms.Label lblBatchNo;
        private System.Windows.Forms.GroupBox grpLogDetails;
        private System.Windows.Forms.DataGridView dgvImportExportMemberTransDetail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAPIName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMessage;
        private System.Windows.Forms.DataGridViewLinkColumn ColJson;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColJsonText;
    }
}