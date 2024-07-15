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
using POS.APP_Data;

namespace POS
{
    public partial class CreditTransactionList : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();

        int Qty = 0;

        List<Stock_Transaction> productList = new List<Stock_Transaction>();
       public int index = 0;
       Boolean Isstart = false;


        #endregion

        #region Event

        public CreditTransactionList()
        {
            InitializeComponent();
        }

        private void CreditTransactionList_Load(object sender, EventArgs e)
        {
            dgvTransactionList.AutoGenerateColumns = false;
            Utility.BindShop(cboshoplist);
            cboshoplist.Text = SettingController.DefaultShop.ShopName;
            Utility.ShopComBo_EnableOrNot(cboshoplist);
            Isstart = true;
            LoadData();
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dgvTransactionList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                List<string> type = new List<string>();
                type.Add(TransactionType.Settlement);
                type.Add(TransactionType.Prepaid);
                string currentTransactionId = dgvTransactionList.Rows[e.RowIndex].Cells[ColTransactionId.Index].Value.ToString();
                List<APP_Data.TransactionDetail> _isConsignmentPaidTranList = entity.TransactionDetails.Where(x => x.TransactionId == currentTransactionId && x.IsDeleted == false && x.IsConsignmentPaid == true).ToList();
                var isexp = dgvTransactionList.Rows[e.RowIndex].Cells[dgvTransactionList.Columns.Count - 1].Value.ToString();

                //Refund
                if (e.ColumnIndex == colRefund.Index)
                {
                    if (type.Contains(dgvTransactionList.Rows[e.RowIndex].Cells[colType.Index].Value.ToString()))
                    {
                        colRefund.ReadOnly = true;
                    }
                    else
                    {
                        if (_isConsignmentPaidTranList.Count > 0)
                        {
                            MessageBox.Show("This transaction already paid  Consignment. It cannot be Refundable!");
                        }
                        else
                        {
                            var ts = (from t in entity.Transactions where t.Id == currentTransactionId select t).FirstOrDefault();

                            string date = dgvTransactionList.Rows[e.RowIndex].Cells[2].Value.ToString();
                            DateTime _Trandate = Utility.Convert_Date(date);

                            Update_Settlement(ts, _Trandate, true, true);
                        }
                    }
                }

                //View Detail
                else if (e.ColumnIndex == 8)
                {
                    if (type.Contains(dgvTransactionList.Rows[e.RowIndex].Cells[1].Value.ToString()))
                    {
                        //colViewDetail.ReadOnly = true;
                        switch (dgvTransactionList.Rows[e.RowIndex].Cells[1].Value.ToString())
                        {
                            case "Settlement":
                                var result = (from t in entity.Transactions where t.Id == currentTransactionId select t.TranVouNos).FirstOrDefault();
                                MessageBox.Show("Settlement for Vouncher No. " + result, "mPOS");
                                break;

                            case "Prepaid":
                                colViewDetail.ReadOnly = true;
                                break;

                        }

                    }
                    else
                    {
                        
                        bool bexp = false;
                        bool.TryParse(isexp, out bexp);
                        
                        TransactionDetailForm newForm = new TransactionDetailForm(bexp);
                        newForm.shopid = Convert.ToInt32(cboshoplist.SelectedValue);
                        newForm.transactionId = currentTransactionId;
                        newForm.IsCash = false;
                        newForm.Show();
                    }
                }
                //Delete
                else if (e.ColumnIndex == 9)
                {
                    if (bool.Parse(isexp.ToString()))
                    {
                        MessageBox.Show("You can't delete SAP exported transaction!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    if (type.Contains(dgvTransactionList.Rows[e.RowIndex].Cells[1].Value.ToString()))
                    {
                        colDelete.ReadOnly = true;
                    }
                    else
                    {
                        Transaction ts = entity.Transactions.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                        if (ts.Transaction1.Count > 0)
                        {
                            MessageBox.Show("This transaction already make refund. So it can't be delete!");
                        }
                        else if (_isConsignmentPaidTranList.Count > 0)
                        {
                            MessageBox.Show("This transaction already paid  Consignment. It cannot be deleted!");
                        }
                        else
                        {

                            DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (result.Equals(DialogResult.OK))
                            {
                                ELC_CustomerPointSystem.Delete_ReferralPointInTransaction(currentTransactionId);
                                string date = dgvTransactionList.Rows[e.RowIndex].Cells[2].Value.ToString();
                                DateTime _Trandate = Utility.Convert_Date(date);
                                if (Update_Settlement(ts, _Trandate, true))
                                {
                                    Transaction_Delete(ts, _Trandate);
                                }
                            }
                        }
                    }

                }
                else if (e.ColumnIndex == 10)
                {
                    if (bool.Parse(isexp.ToString()))
                    {
                        MessageBox.Show("You can't make delete SAP exported transaction!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    if (type.Contains(dgvTransactionList.Rows[e.RowIndex].Cells[1].Value.ToString()))
                    {
                        colDeleteAndCopy.ReadOnly = true;
                    }
                    else
                    {
                        Transaction ts = entity.Transactions.Where(x => x.Id == currentTransactionId).FirstOrDefault();

                        List<Transaction> rlist = new List<Transaction>();

                        if (ts.Transaction1.Count > 0)
                        {
                            rlist = ts.Transaction1.Where(x => x.IsDeleted == false).ToList();
                        }
                        if (rlist.Count > 0)
                        {
                            MessageBox.Show("This transaction already make refund. So it can't be delete!");
                        }
                        else if (_isConsignmentPaidTranList.Count > 0)
                        {
                            MessageBox.Show("This transaction already paid  Consignment. It cannot be deleted!");
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (result.Equals(DialogResult.OK))
                            {
                                ELC_CustomerPointSystem.Delete_ReferralPointInTransaction(currentTransactionId);
                                if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                                {
                                    Sales openedForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                                    Boolean IsFormClosing = openedForm.DeleteCopy(currentTransactionId);
                                    if (IsFormClosing)
                                    {
                                        this.Dispose();
                                    }
                                }
                            }
                        }
                    }

                }
                //refund
                else if (e.ColumnIndex == 11)
                {
                    bool bexp = false;
                    bool.TryParse(isexp, out bexp);
                    RefundTransaction refundForm = new RefundTransaction(bexp);
                    refundForm.ShowDialog();
                }
            }
        }

        private void dgvTransactionList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvTransactionList.Rows)
            {
                Transaction currentt = (Transaction)row.DataBoundItem;
                row.Cells[0].Value = currentt.Id;
                row.Cells[1].Value = currentt.Type;
                row.Cells[2].Value = currentt.DateTime.Value.ToString("dd-MM-yyyy");
                row.Cells[3].Value = currentt.DateTime.Value.ToString("hh:mm");
                row.Cells[4].Value = currentt.User.Name;
                row.Cells[5].Value = (currentt.Customer == null) ? "-" : currentt.Customer.Name;
                if (currentt.Type == "Settlement")
                {
                    row.Cells[6].Value = currentt.TotalAmount;
                    // row.DefaultCellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#c6dff9");
                    row.DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    row.Cells[7].Value = 0;

                }
                else
                {
                    List<string> type = new List<string>();
                    type.Add(TransactionType.CreditRefund);
                    type.Add(TransactionType.Refund);
                    var refundList = (from t in entity.Transactions where type.Contains(t.Type) && t.ParentId == currentt.Id && t.IsDeleted == false select t).ToList();
                    int refundAmt = Convert.ToInt32(refundList.Sum(x => x.TotalAmount));
                
                    var usePrepaidAmt = 0;
                    usePrepaidAmt = Convert.ToInt32(entity.UsePrePaidDebts.AsEnumerable().Where(x => x.CreditTransactionId == currentt.Id).Select(x => x.UseAmount).Sum());


                    var DiscountAmt = Convert.ToInt32(refundList.Sum(x => x.DiscountAmount));
                    int currentRefundAmt = refundAmt - DiscountAmt;

                    int receiveAmt = Convert.ToInt32(currentt.RecieveAmount);
                 
                        int _inAmt = (receiveAmt + usePrepaidAmt);

                        row.Cells[6].Value = currentt.TotalAmount - _inAmt - currentRefundAmt;
                  
                    row.Cells[7].Value = currentRefundAmt;
                }

                if (BOOrPOS.IsBackOffice == true)
                {
                    if (dgvTransactionList.Columns[10].Visible != false)
                    {
                        dgvTransactionList.Columns[10].Visible = false;
                    }
                }
                else
                {
                    if (dgvTransactionList.Columns[10].Visible != false)
                    {
                        dgvTransactionList.Columns[10].Visible = true;
                    }
                }
            }
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDate.Checked)
            {
                gbDate.Enabled = true;
                gbId.Enabled = false;
                txtId.Clear();
            }
            else
            {
                gbDate.Enabled = false;
                gbId.Enabled = true;
            }
            LoadData();
        }

        private void cboshoplist_selectedindexchanged(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Function
        public Boolean Update_Settlement(Transaction ts, DateTime _date, Boolean IsSettlement = false, Boolean IsRefund = false)
        {
            Boolean IsContinue = false;
            if (ts.IsSettlement == true)
            {
                string Id = ts.Id;
                string settlementVouNo = ts.TranVouNos;
                string text = "";
                if (IsRefund == true)
                {
                    text = "Refund";
                }
                else
                {
                    text = "Delete";
                }

                //if (Convert.ToInt32(dgvTransactionList.Rows[index].Cells[6].Value) > Convert.ToInt32(dgvTransactionList.Rows[index].Cells[7].Value))
                //{
                if (IsRefund)
                {
                    if (Convert.ToInt32(dgvTransactionList.Rows[index].Cells[6].Value) < Convert.ToInt32(dgvTransactionList.Rows[index].Cells[7].Value))
                    {
                        var isexp = dgvTransactionList.Rows[index].Cells[dgvTransactionList.Columns.Count - 1].Value.ToString();

                        bool bexp = false;
                        bool.TryParse(isexp, out bexp);
                        RefundTransaction newForm = new RefundTransaction(bexp);
                        newForm.transactionId = ts.Id;
                        newForm.Show();
                        return IsContinue;
                    }
                       
                }
                    DialogResult result1 = MessageBox.Show("'" + Id + "' is already made settlement with Vouncher No. '"
                  + settlementVouNo + "' ! Are you sure want to " + text + " TransactionId '" + Id + "' ?", "mPOS", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result1.Equals(DialogResult.OK))
                    {
                        if (IsRefund)
                        {
                        var isexp = dgvTransactionList.Rows[index].Cells[dgvTransactionList.Columns.Count - 1].Value.ToString();

                        bool bexp = false;
                        bool.TryParse(isexp, out bexp);
                        RefundTransaction newForm = new RefundTransaction(bexp);
                        newForm.transactionId = ts.Id;
                                newForm.Show();
                         
                        }
                        else
                        {
                            var settlementResult = (from t in entity.Transactions where t.Id == settlementVouNo select t).FirstOrDefault();
                            settlementResult.TotalAmount = settlementResult.TotalAmount - ts.TotalAmount;

                            string _tranVouNos = settlementResult.TranVouNos;
                            string[] _tranWord = _tranVouNos.Split(',');

                            if (_tranWord.Length > 1)
                            {
                                var _tranList = (from t in _tranWord where t != Id select t).ToList();

                                string joinedTranIdList = string.Join(",", _tranList);

                                settlementResult.TranVouNos = joinedTranIdList;
                                entity.Entry(settlementResult).State = EntityState.Modified;
                                entity.SaveChanges();
                            }
                            else
                            {
                                settlementResult.IsDeleted = true;
                                entity.Entry(settlementResult).State = EntityState.Modified;
                                entity.SaveChanges();
                            }


                            if (IsSettlement)
                            {
                                Transaction_Delete(ts, _date);
                            }
                        }
                        IsContinue = true;
                    }
                //}
                //else
                //{
                //    MessageBox.Show( "Invoice No." + ts.Id + "  is already refunds all items.","mPOS");
                //}
              

            }
            else
            {
                if (!IsRefund)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        //if (IsSettlement)
                        //{
                        //Transaction_Delete(ts, _date);
                        return IsContinue=true;
                        //    IsContinue = true;
                        //}

                    }

                }
                else
                {
                    var isexp = dgvTransactionList.Rows[index].Cells[dgvTransactionList.Columns.Count - 1].Value.ToString();

                    bool bexp = false;
                    bool.TryParse(isexp, out bexp);
                    RefundTransaction newForm = new RefundTransaction(bexp);
                    newForm.transactionId = ts.Id;
                    newForm.Show();
                }

            }
            return IsContinue;
        }

        private void Transaction_Delete(Transaction ts, DateTime _date)
        {
            ts.IsDeleted = true;
            Utility.GiftCardIsBack(ts);
            foreach (TransactionDetail detail in ts.TransactionDetails.Where(x => x.IsDeleted == false))
            {
                detail.IsDeleted = true;
                detail.Product.Qty = detail.Product.Qty + detail.Qty;


                var puInTranDetail = entity.PurchaseDetailInTransactions.Where(x => x.TransactionDetailId == detail.Id).FirstOrDefault();

                if (puInTranDetail != null)
                {
                    entity.PurchaseDetailInTransactions.Remove(puInTranDetail);
                    entity.SaveChanges();
                }

                #region add qty in stock filling from sap KHS
                Utility.AddProductAvailableQty(entity, (long)detail.ProductId, detail.BatchNo, (int)detail.Qty);

                #endregion
                // update Prepaid Transaction id = false   and delete list in useprepaiddebt table
                Utility.Plus_PreaidAmt(ts);

                //save in stocktransaction

                Stock_Transaction st = new Stock_Transaction();
                st.ProductId = detail.Product.Id;
                Qty -= Convert.ToInt32(detail.Qty);
                st.Sale = Qty;
                productList.Add(st);
                Qty = 0;
            }
            //save in stock transaction
           
            Save_SaleQty_ToStockTransaction(productList, _date);
            productList.Clear();
            DeleteLog dl = new DeleteLog();
            dl.DeletedDate = DateTime.Now;
            dl.CounterId = MemberShip.CounterId;
            dl.UserId = MemberShip.UserId;
            dl.IsParent = true;
            dl.TransactionId = ts.Id;

            entity.DeleteLogs.Add(dl);

            entity.SaveChanges();
            var result=entity.Transactions.Where(x=>x.Id == "WS000003").Select(x=>x.IsDeleted).FirstOrDefault();

            // By SYM // For MemberType of Customer
            #region Member Type
            string minimumAmountofThisMemberType = (from mCardRule in entity.MemberCardRules.AsEnumerable() where mCardRule.IsActive == true orderby int.Parse(mCardRule.RangeFrom) select mCardRule.RangeFrom).FirstOrDefault();
            int memberValidityYear = Convert.ToInt32(SettingController.DefaultMemberValidityYear.ToString());
            int customerID = Convert.ToInt32(ts.CustomerId); // customerid, transactionid
            Customer currentCustomer = (from c in entity.Customers where c.Id == customerID select c).FirstOrDefault<Customer>();
            List<MemberCardRule> memberCardRuleList = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true orderby int.Parse(m.RangeFrom) select m).ToList();
            int totalAmount = 0;
            int giftCardTotalAmount = 0;
            int totalRAmount = 0;
            int memberTypeId = 0;

            if (currentCustomer.Name != "Default")
            {
                if (currentCustomer.MemberTypeID != null)
                {
                    if (ts.DateTime.Value.Date >= currentCustomer.StartDate && ts.DateTime.Value.Date <= currentCustomer.ExpireDate)
                    { // not expired

                        List<Transaction> transactionList = (from transactions in entity.Transactions
                                                             join c in entity.Customers
                                                             on transactions.CustomerId equals c.Id
                                                             where (transactions.CustomerId == customerID) && (c.Id == customerID) && (transactions.IsDeleted == false)
                                                             && (transactions.IsComplete == true) && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                             && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
                                                             && (transactions.Type != "Refund") && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
                                                             select transactions).ToList();

                        List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                               join transactions in entity.Transactions
                                                                               on td.TransactionId equals transactions.ParentId
                                                                               join c in entity.Customers
                                                                               on transactions.CustomerId equals c.Id
                                                                               where (transactions.CustomerId == customerID) && (c.Id == customerID) && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
                                                                               && (transactions.IsDeleted == false) && (transactions.IsComplete == true) && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                               && (transactions.Type != "Refund") && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
                                                                               select td).ToList();

                        foreach (Transaction t in transactionList)
                        {
                            totalAmount += Convert.ToInt32(t.TotalAmount);
                        }
                        foreach (Transaction t in transactionList)
                        {
                            giftCardTotalAmount += Convert.ToInt32(t.GiftCardAmount);
                        }
                        totalAmount = totalAmount - giftCardTotalAmount;
                        foreach (TransactionDetail td in RefundTransactionDetailList)
                        {
                            totalRAmount += Convert.ToInt32(td.TotalAmount);
                        }
                        totalAmount = totalAmount - totalRAmount;
                        foreach (MemberCardRule memberCard in memberCardRuleList)
                        {
                            if (Convert.ToInt32(memberCard.RangeFrom) <= totalAmount)
                            {
                                memberTypeId = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();
                            }
                        }

                        currentCustomer.MemberTypeID = memberTypeId;
                        if (memberTypeId == 0)
                        {
                            currentCustomer.MemberTypeID = null;
                            currentCustomer.StartDate = null;
                            currentCustomer.FirstTime = null;
                            currentCustomer.ExpireDate = null;
                            currentCustomer.PromoteDate = null;
                        }
                        entity.SaveChanges();

                    }

                }
            }

            #endregion

            LoadData();
        }

        #region for saving Sale Qty in Stock Transaction table
        private void Save_SaleQty_ToStockTransaction(List<Stock_Transaction> productList, DateTime _tranDate)
        {
            int _year, _month;

            _year = _tranDate.Year;
            _month = _tranDate.Month;
            Utility.Sale_Run_Process(_year, _month, productList);
        }
        #endregion

   
        public void LoadData()
        {
            if (Isstart == true)
            {
                entity = new POSEntities();
                dgvTransactionList_CustomCellFormatting();
                dgvTransactionList.DataSource = "";

                int shopid = Convert.ToInt32(cboshoplist.SelectedValue);

                string currentshortcode = (from p in entity.Shops where p.Id == shopid select p.ShortCode).FirstOrDefault();


               bool optionvisible=Utility.TransactionDelRefHide(shopid);
             //  dgvTransactionList.Columns[8].Visible = optionvisible;
               dgvTransactionList.Columns[9].Visible = optionvisible;
               dgvTransactionList.Columns[10].Visible = optionvisible;
                List<string> type = new List<string>();
                type.Add(TransactionType.Credit);
                type.Add(TransactionType.Settlement);

                if (rdbDate.Checked)
                {
                    DateTime fromDate = dtpFrom.Value.Date;
                    DateTime toDate = dtpTo.Value.Date;



                    List<Transaction> transList = (from t in entity.Transactions
                                                   where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate
                                                       && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true
                                                       && t.IsActive == true && type.Contains(t.Type) && t.Id.Substring(2, 2) == currentshortcode
                                                   select t).ToList<Transaction>();

                
                             dgvTransactionList.DataSource = transList.Where(x => x.IsDeleted != true).ToList();
                  
          
                    
                 //   dgvTransactionList.AutoGenerateColumns = false;
                //    dgvTransactionList.DataSource = transList.Where(x => x.IsDeleted != true).ToList();
                }
                else
                {
                    string Id = txtId.Text;

                    if (Id.Trim() != string.Empty)
                    {
                        List<Transaction> transList = (from t in entity.Transactions where t.Id == Id && type.Contains(t.Type) && t.Id.Substring(2, 2) == currentshortcode select t).ToList().Where(x => x.IsDeleted != true).ToList();
                        if (transList.Count > 0)
                        {
                            dgvTransactionList.DataSource = transList;
                        }
                        else
                        {
                            dgvTransactionList.DataSource = "";
                            MessageBox.Show("Item not found!", "Cannot find");
                        }
                    }
                    else
                    {
                        dgvTransactionList.DataSource = "";
                    }
                }
            }
        }

        private void dgvTransactionList_CustomCellFormatting()
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            // Transaction Delete
            if (!MemberShip.isAdmin && !controller.CreditTransaction.EditOrDelete)
            {
                dgvTransactionList.Columns["colDelete"].Visible = false;
            }
            // Transaction Delete And Copy
            if (!MemberShip.isAdmin && !controller.CreditTransaction.DeleteAndCopy)
            {
                dgvTransactionList.Columns["colDeleteAndCopy"].Visible = false;
            }
        }
        #endregion





    }
}
