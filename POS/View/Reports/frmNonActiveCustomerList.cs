using Microsoft.Reporting.WinForms;
using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class frmNonActiveCustomerList : Form
    {
        public frmNonActiveCustomerList()
        {
            InitializeComponent();
        }

        private void frmNonActiveCustomerList_Load(object sender, EventArgs e)
        {
            bindMemberType();
            LoadActiveCustomer();
            LoadData();
        }
        #region Class 
        private class MemberCustomer
        {

            public string PhoneNumber { get; set; }

            public string MemberTypeName { get; set; }

            public string MemberCode { get; set; }

            public DateTime LastPurchaseDate { get; set; }

            public int TotalAmount { get; set; }

            public string CustomerCode { get; set; }
        }

        private class NonActiveCustomer
        {
            public string Name { get; set; }
            public string MemberType { get; set; }
            public DateTime LastPaidDate { get; set; }
            public string PhoneNumber { get; set; }
            public int TotalAmount { get; set; }
            public string CustomerCode { get; set; }
        }
        #endregion

        #region Method
        private void bindMemberType()
        {
            POSEntities entity = new POSEntities();
            List<MemberType> memberTypeLsit = new List<MemberType>();
            MemberType memberType = new MemberType();
            memberType.Id = 0;
            memberType.Name = "Select";
            memberTypeLsit.Add(memberType);
            memberTypeLsit.AddRange(entity.MemberTypes.Where(x => x.IsDelete == false).ToList());
            cboMemberType.DataSource = memberTypeLsit;
            cboMemberType.DisplayMember = "Name";
            cboMemberType.ValueMember = "Id";

        }

        private void LoadActiveCustomer()
        {
            POSEntities entity = new POSEntities();

            var memberTransactionList = (from c in entity.Customers
                                         join t in entity.Transactions on c.Id equals t.CustomerId
                                         join m in entity.MemberTypes on c.MemberTypeID equals m.Id
                                         where t.IsActive == true && t.IsDeleted == false && c.MemberType != null
                                         && c.VIPMemberId != " "
                                         group new { c, m, t } by new
                                         {
                                             c.CustomerCode,
                                             c.VIPMemberId,
                                             m.Name,
                                             c.PhoneNumber,

                                         } into cust
                                         select new MemberCustomer
                                         {
                                             PhoneNumber = cust.Key.PhoneNumber,
                                             MemberTypeName = cust.Key.Name,
                                             MemberCode = cust.Key.VIPMemberId,
                                             LastPurchaseDate = cust.Max(x => x.t.DateTime).Value,
                                             TotalAmount = (int)(cust.Sum(x => x.t.TotalAmount) - cust.Sum(x => x.t.GiftCardAmount)),
                                             CustomerCode = cust.Key.CustomerCode
                                         }).ToList();


            foreach (var item in memberTransactionList)
            {
                var vipTransactionList = entity.VipCustomers.Where(x => x.CustomerCode == item.CustomerCode).OrderByDescending(x => x.LastPaidDate).FirstOrDefault();
                if (vipTransactionList != null)
                {
                    if (vipTransactionList.TwoYearsTotalAmount != item.TotalAmount && vipTransactionList.LastPaidDate != item.LastPurchaseDate)
                    {
                        VipCustomer vipCustomer = new VipCustomer();
                        vipCustomer.CustomerCode = item.CustomerCode;
                        vipCustomer.LastPaidDate = item.LastPurchaseDate;
                        vipCustomer.MemberType = item.MemberTypeName;
                        vipCustomer.TwoYearsTotalAmount = item.TotalAmount;
                        vipCustomer.ShopCode = SettingController.DefaultShop.ShortCode;
                        vipCustomer.CreatedUserID = MemberShip.UserId;
                        vipCustomer.CreatedDate = DateTime.Now;
                        entity.VipCustomers.Add(vipCustomer);
                        entity.SaveChanges();

                    }

                }
                else
                {
                    VipCustomer vipCustomer = new VipCustomer();
                    vipCustomer.CustomerCode = item.CustomerCode;
                    vipCustomer.LastPaidDate = item.LastPurchaseDate;
                    vipCustomer.MemberType = item.MemberTypeName;
                    vipCustomer.TwoYearsTotalAmount = item.TotalAmount;
                    vipCustomer.ShopCode = SettingController.DefaultShop.ShortCode;
                    vipCustomer.CreatedUserID = MemberShip.UserId;
                    vipCustomer.CreatedDate = DateTime.Now;
                    entity.VipCustomers.Add(vipCustomer);
                    entity.SaveChanges();
                }
            }
        }



        private void LoadData()
        {
            POSEntities entity = new POSEntities();
            var lastTwoYearDate = DateTime.Now.AddYears(-2);
            List<NonActiveCustomer> result = new List<NonActiveCustomer>();
            List<NonActiveCustomer> customerList = new List<NonActiveCustomer>();
            

            if (rdoActiveCustomer.Checked)
            {
                result = (from v in entity.VipCustomers
                          join c in entity.Customers on v.CustomerCode equals c.CustomerCode
                          where v.LastPaidDate >= lastTwoYearDate && v.LastPaidDate <= DateTime.Now
                          group new { c, v } by new
                          {
                              c.Name,  
                              v.MemberType,                                                                                
                              c.VIPMemberId,
                              c.PhoneNumber,                        

                          } into cust
                          select new NonActiveCustomer
                          {
                              Name = cust.Key.Name,
                              LastPaidDate = cust.Max(x => x.v.LastPaidDate),
                              MemberType =cust.Key.MemberType,
                              PhoneNumber = cust.Key.PhoneNumber,
                              TotalAmount =(int)cust.Max(x=>x.v.TwoYearsTotalAmount),
                              CustomerCode = cust.Key.VIPMemberId

                          }).OrderBy(x => x.Name).ToList(); 
            }
            else
            {
                result = (from v in entity.VipCustomers
                          join c in entity.Customers on v.CustomerCode equals c.CustomerCode
                          where v.LastPaidDate < lastTwoYearDate
                          group new { c, v } by new
                          {
                              c.Name,
                              v.MemberType,
                              c.VIPMemberId,
                              c.PhoneNumber,

                          } into cust
                          select new NonActiveCustomer
                          {
                              Name = cust.Key.Name,
                              LastPaidDate = cust.Max(x => x.v.LastPaidDate),
                              MemberType = cust.Key.MemberType,
                              PhoneNumber = cust.Key.PhoneNumber,
                              TotalAmount = (int)cust.Max(x => x.v.TwoYearsTotalAmount),
                              CustomerCode = cust.Key.VIPMemberId

                          }).OrderBy(x => x.Name).ToList(); ;
            }



            if (cboMemberType.SelectedIndex > 0 && String.IsNullOrWhiteSpace(txtCustomerCode.Text))
            {
                customerList = result.Where(x => x.MemberType == cboMemberType.Text).ToList();
            }
            else if (cboMemberType.SelectedIndex == 0 && !String.IsNullOrWhiteSpace(txtCustomerCode.Text))
            {
                customerList = result.Where(x => x.CustomerCode == txtCustomerCode.Text).ToList();
            }
            else
            {
                customerList = result;
            }
            if (customerList.Count > 0)
            {
                List<NonActiveCustomer> nonActiveCustomerForClass = new List<NonActiveCustomer>();
                foreach (var item in customerList)
                {
                    var nonActiveCustomer = new NonActiveCustomer();
                    nonActiveCustomer.Name = item.Name;
                    nonActiveCustomer.MemberType = item.MemberType;
                    nonActiveCustomer.CustomerCode = item.CustomerCode;
                    nonActiveCustomer.PhoneNumber = item.PhoneNumber;
                    nonActiveCustomer.TotalAmount = item.TotalAmount;
                    nonActiveCustomer.LastPaidDate = item.LastPaidDate;
                    nonActiveCustomerForClass.Add(nonActiveCustomer);
                }
                reportViewer1.Visible = true;
                ReportDataSource rds = new ReportDataSource();
                rds.Name = "NonActiveCustomer";
                rds.Value = nonActiveCustomerForClass;
                string reportPath = Application.StartupPath + @"\Reports\NonActiveCustomerList.rdlc";
                reportViewer1.LocalReport.ReportPath = reportPath;
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);
                ReportParameter Count = new ReportParameter("Count", customerList.Count.ToString());
                reportViewer1.LocalReport.SetParameters(Count);
                if (rdoActiveCustomer.Checked)
                {
                    ReportParameter Title = new ReportParameter("Title", "Active Customer List");
                    reportViewer1.LocalReport.SetParameters(Title);
                }
                else
                {
                    ReportParameter Title = new ReportParameter("Title", "Non-Active Customer List");
                    reportViewer1.LocalReport.SetParameters(Title);
                }

                reportViewer1.RefreshReport();
            }
            else
            {
                MessageBox.Show("There is No Data");
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.RefreshReport();
            }


        }

        private void InitialState()
        {
            cboMemberType.SelectedIndex = 0;
            txtCustomerCode.Clear();
        }
        #endregion

        #region Button Click
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            InitialState();
            LoadData();
        }
        #endregion

        #region Check Change
        private void rdoNonActiveCustomer_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdoActiveCustomer_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion
    }
}
