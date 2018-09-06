using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataGet.Extensions;
using DataGet.ProvinceInvestmentPlatform.Models;
using DataGet.ProvinceInvestmentPlatform.Services;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataGet
{
    /// <summary>
    /// 实现思路
    /// 1、同步部门（多线程执行）  成功以后
    /// 2、根据（1）中的部门编码，获取【已发布投资事项列表】（多线程执行）
    /// 3、获取事项以后，根据省接口获取到的事项Id获取事项材料
    /// 
    /// </summary>
    public class GetProvinceInvestmentPlatform
    {
        private static readonly IGetProvinceInvestmentPlatform Services = new ProvinceInvestmentPlatform.Services.GetProvinceInvestmentPlatform();
        //每个线程执行多少个部门同步任务
        private const int areaCodeThreadCount = 200;
        //每个线程同步的事项任务
        private const int proceedingInfoThreadCount = 200;
        //每个线程同步的材料任务
        private const int materialInfoThreadCount = 200;
        /// <summary>
        /// 全部执行完成委托
        /// </summary>
        public delegate void AllSuccess();
        /// <summary>
        /// 当同步操作全部执行成功后，调用次方法
        /// </summary>
        public event AllSuccess OnAllSuccess;

        //获取所有的部门
        private static List<dynamic> _DicCompetentauth = new List<dynamic>();

        //获取所有的事项
        private static List<dynamic> _Affairs = new List<dynamic>();

        //获取所有的事项
        private static List<dynamic> _dicAttribution = new List<dynamic>();

        private long allAffairsCount = 0;
        private static object _lock = new object();
        #region 测试
        public static void TestGetMyDb()
        {
            var result = Services.GetMyDataBaseByDicAttribution().Result;
            var listAreaCode = ((DataSet)result)?.Tables[0].ToDynamicList();
        }
        #endregion

        public void Start()
        {
            #region 部门
            //获取行政区域
            var resultAreaCode = Services.GetMyDataBaseByDicAttribution().Result;
            var listAreaCode = ((DataSet)resultAreaCode)?.Tables[0].ToDynamicList() ?? new List<dynamic>();
            //listAreaCode.Add(new { AreaCode = "530000" });

            //获取部门（dic_competentauth）表
            var resultDicCompetentauth = Services.GetMyDataBaseByDicCompetentauth().Result;
            var listDicCompetentauth = ((DataSet)resultDicCompetentauth)?.Tables[0].ToDynamicList() ?? new List<dynamic>();
            _DicCompetentauth = listDicCompetentauth;
            //获取所有的事项
            var resultAffairs = Services.GetMyDataBaseAffairs().Result;
            var listAffairs = ((DataSet)resultAffairs)?.Tables[0].ToDynamicList() ?? new List<dynamic>();
            _Affairs = listAffairs;
            var isExit = listDicCompetentauth.Any(a => a.DeptCode == "121212");
            if (listAreaCode.Any())
            {
                _dicAttribution = listAreaCode;
                //每200个区域码使用一个线程获取数据
                var areaCodeThSeed = Services.GetSeedCount(areaCodeThreadCount, listAreaCode.Count);
                using (var mthEvent = new MutipleThreadResetEvent(areaCodeThSeed))
                {
                    for (int i = 0; i < areaCodeThSeed; i++)
                    {

                        var thModel = new AsyncDepartmentModel()
                        {
                            AreaCodeList = listAreaCode,
                            AreaCodeThreadCount = areaCodeThreadCount,
                            ThreadResetEvent = mthEvent,
                            ThisThreadIndex = i
                        };
                        ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDepartment), thModel);
                    }
                    mthEvent.WaitAll();
                }
            }
            #endregion
            Console.WriteLine("部门编码同步完成");
            #region 事项

            //部门编码同步完成以后，从新获取一次，以便获取最新数据
            //获取部门（dic_competentauth）表
            resultDicCompetentauth = Services.GetMyDataBaseByDicCompetentauth().Result;
            listDicCompetentauth = ((DataSet)resultDicCompetentauth)?.Tables[0].ToDynamicList() ?? new List<dynamic>();
            _DicCompetentauth = listDicCompetentauth;

            //部门编码同步完成后,同步
            var resultDeptIds = Services.GetMyDataBaseByDicCompetentauthDetId().Result;
            var listDeptIds = ((DataSet)resultDeptIds)?.Tables[0].ToDynamicList() ?? new List<dynamic>();
            if (listDeptIds.Any())
            {
                //每200个区域码使用一个线程获取数据
                var proceedingInfoThSeed = Services.GetSeedCount(proceedingInfoThreadCount, listDeptIds.Count);
                using (var mthEvent = new MutipleThreadResetEvent(proceedingInfoThSeed))
                {

                    for (int i = 0; i < proceedingInfoThSeed; i++)
                    {

                        var thModel = new AsyncProceedingInfoModel()
                        {
                            DeptIdList = listDeptIds,
                            ProceedingInfoThreadCount = proceedingInfoThreadCount,
                            ThreadResetEvent = mthEvent,
                            ThisThreadIndex = i
                        };
                        ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncProceedingInfo), thModel);
                    }
                    mthEvent.WaitAll();
                }
            }
            #endregion
            Console.WriteLine("事项同步完成");
            #region 材料
            //同步材料
            var resultInvestmentItemCode = Services.GetMyDataBaseAffairsWhereInvestmentItemCodeIsNotNull().Result;
            var listInvestmentItemCode = ((DataSet)resultInvestmentItemCode)?.Tables[0].ToDynamicList() ?? new List<dynamic>();
            if (listInvestmentItemCode.Any())
            {
                var materialInfoThSeed = Services.GetSeedCount(materialInfoThreadCount, listInvestmentItemCode.Count);
                using (var mthEvent = new MutipleThreadResetEvent(materialInfoThSeed))
                {

                    for (int i = 0; i < materialInfoThSeed; i++)
                    {

                        var thModel = new AsyncMaterialInfoModel()
                        {
                            MaterialInfoList = listInvestmentItemCode,
                            AreaCodeThreadCount = materialInfoThreadCount,
                            ThreadResetEvent = mthEvent,
                            ThisThreadIndex = i
                        };
                        ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncMaterialInfo), thModel);
                    }
                    mthEvent.WaitAll();
                }
            }
            #endregion
            Console.WriteLine("材料同步完成");
            //全部完成后释放内存
            resultAreaCode = null;
            listAreaCode = null;
            resultDicCompetentauth = null;
            listDicCompetentauth = null;
            _DicCompetentauth = null;
            resultAffairs = null;
            _Affairs = null;

            OnAllSuccess?.Invoke();
        }
        #region 同步部门

        private void AsyncDepartment(object o)
        {
            var model = o as AsyncDepartmentModel;

            try
            {
                if (model != null)
                {
                    //List分组
                    var pageList = model.AreaCodeList.Skip(model.ThisThreadIndex * areaCodeThreadCount)
                        .Take(areaCodeThreadCount);
                    var enumerable = pageList as dynamic[] ?? pageList.ToArray();
                    Console.WriteLine("部门线程【" + model.ThisThreadIndex + "】,编码条数：【" + enumerable.Count() + "】");
                    //获取到list以后循环去省接口里面调用数据
                    foreach (var item in enumerable)
                    {
                        string areaCode = (item.AreaCode).ToString();
                        string pAreCode = string.Empty;
                        if (item.Level.ToString() == "1" || item.Level.ToString() == "2")
                        {
                            pAreCode = areaCode.Substring(0, 6);
                        }
                        else if (item.Level.ToString() == "3")
                        {
                            pAreCode = areaCode.Substring(0, 10);
                        }
                        else
                        {
                            pAreCode = areaCode.Substring(0, 12);
                        }

                        if (pAreCode == "530195")
                        {
                            //昆明倘甸产业园区，特殊的
                            pAreCode = "530134";
                        }

                        if (pAreCode == "530114")
                        {
                            //呈贡
                            pAreCode = "530121";
                        }

                        if (pAreCode == "530191")
                        {
                            //经开区
                            pAreCode = "530131";
                        }
                        if (pAreCode == "530192")
                        {
                            //高新区
                            pAreCode = "530130";
                        }

                        if (pAreCode == "530193")
                        {
                            pAreCode = "530132";
                        }

                        if (pAreCode == "530194")
                        {
                            pAreCode = "530133";
                        }
                        var jsonData = Services.GetDeptInfoByCode(pAreCode);
                        if (!string.IsNullOrEmpty(jsonData) && jsonData != "[]")
                        {

                            //using (StreamWriter sw = new StreamWriter("D://JsonFile/" + areaCode + ".json", true))
                            //{
                            //    sw.WriteLine(jsonData);
                            //}

                            //var jStr = jsonData;
                            var objData = (JArray)JsonConvert.DeserializeObject<dynamic>(jsonData);
                            foreach (var jItem in objData)
                            {
                                if (!string.IsNullOrEmpty(jItem["orgcode"]?.ToString()))
                                {
                                    //去除orgcode中的-
                                    string orgCode = jItem["orgcode"]?.ToString().Replace("-", "");

                                    switch (orgCode.ToUpper())
                                    {
                                        case "5301023":
                                            orgCode = "015115361";
                                            break;
                                        case "PLKX001":
                                            orgCode = "015119215";
                                            break;
                                        case "741487067":
                                            orgCode = "741457067";
                                            break;
                                        case "552719748":
                                            orgCode = "552719478";
                                            break;
                                        case "53011307":
                                            orgCode = "552736083";
                                            break;
                                        case "132469789":
                                            orgCode = "015127127";
                                            break;

                                        case "111236478":
                                            orgCode = "015127231";
                                            break;
                                        case "530124569":
                                            orgCode = "FMXGAGXFD";
                                            break;
                                        case "888999666":
                                            orgCode = "015127370";
                                            break;
                                        case "332664881":
                                            orgCode = "G82762522";
                                            break;
                                        case "552552331":
                                            orgCode = "787359163";
                                            break;



                                        case "134556789":
                                            orgCode = "015108890";
                                            break;
                                        case "123569789":
                                            orgCode = "557786629";
                                            break;
                                        case "444555666":
                                            orgCode = "775527094";
                                            break;
                                        case "431444498":
                                            orgCode = "31444498F";
                                            break;
                                        case "11530126015124911U":
                                            orgCode = "015124911";
                                            break;


                                        case "11530126MB0L087227":
                                            orgCode = "015124903";
                                            break;
                                        //case "431465010":
                                        //    orgCode = "KMSSMXQXJ";
                                        //    break;
                                        //case "431460076":
                                        //    orgCode = "KMSXMXFZJ";
                                        //    break;
                                        case "115301276812685549":
                                        case "115301276812685000":
                                            orgCode = "681268554";
                                            break;
                                        case "015128365":
                                            orgCode = "MB0P60671";
                                            break;

                                        //case "41348086X":
                                        //    orgCode = "KMSLQQXJ";
                                        //    break;
                                        case "054663600":
                                            orgCode = "MB0U4692X";
                                            break;
                                        case "530128Z1":
                                            orgCode = "566234092";
                                            break;
                                        //case "58963120X":
                                        //    orgCode = "KMSTDQQXJ";
                                        //    break;

                                        case "530114":
                                            orgCode = "015129093";
                                            break;
                                        case "XX9471528":
                                            orgCode = "GXQFGWJFJ";
                                            break;
                                        case "64279477":
                                            orgCode = "356104230";
                                            break;
                                        case "530112001":
                                            orgCode = "06429102X";
                                            break;
                                        //度假区地方事务管理局
                                        case "530112004":
                                            orgCode = "571876815";
                                            break;
                                        //滇池度假开发区管理部门
                                        //case "530112":
                                        //    orgCode = "530193ZX01";
                                        //    break;
                                        //盘龙区政务局
                                        case "900000007":
                                            orgCode = "727305083";
                                            break;
                                        //已经有的 530132
                                        case "530112007":
                                            orgCode = "06428468X";
                                            break;
                                        case "530112006":
                                            orgCode = "067113368";
                                            break;
                                        //昆明市管理部门
                                        //case "900000000":
                                        //    orgCode = "5301ZX01";
                                        //    break;
                                        //官渡区管理部门
                                        case "900000008":
                                            orgCode = "563163831";
                                            break;
                                        //西山区政务局
                                        case "900000004":
                                            orgCode = "356077906";
                                            break;
                                        //西山区城市管理综合行政执法局
                                        case "167087092":
                                            orgCode = "767087092";
                                            break;
                                        //西山区气象局
                                        case "53011206":
                                            orgCode = "431411549";
                                            break;
                                        //东川区综合窗口
                                        //case "900000009":
                                        //    orgCode = "530113ZX01";
                                        //    break;
                                        //呈贡区综合窗口
                                        //case "900000003":
                                        //    orgCode = "530114ZX01";
                                            break;
                                        //晋宁县管理部门
                                        //case "900000002":
                                        //    orgCode = "530122ZX01";
                                        //    break;
                                        //石林彝族自治县管理部门
                                        //case "900000013":
                                        //    orgCode = "530126ZX01";
                                        //    break;
                                        //嵩明县管理部门
                                        //case "900000001":
                                        //    orgCode = "530127ZX01";
                                        //    break;
                                        case "900000012":
                                            orgCode = "MB0Q24540";
                                            break;
                                        //case "530111":
                                        //    orgCode = "530191ZX01";
                                        //    break;
                                        case "XX512893X":
                                            orgCode = "MB0L04983";
                                            break;
                                        //case "900000015":
                                        //    orgCode = "530192ZX01";
                                        //    break;
                                        case "XX1569620":
                                            orgCode = "GXQNLSNLS";
                                            break;
                                        //case "530125":
                                        //    orgCode = "530194ZX01";
                                        //    break;
                                        //case "900000006":
                                        //    orgCode = "530102ZX01";
                                        //    break;
                                        //昆明经开区综合窗口
                                        case "530111ZX02":
                                            orgCode = "530191ZX01";
                                            break;
                                        //昆明高新区综合窗口
                                        case "530102ZX02":
                                            orgCode = "530192ZX01";
                                            break;
                                        //昆明滇池度假区综合窗口
                                        case "530112ZX02":
                                            orgCode = "530193ZX01";
                                            break;
                                        //昆明倘甸产业园区综合窗口
                                        case "530129XZ02":
                                            orgCode = "530195ZX01";
                                            break;
                                    }

                                    if (jItem["orgcode"]?.ToString() == "01511918-6")
                                    {
                                        orgCode = "697953273";
                                    }



                                    if (orgCode.Length >= 9 && orgCode.Length != 10)
                                    {
                                        orgCode = orgCode.Substring(0, 9);
                                    }
                                    
                                    if (jItem["orgcode"]?.ToString() == "431465010")
                                    {

                                    }
                                    //if (pAreCode == "530100")
                                    //{
                                    //  bool bb=  _DicCompetentauth.Where(w=>w.DeptCode == "01511326X").Count()>0;
                                    //}
                                    if (_DicCompetentauth.Any(
                                        exit => exit.DeptCode.ToString().ToUpper().Equals(orgCode.ToUpper())))
                                    {
                                        var findModel = _DicCompetentauth.FirstOrDefault(f =>
                                            f.DeptCode.ToString().ToUpper().Equals(orgCode.ToUpper()));
                                        if (findModel != null)
                                        {
                                            string depId = jItem["depId"]?.ToString();

                                            Services.UpDateMyDataBaseByDicCompetentauth(Convert.ToInt32(findModel.Id),
                                                "", "", "", "", depId, jItem["orgcode"]?.ToString());
                                        }
                                        else
                                        {
                                            //string areacode = areaCode;


                                            //if (jItem["regionalismCode"] != null)
                                            //{
                                            //    var regionalismCode = jItem["regionalismCode"]?.ToString();
                                            //    if (!string.IsNullOrEmpty(regionalismCode))
                                            //    {
                                            //        if (regionalismCode == "530134")
                                            //        {
                                            //            regionalismCode = "530195";
                                            //        }
                                            //        else if (regionalismCode == "530121")
                                            //        {
                                            //            regionalismCode = "530114";
                                            //        }
                                            //        else if (regionalismCode == "530131")
                                            //        {
                                            //            regionalismCode = "530191";
                                            //        }
                                            //        else if (regionalismCode == "530130")
                                            //        {
                                            //            regionalismCode = "530192";
                                            //        }
                                            //        else if (regionalismCode == "530132")
                                            //        {
                                            //            regionalismCode = "530193";
                                            //        }
                                            //        else if (regionalismCode == "530133")
                                            //        {
                                            //            regionalismCode = "530194";
                                            //        }
                                            //    }

                                            //    areacode = regionalismCode.PadRight(12, '0');
                                            //}
                                            //else
                                            //{
                                            //    //五华区气象局
                                            //    if (jItem["orgcode"]?.ToString() == "5301024")
                                            //    {
                                            //        areacode = "530102000000";
                                            //    }
                                            //    //市公安局五华分局消防大队窗口
                                            //    else if (jItem["orgcode"]?.ToString() == "5301022")
                                            //    {
                                            //        areacode = "530102000000";
                                            //    }
                                            //    //西山消防大队
                                            //    else if (jItem["orgcode"]?.ToString() == "53011202")
                                            //    {
                                            //        areacode = "530112000000";
                                            //    }
                                            //    //禄劝消防大队
                                            //    else if (jItem["orgcode"]?.ToString() == "530128XF")
                                            //    {
                                            //        areacode = "530128000000";
                                            //    }
                                            //    //安宁市消防大队
                                            //    else if (jItem["orgcode"]?.ToString() == "53018102")
                                            //    {
                                            //        areacode = "530181000000";
                                            //    }

                                            //    else if (jItem["orgcode"]?.ToString() == "PLGH001")
                                            //    {
                                            //        areacode = "530103000000";
                                            //    }
                                            //    else if (jItem["orgcode"]?.ToString() == "GDGH001")
                                            //    {
                                            //        areacode = "530111000000";
                                            //    }
                                            //    else if (jItem["orgcode"]?.ToString() == "686170329")
                                            //    {
                                            //        areacode = "530191000000";
                                            //    }
                                            //    //市规划局五华分局
                                            //    else if (jItem["orgcode"]?.ToString() == "0151141wh")
                                            //    {
                                            //        areacode = "530102000000";
                                            //    }
                                            //    //官渡区食品药品监督管理局
                                            //    else if (jItem["orgcode"]?.ToString() == "431400567")
                                            //    {
                                            //        areacode = "530111000000";
                                            //    }
                                            //    //昆明市官渡区质量技术监督局
                                            //    else if (jItem["orgcode"]?.ToString() == "431400073")
                                            //    {
                                            //        areacode = "530111000000";
                                            //    }

                                            //    //宜良县政务服务管理局
                                            //    else if (jItem["orgcode"]?.ToString() == "900000010")
                                            //    {
                                            //        areacode = "530125000000";
                                            //    }
                                            //    //禄劝气象局
                                            //    else if (jItem["orgcode"]?.ToString() == "41348086-x")
                                            //    {
                                            //        areacode = "530128000000";
                                            //    }
                                            //    //经开区国土分局
                                            //    else if (jItem["orgcode"]?.ToString() == "745296766")
                                            //    {
                                            //        areacode = "530191000000";
                                            //    }
                                            //    else if (jItem["orgcode"]?.ToString() == "XX6590338")
                                            //    {
                                            //        areacode = "530192000000";
                                            //    }
                                            //    else if (jItem["orgcode"]?.ToString() == "900000016")
                                            //    {
                                            //        areacode = "530195000000";
                                            //    }
                                            //    else if (jItem["orgcode"]?.ToString() == "530129")
                                            //    {
                                            //        areacode = "530195000000";
                                            //    }
                                            //    else if (jItem["orgcode"]?.ToString() == "58963120-X")
                                            //    {
                                            //        areacode = "530195000000";
                                            //    }

                                            //    else if (jItem["orgcode"]?.ToString() == "431465010")
                                            //    {
                                            //        areacode = "530127000000";
                                            //    }
                                            //    else if (jItem["orgcode"]?.ToString() == "431460076")
                                            //    {
                                            //        areacode = "530127000000";
                                            //    }
                                            //}
                                            //没找到就新增
                                            Console.WriteLine("需要新增部门");
                                            string depId = jItem["depId"]?.ToString();
                                            string name = jItem["name"]?.ToString();
                                            string deptCode = jItem["orgcode"]?.ToString().Replace("-", "");
                                            string desc = jItem["desc"]?.ToString();
                                            string code = jItem["code"]?.ToString();
                                            Services.UpDateMyDataBaseByDicCompetentauth(0,
                                                name, desc, deptCode, areaCode, depId, jItem["orgcode"]?.ToString());
                                        }

                                    }
                                    else
                                    {
                                        //string areacode = "";


                                        //if (jItem["regionalismCode"] != null)
                                        //{
                                        //    var regionalismCode = jItem["regionalismCode"]?.ToString();
                                        //    if (!string.IsNullOrEmpty(regionalismCode))
                                        //    {
                                        //        if (regionalismCode == "530134")
                                        //        {
                                        //            regionalismCode = "530195";
                                        //        }
                                        //        else if (regionalismCode == "530121")
                                        //        {
                                        //            regionalismCode = "530114";
                                        //        }
                                        //        else if (regionalismCode == "530131")
                                        //        {
                                        //            regionalismCode = "530191";
                                        //        }
                                        //        else if (regionalismCode == "530130")
                                        //        {
                                        //            regionalismCode = "530192";
                                        //        }
                                        //        else if (regionalismCode == "530132")
                                        //        {
                                        //            regionalismCode = "530193";
                                        //        }
                                        //        else if (regionalismCode == "530133")
                                        //        {
                                        //            regionalismCode = "530194";
                                        //        }
                                        //    }

                                        //    areacode = regionalismCode.PadRight(12, '0');
                                        //}
                                        //else
                                        //{
                                        //    //五华区气象局
                                        //    if (jItem["orgcode"]?.ToString() == "5301024")
                                        //    {
                                        //        areacode = "530102000000";
                                        //    }
                                        //    //市公安局五华分局消防大队窗口
                                        //    else if (jItem["orgcode"]?.ToString() == "5301022")
                                        //    {
                                        //        areacode = "530102000000";
                                        //    }
                                        //    //西山消防大队
                                        //    else if (jItem["orgcode"]?.ToString() == "53011202")
                                        //    {
                                        //        areacode = "530112000000";
                                        //    }
                                        //    //禄劝消防大队
                                        //    else if (jItem["orgcode"]?.ToString() == "530128XF")
                                        //    {
                                        //        areacode = "530128000000";
                                        //    }
                                        //    //安宁市消防大队
                                        //    else if (jItem["orgcode"]?.ToString() == "53018102")
                                        //    {
                                        //        areacode = "530181000000";
                                        //    }

                                        //    else if (jItem["orgcode"]?.ToString() == "PLGH001")
                                        //    {
                                        //        areacode = "530103000000";
                                        //    }
                                        //    else if (jItem["orgcode"]?.ToString() == "GDGH001")
                                        //    {
                                        //        areacode = "530111000000";
                                        //    }
                                        //    else if (jItem["orgcode"]?.ToString() == "686170329")
                                        //    {
                                        //        areacode = "530191000000";
                                        //    }
                                        //    //市规划局五华分局
                                        //    else if (jItem["orgcode"]?.ToString() == "0151141wh")
                                        //    {
                                        //        areacode = "530102000000";
                                        //    }
                                        //    //官渡区食品药品监督管理局
                                        //    else if (jItem["orgcode"]?.ToString() == "431400567")
                                        //    {
                                        //        areacode = "530111000000";
                                        //    }
                                        //    //昆明市官渡区质量技术监督局
                                        //    else if (jItem["orgcode"]?.ToString() == "431400073")
                                        //    {
                                        //        areacode = "530111000000";
                                        //    }

                                        //    //宜良县政务服务管理局
                                        //    else if (jItem["orgcode"]?.ToString() == "900000010")
                                        //    {
                                        //        areacode = "530125000000";
                                        //    }
                                        //    //禄劝气象局
                                        //    else if (jItem["orgcode"]?.ToString() == "41348086-x")
                                        //    {
                                        //        areacode = "530128000000";
                                        //    }
                                        //    //经开区国土分局
                                        //    else if (jItem["orgcode"]?.ToString() == "745296766")
                                        //    {
                                        //        areacode = "530191000000";
                                        //    }
                                        //    else if (jItem["orgcode"]?.ToString() == "XX6590338")
                                        //    {
                                        //        areacode = "530192000000";
                                        //    }
                                        //    else if (jItem["orgcode"]?.ToString() == "900000016")
                                        //    {
                                        //        areacode = "530195000000";
                                        //    }
                                        //    else if (jItem["orgcode"]?.ToString() == "530129")
                                        //    {
                                        //        areacode = "530195000000";
                                        //    }
                                        //    else if (jItem["orgcode"]?.ToString() == "58963120-X")
                                        //    {
                                        //        areacode = "530195000000";
                                        //    }

                                        //    else if (jItem["orgcode"]?.ToString() == "431465010")
                                        //    {
                                        //        areacode = "530127000000";
                                        //    }
                                        //    else if (jItem["orgcode"]?.ToString() == "431460076")
                                        //    {
                                        //        areacode = "530127000000";
                                        //    }
                                        //}
                                        //没找到就新增
                                        Console.WriteLine("需要新增部门");
                                        string depId = jItem["depId"]?.ToString();
                                        string name = jItem["name"]?.ToString();
                                        string deptCode = jItem["orgcode"]?.ToString().Replace("-", "");
                                        string desc = jItem["desc"]?.ToString();
                                        string code = jItem["code"]?.ToString();
                                        Services.UpDateMyDataBaseByDicCompetentauth(0,
                                            name, desc, deptCode, areaCode, depId, jItem["orgcode"]?.ToString());
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("orgcode 为空，id:[" + jItem["depId"] + "],name:[" + jItem["name"] + "]");
                                }
                            }


                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("部门同步错误：" + e.Message);
            }
            Console.WriteLine("部门线程【" + model.ThisThreadIndex + "】同步完成。。。");
            model.ThreadResetEvent.SetOne();

        }

        #endregion

        #region 同步事项
        void AsyncProceedingInfo(object o)
        {
            var model = o as AsyncProceedingInfoModel;
            try
            {
                if (model != null)
                {
                    //List分组
                    var pageList = model.DeptIdList.Skip(model.ThisThreadIndex * proceedingInfoThreadCount).Take(proceedingInfoThreadCount);
                    var enumerable = pageList as dynamic[] ?? pageList.ToArray();
                    Console.WriteLine("事项线程【" + model.ThisThreadIndex + "】,编码条数：【" + enumerable.Count() + "】");
                    //获取到list以后循环去省接口里面调用数据
                    foreach (var item in enumerable)
                    {
                        string deptId = (item.DeptId).ToString();
                        var jsonData = Services.GetProceedingInfoByDept(deptId);
                        if (!string.IsNullOrEmpty(jsonData) && jsonData != "[]")
                        {
                            var objData = (JArray)JsonConvert.DeserializeObject<dynamic>(jsonData);
                            lock (_lock)
                            {
                                allAffairsCount = allAffairsCount + objData.Count;
                            }
                            Console.WriteLine("事项数量：" + allAffairsCount);

                            //判断此事项是否在“我们”的数据库中，如果存在且InvestmentItemCode有值，则不管
                            foreach (var jItem in objData)
                            {
                                string dbLevel = "1";


                                var isExitCode = _Affairs.Any(a => a.InvestmentItemCode.ToString().Equals(jItem["id"].ToString()));
                                //获取部门编码
                                var depMyDataBaseCompetentauth = _DicCompetentauth.FirstOrDefault(w => w.ProvinceInvestmentPlatformDeptId.ToString() == jItem["departmentId"].ToString());

                                var administrativeAuthorityType = "1";
                                if (!string.IsNullOrEmpty(jItem["type"]?.ToString()))
                                {
                                    switch (jItem["type"].ToString().ToUpper())
                                    {
                                        case "SYNDA-PT-ADMINI"://行政许可事项
                                            {
                                                administrativeAuthorityType = "1";
                                                break;
                                            }
                                        case "SYNDA-PT-UNADMINI"://非行政许可事项
                                            {
                                                administrativeAuthorityType = "10";
                                                break;
                                            }
                                        case "SYNDA-PT-SERVICE"://社会服务
                                            {
                                                administrativeAuthorityType = "8";
                                                break;
                                            }
                                        default:
                                            {
                                                administrativeAuthorityType = "14";
                                                break;
                                            }
                                    }
                                }
                                if (!isExitCode)
                                {
                                    Console.WriteLine("新增事项！");
                                    if (depMyDataBaseCompetentauth != null)
                                    {
                                        //获取Level
                                        var level = _dicAttribution.FirstOrDefault(s => s.AreaCode == depMyDataBaseCompetentauth.AREACODE.ToString());
                                        if (level != null)
                                        {
                                            dbLevel = level.Level.ToString();
                                        }


                                        var isToDo = "1";

                                        if (jItem["transactType"]?.ToString() == "SYNDA-TRAN-IMME")
                                        {
                                            isToDo = "1";
                                        }
                                        else if (jItem["transactType"]?.ToString() == "SYNDA-TRAN-COMMI")
                                        {
                                            isToDo = "2";
                                        }
                                        else if (jItem["transactType"]?.ToString() == "SYNDA-TRAN-SEND-UP")
                                        {
                                            isToDo = "2";
                                        }
                                        //不存在则插入
                                        var affairsModel = new
                                        {
                                            AFFAIRCODE = "TZSX" + DateTime.Now.ToString("yyyyMMddHH"),
                                            AFFAIRNAME = jItem["name"].ToString(),
                                            AF_ABBREVIATION = jItem["name"].ToString(),
                                            itemImpleCode = "",
                                            AF_TYPE = depMyDataBaseCompetentauth.Id,
                                            AF_CLASSIFY = 2,//投资事项
                                            AF_LEVEL = dbLevel,
                                            AF_DEPARTMENT = depMyDataBaseCompetentauth.Id,
                                            TIMELIMIT = jItem["legalTime"].ToString(),
                                            TIMELIMITTYPE = 1,
                                            AF_PARENT = 0,
                                            VALID = 1,
                                            RESERVED1 = 0,
                                            ISUSEACCEPTDATE = 0,
                                            IS_FILLIN = 0,
                                            IS_YUYUE = 0,
                                            HANDLE_TYPE = 0,
                                            AffairTheme = 0,
                                            AffairLifeEvent = 0,
                                            AffairPerObject = 0,
                                            ISTODO = isToDo,//是否是即办件 1：即办件 2：流转件
                                            ISDOPASS = 0,
                                            ISCHARGE = jItem["needCharge"].ToString(),
                                            ISSAVEMAT = 0,
                                            IS_LEGALPERSON = 1,
                                            LINKAFFAIRIDLSIT = "",

                                            legalPeriod = jItem["legalTime"].ToString(),
                                            promisePeriod = jItem["commitmentTime"].ToString(),
                                            administrativeAuthorityType = administrativeAuthorityType,
                                            InvestmentItemCode = jItem["id"].ToString()
                                        };
                                        //办事指南
                                        var affairguide = new
                                        {
                                            ACCORDING = jItem["blBasis"]?.ToString(),
                                            PROCEDURES = jItem["powerProcess"]?.ToString(),
                                            MATERIAL = "",
                                            INSTITUTION = "",
                                            ACCESSORYPATH = "",
                                            CONDITION = jItem["conditions"]?.ToString(),
                                            XRNDOMU = depMyDataBaseCompetentauth?.FULLNAME?.ToString(),
                                            SITE = jItem["address"]?.ToString(),
                                            TIME = jItem["legalTime"]?.ToString(),
                                            INQUIRE = jItem["ext1"]?.ToString(),
                                            CHARGE = jItem["chargeStandard"]?.ToString(),
                                            CHARGEGIST = jItem["feeBasis"]?.ToString(),
                                            SEPCIALVERSION = ""
                                        };
                                        Services.InsertAffairs(affairsModel, affairguide, false);
                                    }
                                    else
                                    {
                                        Console.WriteLine("同步事项失败，未在我们数据库找到[省]部门编号为【" + jItem["departmentId"].ToString() + "】的部门，表【dic_competentauth】");
                                    }
                                }
                                else
                                {
                                    string affairsDepId = "";
                                    if (depMyDataBaseCompetentauth != null)
                                    {
                                        //获取Level
                                        var level = _dicAttribution.FirstOrDefault(s =>
                                            s.AreaCode == depMyDataBaseCompetentauth.AREACODE.ToString());
                                        if (level != null)
                                        {
                                            dbLevel = level.Level.ToString();
                                        }

                                        affairsDepId = depMyDataBaseCompetentauth.Id.ToString();
                                    }

                                    //事项已经存在了
                                    //todo 存在了事项以后
                                    //目前存在了暂时只更新administrativeAuthorityType这个字段
                                    var isToDo = "1";
                                    if (jItem["transactType"]?.ToString() == "SYNDA-TRAN-IMME")
                                    {
                                        isToDo = "1";
                                    }
                                    else if (jItem["transactType"]?.ToString() == "SYNDA-TRAN-COMMI")
                                    {
                                        isToDo = "2";
                                    }
                                    else if (jItem["transactType"]?.ToString() == "SYNDA-TRAN-SEND-UP")
                                    {
                                        isToDo = "2";
                                    }

                                    Console.WriteLine("更新administrativeAuthorityType这个字段【值为:" + administrativeAuthorityType + "】,AF_LEVEL【" + dbLevel + "】");
                                    if (!string.IsNullOrEmpty(affairsDepId))
                                    {
                                        Extensions.MySqlHelper.SqlByAdoExecuteNonQuery("UPDATE affairs SET AF_TYPE='" + affairsDepId + "',AF_DEPARTMENT='" + affairsDepId + "',administrativeAuthorityType = '" + administrativeAuthorityType + "',ISTODO='" + isToDo + "',AF_LEVEL='" + dbLevel + "' WHERE InvestmentItemCode='" + jItem["id"].ToString() + "'");
                                    }
                                    else
                                    {
                                        Extensions.MySqlHelper.SqlByAdoExecuteNonQuery("UPDATE affairs SET administrativeAuthorityType = '" + administrativeAuthorityType + "',ISTODO='" + isToDo + "',AF_LEVEL='" + dbLevel + "' WHERE InvestmentItemCode='" + jItem["id"].ToString() + "'");
                                    }
                                }

                            }

                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("事项同步出错：" + e.Message + ";" + e.StackTrace);
            }
            model.ThreadResetEvent.SetOne();
        }

        #endregion

        #region 同步事项材料

        void AsyncMaterialInfo(object o)
        {
            var model = o as AsyncMaterialInfoModel;
            try
            {

                if (model != null)
                {
                    //List分组
                    var pageList = model.MaterialInfoList.Skip(model.ThisThreadIndex * materialInfoThreadCount)
                        .Take(materialInfoThreadCount);
                    var enumerable = pageList as dynamic[] ?? pageList.ToArray();
                    Console.WriteLine("材料线程【" + model.ThisThreadIndex + "】,编码条数：【" + enumerable.Count() + "】");
                    //获取到list以后循环去省接口里面调用数据
                    foreach (var item in enumerable)
                    {
                        var investmentItemCode = item.InvestmentItemCode.ToString();
                        var jsonData = Services.GetMaterialInfoByProc(investmentItemCode);
                        if (!string.IsNullOrEmpty(jsonData) && jsonData != "[]")
                        {
                            var objData = (JArray)JsonConvert.DeserializeObject<dynamic>(jsonData);
                            foreach (var jItem in objData)
                            {
                                var matName = jItem["materialName"].ToString();
                                matName = matName.TrimEnd('；');
                                matName = matName.TrimEnd(';');
                                matName = matName.Replace("\n", "");
                                matName = matName.Replace("\r", "");
                                matName = matName.Replace("\\", "");
                                matName = matName.Replace("/", "");
                                matName = matName.Replace("\"", "");
                                matName = matName.Replace("'", "");
                                matName = matName.Trim();

                                //判断获取的材料名称长度，超过长度就截取（理论上正常没有哪个材料名称会很长的，省网厅数据配置不规范）
                                int matNameLength = System.Text.UTF8Encoding.UTF8.GetBytes(matName).Length;
                                int maxLength = 200;
                                if (matNameLength > maxLength)//varchar=200，数据库只允许存200字符 数据库UTF-8 1个数字、字母、英文符号算1个长度 1个中文、中文符号算3个长度 
                                {
                                    matName = GetSubString(matName, maxLength, Encoding.UTF8);
                                }

                                var detailModel = new
                                {
                                    AFFAIRID = item.AFFAIRID.ToString(),
                                    MATINDEX = 1,
                                    ISTOP = 0,
                                    ISMUST = 1,
                                    MATNAME = matName,
                                    MATTYPE = 99,
                                    MATNUMBER = jItem["materialNumber"].ToString(),
                                    REQUIRED = "是",
                                    ReuseDetail = "",
                                    ReuseTypeID = "1",
                                    MATGROUP = 0,
                                    REMARKS = "",
                                    EXAMPLEPATH = "",
                                    MATCODE = jItem["exchangeCode"].ToString(),
                                    VALID = 1,
                                    TABLEID = 0,
                                    ProcessMaterial= 0,
                                    EMPTYTABLEPATH = "",
                                    ISALLOWLACK = 0,
                                    MATORDER = jItem["materialSort"].ToString(),
                                    ALLOWNODE = 0,
                                    ALLOWTIME = 0,
                                    ProvinceInvestmentPlatformMaterialId = jItem["materialId"].ToString()
                                };
                                Console.WriteLine("新增材料名称【" + matName + "】,材料编码：【" + jItem["materialNumber"].ToString() + "】");
                                Services.InsertMaterialsDetails(detailModel);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("材料同步错误：" + e.Message);
            }
            model.ThreadResetEvent.SetOne();
        }

        #endregion


        /// <summary>
        /// 按不同的字节编码，通过字节数去截取字符串
        /// 数据库UTF-8 1个数字、字母、英文符号算1个长度 1个中文、中文符号算3个长度
        /// </summary>
        /// <param name="origStr">需截取的字符串</param>
        /// <param name="bytesLength">需截取的字节长度</param>
        /// <param name="dstEncoding">截取的字节编码类型</param>
        /// <returns></returns>
        public static string GetSubString(string origStr, int bytesLength, Encoding dstEncoding)
        {
            if (origStr == null || origStr.Length == 0 || bytesLength < 0)
                return "";
            int bytesCount = dstEncoding.GetByteCount(origStr);
            if (bytesCount > bytesLength)
            {
                int readyLength = 0;
                int byteLength;
                for (int i = 0; i < origStr.Length; i++)
                {
                    byteLength = dstEncoding.GetByteCount(new char[] { origStr[i] });
                    readyLength += byteLength;
                    if (readyLength == bytesLength)
                    {
                        origStr = origStr.Substring(0, i + 1);// + "..."; 加省略号
                        break;
                    }
                    else if (readyLength > bytesLength)
                    {
                        origStr = origStr.Substring(0, i);// + "..."; 加省略号
                        break;
                    }
                }
            }
            return origStr;
        }


    }
}
