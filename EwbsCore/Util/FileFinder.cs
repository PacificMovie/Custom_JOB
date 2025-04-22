/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/

//*****************************************************************************
// Jeffery Lin | Ver. 02 | #BR17200	| 2017/03/30							                                   *
// .NET 1.1 轉 .NET 4.5                                                                              				                   *
//*****************************************************************************

using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace EwbsCore.Util
{
    /// <summary>
    /// Provides static methods for finding files
    /// </summary>
    public class FileFinder
    {
        /// <summary>get the path of file</summary>
        /// <param name="fileName">filename</param>
        /// <returns>path name of neighboring file</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static String GetFileName(String fileName)
        {
            return GetFileName("FDB", fileName);
        }

        /// <summary>
        /// Formulate and return a file name for a file in a 
        /// neighboring directory. 
        /// </summary>
        /// <param name="dirName">neighboring directory name</param>
        /// <param name="fileName">file name</param>
        /// <returns>path name of neighboring file</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        private static String GetFileName(String dirName, String fileName)
        {
            String path;
            // Can we find the file using the EWBS configuration variable?
            String ewbsBase = ConfigurationManager.AppSettings["BasePath"];
            //#BR17200-->
            if (ewbsBase != null)
            {
                path = Path.Combine(Path.Combine(ewbsBase, "FDB"), fileName);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            // How 'bout relative to where the bin files are?
            Assembly a = Assembly.GetAssembly(typeof(FileFinder));
            DirectoryInfo thisDir = Directory.GetParent(a.Location);

            //parent directory
            DirectoryInfo targetDir = thisDir;
            while (targetDir != null)
            {
                path = Path.Combine(
                    targetDir.FullName,
                    dirName + Path.DirectorySeparatorChar + fileName);
                if (File.Exists(path))
                {
                    return path;
                }

                //parent directory
                targetDir = Directory.GetParent(targetDir.FullName);
            }

            //No "FDB" in directory name
            targetDir = thisDir;
            while (targetDir != null)
            {
                path = Path.Combine(targetDir.FullName, fileName);
                if (File.Exists(path))
                {
                    return path;
                }
                //parent directory
                targetDir = Directory.GetParent(targetDir.FullName);
            }
            // Ok, how 'bout in the top-level directory?
            path = Path.Combine(Path.Combine(@"\EWBS", dirName), fileName);
            if (File.Exists(path))
            {
                return path;
            }
            return Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(dirName, fileName));
        }
    }
}