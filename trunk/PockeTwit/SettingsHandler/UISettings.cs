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
        private string OriginalTheme = ClientSettings.ThemeName;

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
            if (selectedTheme != OriginalTheme)
            {
                ClientSettings.ThemeName = selectedTheme;
            }
            ClientSettings.LoadColors();

            ClientSettings.SaveSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            if (ClientSettings.ThemeName != OriginalTheme)
            {
                ClientSettings.ThemeName = OriginalTheme;
            }
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
            cmbTheme.Items.Clear();
            foreach (string ThemeFile in System.IO.Directory.GetDirectories(ClientSettings.AppPath + "\\Themes\\"))
            {
                string themeName = System.IO.Path.GetFileNameWithoutExtension(ThemeFile);
                cmbTheme.Items.Add(themeName);
            }
            cmbTheme.SelectedItem = ClientSettings.ThemeName;
        }

		#endregion Methods 

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTheme = (string)cmbTheme.SelectedItem;
            ClientSettings.ThemeName = selectedTheme;
            ClientSettings.LoadColors();
            Themes.FormColors.SetColors(this);
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            string selectedTheme = (string)cmbTheme.SelectedItem;
            ColorPick c = new ColorPick(selectedTheme);
            c.ShowDialog();
            ListThemes();
            cmbTheme_SelectedIndexChanged(null, new EventArgs());
        }

        
    }
}