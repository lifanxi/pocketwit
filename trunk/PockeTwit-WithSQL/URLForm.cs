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
        ContextMenu contextMen;
        MenuItem pasteItem;
        public URLForm()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                SetPasteMenu();
            }
        }

        private void SetPasteMenu()
        {
            contextMen = new ContextMenu();
            pasteItem = new MenuItem();
            pasteItem.Text = "Paste";
            contextMen.MenuItems.Add(pasteItem);
            this.txtURL.ContextMenu = contextMen;
            pasteItem.Click += new EventHandler(pasteItem_Click);
        }

        void pasteItem_Click(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                txtURL.SelectedText = (string)iData.GetData(DataFormats.Text); 
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
            try
            {
                _URL = isgd.ShortenURL(this.txtURL.Text);
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
            catch { }
            
        }

        private void menuOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            
        }


		#endregion Methods 

    }
}