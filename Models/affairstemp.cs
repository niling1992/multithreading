//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataGet.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class affairstemp
    {
        public int id { get; set; }
        public string version { get; set; }
        public string name { get; set; }
        public string dealItemName { get; set; }
        public string itemBasicCode { get; set; }
        public string itemImpleCode { get; set; }
        public string DEPTCODE { get; set; }
        public string orgName { get; set; }
        public string AREACODE { get; set; }
        public Nullable<int> legalPeriod { get; set; }
        public Nullable<int> promisePeriod { get; set; }
        public string transactionTypeCode { get; set; }
        public string transactionTypeName { get; set; }
        public string itemTypeName { get; set; }
        public string transactionFormName { get; set; }
        public string depCateName { get; set; }
        public int VALID { get; set; }
    }
}