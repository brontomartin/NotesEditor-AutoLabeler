using System.Collections.Generic;
using Machine.Specifications;
using PokerstarsAutoNotes.Model;
using PokerstarsAutoNotes.Xml.Definitions;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
namespace PokerStarsAutoNotes.Test.Ratings
{
    public class RatingDefinitionsReaderSpec
    {
        protected static RatingDefinitions reader;
        protected static IList<Rating> ratings;

        protected static void Initiate(string bla)
        {
            reader = new RatingDefinitions();
        }
    }
    
    [Subject("RatingDefinitionsReader")]
    public class when_xml_file_is_valid : RatingDefinitionsReaderSpec
    {
        Establish context = () => Initiate("");

        Because of = () => ratings = reader.Read(@"Ratings\Files\RatingDefintion.xml");

        It should_return_eight_ratings = () =>
            ratings.Count.ShouldEqual(8);
    }

    [Subject("RatingDefinitionsReader")]
    public class when_xml_file_is_invalid : RatingDefinitionsReaderSpec
    {
        Establish context = () => Initiate("");

        Because of = () => ratings = reader.Read(@"Ratings\Files\RatingDefintionCorrupt.xml");

        It should_return_zero_ratings = () =>
            ratings.ShouldBeNull();
    }
}
// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming