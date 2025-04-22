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
using EwbsCore.Util;

namespace nsBaggage
{
    /// <summary>
    /// Summary description for BagRule.
    /// </summary>
    public class BagRule
    {
        private string uldType; //ULD type
        private int qty; //Quantity
        private ArrayList destList; //Arraylist of destination
        private ArrayList categoryList; //Arraylist of category
        private int maxBagWt; //Maximum baggage weight
        private bool transit; //transit baggage


        /// <summary>
        /// BagRule Class Constructor
        /// </summary>
        /// <param name="uldType"> ULD type </param>
        /// <param name="qty"> ULD  number</param>
        /// <param name="dest"> destination </param>
        /// <param name="category"> baggage  class has BC,BY two catagories</param>
        /// <param name="maxBagWt">The ULD net weight is not included in ULD baggage restricted weight</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagRule(string uldType, int qty, string dest, string category, int maxBagWt, bool transit)
        {
            this.uldType = uldType;
            this.qty = qty;

            // The destination station or baggage  class which is splited from string will be add into their own list 
            destList = Strings.StringToList(dest);
            categoryList = Strings.StringToList(category);

            this.maxBagWt = maxBagWt;

            this.transit = transit;
        }

        /// <summary>
        /// BagRule constructor
        /// </summary>
        /// <param name="aBagRule">BagRule obj</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagRule(BagRule aBagRule)
        {
            string destString, categoryString;
            //Use the pass-in parameter to set BagRule's uld type
            this.uldType = String.Copy(aBagRule.uldType);
            //Use the pass-in parameter to set BagRule's quantity
            this.qty = aBagRule.qty;
            //Use the pass-in parameter to set BagRule's Max Weight
            this.maxBagWt = aBagRule.maxBagWt;

            this.destList = new ArrayList();
            destString = Strings.ListToString(aBagRule.destList);
            //Set destination 
            this.destList = Strings.StringToList(destString);

            categoryString = Strings.ListToString(aBagRule.categoryList);
            this.categoryList = new ArrayList();
            //Set catalog
            this.categoryList = Strings.StringToList(categoryString);

            //Set transit
            this.transit = aBagRule.transit;
        }

        /// <summary>
        /// qty
        /// </summary>
        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }

        /// <summary>
        /// uldType
        /// </summary>
        public string ULDType
        {
            get { return uldType; }
            set { uldType = value; }
        }

        /// <summary>
        /// destList
        /// </summary>
        public ArrayList DestList
        {
            get { return destList; }
            set { destList = value; }
        }

        /// <summary>
        /// categoryList
        /// </summary>
        public ArrayList CategoryList
        {
            get { return categoryList; }
            set { categoryList = value; }
        }

        /// <summary>
        /// maxBagWt
        /// </summary>
        public int MaxBagWt
        {
            get { return maxBagWt; }
            set { maxBagWt = value; }
        }


        /// <summary>
        /// bTransit
        /// </summary>
        public bool bTransit
        {
            get { return transit; }
        }

        //--------------------------------------------------------------------

        /// <summary>
        /// Check if two BagRules are the same or not ?
        /// </summary>
        /// <param name="aRule">BagRule obj</param>
        /// <returns>true:equal, false:unequal</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool Equals(BagRule aRule)
        {
            bool bClassSame = false, bDestSame = false;

            if (aRule.ULDType != this.ULDType)
                return false;

            if (aRule.MaxBagWt != this.MaxBagWt)
                return false;

            if (aRule.bTransit != this.bTransit)
                return false;

            // To chech if two Baggage Classes are identical or not.
            bClassSame = Strings.Equals(aRule.CategoryList, this.CategoryList);
            bDestSame = Strings.Equals(aRule.DestList, this.DestList);

            return (bClassSame && bDestSame);
        }

        /// <summary>
        /// Check if two BagRules are the same or not ?
        /// </summary>
        /// <param name="aRule">BagRule</param>
        /// <returns>true: similar, false: not similar</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool Similar(BagRule aRule)
        {
            int count = 4;
            if (aRule.ULDType != this.ULDType)
                count -= 1;

            if (aRule.MaxBagWt != this.MaxBagWt)
                count -= 1;

            // To chech if two Baggage Classes are identical or not.
            if (!Strings.Equals(aRule.CategoryList, this.CategoryList))
                count -= 1;

            if (!Strings.Equals(aRule.DestList, this.DestList))
                count -= 1;


            return count == 3;
        }

    }

    /// <summary>
    /// An arrayList of BagRules
    /// </summary>
    public class BagRuleList : ArrayList
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public BagRuleList()
        { }

        /// <summary>
        ///  search identical BagRule
        /// </summary>
        /// <param name="aRule">BagRule obj</param>
        /// <returns>ArrayList index</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private int FindBagRule(BagRule aRule)
        {
            for (int j = 0; j < base.Count; j++)
            {
                if (aRule.Equals(base[j] as BagRule))
                    return j;
            }
            return -1;
        }


        /// <summary>
        /// Update or add BagRule
        /// </summary>
        /// <param name="value">BagRule obj</param>
        /// <returns>ArrayList index</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public override int Add(object value)
        {
            //If the added object is BagRule
            if (value is BagRule)
            {
                int iRuleIndex = FindBagRule(value as BagRule);
                if (iRuleIndex >= 0)
                { // BagRule exist
                    (base[iRuleIndex] as BagRule).Qty += (value as BagRule).Qty;
                    return iRuleIndex;
                }
                else
                { // no BagRule
                    return base.Add(value);
                }
            }
            else
            {
                return -1;
            }
        }


    }


}