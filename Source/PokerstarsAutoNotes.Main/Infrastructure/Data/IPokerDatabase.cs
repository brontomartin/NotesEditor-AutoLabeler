using System;
using System.Collections.Generic;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Infrastructure.Data
{
    public interface IPokerDatabase
    {
        int Playercount();
        int Playercount(IList<GameType> gameTypes, bool tourney);
        IEnumerable<Player> ReadPlayer();
        IEnumerable<Player> ReadPlayer(IList<GameType> gameTypes, bool tourney);
        IEnumerable<GameType> ReadGametypes();
        IEnumerable<DbField> ReadDbFields();
        IEnumerable<DbPlayer> ReadDbPlayers();
        event Action OnPlayerCompleted;
        PokerSiteEnum PokerSite(PokerSiteEnum pokerSite);
        bool CreateFakePlayer(int count);
    }
}
