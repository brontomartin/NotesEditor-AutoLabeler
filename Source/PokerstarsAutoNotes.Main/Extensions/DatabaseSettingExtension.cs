using System;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class DatabaseSettingExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="datenbank"></param>
        public static void Write(this Database datenbank)
        {
            Properties.Settings.Default.Server = datenbank.Server;
            Properties.Settings.Default.Port = datenbank.Port;
            Properties.Settings.Default.Database = datenbank.Name;
            Properties.Settings.Default.Username = datenbank.Username;
            Properties.Settings.Default.Password = datenbank.Password == string.Empty ? "" : datenbank.Password;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
// ReSharper disable UnusedParameter.Global
        public static Database Read(this Database datenbank)
// ReSharper restore UnusedParameter.Global
        {
            Database database = null;
            try
            {
                database = new Database
                {
                    Server = Properties.Settings.Default.Server,
                    Port = Properties.Settings.Default.Port,
                    Name = Properties.Settings.Default.Database,
                    Username = Properties.Settings.Default.Username,
                    Password = Properties.Settings.Default.Password,
                };
            }
            catch (Exception)
            {
                return database;
            }
            return database;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public static Database Read()
        {
            return new Database
            {
                Server = Properties.Settings.Default.Server,
                Port = Properties.Settings.Default.Port,
                Name = Properties.Settings.Default.Database,
                Username = Properties.Settings.Default.Username,
                Password = Properties.Settings.Default.Password,
            };
        }
    }
}
