﻿using System;

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

        private List<string> LeftMenu;
        private List<string> RightMenu; 
        private Yedda.Twitter.ActionType CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
        Yedda.Twitter Twitter;
        private string ShowUserID;
        private UpdateChecker Checker;
        private string CachedResponse;
        private string LastStatusID = "";
        private Library.status[] CurrentStatuses =null;
        private FingerUI.KListControl statList;
        private bool InitialLoad = true;

        private DeviceType DeviceType;

        private void SwitchToList(FingerUI.KListControl list)
        {
            if (statList != null)
            {
                list.XOffset = statList.XOffset;
                statList.UnHookKey();
            }
            friendsStatslist.Visible = false;
            otherStatslist.Visible = false;
            statList = list;
            statList.HookKey();
            statList.Visible = true;
        }

        private bool CurrentlyConnected
        {
            set
            {
                if (value)
                {
                    statList.Warning = "";
                    SetConnectedMenus(statList);
                }
                else
                {
                    SetDisconnectedMenus(statList);
                }
            }
        }

        private void SetConnectedMenus(FingerUI.KListControl list)
        {
            LeftMenu = new List<string>(new string[] { "Friends TimeLine", "Replies", "Search", "Set Status", "Settings", "About/Feedback", "Exit" });
            RightMenu = new List<string>(new string[] { "@User TimeLine", "Reply @User", "Direct @User", "Make Favorite", "Profile Page", "Stop Following", "Exit" });

            if (!Twitter.FavoritesWork) { RightMenu.Remove("Make Favorite"); }
            if (!Twitter.DirectMessagesWork) { RightMenu.Remove("Direct @User"); }
            list.LeftMenuItems = LeftMenu;
            list.RightMenuItems = RightMenu;
        }
        private void SetDisconnectedMenus(FingerUI.KListControl list)
        {
            list.LeftMenuItems = new List<string>(new string[] { "Reconnect", "Settings", "About/Feedback", "Exit" });
            list.RightMenuItems = new List<string>(new string[] { "Exit" });
        }
        
        private delegate void delChangeCursor(Cursor CursorToset);
        private delegate void delNotify(int Count);
        public TweetList()
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();
            Application.DoEvents();
        }

        private void timerStartup_Tick(object sender, EventArgs e)
        {
            timerStartup.Enabled = false;
            timerStartup.Tick -= new EventHandler(timerStartup_Tick);
            SetEverythingUp();
            SwitchToDone();
            Following.LoadFromTwitter();
        }

        private void SwitchToDone()
        {
            lblLoading.Visible = false;
            lblTitle.Visible = false;
            InitialLoad = false;
            SwitchToList(friendsStatslist);
        }


        private void SetEverythingUp()
        {
            DeviceType = DetectDevice.DeviceType;
            if (DeviceType == DeviceType.Professional)
            {
                this.notification1 = new Microsoft.WindowsCE.Forms.Notification();
                this.notification1.Caption = "New tweets!";
                this.notification1.Text = "notification1";
                this.notification1.ResponseSubmitted += new Microsoft.WindowsCE.Forms.ResponseSubmittedEventHandler(this.notification1_ResponseSubmitted);
            }
            if (string.IsNullOrEmpty(ClientSettings.UserName) | string.IsNullOrEmpty(ClientSettings.Password))
            {
                // SHow Settings page first
                SettingsForm settings = new SettingsForm();
                if (settings.ShowDialog() == DialogResult.Cancel) 
                {
                    Application.Exit();
                    return;
                }
            }

            
            if (ClientSettings.CheckVersion)
            {
                lblLoading.Text = "Launching update checker.";
                Application.DoEvents();
                Checker = new UpdateChecker();
                Checker.UpdateFound += new UpdateChecker.delUpdateFound(UpdateChecker_UpdateFound);
            }

            Twitter = new Yedda.Twitter();
            Twitter.MaxTweets = ClientSettings.MaxTweets;
            Twitter.CurrentServer = ClientSettings.Server;
            
            lblLoading.Text = "Setting up UI lists.";
            Application.DoEvents();
            SetUpListControl(friendsStatslist);
            SetUpListControl(otherStatslist);
            statList = friendsStatslist;

            if (Twitter.BigTimeLines)
            {
                lblLoading.Text = "Loading cached timeline.";
                Application.DoEvents();
                LoadCachedtimeline();
            }
            lblLoading.Text = "Fetching timeline from server.";
            Application.DoEvents();
            GetTimeLine();
            statList.SetSelectedIndexToZero();
            tmrautoUpdate.Interval = ClientSettings.UpdateInterval;
            tmrautoUpdate.Enabled = true;

        }

        private void SetUpListControl(FingerUI.KListControl list)
        {
            list.BackColor = ClientSettings.BackColor;
            list.ForeColor = ClientSettings.ForeColor;
            list.SelectedBackColor = ClientSettings.SelectedBackColor;
            list.SelectedForeColor = ClientSettings.SelectedForeColor;
            list.ItemHeight = (ClientSettings.TextSize * 5)+5;
            list.IsMaximized = true;
            SetConnectedMenus(list);
            list.MenuItemSelected += new FingerUI.KListControl.delMenuItemSelected(statusList_MenuItemSelected);
            list.WordClicked += new FingerUI.StatusItem.ClickedWordDelegate(statusList_WordClicked);
            list.SelectedItemChanged += new EventHandler(statusList_SelectedItemChanged);
            list.SwitchWindowState += new FingerUI.KListControl.delSwitchState(statusList_SwitchWindowState);
            
        }

        private void LoadCachedtimeline()
        {
            string cachePath = ClientSettings.AppPath + "\\" + ClientSettings.UserName + ClientSettings.Server.ToString()+"FriendsTime.xml";
            if (System.IO.File.Exists(cachePath))
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(cachePath))
                {
                    string s = r.ReadToEnd();
                    CurrentStatuses = Library.status.Deserialize(s);
                }
                LastStatusID = CurrentStatuses[0].id;
            }
            AddStatusesToList(CurrentStatuses,0);
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
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (selectedItem != null)
            {
                if (Twitter.FavoritesWork)
                {
                    if (selectedItem.isFavorite)
                    {
                        statList.RightMenuItems = SideMenuFunctions.ReplaceMenuItem(statList.RightMenuItems, "Make Favorite", "Destroy Favorite");
                    }
                    else
                    {
                        statList.RightMenuItems = SideMenuFunctions.ReplaceMenuItem(statList.RightMenuItems, "Destroy Favorite", "Make Favorite");
                    }
                }
                if (selectedItem.isBeingFollowed)
                {
                    statList.RightMenuItems = SideMenuFunctions.ReplaceMenuItem(statList.RightMenuItems, "Follow", "Stop Following");
                }
                else
                {
                    statList.RightMenuItems = SideMenuFunctions.ReplaceMenuItem(statList.RightMenuItems, "Stop Following", "Follow");
                }
            }
            statList.Redraw();
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
                //pi.FileName = "\\Windows\\iexplore.exe";
                pi.FileName = TextClicked;
                //pi.Arguments = TextClicked;
                pi.UseShellExecute = true;
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
            }
            else
            {
                ChangeCursor(Cursors.WaitCursor);
                ShowUserID = TextClicked.Replace("@","");
                CurrentAction = Yedda.Twitter.ActionType.Show;
                SwitchToList(otherStatslist);
                GetTimeLineAsync();
            }
        }

        void statusList_MenuItemSelected(string ItemName)
        {
            switch (ItemName)
            {
                case "Search":
                    TwitterSearch();
                    break;
                case "Public TimeLine":
                    ChangeCursor(Cursors.WaitCursor);
                    SwitchToList(otherStatslist);
                    otherStatslist.SetSelectedMenu("Public TimeLine");
                    CurrentAction = Yedda.Twitter.ActionType.Public_Timeline;
                    statList.RightMenuItems = RightMenu;
                    GetTimeLineAsync();
                    break;
                case "Reconnect":
                case "Friends TimeLine":
                    ChangeCursor(Cursors.WaitCursor);
                    SwitchToList(friendsStatslist);
                    friendsStatslist.SetSelectedMenu("Friends TimeLine");
                    CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
                    statList.RightMenuItems = RightMenu;
                    
                    GetTimeLineAsync();
                    break;
                case "Replies":
                    ChangeCursor(Cursors.WaitCursor);
                    SwitchToList(otherStatslist);
                    otherStatslist.SetSelectedMenu("Replies");
                    CurrentAction = Yedda.Twitter.ActionType.Replies;
                    statList.RightMenuItems = RightMenu;
                    
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

                case "Archived Timeline":
                    ChangeCursor(Cursors.WaitCursor);
                    ShowUserID = ClientSettings.UserName;
                    CurrentAction = Yedda.Twitter.ActionType.Show;
                    SwitchToList(otherStatslist);
                    GetTimeLineAsync();
                    break;
                case "@User TimeLine":
                    ChangeCursor(Cursors.WaitCursor);
                    FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
                    ShowUserID = selectedItem.Tweet.user.screen_name;
                    CurrentAction = Yedda.Twitter.ActionType.Show;
                    SwitchToList(otherStatslist);
                    GetTimeLineAsync();

                    break;
                case "Reply @User":
                    SendReply();
                    break;
                case "Destroy Favorite":
                    DestroyFavorite();
                    break;
                case "Make Favorite":
                    CreateFavoriteAsync();
                    break;
                case "Direct @User":
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
                    statList.Clear();
                    this.Close();
                    break;
            }
        }

        private void TwitterSearch()
        {
            
            SearchForm f = new SearchForm();
            this.statList.Visible = false;
            if (f.ShowDialog() == DialogResult.Cancel)
            {
                this.statList.Visible = true;
                f.Close(); 
                return; 
            }
            this.statList.Visible = true;
            f.Hide();
            ShowUserID = f.SearchText;
            
            ChangeCursor(Cursors.WaitCursor);
            this.statList.Redraw();
            this.statList.Visible = true;

            f.Close();
            SwitchToList(otherStatslist);
            otherStatslist.SetSelectedMenu("Search");
            CurrentAction = Yedda.Twitter.ActionType.Search;
            statList.RightMenuItems = RightMenu;
            GetTimeLineAsync();
        }

        private void ShowAbout()
        {
            AboutForm a = new AboutForm();
            a.ShowDialog();
        }

        

        private void ShowProfile()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
            pi.FileName = Twitter.GetProfileURL(selectedItem.Tweet.user.screen_name);
            pi.UseShellExecute = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
            statList.HookKey();
        }

        private void SendDirectMessage()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string User = selectedItem.Tweet.user.screen_name;
            SetStatus("d " + User);
        }

        private void ChangeSettings()
        {
            this.statList.Visible = false;
            SettingsForm settings = new SettingsForm();
            if (settings.ShowDialog() == DialogResult.Cancel) { this.statList.Visible = true; return; }
            this.statList.Visible = true;
            Twitter.CurrentServer = ClientSettings.Server;
            if (ClientSettings.CheckVersion)
            {
                Checker.CheckForUpdate();
            }
            if (settings.NeedsReset)
            {
                CachedResponse = "";
                LastStatusID = "";
                CurrentStatuses = new PockeTwit.Library.status[0];
                friendsStatslist.Clear();
                otherStatslist.Clear();
                LoadCachedtimeline();
                SwitchToList(friendsStatslist);
                CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
                GetTimeLine();
            }
            settings.Close();
            
        }

        private void SendReply()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
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
            StatusForm.AllowTwitPic = Twitter.AllowTwitPic;
            this.statList.Visible = false;
            if (!string.IsNullOrEmpty(ToUser))
            {
                StatusForm.StatusText = ToUser + " ";
            }
            if (StatusForm.ShowDialog() == DialogResult.OK)
            {
                this.statList.Visible = true;
                StatusForm.Hide();
                string UpdateText = StatusForm.StatusText;
                if (StatusForm.UseTwitPic)
                {
                    Yedda.TwitPic.SendStoredPic(ClientSettings.UserName, ClientSettings.Password, UpdateText, StatusForm.TwitPicFile);
                }
                else
                {
                    Twitter.Update(ClientSettings.UserName, ClientSettings.Password, UpdateText, Yedda.Twitter.OutputFormatType.XML);
                }
                GetTimeLineAsync();
            }
            this.statList.Redraw();
            this.statList.Visible = true;
            StatusForm.Close();
            
        }

        private void GetTimeLine()
        {
            string response = FetchFromTwitter();

            if (!string.IsNullOrEmpty(response) && response != CachedResponse)
            {
                CachedResponse = response;
                Library.status[] newstatuses;
                if (CurrentAction == Yedda.Twitter.ActionType.Search)
                {
                    newstatuses = Library.status.DeserializeFromAtom(response);
                }
                else
                {
                    newstatuses = Library.status.Deserialize(response);
                }
                if (newstatuses.Length > 0)
                {
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
                        }
                        else
                        {
                            mergedstatuses = newstatuses;
                        }
                    }

                    AddStatusesToList(mergedstatuses, newstatuses.Length);
                    if (CurrentAction == Yedda.Twitter.ActionType.Friends_Timeline)
                    {
                        SaveStatuses(mergedstatuses);
                    }
                }
            }
            ChangeCursor(Cursors.Default);
        }

        private void AddStatusesToList(Library.status[] mergedstatuses, int NewCount)
        {
            if (mergedstatuses == null) { return; }
            statList.Clear();
            
            foreach (Library.status stat in mergedstatuses)
            {
                FingerUI.StatusItem item = new FingerUI.StatusItem();
                if (stat.user.screen_name != null)
                {
                    item.Tweet = stat;
                    statList.AddItem(item);
                }
            }
            statList.SetSelectedIndexToZero();
            statList.Redraw();
            if (ClientSettings.BeepOnNew && !InitialLoad && 
                (CurrentAction == Yedda.Twitter.ActionType.Friends_Timeline |
                CurrentAction == Yedda.Twitter.ActionType.Replies)) 
            {
                NotifyTweets(NewCount);
            }
            
        }

        private void NotifyTweets(int NewCount)
        {
            if (InvokeRequired)
            {
                delNotify d = new delNotify(NotifyTweets);
                this.Invoke(d, new object[]{NewCount});
            }
            else
            {
                if (this.Focused)
                {
                    MessageBeep(0);
                    return;
                }
                 
                System.Text.StringBuilder HTMLString = new StringBuilder();
                HTMLString.Append("<html><body>");
                HTMLString.Append(NewCount.ToString() + " new tweets are available!");
                HTMLString.Append("<form method=\'GET\' action=notify>");
                HTMLString.Append("<p align=\"right\">");
                HTMLString.Append("<input type=button name='Show' value='Show'>");
                HTMLString.Append("<input type=button name='Dismiss' value='Dismiss'>");
                HTMLString.Append("</p>");
                HTMLString.Append("</form>");
                HTMLString.Append("</body></html>");
                notification1.Text = HTMLString.ToString();
                
                notification1.Visible = true;
            }
        }

        private void SaveStatuses(PockeTwit.Library.status[] mergedstatuses)
        {
            string StatusString = Library.status.Serialize(mergedstatuses);
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(ClientSettings.AppPath + "\\" + ClientSettings.UserName +ClientSettings.Server.ToString()+ "FriendsTime.xml"))
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
                    case Yedda.Twitter.ActionType.Search:
                        response = Twitter.SearchFor(ShowUserID);
                        break;
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
                        FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
                        response = Twitter.GetUserTimeline(ClientSettings.UserName, ClientSettings.Password, selectedItem.Tweet.user.screen_name, Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Show:
                        response = Twitter.GetUserTimeline(ClientSettings.UserName, ClientSettings.Password, ShowUserID, Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Favorites:
                        response = Twitter.GetFavorites(ClientSettings.UserName, ClientSettings.Password);
                        break;
                }
            }
            catch (Exception ex)
            {
                statList.Warning = ex.Message;
                CurrentlyConnected = false;
            }
            return response;
        }

        private void FollowUser()
        {
            ChangeCursor(Cursors.WaitCursor);
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string UserID = selectedItem.Tweet.user.screen_name;
            Twitter.FollowUser(ClientSettings.UserName, ClientSettings.Password, UserID);

            Following.AddUser(selectedItem.Tweet.user);
            UpdateRightMenu();
            ChangeCursor(Cursors.Default);
        }

        private void StopFollowingUser()
        {
            ChangeCursor(Cursors.WaitCursor);
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string UserID = selectedItem.Tweet.user.screen_name;
            Twitter.StopFollowingUser(ClientSettings.UserName, ClientSettings.Password, UserID);
            Following.StopFollowing(selectedItem.Tweet.user);
            UpdateRightMenu();
            ChangeCursor(Cursors.Default);
        }

        private void DestroyFavorite()
        {
            ChangeCursor(Cursors.WaitCursor);
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string ID = selectedItem.Tweet.id;
            Twitter.DestroyFavorite(ClientSettings.UserName, ClientSettings.Password, ID);
            selectedItem.isFavorite = false;
            UpdateRightMenu();
            ChangeCursor(Cursors.Default);
        }

        private void CreateFavoriteAsync()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (selectedItem == null) { return; }
            ChangeCursor(Cursors.WaitCursor);
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
            GetTimeLineAsync();
        }

        private void GetTimeLineAsync()
        {
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

        private void notification1_ResponseSubmitted(object sender, Microsoft.WindowsCE.Forms.ResponseSubmittedEventArgs e)
        {
            if (e.Response == "Show")
            {
                this.Show();
            }
            notification1.Visible = false;
        }

        

    }
}