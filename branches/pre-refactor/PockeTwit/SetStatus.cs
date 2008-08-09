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
                if (DetectDevice.DeviceType == DeviceType.Professional)
                {
                    btnPic.Visible = value;
                }
                else
                {
                    //TODO -- ADD MENU ITEM
                }
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
            switch (DetectDevice.DeviceType)
            {
                case DeviceType.Professional:
                    SetupProfessional();
                    break;
                case DeviceType.Standard:
                    SetupStandard();
                    break;
            }
            lblCharsLeft.Text = "140";
        }

        private void SetupStandard()
        {
            this.menuSubmit = new System.Windows.Forms.MenuItem();
            this.menuSubmit.Text = "Submit";
            this.menuSubmit.Click += new System.EventHandler(this.menuSubmit_Click);

            this.menuURL = new MenuItem();
            menuURL.Text = "URL...";
            menuURL.Click += new EventHandler(menuURL_Click);

            this.menuPic = new MenuItem();
            menuPic.Text = "Picture";
            menuPic.Click += new EventHandler(menuPic_Click);

            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem1.Text = "Action";

            this.menuItem1.MenuItems.Add(this.menuSubmit);
            this.menuItem1.MenuItems.Add(menuURL);
            this.menuItem1.MenuItems.Add(menuPic);
            this.mainMenu1.MenuItems.Add(menuItem1);
        }

        
        private void SetupProfessional()
        {
            this.btnURL = new System.Windows.Forms.Button();
            this.btnPic = new System.Windows.Forms.Button();
            this.btnURL.BackColor = System.Drawing.Color.Black;
            this.btnURL.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.btnURL.ForeColor = System.Drawing.Color.LightGray;
            this.btnURL.Location = new System.Drawing.Point(3, 3);
            this.btnURL.Name = "button1";
            this.btnURL.Size = new System.Drawing.Size(64, 20);
            this.btnURL.TabIndex = 1;
            this.btnURL.Text = "Insert URL";
            this.btnURL.Click += new System.EventHandler(this.button1_Click);


            this.btnPic.BackColor = System.Drawing.Color.Black;
            this.btnPic.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.btnPic.ForeColor = System.Drawing.Color.LightGray;
            this.btnPic.Location = new System.Drawing.Point(73, 3);
            this.btnPic.Name = "btnPic";
            this.btnPic.Size = new System.Drawing.Size(90, 20);
            this.btnPic.TabIndex = 3;
            this.btnPic.Text = "Include Picture";
            this.btnPic.Click += new System.EventHandler(this.btnPic_Click);

            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Images | *.jpg;*.jpeg";

            this.menuSubmit = new System.Windows.Forms.MenuItem();
            this.menuSubmit.Text = "Submit";
            this.menuSubmit.Click += new System.EventHandler(this.menuSubmit_Click);
            
            this.mainMenu1.MenuItems.Add(this.menuSubmit);

            this.Controls.Add(this.btnPic);
            this.Controls.Add(this.btnURL);
            
        }

        void menuPic_Click(object sender, EventArgs e)
        {
            InsertPicture();
        }
        void menuURL_Click(object sender, EventArgs e)
        {
            InsertURL();
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
            InsertURL();
        }

        private void InsertURL()
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
                InsertPicture();
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

        private void InsertPicture()
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
    }
}