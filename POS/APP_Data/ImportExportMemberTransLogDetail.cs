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
    
    public partial class ImportExportMemberTransLogDetail
    {
        public long Id { get; set; }
        public Nullable<long> ProcessingBatchID { get; set; }
        public string ProcessName { get; set; }
        public string DetailStatus { get; set; }
        public string ResponseMessageFromPiti { get; set; }
        public string PostJson { get; set; }
        public string ResponseJson { get; set; }
    
        public virtual ImportExportMemberTransLog ImportExportMemberTransLog { get; set; }
    }
}
