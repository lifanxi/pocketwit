using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class SettingsForm : MasterForm
    {
        public SettingsForm()
        {
            InitializeComponent();
            PopulateForm();
        }

        private void PopulateForm()
        {
            txtUserName.Text = ClientSettings.UserName;
            txtPassword.Text = ClientSettings.Password;
            chkVersion.Checked = ClientSettings.CheckVersion;
            this.DialogResult = DialogResult.Cancel;
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void menuAccept_Click(object sender, EventArgs e)
        {
            ClientSettings.UserName = txtUserName.Text;
            ClientSettings.Password = txtPassword.Text;
            ClientSettings.CheckVersion = chkVersion.Checked;
            ClientSettings.SaveSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}