/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/

//*****************************************************************************
//* THOMAS    | Ver. 30 | #BR19008  | 2019/03/01                              *
//*---------------------------------------------------------------------------*
//* 所有機型LMC之後新增Tolerances限制值，修改 LAST MINUTES CHANGES 文字顯示。 *
//*****************************************************************************
//* THOMAS    | Ver. 29 | #BR19003  | 2019/03/01                              *
//*---------------------------------------------------------------------------*
//* Bag Plan Remark 新增BS輸入規則                                            *
//*****************************************************************************
//* THOMAS    | Ver. 28 | #BR18114  | 2018/7/1                                *
//*---------------------------------------------------------------------------*
// CPM ULD SN 去除空白                                                        *
//*****************************************************************************
//* Thomas    | Ver.27| #BR18112  | 2018/5/31                                 *
//*---------------------------------------------------------------------------*
//* 1.Baggage頁面 BULK REMARK                                                 *
//* a.輸入BC,BT、BT、BT,BY 時 CPM 的CATG改為REMARK的內容                      *
//* b.輸入非上述字串或者不輸入， CPM 顯示 原本的CATG                          *
//*                                                                           *
//* 2.Baggage頁面 ULD REMARK(非混打櫃)                                        *
//* a.輸入BC,BT、BT、BT,BY 時 CPM 的CATG改為REMARK的內容                      *
//* b.不輸入， CPM 顯示 原本的CATG                                            *
//*                                                                           *
//* 3.Baggage頁面 ULD REMARK(混打櫃，只在BC櫃 REMARK輸入,BY 櫃輸入則不處理)   *
//* a.輸入BC,BT、BT 時 CPM 的CATG改為REMARK的內容                             *
//* b.不輸入， CPM 顯示 /BC                                                   *
//*                                                                           *
//*****************************************************************************
//* Thomas    | Ver.26| #BR18111  | 2018/5/23                                 *
//*---------------------------------------------------------------------------*
//*  787 CRZCG顯示MACZFW跟MACTOW較低的那一個                                  *
//*****************************************************************************
//* Thomas    | Ver.25| #BR18107  | 2018/5/3                                  *
//*---------------------------------------------------------------------------*
//* 判斷 TOW是否在EXPD AREA區域內，若在區域內Load sheet & ELS增加顯示結果     *
//*****************************************************************************
//* Thomas    | Ver.24 | #BR18106  | 2018/4/29                                *
//*---------------------------------------------------------------------------*
//* Load sheet undeload 四捨五入取整數                                        *
//*****************************************************************************
//* Thomas    | Ver.23 | #BR17229  | 2017/10/25    (V1.07)                    *
//*---------------------------------------------------------------------------*
//* Load sheet & ELS 增加787 顯示比對CG與MTOW 結果                            *
//*****************************************************************************
//* Thomas    | Ver.22 | #BR17208  | 2017/10/25    (V1.07)                    *
//*---------------------------------------------------------------------------*
//* TRIMSHEET中新增一條CG線，且比對CG與MTOW，加註ALF/FWD CG於Load sheet & ELS *
//*****************************************************************************
//* Thomas  | Ver. 21 | #BR17169 | 2017/10/16                                 *
//*---------------------------------------------------------------------------*
//*CRZCG MAC計算規則更改，原MACZFW>MACTOW，則回MACTOW- 4.0，其他回MACZFW - 5.0*
//，新規則僅扣1.5即可。                                                       *
//*****************************************************************************
//*****************************************************************************
//* Cherry    | Ver. 16 | #BR15106  | 2015/APR/14                (V02.01.01)  *
//*---------------------------------------------------------------------------*
//CPM的SI欄加一空白符合IATA CODE                                              *
//*****************************************************************************
//*CHERRY CHAN| Ver. 15 | #BR13109 | 2013/12/02                   (V02.08.01) *
//*---------------------------------------------------------------------------*
//* CPM format: change /BC/BY or BY/BC to BC, for STAR                        *
//*****************************************************************************
//*CHERRY CHAN| Ver. 14 | #BR13107 | 2013/08/18                      (V02.07) *
//*---------------------------------------------------------------------------*
//* 轉磅時,格式有誤                 					         			  *
//*****************************************************************************
//*CHERRY CHAN| Ver. 13 | #BR13101 | 2013/02/18                      (V02.05) *
//*---------------------------------------------------------------------------*
//* 為配合入星盟修改LDM/CPM格式內容：  					         			  *
//* Jeffery Lin  | Ver. 20 | #BR17143 | 2017/03/30                            *
//*---------------------------------------------------------------------------*
//*CPM SHC LOOP , 散貨SHC太長 會Error                                         *
//*CPM內文超過65字，自動換行                                                  *
//*****************************************************************************
//* Terri Liu  | Ver. 19 | #BR16113 | 2016/11/15                              *
//*---------------------------------------------------------------------------*
//*acars loadsheet資料調整(同paper loadsheet)  四捨五入                       *
//*****************************************************************************
//* Jay Sheu  | Ver. 18 | #BR15115 | 2015-Sep-16                  (V03.02.01) *
//*---------------------------------------------------------------------------*
//* -subtracted unusable fuel to ZFW when air craft is A330                   *
//* 若330機型時，ZFW-不計算UNUSABLE FUEL 並把ACARS的 ()拿掉                   *
//*****************************************************************************
//* Jay Sheu  | Ver. 17 | #BR15112 | 2015-Sep-16                  (V03.02.01) *
//*---------------------------------------------------------------------------*
//* Added DOW between TPLD & ZFW. Rounded to the nearest tenth,1 decimal point*
//(1)取消LDM/CPM SUBJECT欄位內容，MINET電報SUBJECT將不帶EWBS-BR0805-TPE       *
//(2)LDM(日期後面的月份拿掉/機號B-16705的"-"拿掉)                             *
//   BR0031/17.B16705.38C/63Y/211K.4/12                                       *
//(3)CPM(機號B-16705的"-"拿掉，BR77L-580.8P/20A改以LDV580取代)                *
//   BR31/17.B16705.LDV580                                                    *
//*****************************************************************************
//*CHERRY CHAN| Ver. 12 | #BR12110 | 2012/OCT/08                     (V02.04) *
//*---------------------------------------------------------------------------*
//* 於LOADSHEET中新增B7ATR STAB資訊。					         			  *
//*****************************************************************************
//*CHERRY CHAN| Ver. 11 | #BR12107 | 2012/MAY/24                     (V02.02) *
//*---------------------------------------------------------------------------*
//* 修改ACARS FORMAT，ZFW/TOW/MACTOW加括弧                                    *
//*****************************************************************************
//*CHERRY CHAN| Ver. 10 | #BR12105 | 2012/FEB/24                     (V01.22) *
//*---------------------------------------------------------------------------*
//* 於香港航空的LOADSHEET中新增HX STAB資訊。								  *
//* 於亞洲航空的LOADSHEET中新增D7 STAB資訊。								  *
//* (針對下述機型HX/D7調整)                                                   *
//*****************************************************************************
//*CHERRY CHAN| Ver. 09 | #BR10121 | 2010/SEP/27                     (V01.16) *
//*---------------------------------------------------------------------------*
//* 於廈門航空的LOADSHEET中新增MF757 STAB資訊。								  *
//* (針對下述機型MF調整)                                                      *
//*****************************************************************************
//*CHERRY CHAN| Ver. 08 | #BR10113 | 2010/SEP/09                     (V01.15) *
//*---------------------------------------------------------------------------*
//* 於廈門航空的LOADSHEET中新增STAB資訊。									  *
//* (針對下述機型MF73A/MF73B/ MF73C/ MF73D調整)                               *
//*****************************************************************************
//*CHERRY CHAN| Ver. 07 | #BR09118 | 2009/AUG/27                     (V01.08) *
//*---------------------------------------------------------------------------*
//*reformat Acars, WTS IN                                                     *
//*****************************************************************************
//*Cherry Chan| Ver. 06 | #BR09114  | 2009/AUG/10                     (V01.07)*
//*---------------------------------------------------------------------------*
//*For Dash-8 two compartment to amend loadsheet format (DISTRIBUTION)        *
//*****************************************************************************
//*Cherry Chan| Ver. 05 | #BR09109  | 2009/JUL/26                     (V01.06)*
//*---------------------------------------------------------------------------*
//*amend error word                                                           *
//*****************************************************************************
//*Cherry Chan| Ver. 04 | #BR08111  | 2008/AUG/26                             *
//*---------------------------------------------------------------------------*
//*New create weight unit -LB, change KG to LB                                *
//*****************************************************************************
//*Cherry Chan| Ver. 03 | #BR08109  | 2008/AUG/11                             *
//*---------------------------------------------------------------------------*
//*1. remove ACARS radio keyword                                              *
//*****************************************************************************
//*Cherry Chan| Ver. 02 | #BR08105  | 2008/MAY/06                             *
//*---------------------------------------------------------------------------*
//*1. reformat ACARS loadsheet                                                *
//*****************************************************************************
//* Fio Sun   | Ver. 01 | #BR071015 | 2007/AUG/28                             *
//*---------------------------------------------------------------------------*
//*1. Sent ZFW discrepancy telex to FCD                                       *
//*****************************************************************************
//*Fio Sun   | Ver. 00 | #BR071010 | 2007/AUG/14                              *
//*---------------------------------------------------------------------------*
//* Baggage container can show BT on CPM                                      *
//*****************************************************************************

using System;
using System.Collections;
using System.Globalization;
using FlightDataBase;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;  //BR10113
using System.Linq;

namespace EWBS
{
    /// <summary>
    /// compared class for sorting Compartment object
    /// </summary>
    public class CmpComparer : IComparer
    {
        /// <summary>
        /// compared method for sorting Compartment object
        /// </summary>
        /// <param name="x"> object 一</param>
        /// <param name="y"> object 二</param>
        /// <returns> compared result</returns>
        int IComparer.Compare(Object x, Object y)
        {
            CargoPosnBase px = x as CargoPosnBase;
            CargoPosnBase py = y as CargoPosnBase;

            return (px.Name.CompareTo(py.Name));
        }
    }

    /// <summary>
    /// compared class for sorting Bay object
    /// </summary>
    public class BayComparer : IComparer
    {
        /// <summary>
        /// compared method for sorting Bay object
        /// </summary>
        /// <param name="x"> object 一</param>
        /// <param name="y"> object 二</param>
        /// <returns> compared result</returns>
        int IComparer.Compare(Object x, Object y)
        {
            CargoPosnBase px = x as CargoPosnBase;
            CargoPosnBase py = y as CargoPosnBase;

            return (px.Name.CompareTo(py.Name));
        }
    }

    /// <summary>
    /// used for generating content for each kind of telex and loadsheet.
    /// </summary>
    public class Formatter
    {
        private Flight theFlight;
        private CargoPosnBase[] cmpList;
        private const int LineWidth = 65;
        private const double toPound = 2.20462;
        private const string fuelLoadingStyle = "APPLY NONSTANDARD FUELING\r\n";

        /// <summary>
        /// Formatter Constructor
        /// </summary>
        public Formatter(Flight flight)
        {
            theFlight = flight;
            cmpList = theFlight.CargoPosnMgr.GetCmptList();
        }

        /// <summary>Convet CargoPosnBase [] into ArrayList</summary>
        /// <returns>ArrayList</returns>
        private ArrayList PosnListToArrayList(CargoPosnBase[] posnList)
        {
            ArrayList arList = new ArrayList();

            foreach (CargoPosnBase posn in posnList)
                arList.Add(posn);

            return arList;
        }

        #region Properties/Mthods

        /// <summary>
        /// Weight unit
        /// </summary>
        private bool IsKILOS
        {
            get { return theFlight.IsKILOS; }
        }

        /// <summary>Convert float into string </summary>
        /// <returns>string</returns>
        private string Floating(double f)
        {
            // pretty format for floating numbers
            // such as:  12 => 12.0;   12.67 => 12.6
            //           .5 => 0.5;    .579 => 0.5
            string rtn;
            if (f < 1)
                rtn = string.Format("{0,-10:#,0.#}", f);
            else
                rtn = string.Format("{0,-10:#.0}", f);
            return rtn.Substring(0, rtn.IndexOf(" "));
        }

        // part I
        /// <summary>
        /// get the author of the Flight
        /// </summary>
        public string User
        {
            get { return theFlight.Author.ToUpper(); }
        }

        /// <summary>
        /// get loadsheet version
        /// </summary>
        public int LoadVersion
        {
            get { return theFlight.LoadsheetVersion; }
        }

        /// <summary>
        /// Flight Number
        /// </summary>
        private string Flt_no
        {
            get { return theFlight.FlightNumber; }
        }

        /// <summary>
        /// get all the routes in a string[]
        /// </summary>
        private string[] Route
        {
            get { return theFlight.Route; }
        }

        /// <summary>
        /// get Aircraft registration number
        /// </summary>
        private string AcNum
        {
            get { return theFlight.ACRegNo; }
        }

        /// <summary>
        /// get Cabin configuration
        /// </summary>
        private string CabinConfig
        {
            get { return theFlight.CabinConfiguration; }
        }

        /// <summary>
        /// get a string containing information of total cockpit and cabin crews
        /// </summary>
        private string Crew
        {
            get
            {
                CrewItem ttlCrew = theFlight.Crew.TtlCrew + theFlight.Crew.TtlExtraCrew;
                return string.Format("{0}/{1}", ttlCrew.Cockpit, ttlCrew.Cabin);
            }
        }

        /// <summary>
        /// get STD Datein a "ddMMMyy" format
        /// </summary>
        private string Date
        {
            get
            {
                return theFlight.STD.ToString(
                    "ddMMMyy", new CultureInfo("en-US")).ToUpper();
            }
        }

        /// <summary>
        /// get STD time in a "HHmm" format
        /// </summary>
        private string Time
        {
            get
            {
                return theFlight.STD.ToString(
                    "HHmm", new CultureInfo("en-US")).ToUpper();
            }
        }

        /// <summary>+
        /// get current date in a "ddMMMyy" format.
        /// </summary>
        private string CurrentDate
        {
            get { return DateTime.Now.ToString("ddMMMyy", new CultureInfo("en-US")).ToUpper(); }
        }

        /// <summary>
        /// get current time in a "HHmm" format.
        /// </summary>
        private string CurrentTime
        {
            get { return DateTime.Now.ToString("HHmm", new CultureInfo("en-US")).ToUpper(); }
        }

        /// <summary>
        /// get full flight number BR0851 /15NOV
        /// </summary>
        private string FullFlightNo
        {
            get { return string.Format("{0,-7}/{1:ddMMM}", theFlight.FlightNumber, theFlight.STD).ToUpper(); }
        }

        // part II

        /// <summary>
        /// calculate compartment total weitght
        /// </summary>
        /// <returns>string: compartment total weight</returns>
        private double CmpTotalWt()
        {
            double sum = 0;
            for (int i = 0; i < CmpLength; i++)
                sum += CmpWt(i);

            return sum;  //#BR08111
            //return (IsKILOS) ? sum : sum * toPound; //#BR08111
            //return (IsKILOS) ? sum : (int)Math.Round((float)sum * toPound,0);  //#BR08111
        }

        /// <summary>
        /// get the number of compartments
        /// </summary>
        private int CmpLength
        {
            get { return cmpList.Length; }
        }

        /// <summary>get Compartment Name by index </summary>
        /// <param name="index"></param>
        /// <returns>Compartment Name</returns>
        private string CmpName(int index)
        {
            return cmpList[index].Name;
        }

        /// <summary>get Compartment Weight by index</summary>
        /// <param name="index"></param>
        /// <returns>Compartment Weight</returns>
        private double CmpWt(int index)
        {
            return (IsKILOS) ? cmpList[index].getWeight() :
                (int)Math.Round((float)cmpList[index].getWeight() * toPound, 0);  //#BR08111
                //cmpList[index].getWeight() * toPound;
        }

        /// <summary>
        ///  get total weight of PAX
        /// </summary>
        private double PaxTotalWt
        {
            get
            {
                return (IsKILOS) ? theFlight.Pax.getWeight() :
                    (int)Math.Round((float)theFlight.Pax.getWeight() * toPound, 0);  //#BR08111
                    //theFlight.Pax.getWeight() * toPound; //#BR08111
            }
        }

        /// <summary> get amount of male Pax</summary>
        /// <returns>int: amount of male Pax</returns>
        private int Male()
        {
            int sum = 0;
            for (int i = 1; i < theFlight.Route.Length; i++)
                sum += theFlight.Pax.GenderDstnClassList[i - 1].Male;
            return sum;
        }

        /// <summary> get amount of female Pax</summary>
        /// <returns>int: amount of female Pax</returns>
        private int Female()
        {
            int sum = 0;
            for (int i = 1; i < theFlight.Route.Length; i++)
                sum += theFlight.Pax.GenderDstnClassList[i - 1].Female;
            return sum;
        }

        /// <summary> get amount of child Pax</summary>
        /// <returns>int: amount of child Pax</returns>
        private int Child()
        {
            int sum = 0;
            for (int i = 1; i < theFlight.Route.Length; i++)
                sum += theFlight.Pax.GenderDstnClassList[i - 1].Child;
            return sum;
        }

        /// <summary> get amount of infant Pax</summary>
        /// <returns>int: amount of infant Pax</returns>
        private int Infant()
        {
            int sum = 0;
            for (int i = 1; i < theFlight.Route.Length; i++)
                sum += theFlight.Pax.GenderDstnClassList[i - 1].Infant;
            return sum;
        }

        /// <summary> get amount of Pax</summary>
        /// <returns>int: amount of Pax</returns>
        private int TotalPax
        {
            get { return Male() + Female() + Child() + Infant(); }
        }

        /// <summary> get amount of PAX in first class</summary>
        /// <returns>int: amount of PAX in first class</returns>
        private int Class1Pax(int idx)
        {
            int sum = 0;
            for (int i = 1; i < theFlight.Route.Length; i++)
                sum += theFlight.Pax.ActlClsDstnClassList[i - 1].FrstPaxNumber;
            if (idx == 0) return sum;
            if (idx == 1) return theFlight.Pax.ActlClsDstnClassList[0].FrstPaxNumber;
            if (idx == 2) return theFlight.Pax.ActlClsDstnClassList[1].FrstPaxNumber;
            return 0;
        }

        /// <summary> get amount of PAX in second class</summary>
        /// <returns>int: amount of PAX in second class</returns>
        private int Class2Pax(int idx)
        {
            int sum = 0;
            for (int i = 1; i < theFlight.Route.Length; i++)
                sum += theFlight.Pax.ActlClsDstnClassList[i - 1].SryPaxNumber;
            if (idx == 0) return sum;
            if (idx == 1) return theFlight.Pax.ActlClsDstnClassList[0].SryPaxNumber;
            if (idx == 2) return theFlight.Pax.ActlClsDstnClassList[1].SryPaxNumber;
            return 0;
        }

        /// <summary> get amount of PAX in third class</summary>
        /// <returns>int: amount of PAX in third class</returns>
        private int Class3Pax(int idx)
        {
            int sum = 0;
            for (int i = 1; i < theFlight.Route.Length; i++)
                sum += theFlight.Pax.ActlClsDstnClassList[i - 1].LstPaxNumber;
            if (idx == 0) return sum;
            if (idx == 1) return theFlight.Pax.ActlClsDstnClassList[0].LstPaxNumber;
            if (idx == 2) return theFlight.Pax.ActlClsDstnClassList[1].LstPaxNumber;
            return 0;
        }

        /// <summary>
        ///  get SOC PAX in first class
        /// </summary>
        private int Class1SOC
        {
            get { return theFlight.ServiceItems.SOC.FrstPaxNumber; }
        }

        /// <summary>
        ///  get SOC PAX in second class
        /// </summary>
        private int Class2SOC
        {
            get { return theFlight.ServiceItems.SOC.SryPaxNumber; }
        }

        /// <summary>
        ///  get SOC PAX in third class
        /// </summary>
        private int Class3SOC
        {
            get { return theFlight.ServiceItems.SOC.LstPaxNumber; }
        }

        /// <summary>
        ///  get blocked seat amount 
        /// </summary>
        private int BLKDSeat
        {
            get { return theFlight.Pax.BlkdSeat; }
        }

        // part III
        /// <summary>
        /// get total Traffic load weight
        /// </summary>
        private int TrafficLoadWt
        {
            get
            {
                return Convert.ToInt32((IsKILOS) ? theFlight.TotalTrafficWt :
                    (int)Math.Round((float)theFlight.TotalTrafficWt * toPound, 0));  //#BR08111
                    //theFlight.TotalTrafficWt * toPound);//#BR08111
            }
        }

        /// <summary>
        /// get Dry operating weight
        /// </summary>
        private int DOW
        {
            //#BR08111 get { return Convert.ToInt32((IsKILOS) ? theFlight.DOW : theFlight.DOW * toPound); }
            get { return Convert.ToInt32((IsKILOS) ? theFlight.DOW : (int)Math.Round((float)theFlight.DOW * toPound, 0)); }  //#BR08111
        }

        /// <summary>
        /// get zero fuel weight
        /// </summary>
        private int ZFW
        {
            //#BR08111 get { return Convert.ToInt32((IsKILOS) ? theFlight.ZFW : theFlight.ZFW * toPound); }
            get { return Convert.ToInt32((IsKILOS) ? theFlight.ZFW : (int)Math.Round((float)theFlight.ZFW * toPound, 0)); }  //#BR08111
        }

        /// <summary>
        /// get Takeoff weight
        /// </summary>
        private int TakeoffFuel
        {
            get
            {
                return Convert.ToInt32((IsKILOS) ? theFlight.Fuel.TakeoffFuel :
                    (int)Math.Round((float)Convert.ToInt32(theFlight.Fuel.TakeoffFuel * toPound), 0));  //#BR08111
                    //Convert.ToInt32(theFlight.Fuel.TakeoffFuel * toPound));#BR08111-->
            }
        }

        /// <summary>
        /// get total operating weight
        /// </summary>
        private int TOW
        {
            //#BR08111get { return Convert.ToInt32((IsKILOS) ? theFlight.TOW : theFlight.TOW * toPound); }
            get { return Convert.ToInt32((IsKILOS) ? theFlight.TOW : (int)Math.Round((float)theFlight.TOW * toPound, 0)); }  //#BR08111
        }

        /// <summary>
        /// get trip fuel
        /// </summary>
        private int TripFuel
        {
            get
            {
                //#BR08111<--
                //				return Convert.ToInt32((IsKILOS) ? theFlight.Fuel.TripFuel :
                //					Convert.ToInt32(theFlight.Fuel.TripFuel * toPound));
                //#BR08111-->
                return Convert.ToInt32((IsKILOS) ? theFlight.Fuel.TripFuel :
                    (int)Math.Round((float)Convert.ToInt32(theFlight.Fuel.TripFuel * toPound), 0));  //#BR08111
            }
        }

        /// <summary>
        /// get landing weight
        /// </summary>
        private int LDW
        {
            //#BR08111 get { return Convert.ToInt32((IsKILOS) ? theFlight.LDW : theFlight.LDW * toPound); }
            get { return Convert.ToInt32((IsKILOS) ? theFlight.LDW : (int)Math.Round((float)theFlight.LDW * toPound, 0)); }  //#BR08111
        }

        /// <summary>
        /// get Maximum zero fuel weight
        /// </summary>
        private int MZFW
        {
            get
            {
                //#BR08111<--
                //				return Convert.ToInt32((IsKILOS) ?
                //					theFlight.ActualMZFW : theFlight.ActualMZFW * toPound);
                //#BR08111-->
                return Convert.ToInt32((IsKILOS) ?
                    theFlight.ActualMZFW : (int)Math.Round((float)theFlight.ActualMZFW * toPound, 0));  //#BR08111
            }
        }

        /// <summary>
        /// get Maximum takeoff weight
        /// </summary>
        private int MTOW
        {
            get
            {
                //#BR08111<--
                //				return Convert.ToInt32((IsKILOS) ?
                //					theFlight.ActualMTOW : theFlight.ActualMTOW * toPound);
                //#BR08111-->
                return Convert.ToInt32((IsKILOS) ?
                    theFlight.ActualMTOW : (int)Math.Round((float)theFlight.ActualMTOW * toPound, 0));  //#BR08111
            }
        }

        /// <summary>
        /// get Maximum landing weight
        /// </summary>
        private int MLDW
        {
            get
            {
                //#BR08111<--
                //				return Convert.ToInt32((IsKILOS) ?
                //					theFlight.ActualMLDW : theFlight.ActualMLDW * toPound);
                //#BR08111-->
                return Convert.ToInt32((IsKILOS) ?
                    theFlight.ActualMLDW : (int)Math.Round((float)theFlight.ActualMLDW * toPound, 0));  //#BR08111
            }
        }

        /// <summary>
        /// get Maximum deadload weight
        /// </summary>
        private int DLW
        {
            //#BR08111get { return Convert.ToInt32((IsKILOS) ? theFlight.DLW : theFlight.DLW * toPound); }
            get { return Convert.ToInt32((IsKILOS) ? theFlight.DLW : (int)Math.Round((float)theFlight.DLW * toPound, 0)); }  //#BR08111
        }

        /// <summary>
        /// get zero fuel weight limit
        /// </summary>
        private string ZFWLimit
        {
            get
            {
                return (theFlight.WeightLimitation == EnumWeightLimitation.MZFW) ?
                    "L" : "";
            }
        }

        /// <summary>
        /// get takeoff weight limit
        /// </summary>
        private string TOWLimit
        {
            get
            {
                return (theFlight.WeightLimitation == EnumWeightLimitation.MTOW) ? "L" : "";
            }
        }

        /// <summary>
        /// get landing weight limit
        /// </summary>
        private string LDWLimit
        {
            get
            {
                return (theFlight.WeightLimitation == EnumWeightLimitation.MLDW) ? "L" : "";
            }
        }

        // part IV
        /// <summary>
        /// get basic index
        /// </summary>
        private string BI
        {
            get { return Floating(theFlight.Aircraft.BI); }
        }

        /// <summary>
        /// get dry operating index
        /// </summary>
        private string DOI
        {
            get { return Floating(theFlight.DOI); }
        }

        /// <summary>
        /// get deadload index
        /// </summary>
        private string DLI
        {
            get { return Floating(theFlight.DLI); }
        }

        /// <summary>
        /// get MAC deadload weight
        /// </summary>
        private string MACDLW
        {
            get { return Floating(theFlight.MACDLW); }
        }

        /// <summary>
        /// get laden index of zero fuel weight
        /// </summary>
        private string LIZFW
        {
            get { return Floating(theFlight.LIZFW); }
        }

        private string MACZFW
        {
            get { return Floating(theFlight.MACZFW); }
        }


        /// <summary>
        /// get laden index of takeoff weight
        /// </summary>
        private string LITOW
        {
            get { return Floating(theFlight.LITOW); }
        }

        /// <summary>
        /// get MAC Takeoff weight
        /// </summary>
        private string MACTOW
        {
            get { return Floating(theFlight.MACTOW); }
        }

        /// <summary>
        /// CRZCG --- 777 specific
        /// </summary>
        private string CRZCG
        {
            get
            {
                //<!--#BR17169 THOMAS  CRZCG MAC計算規則更改，原MACZFW>MACTOW，則回MACTOW- 4.0，其他回MACZFW - 5.0，新規則僅扣1.5即可
                if (theFlight.ACType.name.IndexOf("77") >= 0)
                {
                    //return Floating((theFlight.MACZFW > theFlight.MACTOW) ? theFlight.MACTOW - 4.0 : theFlight.MACZFW - 5.0);
                    return Floating((theFlight.MACZFW > theFlight.MACTOW) ? theFlight.MACTOW - 1.5 : theFlight.MACZFW - 1.5);
                }
                //#BR17169 -->

                //<!-- #BR18111 THOMAS CRZCG顯示MACZFW跟MACTOW較低的那一個
                if (theFlight.ACType.name.IndexOf("78") >= 0)
                {
                    return Floating((theFlight.MACZFW > theFlight.MACTOW) ? theFlight.MACTOW : theFlight.MACZFW);
                }
                //#BR18111-->

                return "";
            }
        }

        /// <summary>
        /// get underload
        /// </summary>
        private double Underload
        {
            get
            {
                //#BR08111<--
                //				return (IsKILOS) ? theFlight.UnderLoad :
                //					theFlight.UnderLoad * toPound;
                //#BR08111-->
                //
                //return (IsKILOS) ? theFlight.UnderLoad :
                //    (int)Math.Round((float)theFlight.UnderLoad * toPound, 0);  //#BR08111

                return (IsKILOS) ? Math.Round(theFlight.UnderLoad, 0) : //#BR18106 THOMAS 四捨五入
                    (int)Math.Round((float)theFlight.UnderLoad * toPound, 0); 
            }
        }

        /// <summary>
        ///  string format conversion：
        /// for example：Convert "12.3" into "one two POINT three", and "12.0" into "one two POINT ZERO"</summary>
        /// <param name="f_2_1">string to be converted</param>
        /// <returns>string from conversion</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string Eddie(string f_2_1)
        {
            int pointPos;
            string tmpstr = "(";
            if ((pointPos = f_2_1.IndexOf(".")) >= 0)
            {
                for (int i = 0; i < pointPos; i++)
                    tmpstr += (ToString(f_2_1[i]) + " ");
                tmpstr += "DECIMAL ";
                tmpstr += ToString(f_2_1[pointPos + 1]);
            }
            else
            {
                for (int i = 0; i < f_2_1.Length; i++)
                    tmpstr += (ToString(f_2_1[i]) + " ");
                tmpstr += "POINT ZERO";
            }
            return tmpstr + ")";
        }

        /// <summary>Convert digit character into English naming string ；for example'3'->"three"</summary>
        /// <param name="c"> digit character to be converted</param>
        /// <returns>English naming string</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string ToString(char c)
        {
            switch (c)
            {
                case '0':
                    return "zero";
                case '1':
                    return "one";
                case '2':
                    return "two";
                case '3':
                    return "three";
                case '4':
                    return "four";
                case '5':
                    return "five";
                case '6':
                    return "six";
                case '7':
                    return "seven";
                case '8':
                    return "eight";
                case '9':
                    return "nine";
                default:
                    return "";
            }
        }

        // part V

        /// <summary> get Compartment Weight of some route</summary>
        /// <param name="routeIdx">Route index</param>
        /// <param name="index">Compartment index</param>
        /// <returns>double: Compartment Weight</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private double CmpWt(int routeIdx, int index)
        {
            //#BR08111<--
            //			return (IsKILOS) ?
            //				this.cmpList[index].getWeight(Route[routeIdx]) :
            //				this.cmpList[index].getWeight(Route[routeIdx]) * toPound;
            //#BR08111-->
            return (IsKILOS) ? this.cmpList[index].getWeight(Route[routeIdx]) : (int)Math.Round((float)this.cmpList[index].getWeight(Route[routeIdx]) * toPound, 0);  //#BR08111
        }

        /// <summary> get number of male of some route</summary>
        /// <param name="routeIdx">Route Index</param>
        /// <returns>number of male</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private int Male(int routeIdx)
        {
            return theFlight.Pax.GenderDstnClassList[routeIdx - 1].Male;
        }

        /// <summary> get number of female of some route</summary>
        /// <param name="routeIdx">Route Index</param>
        /// <returns>number of female</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private int Female(int routeIdx)
        {
            return theFlight.Pax.GenderDstnClassList[routeIdx - 1].Female;
        }

        /// <summary> get number of child of some route</summary>
        /// <param name="routeIdx">Route Index</param>
        /// <returns>number of child</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private int Child(int routeIdx)
        {
            return theFlight.Pax.GenderDstnClassList[routeIdx - 1].Child;
        }

        /// <summary> get number of infant of some route</summary>
        /// <param name="routeIdx">Route Index</param>
        /// <returns>number of infant</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private int Infant(int routeIdx)
        {
            return theFlight.Pax.GenderDstnClassList[routeIdx - 1].Infant;
        }

        /// <summary> get Compartment Total Weight of some route</summary>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private double CmpTotalWt(int routeIdx)
        {
            double sum = 0;
            for (int i = 0; i < this.cmpList.Length; i++)
                sum += this.cmpList[i].getWeight(Route[routeIdx]);
            //#BR08111			return (IsKILOS) ? sum : sum * toPound;
            return (IsKILOS) ? sum : (int)Math.Round((float)sum * toPound, 0);  //#BR08111
        }

        // part VI

        /// <summary>
        /// get basic weight
        /// </summary>
        private double BW
        {
            get
            {
                //return (IsKILOS) ? theFlight.Aircraft.BW : theFlight.Aircraft.BW * toPound;//#BR08111
                return (IsKILOS) ? theFlight.Aircraft.BW : (int)Math.Round((float)theFlight.Aircraft.BW * toPound, 0);  //#BR08111
            }
        }

        /// <summary>
        /// get pantry weight
        /// </summary>
        private double PantryWt
        {
            get
            {
                //#BR08111<--
                //return (IsKILOS) ? theFlight.Pantry.GetWeight() : theFlight.Pantry.GetWeight() * toPound;
                //#BR08111-->
                return (IsKILOS) ? theFlight.Pantry.GetWeight() : (int)Math.Round((float)theFlight.Pantry.GetWeight() * toPound, 0);  //#BR08111
            }
        }

        /// <summary>
        /// get pantry index
        /// </summary>
        private string PantryIndex
        {
            get
            {
                if (theFlight.Pantry.GetIndex() < 0)
                    return string.Format("{0}-", theFlight.Pantry.GetIndex() * -1);
                else return string.Format("{0}", theFlight.Pantry.GetIndex());
            }
        }

        /// <summary>
        /// get pantry code
        /// </summary>
        private string Pantry
        {
            get { return theFlight.Pantry.Code; }
        }

        /// <summary>
        /// get weight of fuel in tank, i.e. TakeoffFuel - TailTank
        /// </summary>
        private double Tanks
        {
            get
            {
                //#BR08111<--
                //				return (IsKILOS) ? theFlight.Fuel.TakeoffFuel - TailTank :
                //					(theFlight.Fuel.TakeoffFuel - TailTank) * toPound;
                //#BR08111-->
                return (IsKILOS) ? theFlight.Fuel.TakeoffFuel - TailTank : (int)Math.Round((float)(theFlight.Fuel.TakeoffFuel - TailTank) * toPound, 0);  //#BR08111
            }
        }

        /// <summary>
        /// get weight in tail tank fuel
        /// </summary>
        private double TailTank
        {
            get
            {
                //#BR08111<--
                //return (IsKILOS) ? theFlight.Fuel.TailFuel : theFlight.Fuel.TailFuel * toPound;
                //#BR08111-->
                return (IsKILOS) ? theFlight.Fuel.TailFuel : (int)Math.Round((float)theFlight.Fuel.TailFuel * toPound, 0);  //#BR08111
            }
        }

        /// <summary> get Net Freight Weight of some route</summary>
        /// <param name="routeIdx"></param>
        /// <returns>Net Freight Weight</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private double NetFreight(int routeIdx)
        {
            return Math.Round(IsKILOS ? theFlight.CargoPosnMgr.getFreNetWt(theFlight.Route[routeIdx]) : theFlight.CargoPosnMgr.getFreNetWt(theFlight.Route[routeIdx]) * toPound, 0);
        }

        /// <summary> get Net Post Weight of some route</summary>
        /// <param name="routeIdx"></param>
        /// <returns>Net Post Weight</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private double Post(int routeIdx)
        {
            return Math.Round((IsKILOS) ? theFlight.CargoPosnMgr.getPosNetWt(theFlight.Route[routeIdx]) :
                theFlight.CargoPosnMgr.getPosNetWt(theFlight.Route[routeIdx]) * toPound, 0);
        }

        /// <summary> get Baggage Pieces of some route</summary>
        /// <param name="routeIdx"></param>
        /// <returns>Baggage Pieces</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private int NetBagPiece(int routeIdx)
        {
            return theFlight.Pax.GetBagByDest(theFlight.Route[routeIdx]).BagPcs;
        }

        /// <summary> get Net Baggage Weight of some route</summary>
        /// <param name="routeIdx"></param>
        /// <returns>Net Baggage Weight</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private int NetBag(int routeIdx)
        {
            int result = theFlight.Pax.GetBagByDest(theFlight.Route[routeIdx]).BagWt;
            //#BR08111return (IsKILOS) ? result : Convert.ToInt32(result * toPound);
            return (IsKILOS) ? result : (int)Math.Round((float)Convert.ToInt32(result * toPound), 0);  //#BR08111
        }

        /// <summary> get TRA Net Weight of some route</summary>
        /// <param name="routeIdx"></param>
        /// <returns>TRA Net Weight</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private double TRA(int routeIdx)
        {
            CargoPosnMgr cargoPosnMgr = theFlight.CargoPosnMgr;
            string dest = theFlight.Route[routeIdx];
            double sum =
                cargoPosnMgr.getNetWt("O", dest, true) +
                cargoPosnMgr.getNetWt("C", dest, true) +
                cargoPosnMgr.getNetWt("B", dest, true) +
                cargoPosnMgr.getNetWt("M", dest, true);

            //#BR08111return Math.Round((IsKILOS) ? sum : sum * toPound, 0);
            return Math.Round((IsKILOS) ? sum : sum * toPound, 0);  //#BR08111
        }

        /// <summary>get Class Name by index</summary>
        /// <param name="index"></param>
        /// <returns>Class Name</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string ClassName(int index)
        {
            string cf = this.CabinConfig;
            int count = 0;
            for (int i = 0; i < cf.Length; i++)
                if (Char.IsLetter(cf, i))
                {
                    if (count == index) return cf.Substring(i, 1);
                    count++;
                }
            return "";
        }

        // LIR

        /// <summary> get Cargo Net Weight of some route</summary>
        /// <param name="route">Route Index</param>
        /// <returns>double: Cargo Net Weight</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private double CargoNetWt(int route)
        {
            return theFlight.CargoPosnMgr.getWeight(this.Route[route]);
        }

        /// <summary> get Post Net Weight of some route</summary>
        /// <param name="route">Route Index</param>
        /// <returns>double: Post Net Weight</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private double PostNetWt(int route)
        {
            return theFlight.CargoPosnMgr.getPosNetWt(this.Route[route]);
        }

        /// <summary> get Baggage Net Weight of some route</summary>
        /// <param name="route">Route Index</param>
        /// <returns>double: Baggage Net Weight</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private double BaggageNetWt(int route)
        {
            return theFlight.CargoPosnMgr.getBagNetWt(Route[route]);
        }

        #endregion

        #region LoadSheet

        /// <summary>Reorder (MainDeck+LowerDeck) to (LowerDeck+MainDeck) </summary>
        /// <param name="index"></param>
        /// <returns>int: remainings</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private int reOrder(int index)
        { // Reorder (MainDeck+LowerDeck) to (LowerDeck+MainDeck) 
            //                                0 1 2 3 4 5 6 7 8
            // Original order of compartments P R S T 1 2 3 4 5
            // return new order               1 2 3 4 5 P R S T
            // ie. given   return  Compartment
            //       0        4      0
            //       1        5      1
            //       4        8      5
            //       5        0      P
            //       8        3      T
            int start;
            for (start = 0; start < this.CmpLength; start++)
                if (Char.IsDigit(CmpName(start), 0)) break; // index of cmp '1'
            return (start + index) % this.CmpLength;
        }

        /// <summary>Creating Loadsheet String</summary>
        /// <returns>string: Loadsheet String</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string LoadSheet()
        {
            string unit;
            //theFlight.IsKILOS = false; //#BR08111

            if (IsKILOS) unit = "KILOS";
            else unit = "POUNDS";

            string strLS = "";

            // part I
            strLS += string.Format("{0,-20}{1,10}{2,20}{3,11}\r\n", "L O A D S H E E T", "CHECKED", "APPROVED/TIME", "EDNO");
            strLS += string.Format("{0,-15}{1,-8}{2,-14}{3,22:##,0#}\r\n\r\n", "ALL WEIGHTS IN", unit, User, LoadVersion);
            strLS += string.Format("{0,-8}{1,-14}{2,-8}{3,-13}{4,-8}{5,-8}{6,4}\r\n", "FROM/TO", "FLIGHT", "A/C REG", "VERSION", "CREW", "DATE", "TIME");
            strLS += string.Format("{0,-8}{1,-14}{2,-8}{3,-13}{4,-8}{5,-8}{6,4}\r\n", Route[0] + " " + Route[1], FullFlightNo, AcNum, CabinConfig, Crew, CurrentDate, CurrentTime);

            // part II
            strLS += string.Format("{0,30}{1,23}\r\n", "WEIGHT", "DISTRIBUTION");
            //BR09114<--Assume total number of compartments is at least 2
            if (this.CmpLength <= 2)
            {
                strLS += string.Format("{0,-24}{1,6}{2,6}/{3,6}{4,2}/{5,6}\r\n",
                    "LOAD IN COMPARTMENTS", CmpTotalWt(), CmpName(reOrder(0)), CmpWt(reOrder(0)),
                    CmpName(reOrder(1)), CmpWt(reOrder(1)));

                for (int i = 0; i < Math.Ceiling(CmpLength / 2.0) - 1; i++)
                {
                    if (3 + 3 * i == CmpLength - 1)
                        strLS += string.Format("{0,36}/{1,6}\r\n",
                            CmpName(reOrder(3 + 3 * i)), CmpWt(reOrder(3 + 3 * i)));
                }
            }
            else
            {
                //BR09114-->
                // Assume total number of compartments is at least 3
                strLS += string.Format("{0,-24}{1,6}{2,6}/{3,6}{4,2}/{5,6}{6,2}/{7,6}\r\n",
                    "LOAD IN COMPARTMENTS", CmpTotalWt(), CmpName(reOrder(0)), CmpWt(reOrder(0)),
                    CmpName(reOrder(1)), CmpWt(reOrder(1)), CmpName(reOrder(2)), CmpWt(reOrder(2)));

                for (int i = 0; i < Math.Ceiling(CmpLength / 3.0) - 1; i++)
                {
                    if (3 + 3 * i == CmpLength - 1)
                        strLS += string.Format("{0,36}/{1,6}\r\n",
                            CmpName(reOrder(3 + 3 * i)), CmpWt(reOrder(3 + 3 * i)));
                    else if (3 + 3 * i + 1 == CmpLength - 1)
                        strLS += string.Format("{0,36}/{1,6}{2,2}/{3,6}\r\n",
                            CmpName(reOrder(3 + 3 * i)), CmpWt(reOrder(3 + 3 * i)),
                            CmpName(reOrder(3 + 3 * i + 1)), CmpWt(reOrder(3 + 3 * i + 1)));
                    else
                        strLS += string.Format("{0,36}/{1,6}{2,2}/{3,6}{4,2}/{5,6}\r\n",
                            CmpName(reOrder(3 + 3 * i)), CmpWt(reOrder(3 + 3 * i)),
                            CmpName(reOrder(3 + 3 * i + 1)), CmpWt(reOrder(3 + 3 * i + 1)),
                            CmpName(reOrder(3 + 3 * i + 2)), CmpWt(reOrder(3 + 3 * i + 2)));
                }
            }
            strLS += string.Format("{0,-24}{1,6}{2,4}/{3,3}/{4,3}/{5,3}{6,4}{7,4}\r\n","PASSENGER/CABIN BAG", Convert.ToInt32(PaxTotalWt), Male(), Female(),Child(), Infant(), "TTL", TotalPax);
            strLS += string.Format("{0,34}{1,4}/{2,3}/{3,3}{4,4}{5,4}/{6,3}/{7,3}\r\n","PAX", Class1Pax(0), Class2Pax(0), Class3Pax(0), "SOC",Class1SOC, Class2SOC, Class3SOC);
            strLS += string.Format("{0,35}{1,4}\r\n", "BLKD", BLKDSeat);
            strLS += new string('.', 63);
            strLS += "\r\n";


            // part III

            strLS += string.Format("{0,-23}{1,7}\r\n","TOTAL TRAFFIC LOAD", TrafficLoadWt);
            strLS += string.Format("{0,-23}{1,7}\r\n","DRY OPERATING WEIGHT", DOW);
            strLS += string.Format("{0,-23}{1,7}{2,4}{3,7}{4,3}{5,6}\r\n","ZERO FUEL WEIGHT ACTUAL", ZFW, "MAX", MZFW, ZFWLimit, "ADJ");
            strLS += string.Format("{0,-23}{1,7}\r\n","TAKE OFF FUEL", TakeoffFuel);
            strLS += string.Format("{0,-23}{1,7}{2,4}{3,7}{4,3}{5,6}\r\n","TAKE OFF WEIGHT  ACTUAL", TOW, "MAX", MTOW, TOWLimit, "ADJ");
            strLS += string.Format("{0,-23}{1,7}\r\n","TRIP FUEL", TripFuel);
            strLS += string.Format("{0,-23}{1,7}{2,4}{3,7}{4,3}{5,6}\r\n","LANDING WEIGHT   ACTUAL", LDW, "MAX", MLDW, LDWLimit, "ADJ");
            // test if we need to display DLW. show DLW if DLW > 0
            if (theFlight.ACType.IsDLW) strLS += string.Format("{0,-23}{1,7}\r\n","DEADLOAD WEIGHT", DLW);

            // part IV
            strLS += string.Format("{0,63}\r\n", new string('.', 32));

            //<!--#BR19008 Thomas  所有機型LMC之後新增Tolerances限制值，修改 LAST MINUTES CHANGES 文字顯示。
            //strLS += string.Format("{0,-31}.{1,25}\r\n", "BALANCE AND SEATING CONDITIONS", "LAST MINUTES CHANGES");

                switch (theFlight.ACType.name.Substring(0, 4))
                {
                    case "BR32":
                        strLS += string.Format("{0,-31}.{1,25}\r\n","BALANCE AND SEATING CONDITIONS", "LAST MINUTES CHANGES 400");
                        break;
                    case "BR33":
                        strLS += string.Format("{0,-31}.{1,25}\r\n","BALANCE AND SEATING CONDITIONS", "LAST MINUTES CHANGES 500");
                        break;
                    case "BR77":
                        strLS += string.Format("{0,-31}.{1,25}\r\n","BALANCE AND SEATING CONDITIONS", "LAST MINUTES CHANGES 300");
                        break;
                    case "BR78":
                        if (TOW <= 247000)
                        {
                            strLS += string.Format("{0,-31}.{1,25}\r\n", "BALANCE AND SEATING CONDITIONS", "LAST MINUTES CHANGES 300");
                        }
                        else if (TOW > 247000)
                        {
                            strLS += string.Format("{0,-31}.{1,25}\r\n", "BALANCE AND SEATING CONDITIONS", "LAST MINUTES CHANGES 0");
                        }
                        break;
                    default:
                        break;
                }
              //#BR19008-->
            strLS += string.Format("{0,-8}{1,-7:##.#}{2,-8}{3,4}{4,36}\r\n","BI", BI, "DOI", DOI, ".DEST  SPEC    CL/CPT +/- WEIGHT");
            if (theFlight.ACType.IsDLW)
                strLS += string.Format("{0,-8}{1,-7:##.#}{2,-8}{3,4}{4,5}\r\n", "DLI", DLI, "MACDLW", MACDLW, ".");

            // Eddie Suggestion Format
            strLS += string.Format("{0,-8}{1,-7:#,##.0}{2,-8}{3,-7:#,##.0} .\r\n","LIZFW", LIZFW, "LITOW", LITOW);
            strLS += string.Format("{0,-8}{1,4:#,##.0}{2,-27}\r\n","MACZFW", MACZFW, Eddie(MACZFW.ToString()).ToUpper());

            //<!--#BR17208 THOMAS  判斷TOW在 LINE的哪一側，LOADSHEET 標註文字
            string strMark = CheckPoint("LS");  
            strLS += string.Format("{0,-8}{3}{1,4:#,##.0}{2,-27}\r\n","MACTOW", MACTOW, Eddie(MACTOW.ToString()).ToUpper(),strMark);
           // strLS += string.Format("{0,-8}{1,4:#,##.0}{2,-27}\r\n", "MACTOW", MACTOW, Eddie(MACTOW.ToString()).ToUpper());
            //#BR17208 -->

            //<!-- #BR18111 Thomas 增加787 機型顯示CRZCG
            //if (theFlight.ACType.name.IndexOf("77") >= 0 )
            if (theFlight.ACType.name.IndexOf("77") >= 0 || theFlight.ACType.name.IndexOf("78") >= 0)
                strLS += string.Format("{0,-8}{1,4:#,##.0}{2,-27}\r\n","CRZCG", this.CRZCG, Eddie(this.CRZCG).ToUpper());
            //#BR18111-->


            //BR10113<--
            //BR10121 if(theFlight.ACType.name.Substring(0,4) =="MF73")
            //BR12105if(theFlight.ACType.name.Substring(0,4) =="MF73" || theFlight.ACType.name.Substring(0,4) =="MF75") //BR10121 add MF75
            //BR12110if(theFlight.ACType.name.Substring(0,4) =="MF73" || theFlight.ACType.name.Substring(0,4) =="MF75" ||theFlight.ACType.name.Substring(0,4) =="HX33" ||theFlight.ACType.name.Substring(0,4) =="D733") //BR12105 add HX/D7
            if (theFlight.ACType.name.Substring(0, 4) == "MF73" || theFlight.ACType.name.Substring(0, 4) == "MF75" || theFlight.ACType.name.Substring(0, 4) == "HX33" || theFlight.ACType.name.Substring(0, 4) == "D733" || theFlight.ACType.name == "B7ATR") //BR12110 add ATR
            {
                StabilizerTrimItem[] stabTrims = theFlight.StabilizerTrim;
                if (stabTrims != null && stabTrims.Length > 0)
                {
                    strLS += "STABILIZER TRIM\r\n";
                    foreach (StabilizerTrimItem stabTrim in stabTrims)
                    {
                        strLS += string.Format("{0,-32}/{1}\r\n", stabTrim.Name.ToUpper(), Math.Round(stabTrim.Index, 1));
                    }
                    strLS += "\r\n";
                }
            }
            //BR10113-->

            // Total passengers in Zones and SOC
            string tmpstr = "";
            foreach (ZoneItem item in theFlight.Pax.Zonelist)
            {
                tmpstr += string.Format("{0}{1}.", item.ZoneName, item.TtlPax);
            }

            tmpstr += string.Format("SOC{0}.", theFlight.ServiceItems.SOC.FrstPaxNumber + theFlight.ServiceItems.SOC.SryPaxNumber + theFlight.ServiceItems.SOC.LstPaxNumber);

            if (tmpstr.Length < 31)
                tmpstr += new string(' ', 31 - tmpstr.Length);
            tmpstr += ".\r\n";
            strLS += tmpstr;

            if (theFlight.Pax.BTrimByZone)
                tmpstr = "CABIN AREA TRIM";
            else
                tmpstr = "SEATROW TRIM";
            if (tmpstr.Length < 31)
                tmpstr += new string(' ', 31 - tmpstr.Length);

            tmpstr += ".\r\n";
            strLS += tmpstr;

            if (theFlight.ACType.cpi != null)
                strLS += string.Format("{0,-8}{1,-7}{2,17}\r\n", "CPI", theFlight.DisplayCPI, ".");

            strLS += string.Format("{0,32}\r\n", ".");
            strLS += string.Format("{0,-25}{1,6}.{2,20}\r\n", "UNDERLOAD BEFORE LMC", Underload, "LMC TOTAL");
            strLS += new string('.', 63);
            strLS += "\r\n";

            // part V
            strLS += "LOADMESSAGE AND CAPTAINS INFORMATION BEFORE LMC\r\n";
            strLS += "\r\n";

            //#BR08111<--
            //			strLS += string.Format("{0,-9}{1,6}{2,9}{3,9}{4,4}{5,9}\r\n",
            //			                       "TAXI FUEL", theFlight.Fuel.TaxiFuel, "TAXI WGT", theFlight.TOW + theFlight.Fuel.TaxiFuel,
            //			                       "MAX", theFlight.MTW);
            //#BR08111-->

            //#BR08111<--
            if (IsKILOS)
            {
                strLS += string.Format("{0,-9}{1,6}{2,9}{3,9}{4,4}{5,9}\r\n", "TAXI FUEL", theFlight.Fuel.TaxiFuel, "TAXI WGT", theFlight.TOW + theFlight.Fuel.TaxiFuel, "MAX", theFlight.MTW);
            }
            else
            {
                strLS += string.Format("{0,-9}{1,6}{2,9}{3,9}{4,4}{5,9}\r\n", "TAXI FUEL", (int)Math.Round((float)theFlight.Fuel.TaxiFuel * toPound, 0), "TAXI WGT", 
                    (int)Math.Round((float)(theFlight.TOW + theFlight.Fuel.TaxiFuel) * toPound, 0), "MAX", (int)Math.Round((float)theFlight.MTW * toPound, 0));
            }
            //#BR08111-->

            strLS += "\r\n";
            strLS += LoadMessage();
            strLS += "\r\n";
            // part part VI
            strLS += LoadsheetSI();

            return strLS;
        }

        #endregion

        #region LSHeaderLine

        public string LSHeaderLine
        {
            get
            {
                return string.Format("{0,-8}{1,5}{2,4}{3,4} {4,-10}\r\n",
                                     Flt_no, Date.Substring(0, 5), Route[0], Route[Route.Length - 1], Time);
            }

        }

        #endregion

        #region LoadMessage

        /// <summary>Creating Load Message</summary>
        /// <returns>string: Load Message</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string LoadMessage()
        {
            // Load Message for each route
            string strLS = ""; // load message full string
            string loadmsg = ""; // load message per line
            string tmpstr;
            for (int i = 1; i < Route.Length; i++)
            {
                loadmsg += string.Format("-{0}.{1}/{2}/{3}/{4}.T{5}", Route[i],
                                         Male(i), Female(i), Child(i), Infant(i), CmpTotalWt(i));

                // list each compartment's name and weight for each route
                for (int j = 0; j < this.CmpLength; j++)
                {
                    if (CmpWt(i, reOrder(j)) == 0) continue;

                    tmpstr = "." + CmpName(reOrder(j)) + "/" + CmpWt(i, reOrder(j));
                    if (loadmsg.Length + tmpstr.Length > LineWidth)
                    {
                        strLS += (loadmsg + "\r\n");
                        loadmsg = tmpstr;
                    }
                    else loadmsg += tmpstr;
                }
                // PAX by class
                tmpstr = string.Format(".PAX/{0}/{1}/{2}", Class1Pax(i), Class2Pax(i), Class3Pax(i));
                if (loadmsg.Length + tmpstr.Length > LineWidth)
                {
                    strLS += (loadmsg + "\r\n");
                    loadmsg = tmpstr;
                }
                else loadmsg += tmpstr;
                // PAD 
                tmpstr = ".PAD/0/0/0";
                if (loadmsg.Length + tmpstr.Length > LineWidth)
                {
                    strLS += (loadmsg + "\r\n");
                    loadmsg = tmpstr;
                }
                else loadmsg += tmpstr;
                if (i == 1)
                {
                    // XCR   1021
                    tmpstr = ".XCR";
                    foreach (CrewItem item in theFlight.Crew.ExtraCrews)
                    {
                        tmpstr += ("/" + (item.Cockpit + item.Cabin));
                    }
                    if (tmpstr != ".XCR/0/0/0")
                    {
                        if (loadmsg.Length + tmpstr.Length > LineWidth)
                        {
                            strLS += (loadmsg + "\r\n");
                            loadmsg = tmpstr;
                        }
                        else loadmsg += tmpstr;
                    }
                }
                // Cargo Consignment especially for SHC
                tmpstr = "";
                foreach (CargoPosnBase pos in theFlight.CargoPosnMgr.GenVisibleList(0))
                    foreach (SpecialLoad csgn in theFlight.CargoPosnMgr.SHCConsignments(pos))
                    {
                        if (csgn.Dest.Equals(this.Route[i]))
                        {
                            if (csgn.SHC == "")
                                continue;
                            tmpstr = ("." + csgn.SHC);
                            tmpstr += ("/" + pos.FindParent(typeof(CargoCmpt)).Name); // 0719
                            if (csgn.Pieces > 0) tmpstr += ("/" + csgn.Pieces);
                            //BR13107<--
                            if (IsKILOS)
                            {
                                if (csgn.Weight > 0) tmpstr += ("/" + csgn.Weight);
                            }
                            else
                            {
                                if (csgn.Weight > 0) tmpstr += ("/" + (int)Math.Round((float)csgn.Weight * toPound, 0));
                            }
                            //BR13107-->
                            //BR13107 if (csgn.Weight > 0) tmpstr += ("/" + csgn.Weight);
                            if (loadmsg.Length + tmpstr.Length > LineWidth)
                            {
                                strLS += (loadmsg + "\r\n");
                                loadmsg = tmpstr;
                            }
                            else loadmsg += tmpstr;
                        }
                    }

                strLS += loadmsg + "\r\n";
                loadmsg = "";
            }
            return strLS;
        }

        #endregion

        #region LoadsheetSI

        /// <summary>Creating Loadsheet SI</summary>
        /// <returns>string: Loadsheet SI</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string LoadsheetSI()
        {
            string strSI = "";

            strSI += "SI";
            strSI += "\r\n";
            
            strSI += string.Format("BW{0, 6} BI{1,6} CATERING{2,5}/{3}\r\n",
                                   BW, BI, PantryWt, PantryIndex);

            //<!--#BR18107 THOMAS 判斷 TOW是否在EXPD AREA區域內，若在區域內 在LS顯示
            string str = CheckExpdArea("LS");
            if (str != "")
            {
                strSI += str;
                strSI += "\r\n";
            }

            //#BR18107-->

            if (theFlight.Telex.LDM.SI != "")
                strSI += theFlight.Telex.LDM.SI + "\r\n";

            //			//BR10113<--
            //			if(theFlight.ACType.name.Substring(0,4) =="MF73")
            //			{
            //				StabilizerTrimItem[] stabTrims = theFlight.StabilizerTrim;
            //				if(stabTrims!=null && stabTrims.Length>0)
            //				{
            //					strSI += "STABILIZER TRIM\r\n";
            //					foreach (StabilizerTrimItem stabTrim in stabTrims)
            //					{
            //						strSI +=string.Format("{0,-32}/{1}\r\n", stabTrim.Name.ToUpper(), Math.Round(stabTrim.Index,1));
            //					}
            //					strSI += "\r\n";
            //				}
            //			}
            //			//BR10113-->

            strSI += "SERVICE WEIGHT ADJ WGT/IND\r\n";
            strSI += "ADD";
            strSI += "\r\n";

            bool noItem = true;
            foreach (SvcItem item in theFlight.ServiceItems.ServicesList)
                if (item.Weight > 0)
                {
                    noItem = false;
                    strSI += this.formatServiceItem(item.Desc, item.Index, item.Weight);
                }
            if (noItem) strSI += "NIL\r\n"; // NO ADD   

            noItem = true;
            strSI += "DEDUCTIONS";
            strSI += "\r\n";
            foreach (SvcItem item in theFlight.ServiceItems.ServicesList)
                if (item.Weight < 0)
                {
                    noItem = false;
                    strSI += this.formatServiceItem(item.Desc, item.Index, item.Weight);
                }
            if (noItem) strSI += "NIL\r\n"; // NO DEDUCTIOS  

            strSI += "\r\n";

            strSI += string.Format("PANTRY CODE {0}\r\n", Pantry);

            if (theFlight.Fuel.FuelLoading == FuelLoadingStyle.NSTD)
                strSI += fuelLoadingStyle;

            if (theFlight.Fuel.IdxTailTank >= 0)
                strSI += string.Format("{0,-16}{1,9}{2,11}{3,9}\r\n",
                    "WING/CENTRE TANK", Tanks, "TAIL TANK", TailTank);
            else
                strSI += string.Format("{0,-16}{1,9}\r\n", "WING/CENTRE TANK", Tanks);

            strSI += "AUTHORISED WEIGHTS USED FOR PASSENGERS CREW AND BAGGAGE\r\n";

            strSI += "\r\n";

            for (int rid = 1; rid < Route.Length; rid++)
                strSI += string.Format("{0, -6}FRE{1,8}   POS{2,8}   BAG{3,4}/{4,6}   TRA{5,8}\r\n",
                                       Route[rid], NetFreight(rid), Post(rid),
                                       NetBagPiece(rid), NetBag(rid), TRA(rid));

            return strSI;
        }


        /// <summary>Put the formated Service Item into string</summary>
        /// <param name="name">Item Name</param>
        /// <param name="index">Item index</param>
        /// <param name="weight">Item weight</param>
        /// <returns>string:  string of the formated Service Item data</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string formatServiceItem(string name, double index, double weight)
        {
            string rtn;
            if (index < 0)
                rtn = string.Format(" {0,-20}XI{1,4}-{2,15}  {3,4:#}-\r\n",
                                    name.ToUpper(), Floating(Math.Abs(index)), weight, Floating(Math.Abs(index)));
            else
                rtn = string.Format(" {0,-20}XI{1,4}{2,16}  {3,4}\r\n",
                                    name.ToUpper(), Floating(index), weight, Floating(index));
            return rtn;
        }

        #endregion

        #region ACARS: Radio Loadsheet

        /// <summary>Creating ACARS String</summary>
        /// <param name="real">with ACARS equipment and function</param>
        /// <returns>ACARS String</returns>
        /// <remarks>
        /// Modified date : 2006/09/18
        /// Modified by : Fio Sun
        /// Modified Reason : Change radio loadsheet format , let it words less than 400 characters
        /// </remarks> 
        public string ACARS(bool real, string sender) // real indicate if it is a real ACARS
        {
            string unit;

            //theFlight.IsKILOS = false;  //#BR08111
            if (IsKILOS) unit = "TON";
            else unit = "Klb"; //#BR08111
            //#BR08111 else unit = "KILOPOUNDS";

            string acars = "";

            if (sender == null || sender == "") return "";

            // ACARS LoadSheet part I
            if (real) // M32
            {
                acars += "ELS\r\n";
                acars += string.Format("AN {0}/FI {1}\r\n", this.AcNum, this.Flt_no);
                acars += string.Format("-  W/B{0}{1:ddHHmm}\r\n", sender, DateTime.Now.ToUniversalTime());
            }
            else // M33
            {
                acars += "DLS\r\n";
                acars += string.Format("AN {0}/FI {1}\r\n", this.AcNum, this.Flt_no);
                acars += string.Format("-  P40{0}{1:ddHHmm}\r\n", sender, DateTime.Now.ToUniversalTime());
            }

            // ACARS LoadSheet part II
            //BR08109 acars += "RADIO LOADSHEET\r\n"; //"ACARS\r\n";
            acars += "LOADSHEET\r\n"; //"ACARS\r\n";  //BR08109

            //BR16113  FullFlightNo:BR0851 /15NOV => BR0851/15
            //acars += string.Format("{0,-10}01\r\n", string.Format("{0}/{1:#,0#}", this.Flt_no, DateTime.Now.Day));
            acars += string.Format("{0,-10}01\r\n", FullFlightNo.Substring(0, FullFlightNo.Length - 3).Replace(" ", ""));
            //BR16113

            //*****EX:1224Z 06SEP  EDNO 01
            DateTime utc = DateTime.Now.ToUniversalTime();
            acars += string.Format("{0:HHmm}Z {1} EDNO {2:#,0#}\r\n", utc, utc.ToString("ddMMM", new CultureInfo("en-US")).ToUpper(), theFlight.LoadsheetVersion);
            //*****

            //*****Flight Number, From, To  EX:TPE-BKK  B-16462			 
            acars += string.Format("{0}-{1} {2}\r\n", theFlight.Route[0], theFlight.Route[1], this.AcNum);
            //*****

            // Aircraft Registration Number and Loadsheet Version
            ////acars += string.Format("{0,-15}EDNO {1:#,0#}\r\n", theFlight.ACRegNo, theFlight.LoadsheetVersion);

            // ACARS LoadSheet part III
            // BW
            ////acars += string.Format("              BW {0,5}\r\n", Math.Round((float) this.BW / 1000.0, 1));

            //*****Crew EX:CREW 5/19
            acars += string.Format("CREW {0}\r\n", this.Crew);
            //*****

            //*****PAX, TTL EX:PAX 7/89/214  TTL 310+1 INF
            acars += string.Format("PAX {0}/{1}/{2}  TTL {3}", this.Class1Pax(0), this.Class2Pax(0), this.Class3Pax(0), this.TotalPax - this.Infant());

            if (this.Infant() > 0)
                acars += string.Format("+{0} INF\r\n", this.Infant());
            else
                acars += "\r\n";
            //*****

            //***** TPLD: Traffic Load  EX:TPLD 40.2
            //acars += string.Format("TPLD {0,3}\r\n", Math.Round((float) this.TrafficLoadWt / 1000.0,2,MidpointRounding.AwayFromZero));
            acars += string.Format("TPLD {0,3}\r\n", ((decimal)this.TrafficLoadWt / 1000).ToString("#.00"));  //BR16113 四捨五入=>35.345 原為35.34改成35.35
            //*****

            // <-- #BR15112 -JayS, added DOW
            //acars += string.Format("DOW {0,3}\r\n", Math.Round(theFlight.DOW / 1000, 1));
            acars += string.Format("DOW {0,3}\r\n", (theFlight.DOW / 1000).ToString("#.0")); //BR16113
            // #BR15112 -->

            //<--#BR08013
            //			//***** ZFW, MZFW EX:ZFW 226.2 L MZFW 242.7
            //			acars += string.Format("ZFW {1,-5:#,###.0}{2,2} MZFW {0,5:#,###.0}\r\n",
            //				Math.Round((float) this.MZFW / 1000, 1), Math.Round((float) this.ZFW / 1000, 1), this.ZFWLimit);
            //			//*****
            //#BR08013-->

            //<--#BR08013
            //***** ZFW, MZFW EX:ZFW 226.2 L MZFW 242.7 ->change to:ZFW 226.2 L M 242.7 
            //BR12107			acars += string.Format("ZFW {1,-5:#,###.0}{2,2} M {0,5:#,###.0}\r\n",          
            //BR12107				Math.Round((float) this.MZFW / 1000, 1), Math.Round((float) this.ZFW / 1000, 1), this.ZFWLimit);

            //BR16113<--
            // <-- #BR15115 if A330 & unusable fuel > 0 then remove ZFW ()
            //ZFW (153.3)   M 168.0

            //theFlight.ACType.FullAirTypeName.IndexOf("74") > -1 || theFlight.ACType.FullAirTypeName.IndexOf("77") 

            //BR16114<--
            if (theFlight.ServiceItems.UnusableFuel > 0)
            {
                //777及747機型，因SI新增MACZFW及ZFW FOR FMS已加括號，故將原ZFW的括號取消。
                if (theFlight.ACType.name.Substring(2, 2) == "33" || theFlight.ACType.name.Substring(2, 2) == "32" || theFlight.ACType.FullAirTypeName.IndexOf("74") > -1 || theFlight.ACType.FullAirTypeName.IndexOf("77") > 1)
                {

                    acars += string.Format("ZFW {1,-5:#,###.0}{2,2} M {0,5:#,###.0}\r\n", ((float)this.MZFW / 1000).ToString("#.0"), ((float)this.ZFW / 1000).ToString("#.0"), this.ZFWLimit);
                }
                else
                {
                    acars += string.Format("ZFW ({1,-5:#,###.0}){2,2} M {0,5:#,###.0}\r\n", ((float)this.MZFW / 1000).ToString("#.0"), ((float)this.ZFW / 1000).ToString("#.0"), this.ZFWLimit);
                }
            }
            else
            {
                acars += string.Format("ZFW ({1,-5:#,###.0}){2,2} M {0,5:#,###.0}\r\n", ((float)this.MZFW / 1000).ToString("#.0"), ((float)this.ZFW / 1000).ToString("#.0"), this.ZFWLimit);
            }

            //if(theFlight.ACType.name.Substring(2, 2) == "33" && theFlight.ServiceItems.UnusableFuel > 0)
            //{
            //acars += string.Format("ZFW {1,-5:#,###.0}{2,2} M {0,5:#,###.0}\r\n", Math.Round((float) this.MZFW / 1000, 1), Math.Round((float) this.ZFW / 1000, 1), this.ZFWLimit);
            //	acars += string.Format("ZFW {1,-5:#,###.0}{2,2} M {0,5:#,###.0}\r\n", ((float) this.MZFW / 1000).ToString("#.#"), ((float) this.ZFW / 1000).ToString("#.#"), this.ZFWLimit);
            //}
            //else

            //acars += string.Format("ZFW ({1,-5:#,###.0}){2,2} M {0,5:#,###.0}\r\n", Math.Round((float) this.MZFW / 1000, 1), Math.Round((float) this.ZFW / 1000, 1), this.ZFWLimit);
            //  acars += string.Format("ZFW ({1,-5:#,###.0}){2,2} M {0,5:#,###.0}\r\n", ((float) this.MZFW / 1000).ToString("#.#"), ((float) this.ZFW / 1000).ToString("#.#"), this.ZFWLimit);
            // #BR15115 -->
            //BR15115 acars += string.Format("ZFW ({1,-5:#,###.0}){2,2} M {0,5:#,###.0}\r\n",
            //BR15115	Math.Round((float) this.MZFW / 1000, 1), Math.Round((float) this.ZFW / 1000, 1), this.ZFWLimit);  //BR12107
            //BR16113-->

            //*****
            //#BR08013-->
            //BR16114-->

            //*****TOF: Takeoff Fuel EX:TOF  38.0
            acars += string.Format("TOF {0:#,###.0}\r\n", ((float)this.TakeoffFuel / 1000).ToString("#.0")); //BR16113
            //*****

            //<--#BR08013
            //			//*****TOW, MTOW EX:TOW 264.2   MTOW 317.5
            //			acars += string.Format("TOW {1, -5:#,###.#}{2,2} MTOW {0,5:#,###.0}\r\n",
            //				Math.Round((float) this.MTOW / 1000, 1), Math.Round((float) this.TOW / 1000, 1), this.TOWLimit);
            //			//*****
            //<--#BR08013

            //<--#BR08013
            //*****TOW, MTOW EX:TOW 264.2   MTOW 317.5 -> change to EX:TOW 264.2   M 317.5
            //BR12107			acars += string.Format("TOW {1, -5:#,###.#}{2,2} M {0,5:#,###.0}\r\n",
            //BR12107				Math.Round((float) this.MTOW / 1000, 1), Math.Round((float) this.TOW / 1000, 1), this.TOWLimit);

            acars += string.Format("TOW ({1, -5:#,###.0}){2,2} M {0,5:#,###.0}\r\n",
                ((float)this.MTOW / 1000).ToString("#.0"), ((float)this.TOW / 1000).ToString("#.0"), this.TOWLimit);  //BR12107 //BR16113

            //*****
            //#BR08013-->

            //*****TPF: Trip Fuel   EX:TIF   27.3
            //#BR08111 acars += string.Format("TIF {0:#,###.0}\r\n", Math.Round((float) this.TripFuel / 1000, 1));
            //#BR08111 acars += string.Format("TIF {0:#,###.0}\r\n", Math.Round((float) this.TripFuel / 1000 * toPound, 1));  //#BR08111
            acars += string.Format("TIF {0:#,###.0}\r\n", ((float)this.TripFuel / 1000).ToString("#.0"));  //#BR08111 //BR16113
            //*****

            //<--#BR08105
            //			//*****LDW, MLDW EX:LDW 236.9   MLDW 285.8
            //			acars += string.Format("LDW {1,-5:#,###.0}{2,2} MLDW {0,5:#,###.0}\r\n", Math.Round((float) this.MLDW / 1000, 1), Math.Round((float) this.LDW / 1000, 1),this.LDWLimit);
            //			
            //			//*****
            //#BR08105-->

            //<--#BR08105
            //*****LDW, MLDW EX:LDW 236.9   MLDW 285.8 -> change to EX:LDW 236.9   M 285.8
            //#BR08111 acars += string.Format("LDW {1,-5:#,###.0}{2,2} M {0,5:#,###.0}\r\n", Math.Round((float) this.MLDW / 1000, 1), Math.Round((float) this.LDW / 1000, 1),this.LDWLimit);
            //#BR08111 acars += string.Format("LDW {1,-5:#,###.0}{2,2} M {0,5:#,###.0}\r\n", Math.Round((float) this.MLDW / 1000 * toPound, 1), Math.Round((float) this.LDW / 1000 * toPound, 1),this.LDWLimit);  //#BR08111
            //acars += string.Format("LDW {1,-5:#,###.0}{2,2} M {0,5:#,###.0}\r\n", Math.Round((float) this.MLDW / 1000, 1), Math.Round((float) this.LDW / 1000, 1),this.LDWLimit);  //#BR08111  
            acars += string.Format("LDW {1,-5:#,###.0}{2,2} M {0,5:#,###.0}\r\n", ((float)this.MLDW / 1000).ToString("#.0"), ((float)this.LDW / 1000).ToString("#.0"), this.LDWLimit);  //BR16113
            acars += string.Format("WTS IN {0}\r\n", unit);  //#BR09118

            //*****
            //#BR08105-->

            // UNDLD: Underload
            //#BR08111acars += string.Format("UNDLD {0} KG\r\n", this.Underload); 
            //#BR08111<--
            if (IsKILOS)
                acars += string.Format("UNDLD {0} KG\r\n", this.Underload);
            else
                acars += string.Format("UNDLD {0} LB\r\n", this.Underload);
            //#BR08111-->


            //<--#BR08105
            //			//*****BI,DOI EX:BI  73.1  DOI  69.6
            //			//2006/10/16 the Second Modify by FioSun
            //			//acars += string.Format("BI  {0,4:#,##.0}  DOI  {1:-4#,##.0}\r\n", this.BI,this.DOI);
            //			acars += string.Format("BI  {0,4:#,##.0}\r\n", this.BI,this.DOI);
            //			//*****
            //#BR08105-->

            // DOI, PAX - class1
            ////acars += string.Format("DOI     {0,4:#,##.0}   PAX{1,4}\r\n", this.DOI, this.Class1Pax(0));

            //*****LIZFW, LITOW
            acars += string.Format("LIZFW {0,4:#,##,0} LITOW {1,4:#,##,0}\r\n", this.LIZFW, this.LITOW);

            //*****

            // LITOW, PAX - class3
            ////acars += string.Format("LITOW   {0,4:#,##,0}{1,10}\r\n", this.LITOW, this.Class3Pax(0));

            // MACZFW, MACTOW
            // #BR15115 <-- 2015/10/08 Jay - removed paranthesis when ac type is A330 & unusable fuel is greater than 0
            if (theFlight.ACType.name.Substring(2, 2) == "33" && theFlight.ServiceItems.UnusableFuel > 0)
            {
                //<!--#BR17208 THOMAS  判斷TOW在 LINE的哪一側，ELS 標註文字
                string strMark = CheckPoint("ELS");
                acars += string.Format("MACZFW {0,4:#,##.0} MACTOW ({2}{1,4:#,##.0})\r\n", this.MACZFW, this.MACTOW, strMark);
                //acars += string.Format("MACZFW {0,4:#,##.0} MACTOW {1,4:#,##.0}\r\n", this.MACZFW, this.MACTOW);
                //#BR17208 -->
            }
            else if (theFlight.ACType.name.Substring(2, 2) == "33" || theFlight.ACType.name.Substring(2, 2) == "M1")
            //if (theFlight.ACType.name.Substring(2,2) == "33" || theFlight.ACType.name.Substring(2,2) == "M1")
            // #BR15115 -->
            //BR12107acars += string.Format("MACZFW {0,4:#,##.0} MACTOW {1,4:#,##.0}\r\n", this.MACZFW, this.MACTOW);
            //BR12107-1acars += string.Format("MACZFW {0,4:#,##.0} MACTOW ({1,4:#,##.0})\r\n", this.MACZFW, this.MACTOW);  //BR12107
            //BR12107-1<--	
            {
                //MACZFW (25.3) MACTOW 27.4
                //<!--#BR17208 THOMAS  判斷TOW在 LINE的哪一側，ELS 標註文字
                string strMark = CheckPoint("ELS");
                acars += string.Format("MACZFW ({0,4:#,##.0}) MACTOW ({2}{1,4:#,##.0})\r\n", this.MACZFW, this.MACTOW, strMark);  //BR12107-1
                // acars += string.Format("MACZFW ({0,4:#,##.0}) MACTOW {1,4:#,##.0}\r\n", this.MACZFW, this.MACTOW);  //BR12107-1
                //#BR17208 -->
            }
            else
            {
                //MACZFW 29.8 MACTOW (30.1)
                //<!--#BR17208 THOMAS  判斷TOW在 LINE的哪一側，ELS 標註文字
                string strMark  =CheckPoint("ELS");
                acars += string.Format("MACZFW {0,4:#,##.0} MACTOW ({2}{1,4:#,##.0})\r\n", this.MACZFW, this.MACTOW, strMark);  //BR12107-1
                //acars += string.Format("MACZFW {0,4:#,##.0} MACTOW ({1,4:#,##.0})\r\n", this.MACZFW, this.MACTOW);  //BR12107-1
                //#BR17208 -->
            }

            //BR12107-1-->

            //if (this.Infant() > 0)
            //  acars += string.Format(" PLUS{0,3} INF\r\n", this.Infant());
            //else
            //  acars += "\r\n";

            // MACTOW
            ////acars += string.Format("MACTOW  {0,4:#,##.0}\r\n", this.MACTOW);
            // MACDLW
            if (theFlight.ACType.IsDLW)
                acars += string.Format("MACDLW {0,4:#,##.0}\r\n", this.MACDLW);

            // CRZCG for 777 787
             //<!-- #BR18111 Thomas 增加787 機型顯示CRZCG
            //if (theFlight.ACType.name.IndexOf("77") >= 0
            if (theFlight.ACType.name.IndexOf("77") >= 0 || theFlight.ACType.name.IndexOf("78") >= 0)
            //#BR18111-->
            {
                //BR12107acars += string.Format("CRZCG {0,4:#,##.0}\r\n", this.CRZCG);
                acars += string.Format("CRZCG ({0,4:#,##.0})\r\n", this.CRZCG);  //BR12107
            }

            //BR10113<--
            if (theFlight.ACType.name.Substring(0, 4) == "MF73")
            {
                StabilizerTrimItem[] stabTrims = theFlight.StabilizerTrim;
                if (stabTrims != null && stabTrims.Length > 0)
                {
                    if (theFlight.Telex.LDM.SI == "") acars += "SI\r\n";
                    acars += "STABILIZER TRIM\r\n";
                    foreach (StabilizerTrimItem stabTrim in stabTrims)
                    {
                        //BR16113<--
                        //acars +=string.Format("{0,-32}/{1}\r\n", stabTrim.Name.ToUpper(), Math.Round(stabTrim.Index,1));
                        acars += string.Format("{0,-32}/{1}\r\n", stabTrim.Name.ToUpper(), ((float)stabTrim.Index).ToString("#.0"));
                        //BR16113-->
                    }
                    acars += "\r\n";
                }
            }
            //BR10113-->

            
            // ACARS LoadSheet part IV
            // SI   1104
            if (theFlight.Telex.LDM.SI != "")
            {
                acars += ("SI\r\n" + theFlight.Telex.LDM.SI + "\r\n");
            }

            //<!--#BR18107  THOMAS 判斷 TOW是否在EXPD AREA區域內，若在區域內 在ELS顯示
            string str = CheckExpdArea("ELS");

            if (str != "")
            {
                acars += ("SI\r\n" + str + "\r\n");
            }

            //#BR18107-->


            // ADD/DEDUCTION Service Items
            bool noItem = true;
            acars += "ADD";
            acars += "\r\n";
            foreach (SvcItem item in theFlight.ServiceItems.ServicesList)
                if (item.Weight > 0)
                {
                    noItem = false;
                    //#BR08111<--					
                    acars += string.Format("{0,-19}{1,6}{2,5:#,0.#}\r\n", item.Desc, item.Weight, item.Index);
       
                    //if(IsKILOS)
                    //  acars += string.Format("{0,-19}{1,6}{2,5:#,0.#}\r\n",item.Desc, Math.Round((float)item.Weight * toPound,0), item.Index);
                    //else
                    //  acars += string.Format("{0,-19}{1,6}{2,5:#,0.#}\r\n",item.Desc, item.Weight, item.Index);
                    //#BR08111-->

                }
            if (noItem)
                acars += "NIL\r\n";

            //			noItem = true;
            bool first = true;
            foreach (SvcItem item in theFlight.ServiceItems.ServicesList)
                if (item.Weight < 0)
                {
                    //noItem = false;
                    if (first)
                    {
                        first = false;
                        acars += "DEDUCT";
                        acars += "\r\n";
                    }
                    //#BR08111<--
                    acars += string.Format("{0,-19}{1,6}{2,5:#,0.#}\r\n", item.Desc, item.Weight, item.Index);
    
                    //acars += string.Format("{0,-19}{1,6}{2,5:#,0.#}\r\n", item.Desc, Math.Round((float)item.Weight * toPound,0), item.Index);
                    //#BR08111-->
                }

            //#BR08105			acars += string.Format("PANTRY CODE {0}\r\n", Pantry);

            //#BR08111<--
            if (theFlight.Fuel.IdxTailTank >= 0)
                acars += string.Format("TAIL TANK  {0}\r\n", this.TailTank);

            //if(theFlight.Fuel.IdxTailTank>=0)
            //{
            //  if(IsKILOS)
            //      acars += string.Format("TAIL TANK  {0}\r\n", Math.Round((float)this.TailTank * toPound,0));
            //  else
            //      acars += string.Format("TAIL TANK  {0}\r\n", this.TailTank);
            //}
            //#BR08111-->


            if (theFlight.Fuel.FuelLoading == FuelLoadingStyle.NSTD)  // 20060224
                acars += fuelLoadingStyle;

            //#BR08105			acars += string.Format("PREPARE BY/{0}\r\n", this.User);
            //#BR08105			acars += string.Format("ALL WTS IN {0}\r\n", unit);

            ////DateTime utc = DateTime.Now.ToUniversalTime();
            ////acars += string.Format("{0:hhmm}Z {1}\r\n", utc, utc.ToString("ddMMM", new CultureInfo("en-US")).ToUpper());

            acars += string.Format("BY {0}\r\n", this.User); //#BR08105
            //BR09118 acars += string.Format("WTS IN {0}\r\n", unit);  //#BR08105

            //<!--#BR19008 Thomas 所有機型LMC之後新增Tolerances限制值，修改LMC 文字顯示。

            //acars += string.Format("LMC\r\n");  //#BR08105

            switch (theFlight.ACType.name.Substring(0, 4))
            {
                case "BR32":
                    acars += string.Format("LMC 400\r\n");
                    break;
                case "BR33":
                    acars += string.Format("LMC 500\r\n");
                    break;
                case "BR77":
                    acars += string.Format("LMC 300\r\n");
                    break;
                case "BR78":
                    if (TOW <= 247000)
                    {
                       acars += string.Format("LMC 300\r\n");
                    }
                    else if (TOW > 247000)
                    {
                        acars += string.Format("LMC 0\r\n");
                    }
                    break;
             }
            //#BR19008-->

            acars += string.Format("DEST SPEC        CL/CPT +/- WT");  //#BR08105
            acars += "\r\n\r\n\r\n\r\n";  //#BR08105 換四行
            acars += "END\r\n";

            return acars;
        }

        #endregion

        #region ACARS: Radio Loadsheet

        /// <summary>Creating ACARS String</summary>
        /// <param name="real">with ACARS equipment and function</param>
        /// <returns>ACARS String</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        ////		public string ACARS(bool real, string sender) // real indicate if it is a real ACARS
        ////		{
        ////			string unit;
        ////			if (IsKILOS) unit = "TONNES";
        ////			else unit = "KILOPOUNDS";
        ////
        ////			string acars = "";
        ////
        ////			if (sender == null || sender == "") return "";
        ////
        ////			// ACARS LoadSheet part I
        ////			if (real) // M32
        ////			{
        ////				acars += "ELS\r\n";
        ////				acars += string.Format("AN {0}/FI {1}\r\n", this.AcNum, this.Flt_no);
        ////				acars += string.Format("-  W/B{0}{1:ddhhmm}\r\n", sender, DateTime.Now.ToUniversalTime());
        ////			}
        ////			else // M33
        ////			{
        ////				acars += "DLS\r\n";
        ////				acars += string.Format("AN {0}/FI {1}\r\n", this.AcNum, this.Flt_no);
        ////				acars += string.Format("-  P40{0}{1:ddhhmm}\r\n", sender, DateTime.Now.ToUniversalTime());
        ////			}
        ////
        ////			// ACARS LoadSheet part II
        ////			acars += "RADIO LOADSHEET\r\n"; //"ACARS\r\n";
        ////			acars += string.Format("{0,-10}01\r\n", string.Format("{0}/{1:#,0#}", this.Flt_no, DateTime.Now.Day));
        ////
        ////			// Flight Number, From, To
        ////			acars += string.Format("{0, -15}{1}-{2}\r\n", theFlight.FlightNumber, theFlight.Route[0], theFlight.Route[1]);
        ////
        ////			// Aircraft Registration Number and Loadsheet Version
        ////			acars += string.Format("{0, -15}EDNO {1:#,0#}\r\n", theFlight.ACRegNo, theFlight.LoadsheetVersion);
        ////
        ////			// ACARS LoadSheet part III
        ////			// BW
        ////			acars += string.Format("              BW {0,5}\r\n", Math.Round((float) this.BW / 1000.0, 1));
        ////
        ////			// Crew, DOW
        ////			acars += string.Format("CREW{0, 7}  DOW {1,5}\r\n", this.Crew, Math.Round((float) this.DOW / 1000.0, 1));
        ////			// TFLD: Traffic Load
        ////			acars += string.Format("            TFLD {0,5}\r\n", Math.Round((float) this.TrafficLoadWt / 1000.0, 1));
        ////
        ////			// MZFW, ZFW
        ////			acars += string.Format("MZFW{0, 7:#,###.0}  ZFW {1,5:#,###.0}{2,2}\r\n",
        ////			                       Math.Round((float) this.MZFW / 1000, 1), Math.Round((float) this.ZFW / 1000, 1), this.ZFWLimit);
        ////			// TOF: Takeoff Fuel
        ////			acars += string.Format("             TOF {0,5:#,###.0}\r\n", Math.Round((float) this.TakeoffFuel / 1000, 1));
        ////
        ////			// MTOW, TOW
        ////			acars += string.Format("MTOW{0, 7:###.#}  TOW {1,5:#,###.0}{2,2}\r\n",
        ////			                       Math.Round((float) this.MTOW / 1000, 1), Math.Round((float) this.TOW / 1000, 1), this.TOWLimit);
        ////			// TPF: Trip Fuel
        ////			acars += string.Format("             TPF {0,5:#,###.0}\r\n", Math.Round((float) this.TripFuel / 1000, 1));
        ////
        ////			// MLDW, LDW
        ////			acars += string.Format("MLDW{0, 7:###.#}  LDW {1,5:#,###.0}\r\n", Math.Round((float) this.MLDW / 1000, 1), Math.Round((float) this.LDW / 1000, 1));
        ////
        ////			// UNDLD: Underload
        ////			acars += string.Format("UNDERLOAD {0} KG\r\n", this.Underload);
        ////
        ////			// BI
        ////			acars += string.Format("BI      {0,4:#,##.0}\r\n", BI);
        ////			// DOI, PAX - class1
        ////			acars += string.Format("DOI     {0,4:#,##.0}   PAX{1,4}\r\n", this.DOI, this.Class1Pax(0));
        ////			// LIZFW, PAX - class2
        ////			acars += string.Format("LIZFW   {0,4:#,##,0}{1,10}\r\n", this.LIZFW, this.Class2Pax(0));
        ////			// LITOW, PAX - class3
        ////			acars += string.Format("LITOW   {0,4:#,##,0}{1,10}\r\n", this.LITOW, this.Class3Pax(0));
        ////
        ////			// MACZFW, PAX - Total
        ////			acars += string.Format("MACZFW  {0,4:#,##.0}   TTL{1,4}", this.MACZFW, this.TotalPax - this.Infant());
        ////			if (this.Infant() > 0)
        ////				acars += string.Format(" PLUS{0,3} INF\r\n", this.Infant());
        ////			else
        ////				acars += "\r\n";
        ////
        ////			// MACTOW
        ////			acars += string.Format("MACTOW  {0,4:#,##.0}\r\n", this.MACTOW);
        ////			// MACDLW
        ////			if (theFlight.ACType.IsDLW)
        ////				acars += string.Format("MACDLW  {0,4:#,##.0}\r\n", this.MACDLW);
        ////
        ////			// CRZCG for 777
        ////			if (theFlight.ACType.name.IndexOf("77") >= 0)
        ////			{
        ////				acars += string.Format("CRZCG   {0,4:#,##.0}\r\n", this.CRZCG);
        ////			}
        ////
        ////
        ////			// ACARS LoadSheet part IV
        ////			// SI   1104
        ////			if (theFlight.Telex.LDM.SI != "")
        ////			{
        ////				acars += ("SI\r\n" + theFlight.Telex.LDM.SI + "\r\n");
        ////			}
        ////			/**
        ////			StabilizerTrimItem[] stabTrims = theFlight.StabilizerTrim;
        ////			if(stabTrims!=null && stabTrims.Length>0)
        ////			{
        ////				if(theFlight.Telex.LDM.SI == "")acars += "SI\r\n";
        ////				acars += "STABILIZER TRIM\r\n";
        ////				foreach (StabilizerTrimItem stabTrim in stabTrims)
        ////				{
        ////					acars +=string.Format("{0,-32}/{1}\r\n", stabTrim.Name.ToUpper(), Math.Round(stabTrim.Index,1));
        ////				}
        ////				acars += "\r\n";
        ////			}
        ////			**/
        ////			// ADD/DEDUCTION Service Items
        ////			bool noItem = true;
        ////			acars += "ADD";
        ////			acars += "\r\n";
        ////			foreach (SvcItem item in theFlight.ServiceItems.ServicesList)
        ////				if (item.Weight > 0)
        ////				{
        ////					noItem = false;
        ////					acars += string.Format(" {0,-19}{1,6}{2,5:#,0.#}\r\n",
        ////					                       item.Desc, item.Weight, item.Index);
        ////				}
        ////			if (noItem)
        ////				acars += "NIL\r\n";
        ////
        ////			//			noItem = true;
        ////			bool first = true;
        ////			foreach (SvcItem item in theFlight.ServiceItems.ServicesList)
        ////				if (item.Weight < 0)
        ////				{
        ////					//					noItem = false;
        ////					if (first)
        ////					{
        ////						first = false;
        ////						acars += "DEDUCTIONS";
        ////						acars += "\r\n";
        ////					}
        ////
        ////					acars += string.Format(" {0,-19}{1,6}{2,5:#,0.#}\r\n",
        ////					                       item.Desc, item.Weight, item.Index);
        ////				}
        ////
        ////			acars += string.Format("PANTRY CODE {0}\r\n", Pantry);
        ////
        ////			if(theFlight.Fuel.IdxTailTank>=0)
        ////				acars += string.Format("TAIL TANK  {0}\r\n", this.TailTank);
        ////
        ////			if (theFlight.Fuel.FuelLoading == FuelLoadingStyle.NSTD)  // 20060224
        ////				acars += fuelLoadingStyle;
        ////
        ////			acars += string.Format("PREPARE BY/{0}\r\n", this.User);
        ////			acars += string.Format("ALL WTS IN {0}\r\n", unit);
        ////
        ////			DateTime utc = DateTime.Now.ToUniversalTime();
        ////			acars += string.Format("{0:hhmm}Z {1}\r\n", utc, utc.ToString("ddMMM", new CultureInfo("en-US")).ToUpper());
        ////			acars += "END\r\n";
        ////
        ////			return acars;
        ////		}

        #endregion

        /// <summary>
        /// #BR17229 判斷TOW在 LINE的哪一側， THOMAS 777 與 787 LS 與 ELS顯示 ELS 標註文字
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public string CheckPoint(string Type)
        {
            AirTypeEx theAirType = theFlight.ACType;
            int AlternateCount = 0;
            string nmWtLmt = "";
            
            //#BR18110 判斷PAX Zone or ROW 
            if (System.Configuration.ConfigurationManager.AppSettings["RowZoneMode"] == "Y")
            {
                if (theFlight.Pax.BTrimByZone)
                {
                    nmWtLmt = "TOW_ALTFWDCG_ZONE";
                }
                else
                {
                    nmWtLmt = "TOW_ALTFWDCG";
                }
            }
            else
            {
                nmWtLmt = "TOW_ALTFWDCG";
            }

            double X = theFlight.LITOW;
            double Y = theFlight.TOW;
            string Result = "";

            try
            {
                foreach (TrimSheetWeight wtLmt in theAirType.TrimSheet.weight)
                {
                    if (wtLmt.name.ToUpper().Equals(nmWtLmt.ToUpper()))
                    {
                        if (wtLmt.ctrlcode.Length == 1)// for 777
                        {
                            AlternateCount = 1;
                        }

                        if (wtLmt.ctrlcode.Length == 2) //for 787
                        {
                            AlternateCount = 2;
                        }
                    }
                }

                switch (AlternateCount)
                {
                    case 0:
                        break;
                    case 1:
                        PointF[] points = theAirType.findALTERNATETrimLine(theFlight, nmWtLmt, "ALTERNATE1"); //以ALTERNATE1線判斷  
                        double x1 = points[0].X;
                        double y1 = points[0].Y;
                        double x2 = points[1].X;
                        double y2 = points[1].Y;

                        double tmpx = (x1 - x2) / (y1 - y2) * (Y - y2) + x2;  //判斷座標在線的哪側公式

                        if (Type == "LS")
                        {
                            if (tmpx > X ) //左側
                            {
                                Result = "FWD-L ";
                            }
                            else if (tmpx <= X) //右側
                            {
                                Result = "ALT-L ";
                            }
                        }
                        else if (Type == "ELS")
                        {
                            if (tmpx > X) //左側
                            {
                                Result = "FWD-L ";
                            }
                            else if (tmpx <= X) //右側
                            {
                                Result = "ALT-L ";
                            }
                        }
                        break;
                    case 2:
                        PointF[] points1 = theAirType.findALTERNATETrimLine(theFlight, nmWtLmt, "ALTERNATE1"); //以ALTERNATE1線判斷  
                        double x1_1 = points1[0].X;
                        double y1_1 = points1[0].Y;
                        double x2_1 = points1[1].X;
                        double y2_1 = points1[1].Y;
                        double tmpx_1 = (x1_1 - x2_1) / (y1_1 - y2_1) * (Y - y2_1) + x2_1; //判斷座標在線的哪側公式

                        PointF[] points2 = theAirType.findALTERNATETrimLine(theFlight, nmWtLmt, "ALTERNATE2"); //以ALTERNATE1線判斷  
                        double x1_2 = points2[0].X;
                        double y1_2 = points2[0].Y;
                        double x2_2 = points2[1].X;
                        double y2_2 = points2[1].Y;
                        double tmpx_2 = (x1_2 - x2_2) / (y1_2 - y2_2) * (Y - y2_2) + x2_2; //判斷座標在線的哪側公式

                        //說明: tmpx_1是ALTERNATE1，tmpx_2是ALTERNATE2， X 是TOW CG，  > 是左側， < 是右側， = 是包含線
                        if (Type == "LS")
                        {
                            if (tmpx_1 > X && tmpx_2 > X) //LINE1左側 && LINE2左側
                            {
                                Result = "FWD-L ";
                            }
                            else if (tmpx_1 <= X && tmpx_2 <= X) //LINE1右側 && LINE2右側  
                            {
                                Result = "ALT2L ";
                            }
                            else if (tmpx_1 <= X && tmpx_2 > X) //LINE1右側 && LINE2左側  
                            {
                                Result = "ALT1L ";
                            }
                        }
                        else if (Type == "ELS")
                        {
                            if (tmpx_1 > X && tmpx_2 > X) //LINE1左側 && LINE2左側  
                            {
                                Result = "FWD-L ";
                            }
                            else if (tmpx_1 <= X && tmpx_2 <= X) //LINE1右側 && LINE2右側  
                            {
                                Result = "ALT2L ";
                            }
                            else if (tmpx_1 <= X && tmpx_2 > X) //LINE1右側 && LINE2左側  
                            {
                                Result = "ALT1L ";
                            }
                       }
                        break;
                }
                return Result;
            }
            catch
            {
                return Result;
            }
        }

        /// <summary>
        ///  #BR18107 THOMAS 判斷 TOW是否在EXPD AREA區域內
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public string CheckExpdArea(string Type)  
        {
            AirTypeEx theAirType = theFlight.ACType;

            string nmWtLmt = "";
            //#BR18110 判斷PAX Zone or ROW 
            if (System.Configuration.ConfigurationManager.AppSettings["RowZoneMode"] == "Y")
            {
                if (theFlight.Pax.BTrimByZone)
                {
                    nmWtLmt = "AFT_CG_TAKEOFF_ZONE";
                }
                else
                {
                    nmWtLmt = "AFT_CG_TAKEOFF";
                }
            }
            else
            {
                nmWtLmt = "AFT_CG_TAKEOFF";
            }

            PointF[] points = theAirType.findEXPDAREATrimLine(theFlight, nmWtLmt, "AFT_CG");

            double X = theFlight.LITOW;
            double Y = theFlight.TOW;

            if (points.Length >= 3)
            {
                GraphicsPath myGraphicsPath = new GraphicsPath();
                Region myRegion = new Region();
                myGraphicsPath.Reset();
                Point TowPoint = new Point((int)X, (int)Y);
                myGraphicsPath.AddPolygon(points);//points);
                myRegion.MakeEmpty();
                myRegion.Union(myGraphicsPath);
                bool myPoint = myRegion.IsVisible(TowPoint);
                if (myPoint)
                {
                    if (Type == "ELS")
                    {
                        return "(AFT CG TAKEOFF)";
                    }
                    else if (Type == "LS")
                    {
                        return "AFT CG TAKEOFF";
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        #region LIR: Loading Instruction/Report, NOTOC: Notification To Captain

        /*
		---------+---------+---------+---------+---------+---------+---------+
		OFFLOADING INSTRUCTION REPORT  PREPARED BY                   EDNO
		ALL WEIGHTS IN KILOS                                            1
				
		FROM/TO  FLIGHT  A/C REG    VERSION     GATE  TARMAC DATE    TIME
		TPE SEA  BR0123  B-1234567  #######     B34   HO5    30JUN05 1230
		PLANNED JOINING LOAD
		SEA    F  12   C 123   Y 123   C 123456   M 12345    B 12345
		JOING SPECS     SEE SUMMARY
		TRANSIT SPECS   SEE SUMMARY
		RELOADS

		CPT: 1          MAX
		-----------------------------------------------------------------
		:11   ULD-NUMBER
		:ONLOAD  SEA C/2350
		:SPECS   [SEE SUMMARY]
		:REPORT  
		-----------------------------------------------------------------
		:11L  ULD-NUMBER      :11R  ULD-NUMBER                 :        D
		:ONLOAD  SEA C/2350   :ONLOAD  SEA C/4567              :  2300  O
		:SPECS   SEE SUMMARY  :SPECS   [SEE SUMMARY]           :  4567  O
		:REPORT  2300         :REPORT  4567       11L/11R TOTAL:  6567  R        
		-----------------------------------------------------------------
		THIS AIRCRAFT HAS BEEN LOADED IN ACCORDANCE WITH THESE INSTRUCTIONS AND
		THE DEVIATIONS SHOWN ON THIS REPORT. THE CONTAINERS/PALLETS AND BULKLOAD
		HAVE BEEN SECURED IN ACCORDANCE WITH COMPANY INSTRUCTIONS.
									   SIGNATURE: 
		---------+---------+---------+---------+---------+---------+---------+
		 
		TRANSIT SPECS:
		OFFLOAD SPECS:
		*/


        /// <summary>position have SHC or not</summary>
        /// <param name="posn"></param>
        /// <returns>bool: have SHC or not</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private bool hasSHC(CargoPosnBase posn)
        {
            if (posn.IsBulkPosn())
            {
                foreach (ICargo cgo in posn.CargoList)
                {
                    if (hasSHC(cgo)) return true;
                }
            }

            return false;
        }

        /// <summary>Cargohave SHC or not</summary>
        /// <param name="cgo"></param>
        /// <returns>bool: have SHC or not</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private bool hasSHC(ICargo cgo)
        {
            foreach (Consignment csg in cgo.Consignments)
            {
                if (csg.SHC.Length > 2) return true;
            }
            return false;
        }


        /// <summary>Put all of the string in ArrayList into one string</summary>
        /// <param name="alist">ArrayList</param>
        /// <returns>string</returns>  
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string ToTexts(ArrayList alist)
        {
            string rtn = "";
            foreach (string str in alist)
                rtn += (str.Substring(0, 3) + " " + str.Substring(4, str.Length - 4) + ",");
            return rtn.Substring(0, rtn.Length - 1);
        }


        private const string lirDashLine = "-----------------------------------------------------------------\r\n";

        /// <summary> create report content for parameter of Compartment or Bay
        ///   create  string 
        /// 			 CPT            MAX
        //			 * :11 AKE######
        //			 * :TRANSIT  SEA C/1750
        //			 * :SPECS  SEE SUMMARY  (if SHC exists)
        //			 * :REPORT:
        //			 * -----------------------------------------------------------------
        //			 * :12L AKE######             :12R AKE######                       D
        //			 * :ONLOAD  SEA C/3412        :ONLOAD  SEA C/4562                  O
        //			 * :REPORT:                   :SPECS  SEE SUMMARY                  O
        //			 *                            :REPORT:                             R
        //			 * -----------------------------------------------------------------
        //			 * :11L  ULD-NUMBER      :11R  ULD-NUMBER                 :        D
        //			 * :ONLOAD  SEA C/2350   :ONLOAD  SEA C/4567              :  2300  O
        //			 * :SPECS   SEE SUMMARY  :SPECS   [SEE SUMMARY]           :  4567  O
        //			 * :REPORT  2300         :REPORT  4567       11L/11R TOTAL:  6567  R        
        /// </summary>
        /// <param name="cmp">Compartment or Bay</param>
        /// <param name="realCmp"></param>
        /// <returns>string: LIR string</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string formatCmpBay(CargoPosnBase cmp, CargoPosnBase realCmp)
        {
            /* CPT            MAX
             * :11 AKE######
             * :TRANSIT  SEA C/1750
             * :SPECS  SEE SUMMARY  (if SHC exists)
             * :REPORT:
             * -----------------------------------------------------------------
             * :12L AKE######             :12R AKE######                       D
             * :ONLOAD  SEA C/3412        :ONLOAD  SEA C/4562                  O
             * :REPORT:                   :SPECS  SEE SUMMARY                  O
             *                            :REPORT:                             R
             * -----------------------------------------------------------------
             * :11L  ULD-NUMBER      :11R  ULD-NUMBER                 :        D
             * :ONLOAD  SEA C/2350   :ONLOAD  SEA C/4567              :  2300  O
             * :SPECS   SEE SUMMARY  :SPECS   [SEE SUMMARY]           :  4567  O
             * :REPORT  2300         :REPORT  4567       11L/11R TOTAL:  6567  R        
             */
            string lir = "";
            CargoPosnBase prevPos = null;
            Cargo cgo, cgo2;

            if (cmp.CargoList.Count > 0) // unpartitionable cargo position, for example A1, A2, B, T, 11P (BAY)
            {
                cgo = cmp.CargoList[0] as Cargo;

                lir += string.Format(":{0,-4}{1, -22}\r\n", cmp.Name, cgo.ULDSerialNo);
                lir += string.Format(":ONLOAD   {0,-17}\r\n",
                                     LIRDestCategoryWt(cgo, cmp.getWeight()));
                if (this.hasSHC(cgo))
                    lir += ":SPECS:  SEE SUMMARY\r\n";
                if (realCmp == null)
                    lir += string.Format(":REPORT:  {0}\r\n", cmp.getWeight());
                else
                    lir += string.Format(":REPORT:  {0,-9}{1,46}\r\n", cmp.getWeight(), reportTotal(realCmp));
                lir += lirDashLine;
            }
            else // partitionable cargo position, for example C, D, 11
            {
                foreach (CargoPosnBase pos in cmp)
                {
                    if (cmp.IndexOf(pos) % 2 == 1 && (Visible(pos) || Visible(prevPos)) )   //||prevPos.CargoList.Count > 0 || pos.CargoList.Count > 0))    // 0728
                    {
                        if (prevPos.CargoList.Count > 0)
                            cgo = prevPos.CargoList[0] as Cargo;
                        else
                            cgo = null;

                        if (pos.CargoList.Count > 0)
                            cgo2 = pos.CargoList[0] as Cargo;
                        else
                            cgo2 = null;

                        bool prevShow = Visible(prevPos); // && prevPos.NoFit;
                        bool currShow = Visible(pos); // && pos.NoFit;
                        lir += string.Format(":{0,-4}{1,-25}:{2,-4}{3,-22}\r\n",
                                             (cgo != null) ? prevPos.Name : (prevShow) ? prevPos.Name : "",
                                             (cgo != null) ? cgo.ULDSerialNo : (prevShow) ? "N" : "",
                                             (cgo2 != null) ? pos.Name : (currShow) ? pos.Name : "",
                                             (cgo2 != null) ? cgo2.ULDSerialNo : (currShow) ? "N" : "");

                        if (cgo != null || cgo2 != null)
                            lir += string.Format(":{0,-9}{1,-20}:{2,-9}{3,-17}\r\n",
                                                 (cgo != null) ? "ONLOAD:" : "",
                                                 LIRDestCategoryWt(cgo, prevPos.getWeight()),
                                                 (cgo2 != null) ? "ONLOAD:" : "",
                                                 LIRDestCategoryWt(cgo2, pos.getWeight()));

                        if (cgo != null && this.hasSHC(cgo))
                        {
                            if (cgo2 != null && this.hasSHC(cgo2))
                                lir += ":SPECS:  SEE SUMMARY          :SPECS:  SEE SUMMARY       \r\n";
                            else
                                lir += ":SPECS:  SEE SUMMARY          :\r\n";
                        }
                        else
                        {
                            if (cgo2 != null && this.hasSHC(cgo2))
                                lir += "                              :SPECS:  SEE SUMMARY       \r\n";
                        }

                        if (cgo == null && cgo2 == null && realCmp != null)
                        {
                            //if (cmp.Count < 3 ) // GHL occupied and HR No-fit
                            lir += string.Format(":         {0,-20}:         {1,-6}{2,19}\r\n",
                                                 "", "", reportTotal(realCmp));
                        }

                        if (prevPos.getWeight() > 0)
                        {
                            if (pos.getWeight() > 0)
                            {
                                if (realCmp == null)
                                    lir += string.Format(":REPORT:  {0,-20}:REPORT:  {1,-17}\r\n",
                                                         prevPos.getWeight(), pos.getWeight());
                                else
                                {
                                    if (cmp.Count > 2 && cmp.IndexOf(pos) < 2 &&
                                        ((cmp[2] as CargoPosnBase).Available || (cmp[3] as CargoPosnBase).Available))
                                    { }
                                    else
                                    {
                                        lir += string.Format(":REPORT:  {0,-20}:REPORT:  {1,-6}{2,19}\r\n",
                                                             prevPos.getWeight(), pos.getWeight(), reportTotal(realCmp));
                                    }
                                }
                            }
                            else
                            {
                                if (realCmp == null)
                                    lir += string.Format(":REPORT:  {0,-20}:         {1,-17}\r\n",
                                                         prevPos.getWeight(), "");
                                else
                                {
                                    if (cmp.Count > 2 && cmp.IndexOf(pos) < 2 &&
                                        ((cmp[2] as CargoPosnBase).Available || (cmp[3] as CargoPosnBase).Available))
                                        lir += string.Format(":REPORT:  {0,-20}:         {1,-17}\r\n",
                                                             prevPos.getWeight(), "");
                                    else
                                    {
                                        lir += string.Format(":REPORT:  {0,-20}:         {1,-6}{2,19}\r\n",
                                                             prevPos.getWeight(), "", reportTotal(realCmp));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (pos.getWeight() > 0)
                            {
                                if (realCmp == null)
                                    lir += string.Format(":         {0,-20}:REPORT:  {1,-17}\r\n",
                                                         "", pos.getWeight());
                                else
                                {
                                    if (cmp.Count > 2 && cmp.IndexOf(pos) < 2 &&
                                        ((cmp[2] as CargoPosnBase).Available || (cmp[3] as CargoPosnBase).Available))
                                        lir += string.Format(":         {0,-20}:REPORT:  {1,-17}\r\n",
                                                             "", pos.getWeight());
                                    else
                                    {
                                        lir += string.Format(":         {0,-20}:REPORT:  {1,-6}{2,19}\r\n",
                                                             "", pos.getWeight(), reportTotal(realCmp));
                                    }
                                }
                            }
                        }
                        lir += lirDashLine;
                    }

                    if (cmp.IndexOf(pos) % 2 == 0 )  //CL CDL CD
                        prevPos = pos;
                    else
                        prevPos = null;


                    
                    //<!--  #BR17202 THOMAS  LI/Rpt Telex 內容新增center load
                    if (cmp.IndexOf(prevPos) == 4 && Visible(prevPos))
                    {
                        if (prevPos.CargoList.Count > 0)
                            cgo = prevPos.CargoList[0] as Cargo;
                        else
                            cgo = null;

                        bool prevShow = Visible(prevPos); // && prevPos.NoFit;
                        lir += string.Format(":{0,-4}{1,-25}\r\n",
                                             (cgo != null) ? prevPos.Name : (prevShow) ? prevPos.Name : "",
                                             (cgo != null) ? cgo.ULDSerialNo : (prevShow) ? "N" : "");

                        if (cgo != null )
                            lir += string.Format(":{0,-9}{1,-20}\r\n",(cgo != null) ? "ONLOAD:" : "",LIRDestCategoryWt(cgo, prevPos.getWeight()));

                        if (cgo != null && this.hasSHC(cgo))
                        {
                                lir += ":SPECS:  SEE SUMMARY          :\r\n";
                        }

                        if (cgo == null  && realCmp != null)
                        {
                            //if (cmp.Count < 3 ) // GHL occupied and HR No-fit
                            lir += string.Format(":         {0,-20}:         {1,-6}{2,19}\r\n", "", "", reportTotal(realCmp));
                        }

                        if (prevPos.getWeight() > 0)
                        {
                               if (realCmp == null)
                               {
                                   lir += string.Format(":REPORT:  {0,-20}:         {1,-7}{2,19}\r\n", prevPos.getWeight(), "", reportTotal(realCmp));
                               }
                                else
                                {
                                   lir += string.Format(":REPORT:  {0,-20}         {1,-7}{2,19}\r\n", prevPos.getWeight(), "", reportTotal(realCmp));
                                }
                        }
                        lir += lirDashLine;
                    }
                    //#BR17202  -->
                    

                }
            }
            if (lir == "" && cmp.Available ) // 11P, 12P
            //if (lir == "" && cmp.Available  && cmp.Prev().Count !=5) // 11P, 12P //#BR17202 THOMAS 判斷前一個cmp必須沒有Center load
            {
                lir += string.Format(":{0,-4}{1, -22}:\r\n", cmp.Name, "N");
                if (realCmp != null)
                    lir += string.Format(":         {0,-20}:         {1,-6}{2,19}\r\n",
                                         "", "", reportTotal(realCmp));
                lir += lirDashLine;
            }
            return lir;
        }

        /// <summary> create LIR string which includes destination 、category and weight, for example: "SEA M/300"</summary>
        /// <param name="cgo">Cargo object </param>
        /// <param name="wt">weight</param>
        /// <returns>string</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string LIRDestCategoryWt(Cargo cgo, double wt)
        {
            if (cgo == null) return " ";
            else
                return string.Format("{0} {1}/{2}", cgo.Dest, cgo.Category, wt); // 1021
        }

        /// <summary> create Total report string ；EX: "CPT 32L TOTAL: 123.22"</summary>
        /// <param name="cmp">CargoPosnBase</param>
        /// <returns>string: Total report string </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string reportTotal(CargoPosnBase cmp)
        {
            if (Char.IsDigit(cmp.Name.ToCharArray(0, 1)[0]))
                return string.Format("CPT {0} TOTAL:{1,6}", cmp.Name, cmp.getWeight());
            else
                return string.Format("{0,5} TOTAL:{1,6}", cmp.Name, cmp.getWeight());
        }

        /// <summary> create string for LIR to illustrate Bulk Position</summary>
        /// <param name="cmp">CargoPosnBase</param>
        /// <returns>Bulk Position之 string </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string LIRBulkPosn(CargoPosnBase cmp)
        {
            string lir = "";

            CargoPosnBase parent = (cmp is CargoCmpt) ? cmp : cmp.Parent;

            if (cmp.CargoList.Count > 0)
            {
                lir += string.Format(":{0,-4}{1,-22}\r\n", cmp.Name, "");
                lir += string.Format(":ONLOAD:  {0}\r\n", ToTexts(cmp.BulkInfo()));
                if (this.hasSHC(cmp))
                    lir += ":SPECS:  SEE SUMMARY       \r\n";

                if (cmp == parent || parent.IndexOf(cmp) == parent.Count - 1)
                    lir += string.Format(":REPORT:  {0,-20}          {1,-6}{2,19}\r\n",
                                         cmp.getWeight(), "", reportTotal(parent));
                else
                    lir += string.Format(":REPORT:  {0,-20}\r\n", cmp.getWeight());
            }
            else
            {
                lir += string.Format(":{0,-4}{1,-22}\r\n", cmp.Name, "N");
                if (cmp == parent || parent.IndexOf(cmp) == parent.Count - 1)
                    lir += string.Format(":         {0,-20}          {1,-6}{2,19}\r\n",
                                         "", "", reportTotal(parent));
            }
            lir += lirDashLine;
            return lir;
        }

        /// <summary> create LIR document content</summary>
        /// <returns>string: LIR document content</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string LIR(string lir_si)
        {
            string lir;
            /*
            OFFLOADING INSTRUCTION REPORT  PREPARED BY                   EDNO
            ALL WEIGHTS IN KILOS                                            1			 
             */
            lir = string.Format("LOADING INSTRUCTION REPORT  PREPARED BY   {0,17}  EDNO\r\n", this.User);

            lir += string.Format("ALL WEIGHTS IN {0,-44}{1,5:##,0#}\r\n",
                                 (this.IsKILOS) ? "KILOS" : "POUNDS", theFlight.LIREDNo); // 0929
            /*
            ---------+---------+---------+---------+---------+---------+---------+
            FROM/TO  FLIGHT  A/C REG    VERSION     GATE  TARMAC DATE    TIME
            TPE SEA  BR0123  B-1234567  #######     B34   HO5    30JUN05 1230
            */

            lir += "FROM/TO  FLIGHT  A/C REG    VERSION     GATE  TARMAC DATE    TIME\r\n";

            lir += string.Format("{0,-4}{1,-5}{2,-8}{3,-11}{4,-12}{5,-6}{6,-7}{7}\r\n",
                                 this.Route[0], this.Route[1], this.Flt_no, this.AcNum, this.CabinConfig, "", "",
                                 theFlight.STD.ToString("ddMMMyy HHmm", new CultureInfo("en-US")).ToUpper()); // 1111
            /*
            PLANNED JOINING LOAD
            SEA    F  12   C 123   Y 123   C 123456   M 12345    B 12345
            JOING SPECS     SEE SUMMARY
            TRANSIT SPECS   SEE SUMMARY
            RELOADS
            */
            lir += "PLANNED JOINING LOAD\r\n";
            for (int route = 1; route < this.Route.Length; route++)
                lir += string.Format("{0,-7}{1,-2}{2,-6}{3,-2}{4,-6}{5,-2}{6,-6}C {7,-9}M {8,-9}B {9,-9}\r\n",
                                     this.Route[route], this.ClassName(0), this.Class1Pax(route - 1), this.ClassName(1),
                                     this.Class2Pax(route - 1), this.ClassName(2), this.Class3Pax(route - 1),
                                     this.CargoNetWt(route), this.PostNetWt(route), this.BaggageNetWt(route));

            lir += "JOING SPECS:    SEE SUMMARY\r\n";
            lir += "TRANSIT SPECS:  SEE SUMMARY\r\n"; // this line may be disappear
            lir += "RELOADS:\r\n";
            // For each compartment

            lir += "LOADING INSTRUCTION\r\n";

            CargoPosnBase lastBay;
            string tmpstr;
            double maxWt;
            lir += lirDashLine;
            ArrayList availList = PosnListToArrayList(this.cmpList);

            CargoPosnBase[] cmplist = availList.ToArray(typeof(CargoPosnBase)) as CargoPosnBase[];

            foreach (CargoPosnBase cmp in cmplist)
            {
                tmpstr = "";
                maxWt = 0;
                if (cmp.AreAllChildrenLeaf()) // Main Deck and Bulk
                {
                    if (cmp.IsBulkPosn()) // Bulk
                    {
                        if (cmp.CargoList.Count > 0)
                        {
                            tmpstr += this.LIRBulkPosn(cmp);
                            if (tmpstr != "")
                                maxWt += cmp.MaxGrossWeight; //  maximum gross weight 
                        }
                        else
                        {
                            if (cmp.Count == 0)
                                tmpstr += this.LIRBulkPosn(cmp);
                            else
                            {
                                foreach (CargoPosnBase bay in cmp)
                                {
                                    tmpstr += this.LIRBulkPosn(bay);
                                    if (tmpstr != "")
                                        maxWt += bay.MaxGrossWeight; //  maximum gross weight 
                                }
                            }
                        }
                    }
                    else // Main Deck
                    {
                        tmpstr = this.formatCmpBay(cmp, cmp);
                        maxWt = cmp.MaxGrossWeight; //  maximum gross weight 
                    }
                }
                else // Lower Deck, Available
                {
                    lastBay = this.lastBayWithCargo(cmp);
                    cmp.Sort(new BayComparer()); // Sort bay of compartment
                    foreach (CargoPosnBase bay in cmp)
                    {
                        tmpstr += this.formatCmpBay(bay, (bay == lastBay) ? cmp : null);
                        if (tmpstr != "")
                            maxWt += bay.MaxGrossWeight; //  maximum gross weight 
                    }
                }
                if (tmpstr != "")
                    lir += string.Format("CPT {0,-5} MAX {1}\r\n", cmp.Name, maxWt) + tmpstr;
            }

            lir += ("SI\r\n" + lir_si + "\r\n"); // LIF page provide, LIF SI == LIR SI 
            lir += "\r\nTHIS AIRCRAFT HAS BEEN LOADED IN ACCORDANCE WITH THESE INSTRUCTIONS\r\n";
            lir += string.Format("{0, 43}{1}\r\n", "SIGNATURE:  ", "");
            /*
             * -----------------------------------------------------------------
             * THIS AIRCRAFT HAS BEEN LOADED IN ACCORDANCE WITH THESE INSTRUCTIONS
             * AND THE DEVIATIONS SHOWN ON THIS REPORT. THE CONTAINERS/PALLETS AND
             * BULKLOAD HAVE BEEN SECURED IN ACCORDANCE WITH COMPANY INSTRUCTIONS.
             * 						           SIGNATURE: 
             * 			 
             * THIS AIRCRAFT HAS BEEN LOADED IN ACCORDANCE WITH THESE INSTRUCTIONS.
             */

            return lir;
        }

        /// <summary> search the lase bay with cargo for compartment</summary>
        /// <param name="cmp"></param>
        /// <returns>CargoPosnBase</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private CargoPosnBase lastBayWithCargo(CargoPosnBase cmp)
        {
            CargoPosnBase rtnBay = null;

            foreach (CargoPosnBase bay in cmp)
            {
                if (Visible(bay)) // 1028  && bay.getWeight() > 0) 
                    rtnBay = bay;
                foreach (CargoPosnBase posn in bay) // 0728
                {
                    if (Visible(posn)) // || posn.NoFit)
                    {
                        rtnBay = bay;
                        break;
                    }
                }
            }

            return rtnBay;
        }
        #endregion

        #region NOTOC

        /*
		NOTOC
		---------+---------+---------+---------+---------+---------+---------+
		FROM  FLIGHT  A/C REG    DATE    TIME
		TPE   BR0123  B-1234567  30JUN05 1230

		LOCN JOIN/TRAN DEST  CAT  IMP  PCS  WEIGHT  TI   AWB
		-----------------------------------------------------------------
		FL   JOIN      SEA   C    RRY  1    1234         111 2345678
		FR   JOIN      SEA   C    RFS  1    1234         111 2345678
		-----------------------------------------------------------------
		12R  JOIN      SEA   C    RFL  1    1234         111 2345678
		12R  JOIN      SEA   C    RNG  1    1234         111 2345678
		12R  JOIN      SEA   C    AVI  1    1234         111 2345678	
		*/

        /// <summary> create NOTOC</summary>
        /// <returns>string: NOTOC content</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string NOTOC()
        {
            AirlineEx airline = theFlight.Airline;

            string notoc = "FROM  FLIGHT  A/C REG    DATE    TIME\r\n";
            notoc += string.Format("{0,-6}{1,-8}{2,-11}{3}\r\n", // 1111
                                   this.Route[0], this.Flt_no, this.AcNum, theFlight.STD.ToString("ddMMMyy HHmm", new CultureInfo("en-US")).ToUpper()); // 1111

            notoc += "LOCN JOIN/TRAN DEST  CAT  IMP  PCS  WEIGHT  TI   AWB\r\n";
            notoc += (new string('*', LineWidth) + "\r\n");

            foreach (CargoPosnBase pos in theFlight.CargoPosnMgr.GenVisibleList(0))
            {
                ArrayList aList = theFlight.CargoPosnMgr.SHCConsignments(pos);
                if (aList.Count > 0)
                {
                    string shcstr = "";
                    foreach (SpecialLoad csgn in aList)
                    {
                        if (!airline.isNOTOC(csgn.SHC)) continue; // do not create NOTOC
                        shcstr += string.Format("{0,-5}{1,-10}{2,-6}{3,-5}{4,-5}{5,-5}{6,-8}{7,-5}{8}\r\n",
                                                pos.Name, "JOIN", csgn.Dest, csgn.Category,
                                                csgn.SHC, csgn.Pieces, csgn.Weight, "",
                                                getAWB(csgn, pos.IsBulkPosn() && pos is CargoCmpt));
                    }

                    if (shcstr != "")
                        notoc += (shcstr + (new string('-', LineWidth) + "\r\n"));
                }
            }

            return notoc;
        }


        /// <summary>Get AWB</summary>
        /// <param name="shc"></param>
        /// <param name="isBulkCmp"></param>
        /// <returns>string: AWB</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string getAWB(SpecialLoad shc, bool isBulkCmp)
        {
            if (shc.AWB.Length > 0)
                return shc.AWB;
            if (isBulkCmp)
                return "";
            ICargo parent = shc.ParentULD;
            if (parent == null)
                return "";
            if (parent is Cargo)
                return "";
            else
                return (parent as Consignment).AWB;
        }

        #endregion

        #region Telex: FlightHeader

        /// <summary>
        /// Title of all the telex except UCM
        /// </summary>
        private string FlightHeader
        {
            get
            {
                string sAcRegNo = theFlight.ACRegNo;  //BR13101
                //BR13101sAcRegNo = sAcRegNo.Substring(0,1)+sAcRegNo.Substring(2,5); //BR13101 remove "-"
                sAcRegNo = sAcRegNo.Replace("-", ""); //BR13101 remove "-",20130320 for 9M-XXD B-16205
                FlightNumber fltNo = new FlightNumber(theFlight.FlightNumber);
                string strFltNo = string.Format("{0}{1}{2}", fltNo.CarrierCode, fltNo.SerialNo, fltNo.SuffixCode).Trim();
                return string.Format("{0}/{1:dd}.{2}", // theFlight.STD
                    strFltNo, theFlight.STD, sAcRegNo); //BR13101
                //BR13101   strFltNo, theFlight.STD, theFlight.ACRegNo); // 1111
            }
        }

        /// <summary>
        /// title of UCM telex
        /// </summary>
        private string UCMFlightHeader
        {
            get
            {
                //BR13101<--
                string sAcRegNo = theFlight.ACRegNo;
                //BR13101sAcRegNo = sAcRegNo.Substring(0,1)+sAcRegNo.Substring(2,5); //remove "-"
                sAcRegNo = sAcRegNo.Replace("-", ""); //BR13101 remove "-",20130320 for 9M-XXD B-16205
                return string.Format("{0}/{1:dd}.{2}", theFlight.FlightNumber, theFlight.STD, sAcRegNo);
                //BR13101-->

                //BR13101<--
                //				return string.Format("{0}/{1}.{2}",
                //				                     theFlight.FlightNumber,
                //				                     theFlight.STD.ToString("ddMMM", new CultureInfo("en-US")).ToUpper(),
                //									 theFlight.ACRegNo);
                //BR13101-->
            }
        }

        #endregion

        #region LDM : Load Message

        /// <summary> create LDM</summary>
        /// <returns>string: LDM content string</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string LDM()
        {
            string ldm = "LDM\r\n";

            // 20060310, Flight Header
            ldm += string.Format("{0}.{1}.{2}\r\n", UCMFlightHeader, theFlight.CabinConfiguration, this.Crew);//2006/05/15 change Flight Number BR32 into BR0032

            // LDM
            ldm += this.LoadMessage();

            // SI
            ldm += this.LoadsheetSI();

            return ldm;

        }

        #endregion

        #region UCM : ULD Control Message

        /// <summary> create report content for Compartment or Bay</summary>
        /// <param name="cmp">Compartment or Bay</param>
        /// <param name="count">self-explanatory</param>
        /// <returns>UCMReturn： report content for Compartment or Bay</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private UCMReturn UCMCmpBay(CargoPosnBase cmp, int count)
        {
            // .<uld>/<destination>/<category>

            UCMReturn ucmRtn = new UCMReturn();
            ucmRtn.Count = count;
            ucmRtn.Str = "";
            Cargo cgo;

            if (cmp.CargoList.Count > 0) // unpartitionable cargo position, for example A1, A2, B, T, 11P
            {
                cgo = cmp.CargoList[0] as Cargo;

                if (!cgo.bFlagTransit)
                {
                    if (cgo._Category == "X")
                        ucmRtn.Str += string.Format(".{0}/{1}/X", cgo.ULDSerialNo, cgo.Dest);
                    else
                        ucmRtn.Str += string.Format(".{0}/{1}/{2}", cgo.ULDSerialNo,
                                                    LastRoute(cgo.Dest), cgo.Category.Substring(0, 1));
                    ucmRtn.Count++;
                    if (ucmRtn.Count == 3)
                    {
                        ucmRtn.Count = 0;
                        ucmRtn.Str += "\r\n";
                    }
                }
            }
            else // partitionable cargo position, for example C, D, 11
            {
                foreach (CargoPosnBase pos in cmp)
                {
                    if (pos.CargoList.Count > 0)
                    {
                        cgo = pos.CargoList[0] as Cargo;

                        if (!cgo.bFlagTransit)
                        {
                            if (cgo._Category == "X")
                                ucmRtn.Str += string.Format(".{0}/{1}/X", cgo.ULDSerialNo, cgo.Dest);
                            else
                                ucmRtn.Str += string.Format(".{0}/{1}/{2}", cgo.ULDSerialNo,
                                                            LastRoute(cgo.Dest), cgo.Category.Substring(0, 1));
                            ucmRtn.Count++;
                            if (ucmRtn.Count == 3)
                            {
                                ucmRtn.Count = 0;
                                ucmRtn.Str += "\r\n";
                            }
                        }
                    }
                } // foreach
            } // else

            return ucmRtn;
        }

        /// <summary>
        /// Find out the last station name in the route 
        /// </summary>
        /// <param name="cgoDest"> string is composed of destination</param>
        /// <returns>the last station name in the route</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string LastRoute(string cgoDest)
        {
            if (cgoDest.Length < 1)
                return "";
            string[] dests = cgoDest.Split(",".ToCharArray());
            int lastIdx = 0, idx;
            foreach (string dest in dests)
            {
                idx = RouteIndex(dest);
                if (idx > lastIdx)
                    lastIdx = idx;
            }
            if (lastIdx > 0)
                return theFlight.Route[lastIdx];
            else
                return "";
        }

        /// <summary>
        /// Find the index of station in route
        /// </summary>
        /// <param name="dest"> destination </param>
        /// <returns>int</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private int RouteIndex(string dest)
        {
            for (int i = 1; i < theFlight.Route.Length; i++)
                if (theFlight.Route[i] == dest)
                    return i;
            return 0;
        }

        /// <summary>
        /// Record of createed string and it's amount
        /// </summary>
        public class UCMReturn
        {
            public string Str;
            public int Count;
        }

        /// <summary> create UCM telex content</summary>
        /// <returns>string: UCM telex content string </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string UCM()
        {
            string ucm = "UCM\r\n";

            //UCM FlightHeader
            ucm += string.Format("{0}.{1}\r\nOUT\r\n", UCMFlightHeader, this.Route[0]);

            // one line has 3 containers/pallets infomation
            // .<uld>/<destination>/<category>
            ArrayList availList = PosnListToArrayList(this.cmpList);
            availList.Sort(new CmpComparer()); // sort compartment

            CargoPosnBase[] cmplist = availList.ToArray(typeof(CargoPosnBase)) as CargoPosnBase[];

            int dotCount = 0;
            UCMReturn ucmRtn;
            foreach (CargoPosnBase cmp in cmplist)
            {
                if (cmp.AreAllChildrenLeaf()) // Main Deck and Bulk
                {
                    if (cmp.IsBulkPosn() ||
                        theFlight.bTrimByCmpt) //use 'Trim By Compartment' for bulk or cargo position 
                        continue;
                    else
                    {
                        ucmRtn = this.UCMCmpBay(cmp, dotCount);
                        dotCount = ucmRtn.Count;
                        ucm += ucmRtn.Str;
                    }
                }
                else // Lower Deck, Available
                {
                    cmp.Sort(new BayComparer()); // Sort bay of compartment
                    foreach (CargoPosnBase bay in cmp)
                    {
                        ucmRtn = this.UCMCmpBay(bay, dotCount);
                        dotCount = ucmRtn.Count;
                        ucm += ucmRtn.Str;
                    }
                }
            }
            return ucm;
        }

        #endregion

        #region CPM : Container/Pallet distribution Message

        /// <summary>create the formated SHC data string</summary>
        /// <param name="pos">CargoPosnBase</param>
        /// <returns>SHC data  string </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string SHCFormattedString(CargoPosnBase pos)
        {
            string rtn = "";
            foreach (SpecialLoad csgn in theFlight.CargoPosnMgr.SHCConsignments(pos))
                rtn += ("." + csgn.SHC + "/" + csgn.Dest + "/" + Convert.ToInt32(csgn.Weight) + "/" + csgn.Pieces);

            return rtn;
        }

        /// <summary>create the formated SHC data string by destination</summary>
        /// <param name="pos">CargoPosnBase</param>
        /// <param name="dest"> destination </param>
        /// <returns>SHC data  string </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string SHCFormattedString(CargoPosnBase pos, string dest)
        {
            string rtn = "";
            foreach (SpecialLoad csgn in theFlight.CargoPosnMgr.SHCConsignments(pos))
            {
                if (csgn.Dest == dest)
                {
                    if (csgn.IsOtherSpecialLoad)
                        rtn += ("." + csgn.SHC + "/" + Convert.ToInt32(csgn.Weight) + "/" + csgn.Pieces); // 0817
                    else
                        rtn += ("." + csgn.SHC); // 0817
                }
            }
            return rtn;
        }

        /// <summary>visible position or not</summary>
        /// <param name="posn">CargoPosnBase</param>
        /// <returns>bool: visible or not</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private bool Visible(CargoPosnBase posn)
        {
            ArrayList aList = new ArrayList();
            aList.AddRange(theFlight.CargoPosnMgr.GenVisibleList(0));
            return aList.IndexOf(posn) >= 0;
        }

        /// <summary> create CPM report content for parameter of Compartment or Bay</summary>
        /// <param name="cmp">Compartment or Bay</param>
        /// <returns>string：report content of Compartment or Bay</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string CPMCmpBay(CargoPosnBase cmp)
        {
            /* -11P/<ULD>/<DEST>/<WT>/<CATEGORY>
             * -13L/<ULD>/<DEST>/<WT>/<CATEGORY>-13R/<ULD>/<DEST>/<WT>/<CATEGORY>
             * -52/<ULD>/<DEST>/<WT>/<CATEGORY>
             * -53/X
             * .A/<ULD>/<DEST>/<WT>/<CATEGORY>
             * .B/<ULD>/<DEST>/<WT>/<CATEGORY>
             * X indicates empty ULD
             * N indicate no ULD at position
             */
            
            string rtn = "";
            CargoPosnBase prevPos = null;

            if (cmp.CargoList.Count > 0) // unpartitionable cargo position, for example A1, A2, B, T, 11P
            {
                if (Visible(cmp))
                {
                    rtn += PosCPMFormatString(cmp, true);
                }
            }
            else // partitionable cargo position, for example C, D, 11
            {
                string str1 = "", str2 = "";
                foreach (CargoPosnBase pos in cmp)
                {

                    if (cmp.IndexOf(pos) % 2 == 1  ) //R
                    {
                        if (prevPos.CargoList.Count > 0) //L
                            str1 += PosCPMFormatString(prevPos, false);
                        else if (Visible(prevPos)) // && prevPos.NoFit)
                            str1 += string.Format("-{0}/N", prevPos.Name);

                        if (pos.CargoList.Count > 0)
                        {
                            str2 += PosCPMFormatString(pos, true);
                        }
                        else
                        {
                            if (Visible(pos))
                            {
                                str2 += string.Format("-{0}/N\r\n", pos.Name);
                            }
                            else
                            {
                                if (str1 != "" && cmp.Count < 3)
                                    str2 += "\r\n";
                            }
                        }
                        if (str1 != string.Empty || str2 != string.Empty)
                        {
                            rtn = reformat(str1 + str2);
                        }
                        else if (str1 == null || str2 == null)
                        {
                            rtn = reformat(str1 + str2);
                        }
                        else 
                        {
                            rtn = reformat(str1 + str2);
                        }
                    }

                    if (cmp.IndexOf(pos) % 2 == 0) //R
                        prevPos = pos;
                    else
                        prevPos = null;

                    
                    //<!--  #BR17202 THOMAS  CPM Telex 內容新增center load
                    if (cmp.IndexOf(pos) == 4)
                    {
                        if (prevPos.CargoList.Count > 0) //L
                            str1 += PosCPMFormatString(prevPos , false)+"\r\n";
                        else if (Visible(prevPos)) // && prevPos.NoFit)
                            str1 += string.Format("-{0}/N"+"\r\n", prevPos.Name);

                        if (str1 != string.Empty || str2 != string.Empty)
                        {
                            rtn = reformat(str1 + str2);
                        }
                        else if (str1 == null || str2 == null)
                        {
                            rtn = reformat(str1 + str2);
                        }
                        else
                        {
                            rtn = reformat(str1 + str2);
                        }
                    }
                    //BR17202-->
                    
                } // foreach
            } // else

            if (rtn == "" && Visible(cmp)) // && cmp.NoFit)
                rtn += string.Format("-{0}/N\r\n", cmp.Name);
            return rtn;
        }

        /// <summary>get object category for cargo position by destination and object category parameter</summary>
        /// <param name="pos">CargoPosnBase</param>
        /// <param name="catg">Category</param>
        /// <param name="route">route</param>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string CargoCatg(CargoPosnBase pos, string catg, string route)
        {
            if (catg != "B")
                return catg;

            bool bc = false, by = false;
            if (pos.getNetWt("BC", route) > 0)
                bc = true;
            if (pos.getNetWt("BY", route) > 0)
                by = true;
            if (bc && by)
                return "BC/BY";
            if (bc)
                return "BC";
            if (by)
                return "BY";

            return "";
        }

        /// <summary>get object category for cargo position by destination parameter</summary>
        /// <param name="pos">CargoPosnBase</param>
        /// <param name="route">route</param>
        /// <returns>string : category</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string CargoCatg(CargoPosnBase pos, string route)
        {
            Cargo cgo = null;
            string uldCatg = "";
            if (!pos.IsBulkPosn())
            {
                cgo = pos.CargoList[0] as Cargo;
                uldCatg = firstItem(cgo.Category);
            }
            string rstr = "";
            if (uldCatg != "BY" && uldCatg != "BC" && uldCatg != "BT")
            {
                //<!--#BR18112 Thomas 只檢查"C", "M", "E", "O" 
                //string[] arCatg = {"C", "M", "E", "BC", "BY", "O"};//BR071010
                string[] arCatg = { "C", "M", "E", "O" };   //#BR18112
                //#BR18112 -->
                bool isE = false;
                foreach (string catg in arCatg)
                {
                    if (!pos.IsBulkPosn() && uldCatg == catg && firstItem(cgo.Dest) == route && pos.getGrossWt(catg, route) == cgo.TareWt)
                    {
                        if (catg == "E")
                            isE = true;
                        else
                            return "/" + catg;
                    }

                    if (pos.getNetWt(catg, route) > 0 || isE)
                    {
                        if (uldCatg.Length > 0 && uldCatg.IndexOf(catg) >= 0)
                            rstr = ("/" + catg) + rstr;
                        else
                            rstr = rstr + ("/" + catg);
                    }
                    isE = false;
                }
            }

            //#BR071010 <--
            if (rstr != "") 
            { 
                #region #BR071010-1
                if ((int)((EWBS.ICargo)((EWBS.ICargo)(cgo))).Consignments.Count == 2)
                {
                    string tmpCatg = "";
                    if (((EWBS.ICargo)((EWBS.ICargo)(cgo)).Consignments[0])._Category.Trim() != "")
                        tmpCatg = ((EWBS.ICargo)((EWBS.ICargo)(cgo)).Consignments[0]).Remark.Trim();
                    else
                        tmpCatg = ((EWBS.ICargo)((EWBS.ICargo)(cgo)).Consignments[0]).Category;

                    if (((EWBS.ICargo)((EWBS.ICargo)(cgo)).Consignments[1]).Remark.Trim() != "")
                        tmpCatg = tmpCatg + "," + ((EWBS.ICargo)((EWBS.ICargo)(cgo)).Consignments[1]).Remark.Trim();
                    else
                        tmpCatg = tmpCatg + "," + ((EWBS.ICargo)((EWBS.ICargo)(cgo)).Consignments[1]).Category;

                    ArrayList arrCatg = new ArrayList();

                    string s_remark = tmpCatg.ToUpper().ToString().Substring(0, 2);
                    if (s_remark == "BT" || s_remark == "BY") //#BR071010 
                    { 
                        char[] splitChar = new char[] { ',' };//#BR071010

                        foreach (string subString in tmpCatg.Split(splitChar))
                        {
                            if (!arrCatg.Contains(subString))
                            {
                                arrCatg.Add(subString);
                            }
                        }
                        rstr = "";
                        for (int i = 0; i < arrCatg.Count; i++)
                        {
                            rstr = rstr + "/" + arrCatg[i].ToString();
                        }
                    } //#BR071010
                    else
                    { 
                    } 
                }
                else if ((int)((EWBS.ICargo)((EWBS.ICargo)(cgo))).Consignments.Count == 1)
                {
                    if (((EWBS.ICargo)((EWBS.ICargo)(cgo)).Consignments[0]).Remark.Trim() != "")
                    {
                        string tmpCatg = ((EWBS.ICargo)((EWBS.ICargo)(cgo)).Consignments[0]).Remark.Trim();
                        ArrayList arrCatg = new ArrayList();

                        char[] splitChar = new char[] { ',' };

                        foreach (string subString in tmpCatg.Split(splitChar))
                        {
                            if (!arrCatg.Contains(subString))
                            {
                                arrCatg.Add(subString);
                            }
                        }
                        rstr = "";
                        for (int i = 0; i < arrCatg.Count; i++)
                        {
                            rstr = rstr + "/" + arrCatg[i].ToString();
                        }
                    }
                    //return rstr;
                }
                #endregion #BR071010-1
            } //#BR071010 -->
            else 
            {
                //<!--#BR18112
                rstr = CargoCatgBag(pos, route); //處理行李， 
                //#BR18112-->
            }
            return rstr;
        }

        //<!--#BR18112 Thomas 新增bag Remarker 處理
        /// <summary>get object category for cargo position by destination parameter</summary>
        /// <param name="pos">CargoPosnBase</param>
        /// <param name="route">route</param>
        /// <returns>string : category</returns>
        private string CargoCatgBag(CargoPosnBase pos, string route)
        {
            Cargo cgo = null;
            string uldCatg = "";
            if (!pos.IsBulkPosn())
            {
                cgo = pos.CargoList[0] as Cargo;
                uldCatg = firstItem(cgo.Category);
            }

            string rstr = "";
            string[] CatgArr = cgo.Category.Split(',');
            bool f1 = false;
            bool f2 = false;
            for (int i = 0; i < CatgArr.Length; i++)
            {
                if (CatgArr[i] == "BC")
                {
                    f1 = true;
                }

                if (CatgArr[i] == "BY")
                {
                    f2 = true;
                }
            }

            string tmpstr = "";
            foreach (string catg in CatgArr)
            {
                if (!pos.IsBulkPosn() && pos.getGrossWt(catg, route) > 0)//&& pos.getNetWt(catg, route) > 0)  //check catg only can BC/BY/BT
                {
                    //isMix = false 
                    foreach (ICargo Consignment in ((EWBS.ICargo)((EWBS.ICargo)(cgo))).Consignments)
                    {
                        string remark = ""; 
                        //ULD判斷是否混打
                        if (f1 && f2) //ULD混打(F1、BY BULK)
                        {
                            if (Consignment.Dest == route)
                            {
                                //<!--#BR19003 新增BS規則，必須跟 CheckRemark() 裡面的設定相同
                                if (Consignment._Category == "BC" && 
                                    (
                                        Consignment.Remark == "BS" || 
                                        Consignment.Remark == "BT" || 
                                        Consignment.Remark == "BC,BS" || 
                                        Consignment.Remark == "BC,BT" ||
                                        Consignment.Remark == "BS,BT" ||
                                        Consignment.Remark == "BC,BS,BT")
                                    )
                                {
                                    //輸入上述條件時 CPM 的CATG改為REMARK的內容 。 
                                    remark = Consignment.Remark.ToUpper();
                                }
                                else if (Consignment._Category == "BY" && 
                                        (
                                            Consignment.Remark == "BS" || 
                                            Consignment.Remark == "BT" || 
                                            Consignment.Remark == "BS,BY" ||
                                            Consignment.Remark == "BT,BY" ||
                                            Consignment.Remark == "BS,BT" || 
                                            Consignment.Remark == "BS,BT,BY")
                                        )
                                {
                                    //輸入上述條件時 CPM 的CATG改為REMARK的內容 。 
                                    remark = Consignment.Remark.ToUpper();
                                }
                                else if (Consignment._Category != "BY")
                                {
                                    //非BC、BY櫃，REMARK無輸入或是輸入非 指定文字 ，則 CPM 顯示 原來的Category
                                    remark = Consignment._Category;
                                }
                            }
                        }
                        else //ULD單一(F1、BY)
                        {
                            if (Consignment.Dest == route)
                            {
                                //<!--#BR19003 新增BS規則
                                if (
                                        (Consignment._Category == "BC" && Consignment.Remark == "BS") ||
                                        (Consignment._Category == "BC" && Consignment.Remark == "BT") ||
                                        (Consignment._Category == "BC" && Consignment.Remark == "BC,BS") ||
                                        (Consignment._Category == "BC" && Consignment.Remark == "BC,BT") ||
                                        (Consignment._Category == "BC" && Consignment.Remark == "BC,BS,BT") ||
                                        (Consignment._Category == "BY" && Consignment.Remark == "BS") ||
                                        (Consignment._Category == "BY" && Consignment.Remark == "BT") ||
                                        (Consignment._Category == "BY" && Consignment.Remark == "BS,BT")
                                   )
                                {
                                    //REMARK輸入BC,BT、BT、BT,BY 時 CPM 的CATG改為REMARK的內容 
                                    remark = Consignment.Remark.ToUpper();
                                }
                                else
                                {
                                    //REMARK 無輸入或是輸入非 BC,BT、BT、BT,BY 文字 ，則 CPM 顯示 原來的Category 
                                    remark = Consignment._Category;
                                }
                            }
                        }
                        tmpstr = tmpstr + ("," + remark);
                    }
                }
            }

            ArrayList arrCatg = new ArrayList();
            char[] splitChar = new char[] { ',' };
            foreach (string subString in tmpstr.Split(splitChar))
            {
                if (!arrCatg.Contains(subString))
                {
                    arrCatg.Add(subString);
                }
            }

            rstr = "";

            if (arrCatg != null)
            {
                for (int i = 0; i < arrCatg.Count; i++)
                {
                    if (arrCatg[i].ToString() != "")
                    {
                        rstr = rstr + "/" + arrCatg[i].ToString();
                    }
                }
            }
            return rstr;
        }
        //#BR18112-->

        /// <summary>
        /// Get the first item
        /// </summary>
        /// <param name="catgOrDest">multiple kinds or destinations </param>
        /// <returns></returns>
        private string firstItem(string catgOrDest)
        {
            string first = "";
            if (catgOrDest != "" && catgOrDest.IndexOf(",") > 0)
                first = catgOrDest.Substring(0, catgOrDest.IndexOf(","));
            else
                first = catgOrDest;
            return first;
        }

        /// <summary> create  string of cargo catalog and weight of Bulk</summary>
        /// <param name="pos">CargoPosnBase</param>
        /// <param name="route">route</param>
        /// <returns>string : category</returns>
        private string BulkWtCatg(CargoPosnBase pos, string route)
        {
            string rstr = "";
            string[] arCatg = { "C", "M", "E", "BC", "BY", "O" };
            foreach (string catg in arCatg)
            {
                double wt = pos.getNetWt(catg, route);

                if (wt > 0)
                {
                    //<!--#BR18112 THOMAS Remark 欄位輸入 指定的字串(CheckRemark)時，置換CPM內容
                    foreach (ICargo item in pos.CargoList)
                    {
                        if (item._Category == catg && item.Dest == route)
                        {
                            rstr += string.Format("/{0}/{1}", wt, CheckRemark(item.Remark, item._Category) ? item.Remark.Replace(",", "/") : catg);
                            break; //避免同catg route 產生重複資料
                        }
                    }

                     //rstr += string.Format("/{0}/{1}", wt, catg);
                    //#BR18112-->
                }
            }
            return rstr;
        }

        /// <summary>
        /// #BR18112 THOMAS Remark 字串檢查
        ///  //<!--#BR19003 新增BS規則
        /// </summary>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool CheckRemark(string remark, string Category)
        {
            if (Category == "BC")
            {
                //string[] ArrRemark = remark.Split(',');
                //string[] ArrChkStr = { "BC", "BS", "BT" };
                //return ArrRemark.Intersect(ArrChkStr).ToArray().Length == ArrRemark.Length ? true : false; //判斷輸入的CATE數，是否完全符合條件內 

                string[] ArrChkStr = { "BS", "BT", "BC,BS", "BC,BT", "BS,BT", "BC,BS,BT" }; //條件
                foreach (var item in ArrChkStr)
                {
                    if (remark == item)
                    {
                        return true;
                    }
                }
                return false;
               
            }
            else if (Category == "BY")
            {
                //string[] ArrRemark = remark.Split(',');
                //string[] ArrChkStr = { "BY", "BS", "BT" };
                //return ArrRemark.Intersect(ArrChkStr).ToArray().Length == ArrRemark.Length ? true : false; //判斷輸入的CATE數，是否完全符合條件內 
               
                string[] ArrChkStr = { "BS", "BT", "BS,BY", "BT,BY", "BS,BT", "BS,BT,BY" }; //條件
                foreach (var item in ArrChkStr)
                {
                    if (remark == item)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// #BR18112 THOMAS Remark 字串檢查(原)
        /// </summary>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool CheckRemark(string remark)
        {
            string[] ArrRemark = { "BT", "BY,BT", "BC,BT", "BT,BY", "BT,BC" };
            
            foreach (var item in ArrRemark)
            {
                if (remark ==  item)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary> create position data string for CPM</summary>
        /// <param name="pos">CargoPosnBase</param>
        /// <param name="newline">crlf</param>
        /// <returns>string: CPM之position data  string </returns>
        private string PosCPMFormatString(CargoPosnBase pos, bool newline)
        {
            string rtn = "";
            Cargo cgo = pos.CargoList[0] as Cargo;

            if (cgo._Category == "X")
            {
                #region BR13101 Remark<--
                /*
                if (cgo.ULDSerialNo.Length == 5)
                {
                    string sULDSerialNo = cgo.ULDSerialNo.Substring(0, 3) + "00000" + cgo.ULDSerialNo.Substring(3, 2);
                    if (newline)
                        return string.Format("-{0}/{1}/{2}/{3}/X\r\n", pos.Name, cgo.ULDSerialNo, cgo.Dest, pos.getWeight());
                    else
                        return string.Format("-{0}/{1}/{2}/{3}/X", pos.Name, sULDSerialNo, cgo.Dest, pos.getWeight());
                }
                else
                {
                    if (newline)
                        return string.Format("-{0}/{1}/{2}/{3}/X\r\n", pos.Name, cgo.ULDSerialNo, cgo.Dest, pos.getWeight());
                    else
                        return string.Format("-{0}/{1}/{2}/{3}/X", pos.Name, cgo.ULDSerialNo, cgo.Dest, pos.getWeight());

                }
                */
                #endregion BR13101-->

                if (newline)
                    return string.Format("-{0}/{1}/{2}/{3}/X\r\n", pos.Name, cgo.ULDSerialNo.Replace(" ", ""), cgo.Dest, pos.getWeight()); //BR18114 Thomas CPM ULD SN 去除空白
                else
                    return string.Format("-{0}/{1}/{2}/{3}/X", pos.Name, cgo.ULDSerialNo.Replace(" ", ""), cgo.Dest, pos.getWeight()); //BR18114 Thomas CPM ULD SN 去除空白
            }

            if (pos.CargoList.Count == 0) return "";

            rtn += string.Format("-{0}/{1}", pos.Name, cgo.ULDSerialNo.Replace(" ", "")); //BR18114  Thomas CPM ULD SN 去除空白

            string trstr = "";
            foreach (string route in this.Route)
            {
                if (route == this.Route[0])
                    continue;
                string rstr = ("/" + route);
                double wt = pos.getGrossWt("", route);
                string catg = CargoCatg(pos, route);
                if (catg != "")
                {
                    //BR13109<-- for star
                    if (catg.IndexOf("BC/BY") >= 0)
                        catg = catg.Replace("BC/BY", "BC");
                    else if (catg.IndexOf("BY/BC") >= 0)
                        catg = catg.Replace("BY/BC", "BC");
                    //BR13109-->

                    rstr += string.Format("/{0}{1}", wt, catg);
                    rstr += SHCFormattedString(pos, route);
                    if (rstr.Length > 4)
                    {
                        if (cgo._Dest == route)
                            trstr = rstr + trstr;
                        else
                            trstr = trstr + rstr;
                    }
                }
            }

            if (trstr != "")
                rtn += trstr;

            if (!cgo.IsContainer)
                rtn += ("/" + cgo.HeightCode);

            if (newline)
                rtn += "\r\n";

            return rtn;
        }

        /// <summary>combine two strings as a new string, then format the string</summary>
        /// <param name="str">string</param>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string reformat(string str)
        {
            int width = LineWidth;
            if (str.Length < width)
                return str;
            else
            {
                int i;
                for (i = width - 1; i >= 0; i--)
                    if (str[i] == '/')
                        break;

                string str1 = str.Substring(0, i);
                string str2 = str.Substring(i, str.Length - i);

                //BR
                if (i == 0 && str2.Length > width) 
                {
                    str1 = str2.Substring(0,width);
                    str2 = str2.Substring(width, str2.Length - width);
                }

                return str1 + "\r\n" + reformat(str2);
            }
        }

        /// <summary> create position data string for CPM</summary>
        /// <param name="pos">CargoPosnBase</param>
        /// <returns>string: position data string for CPM </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string CPMBulkPosn(CargoPosnBase pos)
        {
            string rtn = "";
            double[] arWt = new double[5];
            string[] arCat = new string[5];
            for (int i = 0; i < arWt.Length; i++) arWt[i] = 0;
            for (int i = 0; i < arCat.Length; i++) arCat[i] = "";

            // collect weights by dest
            rtn += ("-" + pos.Name);

            if (pos.CargoList.Count == 0) // No Consignments in this Bulk Position
            {
                if (Visible(pos))
                    rtn += "/N";
            }

            foreach (string route in this.Route)
            {
                if (route == this.Route[0])
                    continue;

                string rstr = ("/" + route);

                double wt = pos.getGrossWt("", route);
                if (wt > 0)
                    rstr += BulkWtCatg(pos, route);

                rstr += SHCFormattedString(pos, route);
                if (rstr.Length > 4)
                    rtn += rstr;
            }

            if (rtn != "")
                rtn += "\r\n";

            return reformat(rtn);
        }

        /// <summary> create Compartment data string for CPM</summary>
        /// <param name="cmp">CargoPosnBase</param>
        /// <returns>string: Compartment data string for CPM</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string CPMBulkCmp(CargoPosnBase cmp)
        {
            string rtn = "";
            if (cmp.Count == 0)
                rtn += CPMBulkPosn(cmp);
            else
            {
                foreach (CargoPosnBase pos in cmp)
                    rtn += CPMBulkPosn(pos);
            }
            return rtn;
        }

        /// <summary> create CPM content</summary>
        /// <returns>string:CPM content</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string CPM()
        {
            string cpm = "CPM\r\n";

            //BR13101<--
            // Flight Header
            // cpm += string.Format("{0}.{1}-{2}.{3}\r\n", FlightHeader, theFlight.ACType.name,theFlight.LoadVersion,theFlight.ACType.LoadVersionDescription(theFlight.LoadVersion));
            //BR13101-->

            //BR13101<-- 
            // Flight Header
            //new:BR31/17.B16705.LDV580 old:BR31/17.B-16705.BR77L-580.8P/20A改以LDV580取代.
            cpm += string.Format("{0}.LDV{1}\r\n", FlightHeader, theFlight.LoadVersion);
            //BR13101-->

            // for pos, only if (IndexOf("L") < 0) skip "\r\n";
            // Format:
            // {-<pos>/<uld>/<airport>/<weight>/<category:C/B/M>{.<SHC>}*/[L/R]}...1or2
            ArrayList availList = PosnListToArrayList(this.cmpList);
            availList.Sort(new CmpComparer()); // sort the compartment

            CargoPosnBase[] cmplist = availList.ToArray(typeof(CargoPosnBase)) as CargoPosnBase[];

            foreach (CargoPosnBase cmp in cmplist)
            {
                if (cmp.AreAllChildrenLeaf()) // Main Deck and Bulk
                {
                    if (cmp.IsBulkPosn() || theFlight.bTrimByCmpt)//76E cmp 5 is the only bulk posn(51, 52 ...not exist)
                        cpm += this.CPMBulkCmp(cmp);
                    else
                        cpm += this.CPMCmpBay(cmp);
                }
                else // Lower Deck, Available
                {
                    cmp.Sort(new BayComparer()); // sort the bay of compartment

                    foreach (CargoPosnBase bay in cmp)
                    {
                        cpm += this.CPMCmpBay(bay);
                    }
                }
            }

            string mySI = string.Format("ZFW   {0:#.#}  LIZFW   {1:#.#}", theFlight.ZFW, theFlight.LIZFW);

            if (theFlight.ACType.IsDLW)
                mySI += string.Format("  MACDLW   {0:#.#}", this.MACDLW);

            if (theFlight.ACType.cpi != null)
                mySI += string.Format("   CPI   {0:#.#}", theFlight.DisplayCPI);

            if (theFlight.ACType.name == "BRM1F" || theFlight.ACType.name == "BRM1T" || theFlight.ACType.name == "BRM1N")
            {
                mySI += string.Format("   MACZFW   {0,4:#,##.0}", theFlight.MACZFW);
            }

            //BR15106 cpm += string.Format("\r\nSI\r\n{0}\r\n", mySI);
            cpm += string.Format("\r\nSI \r\n{0}\r\n", mySI);  //BR15106 SI add space

            return cpm;
        }

        #endregion

        #region LPM: Load Planning Message

        /// <summary> create report content of Compartment or Bay parameter</summary>
        /// <param name="cmp">Compartment or Bay</param>
        /// <returns>string：report content of Compartment or Bay parameter</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        private string LPMCmpBay(CargoPosnBase cmp)
        {
            // { .<pos>/<destination>/<category:C/B/M><weight>.<SHC> | -<pos>/X }*
            // X indicates empty ULD
            // N indicate no ULD at position

            string rtn = "";
            Cargo cgo;

            if (cmp.CargoList.Count > 0) // unpartitionable cargo position, for example A1, A2, B, T, 11P
            {
                if (cmp.IsBulkPosn())
                {
                    foreach (Consignment csgn in cmp.CargoList)
                    {
                        if (cmp.CargoList.IndexOf(csgn) == 0)
                            rtn += ("." + cmp.Name);
                        rtn += string.Format("/{0}/{1}/{2}", csgn.Dest.Substring(0, 3),
                                             csgn.Category.Substring(0, 1), csgn.Weight);
                    }
                }
                else
                {
                    cgo = cmp.CargoList[0] as Cargo;
                    if (cgo._Category == "X")
                        rtn = string.Format(".{0}/X", cmp.Name);
                    else
                        rtn = string.Format(".{0}/{1}/{2}/{3}", cmp.Name,
                                            cgo.Dest.Substring(0, 3), cgo.Category.Substring(0, 1), cmp.getWeight());
                }
            }
            else // partitionable cargo position, for example C, D, 11
            {
                foreach (CargoPosnBase pos in cmp)
                {
                    if (pos.IsBulkPosn())
                    {
                        foreach (Consignment csgn in pos.CargoList)
                        {
                            if (pos.CargoList.IndexOf(csgn) == 0)
                                rtn += ("." + pos.Name);
                            rtn += string.Format("/{0}/{1}/{2}", csgn.Dest.Substring(0, 3),
                                                 csgn.Category.Substring(0, 1), csgn.Weight);
                        }
                    }
                    else if (pos.CargoList.Count > 0)
                    {
                        cgo = pos.CargoList[0] as Cargo;

                        if (cgo._Category == "X")
                            rtn = string.Format(".{0}/X", cmp.Name);
                        else
                            rtn += string.Format(".{0}/{1}/{2}/{3}", pos.Name,
                                                 cgo.Dest.Substring(0, 3), cgo.Category.Substring(0, 1), pos.getWeight());
                    }
                }
            }
            return rtn;
        }


        /// <summary> create LPM content</summary>
        /// <returns>string:LPM content</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string LPM()
        {
            string lpm = "LPM\r\n";

            // Flight Header
            lpm += string.Format("{0}\r\n", FlightHeader);

            // Format:
            // -<airport>.PAX</By Class/>.B<weight>.C<weight>.M<weight>
            // .TW######
            // { .<pos>/<destination>/<category:C/B/M><weight>.<SHC> | -<pos>/X }*
            // X indicates empty ULD
            for (int i = 1; i < this.Route.Length; i++)
            {
                lpm += string.Format("-{0}.PAX/{1}/{2}/{3}.B{4},C{5}.M{6}\r\n",
                                     this.Route[i], this.Class1Pax(i), this.Class2Pax(i), this.Class3Pax(i),
                                     this.BaggageNetWt(i), this.CargoNetWt(i), this.PostNetWt(i));
            }

            lpm += string.Format(".TW{0}\r\n", theFlight.TotalTrafficWt);

            string cmpstr = "";
            string tmpstr = "";

            ArrayList availList = PosnListToArrayList(this.cmpList);
            availList.Sort(new CmpComparer()); // sort the compartment

            CargoPosnBase[] cmplist = availList.ToArray(typeof(CargoPosnBase)) as CargoPosnBase[];

            foreach (CargoPosnBase cmp in cmplist)
            {
                if (cmp.AreAllChildrenLeaf()) // Main Deck and Bulk
                {
                    tmpstr = this.LPMCmpBay(cmp);
                    if (tmpstr == "") continue;
                    lpm += (tmpstr + "\r\n");
                }
                else // Lower Deck, Available
                {
                    // sort the bay of compartment

                    cmpstr = "";
                    foreach (CargoPosnBase bay in cmp)
                    {
                        tmpstr = this.LPMCmpBay(bay);
                        if (cmpstr.Length + tmpstr.Length < LineWidth)
                        {
                            cmpstr += tmpstr;
                        }
                        else
                        {
                            lpm += (cmpstr + "\r\n");
                            cmpstr = tmpstr;
                        }
                    }
                    if (cmpstr == "") continue;
                    lpm += (cmpstr + "\r\n");
                }
            }

            // SPCL
            // .SEA/AVI/52
            // .EWR/AVI/53
            bool spclBegin = true;
            foreach (CargoPosnBase pos in theFlight.CargoPosnMgr.GenVisibleList(0))
                foreach (SpecialLoad csgn in theFlight.CargoPosnMgr.SHCConsignments(pos))
                {
                    if (spclBegin)
                    {
                        lpm += "SPCL\r\n";
                        spclBegin = false;
                    }
                    lpm += ("." + csgn.Dest + "/" + csgn.SHC + "/" + pos.Name + "\r\n");
                }

            return lpm;
        }

        #endregion

        #region ALI: Abbreviated Load Information Messsage

        /// <summary> create ALI content</summary>
        /// <returns>string:ALI content</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string ALI()
        {
            string ali = "ALI\r\n";

            // Flight Header
            ali += string.Format("{0}\r\n", FlightHeader);

            /*
             * .1/615.2/2100.4/951.PAX/12/108/.AVI/4,HEA/2/415
             * SI
             */
            string cmpstr = "";
            string tmpstr = "";

            ArrayList availList = PosnListToArrayList(this.cmpList);
            availList.Sort(new CmpComparer()); // sort compartment

            CargoPosnBase[] cmplist = availList.ToArray(typeof(CargoPosnBase)) as CargoPosnBase[];

            foreach (CargoPosnBase cmp in cmplist)
            {
                if (cmp.getWeight() == 0) continue;
                tmpstr = string.Format(".{0}/{1}", cmp.Name, cmp.getWeight());
                if (cmpstr.Length + tmpstr.Length < LineWidth)
                    cmpstr += tmpstr;
                else
                {
                    ali += (cmpstr + "\r\n");
                    cmpstr = tmpstr;
                }
            }
            // .PAX/12/108/
            tmpstr = string.Format(".PAX/{0}/{1}/{2}",
                                   this.Class1Pax(0), this.Class2Pax(0), this.Class3Pax(0));
            if (cmpstr.Length + tmpstr.Length < LineWidth)
                cmpstr += tmpstr;
            else
            {
                ali += (cmpstr + "\r\n");
                cmpstr = tmpstr;
            }
            //  SHC .AVI/4,HEA/2/415
            tmpstr = "";
            foreach (CargoPosnBase pos in theFlight.CargoPosnMgr.GenVisibleList(0))
                foreach (SpecialLoad csgn in theFlight.CargoPosnMgr.SHCConsignments(pos))
                {
                    tmpstr = ("." + csgn.SHC);
                    if (csgn.Pieces > 0)
                        tmpstr += ("/" + csgn.Pieces);
                    if (csgn.Weight > 0)
                        tmpstr += ("/" + csgn.Weight);

                    if (cmpstr.Length + tmpstr.Length < LineWidth)
                        cmpstr += tmpstr;
                    else
                    {
                        ali += (cmpstr + "\r\n");
                        cmpstr = tmpstr;
                    }
                }

            ali += (cmpstr + "\r\n");
            return ali;
        }

        #endregion

        #region SLS: Statistical Load Summary

        /// <summary> get non Revenue Pax of some route</summary>
        /// <param name="routeIndex">Route Index</param>
        /// <returns>double: non Revenue Pax</returns>
        public int nonRevenuePax(int routeIndex)
        {
            return 0;
        }

        /// <summary> get Total Baggage Weight of some route</summary>
        /// <param name="routeIndex">Route Index</param>
        /// <returns>double: Total Baggage Weight</returns>
        public double ttlBagWt(int routeIndex)
        {
            return theFlight.CargoPosnMgr.getBagWt(Route[routeIndex]);
        }

        /// <summary> get Total Cargo Weight of some route</summary>
        /// <param name="routeIndex">Route Index</param>
        /// <returns>double: Total Cargo Weight</returns>
        public double ttlCargoWt(int routeIndex)
        {
            return theFlight.CargoPosnMgr.getFreWt(Route[routeIndex]);
        }

        /// <summary> get Total Post Weight of some route</summary>
        /// <param name="routeIndex">Route Index</param>
        /// <returns>double: Total Post Weight</returns>
        public double ttlPostWt(int routeIndex)
        {
            return theFlight.CargoPosnMgr.getPosWt(Route[routeIndex]);
        }

        /// <summary> create content Statistical Load Summary</summary>
        /// <returns>string:StatisticalLoadSummary</returns>
        public string StatisticalLoadSummary()
        {
            string strSLS = ""; // SLS full string

            for (int i = 1; i < Route.Length; i++)
            {
                strSLS += this.SLSbyDstn(i);
            }
            return strSLS;
        }
        
        /// <summary> create SLS content of some route</summary>
        /// <param name="routeIndex">Route Index</param>
        /// <returns>string:SLS content</returns>
        private string SLSbyDstn(int routeIndex)
        {
            string slsLineByDstn = "";
            if (theFlight.TotalTrafficWt > 0)
            {
                slsLineByDstn = string.Format("-{0}.YY/{1}/{2}/{3}.GG/{4}.B/{5}.C/{6}.M/{7}\r\n",Route[routeIndex],Male(routeIndex), 
                    Female(routeIndex), Child(routeIndex),nonRevenuePax(routeIndex),ttlBagWt(routeIndex),ttlCargoWt(routeIndex),ttlPostWt(routeIndex));
            }
            else
            {
                slsLineByDstn = string.Format("-{0}.{1}\r\n", Route[routeIndex], "NIL");
            }
            return slsLineByDstn;
        }

        /// <summary> create SLS content</summary>
        /// <returns>string:SLS content</returns>
        public string SLS()
        {
            /*
             * -SEA,FF/3/5.YY/12/23.CC/123/3.B/2341.C/7234.M/270
             * -EWR.NIL
             * SI
             */
            string slsString = "SLS\r\n";
            slsString += string.Format("{0}.{1}\r\n", FlightHeader, this.Route[0]);// Flight Header
            slsString += this.StatisticalLoadSummary(); // SLS
            slsString += "END\r\n";

            return slsString;
        }

        #endregion

        #region UWS: ULD Load Weight Signal

        //#BR071015 <--
        public string ZFW_MSG()
        {
            string sContent = "";
            string sTitle = "";
            
            //int i = -1;

            string sText = "";
            string sText1 = "";
            float j = 0;
            /*
            if (Math.Abs((theFlight.PTOW - theFlight.TOW)) >= 1000 && Math.Abs((theFlight.PZFW - theFlight.ZFW)) >= 1000)
            {
                sContent = "SUBJ:ZFW DISCREPANCY NOTICE OF " + theFlight.FlightNumber + " QD\r\n";
                sContent = sContent + "\r\n///////////////////////////////\r\n";
                sTitle = "//  ZFW DISCREPANCY NOTICE  //";
                sContent = sContent + sTitle + "\r\n";
                i = 2;
            }
            else if (Math.Abs((theFlight.PTOW - theFlight.TOW)) >= 1000)
            {
                sContent = "SUBJ:ZFW AND TOW DISCREPANCY NOTICE OF " + theFlight.FlightNumber + " QD\r\n";
                sContent = sContent + "\r\n//////////////////////////////////////\r\n";
                sTitle = "//  TOW DISCREPANCY NOTICE          //";
                sContent = sContent + sTitle + "\r\n";
                i = 1;
            }
            else if (Math.Abs((theFlight.PZFW - theFlight.ZFW)) >= 1000)
            */

            if (Math.Abs((theFlight.PZFW - theFlight.ZFW)) >= 1000)
            {
                sContent = "SUBJ:ZFW DISCREPANCY NOTICE OF " + theFlight.FlightNumber + "\r\n";
                sContent = sContent + "\r\n//////////////////////////////////////\r\n";
                sTitle = "//  ZFW DISCREPANCY NOTICE          //";
                sContent = sContent + sTitle + "\r\n";
                //i = 0;
                if (theFlight.ZFW - theFlight.PZFW >= 1000)
                {
                    sText = "MORE";
                    sText1 = "INCREASED";

                }
                else
                {
                    sText = "LESS";
                    sText1 = "DECREASED";
                }
                j = Math.Abs(theFlight.ZFW - theFlight.PZFW);
            }

            sContent = sContent + "//////////////////////////////////////\r\n\r\n";  //#BR071015
            //#BR08105 sContent = sContent + "FLT NO:" + theFlight.Name + "\r\n";
            sContent = sContent + "FLT NO:" + theFlight.Name.Substring(0, (theFlight.Name.Length - 4)) + "\r\n";  //#BR08105
            sContent = sContent + "REG   :" + theFlight.ACRegNo + "\r\n";

            //#BR071015<--
            //			if(i ==2)
            //			{
            //				sContent = sContent + "EZFW  :" + theFlight.ZFW + "\r\n";
            //				sContent = sContent + "ETOW  :" + theFlight.TOW + "\r\n";
            //			}
            //			else if(i ==1)
            //			{
            //				sContent = sContent + "ETOW  :" + theFlight.TOW + "\r\n";
            //			}
            //			else if(i ==0)
            //			{
            //				sContent = sContent + "EZFW  :" + theFlight.ZFW + "\r\n";				
            //			}
            //#BR071015-->
            sContent = sContent + "EZFW  :" + theFlight.ZFW + " KG" + "\r\n\r\n"; //#BR071015
            sContent = sContent + sText + " THAN CFP:" + Convert.ToString(j) + " KG DU2 CGO " + sText1 + "\r\n\r\n";
            sContent = sContent + "PLZ CORR CFP & FUEL SHEET N RESEND ASAP" + "\r\n\r\n";  //BR09109 amend word
            sContent = sContent + "BRGDS//" + theFlight.Author.ToString() + "\r\n";

            return sContent;
        }
        //#BR071015 -->

        /// <summary> create UWS content</summary>
        /// <returns>string:UWS content</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string UWS()
        {
            string uws = "UWS\r\n";

            // Flight Header
            uws += string.Format("{0}.{1}\r\n", FlightHeader, this.Route[0]);

            // one line has 3 containers/pallets infomation
            // .<uld>/<destination>/<category>
            Cargo cgo;
            bool bulkBegin = true;

            string tmpstr = "";
            foreach (CargoPosnBase pos in theFlight.CargoPosnMgr.GenVisibleList(0))
            {
                tmpstr = "";
                if (pos.IsBulkPosn() || // bulk posn
                    theFlight.bTrimByCmpt)
                {
                    if (bulkBegin)
                    {
                        tmpstr += "BULK";
                        bulkBegin = false;
                        if (pos.CargoList.Count > 0)
                            tmpstr += "\r\n";
                    }


                    foreach (ICargo csgn in pos.CargoList)
                    {
                        if (csgn is Consignment)
                        {
                            tmpstr += string.Format("-{0}/{1}A/{2}/{3}", csgn.Dest, csgn.Weight,csgn.Category.Substring(0, 1), (csgn as Consignment).Pieces);
                        }
                    }
                    tmpstr += SHCFormattedString(pos);
                }
                else
                {
                    if (pos.CargoList.Count > 0)
                    {
                        cgo = pos.CargoList[0] as Cargo;
                        tmpstr += string.Format("-{0}", cgo.ULDSerialNo + theFlight.CarrierCode);

                        const string catgs = "BCEM";
                        int catInd = 0;
                        for (int i = 0; i < 4; i++, catInd = i)
                            if (cgo.Category[0] == catgs[i])
                                break;

                        foreach (string route in this.Route)
                        {
                            if (route == this.Route[0])
                                continue;
                            string rstr = ("/" + route);
                            for (int i = catInd; i < catInd + catgs.Length; i++)
                            {
                                string catg = Char.ToString(catgs[i % 4]);
                                double wt = pos.getGrossWt(catg, route);
                                if (wt > cgo.TareWt || (wt == cgo.TareWt && cgo.Category.Substring(0, 1) == catg && cgo.Dest == route))
                                    rstr += string.Format("/{0}A/{1}", wt, CargoCatg(pos, catg, route));
                            }

                            rstr += SHCFormattedString(pos, route);
                            if (rstr.Length > 4)
                                tmpstr += rstr;
                        }

                        if (!cgo.IsContainer)
                            tmpstr += string.Format("/{0}", cgo.HeightCode);
                    }
                }

                if (tmpstr != "")
                    tmpstr += "\r\n";

                uws += tmpstr;
            }

            return uws;
        }

        #endregion
    }
}