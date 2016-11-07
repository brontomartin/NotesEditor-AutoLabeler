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
    public partial class FormError : Form
    {
        public FormError()
        {
            InitializeComponent();
        }

        public FormError(Exception exception)
        {
            InitializeComponent();
            textBoxError.Text = GetCompleteMessage(exception);
        }

        private void ButtonCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private static string GetCompleteMessage(Exception exception)
        {
            Exception x = exception.InnerException;
            var msg = new StringBuilder(exception.Message);
            while (x != null)
            {
                msg.AppendFormat("\r\n\r\n{0}", x.Message);
                x = x.InnerException;
            }
            msg.Append("\r\n----Stacktrace----\r\n");
            msg.Append(exception.StackTrace);
            return msg.ToString();
        }
    }
}
