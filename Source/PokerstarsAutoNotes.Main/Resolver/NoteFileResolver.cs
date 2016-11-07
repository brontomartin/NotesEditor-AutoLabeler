using PokerstarsAutoNotes.Xml;
using PokerstarsAutoNotes.Xml.PartyPoker;
using PokerstarsAutoNotes.Xml.Pokerstars;

namespace PokerstarsAutoNotes.Resolver
{
    public class NoteFileResolver
    {
        public IPokersiteNotes Resolve(string noteFileName)
        {
            if (noteFileName.EndsWith(".txt"))
            {
                return new PartyPokerNotes();
            }
            return new PokerstarsNotes();
        }
    }
}
