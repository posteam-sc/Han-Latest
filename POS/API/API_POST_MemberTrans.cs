using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Data;
using System.Data.Objects;
using System.Configuration;
using POS.APP_Data;
using System.Net;

namespace POS
{
    class API_POST_MemberTrans
    {
        #region Variables 

        static HttpClient restClient = new HttpClient();
        public static HttpResponseMessage response { get; set; }
        public static string postMemberTransJson { get; set; }
        public static bool postMemberTransSuccess { get; set; } = false;
        public static List<string> postMemberTransResponseMessage;
        public static string ResponseMemberTransJson { get; set; }
        public static string ResponseMemberDisplayName { get; set; }
        #endregion

        #region Models

        public class POST_MemberTransInvoice
        {
            public string member { get; set; }
            public int amount { get; set; }
            public string s_ref { get; set; }
        }

        public class POST_MemberTransResponse
        {
            public string status { get; set; }
            public data data { get; set; }
        }

        public class data
        {
            public int delta { get; set; }
            public int total { get; set; }
            public member member { get; set; }
        }

        public class member
        {
            public string id { get; set; }
            public string phoneNumber { get; set; }
            public string displayName { get; set; }
        }

        #endregion

        #region Methods
        public static POST_MemberTransInvoice GetPostMemberTransData(string TransId)
        {
            POSEntities entity = new POSEntities();
            List<Transaction> transList = new List<Transaction>();
            POST_MemberTransInvoice MemberTransInvoice = new POST_MemberTransInvoice();
            //string mobileNo = "";
            transList = (from t in entity.Transactions where t.Id == TransId && t.IsComplete == true && t.IsActive == true && t.Type == "Sale" && t.IsLoyaltyExported == true select t).ToList<Transaction>();

            if (transList.Count > 0)
            {
                foreach (Transaction transaction in transList)
                {
                    //mobileNo = "";
                    //if (transaction.PitiMemberId == "" || transaction.PitiMemberId==null)
                    //{
                    //    mobileNo = entity.Customers.Where(x => x.Id == transaction.CustomerId).Select(x => x.PhoneNumber).FirstOrDefault();
                    //    MemberTransInvoice.member = mobileNo;
                    //}
                    //else
                    //{
                    //    MemberTransInvoice.member = transaction.PitiMemberId;
                    //}

                    MemberTransInvoice.member = transaction.PitiMemberId;

                    MemberTransInvoice.amount = Convert.ToInt32(transaction.RecieveAmount);
                    MemberTransInvoice.s_ref = transaction.Id;
                }
            }

            return MemberTransInvoice;
        }
        #endregion

        #region API
        public static void POST_MemberTrans(string TransId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                POSEntities entity = new POSEntities();

                restClient = new HttpClient();
                postMemberTransResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                string accessKey = "";
                accessKey = entity.APIKeyCredentials.Select(x => x.AccessKey).FirstOrDefault();
                //accessKey = "111";
                restClient.DefaultRequestHeaders.Add("Authorization", $"{accessKey}");
                POST_MemberTransInvoice ContentData = GetPostMemberTransData(TransId);
                if (ContentData == null)
                {
                    postMemberTransSuccess = true;
                    postMemberTransResponseMessage.Add("No Data to Export");
                    return;
                }
                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                var remove_PostContent_Json = PostContent_Json.Replace("s_ref", "ref");
                postMemberTransJson = remove_PostContent_Json;
                string counterCode = SettingController.CounterCode;
                string apiUri = ConfigurationManager.AppSettings["APIPitiServer"];
                HttpContent Content = new StringContent(remove_PostContent_Json, Encoding.UTF8, Content_Type);

                response = new HttpResponseMessage();
                response = restClient.PostAsync(apiUri, Content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseMemberTransJson = result;
                    

                    POST_MemberTransResponse responseContent = JsonConvert.DeserializeObject<POST_MemberTransResponse>(result);
                    if (responseContent != null)
                    {
                        if (!string.IsNullOrEmpty(responseContent.status) && responseContent.status.ToLower().Contains("success"))
                        {
                            responseContent.status = "Success";
                        }

                        ResponseMemberDisplayName = responseContent.data.member.displayName;

                        postMemberTransSuccess = responseContent.status == "Success" ? true : false;
                        postMemberTransResponseMessage.Add("OK");
                        postMemberTransResponseMessage.Add(responseContent.status);
                    }
                    else
                    {
                        postMemberTransSuccess = false;
                        postMemberTransResponseMessage.Add("OK");
                        postMemberTransResponseMessage.Add("Unrecongnized Response Format");
                    }
                }
                else
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseMemberTransJson = result;

                    POST_MemberTransResponse responseContent = JsonConvert.DeserializeObject<POST_MemberTransResponse>(result);
                    ResponseMemberDisplayName = responseContent.data.member.displayName;

                    postMemberTransSuccess = false;
                    postMemberTransResponseMessage.Add(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                postMemberTransSuccess = false;
                postMemberTransResponseMessage.Add(ex.ToString());
            }
        }
        #endregion
    }
}
