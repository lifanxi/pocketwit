using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class OtherSettings : Form
    {

        #region Constructors (1) 

        public OtherSettings()
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
            
            ClientSettings.UseGPS = chkGPS.Checked;
            ClientSettings.CheckVersion = chkVersion.Checked;
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
            chkVersion.Checked = ClientSettings.CheckVersion;
            chkGPS.Checked = ClientSettings.UseGPS;
            this.DialogResult = DialogResult.Cancel;
        }


		#endregion Methods 

    }
}