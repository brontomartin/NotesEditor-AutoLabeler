using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Configuration
{
    public static class ParseHoldemManagerConfiguration
    {

        public static Database Parse()
        {
            //looking for pokertrackerconfigfile
            var database = new Database();
            try
            {
                var installDir = GetInstallDir();
                if (installDir == null)
                    return null;
                var file = Directory.GetFiles(installDir, "HoldemManager.config", SearchOption.AllDirectories);

                var connectionstring = (from c in XElement.Load(file[0]).Elements("setting")
// ReSharper disable PossibleNullReferenceException
                                    where c.Attribute("name").Value == "ConnectionString"
                                    select c.Value).FirstOrDefault();
                if (connectionstring == null)
                    return null;
                foreach (var conn in connectionstring.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (conn.StartsWith("SERVER="))
                        database.Server = conn.Substring(conn.IndexOf("=") + 1,
                                                         conn.Length - conn.IndexOf("=") - 1);
                    if (conn.StartsWith("PORT="))
                        database.Port = conn.Substring(conn.IndexOf("=") + 1,
                                                       conn.Length - conn.IndexOf("=") - 1);
                    if (conn.StartsWith("UID="))
                        database.Username = conn.Substring(conn.IndexOf("=") + 1,
                                                           conn.Length - conn.IndexOf("=") - 1);
                    if (conn.StartsWith("PWD="))
                        database.Password = conn.Substring(conn.IndexOf("=") + 1,
                                                           conn.Length - conn.IndexOf("=") - 1);
                }

                var databs = (from c in XElement.Load(file[0]).Elements("setting")
                              where c.Attribute("name").Value == "CurrentDatabase"
                              select c.Value).FirstOrDefault();
// ReSharper restore PossibleNullReferenceException
                database.Name = databs;

            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }
            if (database.Server != null && database.Port != null
                && database.Name != null && database.Username != null && database.Password != null)
            {
                return database;
            }
            return null;
        }

        private static string GetInstallDir()
        {
            if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\RVG Software\\HoldemManager", false) ??
                          Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\\\HoldemManager2", false);
                return key == null ? null : key.GetValue("InstallationFolder").ToString();
            }
            else
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\RVG Software\\HoldemManager", false) ??
                          Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\\\HoldemManager2", false);
                return key == null ? null : key.GetValue("InstallationFolder").ToString();
            }
        }
    }
}
