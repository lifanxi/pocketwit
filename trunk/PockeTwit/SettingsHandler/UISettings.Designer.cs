namespace PockeTwit
{
    partial class UISettings
    {

		#region Fields (17) 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTweets;
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
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxTweets = new System.Windows.Forms.TextBox();
            this.lblTweets = new System.Windows.Forms.Label();
            this.chkTimestamps = new System.Windows.Forms.CheckBox();
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
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.LightGray;
            this.label4.Location = new System.Drawing.Point(4, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 20);
            this.label4.Text = "Max Length:";
            // 
            // txtMaxTweets
            // 
            this.txtMaxTweets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxTweets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtMaxTweets.Location = new System.Drawing.Point(81, 3);
            this.txtMaxTweets.Name = "txtMaxTweets";
            this.txtMaxTweets.Size = new System.Drawing.Size(92, 21);
            this.txtMaxTweets.TabIndex = 0;
            // 
            // lblTweets
            // 
            this.lblTweets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTweets.ForeColor = System.Drawing.Color.LightGray;
            this.lblTweets.Location = new System.Drawing.Point(179, 4);
            this.lblTweets.Name = "lblTweets";
            this.lblTweets.Size = new System.Drawing.Size(57, 20);
            this.lblTweets.Text = "(10-200)";
            // 
            // chkTimestamps
            // 
            this.chkTimestamps.ForeColor = System.Drawing.Color.LightGray;
            this.chkTimestamps.Location = new System.Drawing.Point(2, 30);
            this.chkTimestamps.Name = "chkTimestamps";
            this.chkTimestamps.Size = new System.Drawing.Size(235, 20);
            this.chkTimestamps.TabIndex = 3;
            this.chkTimestamps.Text = "Show times";
            // 
            // UISettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.chkTimestamps);
            this.Controls.Add(this.lblTweets);
            this.Controls.Add(this.txtMaxTweets);
            this.Controls.Add(this.label4);
            this.Menu = this.mainMenu1;
            this.Name = "UISettings";
            this.Text = "UI Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkTimestamps;

    }
}