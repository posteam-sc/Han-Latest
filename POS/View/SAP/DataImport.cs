using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Transactions;
using System.Windows.Forms;

namespace POS
{
    public partial class DataImport : Form
    {     
      
        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        #region variables
        POSEntities entity = new POSEntities();
        public static bool IsImportSuccess { get; set; } = true;
        // public bool IsImportByProduct { get; set; }
        public bool IsAutoImport { get; set; }


        public List<string> ProductCodes { get; set; } = new List<string>();
        DateTime todayDate = DateTime.Today;
        Sales saleForm;
        public bool IsAllDataImport { get; set; }

        #endregion

        #region Events

        public DataImport(Sales sForm)
        {
            InitializeComponent();
            saleForm = sForm;
        }

        private void DataImport_Load(object sender, EventArgs e)
        {          
           
            this.Activated += AfterLoading;           
        }
        #endregion
        #region Methods

        private void AfterLoading(object sender, EventArgs e) 
        {
            this.Activated -= AfterLoading;
            ImportDataFromSAP();

        }
        public void ImportDataFromSAP()
        {
            lblImportDate.Text = todayDate.ToString("dd/MM/yyyy");
            lblImportProgress.Text = "Preparing Data Import From SAP, Please Wait...";

            bool IsImportAlreadySuccess = false;
            List<APP_Data.ImportExportLog> InLogList = entity.ImportExportLogs.Where(x => EntityFunctions.TruncateTime(x.ProcessingDateTime) == todayDate && x.Type == "Import").ToList();
            long ImportBatchID = 0;           
            if (InLogList.Count > 0)
            {
                int ImportSuccessCount = InLogList.Where(x => x.Status == "Success").Count();
                if (ImportSuccessCount > 0)
                {
                    IsImportAlreadySuccess = true;
                }

                APP_Data.ImportExportLog InLog = InLogList.OrderBy(x => x.ProcessingDateTime).LastOrDefault();
                switch (InLog.Status)
                {
                    case "Pending":
                        ImportBatchID = InLog.Id; 
                        break;
                    case "Success":
                        ImportBatchID = ImportExportLog.CreateNewImportBatch();
                        if (ImportBatchID == 0)
                        {
                            lblImportProgress.Text = "New Import Log Fails..Data Import Cannot be Started...Please Try Again...!";
                            return;
                        }                        
                        break;
                    case "Fail":
                        ImportBatchID = InLog.Id;
                        break;

                }

            }
            else
            {
                ImportBatchID = ImportExportLog.CreateNewImportBatch();
                IsImportAlreadySuccess = false;
                if (ImportBatchID == 0)
                {
                    lblImportProgress.Text = "New Import Log Fails..Data Import Cannot be Started..Please Try Again...!";
                    return;
                }
              
            }
            bool IsConnected = Utility.CheckInternetAndServerConnection();
            if (!IsConnected)
            {
                if (!IsImportAlreadySuccess)
                {
                    IsImportSuccess = false;
                }
                else
                {
                    IsImportSuccess = true;
                }
                this.Dispose();
                return;
            }

            ExecuteAPIs();
            UpdateLogStatus(ImportBatchID);
           

            if (IsImportSuccess)
            {                
                if (!IsAutoImport) // to show messagebox if not AutoImport
                {
                    MessageBox.Show("Data Import Completed Successfully", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.Dispose();
            }
            else
            {
                if (!IsAutoImport)
                {
                    MessageBox.Show("Data Import Failed!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Auto Import Failed! Please try manually", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            if (IsImportAlreadySuccess)
            {
                IsImportSuccess = true;
            }

            if (saleForm != null)
            {
                if (IsImportSuccess)
                {
                    SalesFormButtonsControl();
                }

            }
        }

        public void SalesFormButtonsControl()
        {
            Button btnPaymentAdd = (Button)saleForm.Controls["btnPaymentAdd"];
            Button btnPaid = (Button)saleForm.Controls["btnPaid"];
            Button btnFOC = (Button)saleForm.Controls["btnFOC"];
            Button btnSave = (Button)saleForm.Controls["btnSave"];
            Button btnLoadDraft = (Button)saleForm.Controls["btnLoadDraft"];
            btnPaymentAdd.Enabled = true;
            btnPaid.Enabled = true;
            btnFOC.Enabled = true;
            btnSave.Enabled = true;
            btnLoadDraft.Enabled = true;
        }

        private void UpdateLogStatus(long batchID)
        {
            APP_Data.ImportExportLog importLog = entity.ImportExportLogs.Where(x => x.Id == batchID).FirstOrDefault();
            importLog.Status = IsImportSuccess ? "Success" : "Fail";
            importLog.LastProcessingDateTime = DateTime.Now;
            entity.Entry(importLog).State = EntityState.Modified;

            List<ImportExportLogDetail> importLogDetail = entity.ImportExportLogDetails.Where(x => x.ProcessingBatchID == batchID).ToList();
            foreach (ImportExportLogDetail log in importLogDetail)
            {
                log.DetailStatus = IsImportSuccess ? "Success" : "Fail";
                switch (log.ProcessName)
                {
                    case "GET_UOM":
                        log.ResponseMessageFromSAP = API_GET.UoMResponseMessage;
                        log.ResponseJson = API_GET.UomResponseJson;
                        break;
                    case "GET_ItemMaster":
                        log.ResponseMessageFromSAP = API_GET.Item_MasterResponseMessage;
                        log.PostJson = API_GET.Item_MasterRequestMessage;
                        log.ResponseJson = API_GET.Item_MasterResponseJson;
                        break;
                    case "GET_StockInByBatch":
                        log.ResponseMessageFromSAP = API_GET.StockByBatchResponseMessage;
                        log.PostJson = API_GET.StockInByBatchRequestMessage;
                        log.ResponseJson = API_GET.StockInResponseJson;
                        break;
                }

            }

            entity.SaveChanges();
        }

        private void ExecuteAPIs()
        {
            int i = 1;
            IsImportSuccess = true;
            int no0fProgressSteps = 17;
            ImportProgressBar.Maximum = no0fProgressSteps;          

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = new TimeSpan(1, 00, 0);
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, options))
            {
                ShowStatus("Step 1 of 15: Getting Access Token...", ++i, true);
                API_Token.Get_AccessToken();

                if (string.IsNullOrEmpty(API_Token.AccessToken) || string.IsNullOrWhiteSpace(API_Token.AccessToken))
                {
                    ShowStatus(" Step 1 of 15: Getting Token Fails!..", i, false);
                    IsImportSuccess = false;
                    API_GET.UoMResponseMessage = API_GET.Item_MasterResponseMessage = API_GET.StockByBatchResponseMessage = "No Access Token";
                    return; // exit import and update log table status to 'fail'
                }

                ShowStatus("Step 1 of 15: Token Received Successfully!", i, true);
                ShowStatus("Step 2 of 15: Getting New UoMs....", ++i, true);
                API_GET.GET_UOMData();
                if (API_GET.Uom == null)
                {
                    ShowStatus("Step 2 of 15: Getting New UoMs Fails....", i, false);
                    if (API_GET.response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    IsImportSuccess = false;
                    return;
                }
                ShowStatus("Step 2 of 15: Received UoMs successfully....", i, true);


                ShowStatus("Step 3 of 15: Updating Existing UoMs....", ++i, true);

                API_GET.UpdateExistingUoms(API_GET.Uom);
                if (API_GET.UpdateUomSuccess == false)
                {
                    ShowStatus("Step 3 of 15: Updading Existing UoMs Fails....", i, false);
                    transaction.Dispose();
                    IsImportSuccess = false;
                    return;
                }
                ShowStatus("Step 3 of 15: Updating Existing Uoms Succefully FINISHED....", i, true);

                ShowStatus("Step 4 of 15: Checking/Saving New UoMs....", ++i, true);

                API_GET.CheckNewUoM(API_GET.Uom);
                if (API_GET.NewUomSaveSuccess == false)
                {
                    ShowStatus("Step 4 of 15: Checking/Saving New UoMs Fails....", i, false);
                    transaction.Dispose();
                    IsImportSuccess = false;
                    return;
                }
                ShowStatus("Step 4 of 15: New UoMs Succefully Saved....", i, true);

                ShowStatus("Step 5 of 15: Getting Master Items....", ++i, true);
                API_GET.GET_MasterItem(IsAllDataImport, ProductCodes);
                if (API_GET.Master_Item == null)
                {
                    ShowStatus("Step 5 of 15: Getting Master Items Fails....", i, false);
                    if (API_GET.response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    transaction.Dispose();
                    IsImportSuccess = false;
                    return;
                }

                ShowStatus("Step 5 of 15: Received Master Items Successfully!", i, true);
                ShowStatus("Step 6 of 15: Retriving Imported Data!...", ++i, true);
                List<API_GET.StockInData> Indata = API_GET.Master_Item.ToList();
                if (Indata.Count == 0)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        ShowStatus("No Item Master Data, Skip to Step 14", ++i, true);
                    }
                }
                else
                {
                    //Filter Products without Barcodes
                    Indata = Indata.Where(x => !string.IsNullOrEmpty(x.BarCode)).ToList();

                    ShowStatus("Step 6 of 15: Updating Existing Brands....", i, true);
                    API_GET.UpdateExistingBrand(Indata);
                    if (API_GET.UpdateBrandSuccess == false)
                    {
                        ShowStatus("Step 6 of 15: Updating Existing Brands Fails....", i, false);
                        transaction.Dispose();
                        IsImportSuccess = false;
                        return;
                    }
                    ShowStatus("Step 6 of 15: Updating Existing Brands Successfully FINISHED....", i, true);

                    ShowStatus("Step 7 of 15: Checking/Saving New Brands....", ++i, true);
                    API_GET.CheckNewBrand(Indata);
                    if (API_GET.NewBrandSaveSuccess == false)
                    {
                        ShowStatus("Step 7 of 15: Checking/Saving New Brands Fails....", i, false);
                        transaction.Dispose();
                        IsImportSuccess = false;
                        return;
                    }
                    ShowStatus("Step 7 of 15: Checking/Saving New Brands Successfully FINISHED....", i, true);

                    ShowStatus("Step 8 of 15: Updating Existing Categories....", ++i, true);
                    API_GET.UpdateExistingCategory(Indata);
                    if (API_GET.UpdateCategorySuccess == false)
                    {
                        ShowStatus("Step 8 of 15: Updating Existing Categories Fails....", i, false);
                        transaction.Dispose();
                        IsImportSuccess = false;
                        return;
                    }
                    ShowStatus("Step 8 of 15: Updating Existing Categories Successfully FINISHED....", i, true);
                    ShowStatus("Step 9 of 15: Checking/Saving New Categories....", ++i, true);
                    API_GET.CheckNewCategory(Indata);
                    if (API_GET.NewCategorySaveSuccess == false)
                    {
                        ShowStatus("Step 9 of 15: Checking/Saving New Categories Fails....", i, false);
                        transaction.Dispose();
                        IsImportSuccess = false;
                        return;
                    }
                    ShowStatus("Step 9 of 15: Checking/Saving New Categories Successfully FINISHED...", i, true);

                    ShowStatus("Step 10 of 15: Updating Existing SubCategories....", ++i, true);
                    API_GET.UpdateExistingSubCategory(Indata);
                    if (API_GET.UpdateSubCategorySuccess == false)
                    {
                        ShowStatus("Step 10 of 15: Updating Existing SubCategories Fails....", i, false);
                        transaction.Dispose();
                        IsImportSuccess = false;
                        return;
                    }
                    ShowStatus("Step 10 of 15: Updating Existing SubCategories Successfully FINISHED....", i, true);
                    ShowStatus("Step 11 of 15: Checking/Saving New SubCategories....", ++i, true);
                    API_GET.CheckNewSubCategory(Indata);
                    if (API_GET.NewSubCategorySaveSuccess == false)
                    {
                        ShowStatus("Step 11 of 15: Checking/Saving New SubCategories Fails....", i, false);
                        transaction.Dispose();
                        IsImportSuccess = false;
                        return;
                    }
                    ShowStatus("Step 11 of 15: Checking/Saving New SubCategories Successfully FINISHED....", i, true);
                    ShowStatus("Step 12 of 15: Updating Existing Products, Please Wait....", ++i, true);
                    API_GET.UpdateExistingProducts(Indata);
                    if (API_GET.UpdateProductSuccess == false)
                    {
                        ShowStatus("Step 12 of 15: Updating Existing Products Fails....", i, false);
                        transaction.Dispose();
                        IsImportSuccess = false;
                        return;
                    }
                    ShowStatus("Step 12 of 15: Updating Existing Products Successfully FINISHED....", i, true);
                    ShowStatus("Step 13 of 15: Checking New Products...", ++i, true);
                    API_GET.CheckNewProduct(Indata);
                    if (API_GET.NewProductSaveSuccess == false)
                    {
                        ShowStatus("Step 13 of 15: Checking/Saving New Products Fails....", i, false);
                        transaction.Dispose();
                        IsImportSuccess = false;
                        return;
                    }
                    ShowStatus("Step 13 of 15: Checking/Saving New Products Successfully FINISHED....", i, true);
                }

                ShowStatus("Step 14 of 15: Getting Product Batch and Quantities, Please Wait....", ++i, true);

                API_GET.GET_InStockByBatch(SettingController.CounterCode, ProductCodes);
                if (API_GET.StockInByBatch == null)
                {
                    ShowStatus("Step 14 of 15: Getting Product Batch and Quantities Fails....", i, false);
                    if (API_GET.response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Unauthorized Access! Please Refresh Access Token", "Unauthorized Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    transaction.Dispose();
                    IsImportSuccess = false;
                    return;
                }
                if (API_GET.StockInByBatch.Count == 0)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        ShowStatus("No Stock Data: Import Data Successfully FINISHED!!!", ++i, true);
                    }
                    transaction.Complete();
                    Sales.getCommonProduct();
                }
                else
                {
                    ShowStatus("Step 14 of 15: Received Product Batch and Quantities Successfully....", i, true);
                    ShowStatus("Step 15 of 15: Saving Product Batch and Quantities, Please Wait....", ++i, true);
                    API_GET.SaveStockInDataByBatch(API_GET.StockInByBatch);
                    if (API_GET.StockFillSuccess == false)
                    {
                        ShowStatus("Step 15 of 15: Saving Product Batch and Quantities Fails....", i, false);
                        transaction.Dispose();
                        IsImportSuccess = false;
                        return;
                    }
                    ShowStatus("Step 15 of 15: Import Data From SAP Successfully FINISHED!!!", ++i, true);
                    transaction.Complete();
                    Sales.getCommonProduct();
                }

            }
            Application.DoEvents();
        }

        public void ShowStatus(string message, int value, bool success)
        {

            this.Invoke((MethodInvoker)delegate
            {

                if (success)
                {
                    lblImportProgress.Text = message;
                    lblImportProgress.Refresh();
                    ImportProgressBar.Value = value;
                    ImportProgressBar.Step = 1;
                    ImportProgressBar.PerformStep();

                }
                else
                {

                    lblImportProgress.Text = message;
                    lblImportProgress.Refresh();
                    int PBM_SETSTATE = 1040; // Code to set the state of progressbar
                    int InProgress = 1; //(Green)
                    int Error = 2; // (Red)
                    int Paused = 3; // (Yellow)
                    SendMessage(ImportProgressBar.Handle, PBM_SETSTATE, Error, InProgress);
                }




            });

        }
        #endregion
    }
}
