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
        private System.Windows.Forms.ContextMenu copyPasteMenu;
        private System.Windows.Forms.MenuItem PasteItem;
        private System.Windows.Forms.MenuItem CopyItem;
		

        public string TwitPicFile = null;
        public bool UseTwitPic = false;
        public string GPSLocation = null;
        private LocationManager l = new LocationManager();
        
        private delegate void delUpdateText(string text);

        
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
            SetImages();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            lblCharsLeft.Text = "140";
            PopulateAccountList();
            if (DetectDevice.DeviceType == DeviceType.Standard)
            {
                SmartPhoneMenu();
            }
            else
            {
                SetupTouchScreen();
            }
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            SizeF currentScreen = this.CurrentAutoScaleDimensions;
            if (currentScreen.Height == 192)
            {
                ResizeForVGA();
            }

            this.ResumeLayout(false);

            l.LocationReady += new LocationManager.delLocationReady(l_LocationReady);

            this.txtStatusUpdate.Focus();
            
        }

        private void SetupTouchScreen()
        {
            this.mainMenu1.MenuItems.Add(this.menuSubmit);
            this.pictureFromCamers.Click += new EventHandler(pictureFromCamers_Click);
            this.pictureFromStorage.Click += new EventHandler(pictureFromStorage_Click);
            this.pictureURL.Click += new EventHandler(pictureURL_Click);
            this.pictureLocation.Click += new EventHandler(pictureLocation_Click);

            this.copyPasteMenu = new System.Windows.Forms.ContextMenu();

            this.PasteItem = new System.Windows.Forms.MenuItem();
            PasteItem.Text = "Paste";

            this.CopyItem = new MenuItem();
            CopyItem.Text = "Copy";

            copyPasteMenu.MenuItems.Add(CopyItem);
            copyPasteMenu.MenuItems.Add(PasteItem);
            this.txtStatusUpdate.ContextMenu = copyPasteMenu;

            CopyItem.Click += new EventHandler(CopyItem_Click);
            PasteItem.Click += new EventHandler(PasteItem_Click);
        }

        void PasteItem_Click(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                this.txtStatusUpdate.SelectedText = (string)iData.GetData(DataFormats.Text);
            }
        }

        void CopyItem_Click(object sender, EventArgs e)
        {
            string selText = this.txtStatusUpdate.SelectedText;
            if (!string.IsNullOrEmpty(selText))
            {
                Clipboard.SetDataObject(selText);
            }
        }

        
        void l_LocationReady(string Location)
        {

            if (InvokeRequired)
            {
                delUpdateText d = new delUpdateText(l_LocationReady);
                this.Invoke(d, Location);
            }
            else
            {
                if (!string.IsNullOrEmpty(Location))
                {
                    l.StopGPS();
                    this.GPSLocation = Location;
                    lblGPS.Text = "Location Found";
                }
            }
        }

        private void StartLocating()
        {
            //StartAnimation
            l.StartGPS();
            pictureLocation.Visible = false;
            lblGPS.Visible = true;
        }

        private void ResizeForVGA()
        {
            
            //pictureFromCamers.Size = new Size(26, 50);
            //pictureFromStorage.Size = new Size(50, 50);
            //pictureLocation.Size = new Size(50, 50);
            //pictureURL.Size = new Size(50, 50);

        }

        private System.Windows.Forms.MenuItem menuExist;
        private System.Windows.Forms.MenuItem menuCamera;
        private System.Windows.Forms.MenuItem menuURL;
        private System.Windows.Forms.MenuItem menuGPS;
        private System.Windows.Forms.MenuItem menuItem1;
        private void SmartPhoneMenu()
        {
            lblGPS.Left = 5;
            pictureFromCamers.Visible = false;
            pictureFromStorage.Visible = false;
            pictureLocation.Visible = false;
            pictureURL.Visible = false;
            this.menuExist = new MenuItem();
            menuExist.Text = "Existing Picture";
            menuExist.Click += new EventHandler(menuExist_Click);

            this.menuCamera = new MenuItem();
            menuCamera.Text = "Take Picture";
            menuCamera.Click += new EventHandler(menuCamera_Click);
            
            this.menuURL = new MenuItem();
            menuURL.Text = "URL...";
            menuURL.Click += new EventHandler(menuURL_Click);

            this.menuGPS = new MenuItem();
            menuGPS.Text = "Update Location";
            menuGPS.Click += new EventHandler(menuGPS_Click);

            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem1.Text = "Action";

            this.menuItem1.MenuItems.Add(this.menuSubmit);
            this.menuItem1.MenuItems.Add(menuURL);
            this.menuItem1.MenuItems.Add(menuExist);
            this.menuItem1.MenuItems.Add(menuCamera);
            this.menuItem1.MenuItems.Add(menuGPS);
            this.mainMenu1.MenuItems.Add(menuItem1);
        }

        void menuGPS_Click(object sender, EventArgs e)
        {
            StartLocating();
        }

        void menuURL_Click(object sender, EventArgs e)
        {
            InsertURL();
        }

        void menuCamera_Click(object sender, EventArgs e)
        {
            InsertPictureFromCamera();
        }

        void menuExist_Click(object sender, EventArgs e)
        {
            InsertPictureFromFile();
        }

        private void SetImages()
        {


            this.pictureFromCamers.Image = new Bitmap(ClientSettings.IconsFolder() + "takepicture.png");
            this.pictureFromStorage.Image = new Bitmap(ClientSettings.IconsFolder() + "existingimage.png");
            this.pictureURL.Image = new Bitmap(ClientSettings.IconsFolder() + "url.png");
            this.pictureLocation.Image = new Bitmap(ClientSettings.IconsFolder() + "map.png");
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
                        this.pictureFromStorage.Image = new Bitmap(ClientSettings.IconsFolder() + "existingimage.png");
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
                this.pictureFromCamers.Image = new Bitmap(ClientSettings.IconsFolder() + "takepicture.png");
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
                if (DetectDevice.DeviceType == DeviceType.Standard && lblGPS.Visible)
                {
                    BoxToUpdate.Left = lblGPS.Right + 5;
                }
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
        void pictureLocation_Click(object sender, EventArgs e)
        {
            StartLocating();
        }
        void pictureURL_Click(object sender, EventArgs e)
        {
            InsertURL();
        }
        void pictureFromStorage_Click(object sender, EventArgs e)
        {
            InsertPictureFromFile();
        }
        void pictureFromCamers_Click(object sender, EventArgs e)
        {
            InsertPictureFromCamera();
        }
        private void menuSubmit_Click(object sender, EventArgs e)
        {
            Program.LastStatus = this.StatusText;
            this.DialogResult = DialogResult.OK;
        }
        private void cmbAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.AccountToSet = (Yedda.Twitter.Account)cmbAccount.SelectedItem;
        }
        #endregion

        

        
        
    }
}