namespace PokerstarsAutoNotes.Model
{
    public class DbField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string NumberType { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string FieldName { get; set; }
    }
}
