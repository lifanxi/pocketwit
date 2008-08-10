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
            if (MaxTweets != ClientSettings.MaxTweets) { NeedsReset = true; }
            ClientSettings.CheckVersion = chkVersion.Checked;
            ClientSettings.BeepOnNew = chkBeep.Checked;
            ClientSettings.MaxTweets = MaxTweets;
            ClientSettings.ShowReplyImages = chkReplyImages.Checked;
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
            this.DialogResult = DialogResult.Cancel;
        }


		#endregion Methods 

        private void btnAccounts_Click(object sender, EventArgs e)
        {
            AccountsForm af = new AccountsForm();
            af.ShowDialog();
            if (af.IsDirty)
            {
                NeedsReset = true;
            }
            af.Close();
        }

    }
}