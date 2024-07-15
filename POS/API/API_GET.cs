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
using System.Data.Linq;
using System.Data.Objects;
using System.Configuration;
using POS.APP_Data;

namespace POS
{
    public static class API_GET
    {
        #region Variables         
        static string AccessToken { get; set; }
        public static List<StockInData> Master_Item { get; set; }
        public static List<UoM> Uom { get; set; }
        public static string UomResponseJson { get; set; }
        public static List<StockInDataByBatch> StockInByBatch { get; set; }

        public static string Item_MasterRequestMessage { get; set; }
        public static string Item_MasterResponseJson { get; set; }
       
        public static string StockInByBatchRequestMessage { get; set; }
        public static string StockInResponseJson { get; set; }
        public static bool NewUomSaveSuccess { get; set; } = false;
        public static bool UpdateUomSuccess { get; set; }

        public static bool NewBrandSaveSuccess { get; set; } = false;
        public static bool UpdateBrandSuccess { get; set; }
        public static bool NewCategorySaveSuccess { get; set; } = false;
        public static bool UpdateCategorySuccess { get; set; }
        public static bool NewSubCategorySaveSuccess { get; set; } = false;
        public static bool UpdateSubCategorySuccess { get; set; }
        public static bool NewProductSaveSuccess { get; set; } = false;
        public static bool UpdateProductSuccess { get; set; } = false;
        public static bool StockFillSuccess { get; set; } = false;
        public static string UoMResponseMessage { get; set; }

        public static string Item_MasterResponseMessage { get; set; }

        public static string StockByBatchResponseMessage { get; set; }

        public static HttpResponseMessage response { get; set; }
        static List<APP_Data.ProductCategory> CategoryList { get; set; }
        static List<APP_Data.ProductSubCategory> SubCategoryList { get; set; }
        static List<APP_Data.Brand> BrandList { get; set; }
        static List<APP_Data.Unit> UnitList { get; set; }
        static List<APP_Data.Product> ProductList { get; set; }
        static List<string> ProductCodes { get; set; }

        static readonly int chunkSize = 100;
        #endregion
        #region API       

        public static void GET_UOMData()
        {
            HttpClient restClient = new HttpClient();
            string Content_Type = "application/json";
            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
            restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);

            string apiServer = ConfigurationManager.AppSettings["APIServer"];
            var Builder = new UriBuilder($"{apiServer}/GET_UOM");

            try
            {
                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, null).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    UomResponseJson = result;
                    Uom = new List<UoM>();
                    Uom = JsonConvert.DeserializeObject<List<UoM>>(result);
                    UoMResponseMessage = response.ReasonPhrase;
                }

            }
            catch (Exception ex)
            {
                Uom = null;
                UoMResponseMessage = ex.ToString();//response.ReasonPhrase;                

                // MessageBox.Show(ex.Message, "Uom Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public static void GET_MasterItem(bool IsAllDataImport, List<string> ItemCodes)
        {
            HttpClient restClient = new HttpClient();
            string Content_Type = "application/json";
            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
            restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
            GET_ItemRequest requestContent;



            if (IsAllDataImport)
            {
                requestContent = new GET_ItemRequest { Date = "", Itm_code = ItemCodes };
            }
            else
            {
                string todayDate = Utility.FormatDate(DateTime.Today.Date);
                requestContent = new GET_ItemRequest { Date = todayDate, Itm_code = ItemCodes };
            }
            //GET_ItemRequest requestContent = new GET_ItemRequest { Date = "18/03/2022", Itm_code = ItemCodes };
            var requestContent_Json = JsonConvert.SerializeObject(requestContent);
            Item_MasterRequestMessage = requestContent_Json;
            string apiServer = ConfigurationManager.AppSettings["APIServer"];
            var Builder = new UriBuilder($"{apiServer}/GET_ITEM");
            HttpContent Content = new StringContent(requestContent_Json, Encoding.UTF8, Content_Type);

            try
            {
                response = new HttpResponseMessage();
                response = restClient.PostAsync(Builder.Uri, Content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    Item_MasterResponseJson = result;
                    Master_Item = new List<StockInData>();
                    Master_Item = JsonConvert.DeserializeObject<List<StockInData>>(result);
                    Item_MasterResponseMessage = response.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                Master_Item = null;
                Item_MasterResponseMessage = ex.ToString();//response.ReasonPhrase;

                //MessageBox.Show(ex.Message, "Item Master Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void GET_InStockByBatch(string WarehouseCode, List<string> ItemCodes)
        {
            HttpClient restClient = new HttpClient();

            string Content_Type = "application/json";
            restClient.DefaultRequestHeaders.Accept.Clear();
            restClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
            restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_Token.AccessToken);
            GET_ItemRequestByBatch requestContent = new GET_ItemRequestByBatch { WhsCode = WarehouseCode, Itm_code = ItemCodes };
            var requestContent_Json = JsonConvert.SerializeObject(requestContent);
            StockInByBatchRequestMessage = requestContent_Json;

            string apiServer = ConfigurationManager.AppSettings["APIServer"];
            var Builder = new UriBuilder($"{apiServer}/GET_InStockbyBtch");
            HttpContent Content = new StringContent(requestContent_Json, Encoding.UTF8, Content_Type);
            try
            {

                response = new HttpResponseMessage();
                response = (HttpResponseMessage)restClient.PostAsync(Builder.Uri, Content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    StockInResponseJson = result;
                    StockInByBatch = new List<StockInDataByBatch>();
                    StockInByBatch = JsonConvert.DeserializeObject<List<StockInDataByBatch>>(result);
                    StockByBatchResponseMessage = response.ReasonPhrase;

                }

            }
            catch (Exception ex)
            {
                StockInByBatch = null;
                StockByBatchResponseMessage = ex.ToString(); //response.ReasonPhrase;
                                                             
            }

        }
        #endregion
        #region Methods
        public static void CheckNewUoM(List<UoM> InUomdata)
        {
            POSEntities entity = new POSEntities();

            List<string> UnitCodes = entity.Units.Select(u => u.UnitCode).ToList();
            List<string> IndataUnitCodes = InUomdata.Where(u => !string.IsNullOrEmpty(u.Code)).Select(u => u.Code).Distinct().ToList();
            List<string> NewUnitCodes = IndataUnitCodes.Except(UnitCodes).ToList();
            NewUomSaveSuccess = true;
            if (NewUnitCodes.Count > 0)
            {

                try
                {

                    foreach (string Ucode in NewUnitCodes)
                    {
                        APP_Data.Unit NewUnit = new APP_Data.Unit();
                        NewUnit.UnitCode = Ucode;
                        NewUnit.UnitName = Uom.Where(u => u.Code == Ucode).Select(u => u.Name).FirstOrDefault();
                        entity.Units.Add(NewUnit);
                    }

                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    NewUomSaveSuccess = false;
                    UoMResponseMessage = ex.ToString();
                }
            }
        }
        public static void UpdateExistingUoms(List<UoM> InUomdata)
        {
            POSEntities entity = new POSEntities();
            UnitList = entity.Units.ToList();
            List<string> UnitCodes = entity.Units.Select(u => u.UnitCode).ToList();
            List<string> IndataUnitCodes = InUomdata.Where(u => !string.IsNullOrEmpty(u.Code)).Select(u => u.Code).Distinct().ToList();
            List<string> ExistingUnitCodes = UnitCodes.Intersect(IndataUnitCodes).ToList();

            UpdateUomSuccess = true;
            if (ExistingUnitCodes.Count > 0)
            {
                APP_Data.Unit ExUnit = new APP_Data.Unit();
                try
                {
                    foreach (string Ucode in ExistingUnitCodes)
                    {
                        ExUnit = new APP_Data.Unit();
                        ExUnit = UnitList.Where(u => u.UnitCode == Ucode).FirstOrDefault();
                        ExUnit.UnitName = InUomdata.Where(u => u.Code == Ucode).Select(u => u.Name).FirstOrDefault();
                        entity.Entry(ExUnit).State = EntityState.Modified;
                    }
                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    UpdateUomSuccess = false;
                    UoMResponseMessage = ex.ToString();
                }
            }

        }

        public static void CheckNewBrand(List<StockInData> Indata)
        {
            POSEntities  entity = new POSEntities();
           
            List<string> BrandCodes = entity.Brands.Where(b => b.IsDelete == false).Select(b => b.BrandCode).ToList();
            List<string> IndataBrandCodes = Indata.Where(b => !string.IsNullOrEmpty(b.Grp_Code)).Select(b => b.Grp_Code).Distinct().ToList();
            List<string> NewBrandCodes = IndataBrandCodes.Except(BrandCodes).ToList();
            NewBrandSaveSuccess = true;
            try
            {
                if (NewBrandCodes.Count > 0)
                {

                    foreach (string Brandcode in NewBrandCodes)
                    {
                        APP_Data.Brand NewBrand = new APP_Data.Brand();
                        NewBrand.BrandCode = Brandcode;
                        NewBrand.Name = Indata.Where(b => b.Grp_Code == Brandcode).Select(b => b.Grp_Name).FirstOrDefault();
                        NewBrand.IsDelete = false;
                        entity.Brands.Add(NewBrand);
                    }
                    entity.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                NewBrandSaveSuccess = false;
                Item_MasterResponseMessage = ex.ToString();
            }

        }
        public static void UpdateExistingBrand(List<StockInData> Indata)
        {
            POSEntities entity = new POSEntities();
            BrandList = entity.Brands.ToList();
            List<string> BrandCodes = entity.Brands.Select(b => b.BrandCode).ToList();
            List<string> IndataBrandCodes = Indata.Where(b => !string.IsNullOrEmpty(b.Grp_Code)).Select(b => b.Grp_Code).Distinct().ToList();
            List<string> ExistingBrandCodes = BrandCodes.Intersect(IndataBrandCodes).ToList();

            UpdateBrandSuccess = true;
            if (ExistingBrandCodes.Count > 0)
            {
                APP_Data.Brand ExBrand = new APP_Data.Brand();
                try
                {
                    foreach (string Bcode in ExistingBrandCodes)
                    {
                        ExBrand = new APP_Data.Brand();
                        ExBrand = BrandList.Where(b => b.BrandCode == Bcode).FirstOrDefault();
                        ExBrand.Name = Indata.Where(b => b.Grp_Code == Bcode).Select(b => b.Grp_Name).FirstOrDefault();
                        entity.Entry(ExBrand).State = EntityState.Modified;
                    }
                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    UpdateBrandSuccess = false;
                    Item_MasterResponseMessage = ex.ToString();
                }

            }
        }
        public static void CheckNewCategory(List<StockInData> Indata)
        {
            POSEntities entity = new POSEntities();
            
            List<string> CategoryCodes = entity.ProductCategories.Where(c => c.IsDelete == false).Select(c => c.ProductCategoryCode).ToList();
            List<string> IndataCategoryCodes = Indata.Where(c => !string.IsNullOrEmpty(c.Cat_Code)).Select(c => c.Cat_Code).Distinct().ToList();
            List<string> NewCategoryCodes = IndataCategoryCodes.Except(CategoryCodes).ToList();
            NewCategorySaveSuccess = true;
            try
            {
                if (NewCategoryCodes.Count > 0)
                {

                    foreach (string Catcode in NewCategoryCodes)
                    {
                        APP_Data.ProductCategory NewCategory = new APP_Data.ProductCategory();
                        NewCategory.ProductCategoryCode = Catcode;
                        NewCategory.Name = Indata.Where(c => c.Cat_Code == Catcode).Select(c => c.Cat_Name).FirstOrDefault();
                        NewCategory.IsDelete = false;
                        entity.ProductCategories.Add(NewCategory);
                    }
                    entity.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                NewCategorySaveSuccess = false;
                Item_MasterResponseMessage = ex.ToString();
            }

        }

        public static void UpdateExistingCategory(List<StockInData> Indata)
        {
            POSEntities entity = new POSEntities();
            CategoryList = entity.ProductCategories.ToList();
            List<string> CategoryCodes = entity.ProductCategories.Select(c => c.ProductCategoryCode).ToList();
            List<string> IndataCategoryCodes = Indata.Where(c => !string.IsNullOrEmpty(c.Cat_Code)).Select(c => c.Cat_Code).Distinct().ToList();
            List<string> ExistingCategoryCodes = CategoryCodes.Intersect(IndataCategoryCodes).ToList();

            UpdateCategorySuccess = true;
            if (ExistingCategoryCodes.Count > 0)
            {
                APP_Data.ProductCategory ExCat = new APP_Data.ProductCategory();
                try
                {
                    foreach (string Ccode in ExistingCategoryCodes)
                    {
                        ExCat = new APP_Data.ProductCategory();
                        ExCat = CategoryList.Where(c => c.ProductCategoryCode == Ccode).FirstOrDefault();
                        ExCat.Name = Indata.Where(c => c.Cat_Code == Ccode).Select(c => c.Cat_Name).FirstOrDefault();
                        entity.Entry(ExCat).State = EntityState.Modified;
                    }
                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    UpdateCategorySuccess = false;
                    Item_MasterResponseMessage = ex.ToString();
                }

            }
        }
        public static void CheckNewSubCategory(List<StockInData> Indata)
        {
            POSEntities entity = new POSEntities();
            List<string> SubCategoryCodes = entity.ProductSubCategories.Select(sc => sc.ProductSubCategoryCode).ToList();
            IQueryable<APP_Data.ProductCategory> ProductCategory = entity.ProductCategories;

            List<string> IndataSubCategoryCodes = Indata.Where(sc => !string.IsNullOrEmpty(sc.SubCat_Code)).Select(sc => sc.SubCat_Code).Distinct().ToList();
            List<string> NewSubCategoryCodes = IndataSubCategoryCodes.Except(SubCategoryCodes).ToList();
            NewSubCategorySaveSuccess = true;
            try
            {
                if (NewSubCategoryCodes.Count > 0)
                {

                    foreach (string SubCatcode in NewSubCategoryCodes)
                    {
                        APP_Data.ProductSubCategory NewSubCategory = new APP_Data.ProductSubCategory();
                        NewSubCategory.ProductSubCategoryCode = SubCatcode;
                        //NewSubCategory.Name = Indata.Where(sc => sc.SubCat_Code == SubCatcode).Select(c => c.SubCat_Name).FirstOrDefault();
                        var SubCat = Indata.Where(sc => sc.SubCat_Code == SubCatcode).FirstOrDefault();
                        NewSubCategory.Name = SubCat.SubCat_Name;
                        NewSubCategory.ProductCategoryId = ProductCategory.Where(x => x.ProductCategoryCode == SubCat.Cat_Code).FirstOrDefault().Id;
                        entity.ProductSubCategories.Add(NewSubCategory);
                    }
                    entity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                NewSubCategorySaveSuccess = false;
                Item_MasterResponseMessage = ex.ToString();

            }

        }

        public static void UpdateExistingSubCategory(List<StockInData> Indata)
        {
            POSEntities entity = new POSEntities();
            IQueryable<APP_Data.ProductCategory> ProductCategory = entity.ProductCategories;
            SubCategoryList = entity.ProductSubCategories.ToList();
            List<string> SubCategoryCodes = SubCategoryList.Select(sc => sc.ProductSubCategoryCode).ToList();
            List<string> IndataSubCategoryCodes = Indata.Where(sc => !string.IsNullOrEmpty(sc.SubCat_Code)).Select(sc => sc.SubCat_Code).Distinct().ToList();
            List<string> ExistingSubCategoryCodes = SubCategoryCodes.Intersect(IndataSubCategoryCodes).ToList();

            UpdateSubCategorySuccess = true;
            if (ExistingSubCategoryCodes.Count > 0)
            {
                APP_Data.ProductSubCategory ExSubCat = new APP_Data.ProductSubCategory();
                try
                {
                    foreach (string SCcode in ExistingSubCategoryCodes)
                    {
                        ExSubCat = new APP_Data.ProductSubCategory();
                        var SCat = Indata.Where(sc => sc.SubCat_Code == SCcode).FirstOrDefault();
                        ExSubCat = SubCategoryList.Where(sc => sc.ProductSubCategoryCode == SCcode).FirstOrDefault();
                        ExSubCat.Name = SCat.SubCat_Name;
                        ExSubCat.ProductCategoryId = ProductCategory.Where(pc => pc.ProductCategoryCode == SCat.Cat_Code).FirstOrDefault().Id;
                        entity.Entry(ExSubCat).State = EntityState.Modified;
                    }
                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    UpdateSubCategorySuccess = false;
                    Item_MasterResponseMessage = ex.ToString();
                }

            }
        }
        public static void CheckNewProduct(List<StockInData> Indata)
        {
            POSEntities entity = new POSEntities();
            ProductList = entity.Products.ToList();
            ProductCodes = ProductList.Select(p => p.ProductCode).ToList();
            List<string> IndataProductCodes = Indata.Where(x => !string.IsNullOrEmpty(x.Itm_Code)).Select(x => x.Itm_Code).ToList();
            List<string> NewProductCodes = IndataProductCodes.Except(ProductCodes).ToList();

            NewProductSaveSuccess = true;

            int taxId = entity.Taxes.Where(x => x.Name == "None").Select(x => x.Id).FirstOrDefault();
            int counter = 0;
            
            try
            {
                if (NewProductCodes.Count > 0) //Save New Product First
                {
                    entity = new POSEntities();
                    BrandList = entity.Brands.ToList();
                    CategoryList = entity.ProductCategories.ToList();
                    SubCategoryList = entity.ProductSubCategories.ToList();
                    UnitList = entity.Units.ToList();

                    Product NewProduct = new Product();
                    StockInData data = new StockInData();
                    foreach (string PCodes in NewProductCodes)
                    {
                        NewProduct = new Product();
                        data = Indata.Where(x => x.Itm_Code == PCodes).FirstOrDefault();
                        //PCode = data.Itm_Code;
                        NewProduct.ProductCode = data.Itm_Code;
                        NewProduct.Name = data.Itm_Name;
                        int bId = BrandList.Where(b => b.BrandCode == data.Grp_Code).Select(b => b.Id).FirstOrDefault();
                        NewProduct.BrandId = bId == 0 ? NewProduct.BrandId : bId;

                        int CatId = CategoryList.Where(c => c.ProductCategoryCode == data.Cat_Code && !string.IsNullOrEmpty(data.Cat_Code)).Select(c => c.Id).FirstOrDefault();
                        NewProduct.ProductCategoryId = CatId == 0 ? NewProduct.ProductCategoryId : CatId;

                        int SCatid = SubCategoryList.Where(sc => sc.ProductSubCategoryCode == data.SubCat_Code && !string.IsNullOrEmpty(data.SubCat_Code)).Select(sc => sc.Id).FirstOrDefault();
                        NewProduct.ProductSubCategoryId = SCatid == 0 ? NewProduct.ProductSubCategoryId : SCatid;

                        int uId = UnitList.Where(u => u.UnitCode == data.BaseUoM).Select(u => u.Id).FirstOrDefault();
                        NewProduct.UnitId = uId == 0 ? NewProduct.UnitId : uId;
                        NewProduct.Barcode = data.BarCode;

                        NewProduct.Price = data.Price;
                        NewProduct.IsDiscontinue = (data.Active == 'Y') ? true : false;
                        NewProduct.TaxId = taxId;
                        NewProduct.IsDeleted = (data.Active == 'Y') ? true : false;
                        NewProduct.IsConsignment = false;
                        NewProduct.CreatedDate = DateTime.ParseExact(data.CreatedDate, "dd/MM/yyyy", null);//DateTime.Parse(data.CreatedDate);
                        NewProduct.DiscountRate = (decimal)0.0;
                        entity.Products.Add(NewProduct);
                        counter++;
                        if (counter > chunkSize)
                        {
                            counter = 0;
                            entity.SaveChanges();

                        }
                    }
                    entity.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                NewProductSaveSuccess = false;
                Item_MasterResponseMessage = ex.ToString();
            }

        }
        public static void UpdateExistingProducts(List<StockInData> Indata)
        {
            POSEntities entity = new POSEntities();
            ProductList = entity.Products.ToList();
            ProductCodes = ProductList.Select(p => p.ProductCode).ToList();
            List<string> IndataProductCodes = Indata.Where(x => !string.IsNullOrEmpty(x.Itm_Code)).Select(x => x.Itm_Code).ToList();

            List<string> ExistingProductCodes = ProductCodes.Intersect(IndataProductCodes).ToList();
            UpdateProductSuccess = true;
            int counter = 0;
            int i = 0;
            try
            {
                if (ExistingProductCodes.Count > 0)
                {

                    BrandList = entity.Brands.ToList();
                    CategoryList = entity.ProductCategories.ToList();
                    SubCategoryList = entity.ProductSubCategories.ToList();
                    UnitList = entity.Units.ToList();

                    Product ExProduct = new Product();
                    StockInData data = new StockInData();

                    foreach (string PCodes in ExistingProductCodes)
                    {
                    
                        ExProduct = new Product();
                        ExProduct = entity.Products.Where(x => x.ProductCode == PCodes).FirstOrDefault();
                        data = new StockInData();

                       // ExProduct = ProductList.Where(p => p.ProductCode == PCodes).FirstOrDefault();
                        data = Indata.Where(p => p.Itm_Code == PCodes).FirstOrDefault();
                        ExProduct.Name = data.Itm_Name;

                        int bId = BrandList.Where(b => b.BrandCode == data.Grp_Code).Select(b => b.Id).FirstOrDefault();
                        ExProduct.BrandId = bId == 0 ? ExProduct.BrandId : bId;

                        int CatId = CategoryList.Where(c => c.ProductCategoryCode == data.Cat_Code && !string.IsNullOrEmpty(data.Cat_Code)).Select(c => c.Id).FirstOrDefault();
                        ExProduct.ProductCategoryId = CatId == 0 ? ExProduct.ProductCategoryId : CatId;

                        int SCatid = SubCategoryList.Where(sc => sc.ProductSubCategoryCode == data.SubCat_Code && !string.IsNullOrEmpty(data.SubCat_Code)).Select(sc => sc.Id).FirstOrDefault();
                        ExProduct.ProductSubCategoryId = SCatid == 0 ? ExProduct.ProductSubCategoryId : SCatid;

                        int uId = UnitList.Where(u => u.UnitCode == data.BaseUoM).Select(u => u.Id).FirstOrDefault();
                        ExProduct.UnitId = uId == 0 ? ExProduct.UnitId : uId;

                        ExProduct.Barcode = data.BarCode;
                        APP_Data.ProductPriceChange priceChange;
                        if (ExProduct.Price != data.Price)
                        {
                            priceChange = new APP_Data.ProductPriceChange();
                            priceChange.OldPrice = ExProduct.Price;
                            priceChange.Price = data.Price;
                            priceChange.ProductId = ProductList.Where(p => p.ProductCode == data.Itm_Code).Select(p => p.Id).FirstOrDefault();
                            priceChange.UpdateDate = DateTime.Now;
                            priceChange.UserID = MemberShip.UserId != 0 ? MemberShip.UserId : Utility.GetAutoImportUserID();
                            ExProduct.ProductPriceChanges.Add(priceChange);
                        }
                        ExProduct.Price = data.Price;
                        ExProduct.IsDiscontinue = (data.Active == 'Y') ? true : false;
                        ExProduct.IsConsignment = false;
                        ExProduct.IsDeleted = (data.Active == 'Y') ? true : false;
                        ExProduct.CreatedDate = DateTime.ParseExact(data.CreatedDate, "dd/MM/yyyy", null); //DateTime.Now;
                       
                        ExProduct.DiscountRate = (Decimal)0.0;
                        entity.Entry(ExProduct).State = EntityState.Modified;
                        counter++;
                        
                      //  Console.WriteLine(counter);
                        if (counter > chunkSize)
                        {
                            i += counter;
                            Console.WriteLine("i:" + i);
                            counter = 0;
                            entity.SaveChanges();

                        }
                        entity.SaveChanges();
                    }
                   
                }
            }
            catch (Exception ex)
            {
                UpdateProductSuccess = false;
                Item_MasterResponseMessage = ex.ToString();
            }

        }
        public static void SaveStockInDataByBatch(List<StockInDataByBatch> InStockData)
        {
            StockFillSuccess = true;
            POSEntities entity = new POSEntities();
           // string pcode = "";
            try
            {
                ProductList = entity.Products.ToList();


                List<StockFillingFromSAP> IncomingStockList = new List<StockFillingFromSAP>();

                foreach (StockInDataByBatch stock in InStockData)
                {

                    long pId = ProductList.Where(x => x.ProductCode == stock.Itm_Code).Select(p => p.Id).FirstOrDefault();

                    //pcode = stock.Itm_Code;//ProductList.Where(x => x.ProductCode == stock.Itm_Code).Select(p =>.FirstOrDefault();
                    if (pId > 0)
                    {
                        foreach (Instock inst in stock.instock)
                        {

                            StockFillingFromSAP sap = new StockFillingFromSAP();
                            sap.ProductId = pId;
                            // pcode = pId.ToString();
                            sap.BatchNo = inst.BatchNo;
                            sap.ExpireDate = DateTime.ParseExact(inst.ExpDate, "dd/MM/yyyy", null); // DateTime.Parse(inst.ExpDate);
                            sap.ProductQty = inst.Qty;
                            sap.AvailableQty = inst.Qty; // To check
                            sap.CreatedDate = DateTime.Now;                            
                            sap.CreatedBy = MemberShip.UserId != 0 ? MemberShip.UserId : Utility.GetAutoImportUserID();
                            sap.IsActive = true;
                            IncomingStockList.Add(sap);
                        }

                    }
                }
                #region updatePreviousStockIn
                List<StockFillingFromSAP> AllStockListBeforeToday = entity.StockFillingFromSAPs.Where(x => EntityFunctions.TruncateTime(x.CreatedDate) < DateTime.Today.Date && x.IsActive == true).ToList();
               // List<StockFillingFromSAP> SameStockList = AllStockListBeforeToday.Intersect(IncomingStockList, new CompareStockList()).ToList();
             
                //if (SameStockList.Count > 0)
                if(AllStockListBeforeToday.Count > 0)
                {
                    // foreach (StockFillingFromSAP sp in SameStockList)
                    int counter = 0;
                   foreach(StockFillingFromSAP sp in AllStockListBeforeToday)
                    {
                        sp.IsActive = false;
                        entity.Entry(sp).State = EntityState.Modified;
                        counter++;
                        if (counter > chunkSize)
                        {
                            counter = 0;
                            entity.SaveChanges();

                        }
                    }
                   entity.SaveChanges();
                }
                #endregion


                List<StockFillingFromSAP> todayStockList = entity.StockFillingFromSAPs.Where(x => EntityFunctions.TruncateTime(x.CreatedDate) == DateTime.Today.Date).ToList();
                if (todayStockList.Count < 0)
                {
                    int counter = 0;
                    foreach (StockFillingFromSAP sp in IncomingStockList)
                    {
                        entity.StockFillingFromSAPs.Add(sp);
                        counter++;
                        if (counter > chunkSize)
                        {
                            counter = 0;
                            entity.SaveChanges();

                        }
                    }
                     entity.SaveChanges();
                }
                else
                {
                    List<StockFillingFromSAP> newStockList = IncomingStockList.Except(todayStockList, new CompareStockList()).ToList();

                    if (newStockList.Count > 0)
                    {
                        int counter = 0;
                        foreach (StockFillingFromSAP sp in newStockList)
                        {                           
                            entity.StockFillingFromSAPs.Add(sp);
                            counter++;
                            if (counter > chunkSize)
                            {
                                counter = 0;
                                entity.SaveChanges();

                            }
                        }
                       
                        entity.SaveChanges();
                    }
                    List<StockFillingFromSAP> ToUpdateStockList = IncomingStockList.Except(newStockList, new CompareStockList()).ToList();
                    List<StockFillingFromSAP> FinalUpdateStockList = todayStockList.Intersect(ToUpdateStockList, new CompareStockList()).ToList();
                    
                    if (ToUpdateStockList.Count > 0)
                    {
                        int prevQty; int counter = 0;
                        foreach (StockFillingFromSAP sap1 in FinalUpdateStockList)
                        {
                           
                            StockFillingFromSAP sap2 = ToUpdateStockList.Where(x => x.ProductId == sap1.ProductId && x.BatchNo == sap1.BatchNo).FirstOrDefault();
                            if (sap2 != null)
                            {
                                prevQty = (int)sap1.ProductQty;
                                sap1.ProductQty = sap2.ProductQty;
                                sap1.AvailableQty = (int)(sap1.AvailableQty + sap1.ProductQty - prevQty);
                                //sap1.AvailableQty = (int) sap2.ProductQty;
                                sap1.UpdatedDate = DateTime.Now;
                                entity.Entry(sap1).State = EntityState.Modified;
                                counter++;
                                if (counter > chunkSize)
                                {
                                    counter = 0;
                                    entity.SaveChanges();
                                }
                            }

                        }
                     
                        entity.SaveChanges();
                    }

                }


            }
            catch (Exception ex)
            {
                StockFillSuccess = false;
                StockByBatchResponseMessage = ex.ToString();
                // MessageBox.Show(pcode);
            }

        }
        #endregion
        #region Models
        public class CompareStockList : IEqualityComparer<APP_Data.StockFillingFromSAP>
        {
            #region IEqualityComparer<StockFillingFromSAP> Members

            public bool Equals(StockFillingFromSAP x, StockFillingFromSAP y)
            {
                return
                    x.ProductId.Equals(y.ProductId) && x.BatchNo.Equals(y.BatchNo);
            }

            public int GetHashCode(StockFillingFromSAP obj)
            {
                int hCode = obj.ProductId.GetHashCode() ^ obj.BatchNo.GetHashCode();
                return hCode.GetHashCode();
            }

            #endregion
        }


        class GET_ItemRequest
        {
            public string Date { get; set; }
            public List<string> Itm_code { get; set; }
        }
        public class GET_ITEM
        {
            public List<StockInData> Indata { get; set; }
        }
        public class StockInData
        {
            public string Itm_Code { get; set; }
            public string Itm_Name { get; set; }
            public string Grp_Code { get; set; }
            public string Grp_Name { get; set; }
            public string LINECode { get; set; }
            public string LINEName { get; set; }

            public string Cat_Code { get; set; }
            public string Cat_Name { get; set; }
            public string SubCat_Code { get; set; }
            public string SubCat_Name { get; set; }
            public string BaseUoM { get; set; }
            public string CreatedDate { get; set; }
            public long Price { get; set; }
            public char ManBtchNum { get; set; }
            public string Cur { get; set; }
            public char Active { get; set; }
            public string BarCode { get; set; }
        }

        public class UoM
        {
            public int Entry { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }
        public class GET_ItemRequestByBatch
        {
            public string WhsCode { get; set; }
            public List<string> Itm_code { get; set; }
        }
        public class GET_StockInByBatch
        {
            public List<StockInDataByBatch> InDataByBatch { get; set; }
        }
        public class StockInDataByBatch
        {
            public string Itm_Code { get; set; }
            public string Itm_Name { get; set; }
            public string BaseUom { get; set; }
            public string WhsCode { get; set; }
            public List<Instock> instock { get; set; }
        }

        public class Instock
        {
            public int Qty { get; set; }
            public string ExpDate { get; set; }
            public string BatchNo { get; set; }
        }
        #endregion
    }
}
