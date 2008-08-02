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
    public partial class TweetList : Form
    {
        [System.Runtime.InteropServices.DllImport("coredll.dll", EntryPoint = "MessageBeep", SetLastError = true)]
        private static extern void MessageBeep(int type);

        private List<string> LeftMenu = new List<string>(new string[] { "Friends TimeLine", "Replies", "Public TimeLine", "Set Status", "Settings", "About/Feedback"});
        private List<string> RightMenu = new List<string>(new string[] { "Reply", "Direct Message", "Make Favorite", "Profile Page", "Stop Following", "Exit" });
        private Yedda.Twitter.ActionType CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
        Yedda.Twitter Twitter;
        private string ShowUserID;
        private UpdateChecker Checker;
        private string CachedResponse;
        private string LastStatusID = "";
        private Library.status[] CurrentStatuses =null;

        
        private bool CurrentlyConnected
        {
            set
            {
                if (value)
                {
                    statusList.Warning = "";
                    SetConnectedMenus();
                }
                else
                {
                    statusList.Warning = "Disconnected";
                    SetDisconnectedMenus();
                }
            }
        }

        private void SetConnectedMenus()
        {
            LeftMenu = new List<string>(new string[] { "Friends TimeLine", "Replies", "Public TimeLine", "Set Status", "Settings", "About/Feedback"});
            RightMenu = new List<string>(new string[] { "Reply", "Direct Message", "Make Favorite", "Profile Page", "Stop Following", "Exit" });

            if (!Twitter.FavoritesWork) { RightMenu.Remove("Make Favorite"); }
            if (!Twitter.DirectMessagesWork) { RightMenu.Remove("Direct Message"); }
            statusList.LeftMenuItems = LeftMenu;
            statusList.RightMenuItems = RightMenu;
        }
        private void SetDisconnectedMenus()
        {
            statusList.LeftMenuItems = new List<string>(new string[] { "Reconnect", "Settings", "About/Feedback"});
            statusList.RightMenuItems = new List<string>(new string[] { "Exit" });
        }
        
        private delegate void delChangeCursor(Cursor CursorToset);
        public TweetList()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(ClientSettings.UserName) | string.IsNullOrEmpty(ClientSettings.Password))
            {
                // SHow Settings page first
                SettingsForm settings = new SettingsForm();
                if (settings.ShowDialog() == DialogResult.Cancel) { return; }
            }
            
            this.WindowState = FormWindowState.Maximized;
            Twitter = new Yedda.Twitter();
            Twitter.CurrentServer = ClientSettings.Server;
            tmrautoUpdate.Interval = ClientSettings.UpdateInterval;
            tmrautoUpdate.Enabled = true;
            statusList.BackColor = ClientSettings.BackColor;
            statusList.ForeColor = ClientSettings.ForeColor;
            statusList.SelectedBackColor = ClientSettings.SelectedBackColor;
            statusList.SelectedForeColor = ClientSettings.SelectedForeColor;
            statusList.ItemHeight = 70;
            statusList.IsMaximized = true;
            SetConnectedMenus();
            statusList.MenuItemSelected += new FingerUI.KListControl.delMenuItemSelected(statusList_MenuItemSelected);
            statusList.WordClicked += new FingerUI.StatusItem.ClickedWordDelegate(statusList_WordClicked);
            statusList.SelectedItemChanged += new EventHandler(statusList_SelectedItemChanged);
            statusList.SwitchWindowState += new FingerUI.KListControl.delSwitchState(statusList_SwitchWindowState);
            if (Twitter.BigTimeLines)
            {
                LoadCachedtimeline();
            }
            GetTimeLine();
            Checker = new UpdateChecker();
            Checker.UpdateFound += new UpdateChecker.delUpdateFound(UpdateChecker_UpdateFound);
        }

        private void LoadCachedtimeline()
        {
            string cachePath = ClientSettings.AppPath + "\\" + ClientSettings.UserName + "FriendsTime.xml";
            if (System.IO.File.Exists(cachePath))
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(cachePath))
                {
                    string s = r.ReadToEnd();
                    CurrentStatuses = Library.status.Deserialize(s);
                }
                LastStatusID = CurrentStatuses[0].id;
            }
            
        }

        void statusList_SwitchWindowState(bool IsMaximized)
        {
            if (IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        
        void statusList_SelectedItemChanged(object sender, EventArgs e)
        {
            UpdateRightMenu();
        }

        private void UpdateRightMenu()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            if (selectedItem != null)
            {
                if (Twitter.FavoritesWork)
                {
                    if (selectedItem.isFavorite)
                    {
                        statusList.RightMenuItems = SideMenuFunctions.ReplaceMenuItem(statusList.RightMenuItems, "Make Favorite", "Destroy Favorite");
                    }
                    else
                    {
                        statusList.RightMenuItems = SideMenuFunctions.ReplaceMenuItem(statusList.RightMenuItems, "Destroy Favorite", "Make Favorite");
                    }
                }
                if (selectedItem.isBeingFollowed)
                {
                    statusList.RightMenuItems = SideMenuFunctions.ReplaceMenuItem(statusList.RightMenuItems, "Follow", "Stop Following");
                }
                else
                {
                    statusList.RightMenuItems = SideMenuFunctions.ReplaceMenuItem(statusList.RightMenuItems, "Stop Following", "Follow");
                }
            }
        }

        void UpdateChecker_UpdateFound(UpdateChecker.UpdateInfo Info)
        {
            UpdateForm uf = new UpdateForm();
            uf.NewVersion = Info;
            uf.ShowDialog();
        }

        void statusList_WordClicked(string TextClicked)
        {
            if (TextClicked.StartsWith("http"))
            {
                System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
                pi.FileName = "\\Windows\\iexplore.exe";
                pi.Arguments = TextClicked;
                pi.UseShellExecute = true;
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
            }
            else
            {
                ChangeCursor(Cursors.WaitCursor);
                ShowUserID = TextClicked.Replace("@","");
                CurrentAction = Yedda.Twitter.ActionType.Show;
                GetTimeLineAsync();
            }
        }

        void statusList_MenuItemSelected(string ItemName)
        {
            switch (ItemName)
            {
                case "Public TimeLine":
                    CurrentAction = Yedda.Twitter.ActionType.Public_Timeline;
                    statusList.RightMenuItems = RightMenu;
                    ChangeCursor(Cursors.WaitCursor);
                    GetTimeLineAsync();
                    break;
                case "Reconnect":
                case "Friends TimeLine":
                    CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
                    statusList.RightMenuItems = RightMenu;
                    ChangeCursor(Cursors.WaitCursor);
                    GetTimeLineAsync();
                    break;
                case "Replies":
                    CurrentAction = Yedda.Twitter.ActionType.Replies;
                    statusList.RightMenuItems = RightMenu;
                    ChangeCursor(Cursors.WaitCursor);
                    GetTimeLineAsync();
                    break;
                case "Favorites":
                    CurrentAction = Yedda.Twitter.ActionType.Favorites;
                    GetTimeLineAsync();
                    break;
                case "Set Status":
                    SetStatus();
                    break;
                case "Settings":
                    ChangeSettings();
                    break;
                case "About/Feedback":
                    ShowAbout();
                    break;

                case "Reply":
                    SendReply();
                    break;
                case "Destroy Favorite":
                    DestroyFavorite();
                    break;
                case "Make Favorite":
                    CreateFavoriteAsync();
                    break;
                case "Direct Message":
                    SendDirectMessage();
                    break;
                case "Profile Page":
                    ShowProfile();
                    break;
                case "Follow":
                    FollowUser();
                    break;
                case "Stop Following":
                    StopFollowingUser();
                    break;

                case "Minimize":
                    this.Hide();
                    break;
                case "Exit":
                    statusList.Clear();
                    this.Close();
                    break;
            }
        }

        private void ShowAbout()
        {
            AboutForm a = new AboutForm();
            a.ShowDialog();
        }

        

        private void ShowProfile()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
            pi.FileName = "\\Windows\\iexplore.exe";
            pi.Arguments = Twitter.GetProfileURL(selectedItem.Tweet.user.screen_name);
            pi.UseShellExecute = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
        }

        private void SendDirectMessage()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            string User = selectedItem.Tweet.user.screen_name;
            SetStatus("d " + User);
        }

        private void ChangeSettings()
        {
            SettingsForm settings = new SettingsForm();
            if (settings.ShowDialog() == DialogResult.Cancel) { return; }
            Twitter.CurrentServer = ClientSettings.Server;
            if (ClientSettings.CheckVersion)
            {
                Checker.CheckForUpdate();
            }
            GetTimeLine();
        }

        private void SendReply()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            string User = selectedItem.Tweet.user.screen_name;
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
                this.Show();
                StatusForm.Hide();
                string UpdateText = StatusForm.StatusText;
                Twitter.Update(ClientSettings.UserName, ClientSettings.Password, UpdateText, Yedda.Twitter.OutputFormatType.XML);

                GetTimeLineAsync();
            }
            this.Show();
            StatusForm.Close();
            
        }

        private void GetTimeLine()
        {
            string response = FetchFromTwitter();

            if (!string.IsNullOrEmpty(response) && response != CachedResponse)
            {
                CachedResponse = response;
                Library.status[] newstatuses = Library.status.Deserialize(response);
                Library.status[] mergedstatuses = null;
                if (!Twitter.BigTimeLines)
                {
                    mergedstatuses = newstatuses;
                }
                else
                {
                    if (CurrentAction == Yedda.Twitter.ActionType.Friends_Timeline)
                    {
                        mergedstatuses = MergeIn(newstatuses, CurrentStatuses);
                        SaveStatuses(mergedstatuses);
                    }
                    else
                    {
                        mergedstatuses = newstatuses;
                    }
                }

                statusList.Clear();
                foreach (Library.status stat in mergedstatuses)
                {
                    FingerUI.StatusItem item = new FingerUI.StatusItem();
                    if (stat.user!=null)
                    {
                        item.Tweet = stat;
                        statusList.AddItem(item);
                    }
                }
                if (ClientSettings.BeepOnNew && 
                    (CurrentAction == Yedda.Twitter.ActionType.Friends | 
                    CurrentAction == Yedda.Twitter.ActionType.Replies)) { MessageBeep(0); }
            }
            ChangeCursor(Cursors.Default);
        }

        private void SaveStatuses(PockeTwit.Library.status[] mergedstatuses)
        {
            string StatusString = Library.status.Serialize(mergedstatuses);
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(ClientSettings.AppPath + "\\" + ClientSettings.UserName + "FriendsTime.xml"))
            {
                w.Write(StatusString);
            }
            CurrentStatuses = mergedstatuses;
        }

        private PockeTwit.Library.status[] MergeIn(PockeTwit.Library.status[] newstatuses, PockeTwit.Library.status[] CurrentStatuses)
        {
            if (CurrentStatuses == null)
            {
                return newstatuses;
            }
            int newLength = (newstatuses.Length + CurrentStatuses.Length > ClientSettings.MaxTweets) ? ClientSettings.MaxTweets : newstatuses.Length + CurrentStatuses.Length;
            Library.status[] MergedList = new PockeTwit.Library.status[newLength];
            int i = 0;
            foreach (Library.status stat in newstatuses)
            {
                if (stat != null)
                {
                    if (i >= ClientSettings.MaxTweets) { break; }
                    MergedList[i] = stat;
                    i++;
                }
            }
            foreach (Library.status stat in CurrentStatuses)
            {
                if (stat != null)
                {
                    if (i >= ClientSettings.MaxTweets) { break; }
                    MergedList[i] = stat;
                    i++;
                }
            }
            LastStatusID = MergedList[0].id;
            return MergedList;
        }

        

        private string FetchFromTwitter()
        {
            string response = "";
            try
            {
                CurrentlyConnected = true;
                switch (CurrentAction)
                {
                    case Yedda.Twitter.ActionType.Friends_Timeline:
                        if (!Twitter.BigTimeLines)
                        {
                            response = Twitter.GetFriendsTimeline(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(LastStatusID))
                            {
                                response = Twitter.GetFriendsTimeLineMax(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML);
                            }
                            else
                            {
                                response = Twitter.GetFriendsTimeLineSince(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML, LastStatusID);
                            }
                        }
                        break;
                    case Yedda.Twitter.ActionType.Public_Timeline:
                        response = Twitter.GetPublicTimeline(Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Replies:
                        response = Twitter.GetRepliesTimeLine(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.User_Timeline:
                        response = Twitter.GetUserTimeline(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Show:
                        response = Twitter.GetUserTimeline(ClientSettings.UserName, ClientSettings.Password, ShowUserID, Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Favorites:
                        response = Twitter.GetFavorites(ClientSettings.UserName, ClientSettings.Password);
                        break;
                }
            }
            catch
            {
                CurrentlyConnected = false;
            }
            return response;
        }

        private void FollowUser()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            string UserID = selectedItem.Tweet.user.id;
            Twitter.FollowUser(ClientSettings.UserName, ClientSettings.Password, UserID);

        }

        private void StopFollowingUser()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            string UserID = selectedItem.Tweet.user.id;
            Twitter.StopFollowingUser(ClientSettings.UserName, ClientSettings.Password, UserID);

        }

        private void DestroyFavorite()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            string ID = selectedItem.Tweet.id;
            Twitter.DestroyFavorite(ClientSettings.UserName, ClientSettings.Password, ID);
            selectedItem.isFavorite = false;
            UpdateRightMenu();
        }

        private void CreateFavoriteAsync()
        {
            ChangeCursor(Cursors.WaitCursor);
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statusList.SelectedItem;
            selectedItem.isFavorite = true;

            string ID = selectedItem.Tweet.id;
            System.Threading.ThreadStart ts = delegate { CreateFavorite(ID); };
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "CreateFavorite";
            t.Start();
        }

        private void CreateFavorite(string ID)
        {
            Twitter.SetFavorite(ClientSettings.UserName, ClientSettings.Password, ID);
            
            UpdateRightMenu();
            ChangeCursor(Cursors.Default);
        }

        private void tmrautoUpdate_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("TICK");
            GetTimeLineAsync();
        }

        private void GetTimeLineAsync()
        {
            System.Diagnostics.Debug.WriteLine("Autoupdate");
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetTimeLine);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "GetTimeLine";
            t.Start();
        }

        private void ChangeCursor(Cursor CursorToset)
        {
            if (InvokeRequired)
            {
                delChangeCursor d = new delChangeCursor(ChangeCursor);
                this.Invoke(d, new object[] { CursorToset });
            }
            else
            {
                Cursor.Current = CursorToset;
            }
        }
    }
}