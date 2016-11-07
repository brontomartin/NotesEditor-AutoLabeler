using System;
using System.Linq;
using PokerstarsAutoNotes.Enums;

namespace PokerstarsAutoNotes.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Rating : IComparable<Rating>
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public string Vpip { get; set; }
        public string Bb100 { get; set; }
        public string Hands { get; set; }
        public string Pfr { get; set; }
        public string VpipTo { get; set; }
        public string Bb100To { get; set; }
        public string PfrTo { get; set; }
        public string VpipPfrRatio { get; set; }
        public string VpipPfrRatioTo { get; set; }
        public string BbFoldToSteal { get; set; }
        public string BbFoldToStealTo { get; set; }
        public string ThreeBet { get; set; }
        public string ThreeBetTo { get; set; }
        public string FoldTo3Bet { get; set; }
        public string FoldTo3BetTo { get; set; }
        public PokerSiteEnum PokerSiteEnum { get; set; }

        public double VpipDouble
        {
            get { return string.IsNullOrEmpty(Vpip) ? 0 : double.Parse(Vpip.Replace(".", ",")); }
        }

        public double VpipDoubleTo
        {
            get { return string.IsNullOrEmpty(VpipTo) ? 100 : double.Parse(VpipTo.Replace(".", ",")); }
        }

        public double PfrDouble
        {
            get { return string.IsNullOrEmpty(Pfr) ? 0 : double.Parse(Pfr.Replace(".", ",")); }
        }

        public double PfrDoubleTo
        {
            get { return string.IsNullOrEmpty(PfrTo) ? 100 : double.Parse(PfrTo.Replace(".", ",")); }
        }

        public double VpipPfrDouble
        {
            get { return string.IsNullOrEmpty(VpipPfrRatio) ? 1 : double.Parse(VpipPfrRatio.Replace(".", ",")); }
        }

        public double VpipPfrDoubleTo
        {
            get { return string.IsNullOrEmpty(VpipPfrRatioTo) ? 100 : double.Parse(VpipPfrRatioTo.Replace(".", ",")); }
        }

        public double BbFoldToStealDouble
        {
            get { return string.IsNullOrEmpty(BbFoldToSteal) ? 0 : double.Parse(BbFoldToSteal.Replace(".", ",")); }
        }

        public double BbFoldToStealDoubleTo
        {
            get { return string.IsNullOrEmpty(BbFoldToStealTo) ? 100 : double.Parse(BbFoldToStealTo.Replace(".", ",")); }
        }

        public double ThreeBetDouble
        {
            get { return string.IsNullOrEmpty(ThreeBet) ? 0 : double.Parse(ThreeBet.Replace(".", ",")); }
        }

        public double ThreeBetDoubleTo
        {
            get { return string.IsNullOrEmpty(ThreeBetTo) ? 100 : double.Parse(ThreeBetTo.Replace(".", ",")); }
        }

        public double Fold3BetDouble
        {
            get { return string.IsNullOrEmpty(FoldTo3Bet) ? 0 : double.Parse(FoldTo3Bet.Replace(".", ",")); }
        }

        public double Fold3BetDoubleTo
        {
            get { return string.IsNullOrEmpty(FoldTo3BetTo) ? 100 : double.Parse(FoldTo3BetTo.Replace(".", ",")); }
        }

        public double Bb100Double
        {
            get { return string.IsNullOrEmpty(Bb100) ? -100000 : double.Parse(Bb100.Replace(".", ",")); }
        }

        public double Bb100DoubleTo
        {
            get { return string.IsNullOrEmpty(Bb100To) ? 100000 : double.Parse(Bb100To.Replace(".", ",")); }
        }

        public double HandsDouble
        {
            get
            {
                if (string.IsNullOrEmpty(Hands))
                    return 0;
                var countk = Hands.Count(hand => hand == 'K' || hand == 'k');
                var hands = Hands.Replace("k", "").Replace("K", "");
                var val = double.Parse(hands.Replace(".", ","));
                if (countk == 0)
                    return val;
                for (int i = 1; i <= countk; i++)
                {
                    val = val*1000;
                }
                return val;
            }
        }

        /// <summary>
        /// Vergleicht das aktuelle Objekt mit einem anderen Objekt desselben Typs.
        /// </summary>
        /// <returns>
        /// Ein Wert, der die relative Reihenfolge der verglichenen Objekte angibt.Der Rückgabewert hat folgende Bedeutung:Wert Bedeutung Kleiner als 0 (null) Dieses Objekt ist kleiner als der <paramref name="other"/>-Parameter.Zero Dieses Objekt ist gleich <paramref name="other"/>. Größer als 0 (null) Dieses Objekt ist größer als <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">Ein Objekt, das mit diesem Objekt verglichen werden soll.</param>
        public int CompareTo(Rating other)
        {
            int comp = other.VpipDouble.CompareTo(VpipDouble);
            if (comp != 0)
                return comp;
            comp = other.PfrDouble.CompareTo(PfrDouble);
            if (comp != 0)
                return comp;
            comp = other.Bb100Double.CompareTo(Bb100Double);
            if (comp != 0)
                return comp;
            comp = other.VpipPfrDouble.CompareTo(VpipPfrDouble);
            if (comp != 0)
                return comp;
            comp = other.BbFoldToStealDouble.CompareTo(BbFoldToStealDouble);
            if (comp != 0)
                return comp;
            comp = other.ThreeBetDouble.CompareTo(ThreeBetDouble);
            if (comp != 0)
                return comp;
            comp = other.Fold3BetDouble.CompareTo(Fold3BetDouble);
            if (comp != 0)
                return comp;
            return other.HandsDouble.CompareTo(HandsDouble);
        }
    }
}
