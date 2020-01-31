using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Budget_Tracking_System
{
    public class CipherService
    {
        
        private const string EncryptKey = "AAECAwQFBgcICQoLDA0ODw";

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(EncryptKey, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(EncryptKey, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        

    }

    public class Encryptions
    {
        public static string EncryptURL(string plainText)
        {
            try
            {
                byte[] encodedBytes = System.Text.Encoding.Unicode.GetBytes(plainText);
                string encodedTxt = Convert.ToBase64String(encodedBytes);
                return WebUtility.UrlEncode(encodedTxt);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public static string DecryptURL(string text)
        {
            try
            {
                text = WebUtility.UrlDecode(text);
                byte[] decodedBytes = Convert.FromBase64String(text);
                string decodedTxt = System.Text.Encoding.UTF8.GetString(decodedBytes);
                string decodedTxt2 = System.Text.Encoding.Unicode.GetString(decodedBytes);
                return decodedTxt2;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
            //return Decrypt(text);
        }

        public static string SHA512(string text)
        {
            try
            {
                //UnicodeEncoding UE = new UnicodeEncoding();
                byte[] hashValue;
                byte[] message = Encoding.UTF8.GetBytes(text);
                SHA512Managed hashString = new SHA512Managed();
                string hex = "";

                hashValue = hashString.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }
                return hex;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public static string MD5Hash(string input)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                    return Encoding.ASCII.GetString(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }


        public static byte[] HashPassword(string Password, out byte[] outkey)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(Password, 20))
            {
                byte[] salt = deriveBytes.Salt;
                outkey = deriveBytes.GetBytes(20);  // derive a 20-byte key
                return salt;
            }
        }

        public static bool VerifyPassword(string Password, byte[] salt, byte[] key)
        {

            using (var deriveBytes = new Rfc2898DeriveBytes(Password, salt))
            {
                byte[] newKey = deriveBytes.GetBytes(20);  // derive a 20-byte key

                if (!newKey.SequenceEqual(key))
                    return false;
                return true;
            }
        }
    }
}
