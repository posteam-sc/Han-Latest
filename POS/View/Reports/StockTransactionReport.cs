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
    public partial class StockTransactionReport : Form
    {
        public StockTransactionReport()
        {
            InitializeComponent();
        }

        #region Variable
        POSEntities entity = new POSEntities();
        int _month, _year;
        bool _start = false;
        string _tranDate;
        List<Product> _nonConProductList;
        List<string> productIdList;
        DateTime companyStartDate;
        List<object> _dataResult = new List<object>();
        string monthName = "";
        
        #endregion

        #region Event
        private void StockTransactionReport_Load(object sender, EventArgs e)
        {
            try
            {
                ////cboMonth.Text = DateTime.Now.ToString("MMMM");
                ////Year_Bind();
                ////Month();
                ////_year = Convert.ToInt32(cboYear.Text);
                ////_tranDate = Month_Name(_month, _year);

                var startYear = Convert.ToDateTime(SettingController.Company_StartDate).Year;
                cboYear.DataSource = Enumerable.Range(startYear, 100).ToList();
                cboYear.Text = DateTime.Now.Year.ToString();
                cboMonth.Text = DateTime.Now.ToString("MMMM");
                Month();
                _year = Convert.ToInt32(cboYear.SelectedValue);
                _tranDate = Month_Name(_month, _year);
                _start = true;
                Data_Bind();
            }
            catch
            {
            }
        }

        private void cboMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            ////Month();
            ////_year = Convert.ToInt32(cboYear.Text);
            ////_tranDate = Month_Name(_month, _year);
           
            Data_Bind();
        }
        #endregion

        #region Function

        private void Year_Bind()
        {
            int startYear = DateTime.Now.Year;
            ////for (int i = startYear; i <= 2030; i++)
            ////{
            ////    cboYear.Items.Add(i);
            ////}
            cboYear.DataSource = Enumerable.Range(startYear, 100).ToList();
            cboYear.SelectedIndex = 0;
        }

        private void Compare_Company_StartDate()
        {
             companyStartDate=Convert.ToDateTime(SettingController.Company_StartDate);
            if (_year >= companyStartDate.Year && _month > companyStartDate.Month)
            {
                _start = true;
            }
        }

        private int? Opening_Balance()
        {
            int? OB=0;
            if (_year > companyStartDate.Year && _month==1)
            {

                var _result = (from e in entity.StockTransactions where (e.Year <= _year ) select e).OrderByDescending(x => x.Year).FirstOrDefault();

                if (_result != null)
                {
                    OB = Convert.ToInt32(_result.Purchase + _result.Consignment + _result.Refund -  +  _result.AdjustmentStockIn +  _result.Sale - _result.AdjustmentStockOut);
                }
            }
            else 
            {
                var _result = (from e in entity.StockTransactions where (e.Year <= _year && e.Month < _month) select e).OrderByDescending(x => x.Year).FirstOrDefault();

                if (_result != null)
                {
                    OB = Convert.ToInt32(_result.Purchase + _result.Consignment + _result.Refund + _result.AdjustmentStockIn - _result.Sale - _result.AdjustmentStockOut);
                }
                
            }

          
            return OB;
        }

        private void Data_Bind()
        {

            if (_start == true)
            {
                monthName = cboMonth.Text;
                _month = DateTime.ParseExact(monthName, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                _year = Convert.ToInt32(cboYear.Text);
                _tranDate = Month_Name(_month, _year);

                IQueryable<object> q = (from e in entity.StockTransactions
                                        join p in entity.Products on e.ProductId equals p.Id
                                        where e.TranDate == _tranDate
                                        orderby p.IsConsignment, e.ProductId
                                        select new
                                        {
                                            ProductId = e.ProductId,
                                            ProductName = p.Name,
                                            ProductCode = p.ProductCode,
                                            Purchase = e.Purchase,
                                            Refund = e.Refund,
                                            Sale = e.Sale,
                                            AdjustmentStockIn = e.AdjustmentStockIn,
                                            AdjustmentStockOut = e.AdjustmentStockOut,
                                            Consignment = e.Consignment,
                                            ConversionStockIn= e.ConversionStockIn,
                                            ConversionStockOut=e.ConversionStockOut,
                                            StockIn=e.StockIn,
                                            StockOut=e.StockOut,
                                            // Opening = Opening_Balance()
                                            Opening = e.Opening
                                        });
                _dataResult = new List<object>(q);

                #region New
                ReportDataSource rds = new ReportDataSource();
                rds.Name = "StockTransactionDataSet";
                rds.Value = _dataResult;
                #endregion

                string reportPath = Application.StartupPath + "\\Reports\\Stock Transaction Report.rdlc";

                reportViewer1.LocalReport.ReportPath = reportPath;
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);

                ReportParameter Month = new ReportParameter("Month", cboMonth.Text.ToString());
                reportViewer1.LocalReport.SetParameters(Month);

                ReportParameter CompanyStartDate = new ReportParameter("CompanyStartDate", Convert.ToDateTime(SettingController.Company_StartDate).ToString("dd-MM-yyyy"));
                reportViewer1.LocalReport.SetParameters(CompanyStartDate);

                this.reportViewer1.ZoomMode = ZoomMode.Percent;
                
                reportViewer1.RefreshReport();
            }
            
        }

        private void Month()
        {
         
            monthName = cboMonth.Text;
            _month = DateTime.ParseExact(monthName, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
            //////////switch (cboMonth.Text)
            //////////{
            //////////    case "Janauary":
            //////////        _month = 1;
            //////////        break;
            //////////    case "February":
            //////////        _month = 2;
            //////////        break;
            //////////    case "March":
            //////////        _month = 3;
            //////////        break;
            //////////    case "April":
            //////////        _month = 4;
            //////////        break;
            //////////    case "May":
            //////////        _month = 5;
            //////////        break;
            //////////    case "June":
            //////////        _month = 6;
            //////////        break;
            //////////    case "July":
            //////////        _month = 7;
            //////////        break;
            //////////    case "August":
            //////////        _month = 8;
            //////////        break;
            //////////    case "September":
            //////////        _month = 9;
            //////////        break;
            //////////    case "October":
            //////////        _month = 10;
            //////////        break;
            //////////    case "November":
            //////////        _month = 11;
            //////////        break;
            //////////    case "December":
            //////////        _month = 12;
            //////////        break;
            //////////}
        }

        public static string Month_Name(int _month, int _year)
        {
            string _tranDate = "";
            switch (_month)
            {
                case 1:
                    _tranDate = "Janauary" + "-" + _year.ToString();
                    break;
                case 2:
                    _tranDate = "February" + "-" + _year.ToString();
                    break;
                case 3:
                    _tranDate = "March" + "-" + _year.ToString();
                    break;
                case 4:
                    _tranDate = "April" + "-" + _year.ToString();
                    break;
                case 5:
                    _tranDate = "May" + "-" + _year.ToString();
                    break;
                case 6:
                    _tranDate = "June" + "-" + _year.ToString();
                    break;
                case 7:
                    _tranDate = "July" + "-" + _year.ToString();
                    break;
                case 8:
                    _tranDate = "August" + "-" + _year.ToString();
                    break;
                case 9:
                    _tranDate = "September" + "-" + _year.ToString();
                    break;
                case 10:
                    _tranDate = "October" + "-" + _year.ToString();
                    break;
                case 11:
                    _tranDate = "November" + "-" + _year.ToString();
                    break;
                case 12:
                    _tranDate = "December" + "-" + _year.ToString();
                    break;
            }
            return _tranDate;
        }
        #endregion

    
    }

}
