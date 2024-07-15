using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using POS.APP_Data;

namespace POS
{
    public partial class GiftCardTransactionHistory : Form
    {
        #region Initialize
        public GiftCardTransactionHistory()
        {
            InitializeComponent();
            dgvGiftTransactionList.AutoGenerateColumns = false;
        }
        #endregion

        #region Vairable
        POSEntities entity = new POSEntities();
        public int GiftCardId = 0;
        #endregion

        #region Method
        private void DataBind()
        {
          
                entity = new POSEntities();
                IQueryable<object> q = from t in entity.Transactions
                                       join td in entity.TransactionPaymentDetails on t.Id equals td.TransactionId
                                       join p in entity.PaymentMethods on td.PaymentMethodId equals p.Id
                                       join u in entity.Users on t.UserId equals u.Id
                                       where (t.IsDeleted == false || t.IsDeleted == null) &&
                                         (t.GiftCardId == GiftCardId) && p.Name=="Gift Card" &&
                                       (t.IsComplete == true) && (t.IsActive == true)
                                       select new
                                       {
                                           TransactionId = t.Id,
                                           Type = t.Type,
                                           PaymentMethod = p.Name,
                                           Date = t.DateTime,
                                           Time = t.DateTime,
                                           SalePerson = u.Name,
                                           TotalAmount = t.TotalAmount,
                                           GiftCardAmount = t.GiftCardAmount,
                                           CashAmount = t.RecieveAmount,
                                           IsExported=t.IsExported
                                       };
                List<object> giftCard = new List<object>(q);

                dgvGiftTransactionList.AutoGenerateColumns = false;
                dgvGiftTransactionList.DataSource = giftCard;

             Int64 TotalGiftCardAmt = dgvGiftTransactionList.Rows.Cast<DataGridViewRow>()
                         .Sum(t =>  Convert.ToInt32(t.Cells["colGiftCardAmount"].Value));

             lblTotalGiftCardAmt.Text = TotalGiftCardAmt.ToString();

        }
     
        #endregion



        private void GiftCardTransactionHistory_Load(object sender, EventArgs e)
        {
            dgvGiftTransactionList.Columns["colDate"].DefaultCellStyle.Format = "dd-MM-yyyy";
            dgvGiftTransactionList.Columns["colTime"].DefaultCellStyle.Format = "hh:mm";
       
            DataBind();
        }

        private void dgvGiftTransactionList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string currentTransactionId = dgvGiftTransactionList.Rows[e.RowIndex].Cells[0].Value.ToString();
            //View Detail
            if (e.ColumnIndex == 9)
            {
                bool isexp = false;
                if (dgvGiftTransactionList.Rows[e.RowIndex].Cells[dgvGiftTransactionList.ColumnCount - 1].Value != null)
                {
                    bool.TryParse(dgvGiftTransactionList.Rows[e.RowIndex].Cells[dgvGiftTransactionList.ColumnCount - 1].Value.ToString(), out isexp);
                }

                TransactionDetailForm newForm = new TransactionDetailForm(isexp);
                    newForm.transactionId = currentTransactionId;
                    newForm.ShowDialog();
                
            }
        }
      
    }
}
