using System;
using System.Collections;
using System.IO;
//*****************************************************************************
//*THOMAS| Ver. 07 | #BR17164 | 2017/8/31                                                                      *
//*---------------------------------------------------------------------------*
//* 物件關閉後 Dispose 標記由GC回收     
//****************************************************************************
namespace EwbsCore.Util
{
    public static class ExceptionLog
    {
        public static void WriteLog(string ErrorMsg, string ErrorStack, string SaveFolder="ExceptionLog")
        {
            string folderPath = Directory.GetCurrentDirectory();

            if (!Directory.Exists(folderPath + "\\" + SaveFolder))
            {
                Directory.CreateDirectory(folderPath + "\\" + SaveFolder);
            }

            StreamWriter file = new StreamWriter(folderPath + "\\" + SaveFolder + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss-ffffff") + ".txt");
            file.WriteLine("TimeStamp: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
            file.WriteLine("----------------------------------------------------------------------------");
            file.WriteLine("Error Massage:" + ErrorMsg);
            file.WriteLine("----------------------------------------------------------------------------");
            file.WriteLine("Stack Trace: ");
            file.WriteLine(ErrorStack);
            file.Close();
            file = null;
        }
        public static void WriteLog(Exception e, string SaveFolder = "ExceptionLog") 
        {
            string folderPath = Directory.GetCurrentDirectory();

            if (!Directory.Exists(folderPath + "\\" + SaveFolder))
            {
                Directory.CreateDirectory(folderPath + "\\" + SaveFolder);
            }

            StreamWriter file = new StreamWriter(folderPath + "\\" + SaveFolder + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss-ffffff") + ".txt");
            file.WriteLine("TimeStamp: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
            file.WriteLine("----------------------------------------------------------------------------");
            file.WriteLine("Error Massage:" + e.Message.ToString());
            file.WriteLine("----------------------------------------------------------------------------");
            file.WriteLine("Stack Trace: ");
            file.WriteLine(e.StackTrace);
            file.Close();
            file = null;
        }
    }
}
