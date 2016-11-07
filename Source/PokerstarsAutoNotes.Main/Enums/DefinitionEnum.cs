using System.ComponentModel;

namespace PokerstarsAutoNotes.Enums
{
    ///<summary>
    ///</summary>
    public enum DefinitionEnum
    {
        [Description("Hands")]
        Hands = 1,
        [Description("Vpip")]
        Vpip = 2,
        [Description("Pfr")]
        Pfr = 3,
        [Description("VpipPfr")]
        VpipPfr = 4,
        [Description("Bb100")]
        Bb100 = 5,
        [Description("BbFts")]
        BbFts = 6,
        [Description("3-Bet")]
        Tbet = 7,
        [Description("FT3-Bet")]
        Ft3Bet = 8,

    }
}
