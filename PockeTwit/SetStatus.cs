using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class SetStatus : Form
    {
        public bool AllowTwitPic
        {
            set
            {
                btnPic.Visible = value;
            }
        }
        public bool UseTwitPic = false;
        public string TwitPicFile = null;
        public string StatusText
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
                this.textBox1.SelectionStart = this.textBox1.Text.Length;
            }
        }

        public SetStatus()
        {
            InitializeComponent();
            lblCharsLeft.Text = "140";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int lengthLeft = 140-textBox1.Text.Length;
            lblCharsLeft.Text = lengthLeft.ToString();
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void menuSubmit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == "Set Status")
            {
                textBox1.SelectionStart = 0;
                textBox1.SelectionLength = textBox1.Text.Length;
            }
            else
            {
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.SelectionLength = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            URLForm f = new URLForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = textBox1.Text + " " + f.URL;
            }
            this.Show();
            f.Close();
        }

        private void btnPic_Click(object sender, EventArgs e)
        {
            try
            {
                using (Microsoft.WindowsMobile.Forms.CameraCaptureDialog c = new Microsoft.WindowsMobile.Forms.CameraCaptureDialog())
                {
                    if (c.ShowDialog() == DialogResult.OK)
                    {
                        TwitPicFile = c.FileName;
                        UseTwitPic = true;
                    }
                }
            }
            catch 
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    TwitPicFile = openFileDialog1.FileName;
                    UseTwitPic = true;
                }
            }
            
        }
    }
}