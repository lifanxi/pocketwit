using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PockeTwit
{
    public partial class TweetList : MasterForm
    {
        private List<string> LeftMenu = new List<string>(new string[] { "Public TimeLine", "Friends TimeLine", "User TimeLine", "Set Status", "Settings", "Exit" });
        private List<string> RightMenu = new List<string>(new string[] { "Reply", "Exit" });
        private Yedda.Twitter.ActionType CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
        Yedda.Twitter Twitter;

        private string CachedResponse;
        public TweetList()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Twitter = new Yedda.Twitter();
            tmrautoUpdate.Interval = 65000;
            tmrautoUpdate.Enabled = true;
            statusList.BackColor = Color.Black;
            statusList.ForeColor = Color.LightGray;
            statusList.SelectedBackColor = Color.DarkSlateGray;
            statusList.SelectedForeColor = Color.White;
            statusList.HighLightBackColor = Color.LightGray;
            statusList.HighLightForeColor = Color.Black;
            statusList.ItemHeight = 70;
            statusList.LeftMenuItems = LeftMenu;
            statusList.RightMenuItems = RightMenu;
            statusList.MenuItemSelected += new FingerUI.KListControl.delMenuItemSelected(statusList_MenuItemSelected);

            GetTimeLine();
            

        }

        void statusList_MenuItemSelected(string ItemName)
        {
            switch (ItemName)
            {
                case "Public TimeLine":
                    CurrentAction = Yedda.Twitter.ActionType.Public_Timeline;
                    GetTimeLine();
                    break;
                case "Friends TimeLine":
                    CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
                    GetTimeLine();
                    break;
                case "User TimeLine":
                    CurrentAction = Yedda.Twitter.ActionType.User_Timeline;
                    GetTimeLine();
                    break;
                case "Set Status":
                    SetStatus();
                    break;
                case "Settings":
                    ChangeSettings();
                    break;

                case "Reply":
                    SendReply();
                    break;


                case "Exit":
                    statusList.Clear();
                    this.Close();
                    break;
            }
        }

        private void ChangeSettings()
        {
            SettingsForm settings = new SettingsForm();
            if (settings.ShowDialog() == DialogResult.Cancel) { return; }
            GetTimeLine();
        }

        private void SendReply()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            string User = selectedItem.User;
            SetStatus(User);
        }

        private void SetStatus()
        {
            SetStatus("");
        }
        private void SetStatus(string ToUser)
        {
            SetStatus StatusForm = new SetStatus();
            if (!string.IsNullOrEmpty(ToUser))
            {
                StatusForm.Text = "@" + ToUser + " ";
            }
            if (StatusForm.ShowDialog() == DialogResult.OK)
            {
                string UpdateText = StatusForm.Text;
                Twitter.Update(ClientSettings.UserName, ClientSettings.Password, UpdateText, Yedda.Twitter.OutputFormatType.XML);
                this.GetTimeLine();
            }
            StatusForm.Close();
            this.Show();
        }

        private void GetTimeLine()
        {
            tmrautoUpdate.Enabled = false;
            statusList.Clear();
            string response = "";
            
            switch (CurrentAction)
            {
                case Yedda.Twitter.ActionType.Friends_Timeline:
                    response = Twitter.GetFriendsTimeline(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML);
                    break;
                case Yedda.Twitter.ActionType.Public_Timeline:
                    response = Twitter.GetPublicTimeline(Yedda.Twitter.OutputFormatType.XML);
                    break;
                case Yedda.Twitter.ActionType.User_Timeline:
                    response = Twitter.GetUserTimeline(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML);
                    break;
            }

            if (response != CachedResponse)
            {
                //response = GetSampleData();
                XmlSerializer s = new XmlSerializer(typeof(Library.status[]));
                Library.status[] statuses;
                using (System.IO.StringReader r = new System.IO.StringReader(response))
                {
                    statuses = (Library.status[])s.Deserialize(r);
                }
                foreach (Library.status stat in statuses)
                {
                    FingerUI.StatusItem item = new FingerUI.StatusItem();
                    item.Tweet = stat.text;
                    item.User = stat.user.screen_name;
                    item.UserImageURL = stat.user.profile_image_url;
                    statusList.AddItem(item);
                }
            }
            tmrautoUpdate.Enabled = true;
        }

        private static string GetSampleData()
        {
            string response;
            string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            using (System.IO.StreamReader r = new System.IO.StreamReader(AppPath + "\\samplestatuses.xml"))
            {
                response = r.ReadToEnd();
            }
            return response;
        }

        private void tmrautoUpdate_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("AutoUpdate!");
            GetTimeLine();
        }
    }
}