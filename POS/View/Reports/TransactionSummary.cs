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
using Microsoft.Reporting.WinForms;
using POS.APP_Data;

namespace POS
{
    public partial class TransactionSummary : Form
    {
        #region Variable

        POSEntities entity = new POSEntities();
        List<Transaction> transList;
        List<Transaction> RtransList;
        List<Transaction> DtransList;
        List<Transaction> CRtransList;
        List<Transaction> GCtransList;
        List<Transaction> CtransList;
        List<Transaction> MPUtransList;
        List<TransactionDetail> FOCtrnsList;
        List<Transaction> TesterCtrnsList;
        List<TransactionDetail> OtherFOCtrnsList;

        Boolean isstart = false;
        #endregion

        #region Event
        public TransactionSummary()
        {
            InitializeComponent();
        }

        private void TransactionSummary_Load(object sender, EventArgs e)
        {
            Utility.BindShop(cboshoplist);
            cboshoplist.Text = SettingController.DefaultShop.ShopName;
            Utility.ShopComBo_EnableOrNot(cboshoplist);
            isstart = true;
            this.reportViewer1.RefreshReport();
            LoadData();
            this.reportViewer1.RefreshReport();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region [print]
            long totalSale = 0, totalRefund = 0, totalDebt = 0, totalCreditRefund = 0, totalSummary = 0; long totalGiftCard = 0, totalCredit = 0, totalCreditRecieve = 0, totalCashInHand = 0, totalExpense = 0, totalIncomeAmount = 0, totalMPU = 0, totalFOC = 0, totalTester=0,totalReceived = 0; long totalDiscount = 0, totalRefundDiscount = 0, totalCreditRefundDiscount = 0; long totalMCDiscount = 0;

            totalSale = transList.Sum(x => x.TotalAmount).Value;

            foreach (Transaction t in transList)
            {
                long itemdiscount = (long)t.TransactionDetails.Sum(x => (x.UnitPrice * (x.DiscountRate / 100)) * x.Qty);
                totalDiscount += (long)t.DiscountAmount - itemdiscount;

                   if ((int)(t.MCDiscountAmt) != 0)
                {
                    totalMCDiscount += (long)(t.MCDiscountAmt);
                }
                else if ((int)(t.BDDiscountAmt) != 0)
                {
                    totalMCDiscount += (long)(t.BDDiscountAmt);
                }
            
            }
            totalRefund = RtransList.Sum(x => x.TotalAmount).Value;
            totalRefundDiscount = RtransList.Sum(x => x.DiscountAmount).Value;
            totalDebt = DtransList.Sum(x => x.TotalAmount).Value;
            totalCreditRefund = CRtransList.Sum(x => x.TotalAmount).Value;
            totalCreditRefundDiscount = CRtransList.Sum(x => x.DiscountAmount).Value;
            totalGiftCard = GCtransList.Sum(x => x.TotalAmount).Value;
            totalCredit = CtransList.Sum(x => x.TotalAmount).Value;
            totalCreditRecieve = CtransList.Sum(x => x.RecieveAmount).Value;
            totalMPU = MPUtransList.Sum(x => x.TotalAmount).Value;
            totalFOC = FOCtrnsList.Sum(x => x.TotalAmount).Value;
            totalTester = TesterCtrnsList.Sum(x => x.TotalAmount).Value;

            //totalSummary = (totalSale + totalDebt + totalCreditRefund + totalGiftCard) - totalRefund;
            totalSummary = ((totalSale + totalCredit + totalGiftCard + totalMPU) - (totalRefund + totalCreditRefund + totalFOC));
            totalCashInHand = (totalSale + totalDebt + totalCreditRecieve) - totalRefund;
            totalExpense = (totalRefund + totalCreditRefund + totalFOC);
            totalIncomeAmount = (totalSale + totalCredit + totalGiftCard + totalMPU);
            totalReceived = (totalSale + totalDebt + totalCreditRecieve);
            string reportPath = Application.StartupPath + "\\Reports\\Transaction Summary.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();

            ReportParameter TotalDiscount = new ReportParameter("TotalDiscount", totalDiscount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalDiscount);

            ReportParameter TotalMCDiscount = new ReportParameter("TotalMCDiscount", totalDiscount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalMCDiscount);

            ReportParameter TotalRefundDiscount = new ReportParameter("TotalRefundDiscount", totalRefundDiscount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalRefundDiscount);

            ReportParameter TotalCreditRefundDiscount = new ReportParameter("TotalCreditRefundDiscount", totalCreditRefundDiscount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalCreditRefundDiscount);

            ReportParameter ActualAmount = new ReportParameter("ActualAmount", totalReceived.ToString());
            reportViewer1.LocalReport.SetParameters(ActualAmount);

            ReportParameter TotalFOC = new ReportParameter("TotalFOC", totalFOC.ToString());
            reportViewer1.LocalReport.SetParameters(TotalFOC);

            ReportParameter TotalTester = new ReportParameter("TotalTester", totalTester.ToString());
            reportViewer1.LocalReport.SetParameters(TotalTester);

            ReportParameter TotalMPU = new ReportParameter("TotalMPU", totalMPU.ToString());
            reportViewer1.LocalReport.SetParameters(TotalMPU);

            ReportParameter TotalSale = new ReportParameter("TotalSale", totalSale.ToString());
            reportViewer1.LocalReport.SetParameters(TotalSale);

            ReportParameter CreditRecieve = new ReportParameter("CreditRecieve", totalCreditRecieve.ToString());
            reportViewer1.LocalReport.SetParameters(CreditRecieve);

            ReportParameter Expense = new ReportParameter("Expense", totalExpense.ToString());
            reportViewer1.LocalReport.SetParameters(Expense);

            ReportParameter IncomeAmount = new ReportParameter("IncomeAmount", totalIncomeAmount.ToString());
            reportViewer1.LocalReport.SetParameters(IncomeAmount);

            ReportParameter CashInHand = new ReportParameter("CashInHand", totalCashInHand.ToString());
            reportViewer1.LocalReport.SetParameters(CashInHand);

            ReportParameter TotalDebt = new ReportParameter("TotalDebt", totalDebt.ToString());
            reportViewer1.LocalReport.SetParameters(TotalDebt);

            ReportParameter TotalRefund = new ReportParameter("TotalRefund", totalRefund.ToString());
            reportViewer1.LocalReport.SetParameters(TotalRefund);

            ReportParameter TotalSummary = new ReportParameter("TotalSummary", totalSummary.ToString());
            reportViewer1.LocalReport.SetParameters(TotalSummary);

            ReportParameter TotalCreditRefund = new ReportParameter("TotalCreditRefund", totalCreditRefund.ToString());
            reportViewer1.LocalReport.SetParameters(TotalCreditRefund);

            ReportParameter TotalGiftCard = new ReportParameter("TotalGiftCard", totalGiftCard.ToString());
            reportViewer1.LocalReport.SetParameters(TotalGiftCard);

            ReportParameter TotalCredit = new ReportParameter("TotalCredit", totalCredit.ToString());
            reportViewer1.LocalReport.SetParameters(TotalCredit);

            ReportParameter HeaderTitle = new ReportParameter("HeaderTitle", "Transaction Summary for " + SettingController.ShopName);
            reportViewer1.LocalReport.SetParameters(HeaderTitle);

            ReportParameter Date = new ReportParameter("Date", " from " + dtpFrom.Value.Date.ToString("dd-MM-yyyy") + " To " + dtpTo.Value.Date.ToString("dd-MM-yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            PrintDoc.PrintReport(reportViewer1);
            #endregion
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        private void cboshoplist_selectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Function
        public void ResetTransLists()
        {
            transList = new List<Transaction>();
            RtransList = new List<Transaction>();
            DtransList = new List<Transaction>();
            CRtransList = new List<Transaction>();
            GCtransList = new List<Transaction>();
            CtransList = new List<Transaction>();
            MPUtransList = new List<Transaction>();
            FOCtrnsList = new List<TransactionDetail>();
            TesterCtrnsList = new List<Transaction>();
            OtherFOCtrnsList = new List<TransactionDetail>();
        }
        private void LoadData()
        {
            ResetTransLists();
            if (isstart == true)
            {
                int shopid = Convert.ToInt32(cboshoplist.SelectedValue);
                string shopname = (from p in entity.Shops where p.Id == shopid select p.ShopName).FirstOrDefault();
                string currentshortcode = (from p in entity.Shops where p.Id == shopid select p.ShortCode).FirstOrDefault();
                DateTime fromDate = dtpFrom.Value.Date;
                DateTime toDate = dtpTo.Value.Date;

                transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.IsComplete == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 1 && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2,2)==currentshortcode select t).ToList<Transaction>();
                RtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Refund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                DtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Settlement || t.Type == TransactionType.Prepaid) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                CRtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.CreditRefund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                GCtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale  && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                CtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Credit && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                MPUtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 3 && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                FOCtrnsList = (from t in entity.Transactions
                               join td in entity.TransactionDetails on t.Id equals td.TransactionId
                               where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true
                                   && t.Type == TransactionType.Sale && t.PaymentTypeId == 12 && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                               select td).ToList<TransactionDetail>();
                TesterCtrnsList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 11 && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                OtherFOCtrnsList = (from t in entity.Transactions
                                    join td in entity.TransactionDetails on t.Id equals td.TransactionId
                                    where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.IsComplete == true
                                   && (td.IsFOC == true)
                                   && t.PaymentTypeId != 12
                                    && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                    select td).ToList<TransactionDetail>();
                ShowReportViewer1(shopname,currentshortcode);
                lblPeriod.Text = fromDate.ToString("dd-MM-yyyy") + " to " + toDate.ToString("dd-MM-yyyy");
                // lblNumberofTransaction.Text = transList.Count.ToString();
                gbTransactionList.Text = "Transaction Summary Report";
                //lblTotalAmount.Text = "";
            }
        }

        private void ShowReportViewer1(string shopname,string currentshortcode)
        {
            long totalSale = 0, totalRefund = 0, totalDebt = 0, totalCreditRefund = 0, totalSummary = 0; long totalGiftCard = 0, totalCashFromGiftCard=0, totalCredit = 0, totalCreditRecieve = 0, totalCashInHand = 0, totalExpense = 0, totalIncomeAmount = 0, totalMPU = 0, 
                totalFOC = 0,otherTotalFOC,totalTester=0, totalReceived = 0; long totalDiscount = 0, totalRefundDiscount = 0, totalCreditRefundDiscount = 0;
            long totalMCDiscount = 0;long totalBankTransfer = 0; long totalGlobalCard = 0; long totalPay = 0;
            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date;
            POSEntities entity = new POSEntities();

            //TTN 
            var tranlist = (from t in entity.TransactionPaymentDetails
                            join td in entity.Transactions on t.TransactionId equals td.Id
                            join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                            where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                            && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                            && td.Id.Contains(currentshortcode) && p.Name == "Cash"
                            select td).ToList();
           // tranlist.AddRange(transList);
           // totalSale +=(long)tranlist.Select(x => x.TotalAmount).ToList().Distinct().Sum();

            totalSale += (long)(from t in entity.TransactionPaymentDetails
                                join td in entity.Transactions on t.TransactionId equals td.Id
                                join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                                && td.Id.Contains(currentshortcode) && p.Name == "Cash"
                                select t.Amount).ToList().Sum();


            List<Transaction> discounttransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit)  && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2,2)==currentshortcode select t).ToList<Transaction>();
            foreach (Transaction t in discounttransList)
            {
               // long itemdiscount = (long)t.TransactionDetails.Sum(x => (x.UnitPrice * (x.DiscountRate / 100)) * x.Qty);
                totalDiscount += (long)t.DiscountAmount;

                if ((int)(t.MCDiscountAmt) != 0)
                {
                    totalMCDiscount += (long)(t.MCDiscountAmt);
                }
                else if ((int)(t.BDDiscountAmt) != 0)
                {
                    totalMCDiscount += (long)(t.BDDiscountAmt);
                }


            }
            totalRefund = RtransList.Sum(x => x.RecieveAmount).Value;
            totalRefundDiscount = RtransList.Sum(x => x.DiscountAmount).Value;
            totalDebt = DtransList.Sum(x => x.TotalAmount).Value;
            totalCreditRefund = CRtransList.Sum(x => x.RecieveAmount).Value;
            totalCreditRefundDiscount = CRtransList.Sum(x => x.DiscountAmount).Value;

            totalGiftCard = (long)(from t in entity.TransactionPaymentDetails
                                   join td in entity.Transactions on t.TransactionId equals td.Id
                                   join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                   where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                   && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                                   && td.Id.Contains(currentshortcode) 
                                   && p.Name == "Gift Card"
                                   select t.Amount).ToList().Sum();

            totalCashFromGiftCard = (long)(from t in entity.TransactionPaymentDetails
                                           join td in entity.Transactions on t.TransactionId equals td.Id
                                           join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                           where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                           && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                                           && td.Id.Contains(currentshortcode)
                                           && p.Name == "Gift Card"
                                           select t.Amount).ToList().Sum(); 

            totalCredit = (long)(from t in entity.TransactionPaymentDetails
                                 join td in entity.Transactions on t.TransactionId equals td.Id
                                 join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                 where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                 && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                                 && td.Id.Contains(currentshortcode)
                                 && p.Name == "Credit"
                                 select t.Amount).ToList().Sum();

            totalCreditRecieve = CtransList.Sum(x => x.RecieveAmount).Value;
            var totalMP = (from t in entity.TransactionPaymentDetails
                           join td in entity.Transactions on t.TransactionId equals td.Id
                           join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                           where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                           && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                           && td.Id.Contains(currentshortcode)
                           && p.PaymentTypeId == 3
                           select td).ToList();
            //totalMP.AddRange(MPUtransList);
           // totalMPU = (long)totalMP.Select(x => x.TotalAmount).ToList().Distinct().Sum();
           totalMPU += (long)(from t in entity.TransactionPaymentDetails
                              join td in entity.Transactions on t.TransactionId equals td.Id
                              join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                              where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                              && td.IsDeleted == false && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                              && td.Id.Contains(currentshortcode)
                              && p.PaymentTypeId == 3
                              select t.Amount).ToList().Sum();
            // totalFOC = FOCtrnsList.Sum(x => x.TotalAmount).Value;
            totalFOC = (long)(from t in entity.TransactionPaymentDetails
                              join td in entity.Transactions on t.TransactionId equals td.Id
                              join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                              where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                              && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                              && td.Id.Contains(currentshortcode)
                              && p.Name == "FOC"
                              select td.TotalAmount).ToList().Distinct().Sum();
            totalFOC += (long)(from t in entity.Transactions
                               join td in entity.TransactionDetails on t.Id equals td.TransactionId
                               where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                     && t.IsDeleted == false && t.IsComplete == true && td.IsFOC == true
                               select td.SellingPrice).ToList().Sum();

            totalBankTransfer = (long)(from t in entity.TransactionPaymentDetails
                                       join td in entity.Transactions on t.TransactionId equals td.Id
                                       join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                       where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                       && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                                       && td.Id.Contains(currentshortcode)
                                       && p.PaymentTypeId == 5
                                       select t.Amount).ToList().Sum();
            totalGlobalCard = (long)(from t in entity.TransactionPaymentDetails
                                     join td in entity.Transactions on t.TransactionId equals td.Id
                                     join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                     where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                     && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                                     && td.Id.Contains(currentshortcode)
                                     && (p.PaymentTypeId == 4 || p.PaymentTypeId == 9 || p.PaymentTypeId == 10)
                                     select t.Amount).ToList().Sum();

            totalPay = (long)(from t in entity.TransactionPaymentDetails
                              join td in entity.Transactions on t.TransactionId equals td.Id
                              join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                              where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                              && td.IsDeleted == false && td.IsComplete == true && td.IsActive == true
                              && td.Id.Contains(currentshortcode)
                              && p.PaymentTypeId == 6
                              select t.Amount).ToList().Sum();

            otherTotalFOC = Convert.ToInt32(OtherFOCtrnsList.Sum((x => x.SellingPrice * x.Qty)));

            
            totalTester = TesterCtrnsList.Sum(x => x.TotalAmount).Value;
            
            //totalSummary = (totalSale + totalDebt + totalCreditRefund + totalGiftCard) - totalRefund;
            totalSummary = ((totalSale + totalCredit + totalGiftCard + totalCashFromGiftCard + totalMPU+totalBankTransfer+totalPay+totalGlobalCard) - (totalRefund + totalCreditRefund + totalFOC));
            totalCashInHand = (totalSale) - totalRefund;
            totalExpense = (totalRefund + totalCreditRefund + totalFOC);
            // totalIncomeAmount = (totalSale + totalCredit + totalGiftCard + totalCashFromGiftCard + totalMPU + totalPay+totalGlobalCard+totalBankTransfer);
            totalIncomeAmount = totalSale + totalCredit + totalMPU + totalPay + totalGlobalCard + totalBankTransfer;
            totalReceived = (totalSale);
            string reportPath = Application.StartupPath + "\\Reports\\Transaction Summary.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();

            ReportParameter TotalDiscount = new ReportParameter("TotalDiscount", totalDiscount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalDiscount);

            ReportParameter TotalMCDiscount = new ReportParameter("TotalMCDiscount", totalMCDiscount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalMCDiscount);

            ReportParameter TotalRefundDiscount = new ReportParameter("TotalRefundDiscount", totalRefundDiscount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalRefundDiscount);

            ReportParameter TotalCreditRefundDiscount = new ReportParameter("TotalCreditRefundDiscount", totalCreditRefundDiscount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalCreditRefundDiscount);

            ReportParameter ActualAmount = new ReportParameter("ActualAmount", totalReceived.ToString());
            reportViewer1.LocalReport.SetParameters(ActualAmount);

            ReportParameter TotalFOC = new ReportParameter("TotalFOC", totalFOC.ToString());
            reportViewer1.LocalReport.SetParameters(TotalFOC);


            ReportParameter TotalOtherFOC = new ReportParameter("TotalOtherFOC", otherTotalFOC.ToString());
            reportViewer1.LocalReport.SetParameters(TotalOtherFOC);

            ReportParameter TotalTester = new ReportParameter("TotalTester", totalTester.ToString());
            reportViewer1.LocalReport.SetParameters(TotalTester);

            ReportParameter TotalMPU = new ReportParameter("TotalMPU", totalMPU.ToString());
            reportViewer1.LocalReport.SetParameters(TotalMPU);

            ReportParameter BankTransfer = new ReportParameter("BankTransfer", totalBankTransfer.ToString());
            reportViewer1.LocalReport.SetParameters(BankTransfer);

            ReportParameter GlobalCard = new ReportParameter("GlobalCard", totalGlobalCard.ToString());
            reportViewer1.LocalReport.SetParameters(GlobalCard);

            ReportParameter Pay = new ReportParameter("Pay", totalPay.ToString());
            reportViewer1.LocalReport.SetParameters(Pay);



            ReportParameter TotalSale = new ReportParameter("TotalSale", (totalSale).ToString());
            reportViewer1.LocalReport.SetParameters(TotalSale);

            ReportParameter CreditRecieve = new ReportParameter("CreditRecieve", totalCreditRecieve.ToString());
            reportViewer1.LocalReport.SetParameters(CreditRecieve);

            ReportParameter Expense = new ReportParameter("Expense", totalExpense.ToString());
            reportViewer1.LocalReport.SetParameters(Expense);

            ReportParameter IncomeAmount = new ReportParameter("IncomeAmount", totalIncomeAmount.ToString());
            reportViewer1.LocalReport.SetParameters(IncomeAmount);

            ReportParameter CashInHand = new ReportParameter("CashInHand", totalCashInHand.ToString());
            reportViewer1.LocalReport.SetParameters(CashInHand);

            ReportParameter TotalDebt = new ReportParameter("TotalDebt", totalDebt.ToString());
            reportViewer1.LocalReport.SetParameters(TotalDebt);

            ReportParameter TotalRefund = new ReportParameter("TotalRefund", totalRefund.ToString());
            reportViewer1.LocalReport.SetParameters(TotalRefund);

            ReportParameter TotalSummary = new ReportParameter("TotalSummary", totalSummary.ToString());
            reportViewer1.LocalReport.SetParameters(TotalSummary);

            
            ReportParameter TotalCreditRefund = new ReportParameter("TotalCreditRefund", totalCreditRefund.ToString());
            reportViewer1.LocalReport.SetParameters(TotalCreditRefund);

            ReportParameter TotalGiftCard = new ReportParameter("TotalGiftCard", totalGiftCard.ToString());
            reportViewer1.LocalReport.SetParameters(TotalGiftCard);

            ReportParameter TotalCredit = new ReportParameter("TotalCredit", totalCredit.ToString());
            reportViewer1.LocalReport.SetParameters(TotalCredit);

            ReportParameter HeaderTitle = new ReportParameter("HeaderTitle", "Transaction Summary for " + shopname  );
            reportViewer1.LocalReport.SetParameters(HeaderTitle);

            ReportParameter Date = new ReportParameter("Date", " from " + dtpFrom.Value.Date.ToString("dd-MM-yyyy") + " To " + dtpTo.Value.Date.ToString("dd-MM-yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            reportViewer1.RefreshReport();
        }

        #endregion

  



    }
}
