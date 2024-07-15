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
using Microsoft.Reporting.WinForms;


namespace POS
{
    public partial class ProfitAndLoss_frm : Form
    {
       

        #region Variable
        List<ProfitAndLoss> plist = new List<ProfitAndLoss>();
        POSEntities entity = new POSEntities();
        int productId, brandId, counterId;
        Int64 totalSaleQty, totalPurchaseAmount, totalSaleAmount, totalDiscountAmount, totalTaxAmount, totalProfitAndLossAmount;
        Boolean isstart = false;
        #endregion

        #region  Events
        public ProfitAndLoss_frm()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private void ProfitAndLoss_frm_Load(object sender, EventArgs e)
        {
           
            //bind Brand combo box
            List<APP_Data.Brand> BrandList = new List<APP_Data.Brand>();

            APP_Data.Brand brandObj2 = new APP_Data.Brand();
            brandObj2.Id = 0;
            brandObj2.Name = "All";
            BrandList.Add(brandObj2);
            BrandList.AddRange((from bList in entity.Brands select bList).ToList());
            cboBrand.DataSource = BrandList;
            cboBrand.DisplayMember = "Name";
            cboBrand.ValueMember = "Id";
            cboBrand.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboBrand.AutoCompleteSource = AutoCompleteSource.ListItems;

            //bind product combo box
            List<APP_Data.Product> ProductList = new List<APP_Data.Product>();
            APP_Data.Product pdObj2 = new APP_Data.Product();
            pdObj2.Id = 0;
            pdObj2.Name = "All";
            ProductList.Add(pdObj2);
            ProductList.AddRange((from plist in entity.Products where plist.IsConsignment == false select plist).ToList());
            cboProductName.DataSource = ProductList;
            cboProductName.DisplayMember = "Name";
            cboProductName.ValueMember = "Id";
            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;

            //bind counter combo box
            List<APP_Data.Counter> counterlist = new List<APP_Data.Counter>();
            APP_Data.Counter cObj = new APP_Data.Counter();
            cObj.Id = 0;
            cObj.Name = "All";
            counterlist.Add(cObj);
            counterlist.AddRange((from c in entity.Counters select c).ToList());
            cboCounter.DataSource = counterlist;
            cboCounter.DisplayMember = "Name";
            cboCounter.ValueMember = "Id";
            cboCounter.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCounter.AutoCompleteSource = AutoCompleteSource.ListItems;

            rdoAll.Checked = true;
            rdoProduct.Checked = false;
            rdoBrand.Checked = false;

            cboBrand.Enabled = false;
            cboProductName.Enabled = false;

            Utility.BindShop(cboshoplist);
            cboshoplist.Text = SettingController.DefaultShop.ShopName;
            cboshoplist.Enabled = false;
            //Utility.ShopComBo_EnableOrNot(cboshoplist);
            isstart = true;
            loadData();
        }       

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            loadData();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            loadData();
        }

        private void cboProductName_SelectedValueChanged(object sender, EventArgs e)
        {
            loadData();
        }

        private void cboBrand_SelectedValueChanged(object sender, EventArgs e)
        {
            loadData();
        }

        private void cbo_SubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //bind Brand combo box
            List<APP_Data.Brand> BrandList = new List<APP_Data.Brand>();

            APP_Data.Brand brandObj2 = new APP_Data.Brand();
            brandObj2.Id = 0;
            brandObj2.Name = "All";
            BrandList.Add(brandObj2);
            BrandList.AddRange((from bList in entity.Brands select bList).ToList());
            cboBrand.DataSource = BrandList;
            cboBrand.DisplayMember = "Name";
            cboBrand.ValueMember = "Id";
            cboBrand.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboBrand.AutoCompleteSource = AutoCompleteSource.ListItems;

            //bind product combo box
            List<APP_Data.Product> ProductList = new List<APP_Data.Product>();
            APP_Data.Product pdObj2 = new APP_Data.Product();
            pdObj2.Id = 0;
            pdObj2.Name = "All";
            ProductList.Add(pdObj2);
            ProductList.AddRange((from plist in entity.Products select plist).ToList());
            cboProductName.DataSource = ProductList;
            cboProductName.DisplayMember = "Name";
            cboProductName.ValueMember = "Id";
            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;

            //bind counter combo box
            List<APP_Data.Counter> counterlist = new List<APP_Data.Counter>();
            APP_Data.Counter cObj = new APP_Data.Counter();
            cObj.Id = 0;
            cObj.Name = "All";
            counterlist.Add(cObj);
            counterlist.AddRange((from c in entity.Counters select c).ToList());
            cboCounter.DataSource = counterlist;
            cboCounter.DisplayMember = "Name";
            cboCounter.ValueMember = "Id";
            cboCounter.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCounter.AutoCompleteSource = AutoCompleteSource.ListItems;

            rdoAll.Checked = true;
            rdoProduct.Checked = false;
            rdoBrand.Checked = false;

            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;

            cboshoplist.Text = SettingController.DefaultShop.ShopName;
          
        }

        private void cbo_Category_SelectedValueChanged(object sender, EventArgs e)
        {
            loadData();

        }

        private void rdoProduct_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoProduct.Checked)
            {
              //  rdoAll.Checked = false;
              //  cboCounter.Enabled = false;
                cboProductName.Enabled = true;
                rdoBrand.Checked = false;
                cboBrand.Enabled = false;
                cboBrand.SelectedIndex = 0;
               // cboCounter.SelectedIndex = 0;
              
            }
        }

        private void rdoBrand_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoBrand.Checked)
            {
                cboBrand.Enabled = true;
            //    rdoAll.Checked = false;
                rdoProduct.Checked = false;
                cboProductName.Enabled = false;
            //    cboCounter.Enabled = false;
              //  rdoCounterName.Checked = false;
                cboProductName.SelectedIndex = 0;
              //  cboCounter.SelectedIndex = 0;
               
            }
        }

        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAll.Checked)
            {
              //  cboProductName.Enabled = false;
               // cboBrand.Enabled = false;
                rdoCounterName.Checked = false;
                cboCounter.Enabled = false;
               // cboProductName.SelectedIndex = 0;
               // cboBrand.SelectedIndex = 0;
                cboCounter.SelectedIndex = 0;
 
            }
          
        }

        private void rdoCounterName_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCounterName.Checked)
            {
                rdoAll.Checked = false;
              //  rdoBrand.Checked = false;
               // rdoProduct.Checked = false;
                rdoCounterName.Checked = true;
                cboCounter.Enabled = true;
              //  cboProductName.Enabled = false;
           //     cboBrand.Enabled = false;
           //     cboBrand.SelectedIndex = 0;
                cboCounter.SelectedIndex = 0;
            //    cboProductName.SelectedIndex = 0;

            }

        }

        private void cboCounter_SelectedValueChanged(object sender, EventArgs e)
        {           
            loadData();
        }


        private void cboshoplist_selectedIndexChanged(object sender, EventArgs e)
        {
            loadData();
        }
        #endregion

        #region Functions

        public void loadData()
        {
            if (isstart == true)
            {
                int shopid = Convert.ToInt32(cboshoplist.SelectedValue);
                string currentshortcode = (from p in entity.Shops where p.Id == shopid select p.ShortCode).FirstOrDefault();
                string currentshopname = (from p in entity.Shops where p.Id == shopid select p.ShopName).FirstOrDefault();

                plist.Clear();
                productId = 0; brandId = 0; counterId = 0;
                totalSaleQty = 0; totalPurchaseAmount = 0; totalSaleAmount = 0; totalDiscountAmount = 0; totalTaxAmount = 0; totalProfitAndLossAmount = 0;

                DateTime fromDate = dtFrom.Value;
                DateTime toDate = dtTo.Value;


                if (rdoProduct.Checked)
                {
                    if (cboProductName.SelectedIndex > 0)
                    {
                        productId = Convert.ToInt32(cboProductName.SelectedValue);
                    }
                }
                if (rdoBrand.Checked)
                {
                    if (cboBrand.SelectedIndex > 0)
                    {
                        brandId = Convert.ToInt32(cboBrand.SelectedValue);
                    }
                }
                if (rdoCounterName.Checked)
                {
                    if (cboCounter.SelectedIndex > 0)
                    {
                        counterId = Convert.ToInt32(cboCounter.SelectedValue);
                    }

                }


                System.Data.Objects.ObjectResult<GetProfitandLoss_Result> prlist = entity.GetProfitandLoss(fromDate, toDate, brandId, productId, counterId,currentshortcode);

                foreach (GetProfitandLoss_Result pr in prlist)
                {
                    ProfitAndLoss pfl = new ProfitAndLoss();
                    pfl.SaleDate = Convert.ToDateTime(pr.SaleDate);
                    pfl.TotalSaleQty = Convert.ToInt32(pr.TotalSaleQty);
                    pfl.TotalPurchaseAmount = Convert.ToInt64(pr.TotalPurchaseAmount);
                    pfl.TotalSaleAmount = Convert.ToInt64(pr.TotalSaleAmount);
                    pfl.TotalDiscountAmount = Convert.ToInt64(pr.TotalDiscountAmount);
                    pfl.TotalTaxAmount = Convert.ToInt64(pr.TotalTaxAmount);
                    pfl.ProfitAndLossAmount = Convert.ToInt64(pr.Total_ProfitAndLoss);

                    totalSaleQty += pfl.TotalSaleQty;
                    totalPurchaseAmount += pfl.TotalPurchaseAmount;
                    totalSaleAmount += pfl.TotalSaleAmount;
                    totalDiscountAmount += pfl.TotalDiscountAmount;
                    totalTaxAmount += pfl.TotalTaxAmount;
                    totalProfitAndLossAmount += pfl.ProfitAndLossAmount;
                    plist.Add(pfl);
                }


                ShowReportViewer(currentshopname);
            }
        }

        private void ShowReportViewer(string currentshopname)
        {
            string ProductName = ""; string BrandName = ""; string CounterName = "";

            if (rdoAll.Checked)
            {
                ProductName = " All ";
            }
            if (rdoProduct.Checked)
            {
                if (productId == 0)
                {
                    ProductName = "All Product ";
                }
                else
                {
                    APP_Data.Product p = entity.Products.Where(x => x.Id == productId).FirstOrDefault();
                    ProductName = " Product - " + p.Name.ToString();
                } 
            }
            if (rdoBrand.Checked)
            {
                if (brandId == 0)
                {
                    BrandName = " All Brand ";
                }
                else
                {
                    APP_Data.Brand b = entity.Brands.Where(x => x.Id == brandId).FirstOrDefault();
                    BrandName = " Brand - " + b.Name.ToString();
                }                
            }
            if (rdoCounterName.Checked)
            {
                if (counterId == 0)
                {
                    CounterName = " All ";
                }
                else
                {
                    APP_Data.Counter c = entity.Counters.Where(x => x.Id == counterId).FirstOrDefault();
                    CounterName = "Counter - " + c.Name.ToString();   
                }                           
            }

            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.ProfitAndLossTableDataTable dtPdReport = (dsReportTemp.ProfitAndLossTableDataTable)dsReport.Tables["ProfitAndLossTable"];
            foreach (ProfitAndLoss pdCon in plist)
            {
                dsReportTemp.ProfitAndLossTableRow newRow = dtPdReport.NewProfitAndLossTableRow();
                newRow.SaleDate = Convert.ToDateTime(pdCon.SaleDate);
                newRow.TotalSaleQty = pdCon.TotalSaleQty.ToString();
                newRow.TotalPurchaseAmount = pdCon.TotalPurchaseAmount.ToString();
                newRow.TotalSaleAmount = pdCon.TotalSaleAmount.ToString();
                newRow.TotalDiscountAmount = pdCon.TotalDiscountAmount.ToString();
                newRow.TotalTaxAmount = pdCon.TotalTaxAmount.ToString();
                newRow.TotalProfitAndLossAmount = pdCon.ProfitAndLossAmount.ToString();
                dtPdReport.AddProfitAndLossTableRow(newRow);
            }
            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ProfitAndLossTable"]);
            string reportPath = Application.StartupPath + "\\Reports\\Profit And Loss Report.rdlc";

            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            //ReportParameter ShopName = new ReportParameter("ShopName", "Profit and Loss Report for " + currentshopname);
            //reportViewer1.LocalReport.SetParameters(ShopName);

            ReportParameter TotalSaleQty = new ReportParameter("TotalSaleQty", totalSaleQty.ToString());
            reportViewer1.LocalReport.SetParameters(TotalSaleQty);

            ReportParameter TotalPurchaseAmount = new ReportParameter("TotalPurchaseAmount", totalPurchaseAmount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalPurchaseAmount);

            ReportParameter TotalSaleAmount = new ReportParameter("TotalSaleAmount", totalSaleAmount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalSaleAmount);

            ReportParameter TotalDiscountAmount = new ReportParameter("TotalDiscountAmount", totalDiscountAmount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalDiscountAmount);

            ReportParameter TotalTaxAmount = new ReportParameter("TotalTaxAmount", totalTaxAmount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalTaxAmount);

            ReportParameter TotalProfitAndLossAmount = new ReportParameter("TotalProfitAndLossAmount", totalProfitAndLossAmount.ToString());
            reportViewer1.LocalReport.SetParameters(TotalProfitAndLossAmount);

            ReportParameter Header = new ReportParameter("Header", "Gross Profit / Loss Report " + ProductName + BrandName + CounterName +  " at " + DateTime.Now.Date.ToString("dd-MM-yyyy"));
            reportViewer1.LocalReport.SetParameters(Header);
            reportViewer1.RefreshReport();
        }

        #endregion

     

        

       
        
    }
}

