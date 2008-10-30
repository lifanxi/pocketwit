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
        private FingerUI.KListControl statList;
        private System.Windows.Forms.Timer timerStartup;
        
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
            this.timerStartup = new System.Windows.Forms.Timer();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblLoading = new System.Windows.Forms.Label();
            this.statList = new FingerUI.KListControl();
            
            this.SuspendLayout();
            // 
            // timerStartup
            // 
            this.timerStartup.Interval = 1000;
            this.timerStartup.Tick += new System.EventHandler(this.timerStartup_Tick);
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.lblTitle.ForeColor = ClientSettings.ForeColor;
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(234, 31);
            this.lblTitle.Text = "Launching PockeTwit";
            if (UpdateChecker.devBuild)
            {
                this.lblTitle.Text = "Launching PockeTwit Dev";
            }
            // 
            // lblLoading
            // 
            this.lblLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLoading.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular);
            this.lblLoading.ForeColor = ClientSettings.ForeColor;
            this.lblLoading.Location = new System.Drawing.Point(3, 31);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(234, 55);
            this.lblLoading.Text = "Loading. . .";
            // 
            // statList
            // 
            this.statList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.statList.Location = new System.Drawing.Point(0, 0);
            this.statList.Name = "statList";
            this.statList.Size = new System.Drawing.Size(240, 294);
            this.statList.TabIndex = 0;
            this.statList.Visible = false;
            // 
            // TweetList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = ClientSettings.BackColor; ;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.statList);
            this.Name = "TweetList";
            this.Text = "PockeTwit";

            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
            }

            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
    }
}

