/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/

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
    /// Summary description for SvcItem.
    /// </summary>
    [Serializable]
    public class SvcItem
    {
        //[NonSerialized] private Flight theFlight;
        private string desc; //Service Item Description
        private float index, weight; //index; weight
        private string posn; //position

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">Service description</param>
        /// <param name="weight">Weight it contains</param>
        /// <param name="posn">the position of the service item</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public SvcItem(string description, float weight, string posn)
        {
            //setup the description, weight and positions.
            this.desc = description;
            this.weight = weight;
            this.posn = posn;
        }

        /// <summary>
        /// Calculate the Index of this Service item inaccordance with the weight and position provided.
        /// </summary>
        /// <param name="theFlight">Flight obj</param>
        /// <returns>the index</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float CalcIndex(Flight theFlight)
        {
            float result;
            AirTypeEx theAirType = theFlight.ACType;

            try
            {
                //check fuel tank first
                //use all of the tank volume as parameters 
                //Note : tank name has to be in lower case
                Hashtable vars = new Hashtable();
                vars[posn.ToLower()] = (this.weight / theFlight.Fuel.FuelDensity).ToString();
                result = theAirType.CalcFuelIndex(vars, theFlight.Fuel.FuelDensity);
                if (result != float.NegativeInfinity) return (float)Math.Round(result, 1);

                //Calculate Crew
                result = theAirType.CalcCrewIndex(posn, weight);
                if (result != float.NegativeInfinity) return (float)Math.Round(result, 1);

                //Calculate Galley
                result = theAirType.CalcGalleyIndex(posn, weight);
                if (result != float.NegativeInfinity) return (float)Math.Round(result, 1);

                //Calculate check seat finally
                result = theAirType.CalcSeatIndex(posn, weight);
                if (result != float.NegativeInfinity) return (float)Math.Round(result, 1);

                if (posn[0] == 'C' && posn.Length > 1)
                {
                    CargoPosnBase cgoPosn = theFlight.CargoPosnMgr.FindPosn(posn.Substring(1));
                    if (cgoPosn != null)
                    {
                        result = cgoPosn.IndexPerKg * weight;
                        if (result != float.NegativeInfinity) return (float)Math.Round(result, 1);
                    }
                }
            }
            catch
            {
            }
            return float.NegativeInfinity;
        }

        #region Properties

        /// <summary>
        /// Description
        /// </summary>
        public string Desc
        {
            //20060310
            get { return desc.ToUpper(); }
            set { desc = value.ToUpper(); }
        }

        /// <summary>
        /// Position
        /// </summary>
        public string Posn
        {
            get { return posn; }
            set { posn = value; }
        }

        /// <summary>
        /// Index
        /// </summary>
        public float Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Weight
        /// </summary>
        public float Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        /// <summary>
        /// The Index that's displayed
        /// </summary>
        public float DisplayIndex
        {
            get { return (float)Math.Round(index, 1); }
        }

        # endregion
    }

    /// <summary>
    /// the List for containing all the service items.
    /// </summary>
    [Serializable]
    public class ServiceItemList : ArrayList
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ServiceItemList()
        {
        }


        /// <summary>
        /// adding a SvcItem into ServiceItemList
        /// </summary>
        /// <param name="item">SvcItem</param>
        /// <returns>int: the index of the position in which the SvcItem is put in ServiceItemList</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Add(SvcItem item)
        {
            //Prevent duplication
            foreach (SvcItem svcItem in this)
            {
                //if the Service item's position is not null
                if (svcItem.Posn != "")
                {
                    //if it is a service item that's already exists
                    if (svcItem.Desc.ToLower().Equals(item.Desc.ToLower()) &&
                        svcItem.Posn.ToLower().Equals(item.Posn.ToLower()))
                    {
                        svcItem.Weight = item.Weight;
                        return this.IndexOf(svcItem);
                    }
                }
            }
            //if not dupliacated, add the item.
            return base.Add(item);
        }


    }

    /// <summary>
    /// For manipulating all the service data
    /// </summary>
    [Serializable]
    public class ServiceItems
    {
        [NonSerialized]
        private Flight theFlight; //Flight
        [NonSerialized]
        private float wtankPercetage; //water tank measured in percentage
        private string wtankPercetageString = "Full"; //string "full" for water tank usage
        private ServiceItemList otherItems; //Other service items
        private ClsDstnClassItem soc; //Seat occupied with cargo

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
        public ServiceItems(Flight theFlight)
        {
            //setup the Flight
            this.theFlight = theFlight;

            //setup the Service items
            otherItems = new ServiceItemList();
            //setup the SOC
            soc = new ClsDstnClassItem("SOC");

            //setup the watertank as full
            WtankPercetage = "Full";

        }

        #endregion

        #region Properties

        /// <summary>
        /// get data of seat occupied by cargo
        /// </summary>
        public ClsDstnClassItem SOC
        {
            get
            {
                int[] seatOccupied = new int[] { 0, 0, 0 };
                if (theFlight != null)
                {
                    string cfg = theFlight.CabinConfiguration;
                    AirTypeEx theAirType = theFlight.ACType;
                    // compute SOC
                    foreach (SvcItem item in otherItems)
                    {
                        try
                        {
                            int row = theAirType.Seat2Row(item.Posn);
                            seatOccupied[theAirType.Row2Class(cfg, row)] += 1;
                        }
                        catch
                        {
                            //ignored it
                        }
                    }
                }

                //Set the soc of each class
                soc.FrstPaxNumber = seatOccupied[0];
                soc.SryPaxNumber = seatOccupied[1];
                soc.LstPaxNumber = seatOccupied[2];
                return soc;
            }
        }

        /// <summary>
        /// get: water tank percentage in string format
        /// set: not only modify the variable "wtankPercetageString", but also have the SvcItem related to watertank changed.
        /// </summary>
        public string WtankPercetage
        {
            get
            {
                foreach (SvcItem svcItem in otherItems)
                {
                    if (svcItem.Desc.ToUpper().IndexOf(FlightUtil.SvcWaterDesc.ToUpper()) >= 0)
                    {
                        return wtankPercetageString;
                    }
                } //foreach
                return "FULL";
            }

            set
            {
                try
                {
                    wtankPercetageString = value;
                    //Expression Parser
                    ExpressionParser e = new ExpressionParser();

                    wtankPercetage = (float)e.Parse(value, null);
                    if (wtankPercetage >= 2.0f)
                    {
                        wtankPercetage /= 100.0f;
                    }

                    AirTypeEx theAirType = theFlight.ACType;
                    float waterDeductionWt;
                    float waterDeductionIdx;
                    //if the carcraft type is a 74E, there'll be some special manipulation on water tank process
                    //regard 3/4 as full
                    if (theAirType.name.IndexOf("74E") >= 0)
                    {
                        if (wtankPercetageString.ToUpper().IndexOf("FULL") >= 0)
                        {
                            throw (new Exception(EWBSCoreWarnings.SvcFullWaterTank));
                        }
                        else
                        {
                            //regard 3/4 as full
                            waterDeductionWt = theAirType.GetWaterWt(wtankPercetage - 0.75f);
                            waterDeductionIdx = theAirType.GetWaterIdx(wtankPercetage - 0.75f);
                        }
                    }
                    else
                    {
                        waterDeductionWt = theAirType.GetWaterWt(wtankPercetage - 1);
                        waterDeductionIdx = theAirType.GetWaterIdx(wtankPercetage - 1);
                    }

                    //Calculate the water deduction by the deduction weight
                    waterDeductionWt = (float)Math.Round(waterDeductionWt, 0);
                    waterDeductionIdx = (float)Math.Round(waterDeductionIdx, 1); //

                    // #BR15111 <-- 2015/10/06 Jay - change the logics to remove the bug created by water tank
                    /*
                    //Traverse through all the service items.
                    foreach (SvcItem svcItem in otherItems)
                    {
                        if (svcItem.Desc.Equals(FlightUtil.SvcWaterDesc))
                        {
                            if (waterDeductionWt == 0f)
                            {
                                otherItems.Remove(svcItem);
                            }
                            else
                            {
                                // Update Water Deduction
                                svcItem.Weight = waterDeductionWt;
                                svcItem.Index = waterDeductionIdx;
                            }
                            return;
                        }
                    } //foreach

                    //Add Water Deduction
                    SvcItem waterDeduction = new SvcItem(FlightUtil.SvcWaterDesc, 0, "");
                    waterDeduction.Weight = waterDeductionWt;
                    waterDeduction.Index = waterDeductionIdx;

                    otherItems.Add(waterDeduction);
                    return;
                    */
                    //#BR15111 -->
                    //#BR15111 <-- 
                    if (waterDeductionWt == 0f)
                    {	// FULL Tank, remove water item if found.
                        foreach (SvcItem svcItem in otherItems)
                        {
                            if (svcItem.Desc.Equals(FlightUtil.SvcWaterDesc))
                            {
                                otherItems.Remove(svcItem);
                                return;
                            }
                        }
                    }
                    else
                    {
                        // update water item if found
                        foreach (SvcItem svcItem in otherItems)
                        {
                            if (svcItem.Desc.Equals(FlightUtil.SvcWaterDesc))
                            {
                                // Update Water Deduction
                                svcItem.Weight = waterDeductionWt;
                                svcItem.Index = waterDeductionIdx;
                                return;
                            }
                        }
                        // add new water item if not found
                        SvcItem waterDeduction = new SvcItem(FlightUtil.SvcWaterDesc, 0, "");
                        waterDeduction.Weight = waterDeductionWt;
                        waterDeduction.Index = waterDeductionIdx;
                        otherItems.Add(waterDeduction);
                    }

                    return;
                    // #BR15111 -->
                }
                catch
                {
                    wtankPercetage = 1.0f; //Full Water Tank
                }

                //Clean up Water Deduction
                foreach (SvcItem svcItem in otherItems)
                {
                    if (svcItem.Desc.Equals(FlightUtil.SvcWaterDesc))
                    {
                        otherItems.Remove(svcItem);
                        return;
                    }
                } //foreach
            }
        }

        /// <summary>
        /// get SvcItems in this ServiceItemList	
        /// </summary>
        public ServiceItemList ServicesList
        {
            get { return otherItems; }
        }

        /// <summary>
        /// get the total weight of ballast fuels
        /// </summary>
        public int Ballast
        {
            get
            {
                try
                {
                    int ttl = 0;
                    //search the scvItems containing Ballast stuff
                    foreach (SvcItem svcItem in otherItems)
                    {
                        if (svcItem.Desc.ToUpper().IndexOf("BALLAST") >= 0)
                        {
                            //Add it into total weight
                            ttl += Convert.ToInt32(svcItem.Weight);
                        }
                    }
                    return ttl;
                }
                catch
                {
                }
                return 0;
            }
        }

        /// <summary>
        /// get the total weight of unusable fuels
        /// </summary>
        public int UnusableFuel
        {
            get
            {
                try
                {
                    int ttl = 0;
                    //search the scvItems containing Ballast stuff
                    foreach (SvcItem svcItem in otherItems)
                    {
                        if (svcItem.Desc.ToUpper().IndexOf("UNUSABLE") >= 0)
                        {
                            //Add it into total weight
                            ttl += Convert.ToInt32(svcItem.Weight);
                        }
                    }
                    return ttl;
                }
                catch
                {
                }
                return 0;
            }
        }

        /// <summary>
        /// get total weight of other items
        /// </summary>
        public int OtherItemsWt
        {
            get
            {
                try
                {
                    int ttl = 0;
                    //search other scvItems
                    foreach (SvcItem svcItem in otherItems)
                    {
                        if (svcItem.Desc.ToUpper().IndexOf("BALLAST") >= 0 ||
                            svcItem.Desc.ToUpper().IndexOf("UNUSABLE") >= 0)
                        {
                            //null
                        }
                        else
                        {
                            //Add it into total weight
                            ttl += Convert.ToInt32(svcItem.Weight);
                        }
                    }
                    return ttl;
                }
                catch
                {
                }
                return 0;
            }
        }

        #endregion

        #region DeSerialize Methods

        /// <summary>fix up the nonserialized data</summary>
        /// <param name="sender"></param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void OnDeserialization(Object sender)
        {
            this.theFlight = (Flight)sender;

            //設定Water Tank的default value 
            WtankPercetage = wtankPercetageString;
        }

        #endregion

        /// <summary>Get  weight </summary>
        /// <returns>double: weight</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float GetWeight()
        {
            float ttlWeight = 0;

            // compute other Weights
            foreach (SvcItem svcItem in otherItems)
            {
                ttlWeight += svcItem.Weight;
            }

            return ttlWeight;
        }


        /// <summary>Get Index</summary>
        /// <returns>double: index</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float GetIndex()
        {
            float index = 0;

            // compute other Indexes
            foreach (SvcItem svcItem in otherItems)
            {
                if (svcItem.Posn != null && svcItem.Posn.Length > 0)
                {
                    index += svcItem.CalcIndex(theFlight);
                }
                else
                {
                    index += svcItem.Index;
                }
            }
            return index;
        }

        /// <summary>
        /// Copy other ServiceItems's data to this ServiceItems obj
        /// </summary>
        /// <param name="other">Other Service item list</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void CopyFrom(ServiceItems other)
        {
            //Copy the Flight
            this.theFlight = other.theFlight;

            //Copy the List
            this.otherItems = new ServiceItemList();
            foreach (SvcItem svcItem in other.otherItems)
                otherItems.Add(new SvcItem(svcItem.Desc, svcItem.Weight, svcItem.Posn));
        }


    }
}