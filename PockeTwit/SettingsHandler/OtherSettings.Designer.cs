namespace PockeTwit
{
    partial class OtherSettings
    {

		#region Fields (17) 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuAccept;
        private System.Windows.Forms.MenuItem menuCancel;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.menuAccept = new System.Windows.Forms.MenuItem();
            this.chkGPS = new System.Windows.Forms.CheckBox();
            this.chkVersion = new System.Windows.Forms.CheckBox();
            this.lblUpDates = new System.Windows.Forms.Label();
            this.txtUpdate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.chkBeep = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            this.mainMenu1.MenuItems.Add(this.menuAccept);
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // menuAccept
            // 
            this.menuAccept.Text = "Accept";
            this.menuAccept.Click += new System.EventHandler(this.menuAccept_Click);
            // 
            // chkGPS
            // 
            this.chkGPS.ForeColor = System.Drawing.Color.LightGray;
            this.chkGPS.Location = new System.Drawing.Point(2, 29);
            this.chkGPS.Name = "chkGPS";
            this.chkGPS.Size = new System.Drawing.Size(235, 20);
            this.chkGPS.TabIndex = 1;
            this.chkGPS.Text = "Use GPS";
            // 
            // chkVersion
            // 
            this.chkVersion.ForeColor = System.Drawing.Color.LightGray;
            this.chkVersion.Location = new System.Drawing.Point(2, 3);
            this.chkVersion.Name = "chkVersion";
            this.chkVersion.Size = new System.Drawing.Size(235, 20);
            this.chkVersion.TabIndex = 0;
            this.chkVersion.Text = "Automatically check for new version";
            // 
            // lblUpDates
            // 
            this.lblUpDates.ForeColor = System.Drawing.Color.LightGray;
            this.lblUpDates.Location = new System.Drawing.Point(6, 75);
            this.lblUpDates.Name = "lblUpDates";
            this.lblUpDates.Size = new System.Drawing.Size(234, 20);
            this.lblUpDates.Text = "Automatic Update (Minutes)";
            // 
            // txtUpdate
            // 
            this.txtUpdate.Location = new System.Drawing.Point(6, 94);
            this.txtUpdate.Name = "txtUpdate";
            this.txtUpdate.Size = new System.Drawing.Size(90, 21);
            this.txtUpdate.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(102, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 20);
            this.label1.Text = "0 to disable";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.ForeColor = System.Drawing.Color.LightBlue;
            this.linkLabel1.Location = new System.Drawing.Point(110, 248);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(130, 20);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.Text = "Configure Notifications";
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // chkBeep
            // 
            this.chkBeep.ForeColor = System.Drawing.Color.LightGray;
            this.chkBeep.Location = new System.Drawing.Point(2, 55);
            this.chkBeep.Name = "chkBeep";
            this.chkBeep.Size = new System.Drawing.Size(235, 20);
            this.chkBeep.TabIndex = 2;
            this.chkBeep.Text = "Beep on new messages";
            // 
            // OtherSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.chkBeep);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUpdate);
            this.Controls.Add(this.lblUpDates);
            this.Controls.Add(this.chkVersion);
            this.Controls.Add(this.chkGPS);
            this.Menu = this.mainMenu1;
            this.Name = "OtherSettings";
            this.Text = "Other Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkGPS;
        private System.Windows.Forms.CheckBox chkVersion;
        private System.Windows.Forms.Label lblUpDates;
        private System.Windows.Forms.TextBox txtUpdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox chkBeep;

    }
}