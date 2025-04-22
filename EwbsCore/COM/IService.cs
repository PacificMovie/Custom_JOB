/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//* THOMAS    | Ver. 03 | #BR17226  | 2018/1/23                               *
//*---------------------------------------------------------------------------*
// 新增 Baggage referance 功能 
//*****************************************************************************
//* Thomas     | Ver. 02 | #BR17224 | 2018/JUN/12                             *
//*---------------------------------------------------------------------------*
// 取消SuffixCode=D 時，去掉SuffixCode重新查找FIS                             *
// QueryCRTS查詢條件增加 suffixCODE                                           * 
// (Creat有suffixCode的班機，若此班機有Diver的情況，在Create Diver班機時      *
// 因搜尋wbf的規則會將Diver班機號碼去除"D"，再去搜尋wbf，因此會取不到此班機   *
// 的 wbf，並有可能會取到同fltNo但無suffixcode 的wbf)                         *
//*****************************************************************************
//* JAY SHEU  | Ver. 01 | #BR15111 | Sep 08, 2015   (V03.02.01)               *
//*---------------------------------------------------------------------------*
//* Added retreive iFuel information                                          *
//*****************************************************************************
//* Terri    | Ver. 04 | #BR17101  | 2017/FEB/02                (V3.04.02 )   *
//*---------------------------------------------------------------------------*
//接cargofly資料                                                              * 
//*****************************************************************************
using System;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using Compression;
using EWBS;

namespace ComInterface
{
    // delegate used for client to call server's service asynchronouslly
    public delegate FlightResp CreateFlightDelegate(FlightArg fltarg);

    public delegate DCSResp DCSInfoDelegate(FlightArg fltarg);

    public delegate FISResp FISInfoDelegate(FlightArg fltarg);

    public delegate CRTSResp CRTSInfoDelegate(FlightArg fltarg);

    public delegate CargoResp CargoInfoDelegate(FlightArg fltarg);

    /// <summary>
    /// Interface for communication services from EWBSClient to EWBSServer.
    /// </summary>
    public interface IService
    {
        bool CheckVersion(string version);

        RespErrInd SyncFDBEx(string filename, Signature sig, string ip); // check the file signature

        FileSignature GetFDB(string filename); // get the file itself and signature

        // save modified FDB file (..\\ARCHIVE\\FDB\\tmp\\<fileName>_<validDate>.xml)
        // Note: this fileName does not include extention
        string SaveFDB(string fileName, FileSignature fsig);

        // save modified FDB file (..\\ARCHIVE\\FDB\\<fileName>_<validDate>.xml)
        // Note: this fileName does not include extention

        FlightResp CreateFlight(FlightArg fltarg );

        FlightResp CreateDiverFlight(FlightArg fltarg);  //#BR17224 新增CreateDiverFlight

        FlightResp MonitorFlight(FlightArg fltarg); // monitor other's working flight

        FlightResp OpenFlight(FlightArg fltarg); // open a existed flight

        RespErrInd CloseFlight(Flight flight); // close and save flight

        RespErrInd CloseFlight(FlightArg fltarg); // close flight

        RespErrInd SaveFlight(Flight flight); // save flight

        DCSResp GetDCSInfo(FlightArg fltarg); // get DCS information

        FISResp GetFISInfo(FlightArg fltarg); // get FIS information

        CRTSResp GetCRTSInfo(FlightArg fltarg); // get CRTS information

        CargoResp GetCargoInfo(FlightArg fltarg); // get CargoWin information

        ArrayList AddHotNews(HotNews news); // save hot news

        bool DelHotNews(long id); // save hot news

        ArrayList GetHotNews(); // get all news of HotNewsBase

        string GetHotNews(long id); // get ten pieces of news

        // SendTelex
        RespErrInd SendTelex(FlightExt fltEx);

        // Baggage Static Report   1103
        string BagStat(string yearMonth);
    
        string GetBagReport(string fltFrom="", string fltTo="");    //#BR17226 THOMAS 新增 Baggage referance 功能

        iFuelResp GetIFuelInfo(FlightArg fltArg); // Get iFuel Info #BR15111 - JayS

        CargoFlyResp GetCargoFlyInfo(FlightArg fltarg); // get Cargofly information  //#BR17101
    }

    /// <summary>
    /// Interface for communication services from EWBSServer to EWBSClient.
    /// </summary>
    public interface IClientService
    {
        bool Ack(PilotAck[] acks); // ACARS Acknowledge

        /// <summary>get flight object from client's flight list</summary>
        /// <param name="fltarg">Flight Argument</param>
        /// <returns>Flight</returns>
        Flight GetFlightData(FlightArg fltarg); // get flight object from client's flight list
    }

    /// <summary>
    /// Pilot Acknowledge data.
    /// </summary>
    [Serializable]
    public class PilotAck
    {
        public string datetime; // the time FIS received ack from pilot
        public string ack; // pilot ack
    }

    /// <summary>
    /// Except Flight instance, telexIdx used to indicate which type of telex.
    /// </summary>
    [Serializable]
    public class FlightExt
    {
        private Flight flt; //Flight

        private int telexIdx; //Telex index

        private string ip = ""; // ip addess

        /// <summary>
        /// Constructor
        /// </summary>
        public FlightExt(Flight _flt, int _telexIdx, string _ip)
        {
            flt = _flt;
            telexIdx = _telexIdx;
            ip = _ip;
        }

        /// <summary>
        /// get: flt
        /// </summary>
        public Flight Flt
        {
            get { return flt; }
        }

        /// <summary>
        /// get: telexIdx
        /// </summary>
        public int TelexIdx
        {
            get { return telexIdx; }
        }

        /// <summary>
        /// get: ip
        /// </summary>
        public string Ip
        {
            get { return ip; }
        }
    }

    /// <summary>
    /// Used to pass user key-in flight data to call EWBS Server services.
    /// </summary>
    [Serializable]
    public class FlightArg
    {
        private string flt_no; //flight number
        private string ac_no; //Aircraft number
        private string[] route; //routes
        private string std; //scheduled time departure
        private string user; // user account name
        private string ipAddr = ""; // client's ip address
        private long ticks; //time ticks

        /// <summary>
        /// FlightArg Constructor
        /// </summary>
        public FlightArg()
        {
        }

        /// <summary>
        /// FlightArg Constructor
        /// </summary>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public FlightArg(Flight theFlight)
        {
            flt_no = theFlight.FlightNumber;
            ac_no = theFlight.ACRegNo;
            route = theFlight.Route;
            std = theFlight.STD.ToString("yyyy/MM/dd HH:mm:ss");
            ticks = theFlight.STD.ToUniversalTime().Ticks;
            user = theFlight.Author;
        }

        /// <summary>
        /// flt_no
        /// </summary>
        public string Flt_no
        {
            get { return flt_no; }
            set { flt_no = value; }
        }

        /// <summary>
        /// Ac_no
        /// </summary>
        public string Ac_no
        {
            get { return ac_no; }
            set { ac_no = value; }
        }

        /// <summary>
        /// get or set STD: "yyyy/MM/dd HH:mm:ss"
        /// </summary>
        public DateTime STD
        {
            get { return DateTime.Parse(std); }
            set
            {
                std = value.ToString("yyyy/MM/dd HH:mm:ss");
                ticks = value.ToUniversalTime().Ticks;

            }
        }

        public long Ticks
        {
            get { return ticks; }
        }

        public string[] Route
        {
            get { return route; }
            set { route = value; }
        }

        /// <summary>
        /// user
        /// </summary>
        public string User
        {
            get { return user; }
            set { user = value; }
        }

        /// <summary>
        /// ipAddr
        /// </summary>
        public string IpAddr
        {
            get { return ipAddr; }
            set { ipAddr = value; }
        }

        /// <summary>
        /// the station from which the flight departs
        /// </summary>
        public string From
        {
            get { return route[0]; }
        }

        /// <summary>
        /// the first destination
        /// </summary>
        public string To
        {
            get { return route[1]; }
        }

        /// <summary>
        /// get Flight Name
        /// </summary>
        public string Name
        {
            get
            {
                return String.Format("{0}-{1}-{2}",this.Flt_no,this.STD.ToString("ddMMM", new CultureInfo("en-US")).ToUpper(),this.Route[1]);
            }
        }
    }

    #region Signature

    /// <summary>
    /// FileSignature inherit form Signature. The byte array of data stores the content of file. 
    /// </summary>
    [Serializable]
    public class FileSignature : Signature
    {
        private byte[] data;

        /// <summary>
        /// FileSignature Constructor
        /// </summary>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public FileSignature(byte[] data)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            this.size = data.Length;
            this.md5Bytes = md5.ComputeHash(data);
            this.data = AdaptiveHuffmanProvider.Compress(data);
        }

        /// <summary>
        /// data
        /// </summary>
        public byte[] Data
        {
            get { return data; }
        }

        /// <summary>
        /// get: new Signature(size, md5Bytes)
        /// </summary>
        public Signature Sig
        {
            get { return new Signature(size, md5Bytes); }
        }
    }

    /// <summary>
    /// used to check if content of file is modified.
    /// </summary>
    [Serializable]
    public class Signature
    {
        protected long size;
        protected byte[] md5Bytes;

        /// <summary>
        /// Signature Constructor
        /// </summary>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public Signature()
        {
        }

        /// <summary>
        /// Signature Constructor
        /// </summary>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public Signature(long _size, byte[] _bytes)
        {
            size = _size; // File Content Size
            md5Bytes = _bytes; // MD5 Code
        }

        /// <summary>Check if it is the same Signature</summary>
        /// <param name="obj">Signature</param>
        /// <returns>bool: is the same Signature or not</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public bool IsSameSig(Signature obj)
        {
            if (size != obj.Size) return false;
            for (int i = 0; i < 16; i++)
                if (md5Bytes[i] != obj.Md5Bytes[i]) return false;
            return true;
        }

        /// <summary>
        /// get: size
        /// </summary>
        public long Size
        {
            get { return size; }
        }

        /// <summary>
        /// get: md5Bytes
        /// </summary>
        public byte[] Md5Bytes
        {
            get { return md5Bytes; }
        }
    }
    #endregion

    #region HotNews

    /// <summary>
    /// HotNewsBase contains basic data for hot news
    /// </summary>
    [Serializable]
    public class HotNewsBase
    {
        private long id = 0;

        public long ID
        {
            get { return id; }
            set { id = value; }
        }

        public string issueDate = "";
        public string issueTime = "";
        public string effectiveDate = "";
        public string effectiveTime = "";
        public string expiredDate = "";
        public string expiredTime = "";

        public string issueBy = "";
        public string newsTitle = "";
    }

    /// <summary>
    /// HotNews inherit from HotNewsBase. Content of hot news is the only additional attribute.
    /// </summary>
    [Serializable]
    public class HotNews : HotNewsBase
    {
        private string newsContent = "";

        public string IssueDate
        {
            get { return issueDate; }
            set { issueDate = value; }
        }

        public string IssueTime
        {
            get { return issueTime; }
            set { issueTime = value; }
        }

        public string EffectiveDate
        {
            get { return effectiveDate; }
            set { effectiveDate = value; }
        }

        public string EffectiveTime
        {
            get { return effectiveTime; }
            set { effectiveTime = value; }
        }

        public string ExpiredDate
        {
            get { return expiredDate; }
            set { expiredDate = value; }
        }

        public string ExpiredTime
        {
            get { return expiredTime; }
            set { expiredTime = value; }
        }

        public string IssueBy
        {
            get { return issueBy; }
            set { issueBy = value; }
        }

        public string NewsTitle
        {
            get { return newsTitle; }
            set { newsTitle = value; }
        }

        public string NewsContent
        {
            get { return newsContent; }
            set { newsContent = value; }
        }

        /// <summary>
        /// HotNews Constructor
        /// </summary>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public HotNews(string _issueDate, string _effectiveDate, string _expiredDate,
                       string _issueBy, string _newsTitle, string _newsContent,
                       string _issueTime, string _effectiveTime, string _expiredTime)
        {
            issueDate = _issueDate;
            effectiveDate = _effectiveDate;
            expiredDate = _expiredDate;
            issueBy = _issueBy;
            newsTitle = _newsTitle;
            newsContent = _newsContent;

            issueTime = _issueTime;

            effectiveTime = _effectiveTime;
            expiredTime = _expiredTime;
        }

    }

    #endregion
}