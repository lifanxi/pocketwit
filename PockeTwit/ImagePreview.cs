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
        private string imagePathToShow = "";
        public ImagePreview(string ImageToShow, string FullURL)
        {
            InitializeComponent();
            this.pictureBox1.Image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            pictureBox1.Resize += new EventHandler(pictureBox1_Resize);
            fullURL = FullURL;
            imagePathToShow = ImageToShow;
            DrawPreview();
        }

        void pictureBox1_Resize(object sender, EventArgs e)
        {
            this.pictureBox1.Image.Dispose();
            this.pictureBox1.Image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            DrawPreview();
        }

        private void DrawPreview()
        {
            if (string.IsNullOrEmpty(imagePathToShow))
            {
                return;
            }
            using (Bitmap imageToShow = new Bitmap(imagePathToShow))
            {
                int controlbigSide = pictureBox1.Image.Width > pictureBox1.Image.Height ? pictureBox1.Image.Width : pictureBox1.Image.Height;
                int imagebigSide = imageToShow.Width > imageToShow.Height ? imageToShow.Width : imageToShow.Height;
                int controlScaleSide = imageToShow.Width > imageToShow.Height ? pictureBox1.Width : pictureBox1.Height;

                decimal scaleFactor = (decimal)controlScaleSide / imagebigSide;

                int leftOfImage = (pictureBox1.Image.Width - (int)(imageToShow.Width * scaleFactor)) / 2;
                int topOfImage = (pictureBox1.Image.Height - (int)(imageToShow.Height * scaleFactor)) / 2;

                Rectangle destRect = new Rectangle(leftOfImage, topOfImage, (int)(imageToShow.Width * scaleFactor), (int)(imageToShow.Height * scaleFactor));
                
                using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                {
                    g.Clear(ClientSettings.BackColor);
                    g.DrawImage(imageToShow, destRect, new Rectangle(0, 0, imageToShow.Width, imageToShow.Height), GraphicsUnit.Pixel);
                }

            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = fullURL;
            p.StartInfo.UseShellExecute = true;
            p.Start();
            this.Close();
        }
    }
}