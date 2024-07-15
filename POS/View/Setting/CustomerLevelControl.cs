using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Windows.Forms;

namespace POS.View.Setting
{
    public partial class CustomerLevelControl : Form
    {
        POSEntities pEntity = new POSEntities();
        IQueryable<Customer> iqc;
        DateTime dr;

        static int defaultCus = 0, iBackMonth = 24;
        DateTime dtFormLoad = DateTime.Now;
        public CustomerLevelControl()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!verifyRecord())
                {
                    return;
                }

                dr = DateTime.Now.AddMonths(0 - iBackMonth);
                DialogResult res = MessageBox.Show("The selected customer(s) will revoke to Non-VIP if he/she doesn't have a transaction from " + dr.ToString("dd-MMMM-yyyy"), "Are you sure want to revoke?", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Application.UseWaitCursor = true;

                    btnSubmit.Enabled = false;

                    if ((int)cboCustomer.SelectedValue == 0)
                    {
                        IQueryable<POS.APP_Data.Transaction> iqTrans = pEntity.Transactions.Where(x => x.IsDeleted == false && x.IsComplete == true && x.DateTime >= dr && x.CustomerId != defaultCus && (x.Type == "Sale" || x.Type == "Special Member"));
                        IQueryable<POS.APP_Data.Transaction> iqTrans_0_2999 = iqTrans.Where(x => x.CustomerId < 3000);
                        IQueryable<POS.APP_Data.Transaction> iqTrans_3000_5999 = iqTrans.Where(x => x.CustomerId >= 3000 && x.CustomerId < 6000);
                        IQueryable<POS.APP_Data.Transaction> iqTrans_6000_8999 = iqTrans.Where(x => x.CustomerId >= 6000 && x.CustomerId < 9000);
                        IQueryable<POS.APP_Data.Transaction> iqTrans_Over_9000 = iqTrans.Where(x => x.CustomerId >= 9000);
                        //IQueryable<POS.APP_Data.Transaction> iqTrans_Over_15000 = iqTrans.Where(x => x.CustomerId >= 15000);
                        foreach (Customer c in iqc.ToList())
                        {
                            if (c != null && c.MemberTypeID != null && (c.LatestRevokeDate == null || c.LatestRevokeDate < DateTime.Now.Date))
                            {
                                //if(c.Id>=15000)
                                //{
                                //    var iqTrans4Each = iqTrans_Over_15000.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                                //    if (iqTrans4Each == null)
                                //    {
                                //        Utility.InsertRevokeHistoryData(c, nonVIPId, txtNote.Text);
                                //    }
                                //}
                                if (c.Id >= 9000)
                                {
                                    var iqTrans4Each = iqTrans_Over_9000.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                                    if (iqTrans4Each == null)
                                    {
                                        Utility.InsertRevokeHistoryData(c, txtNote.Text);
                                    }
                                }
                                else if (c.Id >= 6000)
                                {
                                    var iqTrans4Each = iqTrans_6000_8999.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                                    if (iqTrans4Each == null)
                                    {
                                        Utility.InsertRevokeHistoryData(c, txtNote.Text);
                                    }
                                }
                                else if (c.Id >= 3000)
                                {
                                    var iqTrans4Each = iqTrans_3000_5999.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                                    if (iqTrans4Each == null)
                                    {
                                        Utility.InsertRevokeHistoryData(c, txtNote.Text);
                                    }
                                }
                                else
                                {
                                    var iqTrans4Each = iqTrans_0_2999.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                                    if (iqTrans4Each == null)
                                    {
                                        Utility.InsertRevokeHistoryData(c, txtNote.Text);
                                    }
                                }

                            }

                        }
                        //foreach (Customer c in iqc.ToList())
                        //{
                        //    if (c != null && c.MemberTypeID != null && (c.LatestRevokeDate==null || c.LatestRevokeDate < DateTime.Now.Date))
                        //    {
                        //        var iqTrans4Each = iqTrans.Where(x => x.CustomerId == c.Id).FirstOrDefault();
                        //        if (iqTrans4Each == null)
                        //        {
                        //            Utility.InsertRevokeHistoryData(c, txtNote.Text);
                        //        }

                        //    }

                        //}
                        MessageBox.Show("Revoke process completed!");
                    }
                    else
                    {
                        int cusId = (int)cboCustomer.SelectedValue;
                        var cusdetail = iqc.Where(x => x.Id == cusId).FirstOrDefault();

                        if (cusdetail != null && cusdetail.MemberTypeID != null && (cusdetail.LatestRevokeDate == null || cusdetail.LatestRevokeDate < DateTime.Now.Date))
                        {
                            var iqTrans = pEntity.Transactions.Where(x => x.CustomerId == cusId && x.IsDeleted == false && x.IsComplete == true && x.DateTime >= dr && (x.Type == "Sale" || x.Type == "Special Member")).FirstOrDefault(); ;
                            if (iqTrans == null)
                            {
                                Utility.InsertRevokeHistoryData(cusdetail, txtNote.Text);
                                MessageBox.Show("Revoke process completed!");
                            }
                            else
                            {
                                MessageBox.Show("This customer has some transaction(s) during the specified period.", "Can't revoke this customer!");
                            }
                        }
                    }
                    RefreshData();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Revoke process fail!");

            }

            finally
            {
                btnSubmit.Enabled = true;
                Cursor.Current = Cursors.Default;
                Application.UseWaitCursor = false;
            }
        }


        private bool verifyRecord()
        {
            if (iBackMonth < 1)
            {
                MessageBox.Show("Require parameter is not provided(Duration)!");
                return false;
            }
            else if (iqc == null || iqc.Count() == 0)
            {
                return false;
            }
            return true;
        }
        private void txtMonths_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void CustomerLevelControl_Load(object sender, EventArgs e)
        {
            DateTime dtToday = DateTime.Now.Date;
            iqc = pEntity.Customers.Where(x => x.Name != "Default" && x.MemberTypeID != null && x.PromoteDate != null && EntityFunctions.TruncateTime(x.PromoteDate) != dtToday && (x.LatestRevokeDate == null || x.LatestRevokeDate < dtToday));
            List<Customer> icus = iqc.ToList();

            if (icus != null && icus.Count > 0)
            {
                APP_Data.Customer Customerobj = new APP_Data.Customer();
                Customerobj.Id = 0;
                Customerobj.Name = "All Customers";
                icus.Insert(0, Customerobj);
                cboCustomer.DataSource = icus;
                cboCustomer.DisplayMember = "Name";
                cboCustomer.ValueMember = "Id";
                cboCustomer.AutoCompleteMode = AutoCompleteMode.Suggest;
                cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
            dtFormLoad = DateTime.Now;
            try
            {
                iBackMonth = SettingController.MemberTypeResetBackMonth;
            }
            catch
            {
                MessageBox.Show("An error has occure while getting some setting value, Please contact administrator for assist.");
                this.Close();
            }
        }

        private void CustomerLevelControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
            if (newForm != null)
            {
                newForm.ReloadCustomerList();
            }
        }

        private void RefreshData()
        {

            var iHisList = (from ch in pEntity.CustomerLevelRevokeHistories
                            join cu in pEntity.Customers on ch.CustomerId equals cu.Id
                            join ur in pEntity.Users on ch.ActionBy equals ur.Id
                            //join ct in pEntity.CustomerTypes on ch.LastCustomerLevel equals ct.Id
                            where ch.Active == true && cu.Name != "Default" && ch.ActionOn >= dtFormLoad
                            orderby ch.Id descending
                            select new
                            {
                                CustomerName = cu.Name,
                                Note = ch.Note,
                                ActionOn = ch.ActionOn,
                                ActionBy = ur.Name
                            }).ToList();

            dgvHistoryList.DataSource = iHisList;
            dgvHistoryList.Refresh();
        }
    }
}
