using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Topshelf;
using Quartz;
using Quartz.Impl;
using System.IO;
using System.IO.Compression;
using System.Configuration;
using log4net.Layout;
using System.Xml;
using log4net.Core;
using log4net.Config;
using log4net;
using System.Data;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Net.Mail;
using EWBS;
using ComInterface;
using EwbsCore.Baggage;
using FlightDataBase;
using System.DirectoryServices;
using System.Net;
using CoreDataBaseLibrary;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography;
using File = System.IO.File;

namespace CustomJob
{
    class Program
    {
        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        const uint ENABLE_INSERT_MODE = 0x0020;
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int hConsoleHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

        private string sMailList = ConfigurationManager.AppSettings["mailList"].ToString(); // 須通知的人員名單
        private string sEmailServer = ConfigurationManager.AppSettings["Email_Server"].ToString();

        public static void DisbleQuickEditMode()
        { //移除快速編輯模式
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(hStdin, mode);
        }

        public static void DisbleQuickInsertMode()
        { //移除插入模式
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode &= ~ENABLE_INSERT_MODE;     
            SetConsoleMode(hStdin, mode);
        }

        static void Main(string[] args)
        {
            DisbleQuickEditMode();//移除快速編輯模式
            DisbleQuickInsertMode();//移除插入模式

            //Topshelf 設定
            HostFactory.Run(x =>
            {
                x.Service<MainService>(s =>
                {
                    s.ConstructUsing(name => new MainService());
                    s.WhenStarted(ms => ms.Start());
                    s.WhenStopped(ms => ms.Stop());
                });

                x.SetServiceName("JobBatchService");
                x.SetDisplayName("Job");
                x.SetDescription("Job");
                x.RunAsLocalSystem();
                x.StartAutomatically();
            });
        }
    }

    class MainService
    {
        private IScheduler _scheduler;

        public MainService()
        {
            #region trigger 設定觸發時間、次數等

            //BaggageReportJob 觸發設定 trigger 
            string BaggageTriggerResult = ConfigurationManager.AppSettings["Baggage_Trigger"];
            ITrigger baggageRrport_trigger = TriggerBuilder.Create()
                .WithIdentity("BaggageReportTrigger", "MainGroup")
                //.WithCronSchedule("0/15 * * * * ? *") 
                .WithCronSchedule(BaggageTriggerResult) 
                .StartAt(DateTime.Now) 
                .WithPriority(1) //優先權
                .Build();

            //BaggageReportJob 觸發設定 trigger (run once time)
            ITrigger baggageReport_trigger_once = TriggerBuilder.Create()
            .WithDescription("Once")
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(20)
            .RepeatForever()
            .WithRepeatCount(0))
            .StartAt(DateBuilder.DateOf(01, 01, 01, 1, 1, 2016))
            .Build();

            //AdCheckJob 觸發設定 trigger 
            string AdCheckTriggerResult = ConfigurationManager.AppSettings["AdCheck_Trigger"];

            ITrigger AdCheck_trigger = TriggerBuilder.Create()
                .WithIdentity("AdCheckTrigger", "MainGroup")
                //.WithCronSchedule("0/15 * * * * ? *") 
                .WithCronSchedule(AdCheckTriggerResult)
                .StartAt(DateTime.Now)
                .WithPriority(1) //優先權
                .Build();

            //AdCheckJob 觸發設定 trigger (run once time)
            ITrigger AdCheck_trigger_once = TriggerBuilder.Create()
            .WithDescription("Once")
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(20)
            .RepeatForever()
            .WithRepeatCount(0))
            .StartAt(DateBuilder.DateOf(01, 01, 01, 1, 1, 2016))
            .Build();


            //EwbsWebAuthSyncJob 觸發設定 trigger 
            string EwbsWebAuthSyncTriggerResult = ConfigurationManager.AppSettings["EwbsWebAuthSync_Trigger"];

            ITrigger EwbsWebAuthSync_trigger = TriggerBuilder.Create()
                .WithIdentity("EwbsWebAuthSyncTrigger", "MainGroup")
                //.WithCronSchedule("0/15 * * * * ? *") 
                .WithCronSchedule(EwbsWebAuthSyncTriggerResult)
                .StartAt(DateTime.Now)
                .WithPriority(1) //優先權
                .Build();

            //EwbsWebAuthSyncJob 觸發設定 trigger (run once time)
            ITrigger EwbsWebAuthSync_trigger_once = TriggerBuilder.Create()
            .WithDescription("Once")
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(20)
            .RepeatForever()
            .WithRepeatCount(0))
            .StartAt(DateBuilder.DateOf(01, 01, 01, 1, 1, 2016))
            .Build();



            //HourseKeeping 觸發設定 trigger (every 1 minute)
            string HouseKeepinTriggerResult = ConfigurationManager.AppSettings["HouseKeeping_Trigger"];
            ITrigger houseKeeping_trigger = TriggerBuilder.Create()
                .WithIdentity("HouseKeepingTrigger", "MainGroup")
                //.WithCronSchedule("30/0 * * * * ? *")
                .WithCronSchedule(HouseKeepinTriggerResult)
                .StartAt(DateTime.Now)
                .WithPriority(2) //優先權
                .Build();

            //HourseKeeping 觸發設定 trigger (run once time)
            ITrigger houseKeeping_trigger_once = TriggerBuilder.Create()
            .WithDescription("Once")
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(20)
            .RepeatForever()
            .WithRepeatCount(0))
            .StartAt(DateBuilder.DateOf(01, 01, 01, 1, 1, 2016))
            .Build();

            //FileMonitor
            string FileMonitor_TriggerResult = ConfigurationManager.AppSettings["FileMonitor_Trigger"];
            //觸發設定 trigger (every 1 miunte)
            ITrigger fileMonitor_Trigger = TriggerBuilder.Create()
                .WithIdentity("FileMonitorTrigger", "MainGroup")
                //.WithCronSchedule("0/15 * * * * ? *") 
                .WithCronSchedule(FileMonitor_TriggerResult)
                .StartAt(DateTime.Now)
                .WithPriority(1) //優先權
                .Build();

            // 觸發設定 trigger (run once time)
            ITrigger fileMonitor_Trigger_once = TriggerBuilder.Create()
                .WithDescription("Once")
                .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(20)
                .RepeatForever()
                .WithRepeatCount(0))
                .StartAt(DateBuilder.DateOf(01, 01, 01, 1, 1, 2016))
                .Build();

            #region 範例
            // delete file job 觸發設定 trigger (fire every 10 seconds)
            ITrigger del_trigger = TriggerBuilder.Create()
                .WithIdentity("DeleteFileTrigger", "MainGroup")
                .WithCronSchedule("0/5 * * * * ? *")
                .StartAt(DateTime.Now)
                .WithPriority(1)
                .Build();

            //  zip file job 觸發設定 trigger (fire every 15 seconds)
            ITrigger zip_trigger = TriggerBuilder.Create()
                .WithIdentity("ZipFileTrigger", "MainGroup")
                .WithCronSchedule("0/15 * * * * ? *")
                .StartAt(DateTime.Now)
                .WithPriority(1)
                .Build();

            //  delete file job 觸發設定 trigger (run once time)
            ITrigger del_trigger_once = TriggerBuilder.Create()
            .WithDescription("Once")
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(20)
            .RepeatForever()
            .WithRepeatCount(0))
            .StartAt(DateBuilder.DateOf(01, 01, 01, 1, 1, 2016))
            .Build();

            // zip file  job 觸發設定 trigger (run once time)
            ITrigger zip_trigger_once = TriggerBuilder.Create()
            .WithDescription("Once")
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(10)
                .RepeatForever()
                .WithRepeatCount(0))
                .StartAt(DateBuilder.DateOf(01, 01, 01, 1, 1, 2016))
                .Build();
            #endregion 範例

            #endregion

            #region Job 設定
                               
            // create  job
            IJobDetail baggageReport_job = JobBuilder.Create<BaggageReport_Job>()
                .WithIdentity("BaggageReport", "MainGroup")
                .Build();

            IJobDetail AdCheck_job = JobBuilder.Create<AdCheck_Job>()
                .WithIdentity("AdCheck", "MainGroup")
                .Build();

            IJobDetail EwbsWebAuthSync_job = JobBuilder.Create<EwbsWebAuthSync_Job>()
                .WithIdentity("EwbsWebAuthSync", "MainGroup")
                .Build();

            // create delete file  job
            IJobDetail houseKeeping_job = JobBuilder.Create<HouseKeeping_Job>()
                .WithIdentity("HouseKeepingJob", "MainGroup")
                .Build();

            // create delete file  job
            IJobDetail del_job = JobBuilder.Create<Delete_Job>()
                .WithIdentity("DeleteFileJob", "MainGroup")
                .Build();

            // create zip file  job
            IJobDetail zip_job = JobBuilder.Create<Compress_Job>()
                .WithIdentity("ZipFileJob", "MainGroup")
                .Build();

            IJobDetail fileMonitor_job = JobBuilder.Create<FileMonitor_Job>()
                .WithIdentity("FileMonitorJob", "MainGroup")
                .Build();
            #endregion

            #region Scheduler 排成啟用設定
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
           
             //讀取config 設定開關  ON: enable OFF: disable
            string BaggageJobFlag = ConfigurationManager.AppSettings["BaggageJobEnable"];
            string AdCheckJobFlag = ConfigurationManager.AppSettings["AdCheckJobEnable"];
            string EwbsWebAuthSyncJobFlag = ConfigurationManager.AppSettings["EwbsWebAuthSyncJobEnable"];
            string HousekeepingJobFlag = ConfigurationManager.AppSettings["HousekeepingEnable"];
            string DeleteJobEnable = ConfigurationManager.AppSettings["DeleteEnable"];
            string CompressJobEnable = ConfigurationManager.AppSettings["CompressEnable"];

            if (string.Equals(BaggageJobFlag, "ON"))
            {
                string mode = ConfigurationManager.AppSettings["BaggageJobMode"].ToString();
                if (mode == "ONCE")
                {
                    _scheduler.ScheduleJob(baggageReport_job, baggageReport_trigger_once); //只執行一次
                }
                else
                {
                    _scheduler.ScheduleJob(baggageReport_job, baggageRrport_trigger);
                }
            }

            if (string.Equals(AdCheckJobFlag, "ON"))
            {
                string mode = ConfigurationManager.AppSettings["AdCheckJobMode"].ToString();
                if (mode == "ONCE")
                {
                    _scheduler.ScheduleJob(AdCheck_job, AdCheck_trigger_once); //只執行一次
                }
                else
                {
                    _scheduler.ScheduleJob(AdCheck_job, AdCheck_trigger);
                }
            }

            if (string.Equals(EwbsWebAuthSyncJobFlag, "ON"))
            {
                string mode = ConfigurationManager.AppSettings["EwbsWebAuthSyncJobMode"].ToString();
                if (mode == "ONCE")
                {
                    _scheduler.ScheduleJob(EwbsWebAuthSync_job, EwbsWebAuthSync_trigger_once); //只執行一次
                }
                else
                {
                    _scheduler.ScheduleJob(EwbsWebAuthSync_job, EwbsWebAuthSync_trigger);
                }
            }

            if (string.Equals(HousekeepingJobFlag, "ON"))
            {
                _scheduler.ScheduleJob(houseKeeping_job, houseKeeping_trigger);
                //_scheduler.ScheduleJob(houseKeeping_job, houseKeeping_trigger_once);//只執行一次
            }

            if (string.Equals(DeleteJobEnable, "ON"))
            {
                _scheduler.ScheduleJob(del_job, del_trigger_once);
            }
                           
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();

            if (string.Equals(CompressJobEnable, "ON"))
            {
                _scheduler.ScheduleJob(zip_job, zip_trigger_once);
            }

            string FileMonitorJobFlag = ConfigurationManager.AppSettings["FileMonitorJobEnable"];
            if (string.Equals(FileMonitorJobFlag, "ON"))
            {
                _scheduler.ScheduleJob(fileMonitor_job, fileMonitor_Trigger);
            }
            else if (string.Equals(FileMonitorJobFlag, "ONCE"))
            {
                _scheduler.ScheduleJob(fileMonitor_job, fileMonitor_Trigger_once);
            }
            #endregion
        }

        /// <summary>
        /// 排程啟動
        /// </summary>
        public void Start()
        {
            _scheduler.Start();
        }

        /// <summary>
        /// 排程停止
        /// </summary>
        public void Stop()
        {
            _scheduler.Shutdown();
        }
    }

    [DisallowConcurrentExecution]
    public class BaggageReport_Job : IJob
    {
        //private CoreRetrieve objCoreRetrieveTelex = new CoreRetrieve();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                BaggageReport();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void BaggageReport()
        {
             string mode = ConfigurationManager.AppSettings["BaggageJobMode"].ToString();
             if (mode == "ONCE")
             {
                 string yyyyMM = ConfigurationManager.AppSettings["BaggageYMD"].ToString();
                 BagStatToTxT(yyyyMM);
             }
             else
             {
                 string yyyyMM = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                 BagStatToTxT(yyyyMM);
             }
        }

        public string BagStatToTxT(string yyyyMM)
        {
            FDB.Instance.Init(); // FDB initialization
            string stat = "";
            string Path = ConfigurationManager.AppSettings["BasePath"] + "\\ARCHIVE" + "\\" + yyyyMM; // Save a portion of path of Flight object (directroy of monthly data)
            string txtPath = Path + "\\" + yyyyMM + ".txt";

            Flight theFlight;
            SrvBaggageStatistic.Instance.Clear(); // Clean up SrvBaggageStatistic object content

            if (!Directory.Exists(Path)) // no such path 
            {
                Console.WriteLine("yyyyMM or Directory Path Error");
                return stat;
            }

            try
            {
                string msg = "baggage report " + yyyyMM + " gen start /create dttm:" + DateTime.Now.ToString();
                Console.WriteLine(msg);
                //this.Log("BagStatToTxT", msg);
                foreach (string dirName in Directory.GetDirectories(Path))
                { // daily directroy of monthly directroy
                    foreach (string fileName in Directory.GetFiles(dirName))
                    { // Flight object file in the daily directroy 
                        if (fileName.Substring((fileName.Length - 21), 1) == "\\")	
                        {															
                            if (File.Exists(fileName))
                            { //  file exists						
                                theFlight = DirService.Deserialize(fileName) as Flight; // Get Flight object of the file
                                if (!string.IsNullOrEmpty(theFlight.Telex.LS.Text)) //判斷 是否已經產生 LS ，有才列入BAG計算
                                {
                                    SrvBaggageStatistic.Instance.Add(theFlight); // add the Flight object into SrvBaggageStatistic object
                                    Console.WriteLine(fileName + "---OK");
                                }
                            }
                        }															
                    }
                }
            }
            catch (Exception e)
            {
                string eMag = e.Message;
                Console.WriteLine("output report fail ---" + eMag);
                //this.Log("BagStatToTxT", eMag);
            }
            DateTime s = new DateTime(Convert.ToInt32(yyyyMM.Substring(0, 4)), Convert.ToInt32(yyyyMM.Substring(4, 2)), 1);

            stat = SrvBaggageStatistic.Instance.Report(new DateTime(Convert.ToInt32(yyyyMM.Substring(0, 4)), Convert.ToInt32(yyyyMM.Substring(4, 2)), 1));

            try
            {
                StreamWriter file = new StreamWriter(Path + "\\" + yyyyMM + ".txt");
                file.WriteLine(stat);
                file.Close();
            }
            catch (Exception ex)
            {
                string exMag = ex.Message;
                Console.WriteLine("報表產生失敗 ---" + exMag);
            }

            string msg1 = "baggage report " + yyyyMM + " gen completed /create dttm:" + DateTime.Now.ToString();
            Console.WriteLine(msg1);
            return stat;
        } 
    }


    [DisallowConcurrentExecution]
    public class AdCheck_Job : IJob
    {
        //private CoreRetrieve objCoreRetrieveTelex = new CoreRetrieve();
        ClsAES256EncData.ClsAES256EncData AES256 = new ClsAES256EncData.ClsAES256EncData();
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                AdCheck();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void AdCheck()
        {
            //FDB.Instance.Init(); // FDB initialization

            accountUser[] AllUserList = FDB.Instance.GetAllUser();
            List<UserData> UserList = new List<UserData>();

            string sSendto = ConfigurationManager.AppSettings["Sendto"];

            string sSubject = "EWBS權限檢查(QA)";
            if (CheckSameMask())
            {
                sSubject = "EWBS權限檢查";
            }

            string sSender = "EWBS";
            SendComponent send = new SendComponent();


            foreach (accountUser user in AllUserList)
            {
                if (user.username == "")
                {
                    continue;
                }
                UserData u = new UserData();
                u.ad = user.username; //AD 帳號
                u.station = user.work_at_station;
                u.fullname = user.fullname; //參數代碼 
                u.auth = user.authority.ToString();
                UserList.Add(u);
            }



            string sysname = ConfigurationManager.AppSettings["ADSysID"]; // "844249";
            string syspwd = AES256.DecryptString(ConfigurationManager.AppSettings["ADSysCode"]); // ConfigurationManager.AppSettings["ADSysCode"];  //"Xxx33333XxxSyg";

            //string sysname = "E03014"; 
            //string syspwd ="ji394su3ji394su300~"; 


            List<UserData> nonAllList =  IsAuthenticated( sysname, syspwd, UserList);
            List<UserData>uniAllList = GetUniAdList(sysname, syspwd, UserList);
            Regex EG = new Regex("[^A-Za-z]");
            string sText = "";

            //非在職人員

            Log("---------------------------------------", "1");
            Log(DateTime.Now.ToString(), "1");




            sText += "<font size = '3'>";
            sText += "請管理單位修正EWBS權限，人員新增及調離職人員刪除，謝謝。" + "<br>";
            sText += "一、離職(留停)之人員如下：" + "<br>";
            int no = 1;
            List<UserData> nonList = new List<UserData>();
            foreach (UserData user in nonAllList)
            {
                if(EG.IsMatch(user.ad.Substring(0,3)))
                {
                    nonList.Add(user);
                    //sText += no.ToString() + ". " + "AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad + ", " + "NM = " + user.fullname  + " <br>";
                    sText += no.ToString() + ". " + "AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad + " <br>";
                    //Log("AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad + ", " + "NM = " + user.fullname , "1");
                    Log("AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad , "1");

                    no ++;
                }
            }

            //系統帳號 (前三碼為英文字母)
            Log("---------------------------------------", "2");
            Log(DateTime.Now.ToString(), "2");

            sText += "<br>";
            sText += "二、非長榮航空單位之人員，請確認下述人員是否仍在職：" + "<br>";
            no = 1;
            List<UserData> sysList = new List<UserData>();
            foreach (UserData user in nonAllList)
            {
                if (!EG.IsMatch(user.ad.Substring(0, 3)))
                {
                    sysList.Add(user);
                    
                    //sText += no.ToString() + ". " + "AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad + ", " + "NM = " + user.fullname + " <br>";
                    //Log("AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad + ", " + "NM = " + user.fullname, "2");

                    sText += no.ToString() + ". " + "AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad + " <br>";
                    Log("AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad, "2");

                    no ++;
                }
            }



            //立榮在職人員

            Log("---------------------------------------", "1");
            Log(DateTime.Now.ToString(), "1");

            sText += "<br>";
            sText += "三、立榮在職之人員如下：" + "<br>";
            no = 1;
            List<UserData> uniList = new List<UserData>();
            foreach (UserData user in uniAllList)
            {
                if (EG.IsMatch(user.ad.Substring(0, 3)))
                {
                    uniList.Add(user);
                    //sText += no.ToString() + ". " + "AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad + ", " + "NM = " + user.fullname + " <br>";
                    //Log("AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad + ", " + "NM = " + user.fullname, "1");

                    sText += no.ToString() + ". " + "AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad + " <br>";
                    Log("AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad, "1");

                    no++;
                }
            }

            sText += "</font>";



            //send.SendMail("QAIS", "QAIS", "yenneyhuang@evaair.com", sSubject, sText, 0);   //測試用，發信都發給Yenney
            //send.SendMail("QAIS", "QAIS", sSendto, sSubject, sText, 0);
            send.SendMailHTML(sSender, sSender, sSendto, sSubject, sText, 0);

        }

        public List<UserData> GetUniAdList(string username, string pwd, List<UserData> userlist)
        {
            List<UserData> UserList = new List<UserData>();
            string domainAndUsername2 = "EVAAIR" + @"\" + username;
            string _path2 = "LDAP://" + "uniair.com.tw";
            DirectoryEntry entry2 = new DirectoryEntry(_path2, domainAndUsername2, pwd);
            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj2 = entry2.NativeObject;
                DirectorySearcher search2 = new DirectorySearcher(entry2);

                foreach (UserData sAd in userlist)
                {

                    search2.Filter = "(&(sAMAccountName=" + sAd.ad + "))";
                    search2.PropertiesToLoad.Add("cn");
                    search2.PropertiesToLoad.Add("mail");

                    SearchResult sResult2 = search2.FindOne();

                    if (sResult2 != null)
                    {
                        Regex r = new Regex(",");
                        MatchCollection mc;
                        string tempDivision = "";
                        mc = r.Matches(sResult2.Path);
                        string[] adpath = sResult2.Path.Split(',');

                        //部門資訊
                        for (int i = 0; i <= mc.Count; i++)
                        {
                            if (adpath[i].IndexOf("OU=") >= 0)
                            {
                                tempDivision = adpath[i].Substring(adpath[i].IndexOf("OU=") + 3) + "/" + tempDivision;
                            }
                        }

                        if (tempDivision.Contains("SUSPEND"))
                        {
                            sResult2 = null;
                        }
                    }

                    if ( sResult2 != null)
                    {
                        UserList.Add(sAd);
                    }
                }


            }
            catch (Exception ex)
            {
                string sSendto = ConfigurationManager.AppSettings["Sendto"];

                string sSubject = "EWBS權限檢查(QA)";
                if (CheckSameMask())
                {
                    sSubject = "EWBS權限檢查";
                }

                string sSender = "EWBS";
                SendComponent send = new SendComponent();

                send.SendMailHTML(sSender, sSender, sSendto, sSubject, ex.Message, 0);

                throw new Exception("Error authenticating user. " + ex.Message);

            }
            return UserList;
        }

        public List<UserData> IsAuthenticated(string username, string pwd, List<UserData> userlist )
        {
            List<UserData> nonUserList = new List<UserData>();
            string domainAndUsername1 = "EVAAIR" + @"\" + username;
            string domainAndUsername2 = "EVAAIR" + @"\" + username;
            string _path1 = "LDAP://" + "EVAAIR";
            string _path2 = "LDAP://" + "uniair.com.tw";
            DirectoryEntry entry1 = new DirectoryEntry(_path1, domainAndUsername1, pwd);
            DirectoryEntry entry2 = new DirectoryEntry(_path2, domainAndUsername2, pwd);
            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj1 = entry1.NativeObject;
                object obj2 = entry2.NativeObject;
                DirectorySearcher search1 = new DirectorySearcher(entry1);
                DirectorySearcher search2 = new DirectorySearcher(entry2);

                foreach (UserData sAd in userlist)
                {
                    search1.Filter = "(&(sAMAccountName=" + sAd.ad + "))";
                    search1.PropertiesToLoad.Add("cn");
                    search1.PropertiesToLoad.Add("mail");
                    SearchResult sResult1 = search1.FindOne();

                    if (sResult1 != null)
                    {
                        Regex r = new Regex(",");
                        MatchCollection mc;
                        string tempDivision = "";
                        mc = r.Matches(sResult1.Path);
                        string[] adpath = sResult1.Path.Split(',');

                        //部門資訊
                        for (int i = 0; i <= mc.Count; i++)
                        {
                            if (adpath[i].IndexOf("OU=") >= 0)
                            {
                                tempDivision = adpath[i].Substring(adpath[i].IndexOf("OU=") + 3) + "/" + tempDivision;
                            }
                        }

                        if (tempDivision.Contains("SUSPEND"))
                        {
                            sResult1 = null;
                        }
                    }
                    
                    search2.Filter = "(&(sAMAccountName=" + sAd.ad + "))";
                    search2.PropertiesToLoad.Add("cn");
                    search2.PropertiesToLoad.Add("mail");
                    SearchResult sResult2 = search2.FindOne();

                    if (sResult2 != null)
                    {
                        Regex r = new Regex(",");
                        MatchCollection mc;
                        string tempDivision = "";
                        mc = r.Matches(sResult2.Path);
                        string[] adpath = sResult2.Path.Split(',');

                        //部門資訊
                        for (int i = 0; i <= mc.Count; i++)
                        {
                            if (adpath[i].IndexOf("OU=") >= 0)
                            {
                                tempDivision = adpath[i].Substring(adpath[i].IndexOf("OU=") + 3) + "/" + tempDivision;
                            }
                        }

                        if (tempDivision.Contains("SUSPEND"))
                        {
                            sResult2 = null;
                        }
                    }

                    if (sResult1 == null && sResult2 == null)
                    {
                        nonUserList.Add(sAd);
                    }
                }


            }
            catch (Exception ex)
            {
                string sSendto = ConfigurationManager.AppSettings["Sendto"];

                string sSubject = "EWBS權限檢查(QA)";
                if (CheckSameMask())
                {
                    sSubject = "EWBS權限檢查";
                }

                string sSender = "EWBS";
                SendComponent send = new SendComponent();

                send.SendMailHTML(sSender, sSender, sSendto, sSubject, ex.Message, 0);

                throw new Exception("Error authenticating user. " + ex.Message);

            }
            return nonUserList;
        }

        public List<AD_Info> IsAuthenticatedAll(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + @"\" + username;
            string _path = "LDAP://" + "EVAAIR";
            //string _path = "LDAP://EVAAIR/OU=OMD,DC=EVAAIR,DC=COM";
            //string _path = "LDAP://EVAAIR/DC=EVAAIR,DC=COM";
            List<AD_Info> AdList = new List<AD_Info>();
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);
            try
            {
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(&(objectCategory=person)(objectClass=user)(sAMAccountName=*))";
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("displayname");
                search.PropertiesToLoad.Add("department");
                search.PropertiesToLoad.Add("givenname");
                search.PropertiesToLoad.Add("sn");
               

                SearchResultCollection sResult = search.FindAll();
                if (sResult != null)
                {
                    foreach (SearchResult result in sResult)
                    {
                        AD_Info AdInfo = new AD_Info();
                        AdInfo.AD = result.Properties["samaccountname"][0].ToString();
                        AdInfo.AD_CN = result.Properties["sn"][0].ToString() + result.Properties["givenname"][0].ToString();
                        //AdInfo.Divisoin = result.Properties["department"][0].ToString();
                        //Console.WriteLine(i+"."+result.Properties["displayname"][0]);
                        //Console.WriteLine(i+"."+result.Properties["department"][0]);
                        //Console.WriteLine(i+"."+result.Properties["cn"][0]);
                        AdList.Add(AdInfo);

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user. " + ex.Message);
            }

            return AdList;
        }

        public bool CheckSameMask()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string mask = ConfigurationManager.AppSettings["IpMask"];
            foreach (var ip in host.AddressList)
            {
                if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    if(ip.ToString().Contains(mask))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Log(string Msg, string Type)
        {
            string LogFilePath = "";
            switch (Type)
            {
                case "1":
                    LogFilePath = System.Environment.CurrentDirectory + @"\Log\" + DateTime.Now.ToString("yyyy_MM_dd") + "_USER.txt";
                    break;
                case "2":
                    LogFilePath = System.Environment.CurrentDirectory + @"\Log\" + DateTime.Now.ToString("yyyy_MM_dd") + "_SYS.txt";
                    break;
                default:
                    break;
            }

            StreamWriter sw = File.AppendText(LogFilePath); //建立檔案
            //sw.WriteLine(DateTime.Now.ToString());
            sw.WriteLine(Msg);
            //sw.WriteLine("-------------------------------------");
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public class AD_Info
        {
            public string AD { get; set; }
            public string Divisoin { get; set; }
            public string AD_CN { get; set; }
        }
        public class UserData
        {
            public string ad { get; set; }
            public string station { get; set; }

            public string auth { get; set; }
            public string fullname { get; set; }

            public UserData()
            {
                ad = string.Empty; auth = string.Empty; station = string.Empty; fullname = string.Empty;
            }
        }
    }


    [DisallowConcurrentExecution]
    public class EwbsWebAuthSync_Job : IJob
    {
        //private CoreRetrieve objCoreRetrieveTelex = new CoreRetrieve();
        ClsAES256EncData.ClsAES256EncData AES256 = new ClsAES256EncData.ClsAES256EncData();
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                AuthSync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void AuthSync()
        {
            //FDB.Instance.Init(); // FDB initialization
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " EWBS WEB 使用者帳號同步開始...");
            accountUser[] AllUserList = FDB.Instance.GetAllUser();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 讀取Account.xml...");
            List<UserData> UserListSrc = new List<UserData>();
            List<UserData> UserListTar = new List<UserData>();
            List<UserData> ResultList = new List<UserData>();

            string sSendto = ConfigurationManager.AppSettings["Sendto"];

            string sSubject = "EWBS權限帳號同步EWBS WEB";
            string sSender = "EWBS";
            SendComponent send = new SendComponent();


            foreach (accountUser user in AllUserList)
            {
                if (user.username == "")
                {
                    continue;
                }
                UserData u = new UserData();
                u.ad = user.username; //AD 帳號
                u.station = user.work_at_station;
                u.fullname = user.fullname; //參數代碼 
                u.auth = "10000000";//user.authority.ToString();
                UserListSrc.Add(u);
            }

            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 讀取EWBS WEB EWBS_USER_CONFIG...");
            StringBuilder sSQL = new StringBuilder();
            Dictionary<string, object> sParaDict = new Dictionary<string, object>();
            sSQL.AppendLine("SELECT * FROM EWBS_USER_CONFIG");

            string ewbsConnString = ConfigurationManager.AppSettings["EWBSConn"].ToString();                 
            DataTable dtEWBS_USER_CONFIG = new DataTable();
            CoreDatabase objCoreRetrieve = new CoreDatabase();
            objCoreRetrieve.doConnectDB(ewbsConnString);
            dtEWBS_USER_CONFIG = objCoreRetrieve.doParameterDataTableFill(sSQL.ToString(), sParaDict);    
            dtEWBS_USER_CONFIG.TableName = "dtEWBS_USER_CONFIG";
            objCoreRetrieve.doDisconnectDB();

            foreach (DataRow item in dtEWBS_USER_CONFIG.Rows)
            {
                UserData u = new UserData();
                u.ad = item["USER_ID"].ToString(); //AD 帳號
                u.station = item["USER_STN"].ToString();
                u.fullname = item["USER_NAME"].ToString(); //參數代碼 
                UserListTar.Add(u);
            }

            //比對差異
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 比對差異...");
            var AddUserList = UserListSrc.Except(UserListTar, new UsuerComparer()).ToList();
            var DelUserList = UserListTar.Except(UserListSrc, new UsuerComparer()).ToList();

            objCoreRetrieve.doConnectDB(ewbsConnString);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 刪除" + DelUserList.Count() + "筆帳號...");
            foreach (UserData user in DelUserList)
            {
                UserData u = new UserData();
                try
                {
                    sSQL.Clear();
                    sParaDict.Clear();

                    sSQL.AppendLine("DELETE FROM EWBS_USER_CONFIG WHERE 1=1 AND USER_ID = :USERID");

                    sParaDict.Add("USER_ID", user.ad);

                    int result = objCoreRetrieve.doParameterSqlExecution(sSQL.ToString(), sParaDict);
                    if (result > 0)
                    {
                        u.ad = user.ad;
                        u.station = user.station;
                        u.fullname = user.fullname;
                        u.Type = "DEL";
                        u.Result = "Success";
                    }
                    else
                    {
                        u.ad = user.ad;
                        u.station = user.station;
                        u.fullname = user.fullname;
                        u.Type = "DEL";
                        u.Result = "Fail";
                    }
                    ResultList.Add(u);
                }
                catch (Exception ex)
                {
                    u.ad = user.ad;
                    u.station = user.station;
                    u.fullname = user.fullname;
                    u.Type = "DEL";
                    u.Result = ex.Message.ToString();
                    ResultList.Add(u);
                }
            }
            objCoreRetrieve.doDisconnectDB();

            objCoreRetrieve.doConnectDB(ewbsConnString);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 新增" + AddUserList.Count() + "筆帳號...");
            foreach (UserData user in AddUserList)
            {
                UserData u = new UserData();


                try
                {
                    sSQL.Clear();
                    sParaDict.Clear();

                    sSQL.AppendLine("INSERT INTO EWBS_USER_CONFIG (USER_ID, USER_NAME, USER_AUTH, USER_STN, CREATE_USER, CREATE_TIME) VALUES (:USER_ID, :USER_NAME, :USER_AUTH, :USER_STN, :CREATE_USER, CURRENT_TIMESTAMP )");  //#BR22053 修改SQL

                    sParaDict.Add("USER_ID", user.ad);
                    sParaDict.Add("USER_NAME", user.fullname);
                    sParaDict.Add("USER_AUTH", user.auth);
                    sParaDict.Add("USER_STN", user.station);
                    sParaDict.Add("CREATE_USER", "EWBS");


                    int result = objCoreRetrieve.doParameterSqlExecution(sSQL.ToString(), sParaDict);
                    if(result > 0 )
                    {
                        u.ad = user.ad; 
                        u.station = user.station;
                        u.fullname = user.fullname;
                        u.Type = "ADD";
                        u.Result = "Success";
                    }
                    else
                    {
                        u.ad = user.ad;
                        u.station = user.station;
                        u.fullname = user.fullname;
                        u.Type = "ADD";
                        u.Result = "Fail";
                    }
                    ResultList.Add(u);
                }
                catch (Exception ex)
                {
                    u.ad = user.ad;
                    u.station = user.station;
                    u.fullname = user.fullname;
                    u.Type = "ADD";
                    u.Result = ex.Message.ToString();
                    ResultList.Add(u);
                }
            }
            objCoreRetrieve.doDisconnectDB();

            //結果
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 同步完成...");
            string sText = "";

            sText += "<font size = '3'>";
            sText += "EWBS WEB 人員新增及刪除。" + "<br>";
            sText += "一、新增如下：" + "<br>";
            int no = 1;
            foreach (UserData user in ResultList)
            {
                if (user.Type =="ADD")
                {
                    sText += no.ToString() + ". " + "STN = " + user.station + ", " + "ID = " + user.ad + ", " + "結果 = " + user.Result + " <br>";
                    //Log("AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad, "1");

                    no++;
                }
            }

            ////系統帳號 (前三碼為英文字母)
            //Log("---------------------------------------", "2");
            //Log(DateTime.Now.ToString(), "2");

            sText += "<br>";
            sText += "二、刪除如下：" + "<br>";
            no = 1;
            //List<UserData> sysList = new List<UserData>();
            foreach (UserData user in ResultList)
            {
                if (user.Type == "DEL")
                {
                    sText += no.ToString() + ". " +  "STN = " + user.station + ", " + "ID = " + user.ad + ", " + "結果 = " + user.Result + " <br>";
                    //Log("AUTH = " + user.auth + ", " + "STN = " + user.station + ", " + "ID = " + user.ad, "2");
                    no++;
                }
            }
            send.SendMailHTML(sSender, sSender, sSendto, sSubject, sText, 0);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 產生Report...");
        }

        public List<UserData> GetUniAdList(string username, string pwd, List<UserData> userlist)
        {
            List<UserData> UserList = new List<UserData>();
            string domainAndUsername2 = "EVAAIR" + @"\" + username;
            string _path2 = "LDAP://" + "uniair.com.tw";
            DirectoryEntry entry2 = new DirectoryEntry(_path2, domainAndUsername2, pwd);
            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj2 = entry2.NativeObject;
                DirectorySearcher search2 = new DirectorySearcher(entry2);

                foreach (UserData sAd in userlist)
                {

                    search2.Filter = "(&(sAMAccountName=" + sAd.ad + "))";
                    search2.PropertiesToLoad.Add("cn");
                    search2.PropertiesToLoad.Add("mail");

                    SearchResult sResult2 = search2.FindOne();

                    if (sResult2 != null)
                    {
                        Regex r = new Regex(",");
                        MatchCollection mc;
                        string tempDivision = "";
                        mc = r.Matches(sResult2.Path);
                        string[] adpath = sResult2.Path.Split(',');

                        //部門資訊
                        for (int i = 0; i <= mc.Count; i++)
                        {
                            if (adpath[i].IndexOf("OU=") >= 0)
                            {
                                tempDivision = adpath[i].Substring(adpath[i].IndexOf("OU=") + 3) + "/" + tempDivision;
                            }
                        }

                        if (tempDivision.Contains("SUSPEND"))
                        {
                            sResult2 = null;
                        }
                    }

                    if (sResult2 != null)
                    {
                        UserList.Add(sAd);
                    }
                }


            }
            catch (Exception ex)
            {
                string sSendto = ConfigurationManager.AppSettings["Sendto"];

                string sSubject = "EWBS權限檢查(QA)";
                if (CheckSameMask())
                {
                    sSubject = "EWBS權限檢查";
                }

                string sSender = "EWBS";
                SendComponent send = new SendComponent();

                send.SendMailHTML(sSender, sSender, sSendto, sSubject, ex.Message, 0);

                throw new Exception("Error authenticating user. " + ex.Message);

            }
            return UserList;
        }

        public List<UserData> IsAuthenticated(string username, string pwd, List<UserData> userlist)
        {
            List<UserData> nonUserList = new List<UserData>();
            string domainAndUsername1 = "EVAAIR" + @"\" + username;
            string domainAndUsername2 = "EVAAIR" + @"\" + username;
            string _path1 = "LDAP://" + "EVAAIR";
            string _path2 = "LDAP://" + "uniair.com.tw";
            DirectoryEntry entry1 = new DirectoryEntry(_path1, domainAndUsername1, pwd);
            DirectoryEntry entry2 = new DirectoryEntry(_path2, domainAndUsername2, pwd);
            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj1 = entry1.NativeObject;
                object obj2 = entry2.NativeObject;
                DirectorySearcher search1 = new DirectorySearcher(entry1);
                DirectorySearcher search2 = new DirectorySearcher(entry2);

                foreach (UserData sAd in userlist)
                {
                    search1.Filter = "(&(sAMAccountName=" + sAd.ad + "))";
                    search1.PropertiesToLoad.Add("cn");
                    search1.PropertiesToLoad.Add("mail");
                    SearchResult sResult1 = search1.FindOne();

                    if (sResult1 != null)
                    {
                        Regex r = new Regex(",");
                        MatchCollection mc;
                        string tempDivision = "";
                        mc = r.Matches(sResult1.Path);
                        string[] adpath = sResult1.Path.Split(',');

                        //部門資訊
                        for (int i = 0; i <= mc.Count; i++)
                        {
                            if (adpath[i].IndexOf("OU=") >= 0)
                            {
                                tempDivision = adpath[i].Substring(adpath[i].IndexOf("OU=") + 3) + "/" + tempDivision;
                            }
                        }

                        if (tempDivision.Contains("SUSPEND"))
                        {
                            sResult1 = null;
                        }
                    }

                    search2.Filter = "(&(sAMAccountName=" + sAd.ad + "))";
                    search2.PropertiesToLoad.Add("cn");
                    search2.PropertiesToLoad.Add("mail");
                    SearchResult sResult2 = search2.FindOne();

                    if (sResult2 != null)
                    {
                        Regex r = new Regex(",");
                        MatchCollection mc;
                        string tempDivision = "";
                        mc = r.Matches(sResult2.Path);
                        string[] adpath = sResult2.Path.Split(',');

                        //部門資訊
                        for (int i = 0; i <= mc.Count; i++)
                        {
                            if (adpath[i].IndexOf("OU=") >= 0)
                            {
                                tempDivision = adpath[i].Substring(adpath[i].IndexOf("OU=") + 3) + "/" + tempDivision;
                            }
                        }

                        if (tempDivision.Contains("SUSPEND"))
                        {
                            sResult2 = null;
                        }
                    }

                    if (sResult1 == null && sResult2 == null)
                    {
                        nonUserList.Add(sAd);
                    }
                }


            }
            catch (Exception ex)
            {
                string sSendto = ConfigurationManager.AppSettings["Sendto"];

                string sSubject = "EWBS權限檢查(QA)";
                if (CheckSameMask())
                {
                    sSubject = "EWBS權限檢查";
                }

                string sSender = "EWBS";
                SendComponent send = new SendComponent();

                send.SendMailHTML(sSender, sSender, sSendto, sSubject, ex.Message, 0);

                throw new Exception("Error authenticating user. " + ex.Message);

            }
            return nonUserList;
        }

        public List<AD_Info> IsAuthenticatedAll(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + @"\" + username;
            string _path = "LDAP://" + "EVAAIR";
            //string _path = "LDAP://EVAAIR/OU=OMD,DC=EVAAIR,DC=COM";
            //string _path = "LDAP://EVAAIR/DC=EVAAIR,DC=COM";
            List<AD_Info> AdList = new List<AD_Info>();
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);
            try
            {
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(&(objectCategory=person)(objectClass=user)(sAMAccountName=*))";
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("displayname");
                search.PropertiesToLoad.Add("department");
                search.PropertiesToLoad.Add("givenname");
                search.PropertiesToLoad.Add("sn");


                SearchResultCollection sResult = search.FindAll();
                if (sResult != null)
                {
                    foreach (SearchResult result in sResult)
                    {
                        AD_Info AdInfo = new AD_Info();
                        AdInfo.AD = result.Properties["samaccountname"][0].ToString();
                        AdInfo.AD_CN = result.Properties["sn"][0].ToString() + result.Properties["givenname"][0].ToString();
                        //AdInfo.Divisoin = result.Properties["department"][0].ToString();
                        //Console.WriteLine(i+"."+result.Properties["displayname"][0]);
                        //Console.WriteLine(i+"."+result.Properties["department"][0]);
                        //Console.WriteLine(i+"."+result.Properties["cn"][0]);
                        AdList.Add(AdInfo);

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user. " + ex.Message);
            }

            return AdList;
        }

        public bool CheckSameMask()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string mask = ConfigurationManager.AppSettings["IpMask"];
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    if (ip.ToString().Contains(mask))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Log(string Msg, string Type)
        {
            string LogFilePath = "";
            switch (Type)
            {
                case "1":
                    LogFilePath = System.Environment.CurrentDirectory + @"\Log\" + DateTime.Now.ToString("yyyy_MM_dd") + "_USER.txt";
                    break;
                case "2":
                    LogFilePath = System.Environment.CurrentDirectory + @"\Log\" + DateTime.Now.ToString("yyyy_MM_dd") + "_SYS.txt";
                    break;
                default:
                    break;
            }

            StreamWriter sw = File.AppendText(LogFilePath); //建立檔案
            //sw.WriteLine(DateTime.Now.ToString());
            sw.WriteLine(Msg);
            //sw.WriteLine("-------------------------------------");
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public class AD_Info
        {
            public string AD { get; set; }
            public string Divisoin { get; set; }
            public string AD_CN { get; set; }
        }
        public class UserData
        {
            public string ad { get; set; }
            public string station { get; set; }

            public string auth { get; set; }
            public string fullname { get; set; }

            public string Type { get; set; }
            public string Result { get; set; }

            public UserData()
            {
                ad = string.Empty; auth = string.Empty; station = string.Empty; fullname = string.Empty; Type = string.Empty; Result = string.Empty;
            }
        }

       class UsuerComparer:IEqualityComparer<UserData>
       {
            public bool Equals(UserData x, UserData y)
            {
                if (object.ReferenceEquals(x, y)) return true;
                if (object.ReferenceEquals(x, null) || object.ReferenceEquals(null, y))
                    return false;
                return x.ad == y.ad && x.station == y.station;

            }

            public int GetHashCode(UserData obj)
            {
                if (object.ReferenceEquals(obj, null)) return 0;
                return obj.ad == null ? 0 : obj.ad.GetHashCode();
            }
       }

    }

    [DisallowConcurrentExecution]
    public class HouseKeeping_Job : IJob
    {
        //private CoreRetrieve objCoreRetrieveTelex = new CoreRetrieve();

        public void Execute(IJobExecutionContext context)
        {
            doHouseKeeping();
        }

        private void doHouseKeeping()
        {
        }

        /// <summary>
        /// output Xml format
        /// </summary>
        /// <param name="jobname"></param>
        /// <param name="filename"></param>
        private void Log(string Msg, string Type)
        {
            string LogFilePath = "";
            switch (Type)
            {
                case "1":
                     LogFilePath = System.Environment.CurrentDirectory + @"\Log\" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
                    break;
                case "2":
                     LogFilePath = System.Environment.CurrentDirectory + @"\Log\" + DateTime.Now.ToString("yyyy_MM_dd_Error") + ".txt";
                    break;
                default:
                    break;
            }

            StreamWriter sw = File.AppendText(LogFilePath); //建立檔案
            sw.WriteLine(DateTime.Now.ToString()); 
            sw.WriteLine(Msg);
            sw.WriteLine("-------------------------------------"); 
            sw.Flush(); 
            sw.Close(); 
            sw.Dispose();
        }
    }

    [DisallowConcurrentExecution]
    public class Delete_Job : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //Get delete Path
            string delete_path = ConfigurationManager.AppSettings["DeletePath"];
            if (System.IO.Directory.Exists(delete_path))
            {
                try
                {   
                    Console.WriteLine("開始刪除檔案");
                    string[] files = Directory.GetFiles(delete_path);
                    string fileList = "";
                    foreach (var item in files)
                    {
                        Console.WriteLine("刪除檔案  => " + item);
                        System.IO.File.Delete(item);
                        fileList = fileList + item + "\n";
                    }

                    //Output delete file log
                    WriteLogInformation("Start delete file", fileList);
                    Console.WriteLine("結束刪除檔案");
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("刪除檔案失敗:" + e.Message);
                    return;
                }
            }
        }

        /// <summary>
        /// output Xml format
        /// </summary>
        /// <param name="jobname"></param>
        /// <param name="filename"></param>
        private void WriteLogInformation(string jobname, string filename)
        {
            StringBuilder sbuilder = new StringBuilder();
            using (StringWriter sw = new StringWriter(sbuilder))
            {
                using (XmlTextWriter w = new XmlTextWriter(sw))
                {
                    w.WriteStartElement("Job");
                    w.WriteAttributeString("Name", jobname);
                    w.WriteElementString("Time", DateTime.Now.ToString());
                    w.WriteElementString("filename", filename);
                    w.WriteEndElement();
                }
            }
            string logfilename = ConfigurationManager.AppSettings["LogPath"] + "log_del_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + " .xml";
            using (StreamWriter w = new StreamWriter(logfilename, true, Encoding.UTF8))
            {
                w.WriteLine(sbuilder.ToString());
            }
        }
    }

    [DisallowConcurrentExecution]
    public class Compress_Job : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //Get compress path
            string compress_path = ConfigurationManager.AppSettings["CompressPath"];

            if (System.IO.Directory.Exists(compress_path))
            {
                try
                {
                    Console.WriteLine("開始壓縮檔案");
                    string[] files = Directory.GetFiles(compress_path);
                    string fileList = "";
                    foreach (var item in files)
                    {
                        Console.WriteLine("壓縮檔案  =>" + item);
                        fileList = fileList + item +"\n";
                    }

                    WriteLogInformation("Start compress file", fileList);
                    string savefilename = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                    ZipFile.CreateFromDirectory(compress_path, @"E:\Workspace\compress_" + savefilename + ".zip");
                    Console.WriteLine("產生壓縮檔案 " + savefilename +".zip");
                    Console.WriteLine("結束壓縮檔案");
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("壓縮檔案失敗:" + e.Message);
                    return;
                }
            }
        }

        /// <summary>
        /// output Xml format
        /// </summary>
        /// <param name="jobname"></param>
        /// <param name="filename"></param>
        private void WriteLogInformation( string jobname, string filename)
        {
            StringBuilder sbuilder = new StringBuilder();
            using (StringWriter sw = new StringWriter(sbuilder))
            {
                using (XmlTextWriter w = new XmlTextWriter(sw))
                {
                    w.WriteStartElement("Job");
                    w.WriteAttributeString("Name", jobname);
                    w.WriteElementString("Time", DateTime.Now.ToString());
                    w.WriteElementString("filename", filename);
                    w.WriteEndElement();
                }
            }
            string logfilename = ConfigurationManager.AppSettings["LogPath"] + "log_zip_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + " .xml";
            using (StreamWriter w = new StreamWriter(logfilename, true, Encoding.UTF8))
            {
                w.WriteLine(sbuilder.ToString());
            }
        }
    }



    [DisallowConcurrentExecution]
    public class FileMonitor_Job : IJob
    {

        private static Dictionary<string, DateTime> lastModifiedTimes = new Dictionary<string, DateTime>();
        private static string folderPath = ConfigurationManager.AppSettings["FileMonitorPath"];
        private static string recordFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last_modified.txt");
        //private string sMailList = ConfigurationManager.AppSettings["mailList"].ToString(); // 須通知的人員名單
        //private string sEmailServer = ConfigurationManager.AppSettings["Email_Server"].ToString();


        public void Execute(IJobExecutionContext context)
        {
            //同時間只有一個JOB執行，設定等待時間
            //if(MainService._mutex.WaitOne(TimeSpan.FromSeconds(30)))
            //{
            try
            {
                FileMonitor();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                //MainService._mutex.ReleaseMutex();
            }
            //}
            //else
            //{
            //    //超時無法執行
            //}
        }

        /// <summary>
        /// 取得Telex
        /// </summary>
        private void FileMonitor()
        {
            Console.WriteLine($"[Quartz] 檢查資料夾變更: {DateTime.Now}");
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("資料夾不存在！");
                return;
            }

            LoadLastModifiedTimes(); // 讀取記錄檔

            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            string ReplacePath = folderPath + @"\";
            bool hasNewChanges = false;
            bool hasChanges = false;
            bool hasDelete = false;
            string ChangeFiles = "";
            string DeleteFiles = "";
            int seq = 1;
            foreach (var file in files)
            {
                DateTime lastWriteTime = File.GetLastWriteTime(file);

                if (lastModifiedTimes.TryGetValue(file, out DateTime prevTime))
                {
                    if ((lastWriteTime - prevTime) > TimeSpan.FromSeconds(1))
                    {
                        Console.WriteLine($"檔案更新: {file}");
                        lastModifiedTimes[file] = lastWriteTime;
                        ChangeFiles += $"{seq}.{file.Replace(ReplacePath, "")}({lastWriteTime}) <br>";
                        hasChanges = true;
                        seq++;
                    }
                }
                else
                {
                    Console.WriteLine($"新檔案: {file}");
                    lastModifiedTimes[file] = lastWriteTime;
                    ChangeFiles += $"{seq}.{file.Replace(ReplacePath, "")}({lastWriteTime}) <br>";
                    hasNewChanges = true;
                    seq++;
                }
            }

            // 檢查刪除的檔案
            var deletedFiles = lastModifiedTimes.Keys.Except(files).ToList();
            foreach (var deletedFile in deletedFiles)
            {
                Console.WriteLine($"檔案刪除: {deletedFile}");
                lastModifiedTimes.Remove(deletedFile);
                DeleteFiles += $"{seq}.{deletedFile.Replace(ReplacePath, "")} <br>";
                hasDelete = true;
                seq++;
            }

            MailObject m = new MailObject();
            m.Subject = "EWBS FDB Change Notification";
            m.Body += "EWBS SERVER FDB CHANGE" + " <br>";
            m.Body += "異動檔：" + " <br>";
            m.Body += ChangeFiles;
            m.Body +=  "\r\n";
            m.Body += "刪除檔：" + " <br>";
            m.Body += DeleteFiles;

            if (hasNewChanges)
            {
                SaveLastModifiedTimes();
            }

            if (hasChanges || hasDelete)
            {
                SaveLastModifiedTimes();
                //SendComponent send = new SendComponent();
                //send.SendMailHTML(DisplayName, DisplayName, SendTo, m.Subject, m.Body, 2);

                SendMail(m);
            }
        }


        /// <summary>
        /// 讀取上次的檔案修改時間記錄
        /// </summary>
        private static void LoadLastModifiedTimes()
        {
            if (!File.Exists(recordFile)) return;

            lastModifiedTimes.Clear();
            foreach (var line in File.ReadAllLines(recordFile))
            {
                var parts = line.Split('|');
                if (parts.Length == 2 && DateTime.TryParse(parts[1], out DateTime lastWriteTime))
                {
                    lastModifiedTimes[parts[0]] = lastWriteTime;
                }
            }
        }

        /// <summary>
        /// 儲存當前的檔案修改時間記錄
        /// </summary>
        private static void SaveLastModifiedTimes()
        {
            var lines = lastModifiedTimes.Select(kvp => $"{kvp.Key}|{kvp.Value}");
            File.WriteAllLines(recordFile, lines);
        }


        private void SendMail(MailObject m)
        {
            string MailServerIP = ConfigurationManager.AppSettings["Email_Server"].ToString();
            string MailServerIPSsl = ConfigurationManager.AppSettings["Email_Server_SSL"].ToString();
            string SendFrom = ConfigurationManager.AppSettings["SendFrom"].ToString();
            string DisplayName = ConfigurationManager.AppSettings["DisplayName"].ToString();
            string SendTo = ConfigurationManager.AppSettings["SendTo"].ToString();

            try
            {
                MailMessage mailMsg = new MailMessage();


                mailMsg.Subject = m.Subject;
                mailMsg.BodyEncoding = Encoding.UTF8;
                mailMsg.IsBodyHtml = true;

                MailMessage message = new System.Net.Mail.MailMessage();
                message.Body = m.Body;
                mailMsg.Body = message.Body;
                SmtpClient SmtpSrv = new SmtpClient();
                SmtpSrv.EnableSsl = false;

                if (SmtpSrv.EnableSsl)
                    SmtpSrv.Host = MailServerIPSsl;
                else
                    SmtpSrv.Host = MailServerIP;

                #region send Mail

                if (string.IsNullOrEmpty(SendTo))  //EMAIL為空不寄
                    return;

                if (SendTo.IndexOf(";") > 0)
                {
                    string[] mailto = SendTo.Split(';');

                    foreach (var item in mailto)
                    {
                        if (item != "")
                        {
                            mailMsg.To.Add(item);
                        }
                    }
                }
                else
                {
                    mailMsg.To.Add(SendTo);
                }

                mailMsg.From = new MailAddress(SendFrom, DisplayName);

                SmtpSrv.Send(mailMsg);


                #endregion


            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.ToString();

                //writeLog("[sendEmail() error] " + ex.Message);
            }
        }


        /// <summary>
        /// 如果想更嚴謹判斷內容是否變更（避免只比對時間），可以使用 MD5 計算檔案雜湊值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string ComputeFileHash(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
            }
        }

        public class MailObject
        {
            public string Subject { get; set; }
            public string Body { get; set; }

        }

    }
}
