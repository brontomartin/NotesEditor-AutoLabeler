using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Configuration
{
    ///<summary>
    ///</summary>
    public static class ParsePokerTrackerConfiguration
    {
        ///<summary>
        ///</summary>
        ///<returns></returns>
        public static Database Parse()
        {
            //looking for pokertrackerconfigfile
            var database = new Database();
            try
            {
                var installDir = GetInstallDir();
                if (installDir == null)
                    return null;
                var file = Directory.GetFiles(installDir, "PokerTracker.cfg", SearchOption.AllDirectories);

                // Display all the files.
                using (TextReader rdr = new StreamReader(file[0]))
                {
                    var defaultdb = string.Empty;
                    string line;
                    while ((line = rdr.ReadLine()) != null)
                    {
                        if (line.Contains("[Database]"))
                        {
                            //found the databse section
                            string toend = rdr.ReadToEnd();
                            var lines = Regex.Split(toend, "\r\n");
                            foreach (var s in lines.Where(s => s.EndsWith("Default=Y")))
                            {
                                defaultdb = s.Substring(0, s.IndexOf('.'));
                                break;
                            }
                            if (defaultdb == string.Empty)
                                return database;
                            // ReSharper disable AccessToModifiedClosure
                            var containspostgres =
                                lines.Any(s => s.Contains(string.Format("{0}.Type=postgres", defaultdb)));

                            if (!containspostgres)
                                return database;
                            foreach (var s in lines.Where(s => s.StartsWith(defaultdb + ".Postgres.Database=")))
                            {
                                database.Name = s.Substring(s.IndexOf('=') + 1, s.Length - s.IndexOf('=') - 1);
                            }
                            foreach (var s in lines.Where(s => s.StartsWith(defaultdb + ".Postgres.Server=")))
                            {
                                database.Server = s.Substring(s.IndexOf('=') + 1, s.Length - s.IndexOf('=') - 1);
                            }
                            foreach (var s in lines.Where(s => s.StartsWith(defaultdb + ".Postgres.Port=")))
                            {
                                database.Port = s.Substring(s.IndexOf('=') + 1, s.Length - s.IndexOf('=') - 1);
                            }
                            foreach (var s in lines.Where(s => s.StartsWith(defaultdb + ".Postgres.User=")))
                            {
                                database.Username = s.Substring(s.IndexOf('=') + 1, s.Length - s.IndexOf('=') - 1);
                            }
                            foreach (var s in lines.Where(s => s.StartsWith(defaultdb + ".Postgres.Password=")))
                            {
                                database.Password = s.Substring(s.IndexOf('=') + 1, s.Length - s.IndexOf('=') - 1);
                            }
                            // ReSharper restore AccessToModifiedClosure
                        }
                    }
                }
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
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\PokerTracker3", false);
                return key == null ? null : key.GetValue("InstallDir").ToString();
            }
            else
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\PokerTracker3", false);
                return key == null ? null : key.GetValue("InstallDir").ToString();
            }
        }
    }
}
