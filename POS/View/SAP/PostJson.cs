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
//using System.Json;
using System.IO;
using Newtonsoft.Json;



namespace POS
{
    public partial class PostJson : Form
    {
        #region Variables
        public string Json { get; set; }
        public string batchNo { get; set; }
        public string API_Name { get; set; }
        #endregion
        public PostJson()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                File.WriteAllText(fileName, txtJson.Text);
            }
        }

        private void PostJson_Load(object sender, EventArgs e)
        {
            lblAPIName.Text = API_Name;
            if (!string.IsNullOrEmpty(Json))
            {
                var postjson = JsonConvert.DeserializeObject(Json);
                txtJson.Text = JsonConvert.SerializeObject(postjson, Formatting.Indented);
            }
            else
            {
                btnSave.Enabled = false;

            }

        }


    }
}
