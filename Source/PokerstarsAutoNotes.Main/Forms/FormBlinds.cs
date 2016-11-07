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
    public partial class FormBlinds : Form
    {

        public readonly List<string> Blinds = new List<string>();
        public readonly List<string> Games = new List<string>();
        public bool Canceled;

        private IList<GameType> _gameTypes;
        private bool _userAction;

        /// <summary>
        /// 
        /// </summary>
        public FormBlinds()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTypes"></param>
        public void Initialize(IEnumerable<GameType> gameTypes)
        {
            _gameTypes = gameTypes.ToList();
            var topGame = 25;
            var topLimit = 25;

                //buttonStandard.Visible = false;
                var tourneys = (from g in _gameTypes
                             where g.IsTourney
                             orderby g.ToShortString()
                             select g.ToShortString()).Distinct().ToList();

                var bb = (from g in _gameTypes
                          where g.IsTourney
                          orderby g.BigBlind
                          select g.BigBlind).Distinct().ToList();

                Height = Height + bb.Count() * 25;
                
                foreach (var checkBox in from game in tourneys
                                         where !(from object control in Controls
                                                 where control.GetType() == typeof(CheckBox)
                                                 select (CheckBox)control).Any(cont => cont.Name == "checkBoxGame" + game)
                                         select new CheckBox
                                         {
                                             Location = new Point(25, topGame),
                                             Name = "checkBoxGame" + game,
                                             Size = new Size(90, 20),
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

                foreach (var checkBox in from limit in bb
                                         where (from object control in Controls
                                                where control.GetType() == typeof(CheckBox)
                                                select (CheckBox)control).All(cont => cont.Name != "checkBoxLimit" + limit)
                                         select new CheckBox
                                         {
                                             Location = new Point(160, topLimit),
                                             Name = "checkBoxLimit" + limit,
                                             Size = new Size(90, 20),
                                             TabIndex = 0,
                                             Text = limit.ToString(CultureInfo.InvariantCulture),
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
            if (Blinds.Count == 0)
            {
                MessageBox.Show(this, @"You must select at least 1 blindlevel!",
                                @"No Blinds", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            if (Blinds.Count == 0)
            {
                MessageBox.Show(this, @"You must select at least 1 limit!",
                                @"No Blinds", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var standard = Games.Aggregate(string.Empty, (current, game) => current + (current == string.Empty ? game : ";" + game));
            standard = Blinds.Aggregate(standard, (current, limit) => current + (current == string.Empty ? limit : ";" + limit));
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
                Blinds.Add(checkBox.Tag.ToString());
            }
        }
    }
}
