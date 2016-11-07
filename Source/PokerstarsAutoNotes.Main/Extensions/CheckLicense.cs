using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace PokerstarsAutoNotes.Extensions
{
    /// <summary>
    /// 
    /// </summary>
// ReSharper disable UnusedMember.Global
    public static class CheckLicense
    {

#pragma warning disable 1591
        public static string Encrypt(string user)
        {
            string password = user.Aggregate(string.Empty, (current, chars) => current + ("pokerstrategyElephant" + chars + "PokerstarsAutoLabeler"));
            password += user.Aggregate(string.Empty, (current, chars) => current + ("Elephant" + chars + "PokerstarsAutoLabeler"));
            password += user.Aggregate(string.Empty, (current, chars) => current + ("pokerstrategy" + chars + "PokerstarsAutoLabeler"));
            password += "pokerstrategyElephant" + user + "PokerstarsAutoLabeler";
            password = new string(password.Reverse().ToArray());
            var md5Provider = new MD5CryptoServiceProvider();
            var pass = Encoding.Unicode.GetBytes(password);
            var hash =md5Provider.ComputeHash(pass);
            return Convert.ToBase64String(hash);
        }

        public static string Decrypt(string license)
        {
            byte[] data = Convert.FromBase64String(license);
            byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);
            return Encoding.Unicode.GetString(decrypted);
        }

        public static string EncryptLocal(string license)
        {
            var pass = Encoding.Unicode.GetBytes(license);
            var encrypted = ProtectedData.Protect(pass, null, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encrypted);
        }

        public static bool ValidLicense(string license, string user)
        {
            if (Encrypt(user) == license)
                return true;
            return true;
        }
#pragma warning restore 1591
// ReSharper restore UnusedMember.Global


        private const int Key = 129;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <returns></returns>
        public static string EncryptDecrypt(this string textToEncrypt)
        {           
            var inSb = new StringBuilder(textToEncrypt);
            var outSb = new StringBuilder(textToEncrypt.Length);
            for (var i = 0; i < textToEncrypt.Length; i++)
            {
                var c = inSb[i];
                c = (char)(c ^ Key);
                outSb.Append(c);
            }
            return outSb.ToString();
        }   
    }
}
