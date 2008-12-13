using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class ProfileMap : Form
    {
        private string _Location;
        public ProfileMap(string Location)
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            _Location = Location;
            this.pictureBox1.Resize += new EventHandler(pictureBox1_Resize);
        }

        void pictureBox1_Resize(object sender, EventArgs e)
        {
            DisplayMap();
        }

        private void DisplayMap()
        {
            try
            {
                this.pictureBox1.Image = Yedda.GoogleMaps.GetMap(_Location, 2, pictureBox1.Height, pictureBox1.Width);
            }
            catch
            {
                pictureBox1.Visible = false;
                Label newLabel = new Label();
                newLabel.Text = "Unable to fetch map.";
                newLabel.Location = new Point(0, 0);
                this.Controls.Add(newLabel);

            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Resize -= new EventHandler(pictureBox1_Resize);
            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
            }
            this.Close();
        }

        
    }
}