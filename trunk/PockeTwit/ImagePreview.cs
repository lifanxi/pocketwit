using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class ImagePreview : Form
    {
        private Bitmap imageToShow;
        private string fullURL;
        public ImagePreview(string ImageToShow, string FullURL)
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            fullURL = FullURL;
            imageToShow = new Bitmap(ImageToShow);
            this.pictureBox1.Image = imageToShow;
            
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            imageToShow.Dispose();
            this.Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            imageToShow.Dispose();
            base.OnClosed(e);
            
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = fullURL;
            p.StartInfo.UseShellExecute = true;
            p.Start();
            imageToShow.Dispose();
            this.Close();
        }
    }
}