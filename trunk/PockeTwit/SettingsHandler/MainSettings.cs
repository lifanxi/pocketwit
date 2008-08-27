using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit.SettingsHandler
{
    public partial class MainSettings : Form
    {
        public bool NeedsReset { get; set; }
        private bool Initialized = false;
        public MainSettings()
        {
            InitializeComponent();
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void lnkManageAccounts_Click(object sender, EventArgs e)
        {
            ShowAccounts();
        }

        private DialogResult ShowAccounts()
        {
            AccountsForm af = new AccountsForm();
            DialogResult ret = af.ShowDialog();
            if (af.IsDirty)
            {
                NeedsReset = true;
            }
            af.Close();
            return ret;
        }

        private void MainSettings_Activated(object sender, EventArgs e)
        {
            if (!Initialized)
            {
                Initialized = true;
                if (ClientSettings.AccountsList.Count == 0)
                {
                    if (ShowAccounts() == DialogResult.Cancel)
                    {
                        this.DialogResult = DialogResult.Cancel;
                    }
                }
            }
        }

        private void menuDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void lnkAvatar_Click(object sender, EventArgs e)
        {
            AvatarSettings avsettings = new AvatarSettings();
            DialogResult ret = avsettings.ShowDialog();
            avsettings.Close();
        }

        private void lnkUI_Click(object sender, EventArgs e)
        {
            UISettings UI = new UISettings();
            DialogResult ret = UI.ShowDialog();
            if (ret == DialogResult.OK)
            {
                if (UI.NeedsReset)
                {
                    NeedsReset = true;
                }
            }
            UI.Close();
        }

        private void lnkGPS_Click(object sender, EventArgs e)
        {
            OtherSettings O = new OtherSettings();
            DialogResult ret = O.ShowDialog();
            O.Close();
        }
    }
}