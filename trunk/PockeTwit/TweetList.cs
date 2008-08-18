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

		#region�Fields�(12)�

        private UpdateChecker Checker;
         private Yedda.Twitter.ActionType CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
        private Library.status[] CurrentStatuses =null;
        private DeviceType DeviceType;
        private bool InitialLoad = true;

        private List<string> LeftMenu;
        private List<string> RightMenu;
        private Yedda.Twitter.Account CurrentlySelectedAccount;
        private List<Yedda.Twitter> TwitterConnections = new List<Yedda.Twitter>();
        private Dictionary<Yedda.Twitter, Following> FollowingDictionary = new Dictionary<Yedda.Twitter, Following>();
        private TimelineManagement Manager;
        
        private string ShowUserID;

        #endregion�Fields�

		#region�Constructors�(1)�

        public TweetList()
        {
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            InitializeComponent();
            Application.DoEvents();
            if (ClientSettings.AccountsList.Count == 0)
            {
                // SHow Settings page first
                SettingsForm settings = new SettingsForm();
                if (settings.ShowDialog() == DialogResult.Cancel)
                {
                    Application.Exit();
                    this.Close();
                }
            }
            timerStartup.Enabled = true;
        }

		#endregion�Constructors�

		#region�Properties�(1)�

        private bool CurrentlyConnected
        {
            set
            {
                if (value)
                {
                    statList.Warning = "";
                    SetConnectedMenus();
                }
                else
                {
                    SetDisconnectedMenus();
                }
            }
        }

		#endregion�Properties�

		#region�Delegates�and�Events�(2)�


		//�Delegates�(2)�

        private delegate void delChangeCursor(Cursor CursorToset);
        private delegate void delNotify(int Count);

		#endregion�Delegates�and�Events�

		#region�Methods�(38)�


		//�Private�Methods�(38)�

        private Yedda.Twitter GetMatchingConnection(Yedda.Twitter.Account AccountInfo)
        {
            if (AccountInfo == null)
            {
                return TwitterConnections[0];
            }
            foreach (Yedda.Twitter conn in TwitterConnections)
            {
                if (conn.AccountInfo.Equals(AccountInfo))
                {
                    return conn;
                }
            }
            return TwitterConnections[0];
        }

        private void AddStatusesToList(Library.status[] mergedstatuses)
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

        private void ChangeSettings()
        {
            this.statList.Visible = false;
            SettingsForm settings = new SettingsForm();
            if (settings.ShowDialog() == DialogResult.Cancel) { this.statList.Visible = true; return; }
            this.statList.Visible = true;
            
            if (ClientSettings.CheckVersion)
            {
                Checker.CheckForUpdate();
            }
            if (settings.NeedsReset)
            {
                Cursor.Current = Cursors.WaitCursor;
                ResetDictionaries();
                CurrentlySelectedAccount = ClientSettings.AccountsList[0];

                CurrentStatuses = new PockeTwit.Library.status[0];
                statList.ItemHeight = (ClientSettings.TextSize * ClientSettings.LinesOfText) + 5;
                statList.Clear();
                SwitchToList("Friends_TimeLine");
                CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
                Cursor.Current = Cursors.Default;
            }
            settings.Close();
            
        }

        private void CreateFavorite(string ID, Yedda.Twitter.Account AccountInfo)
        {
            GetMatchingConnection(AccountInfo).SetFavorite(ID);
            UpdateRightMenu();
            ChangeCursor(Cursors.Default);
        }

        private void CreateFavoriteAsync()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            Yedda.Twitter.Account ChosenAccount = selectedItem.Tweet.Account;
            if (selectedItem == null) { return; }
            ChangeCursor(Cursors.WaitCursor);
            selectedItem.isFavorite = true;

            string ID = selectedItem.Tweet.id;
            System.Threading.ThreadStart ts = delegate { CreateFavorite(ID, ChosenAccount); };
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "CreateFavorite";
            t.Start();
        }

        private void DestroyFavorite()
        {
            ChangeCursor(Cursors.WaitCursor);
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string ID = selectedItem.Tweet.id;
            GetMatchingConnection(selectedItem.Tweet.Account).DestroyFavorite(ID);
            selectedItem.isFavorite = false;
            UpdateRightMenu();
            ChangeCursor(Cursors.Default);
        }


        private void FollowUser()
        {
            ChangeCursor(Cursors.WaitCursor);
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            Yedda.Twitter Conn = GetMatchingConnection(selectedItem.Tweet.Account);
            Conn.FollowUser(selectedItem.Tweet.user.screen_name);
            FollowingDictionary[Conn].AddUser(selectedItem.Tweet.user);
            UpdateRightMenu();
            ChangeCursor(Cursors.Default);
        }

        /*
        private void GetTimeLineAsync()
        {
            //Reset the timer
            tmrautoUpdate.Enabled = false;
            tmrautoUpdate.Interval = ClientSettings.UpdateInterval;
            tmrautoUpdate.Enabled = true;

            System.Threading.ThreadStart ts;
            if (CurrentAction == Yedda.Twitter.ActionType.Show)
            {
                ts = new System.Threading.ThreadStart(GetSingleTimeLine);
            }
            else
            {
                ts = new System.Threading.ThreadStart(GetAllTimeLines);
            }
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "GetTimeLine";
            t.Start();
        }
        */

        [System.Runtime.InteropServices.DllImport("coredll.dll", EntryPoint = "MessageBeep", SetLastError = true)]
        private static extern void MessageBeep(int type);


        private void SendDirectMessage()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string User = selectedItem.Tweet.user.screen_name;
            SetStatus("d " + User);
        }

        private void SendReply()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string User = selectedItem.Tweet.user.screen_name;
            SetStatus("@"+User);
        }

        private void SetConnectedMenus()
        {
            SetConnectedMenus(TwitterConnections[0]);
        }
        private void SetConnectedMenus(Yedda.Twitter t)
        {
            LeftMenu = new List<string>(new string[] { "Friends TimeLine", "Replies", "Search", "Set Status", "Settings", "About/Feedback", "Exit" });
            RightMenu = new List<string>(new string[] { "@User TimeLine", "Reply @User", "Direct @User", "Make Favorite", "Profile Page", "Stop Following", "Minimize" });

            if (!t.FavoritesWork) { RightMenu.Remove("Make Favorite"); }
            if (!t.DirectMessagesWork) { RightMenu.Remove("Direct @User"); }
            statList.LeftMenuItems = LeftMenu;
            statList.RightMenuItems = RightMenu;
        }

        private void SetDisconnectedMenus()
        {
            statList.LeftMenuItems = new List<string>(new string[] { "Reconnect", "Settings", "About/Feedback", "Exit" });
            statList.RightMenuItems = new List<string>(new string[] { "Minimize" });
        }

        private bool SetEverythingUp()
        {
            bool ret = true;
            
            ResetDictionaries();
            
            CurrentlySelectedAccount = ClientSettings.AccountsList[0];

            
            if (ClientSettings.CheckVersion)
            {
                lblLoading.Text = "Launching update checker";
                Application.DoEvents();
                Checker = new UpdateChecker();
                Checker.UpdateFound += new UpdateChecker.delUpdateFound(UpdateChecker_UpdateFound);
            }
            
            lblLoading.Text = "Setting up the UI";
            SetUpListControl();
            
            statList.SwitchTolist("Friends_TimeLine");
            Application.DoEvents();

            lblLoading.Text = "Loading cached timelines";
            Application.DoEvents();
            
            lblLoading.Text = "Fetching timelines from server";
            Application.DoEvents();

            AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray());            

            statList.SetSelectedIndexToZero();
            statList.Visible = true;
            return ret;
        }

        private void ResetDictionaries()
        {
            FollowingDictionary.Clear();
            TwitterConnections.Clear();
            foreach (Yedda.Twitter.Account a in ClientSettings.AccountsList)
            {
                Yedda.Twitter TwitterConn = new Yedda.Twitter();
                TwitterConn.AccountInfo.ServerURL= a.ServerURL;
                TwitterConn.AccountInfo.UserName = a.UserName;
                TwitterConn.AccountInfo.Password = a.Password;
                TwitterConn.AccountInfo.Enabled = a.Enabled;
                TwitterConnections.Add(TwitterConn);
                Following f = new Following(TwitterConn);
                FollowingDictionary.Add(TwitterConn, f);
            }
            Manager = new TimelineManagement(TwitterConnections);
            Manager.FriendsUpdated += new TimelineManagement.delFriendsUpdated(Manager_FriendsUpdated);
            Manager.MessagesUpdated += new TimelineManagement.delMessagesUpdated(Manager_MessagesUpdated);
            foreach (Following f in FollowingDictionary.Values)
            {
                f.LoadFromTwitter();
            }
        }

        void Manager_MessagesUpdated(int count)
        {
            if (statList.CurrentList() == "Replies")
            {
                AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Messages].ToArray());
            }
            else
            {
                //Raise Notification
            }
        }

        void Manager_FriendsUpdated(int count)
        {
            if (statList.CurrentList() == "Friends_TimeLine")
            {
                AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray());
            }
            else
            {
                //Raise Notification
            }
        }

        private void SetStatus()
        {
            SetStatus("");
        }

        private void SetStatus(string ToUser)
        {

            SetStatus StatusForm = new SetStatus();
            StatusForm.AccountToSet = CurrentlySelectedAccount;
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
                if (UpdateText != "Set Status")
                {
                    Yedda.Twitter t = GetMatchingConnection(StatusForm.AccountToSet);
                    if (t.AllowTwitPic && StatusForm.UseTwitPic)
                    {
                        Yedda.TwitPic.SendStoredPic(StatusForm.AccountToSet.UserName, StatusForm.AccountToSet.Password, UpdateText, StatusForm.TwitPicFile);
                    }
                    else
                    {
                        t.Update(UpdateText, Yedda.Twitter.OutputFormatType.XML);
                    }
                    //GetTimeLineAsync();
                }
            }
            this.statList.Redraw();
            this.statList.Visible = true;
            StatusForm.Close();
            
        }

        private void SetUpListControl()
        {
            statList.BackColor = ClientSettings.BackColor;
            statList.ForeColor = ClientSettings.ForeColor;
            statList.SelectedBackColor = ClientSettings.SelectedBackColor;
            statList.SelectedForeColor = ClientSettings.SelectedForeColor;
            statList.ItemHeight = (ClientSettings.TextSize * ClientSettings.LinesOfText) + 5;
            statList.IsMaximized = ClientSettings.IsMaximized;
            SetConnectedMenus();
            statList.MenuItemSelected += new FingerUI.KListControl.delMenuItemSelected(statusList_MenuItemSelected);
            statList.WordClicked += new FingerUI.StatusItem.ClickedWordDelegate(statusList_WordClicked);
            statList.SelectedItemChanged += new EventHandler(statusList_SelectedItemChanged);
            statList.SwitchWindowState += new FingerUI.KListControl.delSwitchState(statusList_SwitchWindowState);
            statList.HookKey();
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
            pi.FileName = GetMatchingConnection(selectedItem.Tweet.Account).GetProfileURL(selectedItem.Tweet.user.screen_name);
            pi.UseShellExecute = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
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
                    SwitchToList("Public_TimeLine");
                    statList.SetSelectedMenu("Public TimeLine");
                    CurrentAction = Yedda.Twitter.ActionType.Public_Timeline;
                    statList.RightMenuItems = RightMenu;
                    statList.Redraw();
                    //GetTimeLineAsync();
                    break;
                case "Reconnect":
                case "Friends TimeLine":
                    ChangeCursor(Cursors.WaitCursor);
                    SwitchToList("Friends_TimeLine");
                    statList.SetSelectedMenu("Friends TimeLine");
                    CurrentAction = Yedda.Twitter.ActionType.Friends_Timeline;
                    statList.RightMenuItems = RightMenu;
                    AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray());
                    statList.Redraw();
                    ChangeCursor(Cursors.Default);
                    //GetTimeLineAsync();
                    break;
                case "Replies":
                    ChangeCursor(Cursors.WaitCursor);
                    SwitchToList("Replies_TimeLine");
                    statList.SetSelectedMenu("Replies");
                    CurrentAction = Yedda.Twitter.ActionType.Replies;
                    statList.RightMenuItems = RightMenu;
                    AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Messages].ToArray());
                    statList.Redraw();
                    ChangeCursor(Cursors.Default);
                    //GetTimeLineAsync();
                    break;
                case "Favorites":
                    CurrentAction = Yedda.Twitter.ActionType.Favorites;
                    //GetTimeLineAsync();
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

                case "@User TimeLine":
                    ChangeCursor(Cursors.WaitCursor);
                    FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
                    ShowUserID = selectedItem.Tweet.user.screen_name;
                    CurrentAction = Yedda.Twitter.ActionType.Show;
                    SwitchToList("@User_TimeLine");
                    //GetTimeLineAsync();

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
                    Minimize();
                    break;
                case "Exit":
                    statList.Clear();
                    this.Close();
                    break;
            }
        }

        void statusList_SelectedItemChanged(object sender, EventArgs e)
        {
            FingerUI.StatusItem statItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (statItem == null) { return; }
            CurrentlySelectedAccount = statItem.Tweet.Account;
            SetConnectedMenus(GetMatchingConnection(CurrentlySelectedAccount));
            UpdateRightMenu();
        }

        void statusList_SwitchWindowState(bool IsMaximized)
        {
            ClientSettings.IsMaximized = IsMaximized;
            ClientSettings.SaveSettings();
            if (IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
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
                SwitchToList("@User_TimeLine");
                //GetTimeLineAsync();
            }
        }

        private void StopFollowingUser()
        {
            ChangeCursor(Cursors.WaitCursor);
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            Yedda.Twitter Conn = GetMatchingConnection(selectedItem.Tweet.Account);
            Conn.StopFollowingUser(selectedItem.Tweet.user.screen_name);
            FollowingDictionary[Conn].StopFollowing(selectedItem.Tweet.user);
            UpdateRightMenu();
            ChangeCursor(Cursors.Default);
        }

        private void SwitchToDone()
        {
            lblLoading.Visible = false;
            lblTitle.Visible = false;
            InitialLoad = false;
            SwitchToList("Friends_TimeLine");
        }

        private void SwitchToList(string ListName)
        {
            statList.SwitchTolist(ListName);
        }

        private void timerStartup_Tick(object sender, EventArgs e)
        {
            timerStartup.Enabled = false;
            timerStartup.Tick -= new EventHandler(timerStartup_Tick);
            if (!SetEverythingUp())
            {
                Application.Exit();
                return;
            }
            SwitchToDone();
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
            SwitchToList("Search_TimeLine");
            statList.SetSelectedMenu("Search");
            CurrentAction = Yedda.Twitter.ActionType.Search;
            statList.RightMenuItems = RightMenu;
            //GetTimeLineAsync();
        }

        void UpdateChecker_UpdateFound(UpdateChecker.UpdateInfo Info)
        {
            UpdateForm uf = new UpdateForm();
            uf.NewVersion = Info;
            uf.ShowDialog();
        }

        private void UpdateRightMenu()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (selectedItem == null) { return; }
            Yedda.Twitter conn = GetMatchingConnection(selectedItem.Tweet.Account);
            if (selectedItem != null)
            {
                if (conn.FavoritesWork)
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
                
                if (FollowingDictionary[conn].IsFollowing(selectedItem.Tweet.user))
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
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        [System.Runtime.InteropServices.DllImport("coredll.dll")]
        static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_MINIMIZED = 6;

        void Minimize()
        {
            // The Taskbar must be enabled to be able to do a Smart Minimize
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.WindowState = FormWindowState.Normal;
            this.ControlBox = true;
            this.MinimizeBox = true;
            this.MaximizeBox = true;

            // Since there is no WindowState.Minimize, we have to P/Invoke ShowWindow
            ShowWindow(this.Handle, SW_MINIMIZED);
        }

		#endregion�Methods�

    }

    
}