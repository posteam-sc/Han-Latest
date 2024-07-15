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
    public partial class DataExport : Form
    {

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);
        #region Variables
        POSEntities entity = new POSEntities();
        DateTime todayDate = DateTime.Today;
        bool IsExportSuccess;
        public static bool IsAllExportSuccess { get; set; } = true;
        bool postArNoExport;
        //bool postJENoExport;
        bool postArCredNoExport;

        bool postArSuccess;
       // bool postJESuccess;
        bool postArCredSuccess;
        public static int exportStatus { get; set; } = -1;
        public bool IsBackDateExport;
        public bool IsAutoExport;
        public bool IsNoDataToExport = false;

        Sales saleForm;

        #endregion
        public DataExport(Sales sForm)
        {
            InitializeComponent();
            saleForm = sForm;
        }

        private void AfterLoading(object sender, EventArgs e)
        {
            this.Activated -= AfterLoading;
            ExportDataToSAP();

        }
        private void SAPMenuControlForBackDateExport(bool BackDateExportSuccess)
        {
            ((MDIParent)saleForm.ParentForm).importSAPToolStripMenuItem.Enabled =
            ((MDIParent)saleForm.ParentForm).importByProductToolStripMenuItem.Enabled =
            ((MDIParent)saleForm.ParentForm).importAllDataToolStripMenuItem.Enabled =
            ((MDIParent)saleForm.ParentForm).exportSAPToolStripMenuItem.Enabled = BackDateExportSuccess;
            ((MDIParent)saleForm.ParentForm).backDateExportToolStripMenuItem.Visible = !BackDateExportSuccess;

        }

        private void SAPMenuControlForTodayExport(bool ExportSuccess)
        {
            ((MDIParent)saleForm.ParentForm).importSAPToolStripMenuItem.Enabled =
            ((MDIParent)saleForm.ParentForm).importByProductToolStripMenuItem.Enabled =
            ((MDIParent)saleForm.ParentForm).importAllDataToolStripMenuItem.Enabled = false;
            ((MDIParent)saleForm.ParentForm).exportSAPToolStripMenuItem.Enabled = !ExportSuccess;
            // ((MDIParent)saleForm.ParentForm).backDateExportToolStripMenuItem.Visible = false;

        }
        public void SalesFormButtonsControl()
        {

            Button btnPaymentAdd = (Button)saleForm.Controls["btnPaymentAdd"];
            Button btnPaid = (Button)saleForm.Controls["btnPaid"];
            Button btnSave = (Button)saleForm.Controls["btnSave"];
            Button btnLoadDraft = (Button)saleForm.Controls["btnLoadDraft"];
            Button btnFOC = (Button)saleForm.Controls["btnFOC"];
            btnPaymentAdd.Enabled = false;
            btnPaid.Enabled = false;
            btnSave.Enabled = false;
            btnLoadDraft.Enabled = false;
            btnFOC.Enabled = false;
        }

        private void DataExport_Load(object sender, EventArgs e)
        {            

            if (!IsBackDateExport)
            {
                this.Text = "Data Export";
                if (exportStatus != 1)
                {                 
                    DialogResult result = MessageBox.Show("After Exported, you are no longer allowed to make invoices for today.", "Are you sure to export?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result.Equals(DialogResult.OK))
                    {
                        this.Activated += AfterLoading;
                    }
                    else
                    {
                        this.Close();
                    }
                }
                else
                {
                    this.Activated += AfterLoading;
                }

            }
            else
            {
                this.Text = "Back Date Export";
                this.Activated += AfterLoading;

            }

        }

        public void ExportDataToSAP()
        {

            int backDateDays = SettingController.BackDate_NoOfDays;
            DateTime backDate = todayDate.AddDays(backDateDays).Date;
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime startDate;
            DateTime endDate;

            DateTime executeDate = todayDate; // just initialization
            long executeLogID = 0;
            IsAllExportSuccess = true;

            #region BackDate
            if (IsBackDateExport)
            {
                List<DateTime?> backDateList = new List<DateTime?>();             
                startDate = backDate;
                endDate = todayDate.AddDays(-1).Date;

                backDateList = (from t in entity.Transactions
                                where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= startDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= endDate
                                 &&  t.IsDeleted == false && t.IsComplete == true && (t.Type == "Sale" || t.Type == "Refund") && t.Id.Contains(shortCode)
                                select EntityFunctions.TruncateTime((DateTime)t.DateTime)).Distinct().ToList();
                if (backDateList.Count > 0)
                {

                    foreach (DateTime dt in backDateList)
                    {
                        lblExportDate.Text = "";
                        lblExportDate.Text = dt.Date.ToString("dd/MM/yyyy");
                        // UpdateTransactionsStatus(dt.Date);
                        APP_Data.ImportExportLog exlog = new APP_Data.ImportExportLog();
                        exlog = entity.ImportExportLogs.Where(x => EntityFunctions.TruncateTime(x.ProcessingDateTime) == dt.Date && x.Type == "Export").FirstOrDefault();
                        if (exlog != null)
                        {
                            switch (exlog.Status)
                            {
                                case "Success":
                                    IsExportSuccess = true;
                                    break;
                                case "Pending":
                                    //postArNoExport = postJENoExport = postArCredNoExport = true;
                                    postArNoExport  = postArCredNoExport = true;
                                    IsExportSuccess = false;
                                    executeDate = (DateTime)exlog.ProcessingDateTime;
                                    executeLogID = exlog.Id;
                                    //ExecuteAPIs((DateTime)exlog.ProcessingDateTime);
                                    //UpdateLogStatus(exlog.Id);
                                    break;
                                case "Fail":
                                    List<ImportExportLogDetail> ExlogDetails = new List<ImportExportLogDetail>();
                                    ExlogDetails = entity.ImportExportLogDetails.Where(x => x.ProcessingBatchID == exlog.Id && x.DetailStatus == "Fail").ToList();
                                    foreach (ImportExportLogDetail exDetails in ExlogDetails)
                                    {

                                        switch (exDetails.ProcessName)
                                        {
                                            case "POST_AR":
                                                postArNoExport = true;
                                                break;
                                            //case "POST_JE":
                                            //    postJENoExport = true;
                                            //    break;
                                            case "POST_ARCred":
                                                postArCredNoExport = true;
                                                break;
                                        }
                                    }
                                    IsExportSuccess = false;
                                    executeDate = (DateTime)exlog.ProcessingDateTime;
                                    executeLogID = exlog.Id;
                                    //ExecuteAPIs((DateTime)(exlog.ProcessingDateTime));
                                    //UpdateLogStatus(exlog.Id);
                                    break;
                            }

                        }
                        else
                        {
                            int resultID = 0;
                            resultID = ImportExportLog.CreateNewExportBatch(dt);
                            if (resultID == 0)
                            {
                                lblExportProgress.Text = "New Export Log Fails..Data Import Cannot be Started...Please Try Again...!";
                                return;
                            }
                           // postArNoExport = postJENoExport = postArCredNoExport = true;
                            postArNoExport = postArCredNoExport = true;
                            IsExportSuccess = false;
                            executeDate = dt;
                            executeLogID = resultID;                           
                            //ExecuteAPIs(dt);
                            //UpdateLogStatus(resultID);
                        }
                        if (!IsExportSuccess)
                        {
                            bool IsConnected = Utility.CheckInternetAndServerConnection();
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
                            UpdateTransactionsStatus(dt.Date);
                            ExecuteAPIs(executeDate);
                            UpdateLogStatus(executeLogID);
                        }
                        if (IsExportSuccess)
                        {
                            // IsAllExportSuccess = true;
                            lblExportProgress.Text = "Data Export For " + dt.Date.ToString("dd/MM/yyyy") + " Completed Successfullly..";
                        }
                        else
                        {
                            IsAllExportSuccess = false;
                            lblExportProgress.Text = "Data Export For " + dt.Date.ToString("dd/MM/yyyy") + " Finished with Error..!";
                        }
                    }

                    if (IsAllExportSuccess)
                    {
                        // MessageBox.Show("Back Date Data Export Completed Successfully", "Data Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        lblExportProgress.Text = "Back Date Export Completed Successfully";
                        if (IsAutoExport)
                        {
                            Login.IsBackDateExportSuccess = true;
                        }
                        //else
                        //{
                        //    Sales.IsBackDateExportSuccess = true;
                        //}

                        this.Dispose();

                    }
                    else
                    {

                        lblExportProgress.Text = "Back Date Export Completed with Failures..!";
                        if (IsAutoExport)
                        {
                            Login.IsBackDateExportSuccess = false;
                        }
                        //else
                        //{
                        //    Sales.IsBackDateExportSuccess = false;
                        //}
                        MessageBox.Show("Back-date Export fails..! Data Import cannot be started", "Data Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Dispose();
                        // Try Export Again
                    }
                }
                else
                {
                    lblExportProgress.Text = "No Back Date Export";
                    IsAllExportSuccess = true;
                    if (IsAutoExport)
                    {
                        Login.IsBackDateExportSuccess = true;
                    }
                    //else
                    //{
                    //    Sales.IsBackDateExportSuccess = true;
                    //}
                    this.Dispose();

                }
                //  IsBackDateExport = false;
            }
            #endregion
            #region todayExport
            else
            {
                this.Text = "Data Export";
                List<DateTime?> todayExportList = new List<DateTime?>();
                todayExportList = (from t in entity.Transactions
                                   where EntityFunctions.TruncateTime((DateTime)t.DateTime) == todayDate
                                    && t.IsDeleted == false && t.IsComplete == true && (t.Type == "Sale" || t.Type == "Refund") && t.Id.Contains(shortCode)
                                   select EntityFunctions.TruncateTime((DateTime)t.DateTime)).Distinct().ToList();

                if (todayExportList.Count > 0)
                {


                    //foreach (DateTime dt in todayExportList)
                    //{
                    DateTime dt = DateTime.Now;
                    lblExportDate.Text = dt.ToString("dd/MM/yyyy");
                    // UpdateTransactionsStatus(dt.Date);
                    APP_Data.ImportExportLog exlog = new APP_Data.ImportExportLog();
                    exlog = entity.ImportExportLogs.Where(x => EntityFunctions.TruncateTime(x.ProcessingDateTime) == dt.Date && x.Type == "Export").FirstOrDefault();
                    if (exlog != null)
                    {
                        switch (exlog.Status)
                        {
                            case "Success":
                                IsExportSuccess = true;                                
                                break;
                            case "Pending":
                                //postArNoExport = postJENoExport = postArCredNoExport = true;
                                postArNoExport = postArCredNoExport = true;
                                IsExportSuccess = false;
                                executeDate = (DateTime)exlog.ProcessingDateTime;
                                executeLogID = exlog.Id;
                               // ExecuteAPIs((DateTime)exlog.ProcessingDateTime);
                                //UpdateLogStatus(exlog.Id);
                                break;
                            case "Fail":
                                List<ImportExportLogDetail> ExlogDetails = new List<ImportExportLogDetail>();
                                ExlogDetails = entity.ImportExportLogDetails.Where(x => x.ProcessingBatchID == exlog.Id && x.DetailStatus == "Fail").ToList();
                                foreach (ImportExportLogDetail exDetails in ExlogDetails)
                                {

                                    switch (exDetails.ProcessName)
                                    {
                                        case "POST_AR":
                                            postArNoExport = true;
                                            break;
                                        //case "POST_JE":
                                        //    postJENoExport = true;
                                        //    break;
                                        case "POST_ARCred":
                                            postArCredNoExport = true;
                                            break;
                                    }
                                }
                                IsExportSuccess = false;
                                executeDate = (DateTime)(exlog.ProcessingDateTime);
                                executeLogID = exlog.Id;
                                //ExecuteAPIs((DateTime)(exlog.ProcessingDateTime));
                                //UpdateLogStatus(exlog.Id);
                                break;
                        }

                    }
                    else
                    {
                        int resultID = 0;
                        resultID = ImportExportLog.CreateNewExportBatch(dt);
                        if (resultID == 0)
                        {
                            lblExportProgress.Text = "New Export Log Fails..Data Export Cannot be Started...Please Try Again...!";
                            return;
                        }
                       // postArNoExport = postJENoExport = postArCredNoExport = true;
                        postArNoExport = postArCredNoExport = true;
                        IsExportSuccess =false;
                        executeDate = dt;
                        executeLogID = resultID;
                        //ExecuteAPIs(dt);
                        //UpdateLogStatus(resultID);
                    }
                    if (!IsExportSuccess)
                    {
                        bool IsConnected = Utility.CheckInternetAndServerConnection();
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
                        UpdateTransactionsStatus(dt.Date);
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
                    // }

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
            }

            #endregion
            if (saleForm != null)
            {
                if (IsBackDateExport)
                {
                    SAPMenuControlForBackDateExport(IsAllExportSuccess);
                }
                else
                {
                  
                    if (!IsNoDataToExport)
                    {
                        SalesFormButtonsControl();
                        SAPMenuControlForTodayExport(IsAllExportSuccess);

                    }                 


                }

            }
        }
        public void UpdateTransactionsStatus(DateTime dt)
        {
            if (dt.Date.Equals(todayDate))
            {
                exportStatus = 1;

            }

            List<Transaction> tList = entity.Transactions.Where(x => EntityFunctions.TruncateTime((DateTime)x.DateTime) == dt && x.IsDeleted == false && x.IsExported == false).ToList();
            foreach (Transaction tran in tList)
            {
                tran.IsExported = true;
                entity.Entry(tran).State = EntityState.Modified;
            }
            entity.SaveChanges();


        }

        public void UpdateLogStatus(long LogID)
        {
            POSEntities entity = new POSEntities();

            APP_Data.ImportExportLog exportLog = entity.ImportExportLogs.Where(x => x.Id == LogID).FirstOrDefault();
            exportLog.LastProcessingDateTime = DateTime.Now;
            exportLog.Status = IsExportSuccess ? "Success" : "Fail";

            entity.Entry(exportLog).State = EntityState.Modified;

            List<ImportExportLogDetail> importLogDetail = entity.ImportExportLogDetails.Where(x => x.ProcessingBatchID == LogID && x.DetailStatus != "Success").ToList();
            foreach (ImportExportLogDetail log in importLogDetail)
            {
                switch (log.ProcessName)
                {
                    case "POST_AR":
                        log.DetailStatus = postArSuccess ? "Success" : "Fail";
                        log.ResponseMessageFromSAP = string.Join(";", API_POST.postArResponseMessage.ToArray());
                        log.PostJson = API_POST.postArJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.ResponseArJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.ResponseArJson;

                        break;
                    //case "POST_JE":
                    //    log.DetailStatus = postJESuccess ? "Success" : "Fail";
                    //    log.ResponseMessageFromSAP = string.Join(";", API_POST.postJEResponseMessage.ToArray());
                    //    log.PostJson = API_POST.postJEJson;
                    //    log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.ResponseJEJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.ResponseJEJson;

                    //    break;
                    case "POST_ARCred":
                        log.DetailStatus = postArCredSuccess ? "Success" : "Fail";
                        log.ResponseMessageFromSAP = string.Join(";", API_POST.postArCredResponseMessage.ToArray());
                        log.PostJson = API_POST.postArCredJson;
                        log.ResponseJson = log.ResponseJson == null ? DateTime.Now.ToString() + ";" + API_POST.ResponseArCredJson : log.ResponseJson + ";[" + DateTime.Now.ToString() + "];" + API_POST.ResponseArCredJson;

                        break;
                }

            }
            entity.SaveChanges();

        }

        public void ExecuteAPIs(DateTime postDate)
        {       

          //  int no0fProgressSteps = 5;
            int no0fProgressSteps = 4;
            int i = 1;
            ExportProgressBar.Value = 0; // reset progressbar
            ExportProgressBar.Maximum = no0fProgressSteps;
            postDate = postDate.Date;
            IsExportSuccess = true;
          //  postArSuccess = postJESuccess = postArCredSuccess = true;
            postArSuccess = postArCredSuccess = true;

            ShowStatus("Step 1 of 4: Getting Access Token...", ++i, true);
            API_Token.Get_AccessToken();
            if (string.IsNullOrEmpty(API_Token.AccessToken) || string.IsNullOrWhiteSpace(API_Token.AccessToken))
            {
                ShowStatus(" Step 1 of 4: Getting Access Token Fails!..Cannot Export Data...", i, false);
                IsExportSuccess = false;
                postArSuccess = false;
              //  postJESuccess = false;
                postArCredSuccess = false;
                API_POST.postArResponseMessage.Add("No Access Token");
              //  API_POST.postJEResponseMessage.Add("No Access Token");
                API_POST.postArCredResponseMessage.Add("No Access Token");
                return; // exit import and update log table status to 'fail'
            }

            ShowStatus("Step 1 of 3: Token Received Successfully!", i, true);
            if (postArNoExport)
            {
                ShowStatus("Step 2 of 3: Sending AR Invoices..Please Wait...", ++i, true);
                API_POST.POST_AR(postDate);
                if (!API_POST.postArSuccess)
                {
                    if (API_POST.response != null && API_POST.response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ShowStatus("Step 2 of 3: Sending AR Invoices Fails...", i, false);
                        IsExportSuccess = false;
                        postArSuccess = false;
                    }

                    else
                    {
                        ShowStatus("Step 2 of 3: Sending AR Invoices Fails...", i, false);
                        IsExportSuccess = false;
                        postArSuccess = false;
                    }

                }
                else
                {
                    if (API_POST.postArResponseMessage != null && API_POST.postArResponseMessage.Contains("No Data to Export"))
                    {
                        ShowStatus("Step 2 of 3: No AR Invoices to Export..Skip Sending", i, true);
                    }
                    else
                    {
                        ShowStatus("Step 2 of 3: AR Invoices Exported Successfully..", i, true);
                    }

                }

            }
            else
            {
                ShowStatus("Step 2 of 3: AR Invoices Already Exported...Skip Sendig...!", i, true);

            }

            //if (postJENoExport)
            //{
            //    ShowStatus("Step 3 of 4: Sending Journal Entry....Please Wait...", ++i, true);
            //    API_POST.POST_JE(postDate);
            //    if (!API_POST.postJESuccess)
            //    {
            //        if (API_POST.response != null && API_POST.response.StatusCode == HttpStatusCode.Unauthorized)
            //        {
            //            MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            ShowStatus("Step 3 of 4: Sending Journal Entry Fails...", i, false);
            //            IsExportSuccess = false;
            //            postJESuccess = false;
            //        }

            //        else
            //        {
            //            ShowStatus("Step 3 of 4: Sending Journal Entry Fails...", i, false);
            //            IsExportSuccess = false;
            //            postJESuccess = false;
            //        }


            //    }
            //    else
            //    {
            //        if (API_POST.postJEResponseMessage != null && API_POST.postJEResponseMessage.Contains("No Data to Export"))
            //        {
            //            ShowStatus("Step 3 of 4: No Journal Entry to Export, Skip Sending...", i, true);
            //        }
            //        else
            //        {
            //            ShowStatus("Step 3 of 4: Journal Entry Exported Successfully...", i, true);
            //        }

            //    }

            //}
            //else
            //{
            //    ShowStatus("Step 3 of 4: Journal Entry Already Exported..Skip Sending.", i, true);

            //}
            if (postArCredNoExport)
            {
                //ShowStatus("Step 4 of 4: Sending AR Credit Memo....Please Wait....", ++i, true);
                ShowStatus("Step 3 of 3: Sending AR Credit Memo....Please Wait....", ++i, true);
                API_POST.POST_ARCred(postDate);
                if (!API_POST.postArCredSuccess)
                {
                    if (API_POST.response != null && API_POST.response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       // ShowStatus("Step 4 of 4: Sending AR Credit Memo Fails..", i, false);
                        ShowStatus("Step 3 of 3: Sending AR Credit Memo Fails..", i, false);
                        IsExportSuccess = false;
                        postArCredSuccess = false;
                    }

                    else
                    {
                       // ShowStatus("Step 4 of 4: Sending AR Credit Memo Fails..", i, false);
                        ShowStatus("Step 3 of 3: Sending AR Credit Memo Fails..", i, false);
                        IsExportSuccess = false;
                        postArCredSuccess = false;
                    }

                }
                else
                {
                    if (API_POST.postArCredResponseMessage != null && API_POST.postArCredResponseMessage.Contains("No Data to Export"))
                    {
                       // ShowStatus("Step 4 of 4: No AR Credit Memo to Export.., Skip Sending..!", i, true);
                        ShowStatus("Step 3 of 3: No AR Credit Memo to Export.., Skip Sending..!", i, true);
                    }
                    else
                    {
                       // ShowStatus("Step 4 of 4: AR Credit Memo Exported Successfully...!", i, true);
                        ShowStatus("Step 3 of 3: AR Credit Memo Exported Successfully...!", i, true);
                    }

                }

            }
            else
            {
               // ShowStatus("Step 4 of 4: AR Credit Memo Already Exported...Skip Sending...!", ++i, true);
                ShowStatus("Step 3 of 3: AR Credit Memo Already Exported...Skip Sending...!", ++i, true);
            }

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
