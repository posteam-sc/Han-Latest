using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Objects;
using POS.APP_Data;

namespace POS.View.LoyaltyPts
{
    public partial class ImportExportMemberTransHistory : Form
    {
        public ImportExportMemberTransHistory()
        {
            InitializeComponent();
        }

        public void BindImportExportHistory()
        {
            POSEntities entity = new POSEntities();
            DateTime startDate = StartDatedateTimePicker.Value.Date;
            DateTime endDate = EndDatedateTimePicker.Value.Date;
            List<GetImportExportMemberTransHistory_Result> ExportHistoryList=entity.GetImportExportMemberTransHistory(startDate, endDate).ToList();

            dgvExportHistory.DataSource = "";
            dgvExportHistory.AutoGenerateColumns = false;
            dgvExportHistory.DataSource = ExportHistoryList.Where(x => x.Type == "Export").ToList();
        }

        private void ImportExportMemberTransHistory_Load(object sender, EventArgs e)
        {
            BindImportExportHistory();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindImportExportHistory();
        }

        private void dgvExportHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (e.ColumnIndex == colEDetails.Index)
                {
                    ImportExportMemberTransLogDetails logDetailForm = new ImportExportMemberTransLogDetails();
                    logDetailForm.BatchID = (long)dgvExportHistory.Rows[e.RowIndex].Cells[colEBatchID.Index].Value;
                    logDetailForm.BatchNo = dgvExportHistory.Rows[e.RowIndex].Cells[colEBatch.Index].Value.ToString();
                    logDetailForm.Type = "Export";
                    logDetailForm.ShowDialog();
                }

                if (e.ColumnIndex == colEExport.Index)
                {       
                    string status = "";
                    status= dgvExportHistory.Rows[e.RowIndex].Cells[ColEStatus.Index].Value.ToString();
                    if (status=="Success")
                    {
                        DialogResult result = MessageBox.Show("Already exported!", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string TransId = "";
                        TransId = (string)dgvExportHistory.Rows[e.RowIndex].Cells[colETransId.Index].Value;

                        MemberTransExport exportMemberTransForm = new MemberTransExport(TransId);
                        exportMemberTransForm.Text = "Export Transactions";
                        exportMemberTransForm.ShowDialog();

                        BindImportExportHistory();
                    }
                }
            }
        }

        private void dgvExportHistory_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvExportHistory.Rows)
            {
                //ImportExportLog log = (ImportExportLog)row.DataBoundItem;
                GetImportExportMemberTransHistory_Result log = (GetImportExportMemberTransHistory_Result)row.DataBoundItem;
                row.Cells[colEBatch.Index].Value = log.ProcessingBatch;
                row.Cells[ColEDate.Index].Value = log.ProcessingDateTime;
                row.Cells[ColELastProcessingDateTime.Index].Value = log.LastProcessingDateTime;
                row.Cells[ColEType.Index].Value = log.Type;
                row.Cells[ColEStatus.Index].Value = log.Status;
                row.Cells[colEBatchID.Index].Value = log.Id;
                //row.Cells[colETransId.Index].Value = log.TransactionId;

                POSEntities entity = new POSEntities();
                APP_Data.ImportExportMemberTransLog exlog = new APP_Data.ImportExportMemberTransLog();
                long BatchId = 0;
                BatchId = (long)row.Cells[colEBatchID.Index].Value;
                exlog = entity.ImportExportMemberTransLogs.Where(x => x.Id== BatchId).FirstOrDefault();
                if (exlog != null)
                {
                    row.Cells[colETransId.Index].Value = exlog.TransactionId;
                }
            }
        }
    }
}
