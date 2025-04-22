/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//*Thomas| Ver. 08 | #BR17164 | 2017/8/31                                                                      *
//*---------------------------------------------------------------------------*
//* 物件關閉後 Dispose 標記由GC回收        
//*****************************************************************************
//*Thomas| Ver. 07 | #BR17204 | 2017/08/03   *
//*---------------------------------------------------------------------------*
//* 修正Baggage Staticstic Report View Report時，遇到 AirType  = ""的資料會   *
//* 跳出Exception的問題 (例如: BR1012 )    無法產出Baggage Staticstic Report   *
//*****************************************************************************
//*Cherry Chan| Ver. 06 | #BR11100 | 2011/FEB/14   (V01.17)                   *
//*---------------------------------------------------------------------------*
//* 新增BR777行李報表                                                         *
//*****************************************************************************
//*Cherry Chan| Ver. 05 | #BR10100 | 2010/APR/16   (V01.12)                   *
//*---------------------------------------------------------------------------*
//* 修正ewbs.stat比對格式由TPE-HKG改為比對TPE HKG。                           *
//*****************************************************************************
//*Cherry Chan| Ver. 04 | #BR10100 | 2010/APR/16   (V01.12)                   *
//*---------------------------------------------------------------------------*
//* 修正ewbs.stat比對格式由TPE-HKG改為比對TPE HKG。                           *
//*****************************************************************************
//*Cherry Chan| Ver. 03 | #BR09128 | 2009/DEC/18   (V01.11)                   *
//*---------------------------------------------------------------------------*
//* 新增baggage的月報表的平均飛機行李櫃數及新增班機號碼                       *
//*****************************************************************************
//*Cherry Chan| Ver. 02 | #BR09112 | 2009/AUG/10   (V01.07)                   *
//*---------------------------------------------------------------------------*
//* 修改baggage的月報表的平均旅客行李件數                                     *
//*	pce/pax -point 2														  *
//* wt/pax,wt/pce,pce/cntr - point 1                                          *
//*****************************************************************************
//*Cherry Chan| Ver. 01 | #BR08110 | 2008/AUG/15                              *
//*---------------------------------------------------------------------------*
//* 修改baggage的月報表的平均旅客行李件數pce/pax：由整數改為小數三位          *
//*****************************************************************************
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using EWBS;
using FlightDataBase;
using System.Configuration;

namespace EwbsCore.Baggage
{
    /// <summary>
    /// Summary description for BagStat, a class for statistic data of baggages
    /// </summary>
    [Serializable]
    public class BagStat
    {
        protected string route = ""; //Route
        protected string airtype = ""; //Air Type
        protected string date = ""; // Date
        protected int pax = 0; // number of Passenger 
        protected int cntr = 0; //number of container
        protected int pcs = 0; //Pieces
        protected int wt = 0; //Weight
        protected string fltNo = "";	   //BR09128 flight number
        protected int fltCount = 0;      //BR09128 flight count

        //BR09128<--add flight number and flight count
        public BagStat(string route, string airtype, string date, int pax, int cntr, int pcs, int wt, string sFltNo, int iFltCnt)
        {
            this.route = route;
            this.airtype = airtype;
            this.date = date;
            this.pax = pax;
            this.cntr = cntr;
            this.pcs = pcs;
            this.wt = wt;
            this.fltNo = sFltNo;		//BR09128 flight number
            this.fltCount = iFltCnt;	//BR09128 flight count
        }
        //BR09128-->

        /// <summary>
        /// get: route
        /// </summary>
        public string Route
        {
            get { return route; }
        }

        /// <summary>
        /// get airtype
        /// </summary>
        public string Airtype
        {
            get { return airtype; }
        }

        /// <summary>
        /// date
        /// </summary>
        public string Date
        {
            get { return date; }
        }

        /// <summary>
        /// pax
        /// </summary>
        public int PaxNum
        {
            get { return pax; }
            set { pax = value; }
        }

        /// <summary>
        /// cntr
        /// </summary>
        public int CntrCount
        {
            get { return cntr; }
            set { cntr = value; }
        }

        /// <summary>
        /// pcs
        /// </summary>
        public int Pcs
        {
            get { return pcs; }
            set { pcs = value; }
        }

        /// <summary>
        /// wt
        /// </summary>
        public int Wt
        {
            get { return wt; }
            set { wt = value; }
        }

        //		//BR09128<--
        //flight number
        public string sFltNo
        {
            get { return fltNo; }
            set { fltNo = value; }
        }
        //flight count
        public int iFltCount
        {
            get { return fltCount; }
            set { fltCount = value; }

        }
        //BR09128-->
    }

    /// <summary>
    /// An ArrayList composed of BagStat
    /// </summary>
    [Serializable]
    public class BagStatList : ArrayList
    {
        private const int defaultBagWt = 15; //default baggage weight
        private const int defaultBagPcs = 1; //default baggage pieces

        /// <summary>
        /// Constructor
        /// </summary>
        public BagStatList()
        { }

        /// <summary>
        /// get or set Baggage state by its ArrayList Indices
        /// </summary>
        /// <--  隱藏繼承的成員，加入new 關鍵字-->
        public new BagStat this[int index]
        {
            get { return base[index] as BagStat; }
            set { base[index] = value; }
        }

        /// <summary>
        ///  add BagStat
        /// </summary>
        /// <param name="value">BagStat obj</param>
        /// <returns>ArrayList index</returns>
        public override int Add(object value)
        {
            // TODO:  Add SeatList.Add implementation
            if (value is BagStat)
            {
                BagStat bagStat = value as BagStat;
                foreach (BagStat stat in this)
                {
                    //if the Route, Aircraft type and date correspond to the BagStat, input the data to the bagStat
                    if (stat.Route.Equals(bagStat.Route) && stat.Airtype.Equals(bagStat.Airtype) && stat.Date.Equals(bagStat.Date) && bagStat.Wt > 0)
                    {
                        stat.PaxNum = bagStat.PaxNum;
                        stat.Pcs = bagStat.Pcs;
                        stat.CntrCount = bagStat.CntrCount;
                        stat.Wt = bagStat.Wt;
                        return IndexOf(stat);
                    }
                }
                int result = base.Add(value);

                MaxLimited(3);

                return result;
            }
            else
            {
                //throw(new Exception("Wrong type, expected type is SeatItem."));
                return -1;
            }
        }


        /// <summary>
        /// restrict ArrayList amount 
        /// </summary>
        /// <param name="maxCount">the restricted amount </param>
        private void MaxLimited(int maxCount)
        {
            ArrayList aList = new ArrayList();

            // for every items in this ArrayList
            for (int i = 0; i < this.Count; i++)
            {
                aList.Add(i);

                //if the Route, Aircraft type and date corresponds add j to the aList
                for (int j = i + 1; j < this.Count; j++)
                {
                    if (this[i].Route.Equals(this[j].Route) &&
                        this[i].Airtype.Equals(this[j].Airtype) &&
                        this[i].Date.Equals(this[j].Date))
                    {
                        aList.Add(j);
                    }
                }
                for (int j = aList.Count - maxCount - 1; j >= 0; j--)
                {
                    this.RemoveAt((int)aList[j]);
                }
                aList.Clear();
            }
        }

        /// <summary>
        ///  compute avg. baggage weight
        /// </summary>
        /// <param name="route">flight route </param>
        /// <param name="airtype">aircraft type</param>
        /// <returns>avg. baggage weight</returns>
        public int AverageBagWt(string route, string airtype)
        {
            int count = 0;
            int ttlWt = 0;
            //Traverse all the BafStats in this
            foreach (BagStat stat in this)
            {
                if (stat.Route.Equals(route))
                {
                    if (stat.Airtype.Equals(airtype))
                    {
                        ttlWt += stat.Wt;
                        count += stat.PaxNum;
                    }
                    else if (stat.Airtype.Substring(0, 4) == (airtype.Substring(0, 4)))
                    {
                        if (stat.Airtype.Substring(0, 4) == "BR33" || stat.Airtype.Substring(0, 4) == "BRM9" || stat.Airtype.Substring(0, 4) == "BR77")
                        {
                            ttlWt += stat.Wt;
                            count += stat.PaxNum;
                        }
                    }
                }
            }

            //return the average Baggage Weight
            if (count > 0 && ttlWt / count > 0)
                return (int)Math.Round((float)ttlWt / count, 0);

            return defaultBagWt;
        }

        /// <summary>
        ///  compute avg. baggage number
        /// </summary>
        /// <param name="route">flight route </param>
        /// <param name="airtype">aircraft type</param>
        /// <returns>avg. baggage number</returns>
        public int AverageBagPcs(string route, string airtype)
        {
            int count = 0;
            int ttlPcs = 0;
            //Traverse all the BafStats in this
            foreach (BagStat stat in this)
            {
                if (stat.Route.Equals(route))
                {
                    if (stat.Airtype.Equals(airtype))
                    {
                        ttlPcs += stat.Pcs;
                        count += stat.PaxNum;
                    }
                    else if (stat.Airtype.Substring(0, 4) == (airtype.Substring(0, 4)))
                    {
                        if (stat.Airtype.Substring(0, 4) == "BR33" || stat.Airtype.Substring(0, 4) == "BRM9" || stat.Airtype.Substring(0, 4) == "BR77")
                        {
                            ttlPcs += stat.Pcs;
                            count += stat.PaxNum;
                        }
                    }
                }
            }
            return (count > 0 && ttlPcs >= count ? ttlPcs / count : defaultBagPcs);
            //BR10100 return (count > 0 ? ttlPcs / count : defaultBagPcs);
        }

        #region Serialize/DeSerialize Methods

        /// <summary>
        /// store BagStatList obj
        /// </summary>
        /// <param name="fileStream">FileStream obj</param>
        public void Serialize(FileStream fileStream)
        {
            // use the CLR binary formatter
            BinaryFormatter binaryFormatter =
                new BinaryFormatter();

            //binaryFormatter.TypeFormat=FormatterTypeStyle.TypesWhenNeeded;

            // serialize to disk
            binaryFormatter.Serialize(fileStream, this);
        }

        /// <summary>
        /// read BagStatList obj
        /// </summary>
        /// <param name="fileStream">FileStream obj</param>
        public static BagStatList DeSerialize(FileStream fileStream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            //Setting binary format
            binaryFormatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
            //retrieving value of the statistics
            BagStatList retVal = (BagStatList)binaryFormatter.Deserialize(fileStream);
            return retVal;
        }

        #endregion
    }

    /// <summary>
    /// Class for collecting baggage statistics
    /// </summary>
    public sealed class BaggageStatistic
    {
        public static readonly BaggageStatistic instance = new BaggageStatistic();
        private BagStatList bagStatList; //a List for BagStat objs
        private string filename = "ewbs.stat"; //file name

        /// <summary>
        /// Instance
        /// </summary>
        public static BaggageStatistic Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private BaggageStatistic()
        {
            try
            {
                //if the file exists, read the file; if not, thorw exception
                if (File.Exists(filename))
                {
                    FileStream fileStream = new FileStream(filename, FileMode.Open);
                    bagStatList = BagStatList.DeSerialize(fileStream);
                    fileStream.Close();
                    fileStream.Dispose(); //#BR17164 THOMAS標記由GC釋放
                    fileStream = null; //#BR17164 THOMAS標記由GC釋放
                    System.GC.Collect(); //#BR17164 THOMAS GC強制回收
                }
                else throw (new Exception("File not found!!"));
            }
            catch
            {
                bagStatList = new BagStatList();
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~BaggageStatistic()
        {
            // create a file stream to write the file
            FileStream fileStream = new FileStream(filename, FileMode.Create);
            bagStatList.Serialize(fileStream);
            fileStream.Close();
            fileStream.Dispose(); //#BR17164 標記由GC釋放
            fileStream = null; //#BR17164 標記由GC釋放
            System.GC.Collect(); //#BR17164  GC強制回收
        }

        /// <summary>
        ///  compute avg. baggage weight
        /// </summary>
        /// <param name="from">from station</param>
        /// <param name="to">to station</param>
        /// <param name="airtype">aircraft type</param>
        /// <returns>avg. baggage weight</returns>
        public int AverageBagWt(string from, string to, string airtype)
        {
            //BR10100 return bagStatList.AverageBagWt(from + "-" + to, airtype);
            return bagStatList.AverageBagWt(from + " " + to, airtype);
        }

        /// <summary>
        ///  compute avg. baggage number
        /// </summary>
        /// <param name="from">from station</param>
        /// <param name="to">to station</param>
        /// <param name="airtype">aircraft type</param>
        /// <returns>平均 baggage number</returns>
        public int AverageBagPcs(string from, string to, string airtype)
        {
            //BR10100 return bagStatList.AverageBagPcs(from + "-" + to, airtype);
            return bagStatList.AverageBagPcs(from + " " + to, airtype); //BR10100
        }

        /// <summary>
        ///  add BagStat obj
        /// </summary>
        /// <param name="theFlight">Flight obj</param>
        public void Add(Flight theFlight)
        {
            //for each ClsDstnBagItem in theFlight.Pax.AcceptedBagList, check them all
            foreach (ClsDstnBagItem bagItem in theFlight.Pax.AcceptedBagList)
            {
                string dest = bagItem.Dstn;
                //BR09128 string route = theFlight.Route[0] + "-" + dest;  //TPE-HKG
                string route = theFlight.Route[0] + " " + dest;  //BR09128 TPE HKG
                //get the paxItem by the destination
                ClsDstnClassItem paxItem = theFlight.Pax.ActlClsDstnClassList.Find(dest);

                //record the data from the paxItem
                int pax = paxItem[0] + paxItem[1] + paxItem[2];
                int cntr = theFlight.BaggageList.CountCntr(dest);
                int bagPcs = bagItem[0].BagPcs + bagItem[1].BagPcs + bagItem[2].BagPcs;
                int bagWt = bagItem[0].BagWt + bagItem[1].BagWt + bagItem[2].BagWt;

                string fltNo = theFlight.FlightNumber;  //BR09128
                int iFltCnt = 0;  //BR09128

                //Add a new BagStat created by the information gotten above.
                bagStatList.Add(new BagStat(
                    route, theFlight.ACType.name,
                    String.Format("{0}-{1:ddMMM}-{2}",theFlight.FlightNumber,theFlight.STD,theFlight.Route[1]),pax, cntr, bagPcs, bagWt, fltNo, iFltCnt));
            }
        }
    }

    /// <summary>
    /// An arraylist for Baggage Statistic information for server
    /// </summary>
    [Serializable]
    public class SrvBagStatList : ArrayList
    {
        /// <summary>
        ///  add BagStat
        /// </summary>
        /// <param name="value">BagStat obj</param>
        /// <returns>ArrayList index</returns>
        public void Add(Flight theFlight)
        {
            //for each ClsDstnBagItem in theFlight.Pax.AcceptedBagList
            foreach (ClsDstnBagItem bagItem in theFlight.Pax.AcceptedBagList)
            {
                string dest = bagItem.Dstn;
                string route = theFlight.Route[0] + " " + dest;  //BR09128
                //get the paxItem by the destination
                ClsDstnClassItem paxItem = theFlight.Pax.ActlClsDstnClassList.Find(dest);

                //record the data from the paxItem
                int pax = paxItem[0] + paxItem[1] + paxItem[2];
                int cntr = theFlight.BaggageList.CountCntr(dest);
                int bagPcs = bagItem[0].BagPcs + bagItem[1].BagPcs + bagItem[2].BagPcs;
                int bagWt = bagItem[0].BagWt + bagItem[1].BagWt + bagItem[2].BagWt;

                string fltNo = theFlight.FlightNumber;  //BR09128
                int iFltCnt = 0;  //BR09128
                //get the Aircraft type
                string acTypeName = FDB.Instance.GetAirlineNameByAC(theFlight.ACRegNo) + // 1103
                    FDB.Instance.GetAirTypeNameByAC(theFlight.ACRegNo);
                //Add a new BagStat created by the information gotten above.
                //BR09128<--
                this.Add(new BagStat(route,acTypeName,String.Format("{0}-{1:ddMMM}-{2}",theFlight.FlightNumber,
                    theFlight.STD,theFlight.Route[1]),pax, cntr, bagPcs, bagWt, fltNo, iFltCnt));
            }
        }
    }

    /// <summary>
    /// Baggage Statistic information for server
    /// </summary>
    public sealed class SrvBaggageStatistic
    {
        private static readonly SrvBaggageStatistic instance = new SrvBaggageStatistic(); //Instance

        private SrvBagStatList bagStatList = new SrvBagStatList(); //Baggage statistc data list
        private ArrayList bagStatRepList = new ArrayList(); //Baggage report data list
        //BR09128 private const string reportFormat = "{0,-7}{1,-8}{2,5} {3,6} {4,7} {5,5} {6,7} {7,6} {8,6} {9,7}\r\n"; //string referred by the report formats
        //#BR17222 private const string reportFormat = "{0,7} {1,-8} {2,-3} {3,7} {4,5} {5,6} {6,7} {7,5} {8,9} {9,6} {10,6} {11,7} {12,8} {13,8} \r\n"; //string referred by the report formats //BR09128
        private const string reportFormat = "{0,7} {1,-8} {2,-3} {3,7} {4,5} {5,6} {6,7} {7,5} {8,9} {9,6} {10,6} {11,7} {12,8} {13,8} \r\n"; //#BR17222 string referred by the report formats 

        /// <summary>
        /// Instance
        /// </summary>
        public static SrvBaggageStatistic Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private SrvBaggageStatistic()
        {
            bagStatList = new SrvBagStatList();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SrvBaggageStatistic()
        { }

        /// <summary>
        /// Generate statistic report
        /// </summary>
        /// <returns>statistic report</returns>
        public string Report(DateTime data)
        {
            //Create the string of the report to be written
            string report = "Baggage Statistic Report for " + data.ToString("MMMyy", new CultureInfo("en-US")) + ":\r\n";

            report += string.Format(reportFormat, "FltNo", "FM  TO ", "AC", "FltQty", "Pax", "Pieces", "Weight", "Cntrs",
                "Cntrs/Flt", "Pcs/Pax", "Wt/Pax", "Wt/Pcs", "Pcs/Cntr", "Pax/Cntr");  //#BR17222 增加 PAX/Cntr 欄位

            ArrayList aList = new ArrayList();
            ArrayList reportList = new ArrayList();

            //For each BagStat in bagStatList, if the aList does not contain the specified BagStat, add the BagStat.

                foreach (BagStat bagStat in bagStatList)
                    if (!InReportList(aList, bagStat))
                        aList.Add(bagStat);

            //for each BagStat in aList, generate the report and add it onto the report list.
                foreach (BagStat bagStat in aList)
                {
                    string aReport = GenReport(bagStat);
                    if (aReport != "")
                        reportList.Add(aReport);
                }

                reportList.Sort();
                foreach (string aReport in reportList)
                report += aReport;
            
            bagStatRepList.Clear();
            return report;
        }

        /// <summary>
        /// Check if one BagStat object is in the ArrayList
        /// </summary>
        /// <param name="aList">the ArrayList to be checked</param>
        /// <param name="bagStat">the BagStat object to be checked</param>
        /// <returns>bool: if the BagStat object exists</returns>
        private bool InReportList(ArrayList aList, BagStat bagStat)
        {
            foreach (BagStat aBagStat in aList)
            {
                //BR09128<--
                if (aBagStat.Route == bagStat.Route && aBagStat.sFltNo == bagStat.sFltNo)
                {
                    try
                    {  
                        //<!-- #BR18120 若Airtype為""，也不加入清單
                        if (aBagStat.Airtype == "" || bagStat.Airtype == "")
                        {
                            return true;
                        }
                        //#BR18120-->
                             
                        if (aBagStat.Airtype.Substring(0, 4) == bagStat.Airtype.Substring(0, 4))
                        {
                            //BR11100
                            //if (aBagStat.Airtype.Substring(0,4) =="BRM9" || aBagStat.Airtype.Substring(0,4) =="BR33")
                            if (aBagStat.Airtype.Substring(0, 4) == "BRM9" || aBagStat.Airtype.Substring(0, 4) == "BR33" || aBagStat.Airtype.Substring(0, 4) == "BR77") //BR11100
                                return true;  //not need to add
                            else if (aBagStat.Airtype == bagStat.Airtype)
                                return true;  //not need to add
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(aBagStat.Airtype + "---" + ex.Message);
                    }
                }
                //BR09128-->
            }
            return false;
        }

        /// <summary>
        /// generate report in accordance with a BagStat obj
        /// </summary>
        /// <param name="_bagStat">the BagStat obj to be in accordance with</param>
        /// <returns></returns>
        private string GenReport(BagStat _bagStat)
        {
                string oneReport = "";
                BagStatReport bagStatRep = new BagStatReport(_bagStat);
                BagStatReport bagStatRepError = new BagStatReport(_bagStat);
                bagStatRepList.Add(bagStatRep);
                string ErrorData = "";
                    foreach (BagStat bagStat in bagStatList)
                    {
                        if (bagStat.Route == bagStatRep.Route && bagStat.sFltNo == bagStatRep.sFltNo)
                        {
                            if (bagStat.Airtype != "" && bagStatRep.Airtype!="" && bagStat.Airtype.Substring(0, 4) == bagStatRep.Airtype.Substring(0, 4)) //#BR17204過濾 AirType  = ""的班機(例如: BR1012 )
                            {
                                if (bagStat.Airtype.Substring(0, 4) == "BRM9" || bagStatRep.Airtype.Substring(0, 4) == "BR33" || bagStatRep.Airtype.Substring(0, 4) == "BR77" || bagStat.Airtype == bagStatRep.Airtype)  //BR11100
                                {
                                    bagStatRep.PaxNum += bagStat.PaxNum;
                                    bagStatRep.Pcs += bagStat.Pcs;
                                    bagStatRep.Wt += bagStat.Wt;
                                    bagStatRep.CntrCount += bagStat.CntrCount;
                                    bagStatRep.iFltCount++;
                                }
                            }
                            else if (bagStat.Airtype == "" || bagStatRep.Airtype =="")
                            {
                                ErrorData += bagStat.sFltNo + "," + bagStat.Date + bagStat.PaxNum.ToString() + bagStat.Pcs.ToString() + bagStat.Wt.ToString() + bagStat.CntrCount.ToString() + "\r\n";
                            }
                        }
                    }

                if (bagStatRep.PaxNum == 0)
                    return "";
  
                float fPaxNum;
                float fPcs;
                float fWt;
                float fCntrCount;
                float fFltCntr;
                float fPaxCntr; //#BR17222 增加Pax/Cntr
                string sAcType = "";   //BR09128
                if (bagStatRep.PaxNum == 0)  //divisor is zero
                {
                    fPaxNum = 0;
                    fPcs = 0;
                }
                else
                {
                    fPaxNum = (float)Math.Round((float)bagStatRep.Pcs / bagStatRep.PaxNum, 2);
                    fPcs = (float)Math.Round((float)bagStatRep.Wt / bagStatRep.PaxNum, 1);
                }

                if (bagStatRep.Pcs == 0) //divisor is zero
                    fWt = 0;
                else
                    fWt = (float)Math.Round((float)bagStatRep.Wt / bagStatRep.Pcs, 1);

                if (bagStatRep.CntrCount == 0) //divisor is zero
                    fCntrCount = 0;
                else
                    fCntrCount = (float)Math.Round((float)bagStatRep.Pcs / bagStatRep.CntrCount, 1);

                if (bagStatRep.CntrCount == 0) //divisor is zero
                    fFltCntr = 0;
                else
                    fFltCntr = (float)Math.Round((float)bagStatRep.CntrCount / bagStatRep.iFltCount, 2);

                if (bagStatRep.CntrCount == 0) //divisor is zero
                    fPaxCntr = 0;
                else
                    fPaxCntr = (float)Math.Round((float)bagStatRep.PaxNum / bagStatRep.CntrCount, 2);

                if (bagStatRep.Airtype.Substring(0, 4) == "BRM9" || bagStatRep.Airtype.Substring(0, 4) == "BR33")
                    sAcType = bagStatRep.Airtype.Substring(2, 2) + "0";
                else if (bagStatRep.Airtype.Substring(0, 4) == "BR77")  //BR11100
                    sAcType = bagStatRep.Airtype.Substring(2, 2) + "7";
                else
                    sAcType = bagStatRep.Airtype.Substring(2, 3);

                //#BR17222 傳回值增加fPaxCntr
                oneReport += string.Format(reportFormat, bagStatRep.sFltNo, bagStatRep.Route, sAcType,
                    bagStatRep.iFltCount, bagStatRep.PaxNum, bagStatRep.Pcs, bagStatRep.Wt, bagStatRep.CntrCount,
                    fFltCntr, fPaxNum, fPcs, fWt, fCntrCount, fPaxCntr);
                return oneReport;
        }

        /// <summary>
        ///  add BagStat obj
        /// </summary>
        /// <param name="theFlight">Flight obj</param>
        public void Add(Flight theFlight)
        {
            //Traverse each ClsDstnBagItem in the theFlight.Pax.AcceptedBagList
            foreach (ClsDstnBagItem bagItem in theFlight.Pax.AcceptedBagList)
            {
                string dest = bagItem.Dstn;
                //BR09128string route = theFlight.Route[0] + "-" + dest;
                string route = theFlight.Route[0] + " " + dest;  //BR09128
                //get the paxItem of the specified destination
                ClsDstnClassItem paxItem = theFlight.Pax.ActlClsDstnClassList.Find(dest);

                //record the data from the paxItem found
                int pax = paxItem[0] + paxItem[1] + paxItem[2];
                int cntr = theFlight.BaggageList.CountCntr(dest);
                int bagPcs = bagItem[0].BagPcs + bagItem[1].BagPcs + bagItem[2].BagPcs;
                int bagWt = bagItem[0].BagWt + bagItem[1].BagWt + bagItem[2].BagWt;

                string fltno = theFlight.FlightNumber;  //BR09128
                int iFltCnt = 0;  //BR09128
                //get the aircraft type's name
                string acTypeName = FDB.Instance.GetAirlineNameByAC(theFlight.ACRegNo) +
                    FDB.Instance.GetAirTypeNameByAC(theFlight.ACRegNo);

                //Add a new BagStat into the bagStatList
                //BR09128<--
                //bagStatList.Add(new BagStat(route, acTypeName, "",pax, cntr, bagPcs, bagWt));
                //BR09128-->
                bagStatList.Add(new BagStat(route, acTypeName, "", pax, cntr, bagPcs, bagWt, fltno, iFltCnt));  //BR09128
            }
        }

        /// <summary>
        /// clean up bagStatList
        /// </summary>
        public void Clear()
        {
            bagStatList.Clear();
        }
    }

    /// <summary>
    /// Class used to generate Baggage Statistics reports
    /// </summary>
    public class BagStatReport : BagStat
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bagStat">BagStat obj to be reported</param>
        public BagStatReport(BagStat bagStat)
            : base(bagStat.Route, bagStat.Airtype, "", 0, 0, 0, 0, bagStat.sFltNo, bagStat.iFltCount)  //BR09128
        //BR09128: base(bagStat.Route, bagStat.Airtype, "", 0, 0, 0, 0,bagStat.sFltNo)
        { }
    }
}