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
using POS.View.Setting;

namespace POS
{
    public partial class CouponCode : Form
    {
        #region Variables

        POSEntities posEntity = new POSEntities();
        private bool isEdit = false;
        private ToolTip tp = new ToolTip();
        int currentId;
        int currentCouponCodeId = 0;

        #endregion

        #region Event

        public CouponCode()
        {
            InitializeComponent();
        }

        private void CouponCode_Load(object sender, EventArgs e)
        {
            bool notbackoffice = Utility.IsNotBackOffice();
            if (notbackoffice)
            {
                Utility.Gpvisible(groupBox1, isEdit);
                dgvCouponCodeList.Columns[4].Visible = false;
            }
            else
            {
                dgvCouponCodeList.Columns[4].Visible = true;
            }
            dgvCouponCodeList.AutoGenerateColumns = false;
            //dgvCouponCodeList.DataSource = posEntity.CouponCodes.Where(x => x.IsDelete == false).ToList();
            dgvCouponCodeList.DataSource = posEntity.CouponCodes.Where(x => x.IsDelete == false).OrderBy(x => x.CouponCodeNo).ToList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation
            if (txtCouponCodeNo.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtCouponCodeNo, "Error");
                tp.Show("Please fill up coupon code number!", txtCouponCodeNo);
                hasError = true;
            }

            if (txtAmount.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtAmount, "Error");
                tp.Show("Please fill up amount!", txtAmount);
                hasError = true;
            }

            if (!hasError)
            {
                if (lblStatus.Text == "Add")
                {
                    APP_Data.CouponCode couponcodeObj = new APP_Data.CouponCode();
                    APP_Data.CouponCode couponcodeObj2 = (from t in posEntity.CouponCodes where t.CouponCodeNo == txtCouponCodeNo.Text && t.IsDelete == false select t).FirstOrDefault();
                    if (couponcodeObj2 == null)
                    {
                        couponcodeObj.CouponCodeNo = txtCouponCodeNo.Text;
                        couponcodeObj.Amount = Convert.ToDecimal(txtAmount.Text);
                        couponcodeObj.IsDelete = false;
                        couponcodeObj.CreatedBy =MemberShip.UserId;
                        couponcodeObj.CreatedDate = DateTime.Now;
                        couponcodeObj.UpdatedBy = MemberShip.UserId;
                        couponcodeObj.UpdatedDate = DateTime.Now;
                        posEntity.CouponCodes.Add(couponcodeObj);
                        posEntity.SaveChanges();
                        MessageBox.Show("Successfully Saved!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);


                        currentCouponCodeId = couponcodeObj.Id;
                        dgvCouponCodeList.DataSource = "";
                        dgvCouponCodeList.DataSource = posEntity.CouponCodes.Where(x => x.IsDelete == false).ToList();
                        Clear();
                    }
                    else
                    {
                        tp.SetToolTip(txtCouponCodeNo, "Error");
                        tp.Show("This coupon code number is already exist!", txtCouponCodeNo);
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to update?", "Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        int count;
                        APP_Data.CouponCode couponcodeObj = (from t in posEntity.CouponCodes where t.Id == currentId select t).FirstOrDefault();
                        count = (from t in posEntity.CouponCodes where t.Id != currentId && t.CouponCodeNo == txtCouponCodeNo.Text select t).ToList().Count;
                        if (count == 0)
                        {

                            couponcodeObj.CouponCodeNo = txtCouponCodeNo.Text;
                            couponcodeObj.Amount = Convert.ToDecimal(txtAmount.Text);
                            couponcodeObj.UpdatedBy = MemberShip.UserId;
                            couponcodeObj.UpdatedDate = DateTime.Now;
                            posEntity.Entry(couponcodeObj).State = EntityState.Modified;
                            MessageBox.Show("Successfully Update!", "Update Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            currentCouponCodeId = couponcodeObj.Id;
                            dgvCouponCodeList.DataSource = "";
                            dgvCouponCodeList.DataSource = posEntity.CouponCodes.Where(x => x.IsDelete == false).ToList();
                            posEntity.SaveChanges();
                            Clear();
                            bool notbackoffice = Utility.IsNotBackOffice();
                            if (notbackoffice)
                            {
                                Utility.Gpvisible(groupBox1, false);
                            }
                        }
                        else
                        {
                            tp.SetToolTip(txtCouponCodeNo, "Error");
                            tp.Show("This coupon code number is already exist!", txtCouponCodeNo);
                        }

                    }
                    else
                    {
                        Clear();

                    }

                }
            }
        }

        private void dgvCouponCodeList_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                //Role Management
                RoleManagementController controller = new RoleManagementController();
                controller.Load(MemberShip.UserRoleId);
                if (controller.TaxRate.EditOrDelete || MemberShip.isAdmin)
                {
                    //to edit
                    if (e.ColumnIndex == 3)
                    {
                        bool notbackoffice = Utility.IsNotBackOffice();
                        isEdit = true;
                        DataGridViewRow row = dgvCouponCodeList.Rows[e.RowIndex];
                        currentId = Convert.ToInt32(row.Cells[0].Value);

                        //string cId = currentId.ToString();Update by YMO
                        int count = (from t in posEntity.Transactions where t.CouponCodeId == currentId && t.IsDeleted == false select t).ToList().Count;

                        if (count > 0)
                        {
                            //To show message box 
                            MessageBox.Show("This coupon code is currently in use!", "Enable to edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            APP_Data.CouponCode couponcodeObj1 = (from t in posEntity.CouponCodes where t.Id == currentId select t).FirstOrDefault();
                            txtCouponCodeNo.Text = couponcodeObj1.CouponCodeNo;
                            txtAmount.Text = couponcodeObj1.Amount.ToString();
                            lblStatus.Text = "UPDATE";
                            this.Text = "Edit Coupon Code";
                            groupBox1.Text = "Edit Coupon Code";
                            btnAdd.Image = Properties.Resources.update_small;
                            btnCancel.Visible = true;

                            if (notbackoffice)
                            {
                                Utility.Gpvisible(groupBox1, isEdit);
                            }
                        }                        
                    }
                    if (e.ColumnIndex == 4)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            Clear();
                            DataGridViewRow row = dgvCouponCodeList.Rows[e.RowIndex];
                            currentId = Convert.ToInt32(row.Cells[0].Value);
                            //string cId = currentId.ToString();
                            APP_Data.CouponCode couponcodeObj1 = (from t in posEntity.CouponCodes where t.Id == currentId select t).FirstOrDefault();
                            int count = (from t in posEntity.Transactions where t.CouponCodeId == currentId && t.IsDeleted==false select t).ToList().Count;

                            if (count > 0 )
                            {
                                //To show message box 
                                MessageBox.Show("This coupon code is currently in use!", "Enable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                dgvCouponCodeList.DataSource = "";
                                couponcodeObj1.IsDelete = true;
                                posEntity.SaveChanges();
                                dgvCouponCodeList.DataSource = posEntity.CouponCodes.Where(x => x.IsDelete == false).ToList();
                                currentCouponCodeId = 0;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You are not allowed to edit/delete coupon code.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void CouponCode_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtCouponCodeNo);
            tp.Hide(txtAmount);
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                 (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Function

        private void Clear()
        {
            txtCouponCodeNo.Text = "";
            txtAmount.Text = "";
            btnAdd.Text = "";

            lblStatus.Text = "Add";
            this.Text = "Add New Coupon Code";
            btnAdd.Image = Properties.Resources.add_small;
            btnCancel.Visible = false;
            bool notbackoffice = Utility.IsNotBackOffice();
            if (notbackoffice)
            {
                Utility.Gpvisible(groupBox1, false);
            }
        }
        #endregion
    }
}
