namespace PockeTwit
{
    partial class AboutForm
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
            this.menuUpdate = new System.Windows.Forms.MenuItem();
            this.menuClose = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuUpdate);
            this.mainMenu1.MenuItems.Add(this.menuClose);
            // 
            // menuUpdate
            // 
            this.menuUpdate.Text = "Check Upgrades";
            this.menuUpdate.Click += new System.EventHandler(this.menuUpdate_Click);
            // 
            // menuClose
            // 
            this.menuClose.Text = "Close";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 20);
            this.label1.Text = "PockeTwit";
            // 
            // lblVersion
            // 
            this.lblVersion.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblVersion.ForeColor = System.Drawing.Color.LightGray;
            this.lblVersion.Location = new System.Drawing.Point(96, 4);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(64, 20);
            this.lblVersion.Text = "v0.0";
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.LightGray;
            this.label3.Location = new System.Drawing.Point(4, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(232, 31);
            this.label3.Text = "I\'m just doing this for fun, so please let me know what you think:\r\n";
            // 
            // linkLabel1
            // 
            this.linkLabel1.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel1.Location = new System.Drawing.Point(3, 95);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(233, 23);
            this.linkLabel1.TabIndex = 7;
            this.linkLabel1.Text = "http://twitter.com/pocketwitdev/";
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // linkLabel2
            // 
            this.linkLabel2.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel2.Location = new System.Drawing.Point(3, 24);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(233, 23);
            this.linkLabel2.TabIndex = 8;
            this.linkLabel2.Text = "http://code.google.com/p/pocketwit/";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "AboutForm";
            this.Text = "About PockeTwit";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuItem menuUpdate;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.MenuItem menuClose;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
    }
}