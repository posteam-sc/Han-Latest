using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using POS.APP_Data;
using System.Configuration;
using System.Runtime;
using System.Runtime.InteropServices;

namespace POS
{
    public partial class RefreshToken : Form
    {
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        public RefreshToken()
        {
            InitializeComponent();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            API_Token.Get_AccessTokenFromSAP();
            if (string.IsNullOrEmpty(API_Token.AccessToken) || string.IsNullOrWhiteSpace(API_Token.AccessToken))
            {
                if (API_Token.tokenResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Invalid Login Information", "Access Token", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Token Refresh Failed!", "Access Token", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                using (POSEntities entity = new POSEntities())
                {
                    APICredential credential = entity.APICredentials.FirstOrDefault();
                    if (credential != null)
                    {
                        credential.AccessToken = API_Token.AccessToken;
                        entity.Entry(credential).State = EntityState.Modified;
                        entity.SaveChanges();

                    }
                    MessageBox.Show("Token Successfully Refreshed!", " Access Token", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }

        private void RefreshToken_Load(object sender, EventArgs e)
        {
            int description;
            bool IsInternet = InternetGetConnectedState(out description, 0);

            if (!IsInternet)
            {
                MessageBox.Show("No Internet Connection!", "No Internet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
            else
            {
                bool IsConnect = Utility.CheckConnection(ConfigurationManager.AppSettings["APIServer1"]);
                if (!IsConnect)
                {
                    MessageBox.Show("Server Not Found!", "Server Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }
    }
}
