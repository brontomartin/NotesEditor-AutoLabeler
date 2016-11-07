using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Infrastructure.Data
{

    public class Hm2Database : IPokerDatabase
    {
        private readonly Model.Database _database;
        private readonly IDatabase _databaseFunctions;
        private HmPokerSite _hmPokerSite;

        public Hm2Database(Model.Database database)
        {
            if (database == null)
                throw new ArgumentException("database");
            _database = database;
            _databaseFunctions = new DatabaseFunctions(_database);
            _databaseFunctions.OnPlayerLoadCompleted += PlayerLoaded;
        }

        public PokerSiteEnum PokerSite(PokerSiteEnum pokerSiteEnum)
        {
            _hmPokerSite = pokerSiteEnum == PokerSiteEnum.Partypoker ? HmPokerSite.Party : HmPokerSite.Stars;
            return pokerSiteEnum;
        }

        public bool CreateFakePlayer(int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DbField> ReadDbFields()
        {
            return _databaseFunctions.GetDbFields(
                @"select a.attname as Column, pg_catalog.format_type(a.atttypid, a.atttypmod) as Datatype
                        from pg_catalog.pg_attribute a where a.attnum > 0 and not a.attisdropped
                        and a.attrelid = ( select c.oid from pg_catalog.pg_class c
                        left join pg_catalog.pg_namespace n on n.oid = c.relnamespace
                        where c.relname ~ '^(compiledplayerresults_month)$'
                        and pg_catalog.pg_table_is_visible(c.oid));");
        }

        public IEnumerable<DbPlayer> ReadDbPlayers()
        {
            throw new NotImplementedException();
        }

        public event Action OnPlayerCompleted = delegate { };

        private void PlayerLoaded()
        {
            if (OnPlayerCompleted != null)
                OnPlayerCompleted();
        }

        public IEnumerable<GameType> ReadGametypes()
        {
            var gametypes = _databaseFunctions.GetGametypes(@"select distinct gametypes.gametype_id,
                case when istourney = false and pokergametype_id in (2,5,8,11,15,18,24,27,29) then cast(cast(bigblindincents as float)/100*2 as varchar) 
                else cast(cast(bigblindincents as float)/100 as varchar) end as gametypedescription, istourney,
                case when pokergametype_id in (3,4,5,9,10,11,19,25,26,27) then 2 when pokergametype_id in (13,14,15,16,17,18,28,29,30) then 3 else 1 end as pokergame,
                case when pokergametype_id in (1,4,7,10,14,17,19,23,26,28) then 1 when pokergametype_id in (2,5,8,11,15,18,24,27,29) then 2 else 0 end as pokergametype,
                case when tablesize = 2 then 2 when tablesize = 6 then 1 else 0 end As players,
                cast(case when pokergametype_id in (6,7,8,9,10,11,16,17,18,22,23,24,25,26,27,28,29,30) then 't' else 'f' end as boolean) as IsZoom, bigblindincents/100  from gametypes inner join compiledplayerresults on gametypes.gametype_id = compiledplayerresults.gametype_id
                inner join players on players.player_id = compiledplayerresults.player_id
                where pokersite_id = " + (int)_hmPokerSite + " order by gametypedescription, pokergame, pokergametype, players");
            foreach (var gameType in gametypes)
            {
                gameType.Database = _database.Name;
                gameType.Description = gameType.Description.Replace(" PL", "").Replace(" NL", "").Replace(" LIM", "");
                if (gameType.Description.IndexOf("/", StringComparison.Ordinal) > 0)
                    gameType.Description = gameType.Description.Substring(gameType.Description.IndexOf("/", StringComparison.Ordinal) + 1, gameType.Description.Length - gameType.Description.IndexOf("/", StringComparison.Ordinal) - 1);
            }
            return gametypes;
        }

        public int Playercount()
        {
            return _databaseFunctions.GetInt(@"select cast(count(*) as int) from players where pokersite_id = " + (int)_hmPokerSite);
        }

        public int Playercount(IList<GameType> gameTypes, bool tourney)
        {
            var types = gameTypes.Aggregate(string.Empty, (current, gameType) => current + (current != string.Empty ? "," + gameType.Id : gameType.Id.ToString(CultureInfo.InvariantCulture)));
            return _databaseFunctions.GetInt(@"select cast(count(*) as int) from (select count(compiledplayerresults.player_id)
            from compiledplayerresults inner join players on players.player_id = compiledplayerresults.player_id
            inner join gametypes on gametypes.gametype_id = compiledplayerresults.gametype_id
            where gametypes.gametype_id in (" + types + @") group by compiledplayerresults.player_id, playername, pokersite_id having pokersite_id = " + (int)_hmPokerSite + ") As Counter");
        }


        public IEnumerable<Player> ReadPlayer()
        {
            return ConvertFromCompiledResults(ReadCompiledResults());
        }

        public IEnumerable<Player> ReadPlayer(IList<GameType> gameTypes, bool tourney)
        {
            var gTypes = gameTypes.Select(gameType => gameType.Id).ToList();
            return ConvertFromCompiledResults(ReadCompiledResults().Where(t => gTypes.Contains(t.GameTypeId)));
        }

        private IEnumerable<CompiledResultsMonth> ReadCompiledResults()
        {
            return _databaseFunctions.GetCompiledResultsMonth(
                    @"select compiledplayerresults.player_id, playername, cast(gametype_id as int), totalhands, vpiphands,
                    pfrhands, cast(totalbbswon as float)/100 as bbwon, bigblindstealattempted, bigblindstealdefended, couldthreebet, didthreebet,
                    facedthreebetpreflop, foldedtothreebetpreflop from compiledplayerresults
                    inner join players on players.player_id = compiledplayerresults.player_id where pokersite_id = " +
                    (int)_hmPokerSite).ToList();
        }

        private IEnumerable<Player> ConvertFromCompiledResults(IEnumerable<CompiledResultsMonth> compiledResults)
        {
            return from s in compiledResults
                   group s by new { s.PlayerId, s.PlayerName } into g
                   select new Player
                   {
                       Id = g.Key.PlayerId,
                       Name = g.Key.PlayerName.Replace("Ã", "Í"),
                       Hands = g.Sum(s => s.TotalHands),
                       Vpip = Math.Round(((double)g.Sum(s => s.VpipHands) / g.Sum(s => s.TotalHands)) * 100, 2),
                       Pfr = Math.Round(((double)g.Sum(s => s.PfrHands) / g.Sum(s => s.TotalHands)) * 100, 2),
                       BbWon = Math.Round(g.Sum(s => s.TotalBBsWon) / ((double)g.Sum(s => s.TotalHands) / 100), 2),
                       BbFoldToSteal = double.IsNaN(Math.Round(((g.Sum(s => s.BigBlindStealAttempted) - g.Sum(s => s.BigBlindStealDefended))
                                       / (double)g.Sum(s => s.BigBlindStealAttempted)) * 100, 2))
                                       ? 0
                                       : Math.Round(((g.Sum(s => s.BigBlindStealAttempted) - g.Sum(s => s.BigBlindStealDefended))
                                       / (double)g.Sum(s => s.BigBlindStealAttempted)) * 100, 2),
                       ThreeBet = double.IsNaN(Math.Round(((double)g.Sum(s => s.DidThreeBet) / g.Sum(s => s.CouldThreeBet)) * 100, 2))
                                   ? 0
                                   : Math.Round(((double)g.Sum(s => s.DidThreeBet) / g.Sum(s => s.CouldThreeBet)) * 100, 2),
                       FoldToThreeBet = double.IsNaN(Math.Round(((double)g.Sum(s => s.FoldedThreeBetPreflop)
                                       / g.Sum(s => s.FacedThreeBetPreflop)) * 100, 2))
                                       ? 0
                                       : Math.Round(((double)g.Sum(s => s.FoldedThreeBetPreflop)
                                       / g.Sum(s => s.FacedThreeBetPreflop)) * 100, 2),
                       DoNotAutolabel = false,
                       AutolabelDatabase = string.Empty,
                       VpipPfrRatio = double.IsNaN(Math.Round(Math.Round(((double)g.Sum(s => s.VpipHands) / g.Sum(s => s.TotalHands)) * 100, 2)
                       / Math.Round(((double)g.Sum(s => s.PfrHands) / g.Sum(s => s.TotalHands)) * 100, 2), 2)) ||
                       double.IsInfinity(Math.Round(Math.Round(((double)g.Sum(s => s.VpipHands) / g.Sum(s => s.TotalHands)) * 100, 2)
                       / Math.Round(((double)g.Sum(s => s.PfrHands) / g.Sum(s => s.TotalHands)) * 100, 2), 2)) ||
                       Math.Round(Math.Round(((double)g.Sum(s => s.VpipHands) / g.Sum(s => s.TotalHands)) * 100, 2)
                       / Math.Round(((double)g.Sum(s => s.PfrHands) / g.Sum(s => s.TotalHands)) * 100, 2), 2) > 100
                       ? 100
                       : Math.Round(Math.Round(((double)g.Sum(s => s.VpipHands) / g.Sum(s => s.TotalHands)) * 100, 2)
                       / Math.Round(((double)g.Sum(s => s.PfrHands) / g.Sum(s => s.TotalHands)) * 100, 2), 2)
                   };
        }
    }
}
