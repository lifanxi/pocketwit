using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class ColorChose : Form
    {
        
        public Color MyColor;
        public ColorChose(Color C,Bitmap bmp)
        {
            MyColor = C;
            InitializeComponent();
            
            this.pictureBox1.Image = bmp;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int i = (e.X * 255) / (pictureBox1.Width);
            int j = (e.Y * 255) / (pictureBox1.Height);
            MyColor = ColorPick.GetColorFromPallet(i, j);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}