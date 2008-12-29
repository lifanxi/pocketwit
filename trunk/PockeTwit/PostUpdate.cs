using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class PostUpdate : Form
    {
        public string TwitPicFile = null;
        public bool UseTwitPic = false;
        public string GPSLocation = null;
        
        #region Properties
        private Yedda.Twitter.Account _AccountToSet = ClientSettings.DefaultAccount;
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
                Yedda.Twitter t = new Yedda.Twitter();
                t.AccountInfo = _AccountToSet;
                AllowTwitPic = t.AllowTwitPic;
            }
        }

        public bool AllowTwitPic
        {
            set
            {
                if (DetectDevice.DeviceType == DeviceType.Professional)
                {
                    pictureFromCamers.Visible = value;
                    pictureFromStorage.Visible = value;
                }
                else
                {
                    /*
                    menuCamera.Enabled = value;
                    menuExist.Enabled = value;
                     */
                }
            }
        }

        public string StatusText
        {
            get
            {
                return txtStatusUpdate.Text;
            }
            set
            {
                txtStatusUpdate.Text = value;
                this.txtStatusUpdate.SelectionStart = txtStatusUpdate.Text.Length;
            }
        }
        #endregion


        public PostUpdate()
        {
        
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            lblCharsLeft.Text = "140";
            PopulateAccountList();
            this.ResumeLayout(false);

            this.txtStatusUpdate.Focus();
            
        }

        private void PopulateAccountList()
        {
            foreach (Yedda.Twitter.Account Account in ClientSettings.AccountsList)
            {
                cmbAccount.Items.Add(Account);
            }
        }

        #region Methods
        private void InsertURL()
        {
            URLForm f = new URLForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                txtStatusUpdate.Text = txtStatusUpdate.Text + " " + f.URL;
            }
            this.Show();
            f.Close();
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
                        
                        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetStatus));
                        this.pictureFromStorage.Image = new System.Drawing.Bitmap(ClientSettings.IconsFolder() + "existingimage.png");
                        if (DetectDevice.DeviceType == DeviceType.Standard)
                        {
                            this.pictureFromStorage.Visible = false;
                        }
                        AddPictureToForm(c.FileName, pictureFromCamers);
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
            Microsoft.WindowsMobile.Forms.SelectPictureDialog s = new Microsoft.WindowsMobile.Forms.SelectPictureDialog();
            if (s.ShowDialog() == DialogResult.OK)
            {
                TwitPicFile = s.FileName;
                UseTwitPic = true;
                //if (cameraPictureBox.Visible)
                //{
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetStatus));
                this.pictureFromCamers.Image = new System.Drawing.Bitmap(ClientSettings.IconsFolder() + "takepicture.png");
                if (DetectDevice.DeviceType == DeviceType.Standard)
                {
                    this.pictureFromCamers.Visible = false;
                }
                AddPictureToForm(s.FileName, pictureFromStorage);
                //}
            }
        }

        private void AddPictureToForm(string ImageFile, PictureBox BoxToUpdate)
        {
            try
            {
                BoxToUpdate.Image = new System.Drawing.Bitmap(ImageFile);
                BoxToUpdate.Visible = true;
                txtStatusUpdate_TextChanged(null, new EventArgs());
            }
            catch (OutOfMemoryException)
            {
            }
        }
        #endregion

        #region Events
        private void txtStatusUpdate_TextChanged(object sender, EventArgs e)
        {
            int charsAvail = 140;
            if (this.UseTwitPic)
            {
                charsAvail = charsAvail - 27;
            }
            int lengthLeft = charsAvail - txtStatusUpdate.Text.Length;
            lblCharsLeft.Text = lengthLeft.ToString();
        }
        private void menuCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel the update?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
        #endregion

        
    }
}