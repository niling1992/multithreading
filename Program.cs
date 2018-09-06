using DataGet.Extensions;
using DataGet.Models;
using DataGet.Models.AffairModel;
using DataGet.Models.AreaModel;
using DataGet.Models.depModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using DataGet.ProvinceInvestmentPlatform.Services;

namespace DataGet
{
    class Program
    {

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        //云南省投资项目在线审批监管平台
        static GetProvinceInvestmentPlatform _provinceInvestmentPlatform = new GetProvinceInvestmentPlatform();

        static void closebtn()
        {
            IntPtr windowHandle = FindWindow(null, "云南省政务数据获取程序");
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }

        protected static void CloseConsole(object sender, ConsoleCancelEventArgs e)
        {
            Environment.Exit(0);
        }

        static log4net.ILog log = log4net.LogManager.GetLogger("logData");
        static string isRunDay = ConfigurationManager.AppSettings["isRunDay"].ToString();
        static string isRunHour = ConfigurationManager.AppSettings["isRunHour"].ToString();
        static int startHour = int.Parse(ConfigurationManager.AppSettings["startHour"]);
        static int startMinute = int.Parse(ConfigurationManager.AppSettings["startMinute"]);

        static int isGetProvinceInvestmentPlatform = int.Parse(ConfigurationManager.AppSettings["isGetProvinceInvestmentPlatform"]);

        static void Main(string[] args)
        {
            Console.Title = "云南省政务数据获取程序";
            closebtn();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CloseConsole);

            //if ("1".Equals(isRunDay))
            //{
            //    Thread pengjiang_RunDayThread = new Thread(new ThreadStart(RunDay));
            //    pengjiang_RunDayThread.Start();
            //    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 已开启按天同步线程");
            //}

            //if ("1".Equals(isRunHour))
            //{
            //    Thread pengjiang_RunHourThread = new Thread(new ThreadStart(RunHour));
            //    pengjiang_RunHourThread.Start();
            //    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 已开启按小时同步线程");
            //}

            if (isGetProvinceInvestmentPlatform == 1)
            {
                GetProvinceInvestmentPlatform.TestGetMyDb();
                _provinceInvestmentPlatform.OnAllSuccess += _provinceInvestmentPlatform_OnAllSuccess;
                _provinceInvestmentPlatform.Start();


                //测试530100获取部门
                //IGetProvinceInvestmentPlatform Services = new ProvinceInvestmentPlatform.Services.GetProvinceInvestmentPlatform();
                //string ss = Services.GetDeptInfoByCode("530100");

                //材料拉取测试
                //IGetProvinceInvestmentPlatform Services = new ProvinceInvestmentPlatform.Services.GetProvinceInvestmentPlatform();
                //string clStr = Services.GetMaterialInfoByProc("570D91B5CA31431BA1829B076DD7364D");

            }

            Console.ReadLine();
        }

        private static void _provinceInvestmentPlatform_OnAllSuccess()
        {
            Console.WriteLine("全部省级事项同步完成！");
            //_provinceInvestmentPlatform.Start();
        }

        #region 公用方法
        /// <summary>
        /// 按天跑
        /// </summary>
        public static void RunDay()
        {
            while (true)
            {
                try
                {
                    if (DateTime.Now.Hour == startHour)
                    {

                        Thread.Sleep(72000000);//20小时
                    }
                }
                catch (Exception ex)
                {
                    log.Warn("RunDay()：" + ex.Message);
                }
                finally
                {
                    Thread.Sleep(1800000);//半小时
                }
            }
        }

        /// <summary>
        /// 按小时跑
        /// </summary>
        public static void RunHour()
        {
            while (true)
            {
                try
                {
                    if (DateTime.Now.Minute == startMinute)
                    {

                        Thread.Sleep(1800000);//30分钟
                    }
                }
                catch (Exception ex)
                {
                    log.Warn("RunHour()：" + ex.Message);
                }
                finally
                {
                    Thread.Sleep(30000);//30秒
                }
            }
        }

        #endregion


    }
}
