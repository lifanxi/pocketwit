using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PockeTwit
{
    public partial class userListControl : UserControl
    {

        [DllImport("aygshell.dll", EntryPoint = "SHSetInputContext")]
        private static extern int SHSetInputContext(IntPtr hwnd, SHIC_FEATURE dwFeature, bool lpValue);

        private enum SHIC_FEATURE : uint
        {
            RESTOREDEFAULT = 0,
            AUTOCORRECT = 1,
            AUTOSUGGEST = 2,
            HAVETRAILER = 3,
            CLASS = 4
        };

        public static void SetAutoSuggest(Control window, bool enabled)
        {
            
            SHSetInputContext(window.Handle, SHIC_FEATURE.AUTOSUGGEST, enabled);
        }


        private List<LinkLabel> VisibleItems = new List<LinkLabel>();
        public delegate void delItemChose(string itemText);
        public event delItemChose ItemChosen = delegate { };
        private DeviceType dt = DetectDevice.DeviceType;

        public userListControl()
        {
            InitializeComponent();
            SetAutoSuggest(txtInput, false);
            PockeTwit.Themes.FormColors.SetColors(this);
        }

        public string inputText
        {
            set
            {
                txtInput.Text = value;
                txtInput.SelectionStart = txtInput.Text.Length;
            }
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            int top = txtInput.Bottom;
            
            foreach (LinkLabel existing in VisibleItems)
            {
                existing.Click -= new EventHandler(lblItem_Click);
                this.Controls.Remove(existing);
            }

            VisibleItems.Clear();
            if (string.IsNullOrEmpty(txtInput.Text)) 
            { 
                this.Height = top * 2;
                return; 
            }
            string[] Names = AddressBook.GetList(txtInput.Text);
            if (Names.Length > 0)
            {
                foreach (string Name in Names)
                {
                    LinkLabel lblItem = new LinkLabel();
                    lblItem.Text = Name;
                    lblItem.Top = top;
                    lblItem.Height = ClientSettings.TextSize + 10;
                    lblItem.Width = this.Width;
                    lblItem.Click += new EventHandler(lblItem_Click);
                    lblItem.Font = ClientSettings.TextFont;
                    lblItem.ForeColor = ClientSettings.ForeColor;

                    this.Controls.Add(lblItem);
                    top = top + ClientSettings.TextSize + 10;
                    VisibleItems.Add(lblItem);
                    
                }
                this.Height = top;
            }
            txtInput.Focus();
        }
        

        void lblItem_Click(object sender, EventArgs e)
        {
            LinkLabel lbl = (LinkLabel)sender;
            ItemChosen(lbl.Text);
            this.Visible = false;
        }

        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
            {
                if (string.IsNullOrEmpty(txtInput.Text))
                {
                    this.Visible = false;
                    ItemChosen("");
                    e.Handled = true;
                }
            }
            if (e.KeyChar == '\r' || e.KeyChar == ' ')
            {
                this.Visible = false;
                ItemChosen(txtInput.Text);
                e.Handled = true;
            }
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                if (string.IsNullOrEmpty(txtInput.Text))
                {
                    this.Visible = false;
                    ItemChosen("");
                    e.Handled = true;
                }
            }
            if (e.KeyCode == (Keys.Down))
            {
                if (VisibleItems.Count > 0)
                {
                    VisibleItems[0].Focus();
                    e.Handled = true;
                }
            }
            if ((int)e.KeyCode == 229)
            {
                if (VisibleItems.Count > 0)
                {
                    VisibleItems[0].Focus();
                    e.Handled = true;
                }
            }
        }
    }
}
