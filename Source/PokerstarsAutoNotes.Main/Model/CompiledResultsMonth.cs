namespace PokerstarsAutoNotes.Model
{
    public class CompiledResultsMonth
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GameTypeId { get; set; }
        public int TotalHands { get; set; }
        public int VpipHands { get; set; }
        public int PfrHands { get; set; }
        public double TotalBBsWon { get; set; }
        public int BigBlindStealAttempted { get; set; }
        public int BigBlindStealDefended { get; set; }
        public int CouldThreeBet { get; set; }
        public int DidThreeBet { get; set; }
        public int FacedThreeBetPreflop { get; set; }
        public int FoldedThreeBetPreflop { get; set; }
    }
}
