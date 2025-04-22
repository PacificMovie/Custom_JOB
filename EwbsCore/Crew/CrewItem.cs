/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//* THOMAS    | Ver. 01 | #BR17206  | 2017/8/15                                                                    
//*---------------------------------------------------------------------------*
//* EXTRA CREW 改為ZONE，新增FO/SO   
//*****************************************************************************
using System;

namespace EWBS
{
    /// <summary>
    /// Summary description for CrewItem.
    /// </summary>
    [Serializable]
    public class CrewItem
    {
        private int cockpit; //number of cockpit crew
        private int cabin; //number of cabin crew
        private string name; //crew item name
        private int maxSeat; //Maximum seat
        private float indexPerKg; //index per kg
        private string textName;  //#BR17206 THOMAS 改為ZONE 之後，讀取舊有的 wbf檔案時， Extra Crew Label   First  /  Secondary/ Last  convert to   F/C/Y 讓畫面上顯示F/C/Y，增加此欄位

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">the zone name</param>
        /// <param name="maxSeat">the Maximum seat</param>
        /// <param name="indexPerKg">index per kg of weight</param>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public CrewItem(string name, int maxSeat, float indexPerKg)
        {
            this.name = name;
            this.cockpit = 0;
            this.cabin = 0;
            this.maxSeat = maxSeat;
            this.indexPerKg = indexPerKg;
        }

        //<!--#BR17206 THOMAS
        public string TextName
        {
            get { return textName; }
            set { textName = value; }
        }
        //#BR17206 -->

        /// <summary>
        /// indexPerKg
        /// </summary>
        public float IndexPerKg
        {
            get { return indexPerKg; }
            set { indexPerKg = value; }
        }

        /// <summary>
        /// name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// coxkpit
        /// </summary>
        public int Cockpit
        {
            get { return cockpit; }
            set { cockpit = value; }
        }

        /// <summary>
        /// cabin
        /// </summary>
        public int Cabin
        {
            get { return cabin; }
            set { cabin = value; }
        }

        /// <summary>
        /// maxSeat
        /// </summary>
        public int MaxSeat
        {
            get { return maxSeat; }
            set { maxSeat = value; }
        }

        /// <summary>
        /// to operate while user wanna add CrewItem2 to CrewItem1.
        /// </summary>
        /// <param name="item1">Crew Item 1</param>
        /// <param name="item2">Crew Item 2</param>
        /// <returns>The result after adding</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static CrewItem operator +(CrewItem item1, CrewItem item2)
        {
            //Create a new CrewItem
            CrewItem newCrewInfo = new CrewItem("", 0, 0);

            //setup the new item with the total values of the two items.
            newCrewInfo.cockpit = item1.cockpit + item2.cockpit;
            newCrewInfo.cabin = item1.cabin + item2.cabin;
            newCrewInfo.maxSeat = item1.maxSeat + item2.maxSeat;

            return newCrewInfo;
        }

        /// <summary>
        /// to operate while user wanna minus the CrewItem1 with the data in crewitem 2.
        /// </summary>
        /// <param name="item1">Crew Item 1</param>
        /// <param name="item2">Crew Item 2</param>
        /// <returns>The result after adding</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static CrewItem operator -(CrewItem item1, CrewItem item2)
        {
            //Create a new CrewItem
            CrewItem newCrewInfo = new CrewItem("", 0, 0);

            //setup the new item with the different values of the two items.
            newCrewInfo.cockpit = item1.cockpit - item2.cockpit;
            newCrewInfo.cabin = item1.cabin - item2.cabin;
            newCrewInfo.maxSeat = item1.maxSeat - item2.maxSeat;

            //if the values are under zero, just set them to zero.
            if (newCrewInfo.cockpit < 0) newCrewInfo.cockpit = 0;
            if (newCrewInfo.cabin < 0) newCrewInfo.cabin = 0;
            if (newCrewInfo.maxSeat < 0) newCrewInfo.maxSeat = 0;

            return newCrewInfo;
        }
    }
}