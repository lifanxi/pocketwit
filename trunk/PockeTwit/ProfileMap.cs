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
            this.pictureBox1.Image = Yedda.GoogleMaps.GetMap(_Location, 2, pictureBox1.Height, pictureBox1.Width);
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Resize -= new EventHandler(pictureBox1_Resize);
            this.pictureBox1.Image.Dispose();
            this.Close();
        }

        
    }
}