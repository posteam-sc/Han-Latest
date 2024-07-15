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
using System.Data.Objects;
using Microsoft.Reporting.WinForms;

namespace POS
{
    public partial class frmNetIncomeReport : Form
    {
        #region Initialized
        public frmNetIncomeReport()
        {
            InitializeComponent();
        }
        #endregion

        #region variable
        POSEntities entity = new POSEntities();
    
        int year = 0;
        int month = 0;
        string monthName = "";
        Boolean IsStart = false;
        #endregion

        private void frmNetIncomeReport_Load(object sender, EventArgs e)
        {

            var startYear = Convert.ToDateTime(SettingController.Company_StartDate).Year;
                cboYear.DataSource = Enumerable.Range(startYear, 100).ToList();
                cboYear.Text = DateTime.Now.Year.ToString();
                cboMonth.Text = DateTime.Now.ToString("MMMM") ;

                Utility.BindShop(cboshoplist);
                cboshoplist.Text = SettingController.DefaultShop.ShopName;
                Utility.ShopComBo_EnableOrNot(cboshoplist);

                IsStart = true;
                LoadData();
        }

        private void LoadData()
        {
            if (IsStart == true)
            {
                try
                {
                    int shopid = Convert.ToInt32(cboshoplist.SelectedValue);
                    string currentshopcode = (from p in entity.Shops where p.Id == shopid select p.ShortCode).FirstOrDefault();
                    string currentshopname = (from p in entity.Shops where p.Id == shopid select p.ShopName).FirstOrDefault();
                    int SaleRevenue = 0, CostofGoodSlod = 0, SalaryExpense = 0, UtilitiesExpense = 0, RentExpense = 0, GeneralExpense = 0;
                    year = Convert.ToInt32(cboYear.SelectedValue);
                    monthName = cboMonth.Text;
                    month = DateTime.ParseExact(monthName, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                #region Sale Revenue
                var SaleList = (from td in entity.TransactionDetails
                                join t in entity.Transactions on td.TransactionId equals t.Id
                                join p in entity.Products on td.ProductId equals p.Id
                                //join pc in entity.ProductCategories on p.ProductCategoryId equals pc.Id
                                where
                             (t.DateTime.Value.Year) == year &&
                            (t.DateTime.Value.Month) == month
                             && t.IsDeleted == false &&  td.IsDeleted== true && t.PaymentTypeId != 4 && t.PaymentTypeId != 6 && t.IsComplete == true
                             && t.IsActive == true && t.Id.Substring(2,2)==currentshopcode
                                select new
                                {
                                    TranId = td.TransactionId,
                                    ParentId = t.ParentId,
                                    Type = t.Type,
                                    TotalAmt = td.TotalAmount

                                }
                                ).ToList();

                if (SaleList.Count > 0)
                {
                    List<string> _type = new List<string> { "Sale","Credit"};
                    var _tranIdList = SaleList.Where(x => _type.Contains(x.Type) ).Select(x => x.TranId).ToList();
                    int _RefundTotalAmt = 0;
                    var _RefundList = SaleList.Where(x => _tranIdList.Contains(x.ParentId) && x.Type == "Refund").ToList();

                    _RefundTotalAmt = _RefundList.Sum(x => Convert.ToInt32(x.TotalAmt));


                    var _salerevenueList = SaleList.Where(x => _type.Contains(x.Type)).Sum(x => x.TotalAmt);
                    SaleRevenue = Convert.ToInt32(_salerevenueList) - Convert.ToInt32(_RefundTotalAmt);

                }
                #endregion

                #region Cost of Good Solds

                var GoodSoldExpList = (from pd in entity.PurchaseDetails
                                       join pdt in entity.PurchaseDetailInTransactions on pd.Id equals pdt.PurchaseDetailId
                                       join td in entity.TransactionDetails on pdt.TransactionDetailId equals td.Id
                                       join t in entity.Transactions on td.TransactionId equals t.Id
                                       join p in entity.Products on pd.ProductId equals p.Id
                                   //join pc in entity.ProductCategories on p.ProductCategoryId equals pc.Id
                                       where (t.DateTime.Value.Year) == year &&
                                      (t.DateTime.Value.Month) == month
                                            && t.IsDeleted == false  && t.IsComplete == true
                                               && t.IsActive == true && pd.IsDeleted == false && td.IsDeleted == false
                                               && t.Id.Substring(2,2)==currentshopcode
                                       select new
                                       {
                                           TotalAmt = (pdt.Qty * pd.UnitPrice),

                                       }
                              ).ToList();

                if (GoodSoldExpList.Count > 0)
                {
                    var _goodSoldTotalAmt = GoodSoldExpList.Sum(x => x.TotalAmt);
                    CostofGoodSlod = Convert.ToInt32(_goodSoldTotalAmt);
                }
                #endregion

                #region "Salary,Utilities,Rent and General  Expenses "
                var ExpenseList = (from e in entity.Expenses
                                   join ec in entity.ExpenseCategories on e.ExpenseCategoryId equals ec.Id
                                   where e.IsApproved == true && e.IsDeleted == false 
                           && (e.ExpenseDate.Value.Year) == year &&
                                 (e.ExpenseDate.Value.Month) == month && e.Id.Substring(2,2)==currentshopcode
                                   select new
                                   {
                                       TotalAmount = e.TotalExpenseAmount,
                                       ExpCag = ec.Name
                                   }
                                   ).ToList();

                if (ExpenseList.Count > 0)
                {
                    //var _applianceFees = ExpenseList.Where(x => x.ExpCag == "Appliance").Sum(x => x.TotalAmount);
                    //ApplianceFees = Convert.ToInt32(_applianceFees);
                    List<string> cagType = new List<string> { "Salary", "Utilities", "Rent" };
                    var _salaryExpenseAmt = ExpenseList.Where(x => x.ExpCag == "Salary").Sum(x => x.TotalAmount);
                    SalaryExpense = Convert.ToInt32(_salaryExpenseAmt);

                    var _utilitiesExpenseAmt = ExpenseList.Where(x => x.ExpCag == "Utilities").Sum(x => x.TotalAmount);
                    UtilitiesExpense = Convert.ToInt32(_utilitiesExpenseAmt);

                    var _rentExpenseAmt = ExpenseList.Where(x => x.ExpCag == "Rent").Sum(x => x.TotalAmount);
                    RentExpense = Convert.ToInt32(_rentExpenseAmt);

                    var _generalExpenseAmt = ExpenseList.Where(x => !cagType.Contains(x.ExpCag)).Sum(x => x.TotalAmount);
                    GeneralExpense = Convert.ToInt32(_generalExpenseAmt);
                }
                #endregion
                #region Assign Data to For DataSet
                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.NetIncomeDataTable dtPdReport = (dsReportTemp.NetIncomeDataTable)dsReport.Tables["NetIncome"];

                dsReportTemp.NetIncomeRow newRow = dtPdReport.NewNetIncomeRow();

                newRow.SalesRevenue = SaleRevenue;
                newRow.CostofGoodSold = CostofGoodSlod;
                newRow.SalaryExpense = SalaryExpense;
                newRow.UtilitiesExpense = UtilitiesExpense;
                newRow.RentExpense = RentExpense;
                newRow.GeneralExpense = GeneralExpense;
                dtPdReport.AddNetIncomeRow(newRow);

                #endregion

                #region Show Report Viewer Part

                ReportDataSource rds = new ReportDataSource("NetIncomeDataSet", dsReport.Tables["NetIncome"]);
                string reportPath = Application.StartupPath + "\\Reports\\Net Income Report.rdlc";

                reportViewer1.LocalReport.ReportPath = reportPath;
                reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.LocalReport.DataSources.Add(rds);

                ReportParameter Month = new ReportParameter("MonthName", cboMonth.Text.ToString());
                reportViewer1.LocalReport.SetParameters(Month);

                ReportParameter ShortName = new ReportParameter("ShopName", currentshopname);
                reportViewer1.LocalReport.SetParameters(ShortName);
                reportViewer1.RefreshReport();

                #endregion
                }
                catch
                {
                }
            }
         
        }

        private void cboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboshoplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
