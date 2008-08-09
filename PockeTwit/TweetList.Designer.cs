namespace PockeTwit
{
    partial class TweetList
    {

		#region Fields (8) 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Label lblTitle;
        private Microsoft.WindowsCE.Forms.Notification notification1;
        private FingerUI.KListControl statList;
        private System.Windows.Forms.Timer timerStartup;
        private System.Windows.Forms.Timer tmrautoUpdate;

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
            this.tmrautoUpdate = new System.Windows.Forms.Timer();
            this.timerStartup = new System.Windows.Forms.Timer();
            this.statList = new FingerUI.KListControl();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblLoading = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tmrautoUpdate
            // 
            this.tmrautoUpdate.Tick += new System.EventHandler(this.tmrautoUpdate_Tick);
            // 
            // timerStartup
            // 
            this.timerStartup.Enabled = true;
            this.timerStartup.Interval = 1000;
            this.timerStartup.Tick += new System.EventHandler(this.timerStartup_Tick);
            // 
            // statList
            // 
            this.statList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statList.Location = new System.Drawing.Point(0, 0);
            this.statList.Name = "statList";
            this.statList.Size = new System.Drawing.Size(240, 294);
            this.statList.TabIndex = 0;
            this.statList.Visible = false;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.lblTitle.ForeColor = System.Drawing.Color.LightGray;
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(169, 31);
            this.lblTitle.Text = "Launching PockeTwit";
            // 
            // lblLoading
            // 
            this.lblLoading.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.lblLoading.ForeColor = System.Drawing.Color.LightGray;
            this.lblLoading.Location = new System.Drawing.Point(3, 31);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(193, 55);
            this.lblLoading.Text = "Loading. . .";
            // 
            // TweetList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.statList);
            this.Name = "TweetList";
            this.Text = "PockeTwit";
            this.ResumeLayout(false);

        }

        #endregion
    }
}

