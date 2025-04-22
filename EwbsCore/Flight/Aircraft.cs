/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
using System;
using FlightDataBase;

namespace EWBS
{
    /// <summary>
    /// Aircraft includes ac no, basic wt and basic index.
    /// </summary>
    [Serializable]
    public class Aircraft
    {
        private string name; //ac no
        private int bw; //Basic Weight
        private float bi; //Basic Index

        /// <summary>
        /// get ac no
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// get basic wt
        /// </summary>
        public int BW
        {
            get { return bw; }
        }

        /// <summary>
        /// get basic index
        /// </summary>
        public float BI
        {
            get { return bi; }
        }

        /// <summary>
        /// Aircraft Constructor
        /// </summary>
        /// <param name="other">Aircraft in FDB</param>
        public Aircraft(airinfoAirlineAirtypeAircode other)
        {
            name = other.name;
            bw = other.bw;
            bi = other.bi;

        }
    }
}