namespace PockeTwit
{
    partial class SettingsForm
    {

		#region Fields (17) 

        private System.Windows.Forms.CheckBox chkBeep;
        private System.Windows.Forms.CheckBox chkReplyImages;
        private System.Windows.Forms.CheckBox chkVersion;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuAccept;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.TextBox txtMaxTweets;

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
            this.chkVersion = new System.Windows.Forms.CheckBox();
            this.chkBeep = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxTweets = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkReplyImages = new System.Windows.Forms.CheckBox();
            this.lblError = new System.Windows.Forms.Label();
            this.btnAccounts = new System.Windows.Forms.Button();
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
            // chkVersion
            // 
            this.chkVersion.ForeColor = System.Drawing.Color.LightGray;
            this.chkVersion.Location = new System.Drawing.Point(1, 102);
            this.chkVersion.Name = "chkVersion";
            this.chkVersion.Size = new System.Drawing.Size(235, 20);
            this.chkVersion.TabIndex = 3;
            this.chkVersion.Text = "Automatically check for new version";
            // 
            // chkBeep
            // 
            this.chkBeep.ForeColor = System.Drawing.Color.LightGray;
            this.chkBeep.Location = new System.Drawing.Point(1, 50);
            this.chkBeep.Name = "chkBeep";
            this.chkBeep.Size = new System.Drawing.Size(235, 20);
            this.chkBeep.TabIndex = 1;
            this.chkBeep.Text = "Beep on new messages";
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.LightGray;
            this.label4.Location = new System.Drawing.Point(4, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 20);
            this.label4.Text = "Max Length:";
            // 
            // txtMaxTweets
            // 
            this.txtMaxTweets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxTweets.Location = new System.Drawing.Point(81, 23);
            this.txtMaxTweets.Name = "txtMaxTweets";
            this.txtMaxTweets.Size = new System.Drawing.Size(92, 21);
            this.txtMaxTweets.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.ForeColor = System.Drawing.Color.LightGray;
            this.label5.Location = new System.Drawing.Point(179, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 20);
            this.label5.Text = "(10-200)";
            // 
            // chkReplyImages
            // 
            this.chkReplyImages.ForeColor = System.Drawing.Color.LightGray;
            this.chkReplyImages.Location = new System.Drawing.Point(1, 76);
            this.chkReplyImages.Name = "chkReplyImages";
            this.chkReplyImages.Size = new System.Drawing.Size(235, 20);
            this.chkReplyImages.TabIndex = 2;
            this.chkReplyImages.Text = "Show reply images";
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
            // btnAccounts
            // 
            this.btnAccounts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccounts.Location = new System.Drawing.Point(163, 245);
            this.btnAccounts.Name = "btnAccounts";
            this.btnAccounts.Size = new System.Drawing.Size(72, 20);
            this.btnAccounts.TabIndex = 6;
            this.btnAccounts.Text = "Accounts";
            this.btnAccounts.Click += new System.EventHandler(this.btnAccounts_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.btnAccounts);
            this.Controls.Add(this.chkReplyImages);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtMaxTweets);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkBeep);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.chkVersion);
            this.Menu = this.mainMenu1;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnAccounts;

    }
}