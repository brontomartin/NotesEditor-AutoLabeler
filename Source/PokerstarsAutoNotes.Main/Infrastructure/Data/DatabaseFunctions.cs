using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Infrastructure.Data
{
    public class DatabaseFunctions : IDatabase
    {
        private readonly string _connectionString;

        public DatabaseFunctions(Model.Database database)
        {
            if (database == null)
                throw new ArgumentException("database");
            _connectionString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                database.Server, database.Port, database.Username, database.Password, database.Name);
        }

        public event Action OnPlayerLoadCompleted = delegate { };

        public bool CreateFakePlayer(int count)
        {
            string sql = @"INSERT INTO players(
            player_id, site_id, playername, lastplayeddate, cashhands, tourneyhands, 
            playertype_id)
            Select max(player_id)+1, 2, max(player_id)+1,max(lastplayeddate), max(cashhands), max(tourneyhands), 
            max(playertype_id) from players;
            INSERT INTO compiledresults_month(
            player_id, playedonmonth, numberofplayers, gametype_id, bbgroup_id, 
            totalplayedhands, compiledplayerresults_id)
            select max(player_id), '201108', 6, 1, 0, 2878,
            (select max(compiledplayerresults_id)+1 from compiledresults_month) from players;
            INSERT INTO compiledplayerresults_month(
            compiledplayerresults_id, totalhands, totalamountwon, totalrake, 
            totalbbswon, vpiphands, pfrhands, couldcoldcall, didcoldcall, 
            couldthreebet, didthreebet, couldsqueeze, didsqueeze, facingtwopreflopraisers, 
            calledtwopreflopraisers, raisedtwopreflopraisers, smallblindstealattempted, 
            smallblindstealdefended, smallblindstealreraised, bigblindstealattempted, 
            bigblindstealdefended, bigblindstealreraised, sawnonsmallshowdown, 
            wonnonsmallshowdown, sawlargeshowdown, wonlargeshowdown, sawnonsmallshowdownlimpedflop, 
            wonnonsmallshowdownlimpedflop, sawlargeshowdownlimpedflop, wonlargeshowdownlimpedflop, 
            wonhand, wonhandwhensawflop, wonhandwhensawturn, wonhandwhensawriver, 
            facedthreebetpreflop, foldedtothreebetpreflop, calledthreebetpreflop, 
            raisedthreebetpreflop, facedfourbetpreflop, foldedtofourbetpreflop, 
            calledfourbetpreflop, raisedfourbetpreflop, bigbetpreflopsawshowdown, 
            bigbetflopsawshowdown, bigbetturnsawshowdown, bigbetriversawshowdown, 
            bigcallpreflopsawshowdown, bigcallflopsawshowdown, bigcallturnsawshowdown, 
            bigcallriversawshowdown, bigbetpreflopwonshowdown, bigbetflopwonshowdown, 
            bigbetturnwonshowdown, bigbetriverwonshowdown, bigcallpreflopwonshowdown, 
            bigcallflopwonshowdown, bigcallturnwonshowdown, bigcallriverwonshowdown, 
            turnfoldippassonflopcb, turncallippassonflopcb, turnraiseippassonflopcb, 
            riverfoldippassonturncb, rivercallippassonturncb, riverraiseippassonturncb, 
            sawflop, sawshowdown, wonshowdown, totalbets, totalcalls, flopcontinuationbetpossible, 
            flopcontinuationbetmade, turncontinuationbetpossible, turncontinuationbetmade, 
            rivercontinuationbetpossible, rivercontinuationbetmade, facingflopcontinuationbet, 
            foldedtoflopcontinuationbet, calledflopcontinuationbet, raisedflopcontinuationbet, 
            facingturncontinuationbet, foldedtoturncontinuationbet, calledturncontinuationbet, 
            raisedturncontinuationbet, facingrivercontinuationbet, foldedtorivercontinuationbet, 
            calledrivercontinuationbet, raisedrivercontinuationbet, totalpostflopstreetsseen, 
            totalaggressivepostflopstreetsseen, vs_ep_raise_ip_fold, vs_ep_raise_ip_call, 
            vs_ep_raise_ip_raise, vs_mp_raise_ip_fold, vs_mp_raise_ip_call, 
            vs_mp_raise_ip_raise, vs_co_raise_ip_fold, vs_co_raise_ip_call, 
            vs_co_raise_ip_raise, vs_sb_raise_ip_fold, vs_sb_raise_ip_call, 
            vs_sb_raise_ip_raise, vs_ep_raise_oop_fold, vs_ep_raise_oop_call, 
            vs_ep_raise_oop_raise, vs_mp_raise_oop_fold, vs_mp_raise_oop_call, 
            vs_mp_raise_oop_raise, vs_co_raise_oop_fold, vs_co_raise_oop_call, 
            vs_co_raise_oop_raise, vs_bt_raise_oop_fold, vs_bt_raise_oop_call, 
            vs_bt_raise_oop_raise, ep_vs_raise_ip_fold, ep_vs_raise_ip_call, 
            ep_vs_raise_ip_raise, ep_vs_raise_oop_fold, ep_vs_raise_oop_call, 
            ep_vs_raise_oop_raise, mp_vs_raise_ip_fold, mp_vs_raise_ip_call, 
            mp_vs_raise_ip_raise, mp_vs_raise_oop_fold, mp_vs_raise_oop_call, 
            mp_vs_raise_oop_raise, co_vs_raise_ip_fold, co_vs_raise_ip_call, 
            co_vs_raise_ip_raise, co_vs_raise_oop_fold, co_vs_raise_oop_call, 
            co_vs_raise_oop_raise, bt_vs_raise_oop_fold, bt_vs_raise_oop_call, 
            bt_vs_raise_oop_raise, sb_vs_raise_ip_fold, sb_vs_raise_ip_call, 
            sb_vs_raise_ip_raise, facingsqueezeascaller, foldtosqueezeascaller, 
            facingsqueezeasfirstraiser, foldtosqueezeasfirstraiser, bbfacingsbcompletion, 
            bbraisesbcompletion, facingflopcheckraise, foldtoflopcheckraise, 
            facingturncheckraise, foldtoturncheckraise, facingrivercheckraise, 
            foldtorivercheckraise, facingflopcbetip, foldtoflopcbetip, raiseflopcbetip, 
            callflopcbetip, facingturncbetip, foldtoturncbetip, raiseturncbetip, 
            callturncbetip, facingrivercbetip, foldtorivercbetip, raiserivercbetip, 
            callrivercbetip, facingflopcbetoop, foldtoflopcbetoop, raiseflopcbetoop, 
            callflopcbetoop, facingturncbetoop, foldtoturncbetoop, raiseturncbetoop, 
            callturncbetoop, facingrivercbetoop, foldtorivercbetoop, raiserivercbetoop, 
            callrivercbetoop, cbetflopip, couldcbetflopip, cbetturnip, couldcbetturnip, 
            cbetriverip, couldcbetriverip, cbetflopoop, couldcbetflopoop, 
            cbetturnoop, couldcbetturnoop, cbetriveroop, couldcbetriveroop, 
            bbfacingsbsteal, bbfoldtosbsteal, bbraisesbsteal, bbcallsbsteal, 
            bbfacingbtnsteal, bbfoldtobtnsteal, bbraisebtnsteal, bbcallbtnsteal, 
            bbfacingcosteal, bbfoldtocosteal, bbraisecosteal, bbcallcosteal, 
            facedfivebetpreflop, foldedtofivebetpreflop, calledfivebetpreflop, 
            raisedfivebetpreflop)
SELECT (select max(compiledplayerresults_id) from compiledresults_month), 2878, totalamountwon, totalrake, 
       totalbbswon, vpiphands, pfrhands, couldcoldcall, didcoldcall, 
       couldthreebet, didthreebet, couldsqueeze, didsqueeze, facingtwopreflopraisers, 
       calledtwopreflopraisers, raisedtwopreflopraisers, smallblindstealattempted, 
       smallblindstealdefended, smallblindstealreraised, bigblindstealattempted, 
       bigblindstealdefended, bigblindstealreraised, sawnonsmallshowdown, 
       wonnonsmallshowdown, sawlargeshowdown, wonlargeshowdown, sawnonsmallshowdownlimpedflop, 
       wonnonsmallshowdownlimpedflop, sawlargeshowdownlimpedflop, wonlargeshowdownlimpedflop, 
       wonhand, wonhandwhensawflop, wonhandwhensawturn, wonhandwhensawriver, 
       facedthreebetpreflop, foldedtothreebetpreflop, calledthreebetpreflop, 
       raisedthreebetpreflop, facedfourbetpreflop, foldedtofourbetpreflop, 
       calledfourbetpreflop, raisedfourbetpreflop, bigbetpreflopsawshowdown, 
       bigbetflopsawshowdown, bigbetturnsawshowdown, bigbetriversawshowdown, 
       bigcallpreflopsawshowdown, bigcallflopsawshowdown, bigcallturnsawshowdown, 
       bigcallriversawshowdown, bigbetpreflopwonshowdown, bigbetflopwonshowdown, 
       bigbetturnwonshowdown, bigbetriverwonshowdown, bigcallpreflopwonshowdown, 
       bigcallflopwonshowdown, bigcallturnwonshowdown, bigcallriverwonshowdown, 
       turnfoldippassonflopcb, turncallippassonflopcb, turnraiseippassonflopcb, 
       riverfoldippassonturncb, rivercallippassonturncb, riverraiseippassonturncb, 
       sawflop, sawshowdown, wonshowdown, totalbets, totalcalls, flopcontinuationbetpossible, 
       flopcontinuationbetmade, turncontinuationbetpossible, turncontinuationbetmade, 
       rivercontinuationbetpossible, rivercontinuationbetmade, facingflopcontinuationbet, 
       foldedtoflopcontinuationbet, calledflopcontinuationbet, raisedflopcontinuationbet, 
       facingturncontinuationbet, foldedtoturncontinuationbet, calledturncontinuationbet, 
       raisedturncontinuationbet, facingrivercontinuationbet, foldedtorivercontinuationbet, 
       calledrivercontinuationbet, raisedrivercontinuationbet, totalpostflopstreetsseen, 
       totalaggressivepostflopstreetsseen, vs_ep_raise_ip_fold, vs_ep_raise_ip_call, 
       vs_ep_raise_ip_raise, vs_mp_raise_ip_fold, vs_mp_raise_ip_call, 
       vs_mp_raise_ip_raise, vs_co_raise_ip_fold, vs_co_raise_ip_call, 
       vs_co_raise_ip_raise, vs_sb_raise_ip_fold, vs_sb_raise_ip_call, 
       vs_sb_raise_ip_raise, vs_ep_raise_oop_fold, vs_ep_raise_oop_call, 
       vs_ep_raise_oop_raise, vs_mp_raise_oop_fold, vs_mp_raise_oop_call, 
       vs_mp_raise_oop_raise, vs_co_raise_oop_fold, vs_co_raise_oop_call, 
       vs_co_raise_oop_raise, vs_bt_raise_oop_fold, vs_bt_raise_oop_call, 
       vs_bt_raise_oop_raise, ep_vs_raise_ip_fold, ep_vs_raise_ip_call, 
       ep_vs_raise_ip_raise, ep_vs_raise_oop_fold, ep_vs_raise_oop_call, 
       ep_vs_raise_oop_raise, mp_vs_raise_ip_fold, mp_vs_raise_ip_call, 
       mp_vs_raise_ip_raise, mp_vs_raise_oop_fold, mp_vs_raise_oop_call, 
       mp_vs_raise_oop_raise, co_vs_raise_ip_fold, co_vs_raise_ip_call, 
       co_vs_raise_ip_raise, co_vs_raise_oop_fold, co_vs_raise_oop_call, 
       co_vs_raise_oop_raise, bt_vs_raise_oop_fold, bt_vs_raise_oop_call, 
       bt_vs_raise_oop_raise, sb_vs_raise_ip_fold, sb_vs_raise_ip_call, 
       sb_vs_raise_ip_raise, facingsqueezeascaller, foldtosqueezeascaller, 
       facingsqueezeasfirstraiser, foldtosqueezeasfirstraiser, bbfacingsbcompletion, 
       bbraisesbcompletion, facingflopcheckraise, foldtoflopcheckraise, 
       facingturncheckraise, foldtoturncheckraise, facingrivercheckraise, 
       foldtorivercheckraise, facingflopcbetip, foldtoflopcbetip, raiseflopcbetip, 
       callflopcbetip, facingturncbetip, foldtoturncbetip, raiseturncbetip, 
       callturncbetip, facingrivercbetip, foldtorivercbetip, raiserivercbetip, 
       callrivercbetip, facingflopcbetoop, foldtoflopcbetoop, raiseflopcbetoop, 
       callflopcbetoop, facingturncbetoop, foldtoturncbetoop, raiseturncbetoop, 
       callturncbetoop, facingrivercbetoop, foldtorivercbetoop, raiserivercbetoop, 
       callrivercbetoop, cbetflopip, couldcbetflopip, cbetturnip, couldcbetturnip, 
       cbetriverip, couldcbetriverip, cbetflopoop, couldcbetflopoop, 
       cbetturnoop, couldcbetturnoop, cbetriveroop, couldcbetriveroop, 
       bbfacingsbsteal, bbfoldtosbsteal, bbraisesbsteal, bbcallsbsteal, 
       bbfacingbtnsteal, bbfoldtobtnsteal, bbraisebtnsteal, bbcallbtnsteal, 
       bbfacingcosteal, bbfoldtocosteal, bbraisecosteal, bbcallcosteal, 
       facedfivebetpreflop, foldedtofivebetpreflop, calledfivebetpreflop, 
       raisedfivebetpreflop
  FROM compiledplayerresults_month where compiledplayerresults_id = 2983";
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                while (count >= 0)
                {
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.CommandTimeout = 360;
                        command.ExecuteScalar();
                    }
                    count--;
                }
            }
            return true;
        }

        public int GetInt(string sql)
        {
                using (var connection = new NpgsqlConnection(_connectionString))
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.CommandTimeout = 360;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            return 0;
        }

        public IEnumerable<int> GetIntArray(string sql)
        {
            var ids = new List<int>();
            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandTimeout = 360;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ids.Add(reader.GetInt32(0));
                    }
                }
            }
            return ids;
        }

        public string GetString(string sql)
        {
                using (var connection = new NpgsqlConnection(_connectionString))
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.CommandTimeout = 360;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            return null;
        }

        public IEnumerable<DbPlayer> RetrievePlayers(string sql)
        {
            var players = new List<DbPlayer>();
            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandTimeout = 1200;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var player = MapDbPlayer(reader);
                        players.Add(player);
                    }
                }
            }
            return players;
        }

        public IEnumerable<Player> GetPlayers(string sql)
        {
            var players = new List<Player>();
            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandTimeout = 1200;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var player = MapPlayer(reader);
                        players.Add(player);
                    }
                }
            }
            return players;
        }

        public IList<GameType> GetGametypes(string sql)
        {
            var gametypes = new List<GameType>();
            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandTimeout = 360;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var gametype = MapGametype(reader);
                        gametypes.Add(gametype);
                    }
                }
            }
            return gametypes;
        }

        public IEnumerable<DbField> GetDbFields(string sql)
        {
            var dbfields = new List<DbField>();
            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandTimeout = 360;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dbField = MapDbField(reader);
                        dbfields.Add(dbField);
                    }
                }
            }
            return dbfields;
        }

        public IEnumerable<CompiledResultsMonth> GetCompiledResultsMonth(string sql)
        {
            var compiledResultsMonth = new List<CompiledResultsMonth>();
            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandTimeout = 14400;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dbField = MapCompiledResultsMonth(reader);
                        compiledResultsMonth.Add(dbField);
                    }
                }
            }
            return compiledResultsMonth;
        }

        private static Player MapPlayer(IDataRecord dataReader)
        {
            var player = new Player();
            try
            {
                player = new Player
                {
                    Id = dataReader.GetInt32(0),
                    Name = dataReader.GetString(1),
                    Hands = dataReader.GetInt32(2),
                    Vpip = Math.Round(((double)dataReader.GetInt32(3) / dataReader.GetInt32(2)) * 100, 2),
                    BbWon = Math.Round(dataReader.GetDouble(4) / ((double)dataReader.GetInt32(2) / 100), 2),
                    Pfr = Math.Round(((double)dataReader.GetInt32(5) / dataReader.GetInt32(2)) * 100, 2),
                    BbFoldToSteal = Math.Round(((double)dataReader.GetInt32(6) / dataReader.GetInt32(7)) * 100, 2),
                    ThreeBet = Math.Round(((double)dataReader.GetInt32(9) / dataReader.GetInt32(8)) * 100, 2),
                    FoldToThreeBet = Math.Round(((double)dataReader.GetInt32(10) / dataReader.GetInt32(11)) * 100, 2),
                    DoNotAutolabel = false,
                    AutolabelDatabase = string.Empty
                };
#if DEBUG
                if (player.Name.EndsWith("823"))
                    player.Name = player.Name;
#endif

                if (double.IsNaN(player.BbFoldToSteal))
                    player.BbFoldToSteal = 0;
                if (double.IsNaN(player.FoldToThreeBet))
                    player.FoldToThreeBet = 0;
                if (double.IsNaN(player.ThreeBet))
                    player.ThreeBet = 0;
                player.VpipPfrRatio = Math.Round(player.Vpip / player.Pfr, 2);
                if (double.IsNaN(player.VpipPfrRatio) || double.IsInfinity(player.VpipPfrRatio) || player.VpipPfrRatio > 100)
                    player.VpipPfrRatio = 100.0;
                if (player.Name.Contains("Ã"))
                    player.Name = player.Name.Replace("Ã", "Í");
                return player;
            }
            catch (Exception)
            {
            }
            return player;
        }

        private static GameType MapGametype(IDataRecord reader)
        {
            var gameType = new GameType();
            gameType.Id = reader.GetInt32(0);
            gameType.Description = reader.GetString(1);
            gameType.IsTourney = reader.GetBoolean(2);
            gameType.PokerGame = (PokerGame) reader.GetInt32(3);
            gameType.PokerGameType = (PokerGameType) reader.GetInt32(4);
            gameType.PokerGamePlayer = (PokerGamePlayer) reader.GetInt32(5);
            gameType.IsZoom = reader.GetBoolean(6);
            gameType.BigBlind = reader.GetInt32(7);
            gameType.Description = gameType.Description.Replace("€", "").Replace("$", "").Replace("£", "");
            return gameType;
        }

        private static DbField MapDbField(IDataRecord reader)
        {
            var dbField = new DbField
                              {
                                  Id = reader.GetInt16(0),
                                  FieldName = reader.GetString(1),
                                  NumberType = reader.GetString(2)
                              };
            return dbField;
        }

        private static DbPlayer MapDbPlayer(IDataRecord reader)
        {
            var dbPlayer = new DbPlayer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                IdSite = reader.GetInt32(2),
                IdAlias = reader.GetInt32(3)
            };
            return dbPlayer;
        }

        private static CompiledResultsMonth MapCompiledResultsMonth(IDataRecord reader)
        {
            var result = new CompiledResultsMonth();
                result.PlayerId = reader.GetInt32(0);
                result.PlayerName = reader.GetString(1);
                result.GameTypeId = reader.GetInt32(2);
                result.TotalHands = reader.GetInt32(3);
                result.VpipHands = reader.GetInt32(4);
                result.PfrHands = reader.GetInt32(5);
                result.TotalBBsWon = reader.GetDouble(6);
                result.BigBlindStealAttempted = reader.GetInt32(7);
                result.BigBlindStealDefended = reader.GetInt32(8);
                result.CouldThreeBet = reader.GetInt32(9);
                result.DidThreeBet = reader.GetInt32(10);
                result.FacedThreeBetPreflop = reader.GetInt32(11);
                result.FoldedThreeBetPreflop = reader.GetInt32(12);
            return result;
        }
    }
}
