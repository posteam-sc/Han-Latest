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

namespace POS
{

    public partial class frmPointDeductionPercentage_History : Form
    {
        #region variable

        POSEntities entity = new POSEntities();

        #endregion

        #region event
        public frmPointDeductionPercentage_History()
        {
            InitializeComponent();
        }

        private void PointDeductionPercentage_History_Load(object sender, EventArgs e)
        {
            APP_Data.PointDeductionPercentage_History PDP = (from p in entity.PointDeductionPercentage_History where p.Active == true select p).FirstOrDefault();
            if (PDP == null) { PDPActive.Checked = false; groupBox1.Enabled = false; } else { PDPActive.Checked = true; groupBox1.Enabled = true; }
            LoadData();
        }
        #endregion

        #region function
        private void LoadData()
        {
            List<APP_Data.PointDeductionPercentage_History> PDP = (from p in entity.PointDeductionPercentage_History orderby p.StartDate descending select p).ToList();
            dgvPointDeductionList.AutoGenerateColumns = false;
            dgvPointDeductionList.DataSource = PDP;
        }


        private void Clear()
        {
            txtPercentage.Text = "";

        }
        #endregion

        private void dgvPointDeductionList_DataBindingComplte(object sender, DataGridViewBindingCompleteEventArgs e)
        {

            foreach (DataGridViewRow row in dgvPointDeductionList.Rows)
            {
                APP_Data.PointDeductionPercentage_History p = (APP_Data.PointDeductionPercentage_History)row.DataBoundItem;
                row.Cells[0].Value = p.Id;
                row.Cells[1].Value = Convert.ToInt32(p.DiscountRate) + " %";
                row.Cells[2].Value = p.StartDate.Date.ToString("dd-MM-yyyy");
                row.Cells[3].Value = (p.EndDate == null) ? "" : p.EndDate.Value.Date.ToString("dd-MM-yyyy");
                row.Cells[4].Value = entity.Users.Find(p.UserId).Name;
                row.Cells[5].Value = entity.Counters.Find(p.Counter).Name.ToString();
                row.Cells[6].Value = p.Active == true ? "Active" : "InActive";
                if (row.Cells[6].Value.ToString() == "Active")
                {

                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
            dgvPointDeductionList.ClearSelection();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (txtPercentage.Text.ToString() != "")
            {

                if (Convert.ToInt32(txtPercentage.Text) > 100) { MessageBox.Show("DiscountRate must not exceed 100%", "Create Again", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                else if (Convert.ToInt32(txtPercentage.Text) < 1) { MessageBox.Show("DiscountRate must not less than 1", "Create Again", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                else
                {
                    decimal discountRate = Convert.ToDecimal(txtPercentage.Text);
                    APP_Data.PointDeductionPercentage_History pcurrent = entity.PointDeductionPercentage_History.Where(x => x.Active == true).FirstOrDefault();
                    if (pcurrent != null) { pcurrent.Active = false; pcurrent.EndDate = DateTime.Now; }


                    APP_Data.PointDeductionPercentage_History pdp = new APP_Data.PointDeductionPercentage_History();
                    pdp.DiscountRate = discountRate;
                    pdp.UserId = MemberShip.UserId;
                    pdp.StartDate = DateTime.Now;
                    pdp.Active = true;
                    pdp.Counter = MemberShip.CounterId;
                    entity.PointDeductionPercentage_History.Add(pdp);
                    entity.SaveChanges();
                    LoadData();
                    Clear();
                }
            }
            else
            {
                MessageBox.Show("Insert discount rate for double promotion!", "Create Again", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void PDPActive_CheckedChanged(object sender, EventArgs e)
        {
            if (PDPActive.Checked == true)
            {
                groupBox1.Enabled = true;
                LoadData();
                Clear();
            }
            else
            {
                DialogResult dr = MessageBox.Show("Are you sure not to use double promotion rule?", "Alert", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dr.Equals(DialogResult.OK))
                {
                    groupBox1.Enabled = false;
                    PointDeductionPercentage_History p = entity.PointDeductionPercentage_History.Where(x => x.Active == true).FirstOrDefault();
                    if (p != null)
                    {
                        p.Active = false;
                        p.UserId = MemberShip.UserId;
                        p.Counter = MemberShip.CounterId;
                        p.EndDate = DateTime.Now;

                        entity.SaveChanges();
                        Clear();
                        LoadData();
                    }
                }
                else
                {
                    PDPActive.Checked = true;
                    Clear();
                    LoadData();
                }
            }
        }

        private void txtPercentage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


    }
}
