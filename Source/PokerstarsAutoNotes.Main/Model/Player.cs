using System;

namespace PokerstarsAutoNotes.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Player : IComparable<Player>
    {

#pragma warning disable 1591
        public int Id { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public Label Label { get; set; }
        public DateTime Update { get; set; }
        public double Vpip { get; set; }
        public bool Changed { get; set; }
        public bool Exists { get; set; }
        public double BbWon { get; set; }
        public int Hands { get; set; }
        public double Pfr { get; set; }
        public double VpipPfrRatio { get; set; }
        public double BbFoldToSteal { get; set; }
        public double FoldToThreeBet { get; set; }
        public double ThreeBet { get; set; }
        public bool DoNotAutolabel { get; set; }
        public string AutolabelDatabase { get; set; }

        //they see my stats like
        public double HeroVpip { get; set; }
        public double HeroPfr { get; set; }
        public double HeroVpipPfrRatio { get; set; }
        public double HeroBbFoldToSteal { get; set; }
#pragma warning restore 1591

        /// <summary>
        /// Vergleicht das aktuelle Objekt mit einem anderen Objekt desselben Typs.
        /// </summary>
        /// <returns>
        /// Ein Wert, der die relative Reihenfolge der verglichenen Objekte angibt.Der Rückgabewert hat folgende Bedeutung:Wert Bedeutung Kleiner als 0 (null) Dieses Objekt ist kleiner als der <paramref name="other"/>-Parameter.Zero Dieses Objekt ist gleich <paramref name="other"/>. Größer als 0 (null) Dieses Objekt ist größer als <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">Ein Objekt, das mit diesem Objekt verglichen werden soll.</param>
        public int CompareTo(Player other) { return Name.CompareTo(other.Name); }
    }
}
