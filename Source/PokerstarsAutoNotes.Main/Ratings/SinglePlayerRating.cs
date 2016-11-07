using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Infrastructure;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Ratings
{

    public class SinglePlayerRating
    {
        public event PlayerHandler RatePlayer;
        public delegate void PlayerHandler(SinglePlayerRating ratings, EventArgs e);

        private IEnumerable<Rating> _ratings;
        private IEnumerable<Label> _labels;
        private Database _database;
        private PokerSiteEnum _pokerSiteEnum;
        private string _autorateText;

        public void Parameters(List<Rating> ratings, IEnumerable<Label> labels,
            Database database, PokerSiteEnum pokerSiteEnum, string autorateText)
        {
            _ratings = ratings;
            _labels = labels;
            _database = database;
            _pokerSiteEnum = pokerSiteEnum;
            _autorateText = autorateText;

            _ratings = new RatingRemoverSorter().Remove(ratings, pokerSiteEnum);
        }

        public void Rate(Player player)
        {
#if DEBUG
            if (player.Id == 28592)
                player.Name = player.Name;
#endif

            if (!_ratings.Any())
                return;

            if (player.AutolabelDatabase != string.Empty)
                if (player.AutolabelDatabase != _database.Name)
                    return;

            //if autolabel off but has text change the text
            //if player has a label that hasnt no rating but text change that text
            if (player.DoNotAutolabel || (_ratings.FirstOrDefault(r => r.Index == player.Label.Index) == null && player.Label.Name != "No Label"))
            {
                var match = LookupAutoratetext(player.Note ?? string.Empty);
                if (match.Success)
                    SetAutorateText(player);
                return;
            }

            var rated = false;

// ReSharper disable CompareOfFloatsByEqualityOperator
            foreach (var rating in _ratings.Where(rating => player.Vpip >= rating.VpipDouble && (player.Vpip == 100.0 ? 99.99 : player.Vpip) < rating.VpipDoubleTo
                    && player.Pfr >= rating.PfrDouble && (player.Pfr == 100.0 ? 99.99 : player.Pfr) < rating.PfrDoubleTo
                    && player.VpipPfrRatio >= rating.VpipPfrDouble && (player.VpipPfrRatio == 100.0 ? 99.99 : player.VpipPfrRatio) < rating.VpipPfrDoubleTo
                    && player.BbWon >= rating.Bb100Double && player.BbWon < rating.Bb100DoubleTo
                  && player.BbFoldToSteal >= rating.BbFoldToStealDouble && (player.BbFoldToSteal == 100.0 ? 99.99 : player.BbFoldToSteal) < rating.BbFoldToStealDoubleTo
                    && player.ThreeBet >= rating.ThreeBetDouble && (player.ThreeBet == 100.0 ? 99.99 : player.ThreeBet) < rating.ThreeBetDoubleTo
                    && player.FoldToThreeBet >= rating.Fold3BetDouble && (player.FoldToThreeBet == 100.0 ? 99.99 : player.FoldToThreeBet) < rating.Fold3BetDoubleTo
                  && player.Hands >= rating.HandsDouble && rating.PokerSiteEnum == _pokerSiteEnum))
// ReSharper restore CompareOfFloatsByEqualityOperator
            {
                player.Label = _labels.FirstOrDefault(l => l.Index == rating.Index);
                SetAutorateText(player);
                player.Update = DateTime.Now.AddSeconds(-DateTime.Now.Second);

                rated = true;
               break;
            }
            if (!rated)
            {
                var matchNote = LookupAutoratetext(player.Note ?? string.Empty);
                if (matchNote.Success)
                {
                    player.Note = player.Note.Replace(matchNote.Value, "");
                    player.Label = _labels.First(s => s.Name == "No Label");
                }
            }

            if (RatePlayer != null)
                RatePlayer(this, null);
        }

        private Match LookupAutoratetext(string input)
        {
            return Regex.Match(input, "# AR:.*#", RegexOptions.Multiline);
        }

        private void SetAutorateText(Player player)
        {
            var ratingText = IoC.Resolve<RatingText>();
            if (string.IsNullOrEmpty(player.Note))
            {
                player.Note = ratingText.Get(_autorateText, player);
                return;
            }
            if (player.AutolabelDatabase != string.Empty)
            {
                player.Note = player.Note.Substring(player.AutolabelDatabase.Length + 4);
            }
            var match = LookupAutoratetext(player.Note);
            if (match.Success)
                player.Note = player.Note.Replace(match.Value, ratingText.Get(_autorateText, player));
            else
                player.Note = ratingText.Get(_autorateText, player) + Environment.NewLine + player.Note;
            if (player.AutolabelDatabase != string.Empty)
            {
                player.Note = "##" + player.AutolabelDatabase + "!!" + player.Note;
            }
        }
    }
}
