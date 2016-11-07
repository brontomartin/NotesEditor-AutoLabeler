namespace PokerstarsAutoNotes.Model
{
    public class RawPlayerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Hands { get; set; }
        public int VpipHands { get; set; }
        public double BbWon { get; set; }
        public int PfrHands { get; set; }
        public int BigBlindStealAttempted { get; set; }
        public int BigBlindStealDefended { get; set; }
    }
}
