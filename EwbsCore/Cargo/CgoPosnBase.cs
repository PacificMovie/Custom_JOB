/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//*Thomas| Ver. 03| #BR17164 | 2017/8/31                                                                      *
//*---------------------------------------------------------------------------*
//* 物件關閉後 Dispose 標記由GC回收     
//*******************************************************************************
//**********************************************************************************
//* Thomas   | Ver. 02 | #BR17202 | 2017/07/28                                                                        *
//*-------------------------------------------------------------------------------- *
//*  777貨機MD / LD垂直 載重限制  ，MD 左右兩邊重量加總平衡計算， 左右平衡計算                                                                                                                                                      *
//*****************************************************************************
//* Thomas   | Ver. 01 | #BR17201 | 2017/07/28                                                              *
//*--------------------------------------------------------------------------- *
//* LOAD PLAN頁可裝載CENTER LOAD - Center Row 繪圖                                                                                                                   *
//*****************************************************************************
//* WUPC      | Ver. 00 | #BR071004 | 2007/OCT/04                             *
//*---------------------------------------------------------------------------*
//* To add textboxes for user to input ULD's offloading site                  *
//*****************************************************************************
//using System.Drawing;
//using System.Windows.Forms;
using System;
using System.Collections;
using System.Drawing;
using EwbsCore.Util;
using FlightDataBase;
using System.IO;
using System.Xml;

namespace EWBS
{

    ///#BR071004 <--
    /// <summary>
    /// Y coordinate and Height
    /// </summary>
    public class YnH
    {

        private int y_Coor;
        private int height;
        private int dY;
        private int dH;
        private int x_Coor;
        private int width;
        //private CargoPosnBase posn;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tmpY"></param>
        /// <param name="tmpH"></param>
        public YnH(int tmpY, int tmpH, int tmpX, int tmpW, int tmpDY, int tmpDH)
        {
            y_Coor = tmpY;
            height = tmpH;
            x_Coor = tmpX;
            width = tmpW;
            dY = tmpDY;
            dH = tmpDH;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tmpY"></param>
        /// <param name="tmpH"></param>
        public YnH(int tmpY, int tmpH)
        {
            y_Coor = tmpY;
            height = tmpH;
        }

        /// <summary>
        /// Get/Set y_Coor
        /// </summary>
        public int Y_Coor
        {
            get { return y_Coor; }
            set { y_Coor = value; }
        }

        /// <summary>
        /// Get/Set height
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Get/Set Y cooridnation variation
        /// </summary>
        public int DY
        {
            get { return dY; }
            set { dY = value; }
        }

        /// <summary>
        /// Get/Set height variation
        /// </summary>
        public int DH
        {
            get { return dH; }
            set { dH = value; }
        }

        /// <summary>
        /// Get/Set x_Coor
        /// </summary>
        public int X_Coor
        {
            get { return x_Coor; }
            set { x_Coor = value; }
        }

        /// <summary>
        /// Get/Set width
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
    }
    //#BR071004 -->

    /// <summary>
    /// Geometry Coordinate
    /// </summary>
    public class BaseCode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="other">Base code data from FDB</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BaseCode(CargoPositionHoldCmpBayPosBaseCode other)
        {
            name = other.name;
            maxWt = other.maxWt;
            index = other.index;
        }

        private string name; // Name

        private int maxWt; // Maximum weight

        private float index; //index

        /// <summary>
        /// get: name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Maximum weight
        /// </summary>
        public int MaxWt
        {
            get { return maxWt; }
        }

        /// <summary>
        /// get: index
        /// </summary>
        public float Index
        {
            get { return index; }
        }
    }

    /// <summary>
    /// a CargoList used for positions
    /// </summary>
    public class PCargoList : ArrayList
    {
        private CargoPosnBase posn; //position

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="posn">the position this Cargo List belongs to</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public PCargoList(CargoPosnBase posn)
        {
            this.posn = posn;
        }


        /// <summary>
        /// get: ArrayList index
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        ///<--  隱藏繼承的成員，加入new 關鍵字-->
        public new ICargo this[int index]
        {
            get { return (ICargo)base[index]; }
        }
    }

    /// <summary>
    /// a base for Cargo positions
    /// </summary>
    public class CargoPosnBase : ArrayList
    {
        //position name 
        private string name;
        //Available or not
        private bool available;
        //Max Gross Weight
        private int maxGrossWeight; //is changed as Base Code change
        private float indexPerKg = float.NegativeInfinity; //is changed as Base Code change
        //Cargo array
        private PCargoList cargoList;
        private Geom geom; //Geometry Coordinate
        private string heightCode = "";
        private CargoPosnBase myParent; //record the Parent

        private BaseCode[] baseCode = new BaseCode[0]; //the base codes
        public string specialbaseCode; //Base codes for special loads
        public int cmpWidth = 0; //Compartment width

        private bool nofit = false; //No fit

        private YnH adjRect; //#BR071004 Adjusted Rectangle information

        private Geom default_geom; //default Geometry Coordinate

        public string ShcExclusive = ""; //the exclusive SHCs

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
         public CargoPosnBase()
        {
            cargoList = new PCargoList(this);
        }

        #endregion

        #region Properties
        ///#BR071004 <--
        /// <summary>
        /// Adjusted Rectangle 
        /// </summary>
        public YnH AdjRect
        {
            get { return adjRect; }
            set { adjRect = value; }
        }
        //#BR071004 -->

        /// <summary>
        /// position name 
        /// </summary>
        public string Name
        {
            get
            {
                int idx = name.IndexOf(",");
                if (idx >= 0)
                {
                    return name.Substring(0, idx).Trim();
                }
                return name;
            }
            set { name = value; }
        }

        /// <summary>
        /// Maximum gross weight
        /// </summary>
        public int MaxGrossWeight //is changed as Base Code change
        {
            get { return maxGrossWeight; }
            set { maxGrossWeight = value; }
        }

        /// <summary>
        /// Height code
        /// </summary>
        public string HtCode
        {
            get { return (this.heightCode == null ? "" : this.heightCode); }
            set { this.heightCode = value; }
        }

        /// <summary>
        /// Parent position
        /// </summary>
        public CargoPosnBase Parent
        {
            get { return this.myParent; }
        }

        /// <summary>
        /// Geometry Coordinate
        /// </summary>
        public Geom Geom
        {
            get { return geom; }
            set
            {
                geom = value;
                if (default_geom == null)
                {
                    default_geom = new Geom(geom.x, geom.y, geom.w, geom.h);
                }
            }
        }

        /// <summary>
        /// default  的Geometry Coordinate
        /// </summary>
        public Geom Default_Geom
        {
            get { return default_geom; }
        }


        /// <summary>
        /// Set this position and all of it's child position Available
        /// </summary>
        public void UpdateAllAvailable(bool value)
        {
            available = value;
            foreach (CargoPosnBase posnBase in this)
                posnBase.UpdateAllAvailable(value);
        }

        /// <summary>
        /// Offload unavailable Cargo
        /// </summary>
        public void OffladUnavailableCargo()
        {
            if (available == false)
            {
                for (int i = cargoList.Count; i-- > 0; )
                {
                    cargoList[i].Posn = "";
                }
            }
            foreach (CargoPosnBase posnBase in this)
                posnBase.OffladUnavailableCargo();
        }

        /// <summary>
        /// off load all the Cargo
        /// </summary>
        /// <param name="clearOnly">if the action only wants to clear the objects in the list</param>
        public void OffloadAllCargo(bool clearOnly)
        {
            if (clearOnly)
            {
                //clear only
                cargoList.Clear();
            }
            else
            {
                //offload
                for (int i = cargoList.Count; i-- > 0; )
                {
                    cargoList[i].Posn = "";
                }
            }
            foreach (CargoPosnBase posnBase in this)
                posnBase.OffloadAllCargo(clearOnly);
        }


        /// <summary>
        /// Set this position Available
        /// </summary>
        public bool Available
        {
            get
            {
                if (!available) return false;

                //upperOccupied
                foreach (CargoPosnBase child in this)
                {
                    if (child.lowerOccupied()) return false;
                }

                if ((this is CargoPosn) && myParent != null)
                {
                    int idx = myParent.IndexOf(this);

                    if (idx < 2)
                    {
                        idx += 2;
                        if (this.IsMainDeckPosn())
                        {
                            if (idx < myParent.Count)
                            {
                                if ((myParent[idx] as CargoPosnBase).cargoList.Count > 0)
                                    return false;
                            }

                            CargoPosnBase prevCmpt = myParent.Prev();
                            if (prevCmpt != null)
                            {
                                if (prevCmpt != null &&
                                    idx < prevCmpt.Count &&
                                    ((CargoPosnBase)prevCmpt[idx]).cargoList.Count > 0)
                                    return false;
                            }
                        }

                    }
                    else
                    {
                        idx -= 2;
                        if (this.IsMainDeckPosn())
                        {
                            if (idx < myParent.Count)
                            {
                                if ((myParent[idx] as CargoPosnBase).cargoList.Count > 0)
                                    return false;
                            }

                            CargoPosnBase nextCmpt = myParent.Next();
                            if (nextCmpt != null)
                            {
                                if (nextCmpt != null &&
                                    idx < nextCmpt.Count &&
                                    ((CargoPosnBase)nextCmpt[idx]).cargoList.Count > 0)
                                    return false;
                            }
                        }

                    }
                }
                else if (AreAllChildrenLeaf() && this.IsMainDeckPosn())
                {
                    CargoPosnBase prevCmpt = this.Prev();
                    int idx;
                    if (prevCmpt != null)
                    {
                        idx = 2; //CDL
                        if (idx < prevCmpt.Count &&
                            ((CargoPosnBase)prevCmpt[idx]).cargoList.Count > 0)
                            return false;
                        idx = 3; //CDR
                        if (idx < prevCmpt.Count &&
                            ((CargoPosnBase)prevCmpt[idx]).cargoList.Count > 0)
                        idx = 4; //CD
                        if (idx < prevCmpt.Count &&
                            ((CargoPosnBase)prevCmpt[idx]).cargoList.Count > 0)
                            return false;
                    }
                }
                return true;
            }
            set { available = value; }
        }

        /// <summary>
        /// if it is a leaf position
        /// </summary>
        public bool isLeaf
        {
            get { return Count == 0; }
        }

        /// <summary>
        /// if the lower deck is occupied
        /// </summary>
        /// <returns></returns>
        private bool lowerOccupied()
        {
            if (cargoList.Count > 0) return true;
            //look for child if occupied
            foreach (CargoPosnBase child in this)
            {
                if (child.lowerOccupied()) return true;
            }
            return false;
        }

        /// <summary>
        /// if it is occupied
        /// </summary>
        public bool Occupied
        {
            get { return cargoList.Count > 0; }
        }

        /// <summary>
        /// Set the cargoList of this position
        /// </summary>
        public PCargoList CargoList
        {
            get { return cargoList; }
        }


        /// <summary>
        /// index per kg
        /// </summary>
        public float IndexPerKg
        {
            get { return indexPerKg; }
            set { indexPerKg = value; }
        }


        /// <summary>
        /// Position ID
        /// </summary>
        public string Id
        {
            get
            {
                int idx = name.IndexOf(",");
                if (idx >= 0)
                {
                    return name.Substring(idx + 1).Trim();
                }
                return "";
            }
        }
        #endregion

        /// <summary>
        /// baseCode
        /// </summary>
        public BaseCode[] BaseCode
        {
            get { return baseCode; }
            set { baseCode = value; }
        }

        /// <summary>
        /// get: nofit
        /// set: nofit, OffloadAllCargo()
        /// </summary>
        public bool NoFit
        {
            get { return nofit; }
            set
            {
                if (value && cargoList.Count > 0)
                {
                    OffloadAllCargo(false);
                }
                nofit = value;
            }
        }

        #region Methods

        /// <summary>add some ULD or Bulk object into this position </summary>
        /// <param name="posn"></param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Add(CargoPosnBase posn)
        {
            posn.myParent = this; //
            base.Add(posn);
        }

        /// <summary>Get Cmpt/Bay by name</summary>
        /// <param name="name">Cmpt/Bay name</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CargoPosnBase FindPosn(string name)
        {
            name = name.Trim();
            //if the name of the position equals the input string, the return this position
            if (this.name == name)
            {
                return this;
            }
            //otherwise, check the positions belonging to this position
            foreach (CargoPosnBase posn in this)
            {
                CargoPosnBase result = posn.FindPosn(name);
                if (result != null) return result;
            }
            //if none is found, return null
            return null;
        }

        /// <summary> if the type of this position equals the input type, add this position, otherwise
        /// call the Find method recursively to know if there's concerned positions found. </summary>
        /// <param name="aList">ArrayList</param>
        /// <param name="theType">aircraft type</param>
        /// <returns>ArrayList，CargoPosnBase ArrayList</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private ArrayList Find(ArrayList aList, Type theType)
        {
            Type thisType = this.GetType();
            //if the type of this position equals the input type, add this position
            if (thisType.Equals(theType))
            {
                aList.Add(this);
                return aList;
            }
            // if not, call the Find method recursively to know if there's concerned positions found.
            foreach (CargoPosnBase posn in this)
                posn.Find(aList, theType);
            return aList;
        }

        /// <summary> find out which CargoPosnBase object is this aircraft type using</summary>
        /// <param name="theType">aircraft type</param>
        /// <returns>CargoPosnBase ArrayList</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ArrayList Find(Type theType)
        {
            ArrayList aList = new ArrayList();
            return this.Find(aList, theType);
        }

        /// <summary> find out which CargoPosnBase object is this CargoPosnBase object belongs to</summary>
        /// <param name="theType">aircraft type</param>
        /// <returns>CargoPosnBase object </returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CargoPosnBase FindParent(Type theType)
        {
            Type thisType = this.GetType();
            if (thisType.Equals(theType))
                return this;
            else if (myParent != null)
                return myParent.FindParent(theType);
            else return null;
        }

        /// <summary>
        /// Find previous available Cmpt/Bay
        /// </summary>
        /// <returns>The candidate</returns>
        /// <summary>the position of previous object</summary>
        /// <returns>CargoPosnBase object </returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CargoPosnBase Prev()
        {
            if (myParent != null)
            {
                int idx = myParent.IndexOf(this);
                for (; idx > 0; idx--)
                {
                    CargoPosnBase candidate = (CargoPosnBase)myParent[idx - 1];
                    if (candidate.available) return candidate;
                }
            }
            //if it does not have a parent position, return null
            return null;
        }

        /// <summary>Find next available  Cmpt/Bay</summary>
        /// <returns>The candidate</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CargoPosnBase Next()
        {
            if (myParent != null)
            {
                int idx = myParent.IndexOf(this);
                if (idx >= 0)
                {
                    for (; idx < myParent.Count - 1; idx++)
                    {
                        CargoPosnBase candidate = (CargoPosnBase)myParent[idx + 1];
                        if (candidate.available) return candidate;
                    }
                }
            }
            //if it does not have a parent position, return null
            return null;
        }

        /// <summary>Check if all CargoPosnBase object of this CargoPosnBase are Leaf or not</summary>
        /// <returns>bool：if all are Leaf or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool AreAllChildrenLeaf()
        {
            foreach (CargoPosnBase child in this)
                if (!child.isLeaf) return false;
            return true;
        }

        /// <summary>Create visible object (CargoPosnBase) array</summary>
        /// <param name="candidateType"></param>
        /// <returns>visible object (CargoPosnBase) array</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CargoPosnBase[] GenVisibleList(int candidateType)
        {
            CargoPosnBase aList = new CargoPosnBase();
            foreach (CargoPosnBase child in this)
            {
                child.GetVisibleList(aList);
                //Console.WriteLine(child.Name);
            }
            return aList.ToArray();
        }

        /// <summary>Use array to return CargoPosnBase</summary>
        /// <returns>CargoPosnBase[]</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        /// <--  隱藏繼承的成員，加入new 關鍵字-->
        private new CargoPosnBase[] ToArray()
        {
            CargoPosnBase[] list = new CargoPosnBase[Count];
            for (int i = 0; i < Count; i++)
                list[i] = (CargoPosnBase)this[i];
            return list;
        }


        /// <summary> get the boundary of user interface</summary>
        /// <returns>RectangleF，the occupied square in the picture</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public RectangleF GetBounds() //Calcu Geometric
        {
            float x1 = float.MaxValue, y1 = float.MaxValue, x2 = float.MinValue, y2 = float.MinValue;

            //prepare the data used for checking the bounds
            if (this.Geom != null)
            {
                if (this.Geom.x < x1) x1 = this.Geom.x;
                if (this.Geom.y < y1) y1 = this.Geom.y;
                if (this.Geom.x + this.Geom.w > x2) x2 = this.Geom.x + this.Geom.w;
                if (this.Geom.y + this.Geom.h > y2) y2 = this.Geom.y + this.Geom.h;
            }

            //for all the positions, check if it is the left most and top most,
            //if it is not, go further to the next bound.
            foreach (CargoPosnBase posn in this)
            {
                RectangleF rect = posn.GetBounds();
                if (rect.Left != float.MaxValue && rect.Top != float.MaxValue)
                {
                    if (rect.Left < x1) x1 = rect.Left;
                    if (rect.Top < y1) y1 = rect.Top;

                    if (rect.Left + rect.Width > x2) x2 = rect.Left + rect.Width;
                    if (rect.Top + rect.Height > y2) y2 = rect.Top + rect.Height;
                }
            }

            if (x1 == float.MaxValue || y1 == float.MaxValue)
                return new RectangleF(0, 0, 0, 0);
            return new RectangleF(x1, y1, x2 - x1, y2 - y1);
        }

        #region Get net weight by dest, by category(C/B/M)

        /// <summary> get Net Weight</summary>
        /// <returns>Double: Net Weight</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getNetWt()
        {
            return getNetWt("", "");
        }

        /// <summary> get Net Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <returns>Double: Net Weight</returns>
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
        /// <param name="transit">Is transit or not</param>
        /// <returns>Double: Net Weight</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getNetWt(string catg, string dest, bool transit)
        {
            return getWt(catg, dest, transit, false);
        }

        /// <summary> get Gross Weight</summary>
        /// <returns>Double: Gross Weight</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getGrossWt()
        {
            return getGrossWt("", "");
        }

        /// <summary> get Gross Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <returns>Double: Gross Weight</returns>
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
        /// <param name="newOnBoard">Is New On Board or not</param>
        /// <returns>Double: Gross Weight</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getGrossWt(string catg, string dest, bool newOnBoard)
        {
            return getWt(catg, dest, newOnBoard, true);
        }

        /// <summary> get Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <param name="transit">Is transit or not</param>
        /// <param name="grossWt">Is Gross Weight or not</param>
        /// <returns>Double: Weight</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private double getWt(string catg, string dest, bool transit, bool grossWt)
        {
            double ttlWt = 0;
            foreach (ICargo cgo in this.cargoList)
            {
                ttlWt += cgo.calcuCargoWt(catg, dest, transit, grossWt);
            }

            if ((this is CargoCmpt) && this.IsMainDeckPosn())
            {
                //As CL,CR to C
                for (int i = 0; i < Count && i < 2; i++)
                    ttlWt += ((CargoPosnBase)this[i]).getWt(catg, dest, transit, grossWt);

                //As CDL,CDR to C
                for (int i = 2; i < Count && i < 4; i++)
                {
                    ttlWt += ((CargoPosnBase)this[i]).getWt(catg, dest, transit, grossWt) / 2;
                }

                //<!--#BR17202 THOMAS 當有Center   load取得重量
                if (Count>4)
                {                       
                    for (int i = 4; i < Count && i < 5; i++)    //As CD to C 
                    {
                        ttlWt += ((CargoPosnBase)this[i]).getWt(catg, dest, transit, grossWt) / 2;
                    }
                }
                //#BR17202-->

                CargoPosnBase prev = Prev();
                if (prev != null)
                {
                    for (int i = 2; i < prev.Count && i < prev.Count; i++)     //As CDL,CDR to D 
                    {
                        ttlWt += ((CargoPosnBase)prev[i]).getWt(catg, dest, transit, grossWt) / 2;
                    }

                    //<!--#BR17202 Thomas 當有Center  取得重量
                    if (Count > 4)
                    {
                        for (int i = 4; i < prev.Count && i < prev.Count; i++)//As CD to D 
                        {
                            ttlWt += ((CargoPosnBase)prev[i]).getWt(catg, dest, transit, grossWt) / 2;
                        }
                    }
                    //#BR17202 -->
                }
            }
            else
            {
                foreach (CargoPosnBase posn in this)
                    ttlWt += posn.getWt(catg, dest, transit, grossWt);
            }
            return ttlWt;
        }

        /// <summary> get Special load Weight</summary>
        /// <param name="catg">category</param>
        /// <param name="dest"> destination </param>
        /// <returns>Double: Special load Weight</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getSpecialLoadWt(string catg, string dest)
        {
            double ttlWt = 0;
            foreach (ICargo cgo in this.cargoList)
            {
                ttlWt += cgo.getSpecialLoadWt(catg, dest);
            }

            if ((this is CargoCmpt) && this.IsMainDeckPosn())
            {
                //As CL,CR to C
                for (int i = 0; i < Count && i < 2; i++)
                    ttlWt += ((CargoPosnBase)this[i]).getSpecialLoadWt(catg, dest);

                //As CDL,CDR to C
                for (int i = 2; i < Count && i < 4; i++)
                    ttlWt += ((CargoPosnBase)this[i]).getSpecialLoadWt(catg, dest) / 2;

                CargoPosnBase prev = Prev();
                if (prev != null)
                {
                    //As CDL,CDR to D
                    for (int i = 2; i < prev.Count && i < prev.Count; i++)
                        ttlWt += ((CargoPosnBase)prev[i]).getSpecialLoadWt(catg, dest) / 2;
                }
            }
            else
            {
                foreach (CargoPosnBase posn in this)
                    ttlWt += posn.getSpecialLoadWt(catg, dest);
            }
            return ttlWt;
        }

        #endregion

        /// <summary>
        ///  get the gross weight of all of the ULD, Bulk, and other special Load 
        /// </summary>
        /// <returns> gross weight </returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getWeight()
        {
            return getWeight(""); // compute  gross weight 
        }

        /// <summary>
        ///  get the gross weight of all of the ULD, Bulk, and other special Load 
        /// </summary>
        /// <param name="dest"> destination </param>
        /// <returns> gross weight </returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getWeight(string dest)
        {
            return getGrossWt("", dest);
        }

        /// <summary> get the gross Index of all of the ULD, Bulk, and other special Load</summary>
        /// <returns>gross Index</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getIndex()
        {
            //Firstly, add the index of this position on to the total index
            if (this.cargoList.Count > 0)
            {
                    return this.getWeight() * this.IndexPerKg;
            }
            else
            {
                double ttlIndex = 0;
                foreach (CargoPosnBase posn in this)
                    ttlIndex += posn.getIndex();
                return ttlIndex;
            }
        }

        #endregion

        #region check methods

        /// <summary> compute Compartment width</summary>
        /// <returns>Float: Compartment width</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private float CalcuCmpWidth()
        {
            float width = 0f;

            if (this.CargoList.Count > 0) //Occupied
            {
                foreach (ICargo cgo in this.CargoList)
                {
                    if (cgo is Cargo)
                    {
                        Cargo cargo = cgo as Cargo;
                        float[] uldDim = FDB.Instance.GetUldDimension(cargo.UldType, cargo.CarrierCode);
                        if (uldDim != null && uldDim.Length > 1)
                            width = uldDim[1];
                        break;
                    }
                }
            }
            else if (this is CargoBay)
            {
                foreach (CargoPosnBase posn in this)
                {
                    float w = posn.CalcuCmpWidth();
                    if (w > width) width = w;
                }
            }
            else if (this is CargoCmpt)
            {
                foreach (CargoPosnBase posn in this)
                {
                    width += posn.CalcuCmpWidth();
                }
            }
            return width;
        }

        #endregion

        /// <summary>Check if 'Bulk position' or not</summary>
        /// <returns>bool:'Bulk position' or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsBulkPosn()
        {
            //Check all of the base code
            if (this.baseCode != null)
            {
                foreach (BaseCode baseCd in this.baseCode)
                {
                    if (baseCd != null &&
                        baseCd.Name.Length > 0 &&
                        char.IsLetter(baseCd.Name, 0)) return false;
                }
            }
            //Check all of the position.IsBulkPosn in the lower deck
            foreach (CargoPosnBase posn in this)
                if (!posn.IsBulkPosn()) return false;
            return true;
        }

        /// <summary>Check if 'MainDeck position' or not</summary>
        /// <returns>bool:'MainDeck position' or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsMainDeckPosn()
        {
            if (this.Count > 0)
            {
                foreach (CargoPosnBase child in this)
                    if (child.IsMainDeckPosn()) return true;
            }
            else
            {
                if (this.name.Length > 0 && char.IsLetter(this.name, 0)) return true;

                for (CargoPosnBase posn = this; posn != null; posn = posn.Parent)
                {
                    if ((posn is CargoPosn) && (posn.Parent is CargoCmpt))
                        return true;
                }
            }
            return false;

        }

        /// <summary>Check if 'Lower Deck position' or not</summary>
        /// <returns>bool:'Lower Deck position' or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsLowerDeckPosn()
        {
            if (this.Count > 0)
            {
                foreach (CargoPosnBase child in this)
                    if (child.IsLowerDeckPosn()) return true;
            }
            else
            {
                if (this.name.Length > 0 && char.IsNumber(this.name, 0)) return true;
                for (CargoPosnBase posn = this; posn != null; posn = posn.Parent)
                {
                    if (posn is CargoBay)
                        return true;
                }
            }
            return false;
        }

        /// <summary>Check the Cargo of this position</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void CheckCargo()
        {
            if (this.CargoList.Count > 0)
            {
                string destination = "";
                foreach (ICargo cgo in this.CargoList)
                {
                    if (destination != "") destination += ",";
                    destination += cgo.Dest;
                    if (cgo is Cargo)
                    {
                        Cargo cargo = cgo as Cargo;
                        cargo.Check();
                    }
                } //foreach cargo

                string[] destList = Strings.shortenString(destination).Split(new char[] { ',', ' ' });
                foreach (string dest in destList)
                {
                    if (this.getNetWt("C", dest) < this.getSpecialLoadWt("C", dest))
                        throw (new Exception(EWBSCoreWarnings.CgoSpeLdExceedCgoWt));
                    if (this.getNetWt("M", dest) < this.getSpecialLoadWt("M", dest))
                        throw (new Exception(EWBSCoreWarnings.CgoSpeLdExceedMailWt));
                }
                destList = null; //#BR17164 THOMAS
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            }

            //Check each posn in this bay
            foreach (CargoPosnBase posn in this)
            {
                if (posn != null)
                    posn.CheckCargo();
            } //end foreach posn

        }

        /// <summary>Update  weight and Index of Cargo of this position</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void UpdateWtAndIndex()
        {
            foreach (ICargo cargo in this.cargoList)
            {
                if (cargo is Cargo)
                {
                    //Update maxGrossWeight & Index
                    if (this.BaseCode != null)
                    {
                        foreach (BaseCode baseCd in this.BaseCode)
                        {
                            if (baseCd.Name.Equals((cargo as Cargo).BaseCode)) //found
                            {

                                this.MaxGrossWeight = baseCd.MaxWt;
                                this.IndexPerKg = baseCd.Index; //Index

                                return;
                            }
                        }
                    }
                }
            }
            if (this.BaseCode.Length > 0)
            {
                this.MaxGrossWeight = this.BaseCode[0].MaxWt;
                this.IndexPerKg = this.BaseCode[0].Index; //Index
            }
        }

        /// <summary>swap two posn</summary>
        /// <param name="cpos1">1'st position</param>
        /// <param name="cpos2">2'nd position</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static void Swap(CargoPosnBase cpos1, CargoPosnBase cpos2) //new
        {
            //get the index of the two positions
            int idx1 = cpos1.Parent.IndexOf(cpos1);
            int idx2 = cpos2.Parent.IndexOf(cpos2);

            //if the both positions are at main deck, leaf, and included int the parent's count,
            if (idx1 >= 2 && cpos1.isLeaf && cpos1.IsMainDeckPosn() && idx1 < cpos2.Parent.Count &&
                idx2 >= 2 && cpos2.isLeaf && cpos2.IsMainDeckPosn() && idx2 < cpos1.Parent.Count)
            {
                CargoPosnBase thisCmpt1 = (cpos1 is CargoPosn ? cpos1.Parent : cpos1);
                CargoPosnBase nextCmpt1 = thisCmpt1.Next();

                CargoPosnBase thisCmpt2 = (cpos2 is CargoPosn ? cpos2.Parent : cpos2);
                CargoPosnBase nextCmpt2 = thisCmpt2.Next();

                if (nextCmpt1 != null && nextCmpt2 != null)
                {
                    CargoPosnBase nextPosn1 = nextCmpt1[idx1 % 2] as CargoPosnBase;
                    CargoPosnBase nextPosn2 = nextCmpt2[idx2 % 2] as CargoPosnBase;

                    if (nextCmpt1.Occupied || nextCmpt2.Occupied)
                        Swap(nextCmpt1, nextCmpt2);
                    else if (nextPosn1.Occupied || nextPosn2.Occupied)
                    {
                        Swap(nextPosn1, nextPosn2);
                    }

                    CargoPosnBase thisPosn1 = cpos1.Parent[idx1 % 2] as CargoPosnBase;
                    CargoPosnBase thisPosn2 = cpos2.Parent[idx2 % 2] as CargoPosnBase;
                    if (thisPosn1.Occupied || thisPosn2.Occupied)
                    {
                        Swap(thisPosn1, thisPosn2);
                    }
                }
            }

            PCargoList tmp = cpos1.cargoList;
            cpos1.cargoList = cpos2.cargoList;
            cpos2.cargoList = tmp;

            foreach (ICargo cgo in cpos1.cargoList)
            {
                cgo._Posn = cpos1.Name;//20060810
            }
            foreach (ICargo cgo in cpos2.cargoList)
            {
                cgo._Posn = cpos2.Name;//20060810
            }

            cpos1.UpdateWtAndIndex();
            cpos2.UpdateWtAndIndex();

            for (int i = 0; i < Math.Min(cpos1.Count, cpos2.Count); i++)
            {
                Swap((CargoPosnBase)cpos1[i], (CargoPosnBase)cpos2[i]);
            }

        }

        /// <summary> get Base Code</summary>
        /// <returns>string: Base Code</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string GetBaseCodes()
        {
            string baseCdCollection = "";
            if (specialbaseCode == null || specialbaseCode == "")
            {
                foreach (BaseCode baseCd in this.baseCode)
                {
                    if (baseCd.Name.Length == 1)
                    {
                        if (baseCdCollection != "") baseCdCollection += ", ";
                        baseCdCollection += baseCd.Name;
                    }
                }
            }
            else
            {
                baseCdCollection = specialbaseCode;
            }
            return baseCdCollection;
        }

        /// <summary>Proceed check action</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void check()
        {
            if (!this.IsBulkPosn() && this.CargoList.Count > 1)
            {
                throw (new Exception("There are two cargos in the same position."));
            }

            foreach (CargoPosnBase posn in this)
            {
                posn.check();
            }

            //check cmpWidth
            if (this.cmpWidth > 0)
            {
                float width = this.CalcuCmpWidth();
                if (width > this.cmpWidth)
                    throw (new Exception(string.Format(EWBSCoreWarnings.CgoRemainSpaceNotEnoughtInCmpt_1, this.Name)));
            }



            //check available
            if (!this.Available && this.cargoList.Count > 0)
            {
                throw (new Exception(string.Format(EWBSCoreWarnings.CgoPsnNotAvlbl_1, this.Name)));
            }


            //check Base Code
            if (this.baseCode != null && this.baseCode.Length > 0 && this.cargoList.Count > 0)
            {
                if (!this.IsBulkPosn() &&
                    !((CargoPosnMgr)this.FindParent(typeof(CargoPosnMgr))).Flight.bTrimByCmpt)
                {
                    //collect base codes
                    string baseCdCollection = GetBaseCodes();

                    foreach (ICargo cargo in this.cargoList)
                    {
                        if ((cargo is Cargo) && baseCdCollection.IndexOf((cargo as Cargo).BaseCode) >= 0)
                        {
                            break;
                        }
                        else
                        {
                            if (baseCdCollection.Length == 0) break;
                        }
                        throw (new Exception(string.Format(EWBSCoreWarnings.BaseCodeExpects_3,
                                                          this.Name, baseCdCollection, (cargo as Cargo).BaseCode)));

                    } //for
                }
            }

            //Check  Height Code
            foreach (ICargo cgo in this.CargoList)
            {
                if ((cgo is Cargo) && (this.HtCode.Length > 0)) //only process all of ULD
                {
                    Cargo cargo = cgo as Cargo;
                    if (cargo.HeightCode == null || //No Height Code
                        cargo.HeightCode.Length == 0 || //No Height Code
                        this.HtCode.IndexOf(cargo.HeightCode) < 0) //Height Code doesnot match
                    {
                        throw (new Exception(string.Format(EWBSCoreWarnings.HeightCodeExpects_3,
                                                          this.Name,
                                                          this.HtCode,
                                                          cargo.HeightCode)));
                    }
                }
            } //foreach

            //Check Cargo 
            if (this.CargoList.Count > 0)
            {
                string destination = "";
                foreach (ICargo cgo in this.CargoList)
                {
                    if (destination != "") destination += ",";
                    destination += cgo.Dest;
                } //foreach cargo

                string[] destList = Strings.shortenString(destination).Split(new char[] { ',', ' ' });
                foreach (string dest in destList)
                {
                    if (this.getNetWt("C", dest) < this.getSpecialLoadWt("C", dest))
                        throw (new Exception(EWBSCoreWarnings.CgoSpeLdExceedCgoWt));
                    if (this.getNetWt("M", dest) < this.getSpecialLoadWt("M", dest))
                        throw (new Exception(EWBSCoreWarnings.CgoSpeLdExceedMailWt));
                    if (this.getNetWt("B", dest) < this.getSpecialLoadWt("B", dest))
                        throw (new Exception(EWBSCoreWarnings.CgoSpeLdExceedBagWt));
                }
            }

            //Check if it out of the load restrict(Position Load Limit) of the cargo position 
            //Check if it out of the restrict(Compartment Load Limit) of bay or compartment
            //Check if it out of the restrict(Hold Load Limit) of cargo hold 
            if (this.getWeight() > this.MaxGrossWeight && this.MaxGrossWeight > 0)
            {
                //20060310, Modify warning message "MAN HOLD TTL OVER 70760K"
                string posnName = string.Format("\"{0}\" {1}", this.Name, (this is CargoHold) ? "HOLD" : (this is CargoCmpt) ? "CMPT" : "POSN");
                throw (
                    new Exception(string.Format(EWBSCoreWarnings.ExceedCmptPosnLoadLmt_2,
                                                posnName,
                                                this.MaxGrossWeight)));
            }

            //shc exclusive
            if (this.ShcExclusive != "")
            {
                foreach (ICargo cargo in this.GetSHCCsgmtList(new ArrayList()))
                {
                    if (this.ShcExclusive.IndexOf(cargo.SHC) >= 0)
                    {
                        throw (new Exception(
                            string.Format(EWBSCoreWarnings.ShcShouldNotBeInCmpt,
                                          cargo.SHC, this.Name)));
                    }
                }
            }
        }

        /// <summary> get Bulk information of this position</summary>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ArrayList BulkInfo()
        {
            // if it is not a bulk position, return null
            if (!this.IsBulkPosn())
                return null;

            bool hasDone = false;

            ArrayList alist = new ArrayList();

            //check all the compartments in this CargoList
            foreach (Consignment csgn in this.CargoList)
            {
                hasDone = false;
                foreach (Consignment csgn1 in alist)
                {
                    if (SameCategory(csgn, csgn1) && csgn.Dest == csgn1.Dest)
                    {
                        hasDone = true;
                        break;
                    }
                }
                if (!hasDone && !(csgn is SpecialLoad))
                    alist.Add(csgn);
            }
            foreach (Consignment csgn in this.CargoList)
            {
                hasDone = false;
                foreach (Consignment csgn1 in alist)
                {
                    if (SameCategory(csgn, csgn1) && csgn.Dest == csgn1.Dest)
                    {
                        hasDone = true;
                        break;
                    }
                }
                if (!hasDone && (csgn is SpecialLoad))
                    alist.Add(csgn);
            }
            alist.Sort(new BulkComparer());

            // If all the consignments are checked as not "has done", return null.
            if (alist.Count == 0) return null;
            ArrayList rlist = new ArrayList();
            float wt = 0;
            foreach (Consignment csgn in alist)
            {
                if (!(csgn is SpecialLoad) || csgn.Category.Equals("O"))
                {
                    wt = Convert.ToSingle(this.getNetWt(csgn.Category, csgn.Dest));

                    rlist.Add(csgn.Dest + "/" + csgn.Category + "/" + wt);
                }
            }
            alist = null;  //#BR17164 THOMAS
            System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            return rlist;
        }

        /// <summary>Check if the two Consignments are identical or not</summary>
        /// <param name="csgn1">1'st Consignment</param>
        /// <param name="csgn2">2'nd Consignment</param>
        /// <returns>bool：identical or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static bool SameCategory(Consignment csgn1, Consignment csgn2)
        {
            if (csgn1.Category == csgn2.Category)
                return true;

            return false;
        }

        public class BulkComparer : IComparer
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            /// <summary>Calls CaseInsensitiveComparer.Compare with the parameters reversed.</summary>
            /// <param name="x">objext 1</param>
            /// <param name="y">objext 2</param>
            /// <returns>int</returns>
            /// <remarks></remarks> 
            int IComparer.Compare(Object x, Object y)
            {
                Consignment px = x as Consignment;
                Consignment py = y as Consignment;

                int result = px.Dest.CompareTo(py.Dest);
                if (result == 0)
                    result = px.Category.CompareTo(py.Category);

                return (result);
            }
        }

        /// <summary> get all of the Consignment in this position</summary>
        /// <returns>ArrayList：Consignments</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ArrayList GetPosCsgnList()
        {
            if (this.IsBulkPosn())
                return this.CargoList;
            else
                return this.CargoList[0].Consignments as ArrayList;
        }

        /// <summary>
        /// Create visible object (CargoPosnBase) array to handle non-CDL condition.
        /// </summary>
        /// <param name="aList">a List</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void GetVisibleList(ArrayList aList)
        {
            bool leftFlag = true, rightFlag = true,Flag = true;
            if (!available) return;
        
            if (this.Occupied)     //Add the occupied object into list 將占用的物件添加到列表
            {
                aList.Add(this);
            }
            else if ((this.isLeaf) || (Count > 0 && AreAllChildrenLeaf() && !(this[0] is CargoBay))) 
            {

                #region  判斷前一個區域(CDL、CDR、CD)是否有貨物，若有貨物則不繪製圖形
                // consider been occupied
                CargoPosnBase prev = Prev();
                if (prev != null && prev.IsMainDeckPosn())
                {
                    //判斷CargoPosnBase 前一個位置是否有CDL區域並且aList有貨物在CDL，若條件成立，則DL不繪製
                    if (2 < prev.Count && aList.IndexOf(prev[2]) >= 0) //if CDL will affect DL
                    {
                        leftFlag = false;
                    }
                   //判斷CargoPosnBase 前一個位置是否有CDR區域並且aList有貨物在CDR，若條件成立，則DR不繪製
                    if (3 < prev.Count && aList.IndexOf(prev[3]) >= 0) //if CDR will affect DR
                    {
                        rightFlag = false;
                    }
                    
                    //<!-- #BR17201  THOMAS  判斷CargoPosnBase 前一個位置是否有CD區域以及aList有貨物在CD，若條件成立，則DL與DR不繪製
                    if (4 < prev.Count) 
                    {
                        if (aList.IndexOf(prev[4]) >= 0)
                        {
                            rightFlag = false;
                            leftFlag = false;
                        }
                    }
                    //#BR17201  -->

                }
                #endregion 

                //<!--#BR17201 THOMAS 判斷CENTER區域
                #region 判斷區域(四格中間)
                if (Flag)
                {
                    CargoPosnBase Child;
                    if (4 < Count) //判斷CargoPosnBase是否有CD區域
                    {
                        Child = (CargoPosnBase)this[4]; //取得CD區域資料
                        //判斷如果CD有，並且已有貨物，增加到aList，並且不再繪製CL、CR
                        if (Child.Available && Child.Occupied && Child.cargoList.Count > 0)
                        {
                            aList.Add(Child);
                            leftFlag = false;
                            rightFlag = false;
                            leftFlag = false;
                        }
                    }
                }
                #endregion
                //#BR17201 -->

                #region 判斷區域(左邊 L or CDL)
                if (leftFlag)
                {
                    CargoPosnBase leftChild;
                    if (2 < Count) //判斷CargoPosnBase有CDL區域
                    {
                        leftChild = (CargoPosnBase)this[2]; //取得CDL區域資料
                        //判斷如果CDL有，並且已有貨物，增加到aList，並且不再繪製CL
                        if (leftChild.Available && leftChild.Occupied && leftChild.cargoList.Count > 0) 
                        {
                            aList.Add(leftChild);
                            leftFlag = false;
                        }
                        else
                        {
                            //如果CargoPosnBase沒有CDL區域，就只有CL區域
                            leftChild = (CargoPosnBase)this[0]; //取CL區域資料
                            //判斷如果CL有，並且已有貨物，增加到aList，並且不再繪製CL
                            if (leftChild.Available && leftChild.Occupied)
                            {
                                aList.Add(leftChild);
                                leftFlag = false;
                            }
                        }
                    }
                    else if (0 < Count) //判斷CargoPosnBase只有CL區域 ****
                    {
                        leftChild = (CargoPosnBase)this[0];  //取CL區域資料
                        //判斷如果CL有，並且已有貨物，增加到aList，並且不再繪製CL
                        if (leftChild.Available && leftChild.Occupied) 
                        {
                            aList.Add(leftChild);
                            leftFlag = false;
                        }
                    }
                }
                #endregion

                #region 判斷區域(右邊 R or CDR)
                if (rightFlag)
                {
                    CargoPosnBase rightChild;
                    if (3 < Count) //判斷CargoPosnBase有CDR區域
                    {
                        rightChild = (CargoPosnBase)this[3];  //取CDR區域資料
                        //判斷如果CDR有，並且已有貨物，增加到aList，並且不再繪製CR
                        if (rightChild.Available && rightChild.Occupied && rightChild.cargoList.Count > 0)
                        {
                            aList.Add(rightChild);
                            rightFlag = false;
                        }
                        else
                        {
                            //如果CargoPosnBase沒有CDR區域，就只有CR區域
                            rightChild = (CargoPosnBase)this[1];//取CR區域資料
                            //判斷如果CR有，並且已有貨物，增加到aList，並且不再繪製CR
                            if (rightChild.Available && rightChild.Occupied) 
                            {
                                aList.Add(rightChild);
                                rightFlag = false;
                            }
                        }
                    }
                    else if (1 < Count)//判斷CargoPosnBase只有CR區域 ****
                    {
                        rightChild = (CargoPosnBase)this[1];//取CR區域資料
                        //判斷如果CR有，並且已有貨物，增加到aList，並且不再繪製CR
                        if (rightChild.Available && rightChild.Occupied) 
                        {
                            aList.Add(rightChild);
                            rightFlag = false;
                        }
                    }
                }
                #endregion

                #region 判斷區域(兩格中間 R+L)
                //middle is occupied 
                if (rightFlag && leftFlag)
                {
                    if (this.Available && this.Occupied)
                    {
                        aList.Add(this);
                        rightFlag = false;
                        leftFlag = false;
                    }
                }

                #endregion
                
                #region compensate left & right
                //rule for official evaluation
                //compensate the left 補左邊
                if (leftFlag && 0 < Count)
                {
                    CargoPosnBase leftChild = (CargoPosnBase)this[0];
                    if (leftChild.Available)
                    {
                        aList.Add(leftChild);
                        leftFlag = false;
                    }
                }
                //compensate the right 補右邊
                if (rightFlag && 1 < Count)
                {
                    CargoPosnBase rightChild = (CargoPosnBase)this[1];
                    if (rightChild.Available)
                    {
                        aList.Add(rightChild);
                        rightFlag = false;
                    }
                }
                
                //compensate the left
                if (leftFlag && 0 < Count)
                {
                    CargoPosnBase leftChild = (CargoPosnBase)this[0];
                    if (leftChild.Available)
                    {
                        aList.Add(leftChild);
                        leftFlag = false;
                    }
                }

                //compensate the right
                if (rightFlag && 1 < Count)
                {
                    CargoPosnBase rightChild = (CargoPosnBase)this[1];
                    if (rightChild.Available)
                    {
                        aList.Add(rightChild);
                        rightFlag = false;
                    }
                }
          

                //compensate the middle 補中間
                if (leftFlag && rightFlag)
                {
                    if (this.Available)
                    {
                        aList.Add(this);
                        rightFlag = false;
                        leftFlag = false;
                    }
                }
        #endregion
            }
            else //
            {
                foreach (CargoPosnBase child in this)
                    child.GetVisibleList(aList);
            }
        }

        /// <summary> get Special Load List of the position</summary>
        /// <returns>ArrayList：Special Load</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ArrayList GetSHCCsgmtList(ArrayList rtn)
        {
            foreach (ICargo cgo in this.CargoList)
            {
                cgo.GetSHCCsgmts(rtn);
            }
            foreach (CargoPosnBase child in this)
                child.GetSHCCsgmtList(rtn);
            return rtn;
        }
        
        //<!--- #BR17202 Get Bay Weight For BR77X 
        public double getBayWt(XmlNodeList LargePalletOverLapNodeList)
        {
            return getGrossBayWt("", "", LargePalletOverLapNodeList);
        }

        public double getGrossBayWt(string catg, string dest, XmlNodeList LargePalletOverLapNodeList)
        {
            return getGrossBayWt(catg, dest, true, LargePalletOverLapNodeList) + getGrossBayWt(catg, dest, false, LargePalletOverLapNodeList);
        }

        public double getGrossBayWt(string catg, string dest, bool newOnBoard, XmlNodeList LargePalletOverLapNodeList)
        {
            return getBayWt(catg, dest, newOnBoard, true, LargePalletOverLapNodeList);
        }

        private double getBayWt(string catg, string dest, bool transit, bool grossWt, XmlNodeList LargePalletOverLapNodeList)
        {
            double ttlWt = 0;
            foreach (ICargo cgo in this.cargoList)
            {
                double p = 1;

                //BAY不做53% & 64%的計算
                //foreach (XmlNode PercentNode in LargePalletOverLapNodeList)
                //{
                //    XmlAttributeCollection PercentNodeAt = PercentNode.Attributes; //取Posn屬性
                //    string UldBasecode = PercentNodeAt.Item(0).Value;
                //    string LimitCheckPercent = PercentNodeAt.Item(1).Value;

                //    if (cgo.UserUldBaseCode == UldBasecode  ) 
                //    {
                //        p = double.Parse(LimitCheckPercent) * 2;
                //    }
                //}

                ttlWt += cgo.calcuCargoWt(catg, dest, transit, grossWt)*p;
            }

            if ((this is CargoCmpt) && this.IsMainDeckPosn())
            {
                //As CL,CR to C
                for (int i = 0; i < Count && i < 2; i++)
                    ttlWt += ((CargoPosnBase)this[i]).getBayWt(catg, dest, transit, grossWt, LargePalletOverLapNodeList);

                //As CDL,CDR to C
                for (int i = 2; i < Count && i < 4; i++)
                {
                    double p = 1;
                    if (((CargoPosnBase)this[i]).cargoList.Count > 0 )
                    {                    
                        var cbs = ((CargoPosnBase)this[i]).cargoList[0].UserUldBaseCode; //貨物的BASECODE

                        foreach (XmlNode PercentNode in LargePalletOverLapNodeList)
                        {
                            XmlAttributeCollection PercentNodeAt = PercentNode.Attributes; //取Posn屬性
                            string UldBasecode = PercentNodeAt.Item(0).Value;
                            string LimitCheckPercent = PercentNodeAt.Item(1).Value;

                            if (cbs == UldBasecode) //比對BASECODE
                            {
                                p = double.Parse(LimitCheckPercent) * 2;
                            }
                        }
                    }

                    ttlWt += ((CargoPosnBase)this[i]).getBayWt(catg, dest, transit, grossWt, LargePalletOverLapNodeList) / 2 * p;
                }

                //<!--#BR17202 THOMAS 當有Center 取得重量
                if (Count > 4)
                {
                    for (int i = 4; i < Count && i < 5; i++)    //As CD to C 
                    {
                        double p = 1;
                        if (((CargoPosnBase)this[i]).cargoList.Count > 0)
                        {
                            var cbs = ((CargoPosnBase)this[i]).cargoList[0].UserUldBaseCode; //貨物的BASECODE

                            foreach (XmlNode PercentNode in LargePalletOverLapNodeList)
                            {
                                XmlAttributeCollection PercentNodeAt = PercentNode.Attributes; //取Posn屬性
                                string UldBasecode = PercentNodeAt.Item(0).Value;
                                string LimitCheckPercent = PercentNodeAt.Item(1).Value;

                                if (cbs == UldBasecode) //比對BASECODE
                                {
                                    p = double.Parse(LimitCheckPercent) * 2;
                                }
                            }
                        }
                        ttlWt += ((CargoPosnBase)this[i]).getBayWt(catg, dest, transit, grossWt, LargePalletOverLapNodeList) / 2 * p;
                    }
                }
                //#BR17202-->

                CargoPosnBase prev = Prev();
                if (prev != null)
                {
                    for (int i = 2; i < prev.Count && i < prev.Count; i++)     //As CDL,CDR to D 
                    {
                        double p = 1;
                        if (((CargoPosnBase)prev[i]).cargoList.Count > 0)
                        {
                            var cbs = ((CargoPosnBase)prev[i]).cargoList[0].UserUldBaseCode; //貨物的BASECODE

                            foreach (XmlNode PercentNode in LargePalletOverLapNodeList)
                            {
                                XmlAttributeCollection PercentNodeAt = PercentNode.Attributes; //取Posn屬性
                                string UldBasecode = PercentNodeAt.Item(0).Value;
                                string LimitCheckPercent = PercentNodeAt.Item(1).Value;

                                if (cbs == UldBasecode) //比對BASECODE
                                {
                                    p = double.Parse(LimitCheckPercent) * 2;
                                }
                            }
                        }
                        ttlWt += ((CargoPosnBase)prev[i]).getBayWt(catg, dest, transit, grossWt, LargePalletOverLapNodeList) / 2 * p;
                     
                    }

                    //<!--#BR17202 Thomas 當有Center  取得重量
                    if (Count > 4)
                    {
                        for (int i = 4; i < prev.Count && i < prev.Count; i++)//As CD to D 
                        {
                            double p = 1;
                            if (((CargoPosnBase)prev[i]).cargoList.Count > 0)
                            {
                                var cbs = ((CargoPosnBase)prev[i]).cargoList[0].UserUldBaseCode; //貨物的BASECODE

                                foreach (XmlNode PercentNode in LargePalletOverLapNodeList)
                                {
                                    XmlAttributeCollection PercentNodeAt = PercentNode.Attributes; //取Posn屬性
                                    string UldBasecode = PercentNodeAt.Item(0).Value;
                                    string LimitCheckPercent = PercentNodeAt.Item(1).Value;

                                    if (cbs == UldBasecode) //比對BASECODE
                                    {
                                        p = double.Parse(LimitCheckPercent) * 2;
                                    }
                                }
                            }
                            ttlWt += ((CargoPosnBase)prev[i]).getBayWt(catg, dest, transit, grossWt, LargePalletOverLapNodeList) / 2 * p;
                        }
                    }
                    //#BR17202 -->
                }
            }
            else
            {
                foreach (CargoPosnBase posn in this)
                    ttlWt += posn.getBayWt(catg, dest, transit, grossWt, LargePalletOverLapNodeList) ;
            }
            return ttlWt;
        }
        //#BR17202-->
    } //class CPosn
}