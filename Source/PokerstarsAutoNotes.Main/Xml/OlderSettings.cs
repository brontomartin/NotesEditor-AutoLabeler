using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PokerstarsAutoNotes.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public class OlderSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public void Get()
        {
            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Chrysalis";
                var dirs = Directory.GetFiles(path, "user.config", SearchOption.AllDirectories);
                var doc = new XmlDocument();
                doc.Load(dirs[0]);
                var root = doc.DocumentElement;
                XmlNode settings = root.ChildNodes[0].ChildNodes[0];
                foreach (XmlNode childNode in settings.ChildNodes)
                {
                    if (childNode == null) continue;
                    if (childNode.Attributes[0].Value == "NotesFile")
                        Properties.Settings.Default.NotesFile = childNode.InnerText;
                    if (childNode.Attributes[0].Value == "DefinitionFile")
                        Properties.Settings.Default.DefinitionFile = childNode.InnerText;
                    if (childNode.Attributes[0].Value == "AutorateText")
                        Properties.Settings.Default.AutorateText = childNode.InnerText;
                    if (childNode.Attributes[0].Value == "GameSelection")
                        Properties.Settings.Default.GameSelection = childNode.InnerText;
                    if (childNode.Attributes[0].Value == "Database")
                        Properties.Settings.Default.Database = childNode.InnerText;
                    if (childNode.Attributes[0].Value == "Password")
                        Properties.Settings.Default.Password = childNode.InnerText;
                    if (childNode.Attributes[0].Value == "Port")
                        Properties.Settings.Default.Port = childNode.InnerText;
                    if (childNode.Attributes[0].Value == "Server")
                        Properties.Settings.Default.Server = childNode.InnerText;
                    if (childNode.Attributes[0].Value == "Username")
                        Properties.Settings.Default.Username = childNode.InnerText;
                    if (childNode.Attributes[0].Value == "CheckUpdates")
                    {
                        var output = true;
                        bool.TryParse(childNode.InnerText, out output);
                        Properties.Settings.Default.CheckUpdates = output;
                    }
                }
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
            }
        } 
        
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public bool GetLicense()
        //{
        //    var user = string.Empty;
        //    var license = string.Empty;

        //    foreach (var file in GetFiles())
        //    {
        //        using (TextReader rdr = new StreamReader(file))
        //        {
        //            string line;
        //            while ((line = rdr.ReadLine()) != null)
        //            {
        //                if (!line.Contains("License")) continue;
        //                if (line.Contains("Licensee"))
        //                {
        //                    line = rdr.ReadLine();
        //                    user = line.Replace("<value>", "").Replace("</value>", "").Trim();
        //                    continue;
        //                }
        //                line = rdr.ReadLine();
        //                license = line.Replace("<value>", "").Replace("</value>", "").Trim();
        //            }

        //            if (license != string.Empty && user != string.Empty)
        //            {
        //                Properties.Settings.Default.Licensee = user;
        //                Properties.Settings.Default.License = license;
        //                Properties.Settings.Default.Save();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public bool GetNotes()
        //{
        //    var notes = string.Empty;

        //    foreach (var file in GetFiles())
        //    {
        //        using (TextReader rdr = new StreamReader(file))
        //        {
        //            string line;
        //            while ((line = rdr.ReadLine()) != null)
        //            {
        //                if (!line.Contains("NotesFile")) continue;
        //                line = rdr.ReadLine();
        //                notes = line.Replace("<value>", "").Replace("</value>", "").Trim();
        //            }

        //            if (notes != string.Empty)
        //            {
        //                Properties.Settings.Default.NotesFile = notes;
        //                Properties.Settings.Default.Save();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public bool GetDefinitions()
        //{
        //    var notes = string.Empty;

        //    foreach (var file in GetFiles())
        //    {
        //        using (TextReader rdr = new StreamReader(file))
        //        {
        //            string line;
        //            while ((line = rdr.ReadLine()) != null)
        //            {
        //                if (!line.Contains("DefinitionFile")) continue;
        //                line = rdr.ReadLine();
        //                notes = line.Replace("<value>", "").Replace("</value>", "").Trim();
        //            }

        //            if (notes != string.Empty)
        //            {
        //                Properties.Settings.Default.DefinitionFile = notes;
        //                Properties.Settings.Default.Save();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
    }
}
