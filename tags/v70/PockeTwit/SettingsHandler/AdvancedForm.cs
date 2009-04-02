using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit.SettingsHandler
{
    public partial class AdvancedForm : Form
    {
        public AdvancedForm()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            showAddressBook();
        }
        private void showAddressBook()
        {
            lnkAddressbook.Text = "Clear addressbook: " + AddressBook.Count().ToString();
        }

        private void lnkClearCaches_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all cached statuses?", "PockeTwit", MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                TimelineManagement.ClearCaches();
            }
        }


        private void lnkClearSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete your settings?", "PockeTwit", MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                System.IO.File.Delete(ClientSettings.AppPath + "\\app.config");
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkAddressbook_Click(object sender, EventArgs e)
        {
            AddressBook.Clear();
            showAddressBook();
        }
    }
}
