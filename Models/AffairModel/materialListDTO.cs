using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGet.Models.AffairModel
{
    public class materialListDTO
    {
        /// <summary>
        /// 材料名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 材料编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 材料份数
        /// </summary>
        public int num { get; set; }
        /// <summary>
        /// 材料收件编码
        /// </summary>
        public string dicRequireCode { get; set; }
        /// <summary>
        /// 材料收件类型
        /// </summary>
        public string dicRequireName { get; set; }
    }
}
