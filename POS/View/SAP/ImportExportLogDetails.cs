using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;

namespace POS
{
    public partial class ImportExportLogDetails : Form
    {
        #region Variables
        public string BatchNo { get; set; }
        public long BatchID { get; set; }
        public string Type { get; set; }

        #endregion
        #region Events
        public ImportExportLogDetails()
        {
            InitializeComponent();
        }

        private void ImportExportLogDetails_Load(object sender, EventArgs e)
        {
            lblBatchLable.Text = "";
            grpLogDetails.Text = "";
            lblBatchNo.Text = "";
            lblBatchLable.Text = Type == "Import" ? "Import Batch No.:" : "Export Batch No.:";
            grpLogDetails.Text = Type == "Import" ? "Import Log Details:" : "Export Log Details:";
            lblBatchNo.Text = BatchNo;
            LoadData();

        }

        #endregion
        #region Method
        public void LoadData()
        {
            POSEntities entity = new POSEntities();
            List<APP_Data.ImportExportLogDetail> LogDetailList = entity.ImportExportLogDetails.Where(x => x.ProcessingBatchID == BatchID).ToList();

            dgvImportExportDetail.DataSource = "";
            dgvImportExportDetail.AutoGenerateColumns = false;
            dgvImportExportDetail.DataSource = LogDetailList;
        }

        #endregion

        private void dgvImportExportDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (e.ColumnIndex == ColJson.Index)
                {
                    var saveJson = dgvImportExportDetail.Rows[e.RowIndex].Cells[ColJsonText.Index].Value;
                    if (saveJson != null)
                    {
                        PostJson jsonForm = new PostJson();
                        jsonForm.batchNo = lblBatchNo.Text;
                        jsonForm.API_Name = dgvImportExportDetail.Rows[e.RowIndex].Cells[colAPIName.Index].Value.ToString();
                        jsonForm.Json = saveJson.ToString();
                        jsonForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No Data");
                    }

                }

            }
        }

        private void dgvImportExportDetail_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (BatchNo.Contains("IM"))
            {
                dgvImportExportDetail.Columns[ColJson.Index].Visible = false;
                dgvImportExportDetail.Size = new System.Drawing.Size(625, 159);
            }
            else
            {
                dgvImportExportDetail.Columns[ColJson.Index].Visible = true;
                dgvImportExportDetail.Size = new System.Drawing.Size(725, 159);
            }
        }


    }
}
