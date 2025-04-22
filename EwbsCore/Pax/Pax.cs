/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//* Thomas   | Ver. 13 | #BR18113  | 2018/06/20                (V4.04.01 )    *
//*---------------------------------------------------------------------------*
// PAX 頁面的 TRA 資料於 Connect to DCS後 更新                                *
//*****************************************************************************
//* Thomas  | Ver 12  | #BR18102  | 2018/02/23                                *
//*---------------------------------------------------------------------------*
//* 無論ROW 或是 ZONE ，皆計算上一段航班的SeatMapIndex (解決ZONE切換成 ROW 後 *
//，少了prevSeatMapIndex)                                                     *
//*****************************************************************************
//* Terri  | Ver 11  | #BR16109  | 2016/11/04      (V03.04.1)                 *
//*---------------------------------------------------------------------------*
//* B7國內線旅客重量修改為皆為 73/73/35    2017/1/1生效                       *
//*****************************************************************************
//* Jay Sheu  | Ver 10  | #BR15114  | 2015/10/08      (V03.02.02)             *
//*---------------------------------------------------------------------------*
//* 修改成只要是B7國內線旅客重量皆為 70 Kg                                    *
//*****************************************************************************
//* Cherry    | Ver. 09 | #BR14102  | 2014/MAY/15    (V2.09.01)               *
//*---------------------------------------------------------------------------*
//國際線旅客重量由MALE/FEMALE/CHILD/INFANT 由70/70/70/10KGS改為75/75/35/10KGS *
//國內線(含TPE):                                                              *
//AIRCRAFT TYPE M9C/M9D/M9E 旅客重量MALE/FEMAIL/CHILD/INFANT/70/70/70/10KG。  *
//AIRCRAFT TYPE M9F/M9G     旅客重量MALE/FEMAIL/CHILD/INFANT/75/75/35/10KG。  *
//搭配修改FDB																  *
//*****************************************************************************
//* Cherry    | Ver. 08 | #BR13103  | 2013/MAY/25    (V2.05)                  *
//*---------------------------------------------------------------------------*
//* 只要處理MD9ABCDE機型，旅客預設重量皆為(70/70/70/10)配合乾溼租             *
//* 只要處理MD9FGHI機型，旅客預設重量皆為(75/75/35/10)配合乾溼租              *
//*****************************************************************************
//* Cherry    | Ver. 07 | #BR12107  | 2012/MAY/24    (V2.02)                  *
//*---------------------------------------------------------------------------*
//* 只要BR處理MD9ABCDE機型，旅客預設重量皆為(70/70/70/10)配合乾溼租           *
//*****************************************************************************
//* Cherry    | Ver. 06 | #BR11105  | 2011/APR/21    (V1.18)                  *
//*---------------------------------------------------------------------------*
//* 只要BR處理MD機型，旅客預設重量皆為(70/70/70/10)                           *
//*****************************************************************************
//* Cherry    | Ver. 05 | #BR09120  | 2009/SEP/15    (V1.09)                  *
//*---------------------------------------------------------------------------*
//* To fix bug, in multi-leg, to create second leg, the pax index is error    *
//*****************************************************************************
//* Cherry    | Ver. 04 | #BR08112  | 2009/JAN/15    (V1.04)                  *
//*---------------------------------------------------------------------------*
//* Can update to PAX/CREW weight                                             *
//*****************************************************************************

using System;
using System.Collections;
using EwbsCore.Util;
using FlightDataBase;
//using nsBAGSupport;

namespace EWBS
{
    /// <summary>
    /// class for Passenger related data
    /// </summary>
    [Serializable]
    public class Pax
    {
        private GenderDstnClassList prevActlGenderDstnClassList; // previous
        private GenderDstnClassList actlGenderDstnClassList = new GenderDstnClassList(); //previous + current

        private ClsDstnBagList prevAcceptedBagList; // previous 
        private ClsDstnBagList actlAcceptedBagList = new ClsDstnBagList(); //previous + current
        private ClsDstnBagList adjustedBagList = new ClsDstnBagList(); //
        //private ClsDstnBagList preWbfBagList; //#BR18115 THOMAS TRA BAG變色用

        private ClsDstnClassList prevClsDstnClassList; // previous 
        private ClsDstnClassList actlClsDstnClassList = new ClsDstnClassList(); // current
        private ClsDstnClassList bookedClsDstnClassList = new ClsDstnClassList(); //
        private ClsDstnClassList avlbClassList = new ClsDstnClassList(); //Lists for Baggage usages

        private ZoneList zoneList = new ZoneList();
        private SeatList prevSeatMap; // pre-section
        private SeatList currSeatMap = new SeatList();
        private bool bTrimByZone;

        [NonSerialized]
        private Flight theFlight;

        private int blkdSeat;
        private ClsDstnClassList pad;

        //default sexual weight
        private double maleWeight = 75, femaleWeight = 75, childWeight = 35, infantWeight = 10;

        [NonSerialized]
        private double prevSeatMapIndex = 0;
        [NonSerialized]
        private double currSeatMapIndex = 0;
        [NonSerialized]
        public bool bHavePreFlight = false;  //BR08112

        #region constructor

        public Pax(Flight theFlight)
        {
            this.theFlight = theFlight;

            //seat map
            prevSeatMap = new SeatList();
            currSeatMap = new SeatList();

            zoneList = SeatList2ZoneList(currSeatMap);


            for (int i = 1; i < theFlight.Route.Length; i++)
            {
                avlbClassList.Add(new ClsDstnClassItem(theFlight.Route[i]));

                bookedClsDstnClassList.Add(new ClsDstnClassItem(theFlight.Route[i]));

                actlClsDstnClassList.Add(new ClsDstnClassItem(theFlight.Route[i]));

                actlGenderDstnClassList.Add(new GenderDstnClassItem(theFlight.Route[i]));

                actlAcceptedBagList.Add(new ClsDstnBagItem(theFlight.Route[i]));

                adjustedBagList.Add(new ClsDstnBagItem(theFlight.Route[i]));
            }

            //BR08112<--
            //Get Crew weight 
            //			AirlineEx airline = theFlight.Airline;
            //			maleWeight = airline.PersonWeight.PAX.M;
            //			femaleWeight = airline.PersonWeight.PAX.F;
            //			childWeight = airline.PersonWeight.PAX.C;
            //			infantWeight = airline.PersonWeight.PAX.I;
            //BR08112-->
            //BR08112<--
            //Get Pax weight
            try
            {

                maleWeight = this.theFlight.Pax.PaxMaleWeight;
                femaleWeight = this.theFlight.Pax.PaxFemaleWeight;
                childWeight = this.theFlight.Pax.PaxChildWeight;
                infantWeight = this.theFlight.Pax.PaxInfantWeight;
            }
            catch
            {
                AirlineEx airline = theFlight.Airline;
                maleWeight = airline.PersonWeight.PAX.M;
                femaleWeight = airline.PersonWeight.PAX.F;
                childWeight = airline.PersonWeight.PAX.C;
                infantWeight = airline.PersonWeight.PAX.I;

                //BR14102<--
                string sInternalStn = "TPE,TSA,RMQ,TNN,CYI,KHH,TTT,MZG,KNH,MFK,LZN,HCN,HUN";  //BR16115

                if (theFlight.FlightNumber.Substring(0, 2) == "B7")
                {
                    //BR15114<--
                    if ((sInternalStn.IndexOf(theFlight.Route[0]) >= 0) && (sInternalStn.IndexOf(theFlight.Route[1]) >= 0))
                    {
                        //BR16109<--
                        //maleWeight = 70; femaleWeight = 70; childWeight =70;

                        DateTime dt1 = Convert.ToDateTime("2017/01/01");

                        if ((System.DateTime.Today) >= dt1)
                        {
                            maleWeight = 73; femaleWeight = 73; childWeight = 35;
                        }
                        else
                        {
                            maleWeight = 70; femaleWeight = 70; childWeight = 70;
                        }
                        //BR16109-->
                    }
                    //BR15114-->
                    //BR15114<--
                    //					if (theFlight.ACType.name.Substring(2,3) =="M9C" ||theFlight.ACType.name.Substring(2,3) =="M9D" || 
                    //						theFlight.ACType.name.Substring(2,3) =="M9E" ||theFlight.ACType.name.Substring(2,3) =="DH8"	||
                    //						theFlight.ACType.name.Substring(2,3) =="ATR")  //MD/ATR/DASH  V2.09.02
                    //					{
                    //						if ((sInternalStn.IndexOf(theFlight.Route[0]) >=0 )&&( sInternalStn.IndexOf(theFlight.Route[1])>=0 ))
                    //						{
                    //							maleWeight = 70; femaleWeight = 70; childWeight =70;
                    //						}
                    //					}
                    //BR15114-->
                }
                //BR14102-->
                //BR14102<--
                //				//BR11105<--
                //				//BR12107if (theFlight.ACType.name.Substring(2,2) =="M9")  //MD機型
                //				if (theFlight.ACType.name.Substring(2,3) =="M9A" ||theFlight.ACType.name.Substring(2,3) =="M9B" || 
                //					theFlight.ACType.name.Substring(2,3) =="M9C" ||theFlight.ACType.name.Substring(2,3) =="M9D" ||
                //					theFlight.ACType.name.Substring(2,3) =="M9E")  //MD機型  //BR12107
                //				{
                //					maleWeight = 70; femaleWeight = 70; childWeight =70;
                //				}
                //				//BR11105-->
                //				//BR13103<--//配合BR/B7乾溼租
                //				else if (theFlight.ACType.name.Substring(2,3) =="M9F" ||theFlight.ACType.name.Substring(2,3) =="M9G" || 
                //					theFlight.ACType.name.Substring(2,3) =="M9H" ||theFlight.ACType.name.Substring(2,3) =="M9I")
                //				{
                //					maleWeight = 75; femaleWeight = 75; childWeight =35;
                //				}
                //				//BR13103-->
                //BR14102-->
            }
            //BR08112-->
        }

        #endregion

        #region Properties

        public bool BTrimByZone
        {
            //<!--#BR18113 Thomas  Trim By ZONE or Trim By ROM 判斷條件變更，不受PreSeatMap 及 PreActlGenderDstnClassList物件影響
            #region  remark
            /*
            get
            {
                if (prevActlGenderDstnClassList != null && (prevSeatMap == null || prevSeatMap.Length == 0))
                    bTrimByZone = true;
                return bTrimByZone;
            }
            set
            {
                if (prevActlGenderDstnClassList != null && (prevSeatMap == null || prevSeatMap.Length == 0))
                    bTrimByZone = true;
                else bTrimByZone = value;
            }
            */
            #endregion

            get{return bTrimByZone;}
            set{bTrimByZone = value;}
            //#BR18113-->
        }


        public SeatList SeatMap
        {
            get { return currSeatMap; }
        }

        public ClsDstnClassList BookedClsDstnClassList
        {
            get { return bookedClsDstnClassList; }
            set { bookedClsDstnClassList = value; }
        }

        public ClsDstnClassList ActlClsDstnClassList
        {
            get { return actlClsDstnClassList; }
            set { actlClsDstnClassList = value; }
        }

        public ClsDstnClassList AvlbClassList
        {
            get { return avlbClassList; }
            set { avlbClassList = value; }
        }

        public GenderDstnClassList GenderDstnClassList //ok
        {
            get { return actlGenderDstnClassList; }
        }

        /// <summary>
        /// Sum number of male, female, child, infant relatively between GenderDstnClassItem of ActlGenderDstnClassList
        /// in previous list and GenderDstnClassItem  in previous list, then insert into the relative GenderDstnClassItem 
        /// of actual list
        /// </summary>
        /// <param name="station"> destination </param>
        /// <param name="male">male number</param>
        /// <param name="female">femail number</param>
        /// <param name="child">child number</param>
        /// <param name="infant">infant number</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void setGenderDstnClassList(string station, string male, string female, string child, string infant)
        {
            if (station == "") return;

            GenderDstnClassItem prevItem = null;
            int maleno = 0, femaleno = 0, childno = 0, infantno = 0;
            int pMale = 0, pFemale = 0, pChild = 0, pInfant = 0;

            if (male != "") maleno = Convert.ToInt32(male);
            if (female != "") femaleno = Convert.ToInt32(female);
            if (child != "") childno = Convert.ToInt32(child);
            if (infant != "") infantno = Convert.ToInt32(infant);

            if (prevActlGenderDstnClassList != null)
             {
                if (prevActlGenderDstnClassList.Length > 0)
                {
                    // Get GenderDstnClassItem from prevActlGenderDstnClassList
                    prevItem = prevActlGenderDstnClassList.Find(station);
                }
            }

            GenderDstnClassItem actlItem = actlGenderDstnClassList.Find(station);

            // No this destination in prevAcctlGenderDstnClassList 
            if (prevItem == null)
            {
                // set number of passenger to zero
                pMale = 0;
                pFemale = 0;
                pChild = 0;
                pInfant = 0;
            }
            else
            {
                // get previous passenger number
                pMale = prevItem.Male;
                pFemale = prevItem.Female;
                pChild = prevItem.Child;
                pInfant = prevItem.Infant;
            }
            if (actlItem != null)
            {
                actlItem.Male = pMale + maleno;
                actlItem.Female = pFemale + femaleno;
                actlItem.Child = pChild + childno;
                actlItem.Infant = pInfant + infantno;
            }
        }

        /// <summary>
        /// Sum number of first, second, third class relatively between the passed in parameters and ClsDstnClassItem of prevClsDstnClassList
        /// in previous list, if the station is identical between ClsDstnClassItem of  prevClsDstnClassList in previous list
        /// and actlClsDstnClassList of actual list, then insert into the actlClsDstnClassList of actual list
        /// </summary>
        /// <param name="station"> destination </param>
        /// <param name="FirstClass">first class passenger number</param>
        /// <param name="SecondClass">second class passenger number</param>
        /// <param name="ThirdClass">third class passenger number</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void setClassDstnClassList(string station, string FirstClass, string SecondClass, string ThirdClass)
        {
            if (station == "") return;

            ClsDstnClassItem prevItem = null;
            int Firstno = 0, Secondno = 0, Thirdno = 0;
            int prevFirst = 0, prevSecond = 0, prevThird = 0;

            if (FirstClass != "") Firstno = Convert.ToInt32(FirstClass);
            if (SecondClass != "") Secondno = Convert.ToInt32(SecondClass);
            if (ThirdClass != "") Thirdno = Convert.ToInt32(ThirdClass);

            if (prevClsDstnClassList != null)
            {
                if (prevClsDstnClassList.Length > 0)
                {
                    prevItem = prevClsDstnClassList.Find(station);
                }
            }

            ClsDstnClassItem actlItem = actlClsDstnClassList.Find(station);

            if (prevItem == null)
            {
                prevFirst = 0;
                prevSecond = 0;
                prevThird = 0;
            }
            else
            {
                prevFirst = prevItem.FrstPaxNumber;
                prevSecond = prevItem.SryPaxNumber;
                prevThird = prevItem.LstPaxNumber;
            }
            if (actlItem != null)
            {
                actlItem.FrstPaxNumber = prevFirst + Firstno;
                actlItem.SryPaxNumber = prevSecond + Secondno;
                actlItem.LstPaxNumber = prevThird + Thirdno;

            }
        }


        public ZoneList Zonelist
        {
            get { return zoneList; }
        }

        public ClsDstnBagList AcceptedBagList //ok
        {
            get { return actlAcceptedBagList; }
        }

        public ClsDstnBagList AdjustedBagList
        {
            get { return adjustedBagList; }
        }

        public int BlkdSeat
        {
            get { return blkdSeat; }
            set { blkdSeat = value; }
        }

        public ClsDstnClassList Pad
        {
            get { return pad; }
            set { pad = value; }
        }

        #endregion

        #region Properties 2

        public ClsDstnClassList DCSBookedPaxList
        {
            set
            {
                foreach (ClsDstnClassItem dcsPax in value)
                {
                    ClsDstnClassItem result = this.bookedClsDstnClassList.Find(dcsPax.Dstn);
                    if (result != null)
                    {
                        result.FrstPaxNumber = dcsPax.FrstPaxNumber;
                        result.SryPaxNumber = dcsPax.SryPaxNumber;
                        result.LstPaxNumber = dcsPax.LstPaxNumber;
                    }
                    if (prevClsDstnClassList != null)
                    {
                        ClsDstnClassItem thePax = this.prevClsDstnClassList.Find(dcsPax.Dstn);
                        if (thePax != null)
                        {
                            result.FrstPaxNumber += thePax.FrstPaxNumber;
                            result.SryPaxNumber += thePax.SryPaxNumber;
                            result.LstPaxNumber += thePax.LstPaxNumber;
                        }
                    }

                } //for
            }
        }

        public ClsDstnClassList CurrentChkinPaxList
        {
            set
            {
                foreach (ClsDstnClassItem dcsPax in value)
                {
                    ClsDstnClassItem result = this.actlClsDstnClassList.Find(dcsPax.Dstn);
                    if (result != null)
                    {
                        result.FrstPaxNumber = dcsPax.FrstPaxNumber;
                        result.SryPaxNumber = dcsPax.SryPaxNumber;
                        result.LstPaxNumber = dcsPax.LstPaxNumber;
                    }
                    if (prevClsDstnClassList != null)
                    {
                        ClsDstnClassItem thePax = this.prevClsDstnClassList.Find(dcsPax.Dstn);
                        if (thePax != null)
                        {
                            result.FrstPaxNumber += thePax.FrstPaxNumber;
                            result.SryPaxNumber += thePax.SryPaxNumber;
                            result.LstPaxNumber += thePax.LstPaxNumber;
                        }
                    }

                } //for
            }
        }

        public GenderDstnClassList CurrentChkinGenderList //ok
        {
            set
            {
                foreach (GenderDstnClassItem dcsPax in value)
                {
                    GenderDstnClassItem result = this.actlGenderDstnClassList.Find(dcsPax.Dstn);
                    if (result != null)
                    {
                        result.Male = dcsPax.Male;
                        result.Female = dcsPax.Female;
                        result.Child = dcsPax.Child;
                        result.Infant = dcsPax.Infant;
                    }
                    if (prevActlGenderDstnClassList != null)
                    {
                        GenderDstnClassItem thePax = this.prevActlGenderDstnClassList.Find(dcsPax.Dstn);
                        if (thePax != null)
                        {
                            result.Male += thePax.Male;
                            result.Female += thePax.Female;
                            result.Child += thePax.Child;
                            result.Infant += thePax.Infant;
                        }
                    }

                } //for
            }
        }

        public SeatList CurrentSeatMap
        {
            set
            {
                AirTypeEx theAirType = theFlight.ACType;
                //SeatMap
                //Clean up all of the data and add them back one by one
                currSeatMap.Clear();
                currSeatMapIndex = 0;

                    foreach (SeatItem theSeat in value)
                    {
                        if (!theSeat.IsLegal())
                        {
                            continue;
                        }

                        GenderDstnClassItem paxInfo = theSeat.PaxInfo;
                        float index = theAirType.CalcSeatIndex(theSeat.SeatNo, Convert.ToSingle(
                            paxInfo.Male * maleWeight +
                                paxInfo.Female * femaleWeight +
                                paxInfo.Child * childWeight +
                                paxInfo.Infant * infantWeight));

                        if (!float.IsInfinity(index))
                        {
                            currSeatMap.Add(theSeat);
                            currSeatMapIndex += index;
                            //Console.WriteLine("{0}\t{1}",theSeat.SeatNo, index);
                        }
                    }
            }
        }

        public ZoneList CurrentZonelist
        {
            set
            {
                foreach (ZoneItem dcsPax in value)
                {
                    ZoneItem result = zoneList.Find(dcsPax.ZoneName);
                    if (result != null)
                    {
                        result.TtlPax = dcsPax.TtlPax;
                    }
                } //for
            }
        }

        public ClsDstnBagList CurrentChkinBagList //ok
        {
            set
            {
                foreach (ClsDstnBagItem dcsPax in value)
                {
                    ClsDstnBagItem result = this.actlAcceptedBagList.Find(dcsPax.Dstn);
                    if (result != null)
                    {
                        result.FrstPaxBag = dcsPax.FrstPaxBag;
                        result.SryPaxBag = dcsPax.SryPaxBag;
                        result.LstPaxBag = dcsPax.LstPaxBag;
                    }
                    if (prevAcceptedBagList != null)
                    {
                        ClsDstnBagItem thePax = this.prevAcceptedBagList.Find(dcsPax.Dstn);
                        if (thePax != null)
                        {
                            result.FrstPaxBag += thePax.FrstPaxBag;
                            result.SryPaxBag += thePax.SryPaxBag;
                            result.LstPaxBag += thePax.LstPaxBag;
                        }
                    }

                } //for
            }
        }

        #endregion

        #region Properties 3
        public ClsDstnBagList CurrentAcceptedBagList
        {
            get { return actlAcceptedBagList - prevAcceptedBagList; }
            // operator -, for definition, please see List for UI, Line 580
            set { actlAcceptedBagList = value + prevAcceptedBagList; }
            // operator *, for definition, please see List for UI, Line 602
        }

        public ClsDstnBagList PrevAcceptedBagList
        {
            get { return prevAcceptedBagList; }
            // operator -, for definition, please see List for UI, Line 580
            set { prevAcceptedBagList = value ;} //#BR18113 THOMAS TRA BagList 允許讀寫
            // operator *, for definition, please see List for UI, Line 602
        }

        //<!--#BR18115 THOMAS　TRA Baglist 文字變色使用
        /*
        public ClsDstnBagList PrevWbfBagList
        {
            get { return preWbfBagList; }
            set { preWbfBagList = value; } 
        }
        */
        //#BR18115 -->

        public ClsDstnClassList CurrentAcceptedPaxList
        {
            get { return actlClsDstnClassList - prevClsDstnClassList; }
            // operator -, for definition, please see List for UI, Line 580
            set { ActlClsDstnClassList = value + prevClsDstnClassList; }
            // operator +, for definition, please see List for UI, Line 602
        }

        public GenderDstnClassList CurrentGenderPaxList
        {
            get { return actlGenderDstnClassList - prevActlGenderDstnClassList; }
            // operator -, for definition, please see List for UI, Line 580
            set { actlGenderDstnClassList = value + prevActlGenderDstnClassList; }
            // operator *, for definition, please see List for UI, Line 602
        }

        public GenderDstnClassList PrevActlGenderDstnClassList
        {
            get { return prevActlGenderDstnClassList; }
            set { prevActlGenderDstnClassList = value; }//#BR18113 THOMAS　TRA PAX 允許讀寫
        }

        public ClsDstnClassList PrevClsDstnClassList
        {
            get { return prevClsDstnClassList; }

            //<!--#BR18113 THOMAS 允許讀寫
            set
            {
                foreach (ClsDstnClassItem dcsPax in value)
                {
                    if (prevClsDstnClassList != null)
                    {
                        ClsDstnClassItem result = this.prevClsDstnClassList.Find(dcsPax.Dstn);
                        if (result == null)
                        {
                            result = this.prevClsDstnClassList.Total; ;// ClsDstnClassList();
                        }
                        if (result != null)
                        {
                            result.Dstn = dcsPax.Dstn;
                            result.FrstPaxNumber = dcsPax.FrstPaxNumber;
                            result.SryPaxNumber = dcsPax.SryPaxNumber;
                            result.LstPaxNumber = dcsPax.LstPaxNumber;
                        }
                    }
                }
            }
            //#BR18113-->
        }

        public ClsDstnClassList CurrentBookedPaxList
        {
            get { return bookedClsDstnClassList - prevClsDstnClassList; }
            // operator -, for definition, please see List for UI, Line 580
            set { bookedClsDstnClassList = value + prevClsDstnClassList; }
            // operator +, for definition, please see List for UI, Line 602
        }

        public ClsDstnClassList CurrentClsDstnClassList
        {
            get { return actlClsDstnClassList - prevClsDstnClassList; }
            // operator -, for definition, please see List for UI, Line 580
            //set { actlClsDstnClassList = value +  prevClsDstnClassList ;}
            // operator +, for definition, please see List for UI, Line 602
        }

        #endregion

        #region Methods

        /// <summary>
        /// Convert the PAX SeatList into ZoneList
        /// </summary>
        /// <param name="seatList">SeatList object</param>
        /// <returns>ZoneList object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private ZoneList SeatList2ZoneList(SeatList seatList)
        {
            AirTypeEx theAirType = theFlight.ACType;
            ZoneList aList = new ZoneList();

            //從FDB取得本航班每個zone的名稱及每個zone的最大限制人數，做成ZoneItem存到aList
            //A 28;B 100;C 100...
            //There are no Zone in the freight
            if (theAirType.ZonePantryIndex != null && theAirType.ZonePantryIndex.zone != null)
            {
                // collect zone name in this air type
                for (int i = 0; i < theAirType.ZonePantryIndex.zone.zinfo.Length; i++)
                {
                    ZonePantryIndexZoneZinfo znInfo = theAirType.ZonePantryIndex.zone.zinfo[i];
                    aList.Add(new ZoneItem(znInfo.id, 0, znInfo.max));
                }
            }
            //seatList.seatno轉成row number，row number再轉成Zone number
            foreach (SeatItem seat in seatList)
            {
                // seat number convert to row number
                int row = theAirType.Seat2Row(seat.SeatNo);
                // row number convert to zone name
                string znName = theAirType.Row2ZoneNm(row);
                // find zone name in collected zone name list
                ZoneItem znItem = aList.Find(znName);
                // if found, total passenger number plus one
                if (znItem != null)
                {
                    znItem.TtlPax += 1;
                }
            }
            return aList;

        }

        /// <summary>
        /// Get the unCheck-In PAX List
        /// </summary>
        /// <returns>ClsDstnClassList object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnClassList NotYetCheckInPaxList()
        {
            ClsDstnClassList aList = new ClsDstnClassList();

            AirTypeEx theAirType = theFlight.ACType;
            ClsDstnClassItem maxSeatsByClass = theAirType.GetMaxSeatCabinConfiguration(theFlight.CabinConfiguration);

            ClsDstnClassItem sum = this.actlClsDstnClassList.Total;

            for (int k = 0; k < this.bookedClsDstnClassList.Length; k++)
            {
                ClsDstnClassItem notYetCheckInPax = this.bookedClsDstnClassList[k] - this.actlClsDstnClassList[k];

                bool all_checked_in = true;
                //It is not allowed to have the value less than zero.
                for (int j = 0; j < notYetCheckInPax.Length; j++)
                {
                    if (notYetCheckInPax[j] <= 0) notYetCheckInPax[j] = 0;
                    else all_checked_in = false;
                }

                ClsDstnClassItem result = new ClsDstnClassItem(this.bookedClsDstnClassList[k].Dstn);

                if (!all_checked_in)
                {
                    // compute the overbooking number for each class.
                    ClsDstnClassItem numberToAdjust = sum + notYetCheckInPax - maxSeatsByClass;

                    //Use the max. number of seat to assume, if this number is less than the booked PAX number.
                    if (numberToAdjust[0] + numberToAdjust[1] + numberToAdjust[2] >= 0)
                    {
                        result = maxSeatsByClass - sum;
                    }
                    else
                    {
                        //downgrade
                        for (int i = 0; i < numberToAdjust.Length - 1; i++)
                        {
                            if (numberToAdjust[i] >= 0)
                            {
                                numberToAdjust[i + 1] += numberToAdjust[i];
                                numberToAdjust[i] = 0;
                            }
                        }
                        //upgrade
                        for (int i = numberToAdjust.Length - 1; i > 0; i--)
                        {
                            if (numberToAdjust[i] >= 0)
                            {
                                numberToAdjust[i - 1] += numberToAdjust[i];
                                numberToAdjust[i] = 0;
                            }
                        }

                        //Add the result of adjusted numberToAdjust to ttlBookedClassNumber
                        result = maxSeatsByClass + numberToAdjust - sum;
                    }


                    //It is not allowed to have the value less than zero.
                    for (int j = 0; j < result.Length; j++)
                    {
                        if (result[j] < 0) result[j] = 0;
                    }
                    result.Dstn = this.bookedClsDstnClassList[k].Dstn;
                }
                aList.Add(result);
                sum += result;
            }
            return aList;
        }

        /// <summary>
        /// Get the total weight of PAX，and use the adult weight instead if the PAX is not check-in yet.
        /// </summary>
        /// <returns>weight</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getWeight()
        {
            double ttlWeight = 0;

            //current flight sector
            foreach (GenderDstnClassItem gender in this.actlGenderDstnClassList)
            {
                ttlWeight += gender.Male * maleWeight +
                    gender.Female * femaleWeight +
                    gender.Child * childWeight +
                    gender.Infant * infantWeight;
            }

            //freight
            if (ttlWeight == 0)
            {
                foreach (ZoneItem zn in zoneList)
                {
                    ttlWeight += zn.TtlPax * maleWeight;
                }
            }
            //use the adult weight instead if the PAX is not check-in yet
            ClsDstnClassItem notYetCheckInClassNumber = this.NotYetCheckInPaxList().Total;

            ttlWeight += maleWeight * (
                notYetCheckInClassNumber.FrstPaxNumber +
                    notYetCheckInClassNumber.SryPaxNumber +
                    notYetCheckInClassNumber.LstPaxNumber);


            return ttlWeight;
        }

        /// <summary>
        /// Get the index of PAX incluse the uncheck-in PAX.
        /// </summary>
        /// <returns>index value</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getIndex()
        {
            //Total Index
            double ttlIndex = 0;
            AirTypeEx theAirType = theFlight.ACType;

            //Use the amount of people in ZoneList and index to do the Trim-by-Zone calculation, if the user keys in data.
            if (this.BTrimByZone == true)
            {
                foreach (ZoneItem zn in zoneList)
                {
                    ttlIndex += theAirType.CalcZoneIndex(zn.ZoneName, zn.TtlPax * maleWeight);
                }
            }
            else
            {
                ttlIndex += currSeatMapIndex; //計算PAX Index currSeatMapIndex
            }

            //<!--#BR18113-1 currSeatMap Connect to DCS 後，已經有旅客Ckin 時，prevSeatMapIndex = 0 不計算)
            if (currSeatMap.Count > 0)
            {
                prevSeatMapIndex = 0;
            }

            if (this.prevSeatMap != null && this.prevSeatMap.Count > 0 && !this.BTrimByZone)
            {
                ttlIndex += prevSeatMapIndex;
            }
            //#BR18113-1 -->

            return ttlIndex + notYetCheckInPaxIndex(this.NotYetCheckInPaxList().Total);
        }

        /// <summary>
        /// Get the index of uncheck-in PAX.
        /// </summary>
        /// <param name="notYetCheckInClassNumber">ClsDstnClassItem object</param>
        /// <returns>assumed index</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private double notYetCheckInPaxIndex(ClsDstnClassItem notYetCheckInClassNumber)
        {
            AirTypeEx theAirType = theFlight.ACType;

            //confirm the CabinConfiguration
            CabinConfigurationSeatplan seatplan = theAirType.GetSeatplan(this.theFlight.CabinConfiguration);

            if (seatplan != null)
            {
                double predictedIndex = 0;

                ArrayList classNames = FlightUtil.GetClassNamesFromCabinConfiguarion(this.theFlight.CabinConfiguration);

                for (int i = 0; i < classNames.Count; i++)
                {
                    CabinConfigurationSeatplanInfo seatplanInfo = theAirType.GetCabinClassInfo(seatplan, classNames[i] as String);
                    int ttlSeat = seatplanInfo.seats;
                    //If there have seats in the Class
                    if (ttlSeat > 0)
                    {
                        //Confirm the zone number in each Class
                        string delimStr = " ,";
                        char[] delimiter = delimStr.ToCharArray();
                        string[] zoneNames = seatplanInfo.zones.Split(delimiter);

                        foreach (string znNm in zoneNames)
                        {
                            ZoneItem znItem = zoneList.Find(znNm);
                            if (znItem != null)
                            {
                                //the ration of people in each Zone for each class								
                                double ratio = (double)znItem.MaxPax / ttlSeat;

                                double index = theAirType.CalcZoneIndex(znNm,
                                                                        Convert.ToInt32(notYetCheckInClassNumber[i] * ratio) * maleWeight);

                                if (index == double.NegativeInfinity)
                                    throw (new Exception(string.Format(EWBSCoreWarnings.FDBZoneNotFound_1, znNm)));

                                //Distribute the people to each Zone for each class
                                predictedIndex += index;
                            }
                        } //for
                    }
                } //for
                return predictedIndex;

            } //if(seatplan)

            return 0;

        }

        /// <summary>
        /// get the Baggage number and weight from each destination
        /// </summary>
        /// <param name="dest">destination</param>
        /// <returns>BagType object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagType GetBagByDest(string dest)
        {
            BagType result = new BagType();
            foreach (ClsDstnBagItem bagItem in this.actlAcceptedBagList)
            {
                if (bagItem.Dstn.Equals(dest))
                {
                    result +=
                        bagItem.FrstPaxBag +
                            bagItem.SryPaxBag +
                            bagItem.LstPaxBag;
                }
            }
            if (this.prevAcceptedBagList != null)
            {
                foreach (ClsDstnBagItem bagItem in this.prevAcceptedBagList)
                {
                    if (bagItem.Dstn.Equals(dest))
                    {
                        result -=
                            bagItem.FrstPaxBag +
                                bagItem.SryPaxBag +
                                bagItem.LstPaxBag;
                    }
                }
            }
            foreach (ClsDstnBagItem bagItem in this.adjustedBagList)
            {
                if (bagItem.Dstn.Equals(dest))
                {
                    result +=
                        bagItem.FrstPaxBag +
                            bagItem.SryPaxBag +
                            bagItem.LstPaxBag;
                }
            }
            return result;
        }

        #endregion

        #region Unload Methods

        /// <summary>
        /// Remove the PAX with 'dest' destination from the actlGenderDstnClassList
        /// </summary>
        /// <param name="dest">destination</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void unloadGenderDstnClassList(string dest)
        {
            for (int i = this.actlGenderDstnClassList.Length - 1; i >= 0; i--)
            {
                if (actlGenderDstnClassList[i].Dstn.Equals(dest))
                    actlGenderDstnClassList.RemoveAt(i);
            }
            if (actlGenderDstnClassList.Length > 0)
            {
                if (this.prevActlGenderDstnClassList == null)
                    prevActlGenderDstnClassList = new GenderDstnClassList();
                else prevActlGenderDstnClassList.Clear();

                for (int i = 0; i < this.actlGenderDstnClassList.Length; i++)
                {
                    prevActlGenderDstnClassList.Add(new GenderDstnClassItem(actlGenderDstnClassList[i]));
                }
            }
        }


        /// <summary>
        /// Remove the baggage with 'dest' destination from the actlAcceptedBagList.
        /// </summary>
        /// <param name="dest">destination</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void unloadAcceptedBagList(string dest)
        {
            for (int i = actlAcceptedBagList.Length - 1; i >= 0; i--)
            {
                actlAcceptedBagList[i] += adjustedBagList[i];

                if (actlAcceptedBagList[i].Dstn.Equals(dest))
                    actlAcceptedBagList.RemoveAt(i);
            }
            if (actlAcceptedBagList.Length > 0)
            {
                if (this.prevAcceptedBagList == null) prevAcceptedBagList = new ClsDstnBagList();
                else prevAcceptedBagList.Clear();

                for (int i = 0; i < actlAcceptedBagList.Length; i++)
                {
                    prevAcceptedBagList.Add(new ClsDstnBagItem(actlAcceptedBagList[i]));
                }
            }

            adjustedBagList.Clear();
        }

        /// <summary>
        /// Remove the PAX with 'dest' destination from the actlClsDstnClassList.
        /// </summary>
        /// <param name="dest">destination</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void unloadClsDstnClassList(string dest)
        {
            for (int i = actlClsDstnClassList.Length - 1; i >= 0; i--)
            {
                if (actlClsDstnClassList[i].Dstn.Equals(dest))
                    actlClsDstnClassList.RemoveAt(i);
                if (bookedClsDstnClassList[i].Dstn.Equals(dest))
                    bookedClsDstnClassList.RemoveAt(i);
                if (avlbClassList[i].Dstn.Equals(dest))
                    avlbClassList.RemoveAt(i);
            }
            if (actlClsDstnClassList.Length > 0)
            {
                if (this.prevClsDstnClassList == null) prevClsDstnClassList = new ClsDstnClassList();
                else prevClsDstnClassList.Clear();

                for (int i = 0; i < actlClsDstnClassList.Length; i++)
                {
                    prevClsDstnClassList.Add(new ClsDstnClassItem(actlClsDstnClassList[i]));
                    //bookedClsDstnClassList.Add(new ClsDstnClassItem(actlClsDstnClassList[i]));
                }
            }

        }

        /// <summary>
        /// Remove the PAX with 'dest' destination from the eatMap.
        /// </summary>
        /// <param name="dest">destination</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void unloadSeatMap(string dest)
        {
      
            if (this.bTrimByZone)
            {
                prevSeatMap = new SeatList();
            }
            else
            {
                if (this.prevSeatMap == null) prevSeatMap = new SeatList();

                for (int i = prevSeatMap.Length - 1; i >= 0; i--)
                {
                    if (prevSeatMap[i].Dstn.Equals(dest))
                        prevSeatMap.RemoveAt(i);
                }
                foreach (SeatItem item in currSeatMap)
                {
                    if (!item.Dstn.Equals(dest))
                        prevSeatMap.Add(item);
                }

                AirTypeEx theAirType = theFlight.ACType;

                //calculate index
                prevSeatMapIndex = 0;
                foreach (SeatItem theSeat in this.prevSeatMap)
                {
                    GenderDstnClassItem paxInfo = theSeat.PaxInfo;
                    prevSeatMapIndex += theAirType.CalcSeatIndex(theSeat.SeatNo, Convert.ToSingle(
                        paxInfo.Male * maleWeight +
                            paxInfo.Female * femaleWeight +
                            paxInfo.Child * childWeight +
                            paxInfo.Infant * infantWeight));
                }

            }
            currSeatMap.Clear();
            this.currSeatMapIndex = 0;   //BR09120 clear currSeatMapIndex


        }

        /// <summary>
        /// Call unloadGenderDstnClassList(dest)、unloadAcceptedBagList(dest)、
        /// unloadClsDstnClassList(dest)、and unloadSeatMap(dest) to proceed unload process.
        /// </summary>
        /// <param name="dest">destination</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void UnloadProcedure(string dest)
        {
            if (actlGenderDstnClassList.Find(dest) != null ||
                actlClsDstnClassList.Find(dest) != null)
            {
                unloadGenderDstnClassList(dest);
                unloadAcceptedBagList(dest);
                unloadClsDstnClassList(dest);
                unloadSeatMap(dest);

                if (this.prevSeatMap != null) zoneList = SeatList2ZoneList(this.prevSeatMap);
            }
            for (int i = 1; i < theFlight.Route.Length; i++)
            {
                string station = theFlight.Route[i];
                if (avlbClassList.Find(station) == null)
                    avlbClassList.Add(new ClsDstnClassItem(station));

                if (bookedClsDstnClassList.Find(station) == null)
                    bookedClsDstnClassList.Add(new ClsDstnClassItem(station));

                if (actlClsDstnClassList.Find(station) == null)
                    actlClsDstnClassList.Add(new ClsDstnClassItem(station));

                if (actlGenderDstnClassList.Find(station) == null)
                    actlGenderDstnClassList.Add(new GenderDstnClassItem(station));

                if (actlAcceptedBagList.Find(station) == null)
                    actlAcceptedBagList.Add(new ClsDstnBagItem(station));

                if (adjustedBagList.Find(station) == null)
                    adjustedBagList.Add(new ClsDstnBagItem(station));
            }
        }

        #endregion

        #region DeSerialize Methods

        /// <summary>
        /// After the Flight Deserialized, adjust the Pax related data for those unSerialized data.
        /// </summary>
        /// <param name="sender"></param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void OnDeserialization(Object sender)
        {
            this.theFlight = (Flight)sender;
            //BR08112<--
            //			//get pax weight
            //			AirlineEx airline = theFlight.Airline;
            //			maleWeight = airline.PersonWeight.PAX.M;
            //			femaleWeight = airline.PersonWeight.PAX.F;
            //			childWeight = airline.PersonWeight.PAX.C;
            //			infantWeight = airline.PersonWeight.PAX.I;
            //BR08112-->
            //BR08112<--
            //Get Pax weight
            try
            {
                maleWeight = this.theFlight.Pax.PaxMaleWeight;
                femaleWeight = this.theFlight.Pax.PaxFemaleWeight;
                childWeight = this.theFlight.Pax.PaxChildWeight;
                infantWeight = this.theFlight.Pax.PaxInfantWeight;
            }
            catch
            {
                AirlineEx airline = theFlight.Airline;
                maleWeight = airline.PersonWeight.PAX.M;
                femaleWeight = airline.PersonWeight.PAX.F;
                childWeight = airline.PersonWeight.PAX.C;
                infantWeight = airline.PersonWeight.PAX.I;

                //BR14102<--
                string sInternalStn = "TPE,TSA,RMQ,TNN,CYI,KHH,TTT,MZG,KNH,MFK,LZN,HCN,HUN";  //BR16115

                if (theFlight.FlightNumber.Substring(0, 2) == "B7")
                {
                    //BR15114<--
                    if ((sInternalStn.IndexOf(theFlight.Route[0]) >= 0) && (sInternalStn.IndexOf(theFlight.Route[1]) >= 0))
                    {

                        //BR16109<--
                        //maleWeight = 70; femaleWeight = 70; childWeight =70;
                        DateTime dt1 = Convert.ToDateTime("2017/01/01");

                        if ((System.DateTime.Today) >= dt1)
                        {
                            maleWeight = 73; femaleWeight = 73; childWeight = 35;
                        }
                        else
                        {
                            maleWeight = 70; femaleWeight = 70; childWeight = 70;
                        }

                        //BR16109-->
                    }
                    //BR15114-->

                    //BR15114<--
                    //					if (theFlight.ACType.name.Substring(2,3) =="M9C" ||theFlight.ACType.name.Substring(2,3) =="M9D" || 
                    //						theFlight.ACType.name.Substring(2,3) =="M9E" ||theFlight.ACType.name.Substring(2,3) =="DH8"	||
                    //						theFlight.ACType.name.Substring(2,3) =="ATR")  //MD/ATR/DASH  V2.09.02
                    //					{
                    //						if ((sInternalStn.IndexOf(theFlight.Route[0]) >=0 )&&( sInternalStn.IndexOf(theFlight.Route[1])>=0 ))
                    //						{
                    //							maleWeight = 70; femaleWeight = 70; childWeight =70;
                    //						}
                    //					}
                    //BR15114-->
                }
                //BR14102-->
                //BR14102<--
                //				//BR11105<--
                //				//BR12107if (theFlight.ACType.name.Substring(2,2) =="M9")  //MD機型
                //				if (theFlight.ACType.name.Substring(2,3) =="M9A" ||theFlight.ACType.name.Substring(2,3) =="M9B" || 
                //					theFlight.ACType.name.Substring(2,3) =="M9C" ||theFlight.ACType.name.Substring(2,3) =="M9D" ||
                //					theFlight.ACType.name.Substring(2,3) =="M9E")  //MD機型  //BR12107
                //
                //				{
                //					maleWeight = 70; femaleWeight = 70; childWeight =70;
                //				}
                //				//BR11105-->
                //				//BR13103<--//配合BR/B7乾溼租
                //				else if (theFlight.ACType.name.Substring(2,3) =="M9F" ||theFlight.ACType.name.Substring(2,3) =="M9G" || 
                //					theFlight.ACType.name.Substring(2,3) =="M9H" ||theFlight.ACType.name.Substring(2,3) =="M9I")
                //				{
                //					maleWeight = 75; femaleWeight = 75; childWeight =35;
                //				}
                //				//BR13103-->
                //BR14102
            }
            //BR08112-->

            //if (this.bTrimByZone) //#BR18102 Thomas remark 
            if (true) //#BR18102 Thomas  無論ROW 或是 ZONE ，皆計算上一段航班的SeatMapIndex (解決ZONE切換成 ROW MODE後，少了prevSeatMapIndex)
            {
                AirTypeEx theAirType = this.theFlight.ACType;

                prevSeatMapIndex = 0;
                currSeatMapIndex = 0;

                #region calculate prevSeatMap index
                if (this.prevSeatMap != null)
                {
                    //foreach (SeatItem theSeat in this.prevSeatMap)
                    for (int i = this.prevSeatMap.Length - 1; i >= 0; i--)
                    {
                        SeatItem theSeat = this.prevSeatMap[i];
                        GenderDstnClassItem paxInfo = theSeat.PaxInfo;
                        float index = theAirType.CalcSeatIndex(theSeat.SeatNo, Convert.ToSingle(
                            paxInfo.Male * maleWeight +
                                paxInfo.Female * femaleWeight +
                                paxInfo.Child * childWeight +
                                paxInfo.Infant * infantWeight));

                        if (float.IsNegativeInfinity(index)) this.prevSeatMap.RemoveAt(i);
                        else prevSeatMapIndex += index;
                        //Console.WriteLine(string.Format("*Seat {0} index={1}",theSeat.SeatNo,index));
                    }
                }
                #endregion

                //prevSeatMapIndex = 0; //#BR18113 THOMAS 將不計算上一段帶進來的prevSeatMapIndex 所以將他歸零

                #region calculate currSeatMap index
                if (this.currSeatMap != null)
                {
                    for (int i = this.currSeatMap.Length - 1; i >= 0; i--)
                    {
                        SeatItem theSeat = this.currSeatMap[i];
                        GenderDstnClassItem paxInfo = theSeat.PaxInfo;
                        float index = theAirType.CalcSeatIndex(theSeat.SeatNo, Convert.ToSingle(
                            paxInfo.Male * maleWeight +
                                paxInfo.Female * femaleWeight +
                                paxInfo.Child * childWeight +
                                paxInfo.Infant * infantWeight));
                        if (float.IsNegativeInfinity(index)) this.currSeatMap.RemoveAt(i);
                        else currSeatMapIndex += index;
                        //Console.WriteLine(string.Format("Seat {0} index={1}",theSeat.SeatNo,index));
                    }
                }
                #endregion
            } 
        }

        #endregion

        /// <summary>
        /// Find out if the number of PAX is reasonable or not?
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Check()
        {
            int clsTtl = 0;
            int ZoneTtl = 0;
            int GenderTtl = 0;
            int extraCrew = 0;

            string cfg = theFlight.CabinConfiguration;
            AirTypeEx theAirType = theFlight.ACType;

            ClsDstnClassItem actlClsPax = actlClsDstnClassList.Total;
            string errMsg = "";


            clsTtl = actlClsPax[0] + actlClsPax[1] + actlClsPax[2];

            //Calculate Total Pax from Gender
            for (int i = 0; i < actlGenderDstnClassList.Length; i++)
            {
                GenderTtl +=
                    actlGenderDstnClassList[i].Male
                        + actlGenderDstnClassList[i].Female
                        + actlGenderDstnClassList[i].Child;
            }
            //Calculate Total Pax from Zone
            ClsDstnClassItem actlZonePax = new ClsDstnClassItem("TTL");
            foreach (ZoneItem ZI in this.Zonelist)
            {
                ZoneTtl += ZI.TtlPax;
                actlZonePax[theAirType.Zone2Class(cfg, ZI.ZoneName)] += ZI.TtlPax;
            }

            //the total of MFC and each class shall be the same
            if (clsTtl != GenderTtl)
            {
                errMsg += "\r\n" + EWBSCoreWarnings.PaxClsTtlShouldEqualGdrTtl;
            }


            //the total of MFC and each zone shall be the same
            if (GenderTtl != ZoneTtl) //&& (GenderTtl > 0 || clsTtl > 0))
            {
                errMsg += "\r\n" + EWBSCoreWarnings.PaxZoneTtlShouldEqualGdrTtl;
            }


            if (clsTtl != GenderTtl || GenderTtl != ZoneTtl)
            {
                //the sum of the number of people of each destination and SOC shall be less than MAX in each class. 
                int[] maxSeatsByClass = FlightUtil.GetPaxArrayFromCabinConfiguarion(theFlight.CabinConfiguration);

                //ClsDstnClassItem maxSeatsByClass = theAirType.GetMaxSeatCabinConfiguration(cfg);

                // sum the number of people in each class include SOC
                ClsDstnClassItem ttlPaxIncludeSOC = new ClsDstnClassItem("");
                foreach (ClsDstnClassItem clsDstnClassItem in actlClsDstnClassList)
                {
                    ttlPaxIncludeSOC += clsDstnClassItem;
                }
                ttlPaxIncludeSOC += theFlight.ServiceItems.SOC;

                // compute the number of people in each class include Extra CREW
                for (int cls = 0; cls < theFlight.Crew.ExtraCrews.Length; cls++)
                {
                    extraCrew +=
                        theFlight.Crew.ExtraCrews[cls].Cockpit +
                            theFlight.Crew.ExtraCrews[cls].Cabin;
                }

                for (int cls = 0; cls < ttlPaxIncludeSOC.Length; cls++)
                {
                    if (ttlPaxIncludeSOC[cls] > maxSeatsByClass[cls])
                    {
                        errMsg += "\r\n" + EWBSCoreWarnings.PaxTtlIncludingSOCExceedsMax;
                        break;
                    }
                }

                if (ttlPaxIncludeSOC.FrstPaxNumber + ttlPaxIncludeSOC.SryPaxNumber + ttlPaxIncludeSOC.LstPaxNumber + Math.Max(blkdSeat, extraCrew) >
                    maxSeatsByClass[0] + maxSeatsByClass[1] + maxSeatsByClass[2])
                {
                    errMsg += "\r\n" + EWBSCoreWarnings.PaxTtlIncludingSOCBlkExceedsMax;
                }


                //the number of PAX in each Zone shall not larger than the MAX of PAX
                foreach (ZoneItem ZI in this.Zonelist)
                {
                    if (ZI.TtlPax > ZI.MaxPax)
                    {
                        errMsg += "\r\n" + string.Format(EWBSCoreWarnings.PaxZoneExceedsMax_1, ZI.ZoneName);
                        break;
                    }
                }
            }


            //the number of PAX in each Zone shall be same as CLS
            for (int cls = 0; cls < 3; cls++)
            {
                if (actlClsPax[cls] != actlZonePax[cls])
                {
                    errMsg += "\r\n" + string.Format(EWBSCoreWarnings.PaxZoneNotMatchCls);
                    break;
                }
            }
            actlZonePax = null;  //#BR17164 THOMAS
            //System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            if (errMsg != "") throw (new Exception(errMsg));
        }

        /// <summary>
        /// make a copy of Pax object from the input Pax object
        /// </summary>
        /// <param name="other">Pax object</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void CopyFrom(Pax other)
        {
            this.theFlight = other.theFlight;

            actlAcceptedBagList = new ClsDstnBagList();
            foreach (ClsDstnBagItem bagItem in other.AcceptedBagList)
                this.actlAcceptedBagList.Add(new ClsDstnBagItem(bagItem));

            actlClsDstnClassList = new ClsDstnClassList();
            foreach (ClsDstnClassItem clsItem in other.ActlClsDstnClassList)
                this.actlClsDstnClassList.Add(new ClsDstnClassItem(clsItem));

            actlGenderDstnClassList = new GenderDstnClassList();
            foreach (GenderDstnClassItem genderItem in other.actlGenderDstnClassList)
                actlGenderDstnClassList.Add(new GenderDstnClassItem(genderItem));

            adjustedBagList = new ClsDstnBagList();
            foreach (ClsDstnBagItem bagItem in other.AdjustedBagList)
                adjustedBagList.Add(new ClsDstnBagItem(bagItem));

            avlbClassList = new ClsDstnClassList();
            foreach (ClsDstnClassItem clsItem in other.AvlbClassList)
                avlbClassList.Add(new ClsDstnClassItem(clsItem));

            this.BlkdSeat = other.BlkdSeat;

            bookedClsDstnClassList = new ClsDstnClassList();
            foreach (ClsDstnClassItem clsItem in other.BookedClsDstnClassList)
                bookedClsDstnClassList.Add(new ClsDstnClassItem(clsItem));

            this.bTrimByZone = other.BTrimByZone;

            currSeatMap = new SeatList();
            foreach (SeatItem seatItem in other.SeatMap)
                currSeatMap.Add(new SeatItem(seatItem.SeatNo, seatItem.Dstn, seatItem.Code, seatItem.BagWeight, seatItem.BagPieces, seatItem.Gender));

            if (other.pad != null)
            {
                pad = new ClsDstnClassList();
                foreach (ClsDstnClassItem clsItem in other.pad)
                    pad.Add(new ClsDstnClassItem(clsItem));
            }
            else pad = null;

            if (other.prevAcceptedBagList != null)
            {
                prevAcceptedBagList = new ClsDstnBagList();
                foreach (ClsDstnBagItem bagItem in other.prevAcceptedBagList)
                    prevAcceptedBagList.Add(new ClsDstnBagItem(bagItem));
            }
            else prevAcceptedBagList = null;

            if (other.prevActlGenderDstnClassList != null)
            {
                this.theFlight.Pax.bHavePreFlight = true;  //#BR08112
                prevActlGenderDstnClassList = new GenderDstnClassList();
                foreach (GenderDstnClassItem genderItem in other.prevActlGenderDstnClassList)
                    prevActlGenderDstnClassList.Add(new GenderDstnClassItem(genderItem));
            }
            else prevActlGenderDstnClassList = null;

            if (other.prevSeatMap != null)
            {
                prevSeatMap = new SeatList();
                foreach (SeatItem seatItem in other.prevSeatMap)
                    prevSeatMap.Add(new SeatItem(seatItem.SeatNo, seatItem.Dstn, seatItem.Code, seatItem.BagWeight, seatItem.BagPieces, seatItem.Gender));
            }
            else prevSeatMap = null;


            if (other.prevClsDstnClassList != null)
            {
                prevClsDstnClassList = new ClsDstnClassList();
                foreach (ClsDstnClassItem clsItem in other.prevClsDstnClassList)
                    prevClsDstnClassList.Add(new ClsDstnClassItem(clsItem));
            }
            else prevClsDstnClassList = null;

            zoneList = new ZoneList();
            foreach (ZoneItem zone in other.Zonelist)
                zoneList.Add(new ZoneItem(zone.ZoneName, zone.TtlPax, zone.MaxPax));
            //BR08112<--
            //			maleWeight = other.maleWeight;
            //			femaleWeight = other.femaleWeight;
            //			childWeight = other.childWeight;
            //			infantWeight = other.infantWeight;
            //BR08112-->

            //BR08112<--
            //Get Pax weight
            try
            {
                maleWeight = other.maleWeight;
                femaleWeight = other.femaleWeight;
                childWeight = other.childWeight;
                infantWeight = other.infantWeight;
            }
            catch
            {
                AirlineEx airline = theFlight.Airline;
                maleWeight = airline.PersonWeight.PAX.M;
                femaleWeight = airline.PersonWeight.PAX.F;
                childWeight = airline.PersonWeight.PAX.C;
                infantWeight = airline.PersonWeight.PAX.I;
                //BR14102<--
                string sInternalStn = "TPE,TSA,RMQ,TNN,CYI,KHH,TTT,MZG,KNH,MFK,LZN,HCN,HUN";   //BR16115

                if (theFlight.FlightNumber.Substring(0, 2) == "B7")
                {

                    //#BR15114<-- -JayS, 移除機型，改為只要是B7飛國內就是70kg
                    //if (theFlight.ACType.name.Substring(2,3) =="M9C" ||theFlight.ACType.name.Substring(2,3) =="M9D" || 
                    //  theFlight.ACType.name.Substring(2,3) =="M9E" ||theFlight.ACType.name.Substring(2,3) =="DH8"	||
                    //  theFlight.ACType.name.Substring(2,3) =="ATR")  //MD/ATR/DASH  V2.09.02
                    //#BR15114-->

                    {
                        if ((sInternalStn.IndexOf(theFlight.Route[0]) >= 0) && (sInternalStn.IndexOf(theFlight.Route[1]) >= 0))
                        {
                            //BR16109<--
                            //maleWeight = 70; femaleWeight = 70; childWeight =70;
                            DateTime dt1 = Convert.ToDateTime("2017/01/01");

                            if ((System.DateTime.Today) >= dt1)
                            {
                                maleWeight = 73; femaleWeight = 73; childWeight = 35;
                            }
                            else
                            {
                                maleWeight = 70; femaleWeight = 70; childWeight = 70;
                            }
                            //BR16109-->
                        }
                    }
                }
                //BR14102-->

                //BR14102<--
                //				//BR11105<--
                //				//BR12107if (theFlight.ACType.name.Substring(2,2) =="M9")  //MD機型
                //				if (theFlight.ACType.name.Substring(2,3) =="M9A" ||theFlight.ACType.name.Substring(2,3) =="M9B" || 
                //					theFlight.ACType.name.Substring(2,3) =="M9C" ||theFlight.ACType.name.Substring(2,3) =="M9D" ||
                //					theFlight.ACType.name.Substring(2,3) =="M9E")  //MD機型  //BR12107
                //				{
                //					maleWeight = 70; femaleWeight = 70; childWeight =70;
                //				}
                //				//BR11105-->
                //				//BR13103<--//配合BR/B7乾溼租
                //				else if (theFlight.ACType.name.Substring(2,3) =="M9F" ||theFlight.ACType.name.Substring(2,3) =="M9G" || 
                //					theFlight.ACType.name.Substring(2,3) =="M9H" ||theFlight.ACType.name.Substring(2,3) =="M9I")
                //				{
                //					maleWeight = 75; femaleWeight = 75; childWeight =35;
                //				}
                //				//BR13103-->
                //BR14102-->
            }
            //BR08112-->

        }

        //BR08112<--
        public double PaxMaleWeight
        {
            get
            {
                return maleWeight;
            }
            set
            {
                AirlineEx airline = theFlight.Airline;
                //double tmpFDBPaxWt = Convert.ToDouble(airline.PersonWeight.PAX.M);
                double tmpPaxWeight = maleWeight;
                double userInputWt = value;

                //if (userInputWt < (tmpFDBPaxWt * 0.9) || userInputWt > (tmpFDBPaxWt * 1.1))
                if (userInputWt < 10 || userInputWt > 99)
                {
                    maleWeight = tmpPaxWeight;
                }
                else
                {
                    maleWeight = value;
                }
            }
        }
        public double PaxFemaleWeight
        {
            get
            {
                return femaleWeight;
            }
            set
            {
                AirlineEx airline = theFlight.Airline;
                //double tmpFDBPaxWt = Convert.ToDouble(airline.PersonWeight.PAX.F);
                double tmpPaxWeight = femaleWeight;
                double userInputWt = value;

                //if (userInputWt < (tmpFDBPaxWt * 0.9) || userInputWt > (tmpFDBPaxWt * 1.1))
                if (userInputWt < 10 || userInputWt > 99)
                {
                    femaleWeight = tmpPaxWeight;
                }
                else
                {
                    femaleWeight = value;
                }
            }
        }
        public double PaxChildWeight
        {
            get
            {
                return childWeight;
            }
            set
            {
                AirlineEx airline = theFlight.Airline;
                //double tmpFDBPaxWt = Convert.ToDouble(airline.PersonWeight.PAX.C);
                double tmpPaxWeight = childWeight;
                double userInputWt = value;

                //if (userInputWt < (tmpFDBPaxWt * 0.9) || userInputWt > (tmpFDBPaxWt * 1.1))
                if (userInputWt < 10 || userInputWt > 99)
                {
                    childWeight = tmpPaxWeight;
                }
                else
                {
                    childWeight = value;
                }
            }
        }
        public double PaxInfantWeight
        {
            get
            {
                return infantWeight;
            }
            set
            {
                AirlineEx airline = theFlight.Airline;
                double tmpFDBPaxWt = Convert.ToDouble(airline.PersonWeight.PAX.I);
                double tmpPaxWeight = infantWeight;
                double userInputWt = value;

                //				if (userInputWt < (tmpFDBPaxWt * 0.9) || userInputWt > (tmpFDBPaxWt * 1.1))
                if (userInputWt > 20 || userInputWt < 1)
                {
                    infantWeight = tmpPaxWeight;
                }
                else
                {
                    infantWeight = value;
                }
            }
        }
        //BR08112-->
    }
}