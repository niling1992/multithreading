using System.Collections.Generic;
using DataGet.Extensions;

namespace DataGet.ProvinceInvestmentPlatform.Models
{
    public class AsyncProceedingInfoModel
    {
        public List<dynamic> DeptIdList { get; set; }
        public MutipleThreadResetEvent ThreadResetEvent { get; set; }
        public int ProceedingInfoThreadCount { get; set; }
        public int ThisThreadIndex { get; set; }
    }
}