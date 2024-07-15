using Microsoft.Reporting.WinForms;
using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class frmDailySaleReportForMainOffice : Form
    {
        #region Variable
        List<Transaction> DtransList = new List<Transaction>();
        POSEntities entity = new POSEntities();
        //List<Product> itemList = new List<Product>();
        List<ReportItemSummary> itemList = new List<ReportItemSummary>();
        List<ReportItemSummary> IList = new List<ReportItemSummary>();
        long CashTotal = 0, CreditTotal = 0, FOCAmount = 0, MPUAmount = 0, TesterAmount = 0, GiftCardAmount = 0, Total = 0, CreditReceive = 0, RefundAmt = 0; long UseGiftAmount = 0; long CashAmtFromGiftCard = 0;
        long totalSettlement = 0;
        long Card = 0, BankPay = 0, BankTrasfer = 0;
        List<Transaction> AllTranslist = new List<Transaction>();
        List<ReportItemSummary> FinalResultList = new List<ReportItemSummary>();

        

        Boolean Isstart = false;
        #endregion        

        public frmDailySaleReportForMainOffice()
        {
            InitializeComponent();
        }

        private void frmDailySaleReportForMainOffice_Load(object sender, EventArgs e)
        {
            Utility.BindShop(cboshoplist);
            cboshoplist.Text = SettingController.DefaultShop.ShopName;
            BindBrand();
            Isstart = true;
            Utility.ShopComBo_EnableOrNot(cboshoplist);
            LoadData();
            this.reportViewer1.RefreshReport();
        }

        private void BindBrand()
        {
            List<APP_Data.Brand> BrandList = new List<APP_Data.Brand>();
            APP_Data.Brand brandObj1 = new APP_Data.Brand();
            brandObj1.Id = 0;
            brandObj1.Name = "Select";
            BrandList.Add(brandObj1);
            BrandList.AddRange((from bList in entity.Brands select bList).ToList());
            cboBrand.DataSource = BrandList;
            cboBrand.DisplayMember = "Name";
            cboBrand.ValueMember = "Id";
            cboBrand.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboBrand.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        #region Value Change
        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }
               
        private void cboshoplist_selectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Function

        private void LoadData()
        {
            if (Isstart == true)
            {
                int shopid = Convert.ToInt32(cboshoplist.SelectedValue);
                int BrandId = 0;
                if(cboBrand.SelectedIndex > 0)
                {
                    BrandId =Convert.ToInt32(cboBrand.SelectedValue);
                }
                string currentshortcode = (from p in entity.Shops where p.Id == shopid select p.ShortCode).FirstOrDefault();
                DateTime fromDate = dtFrom.Value.Date;
                DateTime toDate = dtTo.Value.Date;
                bool IsCash = true, IsCredit = true, IsFOC = true, IsMPU = true, IsGiftCard = true, IsTester = true, IsRefund = true;
                CashTotal = 0; CreditTotal = 0; FOCAmount = 0; MPUAmount = 0; TesterAmount = 0; GiftCardAmount = 0; RefundAmt = 0; Total = 0;
                IList.Clear();
                itemList.Clear();
                //System.Data.Objects.ObjectResult<DailySaleReportForMainOffice_Result> resultList; //KHS
                List<Transaction> transList = new List<Transaction>();
                FinalResultList = new List<ReportItemSummary>();
                transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit || t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
               
                DtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Settlement || t.Type == TransactionType.Prepaid) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();

                List<DailySaleReportForMainOffice_Result>  resultList = entity.DailySaleReportForMainOffice(fromDate, toDate, currentshortcode, BrandId).ToList();
                foreach (DailySaleReportForMainOffice_Result r in resultList)
                {
                    ReportItemSummary p = new ReportItemSummary();
                    p.Id = r.ItemId;
                    p.Name = r.ItemName;
                    p.Qty = (int)r.ItemQty;
                    p.UnitPrice = Convert.ToInt32(r.UnitPrice);
                    p.SellingPrice = Convert.ToInt32(r.SellingPrice);
                    p.totalAmount = Convert.ToInt64(r.ItemTotalAmount);
                    p.PaymentId = (int)r.PaymentTypeId;
                    p.Size = r.Size;
                    p.IsFOC = Convert.ToBoolean(r.IsFOC);
                    p.discount = r.tddiscount;
                    //  p.Remark = r.Type;

                    if (r.Type == "Refund")
                    {
                        p.Remark = "Refund";
                    }
                    else if (r.Type == "Credit Refund")
                    {
                        p.Remark = " Credit Refund";
                    }
                    else
                    {
                        if (r.IsFOC == true || r.PaymentTypeId == 12)
                        {
                            p.Remark = "FOC";
                        }
                        else
                        {
                            p.Remark = "";
                        }
                    }
                    FinalResultList.Add(p);
                }
                AllTranslist.Clear();
                CreditReceive = 0;
                UseGiftAmount = 0;
        
                string[] type = { "Refund", "CreditRefund" };
                if (IsCash)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 1 && !type.Contains(x.Remark)).ToList());
                //    CashTotal += Convert.ToInt64(transList.Where(x => x.PaymentTypeId == 1 && !type.Contains(x.Type)).Sum(x => x.RecieveAmount));
                    CashTotal += (long)(from t in entity.TransactionPaymentDetails
                                        join td in entity.Transactions on t.TransactionId equals td.Id
                                        join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                        where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                        && td.IsComplete == true && td.IsActive == true && td.Id.Contains(currentshortcode) 
                                        
                                        && td.IsDeleted == false && p.Name == "Cash"
                                        select t.Amount).ToList().Sum();
                    
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 1 && !type.Contains(x.Type)).ToList());
                }
                if (IsCredit)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 2 && !type.Contains(x.Remark)).ToList());
                   // CreditTotal += Convert.ToInt64(transList.Where(x => x.Type == "Credit").Sum(x => x.TotalAmount));
                    CreditTotal += (long)(from t in entity.TransactionPaymentDetails
                                          join td in entity.Transactions on t.TransactionId equals td.Id
                                          join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                          where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                          && td.IsComplete == true && td.IsActive == true && td.Id.Contains(currentshortcode)
                                          && td.IsDeleted == false && p.Name == "Credit"
                                          select t.Amount).ToList().Sum();
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 2 && !type.Contains(x.Type)).ToList());
                    CreditReceive += Convert.ToInt64(transList.Where(x => x.PaymentTypeId == 2 && !type.Contains(x.Type)).Sum(x => x.RecieveAmount));
                }
                if (IsGiftCard)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 8  && !type.Contains(x.Remark)).ToList());
                    GiftCardAmount = FinalResultList.Where(x => x.PaymentId == 8 && !type.Contains(x.Remark)).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentType.Name == "Gift Card" && !type.Contains(x.Type)).ToList());
                   // UseGiftAmount += Convert.ToInt64(transList.Where(x => x.PaymentTypeId == 3).Sum(x => x.GiftCardAmount));
                    UseGiftAmount += (long)(from t in entity.TransactionPaymentDetails
                                            join td in entity.Transactions on t.TransactionId equals td.Id
                                            join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                            where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                            && td.IsComplete == true && td.IsActive == true && td.Id.Contains(currentshortcode)
                                            && td.IsDeleted == false && p.Name == "Gift Card"
                                            select t.Amount).ToList().Sum();
                    CashAmtFromGiftCard += Convert.ToInt64(transList.Where(x => x.PaymentType.Name == "Gift Card" && !type.Contains(x.Type)).Sum(x => x.TotalAmount - x.GiftCardAmount));

                }
                if (IsRefund)
                {

                    RefundAmt += FinalResultList.Where(x => type.Contains(x.Remark)).Sum(x => x.SellingPrice * x.Qty);
                    AllTranslist.AddRange(transList.Where(x => type.Contains(x.Type)).ToList());
                    
                }
                if (IsFOC)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 12).ToList());

                  //  FOCAmount += FinalResultList.Where(x => x.Remark == "FOC").Sum(x => x.SellingPrice * x.Qty);
                     FOCAmount += FinalResultList.Where(x => x.PaymentId == 12).Sum(x => x.totalAmount);
                    //FOCAmount += (long)(from t in entity.TransactionPaymentDetails
                    //                    join td in entity.Transactions on t.TransactionId equals td.Id
                    //                    join tds in entity.TransactionDetails on td.Id equals tds.TransactionId
                    //                    join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                    //                    join trd in entity.TransactionDetails on td.Id equals trd.TransactionId
                    //                    where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                    //                    && td.IsDeleted == false && (p.Name == "FOC")
                    //                    select tds.SellingPrice).ToList().Distinct().Sum();

                    FOCAmount += (long)(from t in entity.Transactions
                                        join td in entity.TransactionDetails on t.Id equals td.TransactionId
                                        where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                              && t.IsDeleted == false && t.IsComplete == true && td.IsFOC == true
                                        select td.SellingPrice).ToList().Sum();


                    AllTranslist.AddRange(transList.Where(x => x.PaymentType.Name == "FOC").ToList());
                }
                if (IsMPU)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 3).ToList());
                   // MPUAmount += Convert.ToInt64(transList.Where(x => x.PaymentType.Name == "MPU Card").Sum(x => x.RecieveAmount));
                    MPUAmount += (long)(from t in entity.TransactionPaymentDetails
                                        join td in entity.Transactions on t.TransactionId equals td.Id
                                        join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                        where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                        && td.IsDeleted == false & p.PaymentTypeId == 3
                                        select t.Amount).ToList().Sum();
                    AllTranslist.AddRange(transList.Where(x => x.PaymentType.Name == "MPU Card").ToList());
                   
                }
                if (IsTester)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 11).ToList());
                    TesterAmount += FinalResultList.Where(x => x.PaymentId == 11).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentType.Name == "Tester").ToList());
                }

                Card = (long)(from t in entity.TransactionPaymentDetails
                              join td in entity.Transactions on t.TransactionId equals td.Id
                              join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                              where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                              && td.IsDeleted == false & (p.PaymentTypeId == 4 || p.PaymentTypeId == 9 || p.PaymentTypeId == 10)
                              select t.Amount).ToList().Sum();

                BankPay = (long)(from t in entity.TransactionPaymentDetails
                                 join td in entity.Transactions on t.TransactionId equals td.Id
                                 join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                 where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                 && td.IsDeleted == false & p.PaymentTypeId == 6
                                 select t.Amount).ToList().Sum();

                BankTrasfer = (long)(from t in entity.TransactionPaymentDetails
                                     join td in entity.Transactions on t.TransactionId equals td.Id
                                     join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                     where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                     && td.IsDeleted == false & p.PaymentTypeId == 5
                                     select t.Amount).ToList().Sum();

                gbList.Text = "Daily Sales Report";
                ShowReportViewer();
            }
        }

        private void ShowReportViewer()
        {
            int shopid = Convert.ToInt32(cboshoplist.SelectedValue);
            string shopname = (from p in entity.Shops where p.Id == shopid select p.ShopName).FirstOrDefault();
            dsReportTemp dsReport = new dsReportTemp();
            //dsReportTemp.ItemListDataTable dtItemReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["LO'c_ItemSummary"];
            dsReportTemp._LO_c_ItemSummaryDataTable dtItemReport = (dsReportTemp._LO_c_ItemSummaryDataTable)dsReport.Tables["LO'c_ItemSummary"];


            FinalResultList = FinalResultList.OrderBy(x => x.Name).ToList();
            foreach (ReportItemSummary p in FinalResultList)
            {
                //dsReportTemp.ItemListRow newRow = dtItemReport.NewItemListRow();
                dsReportTemp._LO_c_ItemSummaryRow newRow = dtItemReport.New_LO_c_ItemSummaryRow();
                newRow.ItemCode = p.Id;
                newRow.Name = p.Name;
                newRow.Size = p.Size;
                newRow.Qty = (Int16)p.Qty;
                newRow.UnitPrice = (Int16)p.UnitPrice;
                newRow.TotalAmount = Convert.ToInt64(p.totalAmount);
                newRow.SellingPrice = Convert.ToInt64(p.SellingPrice);
                if (!string.IsNullOrEmpty(p.discount) && !string.IsNullOrEmpty(p.Remark))
                {

                    newRow.Remark = p.Remark.ToString() + " ," + p.discount.ToString();
                }
                else if (!string.IsNullOrEmpty(p.discount) && string.IsNullOrEmpty(p.Remark))
                {
                    newRow.Remark = p.discount.ToString();
                }
                else if (string.IsNullOrEmpty(p.discount) && !string.IsNullOrEmpty(p.Remark))
                {
                    newRow.Remark = p.Remark.ToString();
                }

                dtItemReport.Add_LO_c_ItemSummaryRow(newRow);
            }
            Total = CashTotal + CreditTotal + FOCAmount + MPUAmount + UseGiftAmount + TesterAmount + Card + BankPay + BankTrasfer;

            //if (rdbSale.Checked)
            //{
            totalSettlement = DtransList.Sum(x => x.TotalAmount).Value;
            //}
            //else
            //{
            //    totalSettlement = 0;
            //}
            //CashTotal += GiftCardAmount;
            //To Find DiscountAmountOfAllTransactions
            decimal OverAllDis = 0;
            decimal OverAllMCDis = 0;
            decimal CreditMCDis = 0;

            foreach (Transaction t in AllTranslist)
            {
                List<TransactionDetail> tdList = new List<TransactionDetail>();
                tdList = t.TransactionDetails.ToList();
                decimal itemDis = 0;
                //foreach (TransactionDetail td in tdList)
                //{
                //    itemDis += Convert.ToDecimal((td.UnitPrice * (td.DiscountRate / 100)) * td.Qty);
                //}
                OverAllDis += Convert.ToDecimal(t.DiscountAmount - itemDis);

                var creditData = (from a in AllTranslist where a.Type == "Credit" select a).ToList();
                Int64 _totalMCDisocunt = Convert.ToInt64(creditData.Sum(x => x.MCDiscountAmt));
                Int64 _totalBDDiscount = Convert.ToInt64(creditData.Sum(x => x.BDDiscountAmt));

                CreditMCDis = _totalMCDisocunt + _totalBDDiscount;


                if ((int)(t.MCDiscountAmt == null ? 0 : t.MCDiscountAmt) != 0)
                {
                    OverAllMCDis += Convert.ToDecimal(t.MCDiscountAmt);
                }
                else if ((int)(t.BDDiscountAmt == null ? 0 : t.BDDiscountAmt) != 0)
                {
                    OverAllMCDis += Convert.ToDecimal(t.BDDiscountAmt);
                }
            }
            // decimal actualAmount = (Convert.ToDecimal(CashTotal + CreditReceive) - (OverAllDis) - (OverAllMCDis)) + totalSettlement;
            decimal actualAmount = (Convert.ToDecimal(CashTotal + CreditReceive + CashAmtFromGiftCard) + (totalSettlement + CreditMCDis) - (OverAllDis) - (OverAllMCDis));

            ReportDataSource rds = new ReportDataSource("ItemSummary", dsReport.Tables["LO'c_ItemSummary"]);
            string reportPath = Application.StartupPath + "\\Reports\\Daily_Summary.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            ReportParameter ItemReportTitle = new ReportParameter("ItemReportTitle", gbList.Text + " for " + shopname);
            reportViewer1.LocalReport.SetParameters(ItemReportTitle);

            ReportParameter Date = new ReportParameter("Date", " from " + dtFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            ReportParameter TotalAmount = new ReportParameter("TotalAmount", (Total - FOCAmount).ToString());
            reportViewer1.LocalReport.SetParameters(TotalAmount);

            ReportParameter CreditAmount = new ReportParameter("CreditAmount", (CreditTotal).ToString());
            reportViewer1.LocalReport.SetParameters(CreditAmount);

            ReportParameter CashAmount = new ReportParameter("CashAmount", (CashTotal).ToString());
            reportViewer1.LocalReport.SetParameters(CashAmount);

            ReportParameter BankTransfer = new ReportParameter("BankTransfer", (BankTrasfer).ToString());
            reportViewer1.LocalReport.SetParameters(BankTransfer);

            ReportParameter Pay = new ReportParameter("Pay", (BankPay).ToString());
            reportViewer1.LocalReport.SetParameters(Pay);


            ReportParameter GlobalCard = new ReportParameter("GlobalCard", (Card).ToString());
            reportViewer1.LocalReport.SetParameters(GlobalCard);

            ReportParameter DisAmount = new ReportParameter("DisAmount", OverAllDis.ToString());
            reportViewer1.LocalReport.SetParameters(DisAmount);


            ReportParameter MemberCardDiscount = new ReportParameter("MemberCardDiscount", OverAllMCDis.ToString());
            reportViewer1.LocalReport.SetParameters(MemberCardDiscount);

            ReportParameter UsedGiftAmount = new ReportParameter("UsedGiftAmount", UseGiftAmount.ToString());
            reportViewer1.LocalReport.SetParameters(UsedGiftAmount);

            ReportParameter FOC = new ReportParameter("FOC", FOCAmount.ToString());
            reportViewer1.LocalReport.SetParameters(FOC);

            ReportParameter MPU = new ReportParameter("MPU", MPUAmount.ToString());
            reportViewer1.LocalReport.SetParameters(MPU);

            ReportParameter Tester = new ReportParameter("Tester", TesterAmount.ToString());
            reportViewer1.LocalReport.SetParameters(Tester);

            ReportParameter TotalSettlement = new ReportParameter("TotalSettlement", totalSettlement.ToString());
            reportViewer1.LocalReport.SetParameters(TotalSettlement);



            ReportParameter ActualAmount = new ReportParameter("ActualAmount", (CashTotal + totalSettlement).ToString());
            reportViewer1.LocalReport.SetParameters(ActualAmount);

            ReportParameter TotalRefund = new ReportParameter("TotalRefund", RefundAmt.ToString());
            reportViewer1.LocalReport.SetParameters(TotalRefund);


            ReportParameter CashInHand = new ReportParameter("CashInHand", (CashTotal).ToString());
            reportViewer1.LocalReport.SetParameters(CashInHand);
            reportViewer1.RefreshReport();



        }
        #endregion
    }
}
