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
    public partial class RefundDiscount : Form
    {
        
        public RefundDiscount()
        {
            InitializeComponent();
        }

        private void txtDiscount_KeyDown(object sender, KeyEventArgs e)
        {            

            if (e.KeyData == (Keys.Enter))
            {
                
                if (System.Windows.Forms.Application.OpenForms["RefundTransaction"] != null)
                {
                    RefundTransaction newForm = (RefundTransaction)System.Windows.Forms.Application.OpenForms["RefundTransaction"];
                    newForm.DiscountAmount = Convert.ToInt32(txtDiscount.Text);
                    newForm.Reload();
                    this.Dispose();
                }
            }
        }
    }
}
