/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//*Thomas| Ver. 01 | #BR17164 | 2017/8/31                                     *
//*---------------------------------------------------------------------------*
//* 物件關閉後 Dispose 標記由GC回收                                           *
//*****************************************************************************
//*Thomas| Ver. 02 | #BR18117 | 2018/07/15                                    *
//*---------------------------------------------------------------------------*
//* 同步更新 trimsheet by zone jpe檔案                                        *
//***************************************************************************** 
//*Thomas| Ver. 03 | #BR19009 | 2019/03/15                                    *
//*---------------------------------------------------------------------------*
//* 註冊編號排序                                                              *
//*****************************************************************************
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using ComInterface;
using EwbsCore.Util;
using System.Collections.Generic;

namespace FlightDataBase
{
    public delegate void SyncFDB(string xmlPath);

    /// <summary>
    /// FDB type enumeration
    /// </summary>
    public enum FDBType
    {
        Airinfo = 0,
        Airline,
        Account,
        Airtype,
        End
    }

    /// <summary>
    /// main class of Flight Database
    /// </summary>
    public sealed class FDB
    {
        public static readonly FDB instance = new FDB();

        private airinfo acinf; //Aircraft information
        private ArrayList actypeList = new ArrayList(); //List of airTypes
        private airlines airlineList = null; //List of airlines
        private ArrayList userList = new ArrayList(); //List of Users

        //public Airtype airType;

        /// <summary>
        /// get the FDB instance
        /// </summary>
        public static FDB Instance
        {
            get { return instance; }
        }


        /// <summary>Get all of the airline names</summary>
        /// <returns>All of the airline names</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] GetAirlineNames()
        {
            ArrayList aList = new ArrayList();
            foreach (airinfoAirline airline in acinf.airline)
                aList.Add(airline.name);
            return (string[])aList.ToArray(typeof(string));
        }



        /// <summary>Use Aircraft Registrsation number to query Airline Name</summary>
        /// <param name="_airCode">Aircraft Registrsation number</param>
        /// <returns>Airline Name</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string GetAirlineNameByAC(string _airCode)
        {
            //foreach airline, airtype, aircode
            foreach (airinfoAirline airline in acinf.airline)
                foreach (airinfoAirlineAirtype airtype in airline.airtype)
                    foreach (airinfoAirlineAirtypeAircode aircode in airtype.aircode)
                        if (aircode.name.Equals(_airCode)) //the same Aircraft Registrsation number
                            return airline.name;
            return "";
        }


        /// <summary>Use Aircraft Registrsation number to query Air type Name</summary>
        /// <param name="_airCode">Aircraft Registrsation number</param>
        /// <returns>string : Air type Name</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string GetAirTypeNameByAC(string _airCode)
        {
            //foreach airline, airtype, aircode
            foreach (airinfoAirline airline in acinf.airline)
                foreach (airinfoAirlineAirtype airtype in airline.airtype)
                    foreach (airinfoAirlineAirtypeAircode aircode in airtype.aircode)
                        if (aircode.name.Equals(_airCode)) //the same Aircraft Registrsation number
                            return airtype.name;
            return "";
        }


        /// <summary>Use Carrier code and operable aircraft type to query Aircraft Name</summary>
        /// <param name="_carrCode">Carrier Code</param>
        /// <param name="allowOperatingActypes">allowed Operating Actypes</param>
        /// <returns>string[] : Aircraft Names</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] GetAircraftNames(string _carrCode, string[] allowOperatingActypes)
        {
            ArrayList aList = new ArrayList();
            foreach (airinfoAirline airline in acinf.airline)
            {
                //carrCode is valid or carrCode is empty
                if (airline.name.Equals(_carrCode) || _carrCode == "")
                {
                    foreach (airinfoAirlineAirtype acType in airline.airtype)
                    {
                        bool allowed = false;
                        if (allowOperatingActypes != null)
                        {
                            foreach (string allowedType in allowOperatingActypes)
                            {
                                if (allowedType.Equals(acType.opactype)) //valid aircraft type
                                {
                                    allowed = true;
                                    break;
                                }
                            }
                        }
                        else //allowOperatingActypes can be empty
                        {
                            allowed = true;
                        }
                        if (allowed) //get all of the aircode
                        {
                            foreach (airinfoAirlineAirtypeAircode acCode in acType.aircode)
                            {
                                aList.Add(acCode.name);
                            }
                        }
                    } //foreach Airtype
                }
            } //foreach airline

            aList.Sort(); //排序 #BR19009 Thomas 排序

            //aList.Reverse(); //反轉
            return (string[])aList.ToArray(typeof(string));
        }



        public string[] GetAircraftNames1(string _carrCode, string[] allowOperatingActypes)
        {
            //ArrayList aList = new ArrayList();
            var aList = new List<Tuple<string, string>>();

            foreach (airinfoAirline airline in acinf.airline)
            {
                //carrCode is valid or carrCode is empty
                if (airline.name.Equals(_carrCode) || _carrCode == "")
                {
                    foreach (airinfoAirlineAirtype acType in airline.airtype)
                    {
                        bool allowed = false;
                        if (allowOperatingActypes != null)
                        {
                            foreach (string allowedType in allowOperatingActypes)
                            {
                                if (allowedType.Equals(acType.opactype)) //valid aircraft type
                                {
                                    allowed = true;
                                    break;
                                }
                            }
                        }
                        else //allowOperatingActypes can be empty
                        {
                            allowed = true;
                        }
                        if (allowed) //get all of the aircode
                        {
                            foreach (airinfoAirlineAirtypeAircode acCode in acType.aircode)
                            {
                               // aList.Add(new Data(acCode.name,acCode.name));
                                aList.Add(Tuple.Create(acCode.name, acCode.name));
                            }
                        }
                    } //foreach Airtype
                }
            } //foreach airline

            aList.Sort(); //排序 #BR19009 Thomas 排序

            //aList.Reverse(); //反轉
            return null;// (string[])aList.ToArray(typeof(string));
        }

        /// <summary>Get the Aircode object</summary>
        /// <param name="_airCode">Aircraft Registrsation number</param>
        /// <returns>airinfoAirlineAirtypeAircode</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public airinfoAirlineAirtypeAircode GetAC(string _airCode)
        {
            //foreach airline, airtype, aircode
            foreach (airinfoAirline airline in acinf.airline)
                foreach (airinfoAirlineAirtype airtype in airline.airtype)
                    foreach (airinfoAirlineAirtypeAircode aircode in airtype.aircode)
                        if (aircode.name.Equals(_airCode)) //the same Aircraft Registrsation number
                        {
                            return aircode;
                        }
            return null;
        }


        /// <summary>Get the user object</summary>
        /// <param name="username">user name </param>
        /// <returns>accountUser</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public accountUser GetUser(string username)
        {
            //Get All User
            foreach (accountUser user in GetAllUser())
            {
                if (user.username.ToUpper().Equals(username.ToUpper()))
                {
                    return user;
                }
            }
            throw (new Exception(string.Format(EWBSCoreWarnings.FDBUserNotFound_1, username)));
        }


        /// <summary>authentication succeed or failure</summary>
        /// <param name="username">user name </param>
        /// <param name="passwd">password</param>
        /// <returns>bool: authentication succeed or failure</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool Authenticated(string username, string passwd)
        {
            accountUser theUser = GetUser(username);
            string encryptedPasswd = EwbsRC2.RC2Encrypted(passwd);

            if (theUser != null && theUser.password.Equals(encryptedPasswd))
            {
                return true;
            }
            return false;
        }

        /// <summary>File is modified or not</summary>
        /// <param name="xmlPath">xml file path</param>
        /// <param name="sigPath">sig file path</param>
        /// <returns> File is modified or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsTampered(string xmlPath, string sigPath)
        {
            byte[] contents = DirService.FileContent(xmlPath);
            if (contents != null)
            {
                byte[] data = DirService.GetMD5(xmlPath);
                Signature csig = new Signature(contents.Length, data); //Create digital signature

                Signature bsig;
                if (File.Exists(sigPath))
                    bsig = (Signature)DirService.Deserialize(sigPath);
                else // used to enforce to save .sig if local does not have that file
                    bsig = new Signature(contents.Length + 1, data);

                if (bsig.IsSameSig(csig))
                    return false;
            }
            return true;
        }


        /// <summary>Get all of the Airtype Name</summary>
        /// <returns>Airtype Names</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] GetAirtypeNames()
        {
            ArrayList aList = new ArrayList();
            foreach (airinfoAirline airline in acinf.airline)
                foreach (airinfoAirlineAirtype acType in airline.airtype)
                    aList.Add(airline.name + acType.name);
            return (string[])aList.ToArray(typeof(string));
        }

        /// <summary>load FDB file</summary>
        /// <param name="path">file path</param>
        /// <param name="cname">class name</param>
        /// <returns>Object that deserialized</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private Object loadMDB(string path, string cname)
        {
            try
            {
                TextReader reader = new StreamReader(path);
                XmlSerializer serializer = new XmlSerializer(Type.GetType(cname));
                object obj = serializer.Deserialize(reader);
                reader.Close();
                reader.Dispose(); //#BR17164 THOMAS標記由GC釋放
                reader = null;
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
                return obj;
            }
            catch (Exception e)
            {
                throw (new Exception(path + ":" + e.Message));
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        private FDB()
        {
        }


        /// <summary>Get Aircode's Category</summary>
        /// <param name="_airCode">Aircraft Registrsation number</param>
        /// <returns>int : category</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int GetACCategory(string _airCode)
        {
            foreach (airinfoAirline airline in acinf.airline)
                foreach (airinfoAirlineAirtype airtype in airline.airtype)
                    foreach (airinfoAirlineAirtypeAircode aircode in airtype.aircode)
                        if (aircode.name.Equals(_airCode))
                        {
                            return (int)airtype.category;
                        }
            return -1;
        }


        public SyncFDB SyncFDB = null; //Sync FDB procedure

        /// <summary>
        /// Get AirType
        /// </summary>
        /// <param name="_carrCode">Carrier Code</param>
        /// <param name="_airtype">Aircraft type</param>
        /// <returns>Airtype</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public AirTypeEx GetAirType(string _carrCode, string _airtype)
        {
            string airtypeName = _carrCode + _airtype; //
            //find airtype from FDB first
            foreach (AirTypeEx airtype in actypeList)
                if (airtype.name.Equals(airtypeName))
                    return airtype;

            //load airtype dynamically, if airtype not found
            string xmlPath = FileFinder.GetFileName(airtypeName + ".xml");

            if (SyncFDB != null)
            {
                SyncFDB(xmlPath);
            }
            Airtype newAirType = (Airtype)loadMDB(xmlPath, "FlightDataBase.Airtype");

            //Found!!
            if (newAirType != null && newAirType.name.Equals(airtypeName))
            {
                // load trimsheet image file if necessary
                
                string strImgName = FileFinder.GetFileName(airtypeName + "TrimSheet.jpg"); 
                if (SyncFDB != null)
                {
                    SyncFDB(strImgName);
                }

                //<!--#BR18117 Thomas 同步更新Zone.jpg檔案
                string strImgNameZone = FileFinder.GetFileName(airtypeName + "TrimSheetZone.jpg"); //#BR18117 新增同步更新Zone.jpg檔案
                if (SyncFDB != null)
                {
                    SyncFDB(strImgNameZone);
                }
                //#BR18117-->

                AirTypeEx theAirType = new AirTypeEx(newAirType);
                theAirType.FullAirTypeName = airtypeName;
                theAirType.FuelDensity = newAirType.FuelDensity; //#BR19015
                actypeList.Add(theAirType);

                //find airtype description in LIF
                foreach (airinfoAirline airline in acinf.airline)
                {
                    if (airline.name.Equals(_carrCode))
                    {
                        foreach (airinfoAirlineAirtype airtype in airline.airtype)
                            if (airtype.name.EndsWith(_airtype) || airtype.name.Equals(airtypeName))
                            {
                                theAirType.description = airtype.lif;
                            }
                    }
                } //for

                return theAirType;
            }
            //Not Found!!
            throw (new Exception(string.Format(EWBSCoreWarnings.FDBAirtypeNotFound_1, _carrCode + _airtype)));
        }


        /// <summary> get Airline by Carrier code</summary>
        /// <param name="_carrCode">Carrier code</param>
        /// <returns>AirlineEx: Airline object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public AirlineEx GetAirline(string _carrCode)
        {
            //load airlines dynamically
            if (airlineList == null)
            {
                string xmlPath = FileFinder.GetFileName("airlines.xml");

                if (SyncFDB != null)
                    SyncFDB(xmlPath);
                airlineList = (airlines)loadMDB(xmlPath, "FlightDataBase.airlines");
            }
            //find airline by carrier code
            foreach (airlinesAirline airline in airlineList.airline)
            {
                if (_carrCode.Equals(airline.name) || _carrCode == "")
                {
                    //Found!!
                    return new AirlineEx(airline);
                }
            }

            //Not Found!!
            throw (new Exception(string.Format(EWBSCoreWarnings.FDBAirlineNotFound_1, _carrCode)));
        }


        /// <summary>Get all of the user names</summary>
        /// <returns>all of the user names</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public accountUser[] GetAllUser()
        {
            userList.Clear();
            if (userList.Count == 0)
            {
                //load account dynamically
                string xmlPath = FileFinder.GetFileName("account.xml");

                if (SyncFDB != null)
                {
                    SyncFDB(xmlPath);
                }
                FlightDataBase.account fdbAccount = (account)loadMDB(xmlPath, "FlightDataBase.account");
                //save user in FDB
                if (fdbAccount != null)
                {
                    foreach (accountUser user in fdbAccount.user)
                    {
                        userList.Add(user);

                    }
                }
            }
            return (accountUser[])userList.ToArray(typeof(accountUser));

        }


        /// <summary>load airinfo</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void load()
        {
            //sync. airinfo
            string xmlPath = FileFinder.GetFileName("airinfo.xml");

            //# UldBulkLimit.xml  路徑
            //string UldBulkLimit_xmlPath = FileFinder.GetFileName("UldBulkLimit.xml"); 

            if (SyncFDB != null)
            {
                SyncFDB(xmlPath);

                //下載到Client FDB
               // SyncFDB(UldBulkLimit_xmlPath); 
            }
            //load airinfo
            acinf = (airinfo)loadMDB(xmlPath, "FlightDataBase.airinfo");
        }

        /// <summary>FDB Initialize</summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Init()
        {
            //load airinfo
            this.load();
            //Cleanup airline & airtype
            this.airlineList = null;
            this.actypeList.Clear();
        }

        /// <summary>get ULD volume</summary>
        /// <param name="uldType">ULD name</param>
        /// <param name="carrierCode">Carrier code</param>
        /// <returns>ULD volume</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float[] GetUldDimension(string uldType, string carrierCode)
        {
            airlinesAirlineUld uld = GetULD(uldType, carrierCode);
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
            { }
            return null;
        }

        /// <summary>get Super Airtype name</summary>
        /// <param name="fullAirTypeName">Carrier Code + AirType Name</param>
        /// <returns>Super Airtype name</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string GetSuperAcTypeName(string fullAirTypeName)
        {
            foreach (airinfoAirline airline in acinf.airline)
            {
                foreach (airinfoAirlineAirtype airtype in airline.airtype)
                    if (airline.name + airtype.name == fullAirTypeName)
                        return airtype.opactype;
            }
            return "";
        }

        /// <summary>get ULD</summary>
        /// <param name="uldType">uld type code</param>
        /// <param name="carrierCode">carrier code</param>
        /// <returns>airlinesAirlineUld: ULD</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public airlinesAirlineUld GetULD(string uldType, string carrierCode)
        {
            AirlineEx airline;
            airlinesAirlineUld uld = null;
            try
            {
                try
                {
                    //get airline according carrierCode
                    airline = FDB.Instance.GetAirline(carrierCode);
                    //get uld according airline
                    uld = airline.GetULD(uldType);
                    if (uld == null)
                    {
                        throw (new Exception(string.Format(EWBSCoreWarnings.UldNotFound_1, uldType)));
                    }

                }
                catch
                {
                    //get default airline
                    airline = FDB.Instance.GetAirline("");
                    //get uld according airline
                    uld = airline.GetULD(uldType);
                }
            }
            catch
            { }
            return uld;
        }
    }

}