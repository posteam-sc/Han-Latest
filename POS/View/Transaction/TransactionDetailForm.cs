using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using POS.APP_Data;
//using static System.Net.Mime.MediaTypeNames;

namespace POS
{
    public partial class TransactionDetailForm : Form
    {
        #region Variable

        private POSEntities entity = new POSEntities();
        public string transactionId;
        public string transactionDetailId;
        Transaction transactionObject;
        // int ExtraDiscount, ExtraTax;
        //long unitpriceTotalCost;
        private int CustomerId = 0;
        public int shopid;
        public bool delete = false;
        public bool DeleteLink = true;
        int Qty = 0;
        public DateTime date;
        public Boolean IsCash = true;

        List<Stock_Transaction> productList = new List<Stock_Transaction>();
        bool isExported = false;
        #endregion

        #region Event

        public TransactionDetailForm(bool isExported)
        {
            InitializeComponent();
            this.isExported = isExported;
        }

        private void TransactionDetailForm_Load(object sender, EventArgs e)
        {
            List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
            APP_Data.Customer customer = new APP_Data.Customer();
            customer.Id = 0;
            customer.Name = "None";
            customerList.Add(customer);
            customerList.AddRange(entity.Customers.ToList());
            cboCustomer.DataSource = customerList;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";
            cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
            dgvPaymentList.AutoGenerateColumns = false;
            LoadData();
        }

        private void dgvTransactionDetail_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvTransactionDetail.Rows)
            {
                TransactionDetail transactionDetailObj = (TransactionDetail)row.DataBoundItem;
                row.Cells[ColProductCode.Index].Value = transactionDetailObj.Product.ProductCode;
                row.Cells[ColProductName.Index].Value = transactionDetailObj.Product.Name;
                row.Cells[ColBatchNo.Index].Value = transactionDetailObj.BatchNo;
                row.Cells[ColQty.Index].Value = transactionDetailObj.Qty;


                string tranId = transactionDetailObj.TransactionId;
                var _result = entity.Transactions.Where(x => x.ParentId == tranId && x.IsDeleted == false).ToList();

                var _refundIdList = _result.Select(x => x.Id).ToList();

                if (_result.Count > 0)
                {
                    string proCode = transactionDetailObj.Product.ProductCode;

                    var proId = entity.Products.Where(x => x.ProductCode == proCode).Select(x => x.Id).FirstOrDefault();

                    List<TransactionDetail> td = new List<TransactionDetail>();
                    if (IsCash)
                    {
                        td = entity.TransactionDetails.Where(x => _refundIdList.Contains(x.TransactionId) && x.ProductId == proId).ToList();
                        row.Cells[colRefundQty.Index].Value = td.Count;
                    }
                    else
                    {
                        // td = entity.TransactionDetails.Where(x => _refundIdList.Contains(x.TransactionId) && x.ProductId == proId).ToList();
                        var data = (from dd in entity.TransactionDetails
                                    join t in entity.Transactions on dd.TransactionId equals t.Id
                                    where _refundIdList.Contains(dd.TransactionId) && dd.ProductId == proId && (t.Type == TransactionType.CreditRefund || t.Type == TransactionType.Refund)
                                    select td).ToList();
                        row.Cells[colRefundQty.Index].Value = data.Count;
                    }

                }
                else
                {
                    row.Cells[colRefundQty.Index].Value = 0;
                }

                row.Cells[ColUnitPrice.Index].Value = transactionDetailObj.UnitPrice;
                //row.Cells[4].Value = transactionDetailObj.SellingPrice;
                row.Cells[ColDiscountPercent.Index].Value = transactionDetailObj.DiscountRate + "%";
                row.Cells[ColTaxPercent.Index].Value = transactionDetailObj.TaxRate + "%";
                row.Cells[ColCost.Index].Value = transactionDetailObj.TotalAmount;

                int discountamt = Convert.ToInt32(row.Cells[colRefundQty.Index].Value) * Convert.ToInt32(transactionDetailObj.UnitPrice) * Convert.ToInt32(transactionDetailObj.DiscountRate) / 100;

                row.Cells[colRefundCost.Index].Value = (Convert.ToInt32(row.Cells[colRefundQty.Index].Value) * transactionDetailObj.UnitPrice) - discountamt;
                // row.Cells[7].Value = transactionDetailObj.ProductId;
                //row.Cells[10].Value = transactionDetailObj.IsFOC;
                if (transactionDetailObj.IsFOC == true)
                {
                    row.Cells[colIsFOC.Index].Value = "FOC";
                }
                if (!string.IsNullOrEmpty(transactionDetailObj.BdDiscounted))
                {
                    row.Cells[colIsFOC.Index].Value = "Birthday Discount";
                }
                else
                {
                    row.Cells[colIsFOC.Index].Value = "";
                }

                if (Convert.ToInt32(row.Cells[ColQty.Index].Value) == Convert.ToInt32(row.Cells[colRefundQty.Index].Value))
                {
                    row.DefaultCellStyle.BackColor = Color.LightSkyBlue;

                }


            }
        }
        private List<TransactionDetail> PrintDetailList(List<TransactionDetail> Dlist)
        {
            HashSet<TransactionDetail> hset = new HashSet<TransactionDetail>();
            // IEnumerable<TransactionDetail> SameCodeProducts = Dlist.Where(x=> !hset.Add(x));           
            IEnumerable<Nullable<long>> SameCodeProducts = Dlist.GroupBy(x => x.ProductId).Where(g => g.Count() > 1).Select(x => x.Key);
            if (SameCodeProducts.Count() > 0)
            {
                return Dlist.GroupBy(x => new { x.Product, x.DiscountRate, x.UnitPrice })
                    .Select(y => new TransactionDetail()
                    {
                        Product = y.Key.Product,
                        Qty = (int)y.Sum(z => z.Qty),
                        DiscountRate = (decimal)y.Key.DiscountRate,
                        UnitPrice = (long)y.Key.UnitPrice,
                        TotalAmount = (long)y.Sum(z => z.TotalAmount)
                    }).ToList();
            }
            return Dlist;
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            string _defaultPrinter = Utility.GetDefaultPrinter();
            int _tAmt = 0;
            Transaction transactionObj = (from t in entity.Transactions where t.Id == transactionId select t).FirstOrDefault();

            string tranId = transactionObj.Id;
            string pitiMemberName = transactionObj.PitiMemberName;

            List<TransactionDetail> _tdList = (from td in transactionObj.TransactionDetails where td.IsDeleted == false select td).ToList();

            #region for refund
            var _result = entity.Transactions.Where(x => x.ParentId == tranId && x.IsDeleted == false).ToList();

            var _refundIdList = _result.Select(x => x.Id).ToList();

            var _refundDiscountamt = _result.Select(x => x.DiscountAmount).Sum();

            var refundDetailList = entity.TransactionDetails.Where(x => _refundIdList.Contains(x.TransactionId)).ToList();


            int _refundItemDiscAmt = 0;
            foreach (var detail in refundDetailList)
            {
                _refundItemDiscAmt += Convert.ToInt32(detail.UnitPrice * detail.Qty) / 100 * Convert.ToInt32(detail.DiscountRate);
            }

            // int discountAmt = Convert.ToInt32(transactionObj.DiscountAmount - _refundDiscountamt);
            int totalItemDisAmt = 0; int toalDiscAmt = 0;
            #endregion

            //unitpriceTotalCost = 0;
            Int64 _mcDiscountAmt = 0; Int64 _bcDiscountAmt = 0; Int64 totalAmountRep = 0;

            if (transactionObj.PaymentType.Name == "Credit")
            {
                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                //List<TransactionDetail> _tdList = (from td in transactionObj.TransactionDetails where td.IsDeleted == false select td).ToList();
                dsReportTemp.MultiPaymentDataTable multiReport = (dsReportTemp.MultiPaymentDataTable)dsReport.Tables["MultiPayment"];


                foreach (TransactionDetail transaction in PrintDetailList(_tdList))
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.ItemId = transaction.Product.ProductCode;
                    newRow.Name = transaction.Product.Name;
                    newRow.Qty = transaction.Qty.ToString();
                    //newRow.TotalAmount = (int)transaction.TotalAmount; //Edit By ZMH
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
                    //   _tAmt += newRow.TotalAmount;


                    if (_result.Count > 0)
                    {

                        List<TransactionDetail> td = entity.TransactionDetails.Where(x => _refundIdList.Contains(x.TransactionId) && x.ProductId == transaction.ProductId).ToList();

                        if (td.Count != transaction.Qty)
                        {
                            int currentQty = Convert.ToInt32(transaction.Qty - td.Count);
                            newRow.Qty = currentQty.ToString();
                            newRow.TotalAmount = (int)transaction.UnitPrice * currentQty;
                            dtReport.AddItemListRow(newRow);
                            _tAmt += newRow.TotalAmount;
                            totalItemDisAmt += Convert.ToInt32(transaction.TotalAmount) - Convert.ToInt32(transactionObj.DiscountAmount);
                        }

                    }
                    else
                    {
                        _tAmt += newRow.TotalAmount;

                        totalItemDisAmt += Convert.ToInt32(transactionObj.DiscountAmount);
                        dtReport.AddItemListRow(newRow);
                    }


                    // dtReport.AddItemListRow(newRow);
                    // unitpriceTotalCost = (int)transaction.UnitPrice * (int)transaction.Qty;                    
                }

                var data = entity.TransactionPaymentDetails.Where(x => x.TransactionId.Trim() == tranId.Trim()).ToList();
                foreach (var item in data)
                {
                    dsReportTemp.MultiPaymentRow newRow = multiReport.NewMultiPaymentRow();

                    newRow.PaymentName = entity.PaymentMethods.Where(x => x.Id == item.PaymentMethodId).Select(x => x.Name).FirstOrDefault();
                    newRow.Amount = Convert.ToString(item.Amount);
                    multiReport.AddMultiPaymentRow(newRow);
                }
                if (dtReport.Count > 0)
                {

                    string reportPath = "";
                    ReportViewer rv = new ReportViewer();
                    ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                    ReportDataSource rds2 = new ReportDataSource("MultiPayment", dsReport.Tables["MultiPayment"]);

                    reportPath = Application.StartupPath + Utility.GetReportPath("Cash");
                    rv.Reset();
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Add(rds);
                    rv.LocalReport.DataSources.Add(rds2);


                    Utility.Slip_Log(rv);
                    //switch (_defaultPrinter)
                    //{

                    //    case "Slip Printer":
                    //        Utility.Slip_A4_Footer(rv);
                    //        break;
                    //}
                    Utility.Slip_A4_Footer(rv);
                    APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

                    string displaycusName = "";
                    if (pitiMemberName=="")
                    {
                        displaycusName = cus.Name;
                    }
                    else
                    {
                        displaycusName = pitiMemberName;
                    }
                    //ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
                    ReportParameter CustomerName = new ReportParameter("CustomerName", displaycusName);
                    rv.LocalReport.SetParameters(CustomerName);

                    if (Convert.ToInt32(transactionObj.MCDiscountAmt) != 0)
                    {
                        _mcDiscountAmt = Convert.ToInt64(transactionObj.MCDiscountAmt);
                        ReportParameter MCDiscountAmt = new ReportParameter("MCDiscount", "-" + _mcDiscountAmt.ToString());
                        rv.LocalReport.SetParameters(MCDiscountAmt);
                    }

                    else if (Convert.ToInt32(transactionObj.BDDiscountAmt) != 0)
                    {
                        _bcDiscountAmt = Convert.ToInt64(transactionObj.BDDiscountAmt);
                        ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "-" + _bcDiscountAmt.ToString());
                        rv.LocalReport.SetParameters(BCDiscountAmt);
                    }
                    else
                    {
                        ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "0");
                        rv.LocalReport.SetParameters(BCDiscountAmt);
                    }
                    //string _tAmt1 = string.Format("{0:#,##0.00}", _tAmt);
                    //ReportParameter TAmt = new ReportParameter("TAmt", _tAmt1);
                    //rv.LocalReport.SetParameters(TAmt);
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

                    ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
                    rv.LocalReport.SetParameters(TransactionId);

                    APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                    ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                    rv.LocalReport.SetParameters(CounterName);

                    ReportParameter PrintDateTime = new ReportParameter();
                    switch (Utility.GetDefaultPrinter())
                    {
                        case "A4 Printer":
                            PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd-MMM-yyyy"));
                            rv.LocalReport.SetParameters(PrintDateTime);
                            break;
                        case "Slip Printer":
                            PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd/MM/yyyy hh:mm"));
                            rv.LocalReport.SetParameters(PrintDateTime);
                            break;
                    }

                    ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                    rv.LocalReport.SetParameters(CasherName);

                    //ReportParameter TotalAmount = new ReportParameter("TotalAmount", transactionObj.TotalAmount.ToString()); //Edit By ZMH
                    //rv.LocalReport.SetParameters(TotalAmount);
                    //Int64 totalAmountRep = transactionObj.TotalAmount == null ? 0 : Convert.ToInt64(transactionObj.TotalAmount);
                    toalDiscAmt = Convert.ToInt32(transactionObj.DiscountAmount) - Convert.ToInt32(_refundDiscountamt) - _refundItemDiscAmt;
                    totalAmountRep = (_tAmt - _bcDiscountAmt - _mcDiscountAmt - toalDiscAmt);
                    ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                    rv.LocalReport.SetParameters(TotalAmount);

                    ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
                    rv.LocalReport.SetParameters(TaxAmount);

                    if (toalDiscAmt == 0)
                    {
                        ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", toalDiscAmt.ToString());
                        rv.LocalReport.SetParameters(DiscountAmount);
                    }
                    else
                    {
                        ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + toalDiscAmt.ToString());
                        rv.LocalReport.SetParameters(DiscountAmount);
                    }
                    ReportParameter PaidAmount = new ReportParameter("PaidAmount", transactionObj.RecieveAmount.ToString());
                    rv.LocalReport.SetParameters(PaidAmount);

                    //  ReportParameter Change = new ReportParameter("Change",(transactionObj.RecieveAmount - (transactionObj.TotalAmount - ExtraDiscount + ExtraTax)).ToString());//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
                    ReportParameter Change = new ReportParameter("Change", 0.ToString());//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
                    rv.LocalReport.SetParameters(Change);

                    if (Utility.GetDefaultPrinter() == "A4 Printer")
                    {
                        ReportParameter CusAddress = new ReportParameter("CusAddress", cus.Address);
                        rv.LocalReport.SetParameters(CusAddress);
                    }

                    try
                    {
                        ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
                        rv.LocalReport.SetParameters(AvailablePoint);
                    }
                    catch
                    {
                        // MessageBox.Show("Please change Slip Printer in Setting");
                    }
                    ////       PrintDoc.PrintReport(rv, Utility.GetDefaultPrinter());
                    //PrintDoc.PrintReport(rv, "Slip");
                    Utility.Get_Print2(rv);

                }
                else
                {
                    MessageBox.Show("Invoice No." + tranId + "  is already made refund all items.", "mPOS");
                }

            }
            else
            {
                #region [ Print ] for All Payment Methods // TTN


                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                //List<TransactionDetail> _tdList = (from td in transactionObj.TransactionDetails where td.IsDeleted == false select td).ToList();
                dsReportTemp.MultiPaymentDataTable multiReport = (dsReportTemp.MultiPaymentDataTable)dsReport.Tables["MultiPayment"];


                foreach (TransactionDetail transaction in PrintDetailList(_tdList))
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.ItemId = transaction.Product.ProductCode;
                    newRow.Name = transaction.Product.Name;
                    newRow.Qty = transaction.Qty.ToString();
                    //newRow.TotalAmount = (int)transaction.TotalAmount; //Edit By ZMH
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
                    //   _tAmt += newRow.TotalAmount;


                    if (_result.Count > 0)
                    {

                        List<TransactionDetail> td = entity.TransactionDetails.Where(x => _refundIdList.Contains(x.TransactionId) && x.ProductId == transaction.ProductId).ToList();

                        if (td.Count != transaction.Qty)
                        {
                            int currentQty = Convert.ToInt32(transaction.Qty - td.Count);
                            newRow.Qty = currentQty.ToString();
                            newRow.TotalAmount = (int)transaction.UnitPrice * currentQty;
                            dtReport.AddItemListRow(newRow);
                            _tAmt += newRow.TotalAmount;
                            totalItemDisAmt += Convert.ToInt32(transaction.TotalAmount) - Convert.ToInt32(transactionObj.DiscountAmount);
                        }

                    }
                    else
                    {
                        _tAmt += newRow.TotalAmount;

                        totalItemDisAmt += Convert.ToInt32(transactionObj.DiscountAmount);
                        dtReport.AddItemListRow(newRow);
                    }


                    // dtReport.AddItemListRow(newRow);
                    // unitpriceTotalCost = (int)transaction.UnitPrice * (int)transaction.Qty;                    
                }

                var data = entity.TransactionPaymentDetails.Where(x => x.TransactionId.Trim() == tranId.Trim()).ToList();
                foreach (var item in data)
                {
                    dsReportTemp.MultiPaymentRow newRow = multiReport.NewMultiPaymentRow();

                    newRow.PaymentName = entity.PaymentMethods.Where(x => x.Id == item.PaymentMethodId).Select(x => x.Name).FirstOrDefault();
                    newRow.Amount = Convert.ToString(item.Amount);
                    multiReport.AddMultiPaymentRow(newRow);
                }
                if (dtReport.Count > 0)
                {

                    string reportPath = "";
                    ReportViewer rv = new ReportViewer();
                    ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                    ReportDataSource rds2 = new ReportDataSource("MultiPayment", dsReport.Tables["MultiPayment"]);

                    reportPath = Application.StartupPath + Utility.GetReportPath("Cash");
                    rv.Reset();
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Add(rds);
                    rv.LocalReport.DataSources.Add(rds2);


                    Utility.Slip_Log(rv);
                    //switch (_defaultPrinter)
                    //{

                    //    case "Slip Printer":
                    //        Utility.Slip_A4_Footer(rv);
                    //        break;
                    //}
                    Utility.Slip_A4_Footer(rv);
                    APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

                    string displaycusName = "";
                    if (pitiMemberName == "")
                    {
                        displaycusName = cus.Name;
                    }
                    else
                    {
                        displaycusName = pitiMemberName;
                    }

                    //ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
                    ReportParameter CustomerName = new ReportParameter("CustomerName", displaycusName);
                    rv.LocalReport.SetParameters(CustomerName);

                    if (Convert.ToInt32(transactionObj.MCDiscountAmt) != 0)
                    {
                        _mcDiscountAmt = Convert.ToInt64(transactionObj.MCDiscountAmt);
                        ReportParameter MCDiscountAmt = new ReportParameter("MCDiscount", "-" + _mcDiscountAmt.ToString());
                        rv.LocalReport.SetParameters(MCDiscountAmt);
                    }

                    else if (Convert.ToInt32(transactionObj.BDDiscountAmt) != 0)
                    {
                        _bcDiscountAmt = Convert.ToInt64(transactionObj.BDDiscountAmt);
                        ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "-" + _bcDiscountAmt.ToString());
                        rv.LocalReport.SetParameters(BCDiscountAmt);
                    }
                    else
                    {
                        ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "0");
                        rv.LocalReport.SetParameters(BCDiscountAmt);
                    }
                    //string _tAmt1 = string.Format("{0:#,##0.00}", _tAmt);
                    //ReportParameter TAmt = new ReportParameter("TAmt", _tAmt1);
                    //rv.LocalReport.SetParameters(TAmt);
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

                    ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
                    rv.LocalReport.SetParameters(TransactionId);

                    APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                    ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                    rv.LocalReport.SetParameters(CounterName);

                    ReportParameter PrintDateTime = new ReportParameter();
                    switch (Utility.GetDefaultPrinter())
                    {
                        case "A4 Printer":
                            PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd-MMM-yyyy"));
                            rv.LocalReport.SetParameters(PrintDateTime);
                            break;
                        case "Slip Printer":
                            PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd/MM/yyyy hh:mm"));
                            rv.LocalReport.SetParameters(PrintDateTime);
                            break;
                    }

                    ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                    rv.LocalReport.SetParameters(CasherName);

                    //ReportParameter TotalAmount = new ReportParameter("TotalAmount", transactionObj.TotalAmount.ToString()); //Edit By ZMH
                    //rv.LocalReport.SetParameters(TotalAmount);
                    //Int64 totalAmountRep = transactionObj.TotalAmount == null ? 0 : Convert.ToInt64(transactionObj.TotalAmount);
                    toalDiscAmt = Convert.ToInt32(transactionObj.DiscountAmount) - Convert.ToInt32(_refundDiscountamt) - _refundItemDiscAmt;
                    totalAmountRep = (_tAmt - _bcDiscountAmt - _mcDiscountAmt - toalDiscAmt);
                    if (transactionObj.PaymentType.Name == "FOC" || transactionObj.PaymentType.Name == "Tester")
                    {
                        ReportParameter TotalAmount = new ReportParameter("TotalAmount", "0");

                        rv.LocalReport.SetParameters(TotalAmount);
                    }
                    else
                    {
                        ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                        rv.LocalReport.SetParameters(TotalAmount);
                    }


                    ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
                    rv.LocalReport.SetParameters(TaxAmount);

                    if (toalDiscAmt == 0)
                    {
                        ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", toalDiscAmt.ToString());
                        rv.LocalReport.SetParameters(DiscountAmount);
                    }
                    else
                    {
                        ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + toalDiscAmt.ToString());
                        rv.LocalReport.SetParameters(DiscountAmount);
                    }
                    ReportParameter PaidAmount = new ReportParameter("PaidAmount", transactionObj.RecieveAmount.ToString());
                    rv.LocalReport.SetParameters(PaidAmount);

                    //  ReportParameter Change = new ReportParameter("Change",(transactionObj.RecieveAmount - (transactionObj.TotalAmount - ExtraDiscount + ExtraTax)).ToString());//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
                    ReportParameter Change = new ReportParameter("Change", "0"); // TTN // (transactionObj.RecieveAmount - totalAmountRep).ToString());//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
                    rv.LocalReport.SetParameters(Change);

                    if (Utility.GetDefaultPrinter() == "A4 Printer")
                    {
                        ReportParameter CusAddress = new ReportParameter("CusAddress", cus.Address);
                        rv.LocalReport.SetParameters(CusAddress);
                    }

                    try
                    {
                        ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
                        rv.LocalReport.SetParameters(AvailablePoint);
                    }
                    catch
                    {
                        // MessageBox.Show("Please change Slip Printer in Setting");
                    }
                    ////       PrintDoc.PrintReport(rv, Utility.GetDefaultPrinter());
                    //PrintDoc.PrintReport(rv, "Slip");
                    Utility.Get_Print2(rv);

                }
                else
                {
                    MessageBox.Show("Invoice No." + tranId + "  is already made refund all items.", "mPOS");
                }
                #endregion
            }
            //else if (false)
            //{
            //    #region [ Print ] for GiftCard

            //    dsReportTemp dsReport = new dsReportTemp();
            //    dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
            //    //List<TransactionDetail> _tdList = (from td in transactionObj.TransactionDetails where td.IsDeleted == false select td).ToList();



            //    foreach (TransactionDetail transaction in _tdList)
            //    {
            //        dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
            //        newRow.Name = transaction.Product.Name;
            //        newRow.Qty = transaction.Qty.ToString();
            //        //newRow.TotalAmount = (int)transaction.TotalAmount;

            //        newRow.DiscountPercent = transaction.DiscountRate.ToString();
            //        newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; //Edit By ZMH
            //                                                                                // dtReport.AddItemListRow(newRow);
            //       // unitpriceTotalCost = (int)transaction.UnitPrice * (int)transaction.Qty;

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


            //    if (dtReport.Count > 0)
            //    {
            //        string reportPath = "";
            //        ReportViewer rv = new ReportViewer();
            //        ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
            //        reportPath = Application.StartupPath + Utility.GetReportPath("GiftCard");

            //        rv.Reset();
            //        rv.LocalReport.ReportPath = reportPath;
            //        rv.LocalReport.DataSources.Add(rds);

            //        Utility.Slip_Log(rv);
            //        //switch (_defaultPrinter)
            //        //{

            //        //   case "Slip Printer":
            //        //Utility.Slip_A4_Footer(rv);
            //        //     break;
            //        // }

            //        Utility.Slip_A4_Footer(rv);

            //        APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

            //        ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
            //        rv.LocalReport.SetParameters(CustomerName);

            //        if (Convert.ToInt32(transactionObj.MCDiscountAmt) != 0)
            //        {
            //            _mcDiscountAmt = Convert.ToInt64(transactionObj.MCDiscountAmt);
            //            ReportParameter MCDiscountAmt = new ReportParameter("MCDiscount", "-" + _mcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(MCDiscountAmt);
            //        }

            //        else if (Convert.ToInt32(transactionObj.BDDiscountAmt) != 0)
            //        {
            //            _bcDiscountAmt = Convert.ToInt64(transactionObj.BDDiscountAmt);
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "-" + _bcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }
            //        else
            //        {
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "0");
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }

            //        ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
            //        rv.LocalReport.SetParameters(TAmt);

            //        ReportParameter ShopName = new ReportParameter("ShopName", SettingController.ShopName);
            //        rv.LocalReport.SetParameters(ShopName);

            //        ReportParameter BranchName = new ReportParameter("BranchName", SettingController.BranchName);
            //        rv.LocalReport.SetParameters(BranchName);

            //        ReportParameter Phone = new ReportParameter("Phone", SettingController.PhoneNo);
            //        rv.LocalReport.SetParameters(Phone);

            //        ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.OpeningHours);
            //        rv.LocalReport.SetParameters(OpeningHours);

            //        ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
            //        rv.LocalReport.SetParameters(TransactionId);

            //        APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

            //        ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
            //        rv.LocalReport.SetParameters(CounterName);

            //        ReportParameter PrintDateTime = new ReportParameter();
            //        switch (Utility.GetDefaultPrinter())
            //        {
            //            case "A4 Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd-MMM-yyyy"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //            case "Slip Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd/MM/yyyy hh:mm"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //        }

            //        ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
            //        rv.LocalReport.SetParameters(CasherName);

            //        //Int64 totalAmountRep = transactionObj.TotalAmount == null ? 0 : Convert.ToInt64(transactionObj.TotalAmount); //Edit By ZMH


            //        //totalAmountRep = (_tAmt + _bcDiscountAmt + _mcDiscountAmt - Convert.ToInt32(transactionObj.DiscountAmount));
            //        //ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
            //        //rv.LocalReport.SetParameters(TotalAmount);

            //        totalAmountRep = Convert.ToInt32(transactionObj.TotalAmount) - Convert.ToInt32(transactionObj.GiftCardAmount);
            //        ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
            //        rv.LocalReport.SetParameters(TotalAmount);

            //        Int64 GiftCardAmt = Convert.ToInt32(transactionObj.GiftCardAmount);
            //        ReportParameter usedGiftCardAmt = new ReportParameter("UsedGiftCardAmt", GiftCardAmt.ToString());
            //        rv.LocalReport.SetParameters(usedGiftCardAmt);

            //        ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
            //        rv.LocalReport.SetParameters(TaxAmount);

            //        if (Convert.ToInt32(transactionObj.DiscountAmount) == 0)
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", transactionObj.DiscountAmount.ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }
            //        else
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + transactionObj.DiscountAmount.ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }

            //        ReportParameter PaidAmount = new ReportParameter("PaidAmount", transactionObj.RecieveAmount.ToString());
            //        rv.LocalReport.SetParameters(PaidAmount);

            //        ReportParameter Change = new ReportParameter("Change", (transactionObj.RecieveAmount - totalAmountRep).ToString());
            //        rv.LocalReport.SetParameters(Change);

            //        ReportParameter GiftCardNo = new ReportParameter("GiftCardNo", transactionObj.GiftCardId.ToString());
            //        rv.LocalReport.SetParameters(GiftCardNo);

            //        if (Utility.GetDefaultPrinter() == "A4 Printer")
            //        {
            //            ReportParameter CusAddress = new ReportParameter("CusAddress", transactionObj.Customer.Address);
            //            rv.LocalReport.SetParameters(CusAddress);
            //        }

            //        ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
            //        rv.LocalReport.SetParameters(AvailablePoint);

            //        ////  PrintDoc.PrintReport(rv,Utility.GetDefaultPrinter());
            //        Utility.Get_Print2(rv);


            //    }
            //    else
            //    {
            //        MessageBox.Show("Invoice No." + tranId + "  is already made refund all items.", "mPOS");
            //    }
            //    #endregion
            //}
            //else if (transactionObj.PaymentType.Name == "Cash")
            //{
            //    #region [ Print ] for Cash


            //    dsReportTemp dsReport = new dsReportTemp();
            //    dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
            //    //List<TransactionDetail> _tdList = (from td in transactionObj.TransactionDetails where td.IsDeleted == false select td).ToList();
            //    dsReportTemp.MultiPaymentDataTable multiReport = (dsReportTemp.MultiPaymentDataTable)dsReport.Tables["MultiPayment"];


            //    foreach (TransactionDetail transaction in PrintDetailList(_tdList))
            //    {
            //        dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
            //        newRow.ItemId = transaction.Product.ProductCode;
            //        newRow.Name = transaction.Product.Name;
            //        newRow.Qty = transaction.Qty.ToString();
            //        //newRow.TotalAmount = (int)transaction.TotalAmount; //Edit By ZMH
            //        newRow.DiscountPercent = transaction.DiscountRate.ToString();
            //        newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; //Edit By ZMH


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
            //        //   _tAmt += newRow.TotalAmount;


            //        if (_result.Count > 0)
            //        {

            //            List<TransactionDetail> td = entity.TransactionDetails.Where(x => _refundIdList.Contains(x.TransactionId) && x.ProductId == transaction.ProductId).ToList();

            //            if (td.Count != transaction.Qty)
            //            {
            //                int currentQty = Convert.ToInt32(transaction.Qty - td.Count);
            //                newRow.Qty = currentQty.ToString();
            //                newRow.TotalAmount = (int)transaction.UnitPrice * currentQty;
            //                dtReport.AddItemListRow(newRow);
            //                _tAmt += newRow.TotalAmount;
            //                totalItemDisAmt += Convert.ToInt32(transaction.TotalAmount) - Convert.ToInt32(transactionObj.DiscountAmount);
            //            }

            //        }
            //        else
            //        {
            //            _tAmt += newRow.TotalAmount;

            //            totalItemDisAmt += Convert.ToInt32(transactionObj.DiscountAmount);
            //            dtReport.AddItemListRow(newRow);
            //        }


            //        // dtReport.AddItemListRow(newRow);
            //        // unitpriceTotalCost = (int)transaction.UnitPrice * (int)transaction.Qty;                    
            //    }

            //    var data = entity.TransactionPaymentDetails.Where(x => x.TransactionId.Trim() == tranId.Trim()).ToList();
            //    foreach (var item in data)
            //    {
            //        dsReportTemp.MultiPaymentRow newRow = multiReport.NewMultiPaymentRow();

            //        newRow.PaymentName = entity.PaymentMethods.Where(x => x.Id == item.PaymentMethodId).Select(x => x.Name).FirstOrDefault();
            //        newRow.Amount = Convert.ToString(item.Amount);
            //        multiReport.AddMultiPaymentRow(newRow);
            //    }
            //    if (dtReport.Count > 0)
            //    {

            //        string reportPath = "";
            //        ReportViewer rv = new ReportViewer();
            //        ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
            //        ReportDataSource rds2 = new ReportDataSource("MultiPayment", dsReport.Tables["MultiPayment"]);

            //        reportPath = Application.StartupPath + Utility.GetReportPath("Cash");
            //        rv.Reset();
            //        rv.LocalReport.ReportPath = reportPath;
            //        rv.LocalReport.DataSources.Add(rds);
            //        rv.LocalReport.DataSources.Add(rds2);


            //        Utility.Slip_Log(rv);
            //        //switch (_defaultPrinter)
            //        //{

            //        //    case "Slip Printer":
            //        //        Utility.Slip_A4_Footer(rv);
            //        //        break;
            //        //}
            //        Utility.Slip_A4_Footer(rv);
            //        APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

            //        ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
            //        rv.LocalReport.SetParameters(CustomerName);

            //        if (Convert.ToInt32(transactionObj.MCDiscountAmt) != 0)
            //        {
            //            _mcDiscountAmt = Convert.ToInt64(transactionObj.MCDiscountAmt);
            //            ReportParameter MCDiscountAmt = new ReportParameter("MCDiscount", "-" + _mcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(MCDiscountAmt);
            //        }

            //        else if (Convert.ToInt32(transactionObj.BDDiscountAmt) != 0)
            //        {
            //            _bcDiscountAmt = Convert.ToInt64(transactionObj.BDDiscountAmt);
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "-" + _bcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }
            //        else
            //        {
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "0");
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }
            //        //string _tAmt1 = string.Format("{0:#,##0.00}", _tAmt);
            //        //ReportParameter TAmt = new ReportParameter("TAmt", _tAmt1);
            //        //rv.LocalReport.SetParameters(TAmt);
            //        ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
            //        rv.LocalReport.SetParameters(TAmt);

            //        ReportParameter ShopName = new ReportParameter("ShopName", SettingController.ShopName);
            //        rv.LocalReport.SetParameters(ShopName);

            //        ReportParameter BranchName = new ReportParameter("BranchName", SettingController.BranchName);
            //        rv.LocalReport.SetParameters(BranchName);

            //        ReportParameter Phone = new ReportParameter("Phone", SettingController.PhoneNo);
            //        rv.LocalReport.SetParameters(Phone);

            //        ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.OpeningHours);
            //        rv.LocalReport.SetParameters(OpeningHours);

            //        ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
            //        rv.LocalReport.SetParameters(TransactionId);

            //        APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

            //        ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
            //        rv.LocalReport.SetParameters(CounterName);

            //        ReportParameter PrintDateTime = new ReportParameter();
            //        switch (Utility.GetDefaultPrinter())
            //        {
            //            case "A4 Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd-MMM-yyyy"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //            case "Slip Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd/MM/yyyy hh:mm"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //        }

            //        ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
            //        rv.LocalReport.SetParameters(CasherName);

            //        //ReportParameter TotalAmount = new ReportParameter("TotalAmount", transactionObj.TotalAmount.ToString()); //Edit By ZMH
            //        //rv.LocalReport.SetParameters(TotalAmount);
            //        //Int64 totalAmountRep = transactionObj.TotalAmount == null ? 0 : Convert.ToInt64(transactionObj.TotalAmount);
            //        toalDiscAmt = Convert.ToInt32(transactionObj.DiscountAmount) - Convert.ToInt32(_refundDiscountamt) - _refundItemDiscAmt;
            //        totalAmountRep = (_tAmt - _bcDiscountAmt - _mcDiscountAmt - toalDiscAmt);
            //        ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
            //        rv.LocalReport.SetParameters(TotalAmount);

            //        ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
            //        rv.LocalReport.SetParameters(TaxAmount);

            //        if (toalDiscAmt == 0)
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", toalDiscAmt.ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }
            //        else
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + toalDiscAmt.ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }
            //        ReportParameter PaidAmount = new ReportParameter("PaidAmount", transactionObj.RecieveAmount.ToString());
            //        rv.LocalReport.SetParameters(PaidAmount);

            //        //  ReportParameter Change = new ReportParameter("Change",(transactionObj.RecieveAmount - (transactionObj.TotalAmount - ExtraDiscount + ExtraTax)).ToString());//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
            //        ReportParameter Change = new ReportParameter("Change", (transactionObj.RecieveAmount - totalAmountRep).ToString());//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
            //        rv.LocalReport.SetParameters(Change);

            //        if (Utility.GetDefaultPrinter() == "A4 Printer")
            //        {
            //            ReportParameter CusAddress = new ReportParameter("CusAddress", cus.Address);
            //            rv.LocalReport.SetParameters(CusAddress);
            //        }

            //        try
            //        {
            //            ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
            //            rv.LocalReport.SetParameters(AvailablePoint);
            //        }
            //        catch
            //        {
            //            // MessageBox.Show("Please change Slip Printer in Setting");
            //        }
            //        ////       PrintDoc.PrintReport(rv, Utility.GetDefaultPrinter());
            //        //PrintDoc.PrintReport(rv, "Slip");
            //        Utility.Get_Print2(rv);

            //    }
            //    else
            //    {
            //        MessageBox.Show("Invoice No." + tranId + "  is already made refund all items.", "mPOS");
            //    }
            //    #endregion
            //}
            //else if (false)
            //{
            //    #region [ Print ] for MPU


            //    dsReportTemp dsReport = new dsReportTemp();
            //    dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
            //    //List<TransactionDetail> _tdList = (from td in transactionObj.TransactionDetails where td.IsDeleted == false select td).ToList();

            //    foreach (TransactionDetail transaction in _tdList)
            //    {
            //        dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
            //        newRow.Name = transaction.Product.Name;
            //        newRow.Qty = transaction.Qty.ToString();
            //        //newRow.TotalAmount = (int)transaction.TotalAmount; //Edit By ZMH
            //        newRow.DiscountPercent = transaction.DiscountRate.ToString();
            //        newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; //Edit By ZMH

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
            //        //   unitpriceTotalCost = (int)transaction.UnitPrice * (int)transaction.Qty;
            //    }

            //    if (dtReport.Count > 0)
            //    {
            //        string reportPath = "";
            //        ReportViewer rv = new ReportViewer();
            //        ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
            //        reportPath = Application.StartupPath + Utility.GetReportPath("MPU");
            //        rv.Reset();
            //        rv.LocalReport.ReportPath = reportPath;
            //        rv.LocalReport.DataSources.Add(rds);

            //        Utility.Slip_Log(rv);
            //        ////switch (_defaultPrinter)
            //        ////{

            //        ////    case "Slip Printer":
            //        ////        Utility.Slip_A4_Footer(rv);
            //        ////        break;
            //        ////}

            //        Utility.Slip_A4_Footer(rv);

            //        APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

            //        ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
            //        rv.LocalReport.SetParameters(CustomerName);

            //        if (Convert.ToInt32(transactionObj.MCDiscountAmt) != 0)
            //        {
            //            _mcDiscountAmt = Convert.ToInt64(transactionObj.MCDiscountAmt);
            //            ReportParameter MCDiscountAmt = new ReportParameter("MCDiscount", "-" + _mcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(MCDiscountAmt);
            //        }

            //        else if (Convert.ToInt32(transactionObj.BDDiscountAmt) != 0)
            //        {
            //            _bcDiscountAmt = Convert.ToInt64(transactionObj.BDDiscountAmt);
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "-" + _bcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }
            //        else
            //        {
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "0");
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }

            //        ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
            //        rv.LocalReport.SetParameters(TAmt);

            //        ReportParameter ShopName = new ReportParameter("ShopName", SettingController.ShopName);
            //        rv.LocalReport.SetParameters(ShopName);

            //        ReportParameter BranchName = new ReportParameter("BranchName", SettingController.BranchName);
            //        rv.LocalReport.SetParameters(BranchName);

            //        ReportParameter Phone = new ReportParameter("Phone", SettingController.PhoneNo);
            //        rv.LocalReport.SetParameters(Phone);

            //        ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.OpeningHours);
            //        rv.LocalReport.SetParameters(OpeningHours);

            //        ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
            //        rv.LocalReport.SetParameters(TransactionId);

            //        APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

            //        ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
            //        rv.LocalReport.SetParameters(CounterName);
            //        ReportParameter PrintDateTime = new ReportParameter();
            //        switch (Utility.GetDefaultPrinter())
            //        {
            //            case "A4 Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd-MMM-yyyy"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //            case "Slip Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd/MM/yyyy hh:mm"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //        }


            //        ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
            //        rv.LocalReport.SetParameters(CasherName);


            //        totalAmountRep = (_tAmt + _bcDiscountAmt + _mcDiscountAmt - Convert.ToInt32(transactionObj.DiscountAmount));
            //        ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
            //        rv.LocalReport.SetParameters(TotalAmount);

            //        ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
            //        rv.LocalReport.SetParameters(TaxAmount);

            //        if (Convert.ToInt32(transactionObj.DiscountAmount) == 0)
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", transactionObj.DiscountAmount.ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }
            //        else
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + transactionObj.DiscountAmount.ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }

            //        ReportParameter PaidAmount = new ReportParameter("PaidAmount", transactionObj.RecieveAmount.ToString());
            //        rv.LocalReport.SetParameters(PaidAmount);

            //        ReportParameter Change = new ReportParameter("Change", "0");//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
            //        rv.LocalReport.SetParameters(Change);

            //        if (Utility.GetDefaultPrinter() == "A4 Printer")
            //        {
            //            ReportParameter CusAddress = new ReportParameter("CusAddress", transactionObj.Customer.Address);
            //            rv.LocalReport.SetParameters(CusAddress);
            //        }
            //        ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
            //        rv.LocalReport.SetParameters(AvailablePoint);

            //        ////  PrintDoc.PrintReport(rv, Utility.GetDefaultPrinter());
            //        Utility.Get_Print2(rv);


            //    }
            //    else
            //    {
            //        MessageBox.Show("Invoice No." + tranId + "  is already made refund all items.", "mPOS");
            //    }
            //    #endregion
            //}

            //else if (false)
            //{
            //    #region [ Print ] for FOC


            //    dsReportTemp dsReport = new dsReportTemp();
            //    dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
            //    //List<TransactionDetail> _tdList = (from td in transactionObj.TransactionDetails where td.IsDeleted == false select td).ToList();

            //    foreach (TransactionDetail transaction in _tdList)
            //    {
            //        dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
            //        newRow.Name = transaction.Product.Name;
            //        newRow.Qty = transaction.Qty.ToString();
            //        //newRow.TotalAmount = (int)transaction.TotalAmount; //Edit By ZMH
            //        newRow.DiscountPercent = transaction.DiscountRate.ToString();
            //        newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; //Edit By ZMH
            //        switch (Utility.GetDefaultPrinter())
            //        {
            //            case "A4 Printer":
            //                //newRow.UnitPrice = transaction.UnitPrice.ToString();
            //                newRow.UnitPrice = transaction.SellingPrice.ToString();
            //                break;
            //            case "Slip Printer":
            //                //  newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
            //                newRow.UnitPrice = "1@" + transaction.SellingPrice.ToString();
            //                break;
            //        }

            //        _tAmt += newRow.TotalAmount;

            //        dtReport.AddItemListRow(newRow);
            //    }

            //    if (dtReport.Count > 0)
            //    {
            //        string reportPath = "";
            //        ReportViewer rv = new ReportViewer();
            //        ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
            //        reportPath = Application.StartupPath + Utility.GetReportPath("FOC");
            //        rv.Reset();
            //        rv.LocalReport.ReportPath = reportPath;
            //        rv.LocalReport.DataSources.Add(rds);

            //        Utility.Slip_Log(rv);
            //        //switch (_defaultPrinter)
            //        //{

            //        //    case "Slip Printer":
            //        //        Utility.Slip_A4_Footer(rv);
            //        //        break;
            //        //}
            //        Utility.Slip_A4_Footer(rv);

            //        APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

            //        ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
            //        rv.LocalReport.SetParameters(CustomerName);

            //        if (Convert.ToInt32(transactionObj.MCDiscountAmt) != 0)
            //        {
            //            _mcDiscountAmt = Convert.ToInt64(transactionObj.MCDiscountAmt);
            //            ReportParameter MCDiscountAmt = new ReportParameter("MCDiscount", "-" + _mcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(MCDiscountAmt);
            //        }

            //        else if (Convert.ToInt32(transactionObj.BDDiscountAmt) != 0)
            //        {
            //            _bcDiscountAmt = Convert.ToInt64(transactionObj.BDDiscountAmt);
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "-" + _bcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }
            //        else
            //        {
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "0");
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }

            //        ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
            //        rv.LocalReport.SetParameters(TAmt);

            //        ReportParameter ShopName = new ReportParameter("ShopName", SettingController.ShopName);
            //        rv.LocalReport.SetParameters(ShopName);

            //        ReportParameter BranchName = new ReportParameter("BranchName", SettingController.BranchName);
            //        rv.LocalReport.SetParameters(BranchName);

            //        ReportParameter Phone = new ReportParameter("Phone", SettingController.PhoneNo);
            //        rv.LocalReport.SetParameters(Phone);

            //        ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.OpeningHours);
            //        rv.LocalReport.SetParameters(OpeningHours);

            //        ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
            //        rv.LocalReport.SetParameters(TransactionId);

            //        APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

            //        ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
            //        rv.LocalReport.SetParameters(CounterName);

            //        ReportParameter PrintDateTime = new ReportParameter();
            //        switch (Utility.GetDefaultPrinter())
            //        {
            //            case "A4 Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd-MMM-yyyy"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //            case "Slip Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd/MM/yyyy hh:mm"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //        }

            //        ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
            //        rv.LocalReport.SetParameters(CasherName);

            //        totalAmountRep = (_tAmt + _bcDiscountAmt + _mcDiscountAmt - Convert.ToInt32(transactionObj.DiscountAmount));
            //        ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
            //        rv.LocalReport.SetParameters(TotalAmount);

            //        ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
            //        rv.LocalReport.SetParameters(TaxAmount);

            //        if (Convert.ToInt32(transactionObj.DiscountAmount) == 0)
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", Convert.ToInt32(transactionObj.DiscountAmount).ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }
            //        else
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + Convert.ToInt32(transactionObj.DiscountAmount).ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }

            //        ReportParameter PaidAmount = new ReportParameter("PaidAmount", transactionObj.RecieveAmount.ToString());
            //        rv.LocalReport.SetParameters(PaidAmount);

            //        //ReportParameter Change = new ReportParameter("Change", (transactionObj.RecieveAmount - (transactionObj.TotalAmount - ExtraDiscount + ExtraTax)).ToString());//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
            //        ReportParameter Change = new ReportParameter("Change", "0");//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
            //        rv.LocalReport.SetParameters(Change);

            //        if (Utility.GetDefaultPrinter() == "A4 Printer")
            //        {
            //            ReportParameter CusAddress = new ReportParameter("CusAddress", transactionObj.Customer.Address);
            //            rv.LocalReport.SetParameters(CusAddress);
            //        }


            //        ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
            //        rv.LocalReport.SetParameters(AvailablePoint);
            //        //// PrintDoc.PrintReport(rv, Utility.GetDefaultPrinter());
            //        Utility.Get_Print2(rv);

            //    }
            //    else
            //    {
            //        MessageBox.Show("Invoice No." + tranId + "  is already made refund all items.", "mPOS");
            //    }
            //    #endregion
            //}
            //else if (false)
            //{
            //    #region [ Print ] for Tester


            //    dsReportTemp dsReport = new dsReportTemp();
            //    dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
            //    //List<TransactionDetail> _tdList = (from td in transactionObj.TransactionDetails where td.IsDeleted == false select td).ToList();

            //    foreach (TransactionDetail transaction in _tdList)
            //    {
            //        dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
            //        newRow.Name = transaction.Product.Name;
            //        newRow.Qty = transaction.Qty.ToString();
            //        //newRow.TotalAmount = (int)transaction.TotalAmount; //Edit By ZMH
            //        newRow.DiscountPercent = transaction.DiscountRate.ToString();
            //        newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; //Edit By ZMH
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

            //    if (dtReport.Count > 0)
            //    {
            //        string reportPath = "";
            //        ReportViewer rv = new ReportViewer();
            //        ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
            //        reportPath = Application.StartupPath + Utility.GetReportPath("Tester");
            //        rv.Reset();
            //        rv.LocalReport.ReportPath = reportPath;
            //        rv.LocalReport.DataSources.Add(rds);

            //        Utility.Slip_Log(rv);
            //        ////switch (_defaultPrinter)
            //        ////{

            //        ////    case "Slip Printer":
            //        ////        Utility.Slip_A4_Footer(rv);
            //        ////        break;
            //        ////}

            //        Utility.Slip_A4_Footer(rv);

            //        APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

            //        ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Name);
            //        rv.LocalReport.SetParameters(CustomerName);

            //        if (Convert.ToInt32(transactionObj.MCDiscountAmt) != 0)
            //        {
            //            _mcDiscountAmt = Convert.ToInt64(transactionObj.MCDiscountAmt);
            //            ReportParameter MCDiscountAmt = new ReportParameter("MCDiscount", "-" + _mcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(MCDiscountAmt);
            //        }

            //        else if (Convert.ToInt32(transactionObj.BDDiscountAmt) != 0)
            //        {
            //            _bcDiscountAmt = Convert.ToInt64(transactionObj.BDDiscountAmt);
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "-" + _bcDiscountAmt.ToString());
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }
            //        else
            //        {
            //            ReportParameter BCDiscountAmt = new ReportParameter("MCDiscount", "0");
            //            rv.LocalReport.SetParameters(BCDiscountAmt);
            //        }


            //        ReportParameter Title = new ReportParameter("Title", "TESTER SALE INVOICE");
            //        rv.LocalReport.SetParameters(Title);

            //        ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
            //        rv.LocalReport.SetParameters(TAmt);

            //        ReportParameter ShopName = new ReportParameter("ShopName", SettingController.ShopName);
            //        rv.LocalReport.SetParameters(ShopName);

            //        ReportParameter BranchName = new ReportParameter("BranchName", SettingController.BranchName);
            //        rv.LocalReport.SetParameters(BranchName);

            //        ReportParameter Phone = new ReportParameter("Phone", SettingController.PhoneNo);
            //        rv.LocalReport.SetParameters(Phone);

            //        ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.OpeningHours);
            //        rv.LocalReport.SetParameters(OpeningHours);

            //        ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
            //        rv.LocalReport.SetParameters(TransactionId);

            //        APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

            //        ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
            //        rv.LocalReport.SetParameters(CounterName);

            //        ReportParameter PrintDateTime = new ReportParameter();
            //        switch (Utility.GetDefaultPrinter())
            //        {
            //            case "A4 Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd-MMM-yyyy"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //            case "Slip Printer":
            //                PrintDateTime = new ReportParameter("PrintDateTime", Convert.ToDateTime(transactionObj.DateTime).ToString("dd/MM/yyyy hh:mm"));
            //                rv.LocalReport.SetParameters(PrintDateTime);
            //                break;
            //        }

            //        ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
            //        rv.LocalReport.SetParameters(CasherName);

            //        totalAmountRep = (_tAmt + _bcDiscountAmt + _mcDiscountAmt - Convert.ToInt32(transactionObj.DiscountAmount));
            //        ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
            //        rv.LocalReport.SetParameters(TotalAmount);

            //        ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
            //        rv.LocalReport.SetParameters(TaxAmount);

            //        if (Convert.ToInt32(transactionObj.DiscountAmount) == 0)
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", Convert.ToInt32(transactionObj.DiscountAmount).ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }
            //        else
            //        {
            //            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + Convert.ToInt32(transactionObj.DiscountAmount).ToString());
            //            rv.LocalReport.SetParameters(DiscountAmount);
            //        }

            //        ReportParameter PaidAmount = new ReportParameter("PaidAmount", transactionObj.RecieveAmount.ToString());
            //        rv.LocalReport.SetParameters(PaidAmount);

            //        //ReportParameter Change = new ReportParameter("Change", (transactionObj.RecieveAmount - (transactionObj.TotalAmount - ExtraDiscount + ExtraTax)).ToString());//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
            //        ReportParameter Change = new ReportParameter("Change", "0");//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
            //        rv.LocalReport.SetParameters(Change);

            //        if (Utility.GetDefaultPrinter() == "A4 Printer")
            //        {
            //            ReportParameter CusAddress = new ReportParameter("CusAddress", transactionObj.Customer.Address);
            //            rv.LocalReport.SetParameters(CusAddress);
            //        }

            //        ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
            //        rv.LocalReport.SetParameters(AvailablePoint);
            //        ////PrintDoc.PrintReport(rv, Utility.GetDefaultPrinter());
            //        Utility.Get_Print2(rv);


            //    }
            //    else
            //    {
            //        MessageBox.Show("Invoice No." + tranId + "  is already made refund all items.", "mPOS");
            //    }
            //    #endregion
            //}
        }

        private void dgvTransactionDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {


                int currentTransactionDetailId = Convert.ToInt32(dgvTransactionDetail.Rows[e.RowIndex].Cells[ColId.Index].Value.ToString());
                bool IsSame = false;
                //Delete the record and add delete log
                if (e.ColumnIndex == colDelete.Index)
                {
                    if (isExported)
                    {
                        MessageBox.Show("You can't delete SAP exported transaction!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    //int iDis = 0;
                    //if (transactionObject.DiscountAmount != null)
                    //{
                    //    iDis = (int)transactionObject.DiscountAmount;
                    //}
                    //if (transactionObject.GiftCardAmount != null)
                    //{
                    //    iDis += (int)transactionObject.GiftCardAmount;
                    //}
                    if (transactionObject.Customer != null)
                    {
                        bool res = Utility.CanRemoveSaleTransaction(transactionObject,transactionObject.PaymentTypeId, transactionObject.TransactionDetails.AsQueryable(), CustomerId, transactionObject.Customer);
                        if (res == false)
                        {
                            return;
                        }
                    }



                    if (dgvTransactionDetail.Rows[e.RowIndex].Cells[ColQty.Index].Value.ToString() == dgvTransactionDetail.Rows[e.RowIndex].Cells[colRefundQty.Index].Value.ToString())
                    {
                        dgvTransactionDetail.Rows[e.RowIndex].Cells[colDelete.Index].ReadOnly = true;
                        return;
                    }

                    if (!DeleteLink)
                    {
                        dgvTransactionDetail.Rows[e.RowIndex].Cells[colDelete.Index].ReadOnly = true;
                        return;
                    }

                    APP_Data.TransactionDetail tdOBj = new TransactionDetail();

                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        tdOBj = entity.TransactionDetails.Where(x => x.Id == currentTransactionDetailId).FirstOrDefault();

                        APP_Data.TransactionDetail _isConsignmentPaidTranList = entity.TransactionDetails.Where(x => x.Id == currentTransactionDetailId && x.IsDeleted == false && x.IsConsignmentPaid == true).FirstOrDefault();

                        if (transactionObject != null)
                        {
                            // TransactionDetail td = entity.TransactionDetails.Where(x => x.TransactionId == tObj.Id).FirstOrDefault();
                            string proCode = dgvTransactionDetail.Rows[e.RowIndex].Cells[ColProductCode.Index].Value.ToString();

                            var proId = entity.Products.Where(x => x.ProductCode == proCode).Select(x => x.Id).FirstOrDefault();
                            List<TransactionDetail> td = entity.TransactionDetails.Where(x => x.TransactionId == transactionObject.Id && x.ProductId == proId).ToList();
                            if (td.Count > 0)
                            {
                                //if (td.ProductId == tdOBj.ProductId)
                                //{
                                IsSame = true;
                                //  }
                            }
                        }
                        if (IsSame)
                        {
                            MessageBox.Show("This transaction detail already made refund. So it can't be delete!");
                            return;
                        }
                        else if (_isConsignmentPaidTranList != null)
                        {
                            MessageBox.Show("This transaction detail already made  Consignment Settlement. So it can't be delete!");
                            return;
                        }
                        else
                        {
                            if (dgvTransactionDetail.Rows.Count <= 1)
                            {
                                DialogResult result2 = MessageBox.Show("You have only one record!.If you delete this,system will automatically delete Transaction of this record", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                if (result2.Equals(DialogResult.OK))
                                {
                                    TransactionDetail ts = entity.TransactionDetails.Where(x => x.Id == currentTransactionDetailId).FirstOrDefault();
                                    Transaction t = entity.Transactions.Where(x => x.Id == ts.TransactionId).FirstOrDefault();

                                    t.IsDeleted = true;
                                    foreach (TransactionDetail td in t.TransactionDetails)
                                    {
                                        //td.IsDeleted = false;
                                        td.IsDeleted = true;
                                    }
                                    Utility.GiftCardIsBack(t);
                                    #region add qty in stock filling from sap KHS
                                    Utility.AddProductAvailableQty(entity, (long)ts.ProductId, ts.BatchNo, (int)ts.Qty);
                                    #endregion
                                    // update Prepaid Transaction id = false   and delete list in useprepaiddebt table
                                    Utility.Plus_PreaidAmt(t);

                                    ts.Product.Qty = ts.Product.Qty + ts.Qty;

                                    //save in stocktransaction

                                    Stock_Transaction st = new Stock_Transaction();
                                    st.ProductId = ts.Product.Id;
                                    Qty -= Convert.ToInt32(ts.Qty);
                                    st.Sale = Qty;
                                    productList.Add(st);
                                    Qty = 0;


                                    Save_SaleQty_ToStockTransaction(productList, date);
                                    productList.Clear();

                                    if (ts.Product.IsWrapper == true)
                                    {
                                        List<WrapperItem> wList = ts.Product.WrapperItems.ToList();
                                        if (wList.Count > 0)
                                        {
                                            foreach (WrapperItem w in wList)
                                            {
                                                Product wpObj = (from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault();
                                                wpObj.Qty = wpObj.Qty + ts.Qty;
                                            }
                                        }
                                    }

                                    //For Purchase 
                                    #region Purchase Delete



                                    List<APP_Data.PurchaseDetailInTransaction> puInTranDetail = entity.PurchaseDetailInTransactions.Where(x => x.TransactionDetailId == ts.Id && x.ProductId == ts.ProductId).ToList();
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

                                            entity.PurchaseDetailInTransactions.Remove(p);
                                            p.Qty = 0;
                                            entity.Entry(p).State = EntityState.Modified;
                                            entity.SaveChanges();
                                        }
                                    }

                                    #endregion




                                    DeleteLog dl = new DeleteLog();
                                    dl.DeletedDate = DateTime.Now;
                                    dl.CounterId = MemberShip.CounterId;
                                    dl.UserId = MemberShip.UserId;
                                    dl.IsParent = true;
                                    dl.TransactionId = t.Id;
                                    //dl.TransactionDetailId = ts.Id;



                                    List<DeleteLog> delist = entity.DeleteLogs.Where(x => x.TransactionId == t.Id && x.TransactionDetailId != null && x.IsParent == false).ToList();

                                    foreach (DeleteLog d in delist)
                                    {
                                        entity.DeleteLogs.Remove(d);
                                    }
                                    entity.DeleteLogs.Add(dl);
                                    entity.SaveChanges();
                                    if (t.Customer.MemberTypeID != null)
                                    {
                                        DateTime transactionDate = entity.Transactions.Where(x => x.Id == ts.TransactionId).Select(x => x.DateTime.Value).FirstOrDefault();
                                        VipCustomer vipCustomer = new VipCustomer();
                                        DateTime lastTwoYears = DateTime.Now.AddYears(-2);
                                        var vipTransactionlist = entity.Transactions.Where(x => x.IsDeleted == false && x.IsActive == true
                                                     && x.CustomerId == t.CustomerId && (x.DateTime >= lastTwoYears && x.DateTime <= DateTime.Now)).ToList();
                                        if (vipTransactionlist.Count > 0)
                                        {
                                            var totalPaidAmount = (int)(vipTransactionlist.Select(x => x.TotalAmount).Sum() - vipTransactionlist.Select(x => x.GiftCardAmount).Sum());
                                            vipCustomer.CustomerCode = t.Customer.CustomerCode;
                                            vipCustomer.LastPaidDate = transactionDate;
                                            vipCustomer.TwoYearsTotalAmount = totalPaidAmount;
                                            vipCustomer.CreatedUserID = MemberShip.UserId;
                                            vipCustomer.CreatedDate = DateTime.Now;
                                            vipCustomer.ShopCode = SettingController.DefaultShop.ShortCode;
                                            entity.VipCustomers.Add(vipCustomer);
                                            entity.SaveChanges();
                                        }
                                    }

                                    LoadData();
                                    this.Close();

                                    if (System.Windows.Forms.Application.OpenForms["TransactionList"] != null)
                                    {
                                        TransactionList newForm = (TransactionList)System.Windows.Forms.Application.OpenForms["TransactionList"];
                                        newForm.LoadData();
                                    }
                                }
                            }
                            else
                            {
                                TransactionDetail ts = entity.TransactionDetails.Where(x => x.Id == currentTransactionDetailId).FirstOrDefault();
                                Transaction t = entity.Transactions.Where(x => x.Id == ts.TransactionId).FirstOrDefault();

                                ts.IsDeleted = true;

                                ts.Product.Qty = ts.Product.Qty + ts.Qty;

                                #region reduce qty in stock filling from sap KHS
                                Utility.AddProductAvailableQty(entity, (long)ts.ProductId, ts.BatchNo, (int)ts.Qty);
                                #endregion
                                //save in stocktransaction

                                Stock_Transaction st = new Stock_Transaction();
                                st.ProductId = ts.Product.Id;
                                Qty -= Convert.ToInt32(ts.Qty);
                                st.Sale = Qty;
                                productList.Add(st);
                                Qty = 0;

                                Save_SaleQty_ToStockTransaction(productList, date);

                                if (ts.Product.IsWrapper == true)
                                {
                                    List<WrapperItem> wList = ts.Product.WrapperItems.ToList();
                                    if (wList.Count > 0)
                                    {
                                        foreach (WrapperItem w in wList)
                                        {
                                            Product wpObj = (from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault();
                                            wpObj.Qty = wpObj.Qty + ts.Qty;
                                        }
                                    }
                                }
                                DeleteLog dl = new DeleteLog();
                                dl.DeletedDate = DateTime.Now;
                                dl.CounterId = MemberShip.CounterId;
                                dl.UserId = MemberShip.UserId;
                                dl.IsParent = false;
                                dl.TransactionId = ts.TransactionId;
                                dl.TransactionDetailId = ts.Id;

                                Transaction ParentTransaction = entity.Transactions.Where(x => x.Id == ts.TransactionId).FirstOrDefault();
                                ParentTransaction.TotalAmount = ParentTransaction.TotalAmount - ts.TotalAmount;

                                int _disAmt = Convert.ToInt32((ts.UnitPrice / 100) * ts.DiscountRate);
                                ParentTransaction.DiscountAmount = Convert.ToInt32(ParentTransaction.DiscountAmount - _disAmt);

                                entity.DeleteLogs.Add(dl);
                                entity.SaveChanges();

                                if (ParentTransaction.Customer.MemberTypeID != null)
                                {
                                    VipCustomer vipCustomer = new VipCustomer();
                                    DateTime lastTwoYears = DateTime.Now.AddYears(-2);
                                    DateTime transactionDate = entity.Transactions.Where(x => x.Id == ts.TransactionId).Select(x => x.DateTime.Value).FirstOrDefault();
                                    var vipTransactionlist = entity.Transactions.Where(x => x.IsDeleted == false && x.IsActive == true
                                                 && x.CustomerId == ParentTransaction.CustomerId && (x.DateTime >= lastTwoYears && x.DateTime <= DateTime.Now)).ToList();
                                    if (vipTransactionlist.Count > 0)
                                    {
                                        var totalPaidAmount = (int)(vipTransactionlist.Select(x => x.TotalAmount).Sum() - vipTransactionlist.Select(x => x.GiftCardAmount).Sum());
                                        vipCustomer.CustomerCode = ParentTransaction.Customer.CustomerCode;
                                        vipCustomer.LastPaidDate = transactionDate;
                                        vipCustomer.TwoYearsTotalAmount = totalPaidAmount;
                                        vipCustomer.MemberType = ParentTransaction.Customer.MemberType.Name;
                                        vipCustomer.CreatedUserID = MemberShip.UserId;
                                        vipCustomer.CreatedDate = DateTime.Now;
                                        vipCustomer.ShopCode = SettingController.DefaultShop.ShortCode;
                                        entity.VipCustomers.Add(vipCustomer);
                                        entity.SaveChanges();
                                    }
                                }

                                //For Purchase 
                                #region Purchase Delete



                                List<APP_Data.PurchaseDetailInTransaction> puInTranDetail = entity.PurchaseDetailInTransactions.Where(x => x.TransactionDetailId == ts.Id && x.ProductId == ts.ProductId).ToList();
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

                                        entity.PurchaseDetailInTransactions.Remove(p);
                                        entity.SaveChanges();
                                    }
                                }

                                #endregion






                                LoadData();
                                if (System.Windows.Forms.Application.OpenForms["TransactionList"] != null)
                                {
                                    TransactionList newForm = (TransactionList)System.Windows.Forms.Application.OpenForms["TransactionList"];
                                    newForm.LoadData();
                                }

                                if (System.Windows.Forms.Application.OpenForms["CreditTransactionList"] != null)
                                {
                                    CreditTransactionList newForm = (CreditTransactionList)System.Windows.Forms.Application.OpenForms["CreditTransactionList"];
                                    newForm.LoadData();
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion

        #region Function
        private void Visible_Prepaid(bool v)
        {
            lblPrevTitle.Visible = v;
            lblOutstandingAmount.Visible = v;
        }

        private void LoadData()
        {
            bool optionvisible = Utility.TransactionDelRefHide(shopid);
            dgvTransactionDetail.Columns[11].Visible = optionvisible;

            dgvTransactionDetail_CustomCellFormatting();
            dgvTransactionDetail.AutoGenerateColumns = false;
            //tlpCredit.Visible = false;
            Visible_Prepaid(false);
            transactionObject = (from t in entity.Transactions where t.Id == transactionId select t).FirstOrDefault();

            lblSalePerson.Text = (transactionObject.User == null) ? "-" : transactionObject.User.Name;
            lblDate.Text = transactionObject.DateTime.Value.ToString("dd-MM-yyyy");

            date = transactionObject.DateTime.Value;
            lblTime.Text = transactionObject.DateTime.Value.ToString("hh:mm");
            lblCustomerName.Text = (transactionObject.Customer == null) ? "-" : transactionObject.Customer.Name;

            dgvPaymentList.DataSource = entity.TransactionPaymentDetails.Where(x => x.TransactionId.Trim() == transactionId.Trim()).ToList();
            List<TransactionDetail> _tdList = new List<TransactionDetail>();

            if (transactionObject.Customer.Name == null)
            {
                cboCustomer.SelectedIndex = 0;
            }
            else
            {
                cboCustomer.Text = transactionObject.Customer.Name;
                CustomerId = (int)transactionObject.CustomerId;
            }

            if (transactionObject.Type == TransactionType.Settlement)
            {
                dgvTransactionDetail.DataSource = "";
                //lRecieveAmunt.Text = transactionObject.RecieveAmount.ToString();
                lblDiscount.Text = "0";
                if (Convert.ToInt32(transactionObject.MCDiscountAmt) != 0)
                {
                    lblMCDiscount.Text = Convert.ToInt32(transactionObject.MCDiscountAmt).ToString();
                }
                else if (Convert.ToInt32(transactionObject.BDDiscountAmt) != 0)
                {
                    lblMCDiscount.Text = Convert.ToInt32(transactionObject.BDDiscountAmt).ToString();
                }

                lblTotal.Text = transactionObject.TotalAmount.ToString();
                lblPaymentMethod1.Text = (transactionObject.PaymentType == null) ? "-" : transactionObject.PaymentType.Name;

                tlpCash.Visible = true;
            }
            else if (transactionObject.Type == TransactionType.Sale || transactionObject.Type == TransactionType.Credit)
            {
                //dgvTransactionDetail.DataSource = transactionObject.TransactionDetails.Where(x=>x.IsDeleted != true).ToList();
                dgvTransactionDetail.DataSource = transactionObject.TransactionDetails.Where(x => x.IsDeleted == delete && x.ProductId != null).ToList();
                // lblRecieveAmunt.Text = transactionObject.RecieveAmount.ToString();
                int discount = 0;
                int tax = 0;

                //List<TransactionDetail> _tdList = (from td in transactionObject.TransactionDetails where td.IsDeleted == false select td).ToList();
                _tdList = (from td in transactionObject.TransactionDetails where td.IsDeleted == delete && td.ProductId != null select td).ToList();
                foreach (TransactionDetail td in _tdList)
                {
                    discount += Convert.ToInt32(((td.UnitPrice) * (td.DiscountRate / 100)) * td.Qty);
                    tax += Convert.ToInt32((td.UnitPrice * (td.TaxRate / 100)) * td.Qty);
                }
                lblDiscount.Text = (transactionObject.DiscountAmount).ToString();
                lblTotalTax.Text = (transactionObject.TaxAmount).ToString();
                lblTotal.Text = transactionObject.TotalAmount.ToString();
                //ExtraDiscount = Convert.ToInt32(transactionObject.DiscountAmount - discount);
                //ExtraTax = Convert.ToInt32(transactionObject.TaxAmount - tax);
                if (Convert.ToInt32(transactionObject.MCDiscountAmt) != 0)
                {
                    lblMCDiscount.Text = Convert.ToInt32(transactionObject.MCDiscountAmt).ToString();
                }
                else if (Convert.ToInt32(transactionObject.BDDiscountAmt) != 0)
                {
                    lblMCDiscount.Text = Convert.ToInt32(transactionObject.BDDiscountAmt).ToString();
                }
                else
                {
                    lblMCDiscount.Text = "0";
                }



                lblPaymentMethod1.Text = (transactionObject.PaymentType == null) ? "-" : transactionObject.PaymentType.Name; ;
                if (transactionObject.PaymentTypeId == 2)
                {
                    List<Transaction> OldOutStandingList = entity.Transactions.Where(x => x.CustomerId == transactionObject.CustomerId).Where(x => x.IsPaid == false).Where(x => x.DateTime < transactionObject.DateTime).ToList().Where(x => x.IsDeleted != true).ToList();

                    long OldOutstandingAmount = 0;

                    foreach (Transaction t in OldOutStandingList)
                    {
                        OldOutstandingAmount += (long)t.TotalAmount - (long)t.RecieveAmount;
                    }
                    long PrepaidDebt = 0;
                    //List<Transaction> PrePaidList = entity.Transactions.Where(x => x.CustomerId == transactionObject.CustomerId).Where(x => x.IsActive == false).Where(x => x.Type == TransactionType.Prepaid).ToList().Where(x => x.IsDeleted != true).ToList();

                    //  foreach(Transaction t in PrePaidList)
                    //  {
                    //      long useAmount = 0;
                    //      if (t.UsePrePaidDebts != null)
                    //      {
                    //          useAmount = (long)t.UsePrePaidDebts.Sum(x => x.UseAmount);
                    //      }
                    //      //PrepaidDebt += Convert.ToInt32(t.RecieveAmount - useAmount);
                    //      PrepaidDebt += Convert.ToInt32(useAmount);
                    //  }
                    if (transactionObject.UsePrePaidDebts != null)
                    {
                        PrepaidDebt += (long)transactionObject.UsePrePaidDebts.Sum(x => x.UseAmount);
                    }
                    if (OldOutstandingAmount > 0)
                    {
                        OldOutstandingAmount -= PrepaidDebt;
                    }
                    //tlpCredit.Visible = true;
                    Visible_Prepaid(true);

                    lblOutstandingAmount.Text = PrepaidDebt.ToString();


                    lblPrevTitle.Text = "Used Prepaid Amount   :";
                    //lblPayableCredit.Text = ((transactionObject.TotalAmount + OldOutstandingAmount) - transactionObject.RecieveAmount).ToString();
                    //lblOutstandingAmount.Text = OldOutstandingAmount.ToString();
                }
                //   //GiftCard
                else if (transactionObject.PaymentTypeId == 3)
                {
                    lblRecieveAmunt.Text = transactionObject.RecieveAmount.ToString();
                    lblAmountFromGiftCard.Visible = true;
                    lblAmountFromGiftcardTitle.Visible = true;
                    lblAmountFromGiftCard.Text = Convert.ToInt32(transactionObject.GiftCardAmount).ToString();
                }
                tlpCash.Visible = true;

            }



            //var totalQty= dgvTransactionDetail.Rows.Cast<DataGridViewRow>().Select(r=>Convert.ToInt32(r.Cells[3].Value)).Sum();
            //var totalUnitPrice= dgvTransactionDetail.Rows.Cast<DataGridViewRow>().Select(r=>Convert.ToInt32(r.Cells[4].Value)).Sum();
            //var totalDiscountRate = _tdList.Select(x => x.DiscountRate).Sum();
            // var list = _tdList.Where(x => Convert.ToInt32(x.DiscountRate) != 0).ToList();
            ////var totalQty = list.Select(x => x.Qty).Sum();
            ////var totalUnitPrice = list.Select(x => x.UnitPrice).Sum();
            ////var totalDiscountRate = list.Select(x => x.DiscountRate).Sum();
            //////foreach (TransactionDetail td in currentTransaction.TransactionDetails)
            //////{
            //////    discount += Convert.ToInt32(((td.UnitPrice) * (td.DiscountRate / 100)) * td.Qty);
            //////    //tax += Convert.ToInt32((td.UnitPrice * (td.TaxRate / 100)) * td.Qty);
            //////}

            ////int itemDiscountAmt = Convert.ToInt32(totalUnitPrice *totalQty)/100 * Convert.ToInt32(totalDiscountRate);

            int itemDiscountAmt = 0;
            foreach (TransactionDetail td in transactionObject.TransactionDetails)
            {
                itemDiscountAmt += Convert.ToInt32(((td.UnitPrice) * (td.DiscountRate / 100)) * td.Qty);

            }

            var _RefunDiscountAmt = entity.Transactions.Where(x => x.ParentId == transactionId && x.IsDeleted == false).Select(r => r.DiscountAmount).Sum();
            var _RefunTotalAmt = entity.Transactions.Where(x => x.ParentId == transactionId && x.IsDeleted == false).Select(r => r.TotalAmount).Sum();
            var refund = dgvTransactionDetail.Rows.Cast<DataGridViewRow>().Select(r => Convert.ToInt32(r.Cells[colRefundCost.Index].Value)).Sum();


            if (refund != 0)
            {

                var TotalRefundAmt = _RefunTotalAmt - _RefunDiscountAmt;
                lblRefundAmt.Text = TotalRefundAmt.ToString();
            }
            else
            {
                lblRefundAmt.Text = "0";
            }

            //if (IsCash)
            //{
            //    lblTotal.Text = (transactionObject.TotalAmount - Convert.ToInt32(lblRefundAmt.Text)).ToString();
            //}
            //else
            //{
            //    lblTotal.Text = (transactionObject.RecieveAmount).ToString();
            //}

            lblRecieveAmunt.Text = transactionObject.RecieveAmount.ToString();

        }



        private void dgvTransactionDetail_CustomCellFormatting()
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            // Transaction Delete
            if (!MemberShip.isAdmin && !controller.TransactionDetail.EditOrDelete)
            {
                dgvTransactionDetail.Columns["colDelete"].Visible = false;
            }

        }
        #endregion

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to update?", "Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {

                Transaction transactionObject = (from t in entity.Transactions where t.Id == transactionId select t).FirstOrDefault();
                transactionObject.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue);
                transactionObject.UpdatedDate = DateTime.Now;
                entity.Entry(transactionObject).State = EntityState.Modified;
                entity.SaveChanges();

                // By SYM // For MemberType of Customer

                #region Member Type
                string minimumAmountofThisMemberType = (from mCardRule in entity.MemberCardRules.AsEnumerable() where mCardRule.IsActive == true orderby int.Parse(mCardRule.RangeFrom) select mCardRule.RangeFrom).FirstOrDefault();

                int memberValidityYear = Convert.ToInt32(SettingController.DefaultMemberValidityYear.ToString());
                int customerID = Convert.ToInt32(cboCustomer.SelectedValue); // customerid, transactionid
                Customer currentCustomer = (from c in entity.Customers where c.Id == customerID select c).FirstOrDefault<Customer>();
                List<MemberCardRule> memberCardRuleList = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true orderby int.Parse(m.RangeFrom) select m).ToList();
                int totalAmount = 0;
                int giftCardTotalAmount = 0;
                int totalRAmount = 0;
                int memberTypeId = 0;

                if (currentCustomer.Name != "Default")
                {
                    if (currentCustomer.MemberTypeID != null)
                    { // aldy member
                        if (transactionObject.DateTime.Value.Date >= currentCustomer.StartDate && transactionObject.DateTime.Value.Date <= currentCustomer.ExpireDate)
                        { // not expired

                            List<Transaction> transactionList = (from transactions in entity.Transactions
                                                                 join c in entity.Customers
                                                                 on transactions.CustomerId equals c.Id
                                                                 where (transactions.CustomerId == customerID) && (c.Id == customerID) && (transactions.IsDeleted == false)
                                                                 && (transactions.IsComplete == true) && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                 && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
                                                                 && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "Refund") && (transactions.Type != "CreditRefund")
                                                                 select transactions).ToList();

                            List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                   join transactions in entity.Transactions
                                                                                   on td.TransactionId equals transactions.ParentId
                                                                                   join c in entity.Customers
                                                                                   on transactions.CustomerId equals c.Id
                                                                                   where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
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
                        else
                        { // aldy expired                        

                            int fromYear = currentCustomer.ExpireDate.Value.Year - memberValidityYear;
                            int fromMonth = currentCustomer.ExpireDate.Value.Month;
                            int fromDay = currentCustomer.ExpireDate.Value.Day;
                            string fromDate = Convert.ToString(fromYear + "/" + fromMonth + "/" + fromDay);
                            DateTime _fromDate = Convert.ToDateTime(fromDate);

                            int _expireYear = currentCustomer.ExpireDate.Value.Year + memberValidityYear;
                            string expireDate = Convert.ToString(_expireYear + "/" + fromMonth + "/" + fromDay);
                            DateTime _expireDate = Convert.ToDateTime(expireDate);

                            if (DateTime.Today.Date >= _fromDate && DateTime.Today.Date <= _expireDate)
                            { // aldy expired but can extend to next year, so not renew
                                int _currentAmount = 0;
                                List<Transaction> transactionList = (from transactions in entity.Transactions
                                                                     join c in entity.Customers
                                                                     on transactions.CustomerId equals c.Id
                                                                     where (transactions.CustomerId == customerID) && (c.Id == customerID) && (transactions.DateTime >= _fromDate)
                                                                     && (transactions.DateTime <= _expireDate) && (transactions.IsDeleted == false)
                                                                     && (transactions.IsComplete == true) && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                     && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "Refund") && (transactions.Type != "CreditRefund")
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

                                _currentAmount = totalAmount - Convert.ToInt32(transactionObject.TotalAmount);
                                if (_currentAmount >= Convert.ToInt32(minimumAmountofThisMemberType))
                                {// this current amount may be a member type
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
                                { // cann't extend to next year, so renew
                                    foreach (MemberCardRule memberCards in memberCardRuleList)
                                    {
                                        if (Convert.ToInt32(memberCards.RangeFrom) <= transactionObject.TotalAmount)
                                        {
                                            memberTypeId = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();

                                        }

                                    }
                                    currentCustomer.MemberTypeID = memberTypeId;
                                    currentCustomer.StartDate = transactionObject.DateTime.Value.Date;
                                    currentCustomer.FirstTime = true;
                                    int year = transactionObject.DateTime.Value.Year + memberValidityYear + 1;
                                    int month = transactionObject.DateTime.Value.Month;
                                    int day = transactionObject.DateTime.Value.Day;
                                    expireDate = Convert.ToString(year + "/" + month + "/" + day);
                                    currentCustomer.ExpireDate = Convert.ToDateTime(expireDate);
                                    currentCustomer.PromoteDate = transactionObject.DateTime.Value.Date;

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
                            else
                            { // cann't extend to next year, so renew

                                foreach (MemberCardRule memberCards in memberCardRuleList)
                                {
                                    if (Convert.ToInt32(memberCards.RangeFrom) <= transactionObject.TotalAmount)
                                    {
                                        memberTypeId = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Id).FirstOrDefault();

                                    }

                                }
                                currentCustomer.MemberTypeID = memberTypeId;
                                currentCustomer.StartDate = transactionObject.DateTime.Value.Date;
                                currentCustomer.FirstTime = true;
                                int year = transactionObject.DateTime.Value.Year + memberValidityYear + 1;
                                int month = transactionObject.DateTime.Value.Month;
                                int day = transactionObject.DateTime.Value.Day;
                                expireDate = Convert.ToString(year + "/" + month + "/" + day);
                                currentCustomer.ExpireDate = Convert.ToDateTime(expireDate);
                                currentCustomer.PromoteDate = transactionObject.DateTime.Value.Date;

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
                    else
                    { // new member condition
                        //List<Transaction> transactionList = (from transactions in entity.Transactions
                        //                                     join c in entity.Customers
                        //                                     on transactions.CustomerId equals c.Id
                        //                                     where (transactions.CustomerId == customerID) && (c.Id == customerID) && (transactions.IsDeleted == false)
                        //                                     select transactions).ToList();

                        //foreach (Transaction t in transactionList)
                        //{
                        //    totalAmount += Convert.ToInt32(t.TotalAmount);
                        //}
                        foreach (MemberCardRule memberCard in memberCardRuleList)
                        {
                            if (Convert.ToInt32(memberCard.RangeFrom) <= transactionObject.TotalAmount)
                            {
                                memberTypeId = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();
                            }
                        }

                        if (memberTypeId != 0)
                        {
                            currentCustomer.MemberTypeID = memberTypeId;
                            currentCustomer.StartDate = transactionObject.DateTime.Value.Date;
                            currentCustomer.FirstTime = true;
                            int year = transactionObject.DateTime.Value.Year + memberValidityYear + 1;
                            int month = transactionObject.DateTime.Value.Month;
                            int day = transactionObject.DateTime.Value.Day;
                            string expireDate = Convert.ToString(year + "/" + month + "/" + day);
                            currentCustomer.ExpireDate = Convert.ToDateTime(expireDate);
                            currentCustomer.PromoteDate = transactionObject.DateTime.Value.Date;

                            entity.SaveChanges();

                        }
                    }
                }

                #endregion

                MessageBox.Show("Successfully Updated!", "Update");
            }
        }

        private void lbAdvanceSearch_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //CustomerSearch form = new CustomerSearch();
            //form.ShowDialog();
        }

        #region for saving Sale Qty in Stock Transaction table
        private void Save_SaleQty_ToStockTransaction(List<Stock_Transaction> productList, DateTime _tranDate)
        {
            int _year, _month;

            _year = _tranDate.Year;
            _month = _tranDate.Day;
            Utility.Sale_Run_Process(_year, _month, productList);
        }
        #endregion

        private void dgvPaymentList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvPaymentList.Rows)
            {
                POSEntities entities = new POSEntities();
                TransactionPaymentDetail transactionDetailObj = (TransactionPaymentDetail)row.DataBoundItem;
                var paymentName = entities.PaymentMethods.Where(x => x.Id == transactionDetailObj.PaymentMethodId).Select(x => x.Name).FirstOrDefault();
                row.Cells[0].Value = paymentName;
                row.Cells[1].Value = transactionDetailObj.Amount;
            }
        }

        private void TransactionDetailForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (System.Windows.Forms.Application.OpenForms["TransactionList"] != null)
            {
                TransactionList newForm = (TransactionList)System.Windows.Forms.Application.OpenForms["TransactionList"];
                newForm.LoadData();
            }
        }


    }
}
