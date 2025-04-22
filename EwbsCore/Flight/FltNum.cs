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
    /// Flight number includes carrier code, serial number and suffix Code.
    /// </summary>
    [Serializable]
    public class FlightNumber
    {
        private string carrierCode; //Carrier code
        private int serialNo; //Serial number
        private char suffixCode; //Suffix Code

        /// <summary>
        /// Flight Number constructor
        /// </summary>
        /// <param name="fltno">flight number</param>
        public FlightNumber(string fltno)
        {
            //Parse fltno
            if (fltno.Length > 2)
            {
                int len = fltno.Length;
                int i;

                //Retrieve carrier code
                carrierCode = fltno.Substring(0, 2);

                //Retrieve serial no and suffix code
                for (i = 2; i < len; i++)
                    if (!Char.IsNumber(fltno[i])) break;
                if (i <= len)
                {
                    serialNo = int.Parse(fltno.Substring(2, i - 2));
                    suffixCode = (i < len ? fltno[i] : ' ');
                }
            }
        }

        /// <summary>
        /// Flight Number constructor
        /// </summary>
        /// <param name="carrierCode">carrier Code</param>
        /// <param name="serialNo">serialNo</param>
        public FlightNumber(string carrierCode, int serialNo)
        {
            //Carrier Code, no more then 2 chars
            this.carrierCode = carrierCode;
            if (this.carrierCode.Length > 2)
                this.carrierCode = carrierCode.Substring(0, 2);
            this.serialNo = serialNo;
        }

        /// <summary>
        /// Flight Number constructor
        /// </summary>
        /// <param name="o">Flight Number</param>
        public FlightNumber(FlightNumber o)
        {
            carrierCode = o.carrierCode;
            serialNo = o.serialNo;
            suffixCode = o.suffixCode;
        }


        /// <summary>
        /// get Carrier code
        /// </summary>
        public string CarrierCode
        {
            get { return carrierCode; }
        }

        /// <summary>
        /// get Serial number
        /// </summary>
        public int SerialNo
        {
            get { return serialNo; }
        }

        /// <summary>
        /// get and set Suffix Code
        /// </summary>
        public char SuffixCode
        {
            get { return suffixCode; }
            set { suffixCode = value; }
        }


        /// <summary>Convert flight number to string</summary>
        /// <returns>Flight Number in string</returns>
        /// <remarks>
        /// </remarks> 
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public override string ToString()
        {
            if (Char.IsUpper(suffixCode))
            {
                return String.Format("{0}{1:d04}{2}", carrierCode, serialNo, suffixCode).Trim();
            }
            else
            {
                return String.Format("{0}{1:d04}", carrierCode, serialNo);
            }
        }
    }

}