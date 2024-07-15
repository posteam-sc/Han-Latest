using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Data;
using POS.APP_Data;
using System.Configuration;

namespace POS
{
    class API_Token
    {
        #region Variables               
        static APICredential credential { get; set; }
        public static string AccessToken { get; set; }
        public static HttpResponseMessage tokenResponse { get; set; }
        #endregion

        #region Methods        
        public static void Get_AccessToken()
        {
            if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrWhiteSpace(AccessToken))
            {
                POSEntities entity = new POSEntities();
                credential = entity.APICredentials.FirstOrDefault();
                AccessToken = credential.AccessToken;
                if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrWhiteSpace(AccessToken))
                {
                    Get_AccessTokenFromSAP();
                    if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrWhiteSpace(AccessToken))
                    {
                        if (API_Token.tokenResponse.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            MessageBox.Show("Invalid Login Information", "Access Token", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("No Access Token!", "Access Token", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    else
                    {
                        using (entity = new POSEntities())
                        {

                            if (credential != null)
                            {
                                credential.AccessToken = AccessToken;
                                entity.Entry(credential).State = EntityState.Modified;
                                entity.SaveChanges();

                            }

                        }

                    }
                }
            }


        }
        public static void Get_AccessTokenFromSAP()
        {
            try
            {
                HttpClient restClient = new HttpClient();
                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/ACCESS_TOKEN");

                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credential.AuthorizationToken);
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));

                string LoginData = "{\"client_id\":\"" + credential.ClientId + "\"," +
                               "\"client_secret\":\"" + credential.ClientSecret + "\"," +
                               "\"grant_type\":\"" + credential.Grant_Type + "\"}";

                HttpContent Content = new StringContent(LoginData, Encoding.UTF8, Content_Type);
                tokenResponse = new HttpResponseMessage();
                tokenResponse = (HttpResponseMessage)restClient.PostAsync(Builder.Uri, Content).Result;


                if (tokenResponse.IsSuccessStatusCode)
                {
                    string result = tokenResponse.Content.ReadAsStringAsync().Result;
                    AccessToken = result.Remove(0, 1).Remove(result.Length - 2, 1);

                }

            }
            catch (Exception)
            {
                AccessToken = null;
            }


        }

        #endregion

    }
}
