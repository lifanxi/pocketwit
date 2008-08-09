﻿using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class AboutForm : Form
    {
        private UpdateChecker Checker = new UpdateChecker(false);
        public AboutForm()
        {
            InitializeComponent();
            Checker.UpdateFound += new UpdateChecker.delUpdateFound(Checker_UpdateFound);
            Checker.CurrentVersion += new UpdateChecker.delUpdateFound(Checker_CurrentVersion);
            lblVersion.Text = Checker.currentVersion.ToString();
        }

        void Checker_CurrentVersion(UpdateChecker.UpdateInfo Info)
        {
            MessageBox.Show(Info.webVersion.ToString() + " is the latest version.", "No upgrades found.");
        }

        void Checker_UpdateFound(UpdateChecker.UpdateInfo Info)
        {
            UpdateForm uf = new UpdateForm();
            uf.NewVersion = Info;
            uf.ShowDialog();
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            
            Checker.CheckForUpdate();
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            Yedda.Twitter Twitter = new Yedda.Twitter();
            SetStatus s = new SetStatus();
            s.StatusText = "@PockeTwitDev ";
            s.ShowDialog();
            s.Hide();
            string UpdateText = s.StatusText;
            if (s.DialogResult == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                Twitter.Update(ClientSettings.UserName, ClientSettings.Password, UpdateText, Yedda.Twitter.OutputFormatType.XML);
                Cursor.Current = Cursors.Default;
            }
            this.Show();
            s.Close();
        }

        

        
    }
}