namespace PokerstarsAutoNotes.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseType
    {
        public string Name { get; set; }
        public DatabaseTypeEnum DatabaseTypeEnum { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
