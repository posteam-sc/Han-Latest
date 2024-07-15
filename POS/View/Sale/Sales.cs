using Microsoft.Reporting.WinForms;
using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace POS
{
    public partial class Sales : Form
    {
        #region Variables

        private bool isDraft = false;

        private string DraftId = string.Empty;

        private bool IsBirthday = false;

        private bool IsExportPts = false;

        public decimal? NonConsignProAmt = 0;

        public int? MemberTypeID = 0;

        public decimal? MCDiscountPercent = 0;

        public bool? IsWholeSale = false;

        public int total = 0;
        private static bool deleteAction { get; set; }
        public int CurrentCustomerId = 0;



        public string DiscountType = "";
        public int _rowIndex;
        public static int balance = 0;

        //int Qty = 0;
        List<Stock_Transaction> productList = new List<Stock_Transaction>();
        List<TransactionDetail> PList = new List<TransactionDetail>();

        public int FOCQty = 1;
        public static bool Isduplicate { get; set; }

        List<AvailableProductQtyWithBatch> availablePList = new List<AvailableProductQtyWithBatch>();

        POSEntities entity = new POSEntities();
        public bool IsBackDateExportSuccess { get; set; }
        public bool IsAutoImportSuccess { get; set; }

        static int backMonths4Reset = 24;

        static int MemberOldLevel=0;
        #endregion

        #region Events

        public Sales()
        {
            InitializeComponent();

        }




        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        private void CheckExported()
        {

            if (!IsBackDateExportSuccess || !IsAutoImportSuccess)
            {
                SalesButtonsControl(false);

            }
            else
            {
                // SalesButtonsControl(true);
                if (DataExport.exportStatus == -1)
                {
                    DateTime todayDate = DateTime.Now.Date;
                    bool Unexported = true;
                    // List<Transaction> todayExportedTransactionList = entity.Transactions.Where(x => EntityFunctions.TruncateTime(x.DateTime) == todayDate && x.IsDeleted != true && x.IsExported == true).ToList();
                    APP_Data.ImportExportLog exLog = entity.ImportExportLogs.Where(x => EntityFunctions.TruncateTime(x.ProcessingDateTime) == todayDate && x.Type == "Export").FirstOrDefault();
                    if (exLog != null)
                    {
                        Unexported = false;
                        DataExport.exportStatus = 1;
                    }

                    SalesButtonsControl(Unexported);
                }
                else if (DataExport.exportStatus == 1)
                {
                    SalesButtonsControl(false);
                }

            }

            SAPMenuControl();


        }

        private void SAPMenuControl()
        {
            ((MDIParent)this.ParentForm).importSAPToolStripMenuItem.Enabled =
            ((MDIParent)this.ParentForm).importByProductToolStripMenuItem.Enabled =
            ((MDIParent)this.ParentForm).importAllDataToolStripMenuItem.Enabled =
            ((MDIParent)this.ParentForm).exportSAPToolStripMenuItem.Enabled = IsBackDateExportSuccess;
            ((MDIParent)this.ParentForm).backDateExportToolStripMenuItem.Visible = !IsBackDateExportSuccess;

        }

        private void SalesButtonsControl(bool buttonControl)
        {
            btnPaymentAdd.Enabled = buttonControl;
            btnPaid.Enabled = buttonControl;
            btnSave.Enabled = buttonControl;
            btnLoadDraft.Enabled = buttonControl;
            btnFOC.Enabled = buttonControl;
        }

        #region Hot keys handler
        void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.M)      //  Ctrl + M => Focus Member Id
            {
                cboProductName.DroppedDown = false;
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                txtMEMID.Focus();
            }
            else if (e.Control && e.KeyCode == Keys.E)      // Ctrl + E => Focus DropDown Customer
            {
                cboProductName.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                cboCustomer.DroppedDown = true;
                if (cboCustomer.Focused != true)
                {
                    cboCustomer.Focus();
                }
            }
            else if (e.Control && e.KeyCode == Keys.N) //  Ctrl + N => Click Create New Customer
            {
                cboProductName.DroppedDown = false;
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                btnAddNewCustomer.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.A) // Ctrl + A => Focus Search Product Code Drop Down 
            {
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                cboProductName.DroppedDown = true;
                if (cboProductName.Focused != true)
                {
                    cboProductName.Focus();
                }
            }
            else if (e.Control && e.KeyCode == Keys.H) // Ctrl + H => Click Search 
            {
                cboProductName.DroppedDown = false;
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                btnSearch.PerformClick();
            }
            //else if (e.Control && e.KeyCode == Keys.D) // Ctrl + D => focus discount
            //{
            //    cboProductName.DroppedDown = false;
            //    cboCustomer.DroppedDown = false;
            //    cboPaymentMethod.DroppedDown = false;
            //    txtAdditionalDiscount.Focus();
            //}
            else if (e.Control && e.KeyCode == Keys.Y) // Ctrl + Y => focus Payment Method
            {
                cboProductName.DroppedDown = false;
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = true;
                if (cboPaymentMethod.Focused != true)
                {
                    cboPaymentMethod.Focus();
                }
            }
            else if (e.Control && e.KeyCode == Keys.T) // Ctrl + T => focus c in table
            {
                cboProductName.DroppedDown = false;
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                dgvSalesItem.CurrentCell = dgvSalesItem.Rows[0].Cells[9];
                dgvSalesItem.Focus();
            }
            else if (e.Control && e.KeyCode == Keys.Q) // Ctrl + Q => focus Quantity in table
            {
                cboProductName.DroppedDown = false;
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                dgvSalesItem.CurrentCell = dgvSalesItem.Rows[0].Cells[3];
                dgvSalesItem.Focus();
            }
            //else if (e.Control && e.KeyCode == Keys.C)     // Ctrl + C => Click Cancel
            //{
            //    cboProductName.DroppedDown = false;
            //    cboCustomer.DroppedDown = false;
            //    cboPaymentMethod.DroppedDown = false;
            //    btnCancel.PerformClick();
            //}
            else if (e.Control && e.KeyCode == Keys.P)     // Ctrl + P => Click Paid
            {
                cboProductName.DroppedDown = false;
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                btnPaid.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.S)     // Ctrl + S => Click Save As Draft
            {
                cboProductName.DroppedDown = false;
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                btnSave.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.L)     // Ctrl + L => Click Load As Draft
            {
                cboProductName.DroppedDown = false;
                cboCustomer.DroppedDown = false;
                cboPaymentMethod.DroppedDown = false;
                btnLoadDraft.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.F)     // Ctrl + C => Click FOC
            {
                btnFOC.PerformClick();
            }
        }
        #endregion
        private bool UpdateItem4AvailableProductQtyWithBatch(long ProductID, string BatchNo, int Qty, DataGridViewRow row, bool Foc)
        {
            try
            {
                AvailableProductQtyWithBatch avaP = availablePList.Where(Product => Product.ProductID == ProductID && Product.BatchNo == BatchNo).FirstOrDefault();
                if (avaP != null)
                {
                    int qtyCount = 0;

                    if (Foc)
                    {
                        qtyCount = NormalQtyCount(avaP.ProductID.ToString(), avaP.BatchNo);
                    }
                    else
                    {
                        qtyCount = FOCQtyCount(avaP.ProductID.ToString(), avaP.BatchNo);

                    }
                    if (avaP.AvailableQty >= (Qty - avaP.InUseQty) + qtyCount)//CountInUse(avaP)))
                    {
                        //>= (Qty - Product.InUseQty)
                        avaP.AvailableQty -= Qty - avaP.InUseQty;
                        avaP.InUseQty += Qty - avaP.InUseQty;
                        row.Cells[(int)sCol.colQty].Value = Qty.ToString();
                        return true;
                    }
                    else
                    {
                        if (Foc)
                        {
                            Utility.ShowErrMessage("Current batch is insufficient!", "Current batch's available product quantity is " + (avaP.OrgQty - qtyCount).ToString());

                            row.Cells[(int)sCol.colQty].Value = avaP.OrgQty - qtyCount;
                        }
                        else
                        {

                            Utility.ShowErrMessage("Current batch is insufficient!", "Current batch's available product quantity is " + (avaP.OrgQty - qtyCount).ToString());

                            row.Cells[(int)sCol.colQty].Value = avaP.OrgQty - qtyCount;
                        }
                        avaP.InUseQty = avaP.OrgQty;
                        avaP.AvailableQty = 0;

                        return true;
                    }
                }
                else
                {
                    Utility.ShowErrMessage("No Product!", "Product is not available.");
                    return false;

                }


            }
            catch (Exception ex)
            {
                return false;
                //  Utility.ShowErrMessage("UpdateItem4AvailableProductQtyWithBatch", ex.Message);
            }
        }

        private int NormalQtyCount(string productID, string BatchNo)//CountQtyTotalOnGridWithProductAndBatch
        {
            int totalQty = 0;

            try
            {
                List<int> index = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                                   where r.Cells[(int)sCol.colId].Value != null && !string.IsNullOrEmpty(r.Cells[(int)sCol.colId].Value.ToString())
                                   && r.Cells[(int)sCol.colProductCode].Value != null && r.Cells[(int)sCol.colQty].Value != null
                                   && r.Cells[(int)sCol.colQty].Value.ToString() != "0"
                                   && r.Cells[(int)sCol.colId].Value.ToString() == productID && r.Cells[(int)sCol.colProductBatchNo].Value != null
                                   && !string.IsNullOrEmpty(r.Cells[(int)sCol.colProductBatchNo].Value.ToString())
                                   && r.Cells[(int)sCol.colProductBatchNo].Value.ToString() == BatchNo
                                   && (r.Cells[(int)sCol.colFOC].Value == null || string.IsNullOrEmpty(r.Cells[(int)sCol.colFOC].Value.ToString()))
                                   select r.Index).ToList();
                index.Remove(dgvSalesItem.CurrentCell.RowIndex);
                if (index.Count > 0)
                {

                    foreach (var a in index)
                    {
                        try
                        {
                            totalQty += Convert.ToInt32(dgvSalesItem.Rows[a].Cells[(int)sCol.colQty].Value.ToString());
                        }
                        catch
                        {

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //  Utility.ShowErrMessage("NormalProductQtyCount", ex.Message);
            }
            return totalQty;
        }
        private int FOCQtyCount(string productID, string BatchNo)//CountQtyTotalOnGridWithProductAndBatch
        {
            int totalQty = 0;

            try
            {
                List<int> index = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                                   where r.Cells[(int)sCol.colId].Value != null && !string.IsNullOrEmpty(r.Cells[(int)sCol.colId].Value.ToString())
                                   && r.Cells[(int)sCol.colProductCode].Value != null && r.Cells[(int)sCol.colQty].Value != null
                                   && r.Cells[(int)sCol.colQty].Value.ToString() != "0"
                                   && r.Cells[(int)sCol.colId].Value.ToString() == productID && r.Cells[(int)sCol.colProductBatchNo].Value != null
                                   && !string.IsNullOrEmpty(r.Cells[(int)sCol.colProductBatchNo].Value.ToString())
                                   && r.Cells[(int)sCol.colProductBatchNo].Value.ToString() == BatchNo
                                   && r.Cells[(int)sCol.colFOC].Value != null && r.Cells[(int)sCol.colFOC].Value.ToString() == "FOC"
                                   select r.Index).ToList();

                index.Remove(dgvSalesItem.CurrentCell.RowIndex);
                if (index.Count > 0)
                {

                    foreach (var a in index)
                    {
                        try
                        {
                            totalQty += Convert.ToInt32(dgvSalesItem.Rows[a].Cells[(int)sCol.colQty].Value.ToString());
                        }
                        catch
                        {

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //  Utility.ShowErrMessage("FOCProductQtyCount", ex.Message);
            }
            return totalQty;
        }
        private void AddNew4AvailableProductQtyWithBatch(long ProductID, string BatchNo)
        {
            try
            {
                List<AvailableProductQtyWithBatch> newAvList = null;
                if (string.IsNullOrEmpty(BatchNo))
                {
                    newAvList = (from s in entity.StockFillingFromSAPs
                                 let p = entity.Products.Where(x => x.Id == s.ProductId).FirstOrDefault()
                                 where p.Id == ProductID && s.AvailableQty > 0 && s.IsActive == true
                                 orderby s.ExpireDate ascending
                                 select new AvailableProductQtyWithBatch()
                                 {
                                     ProductID = p.Id,
                                     BatchNo = s.BatchNo,
                                     OrgQty = s.AvailableQty,
                                     AvailableQty = s.AvailableQty,
                                     ExpireDate = s.ExpireDate,
                                     InUseQty = 0
                                 }).ToList();
                }
                else
                {
                    newAvList = (from s in entity.StockFillingFromSAPs
                                 let p = entity.Products.Where(x => x.Id == s.ProductId).FirstOrDefault()
                                 where p.Id == ProductID && s.AvailableQty > 0 && s.IsActive == true && s.BatchNo == BatchNo
                                 orderby s.ExpireDate ascending
                                 select new AvailableProductQtyWithBatch()
                                 {
                                     ProductID = p.Id,
                                     BatchNo = s.BatchNo,
                                     OrgQty = s.AvailableQty,
                                     AvailableQty = s.AvailableQty,
                                     ExpireDate = s.ExpireDate,
                                     InUseQty = 0
                                 }).ToList();
                }
                if (newAvList != null && newAvList.Count() > 0)
                {
                    foreach (AvailableProductQtyWithBatch ap in newAvList)
                    {
                        availablePList.Add(ap);
                    }
                }
                else
                {
                    availablePList.Add(new AvailableProductQtyWithBatch
                    {
                        ProductID = ProductID,
                        BatchNo = null,
                        AvailableQty = 0,
                        InUseQty = 0
                    });
                }
            }
            catch (Exception ex)
            {
                //  Utility.ShowErrMessage("AddNew4AvailableProductQtyWithBatch", ex.Message);
            }
        }

        private void dgvSalesItem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    int col = dgvSalesItem.CurrentCell.ColumnIndex;
                    int row = dgvSalesItem.CurrentCell.RowIndex;

                    if (col == (int)sCol.colDelete) // updated from 9 (TTN)//up khs
                    {
                        deleteAction = true;
                        object deleteProductCode = dgvSalesItem[1, row].Value;

                        //If product code is null, this is just new role without product. Do not need to delete the row.
                        if (deleteProductCode != null)
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (result.Equals(DialogResult.OK))
                            {
                                if (dgvSalesItem.Rows.Count != 0)
                                {
                                    if (dgvSalesItem[(int)sCol.colId, row].Value != null && Convert.ToInt32(dgvSalesItem[(int)sCol.colId, row].Value) > 0) // product ID
                                    {
                                        int currentProductId = Convert.ToInt32(dgvSalesItem[11, row].Value);
                                        Product pro = iTempP.Where(p => p.Id == currentProductId).FirstOrDefault<Product>();
                                        if (pro.IsConsignment == false)
                                        {
                                            int unitPrice = Convert.ToInt32(dgvSalesItem[5, row].Value); // Updated from 4 to 5 (TTN)
                                            int Qty = Convert.ToInt32(dgvSalesItem[(int)sCol.colQty, row].Value);// Updated from 3 to 4 (TTN)
                                            int Tax = Convert.ToInt32(dgvSalesItem[(int)sCol.colTax, row].Value); // Updated from 6 to 7 (TTN)
                                            decimal pricePerProduct = unitPrice * Qty;
                                            //NonConsignProAmt = pricePerProduct + ((pricePerProduct / 100) * pro.Tax.TaxPercent);
                                            NonConsignProAmt = NonConsignProAmt - (pricePerProduct + ((pricePerProduct / 100) * pro.Tax.TaxPercent));


                                        }
                                        RemoveProductByLineFromDataGrid(pro, dgvSalesItem.Rows[row]);


                                    }
                                }
                                if (dgvSalesItem.Rows.Count > 0 && dgvSalesItem.Rows[row] != null && dgvSalesItem.Rows.Count > row)
                                {

                                    dgvSalesItem.Rows.RemoveAt(row);

                                }
                                UpdateTotalCost();
                                dgvSalesItem.CurrentCell = dgvSalesItem[0, row];
                                dgvPaymentType.Rows.Clear();
                                CalculateChargesAmount();
                                Cell_ReadOnly();
                            }
                        }
                        else
                        {
                            dgvSalesItem.Rows.RemoveAt(row);
                        }

                    }
                    else if (col == 4)
                    {
                        int currentQty = Convert.ToInt32(dgvSalesItem.Rows[row].Cells[4].Value); // updated 3 to 4
                        if (currentQty == 0 || currentQty.ToString() == string.Empty)
                        {
                            //row.Cells[2].Value = "1";
                            MessageBox.Show("Please fill Quantity.");

                            dgvSalesItem.Rows[row].Cells[4].Selected = true;
                            return;
                        }
                    }

                    e.Handled = true;
                }
            }
            catch { }
        }

        private void Save_SaleQty_ToStockTransaction(List<Stock_Transaction> productList)
        {
            int _year, _month;

            _year = System.DateTime.Now.Year;
            _month = System.DateTime.Now.Month;
            Utility.Sale_Run_Process(_year, _month, productList);
        }

        #region paid
        private void btnPaid_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                bool isCredit = false;
                string memberId = string.Empty;
                if (Utility.Customer_Combo_Control(cboCustomer))
                {
                    Cursor.Current = Cursors.Default;
                    return;

                }


                List<TransactionDetail> DetailList = GetTranscationListFromDataGridView();

                if (DetailList.Count() != 0)
                {


                    var FOCList = DetailList.Where(x => x.IsFOC == true).ToList();

                    if (dgvPaymentType.Rows.Count == 1 && Convert.ToString(dgvPaymentType[1, 0].Value).Trim() == "FOC")
                    {
                        if (DetailList.Count == FOCList.Count)
                        {
                            MessageBox.Show("Not allow to save only FOC item. For this operation, please choose FOC payment method.");
                            cboPaymentMethod.Focus();
                            Cursor.Current = Cursors.Default;

                            return;
                        }
                    }


                    List<int> index = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                                       where r.Cells[4].Value == null || r.Cells[4].Value.ToString() == String.Empty || r.Cells[4].Value.ToString() == "0"
                                       select r.Index).ToList();


                    index.RemoveAt(index.Count - 1);

                    if (index.Count > 0)
                    {

                        foreach (var a in index)
                        {
                            try
                            {
                                dgvSalesItem.Rows.RemoveAt(a);
                            }
                            catch
                            {
                                dgvSalesItem.Rows[a].DefaultCellStyle.BackColor = Color.Red; // highlight the rows with qty = null/0/empty 
                            }
                        }
                        Cursor.Current = Cursors.Default;

                        return;
                    }

                    if (cboCustomer.SelectedIndex == 0 || cboCustomer.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please select customer!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cboCustomer.Focus();
                        Cursor.Current = Cursors.Default;

                        return;
                    }
                    else
                    {

                        //   Check_MType();        SD
                        UpdateTotalCost();

                        #region multiPayment
                        Boolean hasError = false;
                        //int _extraDiscount = 0;
                        //Int32.TryParse(txtAdditionalDiscount.Text, out _extraDiscount);
                        int _extraTax = 0;
                        Int32.TryParse(txtExtraTax.Text, out _extraTax);
                        decimal BDDiscount = 0;
                        decimal MCDiscount = 0;
                        if (DiscountType == "BD")
                        {
                            BDDiscount = Convert.ToDecimal(txtMCDiscount.Text);

                        }
                        else
                        {
                            MCDiscount = Convert.ToDecimal(txtMCDiscount.Text);
                        }

                        Currency cu = entity.Currencies.FirstOrDefault(x => x.Id == 1);

                        Transaction insertedTransaction = new Transaction();
                        int paidAmount = 0; bool isFoc = false; int giftcardAmt = 0; int creditAmount = 0;
                        int couponcodeAmt = 0; string tmpCouponCodeNo = "0";
                        foreach (DataGridViewRow row in dgvPaymentType.Rows)
                        {
                            //Updated by YiMon
                            if (!string.IsNullOrEmpty(txtMemberId.Text))
                            {
                                //memberId = txtMemberId.Text;
                                memberId = txtMemberId.Text.Replace("member:", "");
                                IsExportPts = true;                        
                            }
                            //Updated by YiMon
                            if (row.Cells[5].Value == null) // 
                            {
                                paidAmount = 0;
                                isFoc = true;
                            }
                            else
                            {
                                if (row.Cells[2].Value.ToString() == "Gift Card")
                                {
                                    giftcardAmt += Convert.ToInt32(row.Cells[5].Value);
                                }
                                
                                if (row.Cells[1].Value.ToString() == "Coupon Code")
                                {
                                    couponcodeAmt += Convert.ToInt32(row.Cells[5].Value);
                                    tmpCouponCodeNo = row.Cells[0].Value.ToString();
                                }

                                if (row.Cells[2].Value.ToString() == "Credit")
                                {
                                    creditAmount += Convert.ToInt32(row.Cells[5].Value);
                                }
                                paidAmount += Convert.ToInt32(row.Cells[5].Value);

                            }
                        }
                        if (cboPaymentMethod.Text == "FOC")
                        {
                            isFoc = true;
                        }

                        int totalAmount = Convert.ToInt32(lblTotal.Text);
                        if (paidAmount == 0 && isFoc == false)
                        {
                            MessageBox.Show("Please fill up receive amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            hasError = true;
                        }
                        else if (totalAmount > (paidAmount) && isFoc == false)
                        {
                            MessageBox.Show("Receive amount must be greater than total cost!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            hasError = true;
                        }

                        if (giftcardAmt > 0)
                        {
                            if (!verifyDiscount())
                            {
                                Cursor.Current = Cursors.Default;
                                return;
                            }
                        }

                        if (!hasError)
                        {
                            int paymentTypeCountnPayment = (from r in dgvPaymentType.Rows.Cast<DataGridViewRow>()
                                                            where r.Cells[0].Value != null && !string.IsNullOrEmpty(r.Cells[0].Value.ToString()) && r.Cells[0].Value.ToString() != "0"
                                                            select r.Cells[0].Value.ToString()).Distinct().Count();
                            if (paymentTypeCountnPayment > 1)
                            {
                                paymentTypeCountnPayment = Utility.PaymentTypeID.MultiPayment;//Multipayment
                            }
                            else
                            {
                                paymentTypeCountnPayment = int.Parse(dgvPaymentType.Rows[0].Cells[MainPaymentId.Index].Value.ToString());
                            }
                            System.Data.Objects.ObjectResult<String> Id;

                            long totalCost = (long)DetailList.Sum(x => x.TotalAmount) - (long)BDDiscount - (long)MCDiscount;


                            if (!isFoc)
                            {
                                //Sale
                                if (creditAmount == 0)
                                {
                                    //Updated by Lele
                                    //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, paymentTypeCountnPayment, _extraTax + Convert.ToInt32(lblTaxTotal.Text), Convert.ToInt32(lblDiscountTotal.Text), totalCost, paidAmount - (giftcardAmt+couponcodeAmt) , null, Convert.ToInt32(cboCustomer.SelectedValue), MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", chkWholeSale.Checked, giftcardAmt, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, true);
                                    //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, paymentTypeCountnPayment, _extraTax + Convert.ToInt32(lblTaxTotal.Text), Convert.ToInt32(lblDiscountTotal.Text), totalCost, paidAmount - (giftcardAmt + couponcodeAmt), null, Convert.ToInt32(cboCustomer.SelectedValue), MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", chkWholeSale.Checked, giftcardAmt, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, Convert.ToInt32(tmpCouponCodeNo), couponcodeAmt,false, true);
                                    //Updated by YiMon
                                    Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, paymentTypeCountnPayment, _extraTax + Convert.ToInt32(lblTaxTotal.Text), Convert.ToInt32(lblDiscountTotal.Text), totalCost, paidAmount - (giftcardAmt + couponcodeAmt), null, Convert.ToInt32(cboCustomer.SelectedValue), MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", chkWholeSale.Checked, giftcardAmt, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, Convert.ToInt32(tmpCouponCodeNo), couponcodeAmt,false, true, memberId,"");
                                }

                                //Credit
                                else
                                {
                                    //Updated by Lele
                                    //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, paymentTypeCountnPayment, _extraTax + Convert.ToInt32(lblTaxTotal.Text), Convert.ToInt32(lblDiscountTotal.Text), totalCost, paidAmount - (giftcardAmt + creditAmount + couponcodeAmt), null, Convert.ToInt32(cboCustomer.SelectedValue), MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", chkWholeSale.Checked, giftcardAmt, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, true);
                                    //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, paymentTypeCountnPayment, _extraTax + Convert.ToInt32(lblTaxTotal.Text), Convert.ToInt32(lblDiscountTotal.Text), totalCost, paidAmount - (giftcardAmt + creditAmount + couponcodeAmt), null, Convert.ToInt32(cboCustomer.SelectedValue), MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", chkWholeSale.Checked, giftcardAmt, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, Convert.ToInt32(tmpCouponCodeNo), couponcodeAmt,false, true);
                                    //Updated by YiMon
                                    Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, paymentTypeCountnPayment, _extraTax + Convert.ToInt32(lblTaxTotal.Text), Convert.ToInt32(lblDiscountTotal.Text), totalCost, paidAmount - (giftcardAmt + creditAmount + couponcodeAmt), null, Convert.ToInt32(cboCustomer.SelectedValue), MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", chkWholeSale.Checked, giftcardAmt, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, Convert.ToInt32(tmpCouponCodeNo), couponcodeAmt,false, true, memberId,"");
                                }
                            }
                            //FOC
                            else
                            {
                                //Updated by Lele
                                //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Utility.PaymentTypeID.FOC, 0, 0, 0, 0, null, Convert.ToInt32(cboCustomer.SelectedValue), MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", chkWholeSale.Checked, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, true);
                                //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Utility.PaymentTypeID.FOC, 0, 0, 0, 0, null, Convert.ToInt32(cboCustomer.SelectedValue), MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", chkWholeSale.Checked, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, Convert.ToInt32(tmpCouponCodeNo), couponcodeAmt,false, true);
                                //Updated by YiMon
                                Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Utility.PaymentTypeID.FOC, 0, 0, 0, 0, null, Convert.ToInt32(cboCustomer.SelectedValue), MCDiscount, BDDiscount, null, MCDiscountPercent, false, "", chkWholeSale.Checked, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, Convert.ToInt32(tmpCouponCodeNo), couponcodeAmt,false, true, memberId,"");
                            }

                            entity = new POSEntities();
                            string resultId = Id.FirstOrDefault().ToString();
                            insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                            string TId = insertedTransaction.Id;
                            insertedTransaction.IsDeleted = false;
                            insertedTransaction.ReceivedCurrencyId = 1;




                            foreach (TransactionDetail detail in DetailList)
                            {

                                #region reduce qty in stock filling from sap KHS
                                Utility.MinusProductAvailableQtyCosOfSales(entity, (long)detail.ProductId, detail.BatchNo, (int)detail.Qty);

                                #endregion
                                detail.IsDeleted = false;//Update IsDelete (Null to 0)
                                if (detail.ConsignmentPrice == null)
                                {
                                    detail.ConsignmentPrice = 0;
                                }

                                detail.Product = iTempP.Where(p => p.Id == (long)detail.ProductId).FirstOrDefault();

                                Boolean? IsConsignmentPaid = Utility.IsConsignmentPaid(detail.Product);
                                //    var detailID = entity.InsertTransactionDetail(TId, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, detail.ConsignmentPrice, IsConsignmentPaid).SingleOrDefault();

                                var detailID = entity.InsertTransactionDetail(TId, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, detail.ConsignmentPrice, IsConsignmentPaid, detail.IsFOC, Convert.ToInt32(detail.SellingPrice), Convert.ToDouble(detail.IsDeductedBy), detail.BatchNo).SingleOrDefault();

                                if (detail.DiscountRate > 5)
                                {
                                    IsExportPts = false;
                                }

                                var detailforBDdiscount = entity.TransactionDetails.Find(detailID);
                                detailforBDdiscount.BdDiscounted = detail.BdDiscounted;

                                detail.Product.Qty = detail.Product.Qty - detail.Qty;

                                //save in stocktransaction
                                int Qty = 0;
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
                                            Product wpObj = iTempP.Where(p => p.Id == w.ChildProductId).FirstOrDefault();
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

                            if (giftcardAmt != 0)
                            {
                                foreach (DataGridViewRow row in dgvPaymentType.Rows)
                                {
                                    if (row.Cells[2].Value.ToString() == "Gift Card")
                                    {
                                        int customerid = Convert.ToInt32(cboCustomer.SelectedValue);
                                        string cardNumber = row.Cells[3].Value.ToString();
                                        int giftcardid = entity.GiftCards.Where(x => x.CardNumber.Trim() == cardNumber && x.CustomerId == customerid).Select(x => x.Id).FirstOrDefault();

                                        if (giftcardid != 0)
                                        {
                                            GiftCardInTransaction gic = new GiftCardInTransaction();
                                            gic.TransactionId = TId;
                                            gic.GiftCardId = giftcardid;
                                            entity.GiftCardInTransactions.Add(gic);
                                            //Clear giftcard in giftcard list

                                            GiftCard giftcard = entity.GiftCards.Where(x => x.Id == giftcardid).FirstOrDefault();
                                            giftcard.IsUsed = true;
                                        }

                                    }
                                }
                            }


                            List<MultiPayment> multiPaymentList = new List<MultiPayment>();
                            foreach (DataGridViewRow row in dgvPaymentType.Rows)
                            {
                                if (multiPaymentList.Count != 0)
                                {

                                    var data = multiPaymentList.Where(x => x.id == (int)row.Cells[0].Value).FirstOrDefault();
                                    if (data != null)
                                    {
                                        data.amount += Convert.ToInt32(row.Cells[5].Value);
                                    }
                                    else
                                    {
                                        MultiPayment multiPayment = new MultiPayment();

                                        //Added by Lele

                                        if (row.Cells[1].Value.ToString() == "Coupon Code")
                                        {
                                            PaymentMethod tmpPaymentMethod = new APP_Data.PaymentMethod();
                                            tmpPaymentMethod = (from t in entity.PaymentMethods where t.Name == "Coupon Code" select t).FirstOrDefault();
                                            multiPayment.id = tmpPaymentMethod.Id;
                                        }
                                        else
                                        {
                                            multiPayment.id = Convert.ToInt32(row.Cells[0].Value);
                                        }
                                                                                
                                        multiPayment.paymentName = Convert.ToString(row.Cells[2].Value);
                                        multiPayment.amount = Convert.ToInt32(row.Cells[5].Value);
                                        multiPaymentList.Add(multiPayment);
                                    }
                                }
                                else
                                {
                                    MultiPayment multiPayment = new MultiPayment();
                                    
                                    //Added by Lele
                                    if (row.Cells[1].Value.ToString() == "Coupon Code")
                                    {
                                        PaymentMethod tmpPaymentMethod = new APP_Data.PaymentMethod();
                                        tmpPaymentMethod= (from t in entity.PaymentMethods  where t.Name == "Coupon Code" select t).FirstOrDefault();
                                        multiPayment.id = tmpPaymentMethod.Id;
                                    }
                                    else
                                    {
                                        multiPayment.id = Convert.ToInt32(row.Cells[0].Value);
                                    }
                                    //multiPayment.id = Convert.ToInt32(row.Cells[0].Value);
                                    multiPayment.paymentName = Convert.ToString(row.Cells[2].Value);
                                    multiPayment.amount = Convert.ToInt32(row.Cells[5].Value);
                                    multiPaymentList.Add(multiPayment);
                                }
                            }

                            foreach (var item in multiPaymentList)
                            {
                                if (item.paymentName == "Credit")
                                {
                                    isCredit = true;
                                }
                                TransactionPaymentDetail tranPaymentDetail = new TransactionPaymentDetail();
                                tranPaymentDetail.TransactionId = TId;
                                tranPaymentDetail.PaymentMethodId = item.id;
                                tranPaymentDetail.Amount = item.amount;
                                entity.TransactionPaymentDetails.Add(tranPaymentDetail);
                                entity.SaveChanges();
                            }



                            ExchangeRateForTransaction ex = new ExchangeRateForTransaction();
                            ex.TransactionId = TId;
                            ex.CurrencyId = cu.Id;
                            ex.ExchangeRate = Convert.ToInt32(cu.LatestExchangeRate);
                            entity.ExchangeRateForTransactions.Add(ex);
                            entity.SaveChanges();

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
                            bool FirstTime = false;
                            int TotalAmt = Convert.ToInt32(lblTotal.Text);
                            int memberValidityYear = Convert.ToInt32(SettingController.DefaultMemberValidityYear.ToString());
                            NewCustomer _newCustomer = new NewCustomer();
                            List<MemberCardRule> memberCardRuleList = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true orderby int.Parse(m.RangeFrom) select m).ToList();
                            int memberTypeID = 0;
                            string memName = null;
                            int total = 0;
                            int giftCardTotalAmount = 0;
                            int couponCodeTotalAmount = 0;
                            int totalRAmount = 0;
                            string minimumAmountofThisMemberType = (from mCardRule in entity.MemberCardRules.AsEnumerable() where mCardRule.IsActive == true orderby int.Parse(mCardRule.RangeFrom) select mCardRule.RangeFrom).FirstOrDefault();

                            string name = entity.Customers.Where(x => x.Id == _customerId).Select(x => x.Name).FirstOrDefault();
                            if (name != "Default")
                            {
                                //Update Member Type
                                //ELC_CustomerPointSystem.Get_ExpiredMemberList_And_Update_ExpiredMember(CustomerId);

                                APP_Data.Customer customerObj = entity.Customers.Where(x => x.Id == _customerId).FirstOrDefault();
                                int mTypeID = (from memberCardRule in entity.MemberCardRules.AsEnumerable() where memberCardRule.IsActive == true orderby int.Parse(memberCardRule.RangeFrom) descending select memberCardRule.MemberTypeId).FirstOrDefault();
                                if (customerObj.PromoteDate != null)
                                {
                                    MemberOldLevel = customerObj.MemberTypeID==null?0: (int)customerObj.MemberTypeID;
                                    // isexpired? == no
                                    if (DateTime.Today.Date <= customerObj.ExpireDate)
                                    { // ok
                                        var LastOneYears = DateTime.Now.AddYears(-1);
                                        if (customerObj.MemberTypeID != mTypeID) //if selected customer is not the greatest
                                        {
                                            List<Transaction> transactionList = (from t in entity.Transactions
                                                                                 join tus in entity.Customers
                                                                                 on t.CustomerId equals tus.Id
                                                                                 where (t.CustomerId == _customerId) && (tus.Id == _customerId) && (t.DateTime >= LastOneYears)
                                                                                 && (t.DateTime <= DateTime.Now) && (t.IsDeleted == false) && (t.IsComplete == true)
                                                                                 && (t.PaymentTypeId == 1 || t.PaymentTypeId == 2 || t.PaymentTypeId == 3 || t.PaymentTypeId == 5 || t.PaymentTypeId == null)
                                                                                 && (t.Type != "Prepaid") && (t.Type != "Settlement") && (t.Type != "Refund") && (t.Type != "CreditRefund")
                                                                                 select t).ToList();

                                            List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                                   join transactions in entity.Transactions
                                                                                                   on td.TransactionId equals transactions.ParentId
                                                                                                   join tus in entity.Customers
                                                                                                   on transactions.CustomerId equals tus.Id
                                                                                                   where (transactions.CustomerId == _customerId) && (tus.Id == _customerId) && (transactions.DateTime >= tus.StartDate) && (transactions.DateTime <= tus.ExpireDate)
                                                                                                   && (transactions.IsDeleted == false) && (transactions.IsComplete == true)
                                                                                                   && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                                                   && (transactions.Type != "Refund") && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
                                                                                                   select td).ToList();
                                            foreach (Transaction t in transactionList)
                                            {
                                                total += Convert.ToInt32(t.TotalAmount);
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

                                            //updated by Lele
                                            //totalAmount = totalAmount - giftCardTotalAmount;
                                            totalAmount = totalAmount - (giftCardTotalAmount + couponCodeTotalAmount );

                                            foreach (TransactionDetail td in RefundTransactionDetailList)
                                            {
                                                totalRAmount += Convert.ToInt32(td.TotalAmount);
                                            }
                                            total = total - totalRAmount;


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
                                                    _newCustomer.CustomerId = _customerId;
                                                    _newCustomer.MemerTypeName = memName;

                                                    _newCustomer.getTotalAmount = Convert.ToInt32(lblTotal.Text);
                                                    _newCustomer.Type = 'S';
                                                    _newCustomer.isEdit = true;
                                                    _newCustomer.TransactionId = resultId;
                                                    _newCustomer.Text = "Update Customer";
                                                    // FirstTime = true;
                                                    _newCustomer.IsClosed = FirstTime;
                                                    //_newCustomer.ShowDialog(); //POS will not control customer info, no need to add/edit
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
                                                                                 join tus in entity.Customers
                                                                                 on transactions.CustomerId equals tus.Id
                                                                                 where (transactions.CustomerId == _customerId) && (tus.Id == _customerId) && (transactions.DateTime >= _fromDate)
                                                                                 && (transactions.DateTime <= _expireDate) && (transactions.IsDeleted == false) && (transactions.IsComplete == true)
                                                                                 && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                                 && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "Refund") && (transactions.Type != "CreditRefund")
                                                                                 select transactions).ToList();


                                            List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                                   join transactions in entity.Transactions
                                                                                                   on td.TransactionId equals transactions.ParentId
                                                                                                   join tus in entity.Customers
                                                                                                   on transactions.CustomerId equals tus.Id
                                                                                                   where (transactions.CustomerId == _customerId) && (tus.Id == _customerId) && (transactions.IsDeleted == false) && (transactions.IsComplete == true)
                                                                                                   && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                                                   && (transactions.Type != "Refund") && (transactions.DateTime >= tus.StartDate) && (transactions.DateTime <= tus.ExpireDate)
                                                                                                   && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
                                                                                                   select td).ToList();

                                            //totalAmount = Convert.ToInt32(TotalAmt);

                                            foreach (Transaction t in transactionList)
                                            {
                                                total += Convert.ToInt32(t.TotalAmount);
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

                                            //updated by Lele
                                            // total = total - giftCardTotalAmount;
                                            total = total - (giftCardTotalAmount + couponCodeTotalAmount);

                                            foreach (TransactionDetail td in RefundTransactionDetailList)
                                            {
                                                totalRAmount += Convert.ToInt32(td.TotalAmount);
                                            }
                                            total = total - totalRAmount;

                                            _currentAmount = total - Convert.ToInt32(TotalAmt);
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
                                                    _newCustomer.CustomerId = _customerId;
                                                    _newCustomer.MemerTypeName = memName;
                                                    _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
                                                    _newCustomer.Type = 'S';
                                                    _newCustomer.isEdit = true;
                                                    _newCustomer.isExpired = true;
                                                    _newCustomer.TransactionId = resultId;
                                                    _newCustomer.IsClosed = FirstTime;
                                                    //_newCustomer.ShowDialog(); //POS will not control customer info, no need to add/edit
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
                                                    _newCustomer.CustomerId = _customerId;
                                                    _newCustomer.MemerTypeName = memName;
                                                    _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
                                                    _newCustomer.Type = 'S';
                                                    _newCustomer.isEdit = true;
                                                    _newCustomer.isReNew = true;
                                                    _newCustomer.TransactionId = resultId;
                                                    _newCustomer.IsClosed = FirstTime;
                                                    //_newCustomer.ShowDialog(); //POS will not control customer info, no need to add/edit
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
                                                _newCustomer.CustomerId = _customerId;
                                                _newCustomer.MemerTypeName = memName;
                                                _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
                                                _newCustomer.Type = 'S';
                                                _newCustomer.isEdit = true;
                                                _newCustomer.isReNew = true;
                                                _newCustomer.TransactionId = resultId;

                                                _newCustomer.IsClosed = FirstTime;
                                                //_newCustomer.ShowDialog(); //POS will not control customer info, no need to add/edit
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

                                            _newCustomer.CustomerId = _customerId;
                                            _newCustomer.MemerTypeName = memName;
                                            _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
                                            _newCustomer.Type = 'S';
                                            _newCustomer.TransactionId = resultId;
                                            _newCustomer.isEdit = true;
                                            _newCustomer.IsClosed = FirstTime;
                                            //_newCustomer.ShowDialog(); //POS will not control customer info, no need to add/edit
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
                                        _newCustomer.CustomerId = _customerId;
                                        _newCustomer.MemerTypeName = memName;
                                        _newCustomer.getTotalAmount = Convert.ToInt32(TotalAmt);
                                        _newCustomer.Type = 'S';
                                        _newCustomer.TransactionId = resultId;
                                        _newCustomer.IsClosed = FirstTime;
                                        //_newCustomer.ShowDialog(); //POS will not control customer info, no need to add/edit
                                        FirstTime = ELC_CustomerPointSystem.IsFirstTimeOrNot(_newCustomer.IsClosed);

                                    }

                                }
                            }



                            #endregion

                            #region Update IsCalculatedPoint in Transaction And PointHistoryId in Transaction Detail and insert VIP Customer

                            if (!isCredit)
                            {
                                VipCustomer vipCustomer = new VipCustomer();
                                DateTime lastTwoYears = DateTime.Now.AddYears(-2);
                                int customerId1 = (int)cboCustomer.SelectedValue;
                                //var _customerId1 = entity1.Transactions.Where(x => x.Id == resultId).Select(x => x.CustomerId).FirstOrDefault();
                                DateTime transactionDate = entity1.Transactions.Where(x => x.Id == resultId).Select(x => x.DateTime.Value).FirstOrDefault();
                                var _custList = entity1.Customers.Where(x => x.Id == customerId1).FirstOrDefault();
                                if (_custList.MemberTypeID != null)
                                {
                                    int memberTypeID2 = (int)_custList.MemberTypeID;
                                    var _isCalucatePoint = entity.MemberCardRules.Where(x => x.IsActive == true && x.MemberTypeId == memberTypeID2).Select(x => x.IsCalculatePoints).FirstOrDefault();
                                    bool isCalucatePoint2 = _isCalucatePoint == null ? false : (bool)_isCalucatePoint;
                                    if (FirstTime == true && _custList.PromoteDate!=null && MemberOldLevel > 0 && isCalucatePoint2 == false)
                                    {
                                        FirstTime = false; 
                                        isCalucatePoint2 = true;
                                    }
                                    else
                                    {
                                        MemberOldLevel = memberTypeID2;
                                    }
                                    ELC_CustomerPointSystem.Update_ForPoint_InTransaction(FirstTime, MemberOldLevel, resultId, 1, _custList.Id, isCalucatePoint2);
                                    var vipTransactionlist = entity.Transactions.Where(x => x.IsDeleted == false && x.IsActive == true
                                                             && x.CustomerId == customerId1 && (x.DateTime >= lastTwoYears && x.DateTime <= DateTime.Now)).ToList();
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
                            // CheckExportStatus();

                            #region [ Print ]
                            if (true)
                            {

                                dsReportTemp dsReport = new dsReportTemp();
                                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                                dsReportTemp.MultiPaymentDataTable multiReport = (dsReportTemp.MultiPaymentDataTable)dsReport.Tables["MultiPayment"];
                                int _tAmt = 0;
                                PList.Clear();
                                Isduplicate = false;
                                PrintDetailList(DetailList);
                                if (Isduplicate)
                                {
                                    DetailList = PList;
                                }

                                foreach (TransactionDetail transaction in DetailList)
                                {
                                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                                    newRow.ItemId = transaction.Product.ProductCode;
                                    newRow.Name = transaction.Product.Name;
                                    newRow.Qty = transaction.Qty.ToString();
                                    newRow.DiscountPercent = transaction.DiscountRate.ToString();
                                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty;

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

                                foreach (var item in multiPaymentList)
                                {
                                    dsReportTemp.MultiPaymentRow newRow = multiReport.NewMultiPaymentRow();
                                    newRow.PaymentName = item.paymentName;
                                    newRow.Amount = Convert.ToString(item.amount);
                                    multiReport.AddMultiPaymentRow(newRow);
                                }
                                if (multiReport.Count < 1)
                                {
                                    try
                                    {
                                        dsReportTemp.MultiPaymentRow newRow = multiReport.NewMultiPaymentRow();
                                        newRow.PaymentName = dgvPaymentType.Rows[0].Cells[1].Value.ToString();
                                        newRow.Amount = Convert.ToString(dgvPaymentType.Rows[0].Cells[3].Value.ToString());
                                        multiReport.AddMultiPaymentRow(newRow);
                                    }
                                    catch { }
                                }
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

                                APP_Data.Counter c = entity.Counters.Where(x => x.Id == MemberShip.CounterId).FirstOrDefault();

                                ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                                rv.LocalReport.SetParameters(CounterName);

                                ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
                                rv.LocalReport.SetParameters(PrintDateTime);

                                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                                rv.LocalReport.SetParameters(CasherName);

                                Int64 totalAmountRep = insertedTransaction.TotalAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TotalAmount);
                                ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                                rv.LocalReport.SetParameters(TotalAmount);

                                Int64 taxAmountRep = insertedTransaction.TaxAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TaxAmount);
                                ReportParameter TaxAmount = new ReportParameter("TaxAmount", taxAmountRep.ToString());
                                rv.LocalReport.SetParameters(TaxAmount);

                                string CurrencySymbol = "Ks";

                                ReportParameter CurrencyCode = new ReportParameter("CurrencyCode", CurrencySymbol);
                                rv.LocalReport.SetParameters(CurrencyCode);

                                Int64 disAmountRep = insertedTransaction.DiscountAmount == null ? 0 : Convert.ToInt64(insertedTransaction.DiscountAmount);

                                if (disAmountRep == 0)
                                {
                                    ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", disAmountRep.ToString());
                                    rv.LocalReport.SetParameters(DiscountAmount);
                                }
                                else
                                {
                                    ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", "-" + disAmountRep.ToString());
                                    rv.LocalReport.SetParameters(DiscountAmount);
                                }
                                //updated by Lele
                                //ReportParameter PaidAmount = new ReportParameter("PaidAmount", (paidAmount - giftcardAmt).ToString());
                                ReportParameter PaidAmount = new ReportParameter("PaidAmount", (paidAmount - (giftcardAmt+couponcodeAmt)).ToString());
                                rv.LocalReport.SetParameters(PaidAmount);

                                ReportParameter Change = new ReportParameter("Change", lblCharges.Text.ToString());
                                rv.LocalReport.SetParameters(Change);

                                ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", ELC_CustomerPointSystem.Point_Calculation(cus.Id).ToString());
                                rv.LocalReport.SetParameters(AvailablePoint);


                                if (Utility.GetDefaultPrinter() == "A4 Printer")
                                {
                                    ReportParameter CusAddress = new ReportParameter("CusAddress", cus.Address);
                                    rv.LocalReport.SetParameters(CusAddress);
                                }

                                //Clear();

                                //Utility.Get_Print(rv); // print after exported to Piti
                                #endregion

                                POS.APP_Data.Transaction memberTransId = entity.Transactions.Where(t => t.Id == resultId).FirstOrDefault();

                                if (memberTransId.PitiMemberId == "" || memberTransId.PitiMemberId == null)
                                {
                                    DialogResult result = MessageBox.Show("Payment Completed", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else if (!IsExportPts)
                                {
                                    DialogResult result = MessageBox.Show("Payment Completed and this customer will not get points", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    DialogResult result = MessageBox.Show("Payment Completed and wait to export transaction", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (result.Equals(DialogResult.OK))
                                    {
                                        Sales sForm = (Sales)this.ActiveMdiChild;
                                        MemberTransExport exportMemberTransForm = new MemberTransExport(resultId);
                                        exportMemberTransForm.Text = "Export Transactions";
                                        exportMemberTransForm.ShowDialog();
                                        
                                        string memberDisplayName = "";
                                        memberDisplayName = API_POST_MemberTrans.ResponseMemberDisplayName;

                                        //if (memberDisplayName!="" && API_POST_MemberTrans.postMemberTransSuccess==true)
                                        if (memberDisplayName != "")
                                        {
                                            ReportParameter pitiCustomerName = new ReportParameter("CustomerName", memberDisplayName);
                                            rv.LocalReport.SetParameters(pitiCustomerName);
                                        }                                        
                                    }
                                }

                                Clear();
                                Utility.Get_Print(rv);
                            }
                        }

                        #endregion




                        //Cash
                        #region Cash
                        if (false) //Convert.ToInt32(cboPaymentMethod.SelectedValue) == 1
                        {
                            PaidByCash2 form = new PaidByCash2();
                            form.DetailList = DetailList;
                            //int extraDiscount = 0;
                            //Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                            int tax = 0;
                            Int32.TryParse(txtExtraTax.Text, out tax);
                            form.IsPrint = chkPrintSlip.Checked;
                            form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                            form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                            form.isDraft = isDraft;
                            form.DraftId = DraftId;
                            form.ExtraTax = tax;
                            //form.ExtraDiscount = extraDiscount;
                            form.isDebt = false;
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue);
                            Get_MemberTypeID();
                            form.MemberTypeId = MemberTypeID;
                            if (Convert.ToDecimal(txtMCDiscount.Text) == 0)
                            {
                                MCDiscountPercent = 0;
                            }

                            form.MCDiscountPercent = MCDiscountPercent;
                            form.IsWholeSale = chkWholeSale.Checked;
                            if (DiscountType == "BD")
                            {
                                form.BDDiscount = Convert.ToDecimal(txtMCDiscount.Text);

                            }
                            else
                            {
                                form.MCDiscount = Convert.ToDecimal(txtMCDiscount.Text);
                            }
                            //if (cboCustomer.SelectedIndex != 0)
                            //    form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                            form.ShowDialog();
                        }
                        #endregion

                        //Credit
                        #region Credit
                        else if (false) //Convert.ToInt32(cboPaymentMethod.SelectedValue) == 2
                        {
                            PaidByCreditWithPrePaidDebt form = new PaidByCreditWithPrePaidDebt();
                            form.DetailList = DetailList;

                            //int extraDiscount = 0;
                            //Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                            int tax = 0;
                            Int32.TryParse(txtExtraTax.Text, out tax);
                            form.IsPrint = chkPrintSlip.Checked;
                            form.isDraft = isDraft;
                            form.DraftId = DraftId;
                            form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                            form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                            form.ExtraTax = tax;
                            //form.ExtraDiscount = extraDiscount;
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue);
                            Get_MemberTypeID();
                            form.MemberTypeId = MemberTypeID;
                            if (Convert.ToDecimal(txtMCDiscount.Text) == 0)
                            {
                                MCDiscountPercent = 0;
                            }

                            form.MCDiscountPercent = MCDiscountPercent;
                            form.IsWholeSale = chkWholeSale.Checked;
                            if (DiscountType == "BD")
                            {
                                form.BDDiscount = Convert.ToDecimal(txtMCDiscount.Text);
                            }
                            else
                            {
                                form.MCDiscount = Convert.ToDecimal(txtMCDiscount.Text);
                            }
                            //if (cboCustomer.SelectedIndex != 0)
                            //    form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                            form.ShowDialog();
                        }
                        #endregion

                        //GiftCard
                        #region GiftCard
                        else if (false) //Convert.ToInt32(cboPaymentMethod.SelectedValue) == 3
                        {
                            PaidByGiftCard form = new PaidByGiftCard();
                            form.DetailList = DetailList;
                            //int extraDiscount = 0;
                            //Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                            int tax = 0;
                            Int32.TryParse(txtExtraTax.Text, out tax);
                            form.IsPrint = chkPrintSlip.Checked;
                            form.isDraft = isDraft;
                            form.DraftId = DraftId;
                            form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                            form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                            form.ExtraTax = tax;
                            //form.ExtraDiscount = extraDiscount;
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue);
                            Get_MemberTypeID();
                            form.MemberTypeId = MemberTypeID;
                            if (Convert.ToDecimal(txtMCDiscount.Text) == 0)
                            {
                                MCDiscountPercent = 0;
                            }

                            form.MCDiscountPercent = MCDiscountPercent;
                            form.IsWholeSale = chkWholeSale.Checked;
                            if (DiscountType == "BD")
                            {
                                form.BDDiscount = Convert.ToDecimal(txtMCDiscount.Text);
                            }
                            else
                            {
                                form.MCDiscount = Convert.ToDecimal(txtMCDiscount.Text);
                            }
                            ////if (cboCustomer.SelectedIndex != 0)
                            ////    form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                            form.ShowDialog();
                        }
                        #endregion
                       
                        //MPU
                        #region MPU
                        else if (false)// Convert.ToInt32(cboPaymentMethod.SelectedValue) == 5
                        {
                            PaidByMPU form = new PaidByMPU();
                            form.DetailList = DetailList;
                            //int extraDiscount = 0;
                            //Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                            int tax = 0;
                            Int32.TryParse(txtExtraTax.Text, out tax);
                            form.IsPrint = chkPrintSlip.Checked;
                            form.isDraft = isDraft;
                            form.DraftId = DraftId;
                            form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                            form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                            form.ExtraTax = tax;
                            //form.ExtraDiscount = extraDiscount;
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue);
                            Get_MemberTypeID();
                            form.MemberTypeId = MemberTypeID;
                            if (Convert.ToDecimal(txtMCDiscount.Text) == 0)
                            {
                                MCDiscountPercent = 0;
                            }

                            form.MCDiscountPercent = MCDiscountPercent;
                            form.IsWholeSale = chkWholeSale.Checked;
                            if (DiscountType == "BD")
                            {
                                form.BDDiscount = Convert.ToDecimal(txtMCDiscount.Text);
                            }
                            else
                            {
                                form.MCDiscount = Convert.ToDecimal(txtMCDiscount.Text);
                            }


                            ////if (cboCustomer.SelectedIndex != 0)
                            ////    form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());

                            form.ShowDialog();
                        }
                        #endregion

                        // FOC
                        #region FOC
                        else if (false) //Convert.ToInt32(cboPaymentMethod.SelectedValue) == 4
                        {
                            PaidByFOC form = new PaidByFOC();
                            form.DetailList = DetailList;
                            form.Type = 4;
                            //int extraDiscount = 0;
                            //Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                            int tax = 0;
                            Int32.TryParse(txtExtraTax.Text, out tax);
                            form.IsPrint = chkPrintSlip.Checked;
                            form.isDraft = isDraft;
                            form.DraftId = DraftId;
                            form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                            form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                            form.ExtraTax = tax;
                            //form.ExtraDiscount = extraDiscount;
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue);
                            Get_MemberTypeID();
                            form.MemberTypeId = MemberTypeID;
                            if (Convert.ToDecimal(txtMCDiscount.Text) == 0)
                            {
                                MCDiscountPercent = 0;
                            }

                            form.MCDiscountPercent = MCDiscountPercent;
                            form.BDDiscount = 0;
                            form.MCDiscount = 0;
                            form.IsWholeSale = chkWholeSale.Checked;

                            ////if (cboCustomer.SelectedIndex != 0)
                            ////    form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                            form.ShowDialog();
                        }
                        #endregion

                        else if (false) //Convert.ToInt32(cboPaymentMethod.SelectedValue) == 6
                        {
                            PaidByFOC form = new PaidByFOC();
                            form.DetailList = DetailList;
                            form.Type = 6;
                            //int extraDiscount = 0;
                            //Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                            int tax = 0;
                            Int32.TryParse(txtExtraTax.Text, out tax);
                            form.IsPrint = chkPrintSlip.Checked;
                            form.isDraft = isDraft;
                            form.DraftId = DraftId;
                            form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                            form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                            form.ExtraTax = tax;
                            //form.ExtraDiscount = extraDiscount;

                            form.BDDiscount = 0;
                            form.MCDiscount = 0;
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue);
                            Get_MemberTypeID();
                            form.MemberTypeId = MemberTypeID;
                            if (Convert.ToDecimal(txtMCDiscount.Text) == 0)
                            {
                                MCDiscountPercent = 0;
                            }

                            form.MCDiscountPercent = MCDiscountPercent;
                            form.IsWholeSale = chkWholeSale.Checked;
                            form.ShowDialog();
                        }
                        ////New = "";
                        ////if (Save == true)
                        ////{
                        ////    DiscountType = "";
                        ////    //ItemDiscountZeroAmt = 0;
                        ////    MemberTypeID = 0;
                        ////    Save = false;
                        ////}

                    }
                    txtMemberId.Visible = true;
                    lblMemberId.Visible = false;
                    txtMemberId.Text = ""; 
                }
                else
                {
                    MessageBox.Show("You haven't select any item to paid");
                }
            }
            catch (Exception ex)
            {
                Utility.ShowErrMessage("Unable to pay!", "An error occure while paid, Please contact administrator for assist!" + Environment.NewLine + "Error Message :" + Environment.NewLine + ex.ToString());
            }
        }
        #endregion paid
        private void Get_MemberTypeID()
        {
            if (MemberTypeID == 0 || MemberTypeID == null)
            {
                MemberTypeID = 0;
            }
            else
            {
                if (DiscountType == "BD")
                {
                    MCDiscountPercent = (from m in entity.MemberCardRules where m.MemberTypeId == MemberTypeID select m.BDDiscount).FirstOrDefault();
                }
                else if (DiscountType == "MC")
                {
                    MCDiscountPercent = (from m in entity.MemberCardRules where m.MemberTypeId == MemberTypeID select m.MCDiscount).FirstOrDefault();
                }

            }
        }

        private void PrintDetailList(List<TransactionDetail> Dlist)
        {
            HashSet<TransactionDetail> hset = new HashSet<TransactionDetail>();
            // IEnumerable<TransactionDetail> SameCodeProducts = Dlist.Where(x=> !hset.Add(x));           
            IEnumerable<Nullable<long>> SameCodeProducts = Dlist.GroupBy(x => x.ProductId).Where(g => g.Count() > 1).Select(x => x.Key);
            if (SameCodeProducts.Count() > 0)
            {

                Isduplicate = true;

                PList = Dlist.GroupBy(x => new { x.Product, x.DiscountRate, x.UnitPrice, x.IsFOC })
                    .Select(y => new TransactionDetail()
                    {
                        Product = y.Key.Product,
                        Qty = (int)y.Sum(z => z.Qty),
                        DiscountRate = (decimal)y.Key.DiscountRate,
                        UnitPrice = (long)y.Key.UnitPrice,
                        TotalAmount = (long)y.Sum(z => z.TotalAmount),
                        IsFOC = (bool)y.Key.IsFOC,
                    }).ToList();
            }

        }
        private void Check_MType()
        {
            int[] FOCList = { 4, 6 }; //???
            if (!FOCList.Contains(Convert.ToInt32(cboPaymentMethod.SelectedValue)))
            {
                Fill_Cus();
            }
            else
            {
                txtMCDiscount.Text = "0";
            }
        }

        private void btnLoadDraft_Click(object sender, EventArgs e)
        {
            if (Utility.Customer_Combo_Control(cboCustomer))
            {
                return;
            }
            DialogResult result = MessageBox.Show("This action will erase current sale data. Would you like to continue?", "Load", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {
                DraftList form = new DraftList();
                form.Show();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //will only work if the grid have data row
            //datagrid count header as a row, so we have to check there is more than one row
            if (Utility.Customer_Combo_Control(cboCustomer))
            {
                return;
            }
            if (dgvSalesItem.Rows.Count > 1)
            {
                List<TransactionDetail> DetailList = GetTranscationListFromDataGridView();

                //int extraDiscount = 0;
                //Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);

                int tax = 0;
                Int32.TryParse(txtExtraTax.Text, out tax);
                int cusId = Convert.ToInt32(cboCustomer.SelectedValue);


                IsWholeSale = Convert.ToBoolean(chkWholeSale.Checked);
                System.Data.Objects.ObjectResult<String> Id;
                if (cusId > 0)
                {
                    //  Id = entity.InsertDraft(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 1, tax, extraDiscount, DetailList.Sum(x => x.TotalAmount) + tax - extraDiscount, null, null, cusId);
                    Id = entity.InsertDraft(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Convert.ToInt32(cboPaymentMethod.SelectedValue), tax, 0, DetailList.Sum(x => x.TotalAmount) + tax, null, null, cusId, IsWholeSale);
                }
                else
                {
                    //Id = entity.InsertDraft(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 1, tax, extraDiscount, DetailList.Sum(x => x.TotalAmount) + tax - extraDiscount, null, null, null);
                    Id = entity.InsertDraft(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, Convert.ToInt32(cboPaymentMethod.SelectedValue), tax, 0, DetailList.Sum(x => x.TotalAmount) + tax, null, null, null, IsWholeSale);
                }
                entity = new POSEntities();
                string resultId = Id.FirstOrDefault().ToString();
                Transaction insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                insertedTransaction.IsDeleted = false;


                if (insertedTransaction != null)
                {
                    foreach (TransactionDetail detail in DetailList)
                    {
                        detail.IsDeleted = false;
                        insertedTransaction.TransactionDetails.Add(detail);

                        #region reduce qty in stock filling from sap KHS

                        Utility.MinusProductAvailableQtyCosOfSales(entity, (long)detail.ProductId, detail.BatchNo, (int)detail.Qty);

                        #endregion
                    }
                }


                entity.SaveChanges();
                Clear();
            }
        }
        private void Sales_Activated(object sender, EventArgs e)
        {
            //DailyRecord latestRecord = (from rec in entity.DailyRecords where rec.CounterId == MemberShip.CounterId && rec.IsActive == true select rec).FirstOrDefault();
            //if (latestRecord == null)
            //{
            //    StartDay form = new StartDay();
            //    form.Show();
            //}
        }
        public class AvailableProductQtyWithBatch
        {

            public long ProductID { get; set; }
            public string BatchNo { get; set; }
            public DateTime? ExpireDate { get; set; }

            public int OrgQty { get; set; }
            public int AvailableQty { get; set; }

            public int InUseQty { get; set; }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            AvoidAction();
            string productName = cboProductName.Text.Trim();
            List<Product> pList = iTempP.Where(x => x.Name.Trim().Contains(productName) && x.IsDeleted == false).ToList();


            if (pList.Count > 0)
            {
                dgvSearchProductList.DataSource = pList;
                dgvSearchProductList.Focus();
            }
            else
            {
                MessageBox.Show("Item not found!", "Cannot find");
                dgvSearchProductList.DataSource = "";
                ListenAction();
                return;
            }
            this.AcceptButton = null;
            ListenAction();
        }

        private void dgvSearchProductList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                AvoidAction();
                // if (e.RowIndex >= 0)
                if (e.RowIndex > -1 && dgvSearchProductList.Rows.Count > 0)
                {
                    deleteAction = false;
                    long currentProductId = Convert.ToInt64(dgvSearchProductList.Rows[e.RowIndex].Cells[0].Value);
                    if (e.ColumnIndex == 1)
                    {
                        Product pro = iTempP.Where(p => p.Id == currentProductId).FirstOrDefault<Product>();
                        if (pro != null)
                        {
                            List<AvailableProductQtyWithBatch> tempProductControl = availablePList == null ? null : availablePList.Where(Product => Product.ProductID == currentProductId).ToList();
                            if (tempProductControl == null || tempProductControl.Count == 0)
                            {
                                AddNew4AvailableProductQtyWithBatch(currentProductId, string.Empty);

                            }
                            tempProductControl = availablePList.Where(Product => Product.ProductID == currentProductId && Product.AvailableQty > 0).OrderBy(p => p.ExpireDate).ToList();
                            if (tempProductControl != null && tempProductControl.Count > 0)
                            {
                                decimal DisRate = pro.DiscountRate > 0 ? pro.DiscountRate : IsBirthday == true && pro.DiscountRate == 0 ? SettingController.birthday_discount : pro.DiscountRate;
                                //pro.DiscountRate = pro.DiscountRate > 0 ? pro.DiscountRate : IsBirthday == true && pro.DiscountRate == 0 ? SettingController.birthday_discount : pro.DiscountRate;

                                UpdateDataGridViewProcess(pro, tempProductControl, dgvSalesItem.Rows.Count - 1, false, false, 1, DisRate, tempProductControl[0].BatchNo);

                            }
                            else
                            {
                                MessageBox.Show("Product Out of Stock..!");
                            }
                        }
                        else
                        {

                            MessageBox.Show("Item not found!", "Cannot find");
                        }

                        UpdateTotalCost();

                    }
                }
                dgvSalesItem.CommitEdit(new DataGridViewDataErrorContexts());

                ListenAction();
            }
            catch
            { ListenAction(); }

        }
        public enum sCol
        {
            colBarCode,
            colProductCode,
            colProductName,
            colProductBatchNo,
            colQty,
            colUnitPrice,
            colDiscountRate,
            colTax,
            colCost,
            colFOC,
            colDelete,
            colId,
            colConsignmentPrice
        }
        private static string coldelete = "Delete";


        private void dgvSearchProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.Enter && dgvSearchProductList.CurrentCell != null) || (e.KeyData == Keys.Space && dgvSearchProductList.CurrentCell != null))
            {
                int Row = dgvSearchProductList.CurrentCell.RowIndex;
                int Column = dgvSearchProductList.CurrentCell.ColumnIndex;
                int currentProductId = Convert.ToInt32(dgvSearchProductList.Rows[Row].Cells[0].Value);
                int count = dgvSalesItem.Rows.Count;
                if (Column == (int)sCol.colProductCode)
                {

                    Product pro = iTempP.Where(p => p.Id == currentProductId).FirstOrDefault<Product>();
                    if (pro != null)
                    {
                        List<AvailableProductQtyWithBatch> tempProductControl = availablePList == null ? null : availablePList.Where(Product => Product.ProductID == currentProductId).ToList();
                        if (tempProductControl == null || tempProductControl.Count == 0)
                        {
                            AddNew4AvailableProductQtyWithBatch(currentProductId, string.Empty);
                        }
                        tempProductControl = availablePList.Where(Product => Product.ProductID == currentProductId && Product.AvailableQty > 0).OrderBy(p => p.ExpireDate).ToList();
                        if (tempProductControl != null && tempProductControl.Count > 0)
                        {
                            decimal DisRate = pro.DiscountRate > 0 ? pro.DiscountRate : IsBirthday == true && pro.DiscountRate == 0 ? SettingController.birthday_discount : pro.DiscountRate;
                            //pro.DiscountRate = pro.DiscountRate > 0 ? pro.DiscountRate : IsBirthday == true && pro.DiscountRate == 0 ? SettingController.birthday_discount : pro.DiscountRate;

                            UpdateDataGridViewProcess(pro, tempProductControl, dgvSalesItem.Rows.Count - 1, false, false, 1, DisRate, tempProductControl[0].BatchNo);

                        }
                        else
                        {
                            MessageBox.Show("Product Out of Stock..!");
                        }

                    }
                    else
                    {

                        MessageBox.Show("Item not found!", "Cannot find");
                    }

                    UpdateTotalCost();
                }
            }
        }

        private void cboProductName_KeyDown(object sender, KeyEventArgs e)
        {
            this.AcceptButton = btnSearch;
        }

        private void txtAdditionalDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtExtraTax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Function

        private List<TransactionDetail> GetTranscationListFromDataGridView() // TTN updating 
        {
            List<TransactionDetail> DetailList = null;
            DetailList = new List<TransactionDetail>();
            PointDeductionPercentage_History pdp = entity.PointDeductionPercentage_History.Where(p => p.Active == true).FirstOrDefault();
            foreach (DataGridViewRow row in dgvSalesItem.Rows)
            {
                if (!row.IsNewRow && row.Cells[(int)sCol.colId].Value != null && row.Cells[(int)sCol.colBarCode].Value != null && row.Cells[(int)sCol.colProductCode].Value != null && row.Cells[(int)sCol.colProductName].Value != null)
                {
                    TransactionDetail transDetail = new TransactionDetail();
                    bool IsFOC = false;
                    if (row.Cells[9].Value.ToString() == "FOC")
                    {
                        IsFOC = true;
                    }
                    int qty = 0, productId = 0;
                    //  bool alreadyinclude = false;
                    decimal discountRate = 0;
                    Int32.TryParse(row.Cells[(int)sCol.colId].Value.ToString(), out productId);
                    Int32.TryParse(row.Cells[(int)sCol.colQty].Value.ToString(), out qty);
                    Decimal.TryParse(row.Cells[(int)sCol.colDiscountRate].Value.ToString(), out discountRate);
                    ////Check if the product is already include in above row
                    //foreach (TransactionDetail td in DetailList)
                    //{
                    //    if (td.ProductId == productId && td.DiscountRate == discountRate)
                    //    {
                    //        Product tempProd = (from p in entity.Products where p.Id == productId select p).FirstOrDefault<Product>();
                    //        td.Qty = td.Qty + qty;
                    //        td.TotalAmount = Convert.ToInt64(getActualCost(tempProd, discountRate, IsFOC)) * td.Qty;
                    //        alreadyinclude = true;
                    //    }
                    //}

                    //if (!alreadyinclude)
                    //{
                    //Check productId is valid or not.
                    Product pro = iTempP.Where(p => p.Id == productId).FirstOrDefault<Product>();
                    if (pro != null)
                    {
                        transDetail.ProductId = pro.Id;
                        //  transDetail.UnitPrice = pro.Price;
                        if (IsFOC)
                        {
                            transDetail.SellingPrice = 0;

                            transDetail.UnitPrice = 0;

                            transDetail.DiscountRate = 0;

                            transDetail.TaxRate = 0;
                        }
                        else
                        {
                            transDetail.SellingPrice = Utility.WholeSalePriceOrSellingPrice(pro, chkWholeSale.Checked);

                            transDetail.UnitPrice = Utility.WholeSalePriceOrSellingPrice(pro, chkWholeSale.Checked);

                            transDetail.DiscountRate = discountRate;

                            transDetail.TaxRate = Convert.ToDecimal(pro.Tax.TaxPercent);
                        }
                        transDetail.BatchNo = row.Cells[(int)sCol.colProductBatchNo].Value.ToString();

                        transDetail.Qty = qty;
                        transDetail.IsDeductedBy = txtMEMID.Text.Trim() != "" && pdp != null && transDetail.DiscountRate != 0 ? pdp.DiscountRate : (decimal?)null;
                        transDetail.BdDiscounted = IsBirthday == true && pro.DiscountRate == 0 ? SettingController.birthday_discount.ToString() : null;

                        // transDetail.TotalAmount = Convert.ToInt64(getActualCost(pro, discountRate, IsFOC)) * qty;
                        if (Convert.ToInt32(cboPaymentMethod.SelectedValue) == 4)
                        {
                            transDetail.TotalAmount = 0;
                        }
                        else
                        {
                            // by SYM
                            transDetail.TotalAmount = Convert.ToInt64(Convert.ToDecimal(row.Cells[(int)sCol.colCost].Value));
                            //transDetail.TotalAmount = Convert.ToInt64(getActualCost(pro, discountRate, IsFOC)) * qty;
                        }
                        transDetail.ConsignmentPrice = pro.ConsignmentPrice;
                        //transDetail.IsFOC = Convert.ToBoolean(row.Cells[8].Value);
                        if (row.Cells[9].Value.ToString() == "FOC")
                        {
                            transDetail.IsFOC = true;
                        }
                        else
                        {
                            transDetail.IsFOC = false;
                        }

                        DetailList.Add(transDetail);
                    }
                    //   }
                }
            }

            return DetailList;
        }

        private void UpdateTotalCost()
        {
            int discount = 0, tax = 0, totalqty = 0;
            total = 0;
            //int no_rows = dgvSalesItem.RowCount;
            foreach (DataGridViewRow dgrow in dgvSalesItem.Rows)
            {

                //check if the current one is new empty row
                if (!dgrow.IsNewRow && dgrow.Cells[(int)sCol.colProductCode].Value != null && dgrow.Cells[(int)sCol.colId].Value != null)
                {
                    string rowProductCode = string.Empty;
                    int qty = 0;
                    //rowProductCode = dgrow.Cells[1].Value.ToString().Trim();
                    rowProductCode = dgrow.Cells[(int)sCol.colProductCode].Value.ToString();
                    //Boolean IsFOC = Convert.ToBoolean(dgrow.Cells[8].Value);
                    Boolean IsFOC = false;
                    if (dgrow.Cells[(int)sCol.colFOC].Value == null)
                    {
                        IsFOC = false;
                    }
                    else if (dgrow.Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                    {
                        IsFOC = true;
                    }

                    if (rowProductCode != string.Empty && dgrow.Cells[(int)sCol.colQty].Value != null)
                    {
                        //Get qty
                        Int32.TryParse(dgrow.Cells[(int)sCol.colQty].Value.ToString(), out qty);

                        Product pro = iTempP.Where(p => p.ProductCode == rowProductCode).FirstOrDefault<Product>();

                        decimal productDiscount = 0;
                        if (dgrow.Cells[(int)sCol.colDiscountRate].Value != null)
                        {
                            Decimal.TryParse(dgrow.Cells[(int)sCol.colDiscountRate].Value.ToString(), out productDiscount);
                        }
                        else
                        {
                            productDiscount = pro.DiscountRate;
                        }
                        // by SYM
                        //total += (int)Math.Ceiling(getActualCost(pro, productDiscount, IsFOC) * qty);
                        total += (int)Math.Ceiling(Convert.ToDecimal(dgrow.Cells[(int)sCol.colCost].Value));
                        // discount += (int)Math.Ceiling(getDiscountAmount(pro.Price, productDiscount) * qty);
                        discount += (int)Math.Ceiling(getDiscountAmount(Utility.WholeSalePriceOrSellingPrice(pro, chkWholeSale.Checked), productDiscount) * qty);
                        tax += (int)Math.Ceiling(getTaxAmount(pro, IsFOC) * qty);
                        totalqty += qty;

                        //get discount % 0 Unit Price
                        if (productDiscount == 0)
                        {
                            //  decimal pricePerProduct = pro.Price * qty;
                            decimal pricePerProduct = Utility.WholeSalePriceOrSellingPrice(pro, chkWholeSale.Checked) * qty;


                        }


                        if (pro.IsConsignment == false)
                        {
                            //  decimal pricePerProduct = pro.Price * qty;
                            decimal pricePerProduct = Utility.WholeSalePriceOrSellingPrice(pro, chkWholeSale.Checked) * qty;

                        }
                        Check_MType();
                    }

                }

            }

            if (dgvSalesItem.Rows.Count == 1)
            {
                txtMCDiscount.Text = "0";
            }

            lblTotal.Text = total.ToString();
            lblDiscountTotal.Text = discount.ToString();
            lblTaxTotal.Text = tax.ToString();
            lblTotalQty.Text = totalqty.ToString();
            CalculateChargesAmount();
        }

        //private void Check_MTypeAndCalculation()
        //{
        //    ////int cId=Convert.ToInt32(cboCustomer.SelectedValue);
        //    ////var Sus = (from a in entity.Customers  where a.Id == cId select a).FirstOrDefault();
        //    ////if (Sus.Stus == null)
        //    ////{
        //    string[] focList = { "FOC", "Tester" };
        //    var mIdList = (from a in entity.PaymentTypes where !focList.Contains(a.Name) select a.Id).ToList();


        //    int paymentType = Convert.ToInt32(cboPaymentMethod.SelectedValue);

        //    if (mIdList.Contains(paymentType))
        //    {
        //        Calculate_MCDisocunt();
        //    }
        //    else
        //    {
        //        txtMCDiscount.Text = "0";
        //    }
        //}


        private void Calculate_MCDisocunt()
        {
            APP_Data.MemberCardRule data = (from a in entity.MemberCardRules where a.MemberTypeId == MemberTypeID select a).FirstOrDefault();
            decimal? _discount = 0;
            if (data != null)
            {
                int? cusId = Convert.ToInt32(cboCustomer.SelectedValue);

                string[] Birthday = { "", "-" };
                if (!Birthday.Contains(lblBirthday.Text))
                {
                    if (Convert.ToDateTime(lblBirthday.Text).Month == System.DateTime.Now.Month)
                    {
                        var bdList = (from t in entity.Transactions where t.CustomerId == cusId && t.BDDiscountAmt != 0 select t).ToList();
                        if (bdList.Count == 0)
                        {
                            _discount = data.BDDiscount;
                            DiscountType = "BD";
                        }
                        else
                        {
                            _discount = data.MCDiscount;
                            DiscountType = "MC";
                        }
                    }
                    else
                    {
                        _discount = data.MCDiscount;
                        DiscountType = "MC";
                    }
                }
                else
                {
                    _discount = data.MCDiscount;
                    DiscountType = "MC";
                }

                //if (txtAdditionalDiscount.Text == "")
                //{
                //    txtAdditionalDiscount.Text = "0";
                //}
                decimal? Dis = 0;
                // decimal? MinusDisTotal = ItemDiscountZeroAmt - Convert.ToDecimal(txtAdditionalDiscount.Text);
                decimal? MinusDisTotal = NonConsignProAmt;
                Dis = ((MinusDisTotal) / 100 * _discount);
                if (Dis > 0)
                {
                    txtMCDiscount.Text = Convert.ToInt32(Dis).ToString();
                }
                else
                {
                    txtMCDiscount.Text = "0";
                }
            }
        }

        private void Fill_Cus()
        {
            if (lblTotal.Text != "" || lblTotal.Text != "0")
            {
                int cId = Convert.ToInt32(cboCustomer.SelectedValue);
                MemberTypeID = (from c in entity.Customers where c.Id == cId select c.MemberTypeID).FirstOrDefault();
                if (txtMEMID.Text == "" || txtMEMID.Text == "-")
                {
                    if (MemberTypeID != null)
                    {
                        Calculate_MCDisocunt();
                    }
                    else
                    {
                        txtMCDiscount.Text = "0";
                    }
                }
                else
                {
                    Calculate_MCDisocunt();
                }
            }
        }

        //private void FirstTimeMember()
        //{
        //    List<MemberCardRule> mR = (from p in entity.MemberCardRules select p).ToList();
        //    var minRange = mR.Min(r => r.RangeFrom);
        //    int mTypeId = 0;
        //    string mType = "";


        //    int MinusDisTotal = total - Convert.ToInt32(txtAdditionalDiscount.Text);
        //    if (txtMEMID.Text == "")
        //    {
        //        if (MinusDisTotal >= Convert.ToInt32(minRange))
        //        {
        //            for (int i = 0; i <= mR.Count - 1; i++)
        //            {
        //                if (mR[i].RangeTo == "Above")
        //                {
        //                    //if (MinusDisTotal >= Convert.ToInt32(mR[i].RangeFrom))
        //                    //{
        //                    mR[i].RangeTo = (MinusDisTotal + 1).ToString();
        //                    //  }
        //                }
        //                if (Enumerable.Range(Convert.ToInt32(mR[i].RangeFrom), Convert.ToInt32(mR[i].RangeTo)).Contains(total))
        //                {
        //                    mTypeId = mR[i].MemberTypeId;
        //                    mType = (from p in entity.MemberTypes where p.Id == mTypeId select p.Name).FirstOrDefault();
        //                    break;
        //                }
        //            }
        //            Customer_Display(mType);
        //        }

        //    }
        //}



        private decimal getActualCost(Product prod, bool IsFOC)
        {
            if (IsFOC)
            {
                return 0;
            }
            decimal? actualCost = 0;

            //decrease discount ammount            
            // actualCost = prod.Price - ((prod.Price / 100) * prod.DiscountRate);

            actualCost = Utility.WholeSalePriceOrSellingPrice(prod, chkWholeSale.Checked) - ((Utility.WholeSalePriceOrSellingPrice(prod, chkWholeSale.Checked) / 100) * prod.DiscountRate);
            //add tax ammount            
            // actualCost = actualCost + ((prod.Price / 100) * prod.Tax.TaxPercent);
            actualCost = actualCost + ((Utility.WholeSalePriceOrSellingPrice(prod, chkWholeSale.Checked) / 100) * prod.Tax.TaxPercent);

            return (decimal)actualCost;
        }

        private decimal getActualCost(Product prod, decimal discountRate, Boolean IsFOC)
        {
            if (IsFOC)
            {
                return 0;
            }
            decimal? actualCost = 0;
            //decrease discount ammount            
            // actualCost = prod.Price - ((prod.Price / 100) * discountRate);
            actualCost = Utility.WholeSalePriceOrSellingPrice(prod, chkWholeSale.Checked) - ((Utility.WholeSalePriceOrSellingPrice(prod, chkWholeSale.Checked) / 100) * discountRate);
            //add tax ammount            
            actualCost = actualCost + ((Utility.WholeSalePriceOrSellingPrice(prod, chkWholeSale.Checked) / 100) * prod.Tax.TaxPercent);
            return (decimal)actualCost;
        }


        //private decimal getDiscountAmount(Product prod)
        //{
        //    return (((decimal)prod.Price / 100) * prod.DiscountRate);
        //}

        //private decimal getDiscountAmount(long productPrice, decimal productDiscount)
        //{
        //    return (((decimal)productPrice / 100) * productDiscount);
        //}
        private decimal getDiscountAmount(decimal productPrice, decimal productDiscount)
        {
            return (((decimal)productPrice / 100) * productDiscount);
        }

        private decimal getTaxAmount(Product prod, Boolean IsFOC)
        {
            if (IsFOC)
            {
                return 0;
            }
            return ((Utility.WholeSalePriceOrSellingPrice(prod, chkWholeSale.Checked) / 100) * Convert.ToDecimal(prod.Tax.TaxPercent));

        }





        public void Clear()
        {
            MemberOldLevel = 0;
            CurrentCustomerId = 0;
            entity = new POSEntities();
            dgvSalesItem.Rows.Clear();
            dgvSalesItem.Focus();
            //txtAdditionalDiscount.Text = "0";
            txtExtraTax.Text = "0";
            lblTotal.Text = "0";
            txtMCDiscount.Text = "0";
            lblTaxTotal.Text = "0";
            lblDiscountTotal.Text = "0";
            lblTotalQty.Text = "0";
            isDraft = false;
            DraftId = string.Empty;
            dgvSearchProductList.DataSource = "";
            cboProductName.SelectedIndex = 0;
            List<Product> productList = new List<Product>();
            Product productObj = new Product();
            productObj.Id = 0;
            productObj.Name = "";
            productList.Add(productObj);
            productList.AddRange(iTempP.Where(p => p.IsDeleted == false).ToList());
            cboProductName.DataSource = productList;
            cboProductName.DisplayMember = "Name";
            cboProductName.ValueMember = "Id";
            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;
            //cboCustomer.SelectedIndex = 0;
            txtMEMID.Text = "";
            lblMemberType.Text = "";
            lblPhone.Text = "";
            lblAddress.Text = "";
            lblBirthday.BackColor = System.Drawing.Color.Transparent;
            chkWholeSale.Checked = false;
            txtBarcode.Clear();
            _rowIndex = 0;
            IsBirthday = false;
            IsExportPts = false;
            lblbday.Text = "-";
            lblbday.BackColor = System.Drawing.Color.Transparent;
            lblBDMessage.Visible = false;

            cboPaymentMethod.SelectedIndex = 0;
            cboPaymentType.SelectedIndex = 0;
            txtAmount.Clear();
            dgvPaymentType.Rows.Clear();
            cboPaymentMethod.Enabled = true;
            cboPaymentType.Enabled = true;

            txtAmount.Enabled = true;
            btnPaymentAdd.Enabled = true;
            lblCharges.Text = "0";

            txtMemberId.Visible = true;
            lblMemberId.Visible = false;
            txtMemberId.Text = string.Empty;

            availablePList.Clear();
            ReloadCustomerList();
        }

        public void SetCurrentCustomer(Int32 CustomerId, bool isLoad)
        {
            cboCustomer.SelectedIndexChanged -= cboCustomer_SelectedIndexChanged;

            CurrentCustomerId = CustomerId;
            Customer currentCustomer = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();
            if (currentCustomer != null)
            {
                DateTime dtToday = DateTime.Now.Date;

                if (currentCustomer.Name != "Default" && currentCustomer.PromoteDate != null && currentCustomer.PromoteDate.Value != null && currentCustomer.PromoteDate.Value.Date != dtToday && currentCustomer.MemberTypeID != null && (currentCustomer.LatestRevokeDate == null || currentCustomer.LatestRevokeDate < dtToday) && backMonths4Reset > 0)
                {
                    

                    DateTime dtDate = DateTime.Now.AddMonths(0 - backMonths4Reset);
                    var iqTrans = entity.Transactions.Where(x => x.CustomerId == currentCustomer.Id && x.IsDeleted == false && x.IsComplete == true && x.DateTime >= dtDate && (x.Type == "Sale" || x.Type == "Special Member")).FirstOrDefault();
                    if (iqTrans == null)
                    {
                        DialogResult drs = MessageBox.Show("This customer has no transaction in the last " + backMonths4Reset + " months.", "Do you want to revoke?", MessageBoxButtons.YesNo);
                        if (drs == DialogResult.Yes)
                        {
                            Utility.InsertRevokeHistoryData(currentCustomer, "Auto revoke from sale from.");
                            currentCustomer.MemberTypeID = null;
                            currentCustomer.VIPMemberId = null;

                        }
                    }
                }
                lblNRIC.Text = currentCustomer.NRC;
                lblBirthday.Visible = false;
                lblBirthday.Text = currentCustomer.Birthday.ToString();
                lblPhone.Text = currentCustomer.PhoneNumber;
                lblAddress.Text = currentCustomer.Address;

                //lblPhoneNumber.Text = currentCustomer.PhoneNumber;

                if (currentCustomer.Birthday == null)
                {
                    lblbday.Text = "-";
                    lblBDMessage.ResetText();
                    IsBirthday = false;
                    lblbday.BackColor = System.Drawing.Color.Transparent;
                    if (dgvSalesItem.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgvSalesItem.Rows.Count - 1; i++)
                        {
                            if (dgvSalesItem.Rows[i].Cells[(int)sCol.colId].Value != null && !string.IsNullOrEmpty(dgvSalesItem.Rows[i].Cells[(int)sCol.colId].Value.ToString()))
                            {
                                string GridProductCOde = (string)dgvSalesItem.Rows[i].Cells[1].Value;
                                Product itemp = iTempP.Where(p => p.ProductCode == GridProductCOde).FirstOrDefault();
                                if (itemp != null && !isLoad)
                                {
                                    bool isFoc = false;
                                    if (dgvSalesItem.Rows[i].Cells[(int)sCol.colFOC].Value != null && dgvSalesItem.Rows[i].Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                                    {
                                        isFoc = true;
                                    }
                                    dgvSalesItem.Rows[i].Cells[(int)sCol.colDiscountRate].Value = itemp.DiscountRate.ToString();
                                    int qty = Convert.ToInt32(dgvSalesItem.Rows[i].Cells[(int)sCol.colQty].Value);
                                    dgvSalesItem.Rows[i].Cells[(int)sCol.colCost].Value = getActualCost(itemp, isFoc) * qty;
                                }
                            }
                            else
                            {
                                dgvSalesItem.Rows.RemoveAt(i);
                            }
                        }
                        UpdateTotalCost();
                    }
                }
                else
                {
                    var bod = Convert.ToDateTime(currentCustomer.Birthday).ToString("dd-MMM-yyyy");
                    lblbday.Text = bod.ToString();

                    int count = dgvSalesItem.Rows.Count;
                    if (Convert.ToDateTime(lblbday.Text).Month == System.DateTime.Now.Month && currentCustomer.VIPMemberId != null)
                    {
                        //int cusId = Convert.ToInt32(cboCustomer.SelectedValue);
                        //var bdList = (from t in entity.Transactions where t.CustomerId == cusId && t.BDDiscountAmt != 0 select t).ToList();
                        var bdPeryear = entity.Transactions.Where(t => t.CustomerId == CustomerId && t.DateTime.Value.Year == DateTime.Now.Year && false == t.IsDeleted)
                            .SelectMany(x => x.TransactionDetails).Where(td => td.DiscountRate > 0 && !string.IsNullOrEmpty(td.BdDiscounted)).ToList();

                        if (bdPeryear.Count == 0 && SettingController.birthday_discount > 0)
                        {
                            lblbday.BackColor = System.Drawing.Color.Yellow;
                            IsBirthday = true;

                            if (dgvSalesItem.Rows.Count > 0)
                            {

                                for (int i = 0; i < count - 1; i++)
                                {
                                    if (Convert.ToDecimal(dgvSalesItem.Rows[i].Cells[6].Value) == 0)
                                    {
                                        bool isFoc = false;
                                        if (dgvSalesItem.Rows[i].Cells[(int)sCol.colFOC].Value != null && dgvSalesItem.Rows[i].Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                                        {
                                            isFoc = true;
                                        }
                                        dgvSalesItem.Rows[i].Cells[6].Value = SettingController.birthday_discount;
                                        string GridProductCOde = (string)dgvSalesItem.Rows[i].Cells[1].Value;
                                        Product itemp = iTempP.Where(p => p.ProductCode == GridProductCOde).FirstOrDefault();
                                        if (itemp != null && itemp.DiscountRate == 0 && !isLoad)
                                        {
                                            int qty = Convert.ToInt32(dgvSalesItem.Rows[i].Cells[(int)sCol.colQty].Value);
                                            dgvSalesItem.Rows[i].Cells[(int)sCol.colCost].Value = getActualCost(itemp, SettingController.birthday_discount, isFoc) * qty;
                                        }
                                    }
                                }
                                lblBDMessage.Visible = true;
                                lblBDMessage.Text = "Birthday Discount ( " + SettingController.birthday_discount + "% ) will be discounted for a Transaction.";
                                UpdateTotalCost();
                            }
                        }
                        else
                        {
                            lblBDMessage.ResetText();
                            lblBDMessage.Text = "";
                            lblbday.BackColor = System.Drawing.Color.Transparent;
                            IsBirthday = false;
                        }
                    }
                    else
                    {
                        lblBDMessage.ResetText();
                        lblBDMessage.Text = "";
                        IsBirthday = false;
                        lblbday.BackColor = System.Drawing.Color.Transparent;
                        if (dgvSalesItem.Rows.Count > 0)
                        {
                            for (int i = 0; i < count - 1; i++)
                            {
                                string gridProductCode = (string)dgvSalesItem.Rows[i].Cells[1].Value.ToString();

                                Product itemp = iTempP.Where(p => p.ProductCode == gridProductCode).FirstOrDefault();
                                dgvSalesItem.Rows[i].Cells[6].Value = itemp.DiscountRate;// "0.0";
                                bool isFoc = false;
                                if (dgvSalesItem.Rows[i].Cells[(int)sCol.colFOC].Value != null && dgvSalesItem.Rows[i].Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                                {
                                    isFoc = true;
                                }
                                int qty = Convert.ToInt32(dgvSalesItem.Rows[i].Cells[(int)sCol.colQty].Value);
                                dgvSalesItem.Rows[i].Cells[(int)sCol.colCost].Value = getActualCost(itemp, isFoc) * qty;

                                //if (iTempP.Where(p => p.ProductCode == gridProductCode).FirstOrDefault().DiscountRate == 0)
                                //{
                                //    dgvSalesItem.Rows[i].Cells[6].Value = "0.0";
                                //    int qty = Convert.ToInt32(dgvSalesItem.Rows[i].Cells[(int)sCol.colQty].Value);
                                //    dgvSalesItem.Rows[i].Cells[(int)sCol.colCost].Value = getActualCost(itemp, isFoc) * qty;
                              
                                //}
                            }
                            UpdateTotalCost();


                        }
                    }
                }
                cboCustomer.Text = currentCustomer.Name;
                cboCustomer.SelectedValue = currentCustomer.Id;
                txtMEMID.Text = currentCustomer.VIPMemberId;

                int? MTID = currentCustomer.MemberTypeID;

                if (MTID != null)
                {
                    MemberType mt = entity.MemberTypes.Where(x => x.Id == MTID).FirstOrDefault();
                    lblMemberType.Text = mt.Name;
                }
                else
                {
                    lblMemberType.Text = "-";
                }
                //*SD*
                ///// Check_MType();////
            }
            cboCustomer.SelectedIndexChanged += cboCustomer_SelectedIndexChanged;

        }

        public void ReloadCustomerList()
        {

            List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
            cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
            entity = new POSEntities();
            //Add Customer List with default option
            APP_Data.Customer customer = new APP_Data.Customer();
            customer.Id = 0;
            customer.Name = "None";
            customerList.Add(customer);
            customerList.AddRange(entity.Customers.ToList());
            var count = customerList.Count();

            cboCustomer.DataSource = customerList;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";
            cboCustomer.Text = customerList.Where(x => x.Name == "Default").Select(x => x.Name).FirstOrDefault();



        }


        private void Cell_ReadOnly()
        {
            if (_rowIndex != -1 && dgvSalesItem.Rows.Count > _rowIndex)
            {

                DataGridViewRow row = dgvSalesItem.Rows[_rowIndex];
                if (_rowIndex > 0)
                {
                    if (row.Cells[1].Value != null)
                    {
                        string currentProductCode = row.Cells[1].Value.ToString();
                        List<string> _productList = dgvSalesItem.Rows
                               .OfType<DataGridViewRow>()
                               .Where(r => r.Cells[1].Value != null)
                               .Select(r => r.Cells[1].Value.ToString())
                               .ToList();

                        List<string> _checkProList = new List<string>();

                        _checkProList = (from p in _productList where p.Contains(currentProductCode) select p).ToList();
                        _checkProList.RemoveAt(_checkProList.Count - 1);
                        if (_checkProList.Count == 0)
                        {
                            dgvSalesItem.Rows[_rowIndex].Cells[0].ReadOnly = true;
                            dgvSalesItem.Rows[_rowIndex].Cells[1].ReadOnly = true;
                            dgvSalesItem.Rows[_rowIndex].Cells[2].ReadOnly = true;
                        }
                    }

                }

            }


            dgvSalesItem.CurrentCell = dgvSalesItem[0, dgvSalesItem.Rows.Count - 1];

        }

        #endregion



        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCustomer.SelectedIndex > 0)
            {
                int _currentCustomerID = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                lblbday.Text = "-";
                lblbday.BackColor = System.Drawing.Color.Transparent;

                SetCurrentCustomer(Convert.ToInt32(cboCustomer.SelectedValue.ToString()), false);
                if (cboPaymentMethod.Enabled == false)
                {
                    cboPaymentMethod.Enabled = true;
                    cboPaymentType.Enabled = true;
                    btnPaymentAdd.Enabled = true;
                    cboPaymentMethod.SelectedIndex = 0;
                    txtAmount.Enabled = true;
                }
                dgvPaymentType.Rows.Clear();
            }
            else
            {
                CurrentCustomerId = 0;
                lblbday.Text = "-";
                lblbday.BackColor = System.Drawing.Color.Transparent;

                //Clear customer data
                //lblCustomerName.Text = "-";
                lblBirthday.Text = "-";
                lblNRIC.Text = "-";
                // lblPhoneNumber.Text = "-";
                txtMEMID.Text = "-";
                lblMemberType.Text = "-";
                lblPhone.Text = "-";
                lblAddress.Text = "-";
                IsBirthday = false;
                IsExportPts = false;
            }

            UpdateTotalCost();


        }



        private void btnAddNewCustomer_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Customer.Add || MemberShip.isAdmin)
            {

                NewCustomer form = new NewCustomer();
                form.isEdit = false;
                form.Type = 'C';
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add new customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void LoadDraft(string TransactionId)
        {
            Clear();
            DraftId = TransactionId;

            POSEntities entity = new POSEntities();
            POS.APP_Data.Transaction delete_draft = entity.Transactions.Where(t => t.Id == DraftId).FirstOrDefault();

            Transaction draft = delete_draft.IsComplete == false ? delete_draft : null;
            //this.dgvSalesItem.CellValueChanged -= this.dgvSalesItem_CellValueChanged;
            if (draft != null)
            {
                var _tranDetails = (from a in entity.TransactionDetails where a.TransactionId == TransactionId select a).ToList();
                //pre add the rows
                availablePList.Clear();
                dgvSalesItem.Rows.Clear();
                // dgvSalesItem.Rows.Insert(0, draft.TransactionDetails.Count());
                int erowindex = 0;
                //foreach (TransactionDetail detail in draft.TransactionDetails)
                foreach (TransactionDetail detail in _tranDetails)
                {

                    //If product still exist
                    if (detail.Product != null)
                    {
                        Utility.AddProductAvailableQty(entity, (long)detail.ProductId, detail.BatchNo, (int)detail.Qty);

                        List<AvailableProductQtyWithBatch> tempProductControl = availablePList == null ? null : availablePList.Where(Product => Product.ProductID == detail.Product.Id && Product.BatchNo == detail.BatchNo).ToList();
                        if (tempProductControl == null || tempProductControl.Count == 0)
                        {
                            AddNew4AvailableProductQtyWithBatch(detail.Product.Id, detail.BatchNo);
                        }
                        tempProductControl = availablePList.Where(Product => Product.ProductID == detail.Product.Id && Product.BatchNo == detail.BatchNo).ToList();
                        if (tempProductControl == null)
                        {
                            DialogResult dr = MessageBox.Show("Can't Load all products from this draft, some products are not available at that time", "Do you want to continue?", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                            if (dr == DialogResult.OK)
                            {
                                continue;
                            }
                            else
                            {
                                isDraft = true;
                                // this.dgvSalesItem.CellValueChanged += this.dgvSalesItem_CellValueChanged;
                                chkWholeSale.CheckedChanged += new EventHandler(chkWholeSale_CheckedChanged);
                                return;
                            }
                        }

                        bool bFoc = false;
                        bool.TryParse(detail.IsFOC.ToString(), out bFoc);
                        decimal DisRate = detail.DiscountRate > 0 ? detail.DiscountRate : IsBirthday == true && detail.DiscountRate == 0 ? SettingController.birthday_discount : detail.DiscountRate;


                        UpdateDataGridViewProcess(detail.Product, tempProductControl, erowindex, bFoc, false, (int)detail.Qty, DisRate, detail.BatchNo);
                        erowindex++;
                        //}
                        //else
                        //{
                        //    isDraft = true;
                        //  //  this.dgvSalesItem.CellValueChanged += this.dgvSalesItem_CellValueChanged;

                        //    return;
                        //}
                    }
                }



                cboPaymentMethod.SelectedValue = draft.PaymentTypeId;
                //txtAdditionalDiscount.Text = draft.DiscountAmount.ToString();
                txtExtraTax.Text = draft.TaxAmount.ToString();

                chkWholeSale.CheckedChanged -= new EventHandler(chkWholeSale_CheckedChanged);
                chkWholeSale.Checked = Convert.ToBoolean(draft.IsWholeSale);

                if (draft.Customer != null)
                {
                    SetCurrentCustomer((int)draft.CustomerId, true);
                }
                UpdateTotalCost();


                //** delete draft transaction **

                delete_draft.TransactionDetails.Clear();
                var Detail = entity.TransactionDetails.Where(d => d.TransactionId == delete_draft.Id);
                foreach (var d in Detail)
                {
                    entity.TransactionDetails.Remove(d);
                }
                entity.Transactions.Remove(delete_draft);
                entity.SaveChanges();

            }
            else
            {
                //no associate transaction
                MessageBox.Show("The item doesn't exist anymore!");
            }

            isDraft = true;
            // this.dgvSalesItem.CellValueChanged += this.dgvSalesItem_CellValueChanged;
            chkWholeSale.CheckedChanged += new EventHandler(chkWholeSale_CheckedChanged);
        }

        internal bool DeleteCopy(string TransactionId)
        {
            int Qty = 0;
            this.cboPaymentMethod.SelectedIndexChanged -= cboPaymentMethod_SelectedIndexChanged;
            // this.dgvSalesItem.CellValueChanged -= this.dgvSalesItem_CellValueChanged;
            Boolean IsContinue = true; Boolean IsFormClosing = true;

            //Transaction draft = (from ts in entity.Transactions where ts.Id == TransactionId select ts).FirstOrDefault<Transaction>();
            Transaction draft = (from ts in entity.Transactions where ts.Id == TransactionId select ts).FirstOrDefault();

            if (draft.Type == "Credit")
            {
                CreditTransactionList _crList = new CreditTransactionList();

                IsContinue = _crList.Update_Settlement(draft, Convert.ToDateTime(draft.DateTime));
            }

            if (IsContinue)
            {
                Clear();
                draft = (from ts in entity.Transactions where ts.Id == TransactionId select ts).FirstOrDefault();
                draft.IsDeleted = true;
                DraftId = TransactionId;
                decimal taxTotal = 0;
                //Delete transaction
                // draft.IsDeleted = true;

                chkWholeSale.Checked = Convert.ToBoolean(draft.IsWholeSale);

                //  List<TransactionDetail> tempTransactionDetaillist = entity.TransactionDetails.Where(x => x.IsDeleted == false).ToList();

                // add gift card amount
                if (draft.GiftCardId != null && draft.GiftCard != null)
                {
                    draft.GiftCard.Amount = draft.GiftCard.Amount + Convert.ToInt32(draft.GiftCardAmount);
                }

                IEnumerable<TransactionDetail> iList = draft.TransactionDetails.Where(x => x.IsDeleted == false);
                var list = iList.ToList();
                foreach (TransactionDetail detail in iList)
                {

                    detail.IsDeleted = true;
                    detail.Product.Qty = detail.Product.Qty + detail.Qty;



                    // update Prepaid Transaction id = false   and delete list in useprepaiddebt table
                    Utility.Plus_PreaidAmt(draft);


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
                                Product wpObj = iTempP.Where(p => p.Id == w.ChildProductId).FirstOrDefault();
                                wpObj.Qty = wpObj.Qty + detail.Qty;
                                entity.SaveChanges();
                            }
                        }
                    }
                    entity.SaveChanges();
                }

                //save in stock transaction
                Save_SaleQty_ToStockTransaction(productList, Convert.ToDateTime(draft.DateTime));
                productList.Clear();

                DeleteLog dl = new DeleteLog();
                dl.DeletedDate = DateTime.Now;
                dl.CounterId = MemberShip.CounterId;
                dl.UserId = MemberShip.UserId;
                dl.IsParent = true;
                dl.TransactionId = draft.Id;

                entity.DeleteLogs.Add(dl);
                entity.SaveChanges();
                this.cboCustomer.SelectedIndexChanged -= cboCustomer_SelectedIndexChanged;

                //copy transaction
                if (draft != null)
                {

                    //pre add the rows
                    // dgvSalesItem.Rows.Insert(0, draft.TransactionDetails.Count());
                    //dgvSalesItem.Rows.Insert(0, list.Count());
                    int erowindex = 0;
                    // foreach (TransactionDetail detail in draft.TransactionDetails)
                    decimal eachDisAmount = 0;
                    foreach (TransactionDetail detail in list)
                    {
                        #region add qty in stock filling from sap KHS

                        Utility.AddProductAvailableQty(entity, (long)detail.ProductId, detail.BatchNo, (int)detail.Qty);

                        #endregion

                        List<AvailableProductQtyWithBatch> tempProductControl = availablePList == null ? null : availablePList.Where(Product => Product.ProductID == detail.Product.Id && Product.BatchNo == detail.BatchNo).ToList();
                        if (tempProductControl == null || tempProductControl.Count == 0)
                        {
                            AddNew4AvailableProductQtyWithBatch(detail.Product.Id, detail.BatchNo);
                        }
                        tempProductControl = availablePList.Where(Product => Product.ProductID == detail.Product.Id && Product.BatchNo == detail.BatchNo).ToList();
                        if (tempProductControl != null && tempProductControl.Count > 0)
                        {
                            //    MessageBox.Show("Can't Load all products from this transaction, some products are not available at that time", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //    //this.dgvSalesItem.CellValueChanged += this.dgvSalesItem_CellValueChanged;
                            //    this.cboPaymentMethod.SelectedIndexChanged += cboPaymentMethod_SelectedIndexChanged;
                            //    return false;
                            //}
                            decimal DisRate = detail.DiscountRate > 0 ? detail.DiscountRate : IsBirthday == true && detail.DiscountRate == 0 ? SettingController.birthday_discount : detail.DiscountRate;

                            bool bFoc = false;
                            bool.TryParse(detail.IsFOC.ToString(), out bFoc);
                            if (!bFoc)
                            {
                                eachDisAmount += (decimal)detail.UnitPrice * (DisRate / 100);
                            }
                            UpdateDataGridViewProcess(detail.Product, tempProductControl, erowindex, bFoc, false, (int)detail.Qty, DisRate, detail.BatchNo);
                            erowindex++;
                        }
                        else
                        {
                            MessageBox.Show("Can't Load all products from this transaction, some products are not available at that time.");
                        }
                    }
                    //cboPaymentMethod.SelectedValue = draft.PaymentTypeId;
                    //txtAdditionalDiscount.Text = ((int)draft.DiscountAmount - Decimal.ToInt32(eachDisAmount)).ToString();
                    txtExtraTax.Text = (draft.TaxAmount - taxTotal).ToString();

                    chkWholeSale.CheckedChanged -= new EventHandler(chkWholeSale_CheckedChanged);
                    chkWholeSale.Checked = Convert.ToBoolean(draft.IsWholeSale);


                    if (draft.Customer != null)
                    {
                        SetCurrentCustomer((int)draft.CustomerId, true);
                    }

                    if (draft.Type == "Credit")
                    {
                        cboPaymentMethod.Text = "Credit";
                    }
                    UpdateTotalCost();

                }
            }
            else
            {
                IsFormClosing = false;
            }
            this.cboCustomer.SelectedIndexChanged += cboCustomer_SelectedIndexChanged;

            // By SYM // For MemberType of Customer
            #region Member Type
            string minimumAmountofThisMemberType = (from mCardRule in entity.MemberCardRules.AsEnumerable() where mCardRule.IsActive == true orderby int.Parse(mCardRule.RangeFrom) select mCardRule.RangeFrom).FirstOrDefault();
            int memberValidityYear = Convert.ToInt32(SettingController.DefaultMemberValidityYear.ToString());
            int customerID = Convert.ToInt32(draft.CustomerId); // customerid, transactionid
            Customer currentCustomer = (from c in entity.Customers where c.Id == customerID select c).FirstOrDefault<Customer>();
            List<MemberCardRule> memberCardRuleList = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true orderby int.Parse(m.RangeFrom) select m).ToList();
            int totalAmount = 0;
            int giftCardTotalAmount = 0;
            int couponCodeTotalAmount = 0;
            int totalRAmount = 0;
            int memberTypeId = 0;

            if (currentCustomer.Name != "Default")
            {
                if (currentCustomer.MemberTypeID != null)
                {
                    if (draft.DateTime.Value.Date >= currentCustomer.StartDate && draft.DateTime.Value.Date <= currentCustomer.ExpireDate)
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

                        //Added by Lele
                        foreach (Transaction t in transactionList)
                        {
                            couponCodeTotalAmount += Convert.ToInt32(t.CouponCodeAmount);
                        }

                        //updated by lele
                        //totalAmount = totalAmount - giftCardTotalAmount;
                        totalAmount = totalAmount - (giftCardTotalAmount+couponCodeTotalAmount);

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

            //  this.dgvSalesItem.CellValueChanged += this.dgvSalesItem_CellValueChanged;
            chkWholeSale.CheckedChanged += new EventHandler(chkWholeSale_CheckedChanged);
            this.cboPaymentMethod.SelectedIndexChanged += cboPaymentMethod_SelectedIndexChanged;

            return IsFormClosing;
        }

        private decimal getActualCost(long productPrice, decimal productDiscount, decimal tax)
        {
            decimal? actualCost = 0;

            //decrease discount ammount            
            actualCost = productPrice - ((productPrice / 100) * productDiscount);
            //add tax ammount            
            actualCost = actualCost + ((productPrice / 100) * tax);
            return (decimal)actualCost;
        }

        private decimal getTaxAmount(long productPrice, decimal tax)
        {
            return ((productPrice / 100) * Convert.ToDecimal(tax));
        }


        private void txtMEMID_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtAdditionalDiscount_Leave(object sender, EventArgs e)
        {
            //Check_MType();//SD
            UpdateTotalCost();
        }

        private void txtNO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cboPaymentMethod_TextChanged(object sender, EventArgs e)
        {
            //  Check_MType();  SD
            UpdateTotalCost();
        }
        private void AvoidAction()
        {
            this.dgvSalesItem.CellLeave -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalesItem_CellLeave);
            this.dgvSalesItem.CellClick -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalesItem_CellClick);
            //this.dgvSalesItem.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalesItem_CellValueChanged);

            this.dgvSearchProductList.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.dgvSearchProductList_KeyDown);
            this.dgvSearchProductList.CellClick -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchProductList_CellClick);

        }
        private void ListenAction()
        {
            this.dgvSalesItem.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalesItem_CellLeave);
            this.dgvSalesItem.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalesItem_CellClick);
            //this.dgvSalesItem.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalesItem_CellValueChanged);

            this.dgvSearchProductList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvSearchProductList_KeyDown);
            this.dgvSearchProductList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchProductList_CellClick);

        }
        private void UpdateQtyUpdateDataView(DataGridViewRow row)
        {
            int defaultQty = 1;
            if (row.Cells[(int)sCol.colQty].Value != null && !string.IsNullOrEmpty(row.Cells[(int)sCol.colQty].Value.ToString()))
            {
                string stemp = row.Cells[(int)sCol.colQty].Value.ToString();
                if (!string.IsNullOrEmpty(stemp) || stemp != "0")
                {
                    int.TryParse(stemp, out defaultQty);
                }
            }
            else
            {
                row.Cells[(int)sCol.colQty].Value = 1;
            }

            bool Foc = false;
            if (row.Cells[(int)sCol.colFOC].Value != null && row.Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
            {
                Foc = true;
            }
            if (!DoProductQtyUpdateWithProductID(row, defaultQty, Foc))
            {
                row.Cells[(int)sCol.colQty].Value = 1;
            }
            if (row.Cells[(int)sCol.colId].Value != null)
            {
                int pid = Int32.Parse(row.Cells[(int)sCol.colId].Value.ToString());
                Product pro = iTempP.Where(pp => pp.Id == pid).FirstOrDefault();
                bool foc = false;
                if (row.Cells[(int)sCol.colFOC].Value != null && row.Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                {
                    foc = true;
                }
                decimal defaultDisRate = 0;
                if (row.Cells[(int)sCol.colDiscountRate].Value != null && !string.IsNullOrEmpty(row.Cells[(int)sCol.colDiscountRate].Value.ToString()))
                {
                    string stemp = row.Cells[(int)sCol.colDiscountRate].Value.ToString();
                    if (!string.IsNullOrEmpty(stemp) || stemp != "0")
                    {
                        decimal.TryParse(stemp, out defaultDisRate);
                    }
                }
                else
                {
                    row.Cells[(int)sCol.colDiscountRate].Value = 0.0;
                }
                row.Cells[(int)sCol.colCost].Value = (getActualCost(pro, defaultDisRate, foc) * (Int32.Parse(row.Cells[(int)sCol.colQty].Value.ToString())));
            }
        }

        private bool DoProductQtyUpdateWithProductID(DataGridViewRow row, int newComeQty, bool Foc)
        {
            try
            {
                int tproductId = Convert.ToInt32(row.Cells[(int)sCol.colId].Value.ToString());

                string tcurrentBath = row.Cells[(int)sCol.colProductBatchNo].Value.ToString();
                return UpdateItem4AvailableProductQtyWithBatch(tproductId, tcurrentBath, newComeQty, row, Foc);


            }
            catch (Exception ex)
            {
                return false;
                //  Utility.ShowErrMessage("AddNew4AvailableProductQtyWithBatch", ex.Message);
            }
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


        private void dgvSalesItem_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            TextBox prodCode = new TextBox();
            e.Control.KeyPress -= new KeyPressEventHandler(colQty_KeyPress);
            // prodCode.TextChanged -= new EventHandler(colProductName_TextChanged);
            // if (dgvSalesItem.CurrentCell.ColumnIndex == 2)

            if (dgvSalesItem.CurrentCell.OwningColumn.Name.Equals("colProductName"))
            {
                // TextBox prodCode = new TextBox();
                System.Windows.Forms.Control control = e.Control;
                prodCode = e.Control as TextBox;
                string text = prodCode.Text;
                if (prodCode != null)
                {
                    prodCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    prodCode.AutoCompleteCustomSource = AutoCompleteLoad();
                    prodCode.AutoCompleteSource = AutoCompleteSource.CustomSource;


                }
            }
            else if (dgvSalesItem.CurrentCell.OwningColumn.Name.Equals("colDiscountPercent")) //Desired Column
            {
                prodCode = e.Control as TextBox;
                if (prodCode != null)
                {
                    prodCode.AutoCompleteCustomSource = null;
                    prodCode.KeyPress += new KeyPressEventHandler(colQty_KeyPress);
                }
            }

            else if (dgvSalesItem.CurrentCell.OwningColumn.Name.Equals("colQty")) //Desired Column
            {
                prodCode = e.Control as TextBox;
                if (prodCode != null)
                {
                    prodCode.AutoCompleteCustomSource = null;
                    prodCode.KeyPress += new KeyPressEventHandler(colQty_KeyPress);
                }
            }
            else if (dgvSalesItem.CurrentCell.OwningColumn.Name.Equals("colBarCode") || dgvSalesItem.CurrentCell.OwningColumn.Name.Equals("colProductCode")) //Desired Column
            {
                prodCode = e.Control as TextBox;
                prodCode.AutoCompleteCustomSource = null;

            }


            else if (dgvSalesItem.CurrentCell.OwningColumn.Name.Equals("colProductBatchNo") && e.Control is ComboBox)
            {
                ComboBox comboBox = e.Control as ComboBox;
                if (comboBox.Items.Count < 2)
                {
                    return;
                }
                comboBox.Tag = comboBox.Text;                
                comboBox.SelectedIndexChanged += new System.EventHandler(cboBatchNo_SelectedIndexChanged);
                
            }

        }
        private void cboBatchNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox == null || string.IsNullOrEmpty(comboBox.Text))
            {
                return;
            }
           
            LastColumnBatchNoComboSelectionChanged(comboBox, dgvSalesItem.CurrentCell.RowIndex, e);   
           
        }
        
        
        private void LastColumnBatchNoComboSelectionChanged(ComboBox cb, int rowIndex, EventArgs e)
        {
            try
            {
                AvoidAction();
                int currentQty = 0;

                DataGridViewRow row = dgvSalesItem.Rows[rowIndex];
                string currentProductCode = row.Cells[(int)sCol.colProductCode].Value.ToString();

                //get current Project by Id
                Product pro = iTempP.Where(p => p.ProductCode == currentProductCode).FirstOrDefault<Product>();
                AvailableProductQtyWithBatch tempProductControl = availablePList == null ? null : availablePList.Where(Product => Product.ProductID == pro.Id).FirstOrDefault();
                string BatchNo = cb.Text;
                try
                {

                    int.TryParse(row.Cells[(int)sCol.colQty].Value.ToString(), out currentQty);

                    if (tempProductControl == null)
                    {
                        AddNew4AvailableProductQtyWithBatch(pro.Id, string.Empty);
                    }
                    tempProductControl = availablePList.Where(Product => Product.ProductID == pro.Id && Product.BatchNo == BatchNo).OrderBy(p => p.ExpireDate).FirstOrDefault();
                    int tBatch = CountProductQtyWithBatchInGrid(tempProductControl.ProductID.ToString(), BatchNo, false) + currentQty;

                    if (tempProductControl.OrgQty >= tBatch)
                    {
                        tempProductControl.InUseQty = tBatch;
                        tempProductControl.AvailableQty = tempProductControl.OrgQty - tBatch;
                        bool isFOC = false;
                        string oldBatch = row.Cells[colProductBatchNo.Index].Value.ToString();
                        if (row.Cells[(int)sCol.colFOC].Value != null && row.Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                        {
                            isFOC = true;
                            AvailableProductQtyWithBatch tempProductControl2 = availablePList.Where(Product => Product.ProductID == pro.Id && Product.BatchNo == oldBatch).FirstOrDefault();
                            tempProductControl2.InUseQty = NormalQtyCount(tempProductControl2.ProductID.ToString(), oldBatch);

                            tempProductControl2.AvailableQty = tempProductControl2.OrgQty - tempProductControl2.InUseQty;
                        }
                        else
                        {
                            AvailableProductQtyWithBatch tempProductControl2 = availablePList.Where(Product => Product.ProductID == pro.Id && Product.BatchNo == oldBatch).FirstOrDefault();
                            tempProductControl2.InUseQty = FOCQtyCount(tempProductControl2.ProductID.ToString(), oldBatch);

                            tempProductControl2.AvailableQty = tempProductControl2.OrgQty - tempProductControl2.InUseQty;

                        }

                         Check_SameProductCode_BatchNo(tempProductControl.ProductID, tempProductControl.BatchNo, isFOC);
                       
                    }
                    else
                    {
                        MessageBox.Show("Available product quantity is not enough for batch '" + BatchNo + "', current total available quantity is " + tempProductControl.OrgQty + ".");
                        if (cb.Tag != null && !string.IsNullOrEmpty(cb.Tag.ToString()))
                        {
                            //change back to orginal batch no before change
                            cb.SelectedIndexChanged -= new System.EventHandler(cboBatchNo_SelectedIndexChanged);
                            cb.Text = cb.Tag.ToString();
                            cb.SelectedIndexChanged += new System.EventHandler(cboBatchNo_SelectedIndexChanged);

                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Input quantity have invalid keywords.");
                    row.Cells[(int)sCol.colQty].Value = "1";

                    tempProductControl.AvailableQty -= 1 - tempProductControl.InUseQty;
                    tempProductControl.InUseQty = 1;
                    currentQty = 1;

                }

                // by SYM
                //update the total cost
                bool FOC = false;
                if (row.Cells[(int)sCol.colFOC].Value != null && row.Cells[(int)sCol.colUnitPrice].Value.ToString() == "FOC")
                {
                    FOC = true;
                }
                row.Cells[(int)sCol.colQty].Value = currentQty;
                row.Cells[(int)sCol.colUnitPrice].Value = Convert.ToInt32(getActualCost(pro, FOC));
                decimal discountrate = Convert.ToDecimal(row.Cells[(int)sCol.colDiscountRate].Value);
                row.Cells[(int)sCol.colCost].Value = currentQty * getActualCost(pro,discountrate, FOC);
                // isload = false;
                cb.Tag = cb.Text;
              


            }
            catch (Exception ex)
            {
                ListenAction();
            }
            ListenAction();
        }
        private bool Check_SameProductCode_BatchNo(long currentProductId, string batchNo, bool isFoc)
        {
            bool check = false;
            List<int> _indexCount = null;
         
            if (isFoc)
            {
                _indexCount = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                               where r.Cells[(int)sCol.colId].Value != null && Convert.ToInt64(r.Cells[(int)sCol.colId].Value.ToString()) == currentProductId &&
                               r.Cells[(int)sCol.colProductBatchNo].EditedFormattedValue.ToString() == batchNo && r.Cells[(int)sCol.colFOC].Value != null && r.Cells[(int)sCol.colFOC].Value.ToString() == "FOC"
                               select r.Index).ToList();

            }
            else
            {
                _indexCount = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                               where r.Cells[(int)sCol.colId].Value != null && Convert.ToInt64(r.Cells[(int)sCol.colId].Value.ToString()) == currentProductId &&
                               r.Cells[(int)sCol.colProductBatchNo].EditedFormattedValue.ToString() == batchNo && (r.Cells[(int)sCol.colFOC].Value == null || r.Cells[(int)sCol.colFOC].Value.ToString() != "FOC")
                               select r.Index).ToList();
            }

            if (_indexCount != null && _indexCount.Count > 1)
            {


                _indexCount.RemoveAt(_indexCount.Count - 1);

                int index = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                             where r.Cells[(int)sCol.colId].Value != null && Convert.ToInt64(r.Cells[(int)sCol.colId].Value.ToString()) == currentProductId
                              && r.Cells[(int)sCol.colProductBatchNo].Value.ToString() == batchNo
                             select r.Index).FirstOrDefault();



                int newQty = Convert.ToInt32(dgvSalesItem.Rows[index].Cells[(int)sCol.colQty].Value) + 1;
                dgvSalesItem.Rows[index].Cells[(int)sCol.colQty].Value = newQty;
                decimal drate = Convert.ToDecimal(dgvSalesItem.Rows[index].Cells[(int)sCol.colDiscountRate].Value) / 100;
                int UnitPrice = Convert.ToInt32(dgvSalesItem.Rows[index].Cells[(int)sCol.colUnitPrice].Value);
               
                dgvSalesItem.Rows[index].Cells[(int)sCol.colCost].Value = (UnitPrice - (UnitPrice * drate)) * newQty;
                
                dgvSalesItem.Rows[dgvSalesItem.Rows.Count - 2].Cells[(int)sCol.colId].Value = null; // to not include the last row in updatetotalcost()
                
                UpdateTotalCost();


                BeginInvoke(new Action(() => dgvSalesItem.EndEd‌​it())); // make row commit before deleting
                                                                         //dgvSalesItem.Rows.RemoveAt(dgvSalesItem.Rows.Count - 2);
                try
                {

                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        if (dgvSalesItem.Rows.Count > 1)
                            dgvSalesItem.Rows.RemoveAt(dgvSalesItem.Rows.Count - 2);
                    });


                   
                }

                catch
                { }
                check = true;

            }
            else
            {
                check = false;
            }

            return check;
        }

        
        private int CountProductQtyWithBatchInGrid(string currentProductId, string BatchNo, bool checkAllRow)
        {
            try
            {
                IEnumerable<DataGridViewRow> iViewRow = dgvSalesItem.Rows.Cast<DataGridViewRow>();
                if (!checkAllRow)
                {
                    iViewRow = iViewRow.Where(i => i.Index != dgvSalesItem.CurrentCell.RowIndex);
                }
                return (from r in iViewRow
                        where r.Cells[(int)sCol.colId].Value != null && r.Cells[(int)sCol.colId].Value.ToString() == currentProductId
                        && r.Cells[(int)sCol.colProductBatchNo].Value != null && r.Cells[(int)sCol.colQty].Value != null && r.Cells[(int)sCol.colProductBatchNo].Value.ToString() == BatchNo
                        select int.Parse(r.Cells[(int)sCol.colQty].Value.ToString())).Sum();

            }
            catch
            {
                return 0;
            }
        }
        private void colQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }



        public AutoCompleteStringCollection AutoCompleteLoad()
        {
            AutoCompleteStringCollection str = new AutoCompleteStringCollection();

            var product = iTempP.Where(x => x.IsDeleted == false).Select(x => x.Name).ToList();

            foreach (var p in product)
            {
                str.Add(p.ToString());
            }
            return str;
        }



        private void chkWholeSale_CheckedChanged(object sender, EventArgs e)
        {
            bool chk = chkWholeSale.Checked;
            DialogResult result = new DialogResult();
            string mssg = "";
            if (chk)
            {
                mssg = "Whole Sale";
            }
            else
            {
                mssg = "Retail Sale";
            }

            if (dgvSalesItem.Rows.Count > 1)
            {
                result = MessageBox.Show("Are you sure  want to sell with " + mssg + "? If  Yes, the datas will be clear!", "mPOS", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (result.Equals(DialogResult.Yes))
                {
                    btnCancel.PerformClick();
                }
            }

        }

        private void btnFOC_Click(object sender, EventArgs e)
        {

            txtBarcode.Focus();
            gbFOC.Visible = true;


            List<Product> productList = new List<Product>();
            Product productObj = new Product();
            productObj.Id = 0;
            productObj.Name = "Select All";
            productList.Add(productObj);
            productList.AddRange(iTempP);
            cboProduct.DataSource = productList;
            cboProduct.DisplayMember = "Name";
            cboProduct.ValueMember = "Id";
            cboProduct.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProduct.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboProduct.SelectedIndex = 0;
            txtBarcode.Clear();
            cboProduct.Focus();
        }

        private void Clear_FOC()
        {
            txtBarcode.Clear();
            cboProduct.SelectedIndex = 0;
            txtQty.Text = "0";
        }
        private void UpdateDataGridViewProcess(Product pro, List<AvailableProductQtyWithBatch> tempProductControl, int erowIndex, bool Foc, bool iManural, int pQty, decimal DiscountRate, string DefaultBatch)
        {
            try
            {
                if (tempProductControl != null)
                {
                    if (dgvSalesItem.IsCurrentCellDirty)
                    {
                        dgvSalesItem.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    }
                    BeginInvoke(new Action(() => dgvSalesItem.EndEd‌​it()));

                    int count = dgvSalesItem.Rows.Count;
                    int batchIndex = 0;
                    if (tempProductControl[0].BatchNo != DefaultBatch)
                    {
                        batchIndex = tempProductControl.FindIndex(x => x.BatchNo == DefaultBatch);
                    }
                    int foundedRowIndex = CheckProductInDataGridView(pro.ProductCode, DefaultBatch, Foc);
                    if (foundedRowIndex > -1)//loaded product
                    {
                        if (tempProductControl != null)
                        {
                            tempProductControl[batchIndex].InUseQty += pQty; //breakpoint here
                            tempProductControl[batchIndex].AvailableQty -= pQty;

                            DataGridViewRow row = (DataGridViewRow)dgvSalesItem.Rows[foundedRowIndex];
                            row.Cells[(int)sCol.colQty].Value = Int32.Parse(row.Cells["colQty"].Value.ToString()) + pQty;
                            if (Foc)
                            {
                                row.Cells[(int)sCol.colFOC].Value = "FOC";
                            }
                            else
                            {
                                decimal defaultDisRate = 0;
                                if (row.Cells[(int)sCol.colDiscountRate].Value != null && !string.IsNullOrEmpty(row.Cells[(int)sCol.colDiscountRate].Value.ToString()))
                                {
                                    string stemp = row.Cells[(int)sCol.colDiscountRate].Value.ToString();
                                    if (!string.IsNullOrEmpty(stemp) || stemp != "0")
                                    {
                                        decimal.TryParse(stemp, out defaultDisRate);
                                    }
                                }
                                else
                                {
                                    row.Cells[(int)sCol.colDiscountRate].Value = 0.0;
                                }
                                row.Cells[(int)sCol.colCost].Value = (getActualCost(pro, defaultDisRate, false) * (Int32.Parse(row.Cells[(int)sCol.colQty].Value.ToString())));
                                row.Cells[(int)sCol.colFOC].Value = "";

                            }
                            row.Cells[(int)sCol.colConsignmentPrice].Value = pro.ConsignmentPrice;
                            row.Cells[(int)sCol.colDelete].Value = coldelete;
                            if (dgvSalesItem.Rows[dgvSalesItem.CurrentRow.Index].Cells[(int)sCol.colId].Value == null)
                            {
                                dgvSalesItem.Rows[dgvSalesItem.CurrentRow.Index].Cells[0].Value = "";
                                dgvSalesItem.Rows[dgvSalesItem.CurrentRow.Index].Cells[1].Value = "";
                                dgvSalesItem.Rows[dgvSalesItem.CurrentRow.Index].Cells[2].Value = "";
                            }


                        }
                        else
                        {
                            MessageBox.Show("No more product is available..!");
                        }
                    }
                    else
                    {
                        int UnitPrice = Utility.WholeSalePriceOrSellingPrice(pro, chkWholeSale.Checked);
                        decimal totalcost = getActualCost(pro, DiscountRate, Foc);
                        DataGridViewRow row = (DataGridViewRow)dgvSalesItem.Rows[erowIndex].Clone();
                        try
                        {
                            callSimpleAddMethod(pro, tempProductControl, Foc, UnitPrice, DiscountRate, totalcost, pQty);
                        }
                        catch
                        {

                            if (iManural)
                            {


                                if (dgvSalesItem.Rows[dgvSalesItem.Rows.Count - 1].Cells[(int)sCol.colId].Value != null && !string.IsNullOrEmpty(dgvSalesItem.Rows[dgvSalesItem.Rows.Count - 1].Cells[(int)sCol.colId].Value.ToString()))
                                {
                                    dgvSalesItem.Rows.RemoveAt(dgvSalesItem.Rows.Count - 2);

                                }
                            }


                            row = (DataGridViewRow)dgvSalesItem.Rows[erowIndex];


                            row.Cells[(int)sCol.colBarCode].Value = pro.Barcode;
                            row.Cells[(int)sCol.colProductCode].Value = pro.ProductCode;
                            row.Cells[(int)sCol.colProductName].Value = pro.Name;
                            BindBatchNo(tempProductControl, row.Cells[(int)sCol.colProductBatchNo] as DataGridViewComboBoxCell, tempProductControl[0].BatchNo);
                            row.Cells[(int)sCol.colQty].Value = pQty;
                            row.Cells[(int)sCol.colTax].Value = pro.Tax.TaxPercent;
                            if (Foc)
                            {
                                row.Cells[(int)sCol.colUnitPrice].Value = "0";
                                row.Cells[(int)sCol.colDiscountRate].Value = "0";
                                row.Cells[(int)sCol.colCost].Value = "0";
                                row.Cells[(int)sCol.colFOC].Value = "FOC";
                                row.Cells[(int)sCol.colDiscountRate].ReadOnly = true;
                            }
                            else
                            {
                                row.Cells[(int)sCol.colUnitPrice].Value = UnitPrice;
                                row.Cells[(int)sCol.colDiscountRate].Value = DiscountRate;
                                row.Cells[(int)sCol.colCost].Value = (pQty * totalcost);
                                row.Cells[(int)sCol.colFOC].Value = "";
                            }
                            row.Cells[(int)sCol.colId].Value = pro.Id.ToString();
                            row.Cells[(int)sCol.colConsignmentPrice].Value = pro.ConsignmentPrice;
                            row.Cells[(int)sCol.colDelete].Value = coldelete;
                        }
                        tempProductControl[batchIndex].InUseQty += pQty; //breakpoint here
                        tempProductControl[batchIndex].AvailableQty -= pQty;

                    }


                    for (int i = dgvSalesItem.Rows.Count - 1; i > 0; i--)
                    {
                        //ready olny
                        try
                        {
                            if (dgvSalesItem.Rows[i - 1].Cells[(int)sCol.colId].Value != null && !string.IsNullOrEmpty(dgvSalesItem.Rows[i - 1].Cells[(int)sCol.colId].Value.ToString()))
                            {
                                dgvSalesItem.Rows[i - 1].Cells[(int)sCol.colBarCode].ReadOnly = true;
                                dgvSalesItem.Rows[i - 1].Cells[(int)sCol.colProductCode].ReadOnly = true;
                                dgvSalesItem.Rows[i - 1].Cells[(int)sCol.colProductName].ReadOnly = true;
                                break;
                            }
                            else
                            {
                                dgvSalesItem.CurrentCell = dgvSalesItem.Rows[i - 1].Cells[(int)sCol.colBarCode];
                                dgvSalesItem.Rows[i - 1].Cells[(int)sCol.colBarCode].Selected = true;
                                dgvSalesItem.Focus();
                            }
                        }
                        catch (Exception ex)
                        { }

                    }
                }
                else
                {
                    MessageBox.Show("Product Out of Stock..!");
                    // BeginInvoke(new Action(delegate { dgvSalesItem.Rows.Remove(dgvSalesItem.Rows[e.RowIndex]); }));
                }


                _rowIndex = dgvSalesItem.Rows.Count - 2;
                cboProductName.SelectedIndex = 0;
                dgvSearchProductList.DataSource = "";

                dgvSearchProductList.ClearSelection();
                dgvSalesItem.Focus();

                //  Cell_ReadOnly();



            }
            catch (Exception ex)
            {

            }
        }
        private void BindBatchNo(List<AvailableProductQtyWithBatch> tempProductControlList, DataGridViewComboBoxCell dataGridViewComboBoxCell, string defaultbatch)
        {
            dataGridViewComboBoxCell.Items.Clear();
            tempProductControlList.ForEach(delegate (AvailableProductQtyWithBatch stockIn)
            {
                dataGridViewComboBoxCell.Items.Add(stockIn.BatchNo);
            });
            dataGridViewComboBoxCell.Value = defaultbatch;
        }
        private void callSimpleAddMethod(Product pro, List<AvailableProductQtyWithBatch> tempProductControl, bool Foc, int UnitPrice, decimal DisRate, decimal totalcost, int pQty)
        {
            dgvSalesItem.Rows.Add();
            int rIndex = 0;
            foreach (DataGridViewRow r in dgvSalesItem.Rows)
            {
                if (dgvSalesItem.Rows.Count <= 2)
                    break;
                else if (r.Cells[(int)sCol.colId].Value == null || string.IsNullOrEmpty(r.Cells[(int)sCol.colId].Value.ToString()))
                {
                    rIndex = r.Index; break;
                }
            }
            DataGridViewRow row = (DataGridViewRow)dgvSalesItem.Rows[rIndex];


            row.Cells[(int)sCol.colBarCode].Value = pro.Barcode;
            row.Cells[(int)sCol.colProductCode].Value = pro.ProductCode;
            row.Cells[(int)sCol.colProductName].Value = pro.Name;
            BindBatchNo(tempProductControl, row.Cells[(int)sCol.colProductBatchNo] as DataGridViewComboBoxCell, tempProductControl[0].BatchNo);
            row.Cells[(int)sCol.colQty].Value = pQty;

            row.Cells[(int)sCol.colTax].Value = pro.Tax.TaxPercent;
            if (Foc)
            {
                row.Cells[(int)sCol.colUnitPrice].Value = "0";
                row.Cells[(int)sCol.colDiscountRate].Value = "0";
                row.Cells[(int)sCol.colCost].Value = "0";
                row.Cells[(int)sCol.colFOC].Value = "FOC";
                row.Cells[(int)sCol.colDiscountRate].ReadOnly = true;
            }
            else
            {
                row.Cells[(int)sCol.colUnitPrice].Value = UnitPrice;
                row.Cells[(int)sCol.colDiscountRate].Value = DisRate;
                row.Cells[(int)sCol.colCost].Value = (pQty * totalcost);
                row.Cells[(int)sCol.colFOC].Value = "";
            }

            row.Cells[(int)sCol.colId].Value = pro.Id;
            row.Cells[(int)sCol.colConsignmentPrice].Value = pro.ConsignmentPrice;
            row.Cells[(int)sCol.colDelete].Value = coldelete;
        }

        internal void Add_DataToGrid(int currentProductId)
        {
            //RemoveNoNeedRows();
            Product pro = iTempP.Where(p => p.Id == currentProductId).FirstOrDefault<Product>();
            if (pro != null)
            {
                List<AvailableProductQtyWithBatch> tempProductControl = availablePList == null ? null : availablePList.Where(Product => Product.ProductID == currentProductId).ToList();
                if (tempProductControl == null || tempProductControl.Count == 0)
                {
                    AddNew4AvailableProductQtyWithBatch(currentProductId, string.Empty);
                }
                tempProductControl = availablePList.Where(Product => Product.ProductID == currentProductId && Product.AvailableQty > 0).OrderBy(p => p.ExpireDate).ToList();
                if (tempProductControl != null && tempProductControl.Count > 0)
                {
                    if (tempProductControl[0].AvailableQty >= FOCQty)
                    {
                        //>= (Qty - Product.InUseQty)
                        //tempProductControl.AvailableQty -= FOCQty;
                        //tempProductControl.InUseQty += FOCQty;

                        UpdateDataGridViewProcess(pro, tempProductControl, dgvSalesItem.RowCount - 1, true, false, FOCQty, 0, tempProductControl[0].BatchNo);
                    }
                    else
                    {
                        int normal = NormalQtyCount(tempProductControl[0].ProductID.ToString(), tempProductControl[0].BatchNo);
                        Utility.ShowErrMessage("Current batch is insufficient!", "Current batch's available product quantity is " + (tempProductControl[0].OrgQty - normal).ToString());
                        //tempProductControl.InUseQty = tempProductControl.OrgQty;
                        //tempProductControl.AvailableQty = 0;
                        //decimal DisRate = pro.DiscountRate > 0 ? pro.DiscountRate : IsBirthday == true && pro.DiscountRate == 0 ? SettingController.birthday_discount : pro.DiscountRate;
                        //pro.DiscountRate = pro.DiscountRate > 0 ? pro.DiscountRate : IsBirthday == true && pro.DiscountRate == 0 ? SettingController.birthday_discount : pro.DiscountRate;

                        UpdateDataGridViewProcess(pro, tempProductControl, dgvSalesItem.RowCount - 1, true, false, tempProductControl[0].OrgQty - normal, 0, tempProductControl[0].BatchNo);

                    }

                }
                else
                {
                    MessageBox.Show("Product Out of Stock..!");


                }

            }
        }
        //private void RemoveNoNeedRows()
        //{
        //    try
        //    {
        //        List<int> index = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
        //                           where r.Cells[4].Value == null || r.Cells[4].Value.ToString() == String.Empty || r.Cells[4].Value.ToString() == "0"
        //                           select r.Index).ToList();


        //        index.RemoveAt(index.Count - 1);

        //        if (index.Count > 0)
        //        {

        //            foreach (var a in index)
        //            {
        //                try
        //                {
        //                    DataGridViewCell ir = dgvSalesItem.CurrentCell;
        //                    if (a == 0 || ir == null || string.IsNullOrEmpty(ir.ToString())) continue;
        //                        BeginInvoke(new Action(delegate { dgvSalesItem.Rows.Remove(dgvSalesItem.Rows[a]); }));

        //                }

        //                catch { }
        //            }
        //        }
        //   catch { }
        //}





        //private bool Check_ProductFOCCode_Exist(string currentProductCode)
        //{
        //    bool check = false;
        //    //     string currentProductCode = dgvSalesItem.Rows[_rowIndex].Cells[1].Value.ToString();
        //    List<int> _indexCount = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
        //                             where r.Cells[1].Value != null && r.Cells[1].Value.ToString() == currentProductCode
        //                             //&& Convert.ToBoolean(r.Cells[8].Value) == true
        //                             && (r.Cells[8].Value.ToString() == "FOC")
        //                             select r.Index).ToList();
        //    //  }

        //    if (_indexCount.Count > 1)
        //    {
        //        _indexCount.RemoveAt(_indexCount.Count - 1);

        //        int index = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
        //                     where r.Cells[1].Value != null && r.Cells[1].Value.ToString() == currentProductCode
        //                      // && Convert.ToBoolean(r.Cells[8].Value) == true
        //                      && (r.Cells[8].Value.ToString() == "FOC")
        //                     select r.Index).FirstOrDefault();




        //        dgvSalesItem.Rows[index].Cells[4].Value = Convert.ToInt32(dgvSalesItem.Rows[index].Cells[4].Value) + FOCQty;
        //        // dgvSalesItem.Rows.RemoveAt(dgvSalesItem.Rows.Count-2);
        //        BeginInvoke(new Action(delegate { dgvSalesItem.Rows.RemoveAt(dgvSalesItem.Rows.Count - 2); }));

        //        dgvSalesItem.Rows[dgvSalesItem.Rows.Count - 2].Cells[12].Value = "delete";
        //        check = true;

        //    }
        //    return check;
        //}



        private void txtBarcode_Leave(object sender, EventArgs e)
        {
            Barcode_Input();
        }

        private void Barcode_Input()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtBarcode.Text))
                {
                    string _barcode = txtBarcode.Text;
                    long productId = iTempP.Where(p => p.Barcode == _barcode && p.IsConsignment == false).FirstOrDefault().Id;
                    cboProduct.SelectedValue = productId;
                    cboProduct.Focus();
                }
            }
            catch { }
        }

        private void cboProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProduct.SelectedIndex > 0)
            {
                long productId = Convert.ToInt32(cboProduct.SelectedValue);
                string barcode = iTempP.Where(p => p.Id == productId).FirstOrDefault().Barcode;
                txtBarcode.Text = barcode;
                txtQty.Text = "1";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtBarcode.Text.Trim() != string.Empty && cboProduct.SelectedIndex > 0 && txtQty.Text.Trim() != string.Empty && Convert.ToInt32(txtQty.Text) > 0)
            {
                FOCQty = Convert.ToInt32(txtQty.Text);
                int _proId = Convert.ToInt32(cboProduct.SelectedValue);
                Add_DataToGrid(_proId);
                Clear_FOC();
            }
            // gbFOC.Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            gbFOC.Visible = false;
        }

        private void cboCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cboCustomer.DroppedDown)
            {
                cboCustomer.DroppedDown = false;
            }
        }

        private void cboCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (cboCustomer.DroppedDown)
            {
                cboCustomer.DroppedDown = false;
            }
        }

        private void CalculateChargesAmount()
        {
            int paidAmount = 0; bool isFoc = false;
            int discount = 0;
            //if (!String.IsNullOrWhiteSpace(txtAdditionalDiscount.Text))
            //{
            //    discount = Convert.ToInt32(txtAdditionalDiscount.Text);
            //}

            foreach (DataGridViewRow row in dgvPaymentType.Rows)
            {
                if (row.Cells[5].Value == null)
                {
                    paidAmount = 0;
                    isFoc = true;
                }
                else
                {
                    paidAmount += Convert.ToInt32(row.Cells[5].Value);

                }
            }
            int totalAmount = Convert.ToInt32(lblTotal.Text);
            int changesAmount = totalAmount - (paidAmount + discount);
            lblCharges.Text = changesAmount >= 0 ? changesAmount.ToString() : (changesAmount * -1).ToString();
            labelChanges.Text = changesAmount >= 0 ? "Payable Amount" : "Changes";
            if (isFoc)
            {
                lblCharges.Text = "0";
            }
        }
        private bool verifyDiscount()
        {
            int dis = 0;
            int.TryParse(lblDiscountTotal.Text, out dis);
            if (dis > 0)
            {
                MessageBox.Show("Cannot use product discount and gift card at the same time..", "Unable to add/pay!");
                return false;
            }

            return true;
        }
        private void btnPaymentAdd_Click(object sender, EventArgs e)
        {
            if (cboPaymentType.SelectedIndex != -1 && cboPaymentType.Text.Trim() != "FOC")
            {

                if (!string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    if (cboPaymentType.Text.Trim() == "Gift Card")
                    {
                        if (!verifyDiscount())
                        {
                            return;
                        }
                        Boolean hasError = false;
                        string CardNumber = txtAmount.Text.Trim();
                        GiftCard currentCard = (from gcard in entity.GiftCards where gcard.CardNumber == CardNumber && gcard.IsUsed == false && gcard.IsDelete == false select gcard).FirstOrDefault<GiftCard>();

                        //GiftCard is invalid
                        if (currentCard == null)
                        {
                            MessageBox.Show("Card is already used or invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            hasError = true;
                        }
                        else if (currentCard.CustomerId != Convert.ToInt32(cboCustomer.SelectedValue))
                        {
                            MessageBox.Show("This card is not belong to current customer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            hasError = true;
                        }
                        else
                        {
                            //if GiftCard Already in the list
                            foreach (DataGridViewRow row in dgvPaymentType.Rows)
                            {
                                if (Convert.ToString(row.Cells[1].Value).Trim() == "Gift Card")
                                {
                                    if (Convert.ToString(row.Cells[3].Value).Trim() == currentCard.CardNumber.Trim())
                                    {
                                        MessageBox.Show("Card already in the list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        hasError = true;
                                    }
                                }

                            }
                        }

                        if (!hasError)
                        {
                            dgvPaymentType.Rows.Add(Convert.ToInt32(cboPaymentType.SelectedValue), cboPaymentMethod.Text.Trim(), cboPaymentType.Text.Trim(), txtAmount.Text, "Delete", currentCard.Amount, Convert.ToInt32(cboPaymentMethod.SelectedValue));
                            cboPaymentMethod.SelectedIndex = 0;
                            cboPaymentType.SelectedIndex = 0;
                            txtAmount.Clear();
                            CalculateChargesAmount();
                        }
                    }
                    //Added by Lele on 2023-Dec-18
                    else if (cboPaymentMethod.Text.Trim() == "Coupon Code")
                    {
                        Boolean hasError = false;

                        //if Coupon Already in the list
                        foreach (DataGridViewRow row in dgvPaymentType.Rows)
                        {
                            if (Convert.ToString(row.Cells[1].Value).Trim() == "Coupon Code")
                            {
                                MessageBox.Show("Coupon already in the list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                hasError = true;
                            }
                        }

                        if (!hasError)
                        {
                            dgvPaymentType.Rows.Add(Convert.ToInt32(cboPaymentType.SelectedValue), cboPaymentMethod.Text.Trim(), cboPaymentType.Text.Trim(), txtAmount.Text, "Delete", txtAmount.Text, Convert.ToInt32(cboPaymentMethod.SelectedValue));
                            cboPaymentMethod.SelectedIndex = 0;
                            cboPaymentType.SelectedIndex = 0;
                            txtAmount.Clear();
                            CalculateChargesAmount();
                        }
                    }
                    else
                    {
                        dgvPaymentType.Rows.Add(Convert.ToInt32(cboPaymentType.SelectedValue), cboPaymentMethod.Text.Trim(), cboPaymentType.Text.Trim(), txtAmount.Text, "Delete", txtAmount.Text, Convert.ToInt32(cboPaymentMethod.SelectedValue));
                        cboPaymentMethod.SelectedIndex = 0;
                        cboPaymentType.SelectedIndex = 0;
                        txtAmount.Clear();
                        CalculateChargesAmount();
                    }

                }
            }
            //For FOC
            else
            {
                DialogResult result = MessageBox.Show("Are you sure FOC invoice", "FOC", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result.Equals(DialogResult.OK))
                {

                    dgvPaymentType.Rows.Clear();
                    dgvPaymentType.Rows.Add(Convert.ToInt32(cboPaymentType.SelectedValue), cboPaymentMethod.Text.Trim(), cboPaymentType.Text.Trim(), txtAmount.Text, "Delete", null, Convert.ToInt32(cboPaymentMethod.SelectedValue));
                    cboPaymentMethod.Enabled = false;
                    cboPaymentType.Enabled = false;
                    btnPaymentAdd.Enabled = false;
                    lblCharges.Text = "0";
                }
            }
            int finalrows = dgvPaymentType.Rows.Count;


        }

        private void cboPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedId = Convert.ToInt32(cboPaymentMethod.SelectedValue);
            cboPaymentType.DataSource = null;
            var paymentType = iTempSubPayment.Where(x => x.PaymentTypeId == selectedId).ToList();
            cboPaymentType.DataSource = paymentType;
            cboPaymentType.DisplayMember = "Name";
            cboPaymentType.ValueMember = "Id";

            //Added by Lele on 2023-Dec-18, Allow user to select coupon code
            
            if (cboPaymentMethod.Text.Trim() == "Coupon Code")
            {
                cboPaymentType.DataSource = null;
                List<APP_Data.CouponCode> couponcodeNoList = new List<APP_Data.CouponCode>();
                APP_Data.CouponCode defaultCoupon = new APP_Data.CouponCode();
                defaultCoupon.Id = 0;
                defaultCoupon.CouponCodeNo = "Select Coupon Code";
                couponcodeNoList.Add(defaultCoupon);
                couponcodeNoList.AddRange(entity.CouponCodes.Where(x => x.IsDelete == false).OrderBy(x => x.CouponCodeNo).ToList());
                cboPaymentType.DataSource = couponcodeNoList;
                cboPaymentType.DisplayMember = "CouponCodeNo";
                cboPaymentType.ValueMember = "Id";
                cboPaymentType.SelectedIndex = 0;
            }
            
            //cboPaymentType.SelectedIndex = 0;
            txtAmount.Enabled = cboPaymentType.Text.Trim() == "FOC" || cboPaymentType.Text.Trim() == "Select Coupon Code" ? false : true;
            if (cboPaymentType.Text.Trim() == "FOC")
            {
                //txtAdditionalDiscount.Text = "0";
                txtAmount.Text = "0";
                lblCharges.Text = "0";
                lblTotal.Text = "0";
                lblDiscountTotal.Text = "0";
                lblTaxTotal.Text = "0";

            }
        }

        private void cboPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAmount.Clear();

            //Added by Lele on 2023-Dec-18, Get coupon amount from tbl.CouponCode and not allow user to edit
            if (cboPaymentMethod.Text.Trim() == "Coupon Code")
            {
                int couponcodeId = Convert.ToInt32(cboPaymentType.SelectedValue);
                if (couponcodeId != 0)
                {
                    var couponcodeAmt = entity.CouponCodes.Where(x => x.Id == couponcodeId).Select(x => new { x.Amount }).FirstOrDefault();
                    if (couponcodeAmt!=null)
                    {

                        // Regular expression to match decimal values with exactly two decimal places
                        string pattern = @"[-+]?\b\d+(\.\d{2})\b";

                        // Match decimal values using regex
                        MatchCollection matches = Regex.Matches(couponcodeAmt.ToString(), pattern);

                        foreach (Match match in matches)
                        {
                            string sAmount = match.ToString();
                            txtAmount.Text = sAmount.Replace(".00", string.Empty);
                        }
                    }
                }

                txtAmount.Enabled = false;
            }
        }

        private void dgvPaymentType_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowindex = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                //Delete
                if (e.ColumnIndex == 4)
                {
                    string paymentTypeName = Convert.ToString(dgvPaymentType[1, e.RowIndex].Value);

                    dgvPaymentType.Rows.RemoveAt(e.RowIndex);
                    CalculateChargesAmount();

                    if (paymentTypeName.Trim() == "FOC")
                    {
                        cboPaymentMethod.Enabled = true;
                        cboPaymentMethod.SelectedIndex = 0;
                        cboPaymentType.Enabled = true;
                        cboPaymentType.SelectedIndex = 0;
                        btnPaymentAdd.Enabled = true;
                    }
                }
            }
        }

        private void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnPaymentAdd_Click(sender, e);
            }
        }
        
        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cboPaymentMethod.Text.Trim() != "Gift Card")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }
        private void Sales_Load(object sender, EventArgs e)
        {


            #region Setting Hot Kyes For the Controls
            SendKeys.Send("%"); SendKeys.Send("%"); // Clicking "Alt" on page load to show underline of Hot Keys
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form_KeyDown);
            #endregion

            #region Disable Sort Mode of dgvSaleItem Grid
            foreach (DataGridViewColumn col in dgvSalesItem.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            #endregion
            Thread tLoadProduct = new Thread(LoadProductNameList);
            Thread tLoadPaymentMethod = new Thread(LoadPaymentMethod);

            // Set the priority of threads
            tLoadProduct.Priority = ThreadPriority.Highest;
            tLoadPaymentMethod.Priority = ThreadPriority.AboveNormal;
            ReloadCustomerList();

            tLoadProduct.Start();
            tLoadPaymentMethod.Start();

            dgvSalesItem.ColumnHeadersDefaultCellStyle.Font = new Font("Zawgyi-One", 9F);
            dgvSalesItem.Focus();
            CheckExported();
            lblMemberId.Visible = false;//Added by YMO
            backMonths4Reset = SettingController.MemberTypeResetBackMonth;
        }

        private void LoadProductNameList()
        {
            List<Product> productList = new List<Product>();

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {

                    Product productObj = new Product();
                    productObj.Id = 0;
                    productObj.Name = "";
                    productList.Add(productObj);
                    if (iTempP == null)
                    {
                        getCommonProduct();
                    }
                    productList.AddRange(iTempP.Where(x => x.IsDeleted == false).ToList());
                    cboProductName.DataSource = productList;
                    cboProductName.DisplayMember = "Name";
                    cboProductName.ValueMember = "Id";
                }));
                return;
            }

            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        public static IQueryable<Product> iTempP { get; set; }

        public static void getCommonProduct()
        {
            POSEntities entity = new POSEntities();
            iTempP = entity.Products.Where(p => p.IsDeleted == false);
        }


        public static List<POS.APP_Data.PaymentMethod> iTempSubPayment { get; set; }
        public static List<POS.APP_Data.PaymentType> iTempPaymentType { get; set; }
        public static void getCommonPaymentMethod()
        {
            POSEntities entity = new POSEntities();
            //iTempSubPayment = entity.PaymentMethods.Where(x => (!string.IsNullOrEmpty(x.AccountCode.Trim()) || (string.IsNullOrEmpty(x.AccountCode) && (x.Name == "FOC" || x.Name == "Tester")))).ToList();
            //Updated by Lele on 2023-Dec-18, No AccountCode for coupon code
            iTempSubPayment = entity.PaymentMethods.Where(x => (!string.IsNullOrEmpty(x.AccountCode.Trim()) || (string.IsNullOrEmpty(x.AccountCode) && (x.Name == "FOC" || x.Name == "Tester" || x.Name == "Coupon Code")))).ToList();

        }


        public static void getCommonPaymentType()
        {
            POSEntities entity = new POSEntities();
            //iTempPaymentType = entity.PaymentTypes.Where(x => !x.Name.Contains("Multi")).ToList();
            iTempPaymentType = entity.PaymentTypes.Where(x => !x.Name.Contains("Multi")).ToList();
            //iTempSubPayment = entity.PaymentMethods.Where(x => (!string.IsNullOrEmpty(x.AccountCode.Trim()) || (string.IsNullOrEmpty(x.AccountCode) && (x.Name == "FOC" || x.Name == "Tester")))).ToList();
            //Updated by Lele on 2023-Dec-18, No AccountCode for coupon code
            iTempSubPayment = entity.PaymentMethods.Where(x => (!string.IsNullOrEmpty(x.AccountCode.Trim()) || (string.IsNullOrEmpty(x.AccountCode) && (x.Name == "FOC" || x.Name == "Tester" || x.Name == "Coupon Code")))).ToList();
        }
        private void LoadPaymentMethod()
        {
            this.cboPaymentMethod.TextChanged -= new EventHandler(cboPaymentMethod_TextChanged);
            cboPaymentMethod.SelectedIndexChanged -= cboPaymentMethod_SelectedIndexChanged;
            cboPaymentType.SelectedIndexChanged -= cboPaymentType_SelectedIndexChanged;
            if (iTempPaymentType == null)
            {
                getCommonPaymentType();
            }
            if (iTempSubPayment == null)
            {
                getCommonPaymentMethod();
            }
            //if (iStockFillingFromSAP == null)
            //{
            //    getStockFillingFromSAP();
            //}
            POSEntities entity = new POSEntities();
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    dgvSearchProductList.AutoGenerateColumns = false;
                    cboPaymentMethod.DataSource = iTempPaymentType.ToList();
                    cboPaymentMethod.DisplayMember = "Name";
                    cboPaymentMethod.ValueMember = "Id";
                    cboPaymentMethod.SelectedIndex = 0;
                    cboPaymentType.DataSource = iTempSubPayment.Where(x => x.Id == 1).ToList();
                    cboPaymentType.DisplayMember = "Name";
                    cboPaymentType.ValueMember = "Id";
                    cboPaymentType.SelectedIndex = 0;
                }));
                this.cboPaymentMethod.TextChanged += new EventHandler(cboPaymentMethod_TextChanged);
                cboPaymentMethod.SelectedIndexChanged += cboPaymentMethod_SelectedIndexChanged;
                cboPaymentType.SelectedIndexChanged += cboPaymentType_SelectedIndexChanged;

                return;
            }
            this.cboPaymentMethod.TextChanged += new EventHandler(cboPaymentMethod_TextChanged);
            cboPaymentMethod.SelectedIndexChanged += cboPaymentMethod_SelectedIndexChanged;

            cboPaymentType.SelectedIndexChanged += cboPaymentType_SelectedIndexChanged;
            cboPaymentMethod_SelectedIndexChanged(null, null);

            if (iTempP == null)
            {
                getCommonProduct();
            }

            List<Product> productList = new List<Product>();
            Product productObj = new Product();
            productObj.Id = 0;
            productObj.Name = "Select All";
            productList.Add(productObj);
            productList.AddRange(iTempP.ToList());
            cboProduct.DataSource = productList;
            cboProduct.DisplayMember = "Name";
            cboProduct.ValueMember = "Id";
            cboProduct.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProduct.AutoCompleteSource = AutoCompleteSource.ListItems;


        }


        private void RemoveProductByLineFromDataGrid(Product pro, DataGridViewRow row)
        {
            try
            {
                if (pro != null)
                {
                    bool Foc = false;
                    if (row.Cells[(int)sCol.colFOC].Value != null && row.Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                    {
                        Foc = true;
                    }
                    string batchNo = row.Cells[(int)sCol.colProductBatchNo].Value.ToString();
                    AvailableProductQtyWithBatch newAvList = availablePList.Where(p => p.ProductID == pro.Id && p.BatchNo == batchNo).FirstOrDefault();
                    if (newAvList != null)
                    {
                        if (Foc)
                        {
                            int count = NormalQtyCount(newAvList.ProductID.ToString(), newAvList.BatchNo);
                            newAvList.AvailableQty = newAvList.OrgQty - count;
                            newAvList.InUseQty = count;
                        }
                        else
                        {
                            int count = FOCQtyCount(newAvList.ProductID.ToString(), newAvList.BatchNo);

                            newAvList.AvailableQty = newAvList.OrgQty - count;
                            newAvList.InUseQty = count;
                        }

                    }
                }
            }
            catch { }
        }

        public class MultiPayment
        {
            public int id;
            public string paymentName;
            public int amount;
        }


        private int CheckProductInDataGridView(string currentProductCode, string BatchNo, bool isFOC)
        {
            try
            {
                //List<int> _indexCount = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                //                         where r.Cells[(int)SalesDataGridViewColumn.colProductCode].Value != null && r.Cells[(int)SalesDataGridViewColumn.colProductCode].Value.ToString() == currentProductCode
                //                         && r.Cells[(int)SalesDataGridViewColumn.colProductBatchNo].Value.ToString() == BatchNo
                //                         && (r.Cells[8].Value.ToString() != null) && (r.Cells[8].Value.ToString() != "FOC")
                //                         select r.Index).ToList();
                //return _indexCount;
                foreach (DataGridViewRow irow in dgvSalesItem.Rows)
                {
                    if (irow.Cells[(int)sCol.colId].Value != null && irow.Cells[(int)sCol.colProductCode].Value.ToString() == currentProductCode && irow.Cells[(int)sCol.colProductBatchNo] != null && irow.Cells[(int)sCol.colProductBatchNo].Value.ToString() == BatchNo)
                    {
                        if (isFOC)
                        {
                            if (irow.Cells[(int)sCol.colFOC].Value != null && irow.Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                            {
                                return irow.Index;
                            }
                        }
                        else
                        {
                            if (irow.Cells[(int)sCol.colFOC].Value == null || string.IsNullOrEmpty(irow.Cells[(int)sCol.colFOC].Value.ToString()))
                            {
                                return irow.Index;
                            }
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                // Utility.ShowErrMessage("CheckProductInDataGridView", ex.Message);
                return -1;
            }
        }

        private void InsertNewProductToGridManural(DataGridViewRow row, bool AddByBarCode, DataGridViewCellEventArgs e)
        {
            Application.DoEvents();
            if (deleteAction)
            {
                return;
            }
            Product pro = null;
            if (AddByBarCode)
            {
                //add product manural by barcode
                string barcode = row.Cells[(int)sCol.colBarCode].Value.ToString();
                pro = iTempP.Where(p => p.Barcode == barcode).FirstOrDefault();

            }
            else
            {
                //add product manural by productcode
                string sproductname = "", sproductcode = "";
                if (dgvSalesItem.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    if (e.ColumnIndex == (int)sCol.colProductName)
                    {
                        sproductname = dgvSalesItem.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    else if (e.ColumnIndex == (int)sCol.colProductCode)
                    {
                        sproductcode = dgvSalesItem.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                }

                if (string.IsNullOrEmpty(sproductname.Trim()) && string.IsNullOrEmpty(sproductcode.Trim())) return;
                pro = iTempP.Where(p => p.ProductCode == sproductcode || p.Name == sproductname).FirstOrDefault();

            }
            if (pro != null)
            {
                List<AvailableProductQtyWithBatch> tempProductControl = availablePList == null ? null : availablePList.Where(Product => Product.ProductID == pro.Id).ToList();
                if (tempProductControl == null || tempProductControl.Count == 0)
                {
                    AddNew4AvailableProductQtyWithBatch(pro.Id, string.Empty);
                }
                tempProductControl = availablePList.Where(Product => Product.ProductID == pro.Id && Product.AvailableQty > 0).OrderBy(p => p.ExpireDate).ToList();
                if (tempProductControl != null && tempProductControl.Count > 0)
                {
                    decimal DisRate = pro.DiscountRate > 0 ? pro.DiscountRate : IsBirthday == true && pro.DiscountRate == 0 ? SettingController.birthday_discount : pro.DiscountRate;

                    UpdateDataGridViewProcess(pro, tempProductControl, e.RowIndex, false, true, 1, DisRate, tempProductControl[0].BatchNo);

                }
                else
                {
                    MessageBox.Show("Product Out of Stock..!");
                    BeginInvoke(new Action(delegate { dgvSalesItem.Rows.Remove(dgvSalesItem.Rows[e.RowIndex]); }));
                    //dgvSalesItem.CurrentCell.Value = string.Empty;
                    //this.Invoke((MethodInvoker)delegate
                    //{
                    //    // dgvSalesItem.Rows.RemoveAt(dgvSalesItem.RowCount - 2);
                    //    dgvSalesItem.Rows.Remove(dgvSalesItem.Rows[e.RowIndex]);
                    //});
                }

            }
            else
            {

                MessageBox.Show("Item not found!", "Cannot find");
                try
                {
                    BeginInvoke(new Action(delegate { dgvSalesItem.Rows.Remove(dgvSalesItem.Rows[e.RowIndex]); }));
                }
                catch { }
            }
        }
        private void dgvSalesItem_CellLeave(object sender, DataGridViewCellEventArgs e) // update done on col indexes - ttn
        {
            try
            {
               
                Cursor.Current = Cursors.WaitCursor;
                AvoidAction();
                if (e.RowIndex >= 0 && e.RowIndex < dgvSalesItem.Rows.Count)
                {
                    //RemoveNoNeedRows();

                    DataGridViewRow row = dgvSalesItem.Rows[e.RowIndex];
                    dgvSalesItem.CommitEdit(new DataGridViewDataErrorContexts());
                    if (e.ColumnIndex == (int)sCol.colBarCode && row.Cells[e.ColumnIndex].Value != null && !string.IsNullOrEmpty(row.Cells[e.ColumnIndex].Value.ToString()) && !string.IsNullOrWhiteSpace(row.Cells[e.ColumnIndex].Value.ToString()) && row.Cells[e.ColumnIndex].ReadOnly == false)
                    {
                        InsertNewProductToGridManural(row, true, e);
                        UpdateTotalCost();
                    }
                    else if ((e.ColumnIndex == (int)sCol.colProductCode || e.ColumnIndex == (int)sCol.colProductName) && row.Cells[e.ColumnIndex].Value != null && !string.IsNullOrEmpty(row.Cells[e.ColumnIndex].Value.ToString()) && !string.IsNullOrWhiteSpace(row.Cells[e.ColumnIndex].Value.ToString()) && row.Cells[e.ColumnIndex].ReadOnly == false)
                    {
                        InsertNewProductToGridManural(row, false, e);
                        UpdateTotalCost();
                    }
                    else if (e.ColumnIndex == (int)sCol.colQty && row.Cells[e.ColumnIndex].ReadOnly == false)
                    {
                        if (row.Cells[(int)sCol.colId].Value == null)
                        {
                            ListenAction();
                            Cursor.Current = Cursors.Default;
                            return;
                        }
                        UpdateQtyUpdateDataView(row);
                        UpdateTotalCost();
                    }
                    //Change Batch No.
                    //else if (e.ColumnIndex == (int)sCol.colProductBatchNo)
                    //{

                    //    if (row.Cells[(int)sCol.colId].Value != null && row.Cells[(int)sCol.colProductBatchNo].Value != null)
                    //    {
                    //        int pid = 0;
                    //        int.TryParse(row.Cells[(int)sCol.colId].Value.ToString(), out pid);
                    //        bool isFOC = false;
                    //        if (row.Cells[(int)sCol.colFOC].Value != null && row.Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                    //        {
                    //            isFOC = true;
                    //        }
                    //        Check_SameProductCode_BatchNo(pid, row.Cells[(int)sCol.colProductBatchNo].Value.ToString(), isFOC);

                    //    }
                    //}
                    else if (e.ColumnIndex == (int)sCol.colDiscountRate && row.Cells[e.ColumnIndex].ReadOnly == false)
                    {
                        UpdateDiscountRate(dgvSalesItem.Rows[e.RowIndex]);
                        UpdateTotalCost();
                    }
                    else
                    {
                        ListenAction();
                        Cursor.Current = Cursors.Default;
                        return;
                    }
                    deleteAction = false;



                    dgvSalesItem.CommitEdit(new DataGridViewDataErrorContexts());

                }

                ListenAction();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                ListenAction();
                Cursor.Current = Cursors.Default;
            }
        }


        private void UpdateDiscountRate(DataGridViewRow row)
        {
            if (row.Cells[1].Value == null) return;
            string currentProductCode = row.Cells[1].Value.ToString();
            Product pro = iTempP.Where(p => p.ProductCode == currentProductCode).FirstOrDefault<Product>();



            int currentQty = 1;
            try
            {
                //get updated qty
                currentQty = Convert.ToInt32(row.Cells[(int)sCol.colQty].Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Input quantity have invalid keywords.");
                row.Cells[(int)sCol.colQty].Value = "1";
            }

            decimal DiscountRate = 0;
            try
            {

                // Decimal.TryParse(row.Cells[5].Value.ToString(), out DiscountRate);
                DiscountRate = Convert.ToDecimal(row.Cells[(int)sCol.colDiscountRate].Value);
                if (DiscountRate > 100)
                {
                    row.Cells[(int)sCol.colDiscountRate].Value = 100;
                    DiscountRate = 100;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Input Discount rate have invalid keywords.");
                row.Cells[(int)sCol.colDiscountRate].Value = "0.00";
            }

            // by SYM
            //update the total cost
            //ZP
            bool IsFOC = false;
            if (row.Cells[(int)sCol.colFOC].Value != null && row.Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
            {
                IsFOC = true;
            }
            row.Cells[(int)sCol.colUnitPrice].Value = Convert.ToInt32(getActualCost(pro, IsFOC));
            row.Cells[(int)sCol.colCost].Value = currentQty * getActualCost(pro, DiscountRate, IsFOC);
        }


        private void dgvSalesItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                AvoidAction();
                if (e.RowIndex >= 0)
                {
                    //DeleteItem
                    if (e.ColumnIndex == (int)sCol.colDelete)
                    {
                        if (dgvSalesItem.Rows[e.RowIndex].Cells[(int)sCol.colId].Value != null && !string.IsNullOrEmpty(dgvSalesItem.Rows[e.RowIndex].Cells[(int)sCol.colId].Value.ToString())) // updated from 9 (TTN)//up khs)
                        {
                            deleteAction = true;
                            object deleteProductCode = dgvSalesItem[1, e.RowIndex].Value;

                            //If product code is null, this is just new role without product. Do not need to delete the row.
                            if (deleteProductCode != null)
                            {
                                DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                if (result.Equals(DialogResult.OK))
                                {
                                    if (dgvSalesItem.Rows.Count != 0)
                                    {
                                        if (dgvSalesItem[(int)sCol.colId, e.RowIndex].Value != null && Convert.ToInt32(dgvSalesItem[11, e.RowIndex].Value) > 0) // product ID
                                        {
                                            int currentProductId = Convert.ToInt32(dgvSalesItem[11, e.RowIndex].Value);
                                            Product pro = iTempP.Where(p => p.Id == currentProductId).FirstOrDefault<Product>();
                                            if (pro.IsConsignment == false)
                                            {
                                                int unitPrice = Convert.ToInt32(dgvSalesItem[5, e.RowIndex].Value); // Updated from 4 to 5 (TTN)
                                                int Qty = Convert.ToInt32(dgvSalesItem[4, e.RowIndex].Value);// Updated from 3 to 4 (TTN)
                                                                                                             //int Tax = Convert.ToInt32(dgvSalesItem[7, e.RowIndex].Value); // Updated from 6 to 7 (TTN)
                                                decimal pricePerProduct = unitPrice * Qty;
                                                //NonConsignProAmt = pricePerProduct + ((pricePerProduct / 100) * pro.Tax.TaxPercent);
                                                NonConsignProAmt = NonConsignProAmt - (pricePerProduct + ((pricePerProduct / 100) * pro.Tax.TaxPercent));


                                            }
                                            RemoveProductByLineFromDataGrid(pro, dgvSalesItem.Rows[e.RowIndex]);


                                        }
                                    }
                                    if (dgvSalesItem.Rows.Count > 0 && dgvSalesItem.Rows[e.RowIndex] != null && dgvSalesItem.Rows.Count > e.RowIndex)
                                    {

                                        BeginInvoke(new Action(() => dgvSalesItem.EndEd‌​it()));
                                        if (dgvSalesItem.Rows.Count == 1)
                                        {
                                            dgvSalesItem.Rows.Add((DataGridViewRow)dgvSalesItem.Rows[e.RowIndex].Clone());
                                        }
                                        //if (e.RowIndex > 0 && e.RowIndex == dgvSalesItem.RowCount - 2)
                                        //{
                                        //    dgvSalesItem.Rows[e.RowIndex - 1].Cells[0].ReadOnly = false;
                                        //    dgvSalesItem.Rows[e.RowIndex - 1].Cells[1].ReadOnly = false;
                                        //    dgvSalesItem.Rows[e.RowIndex - 1].Cells[2].ReadOnly = false;
                                        //}
                                        dgvSalesItem.Rows.RemoveAt(e.RowIndex);

                                    }
                                    UpdateTotalCost();
                                    dgvSalesItem.CurrentCell = dgvSalesItem[0, e.RowIndex];
                                    dgvPaymentType.Rows.Clear();
                                    CalculateChargesAmount();
                                    Cell_ReadOnly();
                                }
                            }
                            else
                            {
                                //if (e.RowIndex > 0 && e.RowIndex == dgvSalesItem.RowCount - 2)
                                //{
                                //    dgvSalesItem.Rows[e.RowIndex - 1].Cells[0].ReadOnly = false;
                                //    dgvSalesItem.Rows[e.RowIndex - 1].Cells[1].ReadOnly = false;
                                //    dgvSalesItem.Rows[e.RowIndex - 1].Cells[2].ReadOnly = false;
                                //}
                                dgvSalesItem.Rows.RemoveAt(e.RowIndex);
                            }
                        }
                        else
                        {
                            //if (e.RowIndex > 0 && e.RowIndex == dgvSalesItem.RowCount - 2)
                            //{
                            //    dgvSalesItem.Rows[e.RowIndex - 1].Cells[0].ReadOnly = false;
                            //    dgvSalesItem.Rows[e.RowIndex - 1].Cells[1].ReadOnly = false;
                            //    dgvSalesItem.Rows[e.RowIndex - 1].Cells[2].ReadOnly = false;
                            //}
                            dgvSalesItem.Rows.RemoveAt(e.RowIndex);
                        }
                    }
                    else if (e.ColumnIndex == 6) // discount price ()
                    {

                    }
                    else if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 4) // 3 ko 4 changed (TTN)
                    {
                        dgvSalesItem.CurrentCell = dgvSalesItem.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        dgvSalesItem.BeginEdit(true);
                    }



                    //    }
                    //}));

                }
                ListenAction();
            }
            catch { ListenAction(); }

        }

        private void dgvSalesItem_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvSalesItem.IsCurrentCellDirty)
            {
                dgvSalesItem.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

        }

        //Added by YMO
        private void txtMemberId_MouseLeave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMemberId.Text))
            {
                if (txtMemberId.Text.IndexOf("member:") == -1)
                {
                    MessageBox.Show("Member Id is invalid. Please scan again.");
                    txtMemberId.Text = string.Empty;
                    IsExportPts = false;
                    return;
                }
                txtMemberId.Text = txtMemberId.Text.Replace("member:", "");

                txtMemberId.Visible = false;
                lblMemberId.Visible = true;
            }
        }

        //Added by YMO
        private void txtMemberId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Back)
            {
                if (!string.IsNullOrEmpty(txtMemberId.Text))
                {
                    if (txtMemberId.Text.IndexOf("member:") == -1)
                    {
                        MessageBox.Show("Member Id is invalid. Please scan again.");
                        txtMemberId.Text = string.Empty;
                        IsExportPts = false;
                        return;
                    }
                    txtMemberId.Text = txtMemberId.Text.Replace("member:", "");

                    txtMemberId.Visible = false;
                    lblMemberId.Visible = true;
                }
            }
        }
    }
}
