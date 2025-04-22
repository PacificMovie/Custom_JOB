/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/

//*****************************************************************************
//* THOMAS    | Ver. 10 | #BR19010 | 2019/3/11                                *
//*---------------------------------------------------------------------------*
//* LoadVersion 原從CargWing取得，改為FDB預設                                 *
//*****************************************************************************
//* THOMAS    | Ver. 09 | #BR19007 | 2019/3/11                                *
//*---------------------------------------------------------------------------*
//* 787大小盤限制，新增 BR781                                                 *
//*****************************************************************************
//* THOMAS    | Ver. 08 | #BR18118 | 2018/9/11                                *
//*---------------------------------------------------------------------------*
//* 787 機型依版型檢查 Cmp 不能同時放置兩個PMC                                *
//*****************************************************************************
//* THOMAS    | Ver. 07 | #BR17210 | 2017/9/11   (原#BR16106 )                *
//*---------------------------------------------------------------------------*
//* For A332機型(B-16307~B16312) 31P放ECP(危險品)時出現警示訊息。             *
//*****************************************************************************
//*THOMAS| Ver. 09 | #BR17164 | 2017/8/31                                     *
//*---------------------------------------------------------------------------*
//* 物件關閉後 Dispose 標記由GC回收                                           *
//*****************************************************************************
//* THOMAS   | Ver. 08 | #BR17209 | 2017/08/24                                *
//*---------------------------------------------------------------------------*
//* BR777F    ULD Posn 為31P 32P                                              *
//*****************************************************************************
//* THOMAS    | Ver. 07 | #BR17207  | 2017/8/30 
//*         (原  #BR16105 )
//*---------------------------------------------------------------------------*
//*BR77X機型 25/31放置PLA/FLA時出現警示訊息。  
//* For 777機型，28BAY/31BAY 放置PLA/FLA時出現警示訊息。                              *     
//**********************************************************************************
//* THOMAS   | Ver. 06 | #BR17202 | 2017/07/28                                                                        *
//*-------------------------------------------------------------------------------- *
//* BR77X 777貨機載重限制 (Unsymmetrical load limit、large pallet overlap(53%/64%)、*
//*M/D check for increased lower deck loading (BULK)、M/D max 82100kg)                 *
//**********************************************************************************
//* Terri Liu  | Ver. 05 | #BR16111 | 2016/NOV/15                                                                    *
//*-------------------------------------------------------------------------- -----*
//* 15.	74Y貨機要使用INCREASED CUMULATIVE LOAD時，點選”YES”後，再增加一個警訊    *
//*『take off C.G. point must after of the forward limit “A” on the trim sheet』 *
//**********************************************************************************
//* Cherry Chan  | Ver. 04 | #BR11107 | 2011/OCT/31                   (V1.19)      *
//*---------------------------------------------------------------------------     *
//* For A333 Actype, if low deck is 5, 41P is not PMC                              *
//**********************************************************************************
//* CherryChan   | Ver. 03 | #BR10118 | 2010/SEP/10                   (V01.16)     *
//*---------------------------------------------------------------------------     *
//* For CheckCumulativeLoadLimits method error handle                              *
//**********************************************************************************
//* Cherry Chan  | Ver. 02 | #BR10106 | 2010/AUG/17                   (V01.15)     *
//*---------------------------------------------------------------------------     *
//* if re-open flt's WBF, PMC/PAG maxgroceeweight will error                       *
//* it will unload the PMC													       *
//* BUG:LOADSHEET送出後，LOAD PLAN中的PMC/PAG盤會不在計劃表中。				       *
//* LOADSHEET產生後，若重新開啟航班，LOAD PLAN中，							       *
//* 重量介於PMC與PAG最大值之間的盤 ，會成為UNLOCATE LOAD。(盤會掉下來)             *
//**********************************************************************************
//* Fio Sun      | Ver. 01 | #BR07007 | 2007/APR/10                                *
//*---------------------------------------------------------------------------     *
//* When TOW >=366,400kg, the “A1+A2+B1+ZC+ZD” must >2268kg.                     *
//* add a exception to warning user and loadsheet cant not be produced.            *
//**********************************************************************************



//using System.Drawing;
//using System.Windows.Forms;
using System;
using System.Collections;
using System.Drawing;
using EwbsCore.Util;
using FlightDataBase;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace EWBS
{
    public class CargoLimit
    {
        public string PosnName { get; set; }
        public string BaseCode { get; set; }
        public string UldType { get; set; }
        public double TtlWt { get; set; }
    }

    /// <summary>
    /// geometry coordinate 
    /// </summary>
    public class Geom
    {
        public float x; //x coordinate 
        public float y; //y coordinate 
        public float w; //width
        public float h; //height
        public float d1x; //door's x coordinate (1)
        public float d2x; //door's x coordinate (2)
        public float d1y; //door's y coordinate 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">x coordinate </param>
        /// <param name="y">y coordinate </param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        public Geom(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">x coordinate </param>
        /// <param name="y">y coordinate </param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="d1x">dimention of x 1</param>
        /// <param name="d2x">dimention of x 2</param>
        /// <param name="d1y">dimention of y</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Geom(float x, float y, float w, float h, float d1x, float d2x, float d1y)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.d1x = d1x;
            this.d2x = d2x;
            this.d1y = d1y;
        }

        /// <summary>
        /// w
        /// </summary>
        public float Width
        {
            get { return w; }
        }

        /// <summary>
        /// h
        /// </summary>
        public float Height
        {
            get { return h; }
        }

        /// <summary>
        /// x
        /// </summary>
        public float Left
        {
            get { return x; }
        }

        /// <summary>
        /// y
        /// </summary>
        public float Top
        {
            get { return y; }
        }

        /// <summary>
        /// get: new RectangleF(x, y, w, h)
        /// </summary>
        public RectangleF Rect
        {
            get { return new RectangleF(x, y, w, h); }
        }
    }

    /// <summary>
    /// Class for Hold
    /// </summary>
    public class CargoHold : CargoPosnBase
    {
    }

    /// <summary>
    /// Class for Compartment
    /// </summary>
    public class CargoCmpt : CargoPosnBase
    {
    }

    /// <summary>
    /// Class for Bay
    /// </summary>
    public class CargoBay : CargoPosnBase
    {
    }

    /// <summary>
    /// Class for position
    /// </summary>
    public class CargoPosn : CargoPosnBase
    {
    }

    /// <summary>
    /// maintain cargo position data (invlude Hold、Compartment and Bay etc.)
    /// and the restriction check
    /// </summary>
    public class CargoPosnMgr : CargoPosnBase
    {
        private Flight theFlight; //Flight
        private string[] loadVersionNames; //the names of load versions
        private Hashtable h = new Hashtable(); //a hash table

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight">Flight</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CargoPosnMgr(Flight theFlight)
        {
            this.theFlight = theFlight;
            // get aircraft type
            AirTypeEx theAirType = theFlight.ACType;

            //load data from FDB  
            LoadFromFDB(theAirType);

            //add loadVersionNames
            loadVersionNames = theAirType.GetLoadVersionNames();
        }

        #region Properties

        /// <summary>
        /// get: loadVersionNames
        /// </summary>
        public string[] LoadVersionNames
        {
            get { return loadVersionNames; }
        }

        /// <summary>
        /// get: theFlight
        /// </summary>
        public Flight Flight
        {
            get { return theFlight; }
        }

        #endregion

        //#BR19010 Thomas 依照LoadVersion取得 Data
        public string GetLoadVersionDesc(string LoadVersion)
        {
            AirTypeEx theAirType = theFlight.ACType;
            string VersionDescription = theAirType.LoadVersionDescription(LoadVersion);
            return VersionDescription;

        }

        /// <summary>Set Load version</summary>
        /// <param name="loadVersion">load Version</param>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void SetLoadVersion(string loadVersion)
        {
            try
            {
                loadVersion = loadVersion.Trim();
                if (loadVersion == "") return;

                // First check special case, Lower Deck's middle hold is existed?
                AirTypeEx theAirType = theFlight.ACType;

                // get the load version
                AirtypeCargoVersionLoadVersion theLoadVersion = theAirType.GetLoadVersion(loadVersion);

                if (theLoadVersion != null)
                {
                    AirtypeCargoVersionLoadVersionCmp[] cmpList = theLoadVersion.cmp;
                    if (cmpList == null) return;

                    foreach (AirtypeCargoVersionLoadVersionCmp cmp in cmpList)
                    {
                        CargoPosnBase tmpCmpt = this.Find(cmp.name);
                        if (tmpCmpt == null) continue;
                        tmpCmpt.UpdateAllAvailable(false); //Set All of the Cmpt to false

                        //Check all the positions i the compartment
                        foreach (AirtypeCargoVersionLoadVersionCmpPos posn in cmp.pos)
                        {
                            CargoPosnBase tmpPosn = tmpCmpt.FindPosn(posn.name);

                            if (tmpPosn == null) continue;

                            while (!(tmpPosn.Parent is CargoCmpt))
                            {
                                tmpPosn = tmpPosn.Parent;
                            }

                            //Select the required Bay/Cgo Posn
                            tmpPosn.UpdateAllAvailable(true); //Set All of the Bay to true

                            tmpPosn.Geom.x = tmpPosn.Default_Geom.x;
                            tmpPosn.Geom.y = tmpPosn.Default_Geom.y;
                            foreach (CargoPosnBase child in tmpPosn)
                            {
                                child.Geom.x = child.Default_Geom.x;
                                child.Geom.y = child.Default_Geom.y;
                            }
                            //Modify Geom, if necessary
                            //TODO ...
                            if (posn.xSpecified && posn.x != 0u)
                            {
                                tmpPosn.Geom.x = Convert.ToSingle(posn.x);
                                foreach (CargoPosnBase child in tmpPosn)
                                {
                                    child.Geom.x = tmpPosn.Geom.x;
                                }
                            }
                            if (posn.ySpecified && posn.y != 0u)
                            {
                                float y = Convert.ToSingle(posn.y);
                                tmpPosn.Geom.y = y;
                                foreach (CargoPosnBase child in tmpPosn)
                                {
                                    child.Geom.y = y;
                                    y += child.Geom.Height;
                                }
                            }

                            if (posn.baseCode != null && posn.baseCode.Length > 0)
                            {
                                tmpPosn.specialbaseCode = posn.baseCode;
                            }
                            else
                            {
                                tmpPosn.specialbaseCode = "";
                            }
                        }
                        if (cmp.widthSpecified)
                            tmpCmpt.cmpWidth = cmp.width;
                        else tmpCmpt.cmpWidth = 0;

                        tmpCmpt.Available = true; //

                        //Offload unavailable Cargo
                        tmpCmpt.OffladUnavailableCargo();


                    } //for
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        #region Methods

        /// <summary>Init CargoPosnMgr object</summary>
        /// <param name="aList">CargoList to be added</param>
        /// <param name="bClearAll">cleared or not</param>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Init(ArrayList aList, bool bClearAll)
        {
            if (bClearAll)
            {
                //Clear all of the Cargos in this position
                CargoPosnBase[] vList = this.GenVisibleList(0);
                foreach (CargoPosnBase thePosn in vList)
                    thePosn.CargoList.Clear();
            }
            try
            {
                //check all of the ICargo object in aList 
                foreach (ICargo cargo in aList)
                {
                    if (cargo.Posn != "")
                    {
                        CargoPosnBase oriPosn = this.Find(cargo.Posn);
                        try
                        {
                            if (oriPosn == null || !oriPosn.Available) throw (new Exception(EWBSCoreWarnings.OffloadCargoNow));

                            if (oriPosn.IsBulkPosn() && !oriPosn.isLeaf && !theFlight.bTrimByCmpt)//2006/05/11, skip if bTrimByCmpt
                                throw (new Exception(EWBSCoreWarnings.BulkNotMatched));

                            oriPosn.CargoList.Add(cargo);


                            oriPosn.UpdateWtAndIndex(); //BR10106 reset wt and index

                            oriPosn.check();

                        }
                        catch
                        {
                            if (oriPosn != null) oriPosn.CargoList.Remove(cargo);
                            cargo._Posn = "";
                        }
                        if (oriPosn != null) oriPosn.UpdateWtAndIndex();
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>load the required data from FDB</summary>
        /// <param name="theAirType">aircraft type</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void LoadFromFDB(AirTypeEx theAirType)
        {
            if (theAirType == null) return;
            try
            {
                //Create Hold object, and check Hold's restricted weight 
                foreach (CargoPositionHold hold in theAirType.CargoPosition.hold)
                {
                    CargoHold newHold = new CargoHold();

                    this.Add(newHold);
                    newHold.Name = hold.name;
                    newHold.Available = true;
                    newHold.Geom = new Geom(hold.x, hold.y, hold.w, hold.h, hold.d1x, hold.d2x, hold.d1y);
                    //Hold's restricted weight 
                    newHold.MaxGrossWeight = hold.maxWt;

                    h.Add(newHold.Name, newHold);

                    //Create Compartment object, and check restricted weight of Compartment 
                    foreach (CargoPositionHoldCmp cmp in hold.cmp)
                    {
                        CargoPosnBase newCmpt = new CargoCmpt();
                        newCmpt.Name = cmp.name;
                        newCmpt.IndexPerKg = cmp.index;
                        newCmpt.Available = true;

                        //Create bay object, and check restricted weight of bay 
                        foreach (CargoPositionHoldCmpBay bay in cmp.bay)
                        {
                            CargoPosnBase newBay = new CargoBay();
                            newBay.Name = bay.name;
                            newBay.Available = true;

                            //Create position object, and check restricted weight of position 
                            foreach (CargoPositionHoldCmpBayPos posn in bay.pos)
                            {
                                CargoPosnBase newPosn = new CargoPosn();
                                newPosn.Name = posn.name;
                                newPosn.Geom = new Geom(posn.x, posn.y, posn.w, posn.h);
                                //Ht code
                                newPosn.HtCode = posn.highCode;
                                //base Code
                                if (posn.baseCode != null)
                                {
                                    BaseCode[] baseCodeList = new BaseCode[posn.baseCode.Length];
                                    for (int i = 0; i < posn.baseCode.Length; i++)
                                        baseCodeList[i] = new BaseCode(posn.baseCode[i]);
                                    newPosn.BaseCode = baseCodeList;

                                    // restricted weight of Posn 
                                    newPosn.MaxGrossWeight = posn.baseCode[0].maxWt;
                                    newPosn.IndexPerKg = posn.baseCode[0].index; //Index
                                    newPosn.Available =
                                        (newPosn.MaxGrossWeight > 0);
                                    if (posn.shc != null)
                                        newPosn.ShcExclusive = posn.shc;
                                }

                                //upper deck have no Bay
                                if (cmp.name.ToUpper() == posn.name.ToUpper().Split(",".ToCharArray())[0])
                                {
                                    bay.name = posn.name;
                                    newBay = newPosn;
                                }
                                else if (bay.pos.Length == 1 || //lower deck  have Bay
                                    bay.name.ToUpper().Equals(posn.name.ToUpper()))
                                {
                                    newBay.Geom = newPosn.Geom;

                                    //Ht Code
                                    newBay.HtCode = newPosn.HtCode;
                                    //base Code
                                    newBay.BaseCode = newPosn.BaseCode;
                                    //Posn's restricted weight 
                                    newBay.MaxGrossWeight = newPosn.MaxGrossWeight;
                                    newBay.IndexPerKg = newPosn.IndexPerKg; //Index
                                    newBay.Available = newPosn.Available;
                                    newBay.ShcExclusive = newPosn.ShcExclusive;
                                    foreach (CargoPosnBase child in newPosn)
                                    {
                                        newBay.Add(child);
                                    }
                                }
                                else
                                {
                                    newBay.Add(newPosn);
                                    h.Add(newPosn.Name, newPosn);
                                }
                            }
                            if (cmp.bay.Length == 1 ||
                                (cmp.name.ToUpper() == bay.name.ToUpper().Split(",".ToCharArray())[0]))
                            {
                                newCmpt.Name = bay.name;
                                //Note : newBay maybe Bay or Posn
                                newCmpt.Geom = newBay.Geom;
                                //Ht Code
                                newCmpt.HtCode = newBay.HtCode;
                                //base Code
                                newCmpt.BaseCode = newBay.BaseCode;
                                //Posn's restricted weight 
                                newCmpt.MaxGrossWeight = newBay.MaxGrossWeight;
                                newCmpt.IndexPerKg = newBay.IndexPerKg; //Index
                                newCmpt.Available = newBay.Available;
                                newCmpt.ShcExclusive = newBay.ShcExclusive;
                                foreach (CargoPosnBase child in newBay)
                                {
                                    newCmpt.Add(child);
                                }
                            }
                            else
                            {
                                newCmpt.Add(newBay);
                                h.Add(newBay.Name, newBay);
                            }
                        }
                        newHold.Add(newCmpt);
                        h.Add(newCmpt.Name, newCmpt);
                        newCmpt = null;   //#BR17164 THOMAS
                    }
                    newHold = null;  //#BR17164 THOMAS
                } //Hold
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            }
            catch (Exception e)
            {
                throw (new Exception("CargoPosnMgr initialization error:" + e.Message));
            }
        } //LoadFromFDB()

        /// <summary> get All of the Compartment, put into one List</summary>
        /// <returns>CargoPosnBase[]：Compartments</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CargoPosnBase[] GetCmptList()
        {
            ArrayList aList = new ArrayList();
            foreach (CargoPosnBase hold in this)
            {
                foreach (CargoPosnBase cmpt in hold)
                    aList.Add(cmpt);
            }
            return (CargoPosnBase[])aList.ToArray(typeof(CargoPosnBase));
        }

        /// <summary>put some CargoBase object into one CargoPosnBase position </summary>
        /// <param name="cargo">the to-be-loaded ICargo object </param>
        /// <param name="cargoPosn">the to-be-loaded  position  string </param>
        /// <returns>bool:success or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool UploadCargo(ICargo cargo, string cargoPosn)
        {
            return UploadCargo(cargo, Find(cargoPosn));
        }

        /// <summary>put some CargoBase object into one CargoPosnBase position </summary>
        /// <param name="cargo">the to-be-loaded ICargo object </param>
        /// <param name="cargoPosn">the to-be-loaded  position  object </param>
        /// <returns>bool:success or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool UploadCargo(ICargo cargo, CargoPosnBase cargoPosn)
        {  //貨物拉到位置
            if (cargoPosn != null)
            {
                if (cargoPosn.Available &&
                    cargoPosn.CargoList.IndexOf(cargo) < 0 &&
                    cargo != null)
                {
                    string oriPosn = cargo.Posn;
                    if (cargo.Posn != "") //offload Cargo first
                    {
                        OffloadCargo(cargo, cargo.Posn);
                    }

                    // consider  cargo and position are match or not
                    bool bBulk = cargoPosn.IsBulkPosn();

                    if ((cargo is Cargo) && !bBulk ||
                        (cargo is Consignment) && bBulk ||
                        ((CargoPosnMgr)this.FindParent(typeof(CargoPosnMgr))).Flight.bTrimByCmpt)
                    {
                        cargoPosn.CargoList.Add(cargo);
                    }
                    else
                    {
                        return false; //cargo and  position are asymmetric
                    }

                    cargoPosn.UpdateWtAndIndex();

                    if (Check())
                    {
                        cargo.Posn = cargoPosn.Name;
                        return true;
                    } //if

                    //failure
                    if (oriPosn == null || oriPosn == "")
                    {
                        //Remove Cargo
                        cargoPosn.CargoList.Remove(cargo);

                    }
                    else if (oriPosn.Equals(cargo.Posn))
                    {
                        OffloadCargo(cargo, oriPosn);
                    }
                    else
                    {
                        cargoPosn.CargoList.Remove(cargo);
                        UploadCargo(cargo, oriPosn);
                    }
                    cargoPosn.UpdateWtAndIndex();

                }

                if (cargoPosn.CargoList.IndexOf(cargo) >= 0)
                    return true;
            }
            return false;
        }

        /// <summary>put some CargoBase object into one CargoPosnBase position (without check )</summary>
        /// <param name="cargo">the to-be-loaded ICargo object </param>
        /// <param name="cargoPosn">the to-be-loaded  position  object </param>
        /// <returns>bool:success or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool UploadCargoWithoutCheck(ICargo cargo, CargoPosnBase cargoPosn)
        {
            if (cargoPosn != null &&
                cargoPosn.Available &&
                cargoPosn.CargoList.IndexOf(cargo) < 0 &&
                cargo != null)
            {
                if (cargo.Posn != "") //offload Cargo first
                {
                    OffloadCargo(cargo, cargo.Posn);
                }

                // consider cargo and position are match or not
                bool bBulk = cargoPosn.IsBulkPosn();
                if (((cargo is Cargo) && !bBulk) ||
                    ((cargo is Consignment) && bBulk) ||
                    this.Flight.bTrimByCmpt)
                {
                    cargoPosn.CargoList.Add(cargo);
                }
                else
                {
                    return false; //cargo and position are asymmetric
                }

                cargoPosn.UpdateWtAndIndex();


                cargo.Posn = cargoPosn.Name;
                return true;
            }
            return false;
        }

        /// <summary>unload some CargoBase object from some CargoPosnBase position</summary>
        /// <param name="cargo">the to-be-loaded ICargo object </param>
        /// <param name="cargoPosn">the to-be-loaded  position  string </param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void OffloadCargo(ICargo cargo, string cargoPosn)
        {
            OffloadCargo(cargo, Find(cargoPosn));
        }

        /// <summary>unload some CargoBase object  from some CargoPosnBase position</summary>
        /// <param name="cargo">the to-be-loaded ICargo object </param>
        /// <param name="cargoPosn">the to-be-loaded  position  object </param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void OffloadCargo(ICargo cargo, CargoPosnBase cargoPosn)
        {
            if (cargo != null)
            {
                if (cargoPosn != null && cargoPosn.CargoList.IndexOf(cargo) >= 0)
                {
                    cargoPosn.CargoList.Remove(cargo);
                    cargoPosn.UpdateWtAndIndex();

                    cargo.Posn = "";
                }
            }

        }

        /// <summary>Handle the related exception of Cargo Position</summary>
        /// <param name="sender">pass-in sender object </param>
        /// <param name="warning">Is warning or not</param>
        /// <param name="str">displayed string </param>
        /// <returns>bool:success or not</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private bool CargoPosnExceptionHandler(object sender, bool warning, string str)
        {
            if (theFlight.EwbsEventHandler != null)
            {
                return theFlight.EwbsEventHandler(sender, warning, str);
            }
            return false;
        }

        /// <summary> compute CPI</summary>
        /// <param name="index">index</param>
        /// <returns>double: CPI</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private double calcuCPI(uint index)
        {
            AirTypeEx theAirType = theFlight.ACType;
            if (theAirType != null)
            {
                AirtypeCpi[] cpiList = theAirType.cpi;

                if (cpiList == null) return 0;

                if (index < cpiList.Length)
                {
                    AirtypeCpi cpi = cpiList[index];
                    double result = 0;

                    string delimStr = " +-*\\";
                    char[] delimiter = delimStr.ToCharArray();
                    string[] cmptList = cpi.cpiFor.Split(delimiter);

                    foreach (string cmpt in cmptList)
                    {
                        CargoPosnBase posn = this.Find(cmpt);
                        if (posn != null)
                        {
                            result += posn.getIndex();
                        }
                    } //
                    return result;
                }
            }
            return 0; //double.NegativeInfinity;
        }

        /// <summary> compute CPI</summary>
        /// <returns>double: CPI</returns> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double CalcuCPI()
        {
            return calcuCPI(0); //
        }

        #endregion

        #region check Methods

        /// <summary>check if the two CargoPosnBases are the same Compartment or not</summary>
        /// <param name="pos1">1'st  position </param>
        /// <param name="pos2">2'nd  position </param>
        /// <returns>bool： identical or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private bool IsSameCmpt(CargoPosnBase pos1, CargoPosnBase pos2)
        {
            return pos1.FindParent(typeof(CargoCmpt)) == pos2.FindParent(typeof(CargoCmpt));
        }

        /// <summary>check if it violates the restriction of dangerous goods or not</summary>
        /// <returns>bool：violate or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private bool CheckDangerousGoods()
        {
            // collect all posn that has SHC
            bool hasSHC;

            //CargoPosnBase specialPosns = new CargoPosnBase();
            ArrayList specialPosns = new ArrayList();

            foreach (CargoPosnBase posn in GenVisibleList(0))
            {
                hasSHC = false;
                foreach (ICargo cgo in posn.CargoList)
                {
                    if (cgo is SpecialLoad)
                    {
                        hasSHC = true;
                        break;
                    }
                    else if (cgo is Cargo || cgo is Consignment)
                    { // CargoList consists of Cargo except in BULK posn(consists of Consignment) 
                        foreach (Consignment csgmt in cgo.Consignments)
                            if (csgmt is SpecialLoad)
                            {
                                hasSHC = true;
                                break;
                            }
                        if (hasSHC) break;
                    }
                }
                if (hasSHC) specialPosns.Add(posn);
            }

            // check SHC Conflicts
            string conflicts = "";
            for (int i = 0; i < specialPosns.Count; i++)
            {
                CargoPosnBase posn1 = specialPosns[i] as CargoPosnBase;
                // get all shc in i_th posn
                // check SHC conflicts in ith position
                conflicts += SHCConflicts(posn1, posn1);
                // Compare SHC in i_th posn with SHC in j_th posn
                for (int j = i + 1; j < specialPosns.Count; j++)
                {
                    CargoPosnBase posn2 = specialPosns[j] as CargoPosnBase;
                    // check SHC conflicts in ith and jth position
                    conflicts += SHCConflicts(posn1, posn2);
                }
            }

            if (conflicts.Length > 0)
            {
                conflicts = "SHC Conflicts:\r\n" + conflicts;
                throw (new Exception(conflicts));
                //return false;
            }
            else
                return true;
        }

        /// <summary>check if the two positions have SHC conflict or not</summary>
        /// <param name="pos1">1'st  position </param>
        /// <param name="pos2">2'nd  position </param>
        /// <returns>instruction string of SHC violation </returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private string SHCConflicts(CargoPosnBase pos1, CargoPosnBase pos2)
        {
            string rtn = "";
            int dgCode;
            bool danger = false;

            ArrayList csgn1 = SHCConsignments(pos1);
            ArrayList csgn2 = SHCConsignments(pos2);
            for (int i = 0; i < csgn1.Count; i++)
                for (int j = i; j < csgn2.Count; j++)
                {
                    danger = false;
                    dgCode = theFlight.Airline.DGCode((csgn1[i] as SpecialLoad).SHC,
                                                      (csgn2[j] as SpecialLoad).SHC);
                    if (dgCode == 1 && this.IsAdjacency(pos1, pos2))
                        danger = true;
                    if (dgCode == 2 && this.IsSameCmpt(pos1, pos2))
                    {
                        danger = true;
                    }
                    if (dgCode == 3 && this.IsSameCmpt(pos1, pos2))
                        danger = true;
                    if (danger)
                        rtn += string.Format("{0} and {1} conflict at {2}, {3}\r\n",
                                             (csgn1[i] as SpecialLoad).SHC,
                                             (csgn2[j] as SpecialLoad).SHC, pos1.Name, pos2.Name);
                }
            return rtn;
        }

        /// <summary> get Consignment ArrayList of Special Load of some position</summary>
        /// <param name="pos"> position </param>
        /// <returns>ArrayList: Consignment ArrayList of Special Load</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ArrayList SHCConsignments(CargoPosnBase pos)
        {
            return pos.GetSHCCsgmtList(new ArrayList());
        }

        /// <summary>Check if this position and other positions are stay in the same position or not</summary>
        /// <param name="pos"> position </param>
        /// <returns>bool：in the same position or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private bool isCombinedPos(CargoPosnBase pos)
        {
            CargoPosnBase cmpt = pos.FindParent(typeof(CargoCmpt));
            for (int i = 0; i < cmpt.Count; i++)
                if (cmpt[i] == pos && i > 1) return true;
            return false;
        }

        /// <summary>Check if the two Compartments are neighbor with or not</summary>
        /// <param name="pos1">1'st Compartment</param>
        /// <param name="pos2">2'nd Compartment</param>
        /// <returns>bool: is neighbor with or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private bool IsAdjacencyCmpt(CargoPosnBase pos1, CargoPosnBase pos2)
        { // Adjacent compartment in the same Main hold
            // adjacent position in bulk

            CargoPosnBase cmpt1 = pos1.FindParent(typeof(CargoCmpt));
            CargoPosnBase cmpt2 = pos2.FindParent(typeof(CargoCmpt));

            // Bulk
            if (pos1.IsBulkPosn())
            {
                if (Convert.ToInt32(pos2.Name) - Convert.ToInt32(pos1.Name) == 1)
                    return true;
                else
                    return false;
            }

            // Main Deck
            int count;
            int prevCount;
            foreach (CargoPosnBase hold in this)
            {
                count = prevCount = 0;
                foreach (CargoPosnBase cmpt in hold)
                {
                    if (cmpt.AreAllChildrenLeaf()) // Main Deck or bulk
                    {
                        if (cmpt == cmpt1)
                            prevCount = count;
                        if (cmpt == cmpt2)
                        {
                            if (isCombinedPos(pos1)) // CDL/CDR adjacent with ER/EL
                            {
                                if (prevCount + 2 == count) return true;
                                else return false;
                            }
                            else // C/CL/CR adjacent with D/DL/DR
                            {
                                if (prevCount + 1 == count) return true;
                                else return false;
                            }
                        }

                        count++;
                    }
                    else // Lower Deck
                        continue;
                }
            }
            return false;
        }

        /// <summary>Check if the two bays are neighbor with or not</summary>
        /// <param name="pos1">1'st bay</param>
        /// <param name="pos2">2'nd bay</param>
        /// <returns>bool: is neighbor with or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private bool IsAdjacencyBay(CargoPosnBase pos1, CargoPosnBase pos2)
        { // Adjacent posn in Lower Deck
            CargoPosnBase bay1 = pos1.FindParent(typeof(CargoBay));
            CargoPosnBase bay2 = pos2.FindParent(typeof(CargoBay));
            AirtypeCargoVersionLoadVersionCmp[] cmpList =
                theFlight.ACType.GetLoadVersion(theFlight.LoadVersion).cmp;
            if (cmpList == null) return false;
            int count = 0;
            int prevCount = 0;
            foreach (AirtypeCargoVersionLoadVersionCmp cmp in cmpList)
            {
                foreach (AirtypeCargoVersionLoadVersionCmpPos posn in cmp.pos)
                {
                    if (posn.name == bay1.Name) prevCount = count;
                    if (posn.name == bay2.Name)
                    {
                        if (prevCount + 1 == count) return true;
                        else return false;
                    }
                    count++;
                }
            }
            return false;
        }

        /// <summary>Check if the positions are neighbor with or not</summary>
        /// <param name="pos1">1'st  position </param>
        /// <param name="pos2">2'nd  position </param>
        /// <returns>bool: is neighbor with or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private bool IsAdjacency(CargoPosnBase pos1, CargoPosnBase pos2)
        {
            // identical posn must be neighbor with each other
            if (pos1 == pos2) return true;

            //different Hold can not be neighbor with each other
            if (pos1.FindParent(typeof(CargoHold)) != pos2.FindParent(typeof(CargoHold)))
                return false; //

            // exclude cases such as: CL/DR, CR/DL, CDL/ER, or CDR/EL
            // but (41L,41R) or (CDL,CDR/CR) must be adjancent
            string name1 = pos1.Name;
            string name2 = pos2.Name;
            string tmpname;
            if ((name1.Length > 1 && name1.EndsWith("L") &&
                name2.Length > 1 && name2.EndsWith("R")) ||
                (name1.Length > 1 && name1.EndsWith("R") &&
                    name2.Length > 1 && name2.EndsWith("L")))
            {
                //guarantee lenght of name1 must be greater than or equal to length of name2
                if (name1.Length < name2.Length)
                {
                    tmpname = name1;
                    name1 = name2;
                    name2 = tmpname;
                }
                if (name1.Substring(0, name1.Length - 1).IndexOf(name2.Substring(0, name2.Length - 1)) != -1)
                    return true;

                return false;
            }


            // Guarantee the position of pos1 is before pos2
            // Sort
            if (name1.CompareTo(name2) > 0)
            {
                CargoPosnBase tmp = pos2;
                pos2 = pos1;
                pos1 = tmp;
            }

            // two posn in the same cmpt
            //   adjacent position
            // two posn in the adjacent cmpt(that are in the same hold)
            //   the last + the first
            //
            // begin with hold
            //    Main Deck and Bulk: use CmptList: hold -> cmpt -> posn
            //    Lower Deck: use FDB LoadVersion
            //    foreach cmpt/bay
            //         C/CL/CR/CDL/CDR or 23/23L/23R/22P check sp goods within posn first
            //         and then cross check with sp goods in the next posn
            if (pos1.FindParent(typeof(CargoCmpt)).AreAllChildrenLeaf()) // belong to Main Deck/Bulk
                return IsAdjacencyCmpt(pos1, pos2);
            else // Lower Deck, checking by FDB LoadVersion
                return IsAdjacencyBay(pos1, pos2);

        }

        /// <summary>check if the loading weight of CargoPosnMgr commits to 'Combined Load Limits' or not</summary>
        /// <remarks>Check Combined Load Limitation</remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void CheckCombinedLoadLimits()
        {
            AirTypeEx theAirType = theFlight.ACType;
            if (theAirType.CargoFormula != null)
            {
                string delimStr = " +-*\\";
                char[] delimiter = delimStr.ToCharArray();

                //check if over the Combined Load Limitation or not
                foreach (CargoFormulaInfo formula in theAirType.CargoFormula.info)
                {
                    string[] exp = formula.cbwFor.Split(delimiter);
                    double combinedWt = 0;
                    //calculate combind load limit
                    foreach (string token in exp)
                    {
                        CargoPosnBase posn = this.Find(token);
                        if (posn != null)
                            combinedWt += posn.getWeight();
                    } //foreach
                    if (combinedWt > formula.cbWt)
                    {
                        throw (new Exception(string.Format(EWBSCoreWarnings.CgoExceedsCombinedLoadLmt_2, formula.cbwFor, formula.cbWt)));
                    }
                } //foreach

                //#BR07007 <--  					
                //Fio 2007/04/10 Start-----------				
                double combinedWt2 = 0;
                foreach (CargoFormulaInfo formula in theAirType.CargoFormula.info)
                {
                    string[] exp = formula.cbwFor.Split(delimiter);//cbwFor

                    //calculate combind load limit

                    foreach (string token in exp)
                    {
                        CargoPosnBase posn = this.Find(token);
                        if (posn != null)
                            combinedWt2 += posn.getWeight();
                    } //foreach

                    if (formula.id == "D")
                    {
                        if (theAirType.name == "BR74S" && theFlight.TOW > 366400 && combinedWt2 < 2268)
                        {
                            throw (new Exception(string.Format(EWBSCoreWarnings.CgoExceedsCombinedLoadLmt_3, formula.cuwFor, "2268", combinedWt2)));
                        }
                        break;
                    }
                } //foreach
                combinedWt2 = 0;
                //Fio 2007/04/10 End-----------
                //#BR07007 -->  		
            }
        }

        /// <summary>check the loading weight of CargoPosnMgr commits to 'Cumulative Load Limits' or not</summary>
        /// <param name="bIncreasedCumulativeLoad">'Increased Cumulative Load' or not</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void CheckCumulativeLoadLimits(bool bIncreasedCumulativeLoad)
        {
            AirTypeEx theAirType = theFlight.ACType;
            if (theAirType.CargoFormula != null)
            {
                //collect all of combined load limit
                Hashtable vars = new Hashtable();
                Hashtable varNames = new Hashtable();

                string delimStr = " +-*\\";
                char[] delimiter = delimStr.ToCharArray();

                //BR10118<-- catch exception
                try
                {
                    //BR10118-->
                    // compute Combined Load Limit
                    foreach (CargoFormulaInfo formula in theAirType.CargoFormula.info)
                    {
                        if (formula.cbwFor.Trim() == "") continue;

                        string[] exp = formula.cbwFor.Split(delimiter);
                        double combinedWt = 0;
                        //calculate combind load limit
                        foreach (string token in exp)
                        {
                            CargoPosnBase posn = this.Find(token);
                            if (posn != null)
                                combinedWt += posn.getWeight();
                        } //foreach
                        vars[formula.id] = combinedWt.ToString();
                        varNames[formula.id] = formula.cbwFor.ToString();
                        exp = null; //#BR17164 THOMAS
                    } //foreach
                    System.GC.Collect(); //#BR17164 THOMAS GC強制回收
                    //check if over the restriction of either Normal Cumulative Load Limit or Increased Cumulative Load Limit or not。
                    foreach (CargoFormulaInfo formula in theAirType.CargoFormula.info)
                    {
                        if (formula.cuwFor.Trim() == "") continue;

                        double cumulativeWt = 0;
                        string itemNames = "";
                        string[] exp = formula.cuwFor.Split(delimiter);

                        //calculate cumulative load limit
                        foreach (string token in exp)
                        {
                            string val = vars[token].ToString();
                            if (val != "")
                            {
                                cumulativeWt += Convert.ToDouble(val);
                            }
                            if (itemNames != "") itemNames += "+";
                            itemNames += varNames[token].ToString();
                        } //foreach
                        if (bIncreasedCumulativeLoad)
                        {
                            if (cumulativeWt > formula.icuWt)
                            {
                                throw (new Exception(string.Format(EWBSCoreWarnings.OverIncreasedCumulativeLoad_3, itemNames, cumulativeWt, formula.icuWt)));
                            }
                        }
                        else
                        {
                            if (cumulativeWt > formula.ncuWt)
                            {
                               throw (new Exception(string.Format(EWBSCoreWarnings.OverCumulativeLoad_3, itemNames, cumulativeWt, formula.ncuWt)));
                            }
                        }
                       exp = null; //#BR17164 THOMAS
                    } //foreach
                    //BR10118<--
                    System.GC.Collect(); //#BR17164 THOMAS GC強制回收
                }
                catch (Exception ex_1)
                {
                    throw (new Exception(EWBSCoreWarnings.CheckCumulativeLoadLimits));

                    //丟出例外 無Message String ，暫用空字串
                    throw new Exception("", ex_1);

                }
                //BR10118-->
            }
        }

        /// <summary>mark if each position is 'No Fit' or not</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void MarkNoFit()
        {
            AirTypeEx theAirType = theFlight.ACType;

            //Find load version
            AirtypeCargoVersionLoadVersion thisVersion = theAirType.GetLoadVersion(theFlight.LoadVersion);
            if (thisVersion != null) //found
            {
                ArrayList noFitList = new ArrayList();
                ArrayList foundList = new ArrayList();
                ArrayList checkedList = new ArrayList();

                ArrayList posnList = new ArrayList();
                posnList.AddRange(GenVisibleList(0));
                posnList.AddRange(Find(typeof(CargoPosn)));
                foreach (CargoPosnBase posn in posnList)
                {
                    posn.NoFit = !posn.Occupied;
                }

                //Check no fit
                foreach (AirtypeCargoVersionLoadVersionNofitPattern pattern in theAirType.SortNofitPatterns(thisVersion.nofit.pattern))
                {
                    noFitList.Clear(); //reset first
                    foundList.Clear();

                    int count = 0;

                    for (int i = 0; i + 2 < pattern.names.Length; )
                    {
                        if (Char.IsLetterOrDigit(pattern.names[i]))
                        {
                            string name = pattern.names.Substring(i, 3); //All are 3 characters name, like 11L,11R
                            CargoPosnBase posn = Find(name);

                            if (posn == null) break;

                            count += 1;

                            if (posn.Occupied || posn.Parent.Occupied)
                            {
                                if (checkedList.IndexOf(posn) < 0)
                                    foundList.Add(posn);
                            }
                            else if (posn.NoFit)
                                noFitList.Add(posn);
                            i += 3;
                        }
                        else
                        {
                            i++; //ignore the delimitor
                        }
                    } //for each posn

                    checkedList.AddRange(foundList);

                    if (count != noFitList.Count && foundList.Count > 0)
                    {
                        //marked 'No Fit'
                        foreach (CargoPosnBase posn in noFitList)
                        {
                            posn.NoFit = false;
                        }
                    }

                } //foreach pattern
                noFitList = null;   //#BR17164 THOMAS
                foundList = null; //#BR17164 THOMAS
                checkedList = null; //#BR17164 THOMAS
                posnList = null; //#BR17164 THOMAS
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            }
        }
        
        /// <summary>check if position distribution follow the 'No Fit' rule or not.
        /// Put the invalid position object into the returning ArrayList</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ArrayList CheckNoFit()
        {
            ArrayList noFitList = new ArrayList();
            //Mark NO fit
            MarkNoFit();
            ArrayList posnList = new ArrayList();
            posnList.AddRange(GenVisibleList(0));
            posnList.AddRange(Find(typeof(CargoPosn)));
            foreach (CargoPosnBase posn in posnList)
            {
                //violate No Fit
                if (!posn.NoFit && !posn.Occupied) noFitList.Add(posn);
            }
            if (noFitList.Count > 0)
                return noFitList; //No Fit
            return null;
        }

        /// <summary>check the restriction of single side summation </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void CheckLockMaxWt()
        {
            AirTypeEx theAirType = theFlight.ACType;

            //Find load version
            AirtypeCargoVersionLoadVersion thisVersion = theAirType.GetLoadVersion(theFlight.LoadVersion);
            if (thisVersion != null) //found
            {
                //Check no fit
                foreach (AirtypeCargoVersionLoadVersionNofitPattern pattern in thisVersion.nofit.pattern)
                {
                    if (!pattern.maxWtSpecified) continue;
                    double maxWt = 0f;
                    for (int i = 0; i + 2 < pattern.names.Length; )
                    {
                        if (Char.IsLetterOrDigit(pattern.names[i]))
                        {
                            string name = pattern.names.Substring(i, 3); //All are 3 characters name, like 11L,11R
                            CargoPosnBase posn = Find(name);

                            if (posn is CargoPosn)
                            {
                                if (posn.Occupied)
                                {
                                    maxWt += posn.getWeight();
                                }
                                else if (posn.Parent.Occupied)
                                {
                                    maxWt += posn.Parent.getWeight() / 2;
                                }
                            }
                            i += 3;
                        }
                        else
                        {
                            i++; //ignore the delimitor
                        }
                    } //for each posn

                    if (maxWt > Convert.ToDouble(pattern.maxWt) && pattern.maxWt > 0u)
                    {
                        throw (
                            new Exception(string.Format(EWBSCoreWarnings.PatternExceedsWtLmt_2,
                                                        pattern.names,
                                                        pattern.maxWt)));
                    }
                } //foreach patterns

            }
        }

        //<!--#BR17202 THOMAS 垂直限重判斷 & 左右平衡
        /// <summary>
        /// 限重判斷 
        /// </summary>
        /// <returns></returns>
        public void CheckMdLd_WtLimit()
        {
            CargoPosnMgr CargoPosnMgr = theFlight.CargoPosnMgr;

            CargoPosnBase[] CargoList = CargoPosnMgr.GenVisibleList(0); //已放在Posn上的貨物清單
            CargoPosnBase[] PosnList = CargoPosnMgr.GetCmptList(); //

            ArrayList LowerBayPosnList = new ArrayList();//Lower Deck Bay Position List
            ArrayList MainBayPosnList = new ArrayList();//Main Deck Bay Position List
            ArrayList BulkBayPosnList = new ArrayList(); //Bulk Deck Bay Position List

            ArrayList LowerDeckList = new ArrayList(); //Lower Dack 每一個 bay的重量
            ArrayList MainDeckList = new ArrayList();//Main Dack 每一個 bay的重量
            ArrayList BulkDeckList = new ArrayList();//Bulk 的總重量

            #region 讀取XML檔案  LargePalletOverLap 區段
            string PercentcurrentPath = Directory.GetCurrentDirectory() + "\\FDB";
            string PercentXmlFileName = PercentcurrentPath + "\\" + theFlight.ACType.name + ".xml";
            XmlDocument PercentXmlDoc = new XmlDocument();
            PercentXmlDoc.Load(PercentXmlFileName);

            XmlNodeList LargePalletOverLapNodeList = PercentXmlDoc.SelectNodes("/Airtype/LargePalletOverLap/ULD");
            #endregion

            #region Bay List

            //Lower Deck Bay List
            foreach (CargoPosnBase posn in PosnList)
            {
                if (posn.IsLowerDeckPosn() && posn.Name != "5")
                    foreach (CargoPosnBase p in posn)
                    {
                        if (p.IsLowerDeckPosn())
                        {
                            LowerBayPosnList.Add(new CargoLimit
                            {
                                PosnName = p.Name,
                            });
                        }
                    }
            }

            //Main Deck Bay List
            foreach (CargoPosnBase posn in PosnList)
            {
                if (posn.IsMainDeckPosn())
                    MainBayPosnList.Add(new CargoLimit
                    {
                        PosnName = posn.Name,
                    });
            }

            // Bulk Deck Bay List
            foreach (CargoPosnBase posn in PosnList)
            {
                if (posn.IsLowerDeckPosn() && posn.Name == "5")
                    BulkBayPosnList.Add(new CargoLimit
                    {
                        PosnName = posn.Name,
                    });
            }
            #endregion

            #region Cargo List
            //Lower Deck  Cargo List
            foreach (CargoLimit BayPosn in LowerBayPosnList)
            {
                string Cargo_Basecode = "";
                string Cargo_UldType = "";
                foreach (CargoPosnBase p in CargoList)
                {
                    if (BayPosn.PosnName == p.Name && p.CargoList.Count > 0) //取得 ULD的 Basecode
                    {
                        Cargo_Basecode = p.CargoList[0].UserUldBaseCode;
                        Cargo_UldType = p.CargoList[0].UserUldTpye;
                    }
                }

                LowerDeckList.Add(new CargoLimit
                {
                    PosnName = BayPosn.PosnName,
                    BaseCode = Cargo_Basecode,
                    UldType = Cargo_UldType,
                    TtlWt = CargoPosnMgr.Find(BayPosn.PosnName).getBayWt(LargePalletOverLapNodeList),
                });
                Cargo_Basecode = "";
                Cargo_UldType = "";
            }

            //Main Deck  Cargo List
            foreach (CargoLimit BayPosn in MainBayPosnList)
            {
                string Cargo_Basecode = "";
                string Cargo_UldType = "";
                foreach (CargoPosnBase p in CargoList)
                {
                    if (BayPosn.PosnName == p.Name && p.CargoList.Count > 0)
                    {
                        Cargo_Basecode = p.CargoList[0].UserUldBaseCode;
                        Cargo_UldType = p.CargoList[0].UserUldTpye;
                    }
                }



                MainDeckList.Add(new CargoLimit
                {
                    PosnName = BayPosn.PosnName,
                    BaseCode = Cargo_Basecode,
                    UldType = Cargo_UldType,
                    TtlWt = CargoPosnMgr.Find(BayPosn.PosnName).getBayWt(LargePalletOverLapNodeList),
                });
                Cargo_Basecode = "";
                Cargo_UldType = "";
            }

            //Bulk Deck  Cargo List
            foreach (CargoLimit BayPosn in BulkBayPosnList)
            {
                string Cargo_Basecode = "";
                string Cargo_UldType = "";
                foreach (CargoPosnBase p in CargoList)
                {
                    if (BayPosn.PosnName == p.Name && p.CargoList.Count > 0)
                    {
                        Cargo_Basecode = p.CargoList[0].UserUldBaseCode;
                        Cargo_UldType = p.CargoList[0].UserUldTpye;
                    }
                }

                BulkDeckList.Add(new CargoLimit
                {
                    PosnName = BayPosn.PosnName,
                    BaseCode = Cargo_Basecode,
                    UldType = Cargo_UldType,
                    TtlWt = CargoPosnMgr.Find(BayPosn.PosnName).getBayWt(LargePalletOverLapNodeList),
                });
                Cargo_Basecode = "";
                Cargo_UldType = "";
            }
            #endregion

            #region 讀取XML檔案 UldBulkLimit區段
            string currentPath = Directory.GetCurrentDirectory() + "\\FDB";
            string XmlFileName = currentPath + "\\" + theFlight.ACType.name + ".xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(XmlFileName);

            XmlNodeList PosnDefineNodeList = xmlDoc.SelectNodes("/Airtype/UldBulkLimit/PosnDefine/Posn");
            #endregion

            #region 比對
            foreach (XmlNode PosnDefineNode in PosnDefineNodeList)
            {
                XmlAttributeCollection PosnDefineNodeAt = PosnDefineNode.Attributes; //取Posn屬性
                string strLd = PosnDefineNodeAt.Item(0).Value;
                string strMd = PosnDefineNodeAt.Item(1).Value;

                #region Lower Deck <-> Main Deck
                foreach (CargoLimit LowerDeck in LowerDeckList)
                {
                    if (LowerDeck.PosnName == strLd)
                    {
                        int i = 0;
                        XmlNodeList UldDefineNodeList = xmlDoc.SelectNodes("/Airtype/UldBulkLimit/UldLimit/ULD");
                        foreach (XmlNode UldDefineNode in UldDefineNodeList)
                        {
                            XmlAttributeCollection UldDefineNodeAt = UldDefineNode.Attributes;  //取ULD Basecode屬性節點
                            if (i == 2)
                            {
                                XmlNodeList NodeList = UldDefineNode.ChildNodes;
                                foreach (XmlNode Node in NodeList)
                                {
                                    XmlAttributeCollection ChildNodelAt = Node.Attributes;
                                    double LH_Min = Convert.ToDouble(ChildNodelAt.Item(0).Value); //LowHoldMin
                                    double LH_Max = Convert.ToDouble(ChildNodelAt.Item(1).Value);//LowHoldMax
                                    double MD_Max = Convert.ToDouble(ChildNodelAt.Item(2).Value);//MainDeckMax

                                    //LD貨物重量區間，MD限重判斷
                                    if (LowerDeck.TtlWt >= LH_Min && LowerDeck.TtlWt <= LH_Max)
                                    {
                                        //Console.WriteLine(LH_Min + "~" + LH_Max + " ->" + MD_Max);
                                        string[] strMdList = strMd.Split(',');
                                        foreach (var md in strMdList)
                                        {
                                            foreach (CargoLimit MainDeck in MainDeckList)
                                            {
                                                if (MainDeck.PosnName == md)
                                                {
                                                    if (MainDeck.TtlWt > MD_Max)
                                                    {
                                                        string Msg = string.Format("{0} Weight Range {1} ~ {2} ， {3} Bay Weight Limit {4}", LowerDeck.PosnName, LH_Min, LH_Max, MainDeck.PosnName, MD_Max);
                                                        throw (new Exception(Msg));
                                                        //MessageBox.Show(Msg);
                                                        //return false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (UldDefineNodeAt.Item(0).Value.IndexOf(LowerDeck.BaseCode) > -1 && LowerDeck.BaseCode != "")
                            {
                                XmlNodeList NodeList = UldDefineNode.ChildNodes;
                                foreach (XmlNode Node in NodeList)
                                {
                                    XmlAttributeCollection ChildNodelAt = Node.Attributes;
                                    double LH_Min = Convert.ToDouble(ChildNodelAt.Item(0).Value); //LowHoldMin
                                    double LH_Max = Convert.ToDouble(ChildNodelAt.Item(1).Value);//LowHoldMax
                                    double MD_Max = Convert.ToDouble(ChildNodelAt.Item(2).Value);//MainDeckMax

                                    //計算LD貨物重量區間，以及MD限重判斷
                                    if (LowerDeck.TtlWt >= LH_Min && LowerDeck.TtlWt <= LH_Max)
                                    {
                                        //  Console.WriteLine(LH_Min + "~" + LH_Max + " ->" + MD_Max);
                                        string[] strMdList = strMd.Split(',');
                                        foreach (var md in strMdList)
                                        {
                                            foreach (CargoLimit MainDeck in MainDeckList)
                                            {
                                                if (MainDeck.PosnName == md)
                                                {
                                                    if (MainDeck.TtlWt > MD_Max)
                                                    {
                                                        string Msg = string.Format("{0} Weight Range {1} ~ {2} ， {3} Bay Weight Limit {4}", LowerDeck.PosnName, LH_Min, LH_Max, MainDeck.PosnName, MD_Max);
                                                        throw (new Exception(Msg));
                                                        //MessageBox.Show(Msg);
                                                        //return false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else { i++; }
                        }
                    }
                }
                #endregion

                #region Bulk Deck <-> Main Deck
                foreach (CargoLimit BulkDeck in BulkDeckList)
                {
                    if (BulkDeck.PosnName == strLd) //5
                    {
                        string[] strMdList = strMd.Split(','); //M,P,R
                        foreach (var md in strMdList)
                        {
                            foreach (CargoLimit MainDeck in MainDeckList)
                            {
                                if (MainDeck.PosnName == md)
                                {
                                    XmlNodeList MainDeckNodeList = xmlDoc.SelectNodes("/Airtype/UldBulkLimit/BulkLimit/MainDeck");
                                    foreach (XmlNode MainDeckNode in MainDeckNodeList) //M,P or R
                                    {
                                        XmlAttributeCollection MainDeckNodeAt = MainDeckNode.Attributes;  //取MainDeck屬性節點
                                        if (MainDeckNodeAt.Item(0).Value.IndexOf(MainDeck.PosnName) > -1)
                                        {
                                            XmlNodeList UldBaseCodeNodeList = MainDeckNode.ChildNodes;   //Main Deck ULD Basecode
                                            foreach (XmlNode UldBaseCodeNode in UldBaseCodeNodeList)
                                            {
                                                XmlAttributeCollection UldBaseCodeNodeAt = UldBaseCodeNode.Attributes;  //取ULD Basecode屬性節點
                                                //判斷節點ULD Basecode 屬性是否與貨物Main Deck ULD Basecode相同 (A或M)
                                                if (UldBaseCodeNodeAt.Item(0).Value.IndexOf(MainDeck.BaseCode) > -1)
                                                {
                                                    XmlNodeList ChildNodeList = UldBaseCodeNode.ChildNodes;   //ULD Basecode
                                                    foreach (XmlNode ChildNode in ChildNodeList)
                                                    {
                                                        XmlAttributeCollection ChildNodelAt = ChildNode.Attributes;
                                                        double LH_Min = Convert.ToDouble(ChildNodelAt.Item(0).Value); //LowHoldMin
                                                        double LH_Max = Convert.ToDouble(ChildNodelAt.Item(1).Value);//LowHoldMax
                                                        double MD_Max = Convert.ToDouble(ChildNodelAt.Item(2).Value);//MainDeckMax

                                                        //計算LD貨物重量區間，以及MD限重判斷
                                                        if (BulkDeck.TtlWt >= LH_Min && BulkDeck.TtlWt <= LH_Max)
                                                        {

                                                            if (MainDeck.TtlWt > MD_Max)
                                                            {
                                                                Console.WriteLine(LH_Min + "~" + LH_Max + " ->" + MD_Max);
                                                                //string Msg = string.Format("{0} Weight Range {1} ~ {2} ， {3} Bay Weight Limit {4}", BulkDeck.PosnName, LH_Min, LH_Max, MainDeck.PosnName, MD_Max);
                                                                string Msg = string.Format(" BULK Weight Range {0} ~ {1} ， {2} Bay Weight Limit {3}", LH_Min, LH_Max, MainDeck.PosnName, MD_Max);
                                                                throw (new Exception(Msg));
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }

                    }
                }
                #endregion

            }
            #endregion
        }

        /// <summary>
        /// Unsymmetric Check fo BR77X
        /// </summary>
        public void CheckUnsymmetric(XmlNodeList LargePalletOverLapNodeList)
        {
            AirTypeEx theAirType = theFlight.ACType;
            if (theAirType.asy != null)
            {
                //for all of Unsymmetric Load Condition
                foreach (AirtypeUnsymmetrical asy in theAirType.asy)
                {
                    foreach (AirtypeUnsymmetricalPos pos in asy.pos)
                    {
                        //find posn to apply Unsymmetric Load limitation
                        string posnList = pos.name;

                        for (int i = 0; i < posnList.Length; i++)
                        {
                            if (Char.IsLetterOrDigit(posnList[i])) //for specific posn
                            {
                                //find posn name
                                string posnname = new string(posnList[i], 1); //default name

                                string posnname_prev = new string(posnList[i], 1); //************default Prev name
                                if (i > 0 && posnname != "J")
                                {
                                    posnname_prev = new string(posnList[i - 1], 1); //******Prev name 
                                }

                                for (int j = i + 1; j < posnList.Length; j++)
                                    if (!Char.IsLetterOrDigit(posnList[j]))
                                        posnname = posnList.Substring(i, j - i);

                                //Find Unsymmetric Load limitation
                                CargoPosnBase thisPosn = this.Find(posnname);
                                CargoPosnBase thisPosn_prev = this.Find(posnname_prev); //*******

                                if (thisPosn != null && thisPosn.Count > 0 && thisPosn.AreAllChildrenLeaf())
                                {
                                    double max, min;
                                    if (thisPosn.Occupied)
                                    {
                                        max = min = thisPosn.getBayWt(LargePalletOverLapNodeList) / 2;
                                    }
                                    else
                                    {
                                        double left = 0f;
                                        double right = 0f;
                                        int child;

                                        child = 0;
                                        if (child < thisPosn.Count) left += (thisPosn[child] as CargoPosnBase).getBayWt(LargePalletOverLapNodeList);
                                        child = 1;
                                        if (child < thisPosn.Count) right += (thisPosn[child] as CargoPosnBase).getBayWt(LargePalletOverLapNodeList);
                                        child = 2;
                                        if (child < thisPosn.Count)
                                        {
                                            double p = 1;
                                            if ((thisPosn[child] as CargoPosnBase).CargoList.Count > 0)
                                            {
                                                var cbs = (thisPosn[child] as CargoPosnBase).CargoList[0].UserUldBaseCode; //貨物的BASECODE
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
                                            left += (thisPosn[child] as CargoPosnBase).getBayWt(LargePalletOverLapNodeList) / 2 * p;
                                        }
                                        child = 3;
                                        if (child < thisPosn.Count)
                                        {
                                            double p = 1;
                                            if ((thisPosn[child] as CargoPosnBase).CargoList.Count > 0)
                                            {
                                                var cbs = (thisPosn[child] as CargoPosnBase).CargoList[0].UserUldBaseCode; //貨物的BASECODE
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
                                            right += (thisPosn[child] as CargoPosnBase).getBayWt(LargePalletOverLapNodeList) / 2 * p;
                                        }
                                                                               
                                        //child = 4;
                                        //if (child < thisPosn.Count) left += (thisPosn[child] as CargoPosnBase).getBayWt() / 4;
                                        //if (child < thisPosn.Count) right += (thisPosn[child] as CargoPosnBase).getBayWt() / 4;

                                        if (!thisPosn_prev.Occupied && thisPosn_prev.Count > 2 && (thisPosn.Name !=thisPosn_prev.Name || thisPosn_prev.Name !="J"))
                                        {
                                            child = 2;
                                            if (child < thisPosn_prev.Count)
                                            {
                                                double p = 1;
                                                if ((thisPosn_prev[child] as CargoPosnBase).CargoList.Count > 0)
                                                {
                                                    var cbs = (thisPosn_prev[child] as CargoPosnBase).CargoList[0].UserUldBaseCode; //貨物的BASECODE
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
                                                left += (thisPosn_prev[child] as CargoPosnBase).getBayWt(LargePalletOverLapNodeList) / 2 * p;
                                            } 
                                            child = 3;
                                            if (child < thisPosn_prev.Count)
                                            {
                                                double p = 1;
                                                if ((thisPosn_prev[child] as CargoPosnBase).CargoList.Count > 0)
                                                {
                                                    var cbs = (thisPosn_prev[child] as CargoPosnBase).CargoList[0].UserUldBaseCode; //貨物的BASECODE
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
                                                right += (thisPosn_prev[child] as CargoPosnBase).getBayWt(LargePalletOverLapNodeList) / 2 * p;
                                            }
                                        }

                                        max = Math.Max(left, right);
                                        min = Math.Min(left, right);
                                    }

                                    for (int candidate = 0; candidate < pos.info.Length; candidate++)
                                    {
                                        if (max <= pos.info[candidate].large || ( max > pos.info[candidate].large && candidate == pos.info.Length-1))
                                        {
                                            if (min > pos.info[candidate].small)
                                            {
                                                throw (new Exception(string.Format(EWBSCoreWarnings.UnsymmetricLoadForPosn_3, posnname, pos.info[candidate].large, pos.info[candidate].small)));
                                            }
                                            break;
                                        }
                                    } //while
                                }
                                posnname = null; //#BR17164 THOMAS
                                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
                            }
                        } //for
                    } //foreach
                } //foreach
            }

            //return true;
        }
        //#BR17202  -->

        /// <summary>check Unsymmetric Load Limits</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void CheckUnsymmetric()
        {
              AirTypeEx theAirType = theFlight.ACType;
              if (theAirType.asy != null)
                {
                    //for all of Unsymmetric Load Condition
                    foreach (AirtypeUnsymmetrical asy in theAirType.asy)
                    {
                        foreach (AirtypeUnsymmetricalPos pos in asy.pos)
                        {
                            //find posn to apply Unsymmetric Load limitation
                            string posnList = pos.name;

                            for (int i = 0; i < posnList.Length; i++)
                            {
                                if (Char.IsLetterOrDigit(posnList[i])) //for specific posn
                                {
                                    //find posn name
                                    string posnname = new string(posnList[i], 1); //default name

                                    //<!--BR17202 THOMAS 修正CheckUnsymmetric錯誤，
                                    string posnname_prev = new string(posnList[i], 1); //************default Prev name
                                    if (i > 0)
                                    {
                                        posnname_prev = new string(posnList[i - 1], 1); //******Prev name 
                                    }
                                    //#BR17202 THOMAS -->

                                    for (int j = i + 1; j < posnList.Length; j++)
                                        if (!Char.IsLetterOrDigit(posnList[j]))
                                            posnname = posnList.Substring(i, j - i);

                                    //Find Unsymmetric Load limitation
                                    CargoPosnBase thisPosn = this.Find(posnname);

                                    //<!--BR17202 THOMAS 修正CheckUnsymmetric錯誤，必須比對前一個位置
                                    CargoPosnBase thisPosn_prev = this.Find(posnname_prev);
                                    //#BR17202 THOMAS -->

                                    if (thisPosn != null && thisPosn.Count > 0 && thisPosn.AreAllChildrenLeaf())
                                    {
                                        double max, min;
                                        if (thisPosn.Occupied)
                                        {
                                            max = min = thisPosn.getGrossWt() / 2;
                                        }
                                        else
                                        {
                                            double left = 0f;
                                            double right = 0f;
                                            int child;

                                            child = 0;
                                            if (child < thisPosn.Count) left += (thisPosn[child] as CargoPosnBase).getGrossWt();
                                            child = 1;
                                            if (child < thisPosn.Count) right += (thisPosn[child] as CargoPosnBase).getGrossWt();
                                            child = 2;
                                            if (child < thisPosn.Count) left += (thisPosn[child] as CargoPosnBase).getGrossWt() / 2;
                                            child = 3;
                                            if (child < thisPosn.Count) right += (thisPosn[child] as CargoPosnBase).getGrossWt() / 2;

                                            //<!--BR17202 THOMAS 修正CheckUnsymmetric錯誤，必須比對前一個位置
                                            if (!thisPosn_prev.Occupied && thisPosn_prev.Count > 2 && thisPosn.Name !=thisPosn_prev.Name)                                            {
                                               child = 2;
                                               if (child < thisPosn_prev.Count) left += (thisPosn_prev[child] as CargoPosnBase).getGrossWt() / 2;
                                                child = 3;
                                                if (child < thisPosn_prev.Count) right += (thisPosn_prev[child] as CargoPosnBase).getGrossWt() / 2;
                                           }
                                            //#BR17202 THOMAS -->

                                            max = Math.Max(left, right);
                                            min = Math.Min(left, right);
                                        }

                                        for (int candidate = 0; candidate < pos.info.Length; candidate++)
                                        {
                                            if (max <= pos.info[candidate].large)
                                            {
                                                if (min > pos.info[candidate].small)
                                                {
                                                    throw (new Exception(string.Format(EWBSCoreWarnings.UnsymmetricLoadForPosn_3, posnname, pos.info[candidate].large, pos.info[candidate].small)));
                                                }
                                                break;
                                            }
                                        } //while
                                    }

                                    posnname = null; //#BR17164 THOMAS
                                    System.GC.Collect();  //#BR17164 THOMAS 物件關閉後 Dispose 標記由GC強制回收      

                                }
                            } //for
                        } //foreach
                    } //foreach
                }
              //return true;
        }

        /// <summary>check Lateral Imbalance</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private void checkLateralImbalance()
        {
            AirTypeEx theAirType = theFlight.ACType;
            if (theAirType.CargoInfomation != null)
            {
                double diff = CalcLateralImbalance();


                int result = (int)Math.Abs(diff);


                //check if it over the cargo's Lateral Imbalance or not
                if (result > theAirType.CargoInfomation.info[0].maxAsy && theAirType.CargoInfomation.info[0].maxAsy > 0)
                {
                    throw (new Exception(string.Format(EWBSCoreWarnings.OverLateralImbalanceLmt_3,
                                                      (diff > 0f ? "Left" : "Right"),
                                                      result,
                                                      theAirType.CargoInfomation.info[0].maxAsy)));
                }
            }
        }

        /// <summary>
        /// check 4 ULDs of back deck in B777
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void checkB777Aft4P()
        {
            //B777
            if (FDB.Instance.GetSuperAcTypeName(theFlight.ACType.FullAirTypeName).Substring( 0,4).ToString()=="B777")
            {
                bool smallULD = false; //small ULD in front 
                string[] uldNmList = new string[] { "34P", "33P", "32P", "31P" };

                //<!--#BR17209 THOMAS  B777F ULD Posn 只有31P 32P
                if (theFlight.ACType.name == "BR77X") //貨機
                {
                     uldNmList = new string[] {  "32P", "31P" };
                }
                //#BR17209 -->

                foreach (string uldNm in uldNmList)
                {
                    CargoPosnBase posn = Find(uldNm);
                    if (!posn.Occupied) continue;
                    if (posn.CargoList[0] is Cargo)
                    {
                        Cargo cargo = posn.CargoList[0] as Cargo;
                        if (cargo.BaseCode == "M" && smallULD)
                            throw (new Exception(string.Format("\"PAG\" and \"PMC\" position sequence error.")));
                        if (cargo.BaseCode == "A") smallULD = true;
                    }
                }
            }
        }

        /// <summary>
        /// #BR18118 787 依版型檢查cmp 不能放置兩個 PMC
        /// </summary>
        public void check787TwoPMC()
        {
            if (FDB.Instance.GetSuperAcTypeName(theFlight.ACType.FullAirTypeName).Substring(0, 4).ToString() == "B787")
            {
                //<!-- #BR19007 Thomas 新增 BR781
                if (theFlight.ACType.name.Substring(0, 5) == "BR789" || theFlight.ACType.name.Substring(0, 5) == "BR781")
                {
                    if (theFlight.LoadVersion.Substring(1, 1) == "8")
                    {
                        bool ULDBaseCode = false;
                        string[] uldNmList = new string[] { "24P", "23P", "22P", "21P" };
                        foreach (string uldNm in uldNmList)
                        {
                            CargoPosnBase posn = Find(uldNm);

                            if (posn != null)
                            {
                                if (!posn.Occupied) continue;

                                if (posn.CargoList[0] is Cargo)
                                {
                                    Cargo cargo = posn.CargoList[0] as Cargo;

                                    if (cargo.BaseCode == "M" && ULDBaseCode)
                                        throw (new Exception(string.Format("The remaining space is not enough in \"Cmpt 2 \"." + "\r\n" + "Not allow to load " + "2 \"M\" (basecode)")));

                                    if (cargo.BaseCode == "M") ULDBaseCode = true;
                                }
                            }
                        }
                    }
                    
                    if (theFlight.LoadVersion.Substring(2, 1) == "8")
                    {
                        bool ULDBaseCode = false;
                        string[] uldNmList = new string[] { "34P", "33P", "32P", "31P" };
                        foreach (string uldNm in uldNmList)
                        {
                            CargoPosnBase posn = Find(uldNm);

                            if (posn != null)
                            {
                                if (!posn.Occupied) continue;

                                if (posn.CargoList[0] is Cargo)
                                {
                                    Cargo cargo = posn.CargoList[0] as Cargo;

                                    if (cargo.BaseCode == "M" && ULDBaseCode)
                                        throw (new Exception(string.Format("The remaining space is not enough in \"Cmpt 3 \"." +  "\r\n" + "Not allow to load " + "2 \"M\" (basecode)")));

                                    if (cargo.BaseCode == "M") ULDBaseCode = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        //<!-- #BR17207  B777-200下前貨艙POSN 11-25，PLA不能擺放25/31 (原BR16105搬到此處)
        /// <summary>
        /// 
        /// </summary>
        public void checkBR77Posn()
        {
            //<!--#BR16105/CHERRY/20160509 <-- 
            if (theFlight.ACType.name.Substring(0, 4) == "BR77")
            {
                CargoPosnMgr cargoPosnMgr = theFlight.CargoPosnMgr;
                CargoPosnBase[] cargoPosnList = cargoPosnMgr.GenVisibleList(0); //已放在Posn上的貨物清單
                if (theFlight.ACType.name.Substring(0, 5) == "BR77X")
                {
                    foreach (CargoPosnBase posn in cargoPosnList)
                    {
                        if (posn.Name == "25" || posn.Name == "31")
                        {
                            if (posn.CargoList.Count > 0)
                            {
                                if (posn.CargoList[0].UserUldTpye == "PLA" || posn.CargoList[0].UserUldTpye == "FLA")
                                {
                                    //MessageBox.Show("It is NOT ALLOWED to load PLA/FLA in 31 bay and 25 bay !!");
                                    string Msg = string.Format("It is NOT ALLOWED to load PLA/FLA in 31 bay and 25 bay !!");
                                    throw (new Exception(Msg));
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (CargoPosnBase posn in cargoPosnList)
                    {
                        if (posn.Name == "28" || posn.Name == "31")
                        {
                            if (posn.CargoList.Count > 0)
                            {
                                if (posn.CargoList[0].UserUldTpye == "PLA" || posn.CargoList[0].UserUldTpye == "FLA")
                                {
                                    // MessageBox.Show("It is NOT ALLOWED to load PLA/FLA in 31 bay and 28 bay !!");
                                    string Msg = string.Format("It is NOT ALLOWED to load PLA/FLA in 31 bay and 28 bay !!");
                                    throw (new Exception(Msg));
                                }
                            }
                        }
                    }
                }
            }
            //#BR16105 -->
        }
        //#BR17207 -->

        //<!-- #BR17210  A332機型(B-16307~B16312) 31P放ECP(危險品)時出現警示訊息。 ; (原BR16106搬到此處)
        /// <summary>
        /// A332機型(B-16307~B16312) 31P放ECP(危險品)時出現警示訊息。
        /// </summary>
        public void checkBR33ECP()
        {
            if (theFlight.ACType.name.Substring(0, 4) == "BR33")   //2016.05.19 Cherry for other airline ACRegNo
            {
                int iRegNo = Convert.ToInt32(theFlight.ACRegNo.Substring(2, 5));  //B-16307
                if (iRegNo <= 16312 && iRegNo >= 16307)   //B-16307-B-16312
                {
                    foreach (ICargo aCargo in this.theFlight.DeadloadList)
                    {
                        if (aCargo.Posn == "31P" && aCargo.SHC.IndexOf("ECP") >= 0)
                        {
                            MessageBox.Show("If possible, do not load ECP in 31P !!");
                        }
                    }
                }
            }
        }

        //BR11107<--
        /// <summary>
        /// check 5 ULDs of back deck in B333
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void checkB333Aft5P()
        {
            //B333
            if (theFlight.ACType.name == "BR333" && theFlight.LoadVersion.Substring(2, 1) == "5")
            {
                string[] uldNmList = new string[] { "41P" };
                foreach (string uldNm in uldNmList)
                {
                    CargoPosnBase posn = Find(uldNm);
                    if (!posn.Occupied) continue;
                    if (posn.CargoList[0] is Cargo)
                    {
                        Cargo cargo = posn.CargoList[0] as Cargo;
                        if (cargo.UldType == "PMC")
                        {
                            throw (new Exception(string.Format("For LDV 305/315/325/335/345/355, ''41P'' must be not PMC.")));
                        }

                    }
                }
                uldNmList = null; //#BR17164 THOMAS
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            }
        }
        //BR11107-->

        /// <summary>
        /// check if MD11F upper deck has the ULD with the same size in the position or not 
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void checkMD11FMANSameSize()
        {
            if (FDB.Instance.GetSuperAcTypeName(theFlight.ACType.FullAirTypeName) == "MD11F")
            {
                string[] sameUldList = new string[] { "MR,PR,RR,SR", "CL,DL", "EL,FL,GL,HL,ML,PL,RL,SL" };
                foreach (string uldNames in sameUldList)
                {
                    string baseCode = "";
                    string[] uldNmList = uldNames.Split(',');
                    foreach (string uldNm in uldNmList)
                    {
                        CargoPosnBase posn = Find(uldNm);
                        if (!posn.Occupied) continue;
                        if (posn.CargoList[0] is Cargo)
                        {
                            Cargo cargo = posn.CargoList[0] as Cargo;
                            if (baseCode == "") baseCode = cargo.BaseCode;
                            else if (baseCode != cargo.BaseCode)
                                throw (new Exception(string.Format("{0} should be the same size ULD", uldNames)));
                        }
                    }
                    uldNmList = null; //#BR17164 THOMAS
                    System.GC.Collect(); //#BR17164 THOMAS GC強制回收
                }
            }
        }
        
        //<!-- #BR17202  THOMAS MD限重判斷
        /// <summary>
        ///  MD總重限制判斷
        /// </summary>
        /// <param name="cargo"></param>
        public void CheckMd_TotalWtLimit(bool showMsg = false)
        {
            if (theFlight.ACType.name.Substring(0, 5) == "BR77X")
            {
                CargoPosnMgr cargoPosnMgr = theFlight.CargoPosnMgr;
                CargoPosnBase[] cargoPosnList = cargoPosnMgr.GenVisibleList(0);  //已拉在位置的貨物
                double MD_CargoListWt = 0;
                double JP_CargoListWt = 0;
                string[] uldNmList = new string[] { "J", "K", "L", "M", "P" };
                foreach (CargoPosnBase posn in cargoPosnList)
                {
                    if (posn.IsMainDeckPosn())
                    {
                        MD_CargoListWt += posn.getWeight(); //MD總重

                        foreach (string uldNm in uldNmList)
                        {
                            if (posn.Name.Substring(0, 1) == uldNm)
                            {
                                JP_CargoListWt += posn.getWeight();  //J~P總重
                            }
                        }
                    }

                }

                if (JP_CargoListWt > 40010)
                {
                    string Msg = string.Format("J ~ P is over maximum combine load 40010 kg");
                    throw (new Exception(Msg));
                }

                if (MD_CargoListWt > 82100)
                {
                    //throw (new Exception(string.Format(EWBSCoreWarnings.MDTotalOverLimit)));
                    if (showMsg)
                    {
                        MessageBox.Show("If there is any piercing or rigid cargo load on main deck, then MAIN DECK TOTAL CARGO WEIGHT must less than or equal to 82100 KG");
                    }
                }
            }
        }
        //#BR17202  -->

        /// <summary>proceed check action</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool Check()
        {
            try
            {
                this.check();

                // restrict to sum single side
                CheckLockMaxWt();

                //check over the cargo's Lateral Imbalance restriction or not
                checkLateralImbalance();

                //<!--#BR17202 THOMAS
                //check main deck over the Unsymmetrical Load Limit or not
                //CheckUnsymmetric();                .
                AirTypeEx theAirType = theFlight.ACType;
                if (theAirType.lpol != null)
                {
                    #region 讀取XML檔案  LargePalletOverLap 區段
                    string PercentcurrentPath = Directory.GetCurrentDirectory() + "\\FDB";
                    string PercentXmlFileName = PercentcurrentPath + "\\" + theFlight.ACType.name + ".xml";
                    XmlDocument PercentXmlDoc = new XmlDocument();
                    PercentXmlDoc.Load(PercentXmlFileName);

                    XmlNodeList LargePalletOverLapNodeList = PercentXmlDoc.SelectNodes("/Airtype/LargePalletOverLap/ULD");
                    #endregion
                    CheckUnsymmetric(LargePalletOverLapNodeList);
                }
                else
                {
                    CheckUnsymmetric();
                }
                //#BR17202 -->
 
                #region check commit to 'Combined and Cumulative Load Limits' or not

                //check over the Combined Load Limit or not
                CheckCombinedLoadLimits();

                float tailFuel = 0f;

                try
                {
                    //Check Increased Cumulative Load
                    CheckCumulativeLoadLimits(false);
                    if (theFlight.UsingIncreasedCumulativeLoad)
                    {
                        if (CargoPosnExceptionHandler(this, true, EWBSCoreWarnings.RestoreNormalCumulativeLoad))
                        {
                            theFlight.UsingIncreasedCumulativeLoad = false;
                            if (theFlight.Fuel.IdxTailTank >= 0)
                            {
                                CargoPosnExceptionHandler(this, false, EWBSCoreWarnings.CheckFuelDistribution);
                            }
                        }
                        else throw (new Exception(EWBSCoreWarnings.UserGiveUp));
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        tailFuel = theFlight.Fuel.TailFuel;
                        if (tailFuel > 0)
                        {
                            theFlight.Fuel.Tanks[theFlight.Fuel.IdxCenterTank].Weight += tailFuel;
                            theFlight.Fuel.Tanks[theFlight.Fuel.IdxTailTank].Weight = 0f;
                        }

                        CheckCumulativeLoadLimits(true);
                        if (!theFlight.UsingIncreasedCumulativeLoad)
                        {
                            if (CargoPosnExceptionHandler(this, true, EWBSCoreWarnings.AskApplyIncreasedCmltiveLoad))
                            {


                                //BR16111<--		
                                MessageBox.Show(EWBSCoreWarnings.AskApplyIncreasedCmltiveLoad_remind, "EWBS");
                                //BR16111-->			

                                theFlight.UsingIncreasedCumulativeLoad = true;
                                if (theFlight.Fuel.FuelLoading != FuelLoadingStyle.NSTD && theFlight.Fuel.IdxTailTank >= 0) //check TailTank too
                                    theFlight.Fuel.FuelLoading = FuelLoadingStyle.NOTAIL;

                                if (theFlight.Fuel.IdxTailTank >= 0)
                                {
                                    CargoPosnExceptionHandler(this, false, EWBSCoreWarnings.CheckFuelDistribution);
                                }

                            }
                            else throw (e);
                        }
                    }
                    catch (Exception ee)
                    {
                        if (tailFuel > 0f)
                        {
                            theFlight.Fuel.Tanks[theFlight.Fuel.IdxCenterTank].Weight -= tailFuel;
                            theFlight.Fuel.Tanks[theFlight.Fuel.IdxTailTank].Weight = tailFuel;
                        }

                        if (theFlight.UsingIncreasedCumulativeLoad)
                        {
                            throw (ee);
                        }
                        else
                        {
                            throw (e);
                        }
                    }

                }

                #endregion

                CheckCargo();

                //check 4 ULDs of bcak deck in B777
                checkB777Aft4P();


                check787TwoPMC(); //BR18118 

                //BR11107<--
                checkB333Aft5P();
                //Br11107-->

                //<!--#BR17207 THOMAS 原BR16105移到Check，增加判斷BR77X
                checkBR77Posn();
                //#BR17207 -->

                //<!--#BR17210 THOMAS 原BR16106移到Check
                checkBR33ECP();
                //#BR17207 -->

                //check if MD11F upper deck has the ULD with the same size in the position or not  
                checkMD11FMANSameSize();

                //<!--#BR17202 THOMAS 限重判斷，MD總重限制判斷
                CheckMdLd_WtLimit();
                CheckMd_TotalWtLimit(true);
                //#BR17202 -->



            }
            catch (Exception e)
            {
                if (CargoPosnExceptionHandler(this, false, e.Message))
                {//do nothing
                }
                else //#BR00000<--
                {
                    //#BR00000 <--  					
                    if (e.Message.ToString().IndexOf("TOW >= 366,400kg") == -1)
                    {    
                        return false;
                    }
                    //#BR00000 -->
                }
                //				else return false;
                //#BR00000-->
            }

            try //exit if error occurred in the following statement
            {
                //check Special Handling Code and provide warning message only, if the rule is not followed.
                CheckDangerousGoods();
            }
            catch (Exception e)
            {
                CargoPosnExceptionHandler(this, false, e.Message);
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Get Cmpt/Bay by name
        /// </summary>
        /// <param name="name">Cmpt/Bay name</param>
        /// <returns>CargoPosnBase, Cmpt/Bay</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CargoPosnBase Find(string name)
        {
            return (CargoPosnBase)h[name];
        }

        /// <summary>get Freight Net Weight by destination</summary>
        /// <param name="dest"> destination </param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getFreNetWt(string dest)
        {
            return this.getNetWt("C", dest, false);
        }

        /// <summary>get Position Net Weight by destination</summary>
        /// <param name="dest"> destination </param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getPosNetWt(string dest)
        {
            return this.getNetWt("M", dest, false);
        }

        /// <summary>get baggage Net Weight by destination</summary>
        /// <param name="dest"> destination </param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getBagNetWt(string dest)
        {
            return this.getNetWt("B", dest, false);
        }

        /// <summary>get Freight Weight by destination</summary>
        /// <param name="dest"> destination </param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getFreWt(string dest)
        {
            return this.getGrossWt("C", dest);
        }

        /// <summary>get Position Weight by destination</summary>
        /// <param name="dest"> destination </param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getPosWt(string dest)
        {
            return this.getGrossWt("M", dest);
        }

        /// <summary>get Baggage Weight by destination</summary>
        /// <param name="dest"> destination </param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public double getBagWt(string dest)
        {
            return this.getGrossWt("B", dest);
        }

        /// <summary>
        /// Calculate lateral imbalance
        /// </summary>
        /// <returns></returns>
        public double CalcLateralImbalance()
        {
            double result = 0f;

            //all of posn
            foreach (CargoPosnBase posn in GenVisibleList(0))
            {
                if (posn is CargoPosn) //CargoPosn only
                {

                    //#BR17202 THOMAS 計算貨物重量加總,    說明: 右邊位置 (index=0,2) 為加上重量， 左邊(index =1,3)為扣除重量
                    if (posn.Parent.IndexOf(posn) < 4) //#BR17202 THOMAS CENTER LOAD(index = 4) 重量 不列入計算
                    {             
                        switch (posn.Parent.IndexOf(posn) % 2)
                        {
                            case 0:
                                result += posn.getGrossWt("", "");
                                break;
                            default:
                                result -= posn.getGrossWt("", "");
                                break;
                        }
                    }

                }
            }
            return result;
        }

    } //class HoldMgr
}