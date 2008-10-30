using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class UISettings : Form
    {

        #region Constructors (1) 

        public UISettings()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            PopulateForm();
            
        }

		#endregion Constructors 

		#region Properties (1) 

        public bool NeedsReset { get; set; }

		#endregion Properties 

		#region Methods (4) 


		// Private Methods (4) 

        

        private void menuAccept_Click(object sender, EventArgs e)
        {
            lblTweets.ForeColor = ClientSettings.ForeColor;
            int MaxTweets = 0;
            try
            {
                MaxTweets = int.Parse(txtMaxTweets.Text);
                if (MaxTweets > 200 || MaxTweets < 10)
                {
                    lblTweets.ForeColor = ClientSettings.ErrorColor;
                    Cursor.Current = Cursors.Default;
                    return;
                }
            }
            catch
            {
                lblTweets.ForeColor = ClientSettings.ErrorColor;
                Cursor.Current = Cursors.Default;
                return;
            }
            if (MaxTweets != ClientSettings.MaxTweets) { NeedsReset = true; }
            if (chkTimestamps.Checked != ClientSettings.ShowExtra) { NeedsReset = true; }
            if (chkScreenName.Checked != ClientSettings.IncludeUserName) { NeedsReset = true; }
            ClientSettings.MaxTweets = MaxTweets;
            ClientSettings.UseClickables = chkClickables.Checked;
            ClientSettings.ShowExtra = chkTimestamps.Checked;
            ClientSettings.IncludeUserName = chkScreenName.Checked;
            ClientSettings.UseSkweezer = chkSkweezer.Checked;

            string selectedTheme = (string)cmbTheme.SelectedItem;
            if ((selectedTheme!="Custom...") && (selectedTheme != ClientSettings.ThemeName))
            {
                ClientSettings.ThemeName = selectedTheme;
                ClientSettings.LoadColors();
                PockeTwit.Themes.FormColors.SetColors(this);
            }

            ClientSettings.SaveSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PopulateForm()
        {
            txtMaxTweets.Text = ClientSettings.MaxTweets.ToString();
            chkTimestamps.Checked = ClientSettings.ShowExtra;
            chkClickables.Checked = ClientSettings.UseClickables;
            chkScreenName.Checked = ClientSettings.IncludeUserName;
            chkSkweezer.Checked = ClientSettings.UseSkweezer;
            ListThemes();
            this.DialogResult = DialogResult.Cancel;
        }
        private void ListThemes()
        {
            foreach (string ThemeFile in System.IO.Directory.GetDirectories(ClientSettings.AppPath + "\\Themes\\"))
            {
                string themeName = System.IO.Path.GetFileNameWithoutExtension(ThemeFile);
                cmbTheme.Items.Add(themeName);
            }
            cmbTheme.Items.Add("Custom...");
            cmbTheme.SelectedItem = ClientSettings.ThemeName;
        }

		#endregion Methods 

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTheme = (string)cmbTheme.SelectedItem;
            if (selectedTheme == "Custom...")
            {
                MessageBox.Show("soon...");                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selectedTheme = (string)cmbTheme.SelectedItem;
            ColorPick c = new ColorPick(selectedTheme);
            c.ShowDialog();
        }

        
    }
}