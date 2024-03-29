namespace PockeTwit
{
    partial class SearchForm
    {

        #region Fields (6)

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.MenuItem menuSearch;

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
            this.menuSearch = new System.Windows.Forms.MenuItem();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.chkSaveSearch = new System.Windows.Forms.CheckBox();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.lblWithin = new System.Windows.Forms.Label();
            this.cmbDistance = new System.Windows.Forms.ComboBox();
            this.cmbMeasurement = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbLocation = new System.Windows.Forms.ComboBox();
            this.txtDummy = new System.Windows.Forms.TextBox();
            this.linkLabelAdvSearch = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuSearch);
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            // 
            // menuSearch
            // 
            this.menuSearch.Text = "Search";
            this.menuSearch.Click += new System.EventHandler(this.menuSearch_Click);
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // chkSaveSearch
            // 
            this.chkSaveSearch.ForeColor = System.Drawing.SystemColors.Control;
            this.chkSaveSearch.Location = new System.Drawing.Point(4, 131);
            this.chkSaveSearch.Name = "chkSaveSearch";
            this.chkSaveSearch.Size = new System.Drawing.Size(116, 26);
            this.chkSaveSearch.TabIndex = 5;
            this.chkSaveSearch.Text = "Save as Group";
            this.chkSaveSearch.CheckStateChanged += new System.EventHandler(this.chkSaveSearch_CheckStateChanged);
            this.chkSaveSearch.Click += new System.EventHandler(this.chkSaveSearch_Click);
            // 
            // txtGroupName
            // 
            this.txtGroupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGroupName.Enabled = false;
            this.txtGroupName.Location = new System.Drawing.Point(117, 131);
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.Size = new System.Drawing.Size(120, 21);
            this.txtGroupName.TabIndex = 6;
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.Enabled = false;
            this.chkAutoUpdate.ForeColor = System.Drawing.SystemColors.Control;
            this.chkAutoUpdate.Location = new System.Drawing.Point(25, 163);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(95, 26);
            this.chkAutoUpdate.TabIndex = 7;
            this.chkAutoUpdate.Text = "AutoUpdate";
            // 
            // lblSearch
            // 
            this.lblSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearch.BackColor = System.Drawing.Color.Black;
            this.lblSearch.ForeColor = System.Drawing.Color.LightGray;
            this.lblSearch.Location = new System.Drawing.Point(3, 9);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(260, 20);
            this.lblSearch.Text = "Search Twitter:";
            // 
            // lblWithin
            // 
            this.lblWithin.ForeColor = System.Drawing.Color.LightGray;
            this.lblWithin.Location = new System.Drawing.Point(3, 31);
            this.lblWithin.Name = "lblWithin";
            this.lblWithin.Size = new System.Drawing.Size(48, 20);
            this.lblWithin.Text = "within:";
            // 
            // cmbDistance
            // 
            this.cmbDistance.BackColor = System.Drawing.Color.White;
            this.cmbDistance.ForeColor = System.Drawing.Color.Black;
            this.cmbDistance.Items.Add("1");
            this.cmbDistance.Items.Add("3");
            this.cmbDistance.Items.Add("5");
            this.cmbDistance.Items.Add("10");
            this.cmbDistance.Items.Add("15");
            this.cmbDistance.Items.Add("30");
            this.cmbDistance.Location = new System.Drawing.Point(57, 29);
            this.cmbDistance.Name = "cmbDistance";
            this.cmbDistance.Size = new System.Drawing.Size(44, 22);
            this.cmbDistance.TabIndex = 0;
            // 
            // cmbMeasurement
            // 
            this.cmbMeasurement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMeasurement.BackColor = System.Drawing.Color.White;
            this.cmbMeasurement.ForeColor = System.Drawing.Color.Black;
            this.cmbMeasurement.Items.Add("Miles");
            this.cmbMeasurement.Items.Add("Kilometers");
            this.cmbMeasurement.Location = new System.Drawing.Point(107, 29);
            this.cmbMeasurement.Name = "cmbMeasurement";
            this.cmbMeasurement.Size = new System.Drawing.Size(130, 22);
            this.cmbMeasurement.TabIndex = 1;
            this.cmbMeasurement.SelectedValueChanged += new System.EventHandler(this.cmbMeasurement_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(4, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.Text = "for:";
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.LightGray;
            this.label2.Location = new System.Drawing.Point(3, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 20);
            this.label2.Text = "of:";
            // 
            // cmbLocation
            // 
            this.cmbLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbLocation.Location = new System.Drawing.Point(57, 58);
            this.cmbLocation.Name = "cmbLocation";
            this.cmbLocation.Size = new System.Drawing.Size(180, 22);
            this.cmbLocation.TabIndex = 2;
            // 
            // txtDummy
            // 
            this.txtDummy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDummy.Location = new System.Drawing.Point(57, 87);
            this.txtDummy.Name = "txtDummy";
            this.txtDummy.Size = new System.Drawing.Size(152, 21);
            this.txtDummy.TabIndex = 3;
            this.txtDummy.Visible = false;
            // 
            // linkLabelAdvSearch
            // 
            this.linkLabelAdvSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelAdvSearch.ForeColor = System.Drawing.Color.LightGray;
            this.linkLabelAdvSearch.Location = new System.Drawing.Point(215, 88);
            this.linkLabelAdvSearch.Name = "linkLabelAdvSearch";
            this.linkLabelAdvSearch.Size = new System.Drawing.Size(20, 20);
            this.linkLabelAdvSearch.TabIndex = 12;
            this.linkLabelAdvSearch.Text = "...";
            this.linkLabelAdvSearch.Click += new System.EventHandler(this.btnAdvancedSearch_Click);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.linkLabelAdvSearch);
            this.Controls.Add(this.txtDummy);
            this.Controls.Add(this.chkAutoUpdate);
            this.Controls.Add(this.txtGroupName);
            this.Controls.Add(this.chkSaveSearch);
            this.Controls.Add(this.cmbLocation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbMeasurement);
            this.Controls.Add(this.cmbDistance);
            this.Controls.Add(this.lblWithin);
            this.Controls.Add(this.lblSearch);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Menu = this.mainMenu1;
            this.Name = "SearchForm";
            this.Text = "Twitter Search";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Control txtSearch;
        private System.Windows.Forms.CheckBox chkSaveSearch;
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Label lblWithin;
        private System.Windows.Forms.ComboBox cmbDistance;
        private System.Windows.Forms.ComboBox cmbMeasurement;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbLocation;
        private System.Windows.Forms.TextBox txtDummy;
        private System.Windows.Forms.LinkLabel linkLabelAdvSearch;
    }
}