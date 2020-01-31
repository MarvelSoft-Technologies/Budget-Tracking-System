using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Budget_Tracking_System
{
    public class Utilities
    {
        public static string RemoveSpecialCharacters(string str)
        {
            if (str == null) return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public static string LoadWebURL(string URL)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetAsync(URL);
                var pageContents = response.Result.Content.ReadAsStringAsync();
                return pageContents.Result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }
        public static bool IsValidEmail(string email)
        {
            if (email == null)
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return (addr.Address == email) && EmailIsValid(email);
            }
            catch
            {
                return false;
            }
        }
        public string CleanNumber(string phone)
        {
            try
            {
                Regex digitsOnly = new Regex(@"[^\d]");
                return digitsOnly.Replace(phone, "");
            }
            catch (Exception)
            {

            }
            return "0";
        }
        static Regex ValidEmailRegex = CreateValidEmailRegex();

        static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }

        static bool EmailIsValid(string emailAddress)
        {
            bool isValid = ValidEmailRegex.IsMatch(emailAddress);
            return isValid;
        }

        public static string RandomCode(int Size)
        {
            try
            {
                char[] chars = new char[62];
                chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                byte[] data = new byte[1];
                using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                {
                    crypto.GetNonZeroBytes(data);
                    data = new byte[Size];
                    crypto.GetNonZeroBytes(data);

                }
                StringBuilder result = new StringBuilder(Size);
                foreach (byte b in data)
                {
                    result.Append(chars[b % (chars.Length)]);
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public static string FormattedDate(DateTime ThisDate)
        {
            try
            {
                if (ThisDate != DateTime.MaxValue && ThisDate != DateTime.MinValue)
                {
                    return ThisDate.ToShortDateString() + ":" + ThisDate.ToShortTimeString();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return "";
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public static String unixTimeStamp()
        {

            return ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            //DateTime foo = DateTime.UtcNow;
            //long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            //return unixTime.ToString();

        }
        // RNGCryptoServiceProvider is thread safe in .NET 3.5 and above
        // .NET 3.0 and below will need locking to protect access
        private static readonly RNGCryptoServiceProvider random =
            new RNGCryptoServiceProvider();

        private static byte[] GenerateNonceByte(int length)
        {
            // a default length could be specified instead of being parameterized
            var data = new byte[length];
            random.GetNonZeroBytes(data);
            return data;
        }
        // or
        //public static  string GenerateNonceString(int length)
        //{
        //    var data = new byte[length];
        //    random.GetNonZeroBytes(data);
        //    return Convert.ToBase64String(data);
        //}
        public static string GenerateNonceString(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "1234567890".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        public static String returnSignature(String http_verb, string url, string timeStamp, String nonce)
        {
            //
            // string key = Globals.getPayCodeTokenNew();


            //String baseStringToBeSigned = http_verb + "&" + url + "&" + timeStamp + "&" + nonce + "&" + Globals.ClientIDPayCode + "&" + Globals.SecretKeyPayCode;

            //// String baseStringSigned =(SHA1(baseStringToBeSigned));
            //String baseStringSigned = computeHash(baseStringToBeSigned, "SHA1");
            //return (baseStringSigned);
            StringBuilder signature = new StringBuilder(http_verb);
            signature.Append("&")
                .Append(Uri.EscapeDataString(url))
                .Append("&")
                .Append(timeStamp)
                .Append("&")
                .Append(nonce)
                .Append("&")
                .Append(Globals.PAYCODE_ClientID)
                .Append("&")
                .Append(Globals.PAYCODE_SecretKey);

            //if (SignedParameters != null && !SignedParameters.Equals(""))
            //{
            //    signature.Append("&")
            //    .Append(SignedParameters);
            //}
            return ComputeHash(signature.ToString());
        }
        public static string ComputeHash(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            Sha1Digest hash = new Sha1Digest();
            hash.BlockUpdate(data, 0, data.Length);
            byte[] result = new byte[hash.GetDigestSize()];
            hash.DoFinal(result, 0);
            return Convert.ToBase64String(result);
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

        public static int DateKey(DateTime date)
        {
            try
            {
                if (date != DateTime.MinValue)
                {
                    return int.Parse(date.Year.ToString() + date.Month.ToString() + date.Day.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return 0;
        }

        public static String computeHash(String message, String algo)
        {
            byte[] sourceBytes = Encoding.Default.GetBytes(message);
            byte[] hashBytes = null;
            Console.WriteLine(algo);
            switch (algo.Trim().ToUpper())
            {
                case "MD5":
                    hashBytes = MD5CryptoServiceProvider.Create().ComputeHash(sourceBytes);
                    break;
                case "SHA1":
                    hashBytes = SHA1Managed.Create().ComputeHash(sourceBytes);
                    break;
                case "SHA256":
                    hashBytes = SHA256Managed.Create().ComputeHash(sourceBytes);
                    break;
                case "SHA384":
                    hashBytes = SHA384Managed.Create().ComputeHash(sourceBytes);
                    break;
                case "SHA512":
                    hashBytes = SHA512Managed.Create().ComputeHash(sourceBytes);
                    break;
                default:
                    break;
            }
            return Convert.ToBase64String(hashBytes);
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; hashBytes != null && hashBytes.Length; i++) {
            //    sb.AppendFormat("{0:x2}", hashBytes[i]);
            //}
            //return sb.ToString();
        }
    }

    static public class NumericExtensions
    {
        static public decimal SafeDivision(this int Numerator, int Denominator)
        {
            return (Denominator == 0) ? 0 : Numerator / Denominator;
        }
    }

}
