using System.Collections.Generic;
using System.Linq;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Ratings
{
    public class RatingRemoverSorter
    {
        public IEnumerable<Rating> Remove(List<Rating> ratings, PokerSiteEnum pokerSiteEnum)
        {
            foreach (var rating in
                new List<Rating>(ratings).Where(rating => string.IsNullOrEmpty(rating.Bb100) && string.IsNullOrEmpty(rating.Bb100To) &&
                    string.IsNullOrEmpty(rating.Hands) && string.IsNullOrEmpty(rating.Vpip) && string.IsNullOrEmpty(rating.Pfr) &&
                    string.IsNullOrEmpty(rating.PfrTo) && string.IsNullOrEmpty(rating.VpipTo) &&
                    string.IsNullOrEmpty(rating.VpipPfrRatio) && string.IsNullOrEmpty(rating.VpipPfrRatioTo) &&
                    string.IsNullOrEmpty(rating.BbFoldToSteal) && string.IsNullOrEmpty(rating.BbFoldToStealTo) &&
                    string.IsNullOrEmpty(rating.ThreeBet) && string.IsNullOrEmpty(rating.ThreeBetTo) &&
                    string.IsNullOrEmpty(rating.FoldTo3Bet) && string.IsNullOrEmpty(rating.FoldTo3BetTo) && rating.PokerSiteEnum == pokerSiteEnum))
            {
                ratings.Remove(rating);
            }
            ratings.Sort();
            return ratings.ToArray();
        } 
    }
}
