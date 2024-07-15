using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using POS.APP_Data;

namespace POS
{
    public partial class PaidByCreditWithPrePaidDebt : Form
    {
        #region Variables
        long PayDebtAmount = 0;
        bool FirstTime = false;
        decimal _prePaidDebt = 0, _outstandingPreBalance = 0;
        Transaction insertedTransaction = new Transaction();
        public List<TransactionDetail> DetailList = new List<TransactionDetail>();

        public string _customerName = "";

        public int ReceiveAmount = 0;

        public string DId { get; set; }

        public int Discount { get; set; }

        public int Tax { get; set; }

        public int ExtraDiscount { get; set; }

        public int ExtraTax { get; set; }

        public Boolean isDraft { get; set; }

        public string DraftId { get; set; }

        public Boolean IsWholeSale { get; set; }

        public int CustomerId { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        //private long outstandingBalance = 0;
        long OldOutstandingAmount = 0;
        int PrepaidDebt = 0;

        public decimal BDDiscount { get; set; }

        public decimal MCDiscount { get; set; }

        public int? MemberTypeId { get; set; }

        public decimal? MCDiscountPercent { get; set; }

        public DialogResult _result;

        public decimal TotalAmt = 0;
        public int _TotalAmt = 0;

        string resultId;

        int Qty = 0;

        List<Stock_Transaction> productList = new List<Stock_Transaction>();

       public  Boolean IsPrint = false;
        #endregion

        #region Event
        public PaidByCreditWithPrePaidDebt()
        {
            InitializeComponent();
        }
       
        #region Hot keys handler
        void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)      //  Ctrl + N => Focus CustomerName
            {
                cboCustomerList.DroppedDown = true;
                if (cboCustomerList.Focused != true)
                {
                    cboCustomerList.Focus();
                }
            }
            else if (e.Control && e.KeyCode == Keys.R)      // Ctrl + R => Focus Receive Amt
            {
                cboCustomerList.DroppedDown = false;
                txtReceiveAmount.Focus();
            }
            else if (e.Control && e.KeyCode == Keys.S)      // Ctrl + S => Click Save
            {
                cboCustomerList.DroppedDown = false;
                btnSubmit.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.C)      // Ctrl + C => Focus DropDown Customer
            {
                cboCustomerList.DroppedDown = false;
                btnCancel.PerformClick();
            }
        }
        #endregion
        private void PaidByCreditWithPrePaidDebt_Load(object sender, EventArgs e)
        {
          
            #region Setting Hot Kyes For the Controls
            SendKeys.Send("%"); SendKeys.Send("%"); // Clicking "Alt" on page load to show underline of Hot Keys
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form_KeyDown);
            #endregion

            List<Customer> CustomerList = new List<Customer>();
            Customer none = new Customer();
            none.Name = "Select Customer";
            none.Id = 0;
            CustomerList.Add(none);
            CustomerList.AddRange(entity.Customers.ToList());
            cboCustomerList.DataSource = CustomerList;
            cboCustomerList.DisplayMember = "Name";
            cboCustomerList.ValueMember = "Id";
            cboCustomerList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCustomerList.AutoCompleteSource = AutoCompleteSource.ListItems;


       
            if (CustomerId != null)
            {
                cboCustomerList.Text = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault().Name;
            }

            int previouseBalance=Convert.ToInt32(lblPreviousBalance.Text);
            //lblTotalCost.Text = (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax - MCDiscount - BDDiscount + previouseBalance).ToString();
            lblAccuralCost.Text = (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount  - MCDiscount - BDDiscount).ToString();
            IsPrePaidCheck();
            //lblNetPayable.Text = (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax - MCDiscount - BDDiscount + previouseBalance).ToString();
            TotalAmt = Convert.ToDecimal(lblTotalCost.Text);
            _TotalAmt =Convert.ToInt32( lblAccuralCost.Text);
            txtReceiveAmount.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation
            if (cboCustomerList.SelectedIndex == 0)
            {
                tp.SetToolTip(cboCustomerList, "Error");
                tp.Show("Please select customer name!", cboCustomerList);
                hasError = true;
            }

            else if (txtReceiveAmount.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtReceiveAmount, "Error");
                tp.Show("Please fill up receive amount!", txtReceiveAmount);
                hasError = true;
            }

            else if (Convert.ToInt32(txtReceiveAmount.Text) >= Convert.ToInt32(lblTotalCost.Text))
            {
                DialogResult result =  MessageBox.Show("Receive Amount is greater than Total cost. Are you sure want to change Cash Type! ","mPOS",MessageBoxButtons.OKCancel);
                if (result.Equals(DialogResult.OK))
                {
                    hasError = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Receive Amount should be less than Total cost.");
                    txtReceiveAmount.Focus();
                    hasError = true;
                }
                
            }

            if (!hasError)
            {
                long totalAmount = 0; long totalCost = 0; long receiveAmount = 0;
                Int64.TryParse(txtReceiveAmount.Text, out receiveAmount);

                ReceiveAmount = Convert.ToInt32(txtReceiveAmount.Text) ;

                _customerName = cboCustomerList.Text;
                _prePaidDebt = Convert.ToDecimal(lblPrePaid.Text);
                _outstandingPreBalance = Convert.ToDecimal(lblPreviousBalance.Text);

                totalCost = (long)(DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (long)MCDiscount- (long)BDDiscount);
     
                long unitpriceTotalCost = (long)DetailList.Sum(x => x.UnitPrice * x.Qty);//Edit ZMH


                PayDebtAmount = 0;
                Int64.TryParse(lblPrePaid.Text, out PayDebtAmount);
                totalAmount = receiveAmount;
                if (lblNetPayableTitle.Text == "Change")
                {
                    totalAmount -= Convert.ToInt64(lblNetPayable.Text);
                    receiveAmount -= Convert.ToInt64(lblNetPayable.Text);
                }
                if (chkIsPrePaid.Checked)
                {
                    totalAmount += PayDebtAmount;
                }
                int customerId = 0;
                Int32.TryParse(cboCustomerList.SelectedValue.ToString(), out customerId);

                //set old credit transaction record to paid coz this transaction store old outstanding amount
                Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();
                List<Transaction> transList = cust.Transactions.Where(unpaid => unpaid.IsPaid == false).ToList();
                List<Transaction> prePaidTranList = cust.Transactions.Where(type => type.Type == TransactionType.Prepaid).Where(active => active.IsActive == false).ToList();
                //foreach (Transaction ts in transList)
                //{
                //    //ts.IsPaid = true;
                //}               

                //insert credit Transaction
                System.Data.Objects.ObjectResult<string> Id;
                insertedTransaction = new Transaction();
              
                #region add sale, debt, prepaid transaction when receiveamount is greater than totalCost
                if (receiveAmount > totalCost)
                {
                    //Updated by Lele
                    //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Utility.PaymentTypeID.Cash, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount)  - ExtraDiscount, totalCost, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,true);
                    //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Utility.PaymentTypeID.Cash, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount, totalCost, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, true);
                    //Updated by Yimon
                    Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Utility.PaymentTypeID.Cash, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount, totalCost, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, true,"","");
                    resultId = Id.FirstOrDefault().ToString();
                    insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();


                    foreach (TransactionDetail detail in DetailList)
                    {
                        detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                        detail.Product.Qty = detail.Product.Qty - detail.Qty;
                        

                        //save in stocktransaction
                    
                        Stock_Transaction st = new Stock_Transaction();
                        st.ProductId = detail.Product.Id;
                        Qty = Convert.ToInt32(detail.Qty);
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
                                    wpObj.Qty = wpObj.Qty - detail.Qty;
                                }
                            }
                        }
                        detail.IsDeleted = false;
                        detail.IsConsignmentPaid = Utility.IsConsignmentPaid(detail.Product);
                        insertedTransaction.TransactionDetails.Add(detail);
                    }
                    //save in stocktransaction
                    Save_SaleQty_ToStockTransaction(productList);
                    productList.Clear();
                    insertedTransaction.IsDeleted = false;
                    totalAmount -= totalCost;
                    receiveAmount -= totalCost;
                    //bool prepaidCredit = false;
                    insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                    entity.SaveChanges();


                    #region purchase
                    //// for Purchase Detail and PurchaseDetailInTransacton.

                    //foreach (TransactionDetail detail in insertedTransaction.TransactionDetails)
                    //{
                    //    int Qty = Convert.ToInt32(detail.Qty);
                    //    int pId = Convert.ToInt32(detail.ProductId);

                    //    // Get purchase detail with same product Id and order by purchase date ascending
                    //    List<APP_Data.PurchaseDetail> pulist = (from p in entity.PurchaseDetails
                    //                                            join m in entity.MainPurchases on p.MainPurchaseId equals m.Id
                    //                                            where p.ProductId == pId && p.IsDeleted == false && m.IsCompletedInvoice==true && p.CurrentQy > 0 orderby p.Date ascending select p).ToList();

                    //    if (pulist.Count > 0)
                    //    {
                    //        int TotalQty = Convert.ToInt32(pulist.Sum(x => x.CurrentQy));

                    //        if (TotalQty >= Qty)
                    //        {
                    //            foreach (PurchaseDetail p in pulist)
                    //            {
                    //                if (Qty > 0)
                    //                {
                    //                    if (p.CurrentQy >= Qty)
                    //                    {
                    //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                    //                        pdObjInTran.ProductId = pId;
                    //                        pdObjInTran.TransactionDetailId = detail.Id;
                    //                        pdObjInTran.PurchaseDetailId = p.Id;
                    //                        pdObjInTran.Date = p.Date;
                    //                        pdObjInTran.Qty = Qty;
                    //                        p.CurrentQy = p.CurrentQy - Qty;
                    //                        Qty = 0;

                    //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                    //                        entity.Entry(p).State = EntityState.Modified;
                    //                        entity.SaveChanges();
                    //                        break;
                    //                    }
                    //                    else if (p.CurrentQy <= Qty)
                    //                    {
                    //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                    //                        pdObjInTran.ProductId = pId;
                    //                        pdObjInTran.TransactionDetailId = detail.Id;
                    //                        pdObjInTran.PurchaseDetailId = p.Id;
                    //                        pdObjInTran.Date = p.Date;
                    //                        pdObjInTran.Qty = p.CurrentQy;

                    //                        Qty = Convert.ToInt32(Qty - p.CurrentQy);
                    //                        p.CurrentQy = 0;
                    //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                    //                        entity.Entry(p).State = EntityState.Modified;
                    //                        entity.SaveChanges();
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    #region current transation paid and remain amount is paid other transaction
                    if (totalAmount != 0)
                    {
                        //prepaidCredit = true;
                        long tamount = 0;
                        long differentAmount = totalAmount;
                        long DebtAmount = 0;
                        List<Transaction> tList = new List<Transaction>();
                        List<Transaction> RefundList = new List<Transaction>();
                        foreach (Transaction t in transList)
                        {
                            tamount = (long)t.TotalAmount - (long)t.RecieveAmount;
                            RefundList = (from tr in entity.Transactions where tr.ParentId == t.Id && tr.Type == TransactionType.CreditRefund select tr).ToList();
                            if (RefundList.Count > 0)
                            {
                                foreach (Transaction TRefund in RefundList)
                                {
                                    tamount -= (long)TRefund.RecieveAmount;
                                }
                            }
                            if (tamount <= differentAmount)
                            {
                                tList.Add(t);
                                differentAmount -= tamount;
                            }

                        }
                        int index = tList.Count;
                        for (int outer = index - 1; outer >= 1; outer--)
                        {
                            for (int inner = 0; inner < outer; inner++)
                            {
                                if (tList[inner].TotalAmount - tList[inner].RecieveAmount < tList[inner + 1].TotalAmount - tList[inner + 1].RecieveAmount)
                                {
                                    Transaction t = tList[inner];
                                    tList[inner] = tList[inner + 1];
                                    tList[inner + 1] = t;
                                }
                            }
                        }

                        if (tList.Count > 0)
                        {
                            foreach (Transaction t in tList)
                            {
                                long CreditAmount = 0;
                                CreditAmount = (long)t.TotalAmount - (long)t.RecieveAmount;
                                RefundList = (from tr in entity.Transactions where tr.ParentId == t.Id && tr.Type == TransactionType.CreditRefund select tr).ToList();
                                if (RefundList.Count > 0)
                                {
                                    foreach (Transaction TRefund in RefundList)
                                    {
                                        CreditAmount -= (long)TRefund.RecieveAmount;
                                    }
                                }
                                if (CreditAmount <= totalAmount)
                                {
                                    Transaction CreditT = (from tr in entity.Transactions where tr.Id == t.Id select tr).FirstOrDefault<Transaction>();
                                    CreditT.IsPaid = true;
                                    entity.Entry(CreditT).State = EntityState.Modified;
                                    entity.SaveChanges();
                                    totalAmount -= CreditAmount;
                                    if (CreditAmount <= receiveAmount)
                                    {
                                        DebtAmount += CreditAmount;
                                        receiveAmount -= CreditAmount;
                                    }
                                    else
                                    {
                                        CreditAmount -= receiveAmount;
                                        DebtAmount += receiveAmount;
                                        receiveAmount = 0;
                                        foreach (Transaction PrePaidDebtTrans in prePaidTranList)
                                        {
                                            long PrePaidamount = 0;

                                            int useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                            PrePaidamount = (long)PrePaidDebtTrans.TotalAmount - useAmount;
                                            if (CreditAmount >= PrePaidamount)
                                            {
                                                PrePaidDebtTrans.IsActive = true;
                                                UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                                usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                                usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                                usePrePaidDObj.CreditTransactionId = t.Id;
                                                usePrePaidDObj.CashierId = MemberShip.UserId;
                                                usePrePaidDObj.CounterId = MemberShip.CounterId;
                                                entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                                entity.SaveChanges();
                                                CreditAmount -= PrePaidamount;
                                            }
                                            else
                                            {
                                                UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                                usePrePaidDObj.UseAmount = (int)CreditAmount;
                                                usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                                usePrePaidDObj.CreditTransactionId = t.Id;
                                                usePrePaidDObj.CashierId = MemberShip.UserId;
                                                usePrePaidDObj.CounterId = MemberShip.CounterId;
                                                entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                                entity.SaveChanges();
                                                //CreditAmount -= PrePaidamount;
                                                CreditAmount = 0;
                                            }
                                        }

                                    }

                                    prePaidTranList = (from PDT in entity.Transactions where PDT.Type == TransactionType.Prepaid && PDT.IsActive == false select PDT).ToList();
                                }
                                
                            }
                            if (DebtAmount > 0)
                            {
                                //Updated by Lele
                                //System.Data.Objects.ObjectResult<string> DebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Settlement, true, true, 1, 0, 0, DebtAmount, DebtAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,false);
                                //System.Data.Objects.ObjectResult<string> DebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Settlement, true, true, 1, 0, 0, DebtAmount, DebtAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false);
                                //Updated by Yimon
                                System.Data.Objects.ObjectResult<string> DebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Settlement, true, true, 1, 0, 0, DebtAmount, DebtAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false,"","");
                                entity.SaveChanges();
                            }
                        }
                        else
                        {
                            totalAmount -= PrepaidDebt;
                            receiveAmount -= PrepaidDebt;
                            //System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.PrepaidDebt, true, false, 1, 0, 0, totalAmount, totalAmount, null, customerId);
                            //entity.SaveChanges();
                        }

                    }
                    #endregion

                    if (receiveAmount > 0)
                    {
                        //Updated by Lele
                        //System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Prepaid, true, false, 1, 0, 0, receiveAmount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, false);
                        //System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Prepaid, true, false, 1, 0, 0, receiveAmount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false);
                        //Updated by Yimon
                        System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Prepaid, true, false, 1, 0, 0, receiveAmount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false,"","");
                        entity.SaveChanges();
                    }
                }
                #endregion

                #region add credit Transaction
                else
                {
                    if (totalAmount >= totalCost)
                    {
                        if (chkIsPrePaid.Checked == true)
                        {
                            //Updated by Lele
                            //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount)  - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,false);
                            //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false);
                            //Updated by Yimon
                            Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false,"","");

                            totalAmount -= receiveAmount;
                            totalCost -= receiveAmount;
                            resultId = Id.FirstOrDefault().ToString();
                            insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                            foreach (TransactionDetail detail in DetailList)
                            {
                                detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                                detail.Product.Qty = detail.Product.Qty - detail.Qty;
                                detail.IsDeleted = false;

                                if (detail.ConsignmentPrice == null)
                                {
                                    detail.ConsignmentPrice = 0;
                                }
                                detail.IsConsignmentPaid = Utility.IsConsignmentPaid(detail.Product);
                                insertedTransaction.TransactionDetails.Add(detail);



                                //save in stocktransaction

                                Stock_Transaction st = new Stock_Transaction();
                                st.ProductId = detail.Product.Id;
                                Qty = Convert.ToInt32(detail.Qty);
                                st.Sale = Qty;
                                productList.Add(st);
                                Qty = 0;
                            }
                            insertedTransaction.IsDeleted = false;
                            insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                            entity.SaveChanges();

                            //save in stocktransaction
                            Save_SaleQty_ToStockTransaction(productList);
                            #region purchase
                            // for Purchase Detail and PurchaseDetailInTransacton.

                            //foreach (TransactionDetail detail in DetailList)
                            //{

                            //    int Qty = Convert.ToInt32(detail.Qty);
                            //    int pId = Convert.ToInt32(detail.ProductId);


                            //    // Get purchase detail with same product Id and order by purchase date ascending
                            //    List<APP_Data.PurchaseDetail> pulist = (from p in entity.PurchaseDetails where p.ProductId == pId && p.IsDeleted == false && p.CurrentQy > 0 orderby p.Date ascending select p).ToList();

                            //    if (pulist.Count > 0)
                            //    {
                            //        int TotalQty = Convert.ToInt32(pulist.Sum(x => x.CurrentQy));

                            //        if (TotalQty >= Qty)
                            //        {
                            //            foreach (PurchaseDetail p in pulist)
                            //            {
                            //                if (Qty > 0)
                            //                {
                            //                    if (p.CurrentQy >= Qty)
                            //                    {
                            //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                            //                        pdObjInTran.ProductId = pId;
                            //                        pdObjInTran.TransactionDetailId = detail.Id;
                            //                        pdObjInTran.PurchaseDetailId = p.Id;
                            //                        pdObjInTran.Date = p.Date;
                            //                        pdObjInTran.Qty = Qty;
                            //                        p.CurrentQy = p.CurrentQy - Qty;
                            //                        Qty = 0;

                            //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                            //                        entity.Entry(p).State = EntityState.Modified;
                            //                        entity.SaveChanges();
                            //                        break;
                            //                    }
                            //                    else if (p.CurrentQy <= Qty)
                            //                    {
                            //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                            //                        pdObjInTran.ProductId = pId;
                            //                        pdObjInTran.TransactionDetailId = detail.Id;
                            //                        pdObjInTran.PurchaseDetailId = p.Id;
                            //                        pdObjInTran.Date = p.Date;
                            //                        pdObjInTran.Qty = p.CurrentQy;

                            //                        Qty = Convert.ToInt32(Qty - p.CurrentQy);
                            //                        p.CurrentQy = 0;
                            //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                            //                        entity.Entry(p).State = EntityState.Modified;
                            //                        entity.SaveChanges();
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion



                            foreach (Transaction PrePaidDebtTrans in prePaidTranList)
                            {
                                long PrePaidamount = 0;
                                //int useAmount = 0;
                                int useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                PrePaidamount = (long)PrePaidDebtTrans.TotalAmount - useAmount;
                                if (totalCost >= PrePaidamount)
                                {
                                    PrePaidDebtTrans.IsActive = true;
                                    totalCost -= PrePaidamount;
                                    totalAmount -= PrePaidamount;
                                    entity.SaveChanges();
                                    UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                    usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                    usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                    usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                    usePrePaidDObj.CashierId = MemberShip.UserId;
                                    usePrePaidDObj.CounterId = MemberShip.CounterId;
                                    entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                    entity.SaveChanges();
                                }
                                else
                                {
                                    UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                    usePrePaidDObj.UseAmount = (int)totalCost;
                                    usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                    usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                    usePrePaidDObj.CashierId = MemberShip.UserId;
                                    usePrePaidDObj.CounterId = MemberShip.CounterId;
                                    entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                    entity.SaveChanges();
                                    totalAmount -= totalCost;
                                    totalCost = 0;
                                }

                            }
                        }
                        else
                        {
                            if (chkIsPrePaid.Checked == true)
                            {
                                //Updated by Lele
                                //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount)  - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,false);
                                //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, 0,0,false,false);
                                //Updated by Yimon
                                Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, 0,0,false,false, "","");

                                totalAmount -= receiveAmount;
                                totalCost -= receiveAmount;
                                resultId = Id.FirstOrDefault().ToString();
                                insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                                foreach (TransactionDetail detail in DetailList)
                                {
                                    detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                                    detail.Product.Qty = detail.Product.Qty - detail.Qty;
                                    detail.IsDeleted = false;
                                    detail.IsConsignmentPaid = Utility.IsConsignmentPaid(detail.Product);
                                    insertedTransaction.TransactionDetails.Add(detail);


                                    //save in stocktransaction

                                    Stock_Transaction st = new Stock_Transaction();
                                    st.ProductId = detail.Product.Id;
                                    Qty = Convert.ToInt32(detail.Qty);
                                    st.Sale = Qty;
                                    productList.Add(st);
                                    Qty = 0;
                                }
                                insertedTransaction.IsDeleted = false;
                                insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                                entity.SaveChanges();

                                //save in stock transaction
                                Save_SaleQty_ToStockTransaction(productList);

                                #region purchase
                                // for Purchase Detail and PurchaseDetailInTransacton.

                                //foreach (TransactionDetail detail in DetailList)
                                //{

                                //    int Qty = Convert.ToInt32(detail.Qty);
                                //    int pId = Convert.ToInt32(detail.ProductId);


                                //    // Get purchase detail with same product Id and order by purchase date ascending
                                //    List<APP_Data.PurchaseDetail> pulist = (from p in entity.PurchaseDetails where p.ProductId == pId && p.IsDeleted == false && p.CurrentQy > 0 orderby p.Date ascending select p).ToList();

                                //    if (pulist.Count > 0)
                                //    {
                                //        int TotalQty = Convert.ToInt32(pulist.Sum(x => x.CurrentQy));

                                //        if (TotalQty >= Qty)
                                //        {
                                //            foreach (PurchaseDetail p in pulist)
                                //            {
                                //                if (Qty > 0)
                                //                {
                                //                    if (p.CurrentQy >= Qty)
                                //                    {
                                //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                                //                        pdObjInTran.ProductId = pId;
                                //                        pdObjInTran.TransactionDetailId = detail.Id;
                                //                        pdObjInTran.PurchaseDetailId = p.Id;
                                //                        pdObjInTran.Date = p.Date;
                                //                        pdObjInTran.Qty = Qty;
                                //                        p.CurrentQy = p.CurrentQy - Qty;
                                //                        Qty = 0;

                                //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                                //                        entity.Entry(p).State = EntityState.Modified;
                                //                        entity.SaveChanges();
                                //                        break;
                                //                    }
                                //                    else if (p.CurrentQy <= Qty)
                                //                    {
                                //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                                //                        pdObjInTran.ProductId = pId;
                                //                        pdObjInTran.TransactionDetailId = detail.Id;
                                //                        pdObjInTran.PurchaseDetailId = p.Id;
                                //                        pdObjInTran.Date = p.Date;
                                //                        pdObjInTran.Qty = p.CurrentQy;

                                //                        Qty = Convert.ToInt32(Qty - p.CurrentQy);
                                //                        p.CurrentQy = 0;
                                //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                                //                        entity.Entry(p).State = EntityState.Modified;
                                //                        entity.SaveChanges();
                                //                    }
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
                                #endregion

                                foreach (Transaction PrePaidDebtTrans in prePaidTranList)
                                {
                                    long PrePaidamount = 0;
                                    //int useAmount = 0;
                                    int useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                    PrePaidamount = (long)PrePaidDebtTrans.TotalAmount - useAmount;
                                    if (totalCost >= PrePaidamount)
                                    {
                                        PrePaidDebtTrans.IsActive = true;
                                        totalCost -= PrePaidamount;
                                        totalAmount -= PrePaidamount;
                                        entity.SaveChanges();
                                        UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                        usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                        usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                        usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                        usePrePaidDObj.CashierId = MemberShip.UserId;
                                        usePrePaidDObj.CounterId = MemberShip.CounterId;
                                        entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                        entity.SaveChanges();
                                    }
                                    else
                                    {
                                        UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                        usePrePaidDObj.UseAmount = (int)totalCost;
                                        usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                        usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                        usePrePaidDObj.CashierId = MemberShip.UserId;
                                        usePrePaidDObj.CounterId = MemberShip.CounterId;
                                        entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                        entity.SaveChanges();
                                        totalAmount -= totalCost;
                                        totalCost = 0;
                                    }

                                }
                            }
                            else
                            {
                                //Updated by Lele
                                //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount)  - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,false);
                                //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false);
                                //Updated by Yimon
                                Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false,"","");
                                totalAmount -= receiveAmount;
                                totalCost -= receiveAmount;
                                resultId = Id.FirstOrDefault().ToString();
                                insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                                foreach (TransactionDetail detail in DetailList)
                                {
                                    detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                                    detail.Product.Qty = detail.Product.Qty - detail.Qty;
                                    detail.IsDeleted = false;
                                    detail.IsConsignmentPaid = Utility.IsConsignmentPaid(detail.Product);
                                    insertedTransaction.TransactionDetails.Add(detail);


                                    //save in stocktransaction

                                    Stock_Transaction st = new Stock_Transaction();
                                    st.ProductId = detail.Product.Id;
                                    Qty = Convert.ToInt32(detail.Qty);
                                    st.Sale = Qty;
                                    productList.Add(st);
                                    Qty = 0;
                                }
                                insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                                entity.SaveChanges();

                                //save in stock transaction
                                Save_SaleQty_ToStockTransaction(productList);
                                #region purchase
                                // for Purchase Detail and PurchaseDetailInTransacton.

                                //foreach (TransactionDetail detail in DetailList)
                                //{

                                //    int Qty = Convert.ToInt32(detail.Qty);
                                //    int pId = Convert.ToInt32(detail.ProductId);
                                // // Get purchase detail with same product Id and order by purchase date ascending
                                //    List<APP_Data.PurchaseDetail> pulist = (from p in entity.PurchaseDetails where p.ProductId == pId && p.IsDeleted == false && p.CurrentQy > 0 orderby p.Date ascending select p).ToList();
                                //   if (pulist.Count > 0)
                                //    {
                                //        int TotalQty = Convert.ToInt32(pulist.Sum(x => x.CurrentQy));
                                //        if (TotalQty >= Qty)
                                //        {
                                //            foreach (PurchaseDetail p in pulist)
                                //            {
                                //                if (Qty > 0)
                                //                {
                                //                    if (p.CurrentQy >= Qty)
                                //                    {
                                //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                                //                        pdObjInTran.ProductId = pId;
                                //                        pdObjInTran.TransactionDetailId = detail.Id;
                                //                        pdObjInTran.PurchaseDetailId = p.Id;
                                //                        pdObjInTran.Date = p.Date;
                                //                        pdObjInTran.Qty = Qty;
                                //                        p.CurrentQy = p.CurrentQy - Qty;
                                //                        Qty = 0;

                                //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                                //                        entity.Entry(p).State = EntityState.Modified;
                                //                        entity.SaveChanges();
                                //                        break;
                                //                    }
                                //                    else if (p.CurrentQy <= Qty)
                                //                    {
                                //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                                //                        pdObjInTran.ProductId = pId;
                                //                        pdObjInTran.TransactionDetailId = detail.Id;
                                //                        pdObjInTran.PurchaseDetailId = p.Id;
                                //                        pdObjInTran.Date = p.Date;
                                //                        pdObjInTran.Qty = p.CurrentQy;

                                //                        Qty = Convert.ToInt32(Qty - p.CurrentQy);
                                //                        p.CurrentQy = 0;
                                //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                                //                        entity.Entry(p).State = EntityState.Modified;
                                //                        entity.SaveChanges();
                                //                    }
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
                                #endregion
                            }
                        }
                        //if (totalAmount > 0)
                        //{
                        //    System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.PrepaidDebt, true, false, 1, 0, 0, totalAmount, totalAmount, null, customerId);
                        //    entity.SaveChanges();
                        //}
                    }
                    else
                    {
                        if (chkIsPrePaid.Checked == true)
                        {
                            //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount, receiveAmount, null, customerId);
                            //Updated by Lele
                            //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,false);
                            //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false);
                            //Updated by Yimon
                            Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, receiveAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false,"","");
                            totalAmount -= receiveAmount;
                            totalCost -= receiveAmount;
                            resultId = Id.FirstOrDefault().ToString();
                            insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                            foreach (TransactionDetail detail in DetailList)
                            {
                                detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                                detail.Product.Qty = detail.Product.Qty - detail.Qty;
                                detail.IsDeleted = false;
                                detail.IsConsignmentPaid = Utility.IsConsignmentPaid(detail.Product);
                                insertedTransaction.TransactionDetails.Add(detail);

                                //save in stocktransaction

                                Stock_Transaction st = new Stock_Transaction();
                                st.ProductId = detail.Product.Id;
                                Qty = Convert.ToInt32(detail.Qty);
                                st.Sale = Qty;
                                productList.Add(st);
                                Qty = 0;
                            }
                            insertedTransaction.IsDeleted = false;
                            insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                            entity.SaveChanges();

                            //save in stock transaction
                            Save_SaleQty_ToStockTransaction(productList);

                            foreach (Transaction PrePaidDebtTrans in prePaidTranList)
                            {
                                long PrePaidamount = 0;
                                //int useAmount = 0;
                                int useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                PrePaidamount = (long)PrePaidDebtTrans.TotalAmount - useAmount;
                                if (totalCost >= PrePaidamount)
                                {
                                    PrePaidDebtTrans.IsActive = true;
                                    totalCost -= PrePaidamount;
                                    totalAmount -= PrePaidamount;
                                    entity.SaveChanges();
                                    UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                    usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                    usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                    usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                    usePrePaidDObj.CashierId = MemberShip.UserId;
                                    usePrePaidDObj.CounterId = MemberShip.CounterId;
                                    entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                    entity.SaveChanges();
                                }
                                else
                                {
                                    UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                    usePrePaidDObj.UseAmount = (int)totalCost;
                                    usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                    usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                    usePrePaidDObj.CashierId = MemberShip.UserId;
                                    usePrePaidDObj.CounterId = MemberShip.CounterId;
                                    entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                    entity.SaveChanges();
                                    totalAmount -= totalCost;
                                    totalCost = 0;
                                }

                            }
                        }
                        else
                        {
                            //Updated by Lele
                            //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, totalAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,false);
                            //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, totalAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, 0,0,false,false);
                            //Updated by Yimon
                            Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (int)MCDiscount - (int)BDDiscount, totalAmount, null, customerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, 0,0,false,false,"","");
                            resultId = Id.FirstOrDefault().ToString();
                            insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();


                            foreach (TransactionDetail detail in DetailList)
                            {
                                detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                                detail.Product.Qty = detail.Product.Qty - detail.Qty;

                                //save in stocktransaction

                                Stock_Transaction st = new Stock_Transaction();
                                st.ProductId = detail.Product.Id;
                                Qty = Convert.ToInt32(detail.Qty);
                                st.Sale = Qty;
                                productList.Add(st);
                                Qty = 0;

                                detail.IsDeleted = false;
                                detail.IsConsignmentPaid = Utility.IsConsignmentPaid(detail.Product);
                                insertedTransaction.TransactionDetails.Add(detail);
                            }
                            insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                            insertedTransaction.IsDeleted = false;
                            entity.SaveChanges();

                            //save in stock transaction
                            Save_SaleQty_ToStockTransaction(productList);

                            #region purchase
                            // for Purchase Detail and PurchaseDetailInTransacton.

                            //foreach (TransactionDetail detail in DetailList)
                            //{

                            //    int Qty = Convert.ToInt32(detail.Qty);
                            //    int pId = Convert.ToInt32(detail.ProductId);


                            //    // Get purchase detail with same product Id and order by purchase date ascending
                            //    List<APP_Data.PurchaseDetail> pulist = (from p in entity.PurchaseDetails where p.ProductId == pId && p.IsDeleted == false && p.CurrentQy > 0 orderby p.Date ascending select p).ToList();

                            //    if (pulist.Count > 0)
                            //    {
                            //        int TotalQty = Convert.ToInt32(pulist.Sum(x => x.CurrentQy));

                            //        if (TotalQty >= Qty)
                            //        {
                            //            foreach (PurchaseDetail p in pulist)
                            //            {
                            //                if (Qty > 0)
                            //                {
                            //                    if (p.CurrentQy >= Qty)
                            //                    {
                            //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                            //                        pdObjInTran.ProductId = pId;
                            //                        pdObjInTran.TransactionDetailId = detail.Id;
                            //                        pdObjInTran.PurchaseDetailId = p.Id;
                            //                        pdObjInTran.Date = p.Date;
                            //                        pdObjInTran.Qty = Qty;
                            //                        p.CurrentQy = p.CurrentQy - Qty;
                            //                        Qty = 0;

                            //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                            //                        entity.Entry(p).State = EntityState.Modified;
                            //                        entity.SaveChanges();
                            //                        break;
                            //                    }
                            //                    else if (p.CurrentQy <= Qty)
                            //                    {
                            //                        PurchaseDetailInTransaction pdObjInTran = new PurchaseDetailInTransaction();
                            //                        pdObjInTran.ProductId = pId;
                            //                        pdObjInTran.TransactionDetailId = detail.Id;
                            //                        pdObjInTran.PurchaseDetailId = p.Id;
                            //                        pdObjInTran.Date = p.Date;
                            //                        pdObjInTran.Qty = p.CurrentQy;

                            //                        Qty = Convert.ToInt32(Qty - p.CurrentQy);
                            //                        p.CurrentQy = 0;
                            //                        entity.PurchaseDetailInTransactions.Add(pdObjInTran);
                            //                        entity.Entry(p).State = EntityState.Modified;
                            //                        entity.SaveChanges();
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion
                        }
                    }
                }
                #endregion

                #region purchase
                // for Purchase Detail and PurchaseDetailInTransacton.

                foreach (TransactionDetail detail in DetailList)
                {

                    int Qty = Convert.ToInt32(detail.Qty);
                    int pId = Convert.ToInt32(detail.ProductId);


                    // Get purchase detail with same product Id and order by purchase date ascending
                    List<APP_Data.PurchaseDetail> pulist = (from p in entity.PurchaseDetails where p.ProductId == pId && p.IsDeleted == false && p.CurrentQy > 0 orderby p.Date ascending select p).ToList();

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

                CustomerId=Convert.ToInt32(cboCustomerList.SelectedValue);

                
                _result = MessageShow();
                if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                {
                    Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
              
                    newForm.Clear();
                }
                this.Dispose();
            }

            if (_result.Equals(DialogResult.OK))
            {
                //Common cm = new Common();
                //cm.MemberTypeId = MemberTypeId;
                //cm.TotalAmt = TotalAmt;
                //cm.CustomerId = CustomerId;
                //cm.type = 'S';
                //cm.TransactionId = resultId;
                //cm.Get_MType();

                #region Referral Point for Referral Customer
                POSEntities entity1 = new POSEntities();
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
                            if (customerObj.MemberTypeID != mTypeID) //if selected customer is not the greatest
                            {
                                List<Transaction> transactionList = (from t in entity.Transactions
                                                                     join c in entity.Customers
                                                                     on t.CustomerId equals c.Id
                                                                     where (t.CustomerId == CustomerId) && (c.Id == CustomerId) && (t.DateTime >= c.StartDate)
                                                                     && (t.DateTime <= c.ExpireDate) && (t.IsDeleted == false) && (t.IsComplete == true)
                                                                     && (t.PaymentTypeId == 1 || t.PaymentTypeId == 2 || t.PaymentTypeId == 3 || t.PaymentTypeId == 5 || t.PaymentTypeId == null)
                                                                     && (t.Type != "Prepaid") && (t.Type != "Settlement") && (t.Type != "Refund") && (t.Type != "CreditRefund")
                                                                     select t).ToList();

                                List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                       join transactions in entity.Transactions
                                                                                       on td.TransactionId equals transactions.ParentId
                                                                                       join c in entity.Customers
                                                                                       on transactions.CustomerId equals c.Id
                                                                                       where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
                                                                                       && (transactions.IsDeleted == false) && (transactions.IsComplete == true)
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

                                foreach (TransactionDetail td in RefundTransactionDetailList)
                                {
                                    totalRAmount += Convert.ToInt32(td.TotalAmount);
                                }
                                totalAmount = totalAmount - totalRAmount;
                                totalAmount = totalAmount - giftCardTotalAmount;
                                totalAmount = totalAmount - couponCodeTotalAmount;//Added by Lele
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
                                    _newCustomer.CustomerId = CustomerId;
                                    _newCustomer.MemerTypeName = memName;
                                    _newCustomer.getTotalAmount = Convert.ToInt32(_TotalAmt);
                                    _newCustomer.Type = 'S';
                                    _newCustomer.isEdit = true;
                                    _newCustomer.TransactionId = resultId;
                                    _newCustomer.IsClosed = FirstTime;
                                    _newCustomer.ShowDialog();
                                    FirstTime=ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                                    
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
                                                                     && (transactions.DateTime <= _expireDate) && (transactions.IsDeleted == false) && (transactions.IsComplete == true)
                                                                     && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                     && (transactions.Type != "Refund") && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
                                                                     select transactions).ToList();

                                List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                       join transactions in entity.Transactions
                                                                                       on td.TransactionId equals transactions.ParentId
                                                                                       join c in entity.Customers
                                                                                       on transactions.CustomerId equals c.Id
                                                                                       where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.IsDeleted == false)
                                                                                       && (transactions.IsComplete == true) && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
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
                                totalAmount = totalAmount - giftCardTotalAmount;
                                totalAmount = totalAmount - (giftCardTotalAmount + couponCodeTotalAmount);

                                foreach (TransactionDetail td in RefundTransactionDetailList)
                                {
                                    totalRAmount += Convert.ToInt32(td.TotalAmount);
                                }
                                totalAmount = totalAmount - totalRAmount;
                                _currentAmount = totalAmount - Convert.ToInt32(_TotalAmt);
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
                                        _newCustomer.getTotalAmount = Convert.ToInt32(_TotalAmt);
                                        _newCustomer.Type = 'S';
                                        _newCustomer.isEdit = true;
                                        _newCustomer.isExpired = true;
                                        _newCustomer.TransactionId = resultId;
                                        _newCustomer.IsClosed = FirstTime;
                                        _newCustomer.ShowDialog();
                                        FirstTime=ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                                    }
                                }
                                else
                                { // cann't extend to next year, so renew
                                    foreach (MemberCardRule memberCards in memberCardRuleList)
                                    {
                                        if (Convert.ToInt32(memberCards.RangeFrom) <= _TotalAmt)
                                        {
                                            memberTypeID = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();

                                        }

                                    }

                                    memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                                    if (memName != null)
                                    {
                                        _newCustomer.CustomerId = CustomerId;
                                        _newCustomer.MemerTypeName = memName;
                                        _newCustomer.getTotalAmount = Convert.ToInt32(_TotalAmt);
                                        _newCustomer.Type = 'S';
                                        _newCustomer.isEdit = true;
                                        _newCustomer.isReNew = true;
                                        _newCustomer.TransactionId = resultId;
                                        _newCustomer.IsClosed = FirstTime;
                                        _newCustomer.ShowDialog();
                                        FirstTime=ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                                    }
                                }


                            }
                            else
                            {   // aldy expired and then renew                      

                                foreach (MemberCardRule memberCards in memberCardRuleList)
                                {
                                    if (Convert.ToInt32(memberCards.RangeFrom) <= _TotalAmt)
                                    {
                                        memberTypeID = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();

                                    }

                                }
                                memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                                if (memName != null)
                                {
                                    _newCustomer.CustomerId = CustomerId;
                                    _newCustomer.MemerTypeName = memName;
                                    _newCustomer.getTotalAmount = Convert.ToInt32(_TotalAmt);
                                    _newCustomer.Type = 'S';
                                    _newCustomer.isEdit = true;
                                    _newCustomer.isReNew = true;
                                    _newCustomer.TransactionId = resultId;
                                    _newCustomer.IsClosed = FirstTime;
                                    _newCustomer.ShowDialog();
                                    FirstTime=ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                                }
                            }

                        }
                    }
                    else
                    { //customerObj.PromoteDate == null it means that it's new customer                       
                        if (_TotalAmt >= Convert.ToInt32(minimumAmountofThisMemberType))
                        {
                            foreach (MemberCardRule memberCard in memberCardRuleList)
                            {
                                if (Convert.ToInt32(memberCard.RangeFrom) <= _TotalAmt)
                                {
                                    memberTypeID = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();
                                }
                            }

                            memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                            if (memName != null)
                            {
                                _newCustomer.CustomerId = CustomerId;
                                _newCustomer.MemerTypeName = memName;
                                _newCustomer.getTotalAmount = Convert.ToInt32(_TotalAmt);
                                _newCustomer.Type = 'S';
                                _newCustomer.TransactionId = resultId;
                                _newCustomer.isEdit = true;
                                _newCustomer.IsClosed = FirstTime;
                                _newCustomer.ShowDialog();
                                FirstTime=ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                            }

                        }


                    }

                }
                else // name == 'Default'
                {
                    if (_TotalAmt >= Convert.ToInt32(minimumAmountofThisMemberType))
                    {
                        foreach (MemberCardRule memberCards in memberCardRuleList)
                        {
                            if (Convert.ToInt32(memberCards.RangeFrom) <= _TotalAmt)
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
                            _newCustomer.getTotalAmount = Convert.ToInt32(_TotalAmt);
                            _newCustomer.Type = 'S';
                            _newCustomer.TransactionId = resultId;
                            _newCustomer.IsClosed = FirstTime;
                            _newCustomer.ShowDialog();
                            FirstTime=ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);
                        }


                    }
                }



                #endregion      

                if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                {
                    Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];

                    newForm.Clear();
                }

                //SD
                #region Update IsCalculatedPoint in Transaction And PointHistoryId in Transaction Detail
                var _customerId1 = entity1.Transactions.Where(x => x.Id == resultId).Select(x => x.CustomerId).FirstOrDefault();
                var _custList = entity1.Customers.Where(x => x.Id == _customerId1).FirstOrDefault();
                if (_custList.MemberTypeID != null)
                {
                    int memberTypeID2 = (int)_custList.MemberTypeID;
                    var _isCalucatePoint = entity.MemberCardRules.Where(x => x.IsActive == true && x.MemberTypeId == memberTypeID2).Select(x => x.IsCalculatePoints).FirstOrDefault();

                    bool isCalucatePoint2 = _isCalucatePoint == null ? false : (bool)_isCalucatePoint;
                    ELC_CustomerPointSystem.Update_ForPoint_InTransaction(FirstTime, _custList.MemberTypeID, resultId, 2,(int)_customerId1, isCalucatePoint2, _custList.Id);

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
                        //newRow.TotalAmount = (int)transaction.TotalAmount; //Edit By ZMH
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
                    reportPath = Application.StartupPath + Utility.GetReportPath("Credit");
                    rv.Reset();
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Add(rds);

                    Utility.Slip_Log(rv);
                    //switch (Utility.GetDefaultPrinter())
                    //{

                    //    case "Slip Printer":
                    //        Utility.Slip_Footer(rv);
                    //        break;
                    //}
                    Utility.Slip_A4_Footer(rv);
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

                    APP_Data.Counter counter = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                    ReportParameter CounterName = new ReportParameter("CounterName", counter.Name);
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

                    Int64 totalAmountRep = insertedTransaction.TotalAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TotalAmount); //Edit By ZMH
                    ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                    rv.LocalReport.SetParameters(TotalAmount);



                    //ReportParameter TotalAmount = new ReportParameter("TotalAmount", insertedTransaction.TotalAmount.ToString()); //Edit By ZMH 
                    //rv.LocalReport.SetParameters(TotalAmount);

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



                    ReportParameter PaidAmount = new ReportParameter("PaidAmount", ReceiveAmount.ToString());
                    rv.LocalReport.SetParameters(PaidAmount);

                    ReportParameter PrevOutstanding = new ReportParameter("PrevOutstanding", _outstandingPreBalance.ToString());
                    rv.LocalReport.SetParameters(PrevOutstanding);

                    ReportParameter PrePaidDebt = new ReportParameter("PrePaidDebt", _prePaidDebt.ToString());
                    rv.LocalReport.SetParameters(PrePaidDebt);

                    ReportParameter NetPayable = new ReportParameter("NetPayable", (OldOutstandingAmount + insertedTransaction.TotalAmount - PayDebtAmount).ToString());
                    rv.LocalReport.SetParameters(NetPayable);

                    ReportParameter Balance = new ReportParameter("Balance", ((OldOutstandingAmount + insertedTransaction.TotalAmount - PayDebtAmount) - Convert.ToInt64(ReceiveAmount)).ToString());
                    rv.LocalReport.SetParameters(Balance);

                    APP_Data.Customer cus = ELC_CustomerPointSystem.Get_CustomerName(resultId);
                    ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name.ToString());
                    rv.LocalReport.SetParameters(CustomerName);

                    if (Utility.GetDefaultPrinter() == "A4 Printer")
                    {
                        ReportParameter CusAddress = new ReportParameter("CusAddress", cus.Address);
                        rv.LocalReport.SetParameters(CusAddress);
                    }

                    ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
                    rv.LocalReport.SetParameters(AvailablePoint);

                    Utility.Get_Print(rv);
                    

                }
                #endregion
            }
        }


        private DialogResult MessageShow()
        {
            DialogResult result = MessageBox.Show("Payment Completed", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return result;
        }


        private void cboCustomerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrepaidDebt = 0;
            if (cboCustomerList.SelectedIndex != 0)
            {
                //get remaining outstanding amount
                int customerId = Convert.ToInt32(cboCustomerList.SelectedValue.ToString());
                List<Transaction> rtList = new List<Transaction>();
                Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();
                List<Transaction> OldOutStandingList = entity.Transactions.Where(x => x.CustomerId == cust.Id).ToList().Where(x=>x.IsDeleted != true).ToList();
                OldOutstandingAmount = 0;
                foreach (Transaction ts in OldOutStandingList)
                {
                    if (ts.IsPaid == false)
                    {
                        OldOutstandingAmount += (long)ts.TotalAmount - (long)ts.RecieveAmount;
                        rtList = (from t in entity.Transactions where t.Type == TransactionType.CreditRefund && t.ParentId == ts.Id select t).ToList();
                        if (rtList.Count > 0)
                        {
                            foreach (Transaction rt in rtList)
                            {
                                OldOutstandingAmount -= (int)rt.RecieveAmount;
                            }
                        }
                    }
                    if (ts.Type == TransactionType.Prepaid && ts.IsActive == false)
                    {
                        PrepaidDebt += (int)ts.RecieveAmount;
                        int useAmount = (ts.UsePrePaidDebts1 == null) ? 0 : (int)ts.UsePrePaidDebts1.Sum(x => x.UseAmount);
                        PrepaidDebt -= useAmount;
                    }
                }


                if (OldOutstandingAmount < 0)
                {
                    lblPreviousBalance.Text = "0";
                    lblPrePaid.Text = PrepaidDebt.ToString();
                    lblTotalCost.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount  - MCDiscount - BDDiscount)).ToString();
                    lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount  - MCDiscount - BDDiscount)).ToString();
                }
                else
                {
                    lblPreviousBalance.Text = OldOutstandingAmount.ToString();
                    lblPrePaid.Text = PrepaidDebt.ToString();
                    lblTotalCost.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - MCDiscount - BDDiscount) + OldOutstandingAmount - PrepaidDebt).ToString();
                    lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount  - MCDiscount - BDDiscount) + OldOutstandingAmount - PrepaidDebt).ToString();
                }
                //to 
                int amount = 0;
                Int32.TryParse(txtReceiveAmount.Text, out amount);
                int totalCost = 0;
                Int32.TryParse(lblTotalCost.Text, out totalCost);

                if (amount >= totalCost)
                {
                    lblNetPayableTitle.Text = "Change";
                    lblNetPayable.Text = (amount - totalCost).ToString();
                }
                else
                {
                    lblNetPayableTitle.Text = "Net Payable";
                    lblNetPayable.Text = (totalCost - amount).ToString();
                }
            }
            else
            {
                lblPreviousBalance.Text = "0";
                lblTotalCost.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount  - MCDiscount - BDDiscount) + 0).ToString();
                lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount  - MCDiscount - BDDiscount) + 0).ToString();

                //to 
                int amount = 0;
                Int32.TryParse(txtReceiveAmount.Text, out amount);

                if (amount >= (Int32.Parse(lblTotalCost.Text)))
                {
                    lblNetPayableTitle.Text = "Change";
                    lblNetPayable.Text = (amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount )).ToString();
                }
                else
                {
                    lblNetPayableTitle.Text = "NetPayable";
                    lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount) - amount).ToString();
                }
            }
        }

        private void txtReceiveAmount_KeyUp(object sender, KeyEventArgs e)
        {
            int amount = 0;
            Int32.TryParse(txtReceiveAmount.Text, out amount);
            int totalCost = 0;
            Int32.TryParse(lblTotalCost.Text, out totalCost);

            if (amount >= totalCost)
            {
                lblNetPayableTitle.Text = "Change";
                lblNetPayable.Text = (amount - totalCost).ToString();
            }
            else
            {
                lblNetPayableTitle.Text = "NetPayable";
                lblNetPayable.Text = (totalCost - amount).ToString();
            }

            int receiveamt = 0;
            if (txtReceiveAmount.Text == string.Empty)
            {
                receiveamt = 0;
            }
            else
            {
                receiveamt = Convert.ToInt32(txtReceiveAmount.Text);
            }

            if (receiveamt > Convert.ToInt32(lblAccuralCost.Text))
            {
                lblNetPayableTitle.Text = "Change";
            }
         
         
             
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            NewCustomer newform = new NewCustomer();
            newform.Show();
        }

        private void PaidByCreditWithPrePaidDebt_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(cboCustomerList);
            tp.Hide(txtReceiveAmount);
        }

        private void txtReceiveAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void chkIsPrePaid_CheckedChanged(object sender, EventArgs e)
        {
            IsPrePaidCheck();
        }

        private void IsPrePaidCheck()
        {
            int receiveAmt = 0;
            if (txtReceiveAmount.Text != "")
            {
                receiveAmt = Convert.ToInt32(txtReceiveAmount.Text);
            }

            if (chkIsPrePaid.Checked == false)
            {

                lblTotalCost.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - MCDiscount - BDDiscount) + OldOutstandingAmount).ToString();
                lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - MCDiscount - BDDiscount) + OldOutstandingAmount - receiveAmt).ToString();
            }
            else
            {
                int prepaidAmt = Convert.ToInt32(lblPrePaid.Text);
                //int receiveAmt = 0;
                //if (txtReceiveAmount.Text != "")
                //{
                //    receiveAmt = Convert.ToInt32(txtReceiveAmount.Text);
                //}

                lblTotalCost.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount  - MCDiscount - BDDiscount) + OldOutstandingAmount - prepaidAmt).ToString();
                lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - MCDiscount - BDDiscount) + OldOutstandingAmount - prepaidAmt - receiveAmt).ToString();
            }
        }
        //private void PaidByCredit_Activated(object sender, EventArgs e)
        //{
        //List<Customer> CustomerList = new List<Customer>();
        //Customer none = new Customer();
        //none.Name = "Select Customer";
        //none.Id = 0;
        //CustomerList.Add(none);
        //CustomerList.AddRange(entity.Customers.ToList());
        //cboCustomerList.DataSource = CustomerList;
        //cboCustomerList.DisplayMember = "Name";
        //cboCustomerList.ValueMember = "Id";
        //cboCustomerList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        //cboCustomerList.AutoCompleteSource = AutoCompleteSource.ListItems;
        //}
        #endregion

        #region Function
   

        #region for saving Sale Qty in Stock Transaction table
        private void Save_SaleQty_ToStockTransaction(List<Stock_Transaction> productList)
        {
            int _year, _month;

            _year = System.DateTime.Now.Year;
            _month = System.DateTime.Now.Month;
            Utility.Sale_Run_Process(_year, _month, productList);
        }
        #endregion

        public void LoadForm()
        {
            List<Customer> CustomerList = new List<Customer>();
            Customer none = new Customer();
            none.Name = "Select Customer";
            none.Id = 0;
            CustomerList.Add(none);
            CustomerList.AddRange(entity.Customers.ToList());
            cboCustomerList.DataSource = CustomerList;
            cboCustomerList.DisplayMember = "Name";
            cboCustomerList.ValueMember = "Id";
            cboCustomerList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCustomerList.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        #endregion

        


    }
}
