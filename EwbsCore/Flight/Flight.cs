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
//* Terri    | Ver. 10 | #BR16114  | 2016/11/29                  (V3.04.01)   *
//*---------------------------------------------------------------------------*
//* L/S SI add:                                                               *
//* BR/B7                                                                     *
//* 330-200/300 及321 = > MACZFW (23.6)  ZFW (163.8) FOR FMS                  *
//* B747/B777 => ZFW (163.8) FOR FMS                                          *
//*****************************************************************************
// Jeffery Lin | Ver. 9 | #BR17200	| 2017/03/30                              *
// .NET 1.1 轉 .NET 4.5                                                       *
//*****************************************************************************
//* Terri    | Ver. 8 | #BR16114  | 2016/11/29                  (V3.04.01)   *
//*---------------------------------------------------------------------------*
//* L/S SI add:                                                               *
//* BR/B7                                                                     *
//* 330-200/300 及321 = > MACZFW (23.6)  ZFW (163.8) FOR FMS                  *
//* B747/B777 => ZFW (163.8) FOR FMS                                          *
//*****************************************************************************
//* Jay Sheu  | Ver. 07 | #BR15115 | 2015-Sep-16     (V3.2.1)                 *
//*---------------------------------------------------------------------------*
//* -subtracted unusable fuel to ZFW when air craft is A330                   *
//* 若330機型時，ZFW-不計算UNUSABLE FUEL 並把ACARS的 ()拿掉                   *
//*****************************************************************************
//* Jay Sheu  | Ver. 06 | #BR15113 | 2015-Sep-16     (V3.2.1)                 *
//*---------------------------------------------------------------------------*
//* -Added MTXW to EnumWeightLimitation for setting TXW as the weight limit   *
//* -Added WeightLimitationCmpType class for holding weight and its			  *
//*  EnumWeightLimitation, used for finding the min weight limit, can be used *
//*  for other purpose(s).													  *
//*  檢查TAW/ZFW/LDW/TOW，最大限重落於MTAW，要改以MTOW為主           		  *
//*****************************************************************************
//* Cherry    | Ver. 05 | #BR10107  | 2010/AUG/17    (V1.15)                  *
//*---------------------------------------------------------------------------*
//* For Depart or Arrive "TSA", the flt haul is default "S"                   *
//* 抵達場站亦可設定LONG/SHORT HAUL的設定。									  *
//* (目前針對BR330及BR332抵達松山機場的班機，預設為SHORT HAUL)                *
//*****************************************************************************
//* Cherry    | Ver. 04 | #BR08112  | 2009/JAN/15    (V1.04)                  *
//*---------------------------------------------------------------------------*
//* Can update to PAX/CREW weight                                             *
//*****************************************************************************
//*CHERRY CHAN| Ver. 03 | #BR08108 | 2008/JUL/17                              *
//*---------------------------------------------------------------------------*
//*if app.config haul chk already set, need to refer it to set "S"            *
//*****************************************************************************
//*CHERRY CHAN| Ver. 02 | #BR08104 | 2008/JUL/07                              *
//*---------------------------------------------------------------------------*
//* if no set flt segment, the default haul to set "L"                        *
//*****************************************************************************
//* Fio Sun   | Ver. 01 | #BR070007 | 2007/JUN/04                             *
//*---------------------------------------------------------------------------*
//* Auto catch EPC data. Do not check ECP data.                               *
//*****************************************************************************
//* WUPC      | Ver. 00 | #BR071003 | 2007/OCT/04                             *
//*---------------------------------------------------------------------------*
//* Handle the VTS(Version tolerant serialization) of special load            *
//*****************************************************************************

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using EwbsCore.Baggage;
using EwbsCore.Util;
using FlightDataBase;
using nsBaggage;
using System.Configuration;  //BR08108
namespace EWBS
{
    public delegate bool EwbsExceptionHandler(object sender, bool warning, string str);

	/// <summary>
	/// Weight limitation Enumeration:
	/// </summary>
	public enum EnumWeightLimitation
	{
		MZFW,
		MTOW,
		MLDW,
		MTXW, // #BR15113 -JayS, added MTXW for Taxi Weight
	}
	
	/// <summary>
	/// Flight includes necessory information for Weight & Balance, and supports file serialization and deserialization.
	/// </summary>
	[Serializable]
	public class Flight : ISerializable
	{
		private string status; //Flight Status
		private FlightNumber flt_no; //flight number
		private string[] route; //flight route
		private DateTime std; //scheduled time departure
		private DateTime etd; //estimated time departure
		//BR08104 private string longHaul = "S"; //"L" for long haul and "S" for short haul
		private string longHaul = "L"; //"L" for long haul and "S" for short haul  //BR08104
		private float regulatedZFW; //regulated ZFW
		private float regulatedTOW; //regulated TOW
		private float regulatedLDW; //regulated LDW
		private float plannedZFW; //planned ZFW
		private float plannedTOW; //planned TOW
		private string carrierCode = ""; //Carrier Code
		private string cabinConfiguration = ""; //cabin Configuration
		private int loadsheetVersion = 0; // loadsheetVersion
		private int lifEdition = 1; // LIF edition no
		private bool usingIncreasedCumulativeLoad = false; //apply IncreasedCumulativeLoad
		private int blkdSeat = 0; //blkd Seat
		private string brdgGate = ""; //brdgGate
		private string loadVersion = ""; //loadVersion
		private string unloadVersion = ""; //unloadVersion
		public string FlightFactVersion = ""; //FlightFactVersion
        public string VersionDescription = ""; //#BR19010 Thomas LoadVersion Data
		private Aircraft aircraft = null; //Aircraft
		private bool isKILOS = true; //KILOS or Pound
		private FuelList fuelList; // fuel tanks
		private CrewList crew; //crew
		//BR10117 private CrewList crew1; //crew  //BR08112
		private Pantry pantry; //pantry
		private ServiceItems serviceItems; //Service
		private Pax pax; //pax
		private CargoList deadloadList; //Cargo
		private ConsignmentList consignmentList; //Cargo Consignment
		private CargoList baggageList = null; //Baggage		
		private ConsignmentList bagCsgmtList = null; // Baggage Consignment
		private CargoList unloadCargoList; //unload Cargo(include Baggage & Consignment)
		private Telex telex; //telex
		private string user; //user name
		private bool flagTrimByCmpt = false;
		private ArrayList SpecialLoadExtList; //#BR071003 SpecialLoadExtension List   

		[NonSerialized] private AirTypeEx theAirType = null; //Aircraft Type 
		[NonSerialized] private AirlineEx airline = null; //Airline
		[NonSerialized] private CargoPosnMgr cgoPosnMgr; //CargoPosnMgr
		[NonSerialized] private CargoPosnMgr unloadCgoPosnMgr; //Unload CargoPosnMgr
		[NonSerialized] private BagManager bagMgr; //BagManager
		[NonSerialized] public EwbsExceptionHandler EwbsEventHandler; //EwbsEventHandler
		[NonSerialized] private Formatter theFormatter; //Formatter
		
		#region Flight Constructor(s)
		/// <summary>
		/// Flight Constructor
		/// </summary>
		/// <param name="fltno">Flight number</param>
		/// <param name="acno">Aircraft registration number</param>
		/// <param name="std">Schedule time departure</param>
		/// <param name="route">Flight route</param>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public Flight(string fltno, string acno, DateTime std, string[] route)
		{
            
			this.std = std;
			this.etd = std;
			this.route = route;
			FlightNumber = fltno;
			//Carrier Code
			if (fltno.Length > 2) CarrierCode = fltno.Substring(0, 2);
			ACRegNo = acno; //acno
			telex = new Telex();
			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="info">SerializationInfo</param>
		/// <param name="context">StreamingContext</param>
		public Flight(SerializationInfo info, StreamingContext context)
		{
			try
			{
				//deserialize flight
				status=(string)info.GetValue("status", typeof(string));
				flt_no=(FlightNumber)info.GetValue("flt_no", typeof(FlightNumber));
				route=(string[])info.GetValue("route", typeof(string[]));
				std=(DateTime)info.GetValue("std", typeof(DateTime));
				etd=(DateTime)info.GetValue("etd", typeof(DateTime));

				try
				{
					longHaul=(string)info.GetValue("haul", typeof(string));
				}
				catch
				{
                    /* //BR08104 
                    longHaul="S";
                    if((bool)info.GetValue("longHaul", typeof(bool)))
                        longHaul="L";
                     */

					longHaul="L";  //BR08104
					if((bool)info.GetValue("longHaul", typeof(bool)))  //BR08104
						longHaul="S";  //BR08104
				}

				regulatedZFW=(float)info.GetValue("regulatedZFW", typeof(float));
				regulatedTOW=(float)info.GetValue("regulatedTOW", typeof(float));
				regulatedLDW=(float)info.GetValue("regulatedLDW", typeof(float));
				plannedZFW=(float)info.GetValue("plannedZFW", typeof(float));
				plannedTOW=(float)info.GetValue("plannedTOW", typeof(float));
				carrierCode=(string)info.GetValue("carrierCode", typeof(string));
				cabinConfiguration=(string)info.GetValue("cabinConfiguration", typeof(string));
				loadsheetVersion=(int)info.GetValue("loadsheetVersion", typeof(int));

				if(loadsheetVersion>=(1<<16))
				{
					lifEdition=loadsheetVersion & 0x0ffff;
					loadsheetVersion>>=16;
				}
				else
				{
					lifEdition=(int)info.GetValue("lifEdition", typeof(int));
				}

				usingIncreasedCumulativeLoad=(bool)info.GetValue("usingIncreasedCumulativeLoad", typeof(bool));
				blkdSeat=(int)info.GetValue("blkdSeat", typeof(int));
				brdgGate=(string)info.GetValue("brdgGate", typeof(string));
				loadVersion=(string)info.GetValue("loadVersion", typeof(string));
				unloadVersion=(string)info.GetValue("unloadVersion", typeof(string));
				FlightFactVersion=(string)info.GetValue("FlightFactVersion", typeof(string));
				aircraft=(Aircraft)info.GetValue("aircraft", typeof(Aircraft));
				isKILOS=(bool)info.GetValue("isKILOS", typeof(bool));
				fuelList=(FuelList)info.GetValue("fuelList", typeof(FuelList));
				crew=(CrewList)info.GetValue("crew", typeof(CrewList));
				pantry=(Pantry)info.GetValue("pantry", typeof(Pantry));
				serviceItems=(ServiceItems)info.GetValue("serviceItems", typeof(ServiceItems));
				pax=(Pax)info.GetValue("pax", typeof(Pax));
				deadloadList=(CargoList)info.GetValue("deadloadList", typeof(CargoList));
				consignmentList=(ConsignmentList)info.GetValue("consignmentList", typeof(ConsignmentList));
				baggageList=(CargoList)info.GetValue("baggageList", typeof(CargoList));
				bagCsgmtList=(ConsignmentList)info.GetValue("bagCsgmtList", typeof(ConsignmentList));
				unloadCargoList=(CargoList)info.GetValue("unloadCargoList", typeof(CargoList));
				telex=(Telex)info.GetValue("telex", typeof(Telex));
				user=(string)info.GetValue("user", typeof(string));
 

				try
				{
					flagTrimByCmpt=(bool)info.GetValue("flagTrimByCmpt", typeof(bool));
				}
				catch
				{
					flagTrimByCmpt=false;
					if (!this.IsDivertFlight ||
						this.DeadloadList.Count != 0 ||
						this.BaggageList.Count != 0||
						this.BagCsgmtList.Count != 0)
					{
					}
					else if (this.ConsignmentList.Count == 0)
					{
						flagTrimByCmpt = true;
					}
				}

				//#BR071003 <-- deserialize SpecialLoadExtList
				try
				{
					SpecialLoadExtList=(ArrayList)info.GetValue("SpecialLoadExtList", typeof(ArrayList));
				}
				catch
				{
					SpecialLoadExtList=null;
				}
				//#BR071003 -->
			}
			catch(Exception e)
			{
				throw(new Exception("New version file not supported.\r\n"+e.Message));
			}
		}
		#endregion

		#region Propertys 1

		/// <summary>
		/// get aircraft
		/// </summary>
		public Aircraft Aircraft
		{
			get { return aircraft; }			
		}

		/// <summary>
		/// get AirType
		/// </summary>
		public AirTypeEx ACType
		{
			get { return theAirType; }
		}

		/// <summary>
		/// deadloadList
		/// </summary>
		public CargoList DeadloadList
		{
			get { return deadloadList; }
		}

		/// <summary>
		/// consignmentList
		/// </summary>
		public ConsignmentList ConsignmentList
		{
			get { return consignmentList; }
		}

		/// <summary>
		/// get baggages
		/// </summary>
		public CargoList BaggageList
		{
			get { return baggageList; }
		}

		/// <summary>
		/// get baggage consignments
		/// </summary>
		public ConsignmentList BagCsgmtList
		{
			get { return bagCsgmtList; }
		}

		/// <summary>
		/// get unloadCargoList
		/// </summary>
		public CargoList UnloadCargoList
		{
			get { return unloadCargoList; }
		}

		/// <summary>
		/// get pax
		/// </summary>
		public Pax Pax
		{
			get { return pax; }
		}

		/// <summary>
		/// get crew
		/// </summary>
		public CrewList Crew
		{
			get { return crew; }
		}

		/// <summary>
		/// get pantry
		/// </summary>
		public Pantry Pantry
		{
			get { return pantry; }
		}

		/// <summary>
		/// get fuelList
		/// </summary>
		public FuelList Fuel
		{
			get { return fuelList; }
		}

		/// <summary>
		/// get service
		/// </summary>
		public ServiceItems ServiceItems
		{
			get { return serviceItems; }
		}

		/// <summary>
		/// get Cargo Position Manager
		/// </summary>
		public CargoPosnMgr CargoPosnMgr
		{
			get { return cgoPosnMgr; }
		}

		/// <summary>
		/// get telex
		/// </summary>
		public Telex Telex
		{
			get { return telex; }
		}

		/// <summary>
		/// get baggage Manager
		/// </summary>
		public BagManager BagMgr
		{
			get { return bagMgr; }
		}

		/// <summary>
		/// get/set isKILOS
		/// </summary>
		public bool IsKILOS
		{
			get { return isKILOS; }
			set { isKILOS = value; }
		}


		/// <summary>
		/// get Airline
		/// </summary>
		public AirlineEx Airline
		{
			get
			{
				if (airline == null)
				{
					airline = FDB.Instance.GetAirline(flt_no.CarrierCode);
				}
				return airline;
			}
		}

		/// <summary>
		/// Author
		/// </summary>
		public string Author
		{
			get { return user; }
			set { user = value; }
		}

		/// <summary>
		/// get Unload Cargo Position Manager
		/// </summary>
		public CargoPosnMgr UnloadCargoPosnMgr
		{
			get { return unloadCgoPosnMgr; }
		}

		/// <summary>
		/// get theFormatter
		/// </summary>
		public Formatter Formatter
		{
			get { return theFormatter; }
		}

		/// <summary>
		/// get the departure station.
		/// </summary>
		public string From
		{
			get { return this.route[0]; }
		}

		/// <summary>
		/// get all the stations excepts depature station.
		/// </summary>
		public string To
		{
			get
			{
				string str = ""; 
				str += this.route[1];
				for (int i = 2; i < this.route.Length; i++)
				{
					if (this.route[i] == null || this.route[i] == "") break;
					str += ("/" + this.route[i]);
				}
				return str;
			}
		}

		/// <summary>Trim by compartment or not</summary>
		public bool bTrimByCmpt
		{
			get{return flagTrimByCmpt;}
			set{flagTrimByCmpt=value;}
		}
		#endregion

		#region Propertys 2

		/// <summary>
		/// get Baggage/Cargo/Mail Weight
		/// </summary>
		private float BagCgoMailWt
		{
			get { return Convert.ToSingle(cgoPosnMgr.getWeight()); }
		}

		/// <summary>
		/// get Baggage/Cargo/Mail Index
		/// </summary>
		private float BagCgoMailIndex
		{
			get { return Convert.ToSingle(cgoPosnMgr.getIndex()); }
		}

		/// <summary>
		/// get Total Traffic Weight
		/// </summary>
		public float TotalTrafficWt
		{
			get { return (float) pax.getWeight() + BagCgoMailWt; }
		}

		/// <summary>
		/// get Total Traffic Index
		/// </summary>
		public float TotalTrafficIndex
		{
			//Total Traffic Index = PAX Index + B/C/M Index
			get { return (float) pax.getIndex() + BagCgoMailIndex; }
		}

		/// <summary>
		/// get Dry Operating Weight
		/// </summary>
		public float DOW
		{
			//Dry Operating Weight = Basic Weight + Crew Weight + Pantry Weight + Service Weight
			get { return aircraft.BW + crew.GetWeight() + pantry.GetWeight() + serviceItems.GetWeight(); }
		}

		/// <summary>
		/// get Dry Operating Index
		/// </summary>
		public float DOI
		{
			//Dry Operating Index = Basic Index + Crew Index + Pantry Index + Service Index
			get { return aircraft.BI + crew.GetIndex() + pantry.GetIndex() + serviceItems.GetIndex(); }
		}

		/// <summary>
		/// get Operating Weight
		/// </summary>
		private float OperatingWeight
		{
			//Operating Weight = DOW + Takeoff Fuel
			get { return DOW + Fuel.TakeoffFuel; }
		}

		/// <summary>
		/// get Deadload Weight
		/// </summary>
		public float DLW
		{
			//DLW = B/C/M Weight + DOW
			get { return BagCgoMailWt + DOW; }
		}

		/// <summary>
		/// get Deadload Index
		/// </summary>
		public float DLI
		{
			//DLI = B/C/M Index + DOI
			get { return BagCgoMailIndex + DOI; }
		}

		/// <summary>
		/// get Zefo Fuel Weight
		/// </summary>
		public float ZFW
		{
			//Zefo Fuel Weight=Total Traffic Weight + Dry Operating Weight
			get { return DOW + TotalTrafficWt; }
		}

		/// <summary>
		/// get Takeoff Weight
		/// </summary>
		public float TOW
		{
			//Takeoff Weight = Zero Fuel Weight + Tekeoff Fuel
			get { return ZFW + fuelList.TakeoffFuel; }
		}

		/// <summary>
		/// get Landing Weight
		/// </summary>
		public float LDW
		{
			//Landing Weight=Takeoff Weight - Trip Fuel
			get { return TOW - fuelList.TripFuel; }
		}

		/// <summary>
		/// get Taxi Weight
		/// </summary>
		public float TW
		{
			//Taxi Weight=Zero Fuel Weight + Taxi Fuel
			get { return TOW + fuelList.TaxiFuel; }
		}

		/// <summary>
		/// get LIZFW
		/// </summary>
		public float LIZFW
		{
			//LIZFW=DOI + TotalTrafficIndex
			get { return DOI + TotalTrafficIndex; }
		}

		/// <summary>
		/// get LITOW
		/// </summary>
		public float LITOW
		{
			//LITOW = LIZFW + Tekeoff Fuel Index
			get { return LIZFW + fuelList.TOFIndex; }
		}

		/// <summary>
		/// get MACZFW
		/// </summary>
		public float MACZFW
		{
			get 
			{
				return theAirType.CalcMac(LIZFW, ZFW);
			}
		}

		// #BR15115 <-- 2015/10/07 Jay - subtracted unusable fuel to ZFW when air craft is A330
		/// <summary>MACZFW without unusable fuel</summary>
		public float MACZFW_NoUnUsableFuel
		{
			get
			{
				//BR16114 
				if(this.ServiceItems != null) //&& this.ACType.FullAirTypeName.StartsWith("BR33"))
				{
					ServiceItemList svc = new ServiceItemList();
 
					// get all unusable fuel sv items
					foreach (SvcItem svcItem in ServiceItems.ServicesList)
					{
						if (svcItem.Desc.ToUpper().IndexOf("UNUSABLE") >= 0)
						{
							svc.Add(svcItem);
						}
					}

					//remove unusable fuel items from service list first
					foreach(SvcItem s in svc)
					{
						this.ServiceItems.ServicesList.Remove(s);
					}
					// calculate MACZFW
					float tmp = theAirType.CalcMac(LIZFW, ZFW);
					
					// then insert unusable fuel back to the list
					foreach(SvcItem itm in svc)
					{
						this.ServiceItems.ServicesList.Add(itm);
					}

					return tmp;
				}
				return 0f;
			}
		}
		// #BR15115 -->

		/// <summary>
		/// get MACTOW
		/// </summary>
		public float MACTOW
		{
			get { return theAirType.CalcMac(LITOW, TOW); }
		}

		/// <summary>
		/// get MACDLW
		/// </summary>
		public float MACDLW
		{
			get { return theAirType.CalcMac(DLI, DLW); }
		}

		/// <summary>
		/// get FWD/AFT Limit of ZFW
		/// </summary>
		public WtLmt ZFWTrim
		{
			get { return theAirType.FindZFWTrim(this, ZFW); }
		}

		/// <summary>
		/// get FWD/AFT Limit of TOW
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public WtLmt TOWTrim
		{
			get { return theAirType.FindTOWTrim(this, TOW); }
		}

		/// <summary>
		/// get Ideal Trim of ZFW
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
        public float IdealTrim
        {
            //<!-- #BR18110 787
            //get { return theAirType.FindIdealTrim(ZFW); }
            get {
                    if (System.Configuration.ConfigurationManager.AppSettings["RowZoneMode"] == "Y")
                    {
                        if (Pax.BTrimByZone)
                        {
                            return theAirType.FindIdealTrimZone(ZFW);
                        }
                        else
                        {
                            return theAirType.FindIdealTrim(ZFW);
                        }
                    }
                    else
                    {
                        return theAirType.FindIdealTrim(ZFW); 
                    }
                }
            //#BR18110-->
        }

		/// <summary>
		/// get Maximum Zero Fuel Weight
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float MZFW
		{
			get { return findMaxWt("MZFW", longHaul); }
		}

		/// <summary>
		/// get Maximum takeoff weight
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float MTOW
		{
			get { return findMaxWt("MTOW", longHaul); }
		}

		/// <summary>
		/// get Maximum landing weight
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float MLDW
		{
			get { return findMaxWt("MLDW", longHaul); }
		}

		/// <summary>
		/// get Maximum taxi weight
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float MTW
		{
			get { return findMaxWt("MTW", longHaul); }
		}

		/// <summary>
		/// get Actual Maximum Zero Fuel Weight
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float ActualMZFW
		{
			get
			{
				double mzfw = this.MZFW;
				if (this.regulatedZFW > 0) mzfw = Math.Min(mzfw, this.regulatedZFW);
				return Convert.ToSingle(mzfw);
			}
		}

		/// <summary>
		/// get Actual Maximum Takeoff Weight
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float ActualMTOW
		{
			get
			{
				double mtow = this.MTOW;
				if (this.regulatedTOW > 0) mtow = Math.Min(mtow, this.regulatedTOW);
				return Convert.ToSingle(mtow);
			}
		}

		/// <summary>
		/// get Actual Maximum Landing Weight
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float ActualMLDW
		{
			get
			{
				double mldw = this.MLDW;
				if (this.regulatedLDW > 0) mldw = Math.Min(mldw, this.regulatedLDW);
				return Convert.ToSingle(mldw);
			}
		}

		/// <summary>
		/// get/set regulated Zero Fuel Weight
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float RZFW
		{
			get { return regulatedZFW; }
			set { regulatedZFW = value; }
		}

		/// <summary>
		/// get/set Regulated TOW
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float RTOW
		{
			get { return regulatedTOW; }
			set { regulatedTOW = value; }
		}

		/// <summary>
		/// get/set Regulated LDW
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float RLDW
		{
			get { return regulatedLDW; }
			set { regulatedLDW = value; }
		}

		/// <summary>
		/// get/set Planned ZFW
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float PZFW
		{
			get { return plannedZFW; }
			set { plannedZFW = value; }
		}

		/// <summary>
		/// get/set  planned takeoff weight
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public float PTOW
		{
			get { return plannedTOW; }
			set { plannedTOW = value; }
		}

		/// <summary>
		/// get/set  Aircraft registration number
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string ACRegNo
		{
			get { return aircraft == null ? "" : aircraft.Name; }
			set
			{
				airinfoAirlineAirtypeAircode ac = FDB.Instance.GetAC(value);
				bool reset = false; //change airtype
				if (aircraft == null || !aircraft.Name.Equals(ac.name))
				{
					//Get Carrier Code By AC
					string carrCd = FDB.Instance.GetAirlineNameByAC(value);
					//Get AirType Name By AC
					string acType = FDB.Instance.GetAirTypeNameByAC(value);

					//create an aircraft
					aircraft = new Aircraft(ac);

					//get airtype from FDB
					AirTypeEx airtype = FDB.Instance.GetAirType(carrCd, acType);

					//airtype cannot be null
					if (airtype == null) throw(new Exception(string.Format(EWBSCoreWarnings.FlightCarrCdDontHaveAcType_2, carrCd, acType)));

					//change airtype ?
					if (theAirType != airtype && theAirType != null)
					{
						theAirType = airtype;
						//these part have to be reserved.
						fuelList.CopyFrom(new FuelList(this));
						crew.CopyFrom(new CrewList(this));
						pantry.CopyFrom(new Pantry(this));
						serviceItems.CopyFrom(new ServiceItems(this));
						pax.CopyFrom(new Pax(this));

						//these part have to reset/clear
						baggageList.Clear();
						bagCsgmtList.Clear();
						unloadCargoList.Clear();
						bagMgr = null;
						cgoPosnMgr = null;
						unloadCgoPosnMgr = null;
						this.plannedZFW = 0;
						this.plannedTOW = 0;
						this.regulatedZFW = 0;
						this.regulatedTOW = 0;
						this.regulatedLDW = 0;

						reset = true; //
					}
					else theAirType = airtype;

					//reset fuelList, crew, pantry, service, pax, deadload ..., if null
					if (fuelList == null) fuelList = new FuelList(this);
					if (crew == null) crew = new CrewList(this);
					if (pantry == null) pantry = new Pantry(this);
					if (serviceItems == null) serviceItems = new ServiceItems(this);
					if (pax == null) pax = new Pax(this);
					if (deadloadList == null) deadloadList = new CargoList();
					if (consignmentList == null) consignmentList = new ConsignmentList(null);
					if (baggageList == null) baggageList = new CargoList();
					if (bagCsgmtList == null) bagCsgmtList = new ConsignmentList(null);
					if (unloadCargoList == null) unloadCargoList = new CargoList();
					if (bagMgr == null) bagMgr = new BagManager(this);
					if (cgoPosnMgr == null) cgoPosnMgr = new CargoPosnMgr(this);
					if (unloadCgoPosnMgr == null) unloadCgoPosnMgr = new CargoPosnMgr(this);

					//Initial Flight
					Init();

					//設定LoadVersion
					AirtypeCargoVersionLoadVersion theLoadVersion = theAirType.GetDefaultLoadVersion();
					if (theLoadVersion != null)
					{
						string versions = Strings.ListToString(this.CargoPosnMgr.LoadVersionNames);
						if (!reset || LoadVersion == "" || versions.IndexOf(LoadVersion) < 0)
						{
							LoadVersion = theLoadVersion.name; //defualt LoadVersion
						}
						else
						{
							LoadVersion = LoadVersion; //reset LoadVersion
						}

						//setup UnloadVersion
						UnloadVersion = LoadVersion;
					}

					//upload deadload to aircraft
					this.CargoPosnMgr.Init(deadloadList, true);
					this.CargoPosnMgr.Init(consignmentList, false);

					//reset Formatter
					theFormatter = new Formatter(this);
				}
			}
		}

		/// <summary>
		/// get or set Flight number
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string FlightNumber
		{
			get { return flt_no.ToString(); }
			set { flt_no = new FlightNumber(value); }
		}

		/// <summary>
		/// get Flight Name
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string Name
		{
			get
			{
				return String.Format("{0}-{1}-{2}",this.FlightNumber,this.STD.ToString("ddMMM", new CultureInfo("en-US")).ToUpper(),this.Route[1]);
			}
		}

		/// <summary>
		/// get or set Carrier Code
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string CarrierCode
		{
			get { return carrierCode == "" ? flt_no.CarrierCode : carrierCode; }
			set
			{
				carrierCode = value;
				airline = FDB.Instance.GetAirline(carrierCode);
			}
		}

		/// <summary>set Suffix Code</summary>
		/// <param name="suffix">Suffix Code</param>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public void SetSuffixCode(char suffix)
		{
			flt_no.SuffixCode = suffix;
		}

		/// <summary>
		/// get or set flight status
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string Status
		{
			get { return status == null ? "" : status; }
			set { status = value; }
		}

		/// <summary>
		/// get/set scheduled time departure
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public DateTime STD
		{
			get { return std; }
			set { std = value; }
		}

		/// <summary>
		/// get/set estimated time departure
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public DateTime ETD
		{
			get { return etd; }
			set { etd = value; }
		}

		/// <summary>
		/// get/set route
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string[] Route
		{
			get { return route; }
			set { route = value; }
		}

		/// <summary>
		/// get/set long or short haul
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string Haul
		{
			get { return longHaul; }
			set { longHaul = value.ToUpper(); }
		}

		/// <summary>
		/// get or set Cabin Configuration
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string CabinConfiguration
		{
			get { return cabinConfiguration.Trim(); }
			set { cabinConfiguration = value; }
		}

		/// <summary>
		/// get or set Loadsheet version index
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public int LoadsheetVersion
		{
			get { return loadsheetVersion; }
			set
			{
				//Baggage Statistic
				BaggageStatistic.Instance.Add(this);
				loadsheetVersion = value;
			}
		}

		/// <summary>
		/// get or set LIR Edition Number
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public int LIREDNo
		{
			get { return this.lifEdition; }
			set { lifEdition = value; }
		}


		/// <summary>
		/// whether it uses Increased Cumulative Load or not.
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public bool UsingIncreasedCumulativeLoad
		{
			get { return usingIncreasedCumulativeLoad; }
			set { usingIncreasedCumulativeLoad = value; }
		}

		/// <summary>
		/// get or set load version
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string LoadVersion
		{
			get { return loadVersion; }
			set
			{
				try
				{
					cgoPosnMgr.SetLoadVersion(value);
					loadVersion = value;
				}
				catch (Exception e)
				{
					throw(e);
				}

			}
		}

		/// <summary>
		/// get or set unload version
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public string UnloadVersion
		{
			get { return unloadVersion == "" ? loadVersion : unloadVersion; }
			set
			{
				try
				{
					unloadCgoPosnMgr.SetLoadVersion(value);
					unloadVersion = value;
				}
				catch (Exception e)
				{
					throw(e);
				}

			}
		}

		/// <summary>
		/// divert flight or not
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public bool IsDivertFlight
		{
			get { return FlightUtil.DivertFltCode.IndexOf(flt_no.SuffixCode) == 0; }
		}

		#endregion

		#region Serialize/DeSerialize Methods

		/// <summary>Serialize flight to disk</summary>
		/// <param name="fileStream">File in FileStream</param>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public void Serialize(FileStream fileStream)
		{
			CargoPosnMgr theCargoPosnMgr = this.CargoPosnMgr;
			theCargoPosnMgr.Init(null, true);
			theCargoPosnMgr.Init(this.BaggageList, false);
			theCargoPosnMgr.Init(this.BagCsgmtList, false);
			theCargoPosnMgr.Init(this.DeadloadList, false);
			theCargoPosnMgr.Init(this.ConsignmentList, false);

			//#BR071003 <-- Prepare the Special Load Extension List for Serialization
			accessAllSpecialLoad(this);
			//#BR071003 -->

			// use binary formatter
			BinaryFormatter binaryFormatter = new BinaryFormatter();

			// serialize to disk
			binaryFormatter.Serialize(fileStream, this);
		}

		/// <summary>DeSerialize flight from disk</summary>
		/// <param name="fileStream">File in FileStream</param>
		/// <returns>Flight Object</returns>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public static Flight DeSerialize(FileStream fileStream)
		{			
			// use binary formatter
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
			// DeSerialize from disk
			//myFileStream.Close();
			Flight retVal = (Flight) binaryFormatter.Deserialize(fileStream);
			// fix the nonserialized data up
			retVal.OnDeserialization();
			return retVal;

			//BR08112-->
			
//			// use binary formatter
//			BinaryFormatter binaryFormatter = new BinaryFormatter();
//			binaryFormatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
//			// DeSerialize from disk
//			Flight retVal = (Flight) binaryFormatter.Deserialize(fileStream);
//			// fix the nonserialized data up
//			retVal.OnDeserialization();
//			return retVal;

			//BR08112-->

		}

		/// <summary>fix the nonserialized data up</summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public void OnDeserialization()
		{
			//Get carrier code by ac
			string carrCd = FDB.Instance.GetAirlineNameByAC(aircraft.Name);
			//Get airtype name by ac
			string acType = FDB.Instance.GetAirTypeNameByAC(aircraft.Name);
			//get airtype from FDB
			theAirType = FDB.Instance.GetAirType(carrCd, acType);
			//airtype cannot be null
			if (theAirType == null) throw(new Exception((string.Format(EWBSCoreWarnings.FlightCarrCdDontHaveAcType_2, carrCd, acType))));
			//get airline from FDB
			if (carrierCode == "")
			{
				airline = FDB.Instance.GetAirline(flt_no.CarrierCode);
			}
			else
			{
				airline = FDB.Instance.GetAirline(carrierCode);
			}

			//fix up fuel, crew, service and pax
			fuelList.OnDeserialization(this);
			crew.OnDeserialization(this);
			serviceItems.OnDeserialization(this);
			pax.OnDeserialization(this);

			//reset cgoPosnMgr, unloadCgoPosnMgr and bagMgr
			cgoPosnMgr = new CargoPosnMgr(this);
			unloadCgoPosnMgr = new CargoPosnMgr(this);
			bagMgr = new BagManager(this);

			//set LoadVersion
			LoadVersion = this.loadVersion;
			//reset UnloadVersion
			UnloadVersion = UnloadVersion;
			//unload Procedure
			UnloadProcedure(route); 
			//MarkNoFit
			cgoPosnMgr.MarkNoFit();
			//reset theFormatter
			this.theFormatter = new Formatter(this);
			//fix up plannedZFW and plannedTOW
			if (this.plannedZFW < 1000f) this.plannedZFW *= 1000f;
			if (this.plannedTOW < 1000f) this.plannedTOW *= 1000f;
			//#BR071003 <-- Set the Special Load Extension
			if (SpecialLoadExtList != null)
			{
				foreach (SpecialLoadExt theSLE in SpecialLoadExtList)
				{
					theSLE.setSLExt();
				}
				SpecialLoadExtList.Clear();
			}
			//#BR071003 -->
		}

		///#BR071003 <--
		/// <summary>
		///  Access all of the Special Load
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		private void accessAllSpecialLoad(Flight theFlight)
		{
			SpecialLoadExtList = new ArrayList();
			formSLEList(theFlight.DeadloadList);
			formSLEList(theFlight.ConsignmentList);
			formSLEList(theFlight.BaggageList);
			formSLEList(theFlight.BagCsgmtList);
			formSLEList(theFlight.UnloadCargoList);
		}

		/// <summary>
		/// Add Special Load Extension Object into ArrayList
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		private void formSLEList(ArrayList theList)
		{
			for(int i=theList.Count-1; i>=0; i--)
			{
				ICargo CB = theList[i] as ICargo;
				if (CB is SpecialLoad)
				{
					SpecialLoadExt theSLE = new SpecialLoadExt(CB as SpecialLoad);
					SpecialLoadExtList.Add(theSLE);
				}
				formSLEList(CB.Consignments);
			}
		}
		//#BR071003 -->

		/// <summary>Follow the unloading table to unload the cargo of some ArrayList</summary>
		/// <param name="aList">Cargo list</param>
		/// <param name="dest">Destination</param>
		/// <param name="transit">Transit or not</param>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		private void UnloadCargo(ArrayList aList, string dest, bool transit)
		{
			for (int i = aList.Count - 1; i >= 0; i--)
			{
				ICargo cgo = aList[i] as ICargo;
				if (cgo.Dest.Equals(dest)) // offload cargo
				{
					aList.RemoveAt(i);
				}
				else if (cgo.Dest.IndexOf(dest) >= 0) // offload consignment
				{
					for (int j = cgo.Consignments.Count - 1; j >= 0; j--)
					{
						Consignment aConsignment = cgo.Consignments[j] as Consignment;
						if (aConsignment.Dest.Equals(dest))
						{
							cgo.Consignments.RemoveAt(j);
						}
					} //for
				}

				//process transit cargo
				if (transit)
				{
					cgo.TransitCargoProc();
				}
			}
		}

		/// <summary>
		/// reset data
		/// </summary>
		public void reset()
		{
			//reset following data
			this.Telex.LS.Text = "";
			this.LoadsheetVersion = 0;

			this.Fuel.FuelLoading = FuelLoadingStyle.STD;
			this.Fuel.TripFuel = 0;
			this.Fuel.TakeoffFuel = 0;
			this.plannedZFW = 0;
			this.plannedTOW = 0;
			this.regulatedZFW = 0;
			this.regulatedTOW = 0;
			this.regulatedLDW = 0;
			this.status = "";
			this.telex.ACARS.Sender = "";
			this.telex.ALI.Sender = "";
			this.telex.CPM.Sender = "";
			this.telex.LDM.Sender = "";
			this.telex.LIR.Sender = "";
			this.telex.LPM.Sender = "";
			this.telex.LS.Sender = "";
			this.telex.NOTOC.Sender = "";
			this.telex.SLS.Sender = "";
			this.telex.UCM.Sender = "";
			this.telex.UWS.Sender = "";
			this.telex.ACARS.Receiver = "";
			this.telex.ALI.Receiver = "";
			this.telex.CPM.Receiver = "";
			this.telex.LDM.Receiver = "";
			this.telex.LIR.Receiver = "";
			this.telex.LPM.Receiver = "";
			this.telex.LS.Receiver = "";
			this.telex.NOTOC.Receiver = "";
			this.telex.SLS.Receiver = "";
			this.telex.UCM.Receiver = "";
			this.telex.UWS.Receiver = "";
		}

		/// <summary>Unload Procedure</summary>
		/// <param name="destList">new route</param>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public void UnloadProcedure(string[] destList)
		{
			if (this.IsDivertFlight && destList[1]==this.route[1] && destList[0]!=this.route[0])
			{
				//reset data
				reset();
			}
			else if (destList != null && destList.Length > 0)
			{
				string from = destList[0]; //起點

				bool transit = this.route[1].Equals(from); //如果起點跟WBF路徑的中途點相同，就是第二段航班

				ArrayList newRoute = new ArrayList();
				newRoute.AddRange(this.route); 
				if (transit)
				{
					//remove original departure
					newRoute.RemoveAt(0); //移除第一段起點路徑

					//reset data
					reset();
				}

				//#BR071005 <-- append new route if absent
				for (int i = 0; i < Math.Max(newRoute.Count, destList.Length); i++)
				{
					if (i < destList.Length && i >= newRoute.Count)
					{
						newRoute.Add(destList[i]);
					}
				}
				route = (string[]) newRoute.ToArray(typeof (string));
				//#BR071005 -->

				//Unload Cargo
				this.UnloadVersion = this.LoadVersion;

				//reset CargoPosnMgr
				this.CargoPosnMgr.Init(null, true);

				//generate unloadCargoList
				if (transit)
				{
					unloadCargoList.Clear();
					ArrayList aLiat = new ArrayList();
					aLiat.AddRange(deadloadList);
					aLiat.AddRange(consignmentList);
					aLiat.AddRange(baggageList);
					aLiat.AddRange(bagCsgmtList);
					foreach (ICargo cgo in aLiat)
					{
						if (cgo is Cargo)
							unloadCargoList.Add(new Cargo(cgo as Cargo));
						else if (cgo is Baggage)
							unloadCargoList.Add(new Baggage(cgo as Baggage));
						else if (cgo is SpecialLoad)
							unloadCargoList.Add(new SpecialLoad(cgo as SpecialLoad));
						else if (cgo is Consignment)
							unloadCargoList.Add(new Consignment(cgo as Consignment));
					}
				}


				//Unload baggage
				UnloadCargo(baggageList, from, transit);
				this.CargoPosnMgr.Init(baggageList, false);
				//Unload baggage consignment
				UnloadCargo(bagCsgmtList, from, transit);
				this.CargoPosnMgr.Init(bagCsgmtList, false);
				//Unload cargo
				UnloadCargo(deadloadList, from, transit);
				this.CargoPosnMgr.Init(deadloadList, false);
				//Unload consignment
				UnloadCargo(consignmentList, from, transit);
				this.CargoPosnMgr.Init(consignmentList, false);

				this.UnloadCargoPosnMgr.Init(unloadCargoList, true);

				//unload pax
				this.Pax.UnloadProcedure(from);

				//20060310
				try
				{
					//baggage ReCalculate
					bagMgr.ReCalculate();
				}
				catch
				{
				}

				if (transit)
				{
					//init flight
					this.Init();

					//init baggage rules
					BagRuleList bagRuleList = bagMgr.BagRuleList;
					for (int i = bagRuleList.Count - 1; i >= 0; i--)
					{
						if ((bagRuleList[i] as BagRule).Qty == 0)
							bagRuleList.RemoveAt(i);
					}
				}
			}
		}

		#endregion

		#region methods

		/// <summary>
		/// find Weight Limitation
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public EnumWeightLimitation WeightLimitation
		{
			get
			{
				double mzfw = this.ActualMZFW + this.Fuel.TakeoffFuel; //calculated mtow by actual mzfw
				double mtow = this.ActualMTOW;
				double mldw = this.ActualMLDW + this.Fuel.TripFuel; //calculated mtow by actual mzfw
				
				//<-- BR15113
				#region #BR15113 <-- 新增 Taxi Weight 比對
				
				double mtxw = this.MTW - this.Fuel.TaxiFuel;

				// 將重量與類別作成一個 class，取最小的重量返回它的類別。方便新增類別時的比對
				WeightLimitationCmpType mzfwCmp = new WeightLimitationCmpType(EnumWeightLimitation.MZFW, mzfw);
				WeightLimitationCmpType mtowCmp = new WeightLimitationCmpType(EnumWeightLimitation.MTOW, mtow);
				WeightLimitationCmpType mldwCmp = new WeightLimitationCmpType(EnumWeightLimitation.MLDW, mldw);
				WeightLimitationCmpType mtxwCmp = new WeightLimitationCmpType(EnumWeightLimitation.MTXW, mtxw);

				// 將所有類別塞入 array，用 array 來取最小的重量
				WeightLimitationCmpType[] wtList = new WeightLimitationCmpType[] { mzfwCmp, mtowCmp, mldwCmp, mtxwCmp };
				
				WeightLimitationCmpType minCmp = wtList[0];
				
				for (int i = 1; i < wtList.Length; ++i)
				{
					if (minCmp.weight > wtList[i].weight)
						minCmp = wtList[i];
				}

				return minCmp.wtType;

				#endregion
				//BR15113-->
				
				// #BR15113<-- 以下是原本的 code，將不會執行
                //if (mtow < mldw)
                //{
                //    if (mtow < mzfw)
                //        return EnumWeightLimitation.MTOW;
                //}
                //else
                //{
                //    if (mldw < mzfw)
                //        return EnumWeightLimitation.MLDW;
                //}
                //return EnumWeightLimitation.MZFW; //default wt limitation
				// #BR15113 -->
			}
		}
		/// <summary>
		/// Holds the weight and enum type of weight limit, used for comparisons
		/// </summary>
		private class WeightLimitationCmpType
		{
			public EnumWeightLimitation wtType;
			public double weight;

			public WeightLimitationCmpType(EnumWeightLimitation wtType, double weight)
			{
				this.wtType = wtType;
				this.weight = weight;
			}
		}

		
		/// <summary>
		/// calculate UnderLoad
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public double UnderLoad
		{
			get
			{
				double mzfw = this.ActualMZFW + this.Fuel.TakeoffFuel; //calculated mtow by actual mzfw
				double mtow = this.ActualMTOW;
				double mldw = this.ActualMLDW + this.Fuel.TripFuel; //calculated mtow by actual mzfw
				
				// #BR15113 <--
				double mtxw = this.MTW - this.Fuel.TaxiFuel; // Taxi Weight
				
				// 將全部重量塞入 array，使用 array 來取最小值
				double[] wtList = new double[] { mzfw, mtow, mldw, mtxw };

				double min = wtList[0];

				for(int i = 1; i < wtList.Length; ++i)
				{
					if (min > wtList[i])
						min = wtList[i];
				}

				return min - this.OperatingWeight - this.TotalTrafficWt;
				// #BR15113 --> 以下是原本的 code，將不會執行
//BR15113<--
//				//find allowedWeightForTakeoff
//				double allowedWeightForTakeoff = mzfw;
//				if (mtow < mldw)
//				{
//					if (mtow < mzfw)
//						allowedWeightForTakeoff = mtow;
//				}
//				else
//				{
//					if (mldw < mzfw)
//						allowedWeightForTakeoff = mldw;
//				}
//				return allowedWeightForTakeoff - this.OperatingWeight - this.TotalTrafficWt;
//Br15113-->
			}
		}

		/// <summary>
		/// get/set BlkdSeat
		/// </summary>
		public int BlkdSeat
		{
			get { return blkdSeat; }
			set { blkdSeat = value; }
		}

		/// <summary>
		/// get/set BrdgGate
		/// </summary>
		public string BrdgGate
		{
			get { return brdgGate; }
			set { brdgGate = value; }
		}

		/// <summary>
		/// Calculate Check Point Index
		/// </summary>
		public double CPI
		{
			get { return this.CargoPosnMgr.CalcuCPI() + this.DOI; }
		}

		/// <summary>
		/// Check Point Index for display
		/// </summary>
		public double DisplayCPI
		{
			get { return Math.Ceiling(this.CPI); }
		}


		/// <summary>
		/// Calculate Stabilizer Trim
		/// </summary>
		public StabilizerTrimItem[] StabilizerTrim
		{
			get
			{
				AirTypeEx theAirType = this.ACType;
				if (theAirType != null)
				{
					return theAirType.StabilizerTrim(this.TOW, this.MACTOW);
				}
				return null;
			}
		}


		/// <summary>Get default fuel density</summary>
		/// <returns>default fuel density</returns></returns>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		private float GetFuelDensity()
		{
			if (airline != null)
			{
				return airline.GetFuelDensity(route[0]);
			}
			return 0.792F;
		}

		/// <summary>flight check for weight & balance</summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public void Check()  
		{
            //<!--#BR19016
            //if (this.Fuel.TaxiFuel <= 0)
            //{
            //    throw (new Exception(EWBSCoreWarnings.FlightTaxiFuelError));
            //}

			//1.check if ZFW must be less or equal then MZFW and Regulated MZFW
			if (this.ZFW > this.ActualMZFW)
			{
				if (this.ZFW > this.MZFW)
					throw(new Exception("Error! Zero Fuel Weight exceeds MAX Weight!"));
				else
					throw(new Exception("Error! Zero Fuel Weight exceeds Regulated Weight!"));
			}
			//2.check if TOW must be less or equal then MTOW and Regulated MTOW
			if (this.TOW > this.ActualMTOW)
			{
				if (this.TOW > this.MTOW)
					throw(new Exception(EWBSCoreWarnings.FlightZFWExceedsMax));
				else
					throw(new Exception(EWBSCoreWarnings.FlightZFWExceedsRegulated));
			}
			//3.check if LDW must be less or equal then MLDW and Regulated MLDW
			if (this.LDW > this.ActualMLDW)
			{
				if (this.LDW > this.MLDW)
					throw(new Exception(EWBSCoreWarnings.FlightLDWExceedsMax));
				else
					throw(new Exception(EWBSCoreWarnings.FlightLDWExceedsRegulated));
			}
			//4. check if LIZFW is within the FWD and AFT Limit
            if (this.LIZFW > this.ZFWTrim.Aft || this.LIZFW < this.ZFWTrim.Fwd) 
			{
				throw(new Exception(EWBSCoreWarnings.FlightZFIndexNotWithinLmt));
			}
			//5. check if LITOW is within the FWD and AFT Limit
            if (this.LITOW > this.TOWTrim.Aft || this.LITOW < this.TOWTrim.Fwd)  
			{
				throw(new Exception(EWBSCoreWarnings.FlightTakeoffIndexNotWithinLmt));
			}
			//6. check if MACDLW must be less or equal then max MACDLW
			if (this.MACDLW > this.ACType.CargoInfomation.info[0].maxDLW && this.ACType.IsDLW)
			{
				throw(new Exception(string.Format(EWBSCoreWarnings.FlightMACDLWNotWithinLmt_1, this.ACType.CargoInfomation.info[0].maxDLW)));
			}
			//7.check if booked pax number matched accepted Pax number
			for (int i = 0; i < this.pax.BookedClsDstnClassList.Length; i++)
			{
				for (int cls = 0; cls < 3; cls++)
				{
					if (this.pax.BookedClsDstnClassList[i][cls] != this.pax.ActlClsDstnClassList[i][cls])
					{
						throw(new Exception(EWBSCoreWarnings.BookedPaxNotMatchAcceptedPax));
					}
				}
			}
			//8.Check CPI limitation
			CheckCPI();
			//9.Check fuel limitation
			fuelList.Check();
			//10.Check Crew limitation
			crew.Check();
			//11.Check Pax limitation
			pax.Check();
			//12.Check Position limitation
			if (!cgoPosnMgr.Check())
				throw(new Exception(""));
			//13.Check Cargo limitation except empty ULD Serial No.
			CheckCargo(false); //not final check

			//14.Check no Fit
			ArrayList nofitList = cgoPosnMgr.CheckNoFit();
			if (nofitList != null)
			{
				string nofitString = (nofitList[0] as CargoPosnBase).Name;
				for (int i = 1; i < nofitList.Count; i++)
				{
					nofitString += "," + (nofitList[i] as CargoPosnBase).Name;
				}
				throw(new Exception(string.Format(EWBSCoreWarnings.FlightShouldNotBeNoFitPosn_1, nofitString)));
			}

			//15.check underload
			if (this.UnderLoad < 0)
			{
				throw(new Exception(EWBSCoreWarnings.EstimatedAllowedCgoLoadExceeded));
			}

			//15.check TakeoffFuel & TripFuel
			if (this.Fuel.TakeoffFuel <= 0f || this.Fuel.TripFuel <= 0f)
			{
				throw(new Exception(EWBSCoreWarnings.FuelUnUnreasonableTakeoffFuelorTripFuel));
			}
		}


		/// <summary>
		/// Check CPI
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		private void CheckCPI()
		{
			//find CPI limit from FDB
			AirTypeEx theAirType = this.ACType;
			if (theAirType != null)
			{
				AirtypeCpi[] cpiList = theAirType.cpi;
				if (cpiList != null)
				{
					for (uint i = 0; i < cpiList.Length; i++)
					{
						//CPI must be within the limit
						double result = this.CPI;
						if (result > cpiList[i].index)
							throw(new Exception(string.Format(EWBSCoreWarnings.CheckedPointIndex_2, result, cpiList[i].index)));
					}
				}
			}
		}

		#endregion

		/// <summary>Check if duplicated ULD Serial NO</summary>
		/// <param name="candidate">candidate cargo</param>
		/// <returns>Duplicated or not</returns>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public bool IsDuplicatedCargo(Cargo candidate)
		{
			string uldSerialNo = candidate.ULDSerialNo;
			//convert serialNo to integer
			int serialNo = 0;
			try
			{
				serialNo = Convert.ToInt32(candidate.SerialNo);
			}
			catch
			{
			}

			//find duplicated one from deadloadList and baggageList
			foreach (Cargo cargo in deadloadList + baggageList)
			{
				if (cargo.SerialNo == "") continue;
				if (cargo.ULDSerialNo.Equals(uldSerialNo) ||
					(serialNo == Convert.ToInt32(cargo.SerialNo) &&
						candidate.UldType.Equals(cargo.UldType) &&
						candidate.CarrierCode.Equals(cargo.CarrierCode)))
				{
					if (cargo != candidate) return true;
				}
			}


			//find duplicated one from ECP
			string ecp = this.telex.UCM.ECP.Replace("\r\n", "");
			string[] ecpInfo = ecp.Trim().Split(new char[] {'.', '/'});
			for (int i = 0; i < ecpInfo.Length; i += 3)
			{
				while (i < ecpInfo.Length && ecpInfo[i].Trim() == "") i += 1;

				if (i + 2 < ecpInfo.Length)
				{
					Cargo cargo = new Cargo(this, ecpInfo[i], ecpInfo[i + 2], ecpInfo[i + 1]);

					if (candidate.ULDSerialNo.Equals(ecpInfo[i]) ||
						(serialNo == Convert.ToInt32(cargo.SerialNo) &&
							candidate.UldType.Equals(cargo.UldType) &&
							candidate.CarrierCode.Equals(cargo.CarrierCode)))
					{
						return true;
					}
				}
			}


			return false;
		}

		/// <summary>CheckCargo</summary>
		/// <param name="final">skip empty ULD Serial NO or not</param>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public void CheckCargo(bool final)
		{
			CargoList aList = deadloadList + baggageList;
			for (int i = 0; i < aList.Count; i++)
			{
				Cargo candidate = aList[i] as Cargo;
				string uldSerialNo = candidate.ULDSerialNo;

				//check if cargo is onboard
				if (candidate.Posn == "")
					throw(new Exception(string.Format(EWBSCoreWarnings.FlightULDSnNotOnBoard_1, uldSerialNo)));
//				//#BR070007 <--
//				if((candidate.IsContainer == false && candidate.UserUldHtCode =="" && candidate.NetWt ==0 ))
//				{
//				}
//				else
//				{
//					throw(new Exception(string.Format(EWBSCoreWarnings.FlightULDSnNotOnBoard_1, uldSerialNo)));
//				}
				//#BR070007 -->

				//check if ULD Serial NO is absent
				if (candidate.SerialNo == "")
				{
					if (!final)
						continue;
					else
						throw(new Exception(EWBSCoreWarnings.FlightNoSnForContainer));
				}

				//check if ULD Serial No is duplicated
				for (int j = i + 1; j < aList.Count; j++)
				{
					if ((aList[j] as Cargo).ULDSerialNo.Equals(uldSerialNo))
					{
						throw(new Exception(string.Format(EWBSCoreWarnings.FlightCargoDuplicated_1, uldSerialNo)));
					}
				}
			}
			//check if consignment is onboard
			foreach (ICargo cargo in this.consignmentList)
			{
				if (cargo.Weight > 0f && cargo.Posn == "")
					throw(new Exception(EWBSCoreWarnings.FlightConsignmentNotOnboard));
			}
			foreach (ICargo cargo in this.bagCsgmtList)
			{
				if (cargo.Weight > 0f && cargo.Posn == "")
					throw(new Exception(EWBSCoreWarnings.FlightConsignmentNotOnboard));
			}
		}

		//BR08104 [NonSerialized] private string lastLongHaul = "S"; //last Haul
		[NonSerialized] private string lastLongHaul = "L"; //last Haul  //BR08104
		[NonSerialized] private float lastZFW; //last ZFW
		[NonSerialized] private float lastTOW; //last TOW
		[NonSerialized] private float finalMZFW; //final MZFW
		[NonSerialized] private float finalMTOW; //final MTOW 

		/// <summary>
		/// find Wt Limit
		/// </summary>
		/// <param name="_nmMaxWtLmt">Wt limit name</param>
		/// <param name="_longHaul">long haul or not</param>
		/// <returns>Max Wt limit</returns>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		private float findMaxWt(string _nmMaxWtLmt, string _longHaul)
		{
			AircraftInfomationHaulWtLmt[] foundWtLmt = null;

			if (theAirType.AircraftInfomation != null)
			{
				//find Max Wt limit according Long Haul or short
				foreach (AircraftInfomationHaul haulLmt in theAirType.AircraftInfomation.Haul)
				{
					string haulName = haulLmt.haul.ToUpper();
					if (_longHaul == haulName)
						foundWtLmt = haulLmt.wtLmt;
				}

				//default Max Wt limit
				if (foundWtLmt == null && theAirType.AircraftInfomation.Haul.Length > 0)
					foundWtLmt = theAirType.AircraftInfomation.Haul[0].wtLmt;
			}

			if (foundWtLmt != null && foundWtLmt.Length > 0)
			{
				float min = float.MaxValue;
				float canddateMZFW = foundWtLmt[0].mzfw; //default MZFW
				float canddateMTOW = foundWtLmt[0].mtow; //default MTOW
				float canddateMLDW = foundWtLmt[0].mldw; //default MLDW
				float canddateMTW = foundWtLmt[0].mtw; //default MTW

				float actualZFW = this.ZFW;
				float actualTOW = this.TOW;


				switch (_nmMaxWtLmt)
				{
					case "MZFW":
					case "MTOW":
						if (lastZFW != actualZFW || lastTOW != actualTOW || lastLongHaul != _longHaul)
						{
							lastZFW = actualZFW;
							lastTOW = actualTOW;
							lastLongHaul = _longHaul;


							//find Max Wt limit using 內差法
							for (int i = 0; i < foundWtLmt.Length - 1; i++)
							{
								float tmp = min;
								int mzfw;
								if (actualZFW >= foundWtLmt[i].mzfw)
								{
									//find wt limit quickly
									int inc = 50;
									for (mzfw = foundWtLmt[i].mzfw; mzfw < foundWtLmt[i + 1].mzfw + inc; mzfw += inc)
									{
										float ratio = (float) (mzfw - foundWtLmt[i].mzfw)/(foundWtLmt[i + 1].mzfw - foundWtLmt[i].mzfw);
										float mtow = foundWtLmt[i].mtow + ((foundWtLmt[i + 1].mtow - foundWtLmt[i].mtow))*ratio;

										float diff = Math.Abs(mtow - actualTOW - mzfw + actualZFW);

										if (diff < tmp)
										{
											tmp = diff;
										}
										else break;
									}

									//find wt limit accurately
									int minZFW = Math.Max(foundWtLmt[i].mzfw, mzfw - 2*inc);
									int maxZFW = Math.Min(mzfw, foundWtLmt[i + 1].mzfw);
									for (mzfw = minZFW; mzfw <= maxZFW; mzfw++)
									{
										float ratio = (float) (mzfw - foundWtLmt[i].mzfw)/(foundWtLmt[i + 1].mzfw - foundWtLmt[i].mzfw);
										float mtow = foundWtLmt[i].mtow + ((foundWtLmt[i + 1].mtow - foundWtLmt[i].mtow))*ratio;

										float diff = Math.Abs(mtow - this.TOW - mzfw + this.ZFW);

										if (diff < min)
										{
											min = diff;
											canddateMZFW = mzfw;
											canddateMTOW = mtow;
										}
										else break;
									}
								}
							}

							finalMZFW = canddateMZFW;
							finalMTOW = canddateMTOW;
						}
						break;
					case "MLDW":
						return canddateMLDW;
					case "MTW":
						return canddateMTW;
				}


				switch (_nmMaxWtLmt)
				{
					case "MZFW":
						return foundWtLmt.Length > 1 ? finalMZFW : canddateMZFW;
					case "MTOW":
						return foundWtLmt.Length > 1 ? finalMTOW : canddateMTOW;
					case "MLDW":
						return canddateMLDW;
					case "MTW":
						return canddateMTW;
				}
			}


			return float.NegativeInfinity;
		}

		/// <summary>
		/// Merge similar consignment within list
		/// </summary>
		/// <param name="cgoCsgmtList">cargo or consignment list</param>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public void MergeCsgmt(ArrayList cgoCsgmtList)
		{
			for (int i = cgoCsgmtList.Count - 1; i >= 0; i--)
			{
				bool deepen = false;

				if (cgoCsgmtList[i] is Consignment)
				{
					for (int j = i - 1; j >= 0; j--)
					{
						if (cgoCsgmtList[j] is Consignment)
						{
							//find two consignment to merge
							Consignment aConsignment = cgoCsgmtList[i] as Consignment;
							Consignment csgmt = cgoCsgmtList[j] as Consignment;

							//both consignments are special load and same SHC
							if ((aConsignment is SpecialLoad) && (csgmt is SpecialLoad) && aConsignment.SHC.Equals(csgmt.SHC) ||
								//both consignments are not special load
								!(aConsignment is SpecialLoad) && !(csgmt is SpecialLoad))
							{
								if (
									aConsignment.AWB == csgmt.AWB && //same AWB No
										aConsignment._Category == csgmt._Category && //same category
										aConsignment._Dest == csgmt._Dest && //same destination
									aConsignment.Priority == csgmt.Priority && //same priority
									aConsignment.bFlagTransit == csgmt.bFlagTransit) //same transit status
								{
									//merge two csgmts
									csgmt.Weight += aConsignment.Weight;
									csgmt.Pieces += aConsignment.Pieces;
									csgmt.bFlagUserModified |= aConsignment.bFlagUserModified;
									foreach (ICargo specialLoad in aConsignment.Consignments)
									{
										if (specialLoad.Weight > 0f)
										{
											csgmt.Consignments.Add(specialLoad);
											deepen = true;
										}
									}

									for (int k = aConsignment.Consignments.Count - 1; k >= 0; k--)
										aConsignment.Consignments.Remove(k);

									cgoCsgmtList.RemoveAt(i);
									if (this.consignmentList.IndexOf(aConsignment) >= 0)
										this.consignmentList.Remove(aConsignment);
									else if (this.bagCsgmtList.IndexOf(aConsignment) >= 0)
										this.bagCsgmtList.Remove(aConsignment);

									if (deepen)
									{
										//merge sub cgoCsgmtList
										MergeCsgmt(cgoCsgmtList);
									}
									break;
								}
							}

						}
					} //for

				}
					//merge baggage
				else if (cgoCsgmtList[i] is Baggage)
				{
					Baggage aBaggage = cgoCsgmtList[i] as Baggage;
					MergeCsgmt(aBaggage.Consignments);
					if (aBaggage.GWT > aBaggage.MaxBagWt)
						foreach (Consignment csgmt in aBaggage.Consignments)
						{
							if (!csgmt.bFlagTransit)
								csgmt.bFlagUserModified = true;
						}
				}
				else if (cgoCsgmtList[i] is Cargo)
				{
					Cargo cargo = cgoCsgmtList[i] as Cargo;

					//merge csgmt into cargo
					for (int k = cargo.Consignments.Count - 1; k >= 0; k--)
					{
						if (cargo.Consignments[k] is Consignment)
						{
							Consignment aConsignment = cargo.Consignments[k] as Consignment;
							if (!(aConsignment is SpecialLoad) && //not a special load
								aConsignment.AWB == "" && //AWB must be empty
								aConsignment._Category==cargo._Category && //same category
								aConsignment._Dest==cargo._Dest && //same destination
								aConsignment.Pieces == 0 && //piece must be zero
								aConsignment.bFlagTransit == cargo.bFlagTransit) //same transit status
							{
								cargo.Weight += aConsignment.Weight;
								foreach (Consignment specialLoad in aConsignment.Consignments)
								{
									cargo.Consignments.Add(specialLoad);
								}
								cargo.Consignments.RemoveAt(k);
							}
						}
					} //for
					//merge two csgmts
					MergeCsgmt(cargo.Consignments);
				}
			} //for
		}

		/// <summary>
		/// init flight	
		/// </summary>
		/// <remarks>
		/// Modified Date:
		/// Modified By:
		/// Modified Reason:
		/// </remarks>
		public void Init()
		{
			AircraftConfigurationConfig found = null;

			//get Aircraft Configuration from FDB
			if (theAirType != null && route.Length >= 2 && theAirType.AircraftConfiguration != null)
			{
				//find default parameter according from-to
				foreach (AircraftConfigurationConfig param in theAirType.AircraftConfiguration.config)
				{
					if (route[0].Equals(param.from) && route[1].Equals(param.to))
					{
						found = param;
						break;
					}
				} //foreach

				//if not found, find it in To-From pair
				if (found == null && theAirType.AircraftConfiguration != null)
				{
					foreach (AircraftConfigurationConfig param in theAirType.AircraftConfiguration.config)
					{
						if (route[0].Equals(param.to) && route[1].Equals(param.from))
						{
							found = param;
							break;
						}
					} //foreach
				}

			} //if

			//Ok, found.
			if (found != null)
			{
				//Crew
				crew.NumCockpit = found.pilot;
				crew.NumCrew = found.crew;
				crew.CrewDistribution(found.crew);
				//Pantry
				pantry.Code = found.pantry;
				//Seatplan
				cabinConfiguration = found.seatplan;
				//haul
				longHaul=found.haul;
			}
			else
			{
				//still not found, get the first record in FDB as default parameter
				CabinConfigurationSeatplan seatplan = theAirType.GetSeatplan("");
				if (seatplan != null)
				{
					cabinConfiguration = seatplan.name;
				}	
				//BR08108<--
				//若未設定航線需加判斷機型+起飛點，落在app config中的就給S，其他都為L
				bool findhaulchk =false;
                for (int i = 0; i < ConfigurationManager.AppSettings.AllKeys.Length; i++)
                {
                    if (ConfigurationManager.AppSettings.AllKeys[i].ToString() == "haul chk "+theAirType.name)
					{
						findhaulchk = true;
						break;
					}
					//BR10107<--
					//若判斷機型+抵達點，落在app config arr haul chk中的就給S，其他都為L
                    else if (ConfigurationManager.AppSettings.AllKeys[i].ToString() == "arr haul chk " + theAirType.name)
					{
						findhaulchk = true;
						break;
					}
					//BR10107-->
				}
				//如果有找到app key (這個機型有特殊設定時)
				if(findhaulchk)
				{
					//如果此符合此機型，又符合起飛站時，即給S
                    if (ConfigurationManager.AppSettings["haul chk " + theAirType.name].IndexOf(route[0].ToString()) >= 0)
					{
						this.longHaul = "S";
					}
					//BR10107<--
					//如果此符合此機型，又符合抵達站時，即給S
                    else if (ConfigurationManager.AppSettings["arr haul chk " + theAirType.name].IndexOf(route[0].ToString()) >= 0)
					{
						this.longHaul = "S";
					}
					//BR10107-->
					else
					{
						this.longHaul = "L";
					}
				}
				else
				{
					//若沒找到，就都給L
					this.longHaul = "L";

				}
				//BR08108-->
			}

			//get default fuel density
			this.Fuel.FuelDensity = this.GetFuelDensity();
		}

		#region ISerializable Members

		/// <summary>
		/// Get Object Data
		/// </summary>
		/// <param name="info">SerializationInfo</param>
		/// <param name="context">StreamingContext</param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// TODO:  Add Flight.GetObjectData implementation
			info.AddValue("status",status);
			info.AddValue("flt_no",flt_no);
			info.AddValue("route",route);
			info.AddValue("std",std);
			info.AddValue("etd",etd);
			info.AddValue("haul",longHaul);
			info.AddValue("regulatedZFW",regulatedZFW);
			info.AddValue("regulatedTOW",regulatedTOW);
			info.AddValue("regulatedLDW",regulatedLDW);
			info.AddValue("plannedZFW",plannedZFW);
			info.AddValue("plannedTOW",plannedTOW);
			info.AddValue("carrierCode",carrierCode);
			info.AddValue("cabinConfiguration",cabinConfiguration);
			info.AddValue("loadsheetVersion",loadsheetVersion);
			info.AddValue("lifEdition",lifEdition);
			info.AddValue("usingIncreasedCumulativeLoad",usingIncreasedCumulativeLoad);
			info.AddValue("blkdSeat",blkdSeat);
			info.AddValue("brdgGate",brdgGate);
			info.AddValue("loadVersion",loadVersion);
			info.AddValue("unloadVersion",unloadVersion);
			info.AddValue("FlightFactVersion",FlightFactVersion);
			info.AddValue("aircraft",aircraft);
			info.AddValue("isKILOS",isKILOS);
			info.AddValue("fuelList",fuelList);
			info.AddValue("crew",crew);
			info.AddValue("pantry",pantry);
			info.AddValue("serviceItems",serviceItems);
			info.AddValue("pax",pax);
			info.AddValue("deadloadList",deadloadList);
			info.AddValue("consignmentList",consignmentList);
			info.AddValue("baggageList",baggageList);
			info.AddValue("bagCsgmtList",bagCsgmtList);
			info.AddValue("unloadCargoList",unloadCargoList);
			info.AddValue("telex",telex);
			info.AddValue("user",user);
			info.AddValue("flagTrimByCmpt",flagTrimByCmpt);
			//#BR071003 <-- for Serialization
			info.AddValue("SpecialLoadExtList",SpecialLoadExtList); //#BR071003 To serialize SpecialLoadExtList object data
			//#BR071003 -->
		}

		#endregion

	} //Flight

}