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
using System.IO;


namespace POS
{
    public partial class frmReferralCustomer : Form
    {
        #region Initialize
        public frmReferralCustomer()
        {
            InitializeComponent();
        }
        #endregion

        #region variable
        POSEntities entity = new POSEntities();
        private ToolTip tp = new ToolTip();
        public string TransactionId = "";
        #endregion

        #region Event

        private void frmReferralCustomer_Load(object sender, EventArgs e)
        {
            ELC_CustomerPointSystem.Bind_ValidMember(cboCustomer);
            lblReferralPoint.Text = entity.ReferralPrograms.Where(x=>x.IsActive==true).Select(x => x.ReferralPoint).FirstOrDefault().ToString();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
           

            if ((txtMEMID.Focused) && (keyData == (Keys.Enter) || keyData == (Keys.Tab)))
            {
                MemberCard_Leave();

                return true;
            }

            else if ((cboCustomer.Focused) && (keyData == (Keys.Enter) || keyData == (Keys.Tab)))
            {

                Customer_Leave();
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear_Control();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool hasError = false;
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            if (txtMEMID.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtMEMID, "Error");
                tp.Show("Please fill up Member Card!", txtMEMID);
                hasError = true;
            }
            else if (cboCustomer.SelectedIndex == 0)
            {
                tp.SetToolTip(cboCustomer, "Error");
                tp.Show("Please choose Customer Name!", cboCustomer);
                hasError = true;
            }


            if (!hasError)
            {
                ReferralPointInTransaction _referralPoint = new ReferralPointInTransaction();
                _referralPoint.TransactionId = TransactionId;
                _referralPoint.ReferralCustomerId = Convert.ToInt32(cboCustomer.SelectedValue); ;
                

                _referralPoint.ReferralPoint = (entity.ReferralPrograms.Where(x => x.IsActive == true).Select(x => x.ReferralPoint)).FirstOrDefault();
                _referralPoint.CreatedDate = DateTime.Now;
                _referralPoint.CreatedBy = MemberShip.UserId;
                _referralPoint.IsDelete = false;
                _referralPoint.PointHistoryId = entity.Point_History.Where(x => x.Status == "ReferralProgram").OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();
                entity.ReferralPointInTransactions.Add(_referralPoint);
                entity.SaveChanges();
                MessageBox.Show("Successfully Saved!", "Save");
                this.Dispose();
            }
        }


        private void frmReferralCustomer_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtMEMID);
            tp.Hide(cboCustomer);
        
        }

        private void txtMEMID_Leave(object sender, EventArgs e)
        {
            MemberCard_Leave();
        }

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            Customer_Leave();
        }
        #endregion

        #region Function

        private void Clear_Control()
        {
            txtMEMID.Clear();
            lblMemberType.Text = "";
            cboCustomer.SelectedIndex = 0;
        }
        private void MemberCard_Leave()
        {
            if (txtMEMID.Text != string.Empty)
            {
                APP_Data.Customer _cusInfo = ELC_CustomerPointSystem.Get_MemberInfoByMemberCard(txtMEMID.Text);

                if (_cusInfo != null)
                {
                    lblMemberType.Text = _cusInfo.MemberType.Name;
                    cboCustomer.SelectedValue = _cusInfo.Id;
                    btnSave.Focus();
                }
                else
                {
                    tp.RemoveAll();
                    tp.IsBalloon = true;
                    tp.ToolTipIcon = ToolTipIcon.Error;
                    tp.ToolTipTitle = "Error";

                    tp.SetToolTip(txtMEMID, "Error");
                    tp.Show("Member Card is not found!", txtMEMID);

                }
            }
        }

        private void Customer_Leave()
        {
            if (cboCustomer.SelectedIndex > 0)
            {
                int _customerId = 0;
                _customerId = Convert.ToInt16(cboCustomer.SelectedValue);
                APP_Data.Customer _cusInfo = ELC_CustomerPointSystem.Get_MemberInfoByMemberCard("", _customerId);

                if (_cusInfo != null)
                {

                    txtMEMID.Text = _cusInfo.VIPMemberId;
                    lblMemberType.Text = _cusInfo.MemberType.Name;
                    btnSave.Focus();
                }

            }
        }
        #endregion


    }
}
