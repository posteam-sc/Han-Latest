using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using POS.APP_Data;
using System.Text.RegularExpressions;

namespace POS
{
    public partial class NewCustomer : Form
    {
        #region Variables
        bool IsUpdateOrSave = false;
        public bool IsClosed = false;
        public Boolean isEdit { get; set; }

        public int? CustomerId { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        public string MemerTypeName { get; set; }

        public string _from { get; set; }

        public char Type { get; set; }
        public bool isReNew = false;
        public bool isExpired = false;

        public int getTotalAmount { get; set; }

        int totalAmount = 0;
        public string TransactionId;
        System.Data.Objects.ObjectResult<String> Id;
        string resultId = "-";
        APP_Data.Customer customerObj = new Customer();
        Customer currentCustomer = new Customer();
        public bool mandatory = false;
        //public List<Transaction> transactionList = new List<Transaction>();
        #endregion

        public NewCustomer()
        {
            InitializeComponent();
        }

        private void New_Customer_Load(object sender, EventArgs e)
        {
            int memberValidityYear = Convert.ToInt32(SettingController.DefaultMemberValidityYear.ToString());
            int year = 0;
            cboTitle.Items.Add("Mr");
            cboTitle.Items.Add("Mrs");
            cboTitle.Items.Add("Miss");
            cboTitle.Items.Add("Ms");
            cboTitle.Items.Add("Daw");
            cboTitle.Items.Add("U");
            cboTitle.SelectedIndex = 0;

            Bind_City();

            Bind_MemberType();
            plError.Visible = false;
            //Enable_MType(false);
            txtName.Focus();

            currentCustomer = (from c in entity.Customers where c.Id == CustomerId select c).FirstOrDefault<Customer>();
            //if (Type != 'S' && Type != 'C' && CustomerId==null)
            //{
            //    cboMemberType.Enabled = true;
            //    dtpMemberDate.Enabled = true;
            //    btnAddMenber.Enabled = true;
            //}
            if (isEdit)
            {
                cboMemberType.Enabled = false;
                dtpMemberDate.Enabled = false;
                btnAddMenber.Enabled = false;
                //Editing here

                if (Type == 'S')
                {
                    if (currentCustomer.Name == "Default")
                    {
                        txtName.Visible = true;
                        cboCustomerName.Visible = false;
                        btnCustomerSearch.Visible = true;
                    }
                    else
                    {
                        txtName.Text = currentCustomer.Name;
                        btnCustomerSearch.Visible = false;
                        btnCustomerAdd.Visible = false;
                    }
                    //cboCustomerName.Visible = true;
                    //txtName.Visible = false;
                    //Bind_Customer();
                    //cboCustomerName.Text = currentCustomer.Name;
                }
                else
                {
                    txtName.Text = currentCustomer.Name;
                    btnCustomerAdd.Visible = false;
                    btnCustomerSearch.Visible = false;
                }

                txtPhoneNumber.Text = currentCustomer.PhoneNumber;
                txtNRC.Text = currentCustomer.NRC;
                txtAddress.Text = currentCustomer.Address;
                txtEmail.Text = currentCustomer.Email;
                cboTitle.Text = currentCustomer.Title;
                cboCity.Text = currentCustomer.City.CityName;
                if (currentCustomer.Gender == "Male")
                {
                    rdbMale.Checked = true;
                }
                else
                {
                    rdbFemale.Checked = true;
                }

                if (currentCustomer.Birthday == null)
                {
                    dtpBirthday.Value = DateTime.Now.Date;
                }
                else
                {
                    dtpBirthday.Value = currentCustomer.Birthday.Value.Date;
                }

                if (currentCustomer.MemberTypeID != null)
                {
                    cboMemberType.SelectedValue = currentCustomer.MemberTypeID;
                    string name = (from m in entity.MemberTypes where m.Id == currentCustomer.MemberTypeID select m.Name).FirstOrDefault();
                    txtMemberType.Text = name;
                    cboMemberType.Text = name;
                    txtMemberDate.Text = currentCustomer.StartDate.Value.ToShortDateString();
                    dtpMemberDate.Value = currentCustomer.StartDate.Value.Date;
                    txtMID.Text = currentCustomer.VIPMemberId;
                    //Enable_MType(true);
                }
                btnSubmit.Image = POS.Properties.Resources.update_big;
            }
            else
            {
                cboCustomerName.Visible = false;
                txtName.Visible = true;

                int cityId = 0;
                cityId = SettingController.DefaultCity;
                APP_Data.City cus2 = (from c in entity.Cities where c.Id == cityId select c).FirstOrDefault();
                cboCity.Text = cus2.CityName;
                rdbMale.Checked = true;

                btnCustomerAdd.Visible = false;
                if (CustomerId == 1)
                {
                    btnCustomerSearch.Visible = true;
                }
                else
                {
                    btnCustomerSearch.Visible = false;
                }

                if (currentCustomer != null)
                {
                    if (currentCustomer.VIPMemberId != null)
                    {
                        txtMID.Text = currentCustomer.VIPMemberId;
                    }
                }
                if (Type == 'S')
                {
                    cboMemberType.Enabled = false;
                    btnAddMenber.Enabled = false;
                    dtpMemberDate.Enabled = false;
                }

            }

            if (Type == 'S')
            {
                if (MemerTypeName != null)
                {
                    //cboMemberType.Text = MemerTypeName;
                    txtMemberType.Text = MemerTypeName;
                    cboMemberType.Text = MemerTypeName;
                    if (Type == 'S' && isReNew == true)
                    {
                        txtMemberDate.Text = DateTime.Today.Date.ToShortDateString();
                        dtpMemberDate.Value = DateTime.Today.Date;

                    }
                    else if (Type == 'S' && currentCustomer.Name != "Default" && isEdit == true && currentCustomer.StartDate != null && isExpired == false)
                    {
                        txtMemberDate.Text = currentCustomer.StartDate.Value.ToShortDateString();
                        dtpMemberDate.Value = currentCustomer.StartDate.Value.Date;
                    }
                    else if (Type == 'S' && currentCustomer.Name != "Default" && isEdit == true && currentCustomer.StartDate != null && isExpired == true)
                    {
                        year = currentCustomer.ExpireDate.Value.Year - memberValidityYear;
                        int month = currentCustomer.ExpireDate.Value.Month;
                        int day = currentCustomer.ExpireDate.Value.Day;

                        string startDate = Convert.ToString(year + "/" + month + "/" + day);
                        //currentCustomer.ExpireDate = Convert.ToDateTime(startDate);


                        //txtMemberDate.Text = currentCustomer.ExpireDate.Value.ToShortDateString();
                        //dtpMemberDate.Value = currentCustomer.ExpireDate.Value.Date;
                        txtMemberDate.Text = startDate.ToString();
                        dtpMemberDate.Value = Convert.ToDateTime(startDate);
                    }
                    else
                    {

                        txtMemberDate.Text = DateTime.Today.Date.ToShortDateString();
                        dtpMemberDate.Value = DateTime.Today.Date;
                    }

                    btnCustomerAdd.Visible = false;
                    //Enable_MType(true);
                }
                else
                {

                    txtMemberType.Text = null;
                    cboMemberType.Text = null;
                    //Bind_MemberType();
                    txtMemberDate.Text = null;
                    dtpMemberDate.Value = DateTime.Today.Date;
                    txtMID.Text = null;
                    btnCustomerAdd.Visible = false;
                    btnCustomerSearch.Visible = false;
                }
            }

        }

        public void Bind_City()
        {
            entity = new POSEntities();
            List<APP_Data.City> cityList = new List<APP_Data.City>();
            APP_Data.City city1 = new APP_Data.City();
            city1.Id = 0;
            city1.CityName = "Select";
            cityList.Add(city1);
            cityList.AddRange(entity.Cities.Where(x => x.IsDelete == false).ToList());
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";
            cboCity.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCity.AutoCompleteSource = AutoCompleteSource.ListItems;
        }
        public void Bind_Customer()
        {
            entity = new POSEntities();
            List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
            APP_Data.Customer customers = new APP_Data.Customer();
            customers.Id = 0;
            customers.Name = "Select";
            customerList.Add(customers);
            customerList.AddRange(entity.Customers.Where(x => x.Name != "Default").ToList());
            cboCustomerName.DataSource = customerList;
            cboCustomerName.DisplayMember = "Name";
            cboCustomerName.ValueMember = "Id";
        }

        public void reloadCity(string CityName)
        {
            Bind_City();
            cboCity.Text = CityName;
        }

        public void reloadMemberType(string MType)
        {
            Bind_MemberType();
            cboMemberType.Text = MType;
        }

        public void Bind_MemberType()
        {
            entity = new POSEntities();
            List<APP_Data.MemberType> mTypeList = new List<APP_Data.MemberType>();
            APP_Data.MemberType mType = new APP_Data.MemberType();
            mType.Id = 0;
            mType.Name = "Select";
            mTypeList.Add(mType);
            mTypeList.AddRange((from p in entity.MemberTypes
                                join c in entity.MemberCardRules
                                              on p.Id equals c.MemberTypeId
                                where p.IsDelete == false && c.IsActive == true
                                select p).ToList());
            cboMemberType.DataSource = mTypeList;
            cboMemberType.DisplayMember = "Name";
            cboMemberType.ValueMember = "Id";

            if (mTypeList.Count > 1)
                cboMemberType.SelectedIndex = 0;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;
            POSEntities entity = new POSEntities();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation

            if (txtName.Visible == true)
            {
                if (txtName.Text.Trim() == string.Empty)
                {
                    tp.SetToolTip(txtName, "Error");
                    tp.Show("Please fill up customer name!", txtName);
                    hasError = true;

                }
            }
            else if (cboCustomerName.Visible == true)
            {
                if (cboCustomerName.Text == string.Empty)
                {
                    tp.SetToolTip(cboCustomerName, "Error");
                    tp.Show("Please fill up customer name!", cboCustomerName);

                    hasError = true;
                }

            }


            if (cboCity.SelectedIndex == 0)
            {
                tp.SetToolTip(cboCity, "Error");
                tp.Show("Please fill up city name!", cboCity);
                hasError = true;
            }

            if (cboMemberType.SelectedIndex != 0 && cboMemberType.SelectedIndex != -1)
            {
                if (txtMID.Text == "")
                {
                    tp.SetToolTip(txtMID, "Error");
                    tp.Show("Please fill up Member Id!", txtMID);
                    hasError = true;
                }

            }
            if (!string.IsNullOrEmpty(txtPhoneNumber.Text))
            {

                if (!Regex.IsMatch(txtPhoneNumber.Text, @"^[+?0-9]+$"))
                {
                    tp.SetToolTip(txtPhoneNumber, "Error");
                    tp.Show("PhoneNo must not contain characters!", txtPhoneNumber);
                    hasError = true;
                }
                if (txtPhoneNumber.Text.Length < 7 || txtPhoneNumber.Text.Length > 11)
                {
                    tp.SetToolTip(txtPhoneNumber, "Error");
                    tp.Show("PhoneNo must be between \n 7 or 11 digits!", txtPhoneNumber);
                    hasError = true;
                }
            }

            Customer currentCustomer = new Customer();
            int MemberId = 0;

            if (!hasError)
            {

                int memberValidityYear = Convert.ToInt32(SettingController.DefaultMemberValidityYear.ToString());
                if (isEdit)
                { // update
                    //int CustomerName = 0;

                    //if (txtName.Visible == true)
                    //{
                    //    // CustomerId = (from c in entity.Customers where c.Name == txtName.Text select c.Id).FirstOrDefault();
                    //    CustomerName = (from p in entity.Customers where p.Name.Trim() == txtName.Text.Trim() && p.Id != CustomerId select p).ToList().Count;
                    //}
                    //else
                    //{
                    //    CustomerId = Convert.ToInt32(cboCustomerName.SelectedValue);
                    //    CustomerName = (from p in entity.Customers where p.Name.Trim() == cboCustomerName.Text.Trim() && p.Id != CustomerId select p).ToList().Count;
                    //}

                    if (isEdit == false)
                    {
                        if (cboMemberType.SelectedIndex != 0 && cboMemberType.SelectedIndex != -1)
                        {

                            MemberId = (from p in entity.Customers where p.VIPMemberId.Trim() == txtMID.Text.Trim() select p).ToList().Count;

                            if (MemberId > 0)
                            {
                                tp.SetToolTip(txtMID, "Error");
                                tp.Show("This Member Id is already exist!", txtMID);
                                return;
                            }
                        }

                    }
                    else
                    {
                        if (cboMemberType.SelectedIndex != 0 && cboMemberType.SelectedIndex != -1)
                        {
                            MemberId = (from p in entity.Customers where p.VIPMemberId.Trim() == txtMID.Text.Trim() && p.Id != CustomerId select p).ToList().Count;

                            if (MemberId > 0)
                            {
                                tp.SetToolTip(txtMID, "Error");
                                tp.Show("This Member Id is already exist!", txtMID);
                                return;
                            }
                        }
                    }

                    currentCustomer = (from c in entity.Customers where c.Id == CustomerId select c).FirstOrDefault<Customer>();
                    currentCustomer.Title = cboTitle.Text;
                    if (txtName.Visible == true)
                    {
                        currentCustomer.Name = txtName.Text;
                    }
                    else
                    {
                        currentCustomer.Name = cboCustomerName.Text;
                    }
                    currentCustomer.PhoneNumber = txtPhoneNumber.Text;

                    currentCustomer.NRC = (txtNRC.Text.Replace("(", "'("));
                    currentCustomer.Address = txtAddress.Text;
                    currentCustomer.Email = txtEmail.Text;
                    if (rdbMale.Checked == true)
                    {
                        currentCustomer.Gender = "Male";
                    }
                    else
                    {
                        currentCustomer.Gender = "Female";
                    }
                    if (dtpBirthday.Value.Date == DateTime.Now.Date)
                    {
                        currentCustomer.Birthday = null;
                    }
                    else
                    {
                        currentCustomer.Birthday = dtpBirthday.Value.Date;
                    }

                    // if (txtMemberType.Text!=string.Empty)
                    if (cboMemberType.SelectedIndex != 0 && cboMemberType.SelectedIndex != -1)
                    {
                        //int memberTypeID = (from m in entity.MemberTypes where m.Name == txtMemberType.Text select m.Id).First();
                        int memberTypeID = (from m in entity.MemberTypes where m.Name == cboMemberType.Text select m.Id).First();
                        MemberCardRule _currentMemberCardRule = (from m in entity.MemberCardRules where m.MemberTypeId == memberTypeID && m.IsActive == true select m).FirstOrDefault();
                        //if(currentCustomer.MemberTypeID!=memberTypeID)
                        //{
                        currentCustomer.MemberTypeID = memberTypeID;
                        //currentCustomer.StartDate = dtpMemberDate.Value.Date;

                        currentCustomer.StartDate = Convert.ToDateTime(txtMemberDate.Text);
                        currentCustomer.StartDate = dtpMemberDate.Value.Date;
                        currentCustomer.VIPMemberId = txtMID.Text;
                        if (currentCustomer.VipStartedShop == null)
                        {
                            if (txtMID.Text != "")
                            {
                                currentCustomer.VipStartedShop = SettingController.DefaultShop.ShortCode;
                            }
                        }
                        int year = 0;
                        if (DateTime.Today.Date >= currentCustomer.StartDate && DateTime.Today.Date <= currentCustomer.ExpireDate)
                        {  // ok                                 
                            if (currentCustomer.FirstTime == null)
                            {
                                currentCustomer.PromoteDate = DateTime.Today.Date;
                                currentCustomer.FirstTime = true;
                                year = currentCustomer.StartDate.Value.Year + memberValidityYear + 1;
                                int month = currentCustomer.StartDate.Value.Month;
                                int day = currentCustomer.StartDate.Value.Day;
                                string expireDate = Convert.ToString(year + "/" + month + "/" + day);
                                currentCustomer.ExpireDate = Convert.ToDateTime(expireDate);
                            }
                            else if (currentCustomer.FirstTime == true)
                            {
                                currentCustomer.FirstTime = false;
                                currentCustomer.ExpireDate = currentCustomer.ExpireDate;

                            }
                            else
                            {
                                currentCustomer.PromoteDate = currentCustomer.PromoteDate;
                                currentCustomer.ExpireDate = currentCustomer.ExpireDate;
                            }
                        }
                        else
                        {
                            if (isExpired == true)
                            {
                                year = currentCustomer.ExpireDate.Value.Year + memberValidityYear;
                                int month = currentCustomer.StartDate.Value.Month;
                                int day = currentCustomer.StartDate.Value.Day;
                                string expireDate = Convert.ToString(year + "/" + month + "/" + day);
                                currentCustomer.ExpireDate = Convert.ToDateTime(expireDate);
                            }
                            else if (isReNew == true)
                            {
                                if (TransactionId != null)
                                {
                                    if (_currentMemberCardRule.IsCalculatePoints == false)
                                    {
                                        var Tran = (from t in entity.Transactions
                                                    where t.Id == TransactionId
                                                    select t).First();
                                        Tran.IsCalculatePoint = false;
                                        entity.SaveChanges();
                                    }
                                }

                                currentCustomer.PromoteDate = DateTime.Today.Date;
                                currentCustomer.FirstTime = true;
                                year = currentCustomer.StartDate.Value.Year + memberValidityYear + 1;
                                int month = currentCustomer.StartDate.Value.Month;
                                int day = currentCustomer.StartDate.Value.Day;
                                string expireDate = Convert.ToString(year + "/" + month + "/" + day);
                                currentCustomer.ExpireDate = Convert.ToDateTime(expireDate);
                            }
                            else if (currentCustomer.FirstTime == null)
                            {
                                if (TransactionId != null)
                                {
                                    if (_currentMemberCardRule.IsCalculatePoints == false)
                                    {
                                        var Tran = (from t in entity.Transactions
                                                    where t.Id == TransactionId
                                                    select t).First();
                                        Tran.IsCalculatePoint = false;
                                        entity.SaveChanges();
                                    }
                                }


                                currentCustomer.PromoteDate = DateTime.Today.Date;
                                currentCustomer.FirstTime = true;
                                year = currentCustomer.StartDate.Value.Year + memberValidityYear + 1;
                                int month = currentCustomer.StartDate.Value.Month;
                                int day = currentCustomer.StartDate.Value.Day;
                                string expireDate = Convert.ToString(year + "/" + month + "/" + day);
                                currentCustomer.ExpireDate = Convert.ToDateTime(expireDate);
                            }
                            else
                            {
                                currentCustomer.PromoteDate = currentCustomer.PromoteDate;
                                currentCustomer.ExpireDate = currentCustomer.ExpireDate;
                            }
                        }

                        //}
                        //else
                        //{
                        //    currentCustomer.MemberTypeID = memberTypeID;
                        //    currentCustomer.PromoteDate = currentCustomer.PromoteDate;
                        //    currentCustomer.StartDate = currentCustomer.StartDate;
                        //    currentCustomer.FirstTime = currentCustomer.FirstTime;
                        //    currentCustomer.ExpireDate = currentCustomer.ExpireDate;
                        //}                                                    

                    }
                    else
                    {
                        currentCustomer.MemberTypeID = null;
                        currentCustomer.PromoteDate = null;
                        currentCustomer.StartDate = null;
                        currentCustomer.VIPMemberId = null;
                        currentCustomer.FirstTime = null;
                        currentCustomer.ExpireDate = null;
                    }

                    currentCustomer.CityId = Convert.ToInt32(cboCity.SelectedValue.ToString());
                    entity.Entry(currentCustomer).State = EntityState.Modified;
                    entity.SaveChanges();

                    if (TransactionId != null)
                    {
                        var Tran = (from t in entity.Transactions
                                    where t.Id == TransactionId
                                    select t).First();
                        Tran.CustomerId = currentCustomer.Id;
                        Tran.MemberTypeId = currentCustomer.MemberTypeID;
                        entity.SaveChanges();
                    }

                    MessageBox.Show("Successfully Updated!", "Update");
                    IsUpdateOrSave = true;
                    //  this.Dispose();
                    #region active PaidByCreditWithPrePaidDebt
                    if (System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"] != null)
                    {
                        PaidByCreditWithPrePaidDebt newForm = (PaidByCreditWithPrePaidDebt)System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"];
                        newForm.LoadForm();
                        this.Dispose();
                    }
                    #endregion

                    #region active PaidByCredit
                    if (System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"] != null)
                    {
                        PaidByCreditWithPrePaidDebt newForm = (PaidByCreditWithPrePaidDebt)System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"];
                        newForm.LoadForm();
                        this.Dispose();
                    }
                    #endregion

                    //refresh sales form's customer list
                    if (System.Windows.Forms.Application.OpenForms["CustomerList"] != null)
                    {
                        CustomerList newForm = (CustomerList)System.Windows.Forms.Application.OpenForms["CustomerList"];
                        newForm.DataBind();
                    }

                    if (System.Windows.Forms.Application.OpenForms["GiftCardControl"] != null)
                    {
                        GiftCardControl newForm = (GiftCardControl)System.Windows.Forms.Application.OpenForms["GiftCardControl"];
                        newForm.Customer_Bind();
                        this.Dispose();
                    }

                    if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                    {
                        Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                        newForm.txtMEMID.Text = currentCustomer.VIPMemberId;
                        if (this.Type == 'C')
                        {
                            newForm.ReloadCustomerList();

                            newForm.SetCurrentCustomer(currentCustomer.Id, false);
                        }

                    }
                    clear();
                    this.Dispose();

                }
                else
                { //save
                    int memberTypeID = 0;
                    int CustomerName = 0;
                    if (txtName.Visible == true)
                    {
                        CustomerName = (from p in entity.Customers where p.Name.Trim() == txtName.Text.Trim() select p).ToList().Count;
                    }
                    else
                    {
                        CustomerName = (from p in entity.Customers where p.Name.Trim() == cboCustomerName.Text.Trim() select p).ToList().Count;
                    }
                    if (isEdit == false)
                    {
                        if (cboMemberType.SelectedIndex != 0 && cboMemberType.SelectedIndex != -1)
                        {

                            MemberId = (from p in entity.Customers where p.VIPMemberId.Trim() == txtMID.Text.Trim() select p).ToList().Count;

                            if (MemberId > 0)
                            {
                                tp.SetToolTip(txtMID, "Error");
                                tp.Show("This Member Id is already exist!", txtMID);
                                return;
                            }
                        }

                    }
                    else
                    {
                        if (cboMemberType.SelectedIndex != 0 && cboMemberType.SelectedIndex != -1)
                        {
                            MemberId = (from p in entity.Customers where p.VIPMemberId.Trim() == txtMID.Text.Trim() && p.Id != CustomerId select p).ToList().Count;

                            if (MemberId > 0)
                            {
                                tp.SetToolTip(txtMID, "Error");
                                tp.Show("This Member Id is already exist!", txtMID);
                                return;
                            }
                        }
                    }
                    //CustomerName = (from p in entity.Customers where p.Name.Trim() == txtName.Text.Trim() select p).ToList().Count;
                    if (CustomerName == 0)
                    {
                        var CustomerCode = entity.CustomerAutoID(DateTime.Now, SettingController.DefaultShop.ShortCode);
                        currentCustomer.Title = cboTitle.Text;
                        if (txtName.Visible == true)
                        {
                            currentCustomer.Name = txtName.Text;
                        }
                        else
                        {
                            currentCustomer.Name = cboCustomerName.Text;
                        }
                        currentCustomer.PhoneNumber = txtPhoneNumber.Text;
                        currentCustomer.NRC = txtNRC.Text;
                        currentCustomer.Email = txtEmail.Text;
                        currentCustomer.Address = txtAddress.Text;
                        currentCustomer.CustomerCode = CustomerCode.FirstOrDefault().ToString();
                        if (rdbMale.Checked == true)
                        {
                            currentCustomer.Gender = "Male";
                        }
                        else
                        {
                            currentCustomer.Gender = "Female";
                        }
                        if (dtpBirthday.Value.Date == DateTime.Now.Date)
                        {
                            currentCustomer.Birthday = null;
                        }
                        else
                        {
                            currentCustomer.Birthday = dtpBirthday.Value.Date;
                        }

                        if (cboMemberType.SelectedIndex != 0 && cboMemberType.SelectedIndex != -1)
                        {
                            memberTypeID = (from m in entity.MemberTypes where m.Name == cboMemberType.Text select m.Id).First();
                            MemberCardRule _currentMemberCardRule = (from m in entity.MemberCardRules where m.MemberTypeId == memberTypeID && m.IsActive == true select m).FirstOrDefault();
                            currentCustomer.MemberTypeID = memberTypeID;
                            //currentCustomer.StartDate = dtpMemberDate.Value.Date;
                            currentCustomer.PromoteDate = dtpMemberDate.Value.Date;
                            //currentCustomer.StartDate = Convert.ToDateTime(txtMemberDate.Text);
                            currentCustomer.StartDate = dtpMemberDate.Value.Date;
                            currentCustomer.VIPMemberId = txtMID.Text;
                            if (currentCustomer.VipStartedShop == null)
                            {
                                if (txtMID.Text != "")
                                {
                                    currentCustomer.VipStartedShop = SettingController.DefaultShop.ShortCode;
                                }
                            }
                            currentCustomer.FirstTime = true;
                            int year = currentCustomer.StartDate.Value.Year + memberValidityYear + 1;
                            int month = currentCustomer.StartDate.Value.Month;
                            int day = currentCustomer.StartDate.Value.Day;
                            string expireDate = Convert.ToString(year + "/" + month + "/" + day);
                            currentCustomer.ExpireDate = Convert.ToDateTime(expireDate);
                            if (TransactionId != null)
                            {
                                if (_currentMemberCardRule.IsCalculatePoints == false)
                                {
                                    var Tran = (from t in entity.Transactions
                                                where t.Id == TransactionId
                                                select t).First();
                                    Tran.IsCalculatePoint = false;
                                    entity.SaveChanges();
                                }
                            }

                        }
                        else
                        {
                            currentCustomer.MemberTypeID = null;
                            currentCustomer.PromoteDate = null;
                            currentCustomer.StartDate = null;
                            currentCustomer.VIPMemberId = null;
                            currentCustomer.FirstTime = null;
                            currentCustomer.ExpireDate = null;
                        }

                        currentCustomer.CityId = Convert.ToInt32(cboCity.SelectedValue.ToString());
                        entity.Customers.Add(currentCustomer);
                        entity.SaveChanges();


                        if (TransactionId != null)
                        {
                            var Tran = (from t in entity.Transactions
                                        where t.Id == TransactionId
                                        select t).First();
                            Tran.CustomerId = currentCustomer.Id;
                            Tran.MemberTypeId = currentCustomer.MemberTypeID;

                            PointDeductionPercentage_History pdp = entity.PointDeductionPercentage_History.FirstOrDefault(p => p.Active == true);
                            if (pdp != null)
                            {
                                Tran.TransactionDetails.Where(t => t.DiscountRate > 0).ToList().ForEach(t => t.IsDeductedBy = pdp.DiscountRate);
                            }

                            entity.SaveChanges();
                        }
                        else
                        { // no transaction, but they want to get a member type, so auto inserting to transaction for that member type\
                            if (cboMemberType.SelectedIndex != 0 && cboMemberType.SelectedIndex != -1)
                            {
                                MemberCardRule _currentMemberCardRules = (from m in entity.MemberCardRules where m.MemberTypeId == memberTypeID && m.IsActive == true select m).FirstOrDefault();
                                int totalCost = Convert.ToInt32(_currentMemberCardRules.RangeFrom);

                                Transaction insertedTransaction = new Transaction();
                                //Updated by Lele
                                //Id = entity.InsertTransaction(dtpMemberDate.Value, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 1, 0, 0, totalCost, totalCost, null, currentCustomer.Id, null, null, memberTypeID, null, false, "", false, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode, false);
                                //Id = entity.InsertTransaction(dtpMemberDate.Value, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 1, 0, 0, totalCost, totalCost, null, currentCustomer.Id, null, null, memberTypeID, null, false, "", false, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false);
                                //Updated by Yimon
                                Id = entity.InsertTransaction(dtpMemberDate.Value, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 1, 0, 0, totalCost, totalCost, null, currentCustomer.Id, null, null, memberTypeID, null, false, "", false, 0, SettingController.DefaultShop.Id, SettingController.DefaultShop.ShortCode,0,0,false, false, "","");

                                //entity = new POSEntities();
                                resultId = Id.FirstOrDefault().ToString();
                                insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();

                                insertedTransaction.IsDeleted = false;
                                insertedTransaction.MemberTypeId = Convert.ToInt32(cboMemberType.SelectedValue);
                                insertedTransaction.Type = "Special Member";
                                insertedTransaction.PaymentTypeId = null;
                                entity.SaveChanges();
                            }
                        }
                        MessageBox.Show("Successfully Saved!", "Save");

                        IsUpdateOrSave = true;

                        //  this.Dispose();

                        #region active PaidByCreditWithPrePaidDebt
                        if (System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"] != null)
                        {
                            PaidByCreditWithPrePaidDebt newForm = (PaidByCreditWithPrePaidDebt)System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"];
                            newForm.LoadForm();
                            this.Dispose();
                        }
                        #endregion

                        #region active PaidByCredit
                        if (System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"] != null)
                        {
                            PaidByCreditWithPrePaidDebt newForm = (PaidByCreditWithPrePaidDebt)System.Windows.Forms.Application.OpenForms["PaidByCreditWithPrePaidDebt"];
                            newForm.LoadForm();
                            this.Dispose();
                        }
                        #endregion

                        //refresh sales form's customer list
                        if (System.Windows.Forms.Application.OpenForms["CustomerList"] != null)
                        {
                            CustomerList newForm = (CustomerList)System.Windows.Forms.Application.OpenForms["CustomerList"];
                            newForm.DataBind();
                        }

                        if (System.Windows.Forms.Application.OpenForms["GiftCardControl"] != null)
                        {
                            GiftCardControl newForm = (GiftCardControl)System.Windows.Forms.Application.OpenForms["GiftCardControl"];
                            newForm.Customer_Bind();
                            newForm.SetCurrentCustomer(currentCustomer.Id);

                            this.Dispose();
                        }

                        if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                        {
                            Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                            if (Type == 'C')
                            {
                                newForm.ReloadCustomerList();
                                newForm.SetCurrentCustomer(currentCustomer.Id, false);
                            }
                            else
                            {
                                newForm.ReloadCustomerList();
                                newForm.SetCurrentCustomer(currentCustomer.Id, false);
                            }
                            this.Dispose();

                        }
                        clear();


                    }
                    else if (CustomerName > 0)
                    {
                        tp.SetToolTip(txtName, "Error");
                        tp.Show("This Customer Name is already exist!", txtName);
                    }
                }

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //DialogResult result = MessageBox.Show("Are you sure you want to cancel?", "Cancel", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            //if (result.Equals(DialogResult.OK))
            //{
            //    this.Dispose();
            //}
            clear();
        }


        private void clear()
        {
            cboTitle.Text = "Mr";
            txtAddress.Text = "";
            txtEmail.Text = "";
            txtName.Text = "";
            txtNRC.Text = "";
            txtPhoneNumber.Text = "";
            dtpBirthday.Value = DateTime.Now.Date;
            rdbMale.Checked = true;
            Bind_Customer();
            cboMemberType.Text = "";
            txtMemberDate.Text = "";
            dtpMemberDate.Value = DateTime.Now.Date;
            txtMID.Text = "";

            int cityId = 0;
            cityId = SettingController.DefaultCity;
            APP_Data.City cus2 = (from c in entity.Cities where c.Id == cityId select c).FirstOrDefault();
            cboCity.Text = cus2.CityName;
            btnSubmit.Image = POS.Properties.Resources.save_big;
        }
        private void New_Customer_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtName);
            tp.Hide(txtPhoneNumber);
            tp.Hide(plError);
        }
        public static bool IsEmail(string s)
        {
            Regex EmailExpression = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.Compiled | RegexOptions.Singleline);


            if (!EmailExpression.IsMatch(s))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void btnAddCity_Click(object sender, EventArgs e)
        {
            City newForm = new City();
            newForm.ShowDialog();
        }

        private void cboMemberType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Enable_MType(bool b)
        {
            dtpMemberDate.Enabled = b;
            txtMID.Enabled = b;
        }

        private void btnAddMenber_Click(object sender, EventArgs e)
        {
            newMemberType newType = new newMemberType();
            newType.ShowDialog();
        }

        internal void reloadCity(int p)
        {
            throw new NotImplementedException();
        }

        // By SYM
        private void cboCustomerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            int totAmt = 0;
            int giftCardTotalAmount = 0;
            int couponCodeTotalAmount = 0;
            int totalRAmount = 0;
            int memberValidityYear = Convert.ToInt32(SettingController.DefaultMemberValidityYear.ToString());
            List<MemberCardRule> memberCardRuleList = (from m in entity.MemberCardRules.AsEnumerable() where m.IsActive == true orderby int.Parse(m.RangeFrom) select m).ToList();
            int mTypeID = (from memberCardRule in entity.MemberCardRules.AsEnumerable() where memberCardRule.IsActive == true orderby int.Parse(memberCardRule.RangeFrom) descending select memberCardRule.MemberTypeId).FirstOrDefault();
            string minimumAmountofThisMemberType = (from mCardRule in entity.MemberCardRules.AsEnumerable() where mCardRule.IsActive == true orderby int.Parse(mCardRule.RangeFrom) select mCardRule.RangeFrom).FirstOrDefault();
            #region not need
            //if (CustomerId != 1)
            //{
            //    //int customberid = Convert.ToInt32(cboCustomerName.SelectedValue);
            //    customerObj = (from c in entity.Customers where c.Id == CustomerId select c).FirstOrDefault();
            //    txtPhoneNumber.Text = customerObj.PhoneNumber;
            //    txtNRC.Text = customerObj.NRC;
            //    cboTitle.Text = customerObj.Title;
            //    txtEmail.Text = customerObj.Email;
            //    if (customerObj.Birthday == null)
            //    {
            //        dtpBirthday.Value = DateTime.Now.Date;
            //    }
            //    else
            //    {
            //        dtpBirthday.Value = customerObj.Birthday.Value.Date;
            //    }
            //    if (customerObj.Gender == "Male")
            //    {
            //        rdbMale.Checked = true;
            //    }
            //    else
            //    {
            //        rdbFemale.Checked = true;
            //    }
            //    cboCity.Text = customerObj.City.CityName;
            //    txtAddress.Text = customerObj.Address;
            //    // end
            //    if (customerObj.MemberTypeID != null)
            //    {
            //        // isexpired? == no
            //        if (DateTime.Today.Date <= customerObj.ExpireDate)
            //        {
            //            if (customerObj.MemberTypeID == mTypeID) //if selected customer is the greatest
            //            {
            //                string _name = (from m in entity.MemberTypes where m.Id == customerObj.MemberTypeID select m.Name).FirstOrDefault();
            //                txtMemberType.Text = _name;
            //                cboMemberType.Text = _name;
            //                txtMemberDate.Text = Convert.ToString(customerObj.StartDate.Value.Date);
            //                dtpMemberDate.Value = Convert.ToDateTime(customerObj.StartDate.Value.Date);
            //                txtMID.Text = customerObj.VIPMemberId;
            //            }
            //            else
            //            {

            //                List<Transaction> transactionList = (from t in entity.Transactions
            //                                                     join c in entity.Customers
            //                                                     on t.CustomerId equals c.Id
            //                                                     where (t.CustomerId == CustomerId) && (c.Id == CustomerId) && (t.DateTime >= c.StartDate) && (t.DateTime <= c.ExpireDate)
            //                                                     && (t.IsDeleted == false) && (t.IsComplete == true)
            //                                                      && (t.PaymentTypeId == 1 || t.PaymentTypeId == 2 || t.PaymentTypeId == 3 || t.PaymentTypeId == 5 || t.PaymentTypeId == null)
            //                                                     && (t.Type != "Prepaid") && (t.Type != "Settlement") && (t.Type != "Refund") && (t.Type != "CreditRefund")
            //                                                     select t).ToList();

            //                List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
            //                                                                       join transactions in entity.Transactions
            //                                                                       on td.TransactionId equals transactions.ParentId
            //                                                                       join c in entity.Customers
            //                                                                       on transactions.CustomerId equals c.Id
            //                                                                       where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
            //                                                                       && (transactions.IsDeleted == false) && (transactions.IsComplete == true)
            //                                                                       && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
            //                                                                       && (transactions.Type != "Refund") && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
            //                                                                       select td).ToList();

            //                totalAmount = getTotalAmount;
            //                foreach (Transaction t in transactionList)
            //                {
            //                    totalAmount += Convert.ToInt32(t.TotalAmount);
            //                }
            //                foreach (Transaction t in transactionList)
            //                {
            //                    giftCardTotalAmount += Convert.ToInt32(t.GiftCardAmount);
            //                }
            //                totalAmount = totalAmount - giftCardTotalAmount;

            //                foreach (TransactionDetail td in RefundTransactionDetailList)
            //                {
            //                    totalRAmount += Convert.ToInt32(td.TotalAmount);
            //                }
            //                totalAmount = totalAmount - totalRAmount;

            //                foreach (MemberCardRule memberCard in memberCardRuleList)
            //                {
            //                    if (Convert.ToInt32(memberCard.RangeFrom) <= totalAmount)
            //                    {
            //                        string name = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Name).FirstOrDefault();
            //                        //int id = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();
            //                        cboMemberType.SelectedValue = memberCard.Id;
            //                        txtMemberType.Text = name;
            //                        cboMemberType.Text = name;

            //                        txtMemberDate.Text = Convert.ToString(customerObj.StartDate.Value.Date);
            //                        dtpMemberDate.Value = Convert.ToDateTime(customerObj.StartDate.Value.Date);
            //                        txtMID.Text = customerObj.VIPMemberId;


            //                    }
            //                }
            //            }

            //        }
            //        else //if yes
            //        {
            //            int _expireYear = customerObj.ExpireDate.Value.Year + memberValidityYear;

            //            int fromYear = customerObj.ExpireDate.Value.Year - memberValidityYear;
            //            int fromMonth = customerObj.ExpireDate.Value.Month;
            //            int fromDay = customerObj.ExpireDate.Value.Day;
            //            string fromDate = Convert.ToString(fromYear + "/" + fromMonth + "/" + fromDay);
            //            string expireDate = Convert.ToString(_expireYear + "/" + fromMonth + "/" + fromDay);

            //            DateTime _fromDate = Convert.ToDateTime(fromDate);
            //            DateTime _expireDate = Convert.ToDateTime(expireDate);
            //            string name = null;
            //            if (DateTime.Today.Date >= _fromDate && DateTime.Today.Date <= _expireDate)
            //            {
            //                List<Transaction> transactionList = (from transactions in entity.Transactions
            //                                                     join c in entity.Customers
            //                                                     on transactions.CustomerId equals c.Id
            //                                                     where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.DateTime >= _fromDate)
            //                                                     && (transactions.DateTime <= _expireDate) && (transactions.IsDeleted == false) && (transactions.IsComplete == true)
            //                                                     && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
            //                                                     && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "Refund") && (transactions.Type != "CreditRefund")
            //                                                     select transactions).ToList();

            //                List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
            //                                                                       join transactions in entity.Transactions
            //                                                                       on td.TransactionId equals transactions.ParentId
            //                                                                       join c in entity.Customers
            //                                                                       on transactions.CustomerId equals c.Id
            //                                                                       where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.IsDeleted == false) && (transactions.IsComplete == true)
            //                                                                       && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
            //                                                                       && (transactions.Type != "Refund") && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
            //                                                                       && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
            //                                                                       select td).ToList();

            //                totAmt = getTotalAmount;

            //                foreach (Transaction t in transactionList)
            //                {
            //                    totAmt += Convert.ToInt32(t.TotalAmount);
            //                }
            //                foreach (Transaction t in transactionList)
            //                {
            //                    giftCardTotalAmount += Convert.ToInt32(t.GiftCardAmount);
            //                }
            //                totAmt = totAmt - giftCardTotalAmount;
            //                foreach (TransactionDetail td in RefundTransactionDetailList)
            //                {
            //                    totalRAmount += Convert.ToInt32(td.TotalAmount);
            //                }
            //                totAmt = totAmt - totalRAmount;

            //                int _currentAmount = totAmt - getTotalAmount;
            //                if (_currentAmount >= Convert.ToInt32(minimumAmountofThisMemberType))
            //                {
            //                    foreach (MemberCardRule memberCards in memberCardRuleList)
            //                    {
            //                        if (Convert.ToInt32(memberCards.RangeFrom) <= totAmt)
            //                        {
            //                            name = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Name).FirstOrDefault();

            //                        }
            //                    }
            //                    txtMemberType.Text = name;
            //                    cboMemberType.Text = name;
            //                    txtMemberDate.Text = Convert.ToString(customerObj.ExpireDate.Value.Date);
            //                    dtpMemberDate.Value = Convert.ToDateTime(customerObj.ExpireDate);
            //                    txtMID.Text = customerObj.VIPMemberId;
            //                    isExpired = true;
            //                }
            //                else
            //                {
            //                    foreach (MemberCardRule memberCards in memberCardRuleList)
            //                    {
            //                        if (Convert.ToInt32(memberCards.RangeFrom) <= getTotalAmount)
            //                        {
            //                            name = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Name).FirstOrDefault();

            //                        }
            //                    }
            //                    txtMemberType.Text = name;
            //                    cboMemberType.Text = name;
            //                    txtMemberDate.Text = DateTime.Today.Date.ToShortDateString();
            //                    dtpMemberDate.Value = Convert.ToDateTime(DateTime.Today.Date.ToShortDateString());
            //                    txtMID.Text = customerObj.VIPMemberId;
            //                    isReNew = true;
            //                }

            //            }
            //            else
            //            {
            //                foreach (MemberCardRule memberCards in memberCardRuleList)
            //                {
            //                    if (Convert.ToInt32(memberCards.RangeFrom) <= getTotalAmount)
            //                    {
            //                        name = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Name).FirstOrDefault();

            //                    }
            //                }
            //                txtMemberType.Text = name;
            //                cboMemberType.Text = name;
            //                txtMemberDate.Text = DateTime.Today.Date.ToShortDateString();
            //                dtpMemberDate.Value = Convert.ToDateTime(DateTime.Today.Date.ToShortDateString());
            //                txtMID.Text = customerObj.VIPMemberId;
            //                isReNew = true;

            //            }
            //        }
            //    }


            //}
            #endregion

            //else
            //{ // customer name not equal "Default"
            if (cboCustomerName.SelectedIndex != 0 || cboCustomerName.SelectedIndex == -1)
            {
                int custID = Convert.ToInt32(cboCustomerName.SelectedValue);
                APP_Data.Customer customerObj = (from c in entity.Customers where c.Id == custID select c).FirstOrDefault();

                cboTitle.Text = customerObj.Title;
                txtPhoneNumber.Text = customerObj.PhoneNumber;
                txtNRC.Text = customerObj.NRC;
                txtEmail.Text = customerObj.Email;
                if (customerObj.Birthday == null)
                {
                    dtpBirthday.Value = DateTime.Now.Date;
                }
                else
                {
                    dtpBirthday.Value = customerObj.Birthday.Value.Date;
                }
                if (customerObj.Gender == "Male")
                {
                    rdbMale.Checked = true;
                }
                else
                {
                    rdbFemale.Checked = true;
                }
                cboCity.Text = customerObj.City.CityName;
                txtAddress.Text = customerObj.Address;
                txtMID.Text = customerObj.VIPMemberId;
                // end
                if (customerObj.MemberTypeID != null)
                {
                    mandatory = true;
                    // isexpired? == no
                    if (DateTime.Today.Date <= customerObj.ExpireDate)
                    {
                        if (customerObj.MemberTypeID == mTypeID) //if selected customer is the greatest
                        {
                            string _name = (from m in entity.MemberTypes where m.Id == customerObj.MemberTypeID select m.Name).FirstOrDefault();
                            txtMemberType.Text = _name;
                            cboMemberType.Text = _name;
                            txtMemberDate.Text = Convert.ToString(customerObj.StartDate.Value.Date);
                            dtpMemberDate.Value = Convert.ToDateTime(customerObj.StartDate.Value);
                            txtMID.Text = customerObj.VIPMemberId;
                        }
                        else
                        {

                            List<Transaction> transactionList = (from t in entity.Transactions
                                                                 join c in entity.Customers
                                                                 on t.CustomerId equals c.Id
                                                                 where (t.CustomerId == custID) && (c.Id == custID) && (t.DateTime >= c.StartDate) && (t.DateTime <= c.ExpireDate)
                                                                 && (t.IsDeleted == false) && (t.IsComplete == true) && (t.PaymentTypeId == 1 || t.PaymentTypeId == 2 || t.PaymentTypeId == 3 || t.PaymentTypeId == 5 || t.PaymentTypeId == null)
                                                                 && (t.Type != "Prepaid") && (t.Type != "Settlement") && (t.Type != "Refund") && (t.Type != "CreditRefund")
                                                                 select t).ToList();

                            List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                   join transactions in entity.Transactions
                                                                                   on td.TransactionId equals transactions.ParentId
                                                                                   join c in entity.Customers
                                                                                   on transactions.CustomerId equals c.Id
                                                                                   where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
                                                                                   && (transactions.IsDeleted == false) && (transactions.IsComplete == true) && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                                   && (transactions.Type != "Refund") && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
                                                                                   select td).ToList();

                            totalAmount = getTotalAmount;
                            foreach (Transaction t in transactionList)
                            {
                                totalAmount += Convert.ToInt32(t.TotalAmount);
                            }
                            foreach (Transaction t in transactionList)
                            {
                                giftCardTotalAmount += Convert.ToInt32(t.GiftCardAmount);
                            }

                            //Added by Lele
                            foreach (Transaction t in transactionList)
                            {
                                couponCodeTotalAmount += Convert.ToInt32(t.CouponCodeAmount);
                            }

                            //totalAmount = totalAmount - giftCardTotalAmount;
                            totalAmount = totalAmount - (giftCardTotalAmount+ couponCodeTotalAmount);

                            foreach (TransactionDetail td in RefundTransactionDetailList)
                            {
                                totalRAmount += Convert.ToInt32(td.TotalAmount);
                            }
                            totalAmount = totalAmount - totalRAmount;


                            foreach (MemberCardRule memberCard in memberCardRuleList)
                            {
                                if (Convert.ToInt32(memberCard.RangeFrom) <= totalAmount)
                                {
                                    string name = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Name).FirstOrDefault();
                                    //int id = (from m in entity.MemberTypes where m.Id == memberCard.MemberTypeId select m.Id).FirstOrDefault();
                                    cboMemberType.SelectedValue = memberCard.Id;
                                    txtMemberType.Text = name;

                                    cboMemberType.Text = name;
                                    txtMemberDate.Text = Convert.ToString(customerObj.StartDate.Value.Date);
                                    dtpMemberDate.Value = Convert.ToDateTime(customerObj.StartDate.Value);
                                    txtMID.Text = customerObj.VIPMemberId;

                                }
                            }
                        }

                    }
                    else //if yes
                    {
                        int _expireYear = customerObj.ExpireDate.Value.Year + memberValidityYear;

                        int fromYear = customerObj.ExpireDate.Value.Year - memberValidityYear;
                        int fromMonth = customerObj.ExpireDate.Value.Month;
                        int fromDay = customerObj.ExpireDate.Value.Day;
                        string fromDate = Convert.ToString(fromYear + "/" + fromMonth + "/" + fromDay);

                        string expireDate = Convert.ToString(_expireYear + "/" + fromMonth + "/" + fromDay);

                        DateTime _fromDate = Convert.ToDateTime(fromDate);
                        DateTime _expireDate = Convert.ToDateTime(expireDate);
                        string name = null;
                        if (DateTime.Today.Date >= _fromDate && DateTime.Today.Date <= _expireDate)
                        {
                            List<Transaction> transactionList = (from transactions in entity.Transactions
                                                                 join c in entity.Customers
                                                                 on transactions.CustomerId equals c.Id
                                                                 where (transactions.CustomerId == custID) && (c.Id == custID) && (transactions.DateTime >= _fromDate)
                                                                 && (transactions.DateTime <= _expireDate) && (transactions.IsDeleted == false)
                                                                 && (transactions.IsComplete == true) && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                 && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "Refund") && (transactions.Type != "CreditRefund")
                                                                 select transactions).ToList();

                            List<TransactionDetail> RefundTransactionDetailList = (from td in entity.TransactionDetails
                                                                                   join transactions in entity.Transactions
                                                                                   on td.TransactionId equals transactions.ParentId
                                                                                   join c in entity.Customers
                                                                                   on transactions.CustomerId equals c.Id
                                                                                   where (transactions.CustomerId == CustomerId) && (c.Id == CustomerId) && (transactions.IsDeleted == false)
                                                                                   && (transactions.IsComplete == true) && (transactions.PaymentTypeId == 1 || transactions.PaymentTypeId == 2 || transactions.PaymentTypeId == 3 || transactions.PaymentTypeId == 5 || transactions.PaymentTypeId == null)
                                                                                   && (transactions.Type != "Refund") && (transactions.DateTime >= c.StartDate) && (transactions.DateTime <= c.ExpireDate)
                                                                                   && (transactions.Type != "Prepaid") && (transactions.Type != "Settlement") && (transactions.Type != "CreditRefund")
                                                                                   select td).ToList();

                            totAmt = getTotalAmount;

                            foreach (Transaction t in transactionList)
                            {
                                totAmt += Convert.ToInt32(t.TotalAmount);
                            }
                            foreach (Transaction t in transactionList)
                            {
                                giftCardTotalAmount += Convert.ToInt32(t.GiftCardAmount);
                            }

                            //Added by Lele
                            foreach (Transaction t in transactionList)
                            {
                                couponCodeTotalAmount += Convert.ToInt32(t.CouponCodeAmount);
                            }

                            totAmt = totAmt - (giftCardTotalAmount + couponCodeTotalAmount);

                            foreach (TransactionDetail td in RefundTransactionDetailList)
                            {
                                totalRAmount += Convert.ToInt32(td.TotalAmount);
                            }
                            totAmt = totAmt - totalRAmount;

                            int _currentAmount = totAmt - getTotalAmount;
                            if (_currentAmount >= Convert.ToInt32(minimumAmountofThisMemberType))
                            {// aldy expired but can extend
                                foreach (MemberCardRule memberCards in memberCardRuleList)
                                {
                                    if (Convert.ToInt32(memberCards.RangeFrom) <= totAmt)
                                    {
                                        name = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Name).FirstOrDefault();
                                    }

                                }
                                txtMemberType.Text = name;
                                cboMemberType.Text = name;

                                int year = customerObj.ExpireDate.Value.Year - memberValidityYear;
                                int month = customerObj.ExpireDate.Value.Month;
                                int day = customerObj.ExpireDate.Value.Day;

                                string startDate = Convert.ToString(year + "/" + month + "/" + day);

                                txtMemberDate.Text = startDate.ToString();
                                dtpMemberDate.Value = Convert.ToDateTime(startDate);
                                txtMID.Text = customerObj.VIPMemberId;
                                isExpired = true;
                            }
                            else
                            {// cant extend to next year, so renew
                                foreach (MemberCardRule memberCards in memberCardRuleList)
                                {
                                    if (Convert.ToInt32(memberCards.RangeFrom) <= getTotalAmount)
                                    {
                                        name = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Name).FirstOrDefault();
                                    }
                                }
                                txtMemberType.Text = name;
                                cboMemberType.Text = name;
                                txtMemberDate.Text = DateTime.Today.Date.ToShortDateString();
                                dtpMemberDate.Value = Convert.ToDateTime(DateTime.Today.Date.ToShortDateString());
                                txtMID.Text = customerObj.VIPMemberId;
                                isReNew = true;
                            }
                        }
                        else
                        {// cant extend to next year, so renew
                            foreach (MemberCardRule memberCards in memberCardRuleList)
                            {
                                if (Convert.ToInt32(memberCards.RangeFrom) <= getTotalAmount)
                                {
                                    name = (from m in entity.MemberTypes where m.Id == memberCards.MemberTypeId select m.Name).FirstOrDefault();

                                }
                            }
                            txtMemberType.Text = name;
                            cboMemberType.Text = name;
                            txtMemberDate.Text = DateTime.Today.Date.ToShortDateString();
                            dtpMemberDate.Value = Convert.ToDateTime(DateTime.Today.Date.ToShortDateString());
                            txtMID.Text = customerObj.VIPMemberId;
                            isReNew = true;


                        }

                    }
                }

            }
            //}
        }

        // By SYM
        private void btnCustomerSearch_Click(object sender, EventArgs e)
        {
            cboCustomerName.Visible = true;
            txtName.Visible = false;
            btnCustomerAdd.Visible = true;
            Bind_City();
            Bind_Customer();

            btnSubmit.Image = POS.Properties.Resources.update_big;
            isEdit = true;

        }
        // By SYM
        private void btnCustomerAdd_Click(object sender, EventArgs e)
        {
            cboCustomerName.Visible = false;
            txtName.Visible = true;
            btnCustomerAdd.Visible = false;

            btnSubmit.Image = POS.Properties.Resources.save_big;
            isEdit = false;

            cboTitle.Text = "Mr";
            txtAddress.Text = "";
            txtEmail.Text = "";
            txtName.Text = "";
            txtNRC.Text = "";
            txtPhoneNumber.Text = "";
            dtpBirthday.Value = DateTime.Now.Date;
            rdbMale.Checked = true;
            Bind_Customer();
            //txtMemberType.Text = "";
            //txtMemberDate.Text = "";
            txtMID.Text = "";

            int cityId = 0;
            cityId = SettingController.DefaultCity;
            APP_Data.City cus2 = (from c in entity.Cities where c.Id == cityId select c).FirstOrDefault();
            cboCity.Text = cus2.CityName;

        }

        public void NewCustomer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsUpdateOrSave == true)
            {
                IsClosed = false;
            }
            else
            {
                if (Type == 'S')
                {
                    if (currentCustomer.Name != "Default" && currentCustomer.PromoteDate != null)
                    {
                        MessageBox.Show("Please you need to update Member Type", "Warning");
                        e.Cancel = true;
                    }
                    else if (cboCustomerName.SelectedIndex != 0 && cboCustomerName.SelectedIndex != -1 && mandatory == true)
                    {
                        MessageBox.Show("Please you need to update Member Type", "Warning");
                        e.Cancel = true;
                    }


                }
                else
                {
                    IsClosed = true;

                }
            }
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }


    }
}
