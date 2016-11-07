using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Infrastructure
{
    public class PlayersMapping
    {
        private readonly BlockingCollection<Player> _playersCollection;
        private readonly List<Task> _tasks;
        private readonly List<Player> _players;

        public PlayersMapping()
        {
            _playersCollection = new BlockingCollection<Player>();
            _tasks = new List<Task>();
            _players = new List<Player>();
        }

        public IList<Player> Players
        {
            get
            {
                Task.Factory.ContinueWhenAll(_tasks.ToArray(), _ => _players.AddRange(_playersCollection));
                return _players;
            }
        }

        public void AddMapPlayer(RawPlayerData playerData)
        {
            var task = Task.Factory.StartNew(() => _players.Add(MapPlayer(playerData)));
            _tasks.Add(task);
        }

        private Player MapPlayer(RawPlayerData playerData)
        {
            var player = new Player
            {
                Id = playerData.Id,
                Name = playerData.Name,
                Hands = playerData.Hands,
                Vpip = Math.Round(((double)playerData.VpipHands / playerData.Hands) * 100, 2),
                BbWon = Math.Round(playerData.BbWon / ((double)playerData.Hands / 100), 2),
                Pfr = Math.Round(((double)playerData.PfrHands / playerData.Hands) * 100, 2),
                BbFoldToSteal = Math.Round(((playerData.BigBlindStealAttempted - playerData.BigBlindStealDefended) / (double)playerData.BigBlindStealAttempted) * 100, 2),
                DoNotAutolabel = false,
                AutolabelDatabase = string.Empty
            };
            if (double.IsNaN(player.BbFoldToSteal))
                player.BbFoldToSteal = 0;
            player.VpipPfrRatio = Math.Round(player.Vpip / player.Pfr, 2);
            if (double.IsNaN(player.VpipPfrRatio) || double.IsInfinity(player.VpipPfrRatio) || player.VpipPfrRatio > 100)
                player.VpipPfrRatio = 100.0;
            return player;
        }
    }
}
