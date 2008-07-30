using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class URLForm : Form
    {
        public URLForm()
        {
            InitializeComponent();
        }

        private string _URL;
        public string URL
        {
            get
            {
                return _URL;
            }
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void menuOK_Click(object sender, EventArgs e)
        {
            _URL = isgd.ShortenURL(this.txtURL.Text);
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }
    }
}