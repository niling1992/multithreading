using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGet.Models.depModel
{
    public class deparmentDTO
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        public int orgId { get; set; }
        /// <summary>
        /// 部门ID父级
        /// </summary>
        public int parentOrgId { get; set; }
        /// <summary>
        /// 区域id
        /// </summary>
        public int areaId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string orgName { get; set; }
        /// <summary>
        /// 部门全称
        /// </summary>
        public string fullOrgName { get; set; }
        /// <summary>
        /// 部门类型
        /// </summary>
        public string orgType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string orgCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string seq { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isBusDep { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string depCateCode { get; set; }
        /// <summary>
        /// 法人姓名
        /// </summary>
        public string legalPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contacts { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string orgFunctions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<deparmentDTO> children { get; set; }

    }
}

