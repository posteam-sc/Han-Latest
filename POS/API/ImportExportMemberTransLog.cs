using System;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Transactions;
using POS.APP_Data;

namespace POS
{
    public static class ImportExportMemberTransLog
    {
        public static int CreateNewExportBatch(DateTime tranDate, string TransId)
        {
            POSEntities entity = new POSEntities();
            string type = "Export";
            string shortCode = SettingController.DefaultShop.ShortCode;
            DateTime todayDate = DateTime.Today;
            string[] APINames = { "POST_MemberTrans" };

            int LatestID = 0;


            using (var transaction = new TransactionScope())
            {
                try
                {

                    ObjectResult<int?> InsertedID = entity.InsertImportExportMemberTransLog(tranDate, type, "Pending", shortCode,TransId);
                    foreach (Nullable<int> result in InsertedID)
                    {
                        LatestID = result.Value;
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("BatchID", typeof(int));
                    dt.Columns.Add("ProcessName", typeof(string));
                    dt.Columns.Add("DetailStatus", typeof(string));
                    dt.Columns.Add("ResponseMessageFromPiti", typeof(string));
                    dt.Columns.Add("PostJson", typeof(string));
                    dt.Columns.Add("ResponseJson", typeof(string));

                    dt.Rows.Add(LatestID, APINames[0], "Pending");


                    var parameter = new SqlParameter("@MemberTransProcessList", SqlDbType.Structured);
                    parameter.Value = dt;
                    parameter.TypeName = "dbo.MemberTransProcessList";

                    entity.Database.ExecuteSqlCommand("exec dbo.InsertImportExportMemberTransLogDetail @MemberTransProcessList", parameter);
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
