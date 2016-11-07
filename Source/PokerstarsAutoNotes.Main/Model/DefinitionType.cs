using PokerstarsAutoNotes.Enums;

namespace PokerstarsAutoNotes.Model
{
    public class DefinitionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public DefinitionEnum DefinitionEnum { get; set; }
        public string NumberType { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string HmSql { get; set; }
        public string PtSql { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
