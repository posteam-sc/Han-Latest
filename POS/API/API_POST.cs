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


namespace POS
{
    class API_POST
    {
        #region Variables 

        static HttpClient restClient = new HttpClient();
        public static HttpResponseMessage response { get; set; }
        public static string postArJson { get; set; }
        public static bool postArSuccess { get; set; } = false;
        public static List<string> postArResponseMessage;
        public static string ResponseArJson { get; set; }


        public static string postJEJson { get; set; }
        public static bool postJESuccess { get; set; } = false;
        public static List<string> postJEResponseMessage;
        public static string ResponseJEJson { get; set; }

        public static string postArCredJson { get; set; }
        public static bool postArCredSuccess { get; set; } = false;
        public static List<string> postArCredResponseMessage;
        public static string ResponseArCredJson { get; set; }



        #endregion

        #region API
        public static void POST_AR(DateTime postDate)
        {
            try
            {

                restClient = new HttpClient();
                postArResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                POST_ARInvoice ContentData = GetPostARData(postDate);
                if (ContentData.list == null)
                {
                    postArSuccess = true;
                    postArResponseMessage.Add("No Data to Export");
                    return;
                }
                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postArJson = PostContent_Json;
                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/POST_AR");
                HttpContent Content = new StringContent(PostContent_Json, Encoding.UTF8, Content_Type);


                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, Content).Result;
                if (response.IsSuccessStatusCode)
                {

                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseArJson = result;
                    POST_ARResponseList responseContent = JsonConvert.DeserializeObject<POST_ARResponseList>(result);

                    // postArResponseMessage =  response.StatusCode + ":" + responseContent.list.Select(x => x.message).FirstOrDefault();
                    if (responseContent != null && responseContent.list != null && responseContent.list.Count > 0)
                    {

                        foreach (POST_ARResponse res in responseContent.list)
                        {
                            if (!string.IsNullOrEmpty(res.message) && res.message.ToLower().Contains("already posted"))
                            {
                                res.status = "Success";
                            }
                        }

                        postArSuccess = responseContent.list.Where(x => x.status == "FAIL").Any() ? false : true;
                        postArResponseMessage.Add("OK");
                        postArResponseMessage.AddRange(responseContent.list.Select(x => x.message));
                    }
                    else
                    {
                        postArSuccess = false;
                        postArResponseMessage.Add("OK");
                        postArResponseMessage.Add("Unrecongnized Response Format");
                    }

                }
            }
            catch (Exception ex)
            {
                postArSuccess = false;
                postArResponseMessage.Add(ex.ToString());

            }

        }


        public static void POST_ARCred(DateTime postDate)
        {

            try
            {
                restClient = new HttpClient();
                postArCredResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                POST_ARInvoice ContentData = GetPostARCredData(postDate);

                if (ContentData.list == null) //ContentData.list.Count == 0 ||
                {
                    postArCredSuccess = true;
                    postArCredResponseMessage.Add("No Data to Export");
                    return;
                }
                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postArCredJson = PostContent_Json;
                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/POST_ARCred");
                HttpContent Content = new StringContent(PostContent_Json, Encoding.UTF8, Content_Type);


                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, Content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseArCredJson = result;
                    POST_ARCredResponseList responseContent = JsonConvert.DeserializeObject<POST_ARCredResponseList>(result);
                    if (responseContent != null && responseContent.list != null && responseContent.list.Count > 0)
                    {
                        foreach (POST_ARCredResponse res in responseContent.list)
                        {
                            if (!string.IsNullOrEmpty(res.message) && res.message.ToLower().Contains("already posted"))
                            {
                                res.status = "Success";
                            }
                        }

                        postArCredSuccess = responseContent.list.Where(x => x.status == "FAIL").Any() ? false : true;
                        postArCredResponseMessage.Add("OK");
                        postArCredResponseMessage.AddRange(responseContent.list.Select(x => x.message));
                    }
                    else
                    {
                        postArCredSuccess = false;
                        postArCredResponseMessage.Add("OK");
                        postArCredResponseMessage.Add("Unrecongnized Response Format");
                    }

                    //postArCredResponseMessage = response.StatusCode + ":" + responseContent.list.Select(x => x.message).FirstOrDefault();

                }

            }

            catch (Exception ex)
            {
                postArCredSuccess = false;
                postArCredResponseMessage.Add(ex.ToString());//response.ReasonPhrase;
            }


        }
        public static void POST_JE(DateTime postDate)
        {
            try
            {

                restClient = new HttpClient();
                postJEResponseMessage = new List<string>();
                string Content_Type = "application/json";
                restClient.DefaultRequestHeaders.Accept.Clear();
                restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
                restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
                POST_JEList ContentData = GetPostJEData(postDate);
                if (ContentData.list == null)
                {
                    postJESuccess = true;
                    postJEResponseMessage.Add("No Data to Export");
                    return;
                }

                var PostContent_Json = JsonConvert.SerializeObject(ContentData);
                postJEJson = PostContent_Json;
                string apiUri = ConfigurationManager.AppSettings["APIServer"];
                var Builder = new UriBuilder($"{apiUri}/POST_JE");
                HttpContent Content = new StringContent(PostContent_Json, Encoding.UTF8, Content_Type);

                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, Content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ResponseJEJson = result;
                    POST_JEResponseList responseContent = JsonConvert.DeserializeObject<POST_JEResponseList>(result);
                    if (responseContent != null && responseContent.list != null && responseContent.list.Count > 0)
                    {
                        foreach (POST_JEResponse res in responseContent.list)
                        {
                            if (!string.IsNullOrEmpty(res.message) && res.message.ToLower().Contains("already posted"))
                            {
                                res.status = "Success";
                            }
                        }

                        postJESuccess = responseContent.list.Where(x => x.status == "FAIL").Any() ? false : true;
                        postJEResponseMessage.Add("OK");
                        postJEResponseMessage.AddRange(responseContent.list.Select(x => x.message));
                    }
                    else
                    {
                        postJESuccess = false;
                        postJEResponseMessage.Add("OK");
                        postJEResponseMessage.Add("Unrecongnized Response Format");
                    }

                    // postJEResponseMessage = response.StatusCode.ToString() + ":" + responseContent.list.Select(x => x.message).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                postJESuccess = false;
                postJEResponseMessage.Add(ex.ToString());
            }




        }
        #endregion
        #region Methods
        public static POST_ARInvoice GetPostARData(DateTime postDate)
        {
            POSEntities entity = new POSEntities();
            string sCode = SettingController.DefaultShop.ShortCode;
            string counterCode = SettingController.CounterCode;
            int curId = SettingController.DefaultCurrency;
            string curcyCode = entity.Currencies.Where(x => x.Id == curId).Select(x => x.CurrencyCode).FirstOrDefault();
            List<string> salesTypes = new List<string> { "Sales", "FOC" };
            POST_ARInvoice ARInvoice = new POST_ARInvoice();

            foreach (string sType in salesTypes)
            {
                int salType = salesTypes.FindIndex(x => x.Contains(sType));
                List<GetTotalAmountByProductId_Result> LineTotalData = entity.GetTotalAmountByProductId(postDate, sCode, salType).ToList();


                if (LineTotalData.Count > 0)
                {
                    List<GetTotalQtyByBatchNo_Result> BatchTotalQty = entity.GetTotalQtyByBatchNo(postDate, sCode, salType).ToList();
                    HDR Hdrs = new HDR();

                    //  Hdrs.DocDate = postDate.ToString("dd/MM/yyyy");
                    Hdrs.DocDate = Utility.FormatDate(postDate);
                    Hdrs.RefNum = "POS";
                    Hdrs.Comments = counterCode;
                    Hdrs.CurCode = curcyCode;
                    Hdrs.SalesType = sType;
                    if (sType == "Sales")
                    {
                        var salesAmt = entity.Transactions.Where(x => EntityFunctions.TruncateTime(x.DateTime) == postDate && x.IsDeleted == false && x.IsComplete == true && x.IsActive == true && x.Id.Contains(sCode) && x.PaymentType.Name != "FOC" && x.PaymentType.Name != "Tester" && x.Type == "Sale").Sum(x => x.TotalAmount);
                        if (salesAmt != null)
                        {
                            Hdrs.DocTotal = (long)salesAmt;
                        }

                        var GiftCardAmt = entity.GetGiftCardSalesAmount(postDate, sCode).FirstOrDefault();

                        if (GiftCardAmt != null)
                        {
                            Hdrs.DocTotal -= (long)GiftCardAmt;
                        }

                        var CouponCodeAmt = entity.GetCouponCodeSalesAmount(postDate, sCode).FirstOrDefault();

                        if (CouponCodeAmt != null)
                        {
                            Hdrs.DocTotal -= (long)CouponCodeAmt;
                        }

                        //IQueryable<Transaction> iTrans = entity.Transactions.Where(x => EntityFunctions.TruncateTime(x.DateTime) == postDate && (x.IsDeleted == false || x.IsDeleted != null) && x.IsComplete == true && x.IsActive == true && x.Id.Contains(sCode) && x.PaymentType.Name != "FOC" && x.PaymentType.Name != "Tester" && x.Type == "Sale");
                        //if (iTrans != null && iTrans.Count() > 0)
                        //{
                        //   Hdrs.DocTotal  = (long)iTrans.Sum(x => (long)x.TotalAmount);
                        //}
                    }
                    else
                    {
                        Hdrs.DocTotal = 0;
                    }

                    foreach (GetTotalAmountByProductId_Result tr in LineTotalData)
                    {
                        DTL detail = new DTL();
                        detail.ItemCode = tr.ProductCode;
                        detail.ItemName = tr.Name;
                        detail.WhsCode = counterCode;
                        detail.LineTotal = (long)tr.TotalAmount;
                        detail.UomCode = tr.UnitCode;
                        List<GetTotalQtyByBatchNo_Result> batchTotalQtyByProductId = BatchTotalQty.Where(x => x.ProductId == tr.ProductId).ToList();

                        foreach (GetTotalQtyByBatchNo_Result tm in batchTotalQtyByProductId)
                        {
                            BatQty batchDetail = new BatQty();
                            batchDetail.Qty = (int)tm.TotalQty;
                            batchDetail.ExpDate = Utility.FormatDate(tm.ExpireDate.Value.Date);
                            batchDetail.BatchNum = tm.BatchNo;
                            List<BatQty> batList = new List<BatQty>();
                            batList.Add(batchDetail);
                            if (detail.BatQty == null || detail.BatQty.Count < 1)
                            {
                                detail.BatQty = batList;
                            }
                            else
                            {
                                detail.BatQty.Add(batchDetail);
                            }


                        }

                        List<DTL> dtlList = new List<DTL>();
                        dtlList.Add(detail);
                        if (Hdrs.dtl == null || Hdrs.dtl.Count < 1)
                        {
                            Hdrs.dtl = dtlList;
                        }
                        else
                        {
                            Hdrs.dtl.Add(detail);
                        }
                    }
                    POST_ARData ARData = new POST_ARData();
                    ARData.hdr = Hdrs;
                    List<POST_ARData> ARList = new List<POST_ARData>();
                    ARList.Add(ARData);
                    if (ARInvoice.list == null || ARInvoice.list.Count < 1)
                    {
                        ARInvoice.list = ARList;
                    }
                    else
                    {
                        ARInvoice.list.Add(ARData);
                    }
                }
                else
                {
                    break;
                }
            }

            return ARInvoice;
        }
        public static POST_JEList GetPostJEData(DateTime postDate)
        {
            POSEntities entity = new POSEntities();
            string sCode = SettingController.DefaultShop.ShortCode;
            string counterCode = SettingController.CounterCode;
            POST_JEList JEList = new POST_JEList();

            POSTJE_HDR Hdrs = new POSTJE_HDR();
            List<POSTJE_DTL> jdtlList = new List<POSTJE_DTL>();
            long RefundAmount = 0; long salesAmount = 0;
            var RefundAmt = entity.Transactions.Where(x => EntityFunctions.TruncateTime(x.DateTime) == postDate && x.IsDeleted == false && x.IsComplete == true && x.IsActive == true && x.Id.Contains(sCode) && x.Type == "Refund").Sum(x => x.TotalAmount);
            if (RefundAmt != null)
            {
                RefundAmount = (long)RefundAmt;
            }

            Hdrs.MEMO = "POS";
            Hdrs.POST_DATE = Utility.FormatDate(postDate);
            Hdrs.REF1 = counterCode;
            var salesAmt = entity.Transactions.Where(x => EntityFunctions.TruncateTime(x.DateTime) == postDate && x.IsDeleted == false && x.IsComplete == true && x.IsActive == true && x.Id.Contains(sCode) && x.PaymentType.Name != "FOC" && x.PaymentType.Name != "Tester" && x.Type == "Sale").Sum(x => x.TotalAmount);
            if (salesAmt != null)
            {
                salesAmount = (long)salesAmt;
            }
            Hdrs.DocTotal = salesAmount - RefundAmount;
            List<GetTotalAmountByPaymentTypeId_Result> TotalAmountByPayment = entity.GetTotalAmountByPaymentTypeId(postDate, sCode).ToList();

            foreach (GetTotalAmountByPaymentTypeId_Result tr in TotalAmountByPayment)
            {
                POSTJE_DTL detail = new POSTJE_DTL();
                detail.ACC_CODE = tr.AccountCode;
                detail.Amount = (long)tr.TotalAmount;
                detail.REF1 = tr.ReferenceForSAP;

                if (tr.ReferenceForSAP == "Cash")
                {
                    detail.Amount -= RefundAmount;
                }
                jdtlList.Add(detail);
                if (Hdrs.JEdtl == null || Hdrs.JEdtl.Count < 0)
                {
                    Hdrs.JEdtl = jdtlList;
                }
                //else
                //{
                //    Hdrs.JEdtl.Add(detail);
                //}

            }
            POST_JEData JEData = new POST_JEData();
            JEData.hdr = Hdrs;
            List<POST_JEData> JDataList = new List<POST_JEData>();
            JDataList.Add(JEData);
            if (JEList.list == null || JEList.list.Count < 0)
            {
                JEList.list = JDataList;
            }
            else
            {
                JEList.list.Add(JEData);
            }

            return JEList;
        }
        public static POST_ARInvoice GetPostARCredData(DateTime postDate)
        {
            POSEntities entity = new POSEntities();
            // postDate = Convert.ToDateTime("06/28/2022");
            string sCode = SettingController.DefaultShop.ShortCode;
            string counterCode = SettingController.CounterCode;
            int curId = SettingController.DefaultCurrency;
            string curcyCode = entity.Currencies.Where(x => x.Id == curId).Select(x => x.CurrencyCode).FirstOrDefault();
            List<string> salesTypes = new List<string> { "Refund", "FOC" };

            POST_ARInvoice ARCred = new POST_ARInvoice();

            foreach (string sType in salesTypes)
            {

                int salType = salesTypes.FindIndex(x => x.Contains(sType));
                List<GetRefundTotalAmountByProductId_Result> RefundLineTotalData = entity.GetRefundTotalAmountByProductId(postDate, sCode, salType).ToList();

                if (RefundLineTotalData.Count > 0)
                {
                    List<GetRefundTotalQtyByBatchNo_Result> RefundBatchTotalQty = entity.GetRefundTotalQtyByBatchNo(postDate, sCode, salType).ToList();

                    HDR Hdrs = new HDR();
                    Hdrs.DocDate = Utility.FormatDate(postDate);
                    Hdrs.RefNum = "POS";
                    Hdrs.Comments = counterCode;
                    Hdrs.CurCode = curcyCode;
                    Hdrs.SalesType = sType;
                    if (sType == "Refund")
                    {
                        var salesAmt = entity.Transactions.Where(x => EntityFunctions.TruncateTime(x.DateTime) == postDate && x.IsDeleted == false && x.IsComplete == true && x.IsActive == true && x.Id.Contains(sCode) && x.Type == "Refund").Sum(x => x.TotalAmount);
                        if (salesAmt != null)
                        {
                            Hdrs.DocTotal = (long)salesAmt;
                        }
                    }
                    else
                    {
                        Hdrs.DocTotal = 0;
                    }

                    foreach (GetRefundTotalAmountByProductId_Result tr in RefundLineTotalData)
                    {
                        DTL detail = new DTL();
                        detail.ItemCode = tr.ProductCode;
                        detail.ItemName = tr.Name;
                        detail.WhsCode = counterCode;
                        detail.LineTotal = (long)tr.TotalAmount;
                        detail.UomCode = tr.UnitCode;
                        List<GetRefundTotalQtyByBatchNo_Result> RefundBatchTotalQtyByProductId = RefundBatchTotalQty.Where(x => x.ProductId == tr.ProductId).ToList();

                        foreach (GetRefundTotalQtyByBatchNo_Result tm in RefundBatchTotalQtyByProductId)
                        {
                            BatQty batchDetail = new BatQty();
                            batchDetail.Qty = (int)tm.TotalQty;
                            batchDetail.ExpDate = Utility.FormatDate(tm.ExpireDate.Value.Date);
                            batchDetail.BatchNum = tm.BatchNo;
                            List<BatQty> batList = new List<BatQty>();
                            batList.Add(batchDetail);
                            if (detail.BatQty == null || detail.BatQty.Count < 1)
                            {
                                detail.BatQty = batList;
                            }
                            else
                            {
                                detail.BatQty.Add(batchDetail);
                            }


                        }

                        List<DTL> dtlList = new List<DTL>();
                        dtlList.Add(detail);
                        if (Hdrs.dtl == null || Hdrs.dtl.Count < 1)
                        {
                            Hdrs.dtl = dtlList;
                        }
                        else
                        {
                            Hdrs.dtl.Add(detail);
                        }
                    }
                    POST_ARData ARData = new POST_ARData();
                    ARData.hdr = Hdrs;
                    List<POST_ARData> ARList = new List<POST_ARData>();
                    ARList.Add(ARData);
                    if (ARCred.list == null || ARCred.list.Count < 1)
                    {
                        ARCred.list = ARList;
                    }
                    else
                    {
                        ARCred.list.Add(ARData);
                    }

                }
                else
                {
                    break;
                }

            }

            return ARCred;
        }


        #endregion
        #region Models

        public class POST_ARInvoice
        {
            public List<POST_ARData> list { get; set; }
        }
        public class POST_ARData
        {
            public HDR hdr { get; set; }
        }
        public class HDR
        {
            public string DocDate { get; set; }
            public string RefNum { get; set; }
            public string Comments { get; set; }
            public string CurCode { get; set; }
            public string SalesType { get; set; }
            public long DocTotal { get; set; }
            public List<DTL> dtl { get; set; }
        }

        public class DTL
        {
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string WhsCode { get; set; }
            public long LineTotal { get; set; }
            public string UomCode { get; set; }
            public List<BatQty> BatQty { get; set; }
        }
        public class BatQty
        {
            public int Qty { get; set; }
            public string ExpDate { get; set; }
            public string BatchNum { get; set; }
        }
        public class POST_ARResponseList
        {
            public List<POST_ARResponse> list = new List<POST_ARResponse>();
        }
        public class POST_ARResponse
        {
            public string RefCode { get; set; }
            public string SalesType { get; set; }
            public string Whs { get; set; }
            public string date { get; set; }
            public string message { get; set; }
            public string status { get; set; }
        }
        public class POST_ARCredResponseList
        {
            public List<POST_ARCredResponse> list = new List<POST_ARCredResponse>();
        }
        public class POST_ARCredResponse
        {

            public string RefCode { get; set; }
            public string SalesType { get; set; }
            public string message { get; set; }
            public string status { get; set; }
        }
        public class POST_JEList
        {
            public List<POST_JEData> list { get; set; }
        }

        public class POST_JEData
        {
            public POSTJE_HDR hdr { get; set; }
        }
        public class POSTJE_HDR
        {
            public string MEMO { get; set; }
            public string POST_DATE { get; set; }
            public long DocTotal { get; set; }
            public string REF1 { get; set; }
            public List<POSTJE_DTL> JEdtl { get; set; }
        }
        public class POSTJE_DTL
        {
            public string ACC_CODE { get; set; }
            public long Amount { get; set; }
            public string REF1 { get; set; }
        }
        public class POST_JEResponseList
        {
            public List<POST_JEResponse> list = new List<POST_JEResponse>();
        }
        public class POST_JEResponse
        {
            public string RefCode { get; set; }
            public string message { get; set; }
            public string status { get; set; }
        }

        #endregion

    }
}
