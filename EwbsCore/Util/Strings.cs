/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
using System.Collections;

namespace EwbsCore.Util
{
    /// <summary>
    /// Provides static methods for strings
    /// </summary>
    public class Strings
    {
        private static string DestSep = ","; //delimiter

        /// <summary>
        /// Split string by comma into ArrayList
        /// </summary>
        /// <param name="aString">input string</param>
        /// <returns>string array</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static ArrayList StringToList(string aString)
        {
            ArrayList aList = new ArrayList();

            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            aList.AddRange(aString.Split(delimiter));
            aList.Sort();
            return aList;
        }


        /// <summary>shorten string</summary>
        /// <param name="aString"></param>
        /// <returns>result of </returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static string shortenString(string aString)
        {
            string result = "";
            string CopiedString = aString + DestSep;

            int iPos = 0, iStart = 0;

            // split sting
            do
            {
                iPos = CopiedString.IndexOf(DestSep, iStart);
                if (iPos >= 0)
                {
                    // skip duplicated string
                    string str = aString.Substring(iStart, iPos - iStart).Trim();
                    if (str.Length > 0 && result.IndexOf(str) < 0)
                        result += str + DestSep;
                    iStart = iPos + 1;
                }
            } while (iPos >= 0);
            return (result.Length > 0 ? result.Substring(0, result.Length - 1) : result);
        }


        /// <summary>Convert list into string</summary>
        /// <param name="aList">string array</param>
        /// <returns>result</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static string ListToString(ArrayList aList)
        {
            return ListToString((string[])aList.ToArray(typeof(string)));
        }


        /// <summary>Convert list into string</summary>
        /// <param name="aList">string array</param>
        /// <returns>result</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static string ListToString(string[] aList)
        {
            string result = "";
            int len = aList.Length;
            if (len > 0)
            {
                result = aList[0].ToString();
                for (int i = 1; i < len; i++)
                {
                    result += ",";
                    result += aList[i].ToString();
                }
            }
            return shortenString(result);
        }


        /// <summary>Check if two array are equal</summary>
        /// <param name="aList">ArrayList</param>
        /// <param name="bList">ArrayList</param>
        /// <returns>equal or not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static bool Equals(ArrayList aList, ArrayList bList)
        {
            return Equals((string[])aList.ToArray(typeof(string)), (string[])bList.ToArray(typeof(string)));
        }


        /// <summary>Check if two string are equal</summary>
        /// <param name="aList">string array 1</param>
        /// <param name="bList">string array 2</param>
        /// <returns>equal ot not</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static bool Equals(string[] aList, string[] bList)
        {
            int iSame = 0;

            if (aList.Length != bList.Length) return false;
            foreach (string aString in aList)
            {
                foreach (string bString in bList)
                {
                    if (aString.Equals(bString)) //Found
                    {
                        iSame++;
                        break;
                    }
                }
            }
            if (iSame == aList.Length || iSame == bList.Length)
                return true;
            return false;
        }
        /// <summary>
        /// 判斷字串內容為數字
        /// </summary>
        /// <param name="pStr"></param>
        /// <returns></returns>
        public static bool IsNumeric(string pStr)
        {
            try
            {
                long result = System.Int64.Parse(pStr);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}