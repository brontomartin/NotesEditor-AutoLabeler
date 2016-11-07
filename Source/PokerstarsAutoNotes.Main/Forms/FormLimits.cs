using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PokerstarsAutoNotes.Model;

namespace PokerstarsAutoNotes.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FormLimits : Form
    {

        public readonly List<string> Limits = new List<string>();
        public readonly List<string> Games = new List<string>();
        public bool Canceled;

        private IEnumerable<GameType> _gameTypes;
        private bool _userAction;

        /// <summary>
        /// 
        /// </summary>
        public FormLimits()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTypes"></param>
        public void Initialize(IList<GameType> gameTypes)
        {
            _gameTypes = gameTypes;
            var topGame = 25;
            var topLimit = 25;
                textBoxZoom.Visible = gameTypes.Any(t => t.IsZoom);

                var games = (from g in _gameTypes
                         where !g.IsTourney
                         orderby g.ToString()
                         select g.ToString()).Distinct().ToList();

            var limits = (from g in _gameTypes
                          where !g.IsTourney
                          orderby g.Description
                          select g.Description).Distinct().ToList();

            var cntGames = games.Count();
            var cntLimits = limits.Count();
            if (cntGames > cntLimits)
            {
                Height = Height + cntGames * 30;
            }
            else
            {
                Height = Height + cntLimits * 30;
            }

                foreach (var checkBox in from game in games 
                             where !(from object control in Controls
                             where control.GetType() == typeof (CheckBox)
                             select (CheckBox) control).Any(cont => cont.Name == "checkBoxGame" + game)
                             select new CheckBox
                             {
                                Location = new Point(25, topGame),
                                Name = "checkBoxGame" + game,
                                Size = new Size(120, 20),
                                TabIndex = 0,
                                Text = game,
                                Tag = game,
                                UseVisualStyleBackColor = true
                             })
                {
                    checkBox.CheckedChanged += CheckBoxCheckedChanged;
                    Controls.Add(checkBox);
                    topGame += 25;
                }

                foreach (var checkBox in from limit in limits
                             where (from object control in Controls
                                    where control.GetType() == typeof(CheckBox)
                                    select (CheckBox)control).All(cont => cont.Name != "checkBoxLimit" + limit)
                             select new CheckBox
                             {
                                Location = new Point(160, topLimit),
                                Name = "checkBoxLimit" + limit,
                                Size = new Size(90, 20),
                                TabIndex = 0,
                                Text = limit,
                                Tag = limit,
                                UseVisualStyleBackColor = true
                             })
                {
                    checkBox.CheckedChanged += CheckBoxLimitCheckedChanged;
                    Controls.Add(checkBox);
                    topLimit += 25;
                }
            _userAction = false;
            var standard = Properties.Settings.Default.GameSelection;
            foreach (var checkbox in standard.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).
                SelectMany(game => (from object control in Controls
                                    where control.GetType() == typeof(CheckBox)
                                    select (CheckBox)control into checkbox
                                    where game == checkbox.Tag.ToString()
                                    select checkbox)))
            {
                checkbox.Checked = true;
            }
            _userAction = true;
            Height = Height + 10;
        }

        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!_userAction) return;
            var checkbox = (CheckBox)sender;
            if (checkbox.Name.Contains("checkBoxGame"))
            {
                var limits = (from g in _gameTypes
                              where !g.IsTourney && g.ToString() == checkbox.Text
                              orderby g.Description
                              select g.Description).Distinct();
                foreach (var check in limits.SelectMany(limit =>
                    (from object control in Controls
                     where control.GetType() == typeof(CheckBox)
                     select (CheckBox)control into check
                     where check.Name == "checkBoxLimit" + limit
                     select check)))
                {
                    check.Checked = checkbox.Checked;
                }
            }
            foreach (var checkb in from object control in Controls
                                   where control.GetType() == typeof(CheckBox)
                                   select (CheckBox)control into check
                                   where check.Name.Contains("checkBoxGame") && check.Checked
                                   select (from g in _gameTypes
                                           where !g.IsTourney && g.ToString() == check.Text
                                           orderby g.Description
                                           select g.Description).Distinct() into limits
                                   from checkb in
                                       limits.SelectMany(limit => (from object cont in Controls
                                                                   where cont.GetType() == typeof(CheckBox)
                                                                   select (CheckBox)cont into checkb
                                                                   where checkb.Name == "checkBoxLimit" + limit
                                                                   select checkb))
                                   select checkb)
            {
                checkb.Checked = true;
            }
        }

        private void CheckBoxLimitCheckedChanged(object sender, EventArgs e)
        {
            if (!_userAction) return;
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            Canceled = true;
            Close();
        }

        private void ButtonLoadClick(object sender, EventArgs e)
        {
            Selected();
            if (Limits.Count == 0)
            {
                MessageBox.Show(this, @"You must select at least 1 limit!",
                                @"No Limits", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!Games.Any())
            {
                MessageBox.Show(this, @"You must select at least 1 game!",
                                @"No Games", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Close();
        }

        private void ButtonStandardClick(object sender, EventArgs e)
        {
            Selected();
            if (Limits.Count == 0)
            {
                MessageBox.Show(this, @"You must select at least 1 limit!",
                                @"No Limits", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var standard = Games.Aggregate(string.Empty, (current, game) => current + (current == string.Empty ? game : ";" + game));
            standard = Limits.Aggregate(standard, (current, limit) => current + (current == string.Empty ? limit : ";" + limit));
            Properties.Settings.Default.GameSelection = standard;
            Properties.Settings.Default.Save();
        }

        private void Selected()
        {
            foreach (var checkBox in from object control in Controls
                                     where control.GetType() == typeof(CheckBox)
                                     select (CheckBox)control into checkBox
                                     where checkBox.Name.Contains("checkBoxGame") && checkBox.Checked
                                     select checkBox)
            {
                Games.Add(checkBox.Tag.ToString());
            }
            foreach (var checkBox in from object control in Controls
                                     where control.GetType() == typeof(CheckBox)
                                     select (CheckBox)control into checkBox
                                     where checkBox.Name.Contains("checkBoxLimit") && checkBox.Checked
                                     select checkBox)
            {
                Limits.Add(checkBox.Tag.ToString());
            }
        }
    }
}
