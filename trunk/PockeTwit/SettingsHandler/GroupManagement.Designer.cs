namespace PockeTwit.SettingsHandler
{
    partial class GroupManagement
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbChooseGroup = new System.Windows.Forms.ComboBox();
            this.lnkDeleteGroup = new System.Windows.Forms.LinkLabel();
            this.pnlUsers = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.pnlUsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Done";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 268);
            this.label1.Text = "No groups defined";
            // 
            // cmbChooseGroup
            // 
            this.cmbChooseGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbChooseGroup.Location = new System.Drawing.Point(3, 3);
            this.cmbChooseGroup.Name = "cmbChooseGroup";
            this.cmbChooseGroup.Size = new System.Drawing.Size(183, 22);
            this.cmbChooseGroup.TabIndex = 1;
            this.cmbChooseGroup.SelectedIndexChanged += new System.EventHandler(this.cmbChooseGroup_SelectedIndexChanged);
            // 
            // lnkDeleteGroup
            // 
            this.lnkDeleteGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkDeleteGroup.Location = new System.Drawing.Point(192, 5);
            this.lnkDeleteGroup.Name = "lnkDeleteGroup";
            this.lnkDeleteGroup.Size = new System.Drawing.Size(45, 20);
            this.lnkDeleteGroup.TabIndex = 2;
            this.lnkDeleteGroup.Text = "Delete";
            this.lnkDeleteGroup.Click += new System.EventHandler(this.lnkDeleteGroup_Click);
            // 
            // pnlUsers
            // 
            this.pnlUsers.AutoScroll = true;
            this.pnlUsers.Controls.Add(this.checkBox1);
            this.pnlUsers.Controls.Add(this.label3);
            this.pnlUsers.Controls.Add(this.label2);
            this.pnlUsers.Location = new System.Drawing.Point(4, 32);
            this.pnlUsers.Name = "pnlUsers";
            this.pnlUsers.Size = new System.Drawing.Size(233, 233);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(159, 20);
            this.label2.Text = "Username";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(168, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 20);
            this.label3.Text = "Exclusive";
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(168, 33);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(22, 20);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "checkBox1";
            // 
            // GroupManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.pnlUsers);
            this.Controls.Add(this.lnkDeleteGroup);
            this.Controls.Add(this.cmbChooseGroup);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "GroupManagement";
            this.Text = "Group Management";
            this.pnlUsers.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.ComboBox cmbChooseGroup;
        private System.Windows.Forms.LinkLabel lnkDeleteGroup;
        private System.Windows.Forms.Panel pnlUsers;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}