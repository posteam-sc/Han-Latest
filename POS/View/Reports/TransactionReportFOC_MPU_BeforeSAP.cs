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
    public partial class TransactionReportFOC_MPU_BeforeSAP : Form
    {
        #region Variable

        POSEntities entity = new POSEntities();
        List<Transaction> transList = new List<Transaction>();
        List<Transaction> RtransList = new List<Transaction>();
        List<Transaction> DtransList = new List<Transaction>();
        List<Transaction> CRtransList = new List<Transaction>();
        List<Transaction> GCtransList = new List<Transaction>();
        List<Transaction> CtransList = new List<Transaction>();
        List<Transaction> MPUtransList = new List<Transaction>();
        List<Transaction> GlobalCardList = new List<Transaction>();
        List<Transaction> PayList = new List<Transaction>();
        List<Transaction> BankTransferList = new List<Transaction>();
        List<Transaction> FOCtrnsList = new List<Transaction>();
        private ToolTip tp = new ToolTip();
        List<Transaction> TesterList = new List<Transaction>();
        Boolean Isstart = false;

        #endregion

        #region Events
        public TransactionReportFOC_MPU_BeforeSAP()
        {
            InitializeComponent();
        }
        private void TransactionReportFOC_MPU_BeforeSAP_Load(object sender, EventArgs e)
        {
            List<APP_Data.Counter> counterList = new List<APP_Data.Counter>();
            APP_Data.Counter counterObj = new APP_Data.Counter();
            counterObj.Id = 0;
            counterObj.Name = "Select";
            counterList.Add(counterObj);
            counterList.AddRange((from c in entity.Counters orderby c.Id select c).ToList());
            cboCounter.DataSource = counterList;
            cboCounter.DisplayMember = "Name";
            cboCounter.ValueMember = "Id";

            List<APP_Data.User> userList = new List<APP_Data.User>();
            APP_Data.User userObj = new APP_Data.User();
            userObj.Id = 0;
            userObj.Name = "Select";
            userList.Add(userObj);
            userList.AddRange((from u in entity.Users orderby u.Id select u).ToList());
            cboCashier.DataSource = userList;
            cboCashier.DisplayMember = "Name";
            cboCashier.ValueMember = "Id";

            Utility.BindShop(cboshoplist);
            cboshoplist.Text = SettingController.DefaultShop.ShopName;
            Utility.ShopComBo_EnableOrNot(cboshoplist);
            Isstart = true;
            this.reportViewer1.RefreshReport();
            LoadData();
            gbPaymentType.Enabled = false;
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();

        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbSale_CheckedChanged(object sender, EventArgs e)
        {
            gbPaymentType.Enabled = true;
            LoadData();
        }

        private void rdbRefund_CheckedChanged(object sender, EventArgs e)
        {
            gbPaymentType.Enabled = false;
            LoadData();
        }

        private void rdbSummary_CheckedChanged(object sender, EventArgs e)
        {
            gbPaymentType.Enabled = false;
            LoadData();
        }



        private void chkCashier_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCashier.Checked)
            {
                lblCashierName.Enabled = true;
                cboCashier.Enabled = true;
            }
            else
            {
                lblCashierName.Enabled = false;
                cboCashier.Enabled = false;
                cboCashier.SelectedIndex = 0;
                LoadData();
            }

        }

        private void chkCounter_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCounter.Checked)
            {
                lblCounterName.Enabled = true;
                cboCounter.Enabled = true;
            }
            else
            {
                lblCounterName.Enabled = false;
                cboCounter.Enabled = false;
                cboCounter.SelectedIndex = 0;
                LoadData();
            }

        }


        private void cboCashier_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkChange1();
            LoadData();
        }

        private void cboCounter_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkChange2();
            LoadData();
        }

        private void cboshoplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        private void rdbDebt_CheckedChanged(object sender, EventArgs e)
        {
            gbPaymentType.Enabled = false;
            LoadData();
        }


        private void chkChange1()
        {
            if (cboCashier.Text == "Select")
            {
                chkCashier.Checked = false;
                cboCashier.Enabled = false;
            }
        }
        private void chkChange2()
        {
            if (cboCounter.Text == "Select")
            {
                chkCounter.Checked = false;
                cboCounter.Enabled = false;
            }
        }

        private void chkCash_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkGiftCard_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkCredit_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        private void chkMPU_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkFOC_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkTester_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkPay_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        private void chkGlobalCard_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkBankTransfer_CheckedChanged(object sender, EventArgs e)
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
                string currentshortcode = (from p in entity.Shops where p.Id == shopid select p.ShortCode).FirstOrDefault();

                DateTime fromDate = dtpFrom.Value.Date;
                DateTime toDate = dtpTo.Value.Date;
                bool IsSale = rdbSale.Checked;
                bool IsRefund = rdbRefund.Checked;
                bool IsDebt = rdbDebt.Checked;
                bool IsCounter = chkCounter.Checked;
                bool IsCashier = chkCashier.Checked;
                bool IsCredit = chkCredit.Checked;
                bool IsCash = chkCash.Checked;
                bool IsGiftCard = chkGiftCard.Checked;
                bool IsFOC = chkFOC.Checked;
                bool IsMPU = chkMPU.Checked;
                bool IsTester = chkTester.Checked;
                bool IsSummary = rdbSummary.Checked;
                int FOCAmt = 0;

                int CashierId = 0;
                int CounterId = 0;

                Boolean hasError = false;

                tp.RemoveAll();
                tp.IsBalloon = true;
                tp.ToolTipIcon = ToolTipIcon.Error;
                tp.ToolTipTitle = "Error";
                //Validation
                if (IsCounter)
                {
                    if (cboCounter.SelectedIndex == 0)
                    {
                        tp.SetToolTip(cboCounter, "Error");
                        tp.Show("Please select counter name!", cboCounter);
                        hasError = true;
                    }
                }
                else if (IsCashier)
                {
                    if (cboCashier.SelectedIndex == 0)
                    {
                        tp.SetToolTip(cboCashier, "Error");
                        tp.Show("Please select counter name!", cboCashier);
                        hasError = true;
                    }
                }
                if (!hasError)
                {
                    if (cboCounter.SelectedIndex > 0)
                    {
                        CounterId = Convert.ToInt32(cboCounter.SelectedValue);
                    }
                    if (cboCashier.SelectedIndex > 0)
                    {
                        CashierId = Convert.ToInt32(cboCashier.SelectedValue);
                    }
                    #region get transaction with cashier & counter
                    if (IsCashier == true && IsCounter == true)
                    {
                        if (IsSale)
                        {
                            //*Update SD*
                            transList = (from t in entity.Transactions
                                         join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                         join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                         where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true
                                             && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) &&
                                             (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                         orderby p.Id
                                         select t).ToList<Transaction>();
                            //FOCAmt= transList
                            #region Old Code
                            //if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            ////one payment type false
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && t.PaymentTypeId != 3 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && t.PaymentTypeId != 2 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && t.PaymentTypeId != 1 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && t.PaymentTypeId != 5 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && t.PaymentTypeId != 4 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //// two type false
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 2) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 4) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 4) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 5 && t.PaymentTypeId != 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 4 && t.PaymentTypeId != 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 4 && t.PaymentTypeId != 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            ////three type false
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 3 || t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 2) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 2 || t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 2 || t.PaymentTypeId == 1) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 1) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 2 || t.PaymentTypeId == 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 1 || t.PaymentTypeId == 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 1 || t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            ////four type is false
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 1) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 2) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 4) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            ////all false
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.UserId == CashierId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 2 && t.PaymentTypeId != 3 && t.PaymentTypeId != 4 && t.PaymentTypeId != 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            #endregion

                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Sale Transaction Report for ";
                            // lblTotalAmount.Text = "";
                        }
                        else if (IsRefund)
                        {
                            transList = (from t in entity.Transactions
                                         where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true
                                             && t.IsActive == true && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) && t.CounterId == CounterId && t.UserId == CashierId
                                             && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                         select t).ToList<Transaction>();
                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Refund Transaction Report for ";
                            //lblTotalAmount.Text = "";
                        }
                        else if (IsDebt)
                        {
                            transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Prepaid || t.Type == TransactionType.Settlement) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();

                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Debt Transaction Report for ";
                            //lblTotalAmount.Text = "";
                        }

                        else if (IsSummary)
                        {
                            transList = (from t in entity.Transactions
                                         join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                         join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                         where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                         && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "Cash"
                                         && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                         select t).ToList<Transaction>();


                            RtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Refund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();


                            DtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Settlement || t.Type == TransactionType.Prepaid) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();


                            CRtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.CreditRefund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();



                            GCtransList = (from t in entity.Transactions
                                           join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                           join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                           where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                           && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "Gift Card"
                                           && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                           select t).ToList<Transaction>();



                            CtransList = (from t in entity.Transactions
                                          join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                          join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                          where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                          && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Credit && p.Name == "Credit"
                                          && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                          select t).ToList<Transaction>();


                            MPUtransList = (from t in entity.Transactions
                                            join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                            join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                            where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                            && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 3
                                            && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                            select t).ToList<Transaction>();

                            GlobalCardList = (from t in entity.Transactions
                                              join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                              join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                              where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                              && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && (p.PaymentTypeId == 4 || p.PaymentTypeId == 9 || p.PaymentTypeId == 10)
                                              && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                              select t).ToList<Transaction>();

                            BankTransferList = (from t in entity.Transactions
                                                join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                                join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                                where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                                && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 5
                                                && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                                select t).ToList<Transaction>();

                            BankTransferList = (from t in entity.Transactions
                                                join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                                join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                                where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                                && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 6
                                                && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                                select t).ToList<Transaction>();


                            FOCtrnsList = (from t in entity.Transactions
                                           join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                           join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                           where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                           && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "FOC"
                                           && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                           select t).ToList<Transaction>();
                            //   TesterList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 6 && t.CounterId == CounterId && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            TesterList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 11 && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer1();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Transaction Report";
                            //lblTotalAmount.Text = "";
                        }
                    }
                    #endregion

                    #region get transaction with cashier only
                    else if (IsCashier == true && IsCounter == false)
                    {
                        if (IsSale)
                        {
                            //*Update SD*
                            transList = (from t in entity.Transactions
                                         join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                         join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                         where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true
                                             && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) &&
                                             (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                         orderby p.Id
                                         select t).ToList<Transaction>();

                            #region Old Code
                            //if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true
                            //                     && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && t.PaymentTypeId != 3 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && t.PaymentTypeId != 2 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && t.PaymentTypeId != 1 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && t.PaymentTypeId != 5 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && t.PaymentTypeId != 4 && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //// two type false
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 2) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 4) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 4) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 5 && t.PaymentTypeId != 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 3 && t.PaymentTypeId != 4) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 4 && t.PaymentTypeId != 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            ////three type false
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 3 || t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 2) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 2 || t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 2 || t.PaymentTypeId == 1) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 1) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 2 || t.PaymentTypeId == 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 1 || t.PaymentTypeId == 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 1 || t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            ////four type is false
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 1) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 2) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 4) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId == 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            ////all false

                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.UserId == CashierId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 1 && t.PaymentTypeId != 3 && t.PaymentTypeId != 4 && t.PaymentTypeId != 5) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            //}
                            #endregion

                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Sale Transaction Report for ";
                            // lblTotalAmount.Text = "";
                        }
                        else if (IsRefund)
                        {
                            transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) && t.UserId == CashierId && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Refund Transaction Report for ";
                            //lblTotalAmount.Text = "";
                        }
                        else if (IsDebt)
                        {
                            transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Prepaid || t.Type == TransactionType.Settlement) && t.UserId == CashierId && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Debt Transaction Report for ";
                            //lblTotalAmount.Text = "";
                        }
                        else if (IsSummary)
                        {
                            transList = (from t in entity.Transactions
                                         join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                         join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                         where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                         && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "Cash"
                                         && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                         select t).ToList<Transaction>();


                            RtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Refund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();


                            DtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Settlement || t.Type == TransactionType.Prepaid) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();


                            CRtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.CreditRefund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();



                            GCtransList = (from t in entity.Transactions
                                           join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                           join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                           where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                           && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "Gift Card"
                                           && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                           select t).ToList<Transaction>();



                            CtransList = (from t in entity.Transactions
                                          join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                          join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                          where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                          && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Credit && p.Name == "Credit"
                                          && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                          select t).ToList<Transaction>();


                            MPUtransList = (from t in entity.Transactions
                                            join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                            join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                            where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                            && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 3
                                            && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                            select t).ToList<Transaction>();

                            GlobalCardList = (from t in entity.Transactions
                                              join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                              join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                              where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                              && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && (p.PaymentTypeId == 4 || p.PaymentTypeId == 9 || p.PaymentTypeId == 10)
                                              && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                              select t).ToList<Transaction>();

                            BankTransferList = (from t in entity.Transactions
                                                join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                                join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                                where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                                && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 5
                                                && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                                select t).ToList<Transaction>();

                            PayList = (from t in entity.Transactions
                                       join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                       join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                       where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                       && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 6
                                       && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                       select t).ToList<Transaction>();


                            FOCtrnsList = (from t in entity.Transactions
                                           join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                           join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                           where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                           && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "FOC"
                                           && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                           select t).ToList<Transaction>();
                            //   TesterList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 6 && t.CounterId == CounterId && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            TesterList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 11 && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer1();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Transaction Report";
                            //lblTotalAmount.Text = "";
                        }
                    }
                    #endregion

                    #region get all transactions with counter only
                    else if (IsCashier == false && IsCounter == true)
                    {
                        if (IsSale)
                        {
                            #region Payment

                            //*Update SD*
                            transList = (from t in entity.Transactions
                                         where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true
                                             && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId
                                             && ((IsCash && t.PaymentTypeId == 1) || (IsCredit && t.PaymentTypeId == 2) || (IsGiftCard && t.PaymentTypeId == 8) || (IsFOC && t.PaymentTypeId == 12) || (IsMPU && t.PaymentTypeId == 3) || (IsTester && t.PaymentTypeId == 11)) &&
                                             (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                         orderby t.PaymentTypeId
                                         select t).ToList<Transaction>();


                            #region Old Code
                            //if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId select t).ToList<Transaction>();
                            //}
                            //// one payment type false 
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.PaymentTypeId != 3 select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.PaymentTypeId != 2 select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.PaymentTypeId != 1 select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.PaymentTypeId != 5 select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && t.PaymentTypeId != 4 select t).ToList<Transaction>();
                            //}
                            //// two type false
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 3) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 3) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 2) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 5) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 2 && t.PaymentTypeId != 4) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 5) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 4) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 5 && t.PaymentTypeId != 3) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 4 && t.PaymentTypeId != 3) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 4 && t.PaymentTypeId != 5) select t).ToList<Transaction>();
                            //}
                            ////three type false
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 5) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 3 || t.PaymentTypeId == 4) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 3 || t.PaymentTypeId == 5) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 2) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 2 || t.PaymentTypeId == 5) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 2 || t.PaymentTypeId == 1) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 4 || t.PaymentTypeId == 1) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 2 || t.PaymentTypeId == 3) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 1 || t.PaymentTypeId == 3) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 1 || t.PaymentTypeId == 5) select t).ToList<Transaction>();
                            //}
                            ////four type is false
                            //else if (IsCredit == false && IsCash == true && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 1) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == true && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 2) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == true && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 3) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == true)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 4) select t).ToList<Transaction>();
                            //}
                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == true && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId == 5) select t).ToList<Transaction>();
                            //}
                            ////all false

                            //else if (IsCredit == false && IsCash == false && IsGiftCard == false && IsMPU == false && IsFOC == false)
                            //{
                            //    transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && t.CounterId == CounterId && (t.PaymentTypeId != 1 && t.PaymentTypeId != 2 && t.PaymentTypeId != 3 && t.PaymentTypeId != 4 && t.PaymentTypeId != 5) select t).ToList<Transaction>();
                            //}
                            #endregion

                            #endregion
                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Sale Transaction Report for ";
                            // lblTotalAmount.Text = "";
                        }
                        else if (IsRefund)
                        {
                            transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) && t.CounterId == CounterId && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Refund Transaction Report for ";
                            //lblTotalAmount.Text = "";
                        }
                        else if (IsDebt)
                        {
                            transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Prepaid || t.Type == TransactionType.Settlement) && t.CounterId == CounterId && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Debt Transaction Report for ";
                            //lblTotalAmount.Text = "";
                        }
                        else if (IsSummary)
                        {
                            transList = (from t in entity.Transactions
                                         join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                         join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                         where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                         && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "Cash"
                                         && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                         select t).ToList<Transaction>();


                            RtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Refund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();


                            DtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Settlement || t.Type == TransactionType.Prepaid) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();


                            CRtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.CreditRefund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();



                            GCtransList = (from t in entity.Transactions
                                           join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                           join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                           where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                           && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "Gift Card"
                                           && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                           select t).ToList<Transaction>();



                            CtransList = (from t in entity.Transactions
                                          join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                          join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                          where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                          && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Credit && p.Name == "Credit"
                                          && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                          select t).ToList<Transaction>();


                            MPUtransList = (from t in entity.Transactions
                                            join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                            join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                            where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                            && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 3
                                            && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                            select t).ToList<Transaction>();

                            GlobalCardList = (from t in entity.Transactions
                                              join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                              join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                              where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                              && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && (p.PaymentTypeId == 4 || p.PaymentTypeId == 9 || p.PaymentTypeId == 10)
                                              && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                              select t).ToList<Transaction>();

                            BankTransferList = (from t in entity.Transactions
                                                join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                                join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                                where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                                && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 5
                                                && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                                select t).ToList<Transaction>();

                            PayList = (from t in entity.Transactions
                                       join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                       join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                       where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                       && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 6
                                       && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                       select t).ToList<Transaction>();


                            FOCtrnsList = (from t in entity.Transactions
                                           join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                           join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                           where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                           && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "FOC"
                                           && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                           select t).ToList<Transaction>();
                            //   TesterList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 6 && t.CounterId == CounterId && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            TesterList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 11 && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer1();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Transaction Report";
                            //lblTotalAmount.Text = "";
                        }
                    }
                    #endregion

                    #region get all transactions
                    else
                    {
                        if (IsSale)
                        {
                            //*Update SD*
                            transList = (from t in entity.Transactions
                                         join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                         join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                         where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true
                                             && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) &&
                                             (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                         orderby p.Id
                                         select t).ToList<Transaction>();

                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Sale Transaction Report for ";
                            // lblTotalAmount.Text = "";
                        }
                        else if (IsRefund)
                        {
                            transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Refund Transaction Report for ";
                            //lblTotalAmount.Text = "";
                        }
                        else if (IsDebt)
                        {
                            transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Prepaid || t.Type == TransactionType.Settlement) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Debt Transaction Report for ";
                            //lblTotalAmount.Text = "";
                        }
                        else if (IsSummary)
                        {
                            transList = (from t in entity.Transactions
                                         join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                         join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                         where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                         && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "Cash"
                                         && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                         select t).ToList<Transaction>();


                            RtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Refund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();


                            DtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Settlement || t.Type == TransactionType.Prepaid) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();


                            CRtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.CreditRefund && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();



                            GCtransList = (from t in entity.Transactions
                                           join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                           join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                           where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                           && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "Gift Card"
                                           && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                           select t).ToList<Transaction>();



                            CtransList = (from t in entity.Transactions
                                          join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                          join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                          where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                          && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Credit && p.Name == "Credit"
                                          && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                          select t).ToList<Transaction>();


                            MPUtransList = (from t in entity.Transactions
                                            join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                            join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                            where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                            && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 3
                                            && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                            select t).ToList<Transaction>();

                            GlobalCardList = (from t in entity.Transactions
                                              join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                              join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                              where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                              && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && (p.PaymentTypeId == 4 || p.PaymentTypeId == 9 || p.PaymentTypeId == 10)
                                              && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                              select t).ToList<Transaction>();

                            BankTransferList = (from t in entity.Transactions
                                                join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                                join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                                where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                                && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 5
                                                && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                                select t).ToList<Transaction>();

                            PayList = (from t in entity.Transactions
                                       join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                       join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                       where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                       && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.PaymentTypeId == 6
                                       && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                       select t).ToList<Transaction>();


                            FOCtrnsList = (from t in entity.Transactions
                                           join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                           join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                           where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate
                                           && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && p.Name == "FOC"
                                           && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode
                                           select t).ToList<Transaction>();
                            //   TesterList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 6 && t.CounterId == CounterId && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
                            TesterList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.PaymentTypeId == 6 && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
                            ShowReportViewer1();
                            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
                            // lblNumberofTransaction.Text = transList.Count.ToString();
                            gbTransactionList.Text = "Transaction Report";
                            //lblTotalAmount.Text = "";
                        }
                    }
                    #endregion
                }
            }
        }

        private void ShowReportViewer()
        {
            int shopid = Convert.ToInt32(cboshoplist.SelectedValue);
            string shopname = (from p in entity.Shops where p.Id == shopid select p.ShopName).FirstOrDefault();

            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReport = (dsReportTemp.TransactionListDataTable)dsReport.Tables["TransactionList"];
            foreach (Transaction transaction in transList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReport.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                if (transaction.Type == TransactionType.CreditRefund || transaction.Type == TransactionType.Refund)
                {
                    newRow.Amount = Convert.ToInt32(transaction.RecieveAmount);
                }
                else
                {
                    newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                }
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                //newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x =>  x.Qty));
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                dtTransactionReport.AddTransactionListRow(newRow);
            }

            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["TransactionList"]);
            string reportPath = Application.StartupPath + "\\Reports\\Transaction Report.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            ReportParameter TransactionTitle = new ReportParameter("TransactionTitle", gbTransactionList.Text + " for " + shopname);
            reportViewer1.LocalReport.SetParameters(TransactionTitle);

            ReportParameter Date = new ReportParameter("Date", " from " + dtpFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtpTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            reportViewer1.RefreshReport();
        }

        private void ShowReportViewer1()
        {
            POSEntities entity = new POSEntities();
            int shopid = Convert.ToInt32(cboshoplist.SelectedValue);
            string shopname = (from p in entity.Shops where p.Id == shopid select p.ShopName).FirstOrDefault();
            string currentshortcode = (from p in entity.Shops where p.Id == shopid select p.ShortCode).FirstOrDefault();
            int totalSale = 0, totalRefund = 0, totalDebt = 0, totalCreditRefund = 0, totalSummary = 0; int totalGiftCard = 0,
                totalCashFromGiftCard = 0, totalCredit = 0, totalCreditRecieve = 0, totalCashInHand = 0, totalExpense = 0, totalIncomeAmount = 0, totalMPU = 0, totalFOC = 0, totalReceived = 0;
            long totalDiscount = 0, totalRefundDiscount = 0, totalCreditRefundDiscount = 0; long totalTester = 0;
            long totalMCDiscount = 0;
            int totalOtherFOC = 0;
            int globalcard = 0, banktrasfer = 0, pay = 0;


            #region Transaction
            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReport = (dsReportTemp.TransactionListDataTable)dsReport.Tables["TransactionList"];

            foreach (Transaction transaction in transList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReport.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                //newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                totalSale += Convert.ToInt32(transaction.TotalAmount);

                totalOtherFOC += Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsFOC == true).Sum((x => x.SellingPrice * x.Qty)));
                dtTransactionReport.AddTransactionListRow(newRow);
            }
            ReportDataSource rds1 = new ReportDataSource("SaleDataSet", dsReport.Tables["TransactionList"]);
            #endregion

            #region Get Discount Value

            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date;
            int CashierId = 0;
            int CounterId = 0;

            List<Transaction> discounttransList = new List<Transaction>();

            //If user use filter for both Counter and Casher
            if (cboCounter.SelectedIndex > 0 && cboCashier.SelectedIndex > 0)
            {
                CounterId = Convert.ToInt32(cboCounter.SelectedValue);
                CashierId = Convert.ToInt32(cboCashier.SelectedValue);

                discounttransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.CounterId == CounterId && t.UserId == CashierId && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
            }
            // User just use Counter filter
            else if (cboCounter.SelectedIndex > 0)
            {
                CounterId = Convert.ToInt32(cboCounter.SelectedValue);
                discounttransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.CounterId == CounterId && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
            }
            // User just use Casher filter
            else if (cboCashier.SelectedIndex > 0)
            {
                CashierId = Convert.ToInt32(cboCashier.SelectedValue);
                discounttransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.UserId == CashierId && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
            }
            // User ignore both filter
            else
            {
                discounttransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && (t.IsDeleted == null || t.IsDeleted == false) && t.Id.Substring(2, 2) == currentshortcode select t).ToList<Transaction>();
            }

            foreach (Transaction t in discounttransList)
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

            #endregion

            #region Refund
            dsReportTemp dsReportRefund = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportRefund = (dsReportTemp.TransactionListDataTable)dsReportRefund.Tables["TransactionList"];
            foreach (Transaction transaction in RtransList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportRefund.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.RecieveAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                totalRefund += Convert.ToInt32(transaction.RecieveAmount);
                dtTransactionReportRefund.AddTransactionListRow(newRow);
            }
            ReportDataSource rds2 = new ReportDataSource("RefundDataSet", dsReportRefund.Tables["TransactionList"]);
            totalRefundDiscount = RtransList.Sum(x => x.DiscountAmount).Value;
            #endregion

            #region Debt
            dsReportTemp dsReportDebt = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportDebt = (dsReportTemp.TransactionListDataTable)dsReportDebt.Tables["TransactionList"];
            foreach (Transaction transaction in DtransList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportDebt.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                newRow.Type = transaction.Type;
                totalDebt += Convert.ToInt32(transaction.TotalAmount);
                dtTransactionReportDebt.AddTransactionListRow(newRow);
            }
            ReportDataSource rds3 = new ReportDataSource("DebtDataSet", dsReportDebt.Tables["TransactionList"]);
            #endregion

            #region CreditRefund
            dsReportTemp dsReportCreditRefund = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportCreditRefund = (dsReportTemp.TransactionListDataTable)dsReportCreditRefund.Tables["TransactionList"];
            foreach (Transaction transaction in CRtransList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportCreditRefund.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.RecieveAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                //totalOtherFOC += Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsFOC == true).Sum((x => x.SellingPrice * x.Qty)));
                totalCreditRefund += Convert.ToInt32(transaction.RecieveAmount);
                dtTransactionReportCreditRefund.AddTransactionListRow(newRow);
            }
            ReportDataSource rds4 = new ReportDataSource("CreditRefundDataSet", dsReportCreditRefund.Tables["TransactionList"]);
            totalCreditRefundDiscount = CRtransList.Sum(x => x.DiscountAmount).Value;
            #endregion

            #region GiftCard
            dsReportTemp dsReportGiftCardTransaction = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportGiftCard = (dsReportTemp.TransactionListDataTable)dsReportGiftCardTransaction.Tables["TransactionList"];
            foreach (Transaction transaction in GCtransList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportGiftCard.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                //  newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                totalOtherFOC += Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsFOC == true).Sum((x => x.SellingPrice * x.Qty)));
                //totalGiftCard += Convert.ToInt32(transaction.TotalAmount);
                totalGiftCard += Convert.ToInt32(transaction.GiftCardAmount);
                totalCashFromGiftCard += (Convert.ToInt32(transaction.TotalAmount) - Convert.ToInt32(transaction.GiftCardAmount));
                dtTransactionReportGiftCard.AddTransactionListRow(newRow);
            }
            ReportDataSource rds5 = new ReportDataSource("GiftCardDataSet", dsReportGiftCardTransaction.Tables["TransactionList"]);
            #endregion

            #region Credit
            dsReportTemp dsReportCreditTransaction = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportCredit = (dsReportTemp.TransactionListDataTable)dsReportCreditTransaction.Tables["TransactionList"];
            foreach (Transaction transaction in CtransList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportCredit.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                //  newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                totalOtherFOC += Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsFOC == true).Sum((x => x.SellingPrice * x.Qty)));
                totalCredit += Convert.ToInt32(transaction.TotalAmount);
                totalCreditRecieve += Convert.ToInt32(transaction.RecieveAmount);
                dtTransactionReportCredit.AddTransactionListRow(newRow);
            }
            ReportDataSource rds6 = new ReportDataSource("CreditDataSet", dsReportCreditTransaction.Tables["TransactionList"]);
            #endregion

            #region MPU

            dsReportTemp dsReportMPUTransaction = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportMPU = (dsReportTemp.TransactionListDataTable)dsReportMPUTransaction.Tables["TransactionList"];
            foreach (Transaction transaction in MPUtransList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportMPU.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                //newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                totalOtherFOC += Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsFOC == true).Sum((x => x.SellingPrice * x.Qty)));
                totalMPU += Convert.ToInt32(transaction.TotalAmount);
                dtTransactionReportMPU.AddTransactionListRow(newRow);
            }
            ReportDataSource rds7 = new ReportDataSource("MPUDataSet", dsReportMPUTransaction.Tables["TransactionList"]);
            #endregion

            #region Global Card
            dsReportTemp dsTransactionReportGlobal = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportGlobal = (dsReportTemp.TransactionListDataTable)dsTransactionReportGlobal.Tables["TransactionList"];
            foreach (Transaction transaction in GlobalCardList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportGlobal.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                //newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                totalOtherFOC += Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsFOC == true).Sum((x => x.SellingPrice * x.Qty)));
                globalcard += Convert.ToInt32(transaction.TotalAmount);
                dtTransactionReportGlobal.AddTransactionListRow(newRow);
            }
            ReportDataSource rds10 = new ReportDataSource("GlobalCardDataSet", dsTransactionReportGlobal.Tables["TransactionList"]);
            #endregion

            #region BankTransafer
            dsReportTemp dsTransactionReportBank = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportBank = (dsReportTemp.TransactionListDataTable)dsTransactionReportBank.Tables["TransactionList"];
            foreach (Transaction transaction in BankTransferList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportBank.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                //newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                totalOtherFOC += Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsFOC == true).Sum((x => x.SellingPrice * x.Qty)));
                banktrasfer += Convert.ToInt32(transaction.TotalAmount);
                dtTransactionReportBank.AddTransactionListRow(newRow);
            }
            ReportDataSource rds11 = new ReportDataSource("BankTransferDataSet", dsTransactionReportBank.Tables["TransactionList"]);
            #endregion

            #region Pay
            dsReportTemp dsTransactionReportPay = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportPay = (dsReportTemp.TransactionListDataTable)dsTransactionReportPay.Tables["TransactionList"];
            foreach (Transaction transaction in PayList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportPay.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                //newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                totalOtherFOC += Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsFOC == true).Sum((x => x.SellingPrice * x.Qty)));
                pay += Convert.ToInt32(transaction.TotalAmount);
                dtTransactionReportPay.AddTransactionListRow(newRow);
            }
            ReportDataSource rds12 = new ReportDataSource("PayDataSet", dsTransactionReportPay.Tables["TransactionList"]);
            #endregion

            #region FOC
            dsReportTemp dsReportFOCTransaction = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportFOC = (dsReportTemp.TransactionListDataTable)dsReportFOCTransaction.Tables["TransactionList"];
            foreach (Transaction transaction in FOCtrnsList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportFOC.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                //newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.Amount = Convert.ToInt32(transaction.TransactionDetails.Sum((x => x.SellingPrice * x.Qty)));
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                //  newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Sum(x => x.Qty));
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                //totalFOC += Convert.ToInt32(transaction.TotalAmount);
                totalFOC += Convert.ToInt32(transaction.TransactionDetails.Sum((x => x.SellingPrice * x.Qty)));
                dtTransactionReportFOC.AddTransactionListRow(newRow);
            }
            ReportDataSource rds8 = new ReportDataSource("FOCDataSet", dsReportFOCTransaction.Tables["TransactionList"]);
            #endregion

            #region Tester
            dsReportTemp dsReportTesterTransaction = new dsReportTemp();
            dsReportTemp.TransactionListDataTable dtTransactionReportTester = (dsReportTemp.TransactionListDataTable)dsReportTesterTransaction.Tables["TransactionList"];
            foreach (Transaction transaction in TesterList)
            {
                dsReportTemp.TransactionListRow newRow = dtTransactionReportTester.NewTransactionListRow();
                newRow.TransactionId = transaction.Id;
                newRow.Date = Convert.ToDateTime(transaction.DateTime);
                newRow.SalePerson = transaction.User.Name;
                newRow.PaymentMethod = transaction.PaymentType.Name;
                newRow.Amount = Convert.ToInt32(transaction.TotalAmount);
                newRow.DiscountAmount = transaction.DiscountAmount.ToString();
                newRow.Type = transaction.Type;
                newRow.Qty = Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsDeleted == false).Sum(x => x.Qty));
                totalOtherFOC += Convert.ToInt32(transaction.TransactionDetails.Where(x => x.IsFOC == true).Sum((x => x.SellingPrice * x.Qty)));
                //if (transaction.ReceivedCurrencyId == 1 || transaction.ReceivedCurrencyId == null)
                //{
                //    newRow.Currency = "Ks";
                //    KsTotal += Convert.ToInt64(transaction.TotalAmount);

                //}
                //else
                //{
                //    newRow.Currency = "$";
                //    ExchangeRateForTransaction e = entity.ExchangeRateForTransactions.Where(x => x.TransactionId == transaction.Id).FirstOrDefault();
                //    dollartotal += Convert.ToDecimal(transaction.TotalAmount / e.ExchangeRate);
                //    newRow.Amount = newRow.Amount / e.ExchangeRate;
                //}
                //newRow.Currency = "Ks";//recheck
                //totalGiftCard += Convert.ToInt32(transaction.TotalAmount);
                totalTester += Convert.ToInt64(transaction.TotalAmount);
                dtTransactionReportTester.AddTransactionListRow(newRow);
            }
            ReportDataSource rds9 = new ReportDataSource("TesterDataSet", dsReportTesterTransaction.Tables["TransactionList"]);
            #endregion

            var cashList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && (t.PaymentTypeId == 1) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
            cashList.AddRange((from t in entity.TransactionPaymentDetails
                               join td in entity.Transactions on t.TransactionId equals td.Id
                               join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                               where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                               && td.IsDeleted == false && p.Name == "Cash"
                               select td).ToList<Transaction>());

            totalSale = (int)cashList.Select(x => x.TotalAmount).ToList().Distinct().Sum();

            totalCredit = (int)(from t in entity.TransactionPaymentDetails
                                join td in entity.Transactions on t.TransactionId equals td.Id
                                join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                && td.IsDeleted == false && p.Name == "Credit"
                                select t.Amount).ToList().Sum();

            totalGiftCard = (int)(from t in entity.TransactionPaymentDetails
                                  join td in entity.Transactions on t.TransactionId equals td.Id
                                  join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                  where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                  && td.IsDeleted == false && p.Name == "Gift Card"
                                  select t.Amount).ToList().Sum();

            totalFOC = (int)(from t in entity.TransactionPaymentDetails
                             join td in entity.Transactions on t.TransactionId equals td.Id
                             join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                             join trd in entity.TransactionDetails on td.Id equals trd.TransactionId
                             where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                             && td.IsDeleted == false && (p.Name == "FOC")
                             select td.TotalAmount).ToList().Distinct().Sum();
            var mpuList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && (t.PaymentTypeId == 3) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
            mpuList.AddRange((from t in entity.TransactionPaymentDetails
                              join td in entity.Transactions on t.TransactionId equals td.Id
                              join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                              where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                              && td.IsDeleted == false & p.PaymentTypeId == 3
                              select td).ToList<Transaction>());

            totalMPU = (int)mpuList.Select(x => x.TotalAmount).ToList().Distinct().Sum();

            globalcard = (int)(from t in entity.TransactionPaymentDetails
                               join td in entity.Transactions on t.TransactionId equals td.Id
                               join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                               where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                               && td.IsDeleted == false & (p.PaymentTypeId == 4 || p.PaymentTypeId == 9 || p.PaymentTypeId == 10)
                               select t.Amount).ToList().Sum();

            banktrasfer = (int)(from t in entity.TransactionPaymentDetails
                                join td in entity.Transactions on t.TransactionId equals td.Id
                                join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                && td.IsDeleted == false & p.PaymentTypeId == 5
                                select t.Amount).ToList().Sum();

            pay = (int)(from t in entity.TransactionPaymentDetails
                        join td in entity.Transactions on t.TransactionId equals td.Id
                        join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                        where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                        && td.IsDeleted == false & p.PaymentTypeId == 6
                        select t.Amount).ToList().Sum();

            //totalSummary = (totalSale + totalDebt + totalCreditRefund + totalGiftCard) - totalRefund;
            totalSummary = ((totalSale + totalCredit + totalGiftCard + totalCashFromGiftCard + totalMPU) - (totalRefund + totalCreditRefund + totalFOC));
            totalCashInHand = (totalSale + totalCashFromGiftCard + totalDebt + totalCreditRecieve) - totalRefund;
            totalExpense = (totalRefund + totalCreditRefund + totalFOC);
            totalIncomeAmount = (totalSale + totalCredit + totalGiftCard + totalCashFromGiftCard + totalMPU + globalcard + banktrasfer + pay);
            totalReceived = (totalSale + totalCashFromGiftCard + totalDebt + totalCreditRecieve);
            string reportPath = Application.StartupPath + "\\Reports\\Transactions Detail Report.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds1);
            reportViewer1.LocalReport.DataSources.Add(rds2);
            reportViewer1.LocalReport.DataSources.Add(rds3);
            reportViewer1.LocalReport.DataSources.Add(rds4);
            reportViewer1.LocalReport.DataSources.Add(rds5);
            reportViewer1.LocalReport.DataSources.Add(rds6);
            reportViewer1.LocalReport.DataSources.Add(rds7);
            reportViewer1.LocalReport.DataSources.Add(rds8);
            reportViewer1.LocalReport.DataSources.Add(rds9);
            reportViewer1.LocalReport.DataSources.Add(rds10);
            reportViewer1.LocalReport.DataSources.Add(rds11);
            reportViewer1.LocalReport.DataSources.Add(rds12);



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

            ReportParameter TotalOtherFOC = new ReportParameter("TotalOtherFOC", totalOtherFOC.ToString());
            reportViewer1.LocalReport.SetParameters(TotalOtherFOC);

            ReportParameter TotalMPU = new ReportParameter("TotalMPU", totalMPU.ToString());
            reportViewer1.LocalReport.SetParameters(TotalMPU);

            ReportParameter TotalGlobalCard = new ReportParameter("TotalGlobalCard", globalcard.ToString());
            reportViewer1.LocalReport.SetParameters(TotalGlobalCard);

            ReportParameter TotalBankTransfer = new ReportParameter("TotalBankTransfer", banktrasfer.ToString());
            reportViewer1.LocalReport.SetParameters(TotalBankTransfer);

            ReportParameter TotalPay = new ReportParameter("TotalPay", pay.ToString());
            reportViewer1.LocalReport.SetParameters(TotalPay);

            ReportParameter TotalSale = new ReportParameter("TotalSale", (totalSale + totalCashFromGiftCard).ToString());
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

            ReportParameter HeaderTitle = new ReportParameter("HeaderTitle", "Transaction Summary for " + shopname);
            reportViewer1.LocalReport.SetParameters(HeaderTitle);

            ReportParameter Date = new ReportParameter("Date", " from " + dtpFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtpTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            ReportParameter TesterTotal = new ReportParameter("TesterTotal", totalTester.ToString());
            reportViewer1.LocalReport.SetParameters(TesterTotal);


            reportViewer1.RefreshReport();
        }





        #endregion

       
    }
}
