using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Xml.Pokerstars
{
    /// <summary>
    /// Read and Test the notesfile
    /// </summary>
    public class PokerstarsNotes : IPokersiteNotes
    {

        /// <summary>
        /// Check if the xml is a valid pokerstars notefile
        /// </summary>
        /// <param name="notesfile">xml file to check</param>
        /// <returns>true when xml is valid for pokerstars</returns>
        public bool CheckXml(string notesfile)
        {
            IEnumerable<XElement> labels;
            try
            {
                labels = from c in XElement.Load(notesfile).Elements("labels").Elements("label")
                             select c;
            }
            catch (Exception)
            {
                return false;
            }
            return labels.Any();
        }

        /// <summary>
        /// Read the labels from the notesfile
        /// </summary>
        /// <returns></returns>
        public IList<Label> ReadLabels()
        {
            int position = 0;
            var llables = new List<Label>();
            var labels = from c in XElement.Load(Properties.Settings.Default.NotesFile).Elements("labels").Elements("label")
                        select c;

            foreach (var label in labels)
            {
                // ReSharper disable PossibleNullReferenceException
                var llabel = new Label
                                 {
                                     Index = int.Parse(label.Attribute("id").Value),
                                     Color = GetSystemDrawingColorFromHexString(label.Attribute("color").Value),
                                     Name = label.Value,
                                     Position = position
                                 };
// ReSharper restore PossibleNullReferenceException
                position += 1;
                llables.Add(llabel);
            }
            var returnlabels = llables.ToList();
            returnlabels.Add(new Label { Color = Color.White, Index = returnlabels.Max(l => l.Index) + 1, Name = "No Label", Position = position });
            returnlabels.Sort();
            return returnlabels;
        }

        /// <summary>
        /// Rate the players
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public IEnumerable<Player> ReadPlayers(IEnumerable<Label> labels)
        {
            var notes = from c in XElement.Load(Properties.Settings.Default.NotesFile).Elements("note")
                         select c;

            var players = new List<Player>();
            foreach (var note in notes)
            {
                if ((string)note.Attribute("player") == null)
                    continue;
// ReSharper disable PossibleNullReferenceException
                var player = new Player {Name = note.Attribute("player").Value};

                if ((string)note.Attribute("label") != null)
                {
// ReSharper disable AccessToModifiedClosure
// ReSharper disable PossibleMultipleEnumeration
                    if (note.Attribute("label").Value == "-1")
                        player.Label = labels.Single(p => p.Name == "No Label");
                    else if (int.Parse(note.Attribute("label").Value) > labels.Max(p => p.Index) -1)
                        player.Label = labels.Single(p => p.Name == "No Label");
                    else
                        //ToDo: wenn 2 mal der gleiche index vergeben wurde
                        player.Label = labels.Single(p => p.Index == int.Parse(note.Attribute("label").Value));
                }
                else
                    player.Label = labels.Single(p => p.Name == "No Label");
// ReSharper restore PossibleMultipleEnumeration
// ReSharper restore AccessToModifiedClosure

                player.Note = note.Value;
                player.Update = (string) note.Attribute("update") != null ?
                            ConvTimestamp2Date(int.Parse(note.Attribute("update").Value)) :
                            DateTime.Now;
                // ReSharper restore PossibleNullReferenceException
                player.Changed = false;
                player.Exists = true;
                //var node = note.Attribute("noautolabel");
                //if (node != null)
                //    player.DoNotAutolabel = (bool)note.Attribute("noautolabel");

                //node = note.Attribute("autolabeldatabase");
                //if (node != null)
                //    player.AutolabelDatabase = (string)note.Attribute("autolabeldatabase");
                //else
                //    player.AutolabelDatabase = string.Empty;
                player.DoNotAutolabel = player.Note.StartsWith("#!#");

                if (player.Note.StartsWith("##"))
                {
                    var match = Regex.Match(player.Note, "##.+!!");
                    player.AutolabelDatabase = match.ToString().Replace("##","").Replace("!!","");
                }
                else
                {
                    player.AutolabelDatabase = string.Empty;
                }

                players.Add(player);
            }

            players.Sort();
            return players;
        }

        /// <summary>
        /// Get color from hexstring
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private Color GetSystemDrawingColorFromHexString(string hexString)
        {
            if (hexString.Count() < 6)
                switch (hexString.Count())
                {
                    case 1:
                        hexString = "00000" + hexString;
                        break;
                    case 2:
                        hexString = "0000" + hexString;
                        break;
                    case 3:
                        hexString = "000" + hexString;
                        break;
                    case 4:
                        hexString = "00" + hexString;
                        break;
                    case 5:
                        hexString = "0" + hexString;
                        break;

                }
            if (!Regex.IsMatch(hexString, @"([0-9]|[a-f]|[A-F]){6}\b"))
                throw new ArgumentException();
            int blue = int.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
            int green = int.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
            int red = int.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
            return Color.FromArgb(red, green, blue);
        }


        /// <summary>
        /// Converting unix timestamp to datetime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private DateTime ConvTimestamp2Date(int timestamp)
        {
            //  gerechnet wird ab der UNIX Epoche
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            // den Timestamp addieren           
            dateTime = dateTime.AddSeconds(timestamp);
            return dateTime.AddSeconds(-dateTime.Second);
        }

        readonly char[] _hexDigits = {'0', '1', '2', '3', '4', '5', '6', '7',
         '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        ///<summary>
        ///</summary>
        ///<param name="file"></param>
        ///<param name="players"></param>
        ///<param name="labels"></param>
        public bool Write(string file, IEnumerable<Player> players, IEnumerable<Label> labels)
        {
            var settings = new XmlWriterSettings { NewLineChars = "\n", Indent = true };
            try
            {
                using (var xmlWriter = XmlWriter.Create(file, settings))
                {
                    xmlWriter.WriteStartElement("notes");
                    xmlWriter.WriteAttributeString("version", "1");
                    xmlWriter.WriteStartElement("labels");
                    foreach (var label in labels)
                    {
                        if (label.Name == "No Label") continue;
                        xmlWriter.WriteStartElement("label");
                        xmlWriter.WriteAttributeString("id", label.Index.ToString());
                        xmlWriter.WriteAttributeString("color", ColorToHexString(label.Color));
                        xmlWriter.WriteValue(label.Name);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                    foreach (var player in players)
                    {
                        if (player.Label == null)
                        {
                            player.Label = labels.FirstOrDefault(s => s.Name == "No Label") ??
                                           new Label { Name = "No Label" };
                        }
                        if (player.Label.Name == "No Label" &&
                            (player.Note == null || player.Note.Trim() == string.Empty))
                            continue;
                        xmlWriter.WriteStartElement("note");
                        try
                        {
                            xmlWriter.WriteAttributeString("player", player.Name);
                        }
                        catch (ArgumentException)
                        {
                            xmlWriter.WriteAttributeString("player", SanitizeXmlString(player.Name));
                        }
                        xmlWriter.WriteAttributeString("label",
                                                       player.Label.Name == "No Label"
                                                           ? "-1"
                                                           : player.Label.Index.ToString());
                        xmlWriter.WriteAttributeString("update", ConvDate2Timestamp(player.Update).ToString());
                        xmlWriter.WriteValue(player.Note);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                }

                ReWriteXmlFile(file);
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Rewrite the notes cause pokerstars didnt work with CRLF, use LF only
        /// </summary>
        private void ReWriteXmlFile(string file)
        {
            var textreader = new StreamReader(file);
            var text = textreader.ReadToEnd();
            textreader.Close();
            text = text.Replace("\r", "");
            var textwriter = new StreamWriter(file, false);
            textwriter.Write(text);
            textwriter.Close();
        }

        /// <summary>
        /// Convert a .NET Color to a hex string.
        /// Achtung stars hat nicht rgb sondern bgr
        /// </summary>
        /// <returns>ex: "FFFFFF", "AB12E9"</returns>
        private string ColorToHexString(Color color)
        {
            var bytes = new byte[3];
            bytes[2] = color.R;
            bytes[1] = color.G;
            bytes[0] = color.B;
            var chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = _hexDigits[b >> 4];
                chars[i * 2 + 1] = _hexDigits[b & 0xF];
            }
            return new string(chars);
        }

        /// <summary>
        /// Converting datetime to unix timestamp
        /// </summary>
        /// <returns></returns>
        private int ConvDate2Timestamp(DateTime time)
        {
            var date1 = new DateTime(1970, 1, 1);  //Refernzdatum (festgelegt)
            var date2 = time;
            var ts = new TimeSpan(date2.Ticks - date1.Ticks);  // das Delta ermitteln
            // Das Delta als gesammtzahl der sekunden ist der Timestamp
            return (Convert.ToInt32(ts.TotalSeconds - ts.Seconds));
        }


        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        private string SanitizeXmlString(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            var buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (IsLegalXmlChar(c))
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        private bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }
    }
}
