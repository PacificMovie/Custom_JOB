/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//* WUPC      | Ver. 00 | #BR071005 | 2007/OCT/03                             *
//*---------------------------------------------------------------------------*
//* Increase flight destination                                               *
//*****************************************************************************
//* Cherry    | Ver. 01 | #BR15104  | 2015/APR/14                (V03.01.01)  *
//*---------------------------------------------------------------------------*
//盤櫃只有長榮的,故CREATE B7 FLT時,預設BR盤櫃                                 *
//*****************************************************************************
//*THOMAS| Ver. 02 | #BR17164 | 2017/8/31                                     *
//*---------------------------------------------------------------------------*
//* 物件關閉後 Dispose 標記由GC回收                                           *
//*****************************************************************************
//* THOMAS    | Ver. 03 | #BR17227  | 2018/1/29                               *
//*---------------------------------------------------------------------------*
// Baggage 滑鼠右鍵顯示delete視窗，刪除指定 uld Serial No                     *
//*****************************************************************************
using System;
using System.Collections;
using EWBS;
using EwbsCore.Baggage;
using EwbsCore.Util;

namespace nsBaggage
{
    /// <summary>
    /// Manage the list of Baggage  weight of Baggage Container
    /// </summary>
    public class BagManager
    {
        private Flight theFlight = null; //Flight object

        private BagRuleList bagRuleList = null; //bagRuleList 存 BagRule object


        //
        //  Baggage have "BC"、and "BY" 2 categories.
        //

        private static string Baggage_BC = "BC"; // "BC" category
        private static string Baggage_BY = "BY"; // "BY" category

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight"></param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagManager(Flight theFlight)
        {
            this.theFlight = theFlight;
            bagRuleList = new BagRuleList();

        }

        #endregion

        #region Properties

        /// <summary>
        /// bagRuleList is composed of BagRule object
        /// </summary>
        public BagRuleList BagRuleList
        {
            get { return GenNewBagRules(); }
        }


        /// <summary>
        /// baggageList is composed of Baggage object
        /// </summary>
        private CargoList BaggageList
        {
            get { return theFlight.BaggageList; }
        }

        /// <summary>
        /// bagCsgmtList is composed of Consignment object
        /// </summary>
        private ConsignmentList BagCsgmtList
        {
            get { return theFlight.BagCsgmtList; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Call Overload ReCalculate method to distribute baggage weight
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void ReCalculate()
        {
            ReCalculate(this.GenNewBagRules());
        }


        /// <summary>
        /// ReCalculate baggage weight
        /// </summary>
        /// <param name="_bagRuleList">BagRuleList obj</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void ReCalculate(BagRuleList _bagRuleList, string uldString = " ")
        //public void ReCalculate(BagRuleList _bagRuleList) //#BR17227 THOMAS 改為傳入uldString
        {
            bagRuleList = _bagRuleList;
            
            BuildBaggageList(bagRuleList, BaggageList, uldString);
            //BuildBaggageList(bagRuleList, BaggageList); //#BR17227 THOMAS 改為傳入uldString

            //plan baggage weight。
            BagPlanning();

            //generate BagRules
            GenNewBagRules();
        }


        /// <summary>
        /// Put Baggage Rules into baggageList。
        /// </summary>
        /// <param name="bagRules">Baggage Rule ArrayList</param>
        /// <param name="bagList">Baggage List</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void BuildBaggageList(ArrayList bagRules, CargoList bagList, string uldString =" ")
        //#BR17227 private void BuildBaggageList(ArrayList bagRules, CargoList bagList)  //#BR17227 THOMAS 改為傳入uldString
        {
            //
            // The 4 following conditions should be considered：
            //    Bag Class    Destination   Output
            // 1. BC, BY       SEA, EWR      one Baggage Container with 4 Consignments list, mix-pack is not allowed
            // 2. BC           SEA, EWR      one Baggage Container with 2 Consignments list, mix-pack is allowed
            // 3. BC, BY       SEA           one Baggage Container with 2 Consignments list, mix-pack is allowed
            // 4. BC           SEA           one Baggage Container with 1 Consignment list, mix-pack is not allowed

            ArrayList candidates = new ArrayList();
            candidates.AddRange(bagList);

            for (int i = candidates.Count - 1; i >= 0; i--) //第一次將"有UldserialNo的"會先搜出，並從後面開始去除，剩下沒UldserialNo以及"部分有UldserialNo"的
            {
                Baggage baggage = candidates[i] as Baggage;
                    if (baggage.SerialNo != uldString && (baggage.bFlagUserModified || baggage.SerialNo != "" || baggage.Posn != "" || baggage.SHC != "" || (baggage.HeightCode != "" && !baggage.IsContainer) || baggage.CarrierCode != theFlight.CarrierCode))
                    //if (baggage.bFlagUserModified || baggage.SerialNo != "" || baggage.Posn != "" || baggage.SHC != "" || (baggage.HeightCode != "" && !baggage.IsContainer) || baggage.CarrierCode != theFlight.CarrierCode) //#BR17227 THOMAS  增加判斷baggage.SerialNo
                    { 
                        BagRule bagRule = baggage.GetBagRule();
                        foreach (BagRule tmpRule in bagRules)
                        {
                            if (bagRule.Equals(tmpRule) && tmpRule.Qty > 0 ) //輸入同項目的(BC or BY)，且輸入的QTY數量 >0
                            {
                                tmpRule.Qty -= bagRule.Qty;
                                candidates.RemoveAt(i);//剩下 N Qty
                                break;
                            }
                        }
                    }
            }

            for (int i = candidates.Count - 1; i >= 0; i--) //第二次將剩下無ULDserialNo與有ULDserialNo的項目再次從後面去除
            {
                    Baggage baggage = candidates[i] as Baggage;
                    if (baggage.SerialNo != uldString) //#BR17227 //#BR17227 THOMAS  增加判斷baggage.SerialNo
                    {
                        BagRule bagRule = baggage.GetBagRule();
                        foreach (BagRule tmpRule in bagRules)
                        {
                            if (bagRule.Equals(tmpRule) && tmpRule.Qty > 0)
                            {
                                tmpRule.Qty -= bagRule.Qty; //輸入的數量扣掉??的數量
                                candidates.RemoveAt(i);  //剩下 N Qty
                                break;
                            }
                        }
                    } 
            }

            for (int i = candidates.Count - 1; i >= 0; i--)
            {
                Baggage baggage = candidates[i] as Baggage;
                BagRule bagRule = baggage.GetBagRule();
                foreach (BagRule tmpRule in bagRules)
                {
                    if (bagRule.Similar(tmpRule) && tmpRule.Qty > 0)
                    {
                        if (baggage.MaxBagWt != tmpRule.MaxBagWt)
                            baggage.MaxBagWt = tmpRule.MaxBagWt; //
                        else if (!baggage.UldType.Equals(tmpRule.ULDType))
                            baggage.UldType = tmpRule.ULDType; //
                        else
                        {
                            Baggage aBaggage = genOneBaggage(tmpRule);

                            for (int idx1 = baggage.Consignments.Count - 1; idx1 >= 0; idx1--)
                            {
                                Consignment csgmt = baggage.Consignments[idx1] as Consignment;
                                if (csgmt is SpecialLoad) continue;
                                bool found = false;
                                for (int idx2 = aBaggage.Consignments.Count - 1; idx2 >= 0; idx2--)
                                {
                                    Consignment csgmt2 = aBaggage.Consignments[idx2] as Consignment;
                                    if (csgmt._Category.Equals(csgmt2._Category) && csgmt._Dest.Equals(csgmt2._Dest))
                                    {
                                        found = true;
                                        aBaggage.Consignments.RemoveAt(idx2);
                                        break;
                                    }
                                }
                                if (!found) baggage.Consignments.RemoveAt(idx1);
                            }

                            foreach (Consignment csgmt in aBaggage.Consignments)
                            {
                                baggage.Consignments.Add(csgmt);
                            }
                            aBaggage.Consignments.Clear();
                        }

                        tmpRule.Qty -= bagRule.Qty;
                        candidates.RemoveAt(i);
                        break;
                    }
                }
            }

            foreach (ICargo bag in candidates)
            {
                bag.Posn = "";
                bagList.Remove(bag); //Qty減少，  bagList移除candidates 剩下的
            }

            //Add baggage 
            foreach (BagRule tmpRule in bagRules)
            {
                for (int iCount = tmpRule.Qty; iCount > 0; iCount--)
                {
                    bagList.Add(genOneBaggage(tmpRule));// Qty增加 ，bagList新增空的資料
                }
            }
        }


        /// <summary>
        /// Generate a new Baggage Container
        /// </summary>
        /// <param name="tmpRule">BagRule obj</param>
        /// <returns>Baggage obj</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private Baggage genOneBaggage(BagRule tmpRule)
        {
            // Both category and destination have mix-pack are not allowed
            if (tmpRule.CategoryList.Count >= 1 && tmpRule.DestList.Count == 1 ||
                tmpRule.CategoryList.Count == 1 && tmpRule.DestList.Count >= 1)
            {
                // The Rule shall have at least one ULD
                // Generate a Baggage object to use as Baggage Container
                // aBagContainer is created by Rule
                //BR15104<--
                //				Baggage aBagContainer = new Baggage(
                //					theFlight,
                //					tmpRule.ULDType,
                //					theFlight.CarrierCode,
                //					"",
                //					"",
                //					tmpRule.MaxBagWt);
                //BR15104-->

                //BR15104<--
                Baggage aBagContainer;
                if (theFlight.CarrierCode == "B7")
                {
                    aBagContainer = new Baggage(
                        theFlight,
                        tmpRule.ULDType,
                        "BR",
                        "",
                        "",
                        tmpRule.MaxBagWt);
                }
                else
                {
                    aBagContainer = new Baggage(
                        theFlight,
                        tmpRule.ULDType,
                        theFlight.CarrierCode,
                        "",
                        "",
                        tmpRule.MaxBagWt);
                }
                //BR15104-->


                aBagContainer.bFlagTransit = tmpRule.bTransit;

                // Baggage Class may have mix-pack, for example BC/BY mix-pack, but the Destination shall be unique.
                if (tmpRule.CategoryList.Count > 1)
                {
                    foreach (string catg in tmpRule.CategoryList)
                    {
                        Consignment csgmt = new Consignment(theFlight, catg, tmpRule.DestList[0] as string);
                        csgmt.bFlagTransit = tmpRule.bTransit;
                        aBagContainer.Consignments.Add(csgmt);
                    }
                }
                // dest may have mix-pack, for example SEA/EWR mix-pack, but the Baggage Class shall be unique.
                else if (tmpRule.DestList.Count > 1)
                {
                    foreach (string dest in tmpRule.DestList)
                    {
                        Consignment csgmt = new Consignment(theFlight, tmpRule.CategoryList[0] as string, dest);
                        csgmt.bFlagTransit = tmpRule.bTransit;
                        aBagContainer.Consignments.Add(csgmt);
                    }
                }
                else
                { // Both dest and Baggage are not have mix-pack.
                    Consignment csgmt = new Consignment(theFlight, tmpRule.CategoryList[0] as string, tmpRule.DestList[0] as string);
                    csgmt.bFlagTransit = tmpRule.bTransit;
                    aBagContainer.Consignments.Add(csgmt);
                }

                return aBagContainer;
            }
            return null;

        }


        /// Through this.BagRuleList to get GenNewBagRules() from other programs
        /// <summary>
        /// Generate a new BagRuleList
        /// </summary>
        /// <returns>BagRuleList obj</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private BagRuleList GenNewBagRules()
        {
            foreach (BagRule aBagRule in bagRuleList)
                aBagRule.Qty = 0;

            foreach (Baggage aBagContainer in BaggageList)
            {
                // If BagContainer has mix-pack，then there will have mix-pack data in Dest or  Category,
                // for example Dest ="SEA, EWR", Category = "BC,BY",
                // this imply that BagContainer have destination and Baggage Class data. 

                bagRuleList.Add(aBagContainer.GetBagRule());
            }


            return bagRuleList;
        }


        /// <summary>
        /// Sum the baggage number of Booked and Accepted PAX
        /// </summary>
        /// <returns>ClsDstnBagItem array</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnBagItem[] SumFinalPaxBagItem()
        {
            //  get the Booked but uncheck-In Passengers'  baggage weight
            ClsDstnBagItem[] notCheckInPaxBagItemList = CalNotYetCheckInEachStationBagWeight();

            ArrayList aList = new ArrayList();

            for (int i = 0; i < theFlight.Pax.AcceptedBagList.Length; i++)
            {
                aList.Add(
                    theFlight.Pax.CurrentAcceptedBagList[i] +
                        theFlight.Pax.AdjustedBagList[i] +
                        notCheckInPaxBagItemList[i]);
            }
            return (ClsDstnBagItem[])aList.ToArray(typeof(ClsDstnBagItem));
        }

        /// <summary>
        /// SUm all of FinalPaxBageItem
        /// </summary>
        /// <param name="transit">transit flag</param>
        /// <returns>ClsDstnBagItem object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private ClsDstnBagItem[] SumFinalPaxBagItem(bool transit)
        {
            //  get the Booked but uncheck-In Passengers'  baggage weight
            ClsDstnBagItem[] notCheckInPaxBagItemList = CalNotYetCheckInEachStationBagWeight();

            ArrayList aList = new ArrayList();

            for (int i = 0; i < theFlight.Pax.AcceptedBagList.Length; i++)
            {
                //#BR071005 <--
                string station = theFlight.Pax.AcceptedBagList[i].Dstn;//#BR071005 - Get the destination of baggage list
                //#BR071005 -->
                if (transit)
                {
                    //##BR071005 <-- Prepare the output of baggage
                    ClsDstnBagList prevAcceptedBagList = theFlight.Pax.PrevAcceptedBagList;
                    if (i < prevAcceptedBagList.Length)
                        aList.Add(prevAcceptedBagList[i]);
                    else
                        aList.Add(new ClsDstnBagItem(station));//Create baggagr components in screen for new destinations
                    //aList.Add(
                    //	theFlight.Pax.PrevAcceptedBagList[i]);
                    //##BR071005 -->
                }
                else
                {
                    aList.Add(
                        theFlight.Pax.CurrentAcceptedBagList[i] +
                            theFlight.Pax.AdjustedBagList[i] +
                            notCheckInPaxBagItemList[i]);
                }
            }
            return (ClsDstnBagItem[])aList.ToArray(typeof(ClsDstnBagItem));
        }


        /// <summary>
        /// Find the Consignments whit distributable weight
        /// </summary>
        /// <param name="bagCsgmtList">ConsignmentList obj</param>
        /// <param name="catg"> class</param>
        /// <param name="dest"> destination </param>
        /// <returns>ArrayList obj</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private ArrayList FindCandidateConsignments(ConsignmentList bagCsgmtList, string catg, string dest)
        {
            ArrayList candidates = new ArrayList();
            //bulk portion
            foreach (Consignment aConsignment in bagCsgmtList)
            {
                if (aConsignment._Category.IndexOf("B") >= 0 &&
                    !(aConsignment is SpecialLoad) &&
                    (aConsignment is Consignment))
                {
                    // Substract the consignment weight of same destination and class 
                    if (catg.Equals(aConsignment._Category) && dest.Equals(aConsignment._Dest))
                    {
                        candidates.Add(aConsignment);
                    } // end of BagClass and BagDest comparisson of equality
                }
            }
            return candidates;
        }

        /// <summary>
        /// Find the Baggages whit distributable weight
        /// </summary>
        /// <param name="baggageList">CargoList obj</param>
        /// <param name="catg"> class</param>
        /// <param name="dest"> destination </param>
        /// <returns>ArrayList obj</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private ArrayList FindCandidateBaggages(CargoList baggageList, string catg, string dest)
        {
            ArrayList candidates = new ArrayList();
            // baggage cabin portion
            foreach (Baggage aBaggage in baggageList)
            {
                // To compare every Consignment with every  CandidateHeadNode object of CandidateHead, then decide
                // to put the Consignemnt into candidateList of CandidateHeadNode or not.
                foreach (Consignment aConsignment in aBaggage.Consignments)
                {
                    if (aConsignment._Category.IndexOf("B") >= 0 &&
                        !(aConsignment is SpecialLoad) &&
                        (aConsignment is Consignment))
                    {
                        if (aConsignment._Category.Equals(catg) && aConsignment._Dest.Equals(dest))
                            candidates.Add(aConsignment);
                    }
                }
            }
            return candidates;
        }


        /// <summary>
        /// Distribute baggage weight of baggage weight information  object which belongs to BAG(Final) Panel of pageBagPlan table。
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void BagPlanning()
        {
            if (theFlight == null || theFlight.CabinConfiguration == "") return;

            ArrayList ClassNameArray = FlightUtil.GetClassNamesFromCabinConfiguarion(theFlight.CabinConfiguration);


            // Check every Consignment of every BaggageContainer,
            // set Consignment's weight to 0, if bFlagUserModified is false and not been modified.
            // If it is modified, then add to the gross weight of Baggage Container, but substract it, if underload. 
            // The bFlagUserModified of BaggageContainer shall bot be concerned in weight distribution.

            //InitBaggages(BaggageList);
            //InitConsignments(BagCsgmtList);

            string BagStation;
            string[] flightRoute = theFlight.Route;
            string[] bagCatgList = new string[] { Baggage_BC, Baggage_BY };

            bool transit = theFlight.Pax.PrevAcceptedBagList != null;
            while (true)
            {
                // compute all of the baggage weight
                ClsDstnBagItem[] FinalPaxBagItemList = SumFinalPaxBagItem(transit);

                // Check the every element of FinalPaxBagItemList sequentially to find the Passenger Baggage Information
                for (int i = 0; i < FinalPaxBagItemList.Length; i++)
                {
                    // Get station name of some station
                    BagStation = flightRoute[i + 1];
                    for (int catg = 0; catg < 2; catg++)
                    {
                        string BagClass = bagCatgList[catg];

                        int BagWeight = 0;

                        #region  compute BagWeight

                        // Get the weight of 3 consignment, and first class first.
                        for (int cls = 0; cls < 3; cls++)
                        {
                            // The BagWeight is one of the container's baggage weight of some landing station.
                            // Mapping 3 Classes of Passenger to BC、BY Class of Baggage.
                            if (BagClass == FlightUtil.PaxClassToBagClass(ClassNameArray[cls] as string))
                                BagWeight += FinalPaxBagItemList[i][cls].BagWt;

                        } // end of for (int cls=0)

                        #endregion

                        // If BagWeight in not greater than 0, then ship to next baggage container.
                        if (BagWeight < 0)
                        {
                            throw (new Exception(EWBSCoreWarnings.BagWtLessThanZero));
                        }

                        // Search for the Head Node of desired BagClass and BagStation of candidateHead
                        // Distribute weight of Consignment in candidateList of Head Node by comparing every
                        // CandidateHead, BagClass, and BagStation for each candidateHead Node

                        ArrayList candidates = new ArrayList();

                        candidates.AddRange(FindCandidateBaggages(BaggageList, BagClass, BagStation));
                        candidates.AddRange(FindCandidateConsignments(BagCsgmtList, BagClass, BagStation));

                        for (int candidate = candidates.Count - 1; candidate >= 0; candidate--)
                        {
                            Consignment aConsignment = candidates[candidate] as Consignment;
                            if (aConsignment.bFlagTransit != transit)
                                candidates.RemoveAt(candidate);
                        }

                        BagPlanning(candidates, BagClass, BagStation, BagWeight, transit);
                    } //for each category
                } // end of for (int i=0)

                if (transit == true) transit = false;
                else if (!transit) break;
            }
        }


        /// <summary>
        /// BagPlanning according specified BagClass, BagStation, BagWeight
        /// </summary>
        /// <param name="candidates">an arraylist composed of consignments</param>
        /// <param name="BagClass">BC or BY</param>
        /// <param name="BagStation">bag station</param>
        /// <param name="BagWeight">bag weight</param>
        /// <param name="transit">transit or not</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void BagPlanning(ArrayList candidates, string BagClass, string BagStation, int BagWeight, bool transit)
        {
            ArrayList candidateList = new ArrayList();
            ArrayList candidateConsignments = new ArrayList();

            foreach (Consignment aConsignment in candidates)
            {
                if (aConsignment.ParentULD != null || aConsignment.bFlagUserModified ||
                    (aConsignment.bFlagTransit && aConsignment.Posn != "" && aConsignment.Weight > 0f))
                {
                    if (aConsignment.ParentULD != null) aConsignment.ParentULD.Weight = 0f;
                    candidateList.Add(aConsignment);
                }
                else
                {
                    candidateConsignments.Add(aConsignment);
                    aConsignment.Weight = 0f;
                }
            }

            #region Reserve the transaction data of baggage containder and bulk weight.

            for (int candidate = candidateList.Count - 1; candidate >= 0; candidate--)
            {
                Consignment aConsignment = candidateList[candidate] as Consignment;

                // Baggage weight can not be negative
                if (BagWeight < 0) BagWeight = 0;
                if (aConsignment.bFlagUserModified || aConsignment.bFlagTransit && aConsignment.Weight > 0f)
                {
                    candidateList.RemoveAt(candidate);

                    //keep user's updated weight
                    if (BagWeight >= aConsignment.Weight)
                    {
                        BagWeight -= Convert.ToInt32(aConsignment.Weight);
                    }
                    else
                    {
                        //correct weight
                        aConsignment.Weight = BagWeight;
                        aConsignment.bFlagUserModified = false;
                        BagWeight = 0;
                    }

                    if (aConsignment.Weight == 0f)
                    {
                        aConsignment.bFlagUserModified = false;
                        if (aConsignment.ParentULD == null)
                            candidateConsignments.Add(aConsignment);
                    }
                    else if (aConsignment.Posn != "" && !aConsignment.bFlagUserModified)
                    {
                        candidateConsignments.Add(aConsignment);
                    }
                }
            }

            #endregion

            //Process sorting in between after reserved user updated and before redistribute the rest baggages
            //SortConsignment(candidateList);

            #region Redistribute the rest baggages

            bool bWtGE = true; //Wt>0 first
            while (true)
            {
                foreach (Consignment aConsignment in candidateList)
                {
                    if (bWtGE && aConsignment.Weight == 0 || !bWtGE && aConsignment.Weight > 0) continue;
                    // Baggage weight can not ne negative.
                    if (BagWeight < 0) BagWeight = 0;

                    // If the baggage weight is not over the umloaded weight, then add to Baggage Container.
                    Baggage aBaggage = (aConsignment.ParentULD as Baggage);

                    float underload = CalcUnderLoad(aBaggage) + aConsignment.Weight;
                    if (underload >= BagWeight)
                    {
                        aConsignment.Weight = BagWeight;
                        // Set BagWeight to 0, after all of the baggages are unloaded
                        BagWeight = 0;

                        if (aConsignment.Weight == 0f)
                            aConsignment.Posn = "";
                    }
                    else if (underload > 0)
                    {
                        // get the unloaded baggage weight								
                        aConsignment.Weight = underload;
                        // substract the unloaded baggage weight
                        BagWeight -= Convert.ToInt32(underload);
                    }
                }

                if (bWtGE == true) bWtGE = false;
                else if (!bWtGE) break;
            }

            #endregion

            //distribute to bulk first.
            foreach (Consignment aConsignment in candidateConsignments)
            {
                CargoPosnBase cargoPosn = theFlight.CargoPosnMgr.Find(aConsignment.Posn);
                if (cargoPosn != null)
                {
                    float underload = cargoPosn.MaxGrossWeight - Convert.ToSingle(cargoPosn.getGrossWt());
                    if (BagWeight >= underload)
                    {
                        aConsignment.Weight += underload;
                        BagWeight -= Convert.ToInt32(underload);
                    }
                    else
                    {
                        aConsignment.Weight += BagWeight;
                        BagWeight = 0;
                    }
                }
                if (aConsignment.Weight == 0f)
                    aConsignment.Posn = "";
            }


            if (BagWeight > 0)
            {
                foreach (Consignment aConsignment in candidateConsignments)
                {
                    CargoPosnBase cargoPosn = theFlight.CargoPosnMgr.Find(aConsignment.Posn);
                    if (cargoPosn == null)
                    {
                        aConsignment.Weight += BagWeight;
                        BagWeight = 0;
                    }
                }
            }

            int csgmtCount = 0;
            foreach (Consignment aConsignment in candidates)
            {
                if (aConsignment.ParentULD == null && !aConsignment.bFlagUserModified)
                    csgmtCount += 1;
            }


            //Add one consignment, even the baggage weight is 0.
            if (BagWeight > 0 || csgmtCount == 0)
            {
                Consignment bulkConsignment = new Consignment(theFlight, BagClass, BagStation);
                bulkConsignment.bFlagTransit = transit;
                bulkConsignment.Weight = BagWeight;
                theFlight.BagCsgmtList.Add(bulkConsignment);
                csgmtCount += 1;
            }


            //clear the unused Consignment
            for (int candidate = candidates.Count - 1; candidate >= 0; candidate--)
            {
                Consignment garbage = candidates[candidate] as Consignment;
                if (garbage.Weight == 0f && csgmtCount > 1)
                {
                    garbage.Posn = "";
                    BagCsgmtList.Remove(garbage);
                    csgmtCount -= 1;
                }
            }
        }

        /// <summary>
        /// Calcucate Underload Baggage
        /// </summary>
        /// <param name="aBaggage">Baggage obj</param>
        /// <returns>Underload</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private float CalcUnderLoad(Baggage aBaggage)
        {
            //20060301
            return Math.Min(aBaggage.MaxBagWt, aBaggage.MaxWt) - aBaggage.GWT;
        }

        #endregion

        #region ---- Get AverageBagWeight ----

        /// <summary>
        /// use flight registration number、departure station、landing station as the Key to find out baggage average weight
        /// </summary>
        /// <param name="FromStation">departure station</param>
        /// <param name="ToStation">landing station</param>
        /// <returns>baggage average weight</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private float GetAverageBagWeight(string FromStation, string ToStation)
        {
            
            return BaggageStatistic.Instance.AverageBagWt(FromStation, ToStation, theFlight.ACType.name);
        }

        /// <summary>
        /// Get avwrage baggages
        /// </summary>
        /// <param name="FromStation">departure station</param>
        /// <param name="ToStation">landing station</param>
        /// <returns>avwrage baggages</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private float GetAverageBagPieces(string FromStation, string ToStation)
        {
            return BaggageStatistic.Instance.AverageBagPcs(FromStation, ToStation, theFlight.ACType.name);
        }

        #endregion

        #region ---- Calculate Not-Yet-Check-In Baggage Weight by Station -----

        /// <summary>
        /// Calculate the NotYetCheckIn_baggage weight for each class of each landing stations
        /// NotYetCheckIn_baggage weight in some class = amount of NotYetCheckIn in some class * baggage average weight in some class
        /// </summary>
        /// <returns>Array of not_yet_checkin_baggage information</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnBagItem[] CalNotYetCheckInEachStationBagWeight()
        {
            ClsDstnClassList notYetCheckInPaxList = theFlight.Pax.NotYetCheckInPaxList();
            int len = notYetCheckInPaxList.Length;
            ArrayList aList = new ArrayList();

            // Get departure station name
            string fromStation = theFlight.Route[0];
            for (int i = 0; i < len; i++)
            {
                string toStation = notYetCheckInPaxList[i].Dstn;
                ClsDstnBagItem notYetCheckInBagItem = new ClsDstnBagItem(toStation);
                for (int cls = 0; cls < 3; cls++)
                {

                    notYetCheckInBagItem[cls].BagWt = Convert.ToInt32(
                        // Calculate baggage average weight based on departure_landing stations
                        GetAverageBagWeight(fromStation, toStation) *
                            notYetCheckInPaxList[i][cls]);
                    notYetCheckInBagItem[cls].BagPcs = Convert.ToInt32(
                        // Calculate baggage average weight based on departure_landing stations
                        GetAverageBagPieces(fromStation, toStation) *
                            notYetCheckInPaxList[i][cls]);
                }
                aList.Add(notYetCheckInBagItem);
            }
            return (ClsDstnBagItem[])aList.ToArray(typeof(ClsDstnBagItem));
        }

        #endregion
    }


}