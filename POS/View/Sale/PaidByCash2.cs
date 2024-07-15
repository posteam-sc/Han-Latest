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
    public partial class PaidByCash2 : Form
    {
        #region Variables
        Transaction insertedTransaction = new Transaction();
        public int ReceiveAmt = 0;
        public int CashChange = 0;

        public List<TransactionDetail> DetailList = new List<TransactionDetail>();

        public int Discount { get; set; }

        public int Tax { get; set; }

        public int ExtraDiscount { get; set; }

        public int ExtraTax { get; set; }

        public Boolean isDraft { get; set; }

        public bool FirstTime = false;

        public Boolean isDebt { get; set; }

        public string DraftId { get; set; }

        public string DebtId { get; set; }

        public long DebtAmount { get; set; }

        public Boolean IsWholeSale { get; set; }

        public long PrePaidAmount { get; set; }

        public List<Transaction> CreditTransaction { get; set; }

        public List<Transaction> PrePaidTransaction { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        private long totalAmount = 0, prePaidAmount = 0;

        public decimal BDDiscount { get; set; }

        public decimal MCDiscount { get; set; }

        public int CustomerId { get; set; }

        public int? MemberTypeId { get; set; }

        public decimal? MCDiscountPercent { get; set; }

        public DialogResult _result;

        public decimal TotalAmt = 0;

        private decimal AmountWithExchange = 0;

        string CurrencySymbol = string.Empty;

        long total;

        string resultId = "-";

        int Qty = 0;

        List<Stock_Transaction> productList = new List<Stock_Transaction>();

        public List<string> TranIdList = new List<string>();

        Transaction CreditT = new Transaction();

        public Boolean IsPrint = true;

        System.Data.Objects.ObjectResult<String> Id;

        #endregion

        public PaidByCash2()
        {
            InitializeComponent();
        }

        #region Hot keys handler
        void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.U)      //  Ctrl + U => Focus Currency
            {
                cboCurrency.DroppedDown = true;
                if (cboCurrency.Focused != true)
                {
                    cboCurrency.Focus();
                }
            }
            else if (e.Control && e.KeyCode == Keys.R)      // Ctrl + R => Focus Receive Amt
            {
                cboCurrency.DroppedDown = false;
                txtReceiveAmount.Focus();
            }
            else if (e.Control && e.KeyCode == Keys.S)      // Ctrl + S => Click Save
            {
                cboCurrency.DroppedDown = false;
                btnSubmit.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.C)      // Ctrl + C => Focus DropDown Customer
            {
                cboCurrency.DroppedDown = false;
                btnCancel.PerformClick();
            }
        }
        #endregion

        private void PaidByCash2_Load(object sender, EventArgs e)
        {
            #region Setting Hot Kyes For the Controls
            SendKeys.Send("%"); SendKeys.Send("%"); // Clicking "Alt" on page load to show underline of Hot Keys
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form_KeyDown);
            #endregion

            #region currency
            Currency curreObj = new Currency();
            List<Currency> currencyList = new List<Currency>();
            currencyList = entity.Currencies.ToList();
            foreach (Currency c in currencyList)
            {
                cboCurrency.Items.Add(c.CurrencyCode);
            }
            int id = 0;
            if (SettingController.DefaultCurrency != 0)
            {
                id = Convert.ToInt32(SettingController.DefaultCurrency);
                curreObj = entity.Currencies.FirstOrDefault(x => x.Id == id);
                cboCurrency.Text = curreObj.CurrencyCode;
            }
            //txtExchangeRate.Text = SettingController.DefaultExchangeRate.ToString();
            #endregion

            if (!isDebt)
            {
                //total = (long)(DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - BDDiscount - MCDiscount);
                total = (long)(DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - BDDiscount - MCDiscount);
                lblTotalCost.Text = Utility.CalculateExchangeRate(id, total).ToString();
                TotalAmt = Convert.ToDecimal(lblTotalCost.Text);

                AmountWithExchange = Convert.ToDecimal(lblTotalCost.Text);
            }
            else
            {
                foreach (Transaction tObj in CreditTransaction)
                {
                    if (tObj.Transaction1.Count <= 0)
                    {
                        totalAmount += (long)tObj.TotalAmount - (long)tObj.RecieveAmount;
                    }
                    //Has refund
                    else
                    {
                        totalAmount += (long)tObj.TotalAmount - (long)tObj.RecieveAmount;
                        foreach (Transaction Refund in tObj.Transaction1.Where(x => x.IsDeleted != true))
                        {
                            totalAmount -= (long)Refund.RecieveAmount;
                        }
                    }
                    if (tObj.UsePrePaidDebts != null)
                    {
                        long prepaid = (long)tObj.UsePrePaidDebts.Sum(x => x.UseAmount);
                        totalAmount -= prepaid;
                    }
                }
                foreach (Transaction tObj in PrePaidTransaction)
                {
                    prePaidAmount += (long)tObj.TotalAmount;
                    long useAmount = (tObj.UsePrePaidDebts1 == null) ? 0 : (int)tObj.UsePrePaidDebts1.Sum(x => x.UseAmount);
                    prePaidAmount -= useAmount;
                }
                DebtAmount = (totalAmount - prePaidAmount);
                lblTotalCost.Text = Utility.CalculateExchangeRate(id, DebtAmount).ToString();
            }
            // txtReceiveAmount.Focus();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // string _defaultPrinter = Utility.GetDefaultPrinter();
            
            Boolean hasError = false;
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            long receiveAmount = 0;
            long totalCost = (long)DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (long)BDDiscount - (long)MCDiscount;
            //long totalCost = (long)DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - (long)BDDiscount - (long)MCDiscount;
            //total cost wint unit price
            long unitpriceTotalCost = (long)DetailList.Sum(x => x.UnitPrice * x.Qty);
            Int64.TryParse(txtReceiveAmount.Text, out receiveAmount);

            ReceiveAmt = Convert.ToInt32(receiveAmount);
            CashChange = Convert.ToInt32(lblChanges.Text);
            decimal totalCashSaleAmount = Convert.ToDecimal(lblTotalCost.Text);
            if (cboCurrency.SelectedIndex == -1)
            {
                tp.SetToolTip(cboCurrency, "Error");
                tp.Show("Please select currency!", cboCurrency);
                return;
            }
            string currVal = cboCurrency.Text;
            int currencyId = (from c in entity.Currencies where c.CurrencyCode == currVal select c.Id).SingleOrDefault();
            Currency cu = entity.Currencies.FirstOrDefault(x => x.Id == currencyId);

            //Validation
            if (receiveAmount == 0)
            {
                tp.SetToolTip(txtReceiveAmount, "Error");
                tp.Show("Please fill up receive amount!", txtReceiveAmount);
                hasError = true;
            }
            else if (receiveAmount < AmountWithExchange)
            {
                tp.SetToolTip(txtReceiveAmount, "Error");
                tp.Show("Receive amount must be greater than total cost!", txtReceiveAmount);
                hasError = true;
            }

            if (!hasError)
            {
                CurrencySymbol = string.Empty;
                insertedTransaction = new Transaction();
                List<Transaction> RefundList = new List<Transaction>();
                decimal change = 0;
                if (cu.CurrencyCode == "USD")
                {
                    totalCashSaleAmount = (decimal)totalCashSaleAmount * (decimal)cu.LatestExchangeRate;
                    receiveAmount = receiveAmount * (long)cu.LatestExchangeRate;
                    CurrencySymbol = "$";
                    change = Convert.ToDecimal(lblChanges.Text) * (decimal)cu.LatestExchangeRate;
                }
                else
                {
                    CurrencySymbol = "Ks";
                    change = Convert.ToDecimal(lblChanges.Text);
                }
                if (!isDebt)
                {
                    //Updated by Lele
                    //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Utility.PaymentTypeID.Cash, ExtraTax + Tax, ExtraDiscount + Discount, totalCost, receiveAmount, null, CustomerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, true);
                    //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Utility.PaymentTypeID.Cash, ExtraTax + Tax, ExtraDiscount + Discount, totalCost, receiveAmount, null, CustomerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, true);
                    //Updated by YiMon
                    Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Utility.PaymentTypeID.Cash, ExtraTax + Tax, ExtraDiscount + Discount, totalCost, receiveAmount, null, CustomerId, MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, true,"","");

                    entity = new POSEntities();
                    resultId = Id.FirstOrDefault().ToString();
                    insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                    string TId = insertedTransaction.Id;
                    insertedTransaction.IsDeleted = false;
                    insertedTransaction.ReceivedCurrencyId = currencyId;


                    foreach (TransactionDetail detail in DetailList)
                    {
                        detail.IsDeleted = false;//Update IsDelete (Null to 0)
                        if (detail.ConsignmentPrice == null)
                        {
                            detail.ConsignmentPrice = 0;
                        }

                        detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();

                        Boolean? IsConsignmentPaid = Utility.IsConsignmentPaid(detail.Product);
                        //    var detailID = entity.InsertTransactionDetail(TId, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, detail.ConsignmentPrice, IsConsignmentPaid).SingleOrDefault();

                        var detailID = entity.InsertTransactionDetail(TId, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, detail.ConsignmentPrice, IsConsignmentPaid, detail.IsFOC, Convert.ToInt32(detail.SellingPrice), Convert.ToDouble(detail.IsDeductedBy),detail.BatchNo).SingleOrDefault();


                        var detailforBDdiscount = entity.TransactionDetails.Find(detailID);
                        detailforBDdiscount.BdDiscounted = detail.BdDiscounted;

                        detail.Product.Qty = detail.Product.Qty - detail.Qty;

                        //save in stocktransaction

                        Stock_Transaction st = new Stock_Transaction();
                        st.ProductId = detail.Product.Id;
                        Qty = Convert.ToInt32(detail.Qty);
                        st.Sale = Qty;
                        productList.Add(st);
                        Qty = 0;


                        if (detail.Product.Brand.Name == "Special Promotion")
                        {
                            List<WrapperItem> wList = detail.Product.WrapperItems.ToList();
                            if (wList.Count > 0)
                            {
                                foreach (WrapperItem w in wList)
                                {
                                    Product wpObj = (from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault();
                                    wpObj.Qty = wpObj.Qty - detail.Qty;

                                    SPDetail spDetail = new SPDetail();
                                    spDetail.TransactionDetailID = Convert.ToInt32(detailID);
                                    spDetail.DiscountRate = detail.DiscountRate;
                                    spDetail.ParentProductID = w.ParentProductId;
                                    spDetail.ChildProductID = w.ChildProductId;
                                    spDetail.Price = wpObj.Price;
                                    entity.insertSPDetail(spDetail.TransactionDetailID, spDetail.ParentProductID, spDetail.ChildProductID, spDetail.Price, spDetail.DiscountRate, "PC");
                                    //entity.SPDetails.Add(spDetail);
                                }
                            }
                        }

                        entity.SaveChanges();
                    }
                    //save in stocktransaction
                    Save_SaleQty_ToStockTransaction(productList);
                    productList.Clear();

                    ExchangeRateForTransaction ex = new ExchangeRateForTransaction();
                    ex.TransactionId = TId;
                    ex.CurrencyId = cu.Id;
                    ex.ExchangeRate = Convert.ToInt32(cu.LatestExchangeRate);
                    entity.ExchangeRateForTransactions.Add(ex);
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
                                            pdObjInTran.Date = detail.Transaction.DateTime;
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
                                            pdObjInTran.Date = detail.Transaction.DateTime;
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




                    _result = MessageShow();
                }
                else
                {
                    if (lblChangesText.Text == "Changes")
                    {
                        receiveAmount -= Convert.ToInt64(lblChanges.Text);
                    }
                    long totalAmount = receiveAmount + prePaidAmount;
                    long totalCredit = 0;
                    Int64.TryParse(lblTotalCost.Text, out totalCredit);
                    long DebtAmount = 0;
                    if (totalAmount != 0)
                    {
                        if (CreditTransaction.Count > 0)
                        {
                            int index = CreditTransaction.Count;
                            for (int outer = index - 1; outer >= 1; outer--)
                            {
                                for (int inner = 0; inner < outer; inner++)
                                {
                                    if (CreditTransaction[inner].TotalAmount - CreditTransaction[inner].RecieveAmount < CreditTransaction[inner + 1].TotalAmount - CreditTransaction[inner + 1].RecieveAmount)
                                    {
                                        Transaction t = CreditTransaction[inner];
                                        CreditTransaction[inner] = CreditTransaction[inner + 1];
                                        CreditTransaction[inner + 1] = t;
                                    }
                                }
                            }
                            foreach (Transaction CT in CreditTransaction)
                            {
                                long CreditAmount = 0;
                                CreditAmount = (long)CT.TotalAmount - (long)CT.RecieveAmount;
                                RefundList = (from tr in entity.Transactions where tr.ParentId == CT.Id && tr.Type == TransactionType.CreditRefund select tr).ToList();
                                if (RefundList.Count > 0)
                                {
                                    foreach (Transaction TRefund in RefundList)
                                    {
                                        CreditAmount -= (long)TRefund.RecieveAmount;
                                    }
                                }
                                if (CT.UsePrePaidDebts != null)
                                {
                                    long prePaid = (long)CT.UsePrePaidDebts.Sum(x => x.UseAmount);
                                    CreditAmount -= prePaid;
                                }
                                if (CreditAmount <= totalAmount)
                                {
                                    //CT.IsPaid = true;
                                    //entity.SaveChanges();
                                    CreditT = (from t in entity.Transactions where t.Id == CT.Id select t).FirstOrDefault<Transaction>();
                                    CreditT.IsPaid = true;

                                    TranIdList.Add(CreditT.Id);
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
                                        foreach (Transaction PrePaidDebtTrans in PrePaidTransaction)
                                        {
                                            long PrePaidamount = 0;
                                            //int useAmount = 0;
                                            int useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                            PrePaidamount = (long)PrePaidDebtTrans.TotalAmount - useAmount;
                                            //if (CreditAmount >= PrePaidamount)
                                            //{
                                            //    PrePaidDebtTrans.IsActive = true;
                                            //    //entity.Entry(PrePaidDebtTrans).State = System.Data.EntityState.Modified;
                                            //}

                                            if (CreditAmount >= PrePaidamount)
                                            {
                                                //PrePaidDebtTrans.IsActive = true;
                                                //entity.SaveChanges();
                                                Transaction PD = (from PT in entity.Transactions where PT.Id == PrePaidDebtTrans.Id select PT).FirstOrDefault<Transaction>();
                                                PD.IsActive = true;
                                                entity.Entry(PD).State = EntityState.Modified;
                                                UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                                usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                                usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                                usePrePaidDObj.CreditTransactionId = CT.Id;
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
                                                usePrePaidDObj.CreditTransactionId = CT.Id;
                                                usePrePaidDObj.CashierId = MemberShip.UserId;
                                                usePrePaidDObj.CounterId = MemberShip.CounterId;
                                                entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                                entity.SaveChanges();
                                                CreditAmount -= PrePaidamount;
                                            }
                                        }

                                        PrePaidTransaction = (from PDT in entity.Transactions where PDT.Type == TransactionType.Prepaid && PDT.IsActive == false select PDT).ToList();
                                    }
                                }
                            }
                            if (DebtAmount > 0)
                            {
                                string joinedTranIdList = string.Join(",", TranIdList);
                                //Updated by Lele
                                //System.Data.Objects.ObjectResult<string> DebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Settlement, true, true, 1, 0, 0, DebtAmount, DebtAmount, null, CustomerId, MCDiscount, BDDiscount, MemberTypeId, MCDiscountPercent, true, joinedTranIdList, IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, false);
                                //System.Data.Objects.ObjectResult<string> DebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Settlement, true, true, 1, 0, 0, DebtAmount, DebtAmount, null, CustomerId, MCDiscount, BDDiscount, MemberTypeId, MCDiscountPercent, true, joinedTranIdList, IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false ,false);
                                //Updated by Yimon
                                System.Data.Objects.ObjectResult<string> DebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Settlement, true, true, 1, 0, 0, DebtAmount, DebtAmount, null, CustomerId, MCDiscount, BDDiscount, MemberTypeId, MCDiscountPercent, true, joinedTranIdList, IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false ,false,"","");
                                string _Debt = DebtId.FirstOrDefault().ToString();

                                foreach (var t in TranIdList)
                                {
                                    var result = (from tr in entity.Transactions where tr.Id == t select tr).FirstOrDefault();

                                    // Transaction _tt = new Transaction();
                                    result.TranVouNos = _Debt;
                                    result.IsSettlement = true;
                                    entity.Entry(result).State = EntityState.Modified;
                                    entity.SaveChanges();
                                }

                                entity = new POSEntities();
                                resultId = _Debt;
                                insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                                insertedTransaction.ReceivedCurrencyId = cu.Id;
                                ExchangeRateForTransaction ex = new ExchangeRateForTransaction();
                                ex.TransactionId = resultId;
                                ex.CurrencyId = cu.Id;
                                ex.ExchangeRate = Convert.ToInt32(cu.LatestExchangeRate);
                                entity.ExchangeRateForTransactions.Add(ex);


                                //  Transaction CreditT = (from t in entity.Transactions where t.Id == CT.Id select t).FirstOrDefault<Transaction>();

                                // CreditT.TranVouNos = CreditT

                                entity.SaveChanges();
                                entity.SaveChanges();
                            }
                        }
                        else
                        {
                            totalAmount -= prePaidAmount;
                            receiveAmount -= prePaidAmount;
                            //System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.PrepaidDebt, true, false, 1, 0, 0, totalAmount, totalAmount, null, customerId);
                            //entity.SaveChanges();
                        }
                    }
                    if (receiveAmount > 0)
                    {
                        //Updated by Lele
                        //System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Prepaid, true, false, 1, 0, 0, receiveAmount, receiveAmount, null, CustomerId, MCDiscount, BDDiscount, MemberTypeId, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, false);
                        //System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Prepaid, true, false, 1, 0, 0, receiveAmount, receiveAmount, null, CustomerId, MCDiscount, BDDiscount, MemberTypeId, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, 0,0,false,false);
                        //Updated by Yimon
                        System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Prepaid, true, false, 1, 0, 0, receiveAmount, receiveAmount, null, CustomerId, MCDiscount, BDDiscount, MemberTypeId, MCDiscountPercent, false, "", IsWholeSale, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, 0,0,false,false,"","");
                        entity.SaveChanges();
                        if (DebtAmount == 0)
                        {
                            entity = new POSEntities();
                            resultId = PreDebtId.FirstOrDefault().ToString();
                            insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                            insertedTransaction.ReceivedCurrencyId = cu.Id;
                            ExchangeRateForTransaction ex = new ExchangeRateForTransaction();
                            ex.TransactionId = resultId;
                            ex.CurrencyId = cu.Id;
                            ex.ExchangeRate = Convert.ToInt32(cu.LatestExchangeRate);
                            entity.ExchangeRateForTransactions.Add(ex);
                            entity.SaveChanges();
                        }

                    }
                    if (isDraft)
                    {
                        Transaction draft = (from trans in entity.Transactions where trans.Id == DraftId select trans).FirstOrDefault<Transaction>();
                        if (draft != null)
                        {
                            draft.TransactionDetails.Clear();
                            var Detail = entity.TransactionDetails.Where(d => d.TransactionId == draft.Id);
                            foreach (var d in Detail)
                            {
                                entity.TransactionDetails.Remove(d);
                            }
                            entity.Transactions.Remove(draft);
                            entity.SaveChanges();
                        }
                    }


                    //Print Invoice
                    #region [ Print ]
                    if (IsPrint)
                    {

                        string reportPath = "";
                        ReportViewer rv = new ReportViewer();
                        //ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                        reportPath = Application.StartupPath + Utility.GetReportPath("Settlement");
                        rv.Reset();
                        rv.LocalReport.ReportPath = reportPath;
                        // rv.LocalReport.DataSources.Add(rds);

                        Utility.Slip_Log(rv);
                        //switch (Utility.GetDefaultPrinter())
                        //{

                        //    case "Slip Printer":
                        //        Utility.Slip_Footer(rv);
                        //        break;
                        //}
                        Utility.Slip_A4_Footer(rv);
                        //APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();
                        APP_Data.Customer cus = ELC_CustomerPointSystem.Get_CustomerName(resultId);
                        ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
                        rv.LocalReport.SetParameters(CustomerName);


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

                        APP_Data.Counter c = entity.Counters.Where(x => x.Id == MemberShip.CounterId).FirstOrDefault();

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


                        ReportParameter TotalAmount = new ReportParameter("TotalAmount", lblTotalCost.Text.ToString());
                        rv.LocalReport.SetParameters(TotalAmount);



                        ReportParameter PaidAmount = new ReportParameter("PaidAmount", ReceiveAmt.ToString());
                        rv.LocalReport.SetParameters(PaidAmount);

                        int balance = Convert.ToInt32(lblTotalCost.Text) - Convert.ToInt32(ReceiveAmt);
                        balance = balance < 0 ? 0 : balance;
                        ReportParameter Balance = new ReportParameter("Balance", balance.ToString());
                        rv.LocalReport.SetParameters(Balance);

                        int _change = Convert.ToInt32(ReceiveAmt) - Convert.ToInt32(lblTotalCost.Text);

                        _change = _change < 0 ? 0 : _change;
                        ReportParameter Change = new ReportParameter("Change", _change.ToString());
                        rv.LocalReport.SetParameters(Change);

                        if (Utility.GetDefaultPrinter() == "A4 Printer")
                        {
                            ReportParameter CusAddress = new ReportParameter("CusAddress", cus.Address);
                            rv.LocalReport.SetParameters(CusAddress);
                        }

                        ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
                        rv.LocalReport.SetParameters(AvailablePoint);

                        // //  PrintDoc.PrintReport(rv,Utility.GetDefaultPrinter());
                        Utility.Get_Print(rv);

                    }
                    #endregion
                    _result = MessageShow();
                }

                //entity = new POSEntities();
                //string resultId = Id.FirstOrDefault().ToString();
                //Transaction insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();


                //foreach (TransactionDetail detail in DetailList)
                //{
                //    detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                //    detail.Product.Qty = detail.Product.Qty - detail.Qty;
                //    insertedTransaction.TransactionDetails.Add(detail);
                //}


                if (!isDebt)
                {
                    if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                    {
                        Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];

                        newForm.Clear();
                    }
                }
                else
                {
                    if (System.Windows.Forms.Application.OpenForms["CustomerDetail"] != null)
                    {
                        CustomerDetail newForm = (CustomerDetail)System.Windows.Forms.Application.OpenForms["CustomerDetail"];
                        newForm.Reload();
                    }
                }
                this.Dispose();

            }

            if (_result.Equals(DialogResult.OK))
            {
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
                    //Update Member Type
                    //ELC_CustomerPointSystem.Get_ExpiredMemberList_And_Update_ExpiredMember(CustomerId);

                    APP_Data.Customer customerObj = (from c in entity.Customers where c.Id == CustomerId select c).FirstOrDefault();
                    int mTypeID = (from memberCardRule in entity.MemberCardRules.AsEnumerable() where memberCardRule.IsActive == true orderby int.Parse(memberCardRule.RangeFrom) descending select memberCardRule.MemberTypeId).FirstOrDefault();
                    if (customerObj.PromoteDate != null)
                    {
                        // isexpired? == no
                        if (DateTime.Today.Date <= customerObj.ExpireDate)
                        { // ok
                            var LastOneYears = DateTime.Now.AddYears(-1);
                            if (customerObj.MemberTypeID != mTypeID) //if selected customer is not the greatest
                            {
                                List<Transaction> transactionList = (from t in entity.Transactions
                                                                     join c in entity.Customers
                                                                     on t.CustomerId equals c.Id
                                                                     where (t.CustomerId == CustomerId) && (c.Id == CustomerId) && (t.DateTime >= LastOneYears)
                                                                     && (t.DateTime <= DateTime.Now) && (t.IsDeleted == false) && (t.IsComplete == true)
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

                                        _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
                                        _newCustomer.Type = 'S';
                                        _newCustomer.isEdit = true;
                                        _newCustomer.TransactionId = resultId;
                                        //zp
                                        // FirstTime = true;
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

                            int _expireYear = customerObj.ExpireDate.Value.Year + 1;

                            int fromYear = customerObj.ExpireDate.Value.Year - 1;
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
                                                                     && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "Refund") && (transactions.Type != "CreditRefund")
                                                                     select transactions).ToList();


                                List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                       join transactions in entity.Transactions
                                                                                       on td.TransactionId equals transactions.ParentId
                                                                                       join c in entity.Customers
                                                                                       on transactions.CustomerId equals c.Id
                                                                                       where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.IsDeleted == false) && (transactions.IsComplete == true)
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
                                totalAmount = totalAmount - giftCardTotalAmount;
                                foreach (TransactionDetail td in RefundTransactionDetailList)
                                {
                                    totalRAmount += Convert.ToInt32(td.TotalAmount);
                                }
                                totalAmount = totalAmount - totalRAmount;

                                _currentAmount = totalAmount - Convert.ToInt32(TotalAmt);
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
                                        _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
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
                                        if (Convert.ToInt32(memberCards.RangeFrom) <= TotalAmt)
                                        {
                                            memberTypeID = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();

                                        }

                                    }

                                    memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                                    if (memName != null)
                                    {
                                        _newCustomer.CustomerId = CustomerId;
                                        _newCustomer.MemerTypeName = memName;
                                        _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
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
                                    if (Convert.ToInt32(memberCards.RangeFrom) <= TotalAmt)
                                    {
                                        memberTypeID = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();

                                    }

                                }
                                memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                                if (memName != null)
                                {
                                    _newCustomer.CustomerId = CustomerId;
                                    _newCustomer.MemerTypeName = memName;
                                    _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
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
                    else
                    {  //customerObj.PromoteDate == null it means that it's new customer                      
                        if (TotalAmt >= Convert.ToInt32(minimumAmountofThisMemberType))
                        {
                            foreach (MemberCardRule memberCard in memberCardRuleList)
                            {
                                if (Convert.ToInt32(memberCard.RangeFrom) <= TotalAmt)
                                {
                                    memberTypeID = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();
                                }
                            }

                            memName = (from m in entity.MemberTypes where m.Id == memberTypeID select m.Name).FirstOrDefault();
                            if (memName != null)
                            {

                                _newCustomer.CustomerId = CustomerId;
                                _newCustomer.MemerTypeName = memName;
                                _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
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
                    if (TotalAmt >= Convert.ToInt32(minimumAmountofThisMemberType))
                    {
                        foreach (MemberCardRule memberCards in memberCardRuleList)
                        {
                            if (Convert.ToInt32(memberCards.RangeFrom) <= TotalAmt)
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
                            _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
                            _newCustomer.Type = 'S';
                            _newCustomer.TransactionId = resultId;
                            _newCustomer.IsClosed = FirstTime;
                            _newCustomer.ShowDialog();
                            FirstTime = ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);

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
                #region Update IsCalculatedPoint in Transaction And PointHistoryId in Transaction Detail and insert VIP Customer

                if (!isDebt)
                {
                    VipCustomer vipCustomer = new VipCustomer();
                    DateTime lastTwoYears = DateTime.Now.AddYears(-2);

                    var _customerId1 = entity1.Transactions.Where(x => x.Id == resultId).Select(x => x.CustomerId).FirstOrDefault();
                    DateTime transactionDate = entity1.Transactions.Where(x => x.Id == resultId).Select(x => x.DateTime.Value).FirstOrDefault();
                    var _custList = entity1.Customers.Where(x => x.Id == _customerId1).FirstOrDefault();
                    if (_custList.MemberTypeID != null)
                    {
                        int memberTypeID2 = (int)_custList.MemberTypeID;
                        var _isCalucatePoint = entity.MemberCardRules.Where(x => x.IsActive == true && x.MemberTypeId == memberTypeID2).Select(x => x.IsCalculatePoints).FirstOrDefault();
                        bool isCalucatePoint2 = _isCalucatePoint == null ? false : (bool)_isCalucatePoint;

                        ELC_CustomerPointSystem.Update_ForPoint_InTransaction(FirstTime, _custList.MemberTypeID, resultId, 1, (int)_customerId1, isCalucatePoint2, _custList.Id);
                        var vipTransactionlist = entity.Transactions.Where(x => x.IsDeleted == false && x.IsActive == true
                                                 && x.CustomerId == _customerId1 && (x.DateTime >= lastTwoYears && x.DateTime <= DateTime.Now)).ToList();
                        if (vipTransactionlist.Count > 0)
                        {
                            //Updated by Lele
                            //var totalPaidAmount = (int)(vipTransactionlist.Select(x => x.TotalAmount).Sum() - vipTransactionlist.Select(x => x.GiftCardAmount).Sum());
                            var totalPaidAmount = (int)(vipTransactionlist.Select(x => x.TotalAmount).Sum() - (vipTransactionlist.Select(x => x.GiftCardAmount).Sum() + vipTransactionlist.Select(x => x.CouponCodeAmount).Sum()));

                            vipCustomer.CustomerCode = _custList.CustomerCode;
                            vipCustomer.LastPaidDate = transactionDate;
                            vipCustomer.TwoYearsTotalAmount = totalPaidAmount;
                            vipCustomer.CreatedUserID = MemberShip.UserId;
                            vipCustomer.CreatedDate = DateTime.Now;
                            vipCustomer.ShopCode = SettingController.DefaultShop.ShortCode;
                            vipCustomer.MemberType = _custList.MemberType.Name;
                            entity.VipCustomers.Add(vipCustomer);
                            entity.SaveChanges();
                        }


                    }
                }

                #endregion






                //Print Invoice
                //#region [ Print ]
                //if (IsPrint)
                //{

                //    dsReportTemp dsReport = new dsReportTemp();
                //    dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                //    int _tAmt = 0;

                //    foreach (TransactionDetail transaction in DetailList)
                //    {
                //        dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                //        newRow.ItemId = transaction.Product.ProductCode;
                //        newRow.Name = transaction.Product.Name;
                //        newRow.Qty = transaction.Qty.ToString();
                //        newRow.DiscountPercent = transaction.DiscountRate.ToString();
                //        newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty;

                //        if (transaction.IsFOC == true)
                //        {
                //            newRow.IsFOC = "FOC";
                //        }

                //        switch (Utility.GetDefaultPrinter())
                //        {
                //            case "A4 Printer":
                //                newRow.UnitPrice = transaction.UnitPrice.ToString();
                //                break;
                //            case "Slip Printer":
                //                newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                //                break;
                //        }

                //        _tAmt += newRow.TotalAmount;
                //        dtReport.AddItemListRow(newRow);
                //    }

                //    string reportPath = "";
                //    ReportViewer rv = new ReportViewer();
                //    ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);

                //    reportPath = Application.StartupPath + Utility.GetReportPath("Cash");


                //    rv.Reset();
                //    rv.LocalReport.ReportPath = reportPath;
                //    rv.LocalReport.DataSources.Add(rds);

                //    Utility.Slip_Log(rv);
                //    //switch (Utility.GetDefaultPrinter())
                //    //{

                //    //    case "Slip Printer":
                //    //        Utility.Slip_Footer(rv);
                //    //        break;
                //    //}
                //    Utility.Slip_A4_Footer(rv);
                //    //APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();
                //    APP_Data.Customer cus = ELC_CustomerPointSystem.Get_CustomerName(resultId);
                //    ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
                //    rv.LocalReport.SetParameters(CustomerName);



                //    ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
                //    rv.LocalReport.SetParameters(TAmt);

                //    ReportParameter ShopName = new ReportParameter("ShopName", SettingController.ShopName);
                //    rv.LocalReport.SetParameters(ShopName);

                //    ReportParameter BranchName = new ReportParameter("BranchName", SettingController.BranchName);
                //    rv.LocalReport.SetParameters(BranchName);

                //    ReportParameter Phone = new ReportParameter("Phone", SettingController.PhoneNo);
                //    rv.LocalReport.SetParameters(Phone);

                //    ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.OpeningHours);
                //    rv.LocalReport.SetParameters(OpeningHours);

                //    ReportParameter TransactionId = new ReportParameter("TransactionId", resultId.ToString());
                //    rv.LocalReport.SetParameters(TransactionId);

                //    APP_Data.Counter c = entity.Counters.Where(x => x.Id == MemberShip.CounterId).FirstOrDefault();

                //    ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                //    rv.LocalReport.SetParameters(CounterName);

                //    ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                //    rv.LocalReport.SetParameters(PrintDateTime);

                //    ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                //    rv.LocalReport.SetParameters(CasherName);

                //    Int64 totalAmountRep = insertedTransaction.TotalAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TotalAmount);
                //    ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                //    rv.LocalReport.SetParameters(TotalAmount);

                //    Int64 taxAmountRep = insertedTransaction.TaxAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TaxAmount);
                //    ReportParameter TaxAmount = new ReportParameter("TaxAmount", taxAmountRep.ToString());
                //    rv.LocalReport.SetParameters(TaxAmount);

                //    ReportParameter CurrencyCode = new ReportParameter("CurrencyCode", CurrencySymbol);
                //    rv.LocalReport.SetParameters(CurrencyCode);

                //    Int64 disAmountRep = insertedTransaction.DiscountAmount == null ? 0 : Convert.ToInt64(insertedTransaction.DiscountAmount);

                //    if (disAmountRep == 0)
                //    {
                //        ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", disAmountRep.ToString());
                //        rv.LocalReport.SetParameters(DiscountAmount);
                //    }
                //    else
                //    {
                //        ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + disAmountRep.ToString());
                //        rv.LocalReport.SetParameters(DiscountAmount);
                //    }

                //    ReportParameter PaidAmount = new ReportParameter("PaidAmount", receiveAmount.ToString());
                //    rv.LocalReport.SetParameters(PaidAmount);

                //    ReportParameter Change = new ReportParameter("Change", CashChange.ToString());
                //    rv.LocalReport.SetParameters(Change);

                //    ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
                //    rv.LocalReport.SetParameters(AvailablePoint);


                //    if (Utility.GetDefaultPrinter() == "A4 Printer")
                //    {
                //        ReportParameter CusAddress = new ReportParameter("CusAddress", cus.Address);
                //        rv.LocalReport.SetParameters(CusAddress);
                //    }




                //    //for (int i = 0; i <= 1; i++) //Edit By ZMH
                //    //{
                //    //    PrintDoc.PrintReport(rv, "Slip");
                //    //}


                //    //PrintDoc.PrintReport(rv, "Slip");
                //    //////int copy=SettingController.DefaultNoOfCopies;
                //    //////for (int i = 0; i < copy; i++)
                //    //////{
                //    //////    PrintDoc.PrintReport(rv, Utility.GetDefaultPrinter());
                //    //////}
                //    Utility.Get_Print(rv);


                //}
                //#endregion
            }


        }




        private DialogResult MessageShow()
        {
            DialogResult result = MessageBox.Show("Payment Completed", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return result;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void PaidByCash_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtReceiveAmount);
            tp.Hide(cboCurrency);
        }

        private void txtReceiveAmount_KeyUp(object sender, KeyEventArgs e)
        {
            int amount = 0;
            Int32.TryParse(txtReceiveAmount.Text, out amount);
            int Cost = 0;
            Int32.TryParse(lblTotalCost.Text, out Cost);
            if (!isDebt)
            {
                lblChanges.Text = (amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - MCDiscount - BDDiscount)).ToString();
            }
            else
            {
                string currVal = cboCurrency.Text;
                int cId = (from c in entity.Currencies where c.CurrencyCode == currVal select c.Id).SingleOrDefault();
                Currency currencyObj = entity.Currencies.FirstOrDefault(x => x.Id == cId);
                if (amount >= DebtAmount)
                {
                    lblChanges.Text = (amount - DebtAmount).ToString();
                    lblChangesText.Text = "Changes";
                }
                else
                {
                    lblChangesText.Text = "Net Payable";
                    lblChanges.Text = (DebtAmount - amount).ToString();
                }
            }
        }

        private void txtReceiveAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtReceiveAmount_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && char.IsLetter('.'))
            {
                e.Handled = true;
            }
        }

        private void txtReceiveAmount_KeyUp_1(object sender, KeyEventArgs e)
        {
            decimal amount = 0;
            decimal.TryParse(txtReceiveAmount.Text, out amount);
            decimal Cost = 0;
            decimal.TryParse(lblTotalCost.Text, out Cost);

            if (txtReceiveAmount.Text != string.Empty)
            {


                if (!isDebt)
                {
                    //lblChanges.Text = (amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount - MCDiscount - BDDiscount + ExtraTax)).ToString();
                    lblChanges.Text = (amount - Cost).ToString();
                }
                else
                {
                    decimal DAmount = Convert.ToDecimal(lblTotalCost.Text);
                    string currVal = cboCurrency.Text;
                    int cId = (from c in entity.Currencies where c.CurrencyCode == currVal select c.Id).SingleOrDefault();
                    Currency currencyObj = entity.Currencies.FirstOrDefault(x => x.Id == cId);
                    if (amount >= DAmount)
                    {
                        lblChanges.Text = (amount - DAmount).ToString();
                        lblChangesText.Text = "Changes";
                    }
                    else
                    {
                        lblChangesText.Text = "Net Payable";
                        lblChanges.Text = (DAmount - amount).ToString();
                    }
                }
            }

        }

        private void cboCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            int currencyId = 0;
            string currVal = cboCurrency.Text;
            currencyId = (from c in entity.Currencies where c.CurrencyCode == currVal select c.Id).SingleOrDefault();
            if (currencyId != 0)
            {
                Currency cu = entity.Currencies.FirstOrDefault(x => x.Id == currencyId);
                if (cu != null)
                {
                    if (!isDebt)
                    {

                        lblTotalCost.Text = Utility.CalculateExchangeRate(cu.Id, total).ToString();
                        AmountWithExchange = Convert.ToDecimal(lblTotalCost.Text);
                        //if (txtReceiveAmount.Text != null)
                        //{
                        //    receive = Convert.ToDecimal(txtReceiveAmount.Text);
                        //}
                        //lblChanges.Text = (AmountWithExchange - receive).ToString();
                        decimal receive = 0;

                        Decimal.TryParse(txtReceiveAmount.Text, out receive);
                        decimal changes = AmountWithExchange - receive;

                        lblChanges.Text = changes.ToString();
                    }
                    else
                    {

                        lblTotalCost.Text = Utility.CalculateExchangeRate(cu.Id, DebtAmount).ToString();
                        AmountWithExchange = Convert.ToDecimal(lblTotalCost.Text);
                        decimal receive = 0;

                        Decimal.TryParse(txtReceiveAmount.Text, out receive);
                        decimal changes = AmountWithExchange - receive;
                        lblChanges.Text = changes.ToString();
                    }

                }
            }
        }

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
        #endregion
    }
}
