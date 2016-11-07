using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Xml.Definitions
{
    public interface IRatingDefinitions
    {
        IList<Rating> Read(string filename);
        bool Write(IEnumerable<Rating> ratings, string filename);
        bool Validate(IEnumerable<Rating> ratings);
        IList<Rating> Change(IList<Rating> ratings, int index, string value, DefinitionEnum definitionEnum, bool from, PokerSiteEnum pokerSiteEnum);
    }

    public class RatingDefinitions : IRatingDefinitions
    {
 
        public IList<Rating> Read(string filename)
        {
            try
            {
                var definitions = from c in XElement.Load(filename).Elements("definitions").Elements("definition")
                                  select c;
                return definitions.Select(definition => new Rating
                {
                    Index = definition.Attribute("id") == null ? 0 : int.Parse(definition.Attribute("id").Value),
                    Vpip = definition.Attribute("vpip") == null ? string.Empty : definition.Attribute("vpip").Value,
                    VpipTo = definition.Attribute("vpipto") == null ? string.Empty : definition.Attribute("vpipto").Value,
                    Pfr = definition.Attribute("pfr") == null ? string.Empty : definition.Attribute("pfr").Value,
                    PfrTo = definition.Attribute("pfrto") == null ? string.Empty : definition.Attribute("pfrto").Value,
                    Bb100 = definition.Attribute("bb") == null ? string.Empty : definition.Attribute("bb").Value,
                    Bb100To = definition.Attribute("bbto") == null ? string.Empty : definition.Attribute("bbto").Value,
                    Hands = definition.Attribute("hands") == null ? "0" : definition.Attribute("hands").Value,
                    VpipPfrRatio = definition.Attribute("vpippfr") == null ? string.Empty : definition.Attribute("vpippfr").Value,
                    VpipPfrRatioTo = definition.Attribute("vpippfrto") == null ? string.Empty : definition.Attribute("vpippfrto").Value,
                    BbFoldToSteal = definition.Attribute("bbfoldtosteal") == null ? string.Empty : definition.Attribute("bbfoldtosteal").Value,
                    BbFoldToStealTo = definition.Attribute("bbfoldtostealto") == null ? string.Empty : definition.Attribute("bbfoldtostealto").Value,
                    ThreeBet = definition.Attribute("threebet") == null ? string.Empty : definition.Attribute("threebet").Value,
                    ThreeBetTo = definition.Attribute("threebetto") == null ? string.Empty : definition.Attribute("threebetto").Value,
                    FoldTo3Bet = definition.Attribute("foldthreebet") == null ? string.Empty : definition.Attribute("foldthreebet").Value,
                    FoldTo3BetTo = definition.Attribute("foldthreebetto") == null ? string.Empty : definition.Attribute("foldthreebetto").Value,
                    PokerSiteEnum = definition.Attribute("pokersite") == null ? PokerSiteEnum.Pokerstars : (PokerSiteEnum)Enum.Parse(typeof(PokerSiteEnum), definition.Attribute("pokersite").Value)
                }).ToList();
             
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///<summary>
        ///</summary>
        ///<returns>Saved</returns>
        public bool Write(IEnumerable<Rating> ratings, string filename)
        {
            try
            {
                var settings = new XmlWriterSettings { Indent = true };
                using (var xmlWriter = XmlWriter.Create(filename, settings))
                {
                    xmlWriter.WriteStartElement("autolabelerdefintions");
                    xmlWriter.WriteStartElement("definitions");
                    foreach (var rating in ratings)
                    {
                        xmlWriter.WriteStartElement("definition");
                        xmlWriter.WriteAttributeString("id", rating.Index.ToString(CultureInfo.InvariantCulture));
                        xmlWriter.WriteAttributeString("vpip", rating.Vpip);
                        xmlWriter.WriteAttributeString("vpipto", rating.VpipTo);
                        xmlWriter.WriteAttributeString("pfr", rating.Pfr);
                        xmlWriter.WriteAttributeString("pfrto", rating.PfrTo);
                        xmlWriter.WriteAttributeString("bb", rating.Bb100);
                        xmlWriter.WriteAttributeString("bbto", rating.Bb100To);
                        xmlWriter.WriteAttributeString("hands", rating.Hands);
                        xmlWriter.WriteAttributeString("vpippfr", rating.VpipPfrRatio);
                        xmlWriter.WriteAttributeString("vpippfrto", rating.VpipPfrRatioTo);
                        xmlWriter.WriteAttributeString("bbfoldtosteal", rating.BbFoldToSteal);
                        xmlWriter.WriteAttributeString("bbfoldtostealto", rating.BbFoldToStealTo);
                        xmlWriter.WriteAttributeString("threebet", rating.ThreeBet);
                        xmlWriter.WriteAttributeString("threebetto", rating.ThreeBetTo);
                        xmlWriter.WriteAttributeString("foldthreebet", rating.FoldTo3Bet);
                        xmlWriter.WriteAttributeString("foldthreebetto", rating.FoldTo3BetTo);
                        xmlWriter.WriteAttributeString("pokersite", rating.PokerSiteEnum.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Validate(IEnumerable<Rating> ratings)
        {
            return ratings.Any(rating => rating.VpipDouble > rating.VpipDoubleTo ||
                rating.PfrDouble > rating.PfrDoubleTo || rating.Bb100Double > rating.Bb100DoubleTo ||
                rating.VpipPfrDouble > rating.VpipPfrDoubleTo || rating.BbFoldToStealDouble > rating.BbFoldToStealDoubleTo ||
                rating.ThreeBetDouble > rating.ThreeBetDoubleTo || rating.Fold3BetDouble > rating.Fold3BetDoubleTo);
        }

        public IList<Rating> Change(IList<Rating> ratings, int index, string value, DefinitionEnum definitionEnum, bool from, PokerSiteEnum pokerSiteEnum)
        {
            if (value == "-" || value == "," || value == "k")
                return ratings;
            var rating = ratings.First(r => r.Index == index && r.PokerSiteEnum == pokerSiteEnum);
            switch (definitionEnum)
            {
                case DefinitionEnum.Bb100:
                    if (from)
                        rating.Bb100 = value;
                    else
                        rating.Bb100To = value;
                    break;
                case DefinitionEnum.BbFts:
                    if (from)
                        rating.BbFoldToSteal = value;
                    else
                        rating.BbFoldToStealTo = value;
                    break;
                case DefinitionEnum.Hands:
                    rating.Hands = value;
                    break;
                case DefinitionEnum.Pfr:
                    if (from)
                        rating.Pfr = value;
                    else
                        rating.PfrTo = value;
                    break;
                case DefinitionEnum.Vpip:
                    if (from)
                        rating.Vpip = value;
                    else
                        rating.VpipTo = value;
                    break;
                case DefinitionEnum.VpipPfr:
                    if (from)
                        rating.VpipPfrRatio = value;
                    else
                        rating.VpipPfrRatioTo = value;
                    break;
                case DefinitionEnum.Tbet:
                    if (from)
                        rating.ThreeBet = value;
                    else
                        rating.ThreeBetTo = value;
                    break;
                case DefinitionEnum.Ft3Bet:
                    if (from)
                        rating.FoldTo3Bet = value;
                    else
                        rating.FoldTo3BetTo = value;
                    break;
            }
            return ratings;
        }
    }
}
