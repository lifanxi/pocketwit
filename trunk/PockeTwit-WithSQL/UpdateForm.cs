using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class UpdateForm : Form
    {

		#region Fields (1) 

        private UpdateChecker.UpdateInfo _NewVersion;

		#endregion Fields 

		#region Constructors (1) 

        public UpdateForm()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

		#endregion Constructors 

		#region Properties (1) 

        public UpdateChecker.UpdateInfo NewVersion 
        {
            set
            {
                lblVersion.Text = value.webVersion.ToString();
                lblInfo.Text = value.UpdateNotes;
                _NewVersion = value;
            }
        }

		#endregion Properties 

		#region Methods (2) 


		// Private Methods (2) 

        private void menuIgnore_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
            pi.FileName = _NewVersion.DownloadURL;
            pi.UseShellExecute = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
            Application.Exit();
        }


		#endregion Methods 

    }
}