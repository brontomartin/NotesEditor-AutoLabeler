using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Xml.PartyPoker
{

    /// <summary>
    /// Read and Test the notesfile
    /// </summary>
    public class PartyPokerNotes : IPokersiteNotes
    {

        /// <summary>
        /// Check if the xml is a valid pokerstars notefile
        /// </summary>
        /// <param name="notesfile">xml file to check</param>
        /// <returns>true when xml is valid for pokerstars</returns>
        public bool CheckXml(string notesfile)
        {
            //ToDo: empty file is also okay
            if (notesfile.ToLower().Contains("watchlist"))
                return false;
            var streamReader = new StreamReader(notesfile);
            while (!streamReader.EndOfStream)
            {
                var text = streamReader.ReadLine();
                return text != null && Regex.Match(text, @"^.+~.+>$").Success;
            }
            if (File.Exists(notesfile))
                return true;
            return false;
        }

        /// <summary>
        /// Read the labels from the notesfile
        /// </summary>
        /// <returns></returns>
        public IList<Label> ReadLabels()
        {
            int position = 0;
            var llables = new List<Label>();
            var labels = from c in XElement.Load("PartyPokerColor.xml").Elements("labels").Elements("label")
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
            var notesFile = new FileInfo(Properties.Settings.Default.NotesFile);
            var labelFile = Path.Combine(notesFile.FullName.Replace(notesFile.Name, ""), notesFile.Name.Replace("Notes", "WatchList"));
            var players = new List<Player>();
            var streamReader = new StreamReader(notesFile.FullName);
            while (!streamReader.EndOfStream)
            {
                var text = streamReader.ReadLine();
                if (text == null) continue;
                var plName = Regex.Match(text, @"^.+~").Value;
                if (plName != string.Empty)
                    plName = plName.Replace("~", "");
                var plNote = Regex.Match(text, @"~.+>$").Value;
                if (plNote != string.Empty)
                    plNote = plNote.Replace("~", "").Replace(">", "").Replace("\t", Environment.NewLine);
                var player = new Player {Name = plName, Note = plNote, Changed = false, Exists = true};
                player.DoNotAutolabel = player.Note.StartsWith("#!#");
                if (player.Note.StartsWith("##"))
                {
                    var match = Regex.Match(player.Note, "##.+!!");
                    player.AutolabelDatabase = match.ToString().Replace("##", "").Replace("!!", "");
                }
                else
                {
                    player.AutolabelDatabase = string.Empty;
                }
                players.Add(player);
            }
            streamReader.Close();

            streamReader = new StreamReader(labelFile);
            while (!streamReader.EndOfStream)
            {
                var text = streamReader.ReadLine();
                if (text == null) continue;
                var plName = Regex.Match(text, @"^.+~").Value;
                if (plName != string.Empty)
                    plName = plName.Replace("~", "");
                var plColor = Regex.Match(text, @"~.+>$").Value;
                if (plColor != string.Empty)
                    plColor = plColor.Replace("~", "").Replace(">", "");

                var sPlayer = players.FirstOrDefault(p => p.Name == plName);
                if (sPlayer == null)
                {
                    players.Add(new Player
                                 {
                                     Name = plName,
                                     Note = "",
                                     Label = labels.First(l => l.Name == plColor),
                                     Changed = false,
                                     Exists = true
                                 });
                }
                else
                {
                    var label = labels.FirstOrDefault(l => l.Name == plColor);
                    if (label != null)
                        sPlayer.Label = label;
                }
            }
            streamReader.Close();

            players.Sort();
            return players;
        }

        public bool Write(string file, IEnumerable<Player> players, IEnumerable<Label> labels)
        {
            try
            {
                var notesFile = new FileInfo(file);
                var labelFile = Path.Combine(notesFile.FullName.Replace(notesFile.Name, ""), notesFile.Name.Replace("Notes", "WatchList"));
                var streamWriterNotes = new StreamWriter(file);
                var streamWriterLabels = new StreamWriter(labelFile);
                foreach (var player in players)
                {
                    if (!string.IsNullOrWhiteSpace(player.Note))
                        streamWriterNotes.WriteLine(player.Name + "~" + player.Note.Replace(Environment.NewLine, "\t") + ">");

                    if (player.Label != null && player.Label.Name != "No Label")
                        streamWriterLabels.WriteLine(player.Name + "~" + player.Label.Name + ">");

                }
                streamWriterNotes.Close();
                streamWriterLabels.Close();

            }
            catch (IOException)
            {
                return true;
            }
            return false;
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
    }
}
