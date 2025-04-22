/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//********************************************************************************
//* Thomas    | Ver. 08 | #BR18108  | 2016/04/17                                                               *
//*---------------------------------------------------------------------------      *
//* THOMAS 修改顯示訊息 ,                      *
//********************************************************************************
//* JefferyLin    | Ver. 07 | #BR17150  | 2016/04/17                                                               *
//*---------------------------------------------------------------------------      *
//* 如果目的艙位是5打頭( 5,51,52,53) 代表散貨, 就檢查Category C和M ,                      *
//* 檢查如果AWB No.空白就不搬 跳訊息: 此動作無法執行                                                    *
//********************************************************************************
//* Terri    | Ver. 06 | #BR16120  | 2016/12/08                  (V3.04.01)                                  *
//*---------------------------------------------------------------------------      *
//* 產出ACARS L/S的ZFW、TOW及MACTOW內容，若與PAPPER L/S不同時，         *
//* 則無法送ACARS並給予MESSAGE。                                                                                       *
//********************************************************************************
//* Terri Liu  | Ver. 05 | #BR16111 | 2016/NOV/15                                                                 *
//*--------------------------------------------------------------------------         *
//* 74Y貨機要使用INCREASED CUMULATIVE LOAD時，點選”YES”後，新增:        *
//*『take off C.G. point must after of the forward limit “A” on the trim sheet        *
//********************************************************************************
//* CherryChan   | Ver. 03 | #BR15105  | 2015/APR/14               (V03.01.01)                 *
//*---------------------------------------------------------------------------      *
//Amend Divert flt suffix code, from R to D                                                                              *
//********************************************************************************   
//* CherryChan   | Ver. 02 | #BR10118 | 2010/SEP/10                   (V01.16)                     *
//*---------------------------------------------------------------------------      *
//* For CheckCumulativeLoadLimits method error handle                                                *
//********************************************************************************
//* Fio Sun      | Ver. 01 | #BR07007 | 2007/APR/10                                                               *
//*---------------------------------------------------------------------------      *
//* When TOW >=366,400kg, the “A1+A2+B1+ZC+ZD” must >2268kg.          *
//* add a exception to warning user and loadsheet cant not be produced.                 *
//********************************************************************************  


using System;
using System.Collections;

namespace EwbsCore.Util
{
    /// <summary>
    /// Provides static methods for Flight
    /// </summary>
    public class FlightUtil
    {
        /// <summary>
        /// Restrieve Number From String
        /// </summary>
        /// <param name="value">input string</param>
        /// <param name="from">from</param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static string RestrieveNumberFromString(string value, int from)
        {
            int len = value.Length;
            string sn = "";
            for (; from < len; from++)
            {
                if (char.IsNumber(value[from]))
                    sn += value[from];
                else break;
            }
            return sn;
        }


        /// <summary>Get pax number from Cabin Configuration</summary>
        /// <param name="cabinConfiguration">Cabin Configuration</param>
        /// <returns>int[]: paxn number</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static int[] GetPaxArrayFromCabinConfiguarion(string cabinConfiguration)
        {
            int iStart = 0, iPos = 0;
            ArrayList PaxArray = new ArrayList();
            //copy Configuration data
            string DestString = cabinConfiguration + "/";
            do
            {
                //get number
                iPos = DestString.IndexOf("/", iStart);
                if (iPos > 0 && char.IsLetter(DestString[iPos - 1]))
                {
                    PaxArray.Add(Convert.ToInt32(DestString.Substring(iStart, iPos - iStart - 1)));
                }
                iStart = iPos + 1;
            } while (iPos >= 0);
            return (int[])PaxArray.ToArray(typeof(int));
        }


        /// <summary>get class name from Cabin Configuration</summary>
        /// <param name="cabinConfiguration">Cabin Configuration</param>
        /// <returns>ArrayList：class names</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static ArrayList GetClassNamesFromCabinConfiguarion(string cabinConfiguration)
        {
            int iStart = 0, iPos = 0;

            ArrayList ClassArray = new ArrayList();

            // copy Configuration data
            string DestString = cabinConfiguration + "/";
            do
            {
                //get Class Name
                iPos = DestString.IndexOf("/", iStart);
                if (iPos > 0 && char.IsLetter(DestString[iPos - 1]))
                {
                    ClassArray.Add(new string(DestString[iPos - 1], 1));
                }
                iStart = iPos + 1;
            } while (iPos >= 0);

            return ClassArray;
        }


        /// <summary>
        /// get message between startTag and endTag
        /// </summary>
        /// <param name="message">input string</param>
        /// <param name="startTag">startTag</param>
        /// <param name="endTag">endTag</param>
        /// <returns>message content</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static string retriveDCSMsg(string message, string startTag, string endTag)
        {
            int s = message.IndexOf(startTag);
            if (s < 0) return "";
            string subString = message.Substring(s + startTag.Length);
            int e = subString.IndexOf(endTag);
            if (e < 0) return "";
            return subString.Substring(0, e);
        }


        /// <summary>convert pax class into baggage class</summary>
        /// <param name="PaxClass">pax class</param>
        /// <returns>string : Baggage class</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static string PaxClassToBagClass(string PaxClass)
        {
            switch (PaxClass)
            {
                case "F":
                case "C":
                    return "BC";

                case "Y":
                case "K":
                    return "BY";
                default:
                    return "B";
            }
        }

        //BR15105 public static string DivertFltCode = "R"; //Divert Flt Code
        public static string DivertFltCode = "D"; //Divert Flt Code  //BR15105
        public static string TrainingFltCode = "T"; //Training Flt Code
        public static string SvcWaterDesc = "WATER TANK ADJ."; //Water Tank Adj
    }

    public class EWBSCoreWarnings
    {
        public EWBSCoreWarnings()
        {
        }

        #region Baggage

        public static string BagWtLessThanZero
        {
            get { return "BagWeight < 0."; }
        }

        public static string FileNotFound
        {
            get { return "File not found!!"; }
        }

        #endregion

        #region Cargo

        public static string CgoWtLessThanTWt
        {
            get { return "Error! Cargo Weight cannot be less than its Tare Weight."; }
        }

        public static string CgoSpeLdExceedCgoWt
        {
            get { return "Special load should not exceed cargo weight."; }
        }

        public static string CgoSpeLdExceedMailWt
        {
            get { return "Special load should not exceed mail weight."; }
        }

        public static string CgoSpeLdExceedBagWt
        {
            get { return "Special load should not exceed baggage weight."; }
        }

        public static string SnExpects_3
        {
            get { return "Serial number of \"{0}\" expects \"{1}\" , not \"{2}\"."; }
        }

        public static string CarrCdNotFound_1
        {
            get { return "Carrier code '{0}' cannot be found in database."; }
        }

        public static string UldNotFound_1
        {
            get { return "Database Error! ULD type {0} cannot not be found."; }
        }

        public static string CgoWtExceedsMax_1
        {
            get { return "Error! Cargo Weight exceeds its Maximum Weight({0})."; }
        }

        public static string HeightCdShouldBeEnter_1
        {
            get { return "Error! Height code should be entered in {0} ULD."; }
        }

        public static string UnknownSn_1
        {
            get { return "Unknown serial number:{0}"; }
        }

        public static string CgoUldSerialNoError
        {
            get { return "ULD Serial Number Error!"; }
        }

        public static string CgoExceedsCombinedLoadLmt_2
        {
            get { return "{0} exceeds combined load limitation ({1})."; }
        }

        //#BR07007 <--
        //2007/04/10 Fio Start
        public static string CgoExceedsCombinedLoadLmt_3
        {
            get { return "TOW >= 366,400kg the A1+A2+B1+ZC+ZD must > {1}kg  now only have {2}kg"; }
            //TOW >=366,400kg, the “A1+A2+B1+ZC+ZD” must >2268kg.
        }
        //2007/04/10 Fio End
        //#BR07007 -->


        public static string OverCumulativeLoad_3
        {
            get { return "Cumulative load ({0}) = {1}>{2} "; }
        }

        public static string OverIncreasedCumulativeLoad_3
        {
            get { return "Increased cumulative load ({0}) = {1}>{2} "; }
        }

        public static string PatternExceedsWtLmt_2
        {
            get { return "\"{0}\" exceeds Weight Limit({1})."; }
        }

        public static string UnsymmetricLoadForPosn_3
        {
            get { return "Unsymmetric load for position {0}, the heave side limit at {1}, the other side at {2}"; }
        }

        public static string OverLateralImbalanceLmt_3
        {
            get { return "Error! {0} Side ({1}) is over Lateral Imbalance Limitation({2})!"; }
        }

        public static string CheckedPointIndex_2
        {
            get { return "Checked Point Index({0}) > {1}"; }
        }

        public static string EstimatedAllowedCgoLoadExceeded
        {
            get { return "Estimated_allowed_cargo_load is exceeded."; }
        }

        public static string CheckFuelDistribution
        {
            get { return "Please check Fuel Distribution and Tail Tank."; }
        }

        public static string UserGiveUp
        {
            get { return "User give up."; }
        }

        public static string CgoRemainSpaceNotEnoughtInCmpt_1
        {
            //<!--#BR18108 THOMAS 修改顯示訊息
            //get { return "The remaining space is not enough in \"Cmpt {0}\"."; }
            get { return "The remaining space is not enough in \"Cmpt {0}\"." + "\r\n" + "Not allow to load" + "\r\n" + "2 \"M\" (basecode)"; }
            //#BR18108 -->
        }

        public static string AskApplyIncreasedCmltiveLoad
        {
            get { return "Do you want to apply to INCREASED CUMULATIVE LOAD?"; }
        }

        //BR16111<--
        public static string AskApplyIncreasedCmltiveLoad_remind
        {
            get { return "take off C.G. point must after of the forward limit “A” on the trim sheet"; }
        }

        //BR16111-->


        public static string CgoPsnNotAvlbl_1
        {
            get { return "Position \"{0}\" is not available."; }
        }

        public static string BaseCodeExpects_3
        {
            get { return "Base Code of \"{0}\" expects \"{1}\" , not \"{2}\"."; }
        }

        public static string ExceedCmptPosnLoadLmt_2
        {
            get { return "{0} TTL OVER {1}K."; }//20060515
        }

        public static string HeightCodeExpects_3
        {
            get { return "Height Code of \"{0}\" expects \"{1}\" , not \"{2}\"."; }
        }

        public static string ShcShouldNotBeInCmpt
        {
            get { return "\"{0}\" should not be in \"{1}\"."; }
        }

        public static string UldTypeError
        {
            get { return "ULD Type Error"; }
        }

        public static string CarrierCodeError
        {
            get { return "CarrierCode Error."; }
        }

        public static string UnknownCarrierCodeOrULDType_2
        {
            get { return "Unknown carrier code \"'{0}'\" or ULD type\"{1}\"."; }
        }

        public static string OffloadCargoNow
        {
            get { return "Offload cargo right now."; }
        }

        public static string BulkNotMatched
        {
            get { return "Bulk is not matched."; }
        }

        public static string RestoreNormalCumulativeLoad
        {
            get { return "Do you want to restore to NORMAL CUMULATIVE LOAD?"; }
        }
        //BR10118<--
        public static string CheckCumulativeLoadLimits
        {
            get { return "CheckCumulativeLoadLimits Function Error!! Please Call in charge person!!"; }
        }
        //BR10118-->
        //BR17150 <-- 
        public static string AwbNoEmpty 
        {
            get { return "此動作無法執行!"; }
        }
        //BR17150-->
        #endregion

        #region COM

        public static string ComCantReadUnderlyingStream
        {
            get { return "Can't read from the underlying stream."; }
        }

        public static string ComCantWriteUnderlyingStream
        {
            get { return "Can't write to the underlying stream."; }
        }

        public static string ComCantSeekCompressionStream
        {
            get { return "Can't seek on a compression stream."; }
        }

        public static string ComTreeCantBeEmpty
        {
            get { return "The tree cannot be empty."; }
        }

        public static string ComInvalidBitInStream
        {
            get { return "Invalid bit in the stream."; }
        }

        public static string ComCantSetLengthCompressionStream
        {
            get { return "Can't asynchronously access a compression stream."; }
        }

        public static string ComCantAsyncAccessCompressionStream
        {
            get { return "Can't asynchronously access a compression stream."; }
        }

        public static string ComCantSwapRootNodes
        {
            get { return "Can't swap root nodes."; }
        }

        public static string ComCantSeekBitStream
        {
            get { return "Can't seek on a bit stream."; }
        }

        public static string ComCantWriteThisStream
        {
            get { return "Can't write on this stream."; }
        }

        public static string ComBitMustBe01
        {
            get { return "The bit must be 0 or 1."; }
        }

        public static string ComCantReadThisStream
        {
            get { return "Can't read on this stream."; }
        }

        public static string ComCantSetLengthBitStream
        {
            get { return "Can't set the length of a BitStream."; }
        }

        public static string ComCantAsyncAccessBitStream
        {
            get { return "Can't asynchronously access a bit stream."; }
        }

        public static string ComFileCantBeRead
        {
            get { return "The file could not be read:"; }
        }

        public static string ComCreateDirFailed_1
        {
            get { return "COM:CreateDir failed: {0}"; }
        }

        public static string ComReceiveWrongSignature
        {
            get { return "Receive with Wrong Signature"; }
        }

        public static string ComIOErrorFile
        {
            get { return "IO Error for file: "; }
        }

        public static string ComFileCantBeWritten_2
        {
            get { return "Exception Occurred: The file {0} could not be written:\r\n{1}"; }
        }

        public static string ComFileCantBeWritten
        {
            get { return "ComError: The file could not be written: :"; }
        }

        public static string ComDirServiceSerializeCatchExp
        {
            get { return "DirService.Serialize Catch Exception:"; }
        }

        #endregion

        #region Crew

        public static string CrewExceedsMax_1
        {
            get { return "Error! Total Crew in {0} exceeds MAX!"; }
        }

        public static string CrewExtExceedsRemainingSeat_2
        {
            get { return "Error! Extra Crew Number in {0} class exceeds remaining seats {1}"; }
        }

        public static string CabinConfigurationErrorExtraCrew
        {
            get { return "Cabin configuration error in extracrew calculating."; }
        }

        #endregion

        #region FDB

        public static string FDBUnReasonableTtlFuelWt
        {
            get { return "Unreasonable total fuel weight."; }
        }

        public static string FDBBadZoneNameOrCabinConfig_2
        {
            get { return "Bad zone name'{0}' or cabin configuration '{1}'."; }
        }

        public static string FDBCantConvertSeatCdToZone_1
        {
            get { return "Cannot convert seat code '{0}' to Zone."; }
        }

        public static string FDBCantConvertRowToCls_1
        {
            get { return "Cannot convert row '{0}' to cabin class."; }
        }

        public static string FDBZoneNotFound_1
        {
            get { return "Cannot find zone '{0}' in FDB."; }
        }

        public static string FDBCabinClsNotFound
        {
            get { return "Cannot find cabin class '{0}' in FDB."; }
        }

        public static string FDBUserNotFound_1
        {
            get { return "Database Error, User {0} cannot not be found."; }
        }

        public static string FDBAirtypeNotFound_1
        {
            get { return "Database Error, airtype {0} cannot not be found."; }
        }

        public static string FDBAirlineNotFound_1
        {
            get { return "Database Error, airline {0} cannot not be found."; }
        }

        #endregion

        #region Flight

        public static string FlightCarrCdDontHaveAcType_2
        {
            get { return "{0} do not have {1}."; }
        }

        public static string FlightUnhandlingComplicatedCgoInUnloadCgo
        {
            get { return "Unhandling complicated Cargo in UnloadCargo."; }
        }

        public static string FlightZFWExceedsMax
        {
            get { return "Error! Zero Fuel Weight exceeds MAX Weight!"; }
        }

        public static string FlightZFWExceedsRegulated
        {
            get { return "Error! Zero Fuel Weight exceeds Regulated Weight!"; }
        }

        public static string FlightLDWExceedsMax
        {
            get { return "Error! Landing Weight exceeds MAX Weight!"; }
        }

        public static string FlightLDWExceedsRegulated
        {
            get { return "Error! Landing Weight exceeds Regulated Weight!"; }
        }

        public static string FlightZFIndexNotWithinLmt
        {
            get { return "Error! Index of Zero Fuel Weight is not within the Limits!"; }
        }

        public static string FlightTakeoffIndexNotWithinLmt
        {
            get { return "Error! Index of Takeoff Weight is not within the Limits!"; }
        }

        public static string FlightMACDLWNotWithinLmt_1
        {
            get { return "Error! MACDLW is not within the Limits({0})!"; }
        }

        public static string FlightShouldNotBeNoFitPosn_1
        {
            get { return "Error! {0} should not be no fit position!"; }
        }

        public static string FlightULDSnNotOnBoard_1
        {
            get { return "ULD Serial No {0} is not onboard."; }
        }

        public static string FlightNoSnForContainer
        {
            get { return "No serial number given for ULD(s)."; }
        }

        public static string FlightCargoDuplicated_1
        {
            get { return "Duplicated Cargo {0}."; }
        }

        public static string FlightConsignmentNotOnboard
        {
            get { return "Consignment is not onboard."; }
        }

        #endregion

        #region Fuel

        public static string FuelDensityUnderflow
        {
            get { return "Fuel density is underflow!!"; }
        }

        public static string FuelDensityOverflow
        {
            get { return "Fuel density is overflow!!"; }
        }

        public static string FuelTankFormulaError
        {
            get { return "Tank Formula Error:"; }
        }

        public static string FuelTankWtExceedsMax_1
        {
            get { return "Error! Fuel weight in Tank {0} exceeds MAX!"; }
        }

        public static string FuelDensityViolatesLmt
        {
            get { return "Error! Fuel Density violates the Density Limit!"; }
        }

        public static string FuelTFShouldNotLargerthanTOF
        {
            get { return "Error! Trip Fuel should not be larger than Takeoff Fuel!"; }
        }

        public static string FuelOverflow
        {
            get { return "Fuel is overflow!!"; }
        }

        public static string FuelUnUnreasonableTakeoffFuelorTripFuel
        {
            get { return "Unreasonable Takeoff Fuel or Trip Fuel."; }
        }

        //#BR19016
        public static string FlightTaxiFuelError
        {
            get { return "Taxi fuel error -  Taxi fuel shall be ＞ 0 kg."; }
        }

        #endregion

        #region Output

        //BR16120<--
        public static string LS_ACARS_Check
        {
            get { return "ACARS and PAPER L/S figures are different, please revise L/S and reconfirm that all figures are correct."; }
        }
        //BR16120-->


        #endregion

        #region Pantry

        #endregion

        #region Pax

        public static string PaxClsTtlShouldEqualGdrTtl
        {
            get { return "Error! Total Pax Number from every Class Should be equal to those obtained from Gender!!"; }
        }

        public static string PaxZoneTtlShouldEqualGdrTtl
        {
            get { return "Error! Total Pax Number from every Zone Should be equal to those obtained from Gender!!"; }
        }

        public static string PaxTtlIncludingSOCExceedsMax
        {
            get { return "Total Pax including SOC exceeds MAX"; }
        }

        public static string PaxTtlIncludingSOCBlkExceedsMax
        {
            get { return "Error! Total Pax including SOC and blkd seat exceeds MAX."; }
        }

        public static string PaxZoneExceedsMax_1
        {
            get { return "Error! Pax number of Zone {0} exceeds MAX"; }
        }

        public static string PaxZoneNotMatchCls
        {
            get { return "Error! Pax number of Zone does not match Pax number of Class."; }
        }

        public static string BookedPaxNotMatchAcceptedPax
        {
            get { return "Error! Booked Pax number does not match Accepted Pax number."; }
        }

        #endregion

        #region Svc

        public static string SvcFullWaterTank
        {
            get { return "Full Water Tank"; }
        }

        #endregion

        #region Telex

        #endregion

        #region Util

        public static string UtilFileFinderCantFindInDir_2
        {
            get { return "FileFinder.GetFileName() cannot find {0} in directory {1}"; }
        }

        #endregion

        public static string NewsLock
        {
            get { return "News Lock"; }
        }

        public static string NoNewsExisted
        {
            get { return "No News Existed"; }
        }
    }
}