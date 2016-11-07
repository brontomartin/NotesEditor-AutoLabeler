using System;
using System.Windows.Forms;

namespace PokerstarsAutoNotes.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FormAbout : Form
    {
        ///<summary>
        ///</summary>
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            labelVersion.Text = @"Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
