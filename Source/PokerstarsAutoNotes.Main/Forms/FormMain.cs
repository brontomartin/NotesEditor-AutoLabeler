using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokerstarsAutoNotes.Configuration;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Extensions;
using PokerstarsAutoNotes.Infrastructure;
using PokerstarsAutoNotes.Infrastructure.Data;
using PokerstarsAutoNotes.Infrastructure.Database;
using PokerstarsAutoNotes.Model;
using PokerstarsAutoNotes.Ratings;
using PokerstarsAutoNotes.Resolver;
using PokerstarsAutoNotes.Tools;
using PokerstarsAutoNotes.Web;
using PokerstarsAutoNotes.Xml;
using PokerstarsAutoNotes.Xml.Definitions;
using PokerstarsAutoNotes.Xml.PartyPoker;
using Label = PokerstarsAutoNotes.Model.Label;


namespace PokerstarsAutoNotes.Forms
{

    // ReSharper disable LocalizableElement
    public partial class FormMain : Form
    {
        private readonly IRatingDefinitions _ratingDefinitions;
        private IPokersiteNotes _pokersiteNotes;

        private FormWork _formWork;

        public IList<Label> _labels;
        private IList<Player> _notes;
        private IList<Player> _notesFiltered;
        public IList<Rating> _ratings = new List<Rating>();
        private IList<GameType> _gametypes;
        private Players _players = new Players();
        private bool _userInteract;
        private Player _selected;
        Database _database;
        private bool _unsavedDefinition;
        private bool _unsavedLabels;
        private bool _unsavedNotes;
        private string _newVersion;
        private IPokerDatabase _pokerDatabase;
        public PokerSiteEnum _pokersite;

        /// <summary>
        /// 
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            //InitializeHelp();
            Text = string.Format(@"Note Editor and Autolabeler Vers. {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0,3));
            _ratingDefinitions = IoC.Resolve<IRatingDefinitions>();
#if DEBUG
            debugToolStripMenuItem.Visible = true;
#endif
        }

        private void InitializeMenu()
        {
            if (_gametypes == null)
                return;
            tourneyplayerByGametypeToolStripMenuItem.Enabled = false;
            cashplayerByGametypeToolStripMenuItem.Enabled = false;

            foreach (var gameType in _gametypes)
            {
                if (!gameType.IsTourney)
                {
                    cashplayerByGametypeToolStripMenuItem.Enabled = true;
                }
                else
                {
                    tourneyplayerByGametypeToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void InitializeListView(IEnumerable<Player> players )
        {

            listViewPlayer.BeginUpdate();
            listViewPlayer.Items.Clear();
            var listviewitems = new List<ListViewItem>();
            foreach (var player in players)
            {
                var listviewitem = new ListViewItem(player.Name, player.Label == null ? "" : player.Label.Index.ToString(CultureInfo.InvariantCulture));
                listviewitem.SubItems.Add(player.Note);
                listviewitem.SubItems.Add(player.Update.ToShortDateString());
                listviewitems.Add(listviewitem);
            }
            listViewPlayer.Items.AddRange(listviewitems.ToArray());
            listViewPlayer.EndUpdate();
            listViewPlayer.Refresh();
            labelPlayercount.Text = string.Format("{0} Player", _notes.Count);
        }

        private void InitializeLabels()
        {
            imageListLabels.Images.Clear();
            comboBoxLabels.Items.Clear();

            foreach (var label in _labels)
            {
                var bitmap = new Bitmap(16, 16);
                for (var x = 0; x < bitmap.Width - 1; x++)
                {
                    for (var y = 0; y < bitmap.Height - 1; y++)
                    {
                        bitmap.SetPixel(x, y, label.Color);
                    }
                }
                imageListLabels.Images.Add(label.Index.ToString(CultureInfo.InvariantCulture), bitmap);

                comboBoxLabels.MaxDropDownItems = _labels.Count;
                comboBoxLabels.DrawMode = DrawMode.OwnerDrawVariable;
                comboBoxLabels.ItemHeight = 18;
                comboBoxLabels.Items.Add(label.Name);
            }


            //InitializeListView();
            //foreach (var rating in new List<Rating>(_ratings)
            //    .Where(rating => _labels.FirstOrDefault(l => l.Index == rating.Index && rating.PokerSiteEnum == _pokersite) == null))
            //{
            //    _ratings.Remove(rating);
            //}
            if (_ratings != null)
            {
                foreach (
                    var rating in
                        new List<Rating>(_ratings).Where(
                            rating => rating.Bb100 == string.Empty && rating.Bb100To == string.Empty &&
                                      rating.Vpip == string.Empty && rating.VpipTo == string.Empty &&
                                      rating.Hands == string.Empty && rating.Pfr == string.Empty &&
                                      rating.PfrTo == string.Empty && rating.VpipPfrRatio == string.Empty &&
                                      rating.VpipPfrRatioTo == string.Empty
                                      && rating.BbFoldToSteal == string.Empty && rating.BbFoldToStealTo == string.Empty 
                                      && rating.ThreeBet == string.Empty && rating.ThreeBetTo == string.Empty
                                      && rating.FoldTo3Bet == string.Empty && rating.FoldTo3BetTo == string.Empty)
                    )
                {
                    _ratings.Remove(rating);
                }
            }

            listViewLabels.BeginUpdate();
            listViewLabels.Items.Clear();
            foreach (var label in _labels)
            {
                if (label.Name == "No Label")
                    continue;
                Rating rating = null;
                if (_ratings != null)
                    rating = _ratings.FirstOrDefault(x => x.Index == label.Index && x.PokerSiteEnum == _pokersite);
                var listviewitem = new ListViewItem("", imageListLabels.Images.IndexOfKey(label.Index.ToString(CultureInfo.InvariantCulture)))
                                       {Tag = label.Index};
                listviewitem.SubItems.Add(label.Name);
                listviewitem.SubItems.Add(rating == null ? "" : rating.VpipDouble + " - " + rating.VpipDoubleTo);
                listviewitem.SubItems.Add(rating == null ? "" : rating.PfrDouble + " - " + rating.PfrDoubleTo);
// ReSharper disable CompareOfFloatsByEqualityOperator
                listviewitem.SubItems.Add(rating == null ? "" : rating.VpipPfrDouble + " - " + (rating.VpipPfrDoubleTo == 100 ? "inf" : rating.VpipPfrDoubleTo.ToString(CultureInfo.InvariantCulture)));
// ReSharper restore CompareOfFloatsByEqualityOperator
                listviewitem.SubItems.Add(rating == null ? "" : rating.BbFoldToStealDouble + " - " + rating.BbFoldToStealDoubleTo);
                listviewitem.SubItems.Add(rating == null ? "" : rating.ThreeBetDouble + " - " + rating.ThreeBetDoubleTo);
                listviewitem.SubItems.Add(rating == null ? "" : rating.Fold3BetDouble + " - " + rating.Fold3BetDoubleTo);
                if (rating == null)
                    listviewitem.SubItems.Add("");
                else if (string.IsNullOrEmpty(rating.Bb100) && string.IsNullOrEmpty(rating.Bb100To))
                {
                    listviewitem.SubItems.Add("any");
                }
                else
                {
                    var textto = string.IsNullOrEmpty(rating.Bb100To) ? "inf" : rating.Bb100To;
                    var text = string.IsNullOrEmpty(rating.Bb100) ? "inf" : rating.Bb100;
                    listviewitem.SubItems.Add(text + " - " + textto);
                }
                if (rating == null)
                    listviewitem.SubItems.Add("");
                else if (string.IsNullOrEmpty(rating.Hands))
                    listviewitem.SubItems.Add("any");
                else
                    listviewitem.SubItems.Add(rating.Hands);
                listViewLabels.Items.Add(listviewitem);
            }
            listViewLabels.EndUpdate();
            listViewLabels.Refresh();

            _userInteract = true;

            ResumeLayout(false);
            PerformLayout();

            labelNotefile.Text = Properties.Settings.Default.NotesFile;
            labelDefinitionFile.Text = Properties.Settings.Default.DefinitionFile;
        }

        private void TextBoxLabelTextChanged(object sender, EventArgs e)
        {
            if (!_userInteract) return;
            var texBox = (TextBox) sender;
            var label = _labels.FirstOrDefault(l => l.Index == (int) texBox.Tag);
            if (label == null)
                return;
            label.Name = texBox.Text;
            label.Changed = true;
            _unsavedLabels = true;
            InitializeLabels();
        }
        
        private void ButtonLabelClick(object sender, EventArgs e)
        {
            var button = (Button) sender;
            if (colorDialogColors.ShowDialog() != DialogResult.OK) return;
            var label = _labels.FirstOrDefault(l => l.Index == (int) button.Tag);
            if (label == null)
                return;
            label.Color = colorDialogColors.Color;
            label.Changed = true;
            button.BackColor = colorDialogColors.Color;
            _unsavedLabels = true;
            InitializeLabels();
        }

        private void FormMainLoad(object sender, EventArgs e)
        {
            Database datenbank = null;

            try
            {
                var checkUpdates = Properties.Settings.Default.CheckUpdates;
                if (checkUpdates)
                {
                    checkForUpdatesToolStripMenuItem.Checked = true;
                    UpdateChecker();
                }
                textBoxSearch.Select();

                //iniatialize notes
                var notes = Properties.Settings.Default.NotesFile;
                if (string.IsNullOrEmpty(notes))
                {
                    MissingNotes(false);
                    return;
                }
                if (!File.Exists(notes))
                {
                    MissingNotes(true);
                    return;
                }
                _pokersiteNotes = IoC.Resolve<NoteFileResolver>().Resolve(notes);
                if (_pokersiteNotes.GetType() == typeof(PartyPokerNotes))
                {
                    ButtonLabel.Enabled = false;
                    buttonAddLabel.Enabled = false;
                    buttonDeleteLabel.Enabled = false;
                    TextBoxLabel.Enabled = false;
                    _pokersite = PokerSiteEnum.Partypoker;
                }
                else
                {
                    ButtonLabel.Enabled = true;
                    buttonAddLabel.Enabled = true;
                    buttonDeleteLabel.Enabled = true;
                    TextBoxLabel.Enabled = true;
                    _pokersite = PokerSiteEnum.Pokerstars;
                }

                //iniatialize database
                StartWorking(0);
                _database = new Database().Read() ?? new Database { Server = "localhost", Port = "5432" };
                _pokerDatabase = IoC.Resolve<DatabaseResolver>().Resolve(_database);
                if (_pokerDatabase != null)
                {
                    _pokerDatabase.PokerSite(_pokersite);
                    _gametypes = _pokerDatabase.ReadGametypes().ToList();
                }

                _ratings = _ratingDefinitions.Read(Properties.Settings.Default.DefinitionFile) ?? new List<Rating>();
                ReadLabelsPlayers();
                InitializeLabels();
                InitializeListView(_notes);
                InitializeMenu();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, @"Error while initializing", @"Error initialize", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var errorForm = new FormError(exception);
                errorForm.Show();
            }
            finally
            {
                StopWorking();
            }
        }


        private void ListViewPlayerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPlayer.SelectedItems.Count != 1) return;
            try
            {
                var dictionary = new Dictionary<string, Player>();
                foreach (var player in _notes)
                {
                    if (!dictionary.ContainsKey(player.Name))
                        dictionary.Add(player.Name, player);
                }
                //var dictionary = _notes.ToDictionary(player => player.Name);
                _selected = dictionary[listViewPlayer.SelectedItems[0].Text]; // _notes.Single(p => p.Name == listViewPlayer.SelectedItems[0].Text);
                groupBox1.Text = _selected.Name;
                _userInteract = false;
                textBoxNote.Text = _selected.Note;
                textBoxAutolabelDatabase.Text = _selected.AutolabelDatabase;
                checkBoxNoAutolabeling.Checked = _selected.DoNotAutolabel;
                if (_selected.Label == null)
                    _selected.Label = _labels.FirstOrDefault(s => s.Name == "No Label") ??
                                           new Label { Name = "No Label" };
                comboBoxLabels.SelectedItem = _selected.Label.Name;
                    //_notes.Single(p => p.Name == listViewPlayer.SelectedItems[0].Text).Label.Position;
                labelUpdate.Text = @"Update: " + _selected.Update;
                _userInteract = true;
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        private void ComboBoxLabelsDrawItem(object sender, DrawItemEventArgs e)
        {
            //if (!_userInteract) return;
            if (e.Index != -1)
            {
                var label = _labels[e.Index];
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.DrawBackground();
                e.DrawFocusRectangle();
                var rect = new Rectangle(new Point(e.Bounds.Left, e.Bounds.Top), new Size(comboBoxLabels.Width, 18));
                e.Graphics.DrawRectangle(new Pen(_labels.Single(p => p.Index == label.Index).Color), rect);
                e.Graphics.FillRectangle(new SolidBrush(_labels.Single(p => p.Index == label.Index).Color), rect);
                e.Graphics.DrawString(_labels.Single(p => p.Index == label.Index).Name, comboBoxLabels.Font, new SolidBrush(comboBoxLabels.ForeColor), new PointF(e.Bounds.Left, e.Bounds.Top));
            }
        }

        private void ComboBoxLabelsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_userInteract && _selected != null)
                UpdateRow(_selected, textBoxNote.Text, comboBoxLabels.SelectedItem.ToString());
        }

        private void TextBoxNoteTextChanged(object sender, EventArgs e)
        {
            if (_userInteract && _selected != null)
                UpdateRow(_selected, textBoxNote.Text, comboBoxLabels.SelectedItem.ToString());
        }

        private void TextBoxSearchTextChanged(object sender, EventArgs e)
        {

            var lvItem = listViewPlayer.FindItemWithText(textBoxSearch.Text, false, 0, true);
            if (lvItem != null)
                listViewPlayer.TopItem = lvItem;
        }

        private void TextBoxSearchEnter(object sender, EventArgs e)
        {
            textBoxSearch.Text = string.Empty;
        }


        private void UpdateRow(Player player, string note, string labelname)
        {
            var player1 = _notes.First(p => p.Name == player.Name);
            player1.Changed = true;
            player1.Update = DateTime.Now.AddSeconds(-DateTime.Now.Second);
            player1.Note = note;
            player1.Label = _labels.Single(p => p.Name == labelname);

            labelUpdate.Text = string.Format(@"Update: {0}", player1.Update);
            _players.Add(player1);

            var lvItem = listViewPlayer.FindItemWithText(player.Name, false, 0, true);
            if (lvItem != null)
            {
                lvItem.SubItems[1].Text = player1.Note;
                lvItem.SubItems[2].Text = player1.Update.ToShortDateString();
                _unsavedNotes = true;
                lvItem.ImageKey = player1.Label == null ? "" : player1.Label.Index.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void MissingNotes(bool wrong)
        {
            if (!wrong)
                MessageBox.Show(this, @"Please open a notesfile in the file menu first", @"No notes", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                MessageBox.Show(this, @"Your notesfile path is invalid, please open  a valid notesfile in the file menu first",
                                @"No valid notes", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Properties.Settings.Default.NotesFile = string.Empty;
            }
        }

        private bool CheckNotesAndRating()
        {
            if (Properties.Settings.Default.NotesFile == string.Empty)
            {
                MissingNotes(false);
                return false;
            }
            var ratings = new List<Rating>(_ratings);
            foreach (var rating in
                new List<Rating>(ratings).Where(
                            rating => rating.Bb100 == string.Empty && rating.Bb100To == string.Empty &&
                                      rating.Vpip == string.Empty && rating.VpipTo == string.Empty &&
                                      rating.Hands == string.Empty && rating.Pfr == string.Empty &&
                                      rating.PfrTo == string.Empty && rating.VpipPfrRatio == string.Empty &&
                                      rating.VpipPfrRatioTo == string.Empty
                                      && rating.BbFoldToSteal == string.Empty && rating.BbFoldToStealTo == string.Empty 
                                      && rating.ThreeBet == string.Empty && rating.ThreeBetTo == string.Empty
                                      && rating.FoldTo3Bet == string.Empty && rating.FoldTo3BetTo == string.Empty))
            {
                ratings.Remove(rating);
            }
            if (ratings.Count == 0)
            {
                MessageBox.Show(this, @"You do not have any defintions, please type your definitions first", @"No ratings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private void ButtonDeletePlayerClick(object sender, EventArgs e)
        {
            if (_selected == null) return;
            if (MessageBox.Show(this,string.Format(@"Are you sure to delete the player {0}", _selected.Name),
                    "Delete player", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            _players.Remove(_selected);
            _notes = _players.Playerslist;
            //InitializeListView(_notes);
            if (buttonFilter.Text == @"Filter Off")
            {
                _notesFiltered =
                    _notes.Where(player => player.Note.IndexOf(textBoxSearchInNotes.Text, StringComparison.Ordinal) >= 0).ToList();
                InitializeListView(_notesFiltered);
                labelPlayercount.Text = _notesFiltered.Count + @"Player";
            }
            else
            {
                InitializeListView(_notes);
                labelPlayercount.Text = _notes.Count + @"Player";
                textBoxSearchInNotes.Text = string.Empty;
            }

        }

        private void FormMainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_unsavedDefinition && !_unsavedLabels) return;

            if (_unsavedDefinition)
                if (MessageBox.Show(this, @"You have unsaved definitions, are you sure you want to close?",
                                @"Unsaved definitions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
            if (_unsavedLabels)
                if (MessageBox.Show(this, @"You have unsaved labels, are you sure you want to close?",
                                @"Unsaved labels", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;
            if (_unsavedNotes)
                if (MessageBox.Show(this, @"You have unsaved notes, are you sure you want to close?",
                                @"Unsaved labels", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;
        }

        private void CheckBoxNoAutolabelingCheckedChanged(object sender, EventArgs e)
        {
            if (_userInteract && _selected != null)
            {
                if (checkBoxNoAutolabeling.Checked)
                {
                    _selected.Note = "#!#" + _selected.Note;
                }
                else
                {
                    if (_selected.Note.StartsWith("#!#"))
                        _selected.Note = _selected.Note.Replace("#!#", "");
                }
                _selected.DoNotAutolabel = checkBoxNoAutolabeling.Checked;
                UpdateRow(_selected, _selected.Note, comboBoxLabels.SelectedItem.ToString());
            }
        }

        private void TextBoxAutolabelDatabaseTextChanged(object sender, EventArgs e)
        {
            if (_userInteract && _selected != null)
            {
                if (textBoxAutolabelDatabase.Text != string.Empty)
                {
                    if (_selected.Note.StartsWith("##"))
                    {
                        var match = Regex.Match(_selected.Note, "##.+!!");
                        _selected.Note = _selected.Note.Replace(match.ToString(), "");
                    }
                    _selected.Note = "##" + textBoxAutolabelDatabase.Text + "!!" + _selected.Note;
                }
                else
                {
                    if (_selected.Note.StartsWith("##"))
                    {
                        var match = Regex.Match(_selected.Note, "##.+!!");
                        _selected.Note = _selected.Note.Replace(match.ToString(), "");
                    }
                }
                _selected.AutolabelDatabase = textBoxAutolabelDatabase.Text;
                UpdateRow(_selected, _selected.Note, comboBoxLabels.SelectedItem.ToString());
            }
        }

        private void ListViewLabelsSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listViewLabels.SelectedItems.Count == 0)
                    return;
                var label = _labels.FirstOrDefault(x => x.Index == (int)listViewLabels.SelectedItems[0].Tag);
                Rating rating = null;
                if (_ratings != null)
                    rating = _ratings.FirstOrDefault(x => x.Index == (int)listViewLabels.SelectedItems[0].Tag && x.PokerSiteEnum == _pokersite);
                _userInteract = false;
                if (label != null)
                {
                    if (_pokersite == PokerSiteEnum.Partypoker)
                    {
                        ButtonLabel.Enabled = false;
                        TextBoxLabel.Enabled = false;
                    }
                    else
                    {
                        ButtonLabel.Enabled = true;
                        TextBoxLabel.Enabled = true;
                    }
                    ButtonLabel.Tag = label.Index;
                    ButtonLabel.BackColor = label.Color;
                    TextBoxLabel.Tag = label.Index;
                    TextBoxLabel.Text = label.Name;
                    TextBoxRating.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRating.Text = rating == null ? "" : rating.Vpip;
                    TextBoxRating.Enabled = true;
                    TextBoxRatingPfr.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingPfr.Text = rating == null ? "" : rating.Pfr;
                    TextBoxRatingPfr.Enabled = true;
                    TextBoxRatingBb.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingBb.Text = rating == null ? "" : rating.Bb100;
                    TextBoxRatingBb.Enabled = true;
                    TextBoxRatingTo.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingTo.Text = rating == null ? "" : rating.VpipTo;
                    TextBoxRatingTo.Enabled = true;
                    TextBoxRatingPfrTo.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingPfrTo.Text = rating == null ? "" : rating.PfrTo;
                    TextBoxRatingPfrTo.Enabled = true;
                    TextBoxRatingBbTo.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingBbTo.Text = rating == null ? "" : rating.Bb100To;
                    TextBoxRatingBbTo.Enabled = true;
                    TextBoxRatingHands.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingHands.Text = rating == null ? "" : rating.Hands;
                    TextBoxRatingHands.Enabled = true;
                    TextBoxRatingVpipPfr.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingVpipPfr.Text = rating == null ? "" : rating.VpipPfrRatio;
                    TextBoxRatingVpipPfr.Enabled = true;
                    TextBoxRatingVpipPfrTo.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingVpipPfrTo.Text = rating == null ? "" : rating.VpipPfrRatioTo;
                    TextBoxRatingVpipPfrTo.Enabled = true;
                    TextBoxRatingBbFoldToSteal.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingBbFoldToSteal.Text = rating == null ? "" : rating.BbFoldToSteal;
                    TextBoxRatingBbFoldToSteal.Enabled = true;
                    TextBoxRatingBbFoldToStealTo.Tag = rating == null ? label.Index : rating.Index;
                    TextBoxRatingBbFoldToStealTo.Text = rating == null ? "" : rating.BbFoldToStealTo;
                    TextBoxRatingBbFoldToStealTo.Enabled = true;
                    textBoxRating3Bet.Tag = rating == null ? label.Index : rating.Index;
                    textBoxRating3Bet.Text = rating == null ? "" : rating.ThreeBet;
                    textBoxRating3Bet.Enabled = true;
                    textBoxRating3BetTo.Tag = rating == null ? label.Index : rating.Index;
                    textBoxRating3BetTo.Text = rating == null ? "" : rating.ThreeBetTo;
                    textBoxRating3BetTo.Enabled = true;
                    textBoxRatingFoldTo3Bet.Tag = rating == null ? label.Index : rating.Index;
                    textBoxRatingFoldTo3Bet.Text = rating == null ? "" : rating.FoldTo3Bet;
                    textBoxRatingFoldTo3Bet.Enabled = true;
                    textBoxRatingFoldTo3BetTo.Tag = rating == null ? label.Index : rating.Index;
                    textBoxRatingFoldTo3BetTo.Text = rating == null ? "" : rating.FoldTo3BetTo;
                    textBoxRatingFoldTo3BetTo.Enabled = true;
                    _userInteract = true;
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }

        }

        private void ButtonAddLabelClick(object sender, EventArgs e)
        {
            _labels.Remove(_labels.First(l => l.Name == "No Label"));
            _labels.Add(new Label { Changed = true,
                                    Color = Color.Wheat,
                                    Index = _labels.Max(l => l.Index) + 1,
                                    Name = @"New Label " + (_labels.Max(l => l.Index) + 2),
                                    Position = _labels.Max(l => l.Position) + 1});
            _labels.Add(new Label { Color = Color.White, Index = _labels.Max(l => l.Index) + 1, Name = "No Label", Position = _labels.Max(l => l.Position) + 1});
            _unsavedLabels = true;
            InitializeLabels();
        }

        private void ButtonDeleteLabelClick(object sender, EventArgs e)
        {
            if (listViewLabels.SelectedItems.Count > 0)
            {
                var label = _labels.FirstOrDefault(x => x.Index == (int)listViewLabels.SelectedItems[0].Tag);
                if (label == null) return;
                var rating = _ratings.FirstOrDefault(x => x.Index == (int)listViewLabels.SelectedItems[0].Tag && x.PokerSiteEnum == _pokersite);
                if (rating != null)
                {
                    if (MessageBox.Show(this, @"The selected label has a definition, are you sure you want to delete this label?",
                                @"Label has definition", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
                }
                if (!_notes.Any(player => player.Label.Index == label.Index))
                {
                    RemoveLabelAndRating(rating);
                    return;
                }
                if (MessageBox.Show(this, @"The selected label is assigned to some players, are you sure you want to delete this label?",
                                    @"Label has players", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
                foreach (var player in _notes.Where(player => player.Label.Index == label.Index))
                {
                    player.Label = _labels.First(x => x.Name == "No Label");
                }
                RemoveLabelAndRating(rating);
                InitializeListView(_notes);
            }
            else
                MessageBox.Show(this, @"You must select a label",
                                @"No label selected", MessageBoxButtons.OK);
        }

        private void RemoveLabelAndRating(Rating rating)
        {
            _labels.Remove(
                _labels.FirstOrDefault(x => x.Index == (int)listViewLabels.SelectedItems[0].Tag));
            _unsavedLabels = true;
            if (rating != null)
            {
                _ratings.Remove(rating);
                _unsavedDefinition = true;
            }
            InitializeLabels();
            _userInteract = false;
            ButtonLabel.BackColor = DefaultBackColor;
            TextBoxLabel.Text = string.Empty;
            TextBoxRating.Text = string.Empty;
            TextBoxRatingTo.Text = string.Empty;
            TextBoxRatingPfr.Text = string.Empty;
            TextBoxRatingPfrTo.Text = string.Empty;
            TextBoxRatingBb.Text = string.Empty;
            TextBoxRatingBbTo.Text = string.Empty;
            TextBoxRatingHands.Text = string.Empty;
            TextBoxRatingVpipPfr.Text = string.Empty;
            TextBoxRatingVpipPfrTo.Text = string.Empty;
            TextBoxRatingBbFoldToSteal.Text = string.Empty;
            TextBoxRatingBbFoldToStealTo.Text = string.Empty;
            textBoxRating3Bet.Text = string.Empty;
            textBoxRating3BetTo.Text = string.Empty;
            textBoxRatingFoldTo3Bet.Text = string.Empty;
            textBoxRatingFoldTo3BetTo.Text = string.Empty;
            _userInteract = true;
        }

        private void ReadLabelsPlayers()
        {
            _labels = _pokersiteNotes.ReadLabels();
            _players = new Players { DefaultLabel = _labels.FirstOrDefault(s => s.Name == "No Label") };
            StartWorking(0);
            var plays = _pokersiteNotes.ReadPlayers(_labels).ToList();
            _players.Add(plays);
            _notes = _players.Playerslist;
            StopWorking();
        }

        private void TextBoxSearchInNotesEnter(object sender, EventArgs e)
        {
            textBoxSearchInNotes.Text = string.Empty;
            if (buttonFilter.Text == @"Filter Off")
            {
                buttonFilter.Text = @"Filter On";
                InitializeListView(_notes);
            }
        }

        private void ButtonFilterClick(object sender, EventArgs e)
        {
            if (buttonFilter.Text == @"Filter On")
            {
                buttonFilter.Text = @"Filter Off";
                _notesFiltered =
                    _notes.Where(player => player.Note.IndexOf(textBoxSearchInNotes.Text, StringComparison.Ordinal) >= 0).ToList();
                InitializeListView(_notesFiltered);
                labelPlayercount.Text = _notesFiltered.Count + @"Player";
            }
            else
            {
                buttonFilter.Text = @"Filter On";
                InitializeListView(_notes);
                labelPlayercount.Text = _notes.Count + @"Player";
                textBoxSearchInNotes.Text = string.Empty;
            }
        }

        private void LoadPlayersFromDatabase(IList<GameType> gameTypes, bool tourney, bool allPlayer)
        {
            try
            {
                if (Properties.Settings.Default.NotesFile == string.Empty)
                {
                    MissingNotes(false);
                    return;
                }
                if (_pokerDatabase == null)
                {
                    MessageBox.Show(this, @"You do not have a database, please setup database first", @"No database", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                _unsavedNotes = true;
                StartWorking(_notes.Count);
                //_formWork.Subscribe(_pokerDatabase);

                var players = allPlayer || gameTypes == null ? _pokerDatabase.ReadPlayer().ToList() : _pokerDatabase.ReadPlayer(gameTypes, tourney).ToList();
                _formWork.InitMaximum(players.Count(), null);

                var singlePlayerRating = IoC.Resolve<SinglePlayerRating>();
                singlePlayerRating.Parameters(_ratings.ToList(), _labels, _database, _pokersite, Properties.Settings.Default.AutorateText);

                _players.DefaultLabel = _labels.FirstOrDefault(s => s.Name == "No Label");
                foreach (var player in players)
                {
                    player.Label = _players.DefaultLabel;
                    singlePlayerRating.Rate(player);
                    _players.Add(player);
                    _formWork.Step();
                }
                _notes = _players.Playerslist;

                InitializeListView(_notes);
                StopWorking();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, @"Error while reading playerstats from the database", @"Error reading from database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var errorForm = new FormError(exception);
                errorForm.Show();
                return;
            }
            MessageBox.Show(this, @"Playerstats successful retrieved from database", @"Reading successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateChecker()
        {
            _newVersion = new UpdateCheck().Check();
            if (_newVersion != string.Empty)
            {
                MessageBox.Show(this,string.Format(@"A new version {0} is available, please go to help menu to download the newest version", _newVersion), @"New Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
                downloadNewVersionToolStripMenuItem.Visible = true;
            }
            else
            {
                downloadNewVersionToolStripMenuItem.Visible = false;
            }
        }

        private void InsertFakePlayersToolStripMenuItemClick(object sender, EventArgs e)
        {
            _pokerDatabase.CreateFakePlayer(1000000);
            MessageBox.Show(this, "Players inserted", "Players inserted", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AsyncLoadPlayers()
        {
            var task = Task.Factory.StartNew(() =>
                                                 {

                                                 });
        }

        private void StartWorking(int count)
        {
            if (_formWork != null)
                return;
            _formWork = new FormWork();
            _formWork.Initialize(count);
            _formWork.Show(this);
        }

        private void StopWorking()
        {
            if (_formWork == null)
                return;
            _formWork.Close();
            _formWork.Dispose();
            _formWork = null;
        }

        private void ReadPlayersToolStripMenuItemClick(object sender, EventArgs e)
        {
            var players = JsonSerializer.SerializeToJson(_pokerDatabase.ReadDbPlayers());
        }

        //private void UpdateUI(string item)
        //{
        //    if (Thread.CurrentThread.IsBackground)
        //    {
        //        listEvents.Dispatcher.Invoke(new Action(() => //dispatch to UI Thread 
        //        {
        //            listEvents.Items.Add(item);
        //        }));
        //    }
        //    else
        //    {
        //        listEvents.Items.Add(item);
        //    }
        //}
    }
}
// ReSharper restore LocalizableElement
