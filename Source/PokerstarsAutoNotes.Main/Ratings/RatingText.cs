using System;
using System.Linq;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Ratings
{
    public class RatingText
    {
        public string Get(string autorateText, Player player)
        {
            var outtext = string.Empty;
            foreach (var definitionEnum in autorateText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).
                Select(tex => (DefinitionEnum)Enum.Parse(typeof(DefinitionEnum), tex)))
            {
                if (definitionEnum == DefinitionEnum.Hands)
                    outtext += "Hands: " + player.Hands + " ";
                if (definitionEnum == DefinitionEnum.Vpip)
                    outtext += "Vpip: " + player.Vpip + " ";
                if (definitionEnum == DefinitionEnum.Pfr)
                    outtext += "Pfr: " + player.Pfr + " ";
                if (definitionEnum == DefinitionEnum.Bb100)
                    outtext += "BB/100: " + player.BbWon + " ";
                if (definitionEnum == DefinitionEnum.VpipPfr)
                    outtext += "Vpip/Pfr: " + player.VpipPfrRatio + " ";
                if (definitionEnum == DefinitionEnum.BbFts)
                    outtext += "Bb FtoS: " + player.BbFoldToSteal + " ";
                if (definitionEnum == DefinitionEnum.Tbet)
                    outtext += "3-Bet: " + player.ThreeBet + " ";
                if (definitionEnum == DefinitionEnum.Ft3Bet)
                    outtext += "Fold 3-bet: " + player.FoldToThreeBet + " ";
            }
            if (outtext == string.Empty)
                return string.Empty;
            return "# AR: " + outtext + "#";
        }
    }
}
