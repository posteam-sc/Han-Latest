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
    public partial class PaidByMPU : Form
    {
        #region Variables
        POSEntities entity1 = new POSEntities();
        bool FirstTime = false;

        public List<TransactionDetail> DetailList = new List<TransactionDetail>();

        public int Discount { get; set; }

        public int Tax { get; set; }

        public int ExtraDiscount { get; set; }

        public int ExtraTax { get; set; }

        public Boolean isDraft { get; set; }

        public Boolean IsWholeSale { get; set; }

        public string DraftId { get; set; }

        public int CustomerId { get; set; }

        private POSEntities entity = new POSEntities();

        public decimal BDDiscount { get; set; }

        public decimal MCDiscount { get; set; }

        public int? MemberTypeId { get; set; }

        public decimal? MCDiscountPercent { get; set; }

        public DialogResult _result;

        public decimal TotalAmt = 0;

        string resultId = "";
        long totalCost;

        int Qty = 0;

        List<Stock_Transaction> productList = new List<Stock_Transaction>();

        public Boolean IsPrint = false;
        #endregion
        public PaidByMPU()
        {
            InitializeComponent();
        }

        private void PaidByMPU_Load(object sender, EventArgs e)
        {

            totalCost = (long)DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (long)BDDiscount - (long)MCDiscount;
            long unitpriceTotalCost = (long)DetailList.Sum(x => x.UnitPrice * x.Qty);//Edit ZMH
            //Updated by Lele
            //System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 5, ExtraTax + Tax, ExtraDiscount + Discount, totalCost, totalCost, null, CustomerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, true);
            //System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 5, ExtraTax + Tax, ExtraDiscount + Discount, totalCost, totalCost, null, CustomerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, true);
            //Updated by Yimon
            System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 5, ExtraTax + Tax, ExtraDiscount + Discount, totalCost, totalCost, null, CustomerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, true,"","");

            entity = new POSEntities();
            resultId = Id.FirstOrDefault().ToString();
            Transaction insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
            insertedTransaction.IsDeleted = false;
            entity.SaveChanges();

            #region Referral Point for Referral Customer
            var _customerId = entity1.Transactions.Where(x => x.Id == resultId).Select(x => x.CustomerId).FirstOrDefault();
            if (entity1.Customers.Where(x => x.Id == _customerId).Select(x => x.MemberTypeID).FirstOrDefault() == null)
            {
                (from t in DetailList select t).ToList().ForEach(t => t.TransactionId = resultId);
                ELC_CustomerPointSystem.Buy_ReferralProductOrNot(DetailList);
            }
            #endregion


            // By SYM // for membertype in customer 
            #region MemberType


            int memberValidityYear = Convert.ToInt32(SettingController.DefaultMemberValidityYear.ToString());
            NewCustomer _newCustomer = new NewCustomer();
            List<MemberCardRule> memberCardRuleList = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true orderby int.Parse(m.RangeFrom) select m).ToList();
            int memberTypeID = 0;
            string memName = null;
            int totalAmount = 0;
            int giftCardTotalAmount = 0;
            int couponCodeTotalAmount = 0;
            int totalRAmount = 0;
            string minimumAmountofThisMemberType = (from mCardRule in entity.MemberCardRules.AsEnumerable() where mCardRule.IsActive == true orderby int.Parse(mCardRule.RangeFrom) select mCardRule.RangeFrom).FirstOrDefault();
            string name = (from c in entity.Customers where c.Id == CustomerId select c.Name).FirstOrDefault();
            if (name != "Default")
            {
                APP_Data.Customer customerObj = (from c in entity.Customers where c.Id == CustomerId select c).FirstOrDefault();
                int mTypeID = (from memberCardRule in entity.MemberCardRules.AsEnumerable() where memberCardRule.IsActive == true orderby int.Parse(memberCardRule.RangeFrom) descending select memberCardRule.MemberTypeId).FirstOrDefault();
                if (customerObj.PromoteDate != null)
                {
                    // isexpired? == no
                    if (DateTime.Today.Date <= customerObj.ExpireDate)
                    { // ok
                        var LastOneYear = DateTime.Now.AddYears(-1);
                        if (customerObj.MemberTypeID != mTypeID) //if selected customer is not the greatest
                        {
                            List<Transaction> transactionList = (from t in entity.Transactions
                                                                 join c in entity.Customers
                                                                 on t.CustomerId equals c.Id
                                                                 where (t.CustomerId == CustomerId) && (c.Id == CustomerId) && (t.DateTime >= LastOneYear)
                                                                 && (t.DateTime <= DateTime.Now) && (t.IsDeleted == false || t.IsDeleted == null) && (t.IsComplete == true)
                                                                 && (t.PaymentTypeId == 1 || t.PaymentTypeId == 2 || t.PaymentTypeId == 3 || t.PaymentTypeId == 5 || t.PaymentTypeId == null)
                                                                 && (t.Type != "Prepaid") && (t.Type != "Settlement") && (t.Type != "Refund") && (t.Type != "CreditRefund")
                                                                 select t).ToList();

                            List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                   join transactions in entity.Transactions
                                                                                   on td.TransactionId equals transactions.ParentId
                                                                                   join c in entity.Customers
                                                                                   on transactions.CustomerId equals c.Id
                                                                                   where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
                                                                                   && (transactions.IsDeleted == false || transactions.IsDeleted == null) && (transactions.IsComplete == true)
                                                                                   && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
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

                            //Added by Lele
                            foreach (Transaction t in transactionList)
                            {
                                couponCodeTotalAmount += Convert.ToInt32(t.CouponCodeAmount);
                            }

                            //Updated by Lele
                            //totalAmount = totalAmount - giftCardTotalAmount;
                            totalAmount = totalAmount - (giftCardTotalAmount + couponCodeTotalAmount);

                            foreach (TransactionDetail td in RefundTransactionDetailList)
                            {
                                totalRAmount += Convert.ToInt32(td.TotalAmount);
                            }
                            totalAmount = totalAmount - totalRAmount;
                            foreach (MemberCardRule memberCard in memberCardRuleList)
                            {
                                if (Convert.ToInt32(memberCard.RangeFrom) <= totalAmount)
                                {
                                    memberTypeID = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();
                                    //int id = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();

                                }
                            }

                            memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                            if (customerObj.MemberTypeID != memberTypeID)
                            {
                                if (memName == null)
                                {
                                    switch (customerObj.MemberType.Name)
                                    {
                                        case "Level One":
                                            memName = "Tire One";
                                            break;
                                        case "Tire Two ":
                                            memName = "Tire One";
                                            break;
                                        case "Level Two":
                                            memName = "Level One";
                                            break;
                                        case "Tire Three":
                                            memName = "Tire Two";
                                            break;
                                        case "Level Three":
                                            memName = "Level Two";
                                            break;
                                        case "T2":
                                            memName = "T1";
                                            break;
                                        default:
                                            memName = "Tire One";
                                            break;
                                    }
                                    if (resultId != null)
                                    {
                                        var Tran = (from t in entity.Transactions
                                                    where t.Id == resultId
                                                    select t).First();
                                        Tran.MemberTypeId = customerObj.MemberTypeID;
                                        entity.SaveChanges();
                                    }
                                }
                                else
                                {
                                    _newCustomer.CustomerId = CustomerId;
                                    _newCustomer.MemerTypeName = memName;
                                    _newCustomer.getTotalAmount = Convert.ToInt32(totalCost);
                                    _newCustomer.Type = 'S';
                                    _newCustomer.isEdit = true;
                                    _newCustomer.TransactionId = resultId;
                                    _newCustomer.IsClosed = FirstTime;
                                    _newCustomer.ShowDialog();
                                    FirstTime = ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                                }


                            }
                            else
                            {
                                if (resultId != null)
                                {
                                    var Tran = (from t in entity.Transactions
                                                where t.Id == resultId
                                                select t).First();
                                    Tran.MemberTypeId = customerObj.MemberTypeID;
                                    entity.SaveChanges();
                                }
                            }

                        }
                        else
                        {
                            if (resultId != null)
                            {
                                var Tran = (from t in entity.Transactions
                                            where t.Id == resultId
                                            select t).First();
                                Tran.MemberTypeId = customerObj.MemberTypeID;
                                entity.SaveChanges();
                            }
                        }

                    }
                    else // isexpired? == yes
                    { // ok

                        int _expireYear = customerObj.ExpireDate.Value.Year + memberValidityYear;

                        int fromYear = customerObj.ExpireDate.Value.Year - memberValidityYear;
                        int fromMonth = customerObj.ExpireDate.Value.Month;
                        int fromDay = customerObj.ExpireDate.Value.Day;
                        string fromDate = Convert.ToString(fromYear + "/" + fromMonth + "/" + fromDay);
                        DateTime _fromDate = Convert.ToDateTime(fromDate);

                        string expireDate = Convert.ToString(_expireYear + "/" + fromMonth + "/" + fromDay);
                        DateTime _expireDate = Convert.ToDateTime(expireDate);

                        if (DateTime.Today.Date >= _fromDate && DateTime.Today.Date <= _expireDate)
                        { // aldy expired but can extend to next year, so not renew
                            int _currentAmount = 0;
                            List<Transaction> transactionList = (from transactions in entity.Transactions
                                                                 join c in entity.Customers
                                                                 on transactions.CustomerId equals c.Id
                                                                 where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.DateTime >= _fromDate)
                                                                 && (transactions.DateTime <= _expireDate) && (transactions.IsDeleted == false || transactions.IsDeleted == null) && (transactions.IsComplete == true)
                                                                 && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                 && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "Refund") && (transactions.Type != "CreditRefund")
                                                                 select transactions).ToList();

                            List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                   join transactions in entity.Transactions
                                                                                   on td.TransactionId equals transactions.ParentId
                                                                                   join c in entity.Customers
                                                                                   on transactions.CustomerId equals c.Id
                                                                                   where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.IsDeleted == false || transactions.IsDeleted == null) && (transactions.IsComplete == true)
                                                                                   && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                                   && (transactions.Type != "Refund") && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
                                                                                   && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
                                                                                   select td).ToList();

                            //totalAmount = Convert.ToInt32(TotalAmt);

                            foreach (Transaction t in transactionList)
                            {
                                totalAmount += Convert.ToInt32(t.TotalAmount);
                            }
                            foreach (Transaction t in transactionList)
                            {
                                giftCardTotalAmount += Convert.ToInt32(t.GiftCardAmount);
                            }

                            //Added by Lele
                            foreach (Transaction t in transactionList)
                            {
                                couponCodeTotalAmount += Convert.ToInt32(t.CouponCodeAmount);
                            }

                            //Updated by Lele
                            //totalAmount = totalAmount - giftCardTotalAmount;
                            totalAmount = totalAmount - (giftCardTotalAmount + couponCodeTotalAmount);

                            foreach (TransactionDetail td in RefundTransactionDetailList)
                            {
                                totalRAmount += Convert.ToInt32(td.TotalAmount);
                            }
                            totalAmount = totalAmount - totalRAmount;
                            _currentAmount = totalAmount - Convert.ToInt32(totalCost);
                            if (_currentAmount >= Convert.ToInt32(minimumAmountofThisMemberType))
                            { // this current amount may be a member type
                                foreach (MemberCardRule memberCards in memberCardRuleList)
                                {
                                    if (Convert.ToInt32(memberCards.RangeFrom) <= totalAmount)
                                    {
                                        memberTypeID = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();

                                    }

                                }
                                memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                                if (memName != null)
                                {
                                    _newCustomer.CustomerId = CustomerId;
                                    _newCustomer.MemerTypeName = memName;
                                    _newCustomer.getTotalAmount = Convert.ToInt32(totalCost);
                                    _newCustomer.Type = 'S';
                                    _newCustomer.isEdit = true;
                                    _newCustomer.isExpired = true;
                                    _newCustomer.TransactionId = resultId;
                                    _newCustomer.IsClosed = FirstTime;
                                    _newCustomer.ShowDialog();
                                    FirstTime = ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                                }
                            }
                            else
                            { // cann't extend to next year, so renew
                                foreach (MemberCardRule memberCards in memberCardRuleList)
                                {
                                    if (Convert.ToInt32(memberCards.RangeFrom) <= totalCost)
                                    {
                                        memberTypeID = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();

                                    }

                                }

                                memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                                if (memName != null)
                                {
                                    _newCustomer.CustomerId = CustomerId;
                                    _newCustomer.MemerTypeName = memName;
                                    _newCustomer.getTotalAmount = Convert.ToInt32(totalCost);
                                    _newCustomer.Type = 'S';
                                    _newCustomer.isEdit = true;
                                    _newCustomer.isReNew = true;
                                    _newCustomer.TransactionId = resultId;
                                    _newCustomer.IsClosed = FirstTime;
                                    _newCustomer.ShowDialog();
                                    FirstTime = ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                                }
                            }


                        }
                        else
                        {   // aldy expired and then renew                      

                            foreach (MemberCardRule memberCards in memberCardRuleList)
                            {
                                if (Convert.ToInt32(memberCards.RangeFrom) <= totalCost)
                                {
                                    memberTypeID = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();

                                }

                            }
                            memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                            if (memName != null)
                            {
                                _newCustomer.CustomerId = CustomerId;
                                _newCustomer.MemerTypeName = memName;
                                _newCustomer.getTotalAmount = Convert.ToInt32(totalCost);
                                _newCustomer.Type = 'S';
                                _newCustomer.isEdit = true;
                                _newCustomer.isReNew = true;
                                _newCustomer.TransactionId = resultId;
                                _newCustomer.IsClosed = FirstTime;
                                _newCustomer.ShowDialog();
                                FirstTime = ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                            }
                        }

                    }
                }
                else  //customerObj.PromoteDate == null it means that it's new customer
                {
                    //List<Transaction> transactionList = (from t in entity.Transactions
                    //                                     join c in entity.Customers
                    //                                     on t.CustomerId equals c.Id
                    //                                     where (t.CustomerId == CustomerId) && (c.Id == CustomerId) && (t.IsDeleted == false)
                    //                                     select t).ToList();


                    //foreach (Transaction t in transactionList)
                    //{
                    //    totalAmount += Convert.ToInt32(t.TotalAmount);
                    //}
                    if (totalCost >= Convert.ToInt32(minimumAmountofThisMemberType))
                    {
                        foreach (MemberCardRule memberCard in memberCardRuleList)
                        {
                            if (Convert.ToInt32(memberCard.RangeFrom) <= totalCost)
                            {
                                memberTypeID = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();
                            }
                        }

                        memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                        if (memName != null)
                        {
                            _newCustomer.CustomerId = CustomerId;
                            _newCustomer.MemerTypeName = memName;
                            _newCustomer.getTotalAmount = Convert.ToInt32(totalCost);
                            _newCustomer.Type = 'S';
                            _newCustomer.TransactionId = resultId;
                            _newCustomer.isEdit = true;
                            _newCustomer.IsClosed = FirstTime;
                            _newCustomer.ShowDialog();
                            FirstTime = ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                        }

                    }

                }
            }
            else // name == 'Default'
            {
                if (totalCost >= Convert.ToInt32(minimumAmountofThisMemberType))
                {
                    foreach (MemberCardRule memberCards in memberCardRuleList)
                    {
                        if (Convert.ToInt32(memberCards.RangeFrom) <= totalCost)
                        {
                            memberTypeID = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();
                            //int id = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();

                        }
                    }

                    memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                    if (memName != null)
                    {
                        _newCustomer.CustomerId = CustomerId;
                        _newCustomer.MemerTypeName = memName;
                        _newCustomer.getTotalAmount = Convert.ToInt32(totalCost);
                        _newCustomer.Type = 'S';
                        _newCustomer.TransactionId = resultId;
                        _newCustomer.IsClosed = FirstTime;
                        _newCustomer.ShowDialog();
                        FirstTime = ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                    }


                }
            }



            if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
            {
                Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];

                newForm.Clear();
            }


            #endregion      


            foreach (TransactionDetail detail in DetailList)
            {
                detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                detail.Product.Qty = detail.Product.Qty - detail.Qty;
                PointDeductionPercentage_History pdp = entity.PointDeductionPercentage_History.FirstOrDefault(p => p.Active == true);
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
                detail.IsDeductedBy = pdp != null && detail.DiscountRate > 0 ? pdp.DiscountRate : (decimal?)null;
                insertedTransaction.TransactionDetails.Add(detail);
            }

            //save in stock transaction
            Save_SaleQty_ToStockTransaction(productList);
            productList.Clear();
            insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
            insertedTransaction.IsDeleted = false;
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
                                                        where p.ProductId == pId && p.IsDeleted == false && m.IsCompletedInvoice == true && p.CurrentQy > 0
                                                        orderby p.Date ascending
                                                        select p).ToList();
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


            this.Dispose();
            //MessageBox.Show("Payment Completed");




            //Common cm = new Common();
            //cm.MemberTypeId = MemberTypeId;
            //cm.TotalAmt = totalCost;
            //cm.CustomerId = CustomerId;
            //cm.type = 'S';
            //cm.TransactionId = resultId;
            //cm.Get_MType();

            if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
            {
                Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];

                newForm.Clear();
            }

            //SD
            #region Update IsCalculatedPoint in Transaction And PointHistoryId in Transaction Detail and Insert VIP Customer Table

            VipCustomer vipCustomer = new VipCustomer();
            DateTime lastTwoYears = DateTime.Now.AddYears(-2);
            var _customerId1 = entity1.Transactions.Where(x => x.Id == resultId).Select(x => x.CustomerId).FirstOrDefault();
            var _custList = entity1.Customers.Where(x => x.Id == _customerId1).FirstOrDefault();
            DateTime transactionDate = entity1.Transactions.Where(x => x.Id == resultId).Select(x => x.DateTime.Value).FirstOrDefault();
            if (_custList.MemberTypeID != null)
            {

                int memberTypeID2 = (int)_custList.MemberTypeID;
                var _isCalucatePoint = entity.MemberCardRules.Where(x => x.IsActive == true && x.MemberTypeId == memberTypeID2).Select(x => x.IsCalculatePoints).FirstOrDefault();
                bool isCalucatePoint2 = _isCalucatePoint == null ? false : (bool)_isCalucatePoint;

                ELC_CustomerPointSystem.Update_ForPoint_InTransaction(_custList.FirstTime, _custList.MemberTypeID, resultId, 5, (int)_customerId1, isCalucatePoint2, _custList.Id);
                var vipTransactionlist = entity.Transactions.Where(x => x.IsDeleted == false && x.IsActive == true
                                                 && x.CustomerId == _customerId1 && (x.DateTime >= lastTwoYears && x.DateTime <= DateTime.Now)).ToList();
                if (vipTransactionlist.Count > 0)
                {//to update
                    var totalPaidAmount = (int)(vipTransactionlist.Select(x => x.TotalAmount).Sum() - vipTransactionlist.Select(x => x.GiftCardAmount).Sum());
                    vipCustomer.CustomerCode = _custList.CustomerCode;
                    vipCustomer.LastPaidDate = transactionDate;
                    vipCustomer.TwoYearsTotalAmount = totalPaidAmount;
                    vipCustomer.CreatedUserID = MemberShip.UserId;
                    vipCustomer.CreatedDate = DateTime.Now;
                    vipCustomer.MemberType = _custList.MemberType.Name;
                    vipCustomer.ShopCode = SettingController.DefaultShop.ShortCode;
                    entity.VipCustomers.Add(vipCustomer);
                    entity.SaveChanges();
                }
            }
            #endregion

            //Print Invoice
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
                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; //Edit By ZMH

                    if (transaction.IsFOC == true)
                    {
                        newRow.IsFOC = "FOC";
                    }

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
                reportPath = Application.StartupPath + Utility.GetReportPath("MPU");
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
                //  APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();
                APP_Data.Customer cus = ELC_CustomerPointSystem.Get_CustomerName(resultId);
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

                //ReportParameter TotalAmount = new ReportParameter("TotalAmount", insertedTransaction.TotalAmount.ToString()); //Edit By ZMH
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

                ReportParameter PaidAmount = new ReportParameter("PaidAmount", totalCost.ToString());
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

                ////PrintDoc.PrintReport(rv, Utility.GetDefaultPrinter());
                Utility.Get_Print(rv);

            }
            #endregion
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

