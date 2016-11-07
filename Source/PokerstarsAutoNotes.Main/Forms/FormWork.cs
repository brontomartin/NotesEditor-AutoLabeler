using System;
using System.Reactive.Linq;
using System.Windows.Forms;
using PokerstarsAutoNotes.Infrastructure.Data;
using PokerstarsAutoNotes.Ratings;

namespace PokerstarsAutoNotes.Forms
{
    ///<summary>
    ///</summary>
    public partial class FormWork : Form
    {

        private bool _tenthonly;
        private int _counter;

        ///<summary>
        ///</summary>
        public FormWork()
        {
            InitializeComponent();
        }

        ///<summary />
        ///<param name="count"></param>
        public void Initialize(int count)
        {
            progressBarWorking.Maximum = count + 1;
        }

        ///<summary>
        ///</summary>
        ///<param name="ratings"></param>
        public void Subscribe(SinglePlayerRating ratings)
        {
            ratings.RatePlayer += PlayerRated;
        }

        ///<summary>
        ///</summary>
        ///<param name="databaseConnection"></param>
        public void Subscribe(IPokerDatabase databaseConnection)
        {
            databaseConnection.OnPlayerCompleted += Step;
        }

        public void InitMaximum(int max, EventArgs e)
        {
            progressBarWorking.Value = 0;
            progressBarWorking.Maximum = max;
            _tenthonly = max > 1000000;
        }

        public void Step()
        {
            if (progressBarWorking.Value < progressBarWorking.Maximum)
                progressBarWorking.Value += 1;
            if (!_tenthonly || _counter == 100)
            {
                Refresh();
                _counter = 0;
            }
            else
            {
                _counter ++;
            }
        }

        private void PlayerRated(SinglePlayerRating rating, EventArgs e)
        {
            if (progressBarWorking.Value < progressBarWorking.Maximum)
                progressBarWorking.Value += 1;
            Refresh();
        }

        private void FormWorkFormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
