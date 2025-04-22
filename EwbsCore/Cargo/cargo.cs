/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//* THOMAS    | Ver. 04 | #BR18116  | 2018/7/6                                *
//*---------------------------------------------------------------------------*
//* 修正deadload add item 若 uld sn 末兩碼carrier code 為數字開頭的，無法新增 *
//*****************************************************************************
//* THOMAS    | Ver. 03 | #BR17231  | 2018/2/5                                *
//*---------------------------------------------------------------------------*
//* Dest 去掉空白，解決CPM無法正常顯示的問題                                  *
//*****************************************************************************
//* THOMAS    | Ver. 02 | #BR17225  | 2018/1/22                               *
//*---------------------------------------------------------------------------*
//* Deadload 頁面，Cate 與 Dest資料欄位，若有異動顯示紅色，重新取得cargowing  *
//* 資料後，異動的資料改以綠色顯示，未異動的資料則顯示黑色                     *
//*****************************************************************************
//* THOMAS     | Ver. 01 | #BR17161 | 2007/07/11                             *
//*---------------------------------------------------------------------------*
//* PRIO預設為0* ; Deadload頁面  Add bulk時, PRIO不給預設值0
//*****************************************************************************
//* WUPC      | Ver. 00 | #BR071003 | 2007/OCT/04                             *
//*---------------------------------------------------------------------------*
//* Handle the VTS(Version tolerant serialization) of special load            *
//*****************************************************************************
using System;
using System.Collections;
using System.Text;
using EwbsCore.Util;
using FlightDataBase;
using nsBaggage;
using System.Runtime.Serialization;

namespace EWBS
{
    /// <summary>
    /// the basic class of cargoes, such as cargo, baggage, consignment and special loads.
    /// </summary>
    [Serializable]
    public class ICargo
    {
        private string priority = "0"; //Priority  //#BR17161 THOMAS 預設為0
        private string category = "C"; //Category
        private string dest = ""; //Destination
        private float weight = 0; //Weight:  net weight +  baggage weight
        protected string specialHandlingCode = ""; //Special Handling Code
        private string posn = ""; //Position
        private string remark = ""; //Remark
        protected float tareWt = 0; //Tare Weight: decide the Tare Wt by the user input ULD Serial No, 
        private Flight theFlight; //Flight object 
        private ConsignmentList consignments; // all the consignments that belongs to it

        #region Constructor

        /// <summary>
        /// Constructror
        /// </summary>
        /// <param name="theFlight">Flight</param>
        /// <param name="category">Category</param>
        /// <param name="dest">destination</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        protected ICargo(Flight theFlight, string category, string dest)
        {
            //set up the data int the ICargo object 
            this.category = category.ToUpper();
            this.dest = dest.ToUpper();
            this.theFlight = theFlight;

            consignments = new ConsignmentList(this);
        }

        /// <summary>
        /// Construcor
        /// </summary>
        /// <param name="cargo">Cargo used to copy the data from</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        protected ICargo(ICargo cargo)
        {
            this.priority = cargo.priority;
            this.category = cargo.category;
            this.dest = cargo.dest;
            this.weight = cargo.weight;
            this.specialHandlingCode = cargo.specialHandlingCode;
            this.posn = cargo.posn;
            this.remark = cargo.remark;
            this.tareWt = cargo.tareWt;

            this.theFlight = cargo.theFlight;

            consignments = new ConsignmentList(this);
            foreach (ICargo cgo in cargo.consignments)
            {
                //#BR071003 <--
                /****
                if (cgo is Cargo)
                    consignments.Add(new Cargo(cgo as Cargo));
                else if (cgo is Baggage)
                    consignments.Add(new Baggage(cgo as Baggage));
                else if (cgo is Consignment)
                    consignments.Add(new Consignment(cgo as Consignment));
                else if (cgo is SpecialLoad)
                    consignments.Add(new SpecialLoadNew(cgo as SpecialLoad));
                else
                    consignments.Add(new ICargo(cgo as ICargo));
                    ****/
                if (cgo is Baggage)
                    consignments.Add(new Baggage(cgo as Baggage));
                else if (cgo is Cargo)
                    consignments.Add(new Cargo(cgo as Cargo));
                else if (cgo is SpecialLoad)
                    consignments.Add(new SpecialLoad(cgo as SpecialLoad));
                else if (cgo is Consignment)
                    consignments.Add(new Consignment(cgo as Consignment));
                else
                    consignments.Add(new ICargo(cgo as ICargo));
                //#BR071003 -->
            }
        }

        #endregion

        #region Properites

        /// <summary>
        /// get or set priority
        /// </summary>
        public string Priority
        {
            get { return priority; }
            set { priority = value.ToUpper(); }
        }

        /// <summary>
        /// get or set Category
        /// </summary>
        public string _Category
        {
            get { return category; }
            set { category = value.ToUpper(); }
        }

        /// <summary>
        /// get or set Destination
        /// </summary>
        public string _Dest
        {
            get { return dest.Replace("*", "").Trim(); } //#BR17231 Trim空白
            set
            {
                bool transit = this.bFlagTransit;
                dest = value.Replace("*", "").ToUpper().Trim();//#BR17231 Trim空白
                if (transit) dest += "*";
            }
        }

        /// <summary>
        /// get: All of the ICargo's Category of ICargo
        /// set: Category
        /// </summary>
        public string Category
        {
            get { return Strings.ListToString(CollectCategory(new ArrayList())); }
            set { category = value.ToUpper(); }
        }

        /// <summary>
        /// get: All of the ICargo's Destination of ICargo
        /// set: Destination
        /// </summary>
        public string Dest
        {
            get { return Strings.ListToString(CollectDest(new ArrayList())); }
            set { _Dest = value.ToUpper(); }
        }

        /// <summary>
        /// get: All of the ICargo's Category of ICargo
        /// </summary>
        public string DisplayCategory
        {
            get { return Strings.ListToString(CollectCategory(new ArrayList())); }
        }

        /// <summary>
        /// get: display all of the destination of ICargo 
        /// </summary>
        public string DisplayDest
        {
            get { return Strings.ListToString(CollectDest(new ArrayList())); }
        }

        /// <summary>
        /// get: Weight, 
        /// set: If less than zero, then set the value as net weight 
        /// </summary>
        public float Weight // G/Wt from CargoWing
        {
            get { return weight; }
            set
            {
                weight = value < 0 ? 0 : value;
                if (weight < tareWt) weight = tareWt;
                if (this.Category.ToUpper().IndexOf("X") >= 0)
                {
                    weight = tareWt = value;
                }
            }
        }

        /// <summary>
        /// get:  G/Wt from CargoWing
        /// set: Weight
        /// </summary>
        public float UserWeight //G/Wt from CargoWing
        {
            get { return weight; }
            set
            {
                if (value == 0)
                {
                    bflagUserModified = false;
                    bflagCgoWingModified = false;
                    weight = tareWt;
                }
                else if (Weight != value)
                {
                    if (this is Consignment && (this as Consignment).ParentULD is Baggage)
                    {
                        Baggage baggage = (this as Consignment).ParentULD as Baggage;
                        Weight = value;
                        if (baggage.GWT > baggage.MaxWt)
                        {
                            bflagUserModified = false;
                        }
                        else
                            bflagUserModified = true;
                    }
                    else
                    {
                        Weight = value;
                        bflagUserModified = true;
                    }
                }
            }
        }

        /// <summary>
        /// get: tareWt
        /// set: tareWt, if Weight < tareWt, then weight = tareWt
        /// </summary>
        public float TareWt
        {
            get { return tareWt; }
            set
            {
                if (value < 0) value = 0;

                //if it is an empty compartment
                if (this.category.Equals("X") && Weight == tareWt)
                {
                    Weight = tareWt = value;
                }
                else
                {
                    tareWt = value;
                    if (weight < tareWt) Weight = tareWt;
                }
            }
        }

        /// <summary>
        /// get: GWT - tareWt
        /// </summary>
        public float NetWt //Gross Weight-Tare weight
        {
            get { return this.GWT - tareWt; }
        }

        /// <summary>
        /// get: Gross weight:  net weight + baggage weight +Other Special Load
        /// </summary>
        public float GWT // net weight + baggage weight +Other Special Load
        {
            get { return GetWeight(); }
        }

        /// <summary>
        /// get:  get All of the Consignment's Special Handling Code
        /// </summary>
        public string SHC
        {
            get
            {
                if (specialHandlingCode != "") return specialHandlingCode;
                else
                {
                    StringBuilder strSHC = new StringBuilder();
                    foreach (ICargo cargo in this.consignments)
                    {
                        string shc = cargo.SHC;
                        if (shc != "")
                        {
                            if (strSHC.Length > 0) strSHC.Append("," + shc);
                            else strSHC.Append(shc);
                        }
                    }
                    return strSHC.ToString();
                }
            }
        }

        /// <summary>
        /// get: Position, 
        /// set: call CargoPosnMgr.UploadCargo() method.
        /// </summary>
        public string Posn
        {
            get { return posn; }
            set
            {
                value = value.ToUpper();
                CargoPosnBase cgoPosn = theFlight.CargoPosnMgr.Find(value);
                if (cgoPosn != null && cgoPosn.Available)
                {
                    if (theFlight.CargoPosnMgr.UploadCargo(this, cgoPosn))
                    {
                        posn = value;
                    }
                }
                else if (value == "")
                {
                    theFlight.CargoPosnMgr.OffloadCargo(this, posn);
                    posn = "";
                }

            }
        }

        /// <summary>
        /// to set the position
        /// </summary>
        public string _Posn
        {
            set { posn = value.ToUpper(); }
        }

        /// <summary>
        /// Remark
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value.ToUpper(); }
        }

        /// <summary>
        /// get: include ConsignmentList
        /// </summary>
        public ConsignmentList Consignments
        {
            get { return consignments; }
            //read only
            //set { consignments = value; }
        }

        /// <summary>
        /// get: TheFlight object 
        /// </summary>
        public Flight Flight
        {
            get { return theFlight; }
        }

        /// <summary>
        /// UserULDSerialNbr, if it is Cargo object then return ULDSerialNo；if it is Consignment object then return Parent ULDSerialNbr。
        /// </summary>
        public string UserULDSerialNbr
        {
            get
            {
                if (this is Cargo)
                {
                    return (this as Cargo).ULDSerialNo;
                }
                if (this is Consignment)
                {
                    if ((this as Consignment).ParentULD is Cargo)
                        return ((this as Consignment).ParentULD as Cargo).ULDSerialNo;
                }
                return "";
            }
            set
            {
                if (this is Cargo)
                {
                    (this as Cargo).ULDSerialNo = value;
                }
                if (this is Consignment)
                {
                    ((this as Consignment).ParentULD as Cargo).ULDSerialNo = value;
                }

            }
        }

        /// <summary>
        /// if it is Cargo object then return Posn, otherwise return ""。
        /// </summary>
        public String UserUldPosn
        {
            get
            {
                if (this is Cargo)
                    return (this as Cargo).Posn;
                return "";
            }
            set
            {
                if (this is Cargo)
                    (this as Cargo).Posn = value;
            }
        }

        /// <summary>
        /// if it is Cargo object then return HeightCode, otherwise return ""。
        /// </summary>
        public String UserUldHtCode
        {
            get
            {
                if (this is Cargo)
                    return (this as Cargo).HeightCode;
                return "";
            }
            set
            {
                if (this is Cargo)
                    (this as Cargo).HeightCode = value.ToUpper();
            }
        }

        /// <summary>
        /// if it is Cargo object then return BaseCode, otherwise return ""。
        /// </summary>
        public String UserUldBaseCode
        {
            get
            {
                if (this is Cargo)
                    return (this as Cargo).BaseCode;
                return "";
            }
            set
            {
                if (this is Cargo)
                    (this as Cargo).UserUldBaseCode = value.ToUpper();
            }
        }


        /// <summary>
        /// if it is Cargo object then return BaseCode, otherwise return ""。
        /// </summary>
        public String UserUldTpye
        {
            get
            {
                if (this is Cargo)
                    return (this as Cargo).UldType;
                return "";
            }
            set
            {
                if (this is Cargo)
                    (this as Cargo).UldType = value.ToUpper();
            }
        }

        /// <summary>
        /// if it is Cargo object then return GWT, otherwise return ""。
        /// </summary>
        public String DeadLoadGWDisplay
        {
            get
            {
                if (this is Cargo)
                    return Convert.ToString((this as Cargo).GWT);
                return "";
            }
        }

        /// <summary>
        /// if it is Cargo object then return ULDSerialNo, otherwise return ""。
        /// </summary>
        public string DeadLoadULDSerialNbr
        {
            get
            {
                if (this is Cargo)
                {
                    return (this as Cargo).ULDSerialNo;
                }
                return "";
            }
        }

        #endregion

        /// <summary>Create ArrayList of category</summary>
        /// <param name="catgList">the to-be-added category ArrayList</param>
        /// <returns>category ArrayList after added</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private ArrayList CollectCategory(ArrayList catgList)
        {
            if (!(this is Baggage)) // baggage shall be in the lower deck
                catgList.Add(category);
            foreach (ICargo cargo in this.consignments)
            {
                cargo.CollectCategory(catgList);
            }
            return catgList;
        }

        /// <summary>Create ArrayList of destination</summary>
        /// <param name="destList">the to-be-added destination ArrayList</param>
        /// <returns>destination ArrayList after added</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private ArrayList CollectDest(ArrayList destList)
        {
            //No "X", added by weiwang
            if (!(this is Baggage) && _Dest != "X") // baggage shall be in the lower deck
                destList.Add(_Dest);
            foreach (ICargo cargo in this.consignments)
            {
                cargo.CollectDest(destList);
            }
            return destList;
        }

        /// <summary> get gross weight </summary>
        /// <returns>Float gross weight </returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private float GetWeight()
        {
            float ttl = Weight;
            foreach (Consignment csgmt in consignments)
            {
                if (!(csgmt is SpecialLoad) || (csgmt as SpecialLoad).IsOtherSpecialLoad)
                {
                    ttl += csgmt.Weight;
                }
            }
            return ttl;
        }

        /// <summary>
        /// indicating whether this is a transit cargo
        /// </summary>
        public bool bFlagTransit
        {
            get { return dest.IndexOf("*") >= 0; }
            set
            {
                if (value)
                {
                    if (dest.IndexOf("*") < 0) dest += "*";
                }
                else
                {
                    dest = dest.Replace("*", "");
                }
            }
        }

        /// <summary>
        /// bflagUserModified : true, cargo object been modified       
        /// bflagUserModified : false, unmodified cargo object 
        /// </summary>
        private bool bflagUserModified;

        /// <summary>
        /// #BR17225 THOMAS 修改該欄位flag
        /// bflagUserModifiedCatg : true, cargo object been modified       
        /// bflagUserModifiedCatg : false, unmodified cargo object 
        /// </summary>
        private bool bflagUserModifiedCatg;

        /// <summary>
        /// #BR17225 THOMAS 修改該欄位flag
        /// bflagUserModifiedDest : true, cargo object been modified       
        /// bflagUserModifiedDest : false, unmodified cargo object 
        /// </summary>
        private bool bflagUserModifiedDest;

        /// <summary>
        /// bFlagUserModified becomes true if the data is modified
        /// </summary>
        public bool bFlagUserModified
        {
            get
            {
                if (bflagUserModified) return true;
                return false;
            }
            set { bflagUserModified = value; }
        }

        /// <summary>
        /// #BR17225 THOMAS bFlagUserModifiedCatg becomes true if the data is modified
        /// </summary>
        public bool bFlagUserModifiedCatg
        {
            get
            {
                if (bflagUserModifiedCatg) return true;
                return false;
            }
            set { bflagUserModifiedCatg = value; }
        }

        /// <summary>
        /// #BR17225 THOMAS bFlagUserModifiedDest becomes true if the data is modified
        /// </summary>
        public bool bFlagUserModifiedDest
        {
            get
            {
                if (bflagUserModifiedDest) return true;
                return false;
            }
            set { bflagUserModifiedDest = value; }
        }

        private bool bflagCgoWingModified;

        /// <summary>
        /// bFlagUserModified becomes true if the CargoWing is modified
        /// </summary>
        public bool bFlagCgoWingModified
        {
            get { return bflagCgoWingModified; }
            set { bflagCgoWingModified = value; }
        }

        /// <summary></summary>
        /// <remarks></remarks> 
        public virtual void Check()
        {
            //1. Check if the  weight over TareWeight or not
            if (this.Weight < this.TareWt)
            {
                throw (new Exception(EWBSCoreWarnings.CgoWtLessThanTWt));
            }


            string destination = this.Dest;
            //2.Check if the 內含 weight 
            string[] destList = Strings.shortenString(destination).Split(new char[] { ',', ' ' });
            foreach (string dest in destList)
            {
                double netWt, inclWt;
                netWt = this.getNetWt("C", dest);
                inclWt = this.getSpecialLoadWt("C", dest);

                //Check if the special load weight exceed the cargo weight
                if (netWt < inclWt)
                {
                    throw (new Exception(EWBSCoreWarnings.CgoSpeLdExceedCgoWt));
                }
                netWt = this.getNetWt("M", dest);
                inclWt = this.getSpecialLoadWt("M", dest);
                //Check if the special load weight exceed the mail weight
                if (netWt < inclWt)
                {
                    throw (new Exception(EWBSCoreWarnings.CgoSpeLdExceedMailWt));
                }
                netWt = this.getNetWt("B", dest);
                inclWt = this.getSpecialLoadWt("B", dest);

                //Check if the special load weight exceed the baggage weight
                if (netWt < inclWt)
                {
                    throw (new Exception(EWBSCoreWarnings.CgoSpeLdExceedBagWt));
                }
            }

        }

        /// <summary> compute Cargo weight </summary>
        /// <returns>Double weight </returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double calcuCargoWt(string catg, string dest, bool transit, bool grossWt)
        {
            double ttlWt = 0;

            //only consignment and other special load will be calculated.
            if (!(this is SpecialLoad) || (this as SpecialLoad).IsOtherSpecialLoad)
            {
                if (this._Category.Length > 0 && this._Category.Length >= catg.Length && this._Category.Substring(0, catg.Length).ToUpper().Equals(catg) || catg == "")
                {
                    if (this._Dest.ToUpper().Equals(dest.ToUpper()) || dest == "")
                    {
                        if (this.bFlagTransit == transit)
                        {
                            ttlWt += this.Weight;
                            if (!grossWt) ttlWt -= this.TareWt;
                        }
                    }
                }
            }

            //Add all the consignment weights
            foreach (ICargo child in this.Consignments)
            {
                ttlWt += child.calcuCargoWt(catg, dest, transit, grossWt);
            }

            if (this._Dest.Trim() == "" && grossWt && this.TareWt > 0f && dest != "" && this.bFlagTransit == transit)
            {
                string allDest = this.Dest;
                for (int i = theFlight.Route.Length - 1; i > 0; i--)
                {
                    string thisDest = theFlight.Route[i];
                    if (allDest.IndexOf(thisDest) >= 0)
                    {
                        if (dest.Equals(thisDest)) ttlWt += this.TareWt;
                        break;
                    }
                }
            }
            return ttlWt;
        }

        /// <summary> get Net Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <returns>Double：Net Weight</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getNetWt(string catg, string dest)
        {
            return getNetWt(catg, dest, true) + getNetWt(catg, dest, false);
        }

        /// <summary> get Net Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <param name="transit">is transit or not</param>
        /// <returns>Double：Net Weight</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getNetWt(string catg, string dest, bool transit)
        {
            return calcuCargoWt(catg, dest, transit, false);
        }

        /// <summary> get Gross Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <returns>Double：Gross Weight</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getGrossWt(string catg, string dest)
        {
            return getGrossWt(catg, dest, true) + getGrossWt(catg, dest, false);
        }

        /// <summary> get Gross Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <param name="transit">is transit or not</param>
        /// <returns>Double：Gross Weight</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getGrossWt(string catg, string dest, bool transit)
        {
            return calcuCargoWt(catg, dest, transit, true);
        }

        /// <summary> get Special Load Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <returns>Double：Special Load Weight</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getSpecialLoadWt(string catg, string dest)
        {
            return getSpecialLoadWt(catg, dest, true) + getSpecialLoadWt(catg, dest, false);
        }

        /// <summary> get Special Load Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <param name="transit">is transit or not</param>
        /// <returns>Double：Special Load Weight</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getSpecialLoadWt(string catg, string dest, bool transit)
        {
            double wtSpecialLoad = 0f;

            //if this is a speciaload and is not an other special load, add the weight on to wtSpecialLoad
            if ((this is SpecialLoad) && !(this as SpecialLoad).IsOtherSpecialLoad)
            {
                if (this._Category.Length > 0 &&
                    this._Category.Length >= catg.Length &&
                    this._Category.Substring(0, catg.Length).ToUpper().Equals(catg) || catg == "")
                {
                    if (this._Dest.ToUpper().Equals(dest.ToUpper()) || dest == "")
                    {
                        if (this.bFlagTransit == transit)
                        {
                            wtSpecialLoad += Convert.ToDouble(this.weight);
                        }
                    }
                }
            }

            //Add the consignments' special load weight onto the wtSpecialLoad
            foreach (Consignment csgmt in this.consignments)
            {
                wtSpecialLoad += csgmt.getSpecialLoadWt(catg, dest, transit);
            }
            return wtSpecialLoad;
        }

        /// <summary>Put All of the Special Load into one ArrayList</summary>
        /// <param name="rtn">ArrayList</param>
        /// <returns>ArrayList withSpecial Load object </returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ArrayList GetSHCCsgmts(ArrayList rtn)
        {
            if (this is SpecialLoad)
            {
                rtn.Add(this);
            }
            else
            {
                foreach (ICargo child in this.Consignments)
                {
                    child.GetSHCCsgmts(rtn);
                }
            }
            return rtn;
        }

        /// <summary>Set Cargo object as Transit status, and setup the attributes of bFlagUserModified and NewOnBoard</summary>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void TransitCargoProc()
        {
            if (!this.bFlagTransit)
            {
                this.bFlagUserModified = false;
                this.bFlagCgoWingModified = false;
                bFlagTransit = true;
            }
            foreach (ICargo child in this.Consignments)
                child.TransitCargoProc();
        }

    }

    [Serializable]
    public class Cargo : ICargo
    {
        private string uldType = ""; //EVA:3 characters 
        private string serialNo = ""; //EVA:5 characters 
        private string carrierCode = ""; //EVA:2 characters 
        private string htCode = ""; //Height code
        private string baseCode = ""; //Base Code

        [NonSerialized]
        private bool unknownCarrCd = false; //if it is an unknown carrier code
        private bool isContainer = false; //If it is a container

        protected float maxWt; //Maximum weight

        private bool legal = false; //indicated whether it is legal

        /// <summary>
        /// returns legal
        /// </summary>
        public bool Checked
        {
            get { return legal; }
            set { legal = value; }
        }

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight">Flight</param>
        /// <param name="uldType">Uld Type</param>
        /// <param name="carrierCode">Carrier Code</param>
        /// <param name="category">Category</param>
        /// <param name="dest">destination</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Cargo(Flight theFlight, string uldType, string carrierCode, string category, string dest)
            : base(theFlight, category, dest)
        {
            this.uldType = uldType;
            this.carrierCode = carrierCode;

            //refresh base code & tare weight
            BaseCodeNTareWtRefresh();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight">Flight</param>
        /// <param name="uldSerialNo">ULD Serial Number</param>
        /// <param name="category">Category</param>
        /// <param name="dest">destination</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Cargo(Flight theFlight, string uldSerialNo, string category, string dest)
            : base(theFlight, category, dest)
        {
            ULDSerialNo = uldSerialNo;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cargo">Cargo</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Cargo(Cargo cargo)
            : base(cargo)
        {
            //assign the data onto it.
            htCode = cargo.htCode;
            baseCode = cargo.baseCode;
            unknownCarrCd = cargo.unknownCarrCd;
            isContainer = cargo.isContainer;
            maxWt = cargo.maxWt;
            legal = cargo.legal;

            ULDSerialNo = cargo.ULDSerialNo;
        }

        #endregion

        #region Properties

        /// <summary>
        /// indicates if it is a container
        /// </summary>
        public bool IsContainer
        {
            get { return isContainer; }
        }

        /// <summary>
        /// get and set: the Uld serial number = uldType + serialNo + carrierCode
        /// </summary>
        public String ULDSerialNo
        {
            get { return uldType + serialNo + carrierCode; }
            set
            {
                int len = value.Length;

                if (len == 0) return;
                if (len < 3)
                {
                    throw (new Exception(EWBSCoreWarnings.CgoUldSerialNoError));
                }

                value = value.ToUpper();

                #region #BR18116 Thomas 修正deadload add item 若 uld sn 末兩碼carrier code 為數字開頭的，無法新增
                //Split value into uldType, serialNo, and carrierCode 
                //value判斷最後兩碼是否都是數值 bool result = int.TryParse(s, out i);
                //是-->carrCd 為空，以SN處理(判斷字元皆為IsNumber)
                //否-->此兩碼為carrCd，扣除此兩碼後以SN處理(判斷字元皆為IsNumber)
                
                int i_sn = 0;
                string sn = "";
                string carrCd = "";
                bool result = int.TryParse(value.Substring(len - 2, 2).ToString(), out i_sn);
                if (result)
                {
                     carrCd = "";
                     for (int i = 3; i < len; i++)
                     {
                         if (char.IsNumber(value[i]))
                         {
                             sn += value[i];
                         }
                     }
                }
                else
                {
                    if (value.Length > 5)
                    {
                        carrCd = value.Substring(len - 2, 2);
                    }
                    else
                    {
                        carrCd = "";
                    }
        
                     for (int i = 3; i < len-2; i++)
                     {
                         if (char.IsNumber(value[i]))
                         {
                             sn += value[i];
                         }
                     }
                }
                
                #endregion
                
                #region #BR18116 Thomas remark  Split value into uldType, serialNo, and carrierCode 
                /*
                string sn = "";
                string carrCd = "";
                for (int i = 3; i < len; i++)
                {
                    if (char.IsNumber(value[i]))
                    {
                        sn += value[i];
                    }
                    else
                    {
                        carrCd = value.Substring(i, len - i);
                        break;
                    }
                }
              */
                #endregion
                this.uldType = value.Substring(0, 3);//uldType
                this.serialNo = sn; //serialNo
                this.carrierCode = carrCd; //carrierCode
  
                
                this.BaseCodeNTareWtRefresh(); //get baseCode and TareWt
            }
        }

        /// <summary>
        /// get the Uld type
        /// </summary>
        public String UldType
        {
            get { return uldType; }
            set
            {
                if (value.Length != 3) throw (new Exception("ULD Type Error"));

                uldType = value.ToUpper();
                //get baseCode and TareWt
                this.BaseCodeNTareWtRefresh();
            }

        }

        /// <summary>
        /// get the Serial number
        /// </summary>
        public String SerialNo
        {
            get { return serialNo; }
            set
            {
                try
                {
                    Convert.ToInt32(value);
                    serialNo = value;
                    //get baseCode and TareWt
                    this.BaseCodeNTareWtRefresh();
                }
                catch (Exception e)
                {
                    if (value != "") throw (e);
                    serialNo = value;
                }
            }
        }

        /// <summary>
        /// Carrier code
        /// </summary>
        public String CarrierCode
        {
            get { return carrierCode; }
            set
            {
                if (value.Length != 2)
                {
                    throw (new Exception("EWBSCoreWarnings.CarrierCodeError"));
                }
                else
                {
                    carrierCode = value.ToUpper();
                    //get baseCode and TareWt
                    this.BaseCodeNTareWtRefresh();
                }
            }
        }

        /// <summary>
        /// Height Code
        /// </summary>
        public string HeightCode
        {
            get { return htCode.Replace("*", ""); }
            set
            {
                if (this.htCode.IndexOf("*") >= 0)
                {
                    if (value != "")
                        htCode = value.ToUpper().Replace("*", "") + "*";
                    else htCode = "*";
                }
                else
                    htCode = value.ToUpper();
            }
        }

        /// <summary>
        /// indicates whether the Height code was modified by User
        /// </summary>
        public bool bFlagUserModifiedHt
        {
            get { return this.htCode.IndexOf("*") >= 0; }
            set
            {
                if (value && this.htCode.IndexOf("*") < 0) this.htCode += "*";
                if (!value) this.htCode = this.htCode.Replace("*", "");
            }
        }

        /// <summary>
        /// base code
        /// </summary>
        public string BaseCode
        {
            get { return baseCode; }
        }

        /// <summary>
        /// Maximum weight
        /// </summary>
        public float MaxWt
        {
            get { return maxWt; }
        }

        #endregion

        /// <summary>get Base Code, Tare Weight and the Max. restricted weight </summary>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void BaseCodeNTareWtRefresh()
        {
            legal = false;

            float wt = this.Weight;

            if (this.tareWt == wt) wt = 0;
        
            this.baseCode = "";
            this.tareWt = 0;
            this.maxWt = 0;

            unknownCarrCd = false;
            airlinesAirlineUld uld = null;

            try
            {
                if (carrierCode == "") carrierCode = this.Flight.CarrierCode;
                AirlineEx airline = FDB.Instance.GetAirline(carrierCode);
                uld = airline.GetULD(uldType);
            }
            catch
            {
                unknownCarrCd = true;
            }

            if (uld == null)
            {
                // get Airline's data by carrierCode
                uld = FDB.Instance.GetULD(uldType, carrierCode);
                unknownCarrCd = true;
            }

            if (uld == null)
            {
                if (uldType.Length >= 3)
                {
                    this.baseCode = new string(uldType[1], 1); // get baseCode

                    airlinesAirlineUld[] uldInfo = FDB.Instance.GetAirline(carrierCode).uldInfo;
                    foreach (airlinesAirlineUld aULD in uldInfo)
                    {
                        if (aULD.baseCode == this.baseCode)
                        {
                            this.maxWt = aULD.mgw;
                            if (aULD.serialNo != null && aULD.serialNo.Length >= 1)
                            {
                                this.tareWt = aULD.serialNo[0].tareWt;
                                break;
                            }
                        }
                    }
                }
                throw (new Exception(string.Format(EWBSCoreWarnings.UnknownCarrierCodeOrULDType_2, carrierCode, uldType)));
            }
            else
            {
                if (uld.serialNo == null || uld.serialNo.Length == 0)
                {
                    return;
                }
                if (uld.highCode != null)
                {
                    this.HeightCode = uld.highCode;
                }

                this.isContainer = uld.iscontainer;

                this.baseCode = uld.baseCode; // get baseCode
                this.maxWt = uld.mgw; // get Max Gross Weight


                //Check if the Serial No
                int serial = 0;
                try
                {
                    if (unknownCarrCd) throw (new Exception(string.Format(EWBSCoreWarnings.UnknownCarrierCodeOrULDType_2, carrierCode, uldType)));

                    serial = Convert.ToInt32(this.serialNo);

                    string expectedSerialNo = "";

                    foreach (airlinesAirlineUldSerialNo sn in uld.serialNo)
                    {
                        if (sn.min <= serial && serial <= sn.max)
                        {
                            this.tareWt = sn.tareWt; // get tareWt
                            legal = true;
                            break;
                        }
                        expectedSerialNo += string.Format("{0}~{1} ", sn.min, sn.max);
                    }

                    if (tareWt == 0)
                    {
                        throw (new Exception(string.Format(EWBSCoreWarnings.SnExpects_3,
                                                          uldType,
                                                          expectedSerialNo,
                                                          this.serialNo)));
                    }
                }
                catch (Exception)
                {
                    this.tareWt = uld.serialNo[0].tareWt; //default tareWt
                }
            }
            this.Weight = (wt == 0f ? this.tareWt : wt);
        }

        //Is Empty Container
        public bool IsEmptyContainer
        {
            get
            {
                float wt = this.GWT;
                return tareWt == wt && wt > 0;
            }
        }

        /// <summary>Check if the restricted weight 、Serial No. of Cargo object is valid or not, throw exception if invalid</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public override void Check()
        {
            string errMsg = "";

            base.Check();

            if (!unknownCarrCd)
            {
                try
                {
                    FDB.Instance.GetAirline(carrierCode);
                }
                catch
                {
                    unknownCarrCd = true;
                }


                // get Airline's data from carrierCode
                airlinesAirlineUld uld = FDB.Instance.GetULD(uldType, carrierCode);
                if (uld == null)
                {
                    throw (new Exception(string.Format(EWBSCoreWarnings.UldNotFound_1, uldType)));
                }

                //Check if the weight over restricted weight or not
                if (this.GWT > this.MaxWt)
                {
                    errMsg += string.Format(EWBSCoreWarnings.CgoWtExceedsMax_1, this.MaxWt);
                }

                //shall have HtCode
                if (this.baseCode.IndexOfAny("MAGR".ToCharArray()) >= 0 && this.htCode == "")
                {
                    errMsg += string.Format(EWBSCoreWarnings.HeightCdShouldBeEnter_1, this.UldType);
                }

                //Check if the Serial No
                if (this.serialNo != "")
                {
                    if (unknownCarrCd)
                    {
                        legal = true;
                        errMsg += string.Format(EWBSCoreWarnings.UnknownSn_1, serialNo);
                    }
                    else if (!legal)
                    {
                        string expectedSerialNo = "";

                        int serial = Convert.ToInt32(serialNo);

                        //checking if the serial number is within the upper and lower limits
                        foreach (airlinesAirlineUldSerialNo sn in uld.serialNo)
                        {
                            if (sn.min <= serial && serial <= sn.max)
                            {
                                legal = true;
                                break;
                            }

                            if (expectedSerialNo != "") expectedSerialNo += ", ";
                            expectedSerialNo += string.Format("{0}~{1} ", sn.min, sn.max);
                        }
                        if (!legal)
                        {
                            legal = true;

                            //add a message of exception
                            errMsg += string.Format(EWBSCoreWarnings.SnExpects_3,uldType,expectedSerialNo,this.serialNo);
                        }
                    }
                }
            }
            if (errMsg != "") throw (new Exception(errMsg));
        }

        public bool LegalCarrierCode
        {
            get { return !unknownCarrCd; }
        }
    }


    /// <summary>
    /// Class for Bagagge manipulations
    /// </summary>
    [Serializable]
    public class Baggage : Cargo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight">Flight</param>
        /// <param name="uldType">Uld Type</param>
        /// <param name="carrierCode">Carrier code</param>
        /// <param name="category">Category</param>
        /// <param name="dest">destination</param>
        /// <param name="maxBagWt">Maximum baggage weight</param>
        public Baggage(Flight theFlight, string uldType, string carrierCode, string category, string dest, int maxBagWt)
            : base(theFlight, uldType, carrierCode, category, dest)
        {
            this.maxBagWt = maxBagWt;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight">Flight</param>
        /// <param name="uldSerialNo">Uld Serial Number</param>
        /// <param name="category">Category</param>
        /// <param name="dest">destination</param>
        /// <param name="maxBagWt">Maximum baggage weight</param>
        public Baggage(Flight theFlight, string uldSerialNo, string category, string dest, int maxBagWt)
            : base(theFlight, uldSerialNo, category, dest)
        {
            this.maxBagWt = maxBagWt;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cargo">Baggage obj to provide its weight data</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Baggage(Baggage cargo)
            : base(cargo)
        {
            this.maxBagWt = cargo.maxBagWt;
        }

        private int maxBagWt = 0; //Maximum baggage weight


        /// <summary>
        /// Maximum baggage weight
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int MaxBagWt
        {
            get { return maxBagWt; }
            set { maxBagWt = value; }
        }

        /// <summary> get Baggage Rule</summary>
        /// <returns>BagRule object </returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagRule GetBagRule()
        {
            string catg = this.Category + ",";
            catg = catg.Replace("O,", "");
            catg = catg.Substring(0, catg.Length - 1);

            return new BagRule(this.UldType, 1,
                               this.Dest, catg, this.MaxBagWt, this.bFlagTransit);
        }

        /// <summary>
        /// Check if this cargo is mixed of more than one destination.
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsMixDestCategory
        {
            get
            {
                //For each sonsignments in this cargo, if there are more than two destinations, return true.
                for (int i = 0; i < this.Consignments.Count; i++)
                {
                    Consignment csgn1 = this.Consignments[i] as Consignment;
                    for (int j = i; j < this.Consignments.Count; j++)
                    {
                        Consignment csgn2 = this.Consignments[j] as Consignment;
                        if (csgn1.Dest != csgn2.Dest || csgn1.Category != csgn2.Category)
                            return true;
                    }
                }
                return false;
            }
        }


    }


    /// <summary>
    /// Class used for manipulating Consignment data
    /// </summary>
    [Serializable]
    public class Consignment : ICargo
    {
        private string airWayBillNo = ""; // Airway bill number
        private int pieces = 0; //pieces

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight">Flight</param>
        /// <param name="category">Category</param>
        /// <param name="dest">destination</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Consignment(Flight theFlight, string category, string dest) :
            base(theFlight, category, dest)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="other">other consignment for retrieving data</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Consignment(Consignment other)
            : base(other)
        {
            airWayBillNo = other.airWayBillNo;
            pieces = other.pieces;
        }

        #endregion

        private ICargo belongToContainer; //the container it belongs to 

        /// <summary>
        /// to return the belongToContainer
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ICargo ParentULD
        {
            get { return belongToContainer; }
            //Read only
            //been modified to SetContainer(), do not misuse
            //Disabled by weiwang
            //set { belongToContainer = value; }
        }

        //set Container
        //been modified to auto setting, do not misuse
        /// <summary>Set the Consignment to the desired Container</summary>
        /// <param name="theContainer">ICargo object</param>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void SetContainer(ICargo theContainer)
        {
            belongToContainer = theContainer;
        }

        /// <summary>
        /// the ULD serial number of its parent ULD
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string ParentULDSerialNbr
        {
            get
            {
                // if the parent uld is a cargo, then obtain its serial number
                if (belongToContainer is Cargo)
                {
                    if ((belongToContainer as Cargo).SerialNo != "")
                        return (belongToContainer as Cargo).ULDSerialNo;
                }
                // if the parent uld is a Consignment, then obtain its AWB number
                if (belongToContainer is Consignment)
                {
                    return (belongToContainer as Consignment).AWB;
                }
                // else, return the position of it.
                return this.ParentPosn;
            }
        }

        /// <summary>
        /// the ULD type of its parent ULD
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public String ParentUldType
        {
            get
            {
                if (belongToContainer is Cargo)
                    return (belongToContainer as Cargo).UldType;
                return "";
            }
        }

        /// <summary>
        /// the serial number of its parent ULD
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public String ParentSerialNo
        {
            get
            {
                if (belongToContainer is Cargo)
                    return (belongToContainer as Cargo).SerialNo;
                return "";
            }
            set
            {
                // baggage used by dispalying screen
                if (belongToContainer is Cargo)
                    (belongToContainer as Cargo).SerialNo = value;
            }

        }

        /// <summary>
        /// the carrier code of its parent ULD
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public String ParentCarrierCode
        {
            get
            {
                if (belongToContainer is Cargo)
                    return (belongToContainer as Cargo).CarrierCode;
                return "";
            }
            set
            {
                // baggage used by dispalying screen
                if (belongToContainer is Cargo)
                    (belongToContainer as Cargo).CarrierCode = value;
            }
        }

        /// <summary>
        /// the gross weight of its parent ULD
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float ParentGWT // net weight + baggage weight+Other Special Load
        {
            get
            {
                if (belongToContainer is Cargo) return (belongToContainer as Cargo).GWT;
                return GWT;
            }
        }

        /// <summary>
        /// the tare weight of its parent ULD
        /// </summary>
        public float ParentTareWt // net weight 
        {
            get
            {
                if (belongToContainer is Cargo) return (belongToContainer as Cargo).TareWt;
                return 0f;
            }
            set
            {
                if (belongToContainer is Cargo)
                {
                    (belongToContainer as Cargo).TareWt = value;
                }
            }
        }

        /// <summary>
        /// the position of its parent ULD
        /// </summary>
        public String ParentPosn
        {
            get
            {
                if (belongToContainer is Cargo)
                    return (belongToContainer as Cargo).Posn;
                return this.Posn;
            }
            set
            {
                if (belongToContainer is Cargo)
                    (belongToContainer as Cargo).Posn = value;
            }
        }

        /// <summary>
        /// airway bill number
        /// </summary>
        public string AWB
        {
            get { return airWayBillNo; }
            set { airWayBillNo = value.ToUpper(); }
        }

        /// <summary>
        /// pieces
        /// </summary>
        public int Pieces
        {
            get { return pieces; }
            set { pieces = value; }
        }
    }

    /// <summary>
    /// Summary description for SpecialHandlingCode.
    /// </summary>
    [Serializable]
    public class SpecialLoad : Consignment
    {
        private bool isOtherSpecialLoad = false; //whether it is an other special load
        //#BR071003 <-- Required information for DG / SL
        [NonSerialized]
        private string strSLcontent;
        [NonSerialized]
        private string strSLsupple;
        [NonSerialized]
        private string strSLtemp;
        [NonSerialized]
        private string strSLpos;
        [NonSerialized]
        private string strDGship;
        [NonSerialized]
        private string strDGid;
        [NonSerialized]
        private string strDGclass;
        [NonSerialized]
        private string strDGpack;
        [NonSerialized]
        private string strDGcatalog;
        [NonSerialized]
        private string strDGpos;
        [NonSerialized]
        private string strDGcode;
        [NonSerialized]
        private string strDGrisk;
        [NonSerialized]
        private string strDGcao;
        [NonSerialized]
        private string strDGicao;
        [NonSerialized]
        private string strDGprint;
        [NonSerialized]
        private string strSPprint;
        //#BR071003 -->

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight">Flight</param>
        /// <param name="category">Category</param>
        /// <param name="dest">destination</param>
        /// <param name="shc">special handling code</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public SpecialLoad(Flight theFlight, string category, string dest, string shc)
            : base(theFlight, category, dest)
        {
            specialHandlingCode = shc;

            if (this._Category.Equals("O")) isOtherSpecialLoad = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="other">other special load</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public SpecialLoad(SpecialLoad other)
            : base(other)
        {
            //#BR071003 <-- Required information for DG / SL
            //isOtherSpecialLoad = other.isOtherSpecialLoad;
            IsOtherSpecialLoad = other.IsOtherSpecialLoad;
            strSLcontent = other.StrSLcontent;
            strSLsupple = other.StrSLsupple;
            strSLtemp = other.StrSLtemp;
            strSLpos = other.StrSLpos;
            strDGship = other.StrDGship;
            strDGid = other.StrDGid;
            strDGclass = other.StrDGclass;
            strDGpack = other.StrDGpack;
            strDGcatalog = other.StrDGcatalog;
            strDGpos = other.StrDGpos;
            strDGcode = other.StrDGcode;
            strDGrisk = other.StrDGrisk;
            strDGcao = other.StrDGcao;
            strDGicao = other.StrDGicao;
            strDGprint = "V";
            strSPprint = "V";
            //#BR071003 -->
        }

        /// <summary>
        /// whether it is an Other special load
        /// </summary>
        public bool IsOtherSpecialLoad
        {
            get { return isOtherSpecialLoad; }
            //#BR071003 <-- Required information for DG / SL
            //set { isOtherSpecialLoad = value; }
            set { isOtherSpecialLoad = value; }
            //#BR071003 -->
        }

        //#BR071003 <-- Required information for DG / SL
        /// <summary>
        /// Be printed out on NOTOC or not
        /// </summary>
        public string StrSPprint
        {
            get { return strSPprint; }
            set { strSPprint = value.ToUpper(); }
        }

        /// <summary>
        /// Be printed out on NOTOC or not
        /// </summary>
        public string StrDGprint
        {
            get { return strDGprint; }
            set { strDGprint = value.ToUpper(); }
        }

        /// <summary>
        /// Contents and Description for the special load
        /// </summary>
        public string StrSLcontent
        {
            get { return strSLcontent; }
            set { strSLcontent = value.ToUpper(); }
        }

        /// <summary>
        /// Supplementary Information for the special load
        /// </summary>
        public string StrSLsupple
        {
            get { return strSLsupple; }
            set { strSLsupple = value.ToUpper(); }
        }

        /// <summary>
        /// Temp. requirement for the special load
        /// </summary>
        public string StrSLtemp
        {
            get { return strSLtemp; }
            set { strSLtemp = value.ToUpper(); }
        }

        /// <summary>
        /// Special load position
        /// </summary>
        public string StrSLpos
        {
            get { return strSLpos; }
            set { strSLpos = value.ToUpper(); }
        }

        /// <summary>
        /// Shipping name of the Dangerous goods
        /// </summary>
        public string StrDGship
        {
            get { return strDGship; }
            set { strDGship = value.ToUpper(); }
        }

        /// <summary>
        /// UN or ID No of Dangerous goods
        /// </summary>
        public string StrDGid
        {
            get { return strDGid; }
            set { strDGid = value.ToUpper(); }
        }

        /// <summary>
        /// Dangerous goods class
        /// </summary>
        public string StrDGclass
        {
            get { return strDGclass; }
            set { strDGclass = value.ToUpper(); }
        }

        /// <summary>
        /// UN packing group of Dangerous goods
        /// </summary>
        public string StrDGpack
        {
            get { return strDGpack; }
            set { strDGpack = value.ToUpper(); }
        }

        /// <summary>
        /// Radioactive Mater. Categ. of Dangerous goods
        /// </summary>
        public string StrDGcatalog
        {
            get { return strDGcatalog; }
            set { strDGcatalog = value.ToUpper(); }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGpos
        {
            get { return strDGpos; }
            set { strDGpos = value.ToUpper(); }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGcode
        {
            get { return strDGcode; }
            set { strDGcode = value.ToUpper(); }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGrisk
        {
            get { return strDGrisk; }
            set { strDGrisk = value.ToUpper(); }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGcao
        {
            get { return strDGcao; }
            set { strDGcao = value.ToUpper(); }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGicao
        {
            get { return strDGicao; }
            set { strDGicao = value.ToUpper(); }
        }

        /// <summary>
        /// Setup non-serialized attributes
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void SetupExtAttr()
        {
            if (strSLcontent == null) strSLcontent = "";
            if (strSLsupple == null) strSLsupple = "";
            if (strSLtemp == null) strSLtemp = "";
            if (strSLpos == null) strSLpos = "";
            if (strDGship == null) strDGship = "";
            if (strDGid == null) strDGid = "";
            if (strDGclass == null) strDGclass = "";
            if (strDGpack == null) strDGpack = "";
            if (strDGcatalog == null) strDGcatalog = "";
            if (strDGpos == null) strDGpos = "";
            if (strDGcode == null) strDGcode = "";
            if (strDGrisk == null) strDGrisk = "";
            if (strDGcao == null) strDGcao = "";
            if (strDGicao == null) strDGicao = "";
            if (strDGprint == null) strDGprint = "";
            if (strSPprint == null) strSPprint = "";
        }

    }

    /// <summary>
    /// Summary description for SpecialHandlingCode.
    /// </summary>
    [Serializable]
    public class SpecialLoadExt : ISerializable
    {
        private string strSLcontent = "";
        private string strSLsupple = "";
        private string strSLtemp = "";
        private string strSLpos = "";
        private string strDGship = "";
        private string strDGid = "";
        private string strDGclass = "";
        private string strDGpack = "";
        private string strDGcatalog = "";
        private string strDGpos = "";
        private string strDGcode = "";
        private string strDGrisk = "";
        private string strDGcao = "";
        private string strDGicao = "";
        private SpecialLoad ownerSL = null;

        /// <summary>
        /// Called by Deserialization
        /// </summary>
        protected SpecialLoadExt(SerializationInfo info, StreamingContext context)
        {
            strSLcontent = info.GetString("strSLcontent");
            strSLsupple = info.GetString("strSLsupple");
            strSLtemp = info.GetString("strSLtemp");
            strSLpos = info.GetString("strSLpos");
            strDGship = info.GetString("strDGship");
            strDGid = info.GetString("strDGid");
            strDGclass = info.GetString("strDGclass");
            strDGpack = info.GetString("strDGpack");
            strDGcatalog = info.GetString("strDGcatalog");
            strDGpos = info.GetString("strDGpos");
            strDGcode = info.GetString("strDGcode");
            strDGrisk = info.GetString("strDGrisk");
            strDGcao = info.GetString("strDGcao");
            strDGicao = info.GetString("strDGicao");
            ownerSL = (SpecialLoad)info.GetValue("ownerSL", typeof(SpecialLoad));
        }

        /// <summary>
        /// Called by Serialization
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("strSLcontent", strSLcontent);
            info.AddValue("strSLsupple", strSLsupple);
            info.AddValue("strSLtemp", strSLtemp);
            info.AddValue("strSLpos", strSLpos);
            info.AddValue("strDGship", strDGship);
            info.AddValue("strDGid", strDGid);
            info.AddValue("strDGclass", strDGclass);
            info.AddValue("strDGpack", strDGpack);
            info.AddValue("strDGcatalog", strDGcatalog);
            info.AddValue("strDGpos", strDGpos);
            info.AddValue("strDGcode", strDGcode);
            info.AddValue("strDGrisk", strDGrisk);
            info.AddValue("strDGcao", strDGcao);
            info.AddValue("strDGicao", strDGicao);
            info.AddValue("ownerSL", ownerSL);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SpecialLoadExt(SpecialLoad theSL)
        {
            strSLcontent = theSL.StrSLcontent;
            strSLsupple = theSL.StrSLsupple;
            strSLtemp = theSL.StrSLtemp;
            strSLpos = theSL.StrSLpos;
            strDGship = theSL.StrDGship;
            strDGid = theSL.StrDGid;
            strDGclass = theSL.StrDGclass;
            strDGpack = theSL.StrDGpack;
            strDGcatalog = theSL.StrDGcatalog;
            strDGpos = theSL.StrDGpos;
            strDGcode = theSL.StrDGcode;
            strDGrisk = theSL.StrDGrisk;
            strDGcao = theSL.StrDGcao;
            strDGicao = theSL.StrDGicao;
            ownerSL = theSL;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public void setSLExt()
        {
            if (StrSLcontent != null) ownerSL.StrSLcontent = StrSLcontent;
            if (StrSLsupple != null) ownerSL.StrSLsupple = StrSLsupple;
            if (StrSLtemp != null) ownerSL.StrSLtemp = StrSLtemp;
            if (StrSLpos != null) ownerSL.StrSLpos = StrSLpos;
            if (StrDGship != null) ownerSL.StrDGship = StrDGship;
            if (StrDGid != null) ownerSL.StrDGid = StrDGid;
            if (StrDGclass != null) ownerSL.StrDGclass = StrDGclass;
            if (StrDGpack != null) ownerSL.StrDGpack = StrDGpack;
            if (StrDGcatalog != null) ownerSL.StrDGcatalog = StrDGcatalog;
            if (StrDGpos != null) ownerSL.StrDGpos = StrDGpos;
            if (StrDGcode != null) ownerSL.StrDGcode = StrDGcode;
            if (StrDGrisk != null) ownerSL.StrDGrisk = StrDGrisk;
            if (StrDGcao != null) ownerSL.StrDGcao = StrDGcao;
            if (StrDGicao != null) ownerSL.StrDGicao = StrDGicao;
        }

        /// <summary>
        /// Contents and Description for the special load
        /// </summary>
        public string StrSLcontent
        {
            get { return strSLcontent; }
            set { strSLcontent = value; }
        }

        /// <summary>
        /// Supplementary Information for the special load
        /// </summary>
        public string StrSLsupple
        {
            get { return strSLsupple; }
            set { strSLsupple = value; }
        }

        /// <summary>
        /// Temp. requirement for the special load
        /// </summary>
        public string StrSLtemp
        {
            get { return strSLtemp; }
            set { strSLtemp = value; }
        }

        /// <summary>
        /// Special load position
        /// </summary>
        public string StrSLpos
        {
            get { return strSLpos; }
            set { strSLpos = value; }
        }

        /// <summary>
        /// Shipping name of the Dangerous goods
        /// </summary>
        public string StrDGship
        {
            get { return strDGship; }
            set { strDGship = value; }
        }

        /// <summary>
        /// UN or ID No of Dangerous goods
        /// </summary>
        public string StrDGid
        {
            get { return strDGid; }
            set { strDGid = value; }
        }

        /// <summary>
        /// Dangerous goods class
        /// </summary>
        public string StrDGclass
        {
            get { return strDGclass; }
            set { strDGclass = value; }
        }

        /// <summary>
        /// UN packing group of Dangerous goods
        /// </summary>
        public string StrDGpack
        {
            get { return strDGpack; }
            set { strDGpack = value; }
        }

        /// <summary>
        /// Radioactive Mater. Categ. of Dangerous goods
        /// </summary>
        public string StrDGcatalog
        {
            get { return strDGcatalog; }
            set { strDGcatalog = value; }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGpos
        {
            get { return strDGpos; }
            set { strDGpos = value; }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGcode
        {
            get { return strDGcode; }
            set { strDGcode = value; }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGrisk
        {
            get { return strDGrisk; }
            set { strDGrisk = value; }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGcao
        {
            get { return strDGcao; }
            set { strDGcao = value; }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public string StrDGicao
        {
            get { return strDGicao; }
            set { strDGicao = value; }
        }

        /// <summary>
        /// Dangerous goods position
        /// </summary>
        public SpecialLoad OwnerSL
        {
            get { return ownerSL; }
            set { ownerSL = value; }
        }
        //#BR071003 -->
    }
}