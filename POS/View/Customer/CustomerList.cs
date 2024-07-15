
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
    public partial class CustomerList : Form
    {
        #region Variables

        private POSEntities entity = new POSEntities();

        static int iBackMonth = 24, defaultCus = 0;
        #endregion

        #region Event
        public CustomerList()
        {
            InitializeComponent();
        }
        // By SYM
        //public void updateCustomerPoint()
        //{
        //foreach (DataGridViewRow row in dgvCustomerList.Rows)
        //{
        //    Customer cs = (Customer)row.DataBoundItem;
        //    row.Cells[5].Value = ELC_CustomerPointSystem.Point_Calculation(cs.Id).ToString();
        //}
        //}
        private void CustomerList_Load(object sender, EventArgs e)
        {
            dgvCustomerList.AutoGenerateColumns = false;
            List<Customer> cu = entity.Customers.Where(x => x.VIPMemberId != string.Empty && x.VipStartedShop == null).ToList();
            if (cu.Count > 0)
            {
                btn_fix.Visible = true;

            }
            else
            {
                btn_fix.Visible = false;
            }
            List<APP_Data.Shop> shoplist = new List<APP_Data.Shop>();
            APP_Data.Shop shopobj = new APP_Data.Shop();
            shopobj.ShortCode = "0";
            shopobj.ShopName = "Select";
            shoplist.Add(shopobj);

            List<APP_Data.Shop> shoplistdis = new List<APP_Data.Shop>();

            shoplist.AddRange(entity.Shops.Where(x => x.ShortCode != "-").GroupBy(x => x.ShortCode).Select(x => x.FirstOrDefault()).ToList());
            createdshop.DataSource = shoplist;
            createdshop.DisplayMember = "ShopName";
            createdshop.ValueMember = "ShortCode";
            Bind_MemberType();
            VisbleByRadio();
            LoadData();

            defaultCus = entity.Customers.Where(x => x.Name.Equals("Default")).FirstOrDefault().Id;
            try
            {
                iBackMonth = SettingController.MemberTypeResetBackMonth;
            }
            catch
            {
                MessageBox.Show("An error has occure while getting some setting value, Please contact administrator for assist.");
                this.Close();
            }
        }

        public void DataBind()
        {
            entity = new POSEntities();
            dgvCustomerList.DataSource = entity.Customers.ToList();
        }

        private void dgvCustomerList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvCustomerList.Rows)
            {
                Customer cs = (Customer)row.DataBoundItem;
                if (cs.MemberTypeID != null)
                {
                    row.Cells[4].Value = cs.MemberType.Name;
                }

            }
        }

        private void dgvCustomerList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //View detail information of customer
                if (e.ColumnIndex == 6)
                {

                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.Customer.ViewDetail || MemberShip.isAdmin)
                    {
                        if (System.Windows.Forms.Application.OpenForms["CustomerDetailInfo"] != null)
                        {
                            CustomerDetailInfo newForm = (CustomerDetailInfo)System.Windows.Forms.Application.OpenForms["CustomerDetailInfo"];
                            int CustomerID = Convert.ToInt32(dgvCustomerList.Rows[e.RowIndex].Cells[0].Value);
                            newForm.customerId = CustomerID;
                            ELC_CustomerPointSystem.Get_ExpiredMemberList_And_Update_ExpiredMember(CustomerID);
                            newForm.ShowDialog();
                        }
                        else
                        {
                            CustomerDetailInfo newForm = new CustomerDetailInfo();
                            int CustomerID = Convert.ToInt32(dgvCustomerList.Rows[e.RowIndex].Cells[0].Value);
                            newForm.customerId = CustomerID;
                            ELC_CustomerPointSystem.Get_ExpiredMemberList_And_Update_ExpiredMember(CustomerID);
                            newForm.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to view detail  customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
                //Edit this User
                else if (e.ColumnIndex == 7)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.Customer.EditOrDelete || MemberShip.isAdmin)
                    {
                        NewCustomer form = new NewCustomer();
                        form.isEdit = true;
                        form.Text = "Edit Customer";
                        form.CustomerId = Convert.ToInt32(dgvCustomerList.Rows[e.RowIndex].Cells[0].Value);
                        form.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to edit customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                //Delete this User
                else if (e.ColumnIndex == 8)
                {
                    //Role Management
                    RoleManagementController controller = new RoleManagementController();
                    controller.Load(MemberShip.UserRoleId);
                    if (controller.Customer.EditOrDelete || MemberShip.isAdmin)
                    {

                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            DataGridViewRow row = dgvCustomerList.Rows[e.RowIndex];
                            Customer cust = (Customer)row.DataBoundItem;
                            cust = (from c in entity.Customers where c.Id == cust.Id select c).FirstOrDefault<Customer>();

                            //Need to recheck
                            if (cust.Transactions.Count > 0)
                            {
                                MessageBox.Show("This customer already made transactions!", "Unable to Delete");
                                return;
                            }
                            else
                            {
                                entity.Customers.Remove(cust);
                                entity.SaveChanges();
                                LoadData();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not allowed to delete customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                //if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                //{
                //    Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                //    newForm.Clear();
                //}
            }
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
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add new customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Function

        private void Bind_MemberType()
        {
            List<APP_Data.MemberType> mTypeList = new List<APP_Data.MemberType>();
            APP_Data.MemberType mType = new APP_Data.MemberType();
            mType.Id = 0;
            mType.Name = "All";
            mTypeList.Add(mType);
            mTypeList.AddRange(entity.MemberTypes.Where(x => x.IsDelete == false).ToList());
            cboMemberType.DataSource = mTypeList;
            cboMemberType.DisplayMember = "Name";
            cboMemberType.ValueMember = "Id";
        }

        public void Get_CustomerList()
        {

        }

        private void LoadData()
        {
            List<Customer> customerList = new List<Customer>();
            string selectshop = createdshop.SelectedValue.ToString();
            if (rdoAllCustomers.Checked == true)
            {
                customerList = entity.Customers.ToList();
            }
            else if (rdoNonMemberCustomers.Checked == true)
            {
                customerList = (from c in entity.Customers.AsEnumerable() where c.MemberTypeID == null select c).ToList();
            }
            else if (rdoMemberCustomers.Checked == true)
            {
                customerList = (from c in entity.Customers.AsEnumerable() where c.MemberTypeID != null && (selectshop == "0" && 1 == 1) || (selectshop != "0" && c.VipStartedShop == selectshop) select c).ToList();

                if (cboMemberType.SelectedIndex != 0)
                {
                    customerList = (from c in customerList where c.MemberTypeID == Convert.ToInt32(cboMemberType.SelectedValue) select c).ToList();
                }

            }

            if (txtSearch.Visible == true)
            {
                if (txtSearch.Text.Trim() != string.Empty)
                {
                    if (rdoMemberCardNo.Checked)
                    {
                        //Search BY Member Card No
                        customerList = customerList.Where(x => x.VIPMemberId == txtSearch.Text.Trim()).ToList();
                    }
                    else if (rdoCustomerName.Checked)
                    {
                        //Search BY Customer Name 
                        customerList = customerList.Where(x => x.Name.Trim().ToLower().Contains(txtSearch.Text.Trim().ToLower())).ToList();
                    }
                }
            }
            else
            {
                if (rdoBirthday.Checked)
                {
                    DateTime fromDate = dtpBirthday.Value.Date;

                    var filterCustomer = (from c in customerList where c.Birthday != null select c).ToList();
                    customerList = (from f in filterCustomer where f.Birthday.Value.Date == fromDate select f).ToList<Customer>();
                }
            }


            dgvCustomerList.DataSource = customerList;
            if (customerList.Count == 0)
            {
                MessageBox.Show("Item not found!", "Cannot find");
            }
        }

        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            dgvCustomerList.DataSource = entity.Customers.ToList();
            txtSearch.Text = "";
            rdoMemberCardNo.Checked = true;
            cboMemberType.Text = "All";
            LoadData();
        }

        private void rdoMemberCardNo_CheckedChanged(object sender, EventArgs e)
        {
            lblSearchTitle.Text = "Member Card No.";
            VisibleControl(true, false);
        }

        private void rdoCustomerName_CheckedChanged(object sender, EventArgs e)
        {
            lblSearchTitle.Text = "Customer Name";
            VisibleControl(true, false);
        }

        private void rdoBirthday_CheckedChanged(object sender, EventArgs e)
        {
            lblSearchTitle.Text = "Birthday";
            VisibleControl(false, true);
        }

        private void VisibleControl(bool t, bool f)
        {
            txtSearch.Visible = t;
            dtpBirthday.Visible = f;
        }

        private void VisbleByRadio()
        {
            if (rdoMemberCardNo.Checked == true || rdoCustomerName.Checked == true)
            {
                VisibleControl(true, false);
            }
            else
            {
                VisibleControl(false, true);
            }
        }
        // By SYM
        private void rdoAllCustomers_CheckedChanged(object sender, EventArgs e)
        {
            groupvipstarted.Visible = false;
            gbMemberType.Visible = false;
            LoadData();
        }
        // By SYM
        private void rdoNonMemberCustomers_CheckedChanged(object sender, EventArgs e)
        {
            groupvipstarted.Visible = false;
            gbMemberType.Visible = false;
            LoadData();
        }
        // By SYM
        private void rdoMemberCustomers_CheckedChanged(object sender, EventArgs e)
        {
            groupvipstarted.Visible = true;
            gbMemberType.Visible = true;
            Bind_MemberType();
            LoadData();
        }
        // By SYM
        private void cboMemberType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMemberType.SelectedIndex != 0)
            {
                LoadData();
            }
        }

        private void btn_fix_Click(object sender, EventArgs e)
        {
            List<Customer> cu = entity.Customers.Where(x => x.VIPMemberId != "" && x.VipStartedShop == null).ToList();
            refix_Data(cu);
        }
        private void refix_Data(List<Customer> cu)
        {
            foreach (Customer c in cu)
            {

                c.VipStartedShop = c.CustomerCode.Substring(2, 2).ToString();
                entity.Entry(c).State = EntityState.Modified;

            }
            entity.SaveChanges();
        }

        private void createdshop_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnRevoke_Click(object sender, EventArgs e)
        {
            RevokeFromCustomersList();
        }
        public void RevokeFromCustomersList()
        {
            try
            {

                if (iBackMonth < 1)
                {
                    MessageBox.Show("Require parameter is not provided(Duration)!");
                    return;
                }

                DateTime dtDate = DateTime.Now.AddMonths(0 - iBackMonth);

                DialogResult res = MessageBox.Show("From " + dtDate.ToString("dd-MMMM-yyyy") + ", VIP will be revoked to Non-VIP for those who don't have a transaction.", "Are you sure want to revoke?", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Application.UseWaitCursor = true;
                    btnRevoke.Enabled = false;

                    DateTime dtToday = DateTime.Now.Date;
                    IQueryable<POS.APP_Data.Transaction> iqTrans = entity.Transactions.Where(x => x.IsDeleted == false && x.IsComplete == true && x.DateTime >= dtDate && x.CustomerId != defaultCus && (x.Type == "Sale" || x.Type == "Special Member"));
                    IQueryable<POS.APP_Data.Customer> iqc = entity.Customers.Where(x => x.Name != "Default" && x.MemberTypeID != null && x.PromoteDate != null && EntityFunctions.TruncateTime(x.PromoteDate) != dtToday && (x.LatestRevokeDate == null || x.LatestRevokeDate < dtToday));
                    IQueryable<POS.APP_Data.Transaction> iqTrans_0_2999 = iqTrans.Where(x => x.CustomerId < 3000);
                    IQueryable<POS.APP_Data.Transaction> iqTrans_3000_5999 = iqTrans.Where(x => x.CustomerId >= 3000 && x.CustomerId < 6000);
                    IQueryable<POS.APP_Data.Transaction> iqTrans_6000_8999 = iqTrans.Where(x => x.CustomerId >= 6000 && x.CustomerId < 9000);
                    IQueryable<POS.APP_Data.Transaction> iqTrans_Over_9000 = iqTrans.Where(x => x.CustomerId >= 9000);
                    //IQueryable<POS.APP_Data.Transaction> iqTrans_Over_15000 = iqTrans.Where(x => x.CustomerId >= 15000);
                    foreach (Customer c in iqc.ToList())
                    {
                        if (c != null && c.MemberTypeID != null && (c.LatestRevokeDate == null || c.LatestRevokeDate < DateTime.Now.Date))
                        {
                            //if(c.Id>=15000)
                            //{
                            //    var iqTrans4Each = iqTrans_Over_15000.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                            //    if (iqTrans4Each == null)
                            //    {
                            //        Utility.InsertRevokeHistoryData(c, nonVIPId, txtNote.Text);
                            //    }
                            //}
                            if (c.Id >= 9000)
                            {
                                var iqTrans4Each = iqTrans_Over_9000.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                                if (iqTrans4Each == null)
                                {
                                    Utility.InsertRevokeHistoryData(c, "Revoked from customer list control.");
                                }
                            }
                            else if (c.Id >= 6000)
                            {
                                var iqTrans4Each = iqTrans_6000_8999.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                                if (iqTrans4Each == null)
                                {
                                    Utility.InsertRevokeHistoryData(c, "Revoked from customer list control.");
                                }
                            }
                            else if (c.Id >= 3000)
                            {
                                var iqTrans4Each = iqTrans_3000_5999.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                                if (iqTrans4Each == null)
                                {
                                    Utility.InsertRevokeHistoryData(c, "Revoked from customer list control.");
                                }
                            }
                            else
                            {
                                var iqTrans4Each = iqTrans_0_2999.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                                if (iqTrans4Each == null)
                                {
                                    Utility.InsertRevokeHistoryData(c, "Revoked from customer list control.");
                                }
                            }

                        }

                    }
                    Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                    if (newForm != null)
                    {
                        newForm.ReloadCustomerList();
                    }
                    MessageBox.Show("Revoke process successfully done.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException, "Revoke process failed with an error!");

            }
            finally
            {
                btnRevoke.Enabled = true;
                Cursor.Current = Cursors.Default;
                Application.UseWaitCursor = false;
            }
        }
    }
}
