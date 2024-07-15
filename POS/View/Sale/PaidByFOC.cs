using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using POS.APP_Data;

namespace POS
{
    public partial class PaidByFOC : Form
    {
        #region Variables

        public List<TransactionDetail> DetailList = new List<TransactionDetail>();

        public int Discount { get; set; }

        public int Tax { get; set; }

        public int ExtraDiscount { get; set; }

        public int ExtraTax { get; set; }

        public Boolean isDraft { get; set; }        

        public string DraftId { get; set; }

        public Boolean IsWholeSale { get; set; }

        public int Type { get; set; }
        
        private POSEntities entity = new POSEntities();

        public decimal BDDiscount { get; set; }

        public decimal MCDiscount { get; set; }

        public int CustomerId { get; set; }

        public int? MemberTypeId { get; set; }

        public decimal? MCDiscountPercent { get; set; }

        int Qty = 0;

        List<Stock_Transaction> productList = new List<Stock_Transaction>();
        public Boolean IsPrint = false;

        #endregion
        public PaidByFOC()
        {
            InitializeComponent();
        }

        private void PaidByFOC_Load(object sender, EventArgs e)
        {
            long totalCost = (long)DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (long)BDDiscount - (long)MCDiscount;
            long unitpriceTotalCost = (long)DetailList.Sum(x => x.UnitPrice * x.Qty);//Edit ZMH


            //System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Type, ExtraTax + Tax, ExtraDiscount + Discount, totalCost, 0, null, CustomerId,MCDiscount,BDDiscount,MemberTypeId,MCDiscountPercent,false,"",IsWholeSale);
            //Updated by Lele
            //System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Type, ExtraTax + Tax, 0, totalCost, 0, null, CustomerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,false);
            //System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Type, ExtraTax + Tax, 0, totalCost, 0, null, CustomerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false);
            //Updated by Yimon
            System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Type, ExtraTax + Tax, 0, totalCost, 0, null, CustomerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false,"","");
            entity = new POSEntities();
            string resultId = Id.FirstOrDefault().ToString();
            Transaction insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
            foreach (TransactionDetail detail in DetailList)
            {
                detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                detail.Product.Qty = detail.Product.Qty - detail.Qty;
                detail.IsFOC = true;
                detail.UnitPrice = 0;
                detail.TotalAmount = 0;
                detail.DiscountRate = 0;
                detail.TaxRate = 0;
                //save in stocktransaction
             
                Stock_Transaction st = new Stock_Transaction();
                st.ProductId = detail.Product.Id;
                Qty = Convert.ToInt32(detail.Qty);
                st.Sale = Qty;
                productList.Add(st);
                Qty = 0;


                if (detail.ConsignmentPrice == null)
                {
                    detail.ConsignmentPrice = 0;
                }

                if (detail.Product.IsWrapper == true)
                {
                    List<WrapperItem> wList = detail.Product.WrapperItems.ToList();
                    if (wList.Count > 0)
                    {
                        foreach (WrapperItem w in wList)
                        {
                            Product wpObj = (from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault();
                            wpObj.Qty = wpObj.Qty - detail.Qty;
                        }
                    }
                }
                detail.IsDeleted = false;
                detail.IsConsignmentPaid = Utility.IsConsignmentPaid(detail.Product);
                insertedTransaction.TransactionDetails.Add(detail);
            }

            //save in stock transaction
            Save_SaleQty_ToStockTransaction(productList);
            productList.Clear();

            insertedTransaction.IsDeleted = false;
            //insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
            entity.SaveChanges();
            #region purchase
            // for Purchase Detail and PurchaseDetailInTransacton.

            foreach (TransactionDetail detail in insertedTransaction.TransactionDetails)
            {
                int Qty = Convert.ToInt32(detail.Qty);
                int pId = Convert.ToInt32(detail.ProductId);                
                // Get purchase detail with same product Id and order by purchase date ascending
                List<APP_Data.PurchaseDetail> pulist = (from p in entity.PurchaseDetails
                                                        join m in entity.MainPurchases on p.MainPurchaseId equals m.Id
                                                        where p.ProductId == pId && p.IsDeleted == false && m.IsCompletedInvoice==true && p.CurrentQy > 0 orderby p.Date ascending select p).ToList();

                if (pulist.Count > 0)
                {
                    int TotalQty = Convert.ToInt32(pulist.Sum(x => x.CurrentQy));
                    if (TotalQty >= Qty)
                    {
                        foreach (PurchaseDetail p in pulist)
                        {
                            if (Qty > 0)
                            {
                                if (p.CurrentQy >= Qty)
                                {
                                    PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                                    pdObjInTran.ProductId = pId;
                                    pdObjInTran.TransactionDetailId = detail.Id;
                                    pdObjInTran.PurchaseDetailId = p.Id;
                                    pdObjInTran.Date = p.Date;
                                    pdObjInTran.Qty = Qty;
                                    p.CurrentQy = p.CurrentQy - Qty;
                                    Qty = 0;

                                    entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                                    entity.Entry(p).State = EntityState.Modified;
                                    entity.SaveChanges();
                                    break;
                                }
                                else if (p.CurrentQy <= Qty)
                                {
                                    PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                                    pdObjInTran.ProductId = pId;
                                    pdObjInTran.TransactionDetailId = detail.Id;
                                    pdObjInTran.PurchaseDetailId = p.Id;
                                    pdObjInTran.Date = p.Date;
                                    pdObjInTran.Qty = p.CurrentQy;

                                    Qty = Convert.ToInt32(Qty - p.CurrentQy);
                                    p.CurrentQy = 0;
                                    entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                                    entity.Entry(p).State = EntityState.Modified;
                                    entity.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            #endregion


            //Print Invoice

            if (Type == 4 || Type == 6)
            {
                #region [ Print ]
                if (IsPrint)
                {
                
                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];

                int _tAmt = 0;
                foreach (TransactionDetail transaction in DetailList)
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.Name = transaction.Product.Name;
                    newRow.Qty = transaction.Qty.ToString();
                    newRow.DiscountPercent = transaction.DiscountRate.ToString();
                    //newRow.TotalAmount = (int)transaction.TotalAmount; //Edit By ZMH
                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; //Edit By ZMH
                    switch (Utility.GetDefaultPrinter())
                    {
                        case "A4 Printer":
                            newRow.UnitPrice = transaction.UnitPrice.ToString();
                            break;
                        case "Slip Printer":
                            newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                            break;
                    }
                    _tAmt += newRow.TotalAmount;
                    dtReport.AddItemListRow(newRow);
                }

                string reportPath = "";
                ReportViewer rv = new ReportViewer();
                ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                reportPath = Application.StartupPath + Utility.GetReportPath("FOC");
                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Add(rds);

                Utility.Slip_Log(rv);
                //switch (Utility.GetDefaultPrinter())
                //{

                //    case "Slip Printer":
                //        Utility.Slip_A4_Footer(rv);
                //        break;
                //}
                Utility.Slip_A4_Footer(rv);
                APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();


                if (Type == 4)
                {
                    ReportParameter Title = new ReportParameter("Title", "FOC SALE INVOICE");
                    rv.LocalReport.SetParameters(Title);
                }
                else if (Type == 6)
                {
                    ReportParameter Title = new ReportParameter("Title", "TESTER SALE INVOICE");
                    rv.LocalReport.SetParameters(Title);
                }

                ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
                rv.LocalReport.SetParameters(CustomerName);

                if (Convert.ToInt32(insertedTransaction.MCDiscountAmt) != 0)
                {
                    Int64 _mcDiscountAmt = Convert.ToInt64(insertedTransaction.MCDiscountAmt);
                    ReportParameter MCDiscountAmt = new ReportParameter("MCDiscount", "-" + _mcDiscountAmt.ToString());
                    rv.LocalReport.SetParameters(MCDiscountAmt);
                }

                else if (Convert.ToInt32(insertedTransaction.BDDiscountAmt) != 0)
                {
                    Int64 _bcDiscountAmt = Convert.ToInt64(insertedTransaction.BDDiscountAmt);
                    ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "-" + _bcDiscountAmt.ToString());
                    rv.LocalReport.SetParameters(BCDiscountAmt);
                }
                else
                {
                    ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "0");
                    rv.LocalReport.SetParameters(BCDiscountAmt);
                }

                ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
                rv.LocalReport.SetParameters(TAmt);

                ReportParameter ShopName = new ReportParameter("ShopName", SettingController.ShopName);
                rv.LocalReport.SetParameters(ShopName);


                ReportParameter BranchName = new ReportParameter("BranchName", SettingController.BranchName);
                rv.LocalReport.SetParameters(BranchName);



                ReportParameter Phone = new ReportParameter("Phone", SettingController.PhoneNo);
                rv.LocalReport.SetParameters(Phone);

                ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.OpeningHours);
                rv.LocalReport.SetParameters(OpeningHours);

                ReportParameter TransactionId = new ReportParameter("TransactionId", resultId.ToString());
                rv.LocalReport.SetParameters(TransactionId);

                APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                rv.LocalReport.SetParameters(CounterName);

                ReportParameter PrintDateTime = new ReportParameter();
                switch (Utility.GetDefaultPrinter())
                {
                    case "A4 Printer":
                        PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd-MMM-yyyy"));
                        rv.LocalReport.SetParameters(PrintDateTime);
                        break;
                    case "Slip Printer":
                        PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                        rv.LocalReport.SetParameters(PrintDateTime);
                        break;
                }

                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                rv.LocalReport.SetParameters(CasherName);

                //ReportParameter TotalAmount = new ReportParameter("TotalAmount", insertedTransaction.TotalAmount.ToString());  //Edity By ZMH
                //rv.LocalReport.SetParameters(TotalAmount);

                Int64 totalAmountRep = insertedTransaction.TotalAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TotalAmount); //Edit By ZMH
                ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                rv.LocalReport.SetParameters(TotalAmount);

                ReportParameter TaxAmount = new ReportParameter("TaxAmount", insertedTransaction.TaxAmount.ToString());
                rv.LocalReport.SetParameters(TaxAmount);


                if (Convert.ToInt32(insertedTransaction.DiscountAmount) == 0)
                {
                    ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", insertedTransaction.DiscountAmount.ToString());
                    rv.LocalReport.SetParameters(DiscountAmount);
                }
                else
                {
                    ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + insertedTransaction.DiscountAmount.ToString());
                    rv.LocalReport.SetParameters(DiscountAmount);
                }

                ReportParameter PaidAmount = new ReportParameter("PaidAmount", "0");
                rv.LocalReport.SetParameters(PaidAmount);

                ReportParameter Change = new ReportParameter("Change", "0");
                rv.LocalReport.SetParameters(Change);


                if (Utility.GetDefaultPrinter() == "A4 Printer")
                {
                    ReportParameter CusAddress = new ReportParameter("CusAddress", cus.Address);
                    rv.LocalReport.SetParameters(CusAddress);
                }

                ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
                rv.LocalReport.SetParameters(AvailablePoint);

                //for (int i = 0; i <= 1; i++) //Edit By ZMH
                //{
                //    PrintDoc.PrintReport(rv, "Slip");
                //}


                if (Type == 4 || Type == 6)
              ////      PrintDoc.PrintReport(rv, Utility.GetDefaultPrinter());
                    Utility.Get_Print(rv);
                #endregion
            }
            }


            //MessageBox.Show("Payment Completed");
            
                if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                {
                    Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                    newForm.Clear();
                }
            
            this.Dispose();
        }

        #region for saving Sale Qty in Stock Transaction table
        private void Save_SaleQty_ToStockTransaction(List<Stock_Transaction> productList)
        {
            int _year, _month;

            _year = System.DateTime.Now.Year;
            _month = System.DateTime.Now.Month;
            Utility.Sale_Run_Process(_year, _month, productList);
        }
        #endregion
    }
}
