namespace PockeTwit
{
    partial class ProfileMap
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
            this.menuZoomOut = new System.Windows.Forms.MenuItem();
            this.menuZoomIn = new System.Windows.Forms.MenuItem();
            this.myPictureBox = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Close Map";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.MenuItems.Add(this.menuZoomOut);
            this.menuItem2.MenuItems.Add(this.menuZoomIn);
            this.menuItem2.Text = "Actions";
            // 
            // menuZoomOut
            // 
            this.menuZoomOut.Text = "Zoom Out";
            this.menuZoomOut.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // menuZoomIn
            // 
            this.menuZoomIn.Text = "Zoom In";
            this.menuZoomIn.Click += new System.EventHandler(this.menuZoomIn_Click);
            // 
            // myPictureBox
            // 
            this.myPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myPictureBox.Location = new System.Drawing.Point(0, 0);
            this.myPictureBox.Name = "myPictureBox";
            this.myPictureBox.Size = new System.Drawing.Size(240, 268);
            // 
            // ProfileMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.myPictureBox);
            this.Menu = this.mainMenu1;
            this.Name = "ProfileMap";
            this.Text = "ProfileMap";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox myPictureBox;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuZoomOut;
        private System.Windows.Forms.MenuItem menuZoomIn;
    }
}