using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DataGet.ProvinceInvestmentPlatform.Services
{
    public interface IGetProvinceInvestmentPlatform
    {
        /// <summary>
        /// 获取dic_attribution表的【行政区划编码（AreaCode）】
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetMyDataBaseByDicAttribution();
        /// <summary>
        /// 获取dic_competentauth表数据（做同步是查询是否存在用）
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetMyDataBaseByDicCompetentauth();
        /// <summary>
        /// 获取部门表里部门Id不是空的值
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetMyDataBaseByDicCompetentauthDetId();
        /// <summary>
        /// 获取所有的事项(暂时获取拼接字符串+省平台Id（InvestmentItemCode）)
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetMyDataBaseAffairs();

        /// <summary>
        /// 获取省级事项Id不等于null的数据
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetMyDataBaseAffairsWhereInvestmentItemCodeIsNotNull();
        /// <summary>
        /// 插入事项
        /// </summary>
        /// <param name="affairsModel">事项实体</param>
        /// <param name="affairguide">办事指南</param>
        /// <param name="isDelete">是否删除以后再插入</param>
        /// <returns></returns>
        object InsertAffairs(dynamic affairsModel, dynamic affairguide, bool isDelete);
        /// <summary>
        /// 更新或插入部门表
        /// id=0新增，否则编辑
        /// </summary>
        void UpDateMyDataBaseByDicCompetentauth(int id, string fullName, string avtion, string deptcode, string areacode, string provinceInvestmentPlatformDeptId,string invDepCode);
        /// <summary>
        /// 1.1.1.	根据行政区划编码获取部门列表
        /// </summary>
        /// <returns></returns>
        string GetDeptInfoByCode(string code);
        /// <summary>
        /// 1.1.2.	根据部门编号获取已发布投资事项列表
        /// </summary>
        /// <returns></returns>
        string GetProceedingInfoByDept(string id);

        /// <summary>
        /// 1.1.3.	根据事项编号获取申报材料列表
        /// </summary>
        /// <returns></returns>
        string GetMaterialInfoByProc(string id);


        /// <summary>
        /// 线程分块（传入每个线程需执行数，需执行总数）
        /// </summary>
        /// <returns>分块后的线程总数</returns>
        int GetSeedCount(int onceTheCount, int allCount);
        /// <summary>
        /// 插入材料
        /// </summary>
        /// <returns></returns>
        object InsertMaterialsDetails(dynamic detailModel);
    }
}