/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//*Cherry Chan| Ver. 01 | #BR09128 | 2009/DEC/18   (V01.11)                   *
//*---------------------------------------------------------------------------*
//* 新增baggage的月報表的平均飛機行李櫃數及新增班機號碼                       *
//*****************************************************************************
using System;
using System.Collections;

namespace EWBS
{
    /// <summary>
    /// the Class of a list of cargo
    /// </summary>
    [Serializable]
    public class CargoList : ArrayList
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CargoList()
        { }

        /// <summary>
        /// get all the special loads and return them in a list
        /// </summary>
        /// <returns>an array of all the special loads</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ICargo[] GetAllSpecialLoads()
        {
            ArrayList AllSpecialLoadsArrayList = new ArrayList();
            foreach (ICargo CB in this)
            {
                //if it is a special load, add the CB into the special load list
                if (CB is SpecialLoad)
                {
                    AllSpecialLoadsArrayList.Add(CB);
                }
                //Add the consignments of the cargo into the special load list
                foreach (ICargo subCB in CB.Consignments)
                {
                    if (subCB is SpecialLoad)
                    {
                        AllSpecialLoadsArrayList.Add(subCB);
                    }
                }
            }
            return (ICargo[])AllSpecialLoadsArrayList.ToArray(typeof(ICargo));
        }

        /// <summary>
        /// get all the uld serial numbers of the Cargos in the CargoList and return them in an array
        /// </summary>
        /// <returns>string array of the uld serial numbers</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] GetAllULDSerialNbr()
        {
            ArrayList uldSerialNbrList = new ArrayList();
            foreach (ICargo cgo in this)
            {
                if (cgo is Cargo)
                {
                    Cargo cargo = cgo as Cargo;
                    if (cargo.UldType != "" && cargo.SerialNo != "" && cargo.CarrierCode != "")
                        uldSerialNbrList.Add(cargo.ULDSerialNo);
                }
            }
            uldSerialNbrList.Sort();
            return (string[])uldSerialNbrList.ToArray(typeof(string));
        }

        /// <summary>
        /// find a cargo with the uld serial number
        /// </summary>
        /// <param name="uldSerialNbr"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Cargo FindCargo(string uldSerialNbr)
        {
            foreach (ICargo cgo in this)
            {
                //Return the Cargo object, if ULDSerialNo and string are identical.
                if (cgo is Cargo)
                {
                    Cargo cargo = cgo as Cargo;
                    if (uldSerialNbr.Equals(cargo.ULDSerialNo) && cargo.SerialNo != "")
                        return cargo;
                }
            }
            return null;
        }

        /// <summary>
        /// add an ICargo object into this Cargo list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public override int Add(object value)
        {
            // TODO:  Add SeatList.Add implementation
            if (value is ICargo)
            {
                return base.Add(value);
            }
            else
            {
                return -1;
            }

        }

        /// <summary>
        /// make the two CargoList plus together
        /// </summary>
        /// <param name="o1">CargoList 1</param>
        /// <param name="o2">CargoList 2</param>
        /// <returns>the result cargo list</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static CargoList operator +(CargoList o1, CargoList o2)
        {
            CargoList result = new CargoList();
            foreach (ICargo item in o1)
            {
                result.Add(item);
            }

            foreach (ICargo item in o2)
            {
                result.Add(item);
            }
            return result;
        }
        //BR09128<--
        //		/// <summary>
        //		/// calculate the counts of cargos in the cargo list
        //		/// </summary>
        //		/// <param name="dest"></param>
        //		/// <returns>the counts of cargos</returns>
        //		/// <remarks>
        //		/// Modified Date:
        //		/// Modified By:
        //		/// Modified Reason:
        //		/// </remarks>
        //		public int CountCntr(string dest)
        //		{
        //			int count = 0;
        //			foreach (ICargo cargo in this)
        //			{
        //				if (cargo is Cargo)
        //				{
        //					if (cargo.Dest.IndexOf(dest) >= 0) count += 1;
        //				}
        //			}
        //			return count;
        //		}
        //BR09128-->
        //BR09128<--
        /// <summary>
        /// calculate the counts of cargos in the cargo list
        /// </summary>
        /// <param name="dest"></param>
        /// <returns>the counts of cargos</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int CountCntr(string dest)
        {
            int count = 0;
            string sULD = "";
            foreach (ICargo cargo in this)
            {
                if (cargo is Cargo)
                {
                    if (cargo.Dest.IndexOf(dest) >= 0)
                    {

                        sULD = cargo.UserULDSerialNbr.Substring(0, 3);
                        //ALF/FLA/PLA +2
                        //PAG/PMC/AAF/AAU +3
                        if (sULD == "ALF" || sULD == "FLA" || sULD == "PLA")
                            count += 2;
                        else if (sULD == "PAG" || sULD == "PMC" || sULD == "AAF" || sULD == "AAU")
                            count += 3;
                        else
                            count += 1;
                    }

                }
            }
            return count;
        }
        //BR09128-->
    }


    /// <summary>
    /// a list of Consignments
    /// </summary>
    [Serializable]
    public class ConsignmentList : ArrayList
    {
        [NonSerialized]
        private bool usingContainer = false; //indicates if it uses container
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ConsignmentList()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theContainer">the container of it</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ConsignmentList(ICargo theContainer)
        {
            this.theContainer = theContainer;
            usingContainer = true;
        }


        private ICargo theContainer = null; //the container this Consignment belongs to 

        /// <summary>
        /// returns theContainer
        /// </summary>
        public ICargo BelongToContainer
        {
            get { return theContainer; }
            //set { theContainer = value; }
        }

        /// <summary>
        /// get all the special loads in this Consignment List
        /// </summary>
        /// <returns>ICargo[], all the special loads</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ICargo[] GetAllSpecialLoads()
        {
            ArrayList AllSpecialLoadsArrayList = new ArrayList();
            //Traverse all the ICargo Objects in this
            foreach (ICargo CB in this)
            {
                //if it is a special load, add the CB into the special load list
                if (CB is SpecialLoad)
                {
                    AllSpecialLoadsArrayList.Add(CB);
                    continue;
                }
                //Add the consignments of the cargo into the special load list
                foreach (ICargo subCB in CB.Consignments)
                {
                    if (subCB is SpecialLoad)
                    {
                        AllSpecialLoadsArrayList.Add(subCB);
                    }
                }
            }
            return (ICargo[])AllSpecialLoadsArrayList.ToArray(typeof(ICargo));
        }

        /// <summary>
        /// Add a Consignment into this Consignment List
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public override int Add(object value)
        {
            //Donot put a Consignment into another one.
            if ((value is Consignment) &&
                (theContainer == null || (value is SpecialLoad) || (theContainer is Cargo)))
            {
                int result = IndexOf(value);
                if (result >= 0)
                    return result;
                Consignment aConsignment = value as Consignment;

                //if this CargoList uses container, set the consignment's container to theContainer
                if (usingContainer || theContainer != null)
                {
                    aConsignment.SetContainer(theContainer);
                }

                //if theContainer is a baggage, set its parent uld as the baggage.
                if (theContainer is Baggage)
                {
                    Baggage parentULD = theContainer as Baggage;


                    //add the object into the list
                    result = base.Add(value);

                    if (parentULD.IsMixDestCategory && aConsignment.Weight > 0f)
                    {
                        aConsignment.bFlagUserModified = true;
                    }
                }
                else
                {
                    //add the object into the list
                    result = base.Add(value);
                }
                return result;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// find and then remove a Consignment
        /// </summary>
        /// <param name="value">the object to be added </param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public override void Remove(object value)
        {
            //if the object exists in this list
            if (this.IndexOf(value) >= 0)
            {
                //if it is a consignment
                if (value is Consignment)
                {
                    Consignment csgmt = value as Consignment;

                    //remove the consignment form its parent uld.
                    if (csgmt.ParentULD == theContainer)
                    {
                        csgmt.SetContainer(null);
                    }

                    //remove the consignment
                    base.Remove(csgmt);

                    int count = 0;
                    foreach (Consignment aConsignment in this)
                    {
                        if (!(aConsignment is SpecialLoad))
                            count += 1;
                    }

                    //if the consignment belongs to a baggage, add new consignment onto the consignment list
                    if (theContainer != null && (theContainer is Baggage) && count <= 0)
                    {
                        this.Add(new Consignment(csgmt.Flight, csgmt.Category, csgmt.Dest));
                    }
                }

            }
        }

        /// <summary>
        /// get all the AWB numbers of the consignments in this list
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string[] GetAllAWBNbr()
        {
            ArrayList uldSerialNbrList = new ArrayList();
            //if the AWB is nut empty, add the AWB in to the string arraylist
            foreach (Consignment csgmt in this)
                if (csgmt.AWB != "") uldSerialNbrList.Add(csgmt.AWB);
            // sort the resulted list
            uldSerialNbrList.Sort();
            //trun the arraylist into array and return it
            return (string[])uldSerialNbrList.ToArray(typeof(string));
        }

        /// <summary>
        /// find a consignment in this list with an AWB number
        /// </summary>
        /// <param name="AWB"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Consignment FindConsignment(string AWB)
        {
            foreach (Consignment csgmt in this)
            {
                if (AWB.Equals(csgmt.AWB) && csgmt.AWB != "")
                    return csgmt;
            }
            return null;
        }


    }

}