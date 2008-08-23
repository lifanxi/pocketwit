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
                lblGPS.ForeColor = System.Drawing.Color.Red;
            }

            this.chkGPS.Text = "GPS";
            this.chkGPS.ForeColor = ClientSettings.ForeColor;
            this.chkGPS.Visible = false;

            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuCancel);
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
            this.textBox1.BackColor = System.Drawing.Color.Black;
            this.textBox1.ForeColor = System.Drawing.Color.LightGray;
            this.textBox1.Location = new System.Drawing.Point(3, 65);
            this.textBox1.MaxLength = 140;
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(234, 226);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Set Status";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.GotFocus += new System.EventHandler(this.textBox1_GotFocus);
            // 
            // lblCharsLeft
            // 
            this.lblCharsLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCharsLeft.ForeColor = System.Drawing.Color.LightGray;
            this.lblCharsLeft.Location = new System.Drawing.Point(198, ClientSettings.TextSize + 20);
            this.lblCharsLeft.Name = "lblCharsLeft";
            this.lblCharsLeft.Size = new System.Drawing.Size(39, 20);
            this.lblCharsLeft.Text = "label1";
            // 
            // cmbAccount
            // 
            this.cmbAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAccount.BackColor = System.Drawing.Color.DimGray;
            this.cmbAccount.ForeColor = System.Drawing.Color.LightGray;
            this.cmbAccount.Location = new System.Drawing.Point(97, 3);
            this.cmbAccount.Name = "cmbAccount";
            this.cmbAccount.Size = new System.Drawing.Size(140, 22);
            this.cmbAccount.TabIndex = 0;
            this.cmbAccount.SelectedIndexChanged += new System.EventHandler(this.cmbAccount_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.LightGray;
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
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbAccount);
            this.Controls.Add(this.lblCharsLeft);
            this.Controls.Add(this.textBox1);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Location = new System.Drawing.Point(0, 0);
            this.Menu = this.mainMenu1;
            this.Name = "SetStatus";
            this.Text = "Set Status";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

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
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;

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

            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();

            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.Location = new System.Drawing.Point(66, ClientSettings.TextSize + 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(25, 25);
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox2.Location = new System.Drawing.Point(35, ClientSettings.TextSize + 20);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(25, 25);
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox3.Location = new System.Drawing.Point(4, ClientSettings.TextSize + 20);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(25, 25);
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);

            this.chkGPS.Text = "GPS";
            this.chkGPS.ForeColor = ClientSettings.ForeColor;
            this.chkGPS.Location = new System.Drawing.Point(97, ClientSettings.TextSize+20);
            this.chkGPS.Size = new System.Drawing.Size(100, 20);
            this.chkGPS.Checked = ClientSettings.UseGPS;

            lblGPS.Location = chkGPS.Location;
            lblGPS.Size = chkGPS.Size;

            this.Controls.Add(pictureBox1);
            this.Controls.Add(pictureBox2);
            this.Controls.Add(pictureBox3);
            this.Controls.Add(chkGPS);
        }

        private void SetupStandard()
        {
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

            this.menuItem1.MenuItems.Add(this.menuSubmit);
            this.menuItem1.MenuItems.Add(menuURL);
            this.menuItem1.MenuItems.Add(menuExist);
            this.menuItem1.MenuItems.Add(menuCamera);
            this.mainMenu1.MenuItems.Add(menuItem1);
        }

    }
}