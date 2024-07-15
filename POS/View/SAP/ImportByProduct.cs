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
    public partial class ImportByProduct : Form
    {
        POSEntities entity = new POSEntities();
        List<string> ProductCodes = new List<string>();
        int index = -1;
        Sales saleForm;
        public ImportByProduct(Sales sForm)
        {
            InitializeComponent();
            saleForm = sForm;
        }

        private void btnGetDataByProduct_Click(object sender, EventArgs e)
        {
            this.Dispose();
            DataImport importForm = new DataImport(saleForm);
            importForm.ProductCodes = ProductCodes;
            importForm.IsAllDataImport = false;
            importForm.IsAutoImport = false;
            importForm.Text = "Import By Product";
            importForm.ShowDialog();
        }

        private void ImportByProduct_Load(object sender, EventArgs e)
        {
            cboProductName.SelectedIndexChanged -= cboProductName_SelectedIndexChanged;
            BindProduct();
            cboProductName.SelectedIndexChanged += cboProductName_SelectedIndexChanged;
            cboProductName_SelectedIndexChanged(sender, e);
        }



        private void cboProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (index != -1)
            {
                dgvProductList.Rows[index].DefaultCellStyle.BackColor = Color.White;
            }

            if (cboProductName.SelectedIndex > 0)
            {

                string pcode = cboProductName.SelectedValue.ToString();
                index = ProductCodes.FindIndex(x => x.Contains(pcode));
                if (index > -1)
                {
                    dgvProductList.Rows[index].DefaultCellStyle.BackColor = Color.Yellow;
                }
                else
                {
                    dgvProductList.DefaultCellStyle.BackColor = Color.White;
                    DataGridViewRow row = (DataGridViewRow)dgvProductList.Rows[dgvProductList.Rows.Count - 1].Clone();
                    row.Cells[colProductCode.Index].Value = cboProductName.SelectedValue.ToString();
                    row.Cells[colProductName.Index].Value = cboProductName.GetItemText(cboProductName.SelectedItem);
                    dgvProductList.Rows.Add(row);
                    ProductCodes.Add(row.Cells[colProductCode.Index].Value.ToString());
                }



            }

        }
        private void BindProduct()
        {
            List<APP_Data.Product> productList = new List<APP_Data.Product>();
            APP_Data.Product productObj = new APP_Data.Product();
            productObj.Id = 0;
            productObj.Name = "Select Product";
            productList.Add(productObj);
            productList.AddRange(entity.Products.ToList());
            cboProductName.DataSource = productList;
            cboProductName.DisplayMember = "Name";
            cboProductName.ValueMember = "ProductCode";
            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;

        }

        private void dgvProductList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProductList.Rows.Count > 0)
            {
                if (e.ColumnIndex == colDelete.Index)
                {
                    string pcode = dgvProductList[1, e.RowIndex].Value.ToString();
                    if (!string.IsNullOrEmpty(pcode))
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            dgvProductList.Rows.RemoveAt(e.RowIndex);
                            ProductCodes.Remove(pcode);
                        }
                    }
                }

            }
        }
    }
}
