namespace PockeTwit
{
    partial class AccountInfoForm
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
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.cmbServers = new System.Windows.Forms.ComboBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Cancel";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Accept";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            
            // 
            // lblServer
            // 
            this.lblServer.BackColor = ClientSettings.BackColor;
            this.lblServer.ForeColor = ClientSettings.ForeColor;
            this.lblServer.Location = new System.Drawing.Point(5, 5);
            //this.lblServer.Location = new System.Drawing.Point(5, 103);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(71, 20);
            this.lblServer.Text = "Server:";
            // 
            // cmbServers
            // 
            this.cmbServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbServers.BackColor = ClientSettings.FieldBackColor;
            this.cmbServers.ForeColor = ClientSettings.FieldForeColor;
            //this.cmbServers.Location = new System.Drawing.Point(5, 126);
            this.cmbServers.Location = new System.Drawing.Point(5, 28);
            this.cmbServers.Name = "cmbServers";
            this.cmbServers.Size = new System.Drawing.Size(232, 22);
            this.cmbServers.TabIndex = 0;
            this.cmbServers.SelectedIndexChanged += new System.EventHandler(this.cmbServers_SelectedIndexChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.BackColor = ClientSettings.FieldBackColor;
            this.txtPassword.ForeColor = ClientSettings.FieldForeColor;
            //this.txtPassword.Location = new System.Drawing.Point(5, 79);
            this.txtPassword.Location = new System.Drawing.Point(5, 126);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(232, 21);
            this.txtPassword.TabIndex = 2;
            // 
            // lblPassword
            // 
            this.lblPassword.BackColor = ClientSettings.BackColor;
            this.lblPassword.ForeColor = ClientSettings.ForeColor;
            //this.lblPassword.Location = new System.Drawing.Point(5, 56);
            this.lblPassword.Location = new System.Drawing.Point(5, 103); 
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(71, 20);
            this.lblPassword.Text = "Password:";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.BackColor = ClientSettings.FieldBackColor;
            this.txtUserName.ForeColor = ClientSettings.FieldForeColor;
            //this.txtUserName.Location = new System.Drawing.Point(5, 28);
            this.txtUserName.Location = new System.Drawing.Point(5, 79);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(232, 21);
            this.txtUserName.TabIndex = 1;
            // 
            // lblUser
            // 
            this.lblUser.BackColor = ClientSettings.BackColor;
            this.lblUser.ForeColor = ClientSettings.ForeColor;
            this.lblUser.Location = new System.Drawing.Point(5, 56);
            //this.lblUser.Location = new System.Drawing.Point(5, 5);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(72, 20);
            this.lblUser.Text = "User:";
            // 
            // lblError
            // 
            this.lblError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblError.BackColor = ClientSettings.BackColor;
            this.lblError.ForeColor = ClientSettings.ErrorColor; ;
            this.lblError.Location = new System.Drawing.Point(83, 154);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(152, 20);
            this.lblError.Text = "Unable to verify username and password";
            this.lblError.Visible = false;
            // 
            // AccountInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = ClientSettings.BackColor;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.cmbServers);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.lblUser);
            this.Menu = this.mainMenu1;
            this.Name = "AccountInfoForm";
            this.Text = "Account Info";
            
            switch (DetectDevice.DeviceType)
            {
                case DeviceType.Professional:
                    SetupProfessional();
                    break;
                case DeviceType.Standard:
                    //SetupStandard();
                    break;
            }
            
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbServers;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.ContextMenu copyPasteMenu;
        private System.Windows.Forms.MenuItem PasteItem;
        private void SetupProfessional()
        {
            this.copyPasteMenu = new System.Windows.Forms.ContextMenu();
            this.PasteItem = new System.Windows.Forms.MenuItem();
            PasteItem.Text = "Paste";
            copyPasteMenu.MenuItems.Add(PasteItem);
            PasteItem.Click += new System.EventHandler(PasteItem_Click);
        }

        
		
    }
}