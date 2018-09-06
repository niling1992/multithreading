using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGet.Models.AffairModel
{
    public class affairDetailsDTO
    {
        /// <summary>
        /// 通办范围ID
        /// </summary>
        public int generalTransactionRangeId { get; set; }
        /// <summary>
        /// 不予于受理条件
        /// </summary>
        public string unacceptCondition { get; set; }
        /// <summary>
        /// 审批权限范围
        /// </summary>
        public string approveRange { get; set; }
        /// <summary>
        /// 事项类型名称
        /// </summary>
        public string itemTypeName { get; set; }
        /// <summary>
        /// 监督方式
        /// </summary>
        public string superviseWay { get; set; }
        /// <summary>
        /// 责任事项
        /// </summary>
        public string dutyInfo { get; set; }
        /// <summary>
        /// 材料信息
        /// </summary>
        public List<materialListDTO> materialList { get; set; }
        /// <summary>
        /// 予于受理条件
        /// </summary>
        public string acceptCondition { get; set; }
        /// <summary>
        /// 予以批准（办理）的法定条件
        /// </summary>
        public string approveCondition { get; set; }
        /// <summary>
        /// 数据字典
        /// </summary>
        public List<dictionaryDtoListDTO> dictionaryDtoList { get; set; }
        /// <summary>
        /// 实施机关类型ID
        /// </summary>
        public int transactionOrgTypeId { get; set; }
        /// <summary>
        /// 事项基本码
        /// </summary>
        public string itemBasicCode { get; set; }
        /// <summary>
        /// 服务对象
        /// </summary>
        public string spObjectName { get; set; }
        /// <summary>
        /// 行业主管部门
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
        /// <summary>
        /// 部门ID
        /// </summary>
        public int orgId { get; set; }
        /// <summary>
        /// 实施码
        /// </summary>
        public string itemImpleCode { get; set; }
        /// <summary>
        /// 追责情形
        /// </summary>
        public string accountabilityInfo { get; set; }
        /// <summary>
        /// 咨询方式
        /// </summary>
        public string consultWay { get; set; }
        /// <summary>
        /// 部门Code
        /// </summary>
        public string orgCode { get; set; }
        /// <summary>
        /// 事项名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 行使层级
        /// </summary>
        public string exeTierName { get; set; }
        /// <summary>
        /// 是否支持网上支付(N/Y)
        /// </summary>
        public string isNetPay { get; set; }
        /// <summary>
        /// 办理权限
        /// </summary>
        public itemCommonProcessDTO itemCommonProcess { get; set; }
        /// <summary>
        /// 办理地点
        /// </summary>
        public string transactionSite { get; set; }
        /// <summary>
        /// 不予批准（办理）的法定情形
        /// </summary>
        public string unapproveCondition { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int transactionFormId { get; set; }
        /// <summary>
        /// 是否支持预约办理(Y/N)
        /// </summary>
        public string isNetAppointment { get; set; }
        /// <summary>
        /// 设定依据
        /// </summary>
        public string setGist { get; set; }
        /// <summary>
        /// 受理范围
        /// </summary>
        public string acceptRange { get; set; }
        /// <summary>
        /// 承诺时限
        /// </summary>
        public int promisePeriod { get; set; }
        /// <summary>
        /// 办理项名称
        /// </summary>
        public string dealItemName { get; set; }
        /// <summary>
        /// 追责依据
        /// </summary>
        public string accountabilityGist { get; set; }
        /// <summary>
        /// 结果物集合（不确定，文档中没中文说明）
        /// </summary>
        public itemTransactionResultDTO itemTransactionResult { get; set; }
        /// <summary>
        /// 常见问题
        /// </summary>
        public string faq { get; set; }
        /// <summary>
        /// 数量限制
        /// </summary>
        public string limitNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int supervisionCount { get; set; }
        /// <summary>
        /// 是否互联网公示(Y/N)
        /// </summary>
        public string isInternetPublic { get; set; }
        /// <summary>
        /// 事项类型Code
        /// </summary>
        public string itemTypeCode { get; set; }
        /// <summary>
        /// 实施部门
        /// </summary>
        public string orgName { get; set; }
        ///
        /// 行业所属分类
        /// </summary>
        public string businessTypeName { get; set; }
        /// <summary>
        /// 权力更新类型ID
        /// </summary>
        public int privUpdateTypeId { get; set; }
        /// <summary>
        /// 救济途径
        /// </summary>
        public string relieveWay { get; set; }
        /// <summary>
        /// 收费标准
        /// </summary>
        public List<itemFeesDTO> itemFees { get; set; }
        /// <summary>
        /// 办理时间
        /// </summary>
        public string transactionTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int transactionLevelId { get; set; }
        /// <summary>
        /// 是否投资审批事项(Y/N)
        /// </summary>
        public string isInvestmentProject { get; set; }
        /// <summary>
        /// 办件类型ID
        /// </summary>
        public int transactionTypeId { get; set; }
        /// <summary>
        /// 是否支持物流(Y/N)
        /// </summary>
        public string isExpress { get; set; }
        /// <summary>
        /// 特殊程序
        /// </summary>
        public string SEPCIALVERSION { get; set; }
    }

    public class dictionaryDtoListDTO
    {
        public string valueName { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string valueCode { get; set; }
        public string isLocked { get; set; }
    }

    /// <summary>
    /// 办事流程
    /// </summary>
    public class itemCommonProcessDTO
    {
        /// <summary>
        /// 1、申请
        /// </summary>
        public string apply { get; set; }
        /// <summary>
        /// 2、受理
        /// </summary>
        public string accept { get; set; }
        /// <summary>
        /// 3、审查
        /// </summary>
        public string examine { get; set; }
        /// <summary>
        /// 4、决定
        /// </summary>
        public string decisions { get; set; }
        /// <summary>
        /// 5、送达
        /// </summary>
        public string delivery { get; set; }
        /// <summary>
        /// 6、岗位职责和权限
        /// </summary>
        public string positionDuty { get; set; }
        /// <summary>
        /// 7、公开
        /// </summary>
        public string publish { get; set; }
    }

    public class itemTransactionResultDTO
    {
        public string dzzzName { get; set; }
        public string dzzzTypeName { get; set; }
        public string transactionResult { get; set; }
    }

    /// <summary>
    /// 收费标准
    /// </summary>
    public class itemFeesDTO
    {
        /// <summary>
        /// 是否收费
        /// </summary>
        public string isFee { get; set; }
        /// <summary>
        /// 收费名称
        /// </summary>
        public string feeName { get; set; }
        /// <summary>
        /// 收费标准
        /// </summary>
        public string feeNorm { get; set; }
        /// <summary>
        /// 收费依据
        /// </summary>
        public string feeGist { get; set; }
        /// <summary>
        /// 收费环节
        /// </summary>
        public string feeLink { get; set; }
        /// <summary>
        /// 缴费时间
        /// </summary>
        public string feeTime { get; set; }
        /// <summary>
        /// 缴费方式和地点
        /// </summary>
        public string feeAddress { get; set; }
        /// <summary>
        /// 减免收费的情形
        /// </summary>
        public string feeReduce { get; set; }
    }



}
