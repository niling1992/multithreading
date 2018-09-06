using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGet.Models.AreaModel
{
    public class AreaJsonDTO
    {
        public int areaId { get; set; }
        public int parentId { get; set; }
        public string areaName { get; set; }
        public string areaCode { get; set; }
    }
}
