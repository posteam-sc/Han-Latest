using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;

namespace POS
{
    public partial class GiftCardControl : Form
    {
        #region Variables

        POSEntities posEntity = new POSEntities();

        private ToolTip tp = new ToolTip();
        public Boolean IsStart = false;
        public int CurrentCustomerId = 0;
        private int CustomerId = 0;
        #endregion

        #region Event

      
        private void GiftCardControl_Load(object sender, EventArgs e)
        {
            Customer_Bind();
            SearchCustomer_Bind();
       
            
            IsStart = true;
            if (CurrentCustomerId != 0)
            {
                SetCurrentCustomer(CurrentCustomerId);
            }
            dgvGiftCardList.AutoGenerateColumns = false;
            DataBind();
        }

        private void GiftCardControl_Activated(object sender, EventArgs e)
        {
            DataBind();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
             //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.GiftCard.Add || MemberShip.isAdmin)
            {

                Boolean hasError = false;
                tp.RemoveAll();
                tp.IsBalloon = true;
                tp.ToolTipIcon = ToolTipIcon.Error;
                tp.ToolTipTitle = "Error";

                if (cboCustomer.SelectedIndex == 0)
                {
                    tp.SetToolTip(cboCustomer, "Error");
                    tp.Show("Please choose the customer!", cboCustomer);
                    return;
                }

                if (txtCardNumber.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(txtCardNumber, "Error");
                    tp.Show("Please fill up gift card number!", txtCardNumber);
                    return;
                }

               

                if (rdo5000.Checked == false && rdo6000.Checked == false && rdo7000.Checked == false && rdo18000.Checked == false && rdo21000.Checked == false && rdo28000.Checked == false && rdo30000.Checked == false && rdo35000.Checked == false && rdo50000.Checked == false && rdo70000.Checked == false)
                {
                    MessageBox.Show("Please choose Amount!");
                    hasError = true;
                    return;
                }

                if (!hasError)
                {

                    //Check if giftcard is already register
                    GiftCard giftCardObj2 = (from gC in posEntity.GiftCards where gC.CardNumber == txtCardNumber.Text && gC.IsDelete == false select gC).FirstOrDefault();

                    //Check if giftcard is already register.
                    if (giftCardObj2 == null)
                    {
                        AddGiftCard();
                    }
                    //Card already register
                    else //if (giftCardObj2 != null)
                    {

                        //    //if (giftCardObj2.IsUsed == true && giftCardObj2.GiftCardInTransactions.Count > 0)
                        //    //{
                        //    //    Boolean IsAvailable = false;
                        //    //    List<Transaction> tList = new List<Transaction>();
                        //    //    foreach (GiftCardInTransaction g in giftCardObj2.GiftCardInTransactions)
                        //    //    {
                        //    //        Transaction t = posEntity.Transactions.Where(x => x.Id == g.TransactionId).FirstOrDefault();
                        //    //        tList.Add(t);
                        //    //    }
                        //    //    foreach (Transaction ts in tList)
                        //    //    {
                        //    //        if (ts.IsDeleted==false) IsAvailable = true;
                        //    //    }

                        //    //    if (IsAvailable)
                        //    //    {
                        //    //        AddGiftCard();
                        //    //    }
                        //    //    else
                        //    //    {
                        //    //        tp.SetToolTip(txtCardNumber, "Error");
                        //    //        tp.Show("This card number is already exist!", txtCardNumber);
                        //    //    }

                        //    
                        //else
                        //    {
                        //        tp.SetToolTip(txtCardNumber, "Error");
                        //        tp.Show("This card number is already exist!", txtCardNumber);
                        //    }

                        tp.SetToolTip(txtCardNumber, "Error");
                        tp.Show("This card number is already exist!", txtCardNumber);
                        
                    }


                }
                else
                {
                    MessageBox.Show("You are not allowed to add new giftcard", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void dgvGiftCardList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
             
            if (e.RowIndex >= 0)
            {
                int currentGiftCardId = Convert.ToInt32(dgvGiftCardList.Rows[e.RowIndex].Cells[0].Value);
                int currentGiftCardAmount = Convert.ToInt32(dgvGiftCardList.Rows[e.RowIndex].Cells[3].Value);

                //Top Up
                if (e.ColumnIndex == 4)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.GiftCard.EditOrDelete || MemberShip.isAdmin)
                    {
                        TopUp newform = new TopUp();
                        newform.GiftCardId = currentGiftCardId;
                        newform.Show();
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit giftcards", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);      
                    }
                }
                //Delete
                else if (e.ColumnIndex == 5)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.GiftCard.EditOrDelete || MemberShip.isAdmin)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete? This card has " + currentGiftCardAmount, "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            DataGridViewRow row = dgvGiftCardList.Rows[e.RowIndex];
                            GiftCard giftCardObj = (GiftCard)row.DataBoundItem;

                            bool IsAllowDelete = false;
                            var giftCardInTransaction = (from gt in posEntity.Transactions where gt.GiftCardId == giftCardObj.Id select gt).FirstOrDefault();

                            if (giftCardInTransaction != null)
                            {
                                if (giftCardInTransaction.IsDeleted == true)
                                {
                                    IsAllowDelete = true;
                                }
                                else
                                {
                                    IsAllowDelete = false;
                                }

                            }
                            else
                            {
                                IsAllowDelete = true;
                            }

                          //  if (giftCardObj.Transactions.Count == 0)
                              if(IsAllowDelete == true)
                           
                            {

                               // posEntity.GiftCards.Remove(giftCardObj);
                                giftCardObj.IsDelete = true;
                                posEntity.Entry(giftCardObj).State = EntityState.Modified;
                                posEntity.SaveChanges();
                                DataBind();
                                MessageBox.Show("Successfully Deleted!", "Delete Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else 
                            {
                                
                                MessageBox.Show("The Card is already used in transaction", "Unable to Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete giftcards", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);      
                    }
                }
                //View Detail
                else if (e.ColumnIndex == 6)
                {
                    int gridGiftCardId = Convert.ToInt32(dgvGiftCardList.Rows[e.RowIndex].Cells[0].Value.ToString());
                   
                        GiftCardTransactionHistory newForm = new GiftCardTransactionHistory();
                        newForm.GiftCardId = gridGiftCardId;
                        newForm.ShowDialog();
                   
                }
            }
        }

        private void GiftCardControl_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtCardNumber);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearchCardNo.Clear();
            DataBind();
        }
        // By SYM
        private void btnNewCustomer_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Customer.Add || MemberShip.isAdmin)
            {

                NewCustomer form = new NewCustomer();
                form.isEdit = false;
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add new customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        // By SYM
        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCustomer.SelectedIndex != 0)
            {
                int customerId = Convert.ToInt32(cboCustomer.SelectedValue);
                SetCurrentCustomer(customerId);
            }
            else
            {
                CustomerId = 0;
                lblCustomerName.Text = "-";
                lblNRIC.Text = "-";
                lblPhoneNo.Text = "-";
            }
            DataBind();
        }
        // By SYM
        private void dgvGiftCardList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvGiftCardList.Rows)
            {
                GiftCard _giftCard = (GiftCard)row.DataBoundItem;
                row.Cells[1].Value = _giftCard.Customer.Name;
            }
        }

        #endregion

        #region Function 

        
        public GiftCardControl()
        {
            InitializeComponent();
        }
        // By SYM
        public void SetCurrentCustomer(Int32 CId)
        {
            if (IsStart == true)
            {
                CustomerId = CId;
                Customer currentCustomer = posEntity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();
                lblCustomersName.Text = currentCustomer.Name;
                lblNRIC.Text = currentCustomer.NRC;
                lblPhoneNo.Text = currentCustomer.PhoneNumber;
                // lblEmail.Text = currentCustomer.Email;
                lblMember.Text = Convert.ToString(currentCustomer.VIPMemberId);
                cboCustomer.SelectedItem = currentCustomer;
            }
        }


        // By SYM
        private void DataBind()
        {
            if (IsStart == true)
            {
                int customerId = Convert.ToInt32(cboSearchCustomer.SelectedValue);
                List<GiftCard> GiftCardList = posEntity.GiftCards.Where(x => x.IsUsed != true && ((customerId == 0 && 1 == 1) || (customerId > 0 && x.CustomerId == customerId)) && (x.IsDelete == null || x.IsDelete == false)).ToList();
                if (rdoFilter5000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 5000).ToList();
                }
                else if (rdoFilter6000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 6000).ToList();
                }
                else if (rdoFilter7000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 7000).ToList();
                }
                else if (rdoFilter18000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 18000).ToList();
                }
                else if (rdoFilter21000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 21000).ToList();
                }
                else if (rdoFilter28000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 28000).ToList();
                }
                else if (rdoFilter30000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 30000).ToList();
                }
                else if (rdoFilter35000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 35000).ToList();
                }
                else if (rdoFilter50000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 50000).ToList();
                }
                else if (rdoFilter70000.Checked)
                {
                    GiftCardList = GiftCardList.Where(x => x.Amount == 70000).ToList();
                }
                else
                {
                    GiftCardList = GiftCardList.ToList();
                }
               
                dgvGiftCardList.AutoGenerateColumns = false;
                dgvGiftCardList.DataSource = GiftCardList;
              
            }
        }
        // By SYM
        private void AddGiftCard()
        {
            
            GiftCard giftCardObj1 = new GiftCard();
            giftCardObj1.CardNumber = txtCardNumber.Text;
            giftCardObj1.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue);
            if (rdo5000.Checked)
            {
                giftCardObj1.Amount = 5000;
            }
            else if (rdo6000.Checked)
            {
                giftCardObj1.Amount = 6000;
            }
            else if (rdo7000.Checked)
            {
                giftCardObj1.Amount = 7000;
            }
            else if (rdo18000.Checked)
            {
                giftCardObj1.Amount = 18000;
            }
            else if (rdo21000.Checked)
            {
                giftCardObj1.Amount = 21000;
            }
            else if (rdo28000.Checked)
            {
                giftCardObj1.Amount = 28000;
            }
            else if (rdo30000.Checked)
            {
                giftCardObj1.Amount = 30000;
            }
            else if (rdo35000.Checked)
            {
                giftCardObj1.Amount = 35000;
            }
            else if (rdo50000.Checked)
            {
                giftCardObj1.Amount = 50000;
            }
            else
            {
                giftCardObj1.Amount = 70000;
            }

            //giftCardObj1.ExpireDate = DateTime.Now.AddMonths(1);
            //giftCardObj1.IsUsedDate = DateTime.Now;
            giftCardObj1.IsDelete = false;
            giftCardObj1.IsUsed = false;
            posEntity.GiftCards.Add(giftCardObj1);
            posEntity.SaveChanges();

            if (System.Windows.Forms.Application.OpenForms["CustomerDetailInfo"] != null)
            {
                CustomerDetailInfo newForm = (CustomerDetailInfo)System.Windows.Forms.Application.OpenForms["CustomerDetailInfo"];
                newForm.updateAvailableGiftCards();
            }

            //Update Customer Point in other forms
            if (System.Windows.Forms.Application.OpenForms["CustomerDetailInfo"] != null)
            {
                CustomerDetailInfo newForm = (CustomerDetailInfo)System.Windows.Forms.Application.OpenForms["CustomerDetailInfo"];
                newForm.updateCustomerPoint();
            }


            //dgvBrandList.DataSource = (from b in posEntity.Brands orderby b.Id descending select b).ToList();
            MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DataBind();
            txtCardNumber.Text = "";
        }
        // By SYM
        public void Customer_Bind()
        {
            //Add Customer List with default option
            List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
            APP_Data.Customer customer = new APP_Data.Customer();
            customer.Id = 0;
            customer.Name = "-Choose One-";
            customerList.Add(customer);
            customerList.AddRange(posEntity.Customers.Where(x=>x.Name.Trim()!="Default" ).OrderBy(x=>x.Name).ToList());
            var count = customerList.Count();
            cboCustomer.DataSource = customerList;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";
           // cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
        }
        // By SYM
        public void SearchCustomer_Bind()
        {
            //Add Customer List with default option
            List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
            APP_Data.Customer customer = new APP_Data.Customer();
            customer.Id = 0;
            customer.Name = "ALL";
            customerList.Add(customer);
            customerList.AddRange(posEntity.Customers.ToList());

            cboSearchCustomer.DataSource = customerList;
            cboSearchCustomer.DisplayMember = "Name";
            cboSearchCustomer.ValueMember = "Id";
            cboSearchCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboSearchCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
        }
        // By SYM
        private void cboSearchCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }
        // By SYM
        private void rdoAll_CheckedChanged_1(object sender, EventArgs e)
        {
            DataBind();
        }
        // By SYM
        private void rdoFilter7000_CheckedChanged_1(object sender, EventArgs e)
        {
            DataBind();
        }

        private void cboCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            cboCustomer.DroppedDown = true;
        }

        private void cboCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            cboCustomer.DroppedDown = true;
        }

        private void rdoFilter5000_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter30000_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter50000_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter18000_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter21000_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter28000_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter35000_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter70000_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        private void rdoFilter6000_CheckedChanged(object sender, EventArgs e)
        {
            DataBind();
        }
        #endregion
    }
}
