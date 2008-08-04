using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class SettingsForm : Form
    {
        public bool NeedsReset { get; set; }
        public SettingsForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            FillServerList();
            PopulateForm();
        }

        private void FillServerList()
        {
            cmbServers.Items.Add("twitter");
            cmbServers.Items.Add("identica");
        }

        private void PopulateForm()
        {
            txtUserName.Text = ClientSettings.UserName;
            txtPassword.Text = ClientSettings.Password;
            chkVersion.Checked = ClientSettings.CheckVersion;
            chkBeep.Checked = ClientSettings.BeepOnNew;
            txtMaxTweets.Text = ClientSettings.MaxTweets.ToString();
            chkReplyImages.Checked = ClientSettings.ShowReplyImages;
            switch (ClientSettings.Server)
            {
                case Yedda.Twitter.TwitterServer.twitter:
                    cmbServers.SelectedItem = "twitter";
                    break;
                case Yedda.Twitter.TwitterServer.identica:
                    cmbServers.SelectedItem = "identica";
                    break;
            }
            this.DialogResult = DialogResult.Cancel;
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void menuAccept_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            Cursor.Current = Cursors.WaitCursor;
            int MaxTweets = 0;
            try
            {
                MaxTweets = int.Parse(txtMaxTweets.Text);
                if (MaxTweets > 200 || MaxTweets < 10)
                {
                    lblError.Text = "max tweets must be 10 to 200";
                    lblError.Visible = true;
                    Cursor.Current = Cursors.Default;
                    return;
                }
            }
            catch
            {
                lblError.Text = "max tweets must be 10 to 200";
                lblError.Visible = true;
                Cursor.Current = Cursors.Default;
                return;
            }
            Yedda.Twitter twitter = new Yedda.Twitter();
            Yedda.Twitter.TwitterServer SelectedServer = (Yedda.Twitter.TwitterServer)Enum.Parse(typeof(Yedda.Twitter.TwitterServer), (string)cmbServers.SelectedItem, true);
            twitter.CurrentServer = SelectedServer;
            if (!twitter.Verify(txtUserName.Text, txtPassword.Text))
            {
                lblError.Text = "Unable to verify username and password";
                lblError.Visible = true;
                Cursor.Current = Cursors.Default;
                return;
            }
            else
            {
                NeedsReset = ClientSettings.UserName!=txtUserName.Text |
                             ClientSettings.Server != SelectedServer | 
                             ClientSettings.MaxTweets != MaxTweets;

                ClientSettings.UserName = txtUserName.Text;
                ClientSettings.Password = txtPassword.Text;
                ClientSettings.CheckVersion = chkVersion.Checked;
                ClientSettings.BeepOnNew = chkBeep.Checked;
                ClientSettings.Server = SelectedServer;
                ClientSettings.MaxTweets = MaxTweets;
                ClientSettings.ShowReplyImages = chkReplyImages.Checked;
                ClientSettings.SaveSettings();
                
                Following.Reset();
                Cursor.Current = Cursors.Default;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

        }

    }
}