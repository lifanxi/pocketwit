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
            this.WindowState = FormWindowState.Maximized;
            PopulateForm();
        }

		#endregion Constructors 

		
		#region Methods (4) 
        // Private Methods (4) 
        private void menuAccept_Click(object sender, EventArgs e)
        {
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
            foreach(string Avatar in System.IO.Directory.GetFiles(ClientSettings.AppPath + "\\ArtCache\\"))
            {
                System.IO.File.Delete(Avatar);
            }
            ImageBuffer.Clear();
            MessageBox.Show("The avatar cache was cleared.","PockeTwit");
        }

    }
}