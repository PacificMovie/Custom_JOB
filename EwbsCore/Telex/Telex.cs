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
    /// Telex class includes all kinds of telex.
    /// </summary>
    [Serializable]
    public class Telex
    {
        private TelexBase cpm = new TelexBase(); //Contents of CPM
        private TelexBase ldm = new TelexBase(); //Contents of LDM
        private UCM_T ucm = new UCM_T(); //Contents of UCM
        private TelexBase notoc = new TelexBase(); //Contents of NOTOC
        private TelexBase ali = new TelexBase(); //Contents of ALI
        private TelexBase sls = new TelexBase(); //Contents of SLS
        private TelexBase lpm = new TelexBase(); //Contents of LPM
        private TelexBase uws = new TelexBase(); //Contents of UWS
        private TelexBase lir = new TelexBase(); //Contents of LIR
        private TelexBase ls = new TelexBase(); //Contents of LS
        private TelexBase acars = new TelexBase(); //Contents of ACARS

        /// <summary>
        /// use telex category to get the TelexBase object 
        /// </summary>
        /// <param name="idx">telex category</param>
        /// <returns>TelexBase object </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public TelexBase GetTelexBase(int idx)
        {
            if (idx == 0) return cpm;
            else if (idx == 1) return ldm;
            else if (idx == 2) return ucm;
            else if (idx == 3) return notoc;
            else if (idx == 4) return ali;
            else if (idx == 5) return sls;
            else if (idx == 6) return lpm;
            else if (idx == 7) return uws;
            else if (idx == 8) return lir;
            else if (idx == 9) return ls;
            else if (idx == 10) return acars;
            else return null;
        }

        /// <summary>
        /// use TelexBase object  to get the telex category
        /// </summary>
        /// <param name="_telex">TelexBase object </param>
        /// <returns>int: telex category</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public int GetIndicator(TelexBase _telex)
        {
            if (_telex == cpm) return 0;
            else if (_telex == ldm) return 1;
            else if (_telex == ucm) return 2;
            else if (_telex == notoc) return 3;
            else if (_telex == ali) return 4;
            else if (_telex == sls) return 5;
            else if (_telex == lpm) return 6;
            else if (_telex == uws) return 7;
            else if (_telex == lir) return 8;
            else if (_telex == ls) return 9;
            else if (_telex == acars) return 10;
            else return -1;
        }

        /// <summary>
        /// get Telex content of CPM
        /// </summary>
        public TelexBase CPM
        {
            get { return cpm; }
        }

        /// <summary>
        /// get Telex content of LDM
        /// </summary>
        public TelexBase LDM
        {
            get { return ldm; }
        }


        /// <summary>
        /// get Telex content of UCM
        /// </summary>
        public UCM_T UCM
        {
            get { return ucm; }
        }


        /// <summary>
        /// get Telex content of NOTOC
        /// </summary>
        public TelexBase NOTOC
        {
            get { return notoc; }
        }


        /// <summary>
        /// get Telex content of ALI
        /// </summary>
        public TelexBase ALI
        {
            get { return ali; }
        }


        /// <summary>
        /// get Telex content of SLS
        /// </summary>
        public TelexBase SLS
        {
            get { return sls; }
        }


        /// <summary>
        /// get Telex content of LPM
        /// </summary>
        public TelexBase LPM
        {
            get { return lpm; }
        }


        /// <summary>
        /// get Telex content of UWS
        /// </summary>
        public TelexBase UWS
        {
            get { return uws; }
        }


        /// <summary>
        /// get Telex content of LIR
        /// </summary>
        public TelexBase LIR
        {
            get { return lir; }
        }


        /// <summary>
        /// get Telex content of LS
        /// </summary>
        public TelexBase LS
        {
            get { return ls; }
        }


        /// <summary>
        /// get Telex content of ACARS
        /// </summary>
        public TelexBase ACARS
        {
            get { return acars; }
        }

    }

    /// <summary>
    /// Used for telexs except UCM, LIR, ACARS, and Loadsheet
    /// </summary>
    [Serializable]
    public class TelexBase
    {
        private string dblsig = ""; // company to be charged
        private string priority = "QD"; // QU/QK/QD  1/2/3
        private string receiver = ""; //Receiver
        private string sender = ""; //sender
        private string text = ""; // content
        private string si = ""; // supplementary information

        /// <summary>
        /// to get the Content to send
        /// </summary>
        public string Content
        {
            get
            {
                int idx = text.IndexOf("\r\nSI\r\n");
                if (idx > 0)
                {
                    if (si.Length > 0 && text.Substring(idx).IndexOf(si) < 0)
                        return text + si;
                    else
                        return text;
                }
                //return ((si == "") ? text : text.Insert(text.LastIndexOf("-BULK4NCW-") , "SI\r\n" + si +"\r\n\r\n" ));  //BULK4NCW 排到最後
                return ((si == "") ? text : text + "SI\r\n" + si);
            }
        }

        /// <summary>
        /// Get or set the company to be charged
        /// </summary>
        public string Dblsig
        {
            get { return dblsig; }
            set { dblsig = value; }
        }

        /// <summary>
        /// get or set the priority
        /// </summary>
        public string Priority
        {
            get { return priority; }
            set { priority = value; }
        }


        /// <summary>
        /// get or set receiver
        /// </summary>
        public string Receiver
        {
            get { return receiver; }
            set { receiver = value; }
        }


        /// <summary>
        /// get or set the sender
        /// </summary>
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }


        /// <summary>
        /// get or set the text of telex
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        /// <summary>
        /// get or set the supplementary information
        /// </summary>
        public string SI
        {
            get { return si; }
            set { si = value; }
        }
    }

    /// <summary>
    /// Used for UCM only.
    /// </summary>
    [Serializable]
    public class UCM_T : TelexBase
    {
        private string ecp = ""; // EMPTY CONTAINER PALLET

        /// <summary>
        /// get the content of UCM
        /// </summary>
        new public string Content
        {
            get
            {
                string ucm = this.Text + TrimNewLine(ecp);
                int idx = ucm.LastIndexOf("\r\n");
                if (idx <= 0) return "";

                if (idx + 2 <= 0) return "";
                string prefix = ucm.Substring(0, idx + 2);

                if (ucm.Length - idx - 2 < 0) return "";
                string tmpstr = ucm.Substring(idx + 2, ucm.Length - idx - 2);

                while (tmpstr.Length > 0)
                {
                    int idx1 = -1, i;
                    for (i = 0; i < 4; i++)
                    {
                        idx1 = tmpstr.IndexOf(".", idx1 + 1);
                        if (idx1 < 0)
                            break;
                    }

                    if (i == 4) // continue insert newline
                    {
                        prefix += (tmpstr.Substring(0, idx1) + "\r\n");
                        if (tmpstr.Length - idx1 == 0)
                            tmpstr = "";
                        else
                            tmpstr = tmpstr.Substring(idx1, tmpstr.Length - idx1);
                    }
                    else // goto the end of tmpstr
                    {
                        prefix += (tmpstr + "\r\n");
                        tmpstr = "";
                    }
                }

                if (this.SI == "")
                    return prefix;
                else
                    return (prefix + "SI\r\n" + this.SI);
            }
        }

        /// <summary>
        /// get empty container pallet
        /// </summary>
        public string ECP
        {
            get { return ecp; }
            set { ecp = value; }
        }

        /// <summary>
        /// to trim the format of UCM content with new lines
        /// </summary>
        /// <param name="_str">contents to be trimmed</param>
        /// <returns>str: contents after trimming</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks> 
        public string TrimNewLine(string _str)
        {
            int i;
            string str = _str;
            while ((i = str.IndexOf("\r\n")) >= 0)
                str = str.Substring(0, i) + str.Substring(i + 2, str.Length - i - 2);
            return str;
        }
    }

}