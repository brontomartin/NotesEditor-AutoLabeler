using System;
using System.Windows.Forms;
using PokerstarsAutoNotes.Extensions;
using PokerstarsAutoNotes.Infrastructure;
using PokerstarsAutoNotes.Infrastructure.Database;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FormDatabase : Form
    {
        private Database _datenbank;
        private bool _userInteract;

        ///<summary>
        ///</summary>
        public FormDatabase()
        {
            InitializeComponent();
        }

        ///<summary>
        ///</summary>
        ///<param name="database"></param>
        public void Init(Database database)
        {
            _datenbank = database;
            if (database == null || string.IsNullOrEmpty(database.Server))
            {
                textBoxServer.Text = @"localhost";
                textBoxPort.Text = @"5432";
                _datenbank = new Database { Server = @"localhost", Port = @"5432" };
            }
            else
            {
                ReadDatabases();
                textBoxServer.Text = database.Server;
                textBoxPort.Text = database.Port;
                comboBoxDatabases.SelectedItem = database.Name;
                textBoxUsername.Text = database.Username;
                textBoxPassword.Text = database.Password;
            }
            _userInteract = true;
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            if (comboBoxDatabases.SelectedItem == null)
            {
                MessageBox.Show(this, @"Please select a database!", @"Select database", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                DialogResult = DialogResult.None;
                return;
            }
            if (string.IsNullOrEmpty(comboBoxDatabases.SelectedItem.ToString()))
            {
                MessageBox.Show(this, @"Please select a database!", @"Select database", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                DialogResult = DialogResult.None;
                return;
            }
            var database = new Database
                               {
                                   Server = textBoxServer.Text.Trim(),
                                   Port = textBoxPort.Text.Trim(),
                                   Name = comboBoxDatabases.SelectedItem.ToString(),
                                   Username = textBoxUsername.Text.Trim(),
                                   Password = textBoxPassword.Text.Trim(),
                               };
            database.Write();
            Close();
        }

        private void ReadDatabases()
        {
            if (_datenbank == null) return;
            if (string.IsNullOrEmpty(_datenbank.Server) || string.IsNullOrEmpty(_datenbank.Port) ||
                string.IsNullOrEmpty(_datenbank.Username) || string.IsNullOrEmpty(_datenbank.Password)) return;
            var databases = IoC.Resolve<AvailableDatabaseResolver>().Resolve(_datenbank);
            comboBoxDatabases.SelectedItem = null;
            comboBoxDatabases.BeginUpdate();
            comboBoxDatabases.Items.Clear();
            if (databases != null)
            {
                foreach (var database in databases)
                {
                    comboBoxDatabases.Items.Add(database);
                }
            }
            comboBoxDatabases.EndUpdate();
            comboBoxDatabases.PerformLayout();
        }

        private void TextBoxPasswordTextChanged(object sender, EventArgs e)
        {
            if (!_userInteract) return;
            _datenbank.Password = textBoxPassword.Text;
            ReadDatabases();
        }

        private void TextBoxUsernameTextChanged(object sender, EventArgs e)
        {
            if (!_userInteract) return;
            _datenbank.Username = textBoxUsername.Text;
            ReadDatabases();
        }

        private void TextBoxPortTextChanged(object sender, EventArgs e)
        {
            if (!_userInteract) return;
            _datenbank.Port = textBoxPort.Text;
            ReadDatabases();
        }

        private void TextBoxServerTextChanged(object sender, EventArgs e)
        {
            if (!_userInteract) return;
            _datenbank.Server = textBoxServer.Text;
            ReadDatabases();
        }

        private void ButtonBreakClick(object sender, EventArgs e)
        {
            Close();
        }

    }
}
