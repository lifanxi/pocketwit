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

        private bool Initialized = false;
		#region Constructors (1) 

        public SettingsForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            PopulateForm();
        }

		#endregion Constructors 

		#region Properties (1) 

        public bool NeedsReset { get; set; }

		#endregion Properties 

		#region Methods (4) 


		// Private Methods (4) 

        

        private void menuAccept_Click(object sender, EventArgs e)
        {
            lblTweets.ForeColor = ClientSettings.ForeColor;
            Cursor.Current = Cursors.WaitCursor;
            int MaxTweets = 0;
            try
            {
                MaxTweets = int.Parse(txtMaxTweets.Text);
                if (MaxTweets > 200 || MaxTweets < 10)
                {
                    lblTweets.ForeColor = ClientSettings.ErrorColor;
                    Cursor.Current = Cursors.Default;
                    return;
                }
            }
            catch
            {
                lblTweets.ForeColor = ClientSettings.ErrorColor;
                Cursor.Current = Cursors.Default;
                return;
            }
            if (MaxTweets != ClientSettings.MaxTweets) { NeedsReset = true; }
            if (chkTimestamps.Checked != ClientSettings.ShowExtra) { NeedsReset = true; }
            ClientSettings.UseGPS = chkGPS.Checked;
            ClientSettings.CheckVersion = chkVersion.Checked;
            ClientSettings.BeepOnNew = chkBeep.Checked;
            ClientSettings.MaxTweets = MaxTweets;
            ClientSettings.ShowReplyImages = chkReplyImages.Checked;
            ClientSettings.ShowExtra = chkTimestamps.Checked;
            ClientSettings.SaveSettings();
            
            Cursor.Current = Cursors.Default;
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PopulateForm()
        {
            chkVersion.Checked = ClientSettings.CheckVersion;
            chkBeep.Checked = ClientSettings.BeepOnNew;
            txtMaxTweets.Text = ClientSettings.MaxTweets.ToString();
            chkReplyImages.Checked = ClientSettings.ShowReplyImages;
            chkTimestamps.Checked = ClientSettings.ShowExtra;
            chkGPS.Checked = ClientSettings.UseGPS;
            this.DialogResult = DialogResult.Cancel;
        }


		#endregion Methods 

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!Initialized)
            {
                Initialized = true;
                if (ClientSettings.AccountsList.Count == 0)
                {
                    if (ShowAccounts() == DialogResult.Cancel)
                    {
                        this.DialogResult = DialogResult.Cancel;
                    }
                }
            }
        }
        private DialogResult ShowAccounts()
        {
            AccountsForm af = new AccountsForm();
            DialogResult ret = af.ShowDialog();
            if (af.IsDirty)
            {
                NeedsReset = true;
            }
            af.Close();
            return ret;
        }

        private void lnkAccounts_Click(object sender, EventArgs e)
        {
            ShowAccounts();
        }

        private void lnkClearAvatars_Click(object sender, EventArgs e)
        {
            foreach(string Avatar in System.IO.Directory.GetFiles(ClientSettings.AppPath + "\\ArtCache\\"))
            {
                System.IO.File.Delete(Avatar);
            }
            ImageBuffer.Clear();
            MessageBox.Show("The avatar cache was cleared.","PockeTwit");
        }

    }
}