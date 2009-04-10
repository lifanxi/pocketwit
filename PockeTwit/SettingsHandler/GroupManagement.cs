using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit.SettingsHandler
{
    public partial class GroupManagement : BaseSettingsForm
    {
        public GroupManagement()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            } 
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}