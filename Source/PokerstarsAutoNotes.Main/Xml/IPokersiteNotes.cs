using System.Collections.Generic;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Xml
{
    public interface IPokersiteNotes
    {
        bool CheckXml(string notesfile);
        IList<Label> ReadLabels();
        IEnumerable<Player> ReadPlayers(IEnumerable<Label> labels);
        bool Write(string file, IEnumerable<Player> players, IEnumerable<Label> labels);
    }
}