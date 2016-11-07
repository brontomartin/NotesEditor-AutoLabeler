using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Infrastructure.Data
{
    public class Pt4Database : IPokerDatabase
    {
        private readonly Model.Database _database;
        private readonly DatabaseFunctions _databaseFunctions;
        private PtPokerSite _ptPokerSite;

        public Pt4Database(Model.Database database)
        {
            if (database == null)
                throw new ArgumentException("database");
            _database = database;
            _databaseFunctions = new DatabaseFunctions(_database);
            _databaseFunctions.OnPlayerLoadCompleted += PlayerLoaded;
        }

        public PokerSiteEnum PokerSite(PokerSiteEnum pokerSiteEnum)
        {
            _ptPokerSite = pokerSiteEnum == PokerSiteEnum.Partypoker ? PtPokerSite.Party : PtPokerSite.Stars;
            return pokerSiteEnum;
        }

        public bool CreateFakePlayer(int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DbField> ReadDbFields()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DbPlayer> ReadDbPlayers()
        {
            throw new NotImplementedException();
        }

        public event Action OnPlayerCompleted = delegate { };

        public IEnumerable<Player> ReadHeroVsPlayer()
        {
            throw new NotImplementedException();
        }

        private void PlayerLoaded()
        {
            if (OnPlayerCompleted != null)
                OnPlayerCompleted();
        }

        public IEnumerable<GameType> ReadGametypes()
        {
            var gametypes = _databaseFunctions.GetGametypes(@"select * from
                    (select distinct cast(cash_limit.id_limit as int),
                    replace(replace(limit_name, ' (6 max)',''), ' CAP', '') as limitname, cast(0 as boolean),
                    cast(cash_limit.id_gametype as int) as pokergame,
                    case when flg_pl = 't' then cast(1 as int) when flg_nl = 't' then cast(0 as int) else cast(2 as int) end as pokergametype,
                    case when flg_hu = 't' then 2 when flg_sh = 't' then 1 else 0 end As players,
                    case when limit_name like '%ZOOM%' then cast('t' as boolean) else cast('f' as boolean) end as IsZoom, cast(amt_bb as int) as bigblind from cash_limit inner join cash_cache on cash_cache.id_limit = cash_limit.id_limit
                    inner join player on player.id_player = cash_cache.id_player
                    where id_site = " + (int)_ptPokerSite + @" union
                    select distinct tourney_blinds.id_blinds, '' as limitname, cast(1 as boolean),
                    cast(tourney_blinds.id_gametype as int) as pokergame, case when flg_nlpl = 'f' then cast(2 as int) when flg_nlpl = 't' and blinds_name like '%PL%' then cast(1 as int) else cast(0 as int) end as pokergametype,
                    case when flg_hu = 't' then 2 when flg_sh = 't' then 1 else 0 end As players,
                    cast('f' as boolean) as IsZoom, cast(amt_bb as int) as bigblind from tourney_blinds inner join tourney_cache on tourney_cache.id_blinds = tourney_blinds.id_blinds
                    inner join player on player.id_player = tourney_cache.id_player
                    where id_site = " + (int)_ptPokerSite + @" ) As Call
                    order by  limitname, pokergame, pokergametype, players");
            foreach (var gameType in gametypes)
            {
                gameType.Database = _database.Name;
                gameType.Description = gameType.Description.Replace(" ZOOM", string.Empty).Replace(" PL", "").Replace(" NL", "").Replace(" Hi/Lo", "").Replace(" Hi", "");
                if (gameType.Description.IndexOf("/", StringComparison.Ordinal) > 0)
                    gameType.Description = gameType.Description.Substring(gameType.Description.IndexOf("/", StringComparison.Ordinal) + 1, gameType.Description.Length - gameType.Description.IndexOf("/", StringComparison.Ordinal) - 1);

                gameType.Description = gameType.Description.Trim();
            }
            return gametypes;
        }

        public int Playercount()
        {
            return _databaseFunctions.GetInt(@"select cast(count(*) as int) from player where id_site = " + (int)_ptPokerSite);
        }

        public int Playercount(IList<GameType> gameTypes, bool tourney)
        {
            var typesCash = gameTypes.Where(gameType => !gameType.IsTourney)
                .Aggregate(string.Empty, (current, gameType) => current + (current == string.Empty ? gameType.Id.ToString(CultureInfo.InvariantCulture) : "," + gameType.Id));
            var typesTourney = gameTypes.Where(gameType => gameType.IsTourney)
                .Aggregate(string.Empty, (current, gameType) => current + (current == string.Empty ? gameType.Id.ToString(CultureInfo.InvariantCulture) : "," + gameType.Id));
            if (!tourney)
            {
                return
                    _databaseFunctions.GetInt(
                        @"select cast(count(*) as int) from (select distinct player.id_player
                            from player inner join cash_cache on player.id_player = cash_cache.id_player
                            inner join cash_limit on cash_limit.id_limit = cash_cache.id_limit
                            where id_site = " + (int)_ptPokerSite + @" and cash_limit.id_limit in (" +
                        typesCash + ") group by player.id_player) as counter");
            }
            return
                _databaseFunctions.GetInt(
                    @"select cast(count(*) as int) from (select distinct player.id_player
                            from player inner join tourney_cache on player.id_player = tourney_cache.id_player
                            inner join tourney_blinds on tourney_blinds.id_blinds = tourney_cache.id_blinds
                            where id_site = " + (int)_ptPokerSite + @" and tourney_blinds.id_blinds in (" +
                   typesTourney + ") group by player.id_player) as counter");
        }

        public IEnumerable<Player> ReadPlayer()
        {
            var compiledResults = new List<CompiledResultsMonth>();
            compiledResults.AddRange(ReadCompiledResultsCash());
            compiledResults.AddRange(ReadCompiledResultsTourney());
            return ConvertFromCompiledResults(compiledResults);
        }

        public IEnumerable<Player> ReadPlayer(IList<GameType> gameTypes, bool tourney)
        {
            var gameTypesTourney = gameTypes.Where(t => t.IsTourney).Select(gameType => gameType.Id).ToList();
            var gameTypesCash = gameTypes.Where(t => !t.IsTourney).Select(gameType => gameType.Id).ToList();
            var compiledResults = new List<CompiledResultsMonth>();

            if (tourney)
            {
                if (gameTypesTourney.Any())
                    compiledResults.AddRange(ReadCompiledResultsTourney().Where(t => gameTypesTourney.Contains(t.GameTypeId)));
                return ConvertFromCompiledResults(compiledResults);
            }
            compiledResults.AddRange(ReadCompiledResultsCash().Where(t => gameTypesCash.Contains(t.GameTypeId)));
            return ConvertFromCompiledResults(compiledResults);
        }
        
        private IEnumerable<CompiledResultsMonth> ReadCompiledResultsCash()
        {
            //holdem
            return _databaseFunctions.GetCompiledResultsMonth(
                @"  select cash_cache.id_player, player_name, id_limit, cnt_hands, cnt_vpip,
                    cnt_pfr, cast(amt_bb_won as float) as bbwon, cnt_steal_att + cnt_steal_def_action_call + cnt_steal_def_action_raise as bigblindstealattempted
                    ,cnt_steal_def_action_call + cnt_steal_def_action_raise as bigblindstealdefended
                    ,cnt_p_3bet_opp as couldthreebet
                    ,cnt_p_3bet as didthreebet
                    ,cnt_p_3bet_def_opp as facedthreebetpreflop
                    ,cnt_p_3bet_def_action_fold as foldedtothreebetpreflop 
                    from cash_cache  inner join player on player.id_player = cash_cache.id_player where id_site = " +
                (int)_ptPokerSite).ToList();
        }

        private IEnumerable<CompiledResultsMonth> ReadCompiledResultsTourney()
        {
            //holdem
            return _databaseFunctions.GetCompiledResultsMonth(
                @"  select tourney_cache.id_player, player_name, id_blinds, cnt_hands, cnt_vpip,
                    cnt_pfr, cast(amt_bb_won as float) as bbwon, cnt_steal_att + cnt_steal_def_action_call + cnt_steal_def_action_raise as bigblindstealattempted
                    ,cnt_steal_def_action_call + cnt_steal_def_action_raise as bigblindstealdefended
                    ,cnt_p_3bet_opp as couldthreebet
                    ,cnt_p_3bet as didthreebet
                    ,cnt_p_3bet_def_opp as facedthreebetpreflop
                    ,cnt_p_3bet_def_action_fold as foldedtothreebetpreflop 
                    from tourney_cache  inner join player on player.id_player = tourney_cache.id_player where id_site = " +
                (int)_ptPokerSite).ToList();
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
