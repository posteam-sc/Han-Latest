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
    
    public partial class Promotion
    {
        public int Id { get; set; }
        public int MemberTypeId { get; set; }
        public Nullable<long> Amount { get; set; }
        public string Point { get; set; }
        public Nullable<int> BrandId { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual MemberType MemberType { get; set; }
    }
}
