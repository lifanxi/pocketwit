namespace PockeTwit
{
    partial class SetStatus
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblCharsLeft = new System.Windows.Forms.Label();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.menuSubmit = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            this.mainMenu1.MenuItems.Add(this.menuSubmit);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.MaxLength = 140;
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(234, 240);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // lblCharsLeft
            // 
            this.lblCharsLeft.ForeColor = System.Drawing.Color.LightGray;
            this.lblCharsLeft.Location = new System.Drawing.Point(197, 245);
            this.lblCharsLeft.Name = "lblCharsLeft";
            this.lblCharsLeft.Size = new System.Drawing.Size(39, 20);
            this.lblCharsLeft.Text = "label1";
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // menuSubmit
            // 
            this.menuSubmit.Text = "Submit";
            this.menuSubmit.Click += new System.EventHandler(this.menuSubmit_Click);
            // 
            // SetStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lblCharsLeft);
            this.Controls.Add(this.textBox1);
            this.Menu = this.mainMenu1;
            this.Name = "SetStatus";
            this.Text = "Set Status";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblCharsLeft;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.MenuItem menuSubmit;
    }
}