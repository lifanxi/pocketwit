using System;

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

		#region Fields (1) 

        private UpdateChecker Checker = new UpdateChecker(false);

		#endregion Fields 

		#region Constructors (1) 

        public AboutForm()
        {
            InitializeComponent();
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            Checker.UpdateFound += new UpdateChecker.delUpdateFound(Checker_UpdateFound);
            Checker.CurrentVersion += new UpdateChecker.delUpdateFound(Checker_CurrentVersion);
            lblVersion.Text = UpdateChecker.currentVersion.ToString();
        }

		#endregion Constructors 

		#region Methods (5) 


		// Private Methods (5) 

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
                Twitter.AccountInfo = s.AccountToSet;
                Twitter.Update(UpdateText, Yedda.Twitter.OutputFormatType.XML);
                Cursor.Current = Cursors.Default;
            }
            this.Show();
            s.Close();
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            
            Checker.CheckForUpdate();
        }


		#endregion Methods 

    }
}