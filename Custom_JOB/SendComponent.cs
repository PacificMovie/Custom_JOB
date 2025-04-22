//send email/fax class
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace CustomJob
{
    class SendComponent
    {
        //----------------------------------------------------------------       
        /// <summary>
        /// Send Email  參數包括 1.收件者address 2.主旨 3.內文 4.選舉傳送Server
        /// [收件者] 或 [副本] 在一人以上時請在mail address中間加上逗號 
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"</param>           
        /// <param name="sSendTo">收件者mail address。  EX: "fiosun@evaair.com" </param>
        /// <param name="sSubject">主旨。  EX: "我是主旨，free format" </param>
        /// <param name="sBody">內文。  EX: "我是內文，free format"</param>        
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1</param>
        /// <returns>true or false</returns>
        public bool SendMail(string sSender, string sSendFrom, string sSendTo, string sSubject, string sBody, int iSendBy)
        {
            bool bSendSuccess = false;

            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                bool bExistReceiver = false;
                if (sSendTo != "" && sSendTo != null)
                {
                    Msg.To.Add(sSendTo);    // 收件者信箱
                    bExistReceiver = true;
                }
                else
                    bExistReceiver = false;

                if (bExistReceiver)
                {
                    // 信件主旨
                    Msg.Subject = sSubject;
                    // 將內文轉換為HTML
                    Msg.Body = sBody;
                    // 編碼方式
                    Msg.BodyEncoding = Encoding.UTF8;
                    // 指定電子郵件的格式
                    Msg.IsBodyHtml = false;

                    SmtpClient SmtpSrv = new SmtpClient();

                    // 設定e-mail的帳戶(預設)
                    if (sSendFrom == "" || sSendFrom == null)
                        sSendFrom = "Administrator";

                    // @evatou82.evaair.com    => 必須配合10.3.76.138    => EVATOU81
                    // @evatou82.evaair.com  => 必須配合10.3.76.138    => EVATOU82
                    if (iSendBy == 1)
                    {
                        Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                        SmtpSrv.Host = "10.3.76.138";
                    }
                    else
                    {
                        Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                        SmtpSrv.Host = "10.3.76.138";
                    }

                    SmtpSrv.Send(Msg);
                    bSendSuccess = true;
                }
                else
                    bSendSuccess = false;

            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }

            return bSendSuccess;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// Send Email  參數包括 1.收件者address 2.副本 3.主旨 4.內文 5.選擇傳送Server    
        /// [收件者] 或 [副本] 在一人以上時請在mail address中間加上逗號 
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"</param>           
        /// <param name="sSendTo">收件者信箱。 EX: "fiosun@evaair.com" </param>
        /// <param name="sCc">副本信箱。 EX: "fiosun@evaair.com" </param>
        /// <param name="sSubject">主旨。 EX: "我是主旨，free format" </param>
        /// <param name="sBody">內文。 EX: "我是內文，free format"</param>        
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1</param>
        /// <returns>true or false</returns>
        public bool SendMail(string sSender, string sSendFrom, string sSendTo, string sCc, string sSubject, string sBody, int iSendBy)
        {
            bool bSendSuccess = false;

            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                if (sSendTo != "" && sSendTo != null)
                    Msg.To.Add(sSendTo);    // 收件者信箱

                if (sCc != "" && sCc != null)
                    Msg.CC.Add(sCc);    // 副本信箱

                // 信件主旨
                Msg.Subject = sSubject;
                // 將內文轉換為HTML
                Msg.Body = sBody;
                // 編碼方式
                Msg.BodyEncoding = Encoding.UTF8;
                // 指定電子郵件的格式
                Msg.IsBodyHtml = false;

                SmtpClient SmtpSrv = new SmtpClient();

                // 設定e-mail的帳戶(預設)
                if (sSendFrom == "" || sSendFrom == null)
                    sSendFrom = "Administrator";

                // @evatou82.evaair.com    => 必須配合10.3.76.138    => EVATOU81
                // @evatou82.evaair.com  => 必須配合10.3.76.138    => EVATOU82
                if (iSendBy == 1)
                {

                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }
                else
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }

                SmtpSrv.Send(Msg);
                bSendSuccess = true;
            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }

            return bSendSuccess;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// Send Email  參數包括 1.收件者 2.副本 3.密件副本 4.主旨 5.內文 6.選擇傳送Server  
        /// [收件者] 或 [副本] 在一人以上時請在mail address中間加上逗號 
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"</param>           
        /// <param name="sSendTo">收件者mail address。  EX: "fiosun@evaair.com" </param>
        /// <param name="sCc">副本address。  EX: "fiosun@evaair.com" </param>
        /// <param name="sBcc">密件副本address。  EX: "phoebeliao@evaair.com" </param>
        /// <param name="sSubject">主旨。  EX: "我是主旨，free format" </param>
        /// <param name="sBody">內文。  EX: "我是內文，free format"</param>        
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1</param>
        /// <returns>true false</returns>
        public bool SendMail(string sSender, string sSendFrom, string sSendTo, string sCc, string sBcc, string sSubject, string sBody, int iSendBy)
        {
            bool bSendSuccess = false;

            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                if (sSendTo != "" && sSendTo != null)
                    Msg.To.Add(sSendTo);    // 收件者信箱

                if (sCc != "" && sCc != null)
                    Msg.CC.Add(sCc);    // 副本信箱

                if (sBcc != "" && sBcc != null)
                    Msg.Bcc.Add(sBcc);      // 密件副本信箱

                // 信件主旨
                Msg.Subject = sSubject;
                // 將內文轉換為HTML
                Msg.Body = sBody;
                // 編碼方式
                Msg.BodyEncoding = Encoding.UTF8;
                // 指定電子郵件的格式
                Msg.IsBodyHtml = false;

                SmtpClient SmtpSrv = new SmtpClient();

                // 設定e-mail的帳戶(預設)
                if (sSendFrom == "" || sSendFrom == null)
                    sSendFrom = "Administrator";

                // @evatou82.evaair.com    => 必須配合10.3.76.138    => EVATOU81
                // @evatou82.evaair.com  => 必須配合10.3.76.138    => EVATOU82
                if (iSendBy == 1)
                {

                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }
                else
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }

                SmtpSrv.Send(Msg);
                bSendSuccess = true;
            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }

            return bSendSuccess;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// Send Email  參數包括 1.收件者 2.副本 3.密件副本 4.主旨 5.內文 6.附件 7.選擇傳送Server  
        /// [收件者] 或 [副本] 在一人以上時請在mail address中間加上逗號 
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"</param>           
        /// <param name="sSendTo">收件者mail address。  EX: "fiosun@evaair.com" </param>
        /// <param name="sCc">副本address。  EX: "fiosun@evaair.com" </param>
        /// <param name="sBcc">密件副本address。  EX: "phoebeliao@evaair.com" </param>
        /// <param name="sSubject">主旨。  EX: "我是主旨，free format" </param>
        /// <param name="sBody">內文。  EX: "我是內文，free format"</param>
        /// <param name="sAttach">附件。　EX: "C:\TEST.txt"</param>
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1</param>
        /// <returns>true false</returns>
        public bool SendMail(string sSender, string sSendFrom, string sSendTo, string sCc, string sBcc, string sSubject, string sBody, string sAttach, int iSendBy)
        {
            bool bSendSuccess = false;

            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                if (sSendTo != "" && sSendTo != null)
                    Msg.To.Add(sSendTo);    // 收件者信箱

                if (sCc != "" && sCc != null)
                    Msg.CC.Add(sCc);    // 副本信箱

                if (sBcc != "" && sBcc != null)
                    Msg.Bcc.Add(sBcc);      // 密件副本信箱

                // 信件主旨
                Msg.Subject = sSubject;
                // 將內文轉換為HTML
                Msg.Body = sBody;
                // 編碼方式
                Msg.BodyEncoding = Encoding.UTF8;
                // 指定電子郵件的格式
                Msg.IsBodyHtml = false;

                // 電子郵件的附件
                if (sAttach != "")
                {
                    char[] delim = new char[] { ',' };
                    foreach (string sSubstr in sAttach.Split(delim))
                    {
                        Attachment myAttachment = new Attachment(sSubstr);
                        Msg.Attachments.Add(myAttachment);
                    }
                }

                SmtpClient SmtpSrv = new SmtpClient();

                // 設定e-mail的帳戶(預設)
                if (sSendFrom == "" || sSendFrom == null)
                    sSendFrom = "Administrator";

                // @evatou82.evaair.com    => 必須配合10.3.76.138    => EVATOU81
                // @evatou82.evaair.com  => 必須配合10.3.76.138    => EVATOU82
                if (iSendBy == 1)
                {

                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }
                else
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }

                SmtpSrv.Send(Msg);
                bSendSuccess = true;
            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }

            return bSendSuccess;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// Send Email 參數包括 1.收件者address 2.主旨 3.內文(以HTML呈現) 4.附件 5.選傳送的Server
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"</param>           
        /// <param name="sSendTo">收件者mail address  EX: "phoebeliao@evaair.com"。 </param>
        /// <param name="sSubject">主旨。EX: "我是主旨，free format"。 </param>
        /// <param name="sBody">內文。EX: "我是內文，free format"。</param>                
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1</param>
        /// <returns>true or false</returns>
        public bool SendMailHTML(string sSender, string sSendFrom, string sSendTo, string sSubject, string sBody, int iSendBy)
        {
            bool bSendSuccess = false;

            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                bool bExistReceiver = false;
                if (sSendTo != "" && sSendTo != null)
                {
                    Msg.To.Add(sSendTo);    // 收件者信箱
                    bExistReceiver = true;
                }
                else
                    bExistReceiver = false;

                if (bExistReceiver)
                {
                    // 信件主旨
                    Msg.Subject = sSubject;
                    // 將內文轉換為HTML
                    Msg.Body = this.ConvertStrIntoHTML(sBody);
                    // 編碼方式
                    Msg.BodyEncoding = Encoding.UTF8;
                    // 指定電子郵件的格式
                    Msg.IsBodyHtml = true;

                    SmtpClient SmtpSrv = new SmtpClient();

                    // 設定e-mail的帳戶(預設)
                    if (sSendFrom == "" || sSendFrom == null)
                        sSendFrom = "Administrator";

                    // @evatou82.evaair.com   => 必須配合10.3.76.138    => EVATOU81
                    // @evatou82.evaair.com  => 必須配合10.3.76.138    => EVATOU82
                    if (iSendBy == 1)
                    {
                        Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                        SmtpSrv.Host = "10.3.76.138";
                    }
                    else if (iSendBy == 2)
                    {
                        Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                        SmtpSrv.Host = "10.3.76.138";
                    }
                    else
                    {
                        Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                        SmtpSrv.Host = "10.3.76.138";
                    }

                    SmtpSrv.Send(Msg);
                    bSendSuccess = true;
                }
                else
                    bSendSuccess = false;

            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }

            return bSendSuccess;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// Send Email 參數包括 1.收件者address 2.副件收件人 3.密本副件收件人 4.主旨 5.內文(以HTML呈現) 6.選傳送的Server        
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"</param>           
        /// <param name="sSendTo">收件者mail地址。 EX: "phoebeliao@evaair.com"。 
        /// [收件者]或[副本]在一人以上時請在mail地址中間加上逗號 </param>       
        /// <param name="sCc">副本地址。EX: "fiosun@evaair.com" </param>        
        /// <param name="sSubject">主旨。EX: "我是主旨，free format"。 </param>
        /// <param name="sBody">內文。EX: "我是內文，free format"。</param>                        
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1</param>
        /// <returns>true or false</returns>
        public bool SendMailHTML(string sSender, string sSendFrom, string sSendTo, string sCc, string sSubject, string sBody, int iSendBy)
        {
            bool bSendSuccess = false;

            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                if (sSendTo != "" && sSendTo != null)
                    Msg.To.Add(sSendTo);    // 收件者信箱

                if (sCc != "" && sCc != null)
                    Msg.CC.Add(sCc);    // 副本信箱

                // 信件主旨
                Msg.Subject = sSubject;
                // 將內文轉換為HTML
                Msg.Body = this.ConvertStrIntoHTML(sBody);
                // 編碼方式
                Msg.BodyEncoding = Encoding.UTF8;
                // 指定電子郵件的格式
                Msg.IsBodyHtml = true;

                SmtpClient SmtpSrv = new SmtpClient();

                // 設定e-mail的帳戶(預設)
                if (sSendFrom == "" || sSendFrom == null)
                    sSendFrom = "Administrator";

                // @evatou82.evaair.com    => 必須配合10.3.76.138    => EVATOU81
                // @evatou82.evaair.com  => 必須配合10.3.76.138    => EVATOU82
                if (iSendBy == 1)
                {

                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }
                else
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }

                SmtpSrv.Send(Msg);
                bSendSuccess = true;
            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }

            return bSendSuccess;
        }
        //----------------------------------------------------------------

        /// <summary>
        /// Send Email 參數包括 1.收件者address 2.副件收件人 3.密本副件收件人 4.主旨 5.內文(以HTML呈現) 6.選傳送的Server        
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"</param>   
        /// <param name="sSendTo">收件者mail地址。 EX: "phoebeliao@evaair.com"。 
        /// [收件者]或[副本]在一人以上時請在mail地址中間加上逗號 </param>       
        /// <param name="sCc">副本地址。EX: "fiosun@evaair.com" </param>
        /// <param name="sBcc">密件副本地址。EX: "fiosun@evaair.com" </param>
        /// <param name="sSubject">主旨。EX: "我是主旨，free format"。 </param>
        /// <param name="sBody">內文。EX: "我是內文，free format"。</param>                        
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1</param>
        /// <returns>true or false</returns>
        public bool SendMailHTML(string sSender, string sSendFrom, string sSendTo, string sCc, string sBcc, string sSubject, string sBody, int iSendBy)
        {
            bool bSendSuccess = false;
            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                if (sSendTo != "" && sSendTo != null)
                    Msg.To.Add(sSendTo);    // 收件者信箱

                if (sCc != "" && sCc != null)
                    Msg.CC.Add(sCc);    // 副本信箱

                if (sBcc != "" && sBcc != null)
                    Msg.Bcc.Add(sBcc);      // 密件副本信箱

                // 信件主旨
                Msg.Subject = sSubject;
                // 將內文轉換為HTML
                Msg.Body = this.ConvertStrIntoHTML(sBody);
                // 編碼方式
                Msg.BodyEncoding = Encoding.UTF8;
                // 指定電子郵件的格式
                Msg.IsBodyHtml = true;

                SmtpClient SmtpSrv = new SmtpClient();

                // 設定e-mail的帳戶(預設)
                if (sSendFrom == "" || sSendFrom == null)
                    sSendFrom = "Administrator";

                // @evatou82.evaair.com    => 必須配合10.3.76.138    => EVATOU81
                // @evatou82.evaair.com  => 必須配合10.3.76.138    => EVATOU82                
                if (iSendBy == 1)
                {

                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }
                else
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }

                SmtpSrv.Send(Msg);
                bSendSuccess = true;
            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }
            return bSendSuccess;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// Send Email 參數包括 1.寄件者名稱 2.寄件者的e-mail帳號 3.收件者address 4.副件收件人 5.密本副件收件人 6.主旨 7.內文(以HTML呈現) 8.附件 9.選傳送的Server        
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"</param>   
        /// <param name="sSendTo">收件者mail地址。 EX: "fiosun@evaair.com"。 
        /// [收件者]或[副本]在一人以上時請在mail地址中間加上逗號 </param>       
        /// <param name="sCc">副本地址。EX: "fiosun@evaair.com" </param>
        /// <param name="sBcc">密件副本地址。EX: "fiosun@evaair.com" </param>
        /// <param name="sSubject">主旨。EX: "我是主旨，free format"。 </param>
        /// <param name="sBody">內文。EX: "我是內文，free format"。</param>                
        /// <param name="sAttach">附件。 EX: "C:\TEST.txt"。 多份附件時，以 "逗號" 分隔。</param>    
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1</param>
        /// <returns>true or false</returns>
        public bool SendMailHTML(string sSender, string sSendFrom, string sSendTo, string sCc, string sBcc, string sSubject, string sBody, string sAttach, int iSendBy)
        {
            bool bSendSuccess = false;
            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                if (sSendTo != "" && sSendTo != null)
                    Msg.To.Add(sSendTo);    // 收件者信箱

                if (sCc != "" && sCc != null)
                    Msg.CC.Add(sCc);    // 副本信箱

                if (sBcc != "" && sBcc != null)
                    Msg.Bcc.Add(sBcc);      // 密件副本信箱

                // 信件主旨
                Msg.Subject = sSubject;
                // 將內文轉換為HTML
                Msg.Body = this.ConvertStrIntoHTML(sBody);
                // 編碼方式
                Msg.BodyEncoding = Encoding.UTF8;
                // 指定電子郵件的格式
                Msg.IsBodyHtml = true;

                // 電子郵件的附件
                if (sAttach != "")
                {
                    char[] delim = new char[] { ',' };
                    foreach (string sSubstr in sAttach.Split(delim))
                    {
                        Attachment myAttachment = new Attachment(sSubstr);
                        Msg.Attachments.Add(myAttachment);
                    }
                }

                SmtpClient SmtpSrv = new SmtpClient();

                // 設定e-mail的帳戶(預設)
                if (sSendFrom == "" || sSendFrom == null)
                    sSendFrom = "Administrator";

                // @evatou82.evaair.com    => 必須配合10.3.76.138    => EVATOU81
                // @evatou82.evaair.com  => 必須配合10.3.76.138    => EVATOU82

                if (iSendBy == 1)
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }
                else
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    SmtpSrv.Host = "10.3.76.138";
                }

                SmtpSrv.Send(Msg);
                bSendSuccess = true;
            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }
            return bSendSuccess;
        }
        //----------------------------------------------------------------

        /// <summary>
        /// SendFax 參數包括 1.傳真號碼 2.主旨 3.內文 4.選舉傳送Server
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"。
        /// 系統接受的帳戶名稱有 -> administrator/b7pipp/eservice/eticket/irisfax
        /// </param>   
        /// <param name="sFaxNumber">傳真號碼。 EX: "3510009"</param>
        /// <param name="sSubject">主旨。 EX: "我是主旨，free format"</param>
        /// <param name="sBody">內文。 EX: "我是內文，free format"</param>
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態。 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1 </param>
        /// <returns>true or false</returns>
        public bool SendFax(string sSender, string sSendFrom, string sFaxNumber, string sSubject, string sBody, int iSendBy)
        {
            bool bSendSuccess = false;
            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                Msg.Subject = sSubject;    // 傳真主旨

                SmtpClient SmtpSrv = new SmtpClient();

                // 設定e-mail的帳戶(預設)
                if (sSendFrom == "" || sSendFrom == null)
                    sSendFrom = "Administrator";

                // @evatou82.evaair.com.tw    => 必須配合10.3.76.138    => EVATOU81
                // @evatou82.evaair.com.tw  => 必須配合10.3.76.138    => EVATOU82
                if (iSendBy == 1)
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    Msg.To.Add(new MailAddress(sFaxNumber + "@evatou82.evaair.com.tw"));
                    //Msg.To = new MailAddress(sFaxNumber + "@evatou82.evaair.com.tw");
                    SmtpSrv.Host = "10.3.76.138";
                }
                else
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    Msg.To.Add(new MailAddress(sFaxNumber + "@evatou82.evaair.com.tw"));
                    //Msg.To = new MailAddress(sFaxNumber + "@evatou82.evaair.com.tw");
                    SmtpSrv.Host = "10.3.76.138";
                }

                Msg.Body = sBody;
                Msg.BodyEncoding = Encoding.UTF8;
                //byte[] byt = System.Text.Encoding.GetEncoding("big5").GetBytes(Body);//body
                //Msg.Body = System.Text.Encoding.GetEncoding("big5").GetString(byt);

                SmtpSrv.Send(Msg);
                bSendSuccess = true;
            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }
            return bSendSuccess;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// SendFax 參數包括 1.傳真號碼 2.主旨 3.內文 4.選取傳送Server
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"。
        /// 系統接受的帳戶名稱有 -> administrator/b7pipp/eservice/eticket/irisfax
        /// </param>   
        /// <param name="sFaxNumber">傳真號碼。 EX: "3510009"</param>
        /// <param name="sSubject">主旨。 EX: "我是主旨，free format"</param>
        /// <param name="sBody">內文。 EX: "我是內文，free format"</param>
        /// <param name="sAttach">附件  EX: C:\test.txt</param>
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態。 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1 </param>
        /// <returns>true or false</returns>
        public bool SendFax(string sSender, string sSendFrom, string sFaxNumber, string sSubject, string sBody, string sAttach, int iSendBy)
        {
            bool bSendSuccess = false;
            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                Msg.Subject = sSubject;    // 傳真主旨

                // 附件
                if (sAttach != "")
                {
                    char[] delim = new char[] { ',' };
                    foreach (string sSubstr in sAttach.Split(delim))
                    {
                        Attachment myAttachment = new Attachment(sSubstr);
                        Msg.Attachments.Add(myAttachment);
                    }
                }

                SmtpClient SmtpSrv = new SmtpClient();

                // 設定e-mail的帳戶(預設)
                if (sSendFrom == "" || sSendFrom == null)
                    sSendFrom = "Administrator";

                // @evatou82.evaair.com.tw    => 必須配合10.3.76.138    => EVATOU81
                // @evatou82.evaair.com.tw  => 必須配合10.3.76.138    => EVATOU82
                if (iSendBy == 1)
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    Msg.To.Add(new MailAddress(sFaxNumber + "@evatou82.evaair.com.tw"));
                    SmtpSrv.Host = "10.3.76.138";
                }
                else
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    Msg.To.Add(new MailAddress(sFaxNumber + "@evatou82.evaair.com.tw"));
                    SmtpSrv.Host = "10.3.76.138";
                }

                Msg.Body = sBody;
                Msg.BodyEncoding = Encoding.UTF8;

                SmtpSrv.Send(Msg);
                bSendSuccess = true;
            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }
            return bSendSuccess;
        }
        //----------------------------------------------------------------

         /// <summary>
        /// SendFax 參數包括 1.傳真號碼 2.主旨 3.內文 4.選取傳送Server
        /// </summary>
        /// <param name="sSender">寄件者名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的FIOSUN"。</param>    
        /// <param name="sSendFrom">mail的帳戶名稱。 EX: "FIOSUN [Administrator@evatou82.evaair.com.tw]中的Administrator"。
        /// 系統接受的帳戶名稱有 -> administrator/b7pipp/eservice/eticket/irisfax
        /// </param>   
        /// <param name="sFaxNumber">傳真號碼。 EX: "3510009"</param>
        /// [收件者]或[副本]在一人以上時請在mail地址中間加上逗號 </param>     
        /// <param name="sCc">副本。 這裡是 email address</param>
        /// <param name="sBcc">密本。這裡是 email address</param>
        /// <param name="sSubject">主旨。 EX: "我是主旨，free format"</param>
        /// <param name="sBody">內文。 EX: "我是內文，free format"</param>
        /// <param name="sAttach">附件  EX: C:\test.txt</param>
        /// <param name="iSendBy">選擇用EVATOU81 or EVATOU82 傳送，整數型態。 
        /// EX:若使用EVATOU81--> 請帶入 0  若使用EVATOU82--> 請帶入 1 </param>
        /// <returns>true or false</returns>
        public bool SendFax(string sSender, string sSendFrom, string sFaxNumber, string sCc, string sBcc, string sSubject, string sBody, string sAttach, int iSendBy)
        {
            bool bSendSuccess = false;
            Encoding encoding = Encoding.BigEndianUnicode;
            try
            {
                MailMessage Msg = new MailMessage();

                Msg.Subject = sSubject;    // 傳真主旨

                // 附件
                if (sAttach != "")
                {
                    char[] delim = new char[] { ',' };
                    foreach (string sSubstr in sAttach.Split(delim))
                    {
                        Attachment myAttachment = new Attachment(sSubstr);
                        Msg.Attachments.Add(myAttachment);
                    }
                }

                SmtpClient SmtpSrv = new SmtpClient();

                // 設定e-mail的帳戶(預設)
                if (sSendFrom == "" || sSendFrom == null)
                    sSendFrom = "Administrator";

                // @evatou82.evaair.com.tw    => 必須配合10.3.76.138    => EVATOU81
                // @evatou82.evaair.com.tw  => 必須配合10.3.76.138    => EVATOU82
                if (iSendBy == 1)
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    Msg.To.Add(new MailAddress(sFaxNumber + "@evatou82.evaair.com.tw"));
                    SmtpSrv.Host = "10.3.76.138";
                }
                else
                {
                    Msg.From = new MailAddress(sSendFrom + "@evatou82.evaair.com", sSender);
                    Msg.To.Add(new MailAddress(sFaxNumber + "@evatou82.evaair.com.tw"));
                    SmtpSrv.Host = "10.3.76.138";
                }

                if (sCc != "")
                    Msg.CC.Add(sCc);    // 副本信箱

                if (sBcc != "")
                    Msg.Bcc.Add(sBcc);  // 密件副本信箱
                             
                Msg.Body = sBody;
                Msg.BodyEncoding = Encoding.UTF8;

                SmtpSrv.Send(Msg);
                bSendSuccess = true;
            }
            catch (Exception ex)
            {
                bSendSuccess = false;
                throw (ex);
            }
            return bSendSuccess;
        }
        //----------------------------------------------------------------

        /// <summary>
        /// 驗證Email地址 ChkEmailFmt參數包括 1.E-mail Address
        /// </summary>
        /// <param name="sMailAddr">電子郵件  EX: "phoebeliao@evaair.com" </param>
        /// <returns>true or false</returns>
        public bool ChkEmailFmt(string sMailAddr)
        {
            bool bIsChkEmail = false;
            try
            {
                string sRegExp = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                if (Regex.IsMatch(sMailAddr, sRegExp))
                    bIsChkEmail = true;
                else
                    bIsChkEmail = false;
            }
            catch (Exception ex)
            {
                //this.FailReason = "###Exception(ChkEmailFmt)###-->" + ex.Message.ToString();
                throw ex;
            }
            return bIsChkEmail;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// 將從HOST擷取的e-mail中的"//"字元轉換為"@"; "/-"字元轉換為"_"; 去除空白字元
        /// ReformatEmail參數包括 1.要轉換格式的email，尤其是從HOST擷取的e-mail
        /// </summary>
        /// <param name="sStr">E-mail  EX: "PhoebeLiao//eveair.com" or "PhoebeLiao/-123//evaair.com" </param>
        /// <returns>string</returns>
        public string ReformatEmail(string sStr)
        {
            string sRtnValue = "";
            if (sStr.Length > 0)
            {
                sRtnValue = sStr.Replace("//", "@").Replace("/-", "_").Replace(" ", "");

                // 檢查e-mail的格式是否正確
                if (!this.ChkEmailFmt(sRtnValue))
                    sRtnValue = "";
            }
            else
                sRtnValue = "";
            return sRtnValue;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// ReformatFax參數包括 1.要轉換格式的傳真號碼
        /// </summary>
        /// <param name="sStr">傳真號碼  EX: "88635100009" or "88622519892 </param>
        /// <returns>string</returns>
        // 輸入 : TPE 886 2 28747565
        public string ReformatFax(string sStr)
        {
            const string sDigits = "0123456789";
            string sRtnValue = "";
            bool bInterCall = false;   //Internation call flag
            bool bTaoyuanCall = false; //Taoyuan city call falg
            string sTmpStr="";

            if (sStr.Length > 0)
            {
                // 
                if (sStr.IndexOf("TPE") != -1)
                {
                    bInterCall = false;  
                }
                for (int i = 0; i < sStr.Length; i++)
                {
                    //if (sStr.Substring(i, 1) >= "0" && sStr.Substring(i, 1) <= "9")
                    if(sDigits.IndexOf(sStr.Substring(i,1)) != -1)
                        sTmpStr += sStr.Substring(i, 1);
                }

                if (sStr.Substring(0, 5) == "88632" || sStr.Substring(0, 5) == "88633"
                    || sStr.Substring(0, 5) == "88634")
                {
                    // 將區域碼3去除
                    sStr = sStr.Remove(3, 1);
                    bTaoyuanCall = true;
                    bInterCall = false;
                }

                if (sStr.Substring(0, 3) == "886")   // 886 2 2351 0001
                {
                    sStr = sStr.Remove(0, 3);    // 2 2351 0001

                    // 判斷是否為桃園區域的傳真號碼; 若非,則加0 
                    if (!bTaoyuanCall)
                        sStr = "0" + sStr;     // 02 2351 0001
                    bInterCall = false;
                }

                if (bInterCall)
                    sStr = "+" + sStr;
            }
            else
                sStr = "";

            sRtnValue = sStr;
            return sRtnValue;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// 轉換為 HTML 的換行 br 及 space 轉換為 HTML 的空格符號
        /// ConvertHtmlSpace參數包括 1.要轉換的文字
        /// </summary>
        private string ConvertHtmlSpace(string sStr)
        {
            string sRtnValue = "";
            //joe 空格字串轉換會導致無法使用style修改屬性，故註解
            //sRtnValue = sStr.Replace("\r\n", "<br>").Replace(" ", "&nbsp;");
            sRtnValue = sStr.Replace("\r\n", "<br>").Replace("\r", "<br>");
            return sRtnValue;
        }

        //----------------------------------------------------------------

        /// <summary>
        /// 將內文轉換為HTML的格式。
        /// ConvertStrIntoHTML參數包括。 1.內文        
        /// </summary>
        /// <param name="sBody">內文。  EX: "我是主旨，free format" </param>                
        /// <returns>string</returns>
        private string ConvertStrIntoHTML(string sBody)
        {
            StringBuilder sbSendBody = new StringBuilder();

            sbSendBody.Append("<html>");
            sbSendBody.Append("<head>");
            //sbSendBody.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=big5\">");
            sbSendBody.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">");
            sbSendBody.Append("<title></title>");
            sbSendBody.Append("</head>");
            sbSendBody.Append("<body>");
            //joe 自行編譯HTML內文，故註解
            //sbSendBody.Append("<font face=細明體 size=2>");
            //sbSendBody.Append("<font face=SimHei size-2>");
            sbSendBody.Append(this.ConvertHtmlSpace(sBody));
            //sbSendBody.Append("</font>");
            //sbSendBody.Append(sBody);
            sbSendBody.Append("</body>");
            sbSendBody.Append("</html>");

            return sbSendBody.ToString();
        }

        //----------------------------------------------------------------
    }
}
