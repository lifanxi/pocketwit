namespace PockeTwit
{
    partial class TweetList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.otherStatslist = new FingerUI.KListControl();
            this.tmrautoUpdate = new System.Windows.Forms.Timer();
            this.friendsStatslist = new FingerUI.KListControl();
            this.SuspendLayout();
            // 
            // otherStatslist
            // 
            this.otherStatslist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.otherStatslist.Location = new System.Drawing.Point(0, 0);
            this.otherStatslist.Name = "otherStatslist";
            this.otherStatslist.Size = new System.Drawing.Size(240, 294);
            this.otherStatslist.TabIndex = 0;
            // 
            // tmrautoUpdate
            // 
            this.tmrautoUpdate.Tick += new System.EventHandler(this.tmrautoUpdate_Tick);
            // 
            // friendsStatslist
            // 
            this.friendsStatslist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.friendsStatslist.Location = new System.Drawing.Point(0, 0);
            this.friendsStatslist.Name = "friendsStatslist";
            this.friendsStatslist.Size = new System.Drawing.Size(240, 294);
            this.friendsStatslist.TabIndex = 1;
            // 
            // TweetList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.friendsStatslist);
            this.Controls.Add(this.otherStatslist);
            this.Name = "TweetList";
            this.Text = "PockeTwit";
            this.ResumeLayout(false);

        }

        #endregion

        private FingerUI.KListControl otherStatslist;
        private System.Windows.Forms.Timer tmrautoUpdate;
        private FingerUI.KListControl friendsStatslist;
    }
}

