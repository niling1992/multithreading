using System.Collections.Generic;
using DataGet.Extensions;

namespace DataGet.ProvinceInvestmentPlatform.Models
{
    public class AsyncMaterialInfoModel
    {
        public List<dynamic> MaterialInfoList { get; set; }
        public MutipleThreadResetEvent ThreadResetEvent { get; set; }
        public int AreaCodeThreadCount { get; set; }
        public int ThisThreadIndex { get; set; }
    }
}