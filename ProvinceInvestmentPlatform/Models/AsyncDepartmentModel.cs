using System.Collections.Generic;
using DataGet.Extensions;
using MySql.Data.MySqlClient;

namespace DataGet.ProvinceInvestmentPlatform.Models
{
    public class AsyncDepartmentModel
    {
        public List<dynamic> AreaCodeList { get; set; }
        public MutipleThreadResetEvent ThreadResetEvent { get; set; }
        public int AreaCodeThreadCount { get; set; }
        public int ThisThreadIndex { get; set; }
    }
}