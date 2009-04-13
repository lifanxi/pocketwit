using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit.SettingsHandler
{
    public partial class GroupManagement : BaseSettingsForm
    {
        public GroupManagement()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            SpecialTimeLine[] Times = SpecialTimeLines.GetList();
            if (Times.Length == 0)
            {
                cmbChooseGroup.Visible = false;
                pnlUsers.Visible = false;
                return;
            }
            label1.Visible = false;
            foreach (SpecialTimeLine t in Times)
            {
                cmbChooseGroup.Items.Add(t);
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cmbChooseGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlUsers.Controls.Clear();

            SpecialTimeLine t = (SpecialTimeLine)cmbChooseGroup.SelectedItem;
            int topOflabel = 0;
            foreach (SpecialTimeLine.groupTerm gt in t.Terms)
            {
                Label nameLabel = new Label();
                nameLabel.Text = gt.Term;
                nameLabel.Top = topOflabel;
                nameLabel.Height = ClientSettings.TextSize + 5;
                pnlUsers.Controls.Add(nameLabel);
                
                CheckBox chkExclusive = new CheckBox();
                chkExclusive.Left = nameLabel.Right + 5;
                chkExclusive.Top = nameLabel.Top;
                chkExclusive.Checked = gt.Exclusive;
                pnlUsers.Controls.Add(chkExclusive);

                topOflabel = nameLabel.Bottom + 5;
            }
        }

        private void lnkDeleteGroup_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will move all users from the " + cmbChooseGroup.SelectedItem + " group and delete the group.  The users will all appear in your friends timeline.\n\nProceed?", "Delete " + cmbChooseGroup.SelectedItem + " Group", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SpecialTimeLines.Remove((SpecialTimeLine)cmbChooseGroup.SelectedItem);
                cmbChooseGroup.Items.Remove(cmbChooseGroup.SelectedItem);
                this.NeedsReset = true;
            }
        }
    }
}