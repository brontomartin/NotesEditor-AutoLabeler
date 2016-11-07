using System.Collections.Generic;
using System.Linq;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Ratings
{
    public class PlayersRating
    {
        private readonly SinglePlayerRating _singlePlayerRating;

        public PlayersRating(SinglePlayerRating singlePlayerRating)
        {
            _singlePlayerRating = singlePlayerRating;
        }

        public void Rate(IEnumerable<Player> players)
        {
            players.ForEach(p => _singlePlayerRating.Rate(p));
        }
    }
}
