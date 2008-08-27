namespace PockeTwit
{
    partial class AvatarSettings
    {

		#region Fields (17) 

        private System.Windows.Forms.CheckBox chkReplyImages;
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
            this.chkReplyImages = new System.Windows.Forms.CheckBox();
            this.lnkClearAvatars = new System.Windows.Forms.LinkLabel();
            this.chkAvatar = new System.Windows.Forms.CheckBox();
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
            // chkReplyImages
            // 
            this.chkReplyImages.ForeColor = ClientSettings.ForeColor;
            this.chkReplyImages.Location = new System.Drawing.Point(2, 29);
            this.chkReplyImages.Name = "chkReplyImages";
            this.chkReplyImages.Size = new System.Drawing.Size(235, 20);
            this.chkReplyImages.TabIndex = 1;
            this.chkReplyImages.Text = "Show Reply Avatars";
            // 
            // lnkClearAvatars
            // 
            this.lnkClearAvatars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkClearAvatars.ForeColor = ClientSettings.LinkColor;
            this.lnkClearAvatars.Location = new System.Drawing.Point(126, 248);
            this.lnkClearAvatars.Name = "lnkClearAvatars";
            this.lnkClearAvatars.Size = new System.Drawing.Size(109, 20);
            this.lnkClearAvatars.TabIndex = 2;
            this.lnkClearAvatars.Text = "Clear Avatar Cache";
            this.lnkClearAvatars.Click += new System.EventHandler(this.lnkClearAvatars_Click);
            // 
            // chkAvatar
            // 
            this.chkAvatar.ForeColor = ClientSettings.ForeColor;
            this.chkAvatar.Location = new System.Drawing.Point(2, 3);
            this.chkAvatar.Name = "chkAvatar";
            this.chkAvatar.Size = new System.Drawing.Size(235, 20);
            this.chkAvatar.TabIndex = 0;
            this.chkAvatar.Text = "Show Avatars";
            // 
            // AvatarSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = ClientSettings.BackColor;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.chkAvatar);
            this.Controls.Add(this.lnkClearAvatars);
            this.Controls.Add(this.chkReplyImages);
            this.Menu = this.mainMenu1;
            this.Name = "AvatarSettings";
            this.Text = "Avatar Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnkClearAvatars;
        private System.Windows.Forms.CheckBox chkAvatar;

    }
}