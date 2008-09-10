using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class AccountInfoForm : Form
    {
        private Yedda.Twitter.Account _AccountInfo = new Yedda.Twitter.Account();
        public Yedda.Twitter.Account AccountInfo 
        {
            get
            {
                return _AccountInfo;
            } 
            set
            {
                _AccountInfo = value;
                PopulateForm();
            }
        }

        public AccountInfoForm()
        {
            InitializeComponent();
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            _AccountInfo.Enabled = true;
            FillServerList();
        }

        public AccountInfoForm(Yedda.Twitter.Account Account)
        {
            _AccountInfo = Account;
            InitializeComponent();
            FillServerList();
            PopulateForm();
        }

        private void PopulateForm()
        {
            txtUserName.Text = _AccountInfo.UserName;
            txtPassword.Text = _AccountInfo.Password;
            cmbServers.SelectedItem = _AccountInfo.ServerURL.Name;
        }
        private void FillServerList()
        {
            foreach (string ServerName in Yedda.Servers.ServerList.Keys)
            {
                cmbServers.Items.Add(ServerName);
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            if (cmbServers.SelectedItem == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(txtUserName.Text)) { return; }
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            lblError.Visible = false;
            _AccountInfo.UserName = txtUserName.Text;
            _AccountInfo.Password = txtPassword.Text;
            _AccountInfo.ServerURL = Yedda.Servers.ServerList[(string)cmbServers.SelectedItem];
            //_AccountInfo.Enabled = (_AccountInfo.ServerURL.ServerType != Yedda.Twitter.TwitterServer.pingfm && _AccountInfo.ServerURL.ServerType != Yedda.Twitter.TwitterServer.brightkite);
            _AccountInfo.Enabled = true;
            Yedda.Twitter T = new Yedda.Twitter();
            T.AccountInfo = _AccountInfo;
            Cursor.Current = Cursors.Default;
            if (!T.Verify())
            {
                lblError.Text = "Invalid credentials.";
                lblError.Visible = true;
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void cmbServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            string server = (string)cmbServers.SelectedItem;
            if (server == "ping.fm")
            {
                txtPassword.Visible = false;
                lblPassword.Visible = false;
                txtPassword.Text = ClientSettings.PingApi;
                lblUser.Text = "Ping.FM Key";
                if (DetectDevice.DeviceType == DeviceType.Professional)
                {
                    txtUserName.ContextMenu = copyPasteMenu;
                }
            }
            else
            {
                txtPassword.Text = "";
                txtPassword.Visible = true;
                lblPassword.Visible = true;
                lblUser.Text = "User";
                if (DetectDevice.DeviceType == DeviceType.Professional)
                {
                    txtUserName.ContextMenu = null;
                }
            }
        }
        void PasteItem_Click(object sender, System.EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                txtUserName.Text = (string)iData.GetData(DataFormats.Text);
            }
        }
    }
}