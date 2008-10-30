using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class URLForm : Form
    {

		#region Fields (1) 

        private string _URL;

		#endregion Fields 

		#region Constructors (1) 

        public URLForm()
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

        public string URL
        {
            get
            {
                return _URL;
            }
        }

		#endregion Properties 

		#region Methods (2) 


		// Private Methods (2) 

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void menuOK_Click(object sender, EventArgs e)
        {
            try
            {
                _URL = isgd.ShortenURL(this.txtURL.Text);
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
            catch { }
            
        }


		#endregion Methods 

    }
}