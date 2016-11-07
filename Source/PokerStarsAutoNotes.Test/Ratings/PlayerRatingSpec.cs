using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Infrastructure;
using PokerstarsAutoNotes.Model;
using PokerstarsAutoNotes.Ratings;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
namespace PokerStarsAutoNotes.Test.Ratings
{

    [Subject("SinglePlayerRating")]
    public class when_player_donotautolabel_true : with_playerrating
    {
        Establish context = () =>
                                {
                                    Initialize();
                                    _player.DoNotAutolabel = true;
                                    _player.Note = "# AR:fafasfafafaf#";
                                };

        Because of = () => _subject.Rate(_player);

        It should_set_autoratetext = () =>
            _player.Note.ShouldEqual("# AR: Hands: 30000 Vpip: 20 Pfr: 10 BB/100: 2,25 Vpip/Pfr: 2 Bb FtoS: 25 #");
    }

    [Subject("SinglePlayerRating")]
    public class when_player_have_text_but_label_without_rating : with_playerrating
    {
        Establish context = () =>
                                {
                                    Initialize();
                                    _player.DoNotAutolabel = false;
                                    _player.Note = "# AR:fafasfafafaf#";
                                    _player.Label = new Label {Index = 8, Name = "Label8"};
                                };

        Because of = () => _subject.Rate(_player);

        It should_set_autoratetext = () =>
            _player.Note.ShouldEqual("# AR: Hands: 30000 Vpip: 20 Pfr: 10 BB/100: 2,25 Vpip/Pfr: 2 Bb FtoS: 25 #");

        It should_have_label_index_eight = () =>
            _player.Label.Index.ShouldEqual(8);
    }

    [Subject("SinglePlayerRating")]
    public class when_only_one_rating_bbwon : with_playerrating
    {
        Establish context = () =>
                                {
                                    Initialize();
                                    _player.BbWon = 2.26;
                                    _player.Label = _label1;
                                };

        Because of = () => _subject.Rate(_player);

        It should_set_player_label_index_one = () =>
            _player.Label.Index.ShouldEqual(1);
    }

    [Subject("SinglePlayerRating")]
    public class when_only_one_rating_bbwon_but_player_bbwon_not_in_range : with_playerrating
    {
        Establish context = () =>
                                {
                                    Initialize();
                                    _player.BbWon = 2.24;
                                    _player.Label = _label1;
                                };

        Because of = () => _subject.Rate(_player);

        It should_set_player_label_index_zero = () =>
            _player.Label.Index.ShouldEqual(0);
    }

    public class with_playerrating
    {
        protected static SinglePlayerRating _subject;
        protected static Player _player;
        protected static Rating _ratingA;
        protected static Rating _ratingB;
        protected static Label _label0;
        protected static Label _label1;
        protected static Database _database;

        public static void Initialize()
        {
            _subject = new SinglePlayerRating();
            _player = new Player
                          {
                              BbWon = 2.25,
                              BbFoldToSteal = 25,
                              Hands = 30000,
                              Vpip = 20,
                              VpipPfrRatio = 2,
                              Pfr = 10,
                              AutolabelDatabase = string.Empty,
                          };

            _ratingA = new Rating
                           {
                               Index = 0,
                               Bb100 = "2,00",
                               Bb100To = "2,25",
                               PokerSiteEnum = PokerSiteEnum.Pokerstars
                           };

            _ratingB = new Rating
                          {
                              Index = 1,
                              Bb100 = "2,25",
                              Bb100To = "2,26",
                              PokerSiteEnum = PokerSiteEnum.Pokerstars
                          };

            _label0 = new Label
                         {
                             Index = 0
                         };

            _label1 = new Label
                        {
                            Index = 1
                        };

            _database = new Database {Name = "Testdatabase"};
            //_subject.Parameters(new List<Rating> {_ratingA, _ratingB}, new List<Label> {_label0, _label1}, _database, PokerSiteEnum.Pokerstars);
            IoC.Build();
        }
    }
}
// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming