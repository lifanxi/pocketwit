using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class ProfilePage : Form
    {
        public ProfilePage()
        {
            InitializeComponent();
        }
        public string User
        {
            set
            {
                this.webBrowser1.Navigate(new Uri("http://twitter.com/" + value));
            }
        }
        public string URL
        {
            set
            {
                this.webBrowser1.Navigate(new Uri(value));
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(new Uri("about:blank"));
            this.Close();
        }
    }
}