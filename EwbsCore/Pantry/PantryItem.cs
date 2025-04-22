/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
using System;

namespace EWBS
{
    /// <summary>
    /// Summary description for PantryItem.
    /// </summary>
    [Serializable]
    public class PantryItem
    {
        private string name; //Name of pantry
        private double index, weight; //index, weight

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Pantry zone name</param>
        /// <param name="weight">weight it contains</param>
        /// <param name="index"></param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public PantryItem(string name, double weight, double index)
        {
            this.name = name;
            this.weight = weight;
            this.index = index;
        }

        #region Properties

        /// <summary>
        /// get or set pantry name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// get or set index
        /// </summary>
        public double Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// get or set weight
        /// </summary>
        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        # endregion
    }
}