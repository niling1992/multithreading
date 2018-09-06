using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGet.Models.AffairModel
{
    public class affairsDTO
    {
        /// <summary>
        /// 实施码
        /// </summary>
        public string itemImpleCode { get; set; }
        /// <summary>
        /// 事项基础类型
        /// </summary>
        public string itemTypeName { get; set; }
        /// <summary>
        /// 办理项名称
        /// </summary>
        public string dealItemName { get; set; }
        /// <summary>
        /// 行业主管
        /// </summary>
        public string depCateName { get; set; }
        /// <summary>
        /// 事项名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 子项集合
        /// </summary>
        public List<itemDataDto> itemDataDtoList { get; set; }
        /// <summary>
        /// 事项层级（P主项 C子项）
        /// </summary>
        public itemNodeTypeListDTO itemNodeType { get; set; }
        /// <summary>
        /// 事项基本码
        /// </summary>
        public string itemBasicCode { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string orgName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }

        //下面几个参数不知道json中有没有，增加获取再说

        /// <summary>
        /// 法定时限
        /// </summary>
        public int legalPeriod { get; set; }
        /// <summary>
        /// 承诺时限
        /// </summary>
        public int promisePeriod { get; set; }
        /// <summary>
        /// 办件类型code
        /// </summary>
        public string transactionTypeCode { get; set; }
        /// <summary>
        /// 办件类型名称
        /// </summary>
        public string transactionTypeName { get; set; }
        /// <summary>
        /// 办理形式
        /// </summary>
        public string transactionFormName { get; set; }
    }

    public class itemNodeTypeListDTO
    {
        public string code { get; set; }
        public string title { get; set; }
    }

    public class itemDataDto
    {
        /// <summary>
        /// 实施码
        /// </summary>
        public string itemImpleCode { get; set; }
        /// <summary>
        /// 办理形式
        /// </summary>
        public string transactionFormName { get; set; }
        /// <summary>
        /// 事项基础类型
        /// </summary>
        public string itemTypeName { get; set; }
        /// <summary>
        /// 办理项名称
        /// </summary>
        public string dealItemName { get; set; }
        /// <summary>
        /// 承诺时限
        /// </summary>
        public int promisePeriod { get; set; }
        /// <summary>
        /// 事项名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 办件类型名称
        /// </summary>
        public string transactionTypeName { get; set; }
        /// <summary>
        /// 事项层级（P主项 C子项）
        /// </summary>
        public itemNodeTypeListDTO itemNodeType { get; set; }
        /// <summary>
        /// 事项基本码
        /// </summary>
        public string itemBasicCode { get; set; }
        /// <summary>
        /// 办件类型code
        /// </summary>
        public string transactionTypeCode { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string orgName { get; set; }
        /// <summary>
        /// 行业主管
        /// </summary>
        public string depCateName { get; set; }
        /// <summary>
        /// 法定时限
        /// </summary>
        public int legalPeriod { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }
    }
}
