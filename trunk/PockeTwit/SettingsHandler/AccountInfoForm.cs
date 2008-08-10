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

            switch (_AccountInfo.Server)
            {
                case Yedda.Twitter.TwitterServer.twitter:
                    cmbServers.SelectedItem = "twitter";
                    break;
                case Yedda.Twitter.TwitterServer.identica:
                    cmbServers.SelectedItem = "identica";
                    break;
            }
        }
        private void FillServerList()
        {
            cmbServers.Items.Add("twitter");
            cmbServers.Items.Add("identica");
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            _AccountInfo.UserName = txtUserName.Text;
            _AccountInfo.Password = txtPassword.Text;
            _AccountInfo.Server = (Yedda.Twitter.TwitterServer)Enum.Parse(typeof(Yedda.Twitter.TwitterServer), (string)cmbServers.SelectedItem, true);
            this.DialogResult = DialogResult.OK;
        }
    }
}