/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
// Import the assembly that contains the parser
//*****************************************************************************
//* Jay Sheu      | Ver.01  | #BR15120 | Oct 26, 2015              (V3.2.1)   *
//*---------------------------------------------------------------------------*
//* If taxi weight is set to odd number from iFuel, filling tail tank will    *
//* cause exception on center tank weight check since there is no center tank *
//* on A330-300 ac type. (Array Index Out of Bound)                           *
//* A330-300連結FIS異常 taxi fuel為奇數，且無center時會出錯                   *
//*****************************************************************************
//* Jay Sheu      | Ver. 00 | #BR15111 | Sep 09, 2015              (V3.2.1)   *
//*---------------------------------------------------------------------------*
//* Added iFuel to FuelLoadingStyle                                           *
//*****************************************************************************

using System;
using System.Collections;
using EwbsCore.Util;
using FlightDataBase;
using info.lundin.Math;

namespace EWBS
{
    /// <summary>
    /// Fue loading style Enumerations
    /// </summary>
    [Serializable]
    public enum FuelLoadingStyle
    {
        STD, //Scheduled Time of Departure
        NOTAIL, //no tail
        NSTD, //Non Standard Distribution
        iFuel, // iFuel #BR15111 - JayS
    }

    /// <summary>
    /// The data of each tank's fuel.
    /// </summary>
    [Serializable]
    public class FuelItem
    {
        [NonSerialized]
        private float maxWt; // tank  volume 
        private float weight; // tank  weight 
        private string name; // tank  name 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Fuel name</param>
        /// <param name="maxWt">Maximum Weight</param>
        /// <param name="weight">weight</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public FuelItem(string name, int maxWt, int weight)
        {
            //Assgn the weight, max weight and name.
            this.name = name;
            this.maxWt = maxWt;
            this.weight = weight;
        }

        /// <summary>
        /// get max weight, i.e. tank capacity
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float Max // tank  volume 
        {
            get { return maxWt; }
            set { maxWt = value; }
        }

        /// <summary>
        /// get or set weight
        /// </summary>
        public float Weight // tank  weight 
        {
            get { return weight; }
            set { weight = value; }
        }

        /// <summary>
        /// get or set tank name
        /// </summary>
        public string Name // tank  name 
        {
            get { return name; }
            set { name = value; }
        }

    } ;

    /// <summary>
    /// Summary description for Fuel.
    /// </summary>
    [Serializable]
    public class FuelList //: IDeserializationCallback
    {
        private FuelItem[] fuelList; //the array of Fuel List

        [NonSerialized]
        private Flight theFlight; //theFlight
        private float takeoffFuel; //Takeoff Fuel weight
        private float taxiFuel; //Taxi Fuel Weight
        private float tripFuel; //Trip Fuel Weight
        private float fuelDensity = 0.792f; //fuel density
        private FuelLoadingStyle fuelLoading; //fuel operations(STD/NOTAIL/NSTD)

        //record Tail Tank & Center Tank
        [NonSerialized]
        private short nCenter = -1;
        [NonSerialized]
        private short nTail = -1;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">the parent flight data</param>
        public FuelList(Flight parent)
        {
            this.theFlight = parent;

            AirTypeEx airtype = AirType();

            int len = airtype.FuelTank.info.Length;
            if (len > 0)
            {
                fuelList = new FuelItem[len];
                // get all of the Tank information 
                for (int i = 0; i < len; i++)
                {
                    fuelList[i] = new FuelItem(airtype.FuelTank.info[i].tank, Convert.ToInt32(airtype.FuelTank.info[i].capacity * fuelDensity), 0);

                    //record Tail Tank & Center Tank
                    string code = fuelList[i].Name.Substring(0, 1).ToUpper();
                    if (code.Equals("C")) nCenter = Convert.ToInt16(i);
                    else if (code.Equals("T")) nTail = Convert.ToInt16(i);
                }
            }
            fuelLoading = FuelLoadingStyle.STD;
            takeoffFuel = 0; //Takeoff Fuel weight
            taxiFuel = airtype.FuelTank.taxifuel_default; //Taxi Fuel Weight
            tripFuel = 0; //Trip Fuel Weight
            fuelDensity = 0; //fuel density

        }

        #endregion

        #region DeSerialize Methods

        /// <summary>fix up the nonserialized data</summary>
        /// <param name="sender"></param>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void OnDeserialization(Object sender)
        {
            this.theFlight = (Flight)sender;

            AirTypeEx airtype = AirType();

            nTail = nCenter = -1;
            if (fuelList.Length > 0 && airtype.FuelTank != null)
            {
                for (int i = 0; i < fuelList.Length; i++)
                {
                    //put data - Max to tank
                    fuelList[i].Name = airtype.FuelTank.info[i].tank;
                    fuelList[i].Max = airtype.FuelTank.info[i].capacity * fuelDensity;

                    //record Tail Tank & Center Tank
                    string code = fuelList[i].Name.Substring(0, 1);
                    if (code.Equals("C")) nCenter = Convert.ToInt16(i);
                    else if (code.Equals("T")) nTail = Convert.ToInt16(i);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// to get the Caircrft type information 
        /// </summary>
        /// <returns></returns>
        private AirTypeEx AirType()
        {
            //simulate to get Airtype information 
            return this.theFlight.ACType; //FDB.Instance.airType;
        }


        /// <summary>
        /// get or set //fuel operations(STD/NOTAIL/NSTD)
        /// </summary>
        public FuelLoadingStyle FuelLoading
        {
            get { return fuelLoading; }
            set
            {
                if (value != fuelLoading)
                {
                    fuelLoading = value;

                    //reset the Fuel distribution to standard
                    StdFuelDistribution();
                }
            }
        }

        /// <summary>
        /// get a list of Fuel Item information
        /// </summary>
        public FuelItem[] Tanks
        {
            get { return fuelList; }
        }

        /// <summary>
        /// with Tail Tank or not
        /// </summary>
        public int IdxTailTank
        {
            get
            {
                return this.nTail; //with Tail Tank or not
            }
        }

        /// <summary>
        /// with central Tank or not
        /// </summary>
        public int IdxCenterTank
        {
            get { return this.nCenter; }
        }


        /// <summary>
        /// get or set Tail fuel
        /// </summary>
        public float TailFuel
        {
            get
            {
                return (nTail >= 0 ? fuelList[nTail].Weight : 0); //Tail Tank weight
            }
            set
            {
                if (nTail < 0) return;
                if (value < 0) value = 0;
                if (fuelLoading != FuelLoadingStyle.NSTD)
                {
                    foreach (FuelItem fuelItem in fuelList)
                        fuelItem.Weight = 0f;
                    fuelList[nTail].Weight = value;
                    // compute  Index of TOF, 先proceed std. fuel distribution
                    StdFuelPlanning(takeoffFuel + taxiFuel);
                }
                else
                {
                    //set the value to the weight
                    fuelList[nTail].Weight = value;
                }

                if (value > 0) theFlight.UsingIncreasedCumulativeLoad = false;
            }
        }


        /// <summary>
        /// get or set fuel density
        /// </summary>
        public float FuelDensity
        {
            get
            {
                return fuelDensity; // get fuel density
            }
            set
            {
                if (value < 0.72) //#BR19014 if (value < 0.732)
                    throw (new Exception(EWBSCoreWarnings.FuelDensityUnderflow));
                else if (value > 0.86) //#BR19014 else if (value > 0.851)
                    throw (new Exception(EWBSCoreWarnings.FuelDensityOverflow));
                else if (fuelDensity != value)
                {
                    fuelDensity = value; //set fuel density
                    if (fuelList.Length > 0)
                    {
                        Hashtable h;
                        h = new Hashtable();

                        // get all of the Tank information 
                        foreach (FuelTankInfo ftank in AirType().FuelTank.info)
                            h.Add(ftank.tank, ftank.capacity);

                        for (int i = 0; i < fuelList.Length; i++)
                        {
                            fuelList[i].Max = Convert.ToSingle(h[fuelList[i].Name]) * fuelDensity;
                        }

                        if (theFlight.Fuel.FuelLoading != FuelLoadingStyle.iFuel)
                            this.StdFuelDistribution();
                    } //if
                }
            } //set
        }

        //TaxiFuel
        /// <summary>
        ///  get or set taxi fuel
        /// </summary>
        public int TaxiFuel
        {
            get
            {
                return Convert.ToInt32(taxiFuel); // get Taxi Fuel
            }
            set
            {
                if (value < 0) taxiFuel = 0;
                else taxiFuel = value; //set Taxi Fuel

            }
        }

        //TripFuel
        /// <summary>
        /// get or set trip fuel
        /// </summary>
        public int TripFuel
        {
            get
            {
                //Trip Fuel
                return Convert.ToInt32(tripFuel); // get Trip Fuel
            }
            set
            {
                if (value < 0) tripFuel = 0;
                else
                {
                    tripFuel = value; //set Trip Fuel
                }
            }
        }

        /// <summary>
        /// get or set Takeoff fuel
        /// </summary>
        public int TakeoffFuel
        {
            get
            {
                return Convert.ToInt32(takeoffFuel); //Takeoff Fuel
            }
            set
            {
                if (value < 0)
                    takeoffFuel = 0;
                else
                {
                    //Takeoff Fuel
                    // #BR15111 <-- 2015/10/06 Jay - removed take off fuel auto rounding on iFuel
                    //takeoffFuel = (value + 9) / 10 * 10;
                    if (FuelLoading == FuelLoadingStyle.iFuel)
                        takeoffFuel = value;
                    else
                        takeoffFuel = (value + 9) / 10 * 10; //
                    // #BR15111 -->
                }
            }
        }

        /// <summary>
        /// get Takeoff Index
        /// </summary>
        public float TOFIndex
        {
            get
            {
                float taxifuel = this.taxiFuel;
                int len = fuelList.Length;
                float[] fuelWts = new float[len];

                //use tank  volume as parameter 
                Hashtable vars = new Hashtable();
                for (int i = 0; i < len; i++)
                {
                    FuelItem fuelItem = fuelList[i];
                    vars[fuelItem.Name.ToLower()] = (fuelItem.Weight / this.fuelDensity).ToString();
                    fuelWts[i] = fuelItem.Weight;
                }


                //deduct taxi fuel from Center tank
                if (taxifuel > 0 && this.nCenter >= 0)
                {
                    if (fuelList[nCenter].Weight >= taxifuel)
                    {
                        vars[fuelList[nCenter].Name.ToLower()] = ((fuelList[nCenter].Weight - taxifuel) / this.fuelDensity).ToString();
                        taxifuel = 0;
                    }
                    else
                    {
                        vars[fuelList[nCenter].Name.ToLower()] = "0";
                        taxifuel -= fuelWts[nCenter];
                        fuelWts[nCenter] = 0;
                    }
                }

                if (taxifuel > 0)
                {
                    if (this.nTail >= 0)
                        fuelWts[nTail] = 0;

                    while (taxifuel > 0)
                    {
                        float peekWt = 0f;
                        float preWt = 0f;
                        int count = 0;

                        //find peek fuel wt
                        for (int i = 0; i < len; i++)
                        {
                            if (fuelWts[i] > peekWt)
                            {
                                preWt = peekWt;
                                peekWt = fuelWts[i];
                            }
                        }

                        //Error
                        if (peekWt == 0)
                            return float.NegativeInfinity;

                        //count it
                        for (int i = 0; i < len; i++)
                        {
                            if (fuelWts[i] == peekWt)
                                count += 1;
                        }

                        //deduct every candidate tank equally.   
                        float wt = (peekWt - preWt) * count;
                        if (wt >= taxifuel)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                if (fuelWts[i] == peekWt)
                                    fuelWts[i] -= taxifuel / count;
                            }
                            taxifuel = 0;
                        }
                        else
                        {
                            for (int i = 0; i < len; i++)
                            {
                                if (fuelWts[i] == peekWt)
                                    fuelWts[i] -= wt / count;
                            }
                            taxifuel -= wt;
                        }
                    } //while
                    for (int i = 0; i < len; i++)
                    {
                        if (i != nCenter && i != nTail)
                            vars[fuelList[i].Name.ToLower()] = (fuelWts[i] / this.fuelDensity).ToString();
                    }

                }

                // compute  Index of TOF
                AirTypeEx airType = theFlight.ACType;
                float fuelidx = airType.CalcFuelIndex(vars, this.fuelDensity);
                return fuelidx;
            }
        }

        /// <summary>
        /// get Ramp Fuel
        /// </summary>
        public int RampFuel
        {
            get { return TaxiFuel + TakeoffFuel; }
        }


        #endregion

        #region Methods

        /// <summary>proceed std. fuel distribution</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void StdFuelDistribution()
        {
            if (fuelLoading != FuelLoadingStyle.NSTD)
            {
                foreach (FuelItem fuelItem in fuelList)
                    fuelItem.Weight = 0f;

                // compute  Index of TOF, 先proceed std. fuel distribution
                StdFuelPlanning(takeoffFuel + taxiFuel);

                if (fuelLoading == FuelLoadingStyle.NOTAIL && nTail >= 0 && nCenter >= 0)
                {
                    fuelList[nCenter].Weight += fuelList[nTail].Weight;
                    fuelList[nTail].Weight = 0;
                }
            }
        }


        /// <summary>proceed std. fuel distribution</summary>
        /// <param name="wtTotalFuel">Total Fuel</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void StdFuelPlanning(float wtTotalFuel)
        {
            //takeoffFuel

            //Expression Parser
            ExpressionParser e;
            Hashtable h;

            e = new ExpressionParser();
            h = new Hashtable();

            //simulate to get Airtype information 
            AirTypeEx airtype = AirType();

            h["fuel_density"] = fuelDensity.ToString();
            h["density"] = fuelDensity.ToString();

            h["takeoff_fuel"] = wtTotalFuel.ToString();
            h["ramp_fuel"] = wtTotalFuel.ToString();

            int i;

            // get all of the Tank Capacity information 
            foreach (FuelTankInfo ftank in airtype.FuelTank.info)
                h["v" + ftank.tank.ToLower()] = ftank.capacity.ToString();

            // compute all of the fuel grade,then get the suitable one
            for (i = 0; i < airtype.FuelFormula.fuelGrade.Length; i++)
            {
                try
                {
                    // Parse and write the result
                    double result = e.Parse(airtype.FuelFormula.fuelGrade[i].gradeFor, h);
                    if (result >= wtTotalFuel) break; //found!!
                }
                catch (Exception)
                {
                    throw (new Exception(EWBSCoreWarnings.FuelTankFormulaError + airtype.FuelFormula.fuelGrade[i].gradeFor));
                }
            }
            if (i >= airtype.FuelFormula.fuelGrade.Length)
            {
                throw (new Exception(EWBSCoreWarnings.FuelOverflow));
            }

            // compute the tank weight 
            float totalFuel = 0f;
            ArrayList notFullList = new ArrayList();

            foreach (FuelFormulaFuelGradeTank ftank in airtype.FuelFormula.fuelGrade[i].tank)
            {
                try
                {
                    // Parse and write the result
                    double result = e.Parse(ftank.tankFor, h);
                    int idx = fuelList.Length - 1;
                    for (; idx >= 0; idx--)
                    {
                        if (fuelList[idx].Name.Equals(ftank.name))
                            break;
                    }

                    if (idx < 0) continue;
                    if (fuelList[idx].Weight > 0f)
                    {
                        totalFuel += fuelList[idx].Weight;
                        continue;
                    }

                    //Assign tank wt
                    float maxWt = (float)Math.Floor(fuelList[idx].Max);
                    if (result >= maxWt)
                        totalFuel += (fuelList[idx].Weight = maxWt);
                    else if (result > 0f)
                    {
                        totalFuel += (fuelList[idx].Weight = (float)Math.Ceiling(result));
                        if (idx != nCenter && idx != nTail) notFullList.Add(fuelList[idx]); //record not full tank
                    }
                    else fuelList[idx].Weight = 0f;
                }
                catch (Exception)
                {
                    throw (new Exception(EWBSCoreWarnings.FuelTankFormulaError + ftank.tankFor));
                }
            }
            //
            if (wtTotalFuel - totalFuel < 0f && notFullList.Count > 0)
            {
                for (int idx = notFullList.Count / 2; idx >= 0; idx--)
                {
                    (notFullList[idx] as FuelItem).Weight -= 1;
                    totalFuel -= 1;
                    if (wtTotalFuel - totalFuel >= 0) break;
                    int idx2 = notFullList.Count - 1 - idx;
                    if (idx != idx2)
                    {
                        (notFullList[idx2] as FuelItem).Weight -= 1;
                        totalFuel -= 1;
                        if (wtTotalFuel - totalFuel >= 0) break;
                    }
                }
            }

            //fill center tank up if possible
            if (nCenter >= 0 && wtTotalFuel - totalFuel > 0 && fuelList[nCenter].Weight > 0)
                totalFuel += fillUpTankWithExtraWt(fuelList[nCenter], wtTotalFuel - totalFuel);

            //fill tail tank up if possible
            //#BR15120if (nTail >= 0 && wtTotalFuel - totalFuel > 0 && fuelList[nCenter].Weight > 0)
            //#BR15120totalFuel += fillUpTankWithExtraWt(fuelList[nTail], wtTotalFuel - totalFuel);

            //#BR15120 <--
            if (nTail >= 0 && wtTotalFuel - totalFuel > 0)
            {
                if (theFlight.ACType.FullAirTypeName != "BR333" && fuelList[nCenter].Weight > 0)
                {
                    totalFuel += fillUpTankWithExtraWt(fuelList[nTail], wtTotalFuel - totalFuel);
                }
                else if (theFlight.ACType.FullAirTypeName == "BR333")
                    totalFuel += fillUpTankWithExtraWt(fuelList[nTail], wtTotalFuel - totalFuel);
            }
            //#BR15120 -->

            //fill wing tank up if possible
            if (wtTotalFuel != totalFuel && notFullList.Count > 0)
            {
                bool done = false;
                int extraWt = 1;
                if (wtTotalFuel < totalFuel) extraWt = -extraWt;
                while (!done)
                {
                    done = true;
                    foreach (FuelItem fuelItem in notFullList)
                    {
                        if (fuelItem.Weight + 1 < fuelItem.Max && wtTotalFuel != totalFuel)
                        {
                            fuelItem.Weight += extraWt;
                            totalFuel += extraWt;
                            done = false;
                        }
                    }
                }
            }

            if (nCenter >= 0 && wtTotalFuel != totalFuel)
                fillUpTankWithExtraWt(fuelList[nCenter], wtTotalFuel - totalFuel);


            e = null; //#BR17164 THOMAS
            h = null;
            System.GC.Collect(); //#BR17164 THOMAS GC強制回收
        }

        /// <summary>
        /// Fill up tank with extra wt
        /// </summary>
        /// <param name="aTank">Tank</param>
        /// <param name="extraWt">Extra wt</param>
        /// <returns>Extra wt that filled up tank</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private float fillUpTankWithExtraWt(FuelItem aTank, float extraWt)
        {
            float maxWt = (float)Math.Floor(aTank.Max);
            if (maxWt >= aTank.Weight + extraWt)
            {
                aTank.Weight += extraWt;
                return extraWt;
            }
            else
            {
                aTank.Weight = maxWt;
                return maxWt - aTank.Weight;
            }
        }


        /// <summary>To do some Checks in Pax Quantities</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Check()
        {
            string errMsg = "";
            //if Fuel weight grater then MAX, then display the warning message。
            foreach (FuelItem FI in this.Tanks)
                if (FI.Weight > FI.Max)
                {
                    errMsg += "\r\n" + string.Format("Error! Fuel weight in Tank {0} exceeds MAX!", FI.Name);
                }

            //if Fuel density grater then or less than density restrict, then display the warning message。
            if (this.FuelDensity > 0.86f || this.FuelDensity < 0.72f) // #BR19014 Thomas if (this.FuelDensity > 0.83f || this.FuelDensity < 0.766f)
                errMsg += "\r\n" + EWBSCoreWarnings.FuelDensityViolatesLmt;

            //3. Trip Fuel should not be larger than Take off Fuel
            if (this.tripFuel > this.takeoffFuel)
                errMsg += "\r\n" + EWBSCoreWarnings.FuelTFShouldNotLargerthanTOF;


            if (errMsg != "") throw (new Exception(errMsg));
        }

        #endregion

        /// <summary>
        /// Copy From one fuel list to this fuel list
        /// </summary>
        /// <param name="other">other fuel list</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void CopyFrom(FuelList other)
        {
            //Copy th Flight
            this.theFlight = other.theFlight;

            ArrayList aList = new ArrayList();
            //Copy each fuelItem into the fuellist
            foreach (FuelItem fuelItem in other.fuelList)
                aList.Add(new FuelItem(fuelItem.Name, Convert.ToInt32(fuelItem.Max), Convert.ToInt32(fuelItem.Weight)));

            fuelList = (FuelItem[])aList.ToArray(typeof(FuelItem));
            //Copy all the other data, such as takeoff fuel, taxi fule, trip fuel, etc
            nCenter = other.nCenter;
            nTail = other.nTail;
            fuelLoading = other.fuelLoading;
            takeoffFuel = other.takeoffFuel;
            taxiFuel = other.taxiFuel;
            tripFuel = other.tripFuel;
            fuelDensity = other.fuelDensity;

            aList = null; //#BR17164 THOMAS
            System.GC.Collect(); //#BR17164 THOMAS GC強制回收
        }

    } //

}