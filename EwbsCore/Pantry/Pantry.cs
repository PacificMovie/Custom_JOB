/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
using System;
using System.Collections;
using FlightDataBase;

namespace EWBS
{
    /// <summary>
    /// Summary description for Pantry.
    /// </summary>
    [Serializable]
    public class Pantry
    {
        private string code; //Pantry code
        private PantryItem[] pantryList; //List of information of every pantry
        private PantryItem[] extraPantryList; //List of information of every extra pantry

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="theFlight">the flight that's copied data from</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public Pantry(Flight theFlight)
        {
            //obtain the aircraft type
            AirTypeEx theAirType = theFlight.ACType;

            if (theAirType.PantryCode != null)
            {
                //Get PantryCode from FDB and add into pantryList
                PantryCodeInfo[] pantryCodeInfoList = theAirType.PantryCode.info;
                pantryList = new PantryItem[pantryCodeInfoList.Length];
                for (int i = 0; i < pantryCodeInfoList.Length; i++)
                {
                    pantryList[i] = new PantryItem(pantryCodeInfoList[i].pantry,
                                                   pantryCodeInfoList[i].weight,
                                                   pantryCodeInfoList[i].index);
                }
            }

            //Get ZonePantryIndexGalley from FDB and add into extraPantryList

            //The freight have no extraPantry.
            if (theAirType.ZonePantryIndex != null && theAirType.ZonePantryIndex.galley != null)
            {
                ZonePantryIndexGalleyGinfo[] galleyGinfoList = theAirType.ZonePantryIndex.galley.ginfo;
                extraPantryList = new PantryItem[galleyGinfoList.Length];
                for (int i = 0; i < galleyGinfoList.Length; i++)
                {
                    extraPantryList[i] = new PantryItem(galleyGinfoList[i].id, 0, galleyGinfoList[i].index);
                }
            }

            code = "";
            if (pantryList.Length > 0)
                code = pantryList[0].Name;
        }

        #region Properties

        /// <summary>
        /// get or set pantry code
        /// </summary>
        public string Code
        {
            get { return code.Trim(); }
            set { code = value; }
        }

        /// <summary>
        /// get pantryList
        /// </summary>
        public PantryItem[] PantryList
        {
            get { return pantryList; }
        }

        /// <summary>
        /// get extraPantryList
        /// </summary>
        public PantryItem[] ExtraPantryList
        {
            get { return extraPantryList; }
        }

        #endregion

        #region Methods

        /// <summary>Get  weight </summary>
        /// <returns>double: weight</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float GetWeight()
        {
            float ttlWeight = 0;

            if (code != null)
            {
                // compute Pantry weight 
                foreach (PantryItem pantryItem in pantryList)
                    if (code.Equals(pantryItem.Name))
                        ttlWeight += (float)pantryItem.Weight;
            }
            return ttlWeight;
        }

        /// <summary>Get Index</summary>
        /// <returns>double: index</returns>
        /// <remarks></remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public float GetIndex()
        {
            float index = 0;
            // compute Index of Pantry
            foreach (PantryItem pantryItem in pantryList)
                if (code.Equals(pantryItem.Name))
                    index += (float)pantryItem.Index;
            return index;
        }

        #endregion

        /// <summary>
        /// copy other Pantry obj's data into this Pantry Obj
        /// </summary>
        /// <param name="other">other pantry list</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public void CopyFrom(Pantry other)
        {
            ArrayList aList = new ArrayList();
            foreach (PantryItem pantryItem in other.pantryList)
                aList.Add(new PantryItem(pantryItem.Name, pantryItem.Weight, pantryItem.Index));
            pantryList = (PantryItem[])aList.ToArray(typeof(PantryItem));

            code = other.code;
        }
    }
}