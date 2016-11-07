using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Infrastructure;
using PokerstarsAutoNotes.Model;
using PokerstarsAutoNotes.Xml.Definitions;

// ReSharper disable LocalizableElement
// ReSharper disable CSharpWarnings::CS1591
namespace PokerstarsAutoNotes.Forms
{
    public partial class FormMain : Form
    {

        public void TextBoxStandardKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_userInteract) return;
            if ("1234567890.,\b".IndexOf(e.KeyChar.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) < 0)
            {
                e.Handled = true;
            }
        }

        public void TextBoxRatingTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Vpip, true);
        }

        public void TextBoxRatingToTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Vpip, false);
        }

        public void TextBoxRatingPfrTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Pfr, true);
        }

        public void TextBoxRatingPfrToTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Pfr, false);
        }

        public void TextBoxRatingVpipPfrTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.VpipPfr, true);
        }

        public void TextBoxRatingVpipPfrToTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.VpipPfr, false);
        }

        private void TextBoxRatingBbTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Bb100, true);
        }

        private void TextBoxRatingBbToTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Bb100, false);
        }

        private void TextBoxRatingBbKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_userInteract) return;
            if ("1234567890-.,\b".IndexOf(e.KeyChar.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) < 0)
            {
                e.Handled = true;
            }
        }

        private void TextBoxRatingBbFoldToStealTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.BbFts, true);
        }

        private void TextBoxRatingBbFoldToStealToTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.BbFts, false);
        }

        private void TextBoxRating3BetTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Tbet, true);
        }

        private void TextBoxRating3BetToTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Tbet, false);
        }

        private void TextBoxRatingFoldTo3BetTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Ft3Bet, true);
        }

        private void TextBoxRatingFoldTo3BetToTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Ft3Bet, false);
        }

        private void TextBoxRatingHandsTextChanged(object sender, EventArgs e)
        {
            ChangeRating(sender, DefinitionEnum.Hands, true);
        }

        private void TextBoxRatingHandsKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_userInteract) return;
            if ("1234567890kK\b".IndexOf(e.KeyChar.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) < 0)
            {
                e.Handled = true;
            }
        }

        private void TextBoxRatingsLeave(object sender, EventArgs e)
        {
            if (_ratingDefinitions.Validate(_ratings))
                    MessageBox.Show(this, @"There is a definition where the from value is greater than the to value, please correct"
                        , @"Invalid definition", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ChangeRating(object sender, DefinitionEnum definitionEnum, bool from)
        {
            if (!_userInteract) return;
            var textBox = (TextBox)sender;
            var rating = _ratings.FirstOrDefault(r => r.Index == (int)textBox.Tag && r.PokerSiteEnum == _pokersite);
            if (rating == null)
            {
                _ratings.Add(new Rating { Index = (int)textBox.Tag, PokerSiteEnum = _pokersite });
            }
            _ratings = _ratingDefinitions.Change(_ratings, (int)textBox.Tag, textBox.Text, definitionEnum, from, _pokersite);
            _unsavedDefinition = true;
            InitializeLabels();
        }
    }
}
// ReSharper restore CSharpWarnings::CS1591
// ReSharper restore LocalizableElement