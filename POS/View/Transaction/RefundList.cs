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
    public partial class RefundList : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();
        public string transactionId;

        int Qty = 0;

        List<Stock_Transaction> productList = new List<Stock_Transaction>();

        Boolean isstart = false;
        #endregion

        #region Event

        public RefundList()
        {
            InitializeComponent();
        }

        private void RefundList_Load(object sender, EventArgs e)
        {
            Utility.BindShop(cboshoplist);
            cboshoplist.Text = SettingController.DefaultShop.ShopName;
            Utility.ShopComBo_EnableOrNot(cboshoplist);
            isstart = true;
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

        private void dgvRefundList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvRefundList.Rows)
            {
                Transaction currentt = (Transaction)row.DataBoundItem;
                row.Cells[0].Value = currentt.Id;
                row.Cells[1].Value = currentt.DateTime.Value.ToString("dd-MM-yyyy");
                row.Cells[2].Value = currentt.DateTime.Value.ToString("hh:mm");
                row.Cells[3].Value = currentt.User.Name;
                row.Cells[4].Value = currentt.TotalAmount - currentt.DiscountAmount;
                row.Cells[dgvRefundList.ColumnCount-1].Value = currentt.IsExported;
            }
        }

        private void dgvRefundList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string currentTransactionId = dgvRefundList.Rows[e.RowIndex].Cells[0].Value.ToString();
                //if (e.ColumnIndex == 5)
                //{ 
                //    //to print
                //}
                if (e.ColumnIndex == 5)
                {
                    RefundDetail newForm = new RefundDetail();
                    newForm.transactionId = currentTransactionId;
                    newForm.IsRefund = false;
                    newForm.ShowDialog();
                }
                //Delete
                else if (e.ColumnIndex == 6)
                {
                    var isexp = dgvRefundList.Rows[e.RowIndex].Cells[dgvRefundList.Columns.Count - 1].Value.ToString();
                    if (bool.Parse(isexp.ToString()))
                    {
                        MessageBox.Show("You can't delete SAP exported refunded transaction!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        Transaction DeleteTransactionObj = (from t in entity.Transactions where t.Id == currentTransactionId select t).FirstOrDefault();
                        DeleteTransactionObj.IsDeleted = true;
                        DeleteTransactionObj.IsActive = false;
                        DeleteTransactionObj.UpdatedDate = DateTime.Now;

                        //var deletetranDetail=(from d in DeleteTransactionObj.TransactionDetails select d).ToList();

                        //Settlement
                       var OriginalTran =(from t in entity.Transactions where t.TranVouNos == DeleteTransactionObj.ParentId select t).FirstOrDefault();
                       if (OriginalTran != null)
                       {
                           OriginalTran.TranVouNos = "";
                           entity.Entry(OriginalTran).State = System.Data.EntityState.Modified;
                       }



                        DeleteLog dl = new DeleteLog();
                        dl.DeletedDate = DateTime.Now;
                        dl.CounterId = MemberShip.CounterId;
                        dl.UserId = MemberShip.UserId;
                        dl.IsParent = true;
                        dl.TransactionId = DeleteTransactionObj.Id;

                        entity.DeleteLogs.Add(dl);

                        foreach (TransactionDetail td in DeleteTransactionObj.TransactionDetails)
                        {
                            td.Product.Qty =td.Product.Qty- td.Qty;
                            td.IsDeleted = true;
                            Utility.MinusProductAvailableQtyCosOfSales(entity, (long)td.ProductId, td.BatchNo, (int)td.Qty);
                            //save in stocktransaction

                            Stock_Transaction st = new Stock_Transaction();
                            st.ProductId = td.Product.Id;
                            Qty -= Convert.ToInt32(td.Qty);
                            st.Refund = Qty;
                            productList.Add(st);
                            Qty = 0;

                            if (td.Product.IsWrapper == true)
                            {
                                List<WrapperItem> wList = td.Product.WrapperItems.ToList();
                                if (wList.Count > 0)
                                {
                                    foreach (WrapperItem w in wList)
                                    {
                                        Product wpObj = (from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault();
                                        wpObj.Qty = wpObj.Qty - td.Qty;
                                    }
                                }
                            }
                        }

                        //save in stocktransaction
                        Save_RefundQty_ToStockTransaction(productList, Convert.ToDateTime(DeleteTransactionObj.DateTime));


                        productList.Clear();
                        entity.SaveChanges();


                        #region Purchase Delete                      
                        
                        List<TransactionDetail> tdlist = entity.TransactionDetails.Where(x => x.TransactionId == DeleteTransactionObj.ParentId).ToList();

                        foreach (TransactionDetail td in tdlist)
                        {
                            foreach (TransactionDetail detail in DeleteTransactionObj.TransactionDetails)
                            {
                                List<APP_Data.PurchaseDetailInTransaction> puInTranDetail = (from p in entity.PurchaseDetailInTransactions where p.TransactionDetailId ==td.Id && p.ProductId == detail.ProductId orderby p.Id ascending select p).ToList();
                                if (puInTranDetail.Count > 0)
                                {
                                    foreach (PurchaseDetailInTransaction p in puInTranDetail)
                                    {
                                        PurchaseDetail pud = entity.PurchaseDetails.Where(x => x.Id == p.PurchaseDetailId).FirstOrDefault();
                                        if (pud != null)
                                        {
                                            //if (pud.CurrentQy >=0 && p.Qty >= 0)
                                            if (pud.CurrentQy > 0)
                                            {
                                                pud.CurrentQy = pud.CurrentQy - detail.Qty;
                                                p.Qty = p.Qty + detail.Qty;
                                                entity.Entry(pud).State = EntityState.Modified;
                                                entity.Entry(p).State = EntityState.Modified;
                                                entity.SaveChanges();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }                      

                        //TransactionDetail chktrandetial = entity.TransactionDetails.Where(x => x.TransactionId ==DeleteTransactionObj.ParentId).FirstOrDefault();   
                        #endregion

                        // By SYM // For MemberType of Customer
                        #region Member Type
                        string minimumAmountofThisMemberType = (from mCardRule in entity.MemberCardRules orderby mCardRule.RangeFrom select mCardRule.RangeFrom).FirstOrDefault();
                        int memberValidityYear = Convert.ToInt32(SettingController.DefaultMemberValidityYear.ToString());
                        string parentID = DeleteTransactionObj.ParentId;
                        Transaction _currentTransaction = (from t in entity.Transactions where t.Id == parentID select t).FirstOrDefault();
                        int customerID = Convert.ToInt32(_currentTransaction.CustomerId); // customerid, transactionid
                        Customer currentCustomer = (from c in entity.Customers where c.Id == customerID select c).FirstOrDefault<Customer>();
                        List<MemberCardRule> memberCardRuleList = (from m in entity.MemberCardRules orderby m.RangeFrom select m).ToList();
                        int totalAmount = 0;
                        int memberTypeId = 0;
                        string expireDate = null;

                        if (currentCustomer.Name != "Default")
                        {
                            if (currentCustomer.MemberTypeID != null)
                            {
                                if (DateTime.Today.Date <= currentCustomer.ExpireDate)
                                {//not expired

                                    List<Transaction> transactionList = (from transactions in entity.Transactions
                                                                         join c in entity.Customers
                                                                         on transactions.CustomerId equals c.Id
                                                                         where (transactions.CustomerId == customerID) && (c.Id == customerID) && (transactions.IsDeleted == false)
                                                                         && (transactions.IsComplete == true) && (transactions.PaymentTypeId != 4) && (transactions.PaymentTypeId != 6)
                                                                         && (transactions.DateTime >= c.StartDate)
                                                                         && (transactions.DateTime <= c.ExpireDate)
                                                                         select transactions).ToList();

                                    foreach (Transaction t in transactionList)
                                    {
                                        totalAmount += Convert.ToInt32(t.TotalAmount);
                                    }
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
                                else
                                {// aldy expired
                                    int fromYear = currentCustomer.ExpireDate.Value.Year - memberValidityYear;
                                    int fromMonth = currentCustomer.ExpireDate.Value.Month;
                                    int fromDay = currentCustomer.ExpireDate.Value.Day;
                                    string fromDate = Convert.ToString(fromYear + "/" + fromMonth + "/" + fromDay);
                                    DateTime _fromDate = Convert.ToDateTime(fromDate);

                                    int _expireYear = currentCustomer.ExpireDate.Value.Year + memberValidityYear;
                                    expireDate = Convert.ToString(_expireYear + "/" + fromMonth + "/" + fromDay);
                                    DateTime _expireDate = Convert.ToDateTime(expireDate);

                                    if (DateTime.Today.Date >= _fromDate && DateTime.Today.Date <= _expireDate)
                                    {// aldy expired but can extend to next year, so not renew
                                        //int _currentAmount=0;
                                        List<Transaction> transactionList = (from transactions in entity.Transactions
                                                                             join c in entity.Customers
                                                                             on transactions.CustomerId equals c.Id
                                                                             where (transactions.CustomerId == customerID) && (c.Id == customerID) && (transactions.DateTime >= _fromDate)
                                                                             && (transactions.DateTime <= _expireDate) && (transactions.IsDeleted == false)
                                                                             && (transactions.IsComplete == true) && (transactions.PaymentTypeId != 4) && (transactions.PaymentTypeId != 6)
                                                                             select transactions).ToList();

                                        foreach (Transaction t in transactionList)
                                        {
                                            totalAmount += Convert.ToInt32(t.TotalAmount);
                                        }

                                        foreach (MemberCardRule memberCard in memberCardRuleList)
                                        {
                                            if (Convert.ToInt32(memberCard.RangeFrom) <= totalAmount)
                                            {
                                                memberTypeId = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();
                                            }
                                        }

                                        currentCustomer.MemberTypeID = memberTypeId;
                                        currentCustomer.StartDate = currentCustomer.ExpireDate;
                                        currentCustomer.ExpireDate = _expireDate;
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
                                    else
                                    {
                                        currentCustomer.MemberTypeID = null;
                                        currentCustomer.StartDate = null;
                                        currentCustomer.FirstTime = null;
                                        currentCustomer.ExpireDate = null;
                                        currentCustomer.PromoteDate = null;

                                        entity.SaveChanges();

                                    }
                                }
                            }
                        }

                        #endregion


                        LoadData();
                    }
                }
            }
        }

        private void cboshoplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        #endregion

        #region Function

        #region for saving Refund Qty in Stock Transaction table
        private void Save_RefundQty_ToStockTransaction(List<Stock_Transaction> productList, DateTime _tranDate)
        {
            int _year, _month;

            _year = _tranDate.Year;
            _month = _tranDate.Month;
            Utility.Refund_Run_Process(_year, _month, productList);
        }
        #endregion

        private void LoadData()
        {
            if (isstart == true)
            {
                int shopid = Convert.ToInt32(cboshoplist.SelectedValue);
                string currentshortcode = (from p in entity.Shops where p.Id == shopid select p.ShortCode).FirstOrDefault();
                string currentshopname = (from p in entity.Shops where p.Id == shopid select p.ShopName).FirstOrDefault();

                bool optionvisible = Utility.TransactionDelRefHide(shopid);
                dgvRefundList.Columns[ColDelete.Index].Visible = optionvisible;

                DateTime fromDate = dtpFrom.Value.Date;
                DateTime toDate = dtpTo.Value.Date;
                dgvRefundList_CustomCellFormatting();
                dgvRefundList.AutoGenerateColumns = false;
                if (transactionId == null)
                {
                    //List<Transaction> transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true select t).ToList<Transaction>();
                    List<Transaction> transactionList = (from t in entity.Transactions where t.Id.Substring(2, 2) == currentshortcode && (EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsActive != false) && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) select t).ToList().Where(x => x.IsDeleted != true).ToList<Transaction>();
                    dgvRefundList.DataSource = transactionList;
                }
                else
                {
                    //List<Transaction> transactionList = (from t in entity.Transactions where (EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsActive != false && t.ParentId == transactionId) && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) select t).ToList<Transaction>();
                    List<Transaction> transactionList = (from t in entity.Transactions where t.Id.Substring(2, 2) == currentshortcode && (t.IsActive != false && t.ParentId == transactionId) && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) select t).ToList().Where(x => x.IsDeleted != true).ToList<Transaction>();
                    dgvRefundList.DataSource = transactionList;
                }
            }
        }

        private void dgvRefundList_CustomCellFormatting()
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            // Refund Delete
            if (!MemberShip.isAdmin && !controller.Refund.EditOrDelete)
            {
                dgvRefundList.Columns["ColDelete"].Visible = false;
            }
          
        }
        #endregion

    
        

       
    }
}
