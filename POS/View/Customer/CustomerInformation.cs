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
    public partial class CustomerInformation : Form
    {
        POSEntities db = new POSEntities();
        List<Customer> preload = new List<Customer>();
        public CustomerInformation()
        {
            InitializeComponent();
        }
        private class Args
        {
            public string TypeArg;
            public string Value;
        }
        private void CustomerInformation_Load(object sender, EventArgs e)
        {
            preload = db.Customers.AsNoTracking().AsEnumerable().ToList();
            VisbleByRadio();
            this.cboMemberType.SelectedIndexChanged -= new EventHandler(cboMemberType_SelectedIndexChanged);
            Bind_MemberType();
            LoadData();
            this.cboMemberType.SelectedIndexChanged += new EventHandler(cboMemberType_SelectedIndexChanged);

            
        }
        private void Bind_MemberType()
        {
            List<APP_Data.MemberType> mTypeList = new List<APP_Data.MemberType>();
            APP_Data.MemberType mType = new APP_Data.MemberType();
            mType.Id = 0;
            mType.Name = "All";
            mTypeList.Add(mType);
            mTypeList.AddRange(db.MemberTypes.Where(x => x.IsDelete == false).ToList());
            cboMemberType.DataSource = mTypeList;
            cboMemberType.DisplayMember = "Name";
            cboMemberType.ValueMember = "Id";
        }

        private void VisbleByRadio()
        {
            if (rdoMemberCardNo.Checked == true || rdoCustomerName.Checked == true)
            {
                VisibleControl(true, false);
            }
            else
            {
                VisibleControl(false, true);
            }
        }

        private void VisibleControl(bool t, bool f)
        {
            txtSearch.Visible = t;
            dtpBirthday.Visible = f;
        }


        private void LoadData()
        {
            Args argument = new Args();
            if (preload.Count > 0)
            {
                if (cboMemberType.SelectedValue.ToString() == "0" && txtSearch.Text == "" && rdoBirthday.Checked==false)
                {
                    argument.TypeArg = "";
                    argument.Value = "";
                    EnalbleControls(false);
                    if (!LoadWorker.IsBusy)
                    {
                        LoadWorker.RunWorkerAsync(argument);
                    }

                }
                else if (cboMemberType.SelectedIndex > 0 && txtSearch.Text == "" && rdoBirthday.Checked == false)
                {
                    argument.TypeArg = "cbo";
                    argument.Value = cboMemberType.SelectedValue.ToString();
                    EnalbleControls(false);
                    if (!LoadWorker.IsBusy)
                    {
                        LoadWorker.RunWorkerAsync(argument);
                    }
                }

                if (txtSearch.Visible == true)
                {
                    if (txtSearch.Text.Trim() != string.Empty)
                    {
                        if (rdoMemberCardNo.Checked)
                        {
                            argument.TypeArg = "membercard";
                            argument.Value = txtSearch.Text.Trim();
                            EnalbleControls(false);
                            if (!LoadWorker.IsBusy)
                            {
                                LoadWorker.RunWorkerAsync(argument);
                            }

                        }
                        else if (rdoCustomerName.Checked)
                        {
                            argument.TypeArg = "cusname";
                            argument.Value = txtSearch.Text.Trim();
                            EnalbleControls(false);
                            if (!LoadWorker.IsBusy)
                            {
                                LoadWorker.RunWorkerAsync(argument);
                            }
                        }
                    }
                }
                else if (rdoBirthday.Checked)
                {
                    string fromDate = dtpBirthday.Value.Date.ToShortDateString();

                    argument.TypeArg = "bday";
                    argument.Value = fromDate;
                    EnalbleControls(false);
                    if (!LoadWorker.IsBusy)
                    {
                        LoadWorker.RunWorkerAsync(argument);
                    }
                }
            }
        }

        private void ShowReport(IEnumerable<cls_CustomerInformaion> clist)
        {
            this.reportViewer1.LocalReport.DataSources.Single(ds => ds.Name == "CIreport").Value = clist;
            this.reportViewer1.RefreshReport();
        }


        private void rdoBirthday_CheckedChanged(object sender, EventArgs e)
        {
            lblSearchTitle.Text = "Birthday";
            txtSearch.Text = "";
            VisibleControl(false, true);
        }

        private void cboMemberType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = ""; rdoMemberCardNo.Checked = true;
            EnalbleControls(false);
            LoadData();
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            EnalbleControls(false);
            LoadData();
        }

        void EnalbleControls(bool state)
        {
            cboMemberType.Enabled = state;
            groupBox1.Enabled = state;
            reportViewer1.Enabled = state;
        }
        private void LoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Args workingArg = e.Argument as Args;
            int rows = 0;
            int progress = 0;
            if (workingArg.TypeArg == "")
            {
               LoadBar.CenterText="retrieveing data....";
                IEnumerable<cls_CustomerInformaion> clist = preload
                                      .Select(x => new cls_CustomerInformaion
                                      {
                                          Name = x.Name,
                                          Birthday = x.Birthday.ToString() == "" ? "" : x.Birthday.Value.Date.ToString("dd/MM/yyyy"),
                                          Gender = x.Gender == "" ? "" : x.Gender,
                                          NRC = x.NRC == "" ? "" : x.NRC,
                                          PhoneNumber = x.PhoneNumber == "" ? "" : x.PhoneNumber,
                                          Email = x.Email == "" ? "" : x.Email,
                                          Address = x.Address == "" ? "" : x.Address,
                                          TownShip = x.TownShip == "" ? "" : x.TownShip,
                                          City = x.City.CityName == "" ? "" : x.City.CityName,
                                          VIPMemberId = x.VIPMemberId == "" ? "" : x.VIPMemberId,
                                          MemberType = x.MemberTypeID.ToString() == "" ? "" : x.MemberType.Name,
                                          MemberTypeID = x.MemberTypeID.ToString() == "" ? "" : x.MemberTypeID.ToString(),
                                          PromoteDate = x.PromoteDate.ToString() == "" ? "" : x.PromoteDate.Value.Date.ToString("dd/MM/yyyy"),
                                          ID = x.Id,
                                          Points = 0
                                      }).ToList();
 
                List<cls_CustomerInformaion> filteredList = clist.Where(c => c.MemberTypeID.ToString() != "").ToList();
                rows = filteredList.Count();

                foreach (cls_CustomerInformaion item in filteredList)
                {
                    ELC_CustomerPointSystem.Get_ExpiredMemberList_And_Update_ExpiredMember(item.ID);
                    item.Points = ELC_CustomerPointSystem.Point_Calculation(item.ID);
                    progress++;
                    int percentage = (progress) * 100 / rows;
                    LoadWorker.ReportProgress(percentage);
                }
                e.Result = clist;
            }
            else if (workingArg.TypeArg == "cbo")
            {

                IEnumerable<cls_CustomerInformaion> clist = preload.Where(c => c.MemberTypeID == Convert.ToInt32(workingArg.Value))
                                      .Select(x => new cls_CustomerInformaion
                                      {
                                          Name = x.Name,
                                          Birthday = x.Birthday.ToString() == "" ? "" : x.Birthday.Value.Date.ToString("dd/MM/yyyy"),
                                          Gender = x.Gender == "" ? "" : x.Gender,
                                          NRC = x.NRC == "" ? "" : x.NRC,
                                          PhoneNumber = x.PhoneNumber == "" ? "" : x.PhoneNumber,
                                          Email = x.Email == "" ? "" : x.Email,
                                          Address = x.Address == "" ? "" : x.Address,
                                          TownShip = x.TownShip == "" ? "" : x.TownShip,
                                          City = x.City.CityName == "" ? "" : x.City.CityName,
                                          VIPMemberId = x.VIPMemberId == "" ? "" : x.VIPMemberId,
                                          MemberType = x.MemberTypeID.ToString() == "" ? "" : x.MemberType.Name,
                                          MemberTypeID = x.MemberTypeID.ToString() == "" ? "" : x.MemberTypeID.ToString(),
                                          PromoteDate = x.PromoteDate.ToString() == "" ? "" : x.PromoteDate.Value.Date.ToString("dd/MM/yyyy"),
                                          ID = x.Id,
                                          Points = 0
                                      }).ToList();
                List<cls_CustomerInformaion> filteredList = clist.Where(c => c.MemberTypeID.ToString() != "").ToList();
                rows = filteredList.Count();
                foreach (cls_CustomerInformaion item in filteredList)
                {
                    item.Points = ELC_CustomerPointSystem.Point_Calculation(item.ID);
                    progress++;
                    int percentage = (progress) * 100 / rows;
                    LoadWorker.ReportProgress(percentage);
                }
                e.Result = clist;
            }
            else if (workingArg.TypeArg == "membercard")
            {
                IEnumerable<cls_CustomerInformaion> clist = preload.Where(c => c.VIPMemberId == workingArg.Value)
                                     .Select(x => new cls_CustomerInformaion
                                     {
                                         Name = x.Name,
                                         Birthday = x.Birthday.ToString() == "" ? "" : x.Birthday.Value.Date.ToString("dd/MM/yyyy"),
                                         Gender = x.Gender == "" ? "" : x.Gender,
                                         NRC = x.NRC == "" ? "" : x.NRC,
                                         PhoneNumber = x.PhoneNumber == "" ? "" : x.PhoneNumber,
                                         Email = x.Email == "" ? "" : x.Email,
                                         Address = x.Address == "" ? "" : x.Address,
                                         TownShip = x.TownShip == "" ? "" : x.TownShip,
                                         City = x.City.CityName == "" ? "" : x.City.CityName,
                                         VIPMemberId = x.VIPMemberId == "" ? "" : x.VIPMemberId,
                                         MemberType = x.MemberTypeID.ToString() == "" ? "" : x.MemberType.Name,
                                         MemberTypeID = x.MemberTypeID.ToString() == "" ? "" : x.MemberTypeID.ToString(),
                                         PromoteDate = x.PromoteDate.ToString() == "" ? "" : x.PromoteDate.Value.Date.ToString("dd/MM/yyyy"),
                                         ID = x.Id,
                                         Points = 0
                                     }).ToList();
                List<cls_CustomerInformaion> filteredList = clist.Where(c => c.MemberTypeID.ToString() != "").ToList();
                rows = filteredList.Count();
                foreach (cls_CustomerInformaion item in filteredList)
                {
                    item.Points = ELC_CustomerPointSystem.Point_Calculation(item.ID);
                    progress++;
                    int percentage = (progress) * 100 / rows;
                    LoadWorker.ReportProgress(percentage);
                }
                e.Result = clist;
            }
            else if (workingArg.TypeArg == "cusname")
            {
                IEnumerable<cls_CustomerInformaion> clist = preload.Where(c => c.Name.Trim().ToLower().Contains(workingArg.Value.ToLower()))
                                        .Select(x => new cls_CustomerInformaion
                                        {
                                            Name = x.Name,
                                            Birthday = x.Birthday.ToString() == "" ? "" : x.Birthday.Value.Date.ToString("dd/MM/yyyy"),
                                            Gender = x.Gender == "" ? "" : x.Gender,
                                            NRC = x.NRC == "" ? "" : x.NRC,
                                            PhoneNumber = x.PhoneNumber == "" ? "" : x.PhoneNumber,
                                            Email = x.Email == "" ? "" : x.Email,
                                            Address = x.Address == "" ? "" : x.Address,
                                            TownShip = x.TownShip == "" ? "" : x.TownShip,
                                            City = x.City.CityName == "" ? "" : x.City.CityName,
                                            VIPMemberId = x.VIPMemberId == "" ? "" : x.VIPMemberId,
                                            MemberType = x.MemberTypeID.ToString() == "" ? "" : x.MemberType.Name,
                                            MemberTypeID = x.MemberTypeID.ToString() == "" ? "" : x.MemberTypeID.ToString(),
                                            PromoteDate = x.PromoteDate.ToString() == "" ? "" : x.PromoteDate.Value.Date.ToString("dd/MM/yyyy"),
                                            ID = x.Id,
                                            Points = 0
                                        }).ToList();
                int a = clist.Count();
                List<cls_CustomerInformaion> filteredList = clist.Where(c => c.MemberTypeID.ToString() != "").ToList();
                rows = filteredList.Count();
                foreach (cls_CustomerInformaion item in filteredList)
                {
                    item.Points = ELC_CustomerPointSystem.Point_Calculation(item.ID);
                    progress++;
                    int percentage = (progress) * 100 / rows;
                    LoadWorker.ReportProgress(percentage);
                }
                e.Result = clist;
            }
            else if (workingArg.TypeArg == "bday")
            {
                IEnumerable<cls_CustomerInformaion> clist = preload.Where(c =>c.Birthday==null?false:c.Birthday.Value.Date == Convert.ToDateTime(workingArg.Value))
                                        .Select(x => new cls_CustomerInformaion
                                        {
                                            Name = x.Name,
                                            Birthday = x.Birthday.ToString() == "" ? "" : x.Birthday.Value.Date.ToString("dd/MM/yyyy"),
                                            Gender = x.Gender == "" ? "" : x.Gender,
                                            NRC = x.NRC == "" ? "" : x.NRC,
                                            PhoneNumber = x.PhoneNumber == "" ? "" : x.PhoneNumber,
                                            Email = x.Email == "" ? "" : x.Email,
                                            Address = x.Address == "" ? "" : x.Address,
                                            TownShip = x.TownShip == "" ? "" : x.TownShip,
                                            City = x.City.CityName == "" ? "" : x.City.CityName,
                                            VIPMemberId = x.VIPMemberId == "" ? "" : x.VIPMemberId,
                                            MemberType = x.MemberTypeID.ToString() == "" ? "" : x.MemberType.Name,
                                            MemberTypeID = x.MemberTypeID.ToString() == "" ? "" : x.MemberTypeID.ToString(),
                                            PromoteDate = x.PromoteDate.ToString() == "" ? "" : x.PromoteDate.Value.Date.ToString("dd/MM/yyyy"),
                                            ID = x.Id,
                                            Points = 0
                                        }).ToList();
                int a = clist.Count();
                List<cls_CustomerInformaion> filteredList = clist.Where(c => c.MemberTypeID.ToString() != "").ToList();
                rows = filteredList.Count();
              
                foreach (cls_CustomerInformaion item in filteredList)
                {
                    item.Points = ELC_CustomerPointSystem.Point_Calculation(item.ID);
                    progress++;
                    int percentage = (progress) * 100 / rows;
                    LoadWorker.ReportProgress(percentage);
                }
                e.Result = clist;
            }

        }
        private void LoadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LoadBar.CenterText = "";
            LoadBar.CenterText = "calculating points..";
            LoadBar.Value = e.ProgressPercentage;
        }

        private void LoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowReport(e.Result as IEnumerable<cls_CustomerInformaion>);
            EnalbleControls(true);
        }

        private void rdoMemberCardNo_CheckedChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.Visible = true; dtpBirthday.Visible = false;
            txtSearch.CharacterCasing = CharacterCasing.Upper;
        }

        private void rdoCustomerName_CheckedChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.Visible = true; dtpBirthday.Visible = false;
            txtSearch.CharacterCasing = CharacterCasing.Normal;
        }
    }
}
