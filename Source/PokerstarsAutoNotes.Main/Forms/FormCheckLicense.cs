using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PokerstarsAutoNotes.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FormCheckLicense : Form
    {
        private string _playerName;

        /// <summary>
        /// 
        /// </summary>
        public FormCheckLicense()
        {
            InitializeComponent();
        }

// ReSharper disable CSharpWarnings::CS1591
        public string PlayerName
// ReSharper restore CSharpWarnings::CS1591
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                InitializeText();
            }
        }

        private void InitializeText()
        {

            textBoxCheck.Text = @"Player: " + PlayerName;
        }
    }
}
