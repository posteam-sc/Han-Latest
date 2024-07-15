using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace POS
{
    public partial class frmPriceChangeWithExcel : Form
    {
        private string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
        private string Excel07ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
        private bool check;
        public frmPriceChangeWithExcel()
        {
            InitializeComponent();
        }

        private void frmPriceChangeWithExcel_Load(object sender, EventArgs e)
        {
            check = false;
            CheckSaveButton();
        }

        #region Method
        private void CheckSaveButton()
        {
            btnSave.Visible = (check == false ? false : true);
        }

        private void ExportExcel()
        {
            try
            {
                string filePath = Directory.GetCurrentDirectory();
                using (SaveFileDialog sfd = new SaveFileDialog()
                {

                    InitialDirectory = filePath,
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx|xls file (*.xls)|*.xls|All files (*.*)|*.*",
                    ValidateNames = true
                })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                        Workbook wb = app.Workbooks.Add(XlSheetType.xlWorksheet);
                        Worksheet ws = (Worksheet)app.ActiveSheet;
                        app.Visible = false;

                        ws.Cells[1, 1] = "No";

                        ws.Cells[1, 2] = "Product Code";

                        ws.Cells[1, 3] = "Item Name";

                        ws.Cells[1, 4] = "Price";

                        wb.SaveAs(sfd.FileName, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                        app.Quit();
                        MessageBox.Show("Save Successfully Export !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ofdSelect_FileOk(object sender, CancelEventArgs e)
        {
            btnSave.Visible = false;
            System.Data.DataTable dt = new System.Data.DataTable();
            string filePath = ofd.FileName;
            string extension = Path.GetExtension(filePath);
            string conString = "";
            string sheetName = "";
            switch (extension)
            {
                case ".xls":
                    conString = string.Format(Excel03ConString, filePath, "YES");
                    break;
                case ".xlsx":
                    conString = string.Format(Excel07ConString, filePath, "YES");
                    break;
                default:
                    MessageBox.Show("Invalid file", "Informtion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }
            using (OleDbConnection con = new OleDbConnection(conString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = con;
                    con.Open();
                    System.Data.DataTable dth = new System.Data.DataTable();
                    dth = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    sheetName = dth.Rows[0]["Table_Name"].ToString();
                    con.Close();
                }
            }
            using (OleDbConnection con = new OleDbConnection(conString))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    OleDbDataAdapter oda = new OleDbDataAdapter();
                    cmd.CommandText = "SELECT * FROM [" + sheetName + "]";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    oda.SelectCommand = cmd;
                    oda.Fill(dt);
                    con.Close();
                    dgvImport.DataSource = dt;
                    dgvImport.Columns[0].Width = 50;
                    dgvImport.Columns[1].Width = 115;
                    dgvImport.Columns[2].Width = 290;
                    dgvImport.Columns[3].Width = 80;
                    dgvImport.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


                }
            }
            btnSave.Visible = false;
        }

        #endregion

        #region Button Click
        private void btnAddFile_Click(object sender, EventArgs e)
        {
            ofd.ShowDialog();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                POSEntities entity = new POSEntities();
                if (dgvImport.Rows.Count > 0)
                {

                    check = true;
                    for (int i = 0; i < dgvImport.Rows.Count - 1; i++)
                    {
                        dgvImport.Rows[i].Cells["Product Code"].Style.BackColor = Color.White;
                        dgvImport.Rows[i].Cells["Price"].Style.BackColor = Color.White;
                        bool checkColor = false;
                        bool productCode = false;
                        bool price = false;

                        string dgvProduct = dgvImport.Rows[i].Cells["Product Code"].Value.ToString();
                        string productCodeName = entity.Products.Where(x => x.ProductCode == dgvProduct).Select(x => x.ProductCode).FirstOrDefault();
                        if (productCodeName == null)
                        {
                            dgvImport.Rows[i].Cells["Product Code"].Style.BackColor = Color.Red;
                            check = false;
                            checkColor = true;
                            MessageBox.Show(dgvProduct + " This Product Code Not Include in POS", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;

                        }
                        if (!String.IsNullOrWhiteSpace(Convert.ToString(dgvImport.Rows[i].Cells["Product Code"])))
                        {
                            if (String.IsNullOrWhiteSpace(dgvImport.Rows[i].Cells["Price"].Value.ToString()) || Convert.ToInt32(dgvImport.Rows[i].Cells["Price"].Value.ToString()) == 0)
                            {
                                dgvImport.Rows[i].Cells["Price"].Style.BackColor = Color.Red;
                                check = false;
                                checkColor = true;
                                MessageBox.Show(dgvProduct + " This Product Price Not Include In Excel", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }

                    }
                }
                CheckSaveButton();
            }
            catch (Exception)
            {

                MessageBox.Show("Exce File is Invalid","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (dgvImport.Rows.Count > 0)
            {
                DialogResult ok = MessageBox.Show("are you sure to save data?", "information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ok == DialogResult.Yes)
                {
                    // List<Product> prodcutList = new List<Product>();
                    for (int i = 0; i < dgvImport.Rows.Count - 1; i++)
                    {
                        try
                        {
                            POSEntities entity = new POSEntities();
                            ProductPriceChange productPriceChange = new ProductPriceChange();
                            string dgvProduct = dgvImport.Rows[i].Cells["Product Code"].Value.ToString();
                            string productCodeName = entity.Products.Where(x => x.ProductCode == dgvProduct).Select(x => x.ProductCode).FirstOrDefault();
                            Product product = (from p in entity.Products where p.ProductCode == productCodeName select p).FirstOrDefault();
                            int productId = Convert.ToInt32(product.Id);
                            long oldPrice = product.Price;
                            product.Price = Convert.ToInt64(dgvImport.Rows[i].Cells["Price"].Value.ToString());

                            productPriceChange.ProductId = productId;
                            productPriceChange.OldPrice = oldPrice;
                            productPriceChange.UpdateDate = DateTime.Now;
                            productPriceChange.UserID = MemberShip.UserId;
                            productPriceChange.Price = Convert.ToInt64(dgvImport.Rows[i].Cells["Price"].Value.ToString());
                            entity.ProductPriceChanges.Add(productPriceChange);
                            entity.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                    }
                    MessageBox.Show("Save Successfully", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportExcel(); // To provide data entry format (Column names...) for the clerk
        }
        #endregion
    }
}
