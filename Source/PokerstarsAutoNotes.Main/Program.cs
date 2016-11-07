using System;
using System.Net;
using System.Windows.Forms;
using PokerstarsAutoNotes.Forms;
using PokerstarsAutoNotes.Infrastructure;
using PokerstarsAutoNotes.Ratings;

namespace PokerstarsAutoNotes
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IoC.Build();

            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }

            var notesfile = Properties.Settings.Default.NotesFile;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
