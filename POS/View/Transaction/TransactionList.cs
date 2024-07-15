using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Windows.Forms;
using POS.APP_Data;

namespace POS
{
    public partial class TransactionList : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();
        int Qty = 0;

        List<Stock_Transaction> productList = new List<Stock_Transaction>();

        public int index = 0;
        private Boolean IsStart = false;
        public Boolean IsBackOffice;
        #endregion

        #region Event

        public TransactionList()
        {
            InitializeComponent();
        }

        private void TransactionList_Load(object sender, EventArgs e)
        {
            dgvTransactionList.AutoGenerateColumns = false;



            //switch (((MDIParent)this.ParentForm).tSSBOOrPOS.Text)
            //{
            //    case:
            //}
            Utility.BindShop(cboshoplist);
            cboshoplist.Text = SettingController.DefaultShop.ShopName;
            Utility.ShopComBo_EnableOrNot(cboshoplist);

            IsStart = true;
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
                txtId.Enabled = true;
            }
            LoadData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();

        }

        private void dgvTransactionList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                string currentTransactionId = dgvTransactionList.Rows[e.RowIndex].Cells[0].Value.ToString();

                // List<int> type =new List<int> { 3, 4, 5, 6 };// for what?
                List<APP_Data.TransactionDetail> _isConsignmentPaidTranList = entity.TransactionDetails.Where(x => x.TransactionId == currentTransactionId && x.IsDeleted == false && x.IsConsignmentPaid == true).ToList();

                var isexp = dgvTransactionList.Rows[e.RowIndex].Cells[dgvTransactionList.Columns.Count - 1].Value.ToString();

                //Refund
                if (e.ColumnIndex == colRefund.Index)
                {
                    Transaction tObj = (Transaction)dgvTransactionList.Rows[e.RowIndex].DataBoundItem;

                    if (tObj.PaymentType.Name == "FOC")
                    {
                        MessageBox.Show("FOC Transaction is Non Refundable!", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (tObj.PaymentType.Name == "Tester")
                    {
                        MessageBox.Show("Tester Transaction is Non Refundable!", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (tObj.PaymentType.Name == "Gift Card")
                    {
                        MessageBox.Show("Giftcard Transaction is Non Refundable!", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (tObj.PaymentType.Name == "MultiPayment")
                    {

                        bool IsGiftCardPayment = false;

                        var isGiftPay = (from tpd in entity.TransactionPaymentDetails
                                         join p in entity.PaymentMethods on tpd.PaymentMethodId equals p.Id
                                         where tpd.TransactionId == tObj.Id && p.Name.Contains("Gift")
                                         select tpd);
                        if (isGiftPay != null && isGiftPay.Count() > 0)
                        {
                            IsGiftCardPayment = true;
                        }

                        if (IsGiftCardPayment)
                        {
                            MessageBox.Show("Transaction with GiftCard Payment is Non Refundable!", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {

                            RefundTransaction newForm = new RefundTransaction(false);
                            newForm.transactionId = currentTransactionId;
                            newForm.ShowDialog();
                        }

                    }
                    else
                    {
                        if (tObj.Type != "Settlement")
                        {
                            if (_isConsignmentPaidTranList.Count > 0)
                            {
                                MessageBox.Show("This transaction already  made  Consignment Settlement. It cannot be refund!");
                            }
                            else
                            {
                                // bool bexp = false;
                                // bool.TryParse(isexp, out bexp);
                                RefundTransaction newForm = new RefundTransaction(false);
                                newForm.transactionId = currentTransactionId;
                                newForm.IsCash = true;
                                newForm.ShowDialog();
                            }

                        }
                        else
                        {
                            colRefund.ReadOnly = true;


                        }
                    }
                }

                //View Detail
                else if (e.ColumnIndex == colViewDetail.Index)
                {

                    var tranType = (from t in entity.Transactions where t.Id == currentTransactionId select t.Type).FirstOrDefault();
                    if (tranType == "Settlement")
                    {
                        colViewDetail.ReadOnly = true;
                    }
                    else
                    {
                        bool bexp = false;
                        bool.TryParse(isexp, out bexp);
                        TransactionDetailForm newForm = new TransactionDetailForm(bexp);
                        newForm.transactionId = currentTransactionId;
                        newForm.shopid = Convert.ToInt32(cboshoplist.SelectedValue);
                        newForm.ShowDialog();
                    }
                }
                //Delete the record and add delete log
                else if (e.ColumnIndex == colDelete.Index)
                {
                    if (bool.Parse(isexp.ToString()))
                    {
                        MessageBox.Show("You can't delete SAP exported transaction!", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    Transaction ts = entity.Transactions.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                    if (ts != null)
                    {
                        //int iDis = 0;
                        //if (ts.DiscountAmount != null)
                        //{
                        //    iDis = (int)ts.DiscountAmount;
                        //}
                        //if (ts.GiftCardAmount != null)
                        //{
                        //    iDis += (int)ts.GiftCardAmount;
                        //}
                        if (ts.Customer != null)
                        {
                            bool res = Utility.CanRemoveSaleTransaction(ts,ts.PaymentTypeId, ts.TransactionDetails.AsQueryable(), (int)ts.CustomerId, ts.Customer);
                            if (res == false)
                            {
                                return;
                            }
                        }

                        //Role Management
                        RoleManagementController controller = new RoleManagementController();
                        controller.Load(MemberShip.UserRoleId);
                        if (controller.Transaction.EditOrDelete || MemberShip.isAdmin)
                        {
                            #region Delete

                            if (ts.Type == "Settlement")
                            {
                                colDelete.ReadOnly = true;
                            }
                            else
                            {
                                APP_Data.Transaction ts2 = entity.Transactions.Where(x => x.ParentId == currentTransactionId && x.IsDeleted == false).FirstOrDefault();

                                if (ts2 != null)
                                {
                                    MessageBox.Show("This transaction already made  refund. It cannot be deleted!");

                                }
                                else if (_isConsignmentPaidTranList.Count > 0)
                                {
                                    MessageBox.Show("This transaction already made  Consignment Settlement. It cannot be deleted!");
                                }
                                else
                                {

                                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (result.Equals(DialogResult.OK))
                                    {

                                        ELC_CustomerPointSystem.Delete_ReferralPointInTransaction(currentTransactionId);
                                        ts.IsDeleted = true;
                                        ts.UpdatedDate = DateTime.Now;
                                        // add gift card amount
                                        if (ts.GiftCardId != null && ts.GiftCard != null)
                                        {
                                            ts.GiftCard.Amount = ts.GiftCard.Amount + Convert.ToInt32(ts.GiftCardAmount);
                                        }
                                        Utility.GiftCardIsBack(ts);
                                        foreach (TransactionDetail detail in ts.TransactionDetails)
                                        {
                                            #region reduce qty in stock filling from sap KHS
                                            Utility.AddProductAvailableQty(entity, (long)detail.ProductId, detail.BatchNo, (int)detail.Qty);
                                            #endregion

                                            //detail.IsDeleted = false;
                                            detail.IsDeleted = true;
                                            detail.Product.Qty = detail.Product.Qty + detail.Qty;


                                            //save in stocktransaction

                                            Stock_Transaction st = new Stock_Transaction();
                                            st.ProductId = detail.Product.Id;
                                            Qty -= Convert.ToInt32(detail.Qty);
                                            st.Sale = Qty;
                                            productList.Add(st);
                                            Qty = 0;

                                            if (detail.Product.IsWrapper == true)
                                            {
                                                List<WrapperItem> wList = detail.Product.WrapperItems.ToList();
                                                if (wList.Count > 0)
                                                {
                                                    foreach (WrapperItem w in wList)
                                                    {
                                                        Product wpObj = (from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault();
                                                        wpObj.Qty = wpObj.Qty + detail.Qty;
                                                    }
                                                }
                                            }



                                            //For Purchase 
                                            #region Purchase Delete

                                            List<APP_Data.PurchaseDetailInTransaction> puInTranDetail = entity.PurchaseDetailInTransactions.Where(x => x.TransactionDetailId == detail.Id && x.ProductId == detail.ProductId).ToList();
                                            if (puInTranDetail.Count > 0)
                                            {
                                                foreach (PurchaseDetailInTransaction p in puInTranDetail)
                                                {
                                                    PurchaseDetail pud = entity.PurchaseDetails.Where(x => x.Id == p.PurchaseDetailId).FirstOrDefault();
                                                    if (pud != null)
                                                    {
                                                        pud.CurrentQy = pud.CurrentQy + p.Qty;
                                                    }
                                                    entity.Entry(pud).State = EntityState.Modified;
                                                    entity.SaveChanges();

                                                    //entity.PurchaseDetailInTransactions.Remove(p);
                                                    //entity.SaveChanges();

                                                    p.Qty = 0;
                                                    entity.Entry(p).State = EntityState.Modified;

                                                    entity.PurchaseDetailInTransactions.Remove(p);
                                                    entity.SaveChanges();
                                                }
                                            }
                                            #endregion



                                        }

                                        string date = dgvTransactionList.Rows[e.RowIndex].Cells[3].Value.ToString();
                                        DateTime _Trandate = Utility.Convert_Date(date);
                                        //save in stock transaction
                                        Save_SaleQty_ToStockTransaction(productList, _Trandate);
                                        productList.Clear();
                                        DeleteLog dl = new DeleteLog();
                                        dl.DeletedDate = DateTime.Now;
                                        dl.CounterId = MemberShip.CounterId;
                                        dl.UserId = MemberShip.UserId;
                                        dl.IsParent = true;
                                        dl.TransactionId = ts.Id;

                                        entity.DeleteLogs.Add(dl);

                                        entity.SaveChanges();

                                        VipCustomer vipCustomer = new VipCustomer();
                                        DateTime lastTwoYears = DateTime.Now.AddYears(-2);
                                        int _customerID = (int)ts.CustomerId;
                                        string CustomerCode = ts.Customer.CustomerCode;
                                        string memberTypeName = string.Empty;
                                        if (ts.Customer.MemberType != null)
                                        {
                                            memberTypeName = ts.Customer.MemberType.Name;
                                        }
                                        var vipTransactionlist = entity.Transactions.Where(x => x.IsDeleted == false && x.IsActive == true
                                                     && x.CustomerId == _customerID && (x.DateTime >= lastTwoYears && x.DateTime <= DateTime.Now)).ToList();
                                        if (ts.Customer.MemberTypeID != null)

                                        {
                                            if (vipTransactionlist.Count > 0)
                                            {
                                                var totalPaidAmount = (int)(vipTransactionlist.Select(x => x.TotalAmount).Sum() - vipTransactionlist.Select(x => x.GiftCardAmount).Sum());
                                                vipCustomer.CustomerCode = CustomerCode;
                                                vipCustomer.LastPaidDate = DateTime.Now;
                                                vipCustomer.TwoYearsTotalAmount = totalPaidAmount;
                                                vipCustomer.CreatedUserID = MemberShip.UserId;
                                                vipCustomer.MemberType = memberTypeName;
                                                vipCustomer.CreatedDate = DateTime.Now;
                                                vipCustomer.ShopCode = SettingController.DefaultShop.ShortCode;
                                                entity.VipCustomers.Add(vipCustomer);
                                                entity.SaveChanges();
                                            }

                                        }
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

                                        //#region Updage Gift Card IsUsed=false if use gift card in that transaction
                                        //Update_GiftCard(ts.Id, ts.PaymentTypeId);
                                        //#endregion

                                        LoadData();
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            MessageBox.Show("You are not allowed to delete transaction information", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
                else if (e.ColumnIndex == colDeleteAndCopy.Index)
                {
                    if (bool.Parse(isexp.ToString()))
                    {
                        MessageBox.Show("You can't delete SAP exported transaction!", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    Transaction ts = entity.Transactions.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                    if (ts != null)
                    {
                        //int iDis = 0;
                        //if (ts.DiscountAmount != null)
                        //{
                        //    iDis = (int)ts.DiscountAmount;
                        //}
                        //if (ts.GiftCardAmount != null)
                        //{
                        //    iDis += (int)ts.GiftCardAmount;
                        //}
                        if (ts.Customer != null)
                        {
                            bool res = Utility.CanRemoveSaleTransaction(ts,ts.PaymentTypeId, ts.TransactionDetails.AsQueryable(), (int)ts.CustomerId, ts.Customer);
                            if (res == false)
                            {
                                return;
                            }
                        }

                        //Role Management
                        RoleManagementController controller = new RoleManagementController();
                        controller.Load(MemberShip.UserRoleId);
                        if (controller.Transaction.DeleteAndCopy || MemberShip.isAdmin)
                        {

                            #region Delete And Copy

                            if (ts.Type == "Settlement")
                            {

                                colDeleteAndCopy.ReadOnly = true;

                            }
                            else
                            {
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
                                    MessageBox.Show("This transaction already made  Consignment Settlement. It cannot be deleted!");
                                }
                                else
                                {

                                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                    if (result.Equals(DialogResult.OK))
                                    {
                                        ELC_CustomerPointSystem.Delete_ReferralPointInTransaction(currentTransactionId);

                                        //#region Updage Gift Card IsUsed=false if use gift card in that transaction
                                        //Update_GiftCard(ts.Id, ts.PaymentTypeId);
                                        //#endregion

                                        if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                                        {
                                            Utility.GiftCardIsBack(ts);

                                            Sales openedForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                                            openedForm.DeleteCopy(currentTransactionId);
                                            this.Dispose();
                                        }
                                    }
                                }


                            }
                        }
                        else
                        {
                            MessageBox.Show("You are not allowed to delete transaction information", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }

                }
                #endregion

            }
        }

        //public void Update_GiftCard(string TransactionId, int? paymentTypeId)
        //{
        //    //#region Updage Gift Card IsUsed=false if use gift card in that transaction
        //    //if (paymentTypeId == 11)// change from 3 by TTN
        //    //{
        //    //    ELC_CustomerPointSystem.Update_GiftCard(TransactionId);
        //    //}
        //    //#endregion
        //    #region Updage Gift Card IsUsed=false if use gift card in that transaction
        //        if (paymentTypeId == 11)// change from 3 by TTN
        //        {
        //            ELC_CustomerPointSystem.Update_GiftCard(TransactionId);
        //        }
        //        #endregion

        //}


        private void dgvTransactionList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvTransactionList.Rows)
            {
                Transaction currentt = (Transaction)row.DataBoundItem;
                row.Cells[ColTransactionId.Index].Value = currentt.Id;
                row.Cells[ColType.Index].Value = currentt.Type;
                row.Cells[ColPaymentMethod.Index].Value = currentt.PaymentType.Name;
                row.Cells[ColDate.Index].Value = currentt.DateTime.Value.ToString("dd-MM-yyyy");
                row.Cells[ColTime.Index].Value = currentt.DateTime.Value.ToString("hh:mm");
                row.Cells[ColSalesPerson.Index].Value = currentt.User.Name;



                var refundList = (from t in entity.Transactions where t.Type == TransactionType.Refund && t.ParentId == currentt.Id && t.IsDeleted == false select t).ToList();
                int refundAmt = Convert.ToInt32(refundList.Sum(x => x.TotalAmount));
                var DiscountAmt = Convert.ToInt32(refundList.Sum(x => x.DiscountAmount));

                int currentRefundAmt = refundAmt - DiscountAmt;
                if (currentt.PaymentType.Name != "FOC")
                {
                    row.Cells[ColReceivedAmt.Index].Value = currentt.TotalAmount - currentRefundAmt;
                }
                else
                {
                    row.Cells[ColReceivedAmt.Index].Value = 0;
                }

                row.Cells[colRefundAmt.Index].Value = currentRefundAmt;


                if (BOOrPOS.IsBackOffice == true)
                {
                    if (dgvTransactionList.Columns[colDeleteAndCopy.Index].Visible != false)
                    {
                        dgvTransactionList.Columns[colDeleteAndCopy.Index].Visible = false;
                    }
                }
                else
                {
                    if (dgvTransactionList.Columns[colDeleteAndCopy.Index].Visible != false)
                    {
                        dgvTransactionList.Columns[colDeleteAndCopy.Index].Visible = true;
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
            if (!MemberShip.isAdmin && !controller.Transaction.EditOrDelete)
            {
                dgvTransactionList.Columns["colDelete"].Visible = false;
            }
            // Transaction Delete And Copy
            if (!MemberShip.isAdmin && !controller.Transaction.DeleteAndCopy)
            {

                dgvTransactionList.Columns["colDeleteAndCopy"].Visible = false;
            }
        }

        private void cboshoplist_selectedvaluechanged(object sender, EventArgs e)
        {

            LoadData();
        }
        #endregion

        #region Function
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

            if (IsStart == true)
            {


                entity = new POSEntities();
                dgvTransactionList_CustomCellFormatting();

                int shopid = Convert.ToInt32(cboshoplist.SelectedValue);
                string shortcode = (from p in entity.Shops where p.Id == shopid select p.ShortCode).FirstOrDefault();

                bool optionvisible = Utility.TransactionDelRefHide(shopid);
                //  dgvTransactionList.Columns[8].Visible = optionvisible;
                dgvTransactionList.Columns[colDelete.Index].Visible = optionvisible;
                dgvTransactionList.Columns[colDeleteAndCopy.Index].Visible = optionvisible;

                List<string> type = new List<string>();
                type.Add(TransactionType.Sale);

                if (rdbDate.Checked)
                {
                    DateTime fromDate = dtpFrom.Value.Date;
                    DateTime toDate = dtpTo.Value.Date;

                    // type.Add(TransactionType.Settlement);
                    // type.Add(TransactionType.Prepaid);


                    //List<Transaction> transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && t.Type == TransactionType.Sale && t.IsDeleted==false select t).ToList<Transaction>();
                    List<Transaction> transList = (from t in entity.Transactions
                                                   where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true
                                                       && t.IsActive == true && type.Contains(t.Type) && (t.IsDeleted == false || t.IsDeleted == null) && (t.PaymentTypeId != null)
                                                       && t.Id.Substring(2, 2) == shortcode
                                                   select t).ToList<Transaction>();
                    if (transList.Count > 0)
                    {
                        dgvTransactionList.DataSource = transList.Where(x => x.IsDeleted != true).ToList();
                    }
                    else
                    {
                        dgvTransactionList.DataSource = "";
                        //MessageBox.Show("Item not found!", "Cannot find");
                    }

                }
                else
                {
                    string Id = txtId.Text;
                    if (Id.Trim() != string.Empty)
                    {
                        List<Transaction> transList = (from t in entity.Transactions where t.Id == Id && type.Contains(t.Type) && t.Id.Substring(2, 2) == shortcode select t).ToList().Where(x => x.IsDeleted != true).ToList();
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

        #endregion





    }
}

