using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PokerstarsAutoNotes.Enums;
using PokerstarsAutoNotes.Extensions;
using PokerstarsAutoNotes.Infrastructure;
using PokerstarsAutoNotes.Infrastructure.Database;
using PokerstarsAutoNotes.Model;
using PokerstarsAutoNotes.Ratings;
using PokerstarsAutoNotes.Resolver;
using PokerstarsAutoNotes.Web;
using PokerstarsAutoNotes.Xml.PartyPoker;
using PokerstarsAutoNotes.Xml.Pokerstars;

// ReSharper disable LocalizableElement
namespace PokerstarsAutoNotes.Forms
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// Exits the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void HelpToolStripMenuItem1Click(object sender, EventArgs e)
        {
            new FormHelp().ShowDialog(this);
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new FormAbout().ShowDialog(this);
        }
        
        /// <summary>
        /// Rate the Players if Vpip greater Zero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RateToolStripMenuItemClick(object sender, EventArgs e)
        {
            var check = CheckNotesAndRating();
            if (!check)
                return;
            _unsavedNotes = true;
            StartWorking(_notes.Count);
            var singlePlayerRating = IoC.Resolve<SinglePlayerRating>();
            singlePlayerRating.Parameters(_ratings.ToList(), _labels, _database, _pokersite, Properties.Settings.Default.AutorateText);
            _formWork.Subscribe(singlePlayerRating);
            IoC.Resolve<PlayersRating>().Rate(_notes);
            InitializeListView(_notes);
            StopWorking();
            MessageBox.Show(this, @"Labeling of the players is finished", @"Labeling finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Define the Notefile Location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadNotesToolStripMenuItemClick(object sender, EventArgs e)
        {
            openFileDialogNotes.Filter = "Notes (Notes*.txt; notes*.xml)|Notes*.txt;notes*.xml";
            saveFileDialogNotes.OverwritePrompt = false;
            if (openFileDialogNotes.ShowDialog() == DialogResult.OK)
            {
                _pokersiteNotes = IoC.Resolve<NoteFileResolver>().Resolve(openFileDialogNotes.FileName);
                if (!_pokersiteNotes.CheckXml(openFileDialogNotes.FileName))
                {
                    MessageBox.Show(this, @"The selected file is not a valid notesfile", @"Invalid notes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Properties.Settings.Default.NotesFile = openFileDialogNotes.FileName;
                Properties.Settings.Default.Save();
                ReadLabelsPlayers();
                if (_pokersiteNotes.GetType() == typeof(PartyPokerNotes))
                    _pokersite = PokerSiteEnum.Partypoker;
                else if (_pokersiteNotes.GetType() == typeof(PokerstarsNotes))
                    _pokersite = PokerSiteEnum.Pokerstars;

                if (_pokerDatabase != null)
                {
                    _pokerDatabase.PokerSite(_pokersite);
                    _gametypes = _pokerDatabase.ReadGametypes().ToList();
                }
                //_ratings = new RatingDefinitionsReader().Read(Properties.Settings.Default.DefinitionFile);
                InitializeLabels();
                InitializeListView(_notes);
                InitializeMenu();
            }
        }

        /// <summary>
        /// Saves the notesfile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveNotesToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.NotesFile == string.Empty)
            {
                MessageBox.Show(this, @"Please open notesfile in the file menu first", @"No notes", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var error = _pokersiteNotes.Write(Properties.Settings.Default.NotesFile, _notes, _labels);
            _unsavedNotes = false;
            if (!error)
                MessageBox.Show(this, @"Notes are saved", @"Notes saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, @"Error while saving notes, maybe the file is locked by another program", @"Error notes saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _unsavedLabels = false;
        }

        /// <summary>
        /// Save the notesfile as
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveNotesAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            switch (_pokersite)
            {
                case PokerSiteEnum.Partypoker:
                    saveFileDialogNotes.Filter = "PartyPoker Notes (Notes*.txt)|Notes*.txt";
                    break;
                case PokerSiteEnum.Pokerstars:
                    saveFileDialogNotes.Filter = "Pokerstars Notes (notes*.xml)|notes*.xml";
                    break;
            }

            saveFileDialogNotes.OverwritePrompt = true;
            saveFileDialogNotes.Title = "Save notes as";

            if (saveFileDialogNotes.ShowDialog(this) != DialogResult.OK) return;
            if (saveFileDialogNotes.FileName != string.Empty)
            {
                var error = _pokersiteNotes.Write(saveFileDialogNotes.FileName, _notes, _labels);
                _unsavedNotes = false;
                if (!error)
                    MessageBox.Show(this, @"Notes are saved", @"Notes saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(this, @"Error while saving notes, maybe the file is locked by another program", @"Error notes saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDatabaseToolStripMenuItemClick(object sender, EventArgs e)
        {
            var form = new FormDatabase();
            form.Init(_database);
            if (form.ShowDialog() == DialogResult.Cancel)
                return;
            _database = new Database().Read(); ;
            _pokerDatabase = IoC.Resolve<DatabaseResolver>().Resolve(_database);
            try
            {
                StartWorking(1);
                _pokerDatabase.PokerSite(_pokersite);
                _gametypes = _pokerDatabase.ReadGametypes().ToList();
                InitializeMenu();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, @"Error while reading gametypes from the database", @"Error reading from database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var errorForm = new FormError(exception);
                errorForm.Show();
            }
            finally
            {
                StopWorking();
            }
        }

        private void LoadDefinitionsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_unsavedDefinition)
                if (MessageBox.Show(this, @"You have unsaved definitions, are you sure you want to load other definitions?",
                                @"Unsaved definitions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            openFileDialogNotes.Filter = "Defintions (*Definition*.xml)|*Definition*.xml";
            if (openFileDialogNotes.ShowDialog(this) != DialogResult.OK) return;
            var ratings = _ratingDefinitions.Read(openFileDialogNotes.FileName);
            if (ratings != null)
            {
                Properties.Settings.Default.DefinitionFile = openFileDialogNotes.FileName;
                Properties.Settings.Default.Save();
                _ratings = _ratingDefinitions.Read(Properties.Settings.Default.DefinitionFile);
                InitializeLabels();
                _unsavedDefinition = false;
            }
            else
            {
                MessageBox.Show(this, @"Error while loading the definitions file",
                                @"Error loading definitions",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveDefinitionsToolStripMenuItemClick(object sender, EventArgs e)
        {
            saveFileDialogNotes.OverwritePrompt = true;
            if (Properties.Settings.Default.DefinitionFile == string.Empty)
            {
                MessageBox.Show(this, @"You did not have a definition file, use save as", @"No definition file",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show(this, @"Are you sure to overwrite the current definitions file?",
                    @"Overwrite definitions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            if (_ratingDefinitions.Write(_ratings, Properties.Settings.Default.DefinitionFile))
            {
                MessageBox.Show(this, @"Definitions are saved", @"Definitions saved", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                _unsavedDefinition = false;
            }
            else
                MessageBox.Show(this, @"Error while saving definitions", @"Error save definitions", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SaveDefinitionAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            saveFileDialogNotes.OverwritePrompt = true;
            saveFileDialogNotes.Title = @"Save definition as";
            saveFileDialogNotes.Filter = "Defintions (*Definition*.xml)|*Definition*.xml";
            if (saveFileDialogNotes.ShowDialog(this) == DialogResult.OK)
            {
                if (saveFileDialogNotes.FileName != string.Empty)
                {
                    if (MessageBox.Show(this, @"Do you want to use this definitionfile as standard definitions?",
                            @"Standard definitions", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        Properties.Settings.Default.DefinitionFile = saveFileDialogNotes.FileName;
                        Properties.Settings.Default.Save();
                        labelDefinitionFile.Text = saveFileDialogNotes.FileName;
                    }
                    if (_ratingDefinitions.Write(_ratings, saveFileDialogNotes.FileName))
                    {
                        MessageBox.Show(this, @"Definitions are saved", @"Definitions saved", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        _unsavedDefinition = false;
                    }
                    else
                        MessageBox.Show(this, @"Error while saving definitions", @"Error save definitions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AllPlayerToolStripMenuItemClick(object sender, EventArgs e)
        {
            LoadPlayersFromDatabase(null, false, true);
        }

        private void CashplayerByGametypeToolStripMenuItemClick(object sender, EventArgs e)
        {
            var formLimit = new FormLimits();
            if (_gametypes == null) return;
            foreach (var gameType in _gametypes)
            {
                if (gameType.Database != _database.Name)
                    _gametypes = _pokerDatabase.ReadGametypes().ToList();
                break;
            }
            formLimit.Initialize(_gametypes.ToList());
            formLimit.ShowDialog(this);
            var limits = formLimit.Limits;
            var games = formLimit.Games;
            var cancel = formLimit.Canceled;

            var gametypes = new List<GameType>();
            if (games.Count == 0 && limits.Count == 0 || cancel)
                return;
            foreach (var types in from game in games
                                  from limit in limits
                                  select (from g in _gametypes.ToList()
                                          where g.ToString() == game && g.Description == limit && !g.IsTourney
                                          select g))
            {
                gametypes.AddRange(types);
            }
            if (gametypes.Count > 0)
                LoadPlayersFromDatabase(gametypes, false, false);
            else
                MessageBox.Show("No players will be loaded from the database, no games apply to the selection", @"No games");
        }

        private void TourneyplayerByGametypeToolStripMenuItemClick(object sender, EventArgs e)
        {
            var formBlind = new FormBlinds();
            if (_gametypes == null) return;
            foreach (var gameType in _gametypes)
            {
                if (gameType.Database != _database.Name)
                    _gametypes = _pokerDatabase.ReadGametypes().ToList();
                break;
            }
            formBlind.Initialize(_gametypes.ToList());
            formBlind.ShowDialog(this);
            var games = formBlind.Games;
            var limits = formBlind.Blinds;
            var cancel = formBlind.Canceled;
            var gametypes = new List<GameType>();
            if (games.Count == 0 && limits.Count == 0 || cancel)
                return;
            foreach (var types in from game in games
                                  from limit in limits
                                  select (from g in _gametypes.ToList()
                                          where g.ToShortString() == game && g.BigBlind.ToString() == limit && g.IsTourney
                                          select g))
            {
                gametypes.AddRange(types);
            }
            if (gametypes.Count > 0)
                LoadPlayersFromDatabase(gametypes, true, false);
        }

        private void DownloadNewVersionToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;
            bool downloadcomplete = new UpdateCheck().Download(folderBrowserDialog.SelectedPath, "SetupAutoLabeler.msi");
            if (downloadcomplete)
            {
                MessageBox.Show(this, @"Download saved as " + folderBrowserDialog.SelectedPath + @"\SetupAutoLabeler.msi"
                    , @"Download complete", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                downloadNewVersionToolStripMenuItem.Visible = false;
            }
            else
            {
                MessageBox.Show(this, @"Download error", @"Download error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void CheckForUpdatesToolStripMenuItemClick(object sender, EventArgs e)
        {
            Properties.Settings.Default.CheckUpdates = checkForUpdatesToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            var checkUpdates = Properties.Settings.Default.CheckUpdates;
            if (checkUpdates)
                UpdateChecker();
        }

        private void AutorateTextToolStripMenuItemClick(object sender, EventArgs e)
        {
            new FormAutorateText().ShowDialog(this);
        }
    }
}
// ReSharper restore LocalizableElement