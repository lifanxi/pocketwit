namespace PockeTwit
{
    partial class AccountsForm
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
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.menuAccept = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.lnkAdd = new System.Windows.Forms.LinkLabel();
            this.lstAccounts = new System.Windows.Forms.ListView();
            this.lnkEdit = new System.Windows.Forms.LinkLabel();
            this.lnkRemove = new System.Windows.Forms.LinkLabel();
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
            // label1
            // 
            this.label1.BackColor = ClientSettings.BackColor;
            this.label1.ForeColor = ClientSettings.ForeColor;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.Text = "Accounts:";
            // 
            // lnkAdd
            // 
            this.lnkAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkAdd.ForeColor = ClientSettings.LinkColor;
            this.lnkAdd.Location = new System.Drawing.Point(4, 129);
            this.lnkAdd.Name = "lnkAdd";
            this.lnkAdd.Size = new System.Drawing.Size(149, 20);
            this.lnkAdd.TabIndex = 1;
            this.lnkAdd.Text = "Add New Account";
            this.lnkAdd.Click += new System.EventHandler(this.lnkAdd_Click);
            // 
            // lstAccounts
            // 
            this.lstAccounts.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lstAccounts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstAccounts.BackColor = ClientSettings.ForeColor;
            this.lstAccounts.ForeColor = ClientSettings.BackColor;
            this.lstAccounts.FullRowSelect = true;
            this.lstAccounts.Location = new System.Drawing.Point(4, 27);
            this.lstAccounts.Name = "lstAccounts";
            this.lstAccounts.Size = new System.Drawing.Size(233, 96);
            this.lstAccounts.TabIndex = 9;
            this.lstAccounts.View = System.Windows.Forms.View.List;
            // 
            // lnkEdit
            // 
            this.lnkEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkEdit.ForeColor = ClientSettings.LinkColor;
            this.lnkEdit.Location = new System.Drawing.Point(4, 149);
            this.lnkEdit.Name = "lnkEdit";
            this.lnkEdit.Size = new System.Drawing.Size(149, 20);
            this.lnkEdit.TabIndex = 2;
            this.lnkEdit.Text = "Edit Selected Account";
            this.lnkEdit.Click += new System.EventHandler(this.lnkEdit_Click);
            // 
            // lnkRemove
            // 
            this.lnkRemove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkRemove.ForeColor = ClientSettings.LinkColor;
            this.lnkRemove.Location = new System.Drawing.Point(4, 169);
            this.lnkRemove.Name = "lnkRemove";
            this.lnkRemove.Size = new System.Drawing.Size(149, 20);
            this.lnkRemove.TabIndex = 3;
            this.lnkRemove.Text = "Remove Selected Account";
            this.lnkRemove.Click += new System.EventHandler(this.lnkRemove_Click);
            // 
            // AccountsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = ClientSettings.BackColor;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lnkRemove);
            this.Controls.Add(this.lnkEdit);
            this.Controls.Add(this.lstAccounts);
            this.Controls.Add(this.lnkAdd);
            this.Controls.Add(this.label1);
            this.ForeColor = ClientSettings.ForeColor;
            this.Menu = this.mainMenu1;
            this.Name = "AccountsForm";
            this.Text = "Accounts";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.MenuItem menuAccept;
        private System.Windows.Forms.LinkLabel lnkAdd;
        private System.Windows.Forms.ListView lstAccounts;
        private System.Windows.Forms.LinkLabel lnkEdit;
        private System.Windows.Forms.LinkLabel lnkRemove;
    }
}