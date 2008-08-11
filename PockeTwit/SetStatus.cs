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

		#region Fields (2) 

        public string TwitPicFile = null;
        public bool UseTwitPic = false;

		#endregion Fields 

		#region Constructors (1) 

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
            PopulateAccountList();
        }

        private void PopulateAccountList()
        {
            foreach (Yedda.Twitter.Account Account in ClientSettings.AccountsList)
            {
                if (Account.Enabled)
                {
                    cmbAccount.Items.Add(Account);
                }
            }
        }
		#endregion Constructors 

		#region Properties (2) 

        private Yedda.Twitter.Account _AccountToSet = ClientSettings.AccountsList[0];
        public Yedda.Twitter.Account AccountToSet 
        { 
            get
            {
                return _AccountToSet;
            }
            set
            {
                _AccountToSet = value;
                cmbAccount.SelectedItem = _AccountToSet;
            }
        }

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

		#endregion Properties 

		#region Methods (12) 


		// Private Methods (12) 

        private void btnPic_Click(object sender, EventArgs e)
        {
            try
            {
                InsertPictureFromCamera();
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

        private void InsertPictureFromCamera()
        {
            using (Microsoft.WindowsMobile.Forms.CameraCaptureDialog c = new Microsoft.WindowsMobile.Forms.CameraCaptureDialog())
            {
                try
                {
                    if (c.ShowDialog() == DialogResult.OK)
                    {
                        TwitPicFile = c.FileName;
                        UseTwitPic = true;
                    }
                }
                catch
                {
                    MessageBox.Show("The camera is not available.", "PockeTwit");
                }
            }
        }

        private void InsertPictureFromFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TwitPicFile = openFileDialog1.FileName;
                UseTwitPic = true;
            }
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

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        void menuPic_Click(object sender, EventArgs e)
        {
            InsertPictureFromCamera();
        }

        private void menuSubmit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        void menuURL_Click(object sender, EventArgs e)
        {
            InsertURL();
        }

        private void SetupProfessional()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Images | *.jpg;*.jpeg";

            this.menuSubmit = new System.Windows.Forms.MenuItem();
            this.menuSubmit.Text = "Submit";
            this.menuSubmit.Click += new System.EventHandler(this.menuSubmit_Click);
            
            this.mainMenu1.MenuItems.Add(this.menuSubmit);
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int lengthLeft = 140-textBox1.Text.Length;
            lblCharsLeft.Text = lengthLeft.ToString();
        }


		#endregion Methods 

        private void cmbAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.AccountToSet = (Yedda.Twitter.Account)cmbAccount.SelectedItem;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            InsertPictureFromCamera();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            InsertPictureFromFile();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            InsertURL();
        }


    }
}