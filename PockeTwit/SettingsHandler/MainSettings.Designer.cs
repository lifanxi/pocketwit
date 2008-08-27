namespace PockeTwit.SettingsHandler
{
    partial class MainSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuDone = new System.Windows.Forms.MenuItem();
            this.lnkManageAccounts = new System.Windows.Forms.LinkLabel();
            this.lnkAvatar = new System.Windows.Forms.LinkLabel();
            this.lnkUI = new System.Windows.Forms.LinkLabel();
            this.lnkGPS = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuDone);
            // 
            // menuDone
            // 
            this.menuDone.Text = "Done";
            this.menuDone.Click += new System.EventHandler(this.menuDone_Click);
            // 
            // lnkManageAccounts
            // 
            this.lnkManageAccounts.Location = new System.Drawing.Point(3, 9);
            this.lnkManageAccounts.Name = "lnkManageAccounts";
            this.lnkManageAccounts.Size = new System.Drawing.Size(234, 20);
            this.lnkManageAccounts.TabIndex = 0;
            this.lnkManageAccounts.Text = "Manage Accounts";
            this.lnkManageAccounts.Click += new System.EventHandler(this.lnkManageAccounts_Click);
            // 
            // lnkAvatar
            // 
            this.lnkAvatar.Location = new System.Drawing.Point(3, 29);
            this.lnkAvatar.Name = "lnkAvatar";
            this.lnkAvatar.Size = new System.Drawing.Size(234, 20);
            this.lnkAvatar.TabIndex = 1;
            this.lnkAvatar.Text = "Avatar Settings";
            this.lnkAvatar.Click += new System.EventHandler(this.lnkAvatar_Click);
            // 
            // lnkUI
            // 
            this.lnkUI.Location = new System.Drawing.Point(3, 49);
            this.lnkUI.Name = "lnkUI";
            this.lnkUI.Size = new System.Drawing.Size(234, 20);
            this.lnkUI.TabIndex = 2;
            this.lnkUI.Text = "UI Settings";
            this.lnkUI.Click += new System.EventHandler(this.lnkUI_Click);
            // 
            // lnkGPS
            // 
            this.lnkGPS.Location = new System.Drawing.Point(3, 69);
            this.lnkGPS.Name = "lnkGPS";
            this.lnkGPS.Size = new System.Drawing.Size(234, 20);
            this.lnkGPS.TabIndex = 3;
            this.lnkGPS.Text = "Other Settings";
            this.lnkGPS.Click += new System.EventHandler(this.lnkGPS_Click);
            // 
            // MainSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = ClientSettings.BackColor;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lnkGPS);
            this.Controls.Add(this.lnkUI);
            this.Controls.Add(this.lnkAvatar);
            this.Controls.Add(this.lnkManageAccounts);
            this.Menu = this.mainMenu1;
            this.Name = "MainSettings";
            this.Text = "PockeTwit Settings";
            this.Activated += new System.EventHandler(this.MainSettings_Activated);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainSettings_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainSettings_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainSettings_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnkManageAccounts;
        private System.Windows.Forms.LinkLabel lnkAvatar;
        private System.Windows.Forms.LinkLabel lnkUI;
        private System.Windows.Forms.LinkLabel lnkGPS;
        private System.Windows.Forms.MenuItem menuDone;
    }
}