namespace PokerstarsAutoNotes.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Database
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool DoNotUseTrackerCache { get; set; }
    }
}
