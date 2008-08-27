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
            this.chkGPS.ForeColor = ClientSettings.ForeColor;
            this.chkGPS.Location = new System.Drawing.Point(2, 29);
            this.chkGPS.Name = "chkGPS";
            this.chkGPS.Size = new System.Drawing.Size(235, 20);
            this.chkGPS.TabIndex = 2;
            this.chkGPS.Text = "Use GPS";
            // 
            // chkVersion
            // 
            this.chkVersion.ForeColor = ClientSettings.ForeColor;
            this.chkVersion.Location = new System.Drawing.Point(2, 3);
            this.chkVersion.Name = "chkVersion";
            this.chkVersion.Size = new System.Drawing.Size(235, 20);
            this.chkVersion.TabIndex = 1;
            this.chkVersion.Text = "Automatically check for new version";
            // 
            // lblUpDates
            // 
            this.lblUpDates.ForeColor = ClientSettings.ForeColor;
            this.lblUpDates.Location = new System.Drawing.Point(3, 52);
            this.lblUpDates.Name = "lblUpDates";
            this.lblUpDates.Size = new System.Drawing.Size(234, 20);
            this.lblUpDates.Text = "Automatic Update (Milliseconds)";
            // 
            // txtUpdate
            // 
            this.txtUpdate.Location = new System.Drawing.Point(3, 75);
            this.txtUpdate.Name = "txtUpdate";
            this.txtUpdate.Size = new System.Drawing.Size(90, 21);
            this.txtUpdate.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.ForeColor = ClientSettings.ForeColor;
            this.label1.Location = new System.Drawing.Point(99, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 20);
            this.label1.Text = "0 to disable";
            // 
            // OtherSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = ClientSettings.BackColor;
            this.ClientSize = new System.Drawing.Size(240, 268);
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

    }
}