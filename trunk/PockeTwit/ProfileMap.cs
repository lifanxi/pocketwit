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
        private int _mapSize = 0;
        private int startCount = 0;
        
        public List<string> Locations = new List<string>();

        public ProfileMap()
        {
            InitializeComponent();
            
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            this.pictureBox1.Resize += new EventHandler(pictureBox1_Resize);
        }

        void pictureBox1_Resize(object sender, EventArgs e)
        {
            DisplayMap(_mapSize);
        }

        private void DisplayMap(int size)
        {

            try
            {
                if (Locations.Count > 5)
                {
                    this.pictureBox1.Image = Yedda.GoogleMaps.GetMultiMap(Locations.GetRange(startCount,5).ToArray(), size, pictureBox1.Height, pictureBox1.Width);
                }
                else
                {
                    this.pictureBox1.Image = Yedda.GoogleMaps.GetMultiMap(Locations.ToArray(), size, pictureBox1.Height, pictureBox1.Width);
                }   
                
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == (Keys.LButton | Keys.MButton | Keys.Back))
            {
                _mapSize++;
            }
            DisplayMap(_mapSize);
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

        
        private void menuItem4_Click(object sender, EventArgs e)
        {
            if (_mapSize > 0)
            {
                _mapSize--;
            }
            DisplayMap(_mapSize);
        
        }

        private void menuNext_Click(object sender, EventArgs e)
        {
            if (startCount < Locations.Count)
            {
                startCount = startCount + 5;
                DisplayMap(_mapSize);
            }
        }

        private void menuZoomIn_Click(object sender, EventArgs e)
        {
            _mapSize++;
            DisplayMap(_mapSize);
        }

        
    }
}