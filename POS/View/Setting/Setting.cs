using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;
using System.IO;
namespace POS
{
    public partial class Setting : Form
    {
        #region Variable
        POSEntities entity = new POSEntities();
        private ToolTip tp = new ToolTip();
        private String FilePath;
        #endregion
        public Setting()
        {
            InitializeComponent();
        }

        private void Setting_Load(object sender, EventArgs e)
        {

            if(Utility.IsNotBackOffice()==true)
            {
                btnNewTax.Visible = false;
                btnNewCity.Visible = false;
            }
            else
            {
                btnNewTax.Visible = true;
                btnNewCity.Visible = true;
            }
            #region BindShop
            Utility.BindShop(cboShopList);

            cboShopList.Text = SettingController.DefaultShop.ShopName;
            #endregion

            Utility.ShopComBo_EnableOrNot(cboShopList,true);
            #region Printer

            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                cboBarcodePrinter.Items.Add(printerName);
                cboA4Printer.Items.Add(printerName);
                cboSlipPrinter.Items.Add(printerName);
            }

            if (DefaultPrinter.BarcodePrinter != null)
            {
                cboBarcodePrinter.Text = DefaultPrinter.BarcodePrinter;
            }
            if (DefaultPrinter.A4Printer != null)
            {
                cboA4Printer.Text = DefaultPrinter.A4Printer;
            }
            if (DefaultPrinter.SlipPrinter != null)
            {
                cboSlipPrinter.Text = DefaultPrinter.SlipPrinter;
            }

            if (SettingController.SelectDefaultPrinter != "")
            {
                switch (SettingController.SelectDefaultPrinter)
                {
                    case "A4 Printer":
                        rdoA4Printer.Checked=true;
                        break;
                    case "Slip Printer":
                        rdoSlipPrinter.Checked=true;
                        break;
                }
            }
            #endregion

            #region taxPercent
            List<Tax> taxList = entity.Taxes.ToList();
            List<Tax> result = new List<Tax>();
            foreach (Tax r in taxList)
            {
                Tax t = new Tax();
                t.Id = r.Id;
                t.Name = r.Name + " and " + r.TaxPercent;
                t.TaxPercent = r.TaxPercent;
                result.Add(t);                
            }
            cboTaxList.DataSource = result;
            cboTaxList.DisplayMember = "Name";
            cboTaxList.ValueMember = "Id";
            if (SettingController.DefaultTaxRate != "")
            {
                int id = Convert.ToInt32(SettingController.DefaultTaxRate);
                Tax defaultTax = (from t in entity.Taxes where t.Id == id select t).FirstOrDefault();
                cboTaxList.Text = defaultTax.Name + " and " + defaultTax.TaxPercent;
            }
            #endregion

            #region city
            List<APP_Data.City> cityList = entity.Cities.ToList();
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";
            if (SettingController.DefaultCity != 0)
            {
                int id = Convert.ToInt32(SettingController.DefaultCity);
                APP_Data.City city = entity.Cities.Where(x => x.Id == id).FirstOrDefault();
                cboCity.Text = city.CityName;
            }
            #endregion

            #region currency
            List<Currency> currencyList = new List<Currency>();
            currencyList.AddRange(entity.Currencies.ToList());
            cboCurrency.DataSource = currencyList;
            cboCurrency.DisplayMember = "CurrencyCode";
            cboCurrency.ValueMember = "Id";
            if (SettingController.DefaultCurrency != 0)
            {
                int id = Convert.ToInt32(SettingController.DefaultCurrency);
                Currency curreObj = entity.Currencies.FirstOrDefault(x => x.Id == id);
                cboCurrency.Text = curreObj.CurrencyCode;
            }
            //txtExchangeRate.Text = SettingController.DefaultExchangeRate.ToString();
            #endregion

            //txtShopName.Text = SettingController.ShopName;
            //txtBranchName.Text = SettingController.BranchName;
            //txtPhoneNo.Text = SettingController.PhoneNo;
            //txtOpeningHours.Text = SettingController.OpeningHours;
            txtNoOfCopy.Text = SettingController.DefaultNoOfCopies.ToString();
           // dpCompanyStartDate.Format = DateTimePickerFormat.Custom;
       
           
          
         
                 try { 
            dpCompanyStartDate.Value = Convert.ToDateTime(SettingController.Company_StartDate);
                 }
                 catch
                 {

                 }
                 //dpCompanyStartDate.CustomFormat = "dd-MM-yyyy";
            txtDefaultSalesRow.Text = SettingController.DefaultTopSaleRow.ToString();

            txtMemberValidityYear.Text = SettingController.DefaultMemberValidityYear.ToString();

            chkUseStockAutoGenerate.Checked = SettingController.UseStockAutoGenerate;
            lblShopStatus.Text = SettingController.IsBackOffice;
            txtFooterPage.Text = SettingController.FooterPage;
            if (SettingController.Logo != null && SettingController.Logo != "")
            {
                this.txtImagePath.Text = SettingController.Logo.ToString();
                string FileNmae = txtImagePath.Text.Substring(7);
                this.ptImage.ImageLocation = Application.StartupPath + "\\logo\\" + FileNmae;
                this.ptImage.SizeMode = PictureBoxSizeMode.Zoom;
            }

            #region SAP
            chkSAP.Checked = SettingController.DisabledImportExport;
            #endregion

        }



        public class ReadOnlyCheckBox : System.Windows.Forms.CheckBox
        {
            private bool readOnly;

            protected override void OnClick(EventArgs e)
            {
                // pass the event up only if its not readlonly
                if (!ReadOnly) base.OnClick(e);
            }

            public bool ReadOnly
            {
                get { return readOnly; }
                set { readOnly = value; }
            }
        }
        private void txtNO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation
            //if (txtShopName.Text.Trim() == string.Empty)
            //{
            //    tp.SetToolTip(txtShopName, "Error");
            //    tp.Show("Please fill up shop name!", txtShopName);
            //    hasError = true;
            //}
            //else if (txtBranchName.Text.Trim() == string.Empty)
            //{
            //    tp.SetToolTip(txtBranchName, "Error");
            //    tp.Show("Please fill up branch name or address!", txtBranchName);
            //    hasError = true;
            //}
            //else if (txtPhoneNo.Text.Trim() == string.Empty)
            //{
            //    tp.SetToolTip(txtPhoneNo, "Error");
            //    tp.Show("Please fill up pnone number!", txtPhoneNo);
            //    hasError = true;
            //}
            //else if (txtOpeningHours.Text.Trim() == string.Empty)
            //{
            //    tp.SetToolTip(txtOpeningHours, "Error");
            //    tp.Show("Please fill up opening hour!", txtOpeningHours);
            //    hasError = true;
            //}
            if (cboShopList.SelectedIndex < 0)
            {
                tp.SetToolTip(cboShopList, "Error");
                tp.Show("Please choose default shop!", cboShopList);
                hasError = true;
            }
             if (dpCompanyStartDate.Value == null)
            {
                tp.SetToolTip(dpCompanyStartDate, "Error");
                tp.Show("Please fill up Company Start Date!", dpCompanyStartDate);
                hasError = true;
            }

            else if (txtImagePath.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtImagePath, "Error");
                tp.Show("Please browser Logo!", txtImagePath);
                hasError = true;
            }
            else if (ptImage == null)
            {
                tp.SetToolTip(ptImage, "Error");
                tp.Show("Please browser Logo!", ptImage);
                hasError = true;
            }
            else if (rdoA4Printer.Checked)
            {
                if (cboA4Printer.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(cboA4Printer, "Error");
                    tp.Show("Please select A4 Printer!", cboA4Printer);
                    hasError = true;
                }
            }
            else if (rdoSlipPrinter.Checked)
            {
                if (cboSlipPrinter.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(cboSlipPrinter, "Error");
                    tp.Show("Please select Slip Printer!", cboSlipPrinter);
                    hasError = true;
                }
            }
            else if (txtNoOfCopy.Text.Trim() == string.Empty || Convert.ToInt32(txtNoOfCopy.Text) == 0)
            {
                tp.SetToolTip(txtNoOfCopy, "Error");
                tp.Show("Please browser Logo!", txtNoOfCopy);
                hasError = true;
            }
     
            if (!hasError)
            {

                DefaultPrinter.BarcodePrinter = cboBarcodePrinter.Text;
                DefaultPrinter.A4Printer = cboA4Printer.Text;
                DefaultPrinter.SlipPrinter = cboSlipPrinter.Text;

                SettingController.ShopName = txtShopName.Text;
                SettingController.BranchName = txtBranchName.Text;
                SettingController.PhoneNo = txtPhoneNo.Text;
                SettingController.OpeningHours = txtOpeningHours.Text;
                SettingController.Company_StartDate = dpCompanyStartDate.Value.ToString();
                SettingController.DefaultTaxRate = cboTaxList.SelectedValue.ToString();
                SettingController.UseStockAutoGenerate = chkUseStockAutoGenerate.Checked;
                SettingController.FooterPage = txtFooterPage.Text;
                SettingController.DefaultNoOfCopies = Convert.ToInt32(txtNoOfCopy.Text);

                try
                {
                    File.Copy(txtImagePath.Text, Application.StartupPath + "\\logo\\" + FilePath);
                    SettingController.Logo = "~\\logo\\" + FilePath;
                }
                catch
                {
                    if (FilePath != null)
                    {
                        SettingController.Logo = "~\\logo\\" + FilePath;
                    }
                }


                if (rdoA4Printer.Checked)
                {
                    SettingController.SelectDefaultPrinter = rdoA4Printer.Text.ToString();
                }
                else
                {
                    SettingController.SelectDefaultPrinter = rdoSlipPrinter.Text.ToString();
                }


                int Id = Convert.ToInt32(cboCurrency.SelectedValue);
                SettingController.DefaultCurrency = Id;

                int topcity = 0;
                Int32.TryParse(cboCity.SelectedValue.ToString(), out topcity);
                SettingController.DefaultCity = topcity;
                int topSalesRow = 0;
                Int32.TryParse(txtDefaultSalesRow.Text, out topSalesRow);
                SettingController.DefaultTopSaleRow = topSalesRow;

                int memberValidityYear = 0;
                Int32.TryParse(txtMemberValidityYear.Text, out memberValidityYear);
                SettingController.DefaultMemberValidityYear = memberValidityYear;

                List<APP_Data.Shop> shopList = entity.Shops.Where(x => x.IsDefaultShop == true).ToList();
                (from x in entity.Shops where x.IsDefaultShop == true select x).ToList().ForEach(x=> x.IsDefaultShop = false);

                entity.SaveChanges();

                int defaultShopId = Convert.ToInt32(cboShopList.SelectedValue);
                APP_Data.Shop defaultShop = entity.Shops.Where(x => x.Id == defaultShopId).FirstOrDefault();
                defaultShop.IsDefaultShop = true;
                entity.SaveChanges();


                bool previousState = SettingController.DisabledImportExport;
                SettingController.DisabledImportExport = chkSAP.Checked;

                MessageBox.Show("Successfully Saved!");
                this.Dispose();

                if (previousState != chkSAP.Checked)
                {
                    Application.Restart();

                }

            }
        }

        private void btnNewTax_Click(object sender, EventArgs e)
        {
            Taxes newForm = new Taxes();
            newForm.ShowDialog();
        }

        private void btnNewCity_Click(object sender, EventArgs e)
        {
            City newForm = new City();
            newForm.ShowDialog();
        }

        #region Function
        public void ReLoadCity()
        {
            #region city
            List<APP_Data.City> cityList = entity.Cities.ToList();
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";
            if (SettingController.DefaultCity != null)
            {
                int id = Convert.ToInt32(SettingController.DefaultCity);
                APP_Data.City city = entity.Cities.Where(x => x.Id == id).FirstOrDefault();
                cboCity.Text = city.CityName;
            }
            #endregion
        }

        public void SetCurrentCity(Int32 CityId)
        {
            APP_Data.City currentCity = entity.Cities.Where(x => x.Id == CityId).FirstOrDefault();
            if (currentCity != null)
            {
                cboCity.SelectedValue = currentCity.Id;
            }
        }

        public void ReloadTax()
        {
            entity = new POSEntities();
            #region taxPercent
            List<Tax> taxList = entity.Taxes.ToList();
            List<Tax> result = new List<Tax>();
            foreach (Tax r in taxList)
            {
                Tax t = new Tax();
                t.Id = r.Id;
                t.Name = r.Name + " and " + r.TaxPercent;
                t.TaxPercent = r.TaxPercent;
                result.Add(t);
            }
            cboTaxList.DataSource = result;
            cboTaxList.DisplayMember = "Name";
            cboTaxList.ValueMember = "Id";
            if (SettingController.DefaultTaxRate != null)
            {
                int id = Convert.ToInt32(SettingController.DefaultTaxRate);
                Tax defaultTax = (from t in entity.Taxes where t.Id == id select t).FirstOrDefault();
                cboTaxList.Text = defaultTax.Name + " and " + defaultTax.TaxPercent;
            }
            #endregion
        }


        public void SetCurrentTax(Int32 TaxId)
        {
            APP_Data.Tax currentTax = entity.Taxes.Where(x => x.Id == TaxId).FirstOrDefault();
            if (currentTax != null)
            {
                cboTaxList.SelectedValue = currentTax.Id;
            }
        }
        #endregion

        private void Setting_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtShopName);
            tp.Hide(txtBranchName);
            tp.Hide(txtPhoneNo);
            tp.Hide(txtOpeningHours);
            tp.Hide(dpCompanyStartDate);
            tp.Hide(cboA4Printer);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Picture";
            dlg.Filter = "JPEGs (*.jpg;*.jpeg;*.jpe) |*.jpg;*.jpeg;*.jpe |GIFs (*.gif)|*.gif|PNGs (*.png)|*.png";

            try
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtImagePath.Text = dlg.FileName;
                    ptImage.SizeMode = PictureBoxSizeMode.StretchImage;
                    ptImage.ImageLocation = txtImagePath.Text;
                    FilePath = System.IO.Path.GetFileName(dlg.FileName);

                }

            }
            catch (Exception ex)
            {
                //MessageBox.ShowMessage(Globalizer.MessageType.Warning, "You have to select Picture.\n Try again!!!");
                MessageBox.Show("You have to select Picture.\n Try again!!!");
                throw ex;
            }
        }

        private void cboShopList_SelectedIndexChanged(object sender, EventArgs e)
        {
            APP_Data.Shop currentShop = (APP_Data.Shop)cboShopList.SelectedItem;
            txtShopName.Text = currentShop.ShopName;
            txtBranchName.Text = currentShop.Address;
            txtOpeningHours.Text = currentShop.OpeningHours;
            txtPhoneNo.Text = currentShop.PhoneNumber;
            lblCity.Text = currentShop.City.CityName;
        }

        // By SYM
        private void txtMemberValidityYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
