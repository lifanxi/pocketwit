namespace PockeTwit
{
    partial class SettingsForm
    {

		#region Fields (17) 

        private System.Windows.Forms.CheckBox chkBeep;
        private System.Windows.Forms.CheckBox chkReplyImages;
        private System.Windows.Forms.CheckBox chkVersion;
        private System.Windows.Forms.ComboBox cmbServers;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuAccept;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.TextBox txtMaxTweets;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserName;

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
            this.label1 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkVersion = new System.Windows.Forms.CheckBox();
            this.lblError = new System.Windows.Forms.Label();
            this.chkBeep = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbServers = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxTweets = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkReplyImages = new System.Windows.Forms.CheckBox();
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
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(4, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.Text = "User:";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Location = new System.Drawing.Point(82, 27);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(155, 21);
            this.txtUserName.TabIndex = 0;
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(82, 55);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(155, 21);
            this.txtPassword.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.LightGray;
            this.label2.Location = new System.Drawing.Point(5, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 20);
            this.label2.Text = "Password:";
            // 
            // chkVersion
            // 
            this.chkVersion.ForeColor = System.Drawing.Color.LightGray;
            this.chkVersion.Location = new System.Drawing.Point(2, 189);
            this.chkVersion.Name = "chkVersion";
            this.chkVersion.Size = new System.Drawing.Size(235, 20);
            this.chkVersion.TabIndex = 5;
            this.chkVersion.Text = "Automatically check for new version";
            // 
            // lblError
            // 
            this.lblError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(4, 4);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(233, 20);
            this.lblError.Text = "Unable to verify username and password";
            this.lblError.Visible = false;
            // 
            // chkBeep
            // 
            this.chkBeep.ForeColor = System.Drawing.Color.LightGray;
            this.chkBeep.Location = new System.Drawing.Point(2, 137);
            this.chkBeep.Name = "chkBeep";
            this.chkBeep.Size = new System.Drawing.Size(235, 20);
            this.chkBeep.TabIndex = 4;
            this.chkBeep.Text = "Beep on new messages";
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.LightGray;
            this.label3.Location = new System.Drawing.Point(5, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 20);
            this.label3.Text = "Server:";
            // 
            // cmbServers
            // 
            this.cmbServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbServers.Location = new System.Drawing.Point(82, 82);
            this.cmbServers.Name = "cmbServers";
            this.cmbServers.Size = new System.Drawing.Size(155, 22);
            this.cmbServers.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.LightGray;
            this.label4.Location = new System.Drawing.Point(5, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 20);
            this.label4.Text = "Max Length:";
            // 
            // txtMaxTweets
            // 
            this.txtMaxTweets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxTweets.Location = new System.Drawing.Point(82, 110);
            this.txtMaxTweets.Name = "txtMaxTweets";
            this.txtMaxTweets.Size = new System.Drawing.Size(92, 21);
            this.txtMaxTweets.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.ForeColor = System.Drawing.Color.LightGray;
            this.label5.Location = new System.Drawing.Point(180, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 20);
            this.label5.Text = "(10-200)";
            // 
            // chkReplyImages
            // 
            this.chkReplyImages.ForeColor = System.Drawing.Color.LightGray;
            this.chkReplyImages.Location = new System.Drawing.Point(2, 163);
            this.chkReplyImages.Name = "chkReplyImages";
            this.chkReplyImages.Size = new System.Drawing.Size(235, 20);
            this.chkReplyImages.TabIndex = 9;
            this.chkReplyImages.Text = "Show reply images";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.chkReplyImages);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtMaxTweets);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbServers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkBeep);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.chkVersion);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);

        }

        #endregion
    }
}