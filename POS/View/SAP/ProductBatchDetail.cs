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
    public partial class ProductBatchDetial : Form
    {
        public ProductBatchDetial()
        {
            InitializeComponent();
        }
        #region variables
        public int ProductId { get; set; }
        public string ProdName { get; set; }
        #endregion

        #region Events
        private void ProductBatchDetial_Load(object sender, EventArgs e)
        {

            POSEntities entity = new POSEntities();
           // List<GetBatchNoByExpDateOrder_Result> st = entity.GetBatchNoByExpDateOrder(ProductId).ToList();
            List<StockFillingFromSAP> st = entity.StockFillingFromSAPs.Where(x => x.ProductId == ProductId && x.IsActive == true).OrderBy(x => x.ExpireDate).ToList();
            

            lblProductName.Text = ProdName;
            dgvBatchDetails.AutoGenerateColumns = false;
            dgvBatchDetails.DataSource = st;
            int totalQty = (int) st.AsEnumerable().Sum(x => x.ProductQty);
            int availableQty = (int)st.AsEnumerable().Sum(x => x.AvailableQty);
            lblTotalQty.Text = availableQty > totalQty ? availableQty.ToString() : totalQty.ToString();
            lblAvailableQty.Text = availableQty.ToString();

        }

        private void dgvBatchDetails_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            int totalQty;
            int availableQty;
            foreach (DataGridViewRow row in dgvBatchDetails.Rows)
            {
                StockFillingFromSAP sp = (StockFillingFromSAP)row.DataBoundItem;
                row.Cells[ColBatchNo.Index].Value = sp.BatchNo;
                totalQty = (int)sp.ProductQty;
                availableQty = (int)sp.AvailableQty;               
                row.Cells[ColTotalQty.Index].Value = availableQty > totalQty ? sp.AvailableQty : sp.ProductQty;
                row.Cells[ColAvailableQty.Index].Value = sp.AvailableQty;
                row.Cells[ColExpireDate.Index].Value = sp.ExpireDate.Value.ToString("dd/MM/yyyy");               
            }
        }

        #endregion


    }
}
