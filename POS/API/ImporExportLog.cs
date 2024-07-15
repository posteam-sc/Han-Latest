/*
 Posting JE was stopped in the middle of May by the request of Medi-Myanmar to do so
 
 */


using System;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Transactions;
using POS.APP_Data;

namespace POS
{

    public static class ImportExportLog
    {

        public static int CreateNewImportBatch()
        {
            POSEntities entity = new POSEntities();
            string type = "Import";
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime todayDate = DateTime.Today;
            string[] APINames = { "GET_UOM", "GET_ItemMaster", "GET_StockInByBatch" };

            int LatestID = 0;


            using (var transaction = new TransactionScope())
            {
                try
                {

                    ObjectResult<int?> InsertedID = entity.InsertImportExportLog(DateTime.Now, type, "Pending", shortCode);
                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }


                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromSAP", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));


                    dt.Rows.Add(LatestID, APINames[0], "Pending");
                    dt.Rows.Add(LatestID, APINames[1], "Pending");
                    dt.Rows.Add(LatestID, APINames[2], "Pending");



                    var parameter = new SqlParameter("@ProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.ProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportLogDetail @ProcessList", parameter);
                    transaction.Complete();
                    return LatestID;


                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return 0;
                }
            }

        }

        public static int CreateNewExportBatch(DateTime tranDate)
        {
            POSEntities entity = new POSEntities();
            string type = "Export";
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime todayDate = DateTime.Today;
            //  string[] APINames = { "POST_AR", "POST_JE", "POST_ARCred" };
            string[] APINames = { "POST_AR", "POST_ARCred" };

            int LatestID = 0;


            using (var transaction = new TransactionScope())
            {
                try
                {

                    ObjectResult<int?> InsertedID = entity.InsertImportExportLog(tranDate, type, "Pending", shortCode);
                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromSAP", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));

                    dt.Rows.Add(LatestID, APINames[0], "Pending");
                    dt.Rows.Add(LatestID, APINames[1], "Pending");
                    // dt.Rows.Add(LatestID, APINames[2], "Pending");


                    var parameter = new SqlParameter("@ProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.ProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportLogDetail @ProcessList", parameter);
                    transaction.Complete();
                    return LatestID;


                }
                catch (Exception)
                {
                    transaction.Dispose();
                    return 0;
                }
            }

        }
    }
}
