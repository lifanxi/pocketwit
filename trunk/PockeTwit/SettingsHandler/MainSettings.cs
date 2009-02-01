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
        public bool NeedsRerender { get; set; }
        private bool Initialized = false;
        public MainSettings()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
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

        private void lnkManageAccounts_Click(object sender, EventArgs e)
        {
            ShowAccounts();
        }
        private void lnkAvatar_Click(object sender, EventArgs e)
        {
            ShowAvatar();
        }
        private void lnkUI_Click(object sender, EventArgs e)
        {
            ShowUI();
        }
        private void lnkGPS_Click(object sender, EventArgs e)
        {
            ShowOther();
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
        private void ShowAvatar()
        {
            AvatarSettings avsettings = new AvatarSettings();
            DialogResult ret = avsettings.ShowDialog();
            avsettings.Close();
        }
        private void ShowUI()
        {
            UISettings UI = new UISettings();
            DialogResult ret = UI.ShowDialog();
            if (ret == DialogResult.OK)
            {
                if (UI.NeedsReset)
                {
                    NeedsReset = true;
                }
                if (UI.NeedsRerender)
                {
                    NeedsRerender = true;
                }
                PockeTwit.Themes.FormColors.SetColors(this);
            }
            UI.Close();
        }
        private void ShowOther()
        {
            OtherSettings O = new OtherSettings();
            DialogResult ret = O.ShowDialog();
            O.Close();
        }

        private void MainSettings_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void MainSettings_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void MainSettings_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            AdvancedForm a = new AdvancedForm();
            a.ShowDialog();
        }
    }
}