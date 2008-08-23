namespace PockeTwit
{
    partial class SearchForm
    {

		#region Fields (6) 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.MenuItem menuSearch;
        private System.Windows.Forms.TextBox txtSearch;

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
            this.menuSearch = new System.Windows.Forms.MenuItem();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblWithin = new System.Windows.Forms.Label();
            this.cmbDistance = new System.Windows.Forms.ComboBox();
            this.cmbMeasurement = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblGPSStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            this.mainMenu1.MenuItems.Add(this.menuSearch);
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // menuSearch
            // 
            this.menuSearch.Text = "Search";
            this.menuSearch.Click += new System.EventHandler(this.menuSearch_Click);
            // 
            // lblSearch
            // 
            this.lblSearch.BackColor = System.Drawing.Color.Black;
            this.lblSearch.ForeColor = System.Drawing.Color.LightGray;
            this.lblSearch.Location = new System.Drawing.Point(3, 9);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(166, 20);
            this.lblSearch.Text = "Search Twitter:";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BackColor = System.Drawing.Color.DimGray;
            this.txtSearch.ForeColor = System.Drawing.Color.LightGray;
            this.txtSearch.Location = new System.Drawing.Point(57, 57);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(180, 21);
            this.txtSearch.TabIndex = 1;
            // 
            // lblWithin
            // 
            this.lblWithin.ForeColor = System.Drawing.Color.LightGray;
            this.lblWithin.Location = new System.Drawing.Point(3, 31);
            this.lblWithin.Name = "lblWithin";
            this.lblWithin.Size = new System.Drawing.Size(48, 20);
            this.lblWithin.Text = "within:";
            this.lblWithin.Visible = false;
            // 
            // cmbDistance
            // 
            this.cmbDistance.BackColor = System.Drawing.Color.DimGray;
            this.cmbDistance.Enabled = false;
            this.cmbDistance.Items.Add("1");
            this.cmbDistance.Items.Add("3");
            this.cmbDistance.Items.Add("5");
            this.cmbDistance.Items.Add("10");
            this.cmbDistance.Items.Add("15");
            this.cmbDistance.Items.Add("30");
            this.cmbDistance.Location = new System.Drawing.Point(57, 29);
            this.cmbDistance.Name = "cmbDistance";
            this.cmbDistance.Size = new System.Drawing.Size(44, 22);
            this.cmbDistance.TabIndex = 4;
            this.cmbDistance.Visible = false;
            // 
            // cmbMeasurement
            // 
            this.cmbMeasurement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMeasurement.BackColor = System.Drawing.Color.DimGray;
            this.cmbMeasurement.Enabled = false;
            this.cmbMeasurement.Items.Add("Miles");
            this.cmbMeasurement.Items.Add("Kilometers");
            this.cmbMeasurement.Location = new System.Drawing.Point(107, 29);
            this.cmbMeasurement.Name = "cmbMeasurement";
            this.cmbMeasurement.Size = new System.Drawing.Size(130, 22);
            this.cmbMeasurement.TabIndex = 5;
            this.cmbMeasurement.Visible = false;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(3, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.Text = "for:";
            // 
            // lblGPSStatus
            // 
            this.lblGPSStatus.ForeColor = System.Drawing.Color.Red;
            this.lblGPSStatus.Location = new System.Drawing.Point(3, 29);
            this.lblGPSStatus.Name = "lblGPSStatus";
            this.lblGPSStatus.Size = new System.Drawing.Size(234, 25);
            this.lblGPSStatus.Text = "label2";
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lblGPSStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbMeasurement);
            this.Controls.Add(this.cmbDistance);
            this.Controls.Add(this.lblWithin);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Menu = this.mainMenu1;
            this.Name = "SearchForm";
            this.Text = "Twitter Search";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblWithin;
        private System.Windows.Forms.ComboBox cmbDistance;
        private System.Windows.Forms.ComboBox cmbMeasurement;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblGPSStatus;
    }
}