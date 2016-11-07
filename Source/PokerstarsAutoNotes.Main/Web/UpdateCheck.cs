using System;
using System.IO;
using System.Net;
using System.Reflection;
using PokerstarsAutoNotes.Extensions;

namespace PokerstarsAutoNotes.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateCheck
    {
        private const string Webserver =
            "éõõñ»®®ööö¯íèäòòâéäï¯åä®Àôõîíàãäíäó®åîöïíîàåò®";
        private const string Ftpserver =
            "çõñ»®®ööö¯íèäòòâéäï¯åä»³°®Àôõîíàãäíäó®åîöïíîàåò®åîöïíîàåäó®";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Check()
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(Webserver.EncryptDecrypt() + "version.txt");
            webRequest.UserAgent = "AutoLabeler";
            webRequest.Proxy = null;
            webRequest.Timeout = 3000;
            try
            {
                webRequest.GetResponse();
                string newVersion;
                using (var client = new WebClient())
                {
                    client.Proxy = null;
                    newVersion = client.DownloadString(Webserver.EncryptDecrypt() + "version.txt");
                }
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if (currentVersion != newVersion)
                    return newVersion;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Download(string path, string filename)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.Proxy = null;
                    client.DownloadFile(Webserver.EncryptDecrypt() + filename, Path.Combine(path, filename));
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
