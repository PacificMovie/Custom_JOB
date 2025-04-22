/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//* Cherry    | Ver. 04 | #BR08112  | 2009/JAN/15    (V1.04)                  *
//*---------------------------------------------------------------------------*
//* Can update to PAX/CREW weight                                             *
//*****************************************************************************
using System;
using System.Collections;
using EwbsCore.Util;
using FlightDataBase;
using System.Runtime.Serialization;  //BR08112
using System.Runtime.Serialization.Formatters.Binary;  //BR08112
using System.IO; //BR08112

namespace EWBS
{
    /// <summary>
    /// Summary description for the List of Crew.
    /// </summary>
    /// 
    [Serializable]
    public class CrewList : ISerializable
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [NonSerialized]
        private Flight theFlight; //Flight
        //#BR08112<--
        //		[NonSerialized] private float pilotWeight = 87; //default weight 
        //		[NonSerialized] private float crewWeight = 64; //default weight 
        //#BR08112-->

        //BR08112<--
        private float pilotWeight = 87; //default weight 
        private float crewWeight = 64; //default weight 
        //BR08112-->

        private int numCrew = 0, numCockpit = 0; //number of crew, number of cockpit crew.
        private CrewItem[] crewList; //List of Crew
        private CrewItem[] extraCrewList; //List of Extra Crew
        private ZoneList zoneList = new ZoneList(); //#BR17206 THOMAS  Extra Crew Zone List

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight">Flight</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CrewList(Flight theFlight)
        {
            this.theFlight = theFlight;

            // get AirType
            AirTypeEx airType = theFlight.ACType;

            // gain the crew information from the AirType
            if (airType.CrewInfomation != null)
            {
                CrewInfomationInfo[] aList = airType.CrewInfomation.info;
                crewList = new CrewItem[aList.Length];
                for (int i = 0; i < aList.Length; i++)
                {
                    crewList[i] = new CrewItem(aList[i].crewPos, aList[i].maxSeat, aList[i].index);
                }
            }

            //<!-- #BR17206 THOMAS EXTRA CREW List

            var zoneLen = theFlight.ACType.ZonePantryIndex.zone.zinfo.Length;
            extraCrewList = new CrewItem[zoneLen];
            //Assign the numbers to the Lists
            for (int i = 0; i < zoneLen; i++)
            {
                extraCrewList[i] = new CrewItem(theFlight.ACType.ZonePantryIndex.zone.zinfo[i].id, theFlight.ACType.ZonePantryIndex.zone.zinfo[i].max, theFlight.ACType.ZonePantryIndex.zone.zinfo[i].index);
            }

            #region 原本的
            /*
            extraCrewList = new CrewItem[3];
            //Assign the numbers to the Lists
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                    extraCrewList[i] = new CrewItem("First", 0, 0);
                else if (i == 1)
                    extraCrewList[i] = new CrewItem("Secondary", 0, 0);
                else if (i == 2)
                    extraCrewList[i] = new CrewItem("Last", 0, 0);
            }
             */
            #endregion

            //#BR17206 -->

            //#BR08112<--
            // get Crew weight 
            //			AirlineEx airline = theFlight.Airline; //Modified airlinesAirline to AirlineEx 
            //			pilotWeight = airline.PersonWeight.Crew.P;
            //			crewWeight = airline.PersonWeight.Crew.C;
            //#BR08112-->
            //#BR08112<--
            try
            {
                pilotWeight = theFlight.Crew.CrewWtCockpit;
                crewWeight = theFlight.Crew.CrewWtCabin;
            }
            catch
            {
                // get Crew weight 
                AirlineEx airline = theFlight.Airline;
                pilotWeight = airline.PersonWeight.Crew.P;
                crewWeight = airline.PersonWeight.Crew.C;
            }
            //#BR08112-->
        }
        #endregion

        //BR08112<--
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("numCrew", this.numCrew);
            info.AddValue("numCockpit", this.numCockpit);
            info.AddValue("crewList", this.crewList);
            info.AddValue("extraCrewList", this.extraCrewList);
            info.AddValue("pilotWeight", this.pilotWeight);
            info.AddValue("crewWeight", this.crewWeight);
        }

        public CrewList(SerializationInfo info, StreamingContext context)
        {
            try
            {
                this.numCrew = (int)info.GetValue("numCrew", typeof(int));
                this.numCockpit = (int)info.GetValue("numCockpit", typeof(int));
                this.crewList = (CrewItem[])info.GetValue("crewList", typeof(CrewItem[]));
                this.extraCrewList = (CrewItem[])info.GetValue("extraCrewList", typeof(CrewItem[]));
                this.pilotWeight = (float)info.GetValue("pilotWeight", typeof(float));
                this.crewWeight = (float)info.GetValue("crewWeight", typeof(float));
            }
            catch
            {
                this.pilotWeight = 87;
                this.crewWeight = 64;
            }
        }
        //BR08112-->

        #region Properties

        /// <summary>
        /// numCrew
        /// </summary>
        public int NumCrew
        {
            get { return numCrew; }
            set { numCrew = value; }
        }

        /// <summary>
        /// get: numCockpit
        /// set: numCockpit, crewList[0].Cockpit = value
        /// </summary>
        public int NumCockpit
        {
            get { return numCockpit; }
            set
            {
                numCockpit = value;
                if (crewList != null && crewList.Length > 0)
                    crewList[0].Cockpit = value;
            }
        }


        //<!--#BR17206 THOMAS 
        /// <summary>
        /// get: numCockpit_FO
        /// set: numCockpit_FO, crewList[1].Cockpit = value
        /// </summary>
        public int NumCockpit_FO
        {
            get { return numCockpit; }
            set
            {
                numCockpit = value;
                if (crewList != null && crewList.Length > 0)
                    crewList[1].Cockpit = value;
            }
        }


        /// <summary>
        /// get: numCockpit
        /// set: numCockpit, crewList[2].Cockpit = value
        /// </summary>
        public int NumCockpit_SO
        {
            get { return numCockpit; }
            set
            {
                numCockpit = value;
                if (crewList != null && crewList.Length > 0)
                    crewList[2].Cockpit = value;
            }
        }
        //#BR17206 -->
        /// <summary>
        /// get: crewList
        /// </summary>
        public CrewItem[] Crews
        {
            get { return crewList; }
        }

        /// <summary>
        /// get: extraCrewList
        /// </summary>
        public CrewItem[] ExtraCrews
        {
            get { return extraCrewList; }
        }

        /// <summary>
        /// get: CrewItem that contains data of Maximun number of crew
        /// </summary>
        public CrewItem MaxCrew
        {
            get
            {
                CrewItem result = new CrewItem("", 0, 0);
                if (crewList.Length > 0)
                {
                    result.Cockpit = crewList[0].MaxSeat;
                    for (int i = 1; i < crewList.Length; i++)
                        result.Cabin += crewList[i].MaxSeat;
                }
                return result;
            }
        }

        /// <summary>
        /// get: CrewItem that contains data of total number of crew
        /// </summary>
        public CrewItem TtlCrew
        {
            get
            {
                CrewItem result = new CrewItem("", 0, 0);
                foreach (CrewItem item in this.crewList)
                    result += item;
                return result;
            }
        }

        /// <summary>
        /// get: CrewItem that contains data of total number of extra crew
        /// </summary>
        public CrewItem TtlExtraCrew
        {
            get
            {
                CrewItem result = new CrewItem("", 0, 0);
                foreach (CrewItem item in this.extraCrewList)
                    result += item;
                return result;
            }
        }

        #endregion

        #region Methods

        /// <summary>use Crew number to search the distribution of each class</summary>
        /// <param name="value">Crew number</param>
        /// <returns>bool: find out or not</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool CrewDistribution(int value)
        {
            //look for standard crew distribution
            AirTypeEx airType = theFlight.ACType;
            if (airType.CrewDistribution != null)
            {
                foreach (CrewDistributionDistribution crewInfo in airType.CrewDistribution.Distribution)
                {
                    if (crewInfo.crewCount == value)
                    {
                        Hashtable h = new Hashtable();
                        foreach (CrewDistributionDistributionInfo disInfo in crewInfo.info)
                            h.Add(disInfo.crewPos, disInfo.crewNo);

                        foreach (CrewItem crewItem in crewList)
                        {
                            try
                            {
                                crewItem.Cabin = Convert.ToInt32(h[crewItem.Name]);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        numCrew = value;
                        return true;
                    }
                }
            }

            //crew distribution not found!!
            return false;
        }

        /// <summary> get  weight </summary>
        /// <returns>double: weight</returns>
        /// <remarks></remarks> 
        public float GetWeight()
        {
            float ttlWeight = 0;
            // compute  Crew  weight 
            foreach (CrewItem crewItem in crewList)
                ttlWeight += crewItem.Cockpit * this.pilotWeight + crewItem.Cabin * this.crewWeight;

            // compute  Extra Crew  weight 
            foreach (CrewItem crewItem in extraCrewList)
                ttlWeight += crewItem.Cockpit * this.pilotWeight + crewItem.Cabin * this.crewWeight;
            return ttlWeight;
        }

        /// <summary> get Index</summary>
        /// <returns>double: index</returns>
        /// <remarks></remarks> 
        public float GetIndex()
        {
            float index = 0;
            // compute  Index of Crew
            foreach (CrewItem crewItem in crewList)
                index += (crewItem.Cockpit * this.pilotWeight + crewItem.Cabin * this.crewWeight) * crewItem.IndexPerKg;

            try
            {
                AirTypeEx airType = theFlight.ACType;
                string cabinCFG = this.theFlight.CabinConfiguration;

                //confirm the to be used Cabin Configuration
                CabinConfigurationSeatplan seatplan = airType.GetSeatplan(cabinCFG);
                ArrayList ClassNames = FlightUtil.GetClassNamesFromCabinConfiguarion(cabinCFG);
                //<!--#BR17206 THOMAS 改為ZONE 計算index 
                foreach (CrewItem crewItem in extraCrewList)
                {
                    int i = 0;
                    if (crewItem.Name == "First" || crewItem.Name == "Secondary" || crewItem.Name == "Last") //讀取舊的wbf 時，EXTRA CREW  是以 F/C/Y顯示，以下做判斷轉換
                    {
                            switch (crewItem.Name) 
                            {
                            case "First":
                               i =0;
                               break;
                            case "Secondary":
                               i = 1;
                               break;
                                case "Last":
                               i = 2;
                                break;
                            default:
                               break;
                            }

                            index += (crewItem.Cockpit * this.pilotWeight + crewItem.Cabin * this.crewWeight) * airType.GetCabinClassInfo(seatplan, ClassNames[i] as String).index;
                    }
                    else
                    {
                        index += (crewItem.Cockpit * this.pilotWeight + crewItem.Cabin * this.crewWeight) * crewItem.IndexPerKg;
                    }
                }

                #region 原本的CODE
                /*
                //compute Index of Extra Crew
                for (int i = 0; i < ClassNames.Count; i++)
                {
                    index += (extraCrewList[i].Cockpit * this.pilotWeight + extraCrewList[i].Cabin * this.crewWeight) * airType.GetCabinClassInfo(seatplan, ClassNames[i] as String).index;
                }
                 */
                #endregion

                //#BR17206 -->
            }
            catch
            {
                throw (new Exception(string.Format(EWBSCoreWarnings.CabinConfigurationErrorExtraCrew)));
            }

            return index;
        }

        /// <summary>To do some Checks in Crew Quantities</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Check()
        {
            //check that each of (class Cockpit + Cabin) can not exceed Max.
            foreach (CrewItem CI in this.Crews)
            {
                if (CI.Cockpit + CI.Cabin > CI.MaxSeat)
                    throw (new Exception(string.Format(EWBSCoreWarnings.CrewExceedsMax_1, CI.Name)));
            }
        }

        #endregion

        #region DeSerialize Methods

        /// <summary>
        /// fix up the nonserialized data
        /// </summary>
        /// <param name="sender">sender, the flight</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void OnDeserialization(Object sender)
        {
            this.theFlight = (Flight)sender;
            //#BR08112<--
            // get Crew weight 
            //			AirlineEx airline = theFlight.Airline;
            //			pilotWeight = airline.PersonWeight.Crew.P;
            //			crewWeight = airline.PersonWeight.Crew.C;
            //#BR08112-->
            //#BR08112<--
            try
            {
                pilotWeight = theFlight.Crew.CrewWtCockpit;
                crewWeight = theFlight.Crew.CrewWtCabin;
            }
            catch
            {
                // get Crew weight 
                AirlineEx airline = theFlight.Airline;
                pilotWeight = airline.PersonWeight.Crew.P;
                crewWeight = airline.PersonWeight.Crew.C;
            }
            //#BR08112-->

        }
        #endregion

        /// <summary>
        /// Copy one Crew List to another Crew List.
        /// </summary>
        /// <param name="other">other CrewList</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void CopyFrom(CrewList other)
        {
            theFlight = other.theFlight;
            ArrayList aList = new ArrayList();
            foreach (CrewItem crewItem in other.crewList)
            {
                aList.Add(new CrewItem(crewItem.Name, crewItem.MaxSeat, crewItem.IndexPerKg));
            }

            crewList = (CrewItem[])aList.ToArray(typeof(CrewItem));


            //<!--#BR17206 THOMAS 改為ZONE 計算INDEX
            var zoneLen = theFlight.ACType.ZonePantryIndex.zone.zinfo.Length;
            extraCrewList = new CrewItem[zoneLen];
            for (int i = 0; i < zoneLen; i++)
            {
                extraCrewList[i] = new CrewItem(theFlight.ACType.ZonePantryIndex.zone.zinfo[i].id, 0, theFlight.ACType.ZonePantryIndex.zone.zinfo[i].index);
            }

            #region  原本的
            /*
            extraCrewList = new CrewItem[3];
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                    extraCrewList[i] = new CrewItem("First", 0, 0);
                else if (i == 1)
                    extraCrewList[i] = new CrewItem("Secondary", 0, 0);
                else if (i == 2)
                    extraCrewList[i] = new CrewItem("Last", 0, 0);
            }
             */
            #endregion

            //#BR17206-->

            //#BR08112<--
            //			// get Crew weight 
            //			pilotWeight = other.pilotWeight;
            //			crewWeight = other.crewWeight;
            //#BR08112-->

            //#BR08112<--
            // get Crew weight 
            try
            {
                pilotWeight = other.pilotWeight;
                crewWeight = other.crewWeight;
            }
            catch
            {
                AirlineEx airline = theFlight.Airline;
                pilotWeight = airline.PersonWeight.Crew.P;
                crewWeight = airline.PersonWeight.Crew.C;
            }
            //#BR08112-->
        }
        //#BR08112<--

        public float CrewWtCockpit
        {
            get
            {
                return pilotWeight;
            }
            set
            {
                AirlineEx airline = theFlight.Airline;
                //				float tmpFDBPaxWt = airline.PersonWeight.Crew.P;
                float tmpPaxWeight = pilotWeight;
                float userInputWt = value;

                //				if (userInputWt < (tmpFDBPaxWt * 0.9) || userInputWt > (tmpFDBPaxWt * 1.1))
                if (userInputWt < 10 || userInputWt > 99)
                {
                    pilotWeight = tmpPaxWeight;
                }
                else
                {
                    pilotWeight = value;
                }
            }
        }

        public float CrewWtCabin
        {
            get
            {
                return crewWeight;
            }
            set
            {
                AirlineEx airline = theFlight.Airline;
                //				float tmpFDBPaxWt = airline.PersonWeight.Crew.C;
                float tmpPaxWeight = crewWeight;
                float userInputWt = value;

                //				if (userInputWt < (tmpFDBPaxWt * 0.9) || userInputWt > (tmpFDBPaxWt * 1.1))
                if (userInputWt < 10 || userInputWt > 99)
                {
                    crewWeight = tmpPaxWeight;
                }
                else
                {
                    crewWeight = value;
                }
            }
        }
        //#BR08112-->
    }
}