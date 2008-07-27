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
        private List<string> LeftMenu = new List<string>(new string[] { "Friends TimeLine", "Public TimeLine", "User TimeLine", "Set Status", "Settings", "Exit" });
        private List<string> RightMenu = new List<string>(new string[] { "Reply", "Direct Message", "Profile Page", "Exit" });
        private Yedda.Twitter.ActionType CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
        Yedda.Twitter Twitter;
        private string ShowUserID;

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
            statusList.WordClicked += new FingerUI.StatusItem.ClickedWordDelegate(statusList_WordClicked);

            GetTimeLine();
            

        }

        void statusList_WordClicked(string TextClicked)
        {
            if (TextClicked.StartsWith("http"))
            {
                ProfilePage p = new ProfilePage();
                p.URL = TextClicked;
                p.ShowDialog();
                this.Show();
            }
            else
            {
                ShowUserID = TextClicked.Replace("@","");
                CurrentAction = Yedda.Twitter.ActionType.Show;
                GetTimeLine();
            }
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
                case "Direct Message":
                    SendDirectMessage();
                    break;
                case "Profile Page":
                    ShowProfile();
                    break;

                case "Exit":
                    statusList.Clear();
                    this.Close();
                    break;
            }
        }

        private void ShowProfile()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            string User = selectedItem.User;
            ProfilePage p = new ProfilePage();
            p.User = User;
            p.ShowDialog();
            this.Show();
        }

        private void SendDirectMessage()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            string User = selectedItem.User;
            SetStatus("d " + User);
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
            SetStatus("@"+User);
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
                StatusForm.StatusText = ToUser + " ";
            }
            if (StatusForm.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                this.Show();
                StatusForm.Hide();
                string UpdateText = StatusForm.StatusText;
                Twitter.Update(ClientSettings.UserName, ClientSettings.Password, UpdateText, Yedda.Twitter.OutputFormatType.XML);
                
                this.GetTimeLine();
            }
            this.Show();
            StatusForm.Close();
            
        }

        private void GetTimeLine()
        {
            //Cursor.Current = Cursors.WaitCursor;
            tmrautoUpdate.Enabled = false;

            string response = FetchFromTwitter();

            if (response != CachedResponse)
            {
                Library.status[] statuses = InterpretStatuses(response);
                
                statusList.Clear();
                foreach (Library.status stat in statuses)
                {
                    FingerUI.StatusItem item = new FingerUI.StatusItem();
                    if (stat.user!=null)
                    {
                        item.Tweet = stat.text;
                        item.User = stat.user.screen_name;
                        item.UserImageURL = stat.user.profile_image_url;

                        statusList.AddItem(item);
                    }
                }
            }
            tmrautoUpdate.Enabled = true;
            //Cursor.Current = Cursors.Default;
        }

        private Library.status[] InterpretStatuses(string response)
        {
            CachedResponse = response;
            XmlSerializer s = new XmlSerializer(typeof(Library.status[]));
            Library.status[] statuses;
            using (System.IO.StringReader r = new System.IO.StringReader(response))
            {
                statuses = (Library.status[])s.Deserialize(r);
            }
            return statuses;
        }

        private string FetchFromTwitter()
        {
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
                case Yedda.Twitter.ActionType.Show:
                    response = Twitter.GetUserTimeline(ClientSettings.UserName, ClientSettings.Password, ShowUserID, Yedda.Twitter.OutputFormatType.XML);
                    break;
            }
            return response;
        }

        
        private void tmrautoUpdate_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Autoupdate");
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetTimeLine);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Start();
            //GetTimeLine();
        }
    }
}