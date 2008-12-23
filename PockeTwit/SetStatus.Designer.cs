using System.Windows.Forms;
using System.ComponentModel;
using System;
namespace PockeTwit
{
    partial class SetStatus
    {

		#region Fields (12) 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblCharsLeft;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuExist;
        private System.Windows.Forms.MenuItem menuCamera;
        private System.Windows.Forms.MenuItem menuSubmit;
        private System.Windows.Forms.MenuItem menuURL;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox chkGPS;
        private System.Windows.Forms.Label lblGPS;
        private System.Windows.Forms.ContextMenu Context;
        private System.Windows.Forms.ContextMenu copyPasteMenu;
        private System.Windows.Forms.MenuItem PasteItem;
        private System.Windows.Forms.MenuItem CopyItem;
		#endregion Fields 

		#region Methods (1) 


		// Protected Methods (1) 

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


		#endregion Methods 


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetStatus));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblCharsLeft = new System.Windows.Forms.Label();
            this.cmbAccount = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkGPS = new CheckBox();
            this.lblGPS = new Label();
            
            this.SuspendLayout();

            if (ClientSettings.UseGPS)
            {
                lblGPS.Visible = true;
                lblGPS.Text = "Seeking GPS...";
                lblGPS.ForeColor = ClientSettings.ErrorColor;
                this.lblGPS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));

                this.chkGPS.Text = "GPS";
                this.chkGPS.ForeColor = ClientSettings.ForeColor;
                this.chkGPS.Visible = false;
                this.chkGPS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
            
            }

            

            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = ClientSettings.FieldBackColor;
            this.textBox1.ForeColor = ClientSettings.FieldForeColor;
            this.textBox1.Location = new System.Drawing.Point(3, 65);
            //this.textBox1.MaxLength = 140;
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(234, 226);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Post Update";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.GotFocus += new System.EventHandler(this.textBox1_GotFocus);
            // 
            // lblCharsLeft
            // 
            this.lblCharsLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCharsLeft.ForeColor = ClientSettings.ForeColor;
            this.lblCharsLeft.Location = new System.Drawing.Point(198, ClientSettings.TextSize + 20);
            this.lblCharsLeft.Name = "lblCharsLeft";
            this.lblCharsLeft.Size = new System.Drawing.Size(39, 20);
            this.lblCharsLeft.Text = "label1";
            // 
            // cmbAccount
            // 
            this.cmbAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAccount.BackColor = ClientSettings.FieldBackColor;
            this.cmbAccount.ForeColor = ClientSettings.FieldForeColor;
            this.cmbAccount.Location = new System.Drawing.Point(97, 3);
            this.cmbAccount.Name = "cmbAccount";
            this.cmbAccount.Size = new System.Drawing.Size(140, 22);
            this.cmbAccount.TabIndex = 0;
            this.cmbAccount.SelectedIndexChanged += new System.EventHandler(this.cmbAccount_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.ForeColor = ClientSettings.ForeColor;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 20);
            this.label1.Text = "From Account:";
            
            // 
            // SetStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = ClientSettings.BackColor;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.urlPictureBox);
            this.Controls.Add(this.filePictureBox);
            this.Controls.Add(this.cameraPictureBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbAccount);
            this.Controls.Add(this.lblCharsLeft);
            this.Controls.Add(this.textBox1);
            this.ForeColor = ClientSettings.ForeColor;
            this.Location = new System.Drawing.Point(0, 0);
            this.Menu = this.mainMenu1;
            this.Name = "SetStatus";
            this.Text = "Post Update";

            switch (DetectDevice.DeviceType)
            {
                case DeviceType.Professional:
                    SetupProfessional();
                    break;
                case DeviceType.Standard:
                    SetupStandard();
                    break;
            }

            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbAccount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox cameraPictureBox;
        private System.Windows.Forms.PictureBox filePictureBox;
        private System.Windows.Forms.PictureBox urlPictureBox;

        private void SetupProfessional()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetStatus));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Images | *.jpg;*.jpeg";

            this.menuSubmit = new System.Windows.Forms.MenuItem();
            this.menuSubmit.Text = "Submit";
            this.menuSubmit.Click += new System.EventHandler(this.menuSubmit_Click);

            this.mainMenu1.MenuItems.Add(this.menuSubmit);
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            

            this.cameraPictureBox = new System.Windows.Forms.PictureBox();
            this.filePictureBox = new System.Windows.Forms.PictureBox();
            this.urlPictureBox = new System.Windows.Forms.PictureBox();

            this.cameraPictureBox.Image = new System.Drawing.Bitmap(ClientSettings.IconsFolder()+ "takepicture.png");
            this.cameraPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.cameraPictureBox.Location = new System.Drawing.Point(66, ClientSettings.TextSize + 20);
            this.cameraPictureBox.Name = "cameraPictureBox";
            this.cameraPictureBox.Size = new System.Drawing.Size(25, 25);
            this.cameraPictureBox.Click += new System.EventHandler(this.cameraPictureBox_Click);
            // 
            // filePictureBox
            // 
            this.filePictureBox.Image = new System.Drawing.Bitmap(ClientSettings.IconsFolder() + "existingimage.png");
            this.filePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.filePictureBox.Location = new System.Drawing.Point(35, ClientSettings.TextSize + 20);
            this.filePictureBox.Name = "filePictureBox";
            this.filePictureBox.Size = new System.Drawing.Size(25, 25);
            this.filePictureBox.Click += new System.EventHandler(this.filePictureBox_Click);
            // 
            // urlPictureBox
            // 
            this.urlPictureBox.Image = new System.Drawing.Bitmap(ClientSettings.IconsFolder() + "url.png");
            this.urlPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.urlPictureBox.Location = new System.Drawing.Point(4, ClientSettings.TextSize + 20);
            this.urlPictureBox.Name = "urlPictureBox";
            this.urlPictureBox.Size = new System.Drawing.Size(25, 25);
            this.urlPictureBox.Click += new System.EventHandler(this.urlPictureBox_Click);

            this.chkGPS.Text = "Update GPS";
            this.chkGPS.ForeColor = ClientSettings.ForeColor;
            this.chkGPS.Checked = false;

            chkGPS.Location = new System.Drawing.Point(97, ClientSettings.TextSize + 20);
            chkGPS.Size = new System.Drawing.Size(100, 20);
            lblGPS.Location = chkGPS.Location;
            lblGPS.Size = chkGPS.Size;

         
            this.copyPasteMenu = new System.Windows.Forms.ContextMenu();
            
            this.PasteItem = new System.Windows.Forms.MenuItem();
            PasteItem.Text = "Paste";

            this.CopyItem = new MenuItem();
            CopyItem.Text = "Copy";

            copyPasteMenu.MenuItems.Add(CopyItem);
            copyPasteMenu.MenuItems.Add(PasteItem);
            PasteItem.Click += new System.EventHandler(PasteItem_Click);
            CopyItem.Click += new System.EventHandler(CopyItem_Click);

            textBox1.ContextMenu = copyPasteMenu;
            this.Controls.Add(cameraPictureBox);
            this.Controls.Add(filePictureBox);
            this.Controls.Add(urlPictureBox);
            this.Controls.Add(chkGPS);
            this.Controls.Add(lblGPS);
        }

        private void SetupStandard()
        {
            this.cameraPictureBox = new PictureBox();
            this.filePictureBox = new PictureBox();

            this.cameraPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.cameraPictureBox.Location = new System.Drawing.Point(3, ClientSettings.TextSize + 20);
            this.cameraPictureBox.Name = "cameraPictureBox";
            this.cameraPictureBox.Size = new System.Drawing.Size(25, 25);

            this.filePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.filePictureBox.Location = new System.Drawing.Point(3, ClientSettings.TextSize + 20);
            this.filePictureBox.Name = "filePictureBox";
            this.filePictureBox.Size = new System.Drawing.Size(25, 25);

            this.menuSubmit = new System.Windows.Forms.MenuItem();
            this.menuSubmit.Text = "Submit";
            this.menuSubmit.Click += new System.EventHandler(this.menuSubmit_Click);

            this.menuURL = new MenuItem();
            menuURL.Text = "URL...";
            menuURL.Click += new EventHandler(menuURL_Click);

            this.menuExist = new MenuItem();
            menuExist.Text = "Existing Picture";
            menuExist.Click += new EventHandler(menuExists_Click);

            this.menuCamera = new MenuItem();
            menuCamera.Text = "Take Picture";
            menuCamera.Click += new EventHandler(menuCamera_Click);


            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem1.Text = "Action";

            this.chkGPS.Text = "Update GPS";
            this.chkGPS.ForeColor = ClientSettings.ForeColor;
            this.chkGPS.Checked = ClientSettings.UseGPS;

            chkGPS.Location = new System.Drawing.Point(57, ClientSettings.TextSize + 20);
            chkGPS.Size = new System.Drawing.Size(140, 20);
            lblGPS.Location = chkGPS.Location;
            lblGPS.Size = chkGPS.Size;

            this.mainMenu1.MenuItems.Add(this.menuCancel);
            
            this.menuItem1.MenuItems.Add(this.menuSubmit);
            this.menuItem1.MenuItems.Add(menuURL);
            this.menuItem1.MenuItems.Add(menuExist);
            this.menuItem1.MenuItems.Add(menuCamera);
            this.mainMenu1.MenuItems.Add(menuItem1);

            this.Controls.Add(chkGPS);
            this.Controls.Add(lblGPS);
            this.Controls.Add(cameraPictureBox);
            this.Controls.Add(filePictureBox);
        }

    }
}