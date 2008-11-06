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
        delegate void delNothing();
		#region Fields (1) 

        private UpdateChecker Checker = new UpdateChecker(false);
        private Contributors ContributorChecker;
        public string AskedToSeeUser = null;
		#endregion Fields 

		#region Constructors (1) 

        public AboutForm()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            lblWait.ForeColor = ClientSettings.FieldForeColor;
            ContributorChecker = new Contributors();
            ContributorChecker.ContributorsReady += new Contributors.delContributorsReady(ContributorChecker_ContributorsReady);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            Checker.UpdateFound += new UpdateChecker.delUpdateFound(Checker_UpdateFound);
            Checker.CurrentVersion += new UpdateChecker.delUpdateFound(Checker_CurrentVersion);
            lblVersion.Text = UpdateChecker.currentVersion.ToString();
            if (UpdateChecker.devBuild)
            {
                lblVersion.Text = UpdateChecker.currentVersion.ToString() + " dev";
            }
            this.lnkContributors.Focus();
        }

        void ContributorChecker_ContributorsReady()
        {
            if (InvokeRequired)
            {
                delNothing d = new delNothing(ContributorChecker_ContributorsReady);
                this.Invoke(d);
            }
            else
            {
                panel1.Controls.Remove(lblWait);
                int topOfLabel = 0;
                int labelWidth = panel1.Width / 2;
                foreach (Contributors.Contributor s in ContributorChecker.ContributorsList)
                {

                    LinkLabel nameLabel = new LinkLabel();
                    nameLabel.Text = s.Name;
                    if (s.Name.StartsWith("@"))
                    {
                        nameLabel.ForeColor = ClientSettings.LinkColor;
                        nameLabel.Click += new EventHandler(nameLabel_Click);
                    }
                    else
                    {
                        nameLabel.ForeColor = ClientSettings.FieldForeColor;
                    }
                    nameLabel.Top = topOfLabel;
                    nameLabel.Width = labelWidth;
                    nameLabel.Height = ClientSettings.TextSize+5;
                    panel1.Controls.Add(nameLabel);
                    
                    Label typeLabel = new Label();
                    typeLabel.Text = s.Contribution;
                    typeLabel.Top = topOfLabel;
                    typeLabel.Left = nameLabel.Right;
                    typeLabel.Width = labelWidth;
                    typeLabel.Height = ClientSettings.TextSize + 5;
                    typeLabel.ForeColor = ClientSettings.FieldForeColor;
                    panel1.Controls.Add(typeLabel);

                    

                    topOfLabel = nameLabel.Bottom + 5;
                }
                LinkLabel YouToo = new LinkLabel();
                YouToo.Text = "Your name can be here!";
                YouToo.ForeColor = ClientSettings.LinkColor;
                YouToo.Width = panel1.Width - 2;
                YouToo.Height = ClientSettings.TextSize+5;
                YouToo.Click += new EventHandler(YouToo_Click);
                YouToo.Top = topOfLabel;
                panel1.Controls.Add(YouToo);
            }
        }

        void YouToo_Click(object sender, EventArgs e)
        {
            LaunchSite("http://code.google.com/p/pocketwit/wiki/Contribute");
        }

        void nameLabel_Click(object sender, EventArgs e)
        {
            LinkLabel l = (LinkLabel)sender;
            if (l.Text.StartsWith("@"))
            {
                AskedToSeeUser = l.Text;
                this.Close();
            }
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

        private void lnkContributors_Click(object sender, EventArgs e)
        {
            LaunchSite("http://code.google.com/p/pocketwit/wiki/Contribute");
        }

        private void linkLabel2_Click(object sender, EventArgs e)
        {
            LaunchSite("http://code.google.com/p/pocketwit/");
        }

        private void LaunchSite(string URL)
        {
            System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
            if (ClientSettings.UseSkweezer)
            {
                URL = Yedda.Skweezer.GetSkweezerURL(URL);
            }
            pi.FileName = URL;
            pi.UseShellExecute = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
        }

    }
}