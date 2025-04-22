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

namespace EWBS
{
    /// <summary>
    /// Summary description for PaxSeat.
    /// </summary>
    [Serializable]
    public class SeatItem
    {
        private int bagPieces;
        private double bagWeight;
        private string seatNo, dstn;
        //private PaxInfo PI;
        private string code = ""; // seat status code
        private char gender = ' ';


        #region constructor
        public SeatItem(string seatNoInput, string dstnInput, string codeInput, double bagWeightInput, int bagPiecesInput, string gender)
        {
            this.seatNo = seatNoInput;
            this.dstn = dstnInput;
            this.bagWeight = bagWeightInput;
            this.bagPieces = bagPiecesInput;

            if (codeInput != null) this.code = codeInput.ToUpper();

            if (gender.Length > 0)
            {
                this.gender = gender.ToUpper()[0];
            }

        }
        #endregion

        #region properties
        public int BagPieces
        {
            get { return bagPieces; }
            set { bagPieces = value; }
        }

        public string Dstn
        {
            get { return dstn; }
            set { dstn = value; }
        }

        public double BagWeight
        {
            get { return bagWeight; }
            set { bagWeight = value; }
        }


        public string SeatNo
        {
            get { return seatNo; }
            set { seatNo = value; }
        }

        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        public string Gender
        {
            get { return Convert.ToString(this.gender); }
        }
        #endregion

        /// <summary>
        /// Use SeatItem object to construct GenderDstnClassItem object，then pass out some sexual value depending on SeatItem Gender
        /// </summary>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public GenderDstnClassItem PaxInfo
        {
            get
            {
                GenderDstnClassItem paxInfo = new GenderDstnClassItem(this.dstn);
                switch (this.gender)
                {
                    case 'M':
                        paxInfo.Male = 1;
                        break;
                    case 'F':
                        paxInfo.Female = 1;
                        break;
                    case 'C':
                        paxInfo.Child = 1;
                        break;
                    case 'I':
                        paxInfo.Infant = 1;
                        break;
                }
                return paxInfo;
            }
        }


        /// <summary>
        /// Is legal seat
        /// </summary>
        /// <returns>result</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public bool IsLegal()
        {
            return gender != ' ' && seatNo.IndexOf(" ") < 0;
        }

    }

    /// <summary>
    /// Connect SeatItem list
    /// </summary>
    /// <remarks>
    /// Modified Date:
    /// Modified By:
    /// Modified Reason:
    /// </remarks>
    [Serializable]
    public class SeatList : ArrayList
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SeatList()
        {
        }

        public int Length
        {
            get { return this.Count; }
        }

        //<--  隱藏繼承的成員，加入new 關鍵字-->
        public new  SeatItem this[int index]
        {
            get { return base[index] as SeatItem; }
            set { base[index] = value; }
        }

        /// <summary>
        ///  add SeatItem object
        /// </summary>
        /// <param name="value">SeatItem object</param>
        /// <returns>index value</returns>
        public override int Add(object value)
        {
            // TODO:  Add SeatList.Add implementation
            if (value is SeatItem)
            {
                return base.Add(value);
            }
            else
            {
                return -1;
            }
        }

    }


}