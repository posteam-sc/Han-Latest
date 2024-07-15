using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Objects;
using POS.APP_Data;
using Microsoft.Reporting.WinForms;
using System.Linq.Expressions;

namespace POS.View.Reports
{
    public partial class TransactionUsingCouponCodeReport : Form
    {
        #region variable
        APP_Data.POSEntities entity = new APP_Data.POSEntities();
        //List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
        List<CouponCodeReportNew> CouponCodeReportList = new List<CouponCodeReportNew>();
        string status = "All";
        Boolean isstart = false;
        Boolean btnclick = false;
        string coupouCodeNo;
        int amount = 0;
        decimal purchase = 0;
        #endregion

        public TransactionUsingCouponCodeReport()
        {
            InitializeComponent();
        }

        private void CouponCodeReport_Load(object sender, EventArgs e)
        {
            VisbleByRadio();
            this.cboAmountOP.SelectedIndex = 0;
            LoadData();
            this.reportViewer1.RefreshReport();
        }

        private void rdoCouponCode_CheckedChanged(object sender, EventArgs e)
        {
            VisbleByRadio();
        }

        private void rdoAmount_CheckedChanged(object sender, EventArgs e)
        {
            VisbleByRadio();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnclick = true;
            if (rdoCouponCode.Checked == true)
            {
                coupouCodeNo = txtCouponCode.Text;
            }
            else if (rdoAmount.Checked == true)
            {
                if (txtAmount.Text == string.Empty || txtAmount.Text == "")
                {
                    amount = 0;
                }
                else
                {
                    amount = Convert.ToInt32(txtAmount.Text);
                }
            }
            LoadData();
        }

        private void clear()
        {
            btnclick = false;

            coupouCodeNo = string.Empty;
            amount = 0;
        }

        private void VisbleByRadio()
        {
            lblamount.Visible = rdoAmount.Checked;
            txtAmount.Visible = rdoAmount.Checked;
            lblCouponCode.Visible = rdoCouponCode.Checked;
            txtCouponCode.Visible = rdoCouponCode.Checked;
            cboAmountOP.Visible = rdoAmount.Checked;
            txtCouponCode.Text = string.Empty;
            txtAmount.Text = string.Empty;
            this.cboAmountOP.SelectedIndex = 0;
        }

        private void LoadData()
        {
            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date;
            int totalAmount = 0;
            CouponCodeReportList.Clear();

            var CouponCodeList = (from t in entity.Transactions
                                  join c in entity.CouponCodes on t.CouponCodeId equals c.Id
                                  select new
                                  {
                                      DateTime = t.DateTime,
                                      Id = t.Id,
                                      CouponCodeNo = c.CouponCodeNo,
                                      Amount = c.Amount
                                  }).ToList();
            

            foreach (var c in CouponCodeList)
            {
                CouponCodeReportNew couponCodeReport = new CouponCodeReportNew();
                couponCodeReport.Date = c.DateTime != null ? Convert.ToDateTime(c.DateTime).ToShortDateString() : " ";
                couponCodeReport.TransactionId = c.Id;
                couponCodeReport.CouponCode = c.CouponCodeNo;
                couponCodeReport.Amount = Convert.ToInt32(c.Amount);
                totalAmount = totalAmount + couponCodeReport.Amount;
                couponCodeReport.TotalAmount = totalAmount;
                CouponCodeReportList.Add(couponCodeReport);
            }

            CouponCodeReportList = CouponCodeReportList.Where(x => Convert.ToDateTime(x.Date) >= fromDate && Convert.ToDateTime(x.Date) <= toDate).ToList();

            if (btnclick == true)
            {
                if (!string.IsNullOrEmpty(coupouCodeNo))
                {
                    CouponCodeReportList = CouponCodeReportList.Where(x => x.CouponCode == coupouCodeNo).ToList();
                }
                else if (amount != 0)
                {
                    switch (cboAmountOP.Text)
                    {
                        case "=": CouponCodeReportList = CouponCodeReportList.Where(x => x.Amount == amount).ToList(); break;
                        case ">": CouponCodeReportList = CouponCodeReportList.Where(x => x.Amount > amount).ToList(); break;
                        case "<": CouponCodeReportList = CouponCodeReportList.Where(x => x.Amount < amount).ToList(); break;
                        case "<=": CouponCodeReportList = CouponCodeReportList.Where(x => x.Amount <= amount).ToList(); break;
                        case ">=": CouponCodeReportList = CouponCodeReportList.Where(x => x.Amount >= amount).ToList(); break;
                        default: break;
                    }

                }
            }
            ShowReportViewer();
            clear();
        }

        private void ShowReportViewer()
        {
            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.TransactionUsingCouponCodeReportDataTable dtOCusReport = (dsReportTemp.TransactionUsingCouponCodeReportDataTable)dsReport.Tables["TransactionUsingCouponCodeReport"];
            int totalAmount = 0;
            foreach (CouponCodeReportNew c in CouponCodeReportList)
            {
                dsReportTemp.TransactionUsingCouponCodeReportRow newRow = dtOCusReport.NewTransactionUsingCouponCodeReportRow();
                newRow.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToString("dd-MM-yyyy") : " ";
                newRow.TransactionId = c.TransactionId;
                newRow.CouponCodeNo = c.CouponCode;
                newRow.Amount = Convert.ToString(c.Amount);
                totalAmount = totalAmount + c.Amount;
                newRow.TotalAmount = Convert.ToString(totalAmount);
                dtOCusReport.AddTransactionUsingCouponCodeReportRow(newRow);
            }

            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["TransactionUsingCouponCodeReport"]);
            // ReportDataSource rds = new ReportDataSource("DataSet1", _reportCustomerList);
            string reportPath = string.Empty;
            reportPath = Application.StartupPath + "\\Reports\\TransactionUsingCouponCodeReport.rdlc";

            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            ReportParameter Date = new ReportParameter("Date", "From " + dtpFrom.Value.Date.ToString("dd-MM-yyyy") + " To " + dtpTo.Value.Date.ToString("dd-MM-yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);
            reportViewer1.RefreshReport();
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
