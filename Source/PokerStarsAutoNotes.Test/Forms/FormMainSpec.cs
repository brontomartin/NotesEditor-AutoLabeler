using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Machine.Specifications;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Forms;
using PokerstarsAutoNotes.Model;
using Label = PokerstarsAutoNotes.Model.Label;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
namespace PokerStarsAutoNotes.Test.Forms
{
    [Subject("FormMain")]
    public class when_TextBoxStandardKeyPress_char_handled : with_FormMain
    {
        static KeyPressEventArgs keyPressEventArgs;

        Establish context = () => 
        { 
            Initialize();
            keyPressEventArgs = new KeyPressEventArgs('-');
        };

        Because of = () => _form.TextBoxStandardKeyPress(_textBox, keyPressEventArgs);

        It should_have_handled_true = () =>
           keyPressEventArgs.Handled.ShouldBeTrue();
    }

    [Subject("FormMain")]
    public class when_TextBoxStandardKeyPress_char_unhandled : with_FormMain
    {
        static KeyPressEventArgs keyPressEventArgs;

        Establish context = () =>
        {
            Initialize();
            keyPressEventArgs = new KeyPressEventArgs(',');
        };

        Because of = () => _form.TextBoxStandardKeyPress(_textBox, keyPressEventArgs);

        It should_have_handled_false = () =>
           keyPressEventArgs.Handled.ShouldBeFalse();
    }

    [Subject("FormMain")]
    public class when_TextBoxRatingTextChanged : with_FormMain
    {
        Establish context = () => Initialize();

        Because of = () => _form.TextBoxRatingTextChanged(_textBox, null);

        It should_have_one_rating = () =>
           _form._ratings.Count.ShouldEqual(1);

        It should_have_one_rating_with_vpip_ten = () =>
           _form._ratings[0].VpipDouble.ShouldEqual(10.0);

        It should_have_one_rating_with_vpipto_hundred = () =>
           _form._ratings[0].VpipDoubleTo.ShouldEqual(100.0);

        It should_have_one_rating_with_index_zero = () =>
           _form._ratings[0].Index.ShouldEqual(0);
    }

    [Subject("FormMain")]
    public class when_TextBoxRatingToTextChanged : with_FormMain
    {
        Establish context = () => Initialize();

        Because of = () => _form.TextBoxRatingToTextChanged(_textBox, null);

        It should_have_one_rating = () =>
           _form._ratings.Count.ShouldEqual(1);

        It should_have_one_rating_with_vpip_zero = () =>
           _form._ratings[0].VpipDouble.ShouldEqual(0.0);

        It should_have_one_rating_with_vpipto_ten = () =>
           _form._ratings[0].VpipDoubleTo.ShouldEqual(10.0);

        It should_have_one_rating_with_index_zero = () =>
           _form._ratings[0].Index.ShouldEqual(0);
    }

    [Subject("FormMain")]
    public class when_TextBoxRatingPfrTextChanged : with_FormMain
    {
        Establish context = () => Initialize();

        Because of = () => _form.TextBoxRatingPfrTextChanged(_textBox, null);

        It should_have_one_rating = () =>
           _form._ratings.Count.ShouldEqual(1);

        It should_have_one_rating_with_pfr_ten = () =>
           _form._ratings[0].PfrDouble.ShouldEqual(10.0);

        It should_have_one_rating_with_pfrto_hundred = () =>
           _form._ratings[0].PfrDoubleTo.ShouldEqual(100.0);

        It should_have_one_rating_with_index_zero = () =>
           _form._ratings[0].Index.ShouldEqual(0);
    }

    [Subject("FormMain")]
    public class when_TextBoxRatingPfrToTextChanged : with_FormMain
    {
        Establish context = () => Initialize();

        Because of = () => _form.TextBoxRatingPfrToTextChanged(_textBox, null);

        It should_have_one_rating = () =>
           _form._ratings.Count.ShouldEqual(1);

        It should_have_one_rating_with_pfr_zero = () =>
           _form._ratings[0].PfrDouble.ShouldEqual(0.0);

        It should_have_one_rating_with_pfrto_ten = () =>
           _form._ratings[0].PfrDoubleTo.ShouldEqual(10.0);

        It should_have_one_rating_with_index_zero = () =>
           _form._ratings[0].Index.ShouldEqual(0);
    }


    [Subject("FormMain")]
    public class when_TextBoxRatingVpipPfrTextChanged : with_FormMain
    {
        Establish context = () => Initialize();

        Because of = () => _form.TextBoxRatingVpipPfrTextChanged(_textBox, null);

        It should_have_one_rating = () =>
           _form._ratings.Count.ShouldEqual(1);

        It should_have_one_rating_with_vpippfr_ten = () =>
           _form._ratings[0].VpipPfrDouble.ShouldEqual(10.0);

        It should_have_one_rating_with_vpippfrto_hundred = () =>
           _form._ratings[0].VpipPfrDoubleTo.ShouldEqual(100.0);

        It should_have_one_rating_with_index_zero = () =>
           _form._ratings[0].Index.ShouldEqual(0);
    }

    [Subject("FormMain")]
    public class when_TextBoxRatingVpipPfrToTextChanged : with_FormMain
    {
        Establish context = () => Initialize();

        Because of = () => _form.TextBoxRatingVpipPfrToTextChanged(_textBox, null);

        It should_have_one_rating = () =>
           _form._ratings.Count.ShouldEqual(1);

        It should_have_one_rating_with_vpippfr_one = () =>
           _form._ratings[0].VpipPfrDouble.ShouldEqual(1.0);

        It should_have_one_rating_with_vpippfrto_ten = () =>
           _form._ratings[0].VpipPfrDoubleTo.ShouldEqual(10.0);

        It should_have_one_rating_with_index_zero = () =>
           _form._ratings[0].Index.ShouldEqual(0);
    }

    public class with_FormMain
    {
        protected static FormMain _form;
        protected static TextBox _textBox;

        protected static void Initialize()
        {
            _form = new FormMain {_pokersite = PokerSiteEnum.Pokerstars, _labels = new List<Label>()};
            _textBox = new TextBox {Tag = 0, Text = "10"};
        }
    }
}
// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming
