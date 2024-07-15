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
using System.Data.Objects;

namespace POS
{
    public partial class CustomerDetailInfo : Form
    {
        #region Variables
        POSEntities entity = new POSEntities();
        public int customerId;
        Customer cust = new Customer();
        int oldReedemPoint = 0;  
        int currentRedeemPoint = 0;
        #endregion

        #region Function

        public void updateCustomerPoint()
        {
            //Loyalty Program
            entity = new POSEntities();

            List < DateTime > expiredDateList= ELC_CustomerPointSystem.Get_PointExipredDate(true);
            DateTime ExpiredDate1 = expiredDateList[0];
            DateTime ExpiredDate2 = expiredDateList[1];
            lblTotalPoints.Text = ELC_CustomerPointSystem.Point_Calculation(customerId).ToString();
            lblReferralPoint.Text = ELC_CustomerPointSystem.Point_Calculation(customerId, true, true).ToString();
            //lblCurrentRedeemPoint.Text = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>().
            //    RedeemPoint_History.Where(y => EntityFunctions.TruncateTime(y => y.DateTime.Date) >= ExpiredDate1).Sum(x => x.RedeemPoint).ToString();


            //int _totalRedeemPoint = (from c in entity.Customers
            //            join rd in entity.RedeemPoint_History on c.Id equals rd.CustomerId
            //            where c.Id == customerId
            //            && EntityFunctions.TruncateTime(rd.DateTime) >= ExpiredDate1.Date && EntityFunctions.TruncateTime(rd.DateTime) <= ExpiredDate2.Date
            //            select rd.RedeemPoint).Sum();

            int _totalRedeemPoint = 0;
            if ((from c in entity.Customers join rd in entity.RedeemPoint_History on c.Id equals rd.CustomerId where c.Id == customerId select rd).FirstOrDefault() != null)
            {
                var _totalRedeem = (from c in entity.Customers
                                     join rd in entity.RedeemPoint_History on c.Id equals rd.CustomerId
                                     where c.Id == customerId
                                     && EntityFunctions.TruncateTime(rd.DateTime) >= ExpiredDate1.Date && EntityFunctions.TruncateTime(rd.DateTime) <= ExpiredDate2.Date
                                     select rd.RedeemPoint);
                if(_totalRedeem != null && _totalRedeem.Count()>0)
                {
                    _totalRedeemPoint = _totalRedeem.Sum();
                }
            }

            lblCurrentRedeemPoint.Text = _totalRedeemPoint.ToString();

            Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();
            dgvRedeemHistory.DataSource = cust.RedeemPoint_History.OrderByDescending(x => x.DateTime).ToList();
        }

        public void updateAvailableGiftCards()
        {
            entity = new POSEntities();
            Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();
            dgvGiftCards.DataSource = cust.GiftCards.Where(x => x.IsUsed == false).ToList();

        }

        #endregion                                

        #region Event

        public CustomerDetailInfo()
        {
            InitializeComponent();
        }        

        private void CustomerDetailInfo_Load(object sender, EventArgs e)
        {
            cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();

            lblName.Text = cust.Title + " " + cust.Name;

            lblMCId.Text = cust.VIPMemberId != "" ? cust.VIPMemberId : "-";           

            lblMType.Text = (from m in entity.MemberTypes where m.Id == cust.MemberTypeID select m.Name).FirstOrDefault();
            //if (cust.ExpireDate < DateTime.Today.Date)
            //{
                //lblMType.Text = lblMType.Text ;
            //}        
            if (lblMType.Text == null || lblMType.Text == "")
            {
                lblMType.Text = "-";
            }
            lblPromoteDate.Text = cust.PromoteDate != null ? Convert.ToDateTime(cust.PromoteDate).ToString("dd-MM-yyyy") : "-";
            lblStartDate.Text = cust.StartDate != null ? Convert.ToDateTime(cust.StartDate).ToString("dd-MM-yyyy") : "-";
            lblExpiredDate.Text = cust.ExpireDate != null ? Convert.ToDateTime(cust.ExpireDate).ToString("dd-MM-yyyy") : "-";

            lblPhoneNumber.Text = cust.PhoneNumber != "" ? cust.PhoneNumber : "-";

            lblNrc.Text = cust.NRC != "" ? cust.NRC : "-";

            lblAddress.Text = cust.Address != "" ? cust.Address : "-";

            lblEmail.Text = cust.Email != "" ? cust.Email : "-";

            lblGender.Text = cust.Gender != "" ? cust.Gender : "-";

            lblBirthday.Text = cust.Birthday != null ? Convert.ToDateTime(cust.Birthday).ToString("dd-MM-yyyy") : "-";
            lblCity.Text = cust.City != null ? cust.City.CityName : "-";

            // By SYM // calculate point

            lblTotalPoints.Text = ELC_CustomerPointSystem.Point_Calculation(customerId).ToString();
            lblReferralPoint.Text = ELC_CustomerPointSystem.Point_Calculation(customerId, true, true).ToString();
            //Calculate current redeem point according to duration(eg. from 1-Jan-2017  to 30-Jun-2017)--sym
            #region Calculate Current Redeem Point         

            List<RedeemPoint_History> plist = cust.RedeemPoint_History.ToList();
            if (plist.Count > 0)
            {
                if (DateTime.Now.Month < 7)
                {
                    foreach (RedeemPoint_History po in plist)
                    {
                        if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month >= 1 && po.DateTime.Month <= 6)
                        {
                            currentRedeemPoint += po.RedeemPoint;
                        }
                    }
                }
                else
                {
                    foreach (RedeemPoint_History po in plist)
                    {
                        if (po.DateTime.Year == ((DateTime.Now.Year)) && po.DateTime.Month >= 7 && po.DateTime.Month <= 12)
                        {
                            currentRedeemPoint += po.RedeemPoint;
                        }
                    }
                }

                lblCurrentRedeemPoint.Text = currentRedeemPoint.ToString();
            }
            #endregion

            //Calculate previous redeem point according to duration
            #region Calculate previous redeem point

            lblOldReedemPoint.Text = "0";
            List<RedeemPoint_History> oplist = cust.RedeemPoint_History.ToList();
            if (oplist.Count > 0)
            {
                if (DateTime.Now.Month < 7)
                {
                    foreach (RedeemPoint_History po in plist)
                    {
                        if (po.DateTime.Year == (DateTime.Now.Year - 1) && po.DateTime.Month >= 7 && po.DateTime.Month <= 12)
                        {
                            oldReedemPoint += po.RedeemPoint;
                        }
                    }
                }
                else
                {
                    foreach (RedeemPoint_History po in plist)
                    {
                        if (po.DateTime.Year == ((DateTime.Now.Year)) && po.DateTime.Month >= 1 && po.DateTime.Month <= 6)
                        {
                            oldReedemPoint += po.RedeemPoint;
                        }
                    }
                }
                lblOldReedemPoint.Text = oldReedemPoint.ToString();
            }

            #endregion

            //Calculate available point for previous duration
            #region Calculate Avaliable Point for Previous Duration

            // for available point of previous duration
            //lblAvailablePoint.Text = ELC_CustomerPointSystem.GetPointFromCustomerIdForPreviousDuration(cust.Id).ToString ();
            lblAvailablePoint.Text = ELC_CustomerPointSystem.Point_Calculation(cust.Id,false).ToString();

            // for date duration
            if (DateTime.Now.Month < 7)
            {
                lblDateDuration.Text = "From  01-July-" + (DateTime.Now.Year - 1).ToString() + "  To   " + "31-Dec-" + (DateTime.Now.Year - 1).ToString();                
            }
            else
            {
                lblDateDuration.Text = "From  01-Jan-" + (DateTime.Now.Year).ToString() + "  To   " + "30-June-" + DateTime.Now.Year.ToString();
            }

            #endregion

            // Transaction Data Binding
            dgvNormalTransaction.AutoGenerateColumns = false;
            List<Transaction> transList = cust.Transactions.Where(trans =>( trans.IsDeleted == false || trans.IsDeleted == null) && (trans.IsComplete==true)).OrderByDescending(x=>x.DateTime).ToList();           
            dgvNormalTransaction.DataSource = transList;

            // For Point Redeem History Tab Page
            dgvRedeemHistory.AutoGenerateColumns = false;            
            dgvRedeemHistory.DataSource = cust.RedeemPoint_History.OrderByDescending(x => x.DateTime).ToList();
            
            

            // For Available Gift Cards Tab Page
            dgvGiftCards.AutoGenerateColumns = false;           
            dgvGiftCards.DataSource = cust.GiftCards.Where(x => x.IsUsed == false && (x.IsDelete == null || x.IsDelete == false)).ToList();

            // For Used Gift Cards Tab Page
            dgvUsedGiftCards.AutoGenerateColumns = false;
            dgvUsedGiftCards.DataSource = cust.GiftCards.Where(x => x.IsUsed == true).ToList();

        }   
       
        // SYM
        private void btnRegisterGiftCards_Click(object sender, EventArgs e)
        {
            GiftCardControl _giftCardControl = new GiftCardControl();
            _giftCardControl.CurrentCustomerId = customerId;
            _giftCardControl.ShowDialog();
        }
        // SYM
        private void btnRedeemPoint_Click(object sender, EventArgs e)
        {
            RedeemPoint _redeemPoint = new RedeemPoint();
            _redeemPoint.customerId = customerId;
            _redeemPoint.Show();
        }
        // By SYM
        private void dgvGiftCards_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvGiftCards.Rows)
            {
                GiftCard gInC = (GiftCard)row.DataBoundItem;
                row.Cells[0].Value = gInC.CardNumber;
                row.Cells[1].Value = gInC.Amount;               
            }
        }
        // By SYM
        private void dgvUsedGiftCards_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvUsedGiftCards.Rows)
            {
                GiftCard gInC = (GiftCard)row.DataBoundItem;
                row.Cells[0].Value = gInC.CardNumber;
                row.Cells[1].Value = gInC.Amount;
            }
        }
        // By SYM
        private void dgvNormalTransaction_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string currentTransactionId = dgvNormalTransaction.Rows[e.RowIndex].Cells[0].Value.ToString();
                if (e.ColumnIndex == 8)
                {
                    bool isexp = false;
                    if (dgvNormalTransaction.Rows[e.RowIndex].Cells[dgvNormalTransaction.ColumnCount - 1].Value != null)
                    {
                        bool.TryParse(dgvNormalTransaction.Rows[e.RowIndex].Cells[dgvNormalTransaction.ColumnCount - 1].Value.ToString(), out isexp);
                    }
                    TransactionDetailForm newForm = new TransactionDetailForm(isexp);
                    newForm.transactionId = currentTransactionId;
                    newForm.ShowDialog();
                }
            }
        }
        // By SYM
        private void dgvNormalTransaction_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvNormalTransaction.Rows)
            {
                Transaction ts = (Transaction)row.DataBoundItem;
                row.Cells[0].Value = ts.Id;
                row.Cells[1].Value = ts.DateTime.Value.Date.ToString("dd-MM-yyyy");
                //row.Cells[2].Value = ts.DateTime.Value.TimeOfDay.Hours.ToString() + ts.DateTime.Value.TimeOfDay.Minutes.ToString();
                row.Cells[2].Value = ts.DateTime.Value.TimeOfDay.Hours.ToString() + ":" + ts.DateTime.Value.TimeOfDay.Minutes.ToString() + ":" + ts.DateTime.Value.Second.ToString();
                if(ts.PaymentTypeId!=null)
                {
                    row.Cells[3].Value = ts.PaymentType.Name;
                }                
                row.Cells[4].Value = ts.TotalAmount;
                if(ts.MemberTypeId!=null)
                {
                    row.Cells[5].Value = ts.MemberType.Name;
                }                
                row.Cells[6].Value = ts.Type;
                row.Cells[7].Value = ts.User.Name;
                row.Cells[dgvNormalTransaction.ColumnCount-1].Value = ts.IsExported;
            }
        }

        // By SYM
        private void dgvRedeemHistory_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvRedeemHistory.Rows)
            {
                RedeemPoint_History _redeemPointHistory = (RedeemPoint_History)row.DataBoundItem;
                row.Cells[0].Value = _redeemPointHistory.DateTime.ToString("dd-MM-yyyy");
                row.Cells[1].Value = _redeemPointHistory.DateTime.ToString("hh:mm");
                row.Cells[2].Value = _redeemPointHistory.Counter.Name;
                row.Cells[3].Value = _redeemPointHistory.User.Name;
            }
        }
        // By SYM
        private void tabPage3_Click(object sender, EventArgs e)
        {// point redeem history
            dgvRedeemHistory.DataSource = cust.RedeemPoint_History.OrderByDescending(x => x.DateTime).ToList();
        }

        // By SYM
        private void tabPage4_Click(object sender, EventArgs e)
        {// available gift cards
            dgvGiftCards.DataSource = cust.GiftCards.Where(x => x.IsUsed == false && (x.IsDelete == null || x.IsDelete == false)).ToList();
        }
       
        // By SYM
        private void tabPage5_Click(object sender, EventArgs e)
        {// used gift cards
            dgvUsedGiftCards.DataSource = cust.GiftCards.Where(x => x.IsUsed == true).ToList();
        }

        // By SYM
        private void tabPage2_Click(object sender, EventArgs e)
        { // previous available point and redeem point
            List<RedeemPoint_History> plist = cust.RedeemPoint_History.ToList();
            //List<RedeemPoint_History> oplist = cust.RedeemPoint_History.ToList();
            if (plist.Count > 0)
            {
                if (DateTime.Now.Month < 7)
                {
                    foreach (RedeemPoint_History po in plist)
                    {
                        if (po.DateTime.Year == (DateTime.Now.Year - 1) && po.DateTime.Month >= 7 && po.DateTime.Month <= 12)
                        {
                            oldReedemPoint += po.RedeemPoint;
                        }
                    }
                }
                else
                {
                    foreach (RedeemPoint_History po in plist)
                    {
                        if (po.DateTime.Year == ((DateTime.Now.Year)) && po.DateTime.Month >= 1 && po.DateTime.Month <= 6)
                        {
                            oldReedemPoint += po.RedeemPoint;
                        }
                    }
                }
                lblAvailablePoint.Text = ELC_CustomerPointSystem.Point_Calculation(cust.Id, false).ToString();
                lblOldReedemPoint.Text = oldReedemPoint.ToString();

            }
        }

        #endregion

    }
}
