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
        private bool _StandAlone;
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

        public string in_reply_to_status_id { get; set; }

        #endregion


        public PostUpdate(bool Standalone)
        {
            _StandAlone = Standalone;
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
                    if (DetectDevice.DeviceType == DeviceType.Standard)
                    {
                        // just enable the menuItem
                        if (null != menuGPSInsert)
                        {
                            menuGPSInsert.Enabled = true;
                        }
                    }
                    else
                    {
                        // hide the label, add a new button
                        lblGPS.Visible = false;
                        LinkLabel llGPS = new LinkLabel();
                        llGPS.Text = "Ins. GPS Link";
                        llGPS.ForeColor = Color.White;
                        llGPS.Left = lblGPS.Left;
                        llGPS.Top = lblGPS.Top;
                        llGPS.Height = lblGPS.Height;
                        llGPS.Width = lblGPS.Width;
                        llGPS.Click += new EventHandler(llGPS_Click);
                        this.Controls.Add(llGPS);
                    }
                }
            }
        }

        void llGPS_Click(object sender, EventArgs e)
        {
            InsertGpsLocation();
        }

        private void StartLocating()
        {   
            //StartAnimation
            l.StartGPS();
            pictureLocation.Visible = false;
            lblGPS.Visible = true;
        }

        

        private System.Windows.Forms.MenuItem menuExist;
        private System.Windows.Forms.MenuItem menuCamera;
        private System.Windows.Forms.MenuItem menuURL;
        private System.Windows.Forms.MenuItem menuGPS;
        private System.Windows.Forms.MenuItem menuGPSInsert;
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

            this.menuGPSInsert = new MenuItem();
            menuGPSInsert.Text = "Insert GPS Location";
            menuGPSInsert.Click += new EventHandler(menuGPSInsert_Click);
            menuGPSInsert.Enabled = false;

            this.PasteItem = new MenuItem();
            this.PasteItem.Text = "Paste";
            PasteItem.Click += new EventHandler(PasteItem_Click);

            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem1.Text = "Action";

            this.menuItem1.MenuItems.Add(this.menuSubmit);
            this.menuItem1.MenuItems.Add(PasteItem);
            this.menuItem1.MenuItems.Add(menuURL);
            this.menuItem1.MenuItems.Add(menuExist);
            this.menuItem1.MenuItems.Add(menuCamera);
            this.menuItem1.MenuItems.Add(menuGPS);
            this.menuItem1.MenuItems.Add(menuGPSInsert);
            this.mainMenu1.MenuItems.Add(menuItem1);
        }

        void menuGPSInsert_Click(object sender, EventArgs e)
        {
            InsertGpsLocation();
        }

        private void InsertGpsLocation()
        {
            // google maps url format
            // http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=41.4043197631836,-1.28760504722595
            Cursor.Current = Cursors.WaitCursor;
            string sUrl = string.Format(@"http://maps.google.com/maps?q={0}", this.GPSLocation);
            string gpsUrl = isgd.ShortenURL(sUrl);
            if (string.IsNullOrEmpty(gpsUrl))
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("A communication error occured shortening the URL. Please try again later.");
                return;
            }
            txtStatusUpdate.Text = txtStatusUpdate.Text + " " + gpsUrl;
            Cursor.Current = Cursors.Default;
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
                        
                        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostUpdate));
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
                
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostUpdate));
                this.pictureFromCamers.Image = new Bitmap(ClientSettings.IconsFolder() + "takepicture.png");
                if (DetectDevice.DeviceType == DeviceType.Standard)
                {
                    this.pictureFromCamers.Visible = false;
                }
                AddPictureToForm(s.FileName, pictureFromStorage);
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

        private string TrimTo140(string Original)
        {
            if (Original.Length > 140)
            {
                //Truncate the text to the last available space, the add the URL.
                string URL = Yedda.ShortText.shorten(Original);
                if (string.IsNullOrEmpty(URL))
                {
                    return null;
                }
                int trimLength = 5;
                if (UseTwitPic)
                {
                    trimLength = 30;
                }
                string NewText = Original.Substring(0, Original.LastIndexOf(" ", 140 - (URL.Length + trimLength)));
                return NewText + " ... " + URL;
            }
            return Original;
        }

        private bool PostTheUpdate()
        {
            if (!string.IsNullOrEmpty(StatusText))
            {
                Cursor.Current = Cursors.WaitCursor;
                string UpdateText = TrimTo140(StatusText);

                if (string.IsNullOrEmpty(UpdateText))
                {
                    MessageBox.Show("There was an error shortening the text. Please shorten the message or try again later.");
                    return false;
                }
                Yedda.Twitter TwitterConn = new Yedda.Twitter();
                TwitterConn.AccountInfo = this.AccountToSet;

                try
                {
                    if (this.GPSLocation != null)
                    {
                        TwitterConn.SetLocation(this.GPSLocation);
                    }
                }
                catch { }

                if (TwitterConn.AllowTwitPic && this.UseTwitPic)
                {
                    string retValue;
                    try
                    {
                        retValue = Yedda.TwitPic.SendStoredPic(AccountToSet.UserName, AccountToSet.Password, UpdateText, TwitPicFile);
                    }
                    catch (Exception ex)
                    {
                        Cursor.Current = Cursors.Default;
                        MessageBox.Show("Error sending the image to twitpic. You may want to try again later.");
                        return false;
                    }
                }

                else
                {
                    string retValue = TwitterConn.Update(UpdateText, in_reply_to_status_id, Yedda.Twitter.OutputFormatType.XML);
                    if (string.IsNullOrEmpty(retValue))
                    {
                        MessageBox.Show("Error posting status.  You may want to try again later.");
                        return false;
                    }
                    try
                    {
                        Library.status.DeserializeSingle(retValue, AccountToSet);
                    }
                    catch
                    {
                        MessageBox.Show("Error posting status.  You may want to try again later.");
                        return false;
                    }
                }
                return true;
                
            }
            return true;
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
            if (!string.IsNullOrEmpty(this.txtStatusUpdate.Text))
            {
                if (MessageBox.Show("Are you sure you want to cancel the update?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }
            }
            if (_StandAlone)
            {
                this.Close();
            }
            this.DialogResult = DialogResult.Cancel;
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

            bool Success = PostTheUpdate();
            Cursor.Current = Cursors.Default;
            if (Success)
            {
                if (_StandAlone)
                {
                    this.Close();
                }
                this.DialogResult = DialogResult.OK;
            }
        }
        private void cmbAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.AccountToSet = (Yedda.Twitter.Account)cmbAccount.SelectedItem;
        }
        #endregion

        

        

        
        
    }
}
