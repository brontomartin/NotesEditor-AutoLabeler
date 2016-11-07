using System.Collections.Generic;
using System.Linq;

namespace PokerstarsAutoNotes.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Players
    {
        private readonly Dictionary<string, Player> _players;
        private Label _defaultLabel;

        /// <summary>
        /// 
        /// </summary>
        public Players()
        {
            _players = new Dictionary<string, Player>();
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<Player> Playerslist
        {
            get
            {
                var players = _players.Values.ToList();
                players.Sort();
                return players;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Label DefaultLabel
        {
            get { return _defaultLabel; }
            set { _defaultLabel = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Add(Player player)
        {
            Player pl;
            if (!_players.TryGetValue(player.Name, out pl))
            {
                if (player.Label == null)
                    player.Label = _defaultLabel;
                if (player.Note == null)
                    player.Note = string.Empty;
                _players.Add(player.Name, player);
                return false;
            }
            pl.BbWon = player.BbWon;
            pl.Changed = player.Changed;
            pl.Exists = player.Exists;
            pl.Hands = player.Hands;
            pl.Id = player.Id;
            //pl.Label = player.Label;
            //pl.Name = player.Name;
            pl.Note = (!string.IsNullOrEmpty(player.Note) ? player.Note : pl.Note) ?? string.Empty ;
            pl.Pfr = player.Pfr;
            //pl.Update = player.Update;
            pl.Vpip = player.Vpip;
            pl.VpipPfrRatio = player.VpipPfrRatio;
            pl.BbFoldToSteal = player.BbFoldToSteal;
            pl.HeroVpip = player.HeroVpip;
            pl.HeroPfr = player.HeroPfr;
            pl.HeroVpipPfrRatio = player.HeroVpipPfrRatio;
            //pl.AutolabelDatabase = player.AutolabelDatabase ?? string.Empty;
            //pl.DoNotAutolabel = player.DoNotAutolabel;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public bool Add(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                Add(player);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public void Remove(Player player)
        {
            _players.Remove(player.Name);
        }
    }
}
