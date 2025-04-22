/*
”The Eva Airways and the Institute for the Information Industry (the owners hereafter) equally possess
the intellectual property of this software source codes document herein. The owners may have patents,
patent applications, trademarks, copyrights or other intellectual property rights covering the subject matters
in this document. Except as expressly provided in any written license agreement from the owners, the furnishing of this document does not give
any one any license to the aforementioned intellectual property.
The names of actual companies and products mentioned herein may be the trademarks of their respective owners.”
*/


using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EwbsCore.Util
{
    public class EwbsRC2
    {
        private static byte[] key = { 0x1d, 0x2c, 0x3a, 0x44, 0x51, 0x63, 0x07, 0x8a, 0x9f, 0x16, 0x15, 0x31, 0x13, 0x55, 0x41, 0xe1 }; //RC2 parameter
        private static byte[] IV = { 0x1c, 0x2d, 0x31, 0x45, 0x56, 0x67, 0x78, 0x08, 0x09, 0x11, 0x15, 0x17, 0x19, 0x20, 0x36, 0x48 }; //RC2 parameter

        /// <summary>Encryption</summary>
        /// <param name="password">password</param>
        /// <returns>Encrypted password</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static string RC2Encrypted(string password)
        {
            ASCIIEncoding textConverter = new ASCIIEncoding();
            RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();
            byte[] encrypted;
            byte[] toEncrypt;

            //Get an encryptor.
            ICryptoTransform encryptor = rc2CSP.CreateEncryptor(key, IV);

            //Encrypt the data.
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            //Convert the data to a byte array.
            // toEncrypt = password.ToCharArray();
            toEncrypt = textConverter.GetBytes(password);

            //Write all data to the crypto stream and flush it.
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            //Get encrypted array of bytes.
            encrypted = msEncrypt.ToArray();
            // Convert encrypted data to Base64String
            string EncryptedString = Convert.ToBase64String(encrypted);
            return EncryptedString;

        }


        /// <summary>Decryption</summary>
        /// <param name="base64String">Encrypted password</param>
        /// <returns>string: password</returns>
        /// <remarks>
        /// Modified Date:
        /// Modified By:
        /// Modified Reason:
        /// </remarks>
        public static string RC2Decrypted(string base64String)
        {
            ASCIIEncoding textConverter = new ASCIIEncoding();
            RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();

            //Get a decryptor that uses the same key and IV as the encryptor.
            ICryptoTransform decryptor = rc2CSP.CreateDecryptor(key, IV);

            // Convert base64String to encryptedCharArray
            byte[] encryptedCharArray = Convert.FromBase64String(base64String);

            MemoryStream msDecrypt = new MemoryStream(encryptedCharArray);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            byte[] fromEncrypt;

            // assume maximum password length is 30
            fromEncrypt = new byte[30];

            //Read the data out of the crypto stream.
            csDecrypt.Read(fromEncrypt, 0, 30);

            //Convert the byte array back into a string.
            string password = textConverter.GetString(fromEncrypt);
            // The password may become "password\0\0\0", so must eliminate the \0 at tail
            int pos = password.IndexOf('\0');
            password = password.Substring(0, pos);

            return password;

        }
    }
}