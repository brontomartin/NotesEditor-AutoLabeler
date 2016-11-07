using System;
using System.Drawing;

namespace PokerstarsAutoNotes.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Label : IComparable<Label>
    {
#pragma warning disable 1591
        public int Index { get; set; }
        public int Position { get; set; }
        public Color Color { get; set; }
        public string Name { get; set; }
        public bool Changed { get; set; }
#pragma warning restore 1591

        /// <summary>
        /// Vergleicht das aktuelle Objekt mit einem anderen Objekt desselben Typs.
        /// </summary>
        /// <returns>
        /// Ein Wert, der die relative Reihenfolge der verglichenen Objekte angibt.Der Rückgabewert hat folgende Bedeutung:Wert Bedeutung Kleiner als 0 (null) Dieses Objekt ist kleiner als der <paramref name="other"/>-Parameter.Zero Dieses Objekt ist gleich <paramref name="other"/>. Größer als 0 (null) Dieses Objekt ist größer als <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">Ein Objekt, das mit diesem Objekt verglichen werden soll.</param>
        public int CompareTo(Label other) { return Position.CompareTo(other.Position); }

        public override string ToString()
        {
            return Name;
        }
    }
}
