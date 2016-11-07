using System;
using System.Collections.Generic;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Infrastructure.Data
{
    public interface IDatabase
    {
        int GetInt(string sql);
        IEnumerable<int> GetIntArray(string sql);
        string GetString(string sql);
        IEnumerable<Player> GetPlayers(string sql);
        IList<GameType> GetGametypes(string sql);
        IEnumerable<DbField> GetDbFields(string sql);
        event Action OnPlayerLoadCompleted;
        bool CreateFakePlayer(int count);
        IEnumerable<CompiledResultsMonth> GetCompiledResultsMonth(string sql);
    }
}
