using System;
using System.Collections;
using System.Configuration;
using System.Threading.Tasks;
using DataGet.Extensions;
using MySql.Data.MySqlClient;
using MySqlHelper = DataGet.Extensions.MySqlHelper;

namespace DataGet.ProvinceInvestmentPlatform.Services
{
    public class GetProvinceInvestmentPlatform : IGetProvinceInvestmentPlatform
    {
        private readonly string InvestmentPlatformApi = ConfigurationManager.AppSettings["InvestmentPlatformApi"]?.ToString();
        public Task<dynamic> GetMyDataBaseByDicAttribution()
        {
            return Task.Factory.StartNew<dynamic>(() => MySqlHelper.SqlByAdoExecuteQuery("SELECT AreaCode,Level FROM dic_attribution"));
        }

        public Task<dynamic> GetMyDataBaseByDicCompetentauth()
        {
            return Task.Factory.StartNew<dynamic>(() => MySqlHelper.SqlByAdoExecuteQuery("SELECT Id,DeptCode,ProvinceInvestmentPlatformDeptId,FULLNAME,AREACODE FROM dic_competentauth"));
        }

        public Task<dynamic> GetMyDataBaseByDicCompetentauthDetId()
        {
            return Task.Factory.StartNew<dynamic>(() => MySqlHelper.SqlByAdoExecuteQuery("SELECT ProvinceInvestmentPlatformDeptId as DeptId FROM dic_competentauth where ProvinceInvestmentPlatformDeptId is not null"));
        }

        public Task<dynamic> GetMyDataBaseAffairs()
        {
            return Task.Factory.StartNew<dynamic>(() => MySqlHelper.SqlByAdoExecuteQuery("SELECT *  FROM affairs"));
        }

        public Task<dynamic> GetMyDataBaseAffairsWhereInvestmentItemCodeIsNotNull()
        {
            return Task.Factory.StartNew<dynamic>(() => MySqlHelper.SqlByAdoExecuteQuery("SELECT AFFAIRID,InvestmentItemCode  FROM affairs WHERE InvestmentItemCode IS NOT NULL"));
        }

        public object InsertAffairs(dynamic affairsModel, dynamic affairguide, bool isDelete)
        {
            if (isDelete)
            {
                //删除事项表
                MySqlHelper.SqlByAdoExecuteNonQuery("DELETE affairs WHERE AFFAIRID=" + affairsModel.AFFAIRID + "");
                //删除对象情形表
                MySqlHelper.SqlByAdoExecuteNonQuery("DELETE materials_byobject WHERE AFFAIRID=" + affairsModel.AFFAIRID + "");
                //删除对象材料表
                //MySqlHelper.SqlByAdoExecuteNonQuery("DELETE materials_details WHERE AFFAIRID=" + affairsModel.AFFAIRID + "");
                //删除办事指南表
                MySqlHelper.SqlByAdoExecuteNonQuery("DELETE affairguide WHERE AFFAIRID=" + affairsModel.AFFAIRID + "");


            }

            string insertSql =
                "INSERT INTO affairs(AFFAIRCODE,AFFAIRNAME,AF_ABBREVIATION,itemImpleCode,AF_TYPE,AF_CLASSIFY,AF_LEVEL,AF_DEPARTMENT,TIMELIMIT,TIMELIMITTYPE,AF_PARENT,VALID,RESERVED1,RESERVED2,RESERVED3,RESERVED4,RESERVED5,ISUSEACCEPTDATE,IS_FILLIN,IS_YUYUE,HANDLE_TYPE,AffairTheme,AffairLifeEvent,AffairPerObject,ISTODO,ISDOPASS,ISCHARGE,ISSAVEMAT,IS_LEGALPERSON,LINKAFFAIRIDLSIT,promisePeriod,administrativeAuthorityType,InvestmentItemCode)  " +
                "VALUES('" + affairsModel.AFFAIRCODE.ToString() + "','" + affairsModel.AFFAIRNAME.ToString() + "'," +
                "'" + affairsModel.AF_ABBREVIATION.ToString() + "'," +
                "'" + affairsModel.itemImpleCode.ToString() + "'," +
                "'" + affairsModel.AF_TYPE.ToString() + "'," +
                "'" + affairsModel.AF_CLASSIFY.ToString() + "'," +
                "'" + affairsModel.AF_LEVEL.ToString() + "'," +
                "'" + affairsModel.AF_DEPARTMENT.ToString() + "'," +
                "'" + affairsModel.TIMELIMIT.ToString() + "'," +
                "'" + affairsModel.TIMELIMITTYPE.ToString() + "'," +
                "'" + affairsModel.AF_PARENT.ToString() + "'," +
                "'" + affairsModel.VALID.ToString() + "'," +
                "'" + affairsModel.RESERVED1.ToString() + "'," +
                "null," +
                "null," +
                "null," +
                "'0'," +//是否是电子即办件 0：否 1：是
                        //"','" + affairsModel.RESERVED2.ToString() + "'" +
                        //"','" + affairsModel.RESERVED3.ToString() + "'" +
                        //"','" + affairsModel.RESERVED4.ToString() + "'" +
                        //"','" + affairsModel.RESERVED5.ToString() + "'" +
                "'" + affairsModel.ISUSEACCEPTDATE.ToString() + "'," +
                "'" + affairsModel.IS_FILLIN.ToString() + "'," +
                "'" + affairsModel.IS_YUYUE.ToString() + "'," +
                "'" + affairsModel.HANDLE_TYPE.ToString() + "'," +
                //"'" + affairsModel.AffairTheme.ToString() + "'," +
                //"'" + affairsModel.AffairLifeEvent.ToString() + "'," +
                //"'" + affairsModel.AffairPerObject.ToString() + "'," +
                "null," +
                "null," +
                "null," +
                "'" + affairsModel.ISTODO.ToString() + "'," +
                "'" + affairsModel.ISDOPASS.ToString() + "'," +
                "'" + affairsModel.ISCHARGE.ToString() + "'," +
                "'" + affairsModel.ISSAVEMAT.ToString() + "'," +
                "'" + affairsModel.IS_LEGALPERSON.ToString() + "'," +
                "'" + affairsModel.LINKAFFAIRIDLSIT.ToString() + "'," +
                //"'" + affairsModel.legalPeriod.ToString() + "'," +
                "'" + affairsModel.promisePeriod.ToString() + "'," +
                "'" + affairsModel.administrativeAuthorityType.ToString() + "'," +
                "'" + affairsModel.InvestmentItemCode.ToString() + "'" +
                "); SELECT LAST_INSERT_ID();";
            var insertResult = MySqlHelper.SqlByAdoExecuteScalar(insertSql);
            //更新AFFAIRCODE字段（STZSX+AFFAIRID）
            MySqlHelper.SqlByAdoExecuteNonQuery(
                "UPDATE affairs as up SET up.AFFAIRCODE=CONCAT('TZSP',up.AFFAIRID) where up.InvestmentItemCode IS NOT NULL;");
            ////插入对象情形表

            MySqlHelper.SqlByAdoExecuteQuery("INSERT INTO materials_byobject(AFFAIRID,OBJINDEX,OBJNAME,VALID,OBJPARENT,CHILDSHOW,CHOOSEWAY,SELECTSHOW)" +
                                             " VALUES('" + insertResult.ToString() + "','1','默认情形','1','0','1','0','1')");
            //插入办事指南表
            MySqlHelper.SqlByAdoExecuteNonQuery("INSERT INTO affairguide(AFFAIRID,ACCORDING,PROCEDURES,MATERIAL,INSTITUTION,ACCESSORYPATH,`CONDITION`,XRNDOMU,SITE,TIME,INQUIRE,CHARGE,CHARGEGIST,SEPCIALVERSION)" +
            " VALUES(" +
            "'" + insertResult.ToString() + "'" +
            ",'" + affairguide.ACCORDING?.ToString() + "'" +
            ",'" + affairguide.PROCEDURES?.ToString() + "'" +
            ",'" + affairguide.MATERIAL?.ToString() + "'" +
            ",'" + affairguide.INSTITUTION?.ToString() + "'" +
            ",'" + affairguide.ACCESSORYPATH?.ToString() + "'" +
            ",'" + affairguide.CONDITION?.ToString() + "'" +
            ",'" + affairguide.XRNDOMU?.ToString() + "'" +
            ",'" + affairguide.SITE?.ToString() + "'" +
            ",'" + affairguide.TIME?.ToString() + "'" +
            ",'" + affairguide.INQUIRE?.ToString() + "'" +
            ",'" + affairguide.CHARGE?.ToString() + "'" +
            ",'" + affairguide.CHARGEGIST?.ToString() + "'" +
            ",'" + affairguide.SEPCIALVERSION?.ToString() + "'" +
            ")");
            return 0;

        }

        public void UpDateMyDataBaseByDicCompetentauth(int id, string fullName, string avtion, string deptcode, string areacode,
            string provinceInvestmentPlatformDeptId, string invDepCode)
        {
            if (id == 0)
            {
                int maxId = Convert.ToInt32(MySqlHelper.SqlByAdoExecuteScalar("SELECT MAX(ID) + 1 FROM dic_competentauth"));
                //插入dic_competentauth
                MySqlHelper.SqlByAdoExecuteNonQuery(
                    "INSERT INTO dic_competentauth(ID,FULLNAME,ABBREVIATION,VALID,DEPTCODE,AREACODE,ProvinceInvestmentPlatformDeptId,ProvinceInvestmentPlatformDEPTCODE) VALUES('" + maxId + "','" +
                    fullName + "','" + fullName + "',1,'" + deptcode + "','" + areacode + "','" + provinceInvestmentPlatformDeptId + "','" + invDepCode + "')");
                //插入dic_affairtype
                MySqlHelper.SqlByAdoExecuteNonQuery("INSERT INTO dic_affairtype(CODE,CNAME,SAVPATH,VALID) VALUES('" + maxId + "','" + fullName + "','" + fullName + "',1)");
                //插入dic_locdepartment
                MySqlHelper.SqlByAdoExecuteNonQuery("INSERT INTO dic_locdepartment(CODE,CNAME,VALID) VALUES('" + maxId + "','" + fullName + "',1)");
                //插入lb_classify
                MySqlHelper.SqlByAdoExecuteNonQuery("INSERT INTO lb_classify(ID,CLASSIFYNAME,PROJECTID,VALID) VALUES('" + maxId + "','" + fullName + "',1,1)");
            }
            else
            {
                MySqlHelper.SqlByAdoExecuteNonQuery("UPDATE dic_competentauth SET ProvinceInvestmentPlatformDeptId='" + provinceInvestmentPlatformDeptId + "',ProvinceInvestmentPlatformDEPTCODE='" + invDepCode + "' WHERE id=" + id);
            }
        }

        public string GetDeptInfoByCode(string code)
        {
            WebServiceProxy wsd = new WebServiceProxy(InvestmentPlatformApi, "getDeptInfoByCode");
            string[] str = { code };
            string suc = (string)wsd.ExecuteQuery("getDeptInfoByCode", str);

            return suc;
        }

        public string GetProceedingInfoByDept(string id)
        {
            WebServiceProxy wsd = new WebServiceProxy(InvestmentPlatformApi, "getProceedingInfoByDept");
            string[] str = { id };
            string suc = (string)wsd.ExecuteQuery("getProceedingInfoByDept", str);

            return suc;
        }

        public string GetMaterialInfoByProc(string id)
        {
            WebServiceProxy wsd = new WebServiceProxy(InvestmentPlatformApi, "getMaterialInfoByProc");
            string[] str = { id };
            string suc = (string)wsd.ExecuteQuery("getMaterialInfoByProc", str);

            return suc;
        }




        public int GetSeedCount(int onceTheCount, int allCount)
        {
            return Convert.ToInt32(allCount) % onceTheCount == 0 ? Convert.ToInt32(allCount) / onceTheCount : Convert.ToInt32(allCount) / onceTheCount + 1;
        }

        public object InsertMaterialsDetails(dynamic detailModel)
        {
            //删除对象材料表
            MySqlHelper.SqlByAdoExecuteNonQuery("DELETE from materials_details WHERE AFFAIRID=" + detailModel.AFFAIRID + " AND ProvinceInvestmentPlatformMaterialId = '" + detailModel.ProvinceInvestmentPlatformMaterialId + "'");

            //插入材料表
            MySqlHelper.SqlByAdoExecuteNonQuery("INSERT INTO materials_details(AFFAIRID,MATINDEX,ISTOP,ISMUST,MATNAME,MATTYPE,MATNUMBER,REQUIRED,ReuseDetail,ReuseTypeID,MATGROUP,REMARKS,EXAMPLEPATH,MATCODE,VALID,TABLEID,ProcessMaterial,EMPTYTABLEPATH,ISALLOWLACK,MATORDER,ALLOWNODE,ALLOWTIME,ProvinceInvestmentPlatformMaterialId) VALUES(" +
                                                "'" + detailModel.AFFAIRID + "'," +
                                                "'" + detailModel.MATINDEX + "'," +
                                                "'" + detailModel.ISTOP + "'," +
                                                "'" + detailModel.ISMUST + "'," +
                                                "'" + detailModel.MATNAME + "'," +
                                                "'" + detailModel.MATTYPE + "'," +
                                                "'" + detailModel.MATNUMBER + "'," +
                                                "'" + detailModel.REQUIRED + "'," +
                                                "'" + detailModel.ReuseDetail + "'," +
                                                "'" + detailModel.ReuseTypeID + "'," +
                                                "'" + detailModel.MATGROUP + "'," +
                                                "'" + detailModel.REMARKS + "'," +
                                                "'" + detailModel.EXAMPLEPATH + "'," +
                                                "'" + detailModel.MATCODE + "'," +
                                                "'" + detailModel.VALID + "'," +
                                                "'" + detailModel.TABLEID + "'," +
                                                "'" + detailModel.ProcessMaterial + "'," +
                                                "'" + detailModel.EMPTYTABLEPATH + "'," +
                                                "'" + detailModel.ISALLOWLACK + "'," +
                                                "'" + detailModel.MATORDER + "'," +
                                                "'" + detailModel.ALLOWNODE + "'," +
                                                "'" + detailModel.ALLOWTIME + "'," +
                                                "'" + detailModel.ProvinceInvestmentPlatformMaterialId + "'" +
                                                ")");

            return 0;

        }
    }
}