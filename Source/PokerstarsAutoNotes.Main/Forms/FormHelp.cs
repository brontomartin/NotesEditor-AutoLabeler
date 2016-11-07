using System;
using System.Reflection;
using System.Windows.Forms;

namespace PokerstarsAutoNotes.Forms
{
    ///<summary>
    ///</summary>
    public partial class FormHelp : Form
    {
        ///<summary>
        ///</summary>
        public FormHelp()
        {
            InitializeComponent();
        }

        private void FormHelp_Load(object sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(@"PokerstarsAutoNotes.Resources.Help.htm");
            webBrowserHelp.DocumentStream = stream;
        }
    }
}
