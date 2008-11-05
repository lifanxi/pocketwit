using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class AvatarSettings : Form
    {

        #region Constructors (1) 

        public AvatarSettings()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            PopulateForm();
        }

		#endregion Constructors 

		
		#region Methods (4) 
        // Private Methods (4) 
        private void menuAccept_Click(object sender, EventArgs e)
        {
            if (chkHighQuality.Checked != ClientSettings.HighQualityAvatars)
            {
                if (MessageBox.Show("You should clear the cache when switching avatars.  Would you like to do that now?", "PockeTwit", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    ClearCache();
                }
            }
            ClientSettings.ShowAvatars = chkAvatar.Checked;
            ClientSettings.ShowReplyImages = chkReplyImages.Checked;
            ClientSettings.HighQualityAvatars = chkHighQuality.Checked;
            ClientSettings.SaveSettings();
            
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
            chkAvatar.Checked = ClientSettings.ShowAvatars;
            chkReplyImages.Checked = ClientSettings.ShowReplyImages;
            chkHighQuality.Checked = ClientSettings.HighQualityAvatars;
            this.DialogResult = DialogResult.Cancel;
        }


		#endregion Methods 

        

        private void lnkClearAvatars_Click(object sender, EventArgs e)
        {
            ClearCache();
        }

        private static void ClearCache()
        {
            try
            {
                foreach (string Avatar in System.IO.Directory.GetFiles(ClientSettings.AppPath + "\\ArtCache\\"))
                {
                    System.IO.File.Delete(Avatar);
                }
                foreach (Yedda.Twitter.Account a in ClientSettings.AccountsList)
                {
                    a.Buffer.Clear();
                }
            }
            catch
            {
                MessageBox.Show("There was an error when clearing the cache. You may want to try again.", "PockeTwit");
                return;
            }
            MessageBox.Show("The avatar cache was cleared.", "PockeTwit");
        }

        private void chkReplyImages_CheckStateChanged(object sender, EventArgs e)
        {
            if (chkReplyImages.Checked)
            {
                MessageBox.Show("Replay avatars may cause instability.  Please disable them if you have any problems.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

    }
}