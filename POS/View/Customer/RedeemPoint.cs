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
    public partial class RedeemPoint : Form
    {
        // By SYM
        #region Variables

        POSEntities entity = new POSEntities();
        public int customerId;
        private int redeemAmount = 7000;
        private int redeemPoint = 10;

        #endregion

        public RedeemPoint()
        {
            InitializeComponent();
        }

        private void RedeemPoint_Load(object sender, EventArgs e)
        {
            cboRedeemPointAmount.SelectedIndex = 0;
            Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();
            lblCustomerName.Text = cust.Title + " " + cust.Name;
            lblTotalPoint.Text = ELC_CustomerPointSystem.Point_Calculation(cust.Id).ToString();
        }

        private void btnRedeem_Click(object sender, EventArgs e)
        {
            decimal currentPoint = Convert.ToDecimal(lblTotalPoint.Text);
            if (currentPoint >= redeemPoint)
            {
                RedeemPoint_History redeemPointHistory = new RedeemPoint_History();
                redeemPointHistory.CustomerId = customerId;
                redeemPointHistory.CounterId = MemberShip.CounterId;
                redeemPointHistory.CasherId = MemberShip.UserId;
                redeemPointHistory.DateTime = DateTime.Now;
                redeemPointHistory.RedeemAmount = redeemAmount;
                redeemPointHistory.RedeemPoint = redeemPoint;
                entity.RedeemPoint_History.Add(redeemPointHistory);
                entity.SaveChanges();
                //lblTotalPoint.Text = ELC_CustomerPointSystem.GetPointFromCustomerId(customerId).ToString();
                decimal _point = ELC_CustomerPointSystem.Point_Calculation(customerId);
                lblTotalPoint.Text = _point.ToString();
           

                MessageBox.Show("Redeem process completed!");
               
                if (DialogResult.Yes == MessageBox.Show("Do you want to register the giftcard now?", "Giftcards Register", MessageBoxButtons.YesNo))
                {
                    GiftCardControl _giftCardControl = new GiftCardControl();
                    _giftCardControl.CurrentCustomerId = customerId;
                 
                    _giftCardControl.ShowDialog();
                }

                if (System.Windows.Forms.Application.OpenForms["CustomerDetailInfo"] != null)
                {
                    CustomerDetailInfo newForm = (CustomerDetailInfo)System.Windows.Forms.Application.OpenForms["CustomerDetailInfo"];
                    newForm.updateCustomerPoint();
                }
               
            }
            else
            {
                MessageBox.Show("Customer does not have enough point to redeem.");
            }
        }

        private void cboRedeemPointAmount_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboRedeemPointAmount.SelectedIndex)
            {
                case 0: redeemAmount = 7000; redeemPoint = 10; break;
                case 1: redeemAmount = 14000; redeemPoint = 20; break;
                case 2: redeemAmount = 21000; redeemPoint = 30; break;
                case 3: redeemAmount = 28000; redeemPoint = 40; break;
                case 4: redeemAmount = 35000; redeemPoint = 50; break;
                case 5: redeemAmount = 42000; redeemPoint = 60; break;
                case 6: redeemAmount = 49000; redeemPoint = 70; break;
                case 7: redeemAmount = 56000; redeemPoint = 80; break;
                case 8: redeemAmount = 63000; redeemPoint = 90; break;
                case 9: redeemAmount = 70000; redeemPoint = 100; break;
            }
            lblGiftAmount.Text = String.Format("{0:#,##0}", redeemAmount);
            lblTotalGiftCard.Text = (redeemAmount / 7000).ToString();
        }
    }
}
