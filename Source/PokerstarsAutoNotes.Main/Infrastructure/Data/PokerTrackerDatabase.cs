using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Infrastructure.Data
{
    public class PokerTrackerDatabase : IPokerDatabase
    {
        private readonly Model.Database _database;
        private readonly DatabaseFunctions _databaseFunctions;
        private PtPokerSite _ptPokerSite;

        public PokerTrackerDatabase(Model.Database database)
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

        public IEnumerable<DbPlayer> ReadDbPlayers()
        {
            return
                _databaseFunctions.RetrievePlayers(
                    "SELECT id_player, player_name, cast(id_site as int), id_player_alias FROM player where id_player > 0;");
        } 

        public IEnumerable<GameType> ReadGametypes()
        {
            var gametypes = _databaseFunctions.GetGametypes(@"select * from
                    (select distinct cast(holdem_limit.id_limit as int), replace(replace(replace(limit_name, ' (6 max)',''), ' CAP', ''), ' (2 max)', '') as limitname, cast(0 as boolean),
                    cast(1 as int) as pokergame, case when flg_nlpl = 'f' then cast(2 as int) when flg_nlpl = 't' and limit_name like '%PL%' then cast(1 as int) else cast(0 as int) end as pokergametype
                    , case when limit_name like '%2 max%' then 2 when limit_name like '%6 max%' then 1 else 0 end As players,
                    cast(case when limit_name like '%ZOOM%' THEN 1 else 0 end as boolean) as isZoom, cast(amt_bb as int) as bigblind from holdem_limit 
                    where id_limit in (SELECT distinct id_limit FROM holdem_hand_summary where id_site = " + (int)_ptPokerSite +
                    @") union select distinct omaha_limit.id_limit, replace(replace(replace(limit_name, ' (6 max)',''), ' CAP', ''), ' (2 max)', '') as limitname, cast(0 as boolean),
                    case when flg_lo = 'f' then cast(2 as int) else cast(3 as int) end as pokergame, case when flg_nlpl = 'f' then cast(2 as int) when flg_nlpl = 't' and limit_name like '%PL%' then cast(1 as int) else cast(0 as int) end as pokergametype
                    , case when limit_name like '%2 max%' then 2 when limit_name like '%6 max%' then 1 else 0 end As players,
                    cast(case when limit_name like '%ZOOM%' THEN 1 else 0 end as boolean) as isZoom, cast(amt_bb as int) as bigblind from omaha_limit
                    where id_limit in (SELECT distinct id_limit FROM omaha_hand_summary where id_site = " + (int)_ptPokerSite +
                    @") union select distinct tourney_holdem_blinds.id_blinds, '' as limitname, cast(1 as boolean),
                    cast(1 as int) as pokergame, case when flg_nlpl = 'f' then cast(2 as int) when flg_nlpl = 't' and blinds_name like '%PL%' then cast(1 as int) else cast(0 as int) end as pokergametype
                    , case when blinds_name like '%2 max%' then 2 when blinds_name like '%6 max%' then 1 else 0 end As players,
                    cast(0 as boolean) as Omaha, cast(amt_bb as int) as bigblind from tourney_holdem_blinds
                    where id_blinds in (SELECT distinct id_blinds FROM tourney_holdem_hand_summary where id_site = " + (int)_ptPokerSite +
                    @") union select distinct tourney_omaha_blinds.id_blinds, '' as limitname, cast(1 as boolean),
                    case when flg_lo = 'f' then 2 else 3 end as pokergame, case when flg_nlpl = 'f' then cast(2 as int) when flg_nlpl = 't' and blinds_name like '%PL%' then cast(1 as int) else cast(0 as int) end as pokergametype
                    , case when blinds_name like '%2 max%' then 2 when blinds_name like '%6 max%' then 1 else 0 end As players,
                    cast(1 as boolean) as Omaha, cast(amt_bb as int) as bigblind from tourney_omaha_blinds
                   where id_blinds in (SELECT distinct id_blinds FROM tourney_omaha_hand_summary where id_site = " + (int)_ptPokerSite + @" )) As Call
                    order by  limitname, pokergame, pokergametype, players");

            foreach (var gameType in gametypes)
            {
                gameType.Database = _database.Name;
                gameType.Description = gameType.Description.Replace(" ZOOM", string.Empty).Replace(" PL", "").Replace(" NL", "").Replace(" Hi/Lo", "").Replace(" Hi", "");
                if (gameType.Description.IndexOf("/", StringComparison.Ordinal) > 0)
                    gameType.Description = gameType.Description.Substring(gameType.Description.IndexOf("/", StringComparison.Ordinal) + 1, gameType.Description.Length - gameType.Description.IndexOf("/", StringComparison.Ordinal) - 1);
            }
            return gametypes;
        }

        public int Playercount()
        {
            return _databaseFunctions.GetInt(@"select cast(count(*) as int) from player where id_site = " + (int)_ptPokerSite);
        }

        public int Playercount(IList<GameType> gameTypes, bool tourney)
        {
            var typesOmaha = gameTypes.Where(gameType => gameType.PokerGame != PokerGame.HE)
                .Aggregate(string.Empty, (current, gameType) => current + (current == string.Empty ? gameType.Id.ToString(CultureInfo.InvariantCulture) : "," + gameType.Id));
            var typesHoldem = gameTypes.Where(gameType => gameType.PokerGame == PokerGame.HE)
                .Aggregate(string.Empty, (current, gameType) => current + (current == string.Empty ? gameType.Id.ToString(CultureInfo.InvariantCulture) : "," + gameType.Id));
            if (!tourney)
            {
                if (typesOmaha != string.Empty && typesHoldem != string.Empty)
                    return
                        _databaseFunctions.GetInt(
                            @"select cast(count(*) as int) from (select distinct player.id_player
                            from player inner join holdem_hand_player_statistics on player.id_player = holdem_hand_player_statistics.id_player
                            inner join holdem_limit on holdem_limit.id_limit = holdem_hand_player_statistics.id_limit
                            where id_site = " + (int)_ptPokerSite + @" and holdem_limit.id_limit in (" +
                            typesHoldem + ") group by player.id_player" +
                            @" union select distinct player.id_player
                            from player inner join omaha_hand_player_statistics on player.id_player = omaha_hand_player_statistics.id_player
                            inner join omaha_limit on omaha_limit.id_limit = omaha_hand_player_statistics.id_limit 
                            where id_site = " + (int)_ptPokerSite + @" and omaha_limit.id_limit in (" +
                            typesOmaha + ") group by player.id_player) as counter");
                if (typesOmaha != string.Empty)
                    return
                        _databaseFunctions.GetInt(
                            @"select cast(count(*) as int) from (select distinct player.id_player
                            from player inner join omaha_hand_player_statistics on player.id_player = omaha_hand_player_statistics.id_player
                            inner join omaha_limit on omaha_limit.id_limit = omaha_hand_player_statistics.id_limit 
                            where id_site = " + (int)_ptPokerSite + @" and omaha_limit.id_limit in (" +
                            typesOmaha + ") group by player.id_player) as counter");
                return
                    _databaseFunctions.GetInt(
                        @"select cast(count(*) as int) from (select distinct player.id_player
                            from player inner join holdem_hand_player_statistics on player.id_player = holdem_hand_player_statistics.id_player
                            inner join holdem_limit on holdem_limit.id_limit = holdem_hand_player_statistics.id_limit
                            where id_site = " + (int)_ptPokerSite + @" and holdem_limit.id_limit in (" +
                        typesHoldem + ") group by player.id_player) as counter");
            }
            if (typesOmaha != string.Empty && typesHoldem != string.Empty)
                return
                    _databaseFunctions.GetInt(
                        @"select cast(count(*) as int) from (select distinct player.id_player
                            from player inner join tourney_holdem_hand_player_statistics on player.id_player = tourney_holdem_hand_player_statistics.id_player
                            inner join tourney_holdem_blinds on tourney_holdem_blinds.id_blinds = tourney_holdem_hand_player_statistics.id_blinds
                            where id_site = " + (int)_ptPokerSite + @" and tourney_holdem_blinds.id_blinds in (" +
                        typesHoldem + ") group by player.id_player" +
                        @" union select distinct player.id_player
                            from player inner join tourney_omaha_hand_player_statistics on player.id_player = tourney_omaha_hand_player_statistics.id_player
                            inner join tourney_omaha_blinds on tourney_omaha_blinds.id_blinds = tourney_omaha_hand_player_statistics.id_blinds 
                            where id_site = " + (int)_ptPokerSite + @" and tourney_omaha_blinds.id_blinds in (" +
                        typesOmaha + ") group by player.id_player) as counter");
            if (typesOmaha != string.Empty)
                return
                    _databaseFunctions.GetInt(
                        @"select cast(count(*) as int) from (select distinct player.id_player
                            from player inner join tourney_omaha_hand_player_statistics on player.id_player = tourney_omaha_hand_player_statistics.id_player
                            inner join tourney_omaha_blinds on tourney_omaha_blinds.id_blinds = tourney_omaha_hand_player_statistics.id_blinds 
                            where id_site = " + (int)_ptPokerSite + @" and tourney_omaha_blinds.id_blinds in (" +
                        typesOmaha + ") group by player.id_player) as counter");
            return
                _databaseFunctions.GetInt(
                    @"select cast(count(*) as int) from (select distinct player.id_player
                            from player inner join tourney_holdem_hand_player_statistics on player.id_player = tourney_holdem_hand_player_statistics.id_player
                            inner join tourney_holdem_blinds on tourney_holdem_blinds.id_blinds = tourney_holdem_hand_player_statistics.id_blinds
                            where id_site = " + (int)_ptPokerSite + @" and tourney_holdem_blinds.id_blinds in (" +
                    typesHoldem + ") group by player.id_player) as counter");
        }

        public IEnumerable<Player> ReadPlayer(IList<GameType> gameTypes, bool tourney)
        {
            var gameTypesHoldemTourney = gameTypes.Where(t => t.IsTourney && t.PokerGame == PokerGame.HE).Select(gameType => gameType.Id).ToList();
            var gameTypesOmahaTourney = gameTypes.Where(t => t.IsTourney && t.PokerGame != PokerGame.HE).Select(gameType => gameType.Id).ToList();
            var gameTypesHoldemCash = gameTypes.Where(t => !t.IsTourney && t.PokerGame == PokerGame.HE).Select(gameType => gameType.Id).ToList();
            var gameTypesOmahaCash = gameTypes.Where(t => !t.IsTourney && t.PokerGame != PokerGame.HE).Select(gameType => gameType.Id).ToList();
            var compiledResults = new List<CompiledResultsMonth>();

            if (tourney)
            {
                if (gameTypesHoldemTourney.Any())
                    compiledResults.AddRange(ReadCompiledResultsHoldemTourney().Where(t => gameTypesHoldemTourney.Contains(t.GameTypeId)));
                if (gameTypesOmahaTourney.Any())
                    compiledResults.AddRange(ReadCompiledResultsOmahaTourney().Where(t => gameTypesOmahaTourney.Contains(t.GameTypeId)));
                return ConvertFromCompiledResults(compiledResults);
            }
            if (gameTypesHoldemCash.Any())
            {
                _database.DoNotUseTrackerCache = _databaseFunctions.GetInt(@"select cast(count(*) as int) from holdem_cache limit 1") == 0;
                compiledResults.AddRange(!_database.DoNotUseTrackerCache
                        ? ReadCompiledResultsHoldemCash().Where(t => gameTypesHoldemCash.Contains(t.GameTypeId))
                        : ReadCompiledResultsHoldemCash(gameTypesHoldemCash));
            }
            if (gameTypesOmahaCash.Any())
                compiledResults.AddRange(ReadCompiledResultsOmahaCash().Where(t => gameTypesOmahaCash.Contains(t.GameTypeId)));
            return ConvertFromCompiledResults(compiledResults);

        }


        public IEnumerable<Player> ReadPlayer()
        {
            _database.DoNotUseTrackerCache = _databaseFunctions.GetInt(@"select cast(count(*) as int) from holdem_cache limit 1") == 0;
            var gameTypes = _databaseFunctions.GetIntArray(@"select cast(id_limit as int) from holdem_limit").ToList();
            var compiledResults = new List<CompiledResultsMonth>();
            compiledResults.AddRange(!_database.DoNotUseTrackerCache
                                         ? ReadCompiledResultsHoldemCash()
                                         : ReadCompiledResultsHoldemCash(gameTypes));
            compiledResults.AddRange(ReadCompiledResultsOmahaCash());
            compiledResults.AddRange(ReadCompiledResultsHoldemTourney());
            compiledResults.AddRange(ReadCompiledResultsOmahaTourney());
            return ConvertFromCompiledResults(compiledResults);
        }

        private IEnumerable<CompiledResultsMonth> ReadCompiledResultsHoldemCash(IEnumerable<int> gametypes)
        {
            //holdem
            var limits = gametypes.Aggregate(string.Empty, (current, gametype) => current + (string.IsNullOrEmpty(current) ? gametype.ToString(CultureInfo.InvariantCulture) : ", " + gametype));
            return _databaseFunctions.GetCompiledResultsMonth(@"select id_player, player_name, cast(id_limit as int) ,cast(sum(hands) as int) as hands,
		        cast(sum(vpiphands) as int) as vpiphands, cast(sum(pfrhands) as int) as pfrhands,
                cast(sum(bbwon) as float) as bbwon,
                cast(sum(bigblindstealattempted) as int) as bigblindstealattempted,
                cast(sum(bigblindstealdefended) as int) as bigblindstealdefended,
                cast(sum(couldthreebet) as int) as couldthreebet,
                cast(sum(didthreebet) as int) as didthreebet,
                cast(sum(facedthreebetpreflop) as int) as facedthreebetpreflop,
                cast(sum(foldedtothreebetpreflop) as int) as foldedtothreebetpreflop from (
                select holdem_hand_player_statistics.id_player, player_name, holdem_limit.id_limit ,cast(count(id_hand) as int) as hands
		        ,sum(cast (flg_vpip as int)) as vpiphands
		        ,cast(sum(case when cnt_p_raise = 0 then 0 else 1 end)as int) as pfrhands
		        ,cast(sum(amt_won)/amt_bb as float)  as bbwon
		        ,sum(cast(case when flg_blind_def_opp = 'true' and flg_blind_b = true then 1 else 0 end as int)) as bigblindstealattempted
		        ,sum(cast(case when flg_blind_def_opp = 'true' and flg_blind_b = true then 1 else 0 end as int) 
                        - cast(flg_bb_steal_fold as int)) as bigblindstealdefended
		        ,sum(cast(flg_p_3bet_opp as int)) as couldthreebet
		        ,sum(cast(flg_p_3bet as int)) as didthreebet
		        ,sum(cast(flg_p_3bet_def_opp as int)) as facedthreebetpreflop
		        ,sum(cast(case when enum_p_3bet_action = 'F' then 1 else 0 end as int)) as foldedtothreebetpreflop
		        from holdem_hand_player_statistics inner join
                player on player.id_player = holdem_hand_player_statistics.id_player inner join holdem_limit on holdem_limit.id_limit = holdem_hand_player_statistics.id_limit
                group by holdem_hand_player_statistics.id_player, player_name, holdem_limit.id_limit, id_site, amt_bb
                having  id_site = " + (int)_ptPokerSite + " and holdem_limit.id_limit in (" + limits + ")) as summe group by id_player, player_name, id_limit").ToList();

        }

        private IEnumerable<CompiledResultsMonth> ReadCompiledResultsHoldemCash()
        {
            //holdem
            return _databaseFunctions.GetCompiledResultsMonth(
                @"select holdem_cache.id_player, player_name, id_limit, cnt_hands, cnt_vpip,
                    cnt_pfr, cast(amt_bb_won as float) * 2 as bbwon, cnt_steal_def_opp_bb as bigblindstealattempted
                    ,cnt_bb_steal_call + cnt_bb_steal_raise as bigblindstealdefended
                    ,cnt_p_3bet_opp as couldthreebet
                    ,cnt_p_3bet as didthreebet
                    ,cnt_p_3bet_def_opp as facedthreebetpreflop
                    ,cnt_p_3bet_def_action_fold as foldedtothreebetpreflop 
                    from holdem_cache inner join player on player.id_player = holdem_cache.id_player where id_site = " +
                (int)_ptPokerSite).ToList();
        }

        private IEnumerable<CompiledResultsMonth> ReadCompiledResultsOmahaCash()
        {
            //omaha
                return _databaseFunctions.GetCompiledResultsMonth(
                @"select omaha_cache.id_player, player_name, id_limit, cnt_hands, cnt_vpip,
                    cnt_pfr, cast(amt_bb_won as float) * 2 as bbwon, cnt_steal_def_opp_bb as bigblindstealattempted
                    ,cnt_bb_steal_call + cnt_bb_steal_raise as bigblindstealdefended
                    ,cnt_p_3bet_opp as couldthreebet
                    ,cnt_p_3bet as didthreebet
                    ,cnt_p_3bet_def_opp as facedthreebetpreflop
                    ,cnt_p_3bet_def_action_fold as foldedtothreebetpreflop 
                    from omaha_cache inner join player on player.id_player = omaha_cache.id_player where id_site = " +
                (int) _ptPokerSite).ToList();
        }

        private IEnumerable<CompiledResultsMonth> ReadCompiledResultsHoldemTourney()
        {
            //turnier holdem
                return _databaseFunctions.GetCompiledResultsMonth(
                @"select tourney_holdem_cache.id_player, player_name, id_blinds, cnt_hands, cnt_vpip,
                    cnt_pfr, cast(amt_bb_won as float) * 2 as bbwon, cnt_steal_def_opp_bb as bigblindstealattempted
                    ,cnt_bb_steal_call + cnt_bb_steal_raise as bigblindstealdefended
                    ,cnt_p_3bet_opp as couldthreebet
                    ,cnt_p_3bet as didthreebet
                    ,cnt_p_3bet_def_opp as facedthreebetpreflop
                    ,cnt_p_3bet_def_action_fold as foldedtothreebetpreflop 
                    from tourney_holdem_cache inner join player on player.id_player = tourney_holdem_cache.id_player where id_site = " +
                (int) _ptPokerSite).ToList();
        }

        private IEnumerable<CompiledResultsMonth> ReadCompiledResultsOmahaTourney()
        {
            //turnier omaha
                return _databaseFunctions.GetCompiledResultsMonth(
                    @"select tourney_omaha_cache.id_player, player_name, id_blinds, cnt_hands, cnt_vpip,
                    cnt_pfr, cast(amt_bb_won as float) * 2 as bbwon, cnt_steal_def_opp_bb as bigblindstealattempted
                    ,cnt_bb_steal_call + cnt_bb_steal_raise as bigblindstealdefended
                    ,cnt_p_3bet_opp as couldthreebet
                    ,cnt_p_3bet as didthreebet
                    ,cnt_p_3bet_def_opp as facedthreebetpreflop
                    ,cnt_p_3bet_def_action_fold as foldedtothreebetpreflop 
                    from tourney_omaha_cache inner join player on player.id_player = tourney_omaha_cache.id_player where id_site = " +
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
