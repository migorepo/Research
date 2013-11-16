using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// http://janhannemann.wordpress.com/2013/11/10/simple-encryption-for-windows-winrt-and-windows-phone/

namespace Wp8.Framework.Utils.Encryption
{
    public static class EncryptionHelper
    {

        public static byte[] Encrypt(string dataToEncrypt, string password, string salt)
        {
            AesManaged aes = null;
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                //Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator 
                var rfc2898 = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

                //Create AES algorithm with 256 bit key and 128-bit block size 
                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                rfc2898.Reset(); //needed for WinRT compatibility
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                //Create Memory and Crypto Streams 
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

                //Encrypt Data 
                byte[] data = Encoding.Unicode.GetBytes(dataToEncrypt);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                //Return encrypted data 
                return memoryStream.ToArray();

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (cryptoStream != null)
                    cryptoStream.Close();

                if (memoryStream != null)
                    memoryStream.Close();

                if (aes != null)
                    aes.Clear();

            }
        }

        public static string Decrypt(byte[] dataToDecrypt, string password, string salt)
        {
            AesManaged aes = null;
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;
            string decryptedText = "";
            try
            {
                //Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator 
                var rfc2898 = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

                //Create AES algorithm with 256 bit key and 128-bit block size 
                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                rfc2898.Reset(); //neede to be WinRT compatible
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                //Create Memory and Crypto Streams 
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

                //Decrypt Data 
                cryptoStream.Write(dataToDecrypt, 0, dataToDecrypt.Length);
                cryptoStream.FlushFinalBlock();

                //Return Decrypted String 
                byte[] decryptBytes = memoryStream.ToArray();
                decryptedText = Encoding.Unicode.GetString(decryptBytes, 0, decryptBytes.Length);
            }
            catch (Exception eDecrypt)
            {
                Debug.WriteLine("Error Caught while decription {0}", eDecrypt);
            }
            finally
            {
                if (cryptoStream != null)
                    cryptoStream.Close();

                if (memoryStream != null)
                    memoryStream.Close();

                if (aes != null)
                    aes.Clear();
            }
            return decryptedText;
        }

    }
}
