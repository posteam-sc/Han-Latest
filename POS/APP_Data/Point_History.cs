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
    
    public partial class Point_History
    {
        public Point_History()
        {
            this.ReferralPointInTransactions = new HashSet<ReferralPointInTransaction>();
            this.TransactionDetails = new HashSet<TransactionDetail>();
        }
    
        public int Id { get; set; }
        public Nullable<int> BrandId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> PRMemberTypeId { get; set; }
        public Nullable<decimal> REFMiniPurchaseAmount { get; set; }
        public Nullable<decimal> PRPointAmount { get; set; }
        public decimal Point { get; set; }
        public string Status { get; set; }
    
        public virtual Point_History Point_History1 { get; set; }
        public virtual Point_History Point_History2 { get; set; }
        public virtual ICollection<ReferralPointInTransaction> ReferralPointInTransactions { get; set; }
        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }
    }
}
