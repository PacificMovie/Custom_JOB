/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//* WUPC      | Ver. 00 | #BR071007 | 2007/OCT/03                             *
//*---------------------------------------------------------------------------*
//* Maintain SpecialLoad Restriction in DB                                    *
//*****************************************************************************
//* Thomas      | Ver. 01 | #BR19001 | 2019/01/03                             *
//*---------------------------------------------------------------------------*
//*                                     *
//*****************************************************************************
using System;
using System.Collections;

namespace FlightDataBase
{
    /// <summary>
    /// Provides external function for Airline
    /// </summary>
    public class AirlineEx
    {
        private airlinesAirline airline; //Airline information

        /// <summary>
        /// Constructor
        /// </summary>
        public AirlineEx(airlinesAirline airline)
        {
            this.airline = airline;
        }


        /// <summary>
        /// get ULD Information of an Airline
        /// </summary>
        public airlinesAirlineUld[] uldInfo
        {
            get
            {
                try
                {
                    if (this.airline.uldInfo != null)
                        return this.airline.uldInfo;
                    AirlineEx theAirline = FDB.Instance.GetAirline("");
                    return theAirline.uldInfo;
                }
                catch
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// default Weight for Personel
        /// </summary>
        public airlinesAirlinePersonWeight PersonWeight
        {
            get
            {
                try
                {
                    if (this.airline.PersonWeight != null)
                        return this.airline.PersonWeight;
                    AirlineEx theAirline = FDB.Instance.GetAirline("");
                    return theAirline.PersonWeight;
                }
                catch
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// get fuel density
        /// </summary>
        private airlinesAirlineFinfo[] FuelDensity
        {
            get
            {
                try
                {
                    if (this.airline.FuelDensity != null)
                        return this.airline.FuelDensity;
                    AirlineEx theAirline = FDB.Instance.GetAirline("");
                    return theAirline.FuelDensity;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// get dg information
        /// </summary>
        private airlinesAirlineDginfo[] prohibit
        {
            get
            {
                try
                {
                    if (this.airline.prohibit != null)
                        return this.airline.prohibit;
                    AirlineEx theAirline = FDB.Instance.GetAirline("");
                    return theAirline.prohibit;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// get: airline.name, the name of the airline
        /// </summary>
        public string name
        {
            get { return this.airline.name; }
        }


        /// <summary>
        /// Get Fuel Density
        /// </summary>
        /// <param name="from">departure station</param>
        /// <returns>Fuel Density</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float GetFuelDensity(string from)
        {
            string airport = from.ToUpper();
            try
            {
                foreach (airlinesAirlineFinfo info in this.FuelDensity)
                {
                    if (airport.Equals(info.from.ToUpper()))
                        return Convert.ToSingle(info.density);
                } //for
            }
            catch
            {
            }
            return 0.792f; //default Fuel Density
        }

        /// <summary>
        /// Find the segregation code of dangerous goods
        /// </summary>
        /// <param name="shc1">dg1</param>
        /// <param name="shc2">dg2</param>
        /// <returns>the segregation code</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int DGCode(string shc1, string shc2)
        {
            if (shc1 == "" || shc2 == "")
                return 0;
            try
            {
                foreach (airlinesAirlineDginfo dgInfo in this.prohibit)
                {
                    if (dgInfo.sp1.ToUpper().EndsWith(shc1.ToUpper()))
                    {
                        if (dgInfo.sp2.ToUpper().EndsWith(shc2.ToUpper()))
                            return dgInfo.type;
                    }
                    else if (dgInfo.sp1.ToUpper().EndsWith(shc2.ToUpper()))
                    {
                        if (dgInfo.sp2.ToUpper().EndsWith(shc1.ToUpper()))
                            return dgInfo.type;
                    }
                }
            }
            catch
            {
            }
            return 0;
        }

        /// <summary>
        /// get Sender & dblsig for telex
        /// </summary>
        /// <param name="_station">departure station</param>
        /// <param name="_category">airtype catg</param>
        /// <param name="_telex">telex name</param>
        /// <returns>Sender & dblsig</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] GetDepSitaSenderDBL(string _station, int _category, string _telex)
        {
            string[] rtn = new string[] { "", "" };
            foreach (airlinesAirlineStation station in this.airline.telexInfo)
            {
                if (station.name == _station)
                {
                    foreach (airlinesAirlineStationFlightType ftype in station.flightType)
                    {
                        if (!IsSameFlightType(ftype, _category))
                            continue;
                        rtn[0] = ftype.sender;
                        rtn[1] = ftype.dblsig;
                        return rtn;
                    }
                }
            }
            return rtn;
        }

        /// <summary>check if Flight Type is identical or not</summary>
        /// <param name="ftype">airlinesAirlineStationFlightType</param>
        /// <param name="_category">airtype catg</param>
        /// <returns>bool: Flight Type is identical or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private bool IsSameFlightType(airlinesAirlineStationFlightType ftype, int _category)
        {
            if (_category == Convert.ToInt32(airinfoAirlineAirtypeCategory.Freighter) &&
                ftype.name == "Freighter")
                return true;
            else if (_category == Convert.ToInt32(airinfoAirlineAirtypeCategory.Passenger) &&
                ftype.name == "Passenger")
                return true;
            return false;
        }


        /// <summary> get SITA CODE for taking off</summary>
        /// <param name="_station">depature staion</param>
        /// <param name="_category">airtype catg</param>
        /// <param name="_telex">telex</param>
        /// <returns>string[]: SITA CODES</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] GetDepSitaCodes(string _station, int _category, string _telex)
        { // _category can be got by call FDB.Instance.GetACCategory(accode)
            airlinesAirlineStationFlightTypeFlightTinfoSite[] arSite = null;

            foreach (airlinesAirlineStation station in this.airline.telexInfo)
            {
                if (station.name != _station)
                    continue;
                foreach (airlinesAirlineStationFlightType ftype in station.flightType)
                {
                    if (!IsSameFlightType(ftype, _category))
                        continue;
                    foreach (airlinesAirlineStationFlightTypeFlight _flight in ftype.flight)
                    {
                        if (_flight.name != "DEP")
                            continue;
                        foreach (airlinesAirlineStationFlightTypeFlightTinfo ti in _flight.tinfo)
                        {
                            if (ti.telex != _telex)
                                continue;
                            arSite = ti.site;
                            if (arSite != null)
                            {
                                string[] rtn = new string[arSite.Length];
                                int i = 0;
                                foreach (airlinesAirlineStationFlightTypeFlightTinfoSite si in arSite)
                                    rtn[i++] = si.sita;
                                return rtn;
                            }
                            return null;
                        }
                    }
                }
            }

            return null;
        }


        /// <summary> get SITA CODE for landing</summary>
        /// <param name="_station">station</param>
        /// <param name="_category">category</param>
        /// <param name="_telex">telex</param>
        /// <returns>string[]: SITA CODES</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] GetArrSitaCodes(string _station, int _category, string _telex)
        { // _category can be got by call FDB.Instance.GetACCategory(accode)
            airlinesAirlineStationFlightTypeFlightTinfoSite[] arSite = null;

            foreach (airlinesAirlineStation station in this.airline.telexInfo)
            {
                if (station.name != _station)
                    continue;
                if (station.flightType == null) return null;
                foreach (airlinesAirlineStationFlightType ftype in station.flightType)
                {
                    if (!IsSameFlightType(ftype, _category))
                        continue;
                    if (ftype.flight == null) return null;
                    foreach (airlinesAirlineStationFlightTypeFlight _flight in ftype.flight)
                    {
                        if (_flight.name != "ARR")
                            continue;
                        if (_flight.tinfo == null) return null;
                        foreach (airlinesAirlineStationFlightTypeFlightTinfo ti in _flight.tinfo)
                        {
                            if (ti.telex != _telex)
                                continue;
                            arSite = ti.site;
                            if (arSite != null)
                            {
                                string[] rtn = new string[arSite.Length];
                                int i = 0;
                                foreach (airlinesAirlineStationFlightTypeFlightTinfoSite si in arSite)
                                    rtn[i++] = si.sita;
                                return rtn;
                            }
                            return null;
                        }
                    }
                }
            }

            return null;
        }


        /// <summary> get ULD</summary>
        /// <param name="uldType">uldType string </param>
        /// <returns>airlinesAirlineUld: ULD</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public airlinesAirlineUld GetULD(string uldType)
        {
            //get Airline data from carrierCode 
            foreach (airlinesAirlineUld uld in airline.uldInfo)
            {
                //get baseCode from uldType and serialNo 
                if (uld.type == uldType) return uld;
            }
            return null;

        }


        /// <summary> get  weight unit</summary>
        /// <returns>string:  weight unit</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string GetWtUnit()
        {
            return "KILOS"; // TODO
        }


        /// <summary> get ULD's volume data</summary>
        /// <param name="uldType">ULD type</param>
        /// <returns> float[]:  get ULD's volume data</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float[] ULDDimension(string uldType)
        {
            airlinesAirlineUld uld = GetULD(uldType);
            try
            {
                string delimStr = " ,xX*";
                string[] dim = uld.dim.Split(delimStr.ToCharArray());

                ArrayList aList = new ArrayList();
                foreach (string val in dim)
                    aList.Add(Convert.ToSingle(val));
                return (float[])aList.ToArray(typeof(float));
            }
            catch
            {
            }
            return null;
        }

        #region SHC-CATG-NOTOC-DG Data Collection

        /// <summary>
        ///  get name of related data SHC  
        /// </summary>
        /// <param name="shc">SHC name </param>
        /// <returns>airlinesAirlineShcInfo：SHC related data </returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private airlinesAirlineShcInfo getShcInfo(string shc)
        {
            if (shc != null)
            {
                foreach (airlinesAirlineShcInfo shcInfo in this.airline.SpecialLoad)
                    if (shcInfo.shc == shc)
                        return shcInfo;
            }
            return null;
        }

        /// <summary>
        ///  get SHC of some Category
        /// </summary>
        /// <param name="catg">Category</param>
        /// <returns>string[]: SHC</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] getAllSHCbyCatg(string catg)
        {
            ArrayList shcStringList = new ArrayList();

            foreach (airlinesAirlineShcInfo shcInfo in this.airline.SpecialLoad)
                if (shcInfo.catg == catg)
                    shcStringList.Add(shcInfo.shc);

            shcStringList.Sort();

            return (string[])shcStringList.ToArray(typeof(string));
        }

        /// <summary>
        ///  get all SHCs
        /// </summary>
        /// <returns>string[]: SHC</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] getAllSHC()
        {
            ArrayList shcStringList = new ArrayList();

            foreach (airlinesAirlineShcInfo shcInfo in this.airline.SpecialLoad)
                shcStringList.Add(shcInfo.shc);

            shcStringList.Sort();

            return (string[])shcStringList.ToArray(typeof(string));
        }

        /// <summary>
        ///  get all Categories
        /// </summary>
        /// <returns>string[]: all Categories</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] getAllCatgs()
        {
            ArrayList catgStringList = new ArrayList();
            foreach (airlinesAirlineShcInfo shcInfo in this.airline.SpecialLoad)
            {
                bool isDuplicated = false;
                foreach (string catg in catgStringList)
                {
                    if (shcInfo.catg == catg)
                    {
                        isDuplicated = true;
                        break;
                    }
                }
                if (!isDuplicated)
                    catgStringList.Add(shcInfo.catg);
            }


            catgStringList.Sort();

            return (string[])catgStringList.ToArray(typeof(string));
        }

        /// <summary>
        /// Send NOTOC or not for smoe SHC
        /// </summary>
        /// <param name="shc">SHC</param>
        /// <returns>bool: Send NOTOC or not for smoe SHC</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool isNOTOC(string shc)
        {
            airlinesAirlineShcInfo shcInfo = getShcInfo(shc);
            if (shcInfo != null) return shcInfo.notoc;
            return false;
        }

        /// <summary>
        /// The SHC is dangerous goods or not
        /// </summary>
        /// <param name="shc">SHC</param>
        /// <returns>bool: dangerous goods or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool isDG(string shc)
        {
            airlinesAirlineShcInfo shcInfo = getShcInfo(shc);
            if (shcInfo != null) return shcInfo.dg;
            return false;
        }

        /// <summary>Special Handling Code is legal or not</summary>
        /// <param name="inputSHC"></param>
        /// <returns>bool: Special Handling Code is legal or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool isLegalSHC(string inputSHC)
        {
            return getShcInfo(inputSHC) != null;
        }

        #endregion
    }

}