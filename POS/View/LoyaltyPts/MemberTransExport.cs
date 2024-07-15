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
using System.Net;
using POS.APP_Data;
using System.Configuration;
using System.Runtime;
using System.Runtime.InteropServices;

namespace POS
{
    public partial class MemberTransExport : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        #region Variables
        POSEntities entity = new POSEntities();
        string TransId = "";
        string memberDisplayName = "";
        bool IsExportSuccess;
        public static bool IsAllExportSuccess { get; set; } = true;
        bool postMemberTransNoExport;
        bool postMemberTransSuccess;
        public bool IsBackDateExport;
        public bool IsAutoExport;
        public bool IsNoDataToExport = false;

        #endregion

        public MemberTransExport(string TransactionId)
        {
            InitializeComponent();
            TransId = TransactionId;
        }

        private void DataExport_Load(object sender, EventArgs e)
        {
            this.Text = "Sales Transaction Export";
            this.Activated += AfterLoading;
        }

        private void AfterLoading(object sender, EventArgs e)
        {
            this.Activated -= AfterLoading;
            ExportDataToPiti();
        }

        public void ExportDataToPiti()
        {
            string shortCode = SettingController.DefaultShop.ShortCode;

            DateTime executeDate = DateTime.Today; // just initialization
            long executeLogID = 0;
            IsAllExportSuccess = true;
            API_POST_MemberTrans.ResponseMemberDisplayName = "";

            #region Export Trans
            this.Text = "Sales Transaction Export";
            List<DateTime?> todayExportList = new List<DateTime?>();
            todayExportList = (from t in entity.Transactions
                               where t.Id == TransId
                                && t.IsDeleted == false && t.IsComplete == true && t.Type == "Sale" && t.Id.Contains(shortCode)
                               select EntityFunctions.TruncateTime((DateTime)t.DateTime)).Distinct().ToList();

            if (todayExportList.Count > 0)
            {
                DateTime dt = DateTime.Now;
                lblExportDate.Text = dt.ToString("dd/MM/yyyy");

                APP_Data.ImportExportMemberTransLog exlogdetail = new APP_Data.ImportExportMemberTransLog();
                exlogdetail = entity.ImportExportMemberTransLogs.Where(x => x.TransactionId == TransId).FirstOrDefault();

                if (exlogdetail == null)
                {
                    int resultID = 0;
                    resultID = ImportExportMemberTransLog.CreateNewExportBatch(dt, TransId);
                    if (resultID == 0)
                    {
                        lblExportProgress.Text = "New Export Log Fails..Data Export Cannot be Started...Please Try Again...!";
                        return;
                    }
                    postMemberTransNoExport = true;
                    IsExportSuccess = false;
                    executeDate = dt;
                    executeLogID = resultID;
                }
                else
                {
                    APP_Data.ImportExportMemberTransLog exlog = new APP_Data.ImportExportMemberTransLog();
                    exlog = entity.ImportExportMemberTransLogs.Where(x => x.TransactionId == TransId && x.Type == "Export").FirstOrDefault();
                    if (exlog != null)
                    {
                        switch (exlog.Status)
                        {
                            case "Success":
                                IsExportSuccess = true;
                                break;
                            case "Pending":
                                postMemberTransNoExport = true;
                                IsExportSuccess = false;
                                executeDate = (DateTime)exlog.ProcessingDateTime;
                                executeLogID = exlog.Id;
                                break;
                            case "Fail":
                                List<ImportExportMemberTransLogDetail> ExlogDetails = new List<ImportExportMemberTransLogDetail>();
                                ExlogDetails = entity.ImportExportMemberTransLogDetails.Where(x => x.ProcessingBatchID == exlog.Id && x.DetailStatus == "Fail").ToList();
                                foreach (ImportExportMemberTransLogDetail exDetails in ExlogDetails)
                                {
                                    switch (exDetails.ProcessName)
                                    {
                                        case "POST_MemberTrans":
                                            postMemberTransNoExport = true;
                                            break;
                                    }
                                }
                                IsExportSuccess = false;
                                executeDate = (DateTime)(exlog.ProcessingDateTime);
                                executeLogID = exlog.Id;
                                break;
                        }
                    }
                }
                if (!IsExportSuccess)
                {
                    bool IsConnected = Utility.CheckInternetAndServerConnectionPiti();
                    if (!IsConnected)
                    {
                        if (IsAutoExport)
                        {
                            Login.IsBackDateExportSuccess = false;
                        }
                        else
                        {
                            IsAllExportSuccess = false;
                        }

                        this.Dispose();
                        return;
                    }
                    UpdateTransactionsStatus();
                    ExecuteAPIs(executeDate);
                    UpdateLogStatus(executeLogID);
                }
                if (IsExportSuccess)
                {
                    lblExportProgress.Text = "Data Export For " + dt.Date.ToString("dd/MM/yyyy") + " Completed Successfullly..";
                }
                else
                {
                    IsAllExportSuccess = false;
                    lblExportProgress.Text = "Data Export For " + dt.Date.ToString("dd/MM/yyyy") + " Finished with Error..!";
                }

                if (IsAllExportSuccess)
                {
                    lblExportProgress.Text = "Data Export Completed Successfully";
                    MessageBox.Show("Data Export Completed Successfully", "Data Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Dispose();
                }
                else
                {
                    lblExportProgress.Text = "Data Export Completed with Failures..!";
                    MessageBox.Show("Data Export Completed with Failures..!", "Data Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Dispose();
                    // Try Export Again 
                }
            }
            else
            {
                lblExportProgress.Text = "No Data to Export";
                IsNoDataToExport = true;
                MessageBox.Show("No data to export..!", "Data Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }

            #endregion
        }

        public void ExecuteAPIs(DateTime postDate)
        {
            int no0fProgressSteps = 4;
            int i = 1;
            ExportProgressBar.Value = 0; // reset progressbar
            ExportProgressBar.Maximum = no0fProgressSteps;
            postDate = postDate.Date;
            IsExportSuccess = true;
            postMemberTransSuccess = true;

            ShowStatus("Step 1 of 4: Getting Access API Key...", ++i, true);
            //API_Token.Get_AccessToken();
            //if (string.IsNullOrEmpty(API_Token.AccessToken) || string.IsNullOrWhiteSpace(API_Token.AccessToken))
            //{
            //    ShowStatus(" Step 1 of 4: Getting Access API Key Fails!..Cannot Export Data...", i, false);
            //    IsExportSuccess = false;
            //    postMemberTransSuccess = false;
            //    API_POST_MemberTrans.postMemberTransResponseMessage.Add("No Access API Key");
            //    return; // exit import and update log table status to 'fail'
            //}

            ShowStatus("Step 1 of 3: API Key Received Successfully!", i, true);
            if (postMemberTransNoExport)
            {
                ShowStatus("Step 2 of 3: Sending Member Transactions..Please Wait...", ++i, true);
                API_POST_MemberTrans.POST_MemberTrans(TransId);

                memberDisplayName = API_POST_MemberTrans.ResponseMemberDisplayName;
                UpdateMemberDisplayName(TransId, memberDisplayName);

                if (!API_POST_MemberTrans.postMemberTransSuccess)
                {
                    if (API_POST_MemberTrans.response != null)
                    {
                        if (API_POST_MemberTrans.response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            MessageBox.Show("Unauthorized Access! Please Refresh Access API Key", "Forbidden Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 2 of 3: Sending Member Transactions Fails...", i, false);
                            IsExportSuccess = false;
                            postMemberTransSuccess = false;
                        }
                        if (API_POST_MemberTrans.response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            MessageBox.Show("Invalid Request Params! Please contact administrators.", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 2 of 3: Sending Member Transactions Fails...", i, false);
                            IsExportSuccess = false;
                            postMemberTransSuccess = false;
                        }
                        if (API_POST_MemberTrans.response.StatusCode == HttpStatusCode.PreconditionFailed)
                        {
                            MessageBox.Show("Invalid System Configuration! Please contact administrators.", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 2 of 3: Sending Member Transactions Fails...", i, false);
                            IsExportSuccess = false;
                            postMemberTransSuccess = false;
                        }
                        if (API_POST_MemberTrans.response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            MessageBox.Show("System Error! Please contact administrators.", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ShowStatus("Step 2 of 3: Sending Member Transactions Fails...", i, false);
                            IsExportSuccess = false;
                            postMemberTransSuccess = false;
                        }
                    }

                    else
                    {
                        ShowStatus("Step 2 of 3: Sending Member Transactions Fails...", i, false);
                        IsExportSuccess = false;
                        postMemberTransSuccess = false;
                    }
                }
                else
                {
                    if (API_POST_MemberTrans.postMemberTransResponseMessage != null && API_POST_MemberTrans.postMemberTransResponseMessage.Contains("No Data to Export"))
                    {
                        ShowStatus("Step 2 of 3: No Transactions to Export..Skip Sending", i, true);
                    }
                    else
                    {
                        ShowStatus("Step 2 of 3: Transactions Exported Successfully..", i, true);
                    }
                }
            }
            else
            {
                ShowStatus("Step 2 of 3: Transactions Already Exported...Skip Sendig...!", i, true);
            }

        }

        public void UpdateTransactionsStatus()
        {
            List<Transaction> tList = entity.Transactions.Where(x => x.IsDeleted == false && x.IsLoyaltyExported == false && x.Id == TransId).ToList();
            foreach (Transaction tran in tList)
            {
                tran.IsLoyaltyExported = true;
                //entity.Entry(tran).State = EntityState.Modified;
            }
            entity.SaveChanges();
        }

        public void UpdateMemberDisplayName(string _TransId, string _MemberDisplayName)
        {
            List<Transaction> tList = entity.Transactions.Where(x =>x.Id == TransId).ToList();
            foreach (Transaction tran in tList)
            {
                tran.PitiMemberName  = _MemberDisplayName;
                //entity.Entry(tran).State = EntityState.Modified;
            }
            entity.SaveChanges();
        }

        public void UpdateLogStatus(long LogID)
        {
            POSEntities entity = new POSEntities();

            APP_Data.ImportExportMemberTransLog exportLog = entity.ImportExportMemberTransLogs.Where(x => x.Id == LogID).FirstOrDefault();
            exportLog.LastProcessingDateTime = DateTime.Now;
            exportLog.Status = IsExportSuccess ? "Success" : "Fail";

            entity.Entry(exportLog).State = EntityState.Modified;

            List<ImportExportMemberTransLogDetail> importMemberTransLogDetail = entity.ImportExportMemberTransLogDetails.Where(x => x.ProcessingBatchID == LogID && x.DetailStatus != "Success").ToList();
            foreach (ImportExportMemberTransLogDetail log in importMemberTransLogDetail)
            {
                switch (log.ProcessName)
                {
                    case "POST_MemberTrans":
                        log.DetailStatus = postMemberTransSuccess ? "Success" : "Fail";
                        log.ResponseMessageFromPiti = string.Join(";", API_POST_MemberTrans.postMemberTransResponseMessage.ToArray());
                        log.PostJson = API_POST_MemberTrans.postMemberTransJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST_MemberTrans.ResponseMemberTransJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST_MemberTrans.ResponseMemberTransJson;
                        break;
                }
            }
            entity.SaveChanges();
        }

        public void ShowStatus(string message, int value, bool success)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (success)
                {
                    lblExportProgress.Text = message;
                    lblExportProgress.Refresh();
                    ExportProgressBar.Value = value;
                    ExportProgressBar.Step = 1;
                    ExportProgressBar.PerformStep();
                }
                else
                {
                    lblExportProgress.Text = message;
                    lblExportProgress.Refresh();
                    int PBM_SETSTATE = 1040; // Code to set the state of progressbar
                    int InProgress = 1; //(Green)
                    int Error = 2; // (Red)
                    int Paused = 3; // (Yellow)
                    SendMessage(ExportProgressBar.Handle, PBM_SETSTATE, Error, InProgress);
                }

            });

        }
    }
}
