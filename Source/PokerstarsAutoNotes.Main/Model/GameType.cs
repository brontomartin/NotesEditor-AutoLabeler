using System;
using PokerstarsAutoNotes.Enums;

namespace PokerstarsAutoNotes.Model
{
    public class GameType : IComparable<GameType>
    {
        public int Id { get; set; }
        public bool IsTourney { get; set; }
        public string Description { get; set; }
        public PokerGame PokerGame { get; set; }
        public PokerGameType PokerGameType { get; set; }
        public PokerGamePlayer PokerGamePlayer { get; set; }
        public bool IsZoom { get; set; }
        public string Database { get; set; }
        public int BigBlind { get; set; }

        public int CompareTo(GameType other)
        {
            var gt = (int)PokerGameType;
            var ogt = (int)other.PokerGameType;
            var co = gt.CompareTo(ogt);
            if (co == 0)
                co = PokerGame.CompareTo(other.PokerGame);
            if (co == 0)
                co = PokerGamePlayer.CompareTo(other.PokerGamePlayer);
            return co;
        }

        public override string ToString()
        {
            return PokerGameType.ToString() + PokerGame + (IsZoom ? " Speed" : "") + " " + PokerGamePlayer;
        }

        public string ToShortString()
        {
            return PokerGameType.ToString() + PokerGame;
        }
    }
}
