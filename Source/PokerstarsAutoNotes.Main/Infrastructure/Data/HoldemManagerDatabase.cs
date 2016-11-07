using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Infrastructure.Data
{

    public class HoldemManagerDatabase : IPokerDatabase
    {
        private readonly Model.Database _database;
        private readonly IDatabase _databaseFunctions;
        private HmPokerSite _hmPokerSite;

        public HoldemManagerDatabase(Model.Database database)
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
            return _databaseFunctions.CreateFakePlayer(count);
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
            var gametypes = _databaseFunctions.GetGametypes(@"select distinct gametypes.gametype_id, gametypedescription, istourney,
                    cast(pokergame as int), cast(case when gametypedescription like '%NL%' then 0 when gametypedescription like '%PL%' then 1 when gametypedescription like '%LIM%' then 2
                    when isTourney = 't' then pokergametype end as int) as pokergametype, cast(playercount.playercount as int) As players,
                    cast(case when gametypedescription like '%ZOOM%' then 't' else 'f' end as boolean) as IsZoom, bigblind/100
                    from gametypes inner join compiledresults_month on compiledresults_month.gametype_id = gametypes.gametype_id
                    inner join players on players.player_id = compiledresults_month.player_id
                    inner join (SELECT case when max(numberofplayers) < 3 then 2 when max(numberofplayers) < 7 then 1 else 0 end as playercount , gametype_id
                    FROM compiledresults_month group by gametype_id) as playercount on playercount.gametype_id = compiledresults_month.gametype_id
                    where site_id = " + (int)_hmPokerSite + " order by gametypedescription, pokergame, pokergametype, players");
            foreach (var gameType in gametypes)
            {
                gameType.Database = _database.Name;
                gameType.Description = gameType.Description.Replace(" PL", string.Empty)
                                        .Replace(" NL", string.Empty).Replace(" LIM", string.Empty)
                                        .Replace(" ZOOM", string.Empty).Replace(" CAP", string.Empty).Replace("Trny: ", "");
                if (gameType.Description.IndexOf("/", StringComparison.Ordinal) > 0)
                    gameType.Description = gameType.Description.Substring(gameType.Description.IndexOf("/", StringComparison.Ordinal) + 1, gameType.Description.Length - gameType.Description.IndexOf("/", StringComparison.Ordinal) - 1);
            }
            return gametypes;
        }

        public string GetUser()
        {
            var sql = @"SELECT playername FROM players order by (cashhands + tourneyhands) desc LIMIT 1";
            return _databaseFunctions.GetString(sql);
        }

        public int Playercount()
        {
            return _databaseFunctions.GetInt(@"select cast(count(*) as int) from players where site_id = " + (int)_hmPokerSite);
        }

        public int Playercount(IList<GameType> gameTypes, bool tourney)
        {
            var types = gameTypes.Aggregate(string.Empty, (current, gameType) => current + (current != string.Empty ? "," + gameType.Id : gameType.Id.ToString(CultureInfo.InvariantCulture)));
            return _databaseFunctions.GetInt(@"select cast(count(*) as int) from (select count(compiledresults_month.player_id)
            from compiledresults_month inner join players on players.player_id = compiledresults_month.player_id
            inner join gametypes on gametypes.gametype_id = compiledresults_month.gametype_id
            where site_id = " + (int)_hmPokerSite + " and gametypes.gametype_id in (" + types + @") group by compiledresults_month.player_id, playername) As Counter");
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
                    @"select compiledresults_month.player_id, playername, gametype_id, totalhands, vpiphands,
                    pfrhands, cast(totalbbswon as float) as bbwon, bigblindstealattempted, bigblindstealdefended, couldthreebet, didthreebet,
                    facedthreebetpreflop, foldedtothreebetpreflop from compiledresults_month inner join compiledplayerresults_month on 
                    compiledresults_month.compiledplayerresults_id = compiledplayerresults_month.compiledplayerresults_id
                    inner join players on players.player_id = compiledresults_month.player_id where site_id = " +
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
