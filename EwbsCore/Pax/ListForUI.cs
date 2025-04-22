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
using System;
using System.Collections;

namespace EWBS
{
    /// <summary>
    ///  Pieces and Weight classes which are related to Baggage
    /// </summary>
    [Serializable]
    public class BagType
    {
        private int bagPcs;
        private int bagWt;

        #region constructor
        public BagType(BagType other)
        {
            bagPcs = other.bagPcs;
            bagWt = other.bagWt;
        }

        public BagType()
        {
            bagPcs = 0;
            bagWt = 0;
        }

        #endregion

        /// <summary>
        /// Bag Pieces
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int BagPcs
        {
            get { return bagPcs; }
            set { bagPcs = value; }
        }

        /// <summary>
        /// Bag Wt
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int BagWt
        {
            get { return bagWt; }
            set { bagWt = value; }
        }

        /// <summary>
        ///  Pass out the sum of Piece and Weight of BagType object o1 and o2
        /// </summary>
        /// <param name="o1">BagType object</param>
        /// <param name="o2">BagType object</param>
        /// <returns>BagType object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static BagType operator +(BagType o1, BagType o2)
        {
            BagType result = new BagType();
            result.bagPcs = o1.bagPcs + o2.bagPcs;
            result.bagWt = o1.bagWt + o2.bagWt;
            return result;
        }

        /// <summary>
        ///  Pass out the difference of Piece and Weight of BagType object o1 and o2
        /// </summary>
        /// <param name="o1">BagType object</param>
        /// <param name="o2">BagType object</param>
        /// <returns>BagType object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static BagType operator -(BagType o1, BagType o2)
        {
            BagType result = new BagType();
            result.bagPcs = o1.bagPcs - o2.bagPcs;
            result.bagWt = o1.bagWt - o2.bagWt;
            return result;
        }
    }

    /// <summary>
    /// Catagory the BagType related class by class and then by destination 
    /// </summary>
    [Serializable]
    public class ClsDstnBagItem
    {
        private const int MaxCabinClass = 3; //first class, secondary class, last class
        private string dstn;
        private BagType[] paxBag = new BagType[MaxCabinClass];

        #region Constructor

        public ClsDstnBagItem(ClsDstnBagItem other)
        {
            this.dstn = other.dstn;

            for (int i = 0; i < MaxCabinClass; i++)
                paxBag[i] = new BagType(other.paxBag[i]);
        }

        public ClsDstnBagItem(string dstn)
        {
            this.dstn = dstn;

            for (int i = 0; i < MaxCabinClass; i++) paxBag[i] = new BagType();
        }

        #endregion

        /// <summary>
        /// Length
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Length
        {
            get { return paxBag.Length; }
        }

        /// <summary>
        /// Get object by index
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagType this[int index]
        {
            get { return paxBag[index]; }
            set { paxBag[index] = value; }
        }

        /// <summary>
        /// Get Destination
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string Dstn
        {
            get { return dstn; }
            set { dstn = value; }
        }

        /// <summary>
        /// Get Pax baggage
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagType FrstPaxBag
        {
            get { return paxBag[0]; }
            set { paxBag[0] = value; }
        }

        /// <summary>
        /// Get Pax baggage
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagType SryPaxBag
        {
            get { return paxBag[1]; }
            set { paxBag[1] = value; }
        }

        /// <summary>
        /// Get Pax baggage
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagType LstPaxBag
        {
            get { return paxBag[2]; }
            set { paxBag[2] = value; }
        }

        /// <summary>
        ///  Pass out all of the sum of same_classed paxBag objects of ClsDstnBagItem object o1 and o2
        /// </summary>
        /// <param name="o1">ClsDstnBagItem object</param>
        /// <param name="o2">ClsDstnBagItem object</param>
        /// <returns>ClsDstnBagItem object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static ClsDstnBagItem operator +(ClsDstnBagItem o1, ClsDstnBagItem o2)
        {
            ClsDstnBagItem result = new ClsDstnBagItem(o1.dstn);
            for (int i = 0; i < MaxCabinClass; i++)
            {
                result.paxBag[i] = o1.paxBag[i] + o2.paxBag[i];
            }
            return result;
        }

    }


    /// <summary>
    /// Catagory the PAX number related class by class and then by destination 
    /// </summary>
    [Serializable]
    public class ClsDstnClassItem
    {
        private string dstn = "";
        private const int MaxCabinClass = 3; //first class, secondary class, last class
        private int[] paxNbr = new int[MaxCabinClass];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnClassItem(ClsDstnClassItem other)
        {
            dstn = other.dstn;
            for (int i = 0; i < MaxCabinClass; i++)
            {
                paxNbr[i] = other.paxNbr[i];
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dest"></param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnClassItem(string dest)
        {
            dstn = dest;
        }

        /// <summary>
        /// Length
        /// </summary>
        public int Length
        {
            get { return MaxCabinClass; }
        }

        /// <summary>
        /// Get object by index
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int this[int index]
        {
            get { return paxNbr[index]; }
            set { paxNbr[index] = value; }
        }

        /// <summary>
        /// Get/Set destination
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string Dstn
        {
            get { return dstn; }
            set { dstn = value; }
        }


        /// <summary>
        /// Get/Set pax number
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int FrstPaxNumber
        {
            get { return paxNbr[0]; }
            set { paxNbr[0] = value; }
        }

        /// <summary>
        /// Get/Set pax number
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int SryPaxNumber
        {
            get { return paxNbr[1]; }
            set { paxNbr[1] = value; }
        }

        /// <summary>
        /// Get/Set pax number
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int LstPaxNumber
        {
            get { return paxNbr[2]; }
            set { paxNbr[2] = value; }
        }

        /// <summary>
        ///  Pass out ClsDstnClassItem object which is the sum of PAX number of ClsDstnClassItem object o1 and o2
        /// </summary>
        /// <param name="o1">ClsDstnClassItem object</param>
        /// <param name="o2">ClsDstnClassItem object</param>
        /// <returns>ClsDstnClassItem object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static ClsDstnClassItem operator +(ClsDstnClassItem o1, ClsDstnClassItem o2)
        {
            ClsDstnClassItem result = new ClsDstnClassItem("");
            for (int i = 0; i < MaxCabinClass; i++)
            {
                result.paxNbr[i] = o1.paxNbr[i] + o2.paxNbr[i];
            }
            return result;
        }

        /// <summary>
        ///  Pass out ClsDstnClassItem object which is the difference of PAX number of ClsDstnClassItem object o1 and o2
        /// </summary>
        /// <param name="o1">ClsDstnClassItem object</param>
        /// <param name="o2">ClsDstnClassItem object</param>
        /// <returns>ClsDstnClassItem object</returns>
        public static ClsDstnClassItem operator -(ClsDstnClassItem o1, ClsDstnClassItem o2)
        {
            ClsDstnClassItem result = new ClsDstnClassItem("");
            for (int i = 0; i < MaxCabinClass; i++)
            {
                result.paxNbr[i] = o1.paxNbr[i] - o2.paxNbr[i];
            }
            return result;
        }
    }


    /// <summary>
    /// Catagory the PAX number related class by sex and then by destination 
    /// </summary>
    [Serializable]
    public class GenderDstnClassItem
    {
        private string dstn = "";
        private const int MaxPaxGender = 4; //male , female , child , infant ;
        private int[] gender = new int[MaxPaxGender];

        /// <summary>
        /// Constructor
        /// </summary>
        public GenderDstnClassItem(GenderDstnClassItem other)
        {
            this.dstn = other.dstn;
            for (int i = 0; i < MaxPaxGender; i++)
                gender[i] = other.gender[i];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dstn"></param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public GenderDstnClassItem(string dstn)
        {
            this.dstn = dstn;
        }

        #region Get/Set properties
        public string Dstn
        {
            get { return dstn; }
            set { dstn = value; }
        }

        public int Male
        {
            get { return gender[0]; }
            set { gender[0] = value; }
        }

        public int Female
        {
            get { return gender[1]; }
            set { gender[1] = value; }
        }

        public int Child
        {
            get { return gender[2]; }
            set { gender[2] = value; }
        }

        public int Infant
        {
            get { return gender[3]; }
            set { gender[3] = value; }
        }

        #endregion

        /// <summary>
        /// Compute the sum of MFCI between GenderDstnClassItem object o1, o2 
        /// </summary>
        /// <param name="o1">GenderDstnClassItem object</param>
        /// <param name="o2">GebderDstnClassItem object</param>
        /// <returns>GenderDstnClassItem object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static GenderDstnClassItem operator +(GenderDstnClassItem o1, GenderDstnClassItem o2)
        {
            GenderDstnClassItem result = new GenderDstnClassItem(o1.dstn);

            for (int i = 0; i < MaxPaxGender; i++)
                result.gender[i] = o1.gender[i] + o2.gender[i];
            return result;
        }

        /// <summary>
        /// Compute the difference of MFCI between GenderDstnClassItem object o1, o2 
        /// </summary>
        /// <param name="o1">GenderDstnClassItem object</param>
        /// <param name="o2">GebderDstnClassItem object</param>
        /// <returns>GenderDstnClassItem object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static GenderDstnClassItem operator -(GenderDstnClassItem o1, GenderDstnClassItem o2)
        {
            GenderDstnClassItem result = new GenderDstnClassItem(o1.dstn);

            for (int i = 0; i < MaxPaxGender; i++)
            {
                result.gender[i] = o1.gender[i] - o2.gender[i];
            }
            return result;
        }
    }

    /// <summary>
    /// Catagory the PAX number related class by Zone 
    /// </summary>
    [Serializable]
    public class ZoneItem
    {
        private string zoneName;
        private int paxNum, maxPax;

        /// <summary>
        /// Set ZoneName、paxNum、maxPax of ZoneItem class
        /// </summary>
        /// <param name="ZoneName">Zone name</param>
        /// <param name="paxNum">passenger number</param>
        /// <param name="maxPax">maximum passenger number</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ZoneItem(string ZoneName, int paxNum, int maxPax)
        {
            this.zoneName = ZoneName;
            this.paxNum = paxNum;
            this.maxPax = maxPax;
        }

        /// <summary>
        /// Get/Set zone name
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public string ZoneName
        {
            get { return zoneName; }
            set { zoneName = value; }

        }

        /// <summary>
        /// Count total of pax
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int TtlPax
        {
            get { return paxNum; }
            set { paxNum = value; }
        }

        /// <summary>
        /// Count Max of pax
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int MaxPax
        {
            get { return maxPax; }
            set { maxPax = value; }
        }

    }


    /// <summary>
    /// Connect ClsDstnClassItem list
    /// </summary>
    [Serializable]
    public class ClsDstnClassList : IEnumerable
    {
        private ArrayList clsDstnClassList;

        #region Constructor
        public ClsDstnClassList()
        {
            clsDstnClassList = new ArrayList();
        }

        #endregion

        /// <summary>
        /// Number of object
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Length
        {
            get { return clsDstnClassList.Count; }
        }

        /// <summary>
        /// get/set object by index
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnClassItem this[int index]
        {
            get { return clsDstnClassList[index] as ClsDstnClassItem; }
            set { clsDstnClassList[index] = value; }
        }

        /// <summary>
        /// Dind object by name
        /// </summary>
        /// <param name="dest">name</param>
        /// <returns>Result</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnClassItem Find(string dest)
        {
            foreach (ClsDstnClassItem item in this)
            {
                if (item.Dstn.Equals(dest))
                    return item;
            }
            return null;
        }


        /// <summary>
        /// return the sum of all elements of clsDstnClassList
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnClassItem Total
        {
            get
            {
                ClsDstnClassItem result = new ClsDstnClassItem("");
                foreach (ClsDstnClassItem tmpBookedClassItem in clsDstnClassList)
                    result += tmpBookedClassItem;
                return result;
            }
        }

        #region IEnumerable Members
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public IEnumerator GetEnumerator()
        {
            // TODO:  Add ClsDstnClassList.GetEnumerator implementation
            return clsDstnClassList.GetEnumerator();
        }

        #endregion

        #region IList Members
        /// <summary>
        /// get read-only
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsReadOnly
        {
            get
            {
                // TODO:  Add ClsDstnClassList.IsReadOnly getter implementation
                return clsDstnClassList.IsReadOnly;
            }
        }

        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="index">index</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void RemoveAt(int index)
        {
            clsDstnClassList.RemoveAt(index);
        }

        /// <summary>
        /// Insert an object
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="value">object</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Insert(int index, ClsDstnClassItem value)
        {
            clsDstnClassList.Insert(index, value);
        }

        /// <summary>
        /// Find whether object exists
        /// </summary>
        /// <param name="value">object</param>
        /// <returns>existsor not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Remove(ClsDstnClassItem value)
        {
            clsDstnClassList.Remove(value);
        }

        /// <summary>
        /// Find whether object exists
        /// </summary>
        /// <param name="value">object</param>
        /// <returns>existsor not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool Contains(ClsDstnClassItem value)
        {
            // TODO:  Add ClsDstnClassList.Contains implementation
            return clsDstnClassList.Contains(value);
        }

        /// <summary>
        /// Remove all elements
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Clear()
        {
            // TODO:  Add ClsDstnClassList.Clear implementation
            clsDstnClassList.Clear();
        }

        /// <summary>
        /// Search object and return it index in list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int IndexOf(ClsDstnClassItem value)
        {
            // TODO:  Add ClsDstnClassList.IndexOf implementation
            return clsDstnClassList.IndexOf(value);
        }

        /// <summary>
        /// Add an object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Add(ClsDstnClassItem value)
        {
            // TODO:  Add ClsDstnClassList.Add implementation
            return clsDstnClassList.Add(value);
        }

        /// <summary>
        /// Is Fixed size
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsFixedSize
        {
            get
            {
                // TODO:  Add ClsDstnClassList.IsFixedSize getter implementation
                return clsDstnClassList.IsFixedSize;
            }
        }

        #endregion

        /// <summary>
        /// If the destination of ClsDstnClassItem object between o1 List and o2 List are identical，
        /// then put the difference of FrstPaxNumber、SryPaxNumber、LstPaxNumber of ClsDstnClasstem between o1 and o2 into list，
        /// </summary>
        /// <param name="o1">ClsDstnClassList object</param>
        /// <param name="o2">ClsDstnClassList object</param>
        /// <returns>ClsDstnClassList</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static ClsDstnClassList operator -(ClsDstnClassList o1, ClsDstnClassList o2)
        {
            ClsDstnClassList result = new ClsDstnClassList();
            if (o2 != null)
            {
                ClsDstnClassItem resultItem;
                foreach (ClsDstnClassItem itemO1 in o1)
                {
                    //#BR071005 <-- Concern the new stops added by operator
                    resultItem = new ClsDstnClassItem(itemO1.Dstn);
                    resultItem.FrstPaxNumber = itemO1.FrstPaxNumber;
                    resultItem.SryPaxNumber = itemO1.SryPaxNumber;
                    resultItem.LstPaxNumber = itemO1.LstPaxNumber;
                    foreach (ClsDstnClassItem itemO2 in o2)
                    {
                        if (itemO1.Dstn == itemO2.Dstn)
                        {
                            resultItem.FrstPaxNumber -= itemO2.FrstPaxNumber;
                            resultItem.SryPaxNumber -= itemO2.SryPaxNumber;
                            resultItem.LstPaxNumber -= itemO2.LstPaxNumber;
                            break;
                        }
                    }
                    result.Add(resultItem);
                    //foreach (ClsDstnClassItem itemO2 in o2)
                    //{
                    //	if (itemO1.Dstn == itemO2.Dstn)
                    //	{
                    //		resultItem = new ClsDstnClassItem(itemO1.Dstn);
                    //		resultItem.FrstPaxNumber = itemO1.FrstPaxNumber - itemO2.FrstPaxNumber;
                    //		resultItem.SryPaxNumber = itemO1.SryPaxNumber - itemO2.SryPaxNumber;
                    //		resultItem.LstPaxNumber = itemO1.LstPaxNumber - itemO2.LstPaxNumber;
                    //		result.Add(resultItem);
                    //		break;
                    //	}
                    //}
                    //#BR071005 -->
                }
                return result;
            }
            else
                return o1;
        }

        /// <summary>
        /// If the destination of ClsDstnClassItem object between o1 List and o2 List are identical，
        /// then put the sum of FrstPaxNumber、SryPaxNumber、LstPaxNumber of ClsDstnClasstem between o1 and o2 into list，
        /// </summary>
        /// <param name="o1">ClsDstnClassList object</param>
        /// <param name="o2">ClsDstnClassList object</param>
        /// <returns>ClsDstnClassList</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static ClsDstnClassList operator +(ClsDstnClassList o1, ClsDstnClassList o2)
        {
            ClsDstnClassList result = new ClsDstnClassList();
            if (o2 != null)
            {
                ClsDstnClassItem resultItem;
                foreach (ClsDstnClassItem itemO1 in o1)
                {
                    foreach (ClsDstnClassItem itemO2 in o2)
                    {
                        if (itemO1.Dstn == itemO2.Dstn)
                        {
                            resultItem = new ClsDstnClassItem(itemO1.Dstn);
                            resultItem.FrstPaxNumber = itemO1.FrstPaxNumber + itemO2.FrstPaxNumber;
                            resultItem.SryPaxNumber = itemO1.SryPaxNumber + itemO2.SryPaxNumber;
                            resultItem.LstPaxNumber = itemO1.LstPaxNumber + itemO2.LstPaxNumber;
                            result.Add(resultItem);
                            break;
                        }
                    }
                }
                return result;
            }
            else
                return o1;
        }


    }

    /// <summary>
    /// Connect ClsDstnBagItem object list
    /// </summary>
    [Serializable]
    public class ClsDstnBagList : IEnumerable
    {
        private ArrayList clsDstnBagList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnBagList()
        {
            clsDstnBagList = new ArrayList();
        }

        /// <summary>
        /// Number of object
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Length
        {
            get { return clsDstnBagList.Count; }
        }

        /// <summary>
        /// get/set object by index
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnBagItem this[int index]
        {
            get { return clsDstnBagList[index] as ClsDstnBagItem; }
            set { clsDstnBagList[index] = value; }
        }

        /// <summary>
        /// Dind object by name
        /// </summary>
        /// <param name="dest">name</param>
        /// <returns>Result</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ClsDstnBagItem Find(string dest)
        {
            foreach (ClsDstnBagItem item in this)
            {
                if (item.Dstn.Equals(dest))
                    return item;
            }
            return null;
        }

        #region IEnumerable Members
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        public IEnumerator GetEnumerator()
        {
            // TODO:  Add ClsDstnBagList.GetEnumerator implementation
            return clsDstnBagList.GetEnumerator();
        }

        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        #endregion

        #region IList Members
        /// <summary>
        /// get read-only
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsReadOnly
        {
            get
            {
                // TODO:  Add ClsDstnClassList.IsReadOnly getter implementation
                return clsDstnBagList.IsReadOnly;
            }
        }

        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="index">index</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void RemoveAt(int index)
        {
            clsDstnBagList.RemoveAt(index);
        }

        /// <summary>
        /// Insert an object
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="value">object</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Insert(int index, ClsDstnBagItem value)
        {
            clsDstnBagList.Insert(index, value);
        }

        /// <summary>
        /// remove an object
        /// </summary>
        /// <param name="value">object</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Remove(ClsDstnBagItem value)
        {
            clsDstnBagList.Remove(value);
        }

        /// <summary>
        /// Find whether object exists
        /// </summary>
        /// <param name="value">object</param>
        /// <returns>existsor not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool Contains(ClsDstnBagItem value)
        {
            // TODO:  Add ClsDstnClassList.Contains implementation
            return clsDstnBagList.Contains(value);
        }

        /// <summary>
        /// Remove all elements
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Clear()
        {
            // TODO:  Add ClsDstnClassList.Clear implementation
            clsDstnBagList.Clear();
        }

        /// <summary>
        /// Search object and return it index in list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int IndexOf(ClsDstnBagItem value)
        {
            // TODO:  Add ClsDstnClassList.IndexOf implementation
            return clsDstnBagList.IndexOf(value);
        }

        /// <summary>
        /// Add an object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Add(ClsDstnBagItem value)
        {
            // TODO:  Add ClsDstnClassList.Add implementation
            return clsDstnBagList.Add(value);
        }

        /// <summary>
        /// Is Fixed size
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsFixedSize
        {
            get
            {
                // TODO:  Add ClsDstnClassList.IsFixedSize getter implementation
                return clsDstnBagList.IsFixedSize;
            }
        }

        #endregion

        /// <summary>
        /// If the destination of ClsDstnBag object between o1 List and o2 List are identical，
        /// then put the difference of FrstPaxBag、SryPaxBag、LstPaxBag of ClsDstnBagtem between o1 and o2 into list，
        /// </summary>
        /// <param name="o1">ClsDstnBagList object</param>
        /// <param name="o2">ClsDstnBagList object</param>
        /// <returns>ClsDstnBagList object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static ClsDstnBagList operator -(ClsDstnBagList o1, ClsDstnBagList o2)
        {
            ClsDstnBagList result = new ClsDstnBagList();
            if (o2 != null)
            {
                ClsDstnBagItem resultItem;
                foreach (ClsDstnBagItem itemO1 in o1)
                {
                    //#BR071005 <-- Compute the F/C/Y‘s pax items for different destination
                    resultItem = new ClsDstnBagItem(itemO1.Dstn);
                    resultItem.FrstPaxBag = itemO1.FrstPaxBag;
                    resultItem.SryPaxBag = itemO1.SryPaxBag;
                    resultItem.LstPaxBag = itemO1.LstPaxBag;
                    foreach (ClsDstnBagItem itemO2 in o2)
                    {
                        if (itemO1.Dstn == itemO2.Dstn)
                        {
                            resultItem.FrstPaxBag -= itemO2.FrstPaxBag;
                            resultItem.SryPaxBag -= itemO2.SryPaxBag;
                            resultItem.LstPaxBag -= itemO2.LstPaxBag;
                            break;
                        }
                    }
                    result.Add(resultItem);
                    /***
                    foreach (ClsDstnBagItem itemO2 in o2)
                    {
                        if (itemO1.Dstn == itemO2.Dstn)
                        {
                            resultItem = new ClsDstnBagItem(itemO1.Dstn);
                            resultItem.FrstPaxBag = itemO1.FrstPaxBag - itemO2.FrstPaxBag;
                            resultItem.SryPaxBag = itemO1.SryPaxBag - itemO2.SryPaxBag;
                            resultItem.LstPaxBag = itemO1.LstPaxBag - itemO2.LstPaxBag;
                            result.Add(resultItem);
                            break;
                        }
                    }
                    ***/
                    //#BR071005 -->
                }
                return result;
            }
            else
                return o1;
        }

        /// <summary>
        /// If the destination of ClsDstnBag object between o1 List and o2 List are identical，
        /// then put the sum of FrstPaxBag、SryPaxBag、LstPaxBag of ClsDstnBagtem between o1 and o2 into list，
        /// </summary>
        /// <param name="o1">ClsDstnBagList object</param>
        /// <param name="o2">ClsDstnBagList object</param>
        /// <returns>ClsDstnBagList object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static ClsDstnBagList operator +(ClsDstnBagList o1, ClsDstnBagList o2)
        {
            ClsDstnBagList result = new ClsDstnBagList();
            if (o2 != null)
            {
                ClsDstnBagItem resultItem;
                foreach (ClsDstnBagItem itemO1 in o1)
                {
                    //#BR071005 <-- Compute the F/C/Y‘s baggage items for different destination
                    resultItem = new ClsDstnBagItem(itemO1.Dstn);
                    resultItem.FrstPaxBag = itemO1.FrstPaxBag;
                    resultItem.SryPaxBag = itemO1.SryPaxBag;
                    resultItem.LstPaxBag = itemO1.LstPaxBag;
                    foreach (ClsDstnBagItem itemO2 in o2)
                    {
                        if (itemO1.Dstn == itemO2.Dstn)
                        {
                            resultItem.FrstPaxBag += itemO2.FrstPaxBag;
                            resultItem.SryPaxBag += itemO2.SryPaxBag;
                            resultItem.LstPaxBag += itemO2.LstPaxBag;
                            break;
                        }
                    }
                    result.Add(resultItem);
                    /****
                    foreach (ClsDstnBagItem itemO2 in o2)
                    {
                        if (itemO1.Dstn == itemO2.Dstn)
                        {
                            resultItem = new ClsDstnBagItem(itemO1.Dstn);
                            resultItem.FrstPaxBag = itemO1.FrstPaxBag + itemO2.FrstPaxBag;
                            resultItem.SryPaxBag = itemO1.SryPaxBag + itemO2.SryPaxBag;
                            resultItem.LstPaxBag = itemO1.LstPaxBag + itemO2.LstPaxBag;
                            result.Add(resultItem);
                            break;
                        }
                    }
                    ****/
                    //#BR071005 -->
                }
                return result;
            }
            else
                return o1;
        }

    }


    /// <summary>
    /// Connect ZoneItem list
    /// </summary>
    [Serializable]
    public class ZoneList : IEnumerable
    {
        private ArrayList zoneList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ZoneList()
        {
            zoneList = new ArrayList();
        }

        /// <summary>
        /// Number of object
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Length
        {
            get { return zoneList.Count; }
        }

        /// <summary>
        /// get/set object by index
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ZoneItem this[int index]
        {
            get { return zoneList[index] as ZoneItem; }
            set { zoneList[index] = value; }
        }

        /// <summary>
        /// Dind object by name
        /// </summary>
        /// <param name="znName">name</param>
        /// <returns>Result</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public ZoneItem Find(string znName)
        {
            foreach (ZoneItem item in this)
            {
                if (item.ZoneName.Equals(znName))
                    return item;
            }
            return null;
        }

        #region IEnumerable Members
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public IEnumerator GetEnumerator()
        {
            // TODO:  Add ClsDstnBagList.GetEnumerator implementation
            return zoneList.GetEnumerator();
        }

        #endregion

        #region IList Members
        /// <summary>
        /// get read-only
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsReadOnly
        {
            get
            {
                // TODO:  Add ClsDstnClassList.IsReadOnly getter implementation
                return zoneList.IsReadOnly;
            }
        }

        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="index">index</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void RemoveAt(int index)
        {
            zoneList.RemoveAt(index);
        }

        /// <summary>
        /// Insert an object
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="value">object</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Insert(int index, ZoneItem value)
        {
            zoneList.Insert(index, value);
        }

        /// <summary>
        /// remove an object
        /// </summary>
        /// <param name="value">object</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Remove(ZoneItem value)
        {
            zoneList.Remove(value);
        }

        /// <summary>
        /// Find whether object exists
        /// </summary>
        /// <param name="value">object</param>
        /// <returns>existsor not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool Contains(ZoneItem value)
        {
            // TODO:  Add ClsDstnClassList.Contains implementation
            return zoneList.Contains(value);
        }

        /// <summary>
        /// Remove all elements
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Clear()
        {
            // TODO:  Add ClsDstnClassList.Clear implementation
            zoneList.Clear();
        }

        /// <summary>
        /// Search object and return it index in list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int IndexOf(ZoneItem value)
        {
            // TODO:  Add ClsDstnClassList.IndexOf implementation
            return zoneList.IndexOf(value);
        }

        /// <summary>
        /// Add an object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Add(ZoneItem value)
        {
            // TODO:  Add ClsDstnClassList.Add implementation
            ZoneItem item = this.Find(value.ZoneName);
            if (item != null)
            {
                item.TtlPax += value.TtlPax;
                return IndexOf(item);
            }
            return zoneList.Add(value);
        }

        /// <summary>
        /// Is Fixed size
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsFixedSize
        {
            get
            {
                // TODO:  Add ClsDstnClassList.IsFixedSize getter implementation
                return zoneList.IsFixedSize;
            }
        }

        #endregion

        /// <summary>
        /// Substract an object
        /// </summary>
        /// <param name="value">object</param>
        /// <returns>result</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Sub(ZoneItem value)
        {
            // TODO:  Add ClsDstnClassList.Add implementation
            ZoneItem item = this.Find(value.ZoneName);
            if (item != null)
            {
                item.TtlPax -= value.TtlPax;
                return IndexOf(item);
            }

            value.TtlPax = -value.TtlPax;
            return zoneList.Add(value);
        }

        /// <summary>
        /// Put the sum of ZoneItem of o1 List and o2 List into new ZoneList
        /// </summary>
        /// <param name="o1">ZoneList object</param>
        /// <param name="o2">ZoneList object</param>
        /// <returns>ZoneList object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static ZoneList operator +(ZoneList o1, ZoneList o2)
        {
            ZoneList result = new ZoneList();
            foreach (ZoneItem item in o1)
            {
                result.Add(item);
            }

            foreach (ZoneItem item in o2)
            {
                result.Add(item);
            }
            return result;
        }


    }


    /// <summary>
    /// Connect GenderDstnClassItem list
    /// </summary>
    /// <remarks>
    /// Modified Date:
    /// Modified By:
    /// Modified Reason:
    /// </remarks>
    [Serializable]
    public class GenderDstnClassList : IEnumerable
    {
        private ArrayList genderDstnClassList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public GenderDstnClassList()
        {
            genderDstnClassList = new ArrayList();
        }

        /// <summary>
        /// Number of object
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Length
        {
            get { return genderDstnClassList.Count; }
        }

        /// <summary>
        /// get/set object by index
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public GenderDstnClassItem this[int index]
        {
            get { return genderDstnClassList[index] as GenderDstnClassItem; }
            set { genderDstnClassList[index] = value; }
        }

        /// <summary>
        /// Dind object by name
        /// </summary>
        /// <param name="dest">name</param>
        /// <returns>Result</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public GenderDstnClassItem Find(string dest)
        {
            foreach (GenderDstnClassItem item in this)
            {
                if (item.Dstn.Equals(dest))
                    return item;
            }
            return null;
        }

        /// <summary>
        /// count total
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public GenderDstnClassItem Total
        {
            get
            {
                GenderDstnClassItem result = new GenderDstnClassItem("");
                foreach (GenderDstnClassItem tmpBookedClassItem in genderDstnClassList)
                    result += tmpBookedClassItem;
                return result;
            }
        }

        #region IEnumerable Members
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public IEnumerator GetEnumerator()
        {
            // TODO:  Add ClsDstnBagList.GetEnumerator implementation
            return genderDstnClassList.GetEnumerator();
        }

        #endregion

        #region IList Members

        /// <summary>
        /// get read-only
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsReadOnly
        {
            get
            {
                // TODO:  Add ClsDstnClassList.IsReadOnly getter implementation
                return genderDstnClassList.IsReadOnly;
            }
        }


        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="index">index</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void RemoveAt(int index)
        {
            genderDstnClassList.RemoveAt(index);
        }

        /// <summary>
        /// Insert an object
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="value">object</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Insert(int index, GenderDstnClassItem value)
        {
            genderDstnClassList.Insert(index, value);
        }

        /// <summary>
        /// remove an object
        /// </summary>
        /// <param name="value">object</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Remove(GenderDstnClassItem value)
        {
            genderDstnClassList.Remove(value);
        }

        /// <summary>
        /// Find whether object exists
        /// </summary>
        /// <param name="value">object</param>
        /// <returns>existsor not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool Contains(GenderDstnClassItem value)
        {
            // TODO:  Add ClsDstnClassList.Contains implementation
            return genderDstnClassList.Contains(value);
        }

        /// <summary>
        /// Remove all elements
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void Clear()
        {
            // TODO:  Add ClsDstnClassList.Clear implementation
            genderDstnClassList.Clear();
        }

        /// <summary>
        /// Search object and return it index in list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int IndexOf(GenderDstnClassItem value)
        {
            // TODO:  Add ClsDstnClassList.IndexOf implementation
            return genderDstnClassList.IndexOf(value);
        }

        /// <summary>
        /// Add an object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Add(GenderDstnClassItem value)
        {
            // TODO:  Add ClsDstnClassList.Add implementation
            GenderDstnClassItem item = this.Find(value.Dstn);
            if (item != null)
            {
                item += value;
                return IndexOf(item);
            }
            return genderDstnClassList.Add(value);
        }

        /// <summary>
        /// Is Fixed size
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsFixedSize
        {
            get
            {
                // TODO:  Add ClsDstnClassList.IsFixedSize getter implementation
                return genderDstnClassList.IsFixedSize;
            }
        }

        #endregion

        /// <summary>
        /// Substract an object
        /// </summary>
        /// <param name="value">Substracter</param>
        /// <returns>result</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public int Sub(GenderDstnClassItem value)
        {
            // TODO:  Add ClsDstnClassList.Add implementation
            GenderDstnClassItem item = this.Find(value.Dstn);
            if (item != null)
            {
                item -= value;
                return IndexOf(item);
            }
            throw (new Exception("GenderDstnClassList error."));
        }

        /// <summary>
        /// If the destination of GenderDstnClassItem object between o1 List and o2 List are identical，
        /// then put the sum of Male、Female、Child、Infant of GenderDstnClassItem of o1 and o2  into list，
        /// </summary>
        /// <param name="o1">GenderDstnClassList object</param>
        /// <param name="o2">GenderDstnClassList object</param>
        /// <returns>GenderDstnClassList object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static GenderDstnClassList operator +(GenderDstnClassList o1, GenderDstnClassList o2)
        {
            GenderDstnClassList result = new GenderDstnClassList();
            if (o2 != null)
            {
                GenderDstnClassItem resultItem;
                foreach (GenderDstnClassItem itemO1 in o1)
                {
                    foreach (GenderDstnClassItem itemO2 in o2)
                    {
                        if (itemO1.Dstn == itemO2.Dstn)
                        {
                            resultItem = new GenderDstnClassItem(itemO1.Dstn);
                            resultItem.Male = itemO1.Male + itemO2.Male;
                            resultItem.Female = itemO1.Female + itemO2.Female;
                            resultItem.Child = itemO1.Child + itemO2.Child;
                            resultItem.Infant = itemO1.Infant + itemO2.Infant;

                            result.Add(resultItem);
                            break;
                        }
                    }
                }
                return result;
            }
            else
                return o1;

        }

        /// <summary>
        /// If the destination of GenderDstnClassItem object between o1 List and o2 List are identical，
        /// then put the difference of Male、Female、Child、Infant of GenderDstnClassItem between o1 and o2  into list，
        /// </summary>
        /// <param name="o1">GenderDstnClassList object</param>
        /// <param name="o2">GenderDstnClassList object</param>
        /// <returns>GenderDstnClassList object</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static GenderDstnClassList operator -(GenderDstnClassList o1, GenderDstnClassList o2)
        {
            GenderDstnClassList result = new GenderDstnClassList();
            if (o2 != null)
            {
                GenderDstnClassItem resultItem;
                foreach (GenderDstnClassItem itemO1 in o1)
                {
                    foreach (GenderDstnClassItem itemO2 in o2)
                    {
                        if (itemO1.Dstn == itemO2.Dstn)
                        {
                            resultItem = new GenderDstnClassItem(itemO1.Dstn);

                            resultItem.Male = itemO1.Male - itemO2.Male;
                            resultItem.Female = itemO1.Female - itemO2.Female;
                            resultItem.Child = itemO1.Child - itemO2.Child;
                            resultItem.Infant = itemO1.Infant - itemO2.Infant;

                            result.Add(resultItem);
                            break;
                        }
                    }
                }
                return result;
            }
            else
                return o1;

        }

    }
}