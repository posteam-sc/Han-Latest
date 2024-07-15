//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace POS.APP_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public Product()
        {
            this.ProductPriceChanges = new HashSet<ProductPriceChange>();
            this.ProductQuantityChanges = new HashSet<ProductQuantityChange>();
            this.PurchaseDetails = new HashSet<PurchaseDetail>();
            this.PurchaseDetailInTransactions = new HashSet<PurchaseDetailInTransaction>();
            this.SPDetails = new HashSet<SPDetail>();
            this.SPDetails1 = new HashSet<SPDetail>();
            this.StockFillingFromSAPs = new HashSet<StockFillingFromSAP>();
            this.StockInDetails = new HashSet<StockInDetail>();
            this.StockTransactions = new HashSet<StockTransaction>();
            this.TransactionDetails = new HashSet<TransactionDetail>();
            this.WrapperItems = new HashSet<WrapperItem>();
            this.WrapperItems1 = new HashSet<WrapperItem>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public long Price { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string ProductLocation { get; set; }
        public Nullable<int> ProductCategoryId { get; set; }
        public Nullable<int> ProductSubCategoryId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> TaxId { get; set; }
        public Nullable<int> MinStockQty { get; set; }
        public decimal DiscountRate { get; set; }
        public Nullable<bool> IsWrapper { get; set; }
        public Nullable<bool> IsConsignment { get; set; }
        public Nullable<bool> IsDiscontinue { get; set; }
        public Nullable<long> ConsignmentPrice { get; set; }
        public Nullable<int> ConsignmentCounterId { get; set; }
        public string Size { get; set; }
        public Nullable<long> PurchasePrice { get; set; }
        public Nullable<bool> IsNotifyMinStock { get; set; }
        public Nullable<long> Amount { get; set; }
        public Nullable<int> Percent { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string PhotoPath { get; set; }
        public Nullable<long> WholeSalePrice { get; set; }
        public string UnitType { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual ConsignmentCounter ConsignmentCounter { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ProductSubCategory ProductSubCategory { get; set; }
        public virtual Tax Tax { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual ICollection<ProductPriceChange> ProductPriceChanges { get; set; }
        public virtual ICollection<ProductQuantityChange> ProductQuantityChanges { get; set; }
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual ICollection<PurchaseDetailInTransaction> PurchaseDetailInTransactions { get; set; }
        public virtual ICollection<SPDetail> SPDetails { get; set; }
        public virtual ICollection<SPDetail> SPDetails1 { get; set; }
        public virtual ICollection<StockFillingFromSAP> StockFillingFromSAPs { get; set; }
        public virtual ICollection<StockInDetail> StockInDetails { get; set; }
        public virtual ICollection<StockTransaction> StockTransactions { get; set; }
        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }
        public virtual ICollection<WrapperItem> WrapperItems { get; set; }
        public virtual ICollection<WrapperItem> WrapperItems1 { get; set; }
    }
}
