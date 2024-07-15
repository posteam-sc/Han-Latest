using POS.APP_Data;
using POS.POSInterfaceServiceReference;
using POS.View.Setting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace POS
{
    public partial class MDIParent : Form
    {
        #region Events
        // By SYM
        DateTime fromTime;
        DateTime toTime;
        bool isRunning = false;
        private int childFormNumber = 0;

        public MDIParent()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {// By SYM
            string address = "https://jc-dc.junctionv-aap.com/wsSSI/wsSSIWebService.asmx";
            IsAddressAvailable(address);
            if (isRunning == true)
            {
                Push_DataToWebService();
            }

        }

        public void IsAddressAvailable(string address)
        {// By SYM
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                client.DownloadData(address);
                isRunning = true;
            }
            catch
            {
                isRunning = false;
            }

        }

        public void Push_DataToWebService()
        {// By SYM  

            if (SettingController.At_JunctionCity == "1")
            {
                toTime = DateTime.Now;
                Parameters _parameters = new Parameters();
                Transactions _transaction = new Transactions();
                List<Transactions> _transactionList = new List<Transactions>();

                SqlConnection _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);
                _conn.Open();
                SqlCommand _com = new SqlCommand("JC_POSInterface", _conn);
                _com.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromTime;
                _com.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toTime;
                _com.CommandType = CommandType.StoredProcedure;
                SqlDataReader dataReader = _com.ExecuteReader();
                DataTable dt = new DataTable("Transactions");
                fromTime = DateTime.Now;
                dt.Load(dataReader);

                _conn.Close();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        _transactionList.Add(new Transactions()
                        {
                            Col_01 = dt.Rows[i]["mallCode"].ToString(),
                            Col_02 = dt.Rows[i]["posID"].ToString(),
                            Col_03 = dt.Rows[i]["transactionDate"].ToString(),
                            Col_04 = dt.Rows[i]["transactionTime"].ToString(),
                            Col_05 = dt.Rows[i]["transactionNo"].ToString(),
                            Col_06 = dt.Rows[i]["itemQty"].ToString(),
                            Col_07 = dt.Rows[i]["saleAmtCurrency"].ToString(),
                            Col_08 = dt.Rows[i]["totAmtNoTax"].ToString(),
                            Col_09 = dt.Rows[i]["TotAmtWithTax"].ToString(),
                            Col_10 = dt.Rows[i]["tax"].ToString(),
                            Col_11 = dt.Rows[i]["serviceChargeAmt"].ToString(),
                            Col_12 = dt.Rows[i]["payAmtCurrency"].ToString(),
                            Col_13 = dt.Rows[i]["paymentAmt"].ToString(),
                            Col_14 = dt.Rows[i]["paymentType"].ToString(),
                            Col_15 = dt.Rows[i]["saleType"].ToString(),
                            Col_16 = "",
                            Col_17 = "",
                            Col_18 = "",
                            Col_19 = "",
                            Col_20 = ""

                        });
                    }
                    _parameters.Application_ID = SettingController.Application_ID;
                    _parameters.Col_01 = SettingController.Mall_Code;
                    _parameters.Col_02 = SettingController.POS_ID;
                    _parameters.Col_03 = "";
                    string _todayDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                    _parameters.TimeStamp = _todayDate.ToString();
                    _parameters.Columns = _transactionList.ToArray();


                    wsSSIAuthentication _wsSSIAuthentication = new wsSSIAuthentication();
                    _wsSSIAuthentication.applicationKey = SettingController.Application_Key;
                    _wsSSIAuthentication.encryptedKey = SettingController.Encrypted_Key;

                    POSInterfaceServiceReference.wsSSIWebServiceSoapClient soapClient = new POSInterfaceServiceReference.wsSSIWebServiceSoapClient();
                    POSInterfaceServiceReference.ws_SSI_SendDataRS rqResponse = soapClient.ws_SSI_SendDataRQ(_wsSSIAuthentication, _parameters);

                    string _transactionID = rqResponse.TransactionID;
                    string return_status = rqResponse.ReturnStatus;
                    int record_Received = rqResponse.RecordsReceived;
                    int record_Imported = rqResponse.RecordsImported;
                    string error_Detail = rqResponse.ErrorDetails;
                    string defective_RowNos = rqResponse.DefectiveRowNos;

                    //string _msg = return_status + '-' + record_Received;
                    //MessageBox.Show(_msg);


                }
            }

        }

        private void MDIParent_Load(object sender, EventArgs e)
        {
            Create_RequirementsForDB();
            //ELC_CustomerPointSystem.Get_ExpiredMemberList_And_Update_ExpiredMember();
            if (!Utility.IsRegister())
            {
                Register regform = new Register();
                regform.WindowState = FormWindowState.Maximized;
                regform.MdiParent = this;
                regform.Show();
                this.menuStrip.Enabled = false;
                //this.menuStrip.Visible = false;
            }
            else if (!MemberShip.isLogin)
            {
                Login loginForm = new Login();
                loginForm.MdiParent = this;
                loginForm.WindowState = FormWindowState.Maximized;
                loginForm.Show();
                this.menuStrip.Enabled = false;
                // this.menuStrip.Visible = false;
                toolStripStatusLabel.Text = string.Empty;
                toolStripStatusLabel1.Text = string.Empty;
                tSSBOOrPOS.Text = string.Empty;

            }
           
            else
            {
                if ((Application.OpenForms["Sales"] as Sales) != null)
                {
                    //Form is already open
                }
                else
                {
                    Sales form = new Sales();
                    form.WindowState = FormWindowState.Maximized;
                    form.MdiParent = this;

                    form.Show();
                }


                //Sales form = new Sales();
                //form.WindowState = FormWindowState.Maximized;
                //form.MdiParent = this;
                //form.Show();
            }

            // By SYM // JC POS UI Interface

            if (SettingController.At_JunctionCity == "1")
            {
                Timer MyTimer = new Timer();
                MyTimer.Interval = (15 * 60 * 1000); // 15 mins
                MyTimer.Tick += new EventHandler(MyTimer_Tick);
                MyTimer.Start();
                fromTime = DateTime.Now;
            }
            POSEntities entity = new POSEntities();
            var currentset = entity.Settings.Where(x => x.Key == "IsAdminShop" && x.Value == "1").FirstOrDefault();
            if (currentset == null)
            {
                vIPMemberDoublePromotionRuleToolStripMenuItem.Visible = false;
              
            }
            
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.ShowDialog();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void productTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProductSubCategory newform = new ProductSubCategory();
            newform.ShowDialog();
        }



        private void customerListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.OutstandingCustomer.View || MemberShip.isAdmin)
            {
                OutstandingCustomerList form = new OutstandingCustomerList();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view outstanding customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void productCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProductCategory form = new ProductCategory();
            form.ShowDialog();
        }

        private void addNewCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Customer.Add || MemberShip.isAdmin)
            {
                //mType form = new mType();
                NewCustomer form = new NewCustomer();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void brandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Brand newForm = new Brand();
            newForm.ShowDialog();
        }

        private void counterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Counter newForm = new Counter();
            newForm.ShowDialog();
        }

        private void userListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserControl form = new UserControl();
            form.ShowDialog();
        }

        private void newUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewUser form = new NewUser();
            form.ShowDialog();
        }
        private void transactionSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.TransactionSummaryReport.View || MemberShip.isAdmin)
            {
                TransactionSummary newform = new TransactionSummary();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view transaction summary report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }



        }

        private void newProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProduct newForm = new NewProduct();
            newForm.ShowDialog();
        }

        private void productListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemList newForm = new ItemList();
            newForm.ShowDialog();
        }

        private void giftCardControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GiftCardControl newForm = new GiftCardControl();
            newForm.ShowDialog();
        }

        private void transactionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransactionList newForm = new TransactionList();
            newForm.ShowDialog();
        }

        private void startDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartDay form = new StartDay();
            form.ShowDialog();
        }

        private void endDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EndDay form = new EndDay();
            form.ShowDialog();
        }

        private void refundListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefundList newform = new RefundList();
            newform.ShowDialog();
        }

        private void unitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnitForm newform = new UnitForm();
            newform.ShowDialog();
        }

        private void logInToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Boolean isAlreadyHave = false;

            foreach (Form child in this.MdiChildren)
            {
                if (child.Text == "Login")
                {
                    child.Activate();
                    isAlreadyHave = true;
                }
                else
                {
                    child.Close();
                }
            }
            if (!isAlreadyHave)
            {
                Login f = new Login();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {
                Boolean isAlreadyHave = false;

                foreach (Form child in this.MdiChildren)
                {
                    if (child.Text == "LogIn")
                    {
                        child.Activate();
                        isAlreadyHave = true;
                    }
                    else
                    {
                        child.Close();
                    }
                }
                MemberShip.UserId = 0;
                MemberShip.UserName = "";
                MemberShip.UserRole = null;
                //MemberShip.isAdmin = false;
                //MemberShip.isLogin = false;

                if (!isAlreadyHave)
                {
                    Login f = new Login();
                    f.MdiParent = this;
                    f.WindowState = FormWindowState.Maximized;                    
                    f.Show();

                    //DisableControls();
                }
                menuStrip.Enabled = false;
                toolStripStatusLabel.Text = string.Empty;
                toolStripStatusLabel1.Text = string.Empty;
                tSSBOOrPOS.Text = "Log Out";
                logOutToolStripMenuItem.Visible = false;
                logInToolStripMenuItem1.Visible = true;
            }
        }

        private void creditTransactionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreditTransactionList newForm = new CreditTransactionList();
            newForm.ShowDialog();
        }

        private void transactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.TransactionReport.View || MemberShip.isAdmin)
            {
                TransactionReport_FOC_MPU newform = new TransactionReport_FOC_MPU();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view transaction report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void itemSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.ItemSummaryReport.View || MemberShip.isAdmin)
            {
                ItemSummary newform = new ItemSummary();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view item sale summary report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void roleManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagement newform = new RoleManagement();
            newform.ShowDialog();
        }

        private void taxesSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.TaxSummaryReport.View || MemberShip.isAdmin)
            {
                TaxesSummary newform = new TaxesSummary();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view tax summary report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void addCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.City.Add || MemberShip.isAdmin)
            {
                City newForm = new City();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add city", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void deleteLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteLogForm newform = new DeleteLogForm();
            newform.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting newform = new Setting();
            newform.ShowDialog();
        }

        private void itemPurchaseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.ReorderPointReport.View || MemberShip.isAdmin)
            {
                PurchaseOrderItem newform = new PurchaseOrderItem();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view reorder point report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void transactionDetailByItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.TransactionDetailReport.View || MemberShip.isAdmin)
            {
                TransactionDetailByItem newform = new TransactionDetailByItem();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view transaction detail report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void taxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Taxes newform = new Taxes();
            newform.ShowDialog();
        }

        private void outstandingCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.OutstandingCustomerReport.View || MemberShip.isAdmin)
            {
                OutstandingCustomerReport newform = new OutstandingCustomerReport();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view outstanding report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void customerListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Customer.View || MemberShip.isAdmin)
            {
                CustomerList newForm = new CustomerList();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view customer list", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void addNewProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Product.Add || MemberShip.isAdmin)
            {
                NewProduct newForm = new NewProduct();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add new product", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void productCategoryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Category.View || MemberShip.isAdmin)
            {
                ProductCategory form = new ProductCategory();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add product category", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void productSubCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.SubCategory.View || MemberShip.isAdmin)
            {
                ProductSubCategory newform = new ProductSubCategory();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view sub product category", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void brandToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Brand.View || MemberShip.isAdmin)
            {
                Brand newForm = new Brand();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view brand", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void productListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Product.View || MemberShip.isAdmin)
            {
                ItemList newForm = new ItemList();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view product", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void addNewUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewUser form = new NewUser();
            form.ShowDialog();
        }

        private void userListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UserControl form = new UserControl();
            form.ShowDialog();
        }

        private void roleManagementToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RoleManagement newform = new RoleManagement();
            newform.ShowDialog();
        }

        private void configurationSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting newform = new Setting();
            newform.ShowDialog();
        }

        private void counterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Counter.Add || MemberShip.isAdmin)
            {
                Counter newForm = new Counter();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add counter", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void giftCardContToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.GiftCard.Add || MemberShip.isAdmin)
            {
                GiftCardControl newForm = new GiftCardControl();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add gift card.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void measurementUnitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.MeasurementUnit.Add || MemberShip.isAdmin)
            {
                UnitForm newform = new UnitForm();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add measurement unit.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void taxRatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.TaxRate.Add || MemberShip.isAdmin)
            {
                Taxes newform = new Taxes();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add tax rate.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void transactionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TransactionList newForm = new TransactionList();
            newForm.ShowDialog();
        }

        private void creditTransactionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CreditTransactionList newForm = new CreditTransactionList();
            newForm.ShowDialog();
        }

        private void refundListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RefundList newform = new RefundList();
            newform.ShowDialog();
        }


        private void form1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Form1 f = new Form1();
            //f.ShowDialog();
        }

        private void addSupplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewSupplier newForm = new NewSupplier();
            newForm.ShowDialog();
        }

        private void supplierListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierList newForm = new SupplierList();
            newForm.ShowDialog();
        }

        private void productPurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseInput newForm = new PurchaseInput();
            newForm.ShowDialog();
        }

        private void newPurchaseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.PurchaseRole.Add || MemberShip.isAdmin)
            {
                PurchaseInput newForm = new PurchaseInput();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add new purchasing", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void purchaseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.PurchaseRole.View || MemberShip.isAdmin)
            {
                PurchaseListBySupplier newform = new PurchaseListBySupplier();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view purchase list", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void databaseExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Backup(true);
        }

        private void databaseImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = Backup(false);
            Restore(ref fileName);
        }

        private void MDIParent_FormClosing(object sender, FormClosingEventArgs e)
        {
            toolStripStatusLabel.Text = "Saving data.. Please wait";

            // By SYM
            string address = "http://54.255.175.164/wsSSI/wsSSIWebService.asmx";
            IsAddressAvailable(address);
            if (isRunning == true)
            {
                Push_DataToWebService();
            }

            //Only main server will make backup
            if (DatabaseControlSetting._ServerName.ToUpper().StartsWith(System.Environment.MachineName.ToUpper()))
            {
                Backup(true);
            }
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.TopBestSellerReport.View || MemberShip.isAdmin)
            {
                TopSaleReport newForm = new TopSaleReport();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view best seller item  report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }
        private void addConsigmentCounterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Consignor.Add || MemberShip.isAdmin)
            {
                ConsignmentProductCounter newForm = new ConsignmentProductCounter();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add counter", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private void consignmentCounterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Consigment.View || MemberShip.isAdmin)
            {
                ConsignmentProductReport newForm = new ConsignmentProductReport();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view consigment report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }



        }


        #endregion

        #region Functions

        private void Create_RequirementsForDB()
        {
            POSEntities entity = new POSEntities();
            // First Time Running... and Database is Null / Create a default Admin User
            var _userRoleList = entity.UserRoles.ToList();

            if (_userRoleList.Count == 0)
            {
                #region UserRole
                UserRole _userRole = new UserRole();
                _userRole.RoleName = "Admin";
                entity.UserRoles.Add(_userRole);
                entity.SaveChanges();

                _userRole.RoleName = "Super Cashier";
                entity.UserRoles.Add(_userRole);
                entity.SaveChanges();

                _userRole.RoleName = "Cashier";
                entity.UserRoles.Add(_userRole);
                entity.SaveChanges();
                #endregion

                #region City
                APP_Data.City _city = new APP_Data.City();
                _city.CityName = "Yangon";
                _city.IsDelete = false;
                entity.Cities.Add(_city);
                entity.SaveChanges();
                #endregion


                #region Shop
                APP_Data.Shop _shop = new APP_Data.Shop();
                _shop.ShopName = "MainOffice";
                _shop.CityId = 1;
                _shop.ShortCode = "MO";
                _shop.IsDefaultShop = true;

                entity.Shops.Add(_shop);
                entity.SaveChanges();
                #endregion

                #region Currency
                Currency _currency = new Currency();
                _currency.Country = "Myanmar";
                _currency.Symbol = "Ks";
                _currency.CurrencyCode = "MMK";
                _currency.LatestExchangeRate = 1;
                entity.Currencies.Add(_currency);
                entity.SaveChanges();

                _currency.Country = "United State of America";
                _currency.Symbol = "$";
                _currency.CurrencyCode = "USD";
                _currency.LatestExchangeRate = 1000;
                entity.Currencies.Add(_currency);
                entity.SaveChanges();
                #endregion

                #region Tax
                Tax _tax = new Tax();
                _tax.Name = "None";
                _tax.TaxPercent = 0;
                _tax.IsDelete = false;
                entity.Taxes.Add(_tax);
                entity.SaveChanges();
                #endregion

                #region Setting
                APP_Data.Setting _setting = new APP_Data.Setting();
                _setting.Key = "barcode_printer";
                _setting.Value = "Plz Choose Bar Code Printer";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "a4_printer";
                _setting.Value = "Plz Choose A4 Printer";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "slip_printer_counter1";
                _setting.Value = "Plz Choose Slip Printer";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "shop_name";
                _setting.Value = "Plz Enter Shop Name";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "branch_name";
                _setting.Value = "Plz Enter Branch Name or Address";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "phone_number";
                _setting.Value = "Plz Enter Phone Number";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "opening_hours";
                _setting.Value = "Plz Enter Opening Hours";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "default_tax_rate";
                _setting.Value = "1";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "default_couponcode_id";
                _setting.Value = "1";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "default_city_id";
                _setting.Value = "1";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "default_top_sale_row";
                _setting.Value = "1";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "default_currency";
                _setting.Value = "1";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "Company_StartDate";
                _setting.Value = DateTime.Now.ToString("dd/MM/yyyy");
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "default_printer";
                _setting.Value = "Plz choose Default Printer";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "IsBackOffice";
                _setting.Value = "1";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                _setting.Key = "default_member_validity_year";
                _setting.Value = "1";
                entity.Settings.Add(_setting);
                entity.SaveChanges();

                #endregion

                #region User
                string UserCodeNo = "";
                User _user = new User();
                _user.ShopId = 1;
                _user.Name = "admin";
                _user.UserRoleId = 1;
                _user.Password = Utility.EncryptString("123456", "SCPos");
                _user.DateTime = System.DateTime.Now;
                _user.MenuPermission = "Both";

                //to get user code No
                UserCodeNo = Utility.Get_UserCodeNo(1);

                _user.UserCodeNo = UserCodeNo;
                entity.Users.Add(_user);
                entity.SaveChanges();


                _user.ShopId = 1;
                _user.Name = "sourcecodeadmin";
                _user.UserRoleId = 1;
                _user.Password = Utility.EncryptString("Sourcec0de", "SCPos");
                _user.DateTime = System.DateTime.Now;
                _user.MenuPermission = "Both";
                //to get user code No
                UserCodeNo = Utility.Get_UserCodeNo(1);

                _user.UserCodeNo = UserCodeNo;

                entity.Users.Add(_user);
                entity.SaveChanges();
                #endregion

                #region Counter
                APP_Data.Counter _counter = new APP_Data.Counter();
                _counter.Name = "One";
                _counter.IsDelete = false;
                entity.Counters.Add(_counter);
                entity.SaveChanges();
                #endregion

                #region PointHistory
                APP_Data.Point_History Normal = new APP_Data.Point_History();
                Normal.CreatedBy = (from p in entity.Users select p.Id).FirstOrDefault();
                Normal.CreatedDate = DateTime.Now;
                Normal.BrandId = null;
                Normal.Point = 1;
                Normal.PRPointAmount = 10000;
                Normal.Status = "Normal";
                entity.Point_History.Add(Normal);
                entity.SaveChanges();
                #endregion

                #region Customer
                APP_Data.Customer _customer = new APP_Data.Customer();
                _customer.Name = "Default";
                _customer.CityId = 1;
                _customer.Title = "Mr";
                _customer.PhoneNumber = "";
                _customer.Address = "";
                _customer.NRC = "";
                _customer.Email = "";
                _customer.Birthday = DateTime.Now;
                _customer.Gender = "";
                _customer.VIPMemberId = "";
                _customer.TownShip = "";

                var _customercode = entity.CustomerAutoID(DateTime.Now, SettingController.DefaultShop.ShortCode);
                _customer.CustomerCode = _customercode.FirstOrDefault().ToString();
                entity.Customers.Add(_customer);
                entity.SaveChanges();
                #endregion

                #region PaymentType
                APP_Data.PaymentType _paymentType = new APP_Data.PaymentType();
                _paymentType.Name = "Cash";
                entity.PaymentTypes.Add(_paymentType);
                entity.SaveChanges();

                _paymentType.Name = "Credit";
                entity.PaymentTypes.Add(_paymentType);
                entity.SaveChanges();

                _paymentType.Name = "GiftCard";
                entity.PaymentTypes.Add(_paymentType);
                entity.SaveChanges();

                _paymentType.Name = "FOC";
                entity.PaymentTypes.Add(_paymentType);
                entity.SaveChanges();

                _paymentType.Name = "MPU";
                entity.PaymentTypes.Add(_paymentType);
                entity.SaveChanges();

                _paymentType.Name = "Tester";
                entity.PaymentTypes.Add(_paymentType);
                entity.SaveChanges();
                #endregion

            }
            else
            {

                APP_Data.Point_History pointDefault = (from p in entity.Point_History where p.Status == "Normal" select p).FirstOrDefault();
                if (pointDefault == null)
                {
                    APP_Data.Point_History Normal = new APP_Data.Point_History();
                    Normal.CreatedBy = (from p in entity.Users select p.Id).FirstOrDefault();
                    Normal.CreatedDate = DateTime.Now;
                    Normal.BrandId = null;
                    Normal.Point = 1;
                    Normal.PRPointAmount = 10000;
                    Normal.Status = "Normal";
                    entity.Point_History.Add(Normal);
                    entity.SaveChanges();
                }
            }

        }

        private void Restore(ref string fileName)
        {
            if (MessageBox.Show("Are you sure that you want to restore", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (ofdDBBackup.ShowDialog(this) == DialogResult.Cancel)
                {
                    return;
                }

                POSEntities entity = new POSEntities();
                entity.ClearDBConnections();

                fileName = ofdDBBackup.FileName;
                string destFileName = string.Empty;
                string[] fnArr1 = fileName.Split('_');
                string[] fnArr2 = fileName.Split('.');
                string[] fnArr3 = fileName.Split('\\');
                string filePath = string.Empty;
                for (int i = 0; i < fnArr3.Length - 1; i++)
                {
                    if (i + 1 != fnArr3.Length - 1)
                    {
                        filePath += fnArr3[i] + "/";
                    }
                    else
                    {
                        filePath += fnArr3[i];
                    }
                }

                /*--Decrypt DB--*/
                for (int i = 0; i < fnArr1.Length - 1; i++)
                {
                    if (i + 1 != fnArr1.Length - 1)
                    {
                        destFileName += fnArr1[i] + "_";
                    }
                    else
                    {
                        destFileName += fnArr1[i];
                    }
                }
                destFileName = destFileName + "." + fnArr2[1];
                if (File.Exists(destFileName)) File.Delete(destFileName);

                Utility.DecryptFile(fileName, destFileName);

                /*--Restore DB--*/
                RestoreHelper restoreHelper = new RestoreHelper();

                string[] tempConString = Properties.Settings.Default.MyConnectionString.Split(';');
                string[] userNameArr = tempConString[tempConString.Length - 2].Split('=');
                string[] passwordArr = tempConString[tempConString.Length - 1].Split('=');

                //restoreHelper.RestoreDatabase(Utility._DBName, destFileName + "." + fnArr2[1], Utility._ServerName, userNameArr[userNameArr.Length - 1], passwordArr[passwordArr.Length - 1]);
                restoreHelper.RestoreDatabase(DatabaseControlSetting._DBName, destFileName, DatabaseControlSetting._ServerName, DatabaseControlSetting._DBUser, DatabaseControlSetting._DBPassword);
                try
                {
                    if (File.Exists(destFileName))
                    {
                        File.Delete(destFileName);
                    }
                    MessageBox.Show("Successfully Restored..");
                }
                catch
                {
                    MessageBox.Show("Can't remove temp files", "Error!!");
                }
            }
        }

        private string Backup(Boolean IsManual)
        {
            string activeDir = @"D:\";

            /*-- Create a new subfolder under the current active folder --*/
            string newPath;
            if (IsManual)
            {
                newPath = System.IO.Path.Combine(activeDir, "Manual_Backups");
            }
            else
            {
                newPath = System.IO.Path.Combine(activeDir, "DB_Backups");
            }

            if (!System.IO.Directory.Exists(newPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(newPath);
                //di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            /*-- Backup DB --*/
            BackupHelper backHelper = new BackupHelper();
            string fileName;

            if (IsManual)
                fileName = "D:/Manual_Backups/" + DatabaseControlSetting._DBName + "[" + DateTime.Now.ToString("dd-MM-yyyy hh-mm tt") + "].bak";
            else
                fileName = "D:/DB_Backups/" + DatabaseControlSetting._DBName + "[" + DateTime.Now.ToString("dd-MM-yyyy hh-mm tt") + "].bak";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            bool isBackup = false;
            backHelper.BackupDatabase(DatabaseControlSetting._DBName, DatabaseControlSetting._DBUser, DatabaseControlSetting._DBPassword, DatabaseControlSetting._ServerName, fileName, ref isBackup);
           // KHS
            /*-- Encrypt DB --*/
            string[] fileNameArr = fileName.Split('\\');
            string[] encryptFileNameArr = fileName.Split('.');
            string tempFileName = encryptFileNameArr[0] + "_encrypted." + encryptFileNameArr[1];

            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
            if (isBackup)
            {
                Utility.EncryptFile(fileName, tempFileName);
            }

            try
            {
                File.Delete(fileName);
                if (isBackup)
                {
                    MessageBox.Show("Successfully Exported to " + newPath);
                }
            }
            catch
            {
                MessageBox.Show("Can't remove temporary files");
            }
            return fileName;
        }

        #endregion

        private void customerInfomationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.CustomerInformation.View || MemberShip.isAdmin)
            {
                CustomerInformation newform = new CustomerInformation();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view customer information report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void productReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.ProductReport.View || MemberShip.isAdmin)
            {
                ProductReport_frm newform = new ProductReport_frm();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view product report report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void customersSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.CustomerSales.View || MemberShip.isAdmin)
            {
                frmCustomerSaleReport newform = new frmCustomerSaleReport();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view customer sale report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void saleBreakDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.SaleBreakdown.View || MemberShip.isAdmin)
            {
                SaleBreakDown newform = new SaleBreakDown();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view sale breakdown report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }



        }

        private void productPriceChangeHistoryListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PriceChangeHistoryList newform = new PriceChangeHistoryList();
            newform.ShowDialog();
        }

        private void saleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if ((Application.OpenForms["Sales"] as Sales) != null)
            {
                //Form is already open
            }
            else
            {
                Sales form = new Sales();
                form.WindowState = FormWindowState.Maximized;
                form.MdiParent = this;
                form.Show();
            }
        }

        private void dailySummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POSEntities entites = new POSEntities();
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.DailySaleSummary.View || MemberShip.isAdmin)
            {

                var shop = entites.Shops.Where(x => x.IsDefaultShop == true).FirstOrDefault();
                if (shop.ShortCode == "MO")
                {
                    frmDailySaleReportForMainOffice DailySaleReportForMainOffice = new frmDailySaleReportForMainOffice();
                    DailySaleReportForMainOffice.ShowDialog();
                }
                else
                {
                    Loc_ItemSummary newForm = new Loc_ItemSummary();
                    newForm.ShowDialog();
                }

            }
            else
            {
                MessageBox.Show("You are not allowed to view daily summary report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("MPOS.chm");
        }

        private void purchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.PurchaseReport.View || MemberShip.isAdmin)
            {
                PurchaseReport newform = new PurchaseReport();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view purchase report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void dailyTotalTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.DailyTotalTransactions.View || MemberShip.isAdmin)
            {
                DailyTotalReport newform = new DailyTotalReport();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view daily total transaction report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void purchaseDeleteLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.PurchaseRole.View || MemberShip.isAdmin)
            {
                PurchaseDeleteLog_frm newform = new PurchaseDeleteLog_frm();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view purchase delete", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void profitAndLossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.ProfitAndLoss.View || MemberShip.isAdmin)
            {
                ProfitAndLoss_frm newform = new ProfitAndLoss_frm();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view  profit and loss report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void purchaseDiscountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.PurchaseDiscount.View || MemberShip.isAdmin)
            {
                PurchaseDiscountReport_frm newform = new PurchaseDiscountReport_frm();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view purchase discount report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void addMemeberRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MemberRule newMember = new MemberRule();
            newMember.ShowDialog();
        }

        private void averageMonthlyReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.AverageMonthlyReport.View || MemberShip.isAdmin)
            {
                AverageMonthlySaleReport_frm avgMonthlyReport = new AverageMonthlySaleReport_frm();
                avgMonthlyReport.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view transaction detail report", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void currencyExchangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.CurrencyExchange.Add || MemberShip.isAdmin)
            {
                ExchangeRate newForm = new ExchangeRate();
                newForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add currency exchange", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void outstandingSupplierListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.OutstandingSupplier.View || MemberShip.isAdmin)
            {
                OutstandingSupplierList form = new OutstandingSupplierList();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view outstanding supplier list", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void addNewAdjustmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Adjustment.Add || MemberShip.isAdmin)
            {
                AdjustmentFrm form = new AdjustmentFrm();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add damage.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void AdjustmentListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Adjustment.View || MemberShip.isAdmin)
            {
                AdjustmentListFrm form = new AdjustmentListFrm();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view damage.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void AdjustmentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.AdjustmentReport.View || MemberShip.isAdmin)
            {
                AdjustmentRpt form = new AdjustmentRpt();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view damage report.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void AdjustmentDeleteLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Adjustment.EditOrDelete || MemberShip.isAdmin)
            {
                AdjustmentDeleteLog form = new AdjustmentDeleteLog();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view damage delete log.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }



        private void stockTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockTransaction form = new StockTransaction();
            form.ShowDialog();
        }

        private void stockTransactionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.StockTransactionReport.View || MemberShip.isAdmin)
            {
                StockTransactionReport form = new StockTransactionReport();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view stock transaction report.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void consignmentPaidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.ConsigmentSettlement.EditOrDelete || MemberShip.isAdmin)
            {
                ConsignmentSettlement form = new ConsignmentSettlement();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view stock transaction report.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void consignmentSettlementListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.ConsigmentSettlement.View || MemberShip.isAdmin)
            {
                ConsignmentSettlementList form = new ConsignmentSettlementList();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view stock transaction report.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void stockUnitConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.UnitConversion.Add || MemberShip.isAdmin)
            {
                UnitConversionfrm form = new UnitConversionfrm();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add stock unit conversion.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void stockUnitConversionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.UnitConversion.View || MemberShip.isAdmin)
            {
                UnitConversionListfrm form = new UnitConversionListfrm();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view stock unit conversion.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void createExpenseEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Expense.Add || MemberShip.isAdmin)
            {
                ExpenseEntry form = new ExpenseEntry();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to create expense.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void expenseListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Expense.View || MemberShip.isAdmin)
            {
                ExpenseList form = new ExpenseList();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view expense list.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void expenseDeleteLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Expense.DeleteLog || MemberShip.isAdmin)
            {
                ExpenseDeleteLog form = new ExpenseDeleteLog();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view expense delete log.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                MessageBox.Show("You are not allowed to view expense delete log.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void addExpenseCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.ExpenseCategory.Add || MemberShip.isAdmin)
            {
                ExpenseCategory form = new ExpenseCategory();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add expense category.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void netIncomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.NetIncomeReport.View || MemberShip.isAdmin)
            {
                frmNetIncomeReport form = new frmNetIncomeReport();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to view net income report.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void addShopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shop form = new Shop();
            form.ShowDialog();
        }

        private void stockInOutReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockTransReturnForm _form = new StockTransReturnForm();
            _form.ShowDialog();
        }

        private void stockTransactionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockReceiveList _form = new StockReceiveList();
            _form.ShowDialog();
        }

        private void centralizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Backup(false);
            Centralized _frm = new Centralized();
            _frm.ShowDialog();
        }

        private void tSSBOOrPOS_Click(object sender, EventArgs e)
        {
            switch (tSSBOOrPOS.Text)
            {
                case "Continue to Back Office ->":

                    ContinueToBOORPOS("BackOffice");
                    break;
                case "Continue to POS ->":
                    ContinueToBOORPOS("POS");
                    break;
            }
        }

        private void ContinueToBOORPOS(string _menu)
        {
            POSEntities entity = new POSEntities();
            string menuPermission = entity.Users.Where(x => x.Id == MemberShip.UserId).Select(x => x.MenuPermission).FirstOrDefault();
            Login form = new Login();
            form.MdiParent = this;
            form.Permission_BO_OR_POS(_menu, menuPermission);

        }

        private void uIInterfaceForJunctionCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ELC_POS_UI_Interface_For_JunctionCity _pos_UI_Interface = new ELC_POS_UI_Interface_For_JunctionCity();
            _pos_UI_Interface.ShowDialog();
        }

        private void customerReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewCustomerReport report = new NewCustomerReport();
            report.ShowDialog();
        }

        private void vIPMemberDoublePromotionRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPointDeductionPercentage_History newfrom = new frmPointDeductionPercentage_History();
            newfrom.ShowDialog();
        }

        private void nonActiveCustomerMenuItem_Click(object sender, EventArgs e)
        {
            frmNonActiveCustomerList frmNonActiveCustomer = new frmNonActiveCustomerList();
            frmNonActiveCustomer.ShowDialog();
        }


        private void priceChangeWithExcelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmPriceChangeWithExcel priceChangeForm = new frmPriceChangeWithExcel();
            priceChangeForm.ShowDialog();
        }

        private void StockInFromSAPtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockInFromSAP stockInSAPForm = new StockInFromSAP();
            stockInSAPForm.ShowDialog();
        }

        private void refundListToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            RefundList newform = new RefundList();
            newform.ShowDialog();
        }

        private void exportSAPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales sForm = (Sales)this.ActiveMdiChild;
            DataExport exportForm = new DataExport(sForm);
            exportForm.IsAutoExport = false;
            exportForm.IsBackDateExport = false;
            exportForm.Text = "Export Today Data";
            exportForm.ShowDialog();
        }

        private void importSAPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales sForm = (Sales)this.ActiveMdiChild;
            DataImport importForm = new DataImport(sForm);
            importForm.IsAllDataImport = false;
            importForm.IsAutoImport = false;
            importForm.Text = "Import Today Data";
            importForm.ShowDialog();
        }


        private void exportImportHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportExportHistory imexHistory = new ImportExportHistory();
            imexHistory.ShowDialog();
        }

        private void refreshAccessTokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshToken refreshToken = new RefreshToken();
            refreshToken.ShowDialog();
        }

        private void importByProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales sForm = (Sales)this.ActiveMdiChild;
            ImportByProduct imbyProduct = new ImportByProduct(sForm);
            imbyProduct.ShowDialog();
        }

        private void stockInDataFromDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockInFromSAP stockForm = new StockInFromSAP();
            stockForm.ShowDialog();
        }

        private void backDateExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales sForm = (Sales)this.ActiveMdiChild;
            DataExport exportForm = new DataExport(sForm);
            exportForm.IsAutoExport = false;
            exportForm.IsBackDateExport = true;
            exportForm.Text = "Back Date Export";
            exportForm.ShowDialog();
        }

        private void importAllDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales sForm = (Sales)this.ActiveMdiChild;
            DataImport importForm = new DataImport(sForm);
            importForm.IsAllDataImport = true;
            importForm.IsAutoImport = false;
            importForm.Text = "Import All Data";
            importForm.ShowDialog();
        }

        private void memberTypeResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerLevelControl fromLevel = new CustomerLevelControl();
            fromLevel.ShowDialog();
        }

        private void couponCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.CouponCode.Add || MemberShip.isAdmin)
            {
                CouponCode newform = new CouponCode();
                newform.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add coupon code.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void exportTransactionsHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POS.View.LoyaltyPts.ImportExportMemberTransHistory imexHistory = new POS.View.LoyaltyPts.ImportExportMemberTransHistory();
            imexHistory.ShowDialog();
        }

        private void transactionUsingCouponCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POS.View.Reports.TransactionUsingCouponCodeReport couponCode = new POS.View.Reports.TransactionUsingCouponCodeReport();
            couponCode.ShowDialog();
        }
    }
}

