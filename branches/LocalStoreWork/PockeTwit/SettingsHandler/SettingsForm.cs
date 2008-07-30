using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class SettingsForm : MasterForm
    {
        public SettingsForm()
        {
            InitializeComponent();
            PopulateForm();
        }

        private void PopulateForm()
        {
            txtUserName.Text = ClientSettings.UserName;
            txtPassword.Text = ClientSettings.Password;
            txtMaxTweets.Text = ClientSettings.CachedTweets.ToString();
            chkVersion.Checked = ClientSettings.CheckVersion;
            this.DialogResult = DialogResult.Cancel;
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void menuAccept_Click(object sender, EventArgs e)
        {
            if (!VerifyTweetCacheSize())
            {
                lblError.Visible = true;
                lblError.Text = "Max. Tweets must between 10 and 200";
                return;
            }

            Yedda.Twitter twitter = new Yedda.Twitter();
            if (!twitter.Verify(txtUserName.Text, txtPassword.Text))
            {
                lblError.Visible = true;
                lblError.Text = "Unable to verify username and password";
                return;
            }
            else
            {
                lblError.Visible = false;
                ClientSettings.UserName = txtUserName.Text;
                ClientSettings.Password = txtPassword.Text;
                ClientSettings.CheckVersion = chkVersion.Checked;
                ClientSettings.CachedTweets = int.Parse(txtMaxTweets.Text);
                ClientSettings.SaveSettings();
                Following.Reset();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool VerifyTweetCacheSize()
        {
            try
            {
                int TweetNumber = int.Parse(txtMaxTweets.Text);
                return (TweetNumber >= 10 && TweetNumber <= 200);
            }
            catch
            {
                return false;
            }
        }
    }
}