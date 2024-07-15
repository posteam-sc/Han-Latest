using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.POSInterfaceServiceReference;
using System.Data.SqlClient;
using System.Configuration;

namespace POS
{
    public partial class ELC_POS_UI_Interface_For_JunctionCity : Form
    {
        // By SYM
        DateTime fromTime;
        DateTime toTime;
        bool isRunning = false;
    
        public void IsAddressAvailable(string address)
        {
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

        public ELC_POS_UI_Interface_For_JunctionCity()
        {
            InitializeComponent();
        }

        private void ELC_POS_UI_Interface_For_JunctionCity_Load(object sender, EventArgs e)
        {
            dtpFromDate.Format = DateTimePickerFormat.Custom;
            dtpFromDate.CustomFormat = "dd/MM/yyyy - hh:mm:ss tt";

            dtpToDate.Format = DateTimePickerFormat.Custom;
            dtpToDate.CustomFormat = "dd/MM/yyyy - hh:mm:ss tt";
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //    3/17/2017 12:37:26 PM
            fromTime = dtpFromDate.Value;
            toTime = dtpToDate.Value;

            string address = "https://jc-dc.junctionv-aap.com/wsSSI/wsSSIWebService.asmx";
            IsAddressAvailable(address);
            if (isRunning == true)
            {
                Utility.Push_DataToWebServices(fromTime, toTime);
            }
        }
    }
}
