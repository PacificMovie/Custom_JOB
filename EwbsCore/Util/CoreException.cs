/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
using System;

namespace EwbsCore.Util
{
    /// <summary>
    /// Summary description for CoreException.
    /// </summary>
    public class CoreException : Exception
    {
        private int iMsgID; //Message ID
        private string auxMessage; //auxilliary message
        private string moduleName; //Module name
        //string errReason;
        public CoreException(int iMsgID, string errReason, string auxMessage, string moduleName)
            : base(errReason)
        {
            this.iMsgID = iMsgID;
            this.auxMessage = auxMessage;
            this.moduleName = moduleName;
            // this.errReason = errReason;
        }

        /// <summary>
        /// get the Message ID
        /// </summary>
        public int MsgID
        {
            get { return iMsgID; }
        }

        /// <summary>
        /// get the auxilliary message
        /// </summary>
        public string AuxMessage
        {
            get { return auxMessage; }
        }

        /// <summary>
        /// get module Name
        /// </summary>
        public string ModuleName
        {
            get { return moduleName; }
        }

        //public string ErrReason
        //{
        //	get { return errReason;}
        //}
    }
}