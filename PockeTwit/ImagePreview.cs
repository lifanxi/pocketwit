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
            using (Bitmap imageToShow = new Bitmap(ImageToShow))
            {
                this.pictureBox1.Image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
                using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                {
                    g.DrawImage(imageToShow, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height), new Rectangle(0, 0, imageToShow.Width, imageToShow.Height), GraphicsUnit.Pixel);
                }
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Dispose();
            this.Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            pictureBox1.Image.Dispose();
            base.OnClosed(e);
            
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = fullURL;
            p.StartInfo.UseShellExecute = true;
            p.Start();
            pictureBox1.Image.Dispose();
            this.Close();
        }
    }
}