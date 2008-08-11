using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class AccountsForm : Form
    {
        public bool IsDirty = false;
        public AccountsForm()
        {
            InitializeComponent();
            ListAccounts();
        }
        private void ListAccounts()
        {
            lstAccounts.Items.Clear();
            foreach (Yedda.Twitter.Account a in ClientSettings.AccountsList)
            {
                lstAccounts.Items.Add(a);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AccountInfoForm ai = new AccountInfoForm();
            ai.Hide();
            if (ai.ShowDialog() == DialogResult.OK)
            {
                lstAccounts.Items.Add(ai.AccountInfo);
                IsDirty = true;
            }
            ai.Close();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            Yedda.Twitter.Account a = (Yedda.Twitter.Account)lstAccounts.SelectedItem;
            if (a != null)
            {
                ClientSettings.AccountsList.Remove(a);
                lstAccounts.Items.Remove(a);
                IsDirty = true;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Yedda.Twitter.Account a = (Yedda.Twitter.Account)lstAccounts.SelectedItem;
            AccountInfoForm ai = new AccountInfoForm(a);
            if (ai.ShowDialog() == DialogResult.OK)
            {
                lstAccounts.Items.Remove(a);
                lstAccounts.Items.Add(ai.AccountInfo);
            }
            ai.Close();
        }

        private void menuAccept_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (IsDirty)
            {
                ClientSettings.AccountsList.Clear();
                foreach (Yedda.Twitter.Account a in lstAccounts.Items)
                {
                    ClientSettings.AccountsList.Add(a);
                }
            }
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void lstAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            Yedda.Twitter.Account a = (Yedda.Twitter.Account)lstAccounts.SelectedItem;
            if (a == null) { return; }
            a.Enabled = !a.Enabled;
            ListAccounts();
            IsDirty = true;
        }
    }
}