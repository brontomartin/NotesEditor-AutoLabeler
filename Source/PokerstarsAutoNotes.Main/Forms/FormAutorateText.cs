using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PokerstarsAutoNotes.Enums;

namespace PokerstarsAutoNotes.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FormAutorateText : Form
    {

        string _autorateTextConfig = string.Empty;
        string _autorateText = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public FormAutorateText()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            var autorateText = Properties.Settings.Default.AutorateText;
            listViewAutorate.Columns.Clear();
            listViewAutorate.Items.Clear();
            listViewAutorate.Columns.Add("Defintion",130);

            foreach (var lsvi in autorateText.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => (DefinitionEnum) Enum.Parse(typeof (DefinitionEnum), s))
                .SelectMany(def => (from DefinitionEnum definitionEnum in Enum.GetValues(typeof(DefinitionEnum)) where def == definitionEnum
                 select new ListViewItem
                {
                    Text = ItemText(definitionEnum),
                    Tag = definitionEnum,
                    Checked = true
                })))
            {
                listViewAutorate.Items.Add(lsvi);
            }

            foreach (DefinitionEnum definitionEnum in Enum.GetValues(typeof(DefinitionEnum)))
            {
                var exists = false;
                foreach (var item in listViewAutorate.Items.Cast<ListViewItem>()
                    .Where(item => item.Tag.ToString() == definitionEnum.ToString()))
                {
                    exists = true;
                }
                if (exists) continue;
                var lsvi = new ListViewItem
                {
                    Text = ItemText(definitionEnum),
                    Tag = definitionEnum
                };
                listViewAutorate.Items.Add(lsvi);
            }
            listViewAutorate.View = View.Details;
            labelExample.Text = CalculateText();
        }

        private void ButtonUpClick(object sender, EventArgs e)
        {
            if (listViewAutorate.SelectedItems.Count == 0)
            {
                MessageBox.Show(this, @"You must select a definition for move up!",
                                @"No definition selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            MoveItems(true);
            labelExample.Text = CalculateText();
        }

        private void ButtonDownClick(object sender, EventArgs e)
        {
            if (listViewAutorate.SelectedItems.Count == 0)
            {
                MessageBox.Show(this, @"You must select a definition for move down!",
                                @"No definition selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            MoveItems(false);
            labelExample.Text = CalculateText();
        }

        private void ListViewAutorateItemChecked(object sender, ItemCheckedEventArgs e)
        {
            labelExample.Text = CalculateText();
        }

        private ListViewItem MoveItems(bool up)
        {
            ListViewItem lviNewItem = null;
            try
            {
                var colIndexMove = new List<int>();
                var maxIndex = listViewAutorate.Items.Count - 1;
                if (up)
                    maxIndex = 0;
                foreach (ListViewItem selectedItem in listViewAutorate.SelectedItems)
                {
                    colIndexMove.Add(selectedItem.Index);
                    if (selectedItem.Index == maxIndex)
                        return null;
                }

                if (!up)
                    colIndexMove.Reverse();

                foreach (var i in colIndexMove)
                {
                    lviNewItem = (ListViewItem)listViewAutorate.Items[i].Clone();
                    listViewAutorate.Items[i].Remove();
                    if (up)
                        listViewAutorate.Items.Insert(i - 1, lviNewItem);
                    else
                        listViewAutorate.Items.Insert(i + 1, lviNewItem);
                    lviNewItem.Checked = !lviNewItem.Checked;
                    lviNewItem.Checked = !lviNewItem.Checked;
                    lviNewItem.Selected = true;
            }
            }
            finally
            {
                listViewAutorate.Focus();
            }
            return lviNewItem;
        }

        private string CalculateText()
        {
            _autorateText = string.Empty;
            _autorateTextConfig = string.Empty;
            foreach (var item in listViewAutorate.Items.Cast<ListViewItem>().Where(item => item.Checked))
            {
                _autorateText += _autorateText == string.Empty ? "# AR: " + item.Tag + ":" : " " + item.Tag + ":";
                _autorateTextConfig += _autorateTextConfig != string.Empty ? ";" + item.Tag : item.Tag;
            }
            if (_autorateText != string.Empty)
                _autorateText += " #";
            return _autorateText;
        }

        private string ItemText(DefinitionEnum definitionEnum)
        {
            return (int) definitionEnum == 1 ? "Hands"
                 : (int) definitionEnum == 2 ? "Vpip"
                 : (int) definitionEnum == 3 ? "Pfr"
                 : (int) definitionEnum == 4 ? "Vpip/Pfr Ratio"
                 : (int) definitionEnum == 5 ? "BB/100"
                 : (int)definitionEnum == 6 ? "BB Fold To Steal"
                 : (int)definitionEnum == 7 ? "3-Bet"
                 : "Fold To 3-Bet";

        }

        private void ButtonSaveClick(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutorateText = _autorateTextConfig;
            Properties.Settings.Default.Save();
            Close();
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
