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

namespace POS
{
    public partial class NewCustomerReport : Form
    {


        #region variable
        APP_Data.POSEntities entity = new APP_Data.POSEntities();
        List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
        List<CustomerReportNew> CustomerReportListNew = new List<CustomerReportNew>();
        string status = "All";
        Boolean isstart = false;
        Boolean btnclick = false;
        string customername;
        int age = 0;
        decimal purchase = 0;
        #endregion

        #region Event
        public NewCustomerReport()
        {
            InitializeComponent();
        }

        private void NewCustomerReport_Load(object sender, EventArgs e)
        {
            cboActiveNonActive.SelectedIndex = 0;
            Bind_MemberType();
            VisbleByRadio();
            isstart = true;
            LoadData();
        }

        private void rdoCustomerName_CheckedChanged(object sender, EventArgs e)
        {
            VisbleByRadio();
        }

        private void rdoAge_CheckedChanged(object sender, EventArgs e)
        {
            VisbleByRadio();
        }

        private void rdoPurchase_CheckedChanged(object sender, EventArgs e)
        {
            VisbleByRadio();
        }

        private void cboMemberType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
            txtEnterValue.Text = string.Empty;
            txtSearch.Text = string.Empty;
            cboAgeOP.SelectedIndex = 0;
        }

        private void cboActiveNonActive_SelectedChanged(object sender, EventArgs e)
        {
            status = cboActiveNonActive.Text;
            LoadData();
            txtEnterValue.Text = string.Empty;
            txtSearch.Text = string.Empty;
            cboAgeOP.SelectedIndex = 0;
        }

        #endregion



        #region Method
        private void Bind_MemberType()
        {
            List<APP_Data.MemberType> mTypeList = new List<APP_Data.MemberType>();
            APP_Data.MemberType mType = new APP_Data.MemberType();
            mType.Id = 0;
            mType.Name = "All";
            mTypeList.Add(mType);
            mTypeList.AddRange(entity.MemberTypes.Where(x => x.IsDelete == false).ToList());
            cboMemberType.DataSource = mTypeList;
            cboMemberType.DisplayMember = "Name";
            cboMemberType.ValueMember = "Id";
        }

        private void VisbleByRadio()
        {
            lblage.Visible = rdoAge.Checked;
            lblCustomername.Visible = rdoCustomerName.Checked;
            lblpurchase.Visible = rdoPurchase.Checked;
            txtSearch.Text = string.Empty;
            txtEnterValue.Text = string.Empty;
            txtSearch.Visible = rdoCustomerName.Checked;
            cboAgeOP.SelectedIndex = 0;
            cboAgeOP.Visible = rdoAge.Checked || rdoPurchase.Checked;

            txtEnterValue.Visible = rdoAge.Checked || rdoPurchase.Checked;
        }


        private void LoadData()
        {
            customerList.Clear();
            CustomerReportListNew.Clear();

            if (isstart == true)
            {

                DateTime subdate = DateTime.Now.AddYears(-1);

                if (cboMemberType.SelectedIndex == 0)
                {

                    var CustomerReportList = (from c in entity.Customers

                                              join t in entity.Transactions on c.Id equals t.CustomerId into ct
                                              from cct in ct.DefaultIfEmpty()
                                              group cct by new { c.Id, c.Name, c.Birthday, c.PhoneNumber, c.Address, c.MemberTypeID, c.PromoteDate } into g
                                              select new
                                              {
                                                  id = g.Key.Id,
                                                  Name = g.Key.Name,
                                                  Phone = g.Key.PhoneNumber,
                                                  Address = g.Key.Address,
                                                  Date = g.Key.PromoteDate,
                                                  Age = g.Key.Birthday != null ? DateTime.Now.Year - EntityFunctions.TruncateTime((DateTime)g.Key.Birthday).Value.Year : 0,
                                                  MemberType = (from p in entity.MemberTypes where p.Id == g.Key.MemberTypeID select p.Name).FirstOrDefault(),
                                                  ActiveNonActive = (from p in entity.Transactions where p.CustomerId == g.Key.Id && EntityFunctions.TruncateTime((DateTime)p.DateTime) >= subdate select p).Count() >= 3 ? "Active" : "NonActive",
                                                  TotalPurchase = g.Where(s => s.IsDeleted == false).Sum(x => x.TotalAmount),
                                                  AveragePerVouncher = g.Where(s => s.IsDeleted == false).Sum(x => x.TotalAmount) / g.Count()
                                              }
                                                 ).ToList();
                    if (status == "All")
                    {
                        foreach (var c in CustomerReportList)
                        {
                            CustomerReportNew customer = new CustomerReportNew();
                            ELC_CustomerPointSystem.Get_ExpiredMemberList_And_Update_ExpiredMember(c.id);
                            customer.Name = c.Name;
                            customer.Phone = c.Phone != null ? c.Phone : " ";
                            customer.Address = c.Address != null ? c.Address : " ";
                            customer.ActiveNonActive = c.ActiveNonActive;
                            //customer.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToShortDateString() : " ";
                            customer.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToString("dd-MM-yyyy") : " ";
                            customer.Age = c.Age.ToString();
                            customer.MemberType = c.MemberType != null ? c.MemberType.ToString() : "Not VIP";
                            customer.totalPurchase = c.TotalPurchase != null ? Convert.ToInt64(c.TotalPurchase) : 0;
                            customer.AvgAmountPerVouncher = c.AveragePerVouncher != null ? Convert.ToDecimal(c.AveragePerVouncher) : 0;
                            CustomerReportListNew.Add(customer);

                        }
                    }
                    else
                    {
                        foreach (var c in CustomerReportList)
                        {
                            if (c.ActiveNonActive == status)
                            {
                                CustomerReportNew customer = new CustomerReportNew();
                                customer.Name = c.Name;
                                customer.Phone = c.Phone != null ? c.Phone : " ";
                                customer.Address = c.Address != null ? c.Address : " ";
                                customer.ActiveNonActive = c.ActiveNonActive;
                                //customer.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToShortDateString() : " ";
                                customer.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToString("dd-MM-yyyy") : " ";
                                customer.Age = c.Age.ToString();
                                customer.MemberType = c.MemberType != null ? c.MemberType.ToString() : "Not Member";
                                customer.totalPurchase = c.TotalPurchase != null ? Convert.ToInt64(c.TotalPurchase) : 0;
                                customer.AvgAmountPerVouncher = c.AveragePerVouncher != null ? Convert.ToDecimal(c.AveragePerVouncher) : 0;
                                CustomerReportListNew.Add(customer);
                            }
                        }
                    }

                }
                else
                {
                    int test = Convert.ToInt32(cboMemberType.SelectedValue);
                    var CustomerReportList = (from c in entity.Customers.Where(z => z.MemberTypeID == test)

                                              join t in entity.Transactions on c.Id equals t.CustomerId into ct
                                              from cct in ct.DefaultIfEmpty()

                                              group cct by new { c.Id, c.Name, c.Birthday, c.PhoneNumber, c.Address, c.MemberTypeID, c.PromoteDate } into g
                                              select new
                                              {
                                                  Name = g.Key.Name,
                                                  Phone = g.Key.PhoneNumber,
                                                  Address = g.Key.Address,
                                                  Date = g.Key.PromoteDate,
                                                  Age = g.Key.Birthday != null ? DateTime.Now.Year - EntityFunctions.TruncateTime((DateTime)g.Key.Birthday).Value.Year : 0,
                                                  MemberType = (from p in entity.MemberTypes where p.Id == g.Key.MemberTypeID select p.Name).FirstOrDefault(),
                                                  ActiveNonActive = (from p in entity.Transactions where p.CustomerId == g.Key.Id && EntityFunctions.TruncateTime((DateTime)p.DateTime) >= subdate select p).Count() >= 3 ? "Active" : "NonActive",
                                                  TotalPurchase = g.Where(s => s.IsDeleted == false).Sum(x => x.TotalAmount),
                                                  AveragePerVouncher = g.Where(s => s.IsDeleted == false).Sum(x => x.TotalAmount) / g.Count()
                                              }
                                                ).ToList();
                    if (status == "All")
                    {
                        foreach (var c in CustomerReportList)
                        {
                            CustomerReportNew customer = new CustomerReportNew();
                            customer.Name = c.Name;
                            customer.Phone = c.Phone != null ? c.Phone : " ";
                            customer.Address = c.Address != null ? c.Address : " ";
                            customer.ActiveNonActive = c.ActiveNonActive;
                            //customer.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToShortDateString() : " ";
                            customer.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToString("dd-MM-yyyy") : " ";
                            customer.Age = c.Age.ToString();
                            customer.MemberType = c.MemberType != null ? c.MemberType.ToString() : "Not VIP";
                            customer.totalPurchase = c.TotalPurchase != null ? Convert.ToInt64(c.TotalPurchase) : 0;
                            customer.AvgAmountPerVouncher = c.AveragePerVouncher != null ? Convert.ToDecimal(c.AveragePerVouncher) : 0;
                            CustomerReportListNew.Add(customer);

                        }
                    }
                    else
                    {
                        foreach (var c in CustomerReportList)
                        {
                            if (c.ActiveNonActive == status)
                            {
                                CustomerReportNew customer = new CustomerReportNew();
                                customer.Name = c.Name;
                                customer.Phone = c.Phone != null ? c.Phone : " ";
                                customer.Address = c.Address != null ? c.Address : " ";
                                customer.ActiveNonActive = c.ActiveNonActive;
                                //customer.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToShortDateString() : " ";
                                customer.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToString("dd-MM-yyyy") : " ";
                                customer.Age = c.Age.ToString();
                                customer.MemberType = c.MemberType != null ? c.MemberType.ToString() : "Not VIP";
                                customer.totalPurchase = c.TotalPurchase != null ? Convert.ToInt64(c.TotalPurchase) : 0;
                                customer.AvgAmountPerVouncher = c.AveragePerVouncher != null ? Convert.ToDecimal(c.AveragePerVouncher) : 0;
                                CustomerReportListNew.Add(customer);
                            }
                        }
                    }
                }
                if (btnclick == true)
                {
                    if (customername != null && customername != string.Empty)
                    {
                        CustomerReportListNew = CustomerReportListNew.Where(x => x.Name == customername).ToList();



                    }
                    else if (age != 0)
                    {
                        switch (cboAgeOP.Text)
                        {
                            case "=": CustomerReportListNew = CustomerReportListNew.Where(x => int.Parse(x.Age) == age).ToList(); break;
                            case ">": CustomerReportListNew = CustomerReportListNew.Where(x => int.Parse(x.Age) > age).ToList(); break;
                            case "<": CustomerReportListNew = CustomerReportListNew.Where(x => int.Parse(x.Age) < age).ToList(); break;
                            case "<=": CustomerReportListNew = CustomerReportListNew.Where(x => int.Parse(x.Age) <= age).ToList(); break;
                            case ">=": CustomerReportListNew = CustomerReportListNew.Where(x => int.Parse(x.Age) >= age).ToList(); break;
                            default: break;

                        }



                    }
                    else if (purchase != 0)
                    {
                        switch (cboAgeOP.Text)
                        {
                            case "=": CustomerReportListNew = CustomerReportListNew.Where(x => x.totalPurchase == purchase).ToList(); break;
                            case ">": CustomerReportListNew = CustomerReportListNew.Where(x => x.totalPurchase > purchase).ToList(); break;
                            case "<": CustomerReportListNew = CustomerReportListNew.Where(x => x.totalPurchase < purchase).ToList(); break;
                            case "<=": CustomerReportListNew = CustomerReportListNew.Where(x => x.totalPurchase <= purchase).ToList(); break;
                            case ">=": CustomerReportListNew = CustomerReportListNew.Where(x => x.totalPurchase >= purchase).ToList(); break;
                            default: break;

                        }

                    }
                }

                ShowReportViewer();
                clear();

            }

        }

        private void clear()
        {
            btnclick = false;

            customername = string.Empty;
            age = 0;
            purchase = 0;
        }
        private void ShowReportViewer()
        {
            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.NewCustomerReportDataTable dtOCusReport = (dsReportTemp.NewCustomerReportDataTable)dsReport.Tables["NewCustomerReport"];

            foreach (CustomerReportNew c in CustomerReportListNew)
            {
                dsReportTemp.NewCustomerReportRow newRow = dtOCusReport.NewNewCustomerReportRow();
                newRow.Name = c.Name;
                newRow.Phone = c.Phone;
                newRow.Address = c.Address;
                newRow.Date = c.Date;
                //newRow.Date = c.Date != null ? Convert.ToDateTime(c.Date).ToString("dd-MM-yyyy") : " ";
                newRow.MemberType = c.MemberType.ToString();
                newRow.Age = c.Age;
                newRow.TotalPurchase = c.totalPurchase;
                newRow.ActiveNotActive = c.ActiveNonActive;
                newRow.AverageTransactionPerVouncher = c.AvgAmountPerVouncher;

                dtOCusReport.AddNewCustomerReportRow(newRow);
            }

            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["NewCustomerReport"]);
            // ReportDataSource rds = new ReportDataSource("DataSet1", _reportCustomerList);
            string reportPath = string.Empty;
            reportPath = Application.StartupPath + "\\Reports\\NewCustomerReport.rdlc";

            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.RefreshReport();




        }
        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnclick = true;
            if (rdoCustomerName.Checked == true)
            {
                customername = txtSearch.Text;
            }
            else if (rdoAge.Checked == true)
            {
                if (txtEnterValue.Text == string.Empty || txtEnterValue.Text == "")
                {
                    age = 0;
                }
                else
                {
                    age = Convert.ToInt32(txtEnterValue.Text);
                }
            }
            else if (rdoPurchase.Checked == true)
            {
                if (txtEnterValue.Text == string.Empty || txtEnterValue.Text == "")
                {
                    purchase = 0;
                }
                else
                {
                    purchase = Convert.ToInt32(txtEnterValue.Text);
                }
            }
            LoadData();
        }

        private void txtEnterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }




    }
}
