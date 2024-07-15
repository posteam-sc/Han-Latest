using POS.APP_Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace POS
{
    public partial class Centralized : Form
    {
        public Centralized()
        {
            InitializeComponent();
        }

        private void Centralized_Load(object sender, EventArgs e)
        {
            label1.Refresh();
        }
        #region EncryptDecrypt

        /// <summary>
        /// Encrypting incomming file.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public static bool EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                string password = @"my" + SettingController.centralize_encryption_key + "123";
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;

                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateEncryptor(key, key), CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);

                fsIn.Close();
                cs.Close();
                fsCrypt.Close();

                File.Delete(inputFile);
                return false;
            }
            catch
            {

                MessageBox.Show("Encryption failed!", "Error");
                return true;
            }
        }

        /// <summary>
        /// Decrypting incomming file.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public static bool DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                string password = @"my" + SettingController.centralize_encryption_key + "123";

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, key), CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
                return false;
            }
            catch
            {
                MessageBox.Show("Decryption failed!", "Error");
                return true;
            }
        }

        #endregion

        #region Variable
        POSEntities entity = new POSEntities();
        #endregion

        #region Export
        private void btnExport_Click(object sender, EventArgs e)
        {
            int month = 2;
            try
            {
                month = Convert.ToInt16(txtmonth.Text);
                if (month >= 1 && month <= 12)
                {

                }
                else
                {
                    MessageBox.Show("Maximum value exceed enter between 1 and 12.");
                    return;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid value for export month");
                return;
            }
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter;
            DataSet dsUserRole = new DataSet("UserRole");
            DataSet dsUser = new DataSet("User");
            DataSet dsCity = new DataSet("City");
            DataSet dsConsignmentCounter = new DataSet("ConsignmentCounter");
            DataSet dsLCounter = new DataSet("Counter");
            DataSet dsCurrency = new DataSet("Currency");
            DataSet dsMemberType = new DataSet("MemberType");
            DataSet dsPaymentType = new DataSet("PaymentType");
            DataSet dsExpenseCategory = new DataSet("ExpenseCategory");

            DataSet dsRoleManagement = new DataSet("RoleManagement");
            // DataSet dsAdjustment = new DataSet("Adjustment");
            DataSet dsExpense = new DataSet("Expense");
            DataSet dsExpenseDetail = new DataSet("ExpenseDetail");
            DataSet dsMemberCardRuler = new DataSet("MemberCardRule");
            DataSet dsCustomer = new DataSet("Customer");
            DataSet dsUnitConversion = new DataSet("UnitConversion");
            DataSet dsPurchaseDetail = new DataSet("PurchaseDetail");
            DataSet dsPurchaseDetailInTransaction = new DataSet("PurchaseDetailInTransaction");
            DataSet dsPurchaseDeleteLog = new DataSet("PurchaseDeleteLog");
            DataSet dsTransaction = new DataSet("Transaction");
            DataSet dsTransactionDetail = new DataSet("TransactionDetail");
            DataSet dsConsignmentSettlement = new DataSet("ConsignmentSettlement");
            DataSet dsConsignmentSettlementDetail = new DataSet("ConsignmentSettlementDetail");
            DataSet dsExchangeRateForTransaction = new DataSet("ExchangeRateForTransaction");
            DataSet dsSPDetail = new DataSet("SPDetail");
            DataSet dsUsePrePaidDebt = new DataSet("UsePrePaidDebt");
            DataSet dsPromotion = new DataSet("Promotion");
            DataSet dsReferralProgram = new DataSet("ReferralProgram");
            DataSet dsPointHistory = new DataSet("Point_History");
            DataSet dsPointreferralpointInTransaction = new DataSet("ReferralPointInTransaction");
            DataSet dsGiftCardInTransaction = new DataSet("GiftCardInTransaction");
            DataSet dsRedeemPoint_History = new DataSet("RedeemPoint_History");
            DataSet dsPointDeductionPercentage_History = new DataSet("PointDeductionPercentage_History");

            DateTime today = DateTime.Today;
            DateTime TranDate = DateTime.Today.AddMonths(-month);


            string dtToday = today.ToString("yyyy-MM-dd");

            string dtTranDate = TranDate.ToString("yyyy-MM-dd");


            connetionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            connection = new SqlConnection(connetionString);
            string SqlUserRole = "Select * from UserRole";
            string SqlUser = "Select * from [User]";
            string sqlCity = "Select * from City";
            string sqlConsignmentCounter = "Select * from ConsignmentCounter";
            string SqlCounter = "Select * from Counter";
            string SqlCurrency = "Select * from Currency";
            string SqlMemberType = "Select * from MemberType";
            string SqlPaymentType = "Select * from PaymentType";

            //VipCustomer 
            string SqlVipCustomer = "Select * from VipCustomer where shopCode='" + SettingController.DefaultShop.ShortCode + "'";

            //ZP
            string SqlGiftCard = "Select g.*,c.CustomerCode as CustomerCode1 from GiftCard as g inner join Customer as c on g.CustomerId=c.Id where g.IsDelete=0";
            string SqlExpenseCategory = "Select * from ExpenseCategory";
            string SqlExpense = "Select * from Expense where IsApproved=1 and cast(ExpenseDate as date) >= Cast('" + dtTranDate + "' as date) and cast(ExpenseDate as date) <= Cast('" + dtToday + "' as date)";
            string SqlExpenseDetail = "Select * from ExpenseDetail ED Inner join Expense AS E on E.Id =ED.ExpenseId where E.IsApproved=1 and cast(ExpenseDate as date) >= Cast('" + dtTranDate + "' as date) and cast(ExpenseDate as date) <= Cast('" + dtToday + "' as date)";
            string sqlMemberCardRule = "select m.*,mt.Name from MemberCardRule as m inner join MemberType as mt on m.MemberTypeId=mt.Id";
            string sqlCustomer = "select Cus.*, C.CityName from Customer as Cus left join City as C on Cus.CityId = C.Id where Cus.Id in((select distinct CustomerId from RedeemPoint_History rh where rh.[DateTime]>='" + dtTranDate + "') UNION (select distinct CustomerId from GiftCard) UNION (select distinct ReferralCustomerId as CustomerId from ReferralPointInTransaction rp where rp.CreatedDate>='" + dtTranDate + "') UNION (select distinct CustomerId from [Transaction] ts where ts.UpdatedDate>='" + dtTranDate + "') UNION (select distinct CustomerId from CustomerLevelRevokeHistory clrh where clrh.ActionOn>='" + dtTranDate + "'))";
            //string SqlProductPriceChange = "Select PC.*, U.Name as UName, P.ProductCode from ProductPriceChange as PC inner join Product as P on PC.ProductId = P.Id inner join [User] as U on U.Id = PC.UserID";


            //string SqlWrapperItem = "Select W.*, P1.ProductCode as ParentProductCode, p2.ProductCode as ChildProductCode from WrapperItem as W inner join Product as P1 on p1.Id = W.ParentProductId inner join Product as P2 on p2.Id = w.ChildProductId";

            string sqlTransaction = "select T.*,T.IsCalculatePoint As IsCalculate,ISNULL(T.ParentId,'') As ParentID1,ISNULL(T.TranVouNos,'') As TranVouNos1,T.GiftCardId,T.CustomerId,T.ReceivedCurrencyId,T.ShopId, C.Address, C.Email, C.PhoneNumber, C.CustomerCode, Co.Name as CounterName, U.UserCodeNo as UserCodeNo, S.ShopName as ShopName, S.Address as SAddress,  G.CardNumber from [Transaction] as T left join Customer as C on T.CustomerId = C.Id inner join Counter Co on Co.Id = T.CounterId   inner join [User] as U on T.UserId = U.Id left join Shop as S on S.Id = T.ShopId  left join GiftCard G on G.Id = T.GiftCardId   WHERE T.IsComplete=1  and cast(T.UpdatedDate as date) >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date)";
            string sqlTransactionDetail = "select TD.*,ISNULL(Td.IsConsignmentPaid,'') As IsConsignmentPaid1, P.ProductCode , IsNull(ConsignmentNo,'') As ConsignmentNo from TransactionDetail as TD left join Point_History as PH on TD.PointHistoryId=PH.Id inner join Product as P on TD.ProductId= P.Id inner join [Transaction] as t on t.Id=TD.TransactionId left join ConsignmentSettlementDetail csd on csd.TransactionDetailId=td.Id where t.IsComplete=1 and cast(t.UpdatedDate as date) >= Cast( '" + dtTranDate + "' as date) and cast(t.UpdatedDate as date) <= Cast('" + dtToday + "' as date)";


            string SqlExchangeRateForTransaction = "Select * from ExchangeRateForTransaction E inner join [Transaction] as T on T.Id=E.TransactionId where Cast(T.UpdatedDate as date)  >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date) ";
            string SqlSPDetail = "Select SP.*, P1.ProductCode as ParentProductCode, P2.ProductCode as ChildProductCode, T.Id as TID from SPDetail as SP inner join Product as P1 on p1.Id = SP.ParentProductId inner join Product as P2 on p2.Id = SP.ChildProductId inner join TransactionDetail as Td on Td.Id = SP.TransactionDetailID inner join [Transaction] as T on T.Id = Td.TransactionId where Cast(T.UpdatedDate as date)  >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date) ";
            string SqlUsePrePaidDebt = "Select U.*, C.Name as CounterName, Us.Name as UName from UsePrePaidDebt as U inner join Counter as C on U.CounterId = C.Id inner join [User] as Us on U.CashierId = Us.Id";
            string SqlDeletelog = "Select DL.*, C.Name as CounterName, U.Name as UName from Deletelog as DL inner join Counter as C on DL.CounterId = C.Id inner join [User] as U on DL.UserId = U.Id inner join [Transaction] as T on T.Id=DL.TransactionId where Cast(T.UpdatedDate as date)  >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date) ";
            string SqlConsignmentSettlement = "Select CS.*,U.UserCodeNo as UserCodeNo from ConsignmentSettlement as CS Inner join [User] as U on U.Id=CS.CreatedBy Where  cast(SettlementDate as date) >= Cast('" + dtTranDate + "' as date) and cast(SettlementDate as date) <= Cast('" + dtToday + "' as date) ";
            string SqlConsignmentSettlementDetail = "select csd.* from ConsignmentSettlementDetail csd inner join ConsignmentSettlement cs on cs.ConsignmentNo=csd.ConsignmentNo Where cast(SettlementDate as date) >= Cast('" + dtTranDate + "' as date) and cast(SettlementDate as date) <= Cast('" + dtToday + "' as date) ";


            string SqlShop = "Select S.*,ISNULL(S.Address,'') as Address,ISNULL(S.PhoneNumber,'') as PhoneNumber,ISNULL(S.OpeningHours,'') as OpeningHours, C.CityName from Shop as S left join City as C on S.CityId = C.Id";

            string SqlStockInHeader = "Select * from StockInHeader where IsDelete=0 and IsApproved = 1 and  Status <> 'StockIn' and Cast(Date as date)  >= Cast('" + dtTranDate + "' as date) and cast(Date as date) <= Cast('" + dtToday + "' as date)";

            string SqlStockInDetail = "select SD.*, P.ProductCode, SH.StockCode ,SH.ToShopId  from StockInDetail as SD inner join Product as P on SD.ProductId= P.Id inner join StockInHeader as SH on SH.Id=SD.StockInHeaderId where IsDelete=0 and IsApproved = 1 and Status <> 'StockIn' and Cast(Date as date)  >= Cast('" + dtTranDate + "' as date) and cast(Date as date) <= Cast('" + dtToday + "' as date)  ";

            string sqlPromotion = "select p.*,b.Name as BName,m.Name as MName from Promotion as P inner join Brand as b on P.BrandId=b.Id  inner join MemberType as m on P.MemberTypeId=m.Id";
            string sqlReferralProgram = "select R.*,b.Name as BName from ReferralProgram as R inner join Brand as b on R.BrandId=b.Id";
            string sqlPointHistory = "select P.*,ISNULL(P.PRMemberTypeId,null) as PRMemberTypeId1,ISNULL(P.REFMiniPurchaseAmount,null) as REFMiniPurchaseAmount,b.Name as BName,u.Name as UserName,b.Id as BrandId1,m.Name as MName from Point_History as P left join Brand as b on P.BrandId=b.Id left join [User] as u on P.CreatedBy=u.Id left join MemberType as m on P.PRMemberTypeId=m.Id  where  cast(P.CreatedDate as date) >= Cast( '" + dtTranDate + "' as date) and cast(P.CreatedDate as date) <= Cast('" + dtToday + "' as date)";
            string sqlGiftCard_Point = " select * from [GiftCard_Point] as gp inner join [Transaction] t on t.Id = gp.TransactionId where  cast(t.UpdatedDate as date) >= Cast( '" + dtTranDate + "' as date) and cast(t.UpdatedDate as date) <= Cast('" + dtToday + "' as date)";

            string sqlReferralPointInTransaction = "select r.*,c.CustomerCode as CustomerCode1 from ReferralPointInTransaction as r inner join Customer as c on r.ReferralCustomerId=c.Id inner join [Transaction] t on t.Id = r.TransactionId where  cast(t.UpdatedDate as date) >= Cast( '" + dtTranDate + "' as date) and cast(t.UpdatedDate as date) <= Cast('" + dtToday + "' as date)";
            //ZP
            string sqlgiftcardIntransaction = "select gt.*,gc.CardNumber as CardNumber1 from GiftCardInTransaction as gt inner join GiftCard as gc on gt.GiftCardId=gc.Id inner join [Transaction] t on t.Id = gt.TransactionId where  cast(t.UpdatedDate as date) >= Cast( '" + dtTranDate + "' as date) and cast(t.UpdatedDate as date) <= Cast('" + dtToday + "' as date) and gc.IsDelete=0";
            string sqlRedeemPoint_History = "select rp.*,c.CustomerCode as CustomerCode,u.UserCodeNo as UserCode,co.Name as CounterName from RedeemPoint_History as rp inner join Customer as c on rp.CustomerId=c.Id inner join Counter as co on rp.CounterId=co.Id inner join [User] as u on rp.CasherId=u.Id where  cast(rp.DateTime as date) >= Cast( '" + dtTranDate + "' as date) and cast(rp.DateTime as date) <= Cast('" + dtToday + "' as date)";
            string SqlPointDeductionPercentage_History = "Select * from PointDeductionPercentage_History";
            string SqlTransactionPaymentDetail = "select TD.*,P.Name from TransactionPaymentDetails as TD inner join PaymentMethod as P on TD.PaymentMethodId= P.Id inner join [Transaction] as t on t.Id=TD.TransactionId where t.IsComplete=1 and cast(t.UpdatedDate as date) >= Cast( '" + dtTranDate + "' as date) and cast(t.UpdatedDate as date) <= Cast('" + dtToday + "' as date)";

            try
            {
                connection.Open();

                //Create combination data
                DataSet dsCombineData = new DataSet("ExportData");
                DataTable UserRole = new DataTable("UserRole");
                DataTable User = new DataTable("User");
                DataTable City = new DataTable("City");
                DataTable ConsignmentCounter = new DataTable("ConsignmentCounter");
                DataTable Counter = new DataTable("Counter");
                DataTable Currency = new DataTable("Currency");
                DataTable MemberType = new DataTable("MemberType");
                DataTable PaymentType = new DataTable("PaymentType");
                DataTable GiftCard = new DataTable("GiftCard");
                DataTable ExpenseCategory = new DataTable("ExpenseCategory");
                DataTable VipCustomer = new DataTable("VipCustomer");

                DataTable RoleManagement = new DataTable("RoleManagement");

                DataTable Expense = new DataTable("Expense");
                DataTable ExpenseDetail = new DataTable("ExpenseDetail");
                DataTable MemberCardRule = new DataTable("MemberCardRule");
                DataTable Customer = new DataTable("Customer");
                //  DataTable UnitConversion = new DataTable("UnitConversion");

                DataTable Transaction = new DataTable("Transaction");
                DataTable TransactionDetail = new DataTable("TransactionDetail");
                DataTable ExchangeRateForTransaction = new DataTable("ExchangeRateForTransaction");
                DataTable SPDetail = new DataTable("SPDetail");
                DataTable UsePrePaidDebt = new DataTable("UsePrePaidDebt");
                DataTable DeleteLog = new DataTable("DeleteLog");
                DataTable ConsignmentSettlement = new DataTable("ConsignmentSettlement");

                DataTable ConsignmentSettlementDetail = new DataTable("ConsignmentSettlementDetail");

                DataTable Shop = new DataTable("Shop");

                DataTable StockInHeader = new DataTable("StockInHeader");
                DataTable StockInDetail = new DataTable("StockInDetail");
                DataTable Promotion = new DataTable("Promotion");
                DataTable ReferralProgram = new DataTable("ReferralProgram");
                DataTable Point_History = new DataTable("Point_History");

                DataTable GiftCard_Point = new DataTable("GiftCard_Point");
                DataTable ReferralPointInTransaction = new DataTable("ReferralPointInTransaction");
                DataTable GiftCardInTransaction = new DataTable("GiftCardInTransaction");
                DataTable RedeemPoint_History = new DataTable("RedeemPoint_History");
                DataTable PointDeductionPercentage_History = new DataTable("PointDeductionPercentage_History");
                DataTable TransactionPaymentDetails = new DataTable("TransactionPaymentDetails");


                adapter = new SqlDataAdapter(SqlVipCustomer, connection);
                adapter.Fill(VipCustomer);

                adapter = new SqlDataAdapter(SqlUserRole, connection);
                adapter.Fill(UserRole);
                adapter = new SqlDataAdapter(SqlUser, connection);
                adapter.Fill(User);
                adapter = new SqlDataAdapter(sqlCity, connection);
                adapter.Fill(City);
                adapter = new SqlDataAdapter(sqlConsignmentCounter, connection);
                adapter.Fill(ConsignmentCounter);
                adapter = new SqlDataAdapter(SqlCounter, connection);
                adapter.Fill(Counter);
                adapter = new SqlDataAdapter(SqlCurrency, connection);
                adapter.Fill(Currency);
                adapter = new SqlDataAdapter(SqlMemberType, connection);
                adapter.Fill(MemberType);
                adapter = new SqlDataAdapter(SqlPaymentType, connection);
                adapter.Fill(PaymentType);
                adapter = new SqlDataAdapter(SqlGiftCard, connection);
                adapter.Fill(GiftCard);
                adapter = new SqlDataAdapter(SqlExpenseCategory, connection);
                adapter.Fill(ExpenseCategory);


                adapter = new SqlDataAdapter(SqlExpense, connection);
                adapter.Fill(Expense);
                adapter = new SqlDataAdapter(SqlExpenseDetail, connection);
                adapter.Fill(ExpenseDetail);
                adapter = new SqlDataAdapter(sqlMemberCardRule, connection);
                adapter.Fill(MemberCardRule);
                adapter = new SqlDataAdapter(sqlCustomer, connection);
                adapter.Fill(Customer);
                //adapter = new SqlDataAdapter(SqlProductPriceChange, connection);
                //adapter.Fill(ProductPriceChange);

                //adapter = new SqlDataAdapter(SqlWrapperItem, connection);
                //adapter.Fill(WrapperItem);

                adapter = new SqlDataAdapter(sqlTransaction, connection);
                adapter.Fill(Transaction);
                adapter = new SqlDataAdapter(sqlTransactionDetail, connection);
                adapter.Fill(TransactionDetail);
                adapter = new SqlDataAdapter(SqlExchangeRateForTransaction, connection);
                adapter.Fill(ExchangeRateForTransaction);
                adapter = new SqlDataAdapter(SqlSPDetail, connection);
                adapter.Fill(SPDetail);
                adapter = new SqlDataAdapter(SqlUsePrePaidDebt, connection);
                adapter.Fill(UsePrePaidDebt);
                adapter = new SqlDataAdapter(SqlDeletelog, connection);
                adapter.Fill(DeleteLog);
                adapter = new SqlDataAdapter(SqlConsignmentSettlement, connection);
                adapter.Fill(ConsignmentSettlement);

                adapter = new SqlDataAdapter(SqlConsignmentSettlementDetail, connection);
                adapter.Fill(ConsignmentSettlementDetail);

                adapter = new SqlDataAdapter(SqlShop, connection);
                adapter.Fill(Shop);

                adapter = new SqlDataAdapter(SqlStockInHeader, connection);
                adapter.Fill(StockInHeader);

                adapter = new SqlDataAdapter(SqlStockInDetail, connection);
                adapter.Fill(StockInDetail);

                adapter = new SqlDataAdapter(sqlPromotion, connection);
                adapter.Fill(Promotion);
                adapter = new SqlDataAdapter(sqlReferralProgram, connection);
                adapter.Fill(ReferralProgram);
                adapter = new SqlDataAdapter(sqlPointHistory, connection);
                adapter.Fill(Point_History);

                adapter = new SqlDataAdapter(sqlGiftCard_Point, connection);
                adapter.Fill(GiftCard_Point);
                adapter = new SqlDataAdapter(sqlReferralPointInTransaction, connection);
                adapter.Fill(ReferralPointInTransaction);
                adapter = new SqlDataAdapter(sqlgiftcardIntransaction, connection);
                adapter.Fill(GiftCardInTransaction);
                adapter = new SqlDataAdapter(sqlRedeemPoint_History, connection);
                adapter.Fill(RedeemPoint_History);
                adapter = new SqlDataAdapter(SqlPointDeductionPercentage_History, connection);
                adapter.Fill(PointDeductionPercentage_History);
                adapter = new SqlDataAdapter(SqlTransactionPaymentDetail, connection);
                adapter.Fill(TransactionPaymentDetails);
                dsCombineData.Tables.Add(VipCustomer);

                dsCombineData.Tables.Add(UserRole);
                dsCombineData.Tables.Add(User);
                dsCombineData.Tables.Add(City);
                dsCombineData.Tables.Add(ConsignmentCounter);
                dsCombineData.Tables.Add(Counter);
                dsCombineData.Tables.Add(Currency);
                dsCombineData.Tables.Add(MemberType);
                dsCombineData.Tables.Add(PaymentType);
                dsCombineData.Tables.Add(GiftCard);
                dsCombineData.Tables.Add(ExpenseCategory);

                dsCombineData.Tables.Add(RoleManagement);

                dsCombineData.Tables.Add(Expense);
                dsCombineData.Tables.Add(ExpenseDetail);
                dsCombineData.Tables.Add(MemberCardRule);
                dsCombineData.Tables.Add(Customer);
                //dsCombineData.Tables.Add(ProductPriceChange);
                //dsCombineData.Tables.Add(ProductQuantityChange);
                //dsCombineData.Tables.Add(WrapperItem);
                dsCombineData.Tables.Add(Transaction);
                dsCombineData.Tables.Add(TransactionDetail);
                dsCombineData.Tables.Add(ExchangeRateForTransaction);
                dsCombineData.Tables.Add(SPDetail);
                dsCombineData.Tables.Add(UsePrePaidDebt);
                dsCombineData.Tables.Add(DeleteLog);
                dsCombineData.Tables.Add(ConsignmentSettlement);
                dsCombineData.Tables.Add(ConsignmentSettlementDetail);
                dsCombineData.Tables.Add(Shop);

                dsCombineData.Tables.Add(StockInHeader);
                dsCombineData.Tables.Add(StockInDetail);
                dsCombineData.Tables.Add(Promotion);
                dsCombineData.Tables.Add(ReferralProgram);
                dsCombineData.Tables.Add(Point_History);
                dsCombineData.Tables.Add(GiftCard_Point);
                dsCombineData.Tables.Add(ReferralPointInTransaction);
                dsCombineData.Tables.Add(GiftCardInTransaction);
                dsCombineData.Tables.Add(RedeemPoint_History);
                dsCombineData.Tables.Add(PointDeductionPercentage_History);

                dsCombineData.Tables.Add(TransactionPaymentDetails);
                //Create path to save
                string activeDir = @"D:\";
                string newPath = System.IO.Path.Combine(activeDir, "POS_Export");

                if (!System.IO.Directory.Exists(newPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(newPath);
                }

                //Create File name
                string fileName = "D:/POS_Export/" + SettingController.CounterCode + "_" + "_POS_ExportFile[" + DateTime.Now.ToString("dd-MM-yyyy hh-mm tt") + "].xml";
                dsCombineData.WriteXml(fileName);
                string[] encryptFileNameArr = fileName.Split('.');
                string tempFileName = encryptFileNameArr[0] + ".sc";
                connection.Close();

                //Encrypt File and delete original file
                if (EncryptFile(fileName, tempFileName))
                {
                    return;
                }

                MessageBox.Show("Done, file saved to " + tempFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region Import
        private void btnImport_Click(object sender, EventArgs e)
        {
            string destFileName = string.Empty;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.UseWaitCursor = true;
                if (MessageBox.Show("Are you sure that you want to import the data?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (ofdImportFile.ShowDialog(this) == DialogResult.Cancel)
                    {
                        return;
                    }
                    #region Get xml filename and decrypt

                    string fileName = ofdImportFile.FileName;
                    string[] fnArr1 = fileName.Split('_');
                    string[] fnArr2 = fileName.Split('.');
                    string[] fnArr3 = fileName.Split('\\');
                    string filePath = string.Empty;
                    for (int i = 0; i < fnArr3.Length - 1; i++)
                    {
                        if (i + 1 != fnArr3.Length - 1)
                        {
                            filePath += fnArr3[i] + "/";
                        }
                        else
                        {
                            filePath += fnArr3[i];
                        }
                    }

                    /*--Decrypt DB--*/
                    for (int i = 0; i < fnArr1.Length - 1; i++)
                    {
                        if (i + 1 != fnArr1.Length - 1)
                        {
                            destFileName += fnArr1[i] + "_";
                        }
                        else
                        {
                            destFileName += fnArr1[i];
                        }
                    }
                    destFileName = destFileName + ".xml";
                    if (File.Exists(destFileName)) File.Delete(destFileName);

                    if (DecryptFile(fileName, destFileName))
                    {
                        return;
                    }

                    #endregion

                    // string delimited = @"\G(.+)[\t\u007c](.+)\r?\n";
                    //string delimited = @"'([^']*)";

                    APP_Data.POSEntities entity = new APP_Data.POSEntities();
                    //reading XML file and storing it's data to dataset.
                    DataSet dsxml = new DataSet();

                    dsxml.ReadXml(destFileName);
                    Application.DoEvents();

                    IQueryable<APP_Data.Product> productList = entity.Products;
                    IQueryable<APP_Data.Brand> brandList = entity.Brands;


                    #region UserRole

                    entity = new APP_Data.POSEntities();

                    if (dsxml.Tables["UserRole"] != null)
                    {
                        //loop through dataRow from xml and check if the UserRole is already exist or newone.
                        label1.Text = "Step 1 of 42 : Processing User Role table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["UserRole"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        foreach (DataRow dataFromXML in dsxml.Tables["UserRole"].Rows)
                        {
                            Progressbar1.PerformStep();

                            string RoleName = dataFromXML["RoleName"].ToString();
                            int Id = Convert.ToInt32(dataFromXML["Id"]);
                            APP_Data.UserRole FoundUserRole = entity.UserRoles.Where(x => x.Id == Id).FirstOrDefault();
                            //same User Role found
                            if (FoundUserRole != null)
                            {
                                GetUserRoleFromXML(dataFromXML, FoundUserRole);
                                entity.SaveChanges();

                                UserRole_ForeignKeyTBL(dataFromXML, dsxml.Tables["User"], dsxml.Tables["RoleManagement"], FoundUserRole);
                            }
                            //FoundUserRole is not exist in the current database
                            //add new row
                            else
                            {
                                APP_Data.UserRole userRole = new APP_Data.UserRole();
                                GetUserRoleFromXML(dataFromXML, userRole); ;
                                entity.UserRoles.Add(userRole);
                                entity.SaveChanges();

                                UserRole_ForeignKeyTBL(dataFromXML, dsxml.Tables["User"], dsxml.Tables["RoleManagement"], userRole);
                            }
                        }
                    }


                    #endregion

                    #region User

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["User"] != null)
                    {
                        label1.Text = "Step 2 of 42 : Processing User table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["User"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow from xml and check if the User is already exist or newone.
                        foreach (DataRow dataFromXML in dsxml.Tables["User"].Rows)
                        {
                            Progressbar1.PerformStep();
                            // string Name = dataFromXML["Name"].ToString();
                            //int UserRoleId = Convert.ToInt32(dataFromXML["UserRoleId"].ToString());
                            string UserCodeNo = dataFromXML["UserCodeNo"].ToString();
                            APP_Data.User FoundUser = entity.Users.Where(x => x.UserCodeNo == UserCodeNo).FirstOrDefault();
                            //same User Role found
                            if (FoundUser != null)
                            {

                                GetUserFromXML(dataFromXML, FoundUser);
                                entity.SaveChanges();


                                User_ForeignKeyTBL(dataFromXML, dsxml.Tables["ReferralPointInTransaction"], dsxml.Tables["Point_History"], dsxml.Tables["ConsignmentSettlement"], dsxml.Tables["DeleteLog"], dsxml.Tables["Expense"], dsxml.Tables["StockInHeader"],
                                     dsxml.Tables["UnitConversion"], dsxml.Tables["UsePrePaidDebt"], FoundUser);

                            }
                            //FoundUserRole is not exist in the current database
                            //add new row
                            else
                            {
                                APP_Data.User user = new APP_Data.User();
                                GetUserFromXML(dataFromXML, user); ;
                                entity.Users.Add(user);
                                entity.SaveChanges();

                                User_ForeignKeyTBL(dataFromXML, dsxml.Tables["ReferralPointInTransaction"], dsxml.Tables["Point_History"], dsxml.Tables["ConsignmentSettlement"], dsxml.Tables["DeleteLog"], dsxml.Tables["Expense"], dsxml.Tables["StockInHeader"],
                                   dsxml.Tables["UnitConversion"], dsxml.Tables["UsePrePaidDebt"], user);
                            }
                        }
                    }
                    IQueryable<User> userList = entity.Users;
                    #endregion

                    #region PointDeductionRule
                    entity = new APP_Data.POSEntities();
                    //loop through dataRow come from xml and check if the city is already exist or brand is new one
                    if (dsxml.Tables["PointDeductionPercentage_History"] != null)
                    {
                        label1.Text = "Step 2 of 42 : Processing City table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["PointDeductionPercentage_History"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;

                        foreach (DataRow dataRowFromXML in dsxml.Tables["PointDeductionPercentage_History"].Rows)
                        {

                            Progressbar1.PerformStep();
                            DateTime startdate = Convert.ToDateTime(dataRowFromXML["StartDate"]);
                            decimal rate = Convert.ToDecimal(dataRowFromXML["DiscountRate"]);
                            APP_Data.PointDeductionPercentage_History Foundrule = entity.PointDeductionPercentage_History.Where(x => x.DiscountRate == rate && x.StartDate == startdate).FirstOrDefault();
                            //found same ctiy name
                            if (Foundrule != null)
                            {

                                GetPointDeductRuleFromXML(dataRowFromXML, Foundrule);
                                entity.SaveChanges();


                            }
                            //City name is not exist in current database
                            //add new row
                            else
                            {
                                APP_Data.PointDeductionPercentage_History pointdeduct = new APP_Data.PointDeductionPercentage_History();
                                GetPointDeductRuleFromXML(dataRowFromXML, pointdeduct);
                                entity.PointDeductionPercentage_History.Add(pointdeduct);
                                entity.SaveChanges();

                            }
                        }
                    }
                    #endregion

                    #region City
                    entity = new APP_Data.POSEntities();
                    //loop through dataRow come from xml and check if the city is already exist or brand is new one
                    if (dsxml.Tables["City"] != null)
                    {
                        label1.Text = "Step 4 of 42 : Processing City table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["City"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;

                        foreach (DataRow dataRowFromXML in dsxml.Tables["City"].Rows)
                        {

                            Progressbar1.PerformStep();
                            int _cityId = Convert.ToInt32(dataRowFromXML["Id"].ToString());
                            string cityname = dataRowFromXML["CityName"].ToString();
                            APP_Data.City FoundCity = entity.Cities.Where(x => x.Id == _cityId).FirstOrDefault();
                            //found same ctiy name
                            if (FoundCity != null)
                            {

                                GetCityFromXML(dataRowFromXML, FoundCity);
                                entity.SaveChanges();

                                City_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["Customer"], dsxml.Tables["Shop"], FoundCity);
                            }
                            //City name is not exist in current database
                            //add new row
                            else
                            {
                                APP_Data.City city = new APP_Data.City();
                                GetCityFromXML(dataRowFromXML, city);
                                entity.Cities.Add(city);
                                entity.SaveChanges();

                                City_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["Customer"], dsxml.Tables["Shop"], city);
                            }
                        }
                    }
                    #endregion

                    #region Consigment Counter

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["ConsignmentCounter"] != null)
                    {
                        label1.Text = "Step 5 of 42 : Processing Consignment Counter table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["ConsignmentCounter"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come form xml and check if the Consigment Counter is already exist or Consigment Counter is new one
                        foreach (DataRow dataRowFromXML in dsxml.Tables["ConsignmentCounter"].Rows)
                        {

                            Progressbar1.PerformStep();
                            int id = Convert.ToInt32(dataRowFromXML["Id"].ToString());

                            APP_Data.ConsignmentCounter FoundConsignmentCounter = entity.ConsignmentCounters.Where(x => x.Id == id).FirstOrDefault();
                            //fount same consignment counter
                            if (FoundConsignmentCounter != null)
                            {

                                GetConsignmentCounterFromXML(dataRowFromXML, FoundConsignmentCounter);
                                entity.SaveChanges();

                                ConsignmentCounter_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["ConsignmentSettlement"], FoundConsignmentCounter);

                            }
                            //Consigment Counter is not exist in current database
                            //add new row
                            else
                            {
                                APP_Data.ConsignmentCounter consignmentCounter = new APP_Data.ConsignmentCounter();
                                GetConsignmentCounterFromXML(dataRowFromXML, consignmentCounter);
                                entity.ConsignmentCounters.Add(consignmentCounter);
                                entity.SaveChanges();

                                ConsignmentCounter_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["ConsignmentSettlement"], consignmentCounter);
                            }
                        }
                    }

                    #endregion

                    #region Counter

                    entity = new APP_Data.POSEntities();
                    //Loop through dataRow come from xml and check if the Counter is already exist or Counter is new one
                    if (dsxml.Tables["Counter"] != null)
                    {
                        label1.Text = "Step 6 of 42 : Processing Counter table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["Counter"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;

                        foreach (DataRow dataRowFromXML in dsxml.Tables["Counter"].Rows)
                        {

                            Progressbar1.PerformStep();
                            int _counterId = Convert.ToInt32(dataRowFromXML["Id"].ToString());
                            string CounterName = dataRowFromXML["Name"].ToString();
                            APP_Data.Counter Foundcounter = entity.Counters.Where(x => x.Name == CounterName).FirstOrDefault();
                            //Same Counter Code Found
                            if (Foundcounter != null)
                            {

                                GetCounterFromXML(dataRowFromXML, Foundcounter);
                                entity.SaveChanges();

                                Counter_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["RedeemPoint_History"], dsxml.Tables["Transaction"], dsxml.Tables["UsePrePaidDebt"], Foundcounter);

                            }
                            //Counter is not exist in the current database
                            //add new row
                            else
                            {
                                APP_Data.Counter counter = new APP_Data.Counter();
                                GetCounterFromXML(dataRowFromXML, counter);
                                entity.Counters.Add(counter);
                                entity.SaveChanges();

                                Counter_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["RedeemPoint_History"], dsxml.Tables["Transaction"], dsxml.Tables["UsePrePaidDebt"], counter);

                            }
                        }
                    }

                    IQueryable<APP_Data.Counter> counterList = entity.Counters;

                    #endregion

                    #region Currency

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["Currency"] != null)
                    {
                        label1.Text = "Step 7 of 42 : Processing Currency table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["Currency"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //through rowdata come from xml and check if the currency is already exists or new one;
                        foreach (DataRow dataFromXML in dsxml.Tables["Currency"].Rows)
                        {
                            Progressbar1.PerformStep();
                            string country = dataFromXML["Country"].ToString();
                            int latestExchangeRate = Convert.ToInt32(dataFromXML["LatestExchangeRate"].ToString());
                            APP_Data.Currency FoundCurrency = entity.Currencies.Where(x => x.Country == country).FirstOrDefault();
                            if (FoundCurrency != null)
                            {

                                if (latestExchangeRate < FoundCurrency.LatestExchangeRate)
                                {
                                    GetCurrencyFromXML(dataFromXML, FoundCurrency);
                                    entity.SaveChanges();
                                }

                                Currency_ForeignKeyTBL(dataFromXML, dsxml.Tables["Transaction"], dsxml.Tables["ExchangeRateForTransaction"], FoundCurrency);
                            }
                            else
                            {
                                APP_Data.Currency currency = new APP_Data.Currency();
                                GetCurrencyFromXML(dataFromXML, currency);
                                entity.Currencies.Add(currency);
                                entity.SaveChanges();

                                Currency_ForeignKeyTBL(dataFromXML, dsxml.Tables["Transaction"], dsxml.Tables["ExchangeRateForTransaction"], currency);
                            }
                        }
                    }
                    #endregion

                    #region Member Type

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["MemberType"] != null)
                    {
                        label1.Text = "Step 8 of 42 : Processing Member Type table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["MemberType"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //through rowdata come from xml and check if the currency is already exists or new one;
                        foreach (DataRow dataFromXML in dsxml.Tables["MemberType"].Rows)
                        {
                            Progressbar1.PerformStep();

                            int id = Convert.ToInt32(dataFromXML["Id"].ToString());
                            APP_Data.MemberType FoundMemberType = entity.MemberTypes.Where(x => x.Id == id).FirstOrDefault();
                            if (FoundMemberType != null)
                            {


                                GetMemberTypeFromXML(dataFromXML, FoundMemberType);
                                entity.SaveChanges();


                                MemberType_ForeignKeyTBL(dataFromXML, dsxml.Tables["Promotion"], dsxml.Tables["Point_History"], dsxml.Tables["MemberCardRule"], dsxml.Tables["Transaction"], FoundMemberType);
                            }
                            else
                            {
                                APP_Data.MemberType memberType = new APP_Data.MemberType();
                                GetMemberTypeFromXML(dataFromXML, memberType);
                                entity.MemberTypes.Add(memberType);
                                entity.SaveChanges();

                                MemberType_ForeignKeyTBL(dataFromXML, dsxml.Tables["Promotion"], dsxml.Tables["Point_History"], dsxml.Tables["MemberCardRule"], dsxml.Tables["Transaction"], memberType);
                            }
                        }
                    }
                    #endregion

                    #region PaymentType

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["PaymentType"] != null)
                    {
                        label1.Text = "Step 9 of 42 : Processing Payment Type table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["PaymentType"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //through rowdata come from xml and check if the currency is already exists or new one;
                        foreach (DataRow dataFromXML in dsxml.Tables["PaymentType"].Rows)
                        {
                            Progressbar1.PerformStep();

                            int Id = Convert.ToInt32(dataFromXML["Id"].ToString());
                            APP_Data.PaymentType FoundPaymentType = entity.PaymentTypes.Where(x => x.Id == Id).FirstOrDefault();
                            if (FoundPaymentType != null)
                            {

                                GetPaymentTypeFromXML(dataFromXML, FoundPaymentType);
                                entity.SaveChanges();

                                PaymentType_ForeignKeyTBL(dataFromXML, dsxml.Tables["Transaction"], FoundPaymentType);
                            }
                            else
                            {
                                APP_Data.PaymentType paymentType = new APP_Data.PaymentType();
                                GetPaymentTypeFromXML(dataFromXML, paymentType);
                                entity.PaymentTypes.Add(paymentType);
                                entity.SaveChanges();

                                PaymentType_ForeignKeyTBL(dataFromXML, dsxml.Tables["Transaction"], paymentType);
                            }
                        }
                    }
                    #endregion


                    #region Promotion

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["Promotion"] != null)
                    {
                        label1.Text = "Step 14 of 42 : Processing Promotion  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["Promotion"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["Promotion"].Rows)
                        {

                            Progressbar1.PerformStep();

                            int id = Convert.ToInt32(dataRowFromXMl["Id"]);
                            int membertypeid = Convert.ToInt32(dataRowFromXMl["MemberTypeId"]);
                            int brandid = GetBrandIdByCodeNo(dataRowFromXMl["BName"].ToString(), brandList);
                            APP_Data.Promotion FoundPromotion = entity.Promotions.Where(x => x.MemberTypeId == membertypeid && x.BrandId == brandid).FirstOrDefault();
                            if (FoundPromotion != null)
                            {

                                APP_Data.Promotion promotion = new APP_Data.Promotion();
                                GetPromotionFromXML(dataRowFromXMl, FoundPromotion);
                                entity.SaveChanges();


                            }
                            else
                            {
                                APP_Data.Promotion promotion = new APP_Data.Promotion();
                                GetPromotionFromXML(dataRowFromXMl, promotion);
                                entity.Promotions.Add(promotion);
                                entity.SaveChanges();


                            }

                        }
                    }

                    #endregion

                    #region ReferralProgram

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["ReferralProgram"] != null)
                    {
                        label1.Text = "Step 15 of 42 : Processing Promotion  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["ReferralProgram"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["ReferralProgram"].Rows)
                        {

                            Progressbar1.PerformStep();

                            int id = Convert.ToInt32(dataRowFromXMl["Id"]);
                            int brandid = GetBrandIdByCodeNo(dataRowFromXMl["BName"].ToString(), brandList);

                            APP_Data.ReferralProgram FoundReferralProgram = entity.ReferralPrograms.Where(x => x.BrandId == brandid).FirstOrDefault();
                            if (FoundReferralProgram != null)
                            {


                                APP_Data.ReferralProgram referral = new APP_Data.ReferralProgram();
                                GetReferralFromXML(dataRowFromXMl, FoundReferralProgram);
                                entity.SaveChanges();


                            }
                            else
                            {
                                APP_Data.ReferralProgram referral = new APP_Data.ReferralProgram();
                                GetReferralFromXML(dataRowFromXMl, referral);
                                entity.ReferralPrograms.Add(referral);
                                entity.SaveChanges();


                            }

                        }
                    }

                    #endregion

                    #region Point_History

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["Point_History"] != null)
                    {
                        label1.Text = "Step 16 of 42 : Processing Promotion History  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["Point_History"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["Point_History"].Rows)
                        {

                            Progressbar1.PerformStep();

                            int id = Convert.ToInt32(dataRowFromXMl["Id"]);
                            APP_Data.Point_History FoundPointHistory = entity.Point_History.Where(x => x.Id == id).FirstOrDefault();
                            if (FoundPointHistory != null)
                            {


                                APP_Data.Point_History pointhistory = new APP_Data.Point_History();
                                GetPointHistoryFromXML(dataRowFromXMl, FoundPointHistory);
                                entity.SaveChanges();

                                PointHistory_ForeignKeyTBL(dataRowFromXMl, dsxml.Tables["TransactionDetail"], dsxml.Tables["GiftCard_Point"], FoundPointHistory);
                            }
                            else
                            {
                                APP_Data.Point_History pointhistory = new APP_Data.Point_History();
                                GetPointHistoryFromXML(dataRowFromXMl, pointhistory);
                                entity.Point_History.Add(pointhistory);

                                entity.SaveChanges();
                                PointHistory_ForeignKeyTBL(dataRowFromXMl, dsxml.Tables["TransactionDetail"], dsxml.Tables["GiftCard_Point"], pointhistory);
                                //   ProductSubCag_ForeignKeyTBL(dataRowFromXMl, dtxmlProduct, promotion);
                            }

                        }
                    }

                    #endregion


                    #region Expense Category

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["ExpenseCategory"] != null)
                    {
                        label1.Text = "Step 17 of 42 : Processing Expense Category table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["ExpenseCategory"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["ExpenseCategory"].Rows)
                        {

                            Progressbar1.PerformStep();

                            int id = Convert.ToInt32(dataRowFromXMl["Id"].ToString());
                            APP_Data.ExpenseCategory FoundExpenseCategory = entity.ExpenseCategories.Where(x => x.Id == id).FirstOrDefault();
                            if (FoundExpenseCategory != null)
                            {

                                GetExpenseCategoryFromXML(dataRowFromXMl, FoundExpenseCategory);
                                entity.SaveChanges();

                                ExpenseCag_ForeignKeyTBL(dataRowFromXMl, dsxml.Tables["Expense"], FoundExpenseCategory);
                            }
                            else
                            {
                                APP_Data.ExpenseCategory expenseCag = new APP_Data.ExpenseCategory();
                                GetExpenseCategoryFromXML(dataRowFromXMl, expenseCag);
                                entity.ExpenseCategories.Add(expenseCag);
                                entity.SaveChanges();

                                ExpenseCag_ForeignKeyTBL(dataRowFromXMl, dsxml.Tables["Expense"], expenseCag);
                            }
                        }
                    }

                    #endregion

                    #region Expense 

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["Expense"] != null)
                    {
                        label1.Text = "Step 18 of 42 : Processing Expense  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["Expense"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["Expense"].Rows)
                        {

                            Progressbar1.PerformStep();

                            string id = dataRowFromXMl["Id"].ToString();
                            APP_Data.Expense FoundExpense = entity.Expenses.Where(x => x.Id == id).FirstOrDefault();
                            if (FoundExpense != null)
                            {

                                ////GetExpenseFromXML(dataRowFromXMl, FoundExpense);
                                ////entity.SaveChanges();

                                ////Expense_ForeignKeyTBL(dataRowFromXMl, dtxmlExpenseDetail, FoundExpense);

                                APP_Data.Expense expense = new APP_Data.Expense();
                                GetExpenseFromXML(dataRowFromXMl, expense);
                                entity.Expenses.Add(expense);
                                entity.SaveChanges();

                                Expense_ForeignKeyTBL(dataRowFromXMl, dsxml.Tables["ExpenseDetail"], expense);
                            }

                        }
                    }

                    #endregion

                    #region Expense Detail

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["ExpenseDetail"] != null)
                    {
                        label1.Text = "Step 19 of 42 : Processing Expense Detail  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["ExpenseDetail"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["ExpenseDetail"].Rows)
                        {

                            Progressbar1.PerformStep();

                            string _expenseId = dataRowFromXMl["ExpenseId"].ToString();
                            string _description = dataRowFromXMl["Description"].ToString();
                            APP_Data.ExpenseDetail FoundExpenseDetail = entity.ExpenseDetails.Where(x => x.ExpenseId == _expenseId && x.Description == _description).FirstOrDefault();
                            if (FoundExpenseDetail != null)
                            {

                                APP_Data.ExpenseDetail expenseDetail = new APP_Data.ExpenseDetail();
                                GetExpenseDetailFromXML(dataRowFromXMl, expenseDetail);
                                entity.ExpenseDetails.Add(expenseDetail);
                                entity.SaveChanges();
                            }





                        }
                    }




                    #endregion


                    #region Member Card Rule

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["MemberCardRule"] != null)
                    {
                        label1.Text = "Step 20 of 42 : Processing Member Card Rule table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["MemberCardRule"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["MemberCardRule"].Rows)
                        {

                            Progressbar1.PerformStep();

                            int _id = Convert.ToInt32(dataRowFromXMl["Id"].ToString());
                            int _memberTypeId = Convert.ToInt32(dataRowFromXMl["MemberTypeId"].ToString());
                            APP_Data.MemberCardRule FoundMemberCardRule = entity.MemberCardRules.Where(x => x.MemberTypeId == _memberTypeId).FirstOrDefault();
                            if (FoundMemberCardRule != null)
                            {

                                GetMemberCardRuleFromXML(dataRowFromXMl, FoundMemberCardRule);
                                entity.SaveChanges();

                            }
                            else
                            {
                                APP_Data.MemberCardRule memberCardRule = new APP_Data.MemberCardRule();
                                GetMemberCardRuleFromXML(dataRowFromXMl, memberCardRule);
                                entity.MemberCardRules.Add(memberCardRule);
                                entity.SaveChanges();


                            }
                        }
                    }

                    #endregion

                    #region Customer
                    entity = new APP_Data.POSEntities();
                    //Loop through dataRow come from xml and check if the customer is already exist or a new customer
                    if (dsxml.Tables["Customer"] != null)
                    {
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["Customer"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        label1.Text = "Step 21 of 42 : Processing Customer table please wait!";
                        label1.Refresh();

                        foreach (DataRow dataRowFromXML in dsxml.Tables["Customer"].Rows)
                        {
                            Progressbar1.PerformStep();

                            String CustomerCode = dataRowFromXML["CustomerCode"].ToString();

                            APP_Data.Customer FoundCustomer = entity.Customers.Where(x => x.CustomerCode == CustomerCode).FirstOrDefault();

                            if (FoundCustomer != null)
                            {

                                //APP_Data.Customer FoundCustomer = entity.Customers.Where(x => x.Id == SameCustomerId).FirstOrDefault();
                                GetCustomerFromXML(dataRowFromXML, FoundCustomer);
                                entity.SaveChanges();

                                //Customer_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["RedeemPoint_History"], dsxml.Tables["GiftCard"], dsxml.Tables["ReferralPointInTransaction"], dsxml.Tables["Transaction"], FoundCustomer);
                            }
                            //Customer Data is not exist in the database
                            //add new row
                            else
                            {
                                APP_Data.Customer cs = new APP_Data.Customer();
                                GetCustomerFromXML(dataRowFromXML, cs);
                                entity.Customers.Add(cs);
                                entity.SaveChanges();

                                //Customer_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["RedeemPoint_History"], dsxml.Tables["GiftCard"], dsxml.Tables["ReferralPointInTransaction"], dsxml.Tables["Transaction"], cs);

                            }
                        }
                    }

                    IQueryable<Customer> custList = entity.Customers;

                    #endregion

                    #region GiftCard

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["GiftCard"] != null)
                    {
                        label1.Text = "Step 22 of 42 : Processing Gift Card table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["GiftCard"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //through rowdata come from xml and check if the GiftCard is already existed or new one;
                        foreach (DataRow dataFromXML in dsxml.Tables["GiftCard"].Rows)
                        {

                            Progressbar1.PerformStep();
                            string CardNumber = dataFromXML["CardNumber"].ToString();
                            dataFromXML["CustomerId"] = GetCustomerByCodeNo(dataFromXML["CustomerCode1"].ToString(), custList);


                            //ZP
                            APP_Data.GiftCard FoundGiftCard = entity.GiftCards.Where(x => x.CardNumber == CardNumber && x.IsDelete == false).FirstOrDefault();
                            if (FoundGiftCard != null)
                            {


                                GetGiftCardFromXML(dataFromXML, FoundGiftCard);
                                entity.SaveChanges();

                                // GiftCard_ForeignKeyTBL(dataFromXML, dsxml.Tables["GiftCardInTransaction"], dsxml.Tables["Transaction"], FoundGiftCard);
                            }
                            else
                            {
                                APP_Data.GiftCard giftcard = new APP_Data.GiftCard();
                                GetGiftCardFromXML(dataFromXML, giftcard);
                                entity.GiftCards.Add(giftcard);
                                entity.SaveChanges();

                                // GiftCard_ForeignKeyTBL(dataFromXML, dsxml.Tables["GiftCardInTransaction"], dsxml.Tables["Transaction"], giftcard);
                            }
                        }
                    }
                    IQueryable<APP_Data.GiftCard> gcList = entity.GiftCards;
                    #endregion

                    #region GiftCard_Point
                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["GiftCard_Point"] != null)
                    {
                        label1.Text = "Step 23 of 42 : Processing Promotion  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["GiftCard_Point"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["GiftCard_Point"].Rows)
                        {

                            Progressbar1.PerformStep();

                            string transactionId = dataRowFromXMl["TransactionId"].ToString();
                            int transactionDetailId = Convert.ToInt32(dataRowFromXMl["TransactionDetailId"]);
                            APP_Data.GiftCard_Point FoundGiftCard_Point = entity.GiftCard_Point.Where(x => x.TransactionId == transactionId && x.TransactionDetailId == transactionDetailId).FirstOrDefault();
                            if (FoundGiftCard_Point != null)
                            {

                                APP_Data.GiftCard_Point giftcardpoint = new APP_Data.GiftCard_Point();
                                GetGiftCardPointFromXML(dataRowFromXMl, FoundGiftCard_Point);
                                entity.SaveChanges();


                            }
                            else
                            {
                                APP_Data.GiftCard_Point giftcardpoint = new APP_Data.GiftCard_Point();
                                GetGiftCardPointFromXML(dataRowFromXMl, giftcardpoint);
                                entity.GiftCard_Point.Add(giftcardpoint);
                                entity.SaveChanges();

                            }


                        }
                    }
                    #endregion


                    #region Shop
                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["Shop"] != null)
                    {
                        label1.Text = "Step 27 of 42 : Processing Shop table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["Shop"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow from xml and check if the shop is already exist or newone.
                        foreach (DataRow dataFromXML in dsxml.Tables["Shop"].Rows)
                        {
                            Progressbar1.PerformStep();
                            //string ShopName = dataFromXML["ShopName"].ToString();
                            //string Address = dataFromXML["Address"].ToString();

                            int _shopId = Convert.ToInt32(dataFromXML["Id"].ToString());
                            APP_Data.Shop FoundShop = entity.Shops.Where(x => x.Id == _shopId).FirstOrDefault();
                            //same shop name found
                            if (FoundShop != null)
                            {

                                GetShopFromXML(dataFromXML, FoundShop);
                                entity.SaveChanges();

                                Shop_ForeignKeyTBL(dataFromXML, dsxml.Tables["Transaction"], dsxml.Tables["User"], FoundShop);
                            }
                            //shop name is not exist in the current database
                            //add new row
                            else
                            {
                                APP_Data.Shop shop = new APP_Data.Shop();
                                GetShopFromXML(dataFromXML, shop);
                                entity.Shops.Add(shop);
                                entity.SaveChanges();
                                Shop_ForeignKeyTBL(dataFromXML, dsxml.Tables["Transaction"], dsxml.Tables["User"], shop);
                            }
                        }
                    }
                    #endregion


                    #region Transaction

                    entity = new APP_Data.POSEntities();
                    List<int> TranIdList = new List<int>();

                    if (dsxml.Tables["Transaction"] != null)
                    {
                        label1.Text = "Step 28 of 42 : Processing Transaction table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["Transaction"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //Loop through dataRow come from xml and check if the transaction is already exist or a new one
                        DateTime today = DateTime.Today;
                        DateTime TranDate = DateTime.Today.AddMonths(-5);
                        DataTable _TranProcessing = new DataTable();

                        foreach (DataRow dataRowFromXML in dsxml.Tables["Transaction"].Rows)
                        {
                            Progressbar1.PerformStep();
                            String Id = dataRowFromXML["Id"].ToString();

                            dataRowFromXML["CustomerId"] = GetCustomerByCodeNo(dataRowFromXML["CustomerCode"].ToString(), custList);

                            //Added by YMO
                            if (Convert.ToString(dataRowFromXML["CustomerId"]) == "0")
                            {
                                dataRowFromXML["CustomerId"] = null;
                            }
                            dataRowFromXML["UserId"] = GetUserByCodeNo(dataRowFromXML["UserCodeNo"].ToString(), userList);
                            //

                            if (dsxml.Tables["Transaction"].Columns.Contains("CardNumber"))
                            {
                                dataRowFromXML["GiftCardId"] = GetGiftCardIdByCardNo(dataRowFromXML["CardNumber"].ToString(), gcList);
                            }
                            APP_Data.Transaction FoundTransaction = entity.Transactions.Where(x => x.Id == Id).FirstOrDefault();
                            //Same TransactionId Found
                            if (FoundTransaction != null)
                            {
                                DateTime currentUpdateDateFromXML = Convert.ToDateTime(dataRowFromXML["UpdatedDate"].ToString());

                                if (FoundTransaction.UpdatedDate < currentUpdateDateFromXML)
                                {
                                    GetTransactionFromXML(dataRowFromXML, FoundTransaction);
                                    entity.SaveChanges();

                                    Transaction_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["GiftCardInTransaction"], dsxml.Tables["ReferralPointInTransaction"], dsxml.Tables["GiftCard_Point"], dsxml.Tables["TransactionDetail"], dsxml.Tables["UsePrePaidDebt"], dsxml.Tables["ExchangeRateForTransaction"], FoundTransaction);
                                }
                            }
                            //Transaction Id is not exist in current database
                            //add new row
                            else
                            {
                                APP_Data.Transaction ts = new APP_Data.Transaction();
                                APP_Data.Transaction tsRefund = new APP_Data.Transaction();
                                //Check  for transaction type whether sale or credit or sale or prepaid or fefund or credit refund 

                                if (dataRowFromXML["Type"].ToString() == "Refund" || dataRowFromXML["Type"].ToString() == "CreditRefund")
                                {
                                    //Get  sale or credit transaction Id of  current refund or credit refund transaction.
                                    string OldParentId = dataRowFromXML["ParentId1"].ToString();
                                    //chcek at main Main Database whether having Transaction about parentId of current refund  or credit refund or debt transaction.
                                    APP_Data.Transaction tsParentId = entity.Transactions.Where(x => x.Id == OldParentId).FirstOrDefault();

                                    if (tsParentId != null)
                                    {
                                        GetTransactionFromXML(dataRowFromXML, ts);

                                        entity.Transactions.Add(ts);
                                        entity.SaveChanges();
                                    }
                                    else
                                    {

                                        GetTransactionFromXML(dataRowFromXML, ts);
                                        string tempParent = ts.ParentId;
                                        if (ts.ParentId.StartsWith("TS"))
                                        {
                                            ts.ParentId = null;
                                        }
                                        entity.Transactions.Add(ts);
                                        entity.SaveChanges();
                                        bool bTransExist = false;
                                        foreach (DataRow dr in dsxml.Tables["Transaction"].Select("Id ='" + OldParentId + "'"))
                                        {
                                            dr["UserId"] = GetUserByCodeNo(dr["UserCodeNo"].ToString(), userList);//Added by YMO
                                            dr["CustomerId"] = GetCustomerByCodeNo(dr["CustomerCode"].ToString(), custList);//Added by YMO
                                            if (Convert.ToString(dataRowFromXML["CustomerId"]) == "0")//Added by YMO
                                            {
                                                dataRowFromXML["CustomerId"] = null;//Added by YMO
                                            }
                                            if (dr != null && dr["Type"].ToString() != "Refund" && dr["Type"].ToString() != "CreditRefund")
                                            {
                                                bTransExist = true;
                                                GetTransactionFromXML(dr, tsRefund);

                                                entity.Transactions.Add(tsRefund);

                                                entity.SaveChanges();

                                                Tran_ForeignKeyTBL(dr, dsxml.Tables["GiftCard_Point"], dsxml.Tables["GiftCardInTransaction"], dsxml.Tables["ReferralPointInTransaction"], dsxml.Tables["TransactionDetail"], dsxml.Tables["UsePrePaidDebt"], dsxml.Tables["ExchangeRateForTransaction"], tsRefund);
                                            }
                                        }
                                        if (bTransExist)
                                        {
                                            ts.ParentId = tempParent;
                                            entity.SaveChanges();
                                        }

                                    }
                                }
                                else
                                {
                                    GetTransactionFromXML(dataRowFromXML, ts);
                                    entity.Transactions.Add(ts);
                                    entity.SaveChanges();
                                }

                                Tran_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["GiftCard_Point"], dsxml.Tables["GiftCardInTransaction"], dsxml.Tables["ReferralPointInTransaction"], dsxml.Tables["TransactionDetail"], dsxml.Tables["UsePrePaidDebt"], dsxml.Tables["ExchangeRateForTransaction"], ts);
                            }

                        }
                    }
                    #endregion

                    #region TransactionDetail

                    entity = new APP_Data.POSEntities();

                    if (dsxml.Tables["TransactionDetail"] != null)
                    {
                        //Loop through dataRow come from xml and check if the transaction is already exist or a new one
                        label1.Text = "Step 29 of 42 : Processing Transaction Detail table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        //Progressbar1.Maximum = dtxmlTransactionDetail.Rows.Count;
                        Progressbar1.Maximum = dsxml.Tables["TransactionDetail"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;

                        foreach (DataRow dataRowFromXML in dsxml.Tables["TransactionDetail"].Rows)
                        {
                            Progressbar1.PerformStep();
                            String TransactionId = dataRowFromXML["TransactionId"].ToString();


                            Boolean IsDelete = Convert.ToBoolean(dataRowFromXML["IsDeleted"].ToString());
                            string pid = dataRowFromXML["ProductId"].ToString();
                            string ProductCode = dataRowFromXML["ProductCode"].ToString();

                            long ProductId = FindProductIdByCode(ProductCode, productList);
                            dataRowFromXML["ProductId"] = ProductId;
                            if (TransactionId == "TSMP021749" || TransactionId == "TSMP022162")
                            {

                            }
                            string BatchNo = null;
                            APP_Data.TransactionDetail FoundTransactionDetail = null;
                            if (dataRowFromXML.Table.Columns.Contains("BatchNo"))
                            {
                                BatchNo = dataRowFromXML["BatchNo"].ToString();
                            }
                            FoundTransactionDetail = entity.TransactionDetails.Where(x => x.TransactionId == TransactionId && x.ProductId == ProductId && x.BatchNo == BatchNo).FirstOrDefault();


                            //Same TransactionId Found
                            if (FoundTransactionDetail != null)
                            {
                                GetTransactionDetailFromXML(dataRowFromXML, FoundTransactionDetail);
                                entity.SaveChanges();

                                TransactionDetail_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["GiftCard_Point"], dsxml.Tables["UsePrePaidDebt"], dsxml.Tables["SPDetail"], dsxml.Tables["ConsignmentSettlementDetail"], dsxml.Tables["DeleteLog"], FoundTransactionDetail);

                            }
                            //Transaction Id is not exist in current database
                            //add new row
                            else
                            {
                                APP_Data.TransactionDetail ts = new APP_Data.TransactionDetail();
                                GetTransactionDetailFromXML(dataRowFromXML, ts);
                                entity.TransactionDetails.Add(ts);
                                entity.SaveChanges();

                                TransactionDetail_ForeignKeyTBL(dataRowFromXML, dsxml.Tables["GiftCard_Point"], dsxml.Tables["UsePrePaidDebt"], dsxml.Tables["SPDetail"], dsxml.Tables["ConsignmentSettlementDetail"], dsxml.Tables["DeleteLog"], ts);
                            }
                        }
                    }
                    #endregion

                    #region Transaction Payment Detail
                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["TransactionPaymentDetails"] != null)
                    {
                        //Loop through dataRow come from xml and check if the transaction is already exist or a new one
                        label1.Text = "Step 30 of 42 : Processing Transaction Payment Detail table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        //Progressbar1.Maximum = dtxmlTransactionDetail.Rows.Count;
                        Progressbar1.Maximum = dsxml.Tables["TransactionPaymentDetails"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;

                        foreach (DataRow dataRowFromXML in dsxml.Tables["TransactionPaymentDetails"].Rows)
                        {
                            Progressbar1.PerformStep();
                            Boolean SameRowExist = false;
                            String TransactionId = dataRowFromXML["TransactionId"].ToString();
                            int paymentMethodId = Convert.ToInt32(dataRowFromXML["PaymentMethodId"].ToString());
                            //long ProductId = Convert.ToInt64(dataRowFromXML["ProductId"].ToString());
                            //Boolean IsDelete = Convert.ToBoolean(dataRowFromXML["IsDeleted"].ToString());
                            //string ProductCode = dataRowFromXML["ProductCode"].ToString();
                            var FoundTransactionPaymentDetail = entity.TransactionPaymentDetails.Where(x => x.TransactionId == TransactionId && x.PaymentMethodId == paymentMethodId).FirstOrDefault();

                            //Same TransactionId Found
                            if (FoundTransactionPaymentDetail != null)
                            {
                                SameRowExist = true;
                            }
                            //Found same row,Update the Current One if xml transactions have more recent updatedDate
                            if (SameRowExist)
                            {
                                GetTransactionPaymentDetailFromXML(dataRowFromXML, FoundTransactionPaymentDetail);
                                entity.SaveChanges();

                            }
                            //Transaction Id is not exist in current database
                            //add new row
                            else
                            {
                                APP_Data.TransactionPaymentDetail ts = new APP_Data.TransactionPaymentDetail();
                                GetTransactionPaymentDetailFromXML(dataRowFromXML, ts);
                                entity.TransactionPaymentDetails.Add(ts);
                                entity.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    #region ReferralInTransactionProgram

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["ReferralPointInTransaction"] != null)
                    {
                        label1.Text = "Step 31 of 42: Processing Referral In Transaction  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["ReferralPointInTransaction"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["ReferralPointInTransaction"].Rows)
                        {

                            Progressbar1.PerformStep();

                            int id = Convert.ToInt32(dataRowFromXMl["Id"]);
                            string trans = Convert.ToString(dataRowFromXMl["TransactionId"]);
                            dataRowFromXMl["ReferralCustomerId"] = GetCustomerByCodeNo(dataRowFromXMl["CustomerCode1"].ToString(), custList);

                            APP_Data.ReferralPointInTransaction FoundReferralTransactionProgram = entity.ReferralPointInTransactions.Where(x => x.TransactionId == trans).FirstOrDefault();
                            if (FoundReferralTransactionProgram != null)
                            {


                                APP_Data.ReferralPointInTransaction referral = new APP_Data.ReferralPointInTransaction();
                                GetReferralTransactionFromXML(dataRowFromXMl, FoundReferralTransactionProgram);
                                entity.SaveChanges();


                            }
                            else
                            {
                                APP_Data.ReferralPointInTransaction referral = new APP_Data.ReferralPointInTransaction();
                                GetReferralTransactionFromXML(dataRowFromXMl, referral);
                                entity.ReferralPointInTransactions.Add(referral);
                                entity.SaveChanges();


                            }

                        }
                    }

                    #endregion



                    #region ExchangeRateForTransaction

                    entity = new APP_Data.POSEntities();

                    if (dsxml.Tables["ExchangeRateForTransaction"] != null)
                    {
                        label1.Text = "Step 32 of 42 : Processing Exchange Rate For Transaction table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["ExchangeRateForTransaction"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through the dataRow come from xml and check if ExchangeRateForTransaction is already existed or new one
                        foreach (DataRow dataFromXML in dsxml.Tables["ExchangeRateForTransaction"].Rows)
                        {

                            Progressbar1.PerformStep();
                            String TransactionId = dataFromXML["TransactionId"].ToString();
                            APP_Data.ExchangeRateForTransaction FoundExchangeRate = entity.ExchangeRateForTransactions.Where(x => x.TransactionId == TransactionId).FirstOrDefault();
                            if (FoundExchangeRate != null)
                            {

                                GetExchangeReateForTansction(dataFromXML, FoundExchangeRate);
                                entity.SaveChanges();
                            }
                            else
                            {
                                APP_Data.ExchangeRateForTransaction exchageRate = new APP_Data.ExchangeRateForTransaction();
                                GetExchangeReateForTansction(dataFromXML, exchageRate);
                                entity.ExchangeRateForTransactions.Add(exchageRate);
                                entity.SaveChanges();
                            }
                        }
                    }

                    #endregion

                    #region SPDetail
                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["SPDetail"] != null)
                    {
                        label1.Text = "Step 33 of 42 : Processing SP Detail table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["SPDetail"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        foreach (DataRow dataFromXML in dsxml.Tables["SPDetail"].Rows)
                        {
                            Progressbar1.PerformStep();
                            bool SameRowExit = false;
                            int SameFieldCount = 0;
                            string spId = "";

                            int TransactionDetailID = Convert.ToInt32(dataFromXML["TransactionDetailID"].ToString());
                            long ParentProductID = Convert.ToInt64(dataFromXML["ParentProductID"].ToString());
                            long ChildProductID = Convert.ToInt64(dataFromXML["ChildProductID"].ToString());
                            string SPDetailID = dataFromXML["SPDetailID"].ToString();

                            foreach (APP_Data.SPDetail sp in entity.SPDetails)
                            {
                                SameFieldCount = 0;
                                if (sp.TransactionDetailID == TransactionDetailID) SameFieldCount++;
                                if (sp.ParentProductID == ParentProductID) SameFieldCount++;
                                if (sp.ChildProductID == ChildProductID) SameFieldCount++;
                                if (sp.SPDetailID == SPDetailID) SameFieldCount++;

                                if (SameFieldCount >= 4)
                                {
                                    SameRowExit = true;
                                    spId = sp.SPDetailID;
                                    break;
                                }
                            }
                            if (SameRowExit)
                            {
                                APP_Data.SPDetail foundSpDetail = entity.SPDetails.Where(x => x.SPDetailID == spId).FirstOrDefault();
                                GetSPDetailFromXML(dataFromXML, foundSpDetail);
                                entity.SaveChanges();
                            }
                            else
                            {
                                APP_Data.SPDetail spDObj = new APP_Data.SPDetail();
                                GetSPDetailFromXML(dataFromXML, spDObj);
                                entity.SPDetails.Add(spDObj);
                                entity.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    #region UsePrePaidDebt

                    entity = new APP_Data.POSEntities();

                    if (dsxml.Tables["UsePrePaidDebt"] != null)
                    {
                        label1.Text = "Step 34 of 42 : Processing Use Pre Paid Debt table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["UsePrePaidDebt"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //through rowdata come from xml and check if the GiftCard is already existed or new one;
                        foreach (DataRow dataFromXML in dsxml.Tables["UsePrePaidDebt"].Rows)
                        {
                            Progressbar1.PerformStep();
                            string CreditTransactionId = dataFromXML["CreditTransactionId"].ToString();
                            string PrePaidDebtTransactionId = dataFromXML["PrePaidDebtTransactionId"].ToString();
                            int UseAmount = Convert.ToInt32(dataFromXML["UseAmount"].ToString());

                            //APP_Data.UsePrePaidDebt FoundUserPrePaidDebt = entity.UsePrePaidDebts.Where(x => x.CreditTransactionId == CreditTransactionId && x.PrePaidDebtTransactionId == PrePaidDebtTransactionId && x.UseAmount == UseAmount).FirstOrDefault();
                            APP_Data.UsePrePaidDebt FoundUserPrePaidDebt = entity.UsePrePaidDebts.Where(x => x.CreditTransactionId == CreditTransactionId).FirstOrDefault();
                            if (FoundUserPrePaidDebt != null)
                            {

                                UserPrePaidDebtFromXML(dataFromXML, FoundUserPrePaidDebt);
                                entity.SaveChanges();
                            }
                            else
                            {
                                try
                                {
                                    APP_Data.UsePrePaidDebt usePrePaidDebt = new APP_Data.UsePrePaidDebt();
                                    UserPrePaidDebtFromXML(dataFromXML, usePrePaidDebt);
                                    entity.UsePrePaidDebts.Add(usePrePaidDebt);
                                    entity.SaveChanges();
                                }
                                catch
                                {

                                }
                            }
                        }
                    }

                    #endregion

                    #region Deletelog

                    entity = new APP_Data.POSEntities();

                    if (dsxml.Tables["DeleteLog"] != null)
                    {
                        label1.Text = "Step 35 of 42 : Processing Delete Log table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["DeleteLog"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow from xml and check if the deletelog is already exist  or a new one
                        foreach (DataRow dataFromXML in dsxml.Tables["DeleteLog"].Rows)
                        {

                            Progressbar1.PerformStep();
                            string transactionId = dataFromXML["TransactionId"].ToString();
                            APP_Data.DeleteLog FoundDeleteLog = entity.DeleteLogs.Where(x => x.TransactionId == transactionId).FirstOrDefault();
                            string counterName = dataFromXML[6].ToString();//Added by YMO
                            APP_Data.Counter Counter = entity.Counters.Where(x => x.Name == counterName).FirstOrDefault();//Added by YMO
                            dataFromXML["CounterId"] = Counter.Id;//Added by YMO

                            if (FoundDeleteLog != null)
                            {
                                GetDeleteLogFromXML(dataFromXML, FoundDeleteLog);
                                entity.SaveChanges();
                            }
                            else
                            {

                                APP_Data.DeleteLog deleteLog = new APP_Data.DeleteLog();
                                GetDeleteLogFromXML(dataFromXML, deleteLog);
                                entity.DeleteLogs.Add(deleteLog);
                                entity.SaveChanges();
                            }
                        }
                    }

                    #endregion


                    #region Consingment Settlement

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["ConsignmentSettlement"] != null)
                    {
                        label1.Text = "Step 36 of 42 : Processing Consingment Settlement  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["ConsignmentSettlement"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["ConsignmentSettlement"].Rows)
                        {

                            Progressbar1.PerformStep();

                            string _consignmentNo = dataRowFromXMl["ConsignmentNo"].ToString();
                            APP_Data.ConsignmentSettlement FoundConsignmentSettlement = entity.ConsignmentSettlements.Where(x => x.ConsignmentNo == _consignmentNo).FirstOrDefault();
                            if (FoundConsignmentSettlement != null)
                            {

                                GetConsignmentSettlementFromXML(dataRowFromXMl, FoundConsignmentSettlement);
                                entity.SaveChanges();

                            }
                            else
                            {
                                APP_Data.ConsignmentSettlement consignmentSettlement = new APP_Data.ConsignmentSettlement();
                                GetConsignmentSettlementFromXML(dataRowFromXMl, consignmentSettlement);
                                entity.ConsignmentSettlements.Add(consignmentSettlement);
                                entity.SaveChanges();


                            }
                        }
                    }

                    #endregion


                    #region Consingment Settlement Detail

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["ConsignmentSettlementDetail"] != null)
                    {
                        label1.Text = "Step 37 of 42 : Processing Consingment Settlement Detail  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["ConsignmentSettlementDetail"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["ConsignmentSettlementDetail"].Rows)
                        {

                            Progressbar1.PerformStep();

                            string _consignmentNo = dataRowFromXMl["ConsignmentNo"].ToString();
                            int _tranDetailId = Convert.ToInt32(dataRowFromXMl["TransactionDetailId"].ToString());
                            APP_Data.ConsignmentSettlementDetail FoundConsignmentSettlementDetail = entity.ConsignmentSettlementDetails.Where(x => x.ConsignmentNo == _consignmentNo && x.TransactionDetailId == _tranDetailId).FirstOrDefault();
                            if (FoundConsignmentSettlementDetail != null)
                            {
                                GetConsignmentSettlementDetailFromXML(dataRowFromXMl, FoundConsignmentSettlementDetail);
                                entity.SaveChanges();

                            }
                            else
                            {
                                APP_Data.ConsignmentSettlementDetail consignmentSettlementDetail = new APP_Data.ConsignmentSettlementDetail();
                                GetConsignmentSettlementDetailFromXML(dataRowFromXMl, consignmentSettlementDetail);
                                entity.ConsignmentSettlementDetails.Add(consignmentSettlementDetail);
                                entity.SaveChanges();


                            }
                        }
                    }

                    #endregion

                    #region Stock In Header

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["StockInHeader"] != null)
                    {
                        label1.Text = "Step 38 of 42 : Processing Stock In Header  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["StockInHeader"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["StockInHeader"].Rows)
                        {

                            Progressbar1.PerformStep();
                            string _stockCode = dataRowFromXMl["StockCode"].ToString();
                            int _toShopId = Convert.ToInt32(dataRowFromXMl["ToShopId"].ToString());
                            bool SameRowExit = false;
                            //select stock transfer list by stock code and realtive shop
                            APP_Data.StockInHeader FoundStockInHeader = entity.StockInHeaders.Where(x => x.StockCode == _stockCode && x.ToShopId == _toShopId).FirstOrDefault();
                            if (FoundStockInHeader != null)
                            {
                                SameRowExit = true;
                            }
                            if (SameRowExit)
                            {
                                APP_Data.StockInHeader NewStockInHeader = GetStockInHeaderFromXML(dataRowFromXMl, FoundStockInHeader, dsxml.Tables["StockInDetail"], SameRowExit);

                                if (NewStockInHeader != null)
                                {
                                    StockInHeader_ForeignKeyTBL(dataRowFromXMl, dsxml.Tables["StockInDetail"], NewStockInHeader);
                                }
                            }
                            else
                            {
                                var _ownShopId = SettingController.DefaultShop.Id;
                                if (_ownShopId == _toShopId)
                                {

                                    APP_Data.StockInHeader NewStockInHeader = GetStockInHeaderFromXML(dataRowFromXMl, FoundStockInHeader, dsxml.Tables["StockInDetail"], SameRowExit);


                                    StockInHeader_ForeignKeyTBL(dataRowFromXMl, dsxml.Tables["StockInDetail"], NewStockInHeader);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Stock In Detail

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["StockInDetail"] != null)
                    {
                        label1.Text = "Step 39 of 42 : Processing Stock In Detail  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["StockInDetail"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["StockInDetail"].Rows)
                        {

                            Progressbar1.PerformStep();
                            int _stockInHeaderId = Convert.ToInt32(dataRowFromXMl["StockInHeaderId"].ToString());
                            int _productId = Convert.ToInt32(dataRowFromXMl["ProductId"].ToString());
                            string _stockCode = dataRowFromXMl["StockCode"].ToString();
                            int _defaultShopId = SettingController.DefaultShop.Id;
                            int _toShopId = Convert.ToInt32(dataRowFromXMl["ToShopId"].ToString());

                            APP_Data.StockInDetail FoundStockInDetail = (from sd in entity.StockInDetails
                                                                         join sh in entity.StockInHeaders on sd.StockInHeaderId equals sh.id
                                                                         where sd.StockInHeaderId == _stockInHeaderId && sd.ProductId == _productId
                                                                         && sh.StockCode == _stockCode
                                                                         select sd).FirstOrDefault();
                            if (FoundStockInDetail != null)
                            {

                                //checking  own shop equal export shop
                                if (_defaultShopId == _toShopId)
                                {
                                    long? _alreadyStockInHeaderId = 0;

                                    //select stock in header  list by stockinheaderid(export StockInHeaderId)
                                    _alreadyStockInHeaderId = entity.StockInHeaders.Where(x => x.StockCode == _stockCode && x.Status != "StockIn").Select(x => x.id).FirstOrDefault();

                                    List<StockInDetail> _existStockInDetail = new List<StockInDetail>();

                                    if (_alreadyStockInHeaderId != 0)
                                        ///select stock in detail list by stockinheaderid and productid
                                        _existStockInDetail = entity.StockInDetails.Where(x => x.StockInHeaderId == _alreadyStockInHeaderId && x.ProductId == _productId).ToList();


                                    if (_existStockInDetail.Count == 0)
                                    {
                                        APP_Data.StockInDetail stockInDetail = new APP_Data.StockInDetail();
                                        GetStockInDetailFromXML(dataRowFromXMl, stockInDetail);
                                        entity.StockInDetails.Add(stockInDetail);
                                        entity.SaveChanges();
                                    }
                                }


                            }
                        }
                    }
                    #endregion

                    #region GiftCardInTransaction

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["GiftCardInTransaction"] != null)
                    {
                        label1.Text = "Step 40 of 42 : Processing GiftCardInTransaction  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["GiftCardInTransaction"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["GiftCardInTransaction"].Rows)
                        {

                            Progressbar1.PerformStep();


                            string TransId = dataRowFromXMl["TransactionId"].ToString();
                            dataRowFromXMl["GiftCardId"] = GetGiftCardIdByCardNo(dataRowFromXMl["CardNumber1"].ToString(), gcList);

                            int CardId = Convert.ToInt32(dataRowFromXMl["GiftCardId"].ToString());

                            APP_Data.GiftCardInTransaction FoundGiftCardInTransaction = entity.GiftCardInTransactions.Where(x => x.GiftCardId == CardId && x.TransactionId == TransId).FirstOrDefault();
                            if (FoundGiftCardInTransaction != null)
                            {

                                GetGiftCardInTransactionFromXML(dataRowFromXMl, FoundGiftCardInTransaction);
                                entity.SaveChanges();

                            }
                            else
                            {
                                APP_Data.GiftCardInTransaction giftcardInTransaction = new APP_Data.GiftCardInTransaction();
                                GetGiftCardInTransactionFromXML(dataRowFromXMl, giftcardInTransaction);
                                entity.GiftCardInTransactions.Add(giftcardInTransaction);
                                entity.SaveChanges();


                            }
                        }
                    }

                    #endregion

                    #region RedeemPoint_History

                    entity = new APP_Data.POSEntities();
                    if (dsxml.Tables["RedeemPoint_History"] != null)
                    {
                        label1.Text = "Step 41 of 42 : Processing RedeemPoint_History  table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["RedeemPoint_History"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                        foreach (DataRow dataRowFromXMl in dsxml.Tables["RedeemPoint_History"].Rows)
                        {

                            Progressbar1.PerformStep();

                            dataRowFromXMl["CustomerId"] = GetCustomerByCodeNo(dataRowFromXMl["CustomerCode"].ToString(), custList);
                            int CustomerId = int.Parse(dataRowFromXMl["CustomerId"].ToString());

                            dataRowFromXMl["CasherId"] = GetUserByCodeNo(dataRowFromXMl["UserCode"].ToString(), userList);
                            DateTime dateTrans = Convert.ToDateTime(dataRowFromXMl["DateTime"].ToString());
                            APP_Data.RedeemPoint_History FoundReddem = entity.RedeemPoint_History.Where(x => x.CustomerId == CustomerId && x.DateTime == dateTrans).FirstOrDefault();
                            if (FoundReddem != null)
                            {

                                GetRedeemHistoryFromXML(dataRowFromXMl, FoundReddem);
                                entity.SaveChanges();

                            }
                            else
                            {
                                APP_Data.RedeemPoint_History redeem = new APP_Data.RedeemPoint_History();
                                GetRedeemHistoryFromXML(dataRowFromXMl, redeem);
                                entity.RedeemPoint_History.Add(redeem);
                                entity.SaveChanges();


                            }
                        }
                    }

                    #endregion

                    #region VipCustomer 
                    entity = new APP_Data.POSEntities();

                    if (dsxml.Tables["VipCustomer"] != null)
                    {
                        label1.Text = "Step 42 of 42 : Processing Delete Log table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dsxml.Tables["VipCustomer"].Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow from xml and check if the deletelog is already exist  or a new one
                        foreach (DataRow dataFromXML in dsxml.Tables["VipCustomer"].Rows)
                        {

                            Progressbar1.PerformStep();
                            string shopCode = dataFromXML["ShopCode"].ToString();
                            string customerCode = dataFromXML["CustomerCode"].ToString();
                            DateTime lastPaidDate = Convert.ToDateTime(dataFromXML["LastPaidDate"]);
                            int totalAmount = Convert.ToInt32(dataFromXML["TwoYearsTotalAmount"]);


                            VipCustomer foundVipCustomer = entity.VipCustomers.Where(x => x.LastPaidDate == lastPaidDate &&
                                                            x.TwoYearsTotalAmount == totalAmount && x.CustomerCode == customerCode).FirstOrDefault();

                            if (foundVipCustomer == null)
                            {

                                VipCustomer vipCustomer = new VipCustomer();
                                vipCustomer.CustomerCode = dataFromXML["CustomerCode"].ToString();
                                vipCustomer.MemberType = dataFromXML["MemberType"].ToString();
                                vipCustomer.LastPaidDate = Convert.ToDateTime(dataFromXML["LastPaidDate"]);
                                vipCustomer.ShopCode = dataFromXML["ShopCode"].ToString();
                                vipCustomer.TwoYearsTotalAmount = Convert.ToInt32(dataFromXML["TwoYearsTotalAmount"]);
                                vipCustomer.CreatedDate = Convert.ToDateTime(dataFromXML["CreatedDate"]);
                                vipCustomer.CreatedUserID = Convert.ToInt32(dataFromXML["CreatedUserID"]);
                                entity.VipCustomers.Add(vipCustomer);
                                entity.SaveChanges();
                            }

                        }
                    }
                    #endregion


                    Application.DoEvents();

                    label1.Visible = false;
                    Progressbar1.Visible = false;
                    File.Delete(destFileName);
                    MessageBox.Show("Data updating completed!");

                }
                Application.UseWaitCursor = false;
                Cursor.Current = Cursors.Default;

            }
            catch (Exception ex)
            {
                label1.Visible = false;
                Progressbar1.Visible = false;
                File.Delete(destFileName);
                MessageBox.Show(ex.ToString(), "Data updating failed!");
                Application.UseWaitCursor = false;
                Cursor.Current = Cursors.Default;
            }
        }

        private void GetSPDetailFromXML(DataRow dataFromXML, APP_Data.SPDetail foundSpDetail)
        {
            if (dataFromXML["ParentProductID"].ToString().Trim() != string.Empty && dataFromXML["ParentProductID"].ToString() != "")
            {
                foundSpDetail.ParentProductID = Convert.ToInt64(dataFromXML["ParentProductID"].ToString());
            }
            if (dataFromXML["ChildProductID"].ToString().Trim() != string.Empty && dataFromXML["ChildProductID"].ToString() != "")
            {
                foundSpDetail.ChildProductID = Convert.ToInt64(dataFromXML["ChildProductID"].ToString());
            }
            if (dataFromXML.Table.Columns.Contains("TransactionDetailID"))
            {
                if (dataFromXML["TransactionDetailID"].ToString().Trim() != string.Empty && dataFromXML["TransactionDetailID"].ToString() != "")
                {
                    foundSpDetail.TransactionDetailID = Convert.ToInt64(dataFromXML["TransactionDetailID"].ToString());
                }
            }
            foundSpDetail.SPDetailID = Convert.ToString(dataFromXML["SPDetailID"].ToString());

            if (dataFromXML["DiscountRate"].ToString().Trim() != string.Empty && dataFromXML["DiscountRate"].ToString() != "")
            {
                foundSpDetail.DiscountRate = Convert.ToDecimal(dataFromXML["DiscountRate"].ToString()); //Repair for error
            }
            if (dataFromXML["Price"].ToString().Trim() != string.Empty && dataFromXML["Price"].ToString() != "")
            {
                foundSpDetail.Price = Convert.ToInt64(dataFromXML["Price"].ToString());
            }
        }

        private void GetProductPriceChangeFromXML(DataRow dataFromXML, APP_Data.ProductPriceChange Foundpdc)
        {
            if (dataFromXML["ProductId"].ToString().Trim() != string.Empty && dataFromXML["ProductId"].ToString() != "")
            {
                Foundpdc.ProductId = Convert.ToInt64(dataFromXML["ProductId"].ToString());
            }
            if (dataFromXML["UpdateDate"].ToString().Trim() != string.Empty && dataFromXML["UpdateDate"].ToString() != "")
            {
                Foundpdc.UpdateDate = Convert.ToDateTime(dataFromXML["UpdateDate"].ToString());
            }
            if (dataFromXML["UserID"].ToString().Trim() != string.Empty && dataFromXML["UserID"].ToString() != "")
            {
                Foundpdc.UserID = Convert.ToInt32(dataFromXML["UserID"].ToString());
            }
            if (dataFromXML["Price"].ToString().Trim() != string.Empty && dataFromXML["Price"].ToString() != "")
            {
                Foundpdc.Price = Convert.ToInt64(dataFromXML["Price"].ToString());
            }
            if (dataFromXML["OldPrice"].ToString().Trim() != string.Empty && dataFromXML["OldPrice"].ToString() != "")
            {
                Foundpdc.OldPrice = Convert.ToInt64(dataFromXML["OldPrice"].ToString());
            }
        }

        private void GetWrapperItemFromXML(DataRow dataFromXML, APP_Data.WrapperItem foundWrapperItem)
        {
            if (dataFromXML["ChildProductId"].ToString().Trim() != String.Empty && dataFromXML["ChildProductId"].ToString() != "")
            {
                foundWrapperItem.ChildProductId = Convert.ToInt32(dataFromXML["ChildProductId"].ToString());
            }
            if (dataFromXML["ParentProductId"].ToString().Trim() != String.Empty && dataFromXML["ParentProductId"].ToString() != "")
            {
                foundWrapperItem.ParentProductId = Convert.ToInt32(dataFromXML["ParentProductId"].ToString());
            }
        }

        private void UserPrePaidDebtFromXML(DataRow dataFromXML, APP_Data.UsePrePaidDebt FoundUserPrePaidDebt)
        {
            FoundUserPrePaidDebt.CreditTransactionId = dataFromXML["CreditTransactionId"].ToString();
            FoundUserPrePaidDebt.PrePaidDebtTransactionId = dataFromXML["PrePaidDebtTransactionId"].ToString();

            if (dataFromXML["UseAmount"].ToString() != string.Empty && dataFromXML["UseAmount"].ToString() != "")
            {
                FoundUserPrePaidDebt.UseAmount = Convert.ToInt32(dataFromXML["UseAmount"].ToString());
            }
            if (dataFromXML["CashierId"].ToString() != string.Empty && dataFromXML["CashierId"].ToString() != "")
            {
                FoundUserPrePaidDebt.CashierId = Convert.ToInt32(dataFromXML["CashierId"].ToString());
            }
            if (dataFromXML["CounterId"].ToString() != string.Empty && dataFromXML["CounterId"].ToString() != "")
            {
                FoundUserPrePaidDebt.CounterId = Convert.ToInt32(dataFromXML["CounterId"].ToString());
            }
        }


        private void GetRoleManagementFromXML(DataRow dataFromXML, APP_Data.RoleManagement FoundRoleManagement)
        {
            FoundRoleManagement.RuleFeature = dataFromXML["RuleFeature"].ToString();
            FoundRoleManagement.UserRoleId = Convert.ToInt32(dataFromXML["UserRoleId"].ToString());
            FoundRoleManagement.IsAllowed = Convert.ToBoolean(dataFromXML["IsAllowed"].ToString());
        }

        private void GetGiftCardFromXML(DataRow dataFromXML, APP_Data.GiftCard FoundGiftCard)
        {
            //ZP
            if (FoundGiftCard.IsDelete != true)
            {
                FoundGiftCard.CardNumber = dataFromXML["CardNumber"].ToString();
                if (dataFromXML["Amount"].ToString() != string.Empty && dataFromXML["Amount"].ToString() != "")
                {
                    FoundGiftCard.Amount = Convert.ToInt64(dataFromXML["Amount"].ToString());
                }
                FoundGiftCard.IsDelete = Convert.ToBoolean(dataFromXML["IsDelete"].ToString());
                if (dataFromXML["IsDelete"].ToString() != string.Empty && dataFromXML["IsDelete"].ToString() != "")
                {
                    FoundGiftCard.IsDelete = Convert.ToBoolean(dataFromXML["IsDelete"].ToString());
                }
                if (dataFromXML["IsUsed"].ToString() != string.Empty && dataFromXML["IsUsed"].ToString() != "")
                {
                    FoundGiftCard.IsUsed = Convert.ToBoolean(dataFromXML["IsUsed"].ToString());
                }
                if (dataFromXML["CustomerId"].ToString() != string.Empty && dataFromXML["CustomerId"].ToString() != "")
                {
                    FoundGiftCard.CustomerId = Convert.ToInt32(dataFromXML["CustomerId"].ToString());
                }
            }
        }

        private void GetUserFromXML(DataRow dataFromXML, APP_Data.User FoundUser)
        {
            FoundUser.Name = dataFromXML["Name"].ToString();
            if (dataFromXML["UserRoleId"].ToString().Trim() != String.Empty || dataFromXML["UserRoleId"].ToString() == "")
            {
                FoundUser.UserRoleId = Convert.ToInt32(dataFromXML["UserRoleId"].ToString());
            }
            FoundUser.Password = dataFromXML["Password"].ToString();
            if (dataFromXML["DateTime"].ToString().Trim() != String.Empty || dataFromXML["DateTime"].ToString() == "")
            {
                FoundUser.DateTime = Convert.ToDateTime(dataFromXML["DateTime"].ToString());
            }
            FoundUser.UserCodeNo = dataFromXML["UserCodeNo"].ToString();
            FoundUser.ShopId = Convert.ToInt32(dataFromXML["ShopId"].ToString());
            FoundUser.MenuPermission = dataFromXML["MenuPermission"].ToString();
        }

        private void GetUserRoleFromXML(DataRow dataFromXML, APP_Data.UserRole FoundUserRole)
        {
            FoundUserRole.RoleName = dataFromXML["RoleName"].ToString();
        }

        private void GetSettingTypeFromXML(DataRow dataFromXML, APP_Data.Setting FoundSetting)
        {
            FoundSetting.Key = dataFromXML["Key"].ToString();
            FoundSetting.Value = dataFromXML["Value"].ToString();
        }

        private void GetPaymentTypeFromXML(DataRow dataFromXML, APP_Data.PaymentType FoundPaymentType)
        {
            FoundPaymentType.Name = dataFromXML["Name"].ToString();
        }

        private void GetExchangeReateForTansction(DataRow dataFromXML, APP_Data.ExchangeRateForTransaction FoundExchangeRate)
        {
            if (dataFromXML["CurrencyId"].ToString().Trim() != string.Empty && dataFromXML["CurrencyId"].ToString() != "")
            {
                FoundExchangeRate.CurrencyId = Convert.ToInt32(dataFromXML["CurrencyId"].ToString());
            }
            if (dataFromXML["ExchangeRate"].ToString().Trim() != string.Empty && dataFromXML["ExchangeRate"].ToString() != "")
            {
                FoundExchangeRate.ExchangeRate = Convert.ToInt32(dataFromXML["ExchangeRate"].ToString());
            }
            FoundExchangeRate.TransactionId = dataFromXML["TransactionId"].ToString();
        }

        private void GetCurrencyFromXML(DataRow dataFromXML, APP_Data.Currency FoundCurrency)
        {
            FoundCurrency.Country = dataFromXML["Country"].ToString();
            FoundCurrency.Symbol = dataFromXML["Symbol"].ToString();
            FoundCurrency.CurrencyCode = dataFromXML["CurrencyCode"].ToString();
            if (dataFromXML["LatestExchangeRate"].ToString().Trim() != string.Empty && dataFromXML["LatestExchangeRate"].ToString() != "")
            {
                FoundCurrency.LatestExchangeRate = Convert.ToInt32(dataFromXML["LatestExchangeRate"].ToString());
            }
        }

        private void GetMemberTypeFromXML(DataRow dataFromXML, APP_Data.MemberType FoundMemberType)
        {
            FoundMemberType.Name = dataFromXML["Name"].ToString();


            Boolean isdelete = Convert.ToBoolean(dataFromXML["IsDelete"].ToString());
            int membertypeid = Convert.ToInt32(dataFromXML["Id"].ToString());

            if (isdelete == true)
            {
                if ((from p in entity.Customers where p.MemberTypeID == membertypeid select p).FirstOrDefault() == null
                    && (from p in entity.MemberCardRules where p.MemberTypeId == membertypeid select p).FirstOrDefault() == null
                    && (from p in entity.Promotions where p.MemberTypeId == membertypeid select p).FirstOrDefault() == null)
                {
                    FoundMemberType.IsDelete = isdelete;
                }

            }
            else
            {
                FoundMemberType.IsDelete = isdelete;
            }
        }

        private void GetDeleteLogFromXML(DataRow dataFromXML, APP_Data.DeleteLog FoundDeleteLog)
        {
            if (dataFromXML["UserId"].ToString().Trim() != string.Empty && dataFromXML["UserId"].ToString() != "")
            {
                FoundDeleteLog.UserId = Convert.ToInt32(dataFromXML["UserId"]);
            }
            if (dataFromXML["CounterId"].ToString().Trim() != string.Empty && dataFromXML["CounterId"].ToString() != "")
            {
                FoundDeleteLog.CounterId = Convert.ToInt32(dataFromXML["CounterId"]);
            }
            if (dataFromXML.Table.Columns.Contains("TransactionId"))
            {
                FoundDeleteLog.TransactionId = dataFromXML["TransactionId"].ToString();
            }
            if (dataFromXML.Table.Columns.Contains("TransactionDetailId"))
            {
                if (dataFromXML["TransactionDetailId"].ToString().Trim() != string.Empty && dataFromXML["TransactionDetailId"].ToString() != "")
                {
                    FoundDeleteLog.TransactionDetailId = Convert.ToInt64(dataFromXML["TransactionDetailId"].ToString());
                }
            }
            if (dataFromXML.Table.Columns.Contains("IsParent"))
            {
                if (dataFromXML["IsParent"].ToString().Trim() != string.Empty && dataFromXML["IsParent"].ToString() != "")
                {
                    FoundDeleteLog.IsParent = Convert.ToBoolean(dataFromXML["IsParent"].ToString());
                }
            }
            if (dataFromXML["DeletedDate"].ToString().Trim() != string.Empty && dataFromXML["DeletedDate"].ToString() != "")
            {
                FoundDeleteLog.DeletedDate = Convert.ToDateTime(dataFromXML["DeletedDate"].ToString());
            }
        }


        private void GetPointDeductRuleFromXML(DataRow dataRowFromXML, APP_Data.PointDeductionPercentage_History deductpoint)
        {
            if (dataRowFromXML.Table.Columns.Contains("DiscountRate"))
            {
                if (Convert.ToDecimal(dataRowFromXML["DiscountRate"]) != 0)
                {
                    deductpoint.DiscountRate = Convert.ToDecimal(dataRowFromXML["DiscountRate"]);
                }
            }

            deductpoint.StartDate = Convert.ToDateTime(dataRowFromXML["StartDate"]);
            if (dataRowFromXML.Table.Columns.Contains("EndDate"))
            {
                if (dataRowFromXML["EndDate"] != null && dataRowFromXML["EndDate"].ToString().Trim() != "")
                {
                    deductpoint.EndDate = Convert.ToDateTime(dataRowFromXML["EndDate"]);
                }

            }
            deductpoint.UserId = Convert.ToInt32(dataRowFromXML["UserId"]);
            deductpoint.Counter = Convert.ToInt32(dataRowFromXML["Counter"]);
            deductpoint.Active = Convert.ToBoolean(dataRowFromXML["Active"]);

        }
        private void GetTaxFromXML(DataRow dataFromXML, APP_Data.Tax FoundTax)
        {
            FoundTax.Name = dataFromXML["Name"].ToString();
            FoundTax.TaxPercent = Convert.ToDecimal(dataFromXML["TaxPercent"].ToString());


            Boolean isdelete = Convert.ToBoolean(dataFromXML["IsDelete"].ToString());
            int TaxId = Convert.ToInt32(dataFromXML["Id"].ToString());

            if (isdelete == true)
            {
                if ((from p in entity.Products where p.TaxId == TaxId select p).FirstOrDefault() == null)
                {
                    FoundTax.IsDelete = isdelete;
                }

            }
            else
            {
                FoundTax.IsDelete = isdelete;
            }
        }

        private void GetShopFromXML(DataRow dataFromXML, APP_Data.Shop FoundShop)
        {

            FoundShop.ShopName = dataFromXML["ShopName"].ToString();
            if (dataFromXML["Address1"].ToString().Trim() != string.Empty && dataFromXML["Address1"].ToString() != "")
            {
                FoundShop.Address = dataFromXML["Address"].ToString();
            }
            if (dataFromXML["PhoneNumber1"].ToString().Trim() != string.Empty && dataFromXML["PhoneNumber1"].ToString() != "")
            {
                FoundShop.PhoneNumber = dataFromXML["PhoneNumber"].ToString();
            }
            if (dataFromXML["OpeningHours1"].ToString().Trim() != string.Empty && dataFromXML["OpeningHours1"].ToString() != "")
            {
                FoundShop.OpeningHours = dataFromXML["OpeningHours"].ToString();
            }
            APP_Data.Shop shop = entity.Shops.Where(x => x.IsDefaultShop == true).FirstOrDefault();
            if (dataFromXML["CityId"].ToString().Trim() != string.Empty && dataFromXML["CityId"].ToString() != "")
            {
                FoundShop.CityId = Convert.ToInt16(dataFromXML["CityId"].ToString());
            }
            FoundShop.ShortCode = dataFromXML["ShortCode"].ToString();
            if (dataFromXML["IsDefaultShop"].ToString().Trim() != string.Empty && dataFromXML["IsDefaultShop"].ToString() != "")
            {

                //FoundShop.IsDefaultShop = Convert.ToBoolean(dataFromXML["IsDefaultShop"].ToString());
                if (FoundShop.ShopName == shop.ShopName)
                {
                    FoundShop.IsDefaultShop = true;
                }
                else
                {
                    FoundShop.IsDefaultShop = false;
                }
            }
        }

        private void GetUnitFromXML(DataRow dataRowFromXMl, APP_Data.Unit FoundUnit)
        {
            FoundUnit.UnitName = dataRowFromXMl["UnitName"].ToString();

        }

        private void GetExpenseCategoryFromXML(DataRow dataRowFromXMl, APP_Data.ExpenseCategory FoundExpenseCategory)
        {
            FoundExpenseCategory.Name = dataRowFromXMl["Name"].ToString();

            Boolean isdelete = Convert.ToBoolean(dataRowFromXMl["IsDelete"].ToString());
            int expensecatId = Convert.ToInt32(dataRowFromXMl["Id"].ToString());

            if (isdelete == true)
            {
                if ((from p in entity.Expenses where p.ExpenseCategoryId == expensecatId select p).FirstOrDefault() == null)
                {
                    FoundExpenseCategory.IsDelete = isdelete;
                }

            }
            else
            {
                FoundExpenseCategory.IsDelete = isdelete;
            }
        }

        private void GetExpenseFromXML(DataRow dataRowFromXMl, APP_Data.Expense FoundExpense)
        {
            FoundExpense.Id = dataRowFromXMl["Id"].ToString();
            FoundExpense.ExpenseDate = Convert.ToDateTime(dataRowFromXMl["ExpenseDate"].ToString());
            FoundExpense.IsApproved = Convert.ToBoolean(dataRowFromXMl["IsApproved"].ToString());
            FoundExpense.IsDeleted = Convert.ToBoolean(dataRowFromXMl["IsDeleted"].ToString());
            FoundExpense.CreatedDate = Convert.ToDateTime(dataRowFromXMl["CreatedDate"].ToString());
            FoundExpense.CreatedUser = Convert.ToInt32(dataRowFromXMl["CreatedUser"].ToString());
            if (dataRowFromXMl.Table.Columns.Contains("UpdatedDate"))
            {
                if (dataRowFromXMl["UpdatedDate"].ToString().Trim() != String.Empty && dataRowFromXMl["UpdatedDate"].ToString() != "")
                {
                    FoundExpense.UpdatedDate = Convert.ToDateTime(dataRowFromXMl["UpdatedDate"].ToString());
                }
            }
            if (dataRowFromXMl.Table.Columns.Contains("UpdatedUser"))
            {
                if (dataRowFromXMl["UpdatedUser"].ToString().Trim() != String.Empty && dataRowFromXMl["UpdatedUser"].ToString() != "")
                {
                    FoundExpense.UpdatedUser = Convert.ToInt32(dataRowFromXMl["UpdatedUser"].ToString());
                }
            }
            FoundExpense.TotalExpenseAmount = Convert.ToDecimal(dataRowFromXMl["TotalExpenseAmount"].ToString());
            FoundExpense.ExpenseCategoryId = Convert.ToInt32(dataRowFromXMl["ExpenseCategoryId"].ToString());
            //   FoundExpense.Count = Convert.ToInt32(dataRowFromXMl["Count"].ToString());
        }

        private void GetExpenseDetailFromXML(DataRow dataRowFromXMl, APP_Data.ExpenseDetail FoundExpenseDetail)
        {

            FoundExpenseDetail.ExpenseId = dataRowFromXMl["ExpenseId"].ToString();
            FoundExpenseDetail.Description = dataRowFromXMl["Description"].ToString();
            FoundExpenseDetail.Qty = Convert.ToDecimal(dataRowFromXMl["Qty"].ToString());
            FoundExpenseDetail.Price = Convert.ToDecimal(dataRowFromXMl["Price"].ToString());

        }


        private APP_Data.StockInHeader GetStockInHeaderFromXML(DataRow dataRowFromXMl, APP_Data.StockInHeader FoundStockInHeader, DataTable dtxmlStockInDetail, Boolean SameRowExit)
        {

            APP_Data.StockInHeader NewStockInHeader = new APP_Data.StockInHeader();

            //samerowexit means already had stock code in own database
            if (SameRowExit)
            {
                // Status is StockTransfer or StockReturn
                if (FoundStockInHeader.Status != "StockIn")
                {
                    return null;
                }
                //already entry stock code by theirselves and haven't approved yet 
                else if (FoundStockInHeader.Status == "StockIn" && FoundStockInHeader.IsApproved == false)
                {
                    //if not exist stock code with status "StockTransfer" or "StockReturn" in own database
                    if (Check_ExistTransferOrReturn(FoundStockInHeader))
                    {
                        //save in stockinheader table
                        NewStockInHeader = Save_NewStockInHeader(dataRowFromXMl);
                    }
                    else
                    {
                        NewStockInHeader = null;
                    }
                }
                //already entry stock code by theirselves and already approved 
                else if (FoundStockInHeader.Status == "StockIn" && FoundStockInHeader.IsApproved == true)
                {
                    //if not exist stock code with status "StockTransfer" or "StockReturn" in own database
                    if (Check_ExistTransferOrReturn(FoundStockInHeader))
                    {
                        //Isdelete=true by stockcode with status "StockTransfer" or "StockReturn"
                        NewStockInHeader = Save_NewStockInHeader(dataRowFromXMl, true);
                    }
                    else
                    {
                        NewStockInHeader = null;
                    }
                }


            }
            else
            {
                NewStockInHeader = Save_NewStockInHeader(dataRowFromXMl);
            }
            return NewStockInHeader;


        }

        private bool Check_ExistTransferOrReturn(APP_Data.StockInHeader _foundStockInHeader)
        {

            string _stockCode = _foundStockInHeader.StockCode;

            var _filterList = entity.StockInHeaders.Where(x => x.StockCode == _stockCode && x.Status != "StockIn").ToList();
            if (_filterList.Count > 0)
            {
                return false;
            }
            return true;
        }


        private APP_Data.StockInHeader Save_NewStockInHeader(DataRow dataRowFromXMl, bool isDelete = false)
        {

            APP_Data.StockInHeader NewStockInHeader = new APP_Data.StockInHeader();
            ///save new stock code
            NewStockInHeader.IsApproved = false;
            NewStockInHeader.IsDelete = isDelete;


            NewStockInHeader.StockCode = dataRowFromXMl["StockCode"].ToString();
            NewStockInHeader.Date = Convert.ToDateTime(dataRowFromXMl["Date"].ToString());
            NewStockInHeader.FromShopId = Convert.ToInt32(dataRowFromXMl["FromShopId"].ToString());
            NewStockInHeader.ToShopId = Convert.ToInt32(dataRowFromXMl["ToShopId"].ToString());

            NewStockInHeader.CreatedUser = Convert.ToInt32(dataRowFromXMl["CreatedUser"].ToString());
            NewStockInHeader.CreatedDate = Convert.ToDateTime(dataRowFromXMl["CreatedDate"].ToString());

            if (dataRowFromXMl.Table.Columns.Contains("UpdatedDate"))
            {
                if (dataRowFromXMl["UpdatedDate"].ToString().Trim() != String.Empty && dataRowFromXMl["UpdatedDate"].ToString() != "")
                {
                    NewStockInHeader.UpdatedDate = Convert.ToDateTime(dataRowFromXMl["UpdatedDate"].ToString());
                }
            }
            if (dataRowFromXMl.Table.Columns.Contains("UpdatedUser"))
            {
                if (dataRowFromXMl["UpdatedUser"].ToString().Trim() != String.Empty && dataRowFromXMl["UpdatedUser"].ToString() != "")
                {
                    NewStockInHeader.UpdatedUser = Convert.ToInt32(dataRowFromXMl["UpdatedUser"].ToString());
                }
            }
            NewStockInHeader.Status = dataRowFromXMl["Status"].ToString();


            entity.StockInHeaders.Add(NewStockInHeader);
            entity.SaveChanges();
            return NewStockInHeader;
        }

        private void GetStockInDetailFromXML(DataRow dataRowFromXMl, APP_Data.StockInDetail FoundStockInDetail)
        {

            FoundStockInDetail.StockInHeaderId = Convert.ToInt32(dataRowFromXMl["StockInHeaderId"].ToString());
            FoundStockInDetail.ProductId = Convert.ToInt32(dataRowFromXMl["ProductId"].ToString());
            FoundStockInDetail.Qty = Convert.ToInt32(dataRowFromXMl["Qty"].ToString());


        }

        private void GetMemberCardRuleFromXML(DataRow dataRowFromXMl, APP_Data.MemberCardRule FoundMemberCardRule)
        {

            FoundMemberCardRule.MemberTypeId = Convert.ToInt32(dataRowFromXMl["MemberTypeId"].ToString());
            FoundMemberCardRule.RangeFrom = dataRowFromXMl["RangeFrom"].ToString();
            FoundMemberCardRule.RangeTo = dataRowFromXMl["RangeTo"].ToString();
            FoundMemberCardRule.MCDiscount = Convert.ToInt32(dataRowFromXMl["MCDiscount"].ToString());
            FoundMemberCardRule.BDDiscount = Convert.ToInt32(dataRowFromXMl["BDDiscount"].ToString());
            FoundMemberCardRule.IsCalculatePoints = Convert.ToBoolean(dataRowFromXMl["IsCalculatePoints"]);
            FoundMemberCardRule.IsActive = Convert.ToBoolean(dataRowFromXMl["IsActive"]);
            if (dataRowFromXMl.Table.Columns.Contains("CreatedBy"))
            {
                FoundMemberCardRule.CreatedBy = Convert.ToInt32(dataRowFromXMl["CreatedBy"]);
            }
            if (dataRowFromXMl.Table.Columns.Contains("CreatedDate"))
            {
                FoundMemberCardRule.CreatedDate = Convert.ToDateTime(dataRowFromXMl["CreatedDate"]);
            }
            if (dataRowFromXMl.Table.Columns.Contains("EndBy") && dataRowFromXMl["EndBy"].ToString() != string.Empty)
            {
                FoundMemberCardRule.EndBy = Convert.ToInt32(dataRowFromXMl["EndBy"]);
            }
            if (dataRowFromXMl.Table.Columns.Contains("EndDate") && dataRowFromXMl["EndDate"].ToString() != string.Empty)
            {
                FoundMemberCardRule.EndDate = Convert.ToDateTime(dataRowFromXMl["EndDate"]);
            }
        }
        private void GetTransactionPaymentDetailFromXML(DataRow dataRowFromXML, APP_Data.TransactionPaymentDetail transactionPaymentDetail)
        {
            transactionPaymentDetail.TransactionId = Convert.ToString(dataRowFromXML["TransactionId"].ToString());

            if (dataRowFromXML["PaymentMethodId"].ToString().Trim() != string.Empty && dataRowFromXML["PaymentMethodId"].ToString() != "")
            {
                transactionPaymentDetail.PaymentMethodId = Convert.ToInt32(dataRowFromXML["PaymentMethodId"].ToString());
            }
            if (dataRowFromXML["Amount"].ToString().Trim() != string.Empty && dataRowFromXML["Amount"].ToString() != "")
            {
                transactionPaymentDetail.Amount = Convert.ToInt32(dataRowFromXML["Amount"].ToString());
            }
        }
        private void GetConsignmentSettlementFromXML(DataRow dataRowFromXMl, APP_Data.ConsignmentSettlement FoundConsignmentSettlement)
        {

            FoundConsignmentSettlement.SettlementDate = Convert.ToDateTime(dataRowFromXMl["SettlementDate"].ToString());
            FoundConsignmentSettlement.ConsignorId = Convert.ToInt32(dataRowFromXMl["ConsignorId"].ToString());
            // FoundConsignmentSettlement.TransactionDetailId = dataRowFromXMl["TransactionDetailId"].ToString();
            FoundConsignmentSettlement.TotalSettlementPrice = Convert.ToInt32(dataRowFromXMl["TotalSettlementPrice"].ToString());
            FoundConsignmentSettlement.CreatedDate = Convert.ToDateTime(dataRowFromXMl["CreatedDate"].ToString());
            FoundConsignmentSettlement.CreatedBy = Convert.ToInt32(dataRowFromXMl["CreatedBy"].ToString());
            FoundConsignmentSettlement.IsDelete = Convert.ToBoolean(dataRowFromXMl["IsDelete"].ToString());
            FoundConsignmentSettlement.ConsignmentNo = dataRowFromXMl["ConsignmentNo"].ToString();
            FoundConsignmentSettlement.FromTransactionDate = Convert.ToDateTime(dataRowFromXMl["FromTransactionDate"].ToString());
            FoundConsignmentSettlement.ToTransactionDate = Convert.ToDateTime(dataRowFromXMl["ToTransactionDate"].ToString());
            FoundConsignmentSettlement.Comment = dataRowFromXMl["Comment"].ToString();
        }
        private void GetGiftCardInTransactionFromXML(DataRow dataRowFromXMl, APP_Data.GiftCardInTransaction FoundCardInTransaction)
        {

            FoundCardInTransaction.GiftCardId = Convert.ToInt32(dataRowFromXMl["GiftCardId"].ToString());
            FoundCardInTransaction.TransactionId = dataRowFromXMl["TransactionId"].ToString();

        }
        private void GetRedeemHistoryFromXML(DataRow dataRowFromXMl, APP_Data.RedeemPoint_History FoundRedeem)
        {

            FoundRedeem.CustomerId = Convert.ToInt32(dataRowFromXMl["CustomerId"].ToString());
            FoundRedeem.RedeemAmount = Convert.ToInt32(dataRowFromXMl["RedeemAmount"].ToString());
            FoundRedeem.RedeemPoint = Convert.ToInt32(dataRowFromXMl["RedeemPoint"].ToString());
            FoundRedeem.DateTime = Convert.ToDateTime(dataRowFromXMl["DateTime"].ToString());
            FoundRedeem.CounterId = Convert.ToInt32(dataRowFromXMl["CounterId"].ToString());
            FoundRedeem.CasherId = Convert.ToInt32(dataRowFromXMl["CasherId"].ToString());

        }

        private void GetConsignmentSettlementDetailFromXML(DataRow dataRowFromXMl, APP_Data.ConsignmentSettlementDetail FoundConsignmentSettlementDetail)
        {

            FoundConsignmentSettlementDetail.ConsignmentNo = dataRowFromXMl["ConsignmentNo"].ToString();
            FoundConsignmentSettlementDetail.TransactionDetailId = Convert.ToInt32(dataRowFromXMl["TransactionDetailId"].ToString());

        }

        private void GetPromotionFromXML(DataRow dataRowFromXML, APP_Data.Promotion FoundPromotion)
        {
            FoundPromotion.BrandId = Convert.ToInt32(dataRowFromXML["BrandId"]);
            FoundPromotion.MemberTypeId = Convert.ToInt32(dataRowFromXML["MemberTypeId"]);
            FoundPromotion.Point = dataRowFromXML["Point"].ToString();
            FoundPromotion.Amount = Convert.ToInt64(dataRowFromXML["Amount"]);

        }
        private void GetGiftCardPointFromXML(DataRow dataRowFromXML, APP_Data.GiftCard_Point FoundGiftCardPoint)
        {
            FoundGiftCardPoint.TransactionId = dataRowFromXML["TransactionId"].ToString();
            FoundGiftCardPoint.TransactionDetailId = Convert.ToInt32(dataRowFromXML["TransactionDetailId"]);
            FoundGiftCardPoint.PointHistoryId = Convert.ToInt32(dataRowFromXML["PointHistoryId"]);
            FoundGiftCardPoint.TotalAmount = Convert.ToInt64(dataRowFromXML["TotalAmount"]);

        }

        private void GetReferralFromXML(DataRow dataRowFromXML, APP_Data.ReferralProgram FoundReferral)
        {
            FoundReferral.BrandId = Convert.ToInt32(dataRowFromXML["BrandId"]);
            FoundReferral.ReferralPoint = Convert.ToDecimal(dataRowFromXML["ReferralPoint"].ToString());
            FoundReferral.MiniPurchaseAmount = Convert.ToInt64(dataRowFromXML["MiniPurchaseAmount"]);
            FoundReferral.IsActive = Convert.ToBoolean(dataRowFromXML["IsActive"]);

        }

        private void GetReferralTransactionFromXML(DataRow dataRowFromXML, APP_Data.ReferralPointInTransaction FoundReferral)
        {
            FoundReferral.TransactionId = Convert.ToString(dataRowFromXML["TransactionId"]);
            FoundReferral.ReferralCustomerId = Convert.ToInt32(dataRowFromXML["ReferralCustomerId"].ToString());
            FoundReferral.ReferralPoint = Convert.ToDecimal(dataRowFromXML["ReferralPoint"]);
            FoundReferral.CreatedBy = Convert.ToInt32(dataRowFromXML["CreatedBy"]);
            FoundReferral.CreatedDate = Convert.ToDateTime(dataRowFromXML["CreatedDate"]);
            FoundReferral.PointHistoryId = Convert.ToInt32(dataRowFromXML["PointHistoryId"]);

        }
        private void GetPointHistoryFromXML(DataRow dataRowFromXML, APP_Data.Point_History FoundPointHistory)
        {
            if (dataRowFromXML.Table.Columns.Contains("BrandId1"))
            {
                if (dataRowFromXML["BrandId1"].ToString().Trim() != String.Empty && dataRowFromXML["BrandId1"].ToString() != null)
                {
                    FoundPointHistory.BrandId = Convert.ToInt32(dataRowFromXML["BrandId1"]);
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("PRMemberTypeId1"))
            {
                if (dataRowFromXML["PRMemberTypeId1"].ToString().Trim() != String.Empty && dataRowFromXML["PRMemberTypeId1"].ToString() != null)
                {
                    FoundPointHistory.PRMemberTypeId = Convert.ToInt32(dataRowFromXML["PRMemberTypeId1"]);
                }
            }
            FoundPointHistory.CreatedBy = Convert.ToInt32(dataRowFromXML["CreatedBy"]);
            FoundPointHistory.CreatedDate = Convert.ToDateTime(dataRowFromXML["CreatedDate"]);
            if (dataRowFromXML.Table.Columns.Contains("PRPointAmount"))
            {
                if (dataRowFromXML["PRPointAmount"].ToString().Trim() != String.Empty && dataRowFromXML["PRPointAmount"].ToString() != null)
                {
                    FoundPointHistory.PRPointAmount = Convert.ToDecimal(dataRowFromXML["PRPointAmount"]);
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("REFMiniPurchaseAmount1"))
            {
                if (dataRowFromXML["REFMiniPurchaseAmount1"].ToString().Trim() != String.Empty && dataRowFromXML["REFMiniPurchaseAmount1"].ToString() != null)
                {
                    FoundPointHistory.REFMiniPurchaseAmount = Convert.ToDecimal(dataRowFromXML["REFMiniPurchaseAmount1"]);
                }
            }
            FoundPointHistory.Point = Convert.ToDecimal(dataRowFromXML["Point"]);
            FoundPointHistory.Status = dataRowFromXML["Status"].ToString();

        }



        private void GetCounterFromXML(DataRow dataRowFromXML, APP_Data.Counter Foundcounter)
        {
            // Foundcounter.Id =Convert.ToInt32(dataRowFromXML["Id"].ToString());
            Foundcounter.Name = dataRowFromXML["Name"].ToString();
            Foundcounter.IsDelete = Convert.ToBoolean(dataRowFromXML["IsDelete"].ToString());

            Boolean isdelete = Convert.ToBoolean(dataRowFromXML["IsDelete"].ToString());
            int counterid = Convert.ToInt32(dataRowFromXML["Id"].ToString());

            if (isdelete == true)
            {
                if ((from p in entity.Transactions where p.CounterId == counterid select p).FirstOrDefault() == null && (from p in entity.DeleteLogs where p.CounterId == counterid select p).FirstOrDefault() == null
                    && (from p in entity.PurchaseDeleteLogs where p.CounterId == counterid select p).FirstOrDefault() == null && (from p in entity.RedeemPoint_History where p.CounterId == counterid select p).FirstOrDefault() == null
                   && (from p in entity.UsePrePaidDebts where p.CounterId == counterid select p).FirstOrDefault() == null)
                {
                    Foundcounter.IsDelete = isdelete;
                }
            }
            else
            {
                Foundcounter.IsDelete = isdelete;
            }
        }

        private void GetConsignmentCounterFromXML(DataRow dataRowFromXML, APP_Data.ConsignmentCounter FoundConsignmentCounter)
        {
            Boolean isdelete = Convert.ToBoolean(dataRowFromXML["IsDelete"].ToString());
            int id = Convert.ToInt32(dataRowFromXML["Id"].ToString());
            FoundConsignmentCounter.Name = dataRowFromXML["Name"].ToString();
            FoundConsignmentCounter.IsDelete = Convert.ToBoolean(dataRowFromXML["IsDelete"].ToString());
            if (isdelete == true)
            {
                if ((from p in entity.Products where p.ConsignmentCounterId == id select p).FirstOrDefault() == null && (from p in entity.ConsignmentSettlements where p.ConsignorId == id select p).FirstOrDefault() == null)
                {
                    FoundConsignmentCounter.IsDelete = isdelete;
                }

            }
            else
            {
                FoundConsignmentCounter.IsDelete = isdelete;
            }
            // FoundConsignmentCounter.CounterLocation = dataRowFromXML["CounterLocation"].ToString();
            if (dataRowFromXML.Table.Columns.Contains("PhoneNo"))
            {
                FoundConsignmentCounter.PhoneNo = dataRowFromXML["PhoneNo"].ToString();
            }
            if (dataRowFromXML.Table.Columns.Contains("Email"))
            {
                FoundConsignmentCounter.Email = dataRowFromXML["Email"].ToString();
            }
            if (dataRowFromXML.Table.Columns.Contains("Address"))
            {
                FoundConsignmentCounter.Address = dataRowFromXML["Address"].ToString();
            }
        }

        private void GetCityFromXML(DataRow dataRowFromXML, APP_Data.City City)
        {
            Boolean isdelete = Convert.ToBoolean(dataRowFromXML["IsDelete"].ToString());
            int city = Convert.ToInt32(dataRowFromXML["Id"].ToString());
            City.CityName = dataRowFromXML["CityName"].ToString();
            if (isdelete == true)
            {
                if ((from p in entity.Customers where p.CityId == city select p).FirstOrDefault() == null && (from p in entity.Shops where p.CityId == city select p).FirstOrDefault() == null)
                {
                    City.IsDelete = isdelete;
                }
            }
            else
            {
                City.IsDelete = isdelete;
            }

        }




        private void GetTransactionFromXML(DataRow dataRowFromXML, APP_Data.Transaction transaction)
        {
            transaction.Id = Convert.ToString(dataRowFromXML["Id"].ToString());

            if (dataRowFromXML["DateTime"].ToString().Trim() != string.Empty && dataRowFromXML["DateTime"].ToString() != "")
            {
                transaction.DateTime = Convert.ToDateTime(dataRowFromXML["DateTime"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("UpdatedDate"))
            {
                if (dataRowFromXML["UpdatedDate"].ToString().Trim() != string.Empty && dataRowFromXML["UpdatedDate"].ToString() != "")
                {
                    transaction.UpdatedDate = Convert.ToDateTime(dataRowFromXML["UpdatedDate"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("GiftCardId"))
            {
                if (dataRowFromXML["GiftCardId"].ToString().Trim() != string.Empty && dataRowFromXML["GiftCardId"].ToString() != "")
                {
                    transaction.GiftCardId = Convert.ToInt32(dataRowFromXML["GiftCardId"].ToString());
                }
            }
            if (dataRowFromXML["UserId"].ToString().Trim() != string.Empty && dataRowFromXML["UserId"].ToString() != "")
            {
                transaction.UserId = Convert.ToInt32(dataRowFromXML["UserId"].ToString());
            }
            if (dataRowFromXML["CounterId"].ToString().Trim() != string.Empty && dataRowFromXML["CounterId"].ToString() != "")
            {
                transaction.CounterId = Convert.ToInt32(dataRowFromXML["CounterId"].ToString());
            }
            transaction.Type = Convert.ToString(dataRowFromXML["Type"].ToString());

            if (dataRowFromXML["IsPaid"].ToString().Trim() != string.Empty && dataRowFromXML["IsPaid"].ToString() != "")
            {
                transaction.IsPaid = Convert.ToBoolean(dataRowFromXML["IsPaid"].ToString());
            }
            if (dataRowFromXML["IsComplete"].ToString().Trim() != string.Empty && dataRowFromXML["IsComplete"].ToString() != "")
            {
                transaction.IsComplete = Convert.ToBoolean(dataRowFromXML["IsComplete"].ToString());
            }
            if (dataRowFromXML["IsActive"].ToString().Trim() != string.Empty && dataRowFromXML["IsActive"].ToString() != "")
            {
                transaction.IsActive = Convert.ToBoolean(dataRowFromXML["IsActive"].ToString());
            }
            if (dataRowFromXML["IsDeleted"].ToString().Trim() != string.Empty && dataRowFromXML["IsDeleted"].ToString() != "")
            {
                transaction.IsDeleted = Convert.ToBoolean(dataRowFromXML["IsDeleted"].ToString());
            }

            if (dataRowFromXML["PaymentTypeId"].ToString().Trim() != string.Empty && dataRowFromXML["PaymentTypeId"].ToString() != "")
            {
                transaction.PaymentTypeId = Convert.ToInt32(dataRowFromXML["PaymentTypeId"].ToString());
            }
            if (dataRowFromXML["TaxAmount"].ToString().Trim() != string.Empty && dataRowFromXML["TaxAmount"].ToString() != "")
            {
                transaction.TaxAmount = Convert.ToInt32(dataRowFromXML["TaxAmount"].ToString());
            }


            if (dataRowFromXML["DiscountAmount"].ToString().Trim() != string.Empty && dataRowFromXML["DiscountAmount"].ToString() != "")
            {
                transaction.DiscountAmount = Convert.ToInt32(dataRowFromXML["DiscountAmount"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("ParentId1"))
            {
                if (dataRowFromXML["ParentId1"].ToString().Trim() != string.Empty && dataRowFromXML["ParentId1"].ToString() != "")
                {
                    transaction.ParentId = dataRowFromXML["ParentId1"].ToString();
                }
            }
            if (dataRowFromXML["CustomerId"].ToString().Trim() != string.Empty && dataRowFromXML["CustomerId"].ToString() != "")
            {
                transaction.CustomerId = Convert.ToInt32(dataRowFromXML["CustomerId"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("ReceivedCurrencyId"))
            {
                if (dataRowFromXML["ReceivedCurrencyId"].ToString().Trim() != string.Empty && dataRowFromXML["ReceivedCurrencyId"].ToString() != "")
                {
                    transaction.ReceivedCurrencyId = Convert.ToInt32(dataRowFromXML["ReceivedCurrencyId"].ToString());
                }
            }
            if (dataRowFromXML["ShopId"].ToString().Trim() != string.Empty && dataRowFromXML["ShopId"].ToString() != "")
            {
                transaction.ShopId = Convert.ToInt32(dataRowFromXML["ShopId"].ToString());
            }

            if (dataRowFromXML["MCDiscountAmt"].ToString().Trim() != string.Empty && dataRowFromXML["MCDiscountAmt"].ToString() != "")
            {
                transaction.MCDiscountAmt = Convert.ToDecimal(dataRowFromXML["MCDiscountAmt"].ToString());
            }

            if (dataRowFromXML["BDDiscountAmt"].ToString().Trim() != string.Empty && dataRowFromXML["BDDiscountAmt"].ToString() != "")
            {
                transaction.BDDiscountAmt = Convert.ToDecimal(dataRowFromXML["BDDiscountAmt"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("MemberTypeId"))
            {
                if (dataRowFromXML["MemberTypeId"].ToString().Trim() != string.Empty && dataRowFromXML["MemberTypeId"].ToString() != "")
                {
                    transaction.MemberTypeId = Convert.ToInt32(dataRowFromXML["MemberTypeId"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("MCDiscountPercent"))
            {
                if (dataRowFromXML["MCDiscountPercent"].ToString().Trim() != string.Empty && dataRowFromXML["MCDiscountPercent"].ToString() != "")
                {
                    transaction.MCDiscountPercent = Convert.ToDecimal(dataRowFromXML["MCDiscountPercent"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("IsSettlement"))
            {
                if (dataRowFromXML["IsSettlement"].ToString().Trim() != string.Empty && dataRowFromXML["IsSettlement"].ToString() != "")
                {
                    transaction.IsSettlement = Convert.ToBoolean(dataRowFromXML["IsSettlement"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("TranVouNos1"))
            {

                transaction.TranVouNos = dataRowFromXML["TranVouNos1"].ToString();
            }
            if (dataRowFromXML.Table.Columns.Contains("IsWholeSale"))
            {
                if (dataRowFromXML["IsWholeSale"].ToString().Trim() != string.Empty && dataRowFromXML["IsWholeSale"].ToString() != "")
                {
                    transaction.IsWholeSale = Convert.ToBoolean(dataRowFromXML["IsWholeSale"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("GiftCardAmount"))
            {
                if (dataRowFromXML["GiftCardAmount"].ToString().Trim() != string.Empty && dataRowFromXML["GiftCardAmount"].ToString() != "")
                {
                    transaction.GiftCardAmount = Convert.ToDecimal(dataRowFromXML["GiftCardAmount"].ToString());
                }
            }
            if (dataRowFromXML["TotalAmount"].ToString().Trim() != string.Empty && dataRowFromXML["TotalAmount"].ToString() != "")
            {
                transaction.TotalAmount = Convert.ToInt64(dataRowFromXML["TotalAmount"].ToString());
            }

            if (dataRowFromXML["RecieveAmount"].ToString().Trim() != string.Empty && dataRowFromXML["RecieveAmount"].ToString() != "")
            {
                transaction.RecieveAmount = Convert.ToInt64(dataRowFromXML["RecieveAmount"].ToString());
            }

            if (dataRowFromXML["ReceivedCurrencyId"].ToString().Trim() != string.Empty && dataRowFromXML["ReceivedCurrencyId"].ToString() != "")
            {
                transaction.ReceivedCurrencyId = Convert.ToInt32(dataRowFromXML["ReceivedCurrencyId"].ToString());
            }

            if (dataRowFromXML["IsCalculate"].ToString().Trim() != string.Empty && dataRowFromXML["IsCalculate"].ToString() != "")
            {
                transaction.IsCalculatePoint = Convert.ToBoolean(dataRowFromXML["IsCalculate"].ToString());
            }
            transaction.IsExported = true;

        }

        private void GetTransactionDetailFromXML(DataRow dataRowFromXML, APP_Data.TransactionDetail transactionDetail)
        {
            transactionDetail.TransactionId = Convert.ToString(dataRowFromXML["TransactionId"].ToString());

            if (dataRowFromXML["ProductId"].ToString().Trim() != string.Empty && dataRowFromXML["ProductId"].ToString() != "")
            {
                transactionDetail.ProductId = Convert.ToInt64(dataRowFromXML["ProductId"].ToString());
            }
            if (dataRowFromXML["Qty"].ToString().Trim() != string.Empty && dataRowFromXML["Qty"].ToString() != "")
            {
                transactionDetail.Qty = Convert.ToInt32(dataRowFromXML["Qty"].ToString());
            }
            if (dataRowFromXML["UnitPrice"].ToString().Trim() != string.Empty && dataRowFromXML["UnitPrice"].ToString() != "")
            {
                transactionDetail.UnitPrice = Convert.ToInt64(dataRowFromXML["UnitPrice"].ToString());
            }
            if (dataRowFromXML["DiscountRate"].ToString().Trim() != string.Empty && dataRowFromXML["DiscountRate"].ToString() != "")
            {
                transactionDetail.DiscountRate = Convert.ToDecimal(dataRowFromXML["DiscountRate"].ToString());
            }
            if (dataRowFromXML["TaxRate"].ToString().Trim() != string.Empty && dataRowFromXML["TaxRate"].ToString() != "")
            {
                transactionDetail.TaxRate = Convert.ToDecimal(dataRowFromXML["TaxRate"].ToString());
            }
            if (dataRowFromXML["TotalAmount"].ToString().Trim() != string.Empty && dataRowFromXML["TotalAmount"].ToString() != "")
            {
                transactionDetail.TotalAmount = Convert.ToInt64(dataRowFromXML["TotalAmount"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("IsDeleted"))
            {
                if (dataRowFromXML["IsDeleted"].ToString().Trim() != string.Empty && dataRowFromXML["IsDeleted"].ToString() != "")
                {
                    transactionDetail.IsDeleted = Convert.ToBoolean(dataRowFromXML["IsDeleted"].ToString());
                }
            }
            if (dataRowFromXML["ConsignmentPrice"].ToString().Trim() != string.Empty && dataRowFromXML["ConsignmentPrice"].ToString() != "")
            {
                transactionDetail.ConsignmentPrice = Convert.ToInt64(dataRowFromXML["ConsignmentPrice"].ToString());
            }
            if (dataRowFromXML["IsConsignmentPaid1"].ToString().Trim() != string.Empty && dataRowFromXML["IsConsignmentPaid1"].ToString() != "")
            {
                transactionDetail.IsConsignmentPaid = Convert.ToBoolean(dataRowFromXML["IsConsignmentPaid1"].ToString());
            }
            if (dataRowFromXML["IsFOC"].ToString().Trim() != string.Empty && dataRowFromXML["IsFOC"].ToString() != "")
            {
                transactionDetail.IsFOC = Convert.ToBoolean(dataRowFromXML["IsFOC"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("SellingPrice"))
            {
                if (dataRowFromXML["SellingPrice"].ToString().Trim() != string.Empty && dataRowFromXML["SellingPrice"].ToString() != "")
                {
                    transactionDetail.SellingPrice = Convert.ToInt64(dataRowFromXML["SellingPrice"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("PointHistoryId"))
            {
                if (dataRowFromXML["PointHistoryId"].ToString().Trim() != string.Empty && dataRowFromXML["PointHistoryId"].ToString() != "")
                {
                    transactionDetail.PointHistoryId = Convert.ToInt32(dataRowFromXML["PointHistoryId"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("IsDeductedBy"))
            {
                if (dataRowFromXML["IsDeductedBy"].ToString().Trim() != string.Empty && dataRowFromXML["IsDeductedBy"].ToString() != "")
                {
                    transactionDetail.IsDeductedBy = Convert.ToDecimal(dataRowFromXML["IsDeductedBy"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("BdDiscounted"))
            {
                if (!string.IsNullOrEmpty(dataRowFromXML["BdDiscounted"].ToString()))
                {
                    transactionDetail.BdDiscounted = dataRowFromXML["BdDiscounted"].ToString();
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("BatchNo"))
            {
                if (!string.IsNullOrEmpty(dataRowFromXML["BatchNo"].ToString()))
                {
                    transactionDetail.BatchNo = dataRowFromXML["BatchNo"].ToString();
                }
            }
        }

        private void GetCustomerFromXML(DataRow dataRowFromXML, APP_Data.Customer customer)
        {

            if (dataRowFromXML["Title"].ToString().Trim() != String.Empty && dataRowFromXML["Title"].ToString() != "")
            {
                customer.Title = Convert.ToString(dataRowFromXML["Title"].ToString());
            }
            customer.Name = Convert.ToString(dataRowFromXML["Name"].ToString());
            customer.PhoneNumber = Convert.ToString(dataRowFromXML["PhoneNumber"].ToString());
            customer.Address = Convert.ToString(dataRowFromXML["Address"].ToString());
            customer.NRC = Convert.ToString(dataRowFromXML["NRC"].ToString());
            customer.Email = Convert.ToString(dataRowFromXML["Email"].ToString());
            if (dataRowFromXML.Table.Columns.Contains("VipStartedShop"))
            {
                customer.VipStartedShop = dataRowFromXML["VipStartedShop"].ToString();
                if (dataRowFromXML["VIPMemberId"] == null || string.IsNullOrEmpty(dataRowFromXML["VIPMemberId"].ToString().Trim()))
                {
                    customer.VIPMemberId = null;
                }
                else
                {
                    customer.VIPMemberId = dataRowFromXML["VIPMemberId"].ToString();

                }
            }

            if (dataRowFromXML["CityId"].ToString().Trim() != String.Empty && dataRowFromXML["CityId"].ToString() != "")
            {
                customer.CityId = Convert.ToInt32(dataRowFromXML["CityId"].ToString());
            }

            //  customer.TownShip = Convert.ToString(dataRowFromXML["TownShip"].ToString());
            // customer.TownShip = dataRowFromXML["TownShip"].ToString();
            if (dataRowFromXML.Table.Columns.Contains("Gender"))
            {
                if (dataRowFromXML["Gender"].ToString() != "")
                {
                    customer.Gender = Convert.ToString(dataRowFromXML["Gender"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("Birthday"))
            {
                if (dataRowFromXML["Birthday"].ToString() != "")
                {
                    customer.Birthday = Convert.ToDateTime(dataRowFromXML["Birthday"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("StartDate"))
            {
                if (dataRowFromXML["StartDate"].ToString() != "")
                {
                    customer.StartDate = Convert.ToDateTime(dataRowFromXML["StartDate"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("VIPMemberId"))
            {
                //if (dataRowFromXML["VIPMemberId"].ToString() != "")
                //{
                //    customer.VIPMemberId = Convert.ToString(dataRowFromXML["VIPMemberId"].ToString());
                //}
                if (dataRowFromXML["VIPMemberId"] == null || string.IsNullOrEmpty(dataRowFromXML["VIPMemberId"].ToString()))
                {
                    customer.VIPMemberId = null;
                }
                else
                {
                    customer.VIPMemberId = dataRowFromXML["VIPMemberId"].ToString();

                }
            }


            if (dataRowFromXML.Table.Columns.Contains("CustomerCode"))
            {
                if (dataRowFromXML["CustomerCode"].ToString() != "")
                {
                    customer.CustomerCode = dataRowFromXML["CustomerCode"].ToString();
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("MemberTypeID"))
            {
                //if (dataRowFromXML["MemberTypeId"].ToString() != "")
                //{
                //    customer.MemberTypeID = Convert.ToInt32(dataRowFromXML["MemberTypeId"].ToString());
                //}
                if (dataRowFromXML["MemberTypeID"] == null || string.IsNullOrEmpty(dataRowFromXML["MemberTypeID"].ToString().Trim()))
                {
                    customer.MemberTypeID = null;
                }
                else
                {
                    customer.MemberTypeID = Convert.ToInt32(dataRowFromXML["MemberTypeID"].ToString());

                }
            }
            if (dataRowFromXML.Table.Columns.Contains("FirstTime"))
            {
                if (dataRowFromXML["FirstTime"].ToString() != "")
                {
                    customer.FirstTime = Convert.ToBoolean(dataRowFromXML["FirstTime"].ToString());
                }
            }

            if (dataRowFromXML.Table.Columns.Contains("ExpireDate"))
            {
                if (dataRowFromXML["ExpireDate"].ToString() != "")
                {
                    customer.ExpireDate = Convert.ToDateTime(dataRowFromXML["ExpireDate"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("PromoteDate"))
            {
                if (!string.IsNullOrEmpty(dataRowFromXML["PromoteDate"].ToString().Trim()))
                {
                    customer.PromoteDate = Convert.ToDateTime(dataRowFromXML["PromoteDate"].ToString());
                }
                else
                {
                    customer.PromoteDate = null;
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("LatestRevokeDate"))
            {
                if (!string.IsNullOrEmpty(dataRowFromXML["LatestRevokeDate"].ToString()))
                {
                    customer.LatestRevokeDate = Convert.ToDateTime(dataRowFromXML["LatestRevokeDate"].ToString());
                }
                else
                {
                    customer.LatestRevokeDate = null;
                }
            }
        }


        #region Relation Foreign key table
        //User Role
        private void UserRole_ForeignKeyTBL(DataRow dataFromXML, DataTable dtxmlUser, DataTable dtxmlRoleManagement, APP_Data.UserRole FoundUserRole)
        {
            string OldUserRoleId = dataFromXML["Id"].ToString();
            if (dtxmlUser != null)
            {
                foreach (DataRow o in dtxmlUser.Select("UserRoleId='" + OldUserRoleId + "'"))
                {
                    o["UserRoleId"] = FoundUserRole.Id.ToString();
                }
            }
            if (dtxmlRoleManagement != null)
            {
                foreach (DataRow o in dtxmlRoleManagement.Select("UserRoleId='" + OldUserRoleId + "'"))
                {
                    o["UserRoleId"] = FoundUserRole.Id.ToString();
                }
            }

        }

        private int GetUserByCodeNo(string UserCodeNo, IQueryable<APP_Data.User> userList)
        {
            var ul = userList.Where(x => x.UserCodeNo == UserCodeNo).FirstOrDefault();
            if (ul != null)
            {
                return ul.Id;
            }
            return 0;
        }
        private int GetCustomerByCodeNo(string CustomerCodeNo, IQueryable<APP_Data.Customer> custList)
        {
            var cl = custList.Where(x => x.CustomerCode == CustomerCodeNo).FirstOrDefault();
            if (cl != null)
            {
                return cl.Id;
            }
            return 0;
        }
        private int GetBrandIdByCodeNo(string BrandName, IQueryable<APP_Data.Brand> brandList)
        {
            var bl = brandList.Where(x => x.Name == BrandName).FirstOrDefault();
            if (bl != null)
            {
                return bl.Id;
            }
            return 0;
        }
        private int GetGiftCardIdByCardNo(string CardName, IQueryable<APP_Data.GiftCard> gcList)
        {
            var bl = gcList.Where(x => x.CardNumber == CardName).FirstOrDefault();
            if (bl != null)
            {
                return bl.Id;
            }
            return 0;
        }

        //User
        private void User_ForeignKeyTBL(DataRow dataFromXML, DataTable dtxmlReferralPointInTransaction, DataTable dtxmalPointHistory, DataTable dtxmlConsignmentSettlement, DataTable dtxmlDeleteLog, DataTable dtxmlExpense,
          DataTable dtxmlStockInHeader, DataTable dtxmlUnitConversion,
            DataTable dtxmlUserPrePaidDebt, APP_Data.User FoundUser)

        {
            string OldUserId = dataFromXML["Id"].ToString();
            //if (dtxmlTransaction != null)
            //{
            //    foreach (DataRow o in dtxmlTransaction.Select("UserId='" + OldUserId + "'"))
            //    {
            //        o["UserId"] = FoundUser.Id.ToString();
            //    }
            //}
            if (dtxmlUserPrePaidDebt != null)
            {
                foreach (DataRow o in dtxmlUserPrePaidDebt.Select("CashierId='" + OldUserId + "'"))
                {
                    o["CashierId"] = FoundUser.Id.ToString();
                }
            }
            if (dtxmlDeleteLog != null)
            {
                if (dtxmlDeleteLog != null)
                {
                    foreach (DataRow o in dtxmlDeleteLog.Select("UserId='" + OldUserId + "'"))
                    {
                        o["UserId"] = FoundUser.Id.ToString();
                    }
                }
            }
            if (dtxmlConsignmentSettlement != null)
            {
                foreach (DataRow o in dtxmlConsignmentSettlement.Select("CreatedBy='" + OldUserId + "'"))
                {
                    o["CreatedBy"] = FoundUser.Id.ToString();
                }
            }
            //if (dtxmlProductPriceChange != null)
            //{
            //    foreach (DataRow o in dtxmlProductPriceChange.Select("UserID ='" + OldUserId + "'"))
            //    {
            //        o["UserID"] = FoundUser.Id.ToString();
            //    }
            //}
            if (dtxmlExpense != null)
            {
                foreach (DataRow o in dtxmlExpense.Select("CreatedUser ='" + OldUserId + "'"))
                {
                    o["CreatedUser"] = FoundUser.Id.ToString();
                }
            }

            //if (dtxmlProductQuantityChange != null)
            //{
            //    foreach (DataRow o in dtxmlProductQuantityChange.Select("UserId ='" + OldUserId + "'"))
            //    {
            //        o["UserId"] = FoundUser.Id.ToString();
            //    }
            //}

            if (dtxmlStockInHeader != null)
            {
                foreach (DataRow o in dtxmlStockInHeader.Select("CreatedUser ='" + OldUserId + "'"))
                {
                    o["CreatedUser"] = FoundUser.Id.ToString();
                }

                foreach (DataRow o in dtxmlStockInHeader.Select("UpdatedUser ='" + OldUserId + "'"))
                {
                    o["UpdatedUser"] = FoundUser.Id.ToString();
                }
            }

            //if (dtxmlTransaction != null)
            //{
            //    foreach (DataRow o in dtxmlTransaction.Select("UserId ='" + OldUserId + "'"))
            //    {
            //        o["UserId"] = FoundUser.Id.ToString();
            //    }
            //}

            if (dtxmlUnitConversion != null)
            {
                foreach (DataRow o in dtxmlUnitConversion.Select("CreatedBy ='" + OldUserId + "'"))
                {
                    o["CreatedBy"] = FoundUser.Id.ToString();
                }

                foreach (DataRow o in dtxmlUnitConversion.Select("UpdatedBy ='" + OldUserId + "'"))
                {
                    o["UpdatedBy"] = FoundUser.Id.ToString();
                }
            }

            if (dtxmalPointHistory != null)
            {
                foreach (DataRow o in dtxmalPointHistory.Select("CreatedBy ='" + OldUserId + "'"))
                {
                    o["CreatedBy"] = FoundUser.Id.ToString();
                }
            }
            if (dtxmlReferralPointInTransaction != null)
            {
                foreach (DataRow o in dtxmlReferralPointInTransaction.Select("CreatedBy ='" + OldUserId + "'"))
                {
                    o["CreatedBy"] = FoundUser.Id.ToString();
                }
            }
            //if (dtxmlRedeemPoint_History != null)
            //{
            //    foreach (DataRow o in dtxmlRedeemPoint_History.Select("CasherId ='" + OldUserId + "'"))
            //    {
            //        o["CasherId"] = FoundUser.Id.ToString();
            //    }
            //}
        }

        //Brand
        //private void Brand_ForeignKeyTBL(DataRow dataFromXML, DataTable dtxmlProduct, DataTable dtxmlPromotion, DataTable dtxmlReferralProgram, DataTable dtxmalPointHistory, APP_Data.Brand FoundBrand)
        //{
        //    string oldBrandId = dataFromXML["Id"].ToString();
        //    string BName = dataFromXML["Name"].ToString();
        //    if (dtxmlProduct != null)
        //    {
        //        foreach (DataRow o in dtxmlProduct.Select("BrandId = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
        //        {
        //            o["BrandId"] = FoundBrand.Id.ToString();
        //        }
        //    }
        //    if (dtxmlPromotion != null)
        //    {
        //        foreach (DataRow o in dtxmlPromotion.Select("BrandId = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
        //        {
        //            o["BrandId"] = FoundBrand.Id.ToString();
        //        }
        //    }
        //    if (dtxmlReferralProgram != null)
        //    {
        //        foreach (DataRow o in dtxmlReferralProgram.Select("BrandId = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
        //        {
        //            o["BrandId"] = FoundBrand.Id.ToString();
        //        }
        //    }
        //    if (dtxmalPointHistory != null && dataFromXML.Table.Columns.Contains("BrandId1"))
        //    {
        //        foreach (DataRow o in dtxmalPointHistory.Select("BrandId1 = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
        //        {
        //            o["BrandId1"] = FoundBrand.Id.ToString();
        //        }
        //    }
        //}

        //City
        private void City_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlCustomer, DataTable dtxmlShop, APP_Data.City FoundCity)
        {
            String oldCityId = dataRowFromXML["Id"].ToString();
            String CName = dataRowFromXML["CityName"].ToString();
            //update city in City Table
            if (dtxmlCustomer != null)
            {
                foreach (DataRow o in dtxmlCustomer.Select("CityId='" + oldCityId + "' and CityName ='" + CName.Replace("'", "''") + "'"))
                {
                    o["CityId"] = FoundCity.Id.ToString();
                }
            }
            if (dtxmlShop != null)
            {
                foreach (DataRow o in dtxmlShop.Select("CityId='" + oldCityId + "' and CityName ='" + CName.Replace("'", "''") + "'"))
                {
                    o["CityId"] = FoundCity.Id.ToString();
                }
            }
        }

        //Customer
        private void Customer_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlRedeemPoint_History, DataTable dtxmlGiftCard, DataTable dtxmlReferralPointInTransaction, DataTable dtxmlTransaction, APP_Data.Customer FoundCustomer)
        {
            string oldCustomerId = dataRowFromXML["Id"].ToString();
            string CustomerCode = dataRowFromXML["CustomerCode"].ToString();

            if (dtxmlTransaction != null)
            {
                foreach (DataRow o in dtxmlTransaction.Select("CustomerCode ='" + CustomerCode + "'"))
                {
                    o["CustomerId"] = FoundCustomer.Id.ToString();
                    var c = FoundCustomer.Name;
                }
            }
            if (dtxmlReferralPointInTransaction != null)
            {
                foreach (DataRow o in dtxmlReferralPointInTransaction.Select("CustomerCode1 = '" + CustomerCode + "'"))
                {
                    o["ReferralCustomerId"] = FoundCustomer.Id.ToString();
                    var c = FoundCustomer.Name;
                }
            }
            if (dtxmlGiftCard != null)
            {
                foreach (DataRow o in dtxmlGiftCard.Select("CustomerCode1 = '" + CustomerCode + "'"))
                {
                    o["CustomerId"] = FoundCustomer.Id.ToString();
                    var c = FoundCustomer.Name;
                }
            }
            if (dtxmlRedeemPoint_History != null)
            {
                foreach (DataRow o in dtxmlRedeemPoint_History.Select("CustomerCode = '" + CustomerCode + "'"))
                {
                    o["CustomerId"] = FoundCustomer.Id.ToString();
                    var c = FoundCustomer.Name;
                }
            }
        }

        //Counter
        private void Counter_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlRedeemPoint_History, DataTable dtxmlTransaction, DataTable dtxmlUserPrePaidDebt, APP_Data.Counter Foundcounter)
        {
            String OldCounterId = dataRowFromXML["Id"].ToString();
            String counterName = dataRowFromXML["Name"].ToString();
            if (dtxmlTransaction != null)
            {
                foreach (DataRow o in dtxmlTransaction.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                {
                    o["CounterId"] = Foundcounter.Id.ToString();
                }
            }
            if (dtxmlUserPrePaidDebt != null)
            {
                foreach (DataRow o in dtxmlUserPrePaidDebt.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                {
                    o["CounterId"] = Foundcounter.Id.ToString();
                }
            }
            if (dtxmlRedeemPoint_History != null)
            {
                foreach (DataRow o in dtxmlRedeemPoint_History.Select("CounterName ='" + counterName.Replace("'", "''") + "'"))
                {
                    o["CounterId"] = Foundcounter.Id.ToString();
                }
            }
        }

        //Consignment Counter
        private void ConsignmentCounter_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlConsignmentSettlement, APP_Data.ConsignmentCounter FoundConsignmentCounter)
        {
            String OldConsignmentCounterId = dataRowFromXML["Id"].ToString();
            //update Consigment Counter in City Table



            if (dtxmlConsignmentSettlement != null)
            {
                foreach (DataRow o in dtxmlConsignmentSettlement.Select("ConsignorId='" + OldConsignmentCounterId + "'"))
                {
                    o["ConsignorId"] = FoundConsignmentCounter.Id.ToString();
                }
            }
        }

        //Unit
        private void Unit_ForeignKeyTBL(DataRow dataRowFromXMl, DataTable dtxmlProduct, APP_Data.Unit FoundUnit)
        {

            string OldUnitId = dataRowFromXMl["Id"].ToString();
            if (dtxmlProduct != null)
            {
                foreach (DataRow o in dtxmlProduct.Select("UnitId='" + OldUnitId + "'"))
                {
                    o["UnitId"] = FoundUnit.Id.ToString();
                }
            }
        }

        //Expense Category
        private void ExpenseCag_ForeignKeyTBL(DataRow dataRowFromXMl, DataTable dtxmlExpense, APP_Data.ExpenseCategory FoundExpenseCategory)
        {

            string OldExpCagId = dataRowFromXMl["Id"].ToString();
            if (dtxmlExpense != null)
            {
                foreach (DataRow o in dtxmlExpense.Select("ExpenseCategoryId='" + OldExpCagId + "'"))
                {
                    o["ExpenseCategoryId"] = FoundExpenseCategory.Id.ToString();
                }
            }
        }

        //Expense 
        private void Expense_ForeignKeyTBL(DataRow dataRowFromXMl, DataTable dtxmlExpenseDetail, APP_Data.Expense FoundExpense)
        {

            string OldExpId = dataRowFromXMl["Id"].ToString();
            if (dtxmlExpenseDetail != null)
            {
                foreach (DataRow o in dtxmlExpenseDetail.Select("ExpenseId='" + OldExpId + "'"))
                {
                    o["ExpenseId"] = FoundExpense.Id.ToString();
                }
            }
        }

        //PointHistory 
        private void PointHistory_ForeignKeyTBL(DataRow dataRowFromXMl, DataTable dtxmltransactionDetail, DataTable dtxmlGiftCard_Point, APP_Data.Point_History FoundPointHistory)
        {

            string OldPointHistoyId = dataRowFromXMl["Id"].ToString();
            if (dtxmltransactionDetail != null && dtxmltransactionDetail.Columns.Contains("PointHistoryId"))
            {
                foreach (DataRow o in dtxmltransactionDetail.Select("PointHistoryId='" + OldPointHistoyId + "'"))
                {
                    o["PointHistoryId"] = FoundPointHistory.Id.ToString();
                }
            }
            if (dtxmlGiftCard_Point != null && dtxmlGiftCard_Point.Columns.Contains("PointHistoryId"))
            {
                foreach (DataRow o in dtxmlGiftCard_Point.Select("PointHistoryId='" + OldPointHistoyId + "'"))
                {
                    o["PointHistoryId"] = FoundPointHistory.Id.ToString();
                }
            }
        }
        //Stock In Header 
        private void StockInHeader_ForeignKeyTBL(DataRow dataRowFromXMl, DataTable dtxmlStockInDetail, APP_Data.StockInHeader FoundStockInHeader)
        {

            string OldStockInHeaderId = dataRowFromXMl["Id"].ToString();
            if (dtxmlStockInDetail != null)
            {
                foreach (DataRow o in dtxmlStockInDetail.Select("StockInHeaderId='" + OldStockInHeaderId + "'"))
                {
                    o["StockInHeaderId"] = FoundStockInHeader.id.ToString();
                }
            }
        }


        private long FindProductIdByCode(string ProductCode, IQueryable<Product> products)
        {
            var p = products.Where(x => x.ProductCode == ProductCode).FirstOrDefault();
            if (p != null)
            {
                return p.Id;
            }
            return 0;
        }

        //product
        private void Product_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlSPDetail, DataTable dtxmlStockInDetail,
            DataTable dtxmlTransactionDetail, DataTable dtxmlUnitConversion, APP_Data.Product FoundProduct)
        {
            string oldProductId = dataRowFromXML["Id"].ToString();
            string ProductCode = dataRowFromXML["ProductCode"].ToString();
            ProductCode = ProductCode.Replace("'", "\"");

            if (dtxmlTransactionDetail != null)
            {
                foreach (DataRow o in dtxmlTransactionDetail.Select("ProductId ='" + oldProductId + "'"))
                {
                    o["ProductId"] = FoundProduct.Id.ToString();
                }
            }

            if (dtxmlSPDetail != null && dtxmlSPDetail.Columns.Contains("ParentProductID"))
            {
                foreach (DataRow o in dtxmlSPDetail.Select("ParentProductID ='" + oldProductId + "'"))
                {
                    o["ParentProductID"] = FoundProduct.Id.ToString();
                }
            }
            if (dtxmlSPDetail != null && dtxmlSPDetail.Columns.Contains("ChildProductID"))
            {
                foreach (DataRow o in dtxmlSPDetail.Select("ChildProductID ='" + oldProductId + "'"))
                {
                    o["ChildProductID"] = FoundProduct.Id.ToString();
                }
            }

            if (dtxmlStockInDetail != null && dtxmlStockInDetail.Columns.Contains("ProductId"))
            {
                foreach (DataRow o in dtxmlStockInDetail.Select("ProductId ='" + oldProductId + "'"))
                {
                    o["ProductId"] = FoundProduct.Id.ToString();
                }
            }

        }

        //User
        private void Shop_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlTransaction, DataTable dtxmlUser, APP_Data.Shop FoundShop)
        {
            string OldShopId = dataRowFromXML["Id"].ToString();
            string ShopName = dataRowFromXML["ShopName"].ToString();
            //update in transaction DataTables
            if (dtxmlTransaction != null)
            {
                foreach (DataRow o in dtxmlTransaction.Select("ShopId='" + OldShopId + "' and ShopName ='" + ShopName.Replace("'", "''") + "'"))
                {
                    o["ShopId"] = FoundShop.Id.ToString();
                }
            }

            if (dtxmlUser != null)
            {
                foreach (DataRow o in dtxmlUser.Select("ShopId='" + OldShopId + "'"))
                {
                    o["ShopId"] = FoundShop.Id.ToString();
                }
            }
        }

        //Currency 
        private void Currency_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlTransaction, DataTable dtxmlExchangeRateForTransaction, APP_Data.Currency FoundCurrency)
        {
            int oldCurrencyId = Convert.ToInt32(dataRowFromXML["Id"].ToString());
            if (dtxmlExchangeRateForTransaction != null)
            {
                foreach (DataRow o in dtxmlExchangeRateForTransaction.Select("CurrencyId='" + oldCurrencyId + "'"))
                {
                    o["CurrencyId"] = FoundCurrency.Id.ToString();
                }
            }
            if (dtxmlTransaction != null)
            {
                foreach (DataRow o in dtxmlTransaction.Select("ReceivedCurrencyId='" + oldCurrencyId + "'"))
                {
                    o["ReceivedCurrencyId"] = FoundCurrency.Id.ToString();
                }
            }
        }

        //MemberType 
        private void MemberType_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlPromotion, DataTable dtxmlPointHistory, DataTable dtxmlMemberCardRule, DataTable dtxmlTransaction, APP_Data.MemberType FoundMemberType)
        {
            int oldMemberTypeId = Convert.ToInt32(dataRowFromXML["Id"].ToString());
            if (dtxmlMemberCardRule != null)
            {
                foreach (DataRow o in dtxmlMemberCardRule.Select("Id='" + oldMemberTypeId + "'"))
                {
                    o["Id"] = FoundMemberType.Id.ToString();
                }
            }
            if (dtxmlTransaction != null && dtxmlTransaction.Columns.Contains("MemberTypeId"))
            {
                foreach (DataRow o in dtxmlTransaction.Select("MemberTypeId='" + oldMemberTypeId + "'"))
                {
                    o["MemberTypeId"] = FoundMemberType.Id.ToString();
                }
            }
            if (dtxmlPromotion != null)
            {
                foreach (DataRow o in dtxmlPromotion.Select("MemberTypeId='" + oldMemberTypeId + "'"))
                {
                    o["MemberTypeId"] = FoundMemberType.Id.ToString();
                }
            }
            if (dtxmlPointHistory != null && dtxmlPointHistory.Columns.Contains("PRMemberTypeId1"))
            {
                foreach (DataRow o in dtxmlPointHistory.Select("PRMemberTypeId='" + oldMemberTypeId + "'"))
                {
                    o["PRMemberTypeId"] = FoundMemberType.Id.ToString();
                }
            }
        }

        //PaymentType
        private void PaymentType_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlTransaction, APP_Data.PaymentType FoundPaymentType)
        {
            int OldPaymentTypeId = Convert.ToInt32(dataRowFromXML["Id"].ToString());
            if (dtxmlTransaction != null)
            {
                foreach (DataRow o in dtxmlTransaction.Select("PaymentTypeId='" + OldPaymentTypeId + "'"))
                {
                    o["PaymentTypeId"] = FoundPaymentType.Id.ToString();
                }
            }
        }

        //GiftCard
        private void GiftCard_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlGiftCardInTransaction, DataTable dtxmlTransaction, APP_Data.GiftCard FoundGiftCard)
        {
            int OldGiftCardId = Convert.ToInt32(dataRowFromXML["Id"].ToString());
            string GCardNumber = Convert.ToString(dataRowFromXML["CardNumber"].ToString());

            if (dtxmlTransaction != null && dtxmlTransaction.Columns.Contains("GiftCardId"))
            {
                foreach (DataRow o in dtxmlTransaction.Select("CardNumber ='" + GCardNumber.Replace("'", "''") + "'"))
                {
                    o["GiftCardId"] = FoundGiftCard.Id.ToString();
                }
            }
            if (dtxmlGiftCardInTransaction != null && dtxmlGiftCardInTransaction.Columns.Contains("GiftCardId"))
            {
                foreach (DataRow o in dtxmlGiftCardInTransaction.Select("CardNumber1 ='" + GCardNumber.Replace("'", "''") + "'"))
                {
                    o["GiftCardId"] = FoundGiftCard.Id.ToString();
                }
            }

        }

        //Transaction
        private void Transaction_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlGiftCardInTransaction, DataTable dtxmlReferralPointInTransaction, DataTable dtxmlGiftCard_Point, DataTable dtxmlTransactionDetail, DataTable dtxmlUserPrePaidDebt, DataTable dtxmlExchangeRateForTransaction, APP_Data.Transaction FoundTransaction)
        {
            string OldTransactionId = dataRowFromXML["Id"].ToString();
            if (dtxmlTransactionDetail != null && dtxmlTransactionDetail.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlTransactionDetail.Select("TransactionId ='" + OldTransactionId + "'"))
                {
                    o["TransactionId"] = FoundTransaction.Id.ToString();
                }
            }

            if (dtxmlUserPrePaidDebt != null && dtxmlUserPrePaidDebt.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlUserPrePaidDebt.Select("TransactionId ='" + OldTransactionId + "'"))
                {
                    o["TransactionId"] = FoundTransaction.Id.ToString();
                }
            }

            if (dtxmlUserPrePaidDebt != null && dtxmlUserPrePaidDebt.Columns.Contains("PrePaidDebtTransactionId"))
            {
                foreach (DataRow o in dtxmlUserPrePaidDebt.Select("PrePaidDebtTransactionId ='" + OldTransactionId + "'"))
                {
                    o["PrePaidDebtTransactionId"] = FoundTransaction.Id.ToString();
                }
            }


            if (dtxmlExchangeRateForTransaction != null && dtxmlExchangeRateForTransaction.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlExchangeRateForTransaction.Select("TransactionId ='" + OldTransactionId + "'"))
                {
                    o["TransactionId"] = FoundTransaction.Id.ToString();
                }
            }

            if (dtxmlUserPrePaidDebt != null && dtxmlUserPrePaidDebt.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlUserPrePaidDebt.Select("CreditTransactionId ='" + OldTransactionId + "'"))
                {
                    o["CreditTransactionId"] = FoundTransaction.Id.ToString();
                }
            }

            if (dtxmlGiftCard_Point != null && dtxmlGiftCard_Point.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlGiftCard_Point.Select("TransactionId ='" + OldTransactionId + "'"))
                {
                    o["TransactionId"] = FoundTransaction.Id.ToString();
                }
            }
            if (dtxmlReferralPointInTransaction != null && dtxmlReferralPointInTransaction.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlReferralPointInTransaction.Select("TransactionId ='" + OldTransactionId + "'"))
                {
                    o["TransactionId"] = FoundTransaction.Id.ToString();
                }
            }
            if (dtxmlGiftCardInTransaction != null && dtxmlGiftCardInTransaction.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlGiftCardInTransaction.Select("TransactionId ='" + OldTransactionId + "'"))
                {
                    o["TransactionId"] = FoundTransaction.Id.ToString();
                }
            }
        }

        //Transaction
        private void Tran_ForeignKeyTBL(DataRow dr, DataTable dtxmlGiftCardInTransaction, DataTable dtxmlReferralPointInTransaction, DataTable dtxmlTransactionDetail, DataTable dtxmlGiftCard_Point, DataTable dtxmlUserPrePaidDebt, DataTable dtxmlExchangeRateForTransaction,
            Transaction transaction)
        {
            string OldTransactionIdInner = dr["Id"].ToString();
            if (dtxmlTransactionDetail != null && dtxmlTransactionDetail.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlTransactionDetail.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                {
                    o["TransactionId"] = transaction.Id.ToString();
                }
            }

            if (dtxmlUserPrePaidDebt != null && dtxmlUserPrePaidDebt.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlUserPrePaidDebt.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                {
                    o["TransactionId"] = transaction.Id.ToString();
                }
            }
            if (dtxmlExchangeRateForTransaction != null && dtxmlExchangeRateForTransaction.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlExchangeRateForTransaction.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                {
                    o["TransactionId"] = transaction.Id.ToString();
                }
            }

            if (dtxmlUserPrePaidDebt != null && dtxmlUserPrePaidDebt.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlUserPrePaidDebt.Select("CreditTransactionId ='" + OldTransactionIdInner + "'"))
                {
                    o["CreditTransactionId"] = transaction.Id.ToString();
                }
            }
            if (dtxmlUserPrePaidDebt != null && dtxmlUserPrePaidDebt.Columns.Contains("PrePaidDebtTransactionId"))
            {
                foreach (DataRow o in dtxmlUserPrePaidDebt.Select("PrePaidDebtTransactionId ='" + OldTransactionIdInner + "'"))
                {
                    o["PrePaidDebtTransactionId"] = transaction.Id.ToString();
                }
            }
            if (dtxmlGiftCard_Point != null && dtxmlGiftCard_Point.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlGiftCard_Point.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                {
                    o["TransactionId"] = transaction.Id.ToString();
                }
            }
            if (dtxmlReferralPointInTransaction != null && dtxmlReferralPointInTransaction.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlReferralPointInTransaction.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                {
                    o["TransactionId"] = transaction.Id.ToString();
                }
            }
            if (dtxmlGiftCardInTransaction != null && dtxmlGiftCardInTransaction.Columns.Contains("TransactionId"))
            {
                foreach (DataRow o in dtxmlGiftCardInTransaction.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                {
                    o["TransactionId"] = transaction.Id.ToString();
                }
            }
        }

        //TransactionDetail
        private void TransactionDetail_ForeignKeyTBL(DataRow dataRowFromXML, DataTable dtxmlGiftCard_Point, DataTable dtxmlUserPrePaidDebt, DataTable dtxmlSPDetail, DataTable dtxmlConsignmentSettlementDetail, DataTable dtxmlDeleteLog, APP_Data.TransactionDetail FoundTransactionDetail)
        {
            string OldTransactionDetailId = dataRowFromXML["Id"].ToString();
            String TransactionId = dataRowFromXML["TransactionId"].ToString();
            string ConsignmentNo = dataRowFromXML["ConsignmentNo"].ToString();

            string ProductCode = dataRowFromXML["ProductCode"].ToString();

            if (dtxmlGiftCard_Point != null && dtxmlGiftCard_Point.Columns.Contains("TransactionDetailId"))
            {
                foreach (DataRow o in dtxmlGiftCard_Point.Select("TransactionDetailId ='" + OldTransactionDetailId + "'"))
                {
                    o["TransactionDetailId"] = FoundTransactionDetail.Id.ToString();
                }
            }

            if (dtxmlUserPrePaidDebt != null && dtxmlUserPrePaidDebt.Columns.Contains("TransactionDetailId"))
            {
                foreach (DataRow o in dtxmlUserPrePaidDebt.Select("TransactionDetailId ='" + OldTransactionDetailId + "'"))
                {
                    o["TransactionDetailId"] = FoundTransactionDetail.Id.ToString();
                }
            }
            if (dtxmlSPDetail != null && dtxmlSPDetail.Columns.Contains("TransactionDetailID"))
            {
                foreach (DataRow o in dtxmlSPDetail.Select("TransactionDetailID ='" + OldTransactionDetailId + "' and TID ='" + TransactionId + "' and ParentProductCode ='" + ProductCode + "'"))
                {
                    o["TransactionDetailID"] = FoundTransactionDetail.Id.ToString();
                }
            }
            if (dtxmlDeleteLog != null && dtxmlDeleteLog.Columns.Contains("TransactionDetailId"))
            {
                foreach (DataRow o in dtxmlDeleteLog.Select("TransactionDetailId='" + OldTransactionDetailId + "'"))
                {
                    o["TransactionDetailId"] = FoundTransactionDetail.Id.ToString();
                }
            }
            if (dtxmlConsignmentSettlementDetail != null && dtxmlConsignmentSettlementDetail.Columns.Contains("ConsignmentNo"))
            {

                foreach (DataRow o in dtxmlConsignmentSettlementDetail.Select("TransactionDetailID ='" + OldTransactionDetailId + "' and ConsignmentNo ='" + ConsignmentNo + "'"))
                {
                    o["TransactionDetailID"] = FoundTransactionDetail.Id.ToString();
                }


                ////////foreach(DataRow  _conSettle in dtxmlConsignmentSettlement.Rows)
                ////////{
                ////////    string _tranDetailIdList=_conSettle["TransactionDetailId"].ToString();

                ////////    //remove Comma
                ////////    string[] _removeCommaList = Utility.Remove_Comma(_tranDetailIdList);

                ////////    var _detailIdList=_removeCommaList.Where(x=>OldTransactionDetailId.Contains(x)).ToList();
                ////////    if( _detailIdList.Count > 0)
                ////////    {

                ////////        List<long> _editTranDetailIdList = Utility.Convert_String_To_Long(_removeCommaList);
                ////////        int index = _editTranDetailIdList.FindIndex(a => a == Convert.ToInt64(OldTransactionDetailId));

                ////////        _removeCommaList[index] = FoundTransactionDetail.Id.ToString();
                ////////        string _saveTranDetailIdList = string.Join(",", _removeCommaList);
                ////////        _conSettle["TransactionDetailId"] = _saveTranDetailIdList;
                ////////    }

                ////////}
            }
        }
        #endregion

        private void txtmonth_TextChanged(object sender, EventArgs e)
        {

        }


        #endregion
    }
}
