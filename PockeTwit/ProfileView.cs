using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class ProfileView : Form
    {
        private PockeTwit.Library.User _User;
        public ProfileView(PockeTwit.Library.User User)
        {
            _User = User;
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            avatarBox.Width = ClientSettings.SmallArtSize;
            avatarBox.Height = ClientSettings.SmallArtSize;

            avatarBox.Image = PockeTwit.ThrottledArtGrabber.GetArt(User.screen_name, User.high_profile_image_url);
            lblUserName.Text = User.screen_name;
            lblFullName.Text = User.name;
            lblPosition.Text = User.location;
            lblDescription.Text = User.description;
            PockeTwit.ThrottledArtGrabber.NewArtWasDownloaded += new ThrottledArtGrabber.ArtIsReady(ThrottledArtGrabber_NewArtWasDownloaded);
        }
        private delegate void delUpdateArt(string Argument);

        void ThrottledArtGrabber_NewArtWasDownloaded(string Argument)
        {
            if (InvokeRequired)
            {
                delUpdateArt d = new delUpdateArt(ThrottledArtGrabber_NewArtWasDownloaded);
                this.Invoke(d, Argument);
            }
            else
            {
                avatarBox.Image = PockeTwit.ThrottledArtGrabber.GetArt(_User.screen_name, _User.high_profile_image_url);
            }
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            PockeTwit.ThrottledArtGrabber.NewArtWasDownloaded -= new ThrottledArtGrabber.ArtIsReady(ThrottledArtGrabber_NewArtWasDownloaded);
            this.Close();
        }

        private void lblPosition_Click(object sender, EventArgs e)
        {
            ProfileMap m = new ProfileMap(_User.location);
            m.ShowDialog();
        }
    }
}