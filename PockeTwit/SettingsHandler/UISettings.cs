using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class UISettings : Form
    {

        #region Constructors (1) 

        public UISettings()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            PopulateForm();
        }

		#endregion Constructors 

		#region Properties (1) 

        public bool NeedsReset { get; set; }

		#endregion Properties 

		#region Methods (4) 


		// Private Methods (4) 

        

        private void menuAccept_Click(object sender, EventArgs e)
        {
            lblTweets.ForeColor = ClientSettings.ForeColor;
            int MaxTweets = 0;
            try
            {
                MaxTweets = int.Parse(txtMaxTweets.Text);
                if (MaxTweets > 200 || MaxTweets < 10)
                {
                    lblTweets.ForeColor = ClientSettings.ErrorColor;
                    Cursor.Current = Cursors.Default;
                    return;
                }
            }
            catch
            {
                lblTweets.ForeColor = ClientSettings.ErrorColor;
                Cursor.Current = Cursors.Default;
                return;
            }
            if (MaxTweets != ClientSettings.MaxTweets) { NeedsReset = true; }
            if (chkTimestamps.Checked != ClientSettings.ShowExtra) { NeedsReset = true; }
            ClientSettings.SaveSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PopulateForm()
        {
            txtMaxTweets.Text = ClientSettings.MaxTweets.ToString();
            chkTimestamps.Checked = ClientSettings.ShowExtra;
            this.DialogResult = DialogResult.Cancel;
        }


		#endregion Methods 

        
    }
}