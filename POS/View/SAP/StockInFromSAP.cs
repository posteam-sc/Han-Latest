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
using POS.APP_Data;

namespace POS
{
    public partial class StockInFromSAP : Form
    {
        #region variables
        POSEntities entity = new POSEntities();
        public string ProductCode;
        bool IsFormLoad = false;
        List<StockInListFromSAP> gridList = new List<StockInListFromSAP>();



        #endregion
        public StockInFromSAP()
        {
            InitializeComponent();
        }
        #region Events
        private void StockInFromSAP_Load(object sender, EventArgs e)
        {
            IsFormLoad = true;
            LoadDataByStockInDate();
        }
                       
       
        private void btnProductCodeSearch_Click(object sender, EventArgs e)
        {
            txtProductName.Text = "";
            if (txtProductCode.Text == "")
            {
                MessageBox.Show("Please Enter Product Code  to Search");
                txtProductCode.Focus();
            }
            else
            {
                LoadDataByProductCode();

            }
        }
        private void rdbAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbAll.Checked)
            {
                dgvStockInFromSAP.DataSource = "";
                dgvStockInFromSAP.AutoGenerateColumns = false;
                dgvStockInFromSAP.DataSource = gridList;
            }
        }


        private void rdbActive_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbActive.Checked)
            {
                List<StockInListFromSAP> gridList1 = new List<StockInListFromSAP>();
                gridList1 = gridList.Where(x => x.IsActive == true).ToList();

                if (gridList1.Count > 0)
                {
                    dgvStockInFromSAP.DataSource = "";
                    dgvStockInFromSAP.AutoGenerateColumns = false;
                    dgvStockInFromSAP.DataSource = gridList1;
                }

            }
        }

        private void dgvStockInFromSAP_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvStockInFromSAP.Rows)
            {
                if (row.Cells[ColActive.Index].Value.ToString() == "False")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }
        private void btnProductNameSearch_Click(object sender, EventArgs e)
        {
            txtProductCode.Text = "";
            if (txtProductName.Text == "")
            {
                MessageBox.Show("Please Enter Product Name to Search");
                txtProductName.Focus();
            }
            else
            {
                LoadDataByProductName();

            }
        }

        private void btnStockSearch_Click(object sender, EventArgs e)
        {
            IsFormLoad = false;
            LoadDataByStockInDate();
        }
        #endregion

        #region Methods
        private void LoadDataByStockInDate()
        {
            rdbAll.Checked = true;
            POSEntities entity = new POSEntities();
            DateTime startDate = StartDatedateTimePicker.Value.Date;
            DateTime endDate = EndDatedateTimePicker.Value.Date;

            //List<GetStockInByDate_Result>  stockInListByDate = entity.GetStockInByDate(startDate, endDate).ToList();

            List<StockInListFromSAP> stockInListByDate = entity.GetStockInByDate_1(startDate, endDate).OrderByDescending(x => x.CreatedDate).ToList();




            if (stockInListByDate.Count > 0)
            {
                dgvStockInFromSAP.DataSource = "";
                dgvStockInFromSAP.AutoGenerateColumns = false;
                dgvStockInFromSAP.DataSource = stockInListByDate;
                gridList = stockInListByDate;

            }
            else
            {
                if (!IsFormLoad)
                {
                    MessageBox.Show("No Stock In Data");
                }
                else
                {
                    MessageBox.Show("No Stock In Data Today");
                }

            }

        }

        private void LoadDataByProductCode()
        {
            rdbAll.Checked = true;

            POSEntities entity = new POSEntities();
            string Pcode = txtProductCode.Text.Trim();
            List<StockInListFromSAP> stockInListByCode = entity.GetStockInByProductCode_1(Pcode).OrderByDescending(x => x.CreatedDate).ToList();
            //if (rdbActive.Checked)
            //{
            //    stockInListByCode = stockInListByCode.Where(x => x.IsActive == true).ToList();
            //}
            if (stockInListByCode.Count > 0)
            {

                dgvStockInFromSAP.DataSource = "";
                dgvStockInFromSAP.AutoGenerateColumns = false;
                dgvStockInFromSAP.DataSource = stockInListByCode;
                gridList = stockInListByCode;
            }
            else
            {
                MessageBox.Show("Item Not Found");
            }

        }

        private void LoadDataByProductName()
        {
            rdbAll.Checked = true;
            POSEntities entity = new POSEntities();
            string Pname = txtProductName.Text.Trim();
            List<StockInListFromSAP> stockInListByName = entity.GetStockInByProductName_1(Pname).OrderByDescending(x => x.CreatedDate).ToList();
            //if (rdbActive.Checked)
            //{
            //    stockInListByName = stockInListByName.Where(x => x.IsActive == true).ToList();
            //}
            if (stockInListByName.Count > 0)
            {

                dgvStockInFromSAP.DataSource = "";
                dgvStockInFromSAP.AutoGenerateColumns = false;
                dgvStockInFromSAP.DataSource = stockInListByName;
                gridList = stockInListByName;
            }
            else
            {
                MessageBox.Show("Item Not Found");
            }

        }

        #endregion

             


    }


}
