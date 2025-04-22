/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/
//*****************************************************************************
//*Thomas| Ver. 01 | #BR17164 | 2017/8/31                                                                      *
//*---------------------------------------------------------------------------*
//* 物件關閉後 Dispose 標記由GC回收     
//******************************************************************************
// for Directory
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Compression;
using EwbsCore.Util;

namespace ComInterface
{
    /// <summary>
    /// Utilities used for functinality relative to directory and file.
    /// </summary>
    public class DirService
    {
        #region DirService Utility

        /// <summary>create directroy </summary>
        /// <param name="path"> directroy path</param>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static void CreateDir(string path)
        { // if path exists do nothing. Otherwise, create it
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path))
                {
                    // Create the directory.
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(EWBSCoreWarnings.ComCreateDirFailed_1, e.ToString());
            }
            finally
            {
            }
        }


        /// <summary>Convert file into MD5 format </summary>
        /// <param name="fname"> filename </param>
        /// <returns>byte []: MD5 format </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static byte[] GetMD5(string fname) // small size file(FDB files)
        {
            byte[] rdata = null;
            try
            {
                byte[] data = DirService.FileContent(fname);
                MD5 md5 = new MD5CryptoServiceProvider();
                if (data != null && data.Length > 0)
                    rdata = md5.ComputeHash(data);
            }
            catch (Exception)
            {
            }
            return rdata;
        }


        /// <summary>Convert file into MD5 format </summary>
        /// <param name="fname"> filename </param>
        /// <returns>ArrayList: MD5 format </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static ArrayList GetMD5s(string fname) // large size file(.exe/.dll)
        {
            ArrayList rar = new ArrayList();
            try
            {
                FileStream stream = new FileStream(fname, FileMode.Open);
                int size = Convert.ToInt32(stream.Length) / 10;
                byte[] data = new byte[size];
                int ri;
                MD5 md5 = new MD5CryptoServiceProvider();
                while ((ri = stream.Read(data, 0, size)) > 0)
                {
                    rar.Add(md5.ComputeHash(data));
                }
                stream.Close();
                stream.Dispose(); //#BR17164 THOMAS標記由GC釋放
                stream = null; //#BR17164  THOMAS標記由GC釋放
                data = null;//#BR17164 THOMAS標記由GC釋放
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            }
            catch (Exception)
            {
            }
            return rar;
        }


        /// <summary>read file content</summary>
        /// <param name="filename"> filename </param>
        /// <returns>byte []: file content</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static byte[] FileContent(string filename)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                // using (StreamReader sr = new StreamReader("BR74E.xml"))
                FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                stream.Close();
                stream.Dispose(); //#BR17164 THOMAS標記由GC釋放
                stream = null;//#BR17164 THOMAS標記由GC釋放
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
                return data;

            }
            catch (Exception e)
            {
                Console.WriteLine(EWBSCoreWarnings.ComFileCantBeRead);
                Console.WriteLine(e.Message);
                return null;
            }
        }


        /// <summary>Store data into file</summary>
        /// <param name="data"> data </param>
        /// <param name="path">path</param>
        /// <returns>string:display the warning message, if not success</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static string WriteToFile(byte[] data, string path)
        {
            string rtn = "";
            try
            {
                FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                stream.Write(data, 0, data.Length);
                stream.Close();
                stream.Dispose(); //#BR17164 THOMAS標記由GC釋放
                stream = null;
                System.GC.Collect();
            }
            catch (Exception e)
            {
                Console.WriteLine(EWBSCoreWarnings.ComFileCantBeWritten);
                Console.WriteLine(e.Message);
                rtn = (EWBSCoreWarnings.ComFileCantBeWritten + path);
            }
            return rtn;
        }


        /// <summary>Save to file</summary>
        /// <param name="path">path</param>
        /// <param name="obj"> object </param>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static void Serialize(string path, object obj)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(path, FileMode.Create);

                // use the CLR binary formatter
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                //binaryFormatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
                // serialize to disk
                binaryFormatter.Serialize(fileStream, obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(EWBSCoreWarnings.ComDirServiceSerializeCatchExp + e.Message + "\r\n" + e.StackTrace);
            }

            if (fileStream != null)
            {
                fileStream.Close();
                fileStream.Dispose(); //#BR17164 THOMAS標記由GC釋放
                fileStream = null;
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            }
        }

        //For Test
        public static string Serialize1(string path, object obj)
        {
            string rtn = "";
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(path, FileMode.Create);

                // use the CLR binary formatter
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                //binaryFormatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
                // serialize to disk
                binaryFormatter.Serialize(fileStream, obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(EWBSCoreWarnings.ComDirServiceSerializeCatchExp + e.Message + "\r\n" + e.StackTrace);
                rtn = e.Message;

            }

            if (fileStream != null)
            {
                fileStream.Close();
                fileStream.Dispose(); //#BR17164 THOMAS標記由GC釋放
                fileStream = null;
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            }
            return rtn;
        }

        /// <summary>read file content</summary>
        /// <param name="path">path</param>
        /// <returns>object object </returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static object Deserialize(string path)
        {
            object obj = null;
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

                // use the CLR binary formatter
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                obj = binaryFormatter.Deserialize(fileStream);
            }
            catch (Exception e)
            {
                Console.WriteLine(EWBSCoreWarnings.ComDirServiceSerializeCatchExp + e.Message + "\r\n" + e.StackTrace);
            }

            if (fileStream != null)
            {
                fileStream.Close();
                fileStream.Dispose(); //#BR17164 THOMAS標記由GC釋放
                fileStream = null;
                System.GC.Collect(); //#BR17164 THOMAS GC強制回收
            }
            return obj;
        }


        /// <summary>Convert string into byte[]</summary>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static byte[] ToBytes(string str)
        {
            IEnumerator strEnum;
            int i = 0;
            byte[] data = new byte[str.Length];
            strEnum = str.GetEnumerator();
            while (strEnum.MoveNext())
                data[i++] = Convert.ToByte(strEnum.Current);
            return (data);
        }


        /// <summary>Convert byte[] into string</summary>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static string ToHexString(byte[] byteArray)
        {
            string temp = "";
            for (int i = 0; i < byteArray.Length; i++)
            {
                temp += byteArray[i].ToString("D3");
            }
            return temp;
        }

        /// <summary>save FDB</summary>
        /// <param name="fsig">Signature</param>
        /// <param name="path">path</param>
        /// <returns>string:display the warning message, if not success</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static string SaveFDB(FileSignature fsig, string path)
        { // Save modified FDB file at path
            string rtn = "";
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();

                byte[] data = AdaptiveHuffmanProvider.Decompress(fsig.Data);
                Signature sig = new Signature(data.Length, md5.ComputeHash(data));
                if (!fsig.IsSameSig(sig))
                    rtn = EWBSCoreWarnings.ComReceiveWrongSignature;

                if (rtn == "") 
                    rtn = WriteToFile(data, path);

                if (rtn == "") 
                    Serialize(path.Substring(0, path.IndexOf(".")) + ".sig", sig);
                else
                    rtn = EWBSCoreWarnings.ComIOErrorFile + path;

                return rtn;
            }
            catch (Exception e)
            {
                return (string.Format(EWBSCoreWarnings.ComFileCantBeWritten_2, path, e.Message));
            }
        }


        /// <summary> get path of Signature</summary>
        /// <param name="path">path</param>
        /// <returns>string: Signature之path</returns>
        /// <remarks>
        /// Modified date : 
        /// Modified by :
        /// Modified Reason : 
        /// </remarks>  
        public static string SigPath(string path)
        {
            return path.Substring(0, path.LastIndexOf(".")) + ".sig";
        }

        #endregion
    }

}