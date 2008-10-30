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
        private Library.status[] CurrentStatuses =null;

        private List<string> LeftMenu = new List<string>(new string[] { "Friends TimeLine", "Messages", "Search/Local", "Set Status", "Settings", "About/Feedback", "Exit" });
        private List<string> RightMenu;
        private Yedda.Twitter.Account CurrentlySelectedAccount;
        private List<Yedda.Twitter> TwitterConnections = new List<Yedda.Twitter>();
        private Dictionary<Yedda.Twitter, Following> FollowingDictionary = new Dictionary<Yedda.Twitter, Following>();
        private TimelineManagement Manager;
        private NotificationHandler Notifyer;
        private bool IsLoaded = false;
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
            PockeTwit.Themes.FormColors.SetColors(this);
            Application.DoEvents();
            if (ClientSettings.AccountsList.Count == 0)
            {
                // SHow Settings page first
                SettingsHandler.MainSettings settings = new PockeTwit.SettingsHandler.MainSettings();
                this.lblLoading.ForeColor = ClientSettings.ForeColor;
                this.BackColor = ClientSettings.BackColor;
                this.ForeColor = ClientSettings.ForeColor;

                //SettingsForm settings = new SettingsForm();
                if (settings.ShowDialog() == DialogResult.Cancel)
                {
                    Application.Exit();
                    this.Close();
                }
                
            }
            timerStartup.Enabled = true;
        }

		#endregion�Constructors�

		#region�Delegates�and�Events�(2)�


		//�Delegates�(2)�
        private delegate void delSetWindowState(FormWindowState state);
        private delegate void delAddStatuses(Library.status[] arrayOfStats, int Count);
        private delegate void delChangeCursor(Cursor CursorToset);
        private delegate void delNotify(int Count);
        private delegate bool delBool();
        private delegate void delNone();

		#endregion�Delegates�and�Events�

		#region�Methods�(38)�


		//�Private�Methods�(38)�

        public bool IsFocused()
        {
            if (InvokeRequired)
            {
                delBool d = new delBool(IsFocused);
                bool invokeRetured = (bool)this.Invoke(d, null);
                return invokeRetured;
            }
            else
            {
                try
                {
                    return this.Focused | statList.Focused;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }

        }

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
            AddStatusesToList(mergedstatuses, 0);
        }
        
        private void AddStatusesToList(Library.status[] mergedstatuses, int newItems)
        {
            if (mergedstatuses == null) { return; }
            if (mergedstatuses.Length == 0) { return; }
            if(InvokeRequired)
            {
                delAddStatuses d = new delAddStatuses(AddStatusesToList);
                this.Invoke(d, new object[] {mergedstatuses, newItems});
            }
            else
            {
                int OldOffset = statList.YOffset;
                statList.Clear();
                
                foreach (Library.status stat in mergedstatuses)
                {
                    if (stat != null && stat.user != null && stat.user.screen_name != null)
                    {
                        FingerUI.StatusItem item = new FingerUI.StatusItem();
                        item.Tweet = stat;
                        statList.AddItem(item);
                    }
                }
                statList.SetSelectedIndexToZero();
                FingerUI.StatusItem currentItem = (FingerUI.StatusItem)statList[0];
                if (currentItem != null)
                {
                    CurrentlySelectedAccount = currentItem.Tweet.Account;
                    UpdateRightMenu();
                }
                statList.Redraw();
                statList.YOffset = OldOffset + (newItems * statList.ItemHeight);                
            }
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
            SettingsHandler.MainSettings settings = new PockeTwit.SettingsHandler.MainSettings();
            //SettingsForm settings = new SettingsForm();
            IsLoaded = false;
            if (settings.ShowDialog() == DialogResult.Cancel) { this.statList.Visible = true; IsLoaded = true; return; }
            statList.BackColor = ClientSettings.BackColor;
            statList.ForeColor = ClientSettings.ForeColor;
            IsLoaded = true;
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
            if (selectedItem == null) { return; }
            Yedda.Twitter.Account ChosenAccount = selectedItem.Tweet.Account;
            
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
            
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (selectedItem == null) { return; }
            if (selectedItem.Tweet.user == null) { return; }
            ChangeCursor(Cursors.WaitCursor);
            Yedda.Twitter Conn = GetMatchingConnection(selectedItem.Tweet.Account);
            Conn.FollowUser(selectedItem.Tweet.user.screen_name);
            FollowingDictionary[Conn].AddUser(selectedItem.Tweet.user);
            UpdateRightMenu();
            ChangeCursor(Cursors.Default);
        }

        
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
            if (statList.SelectedItem == null) { return; }
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
            //LeftMenu = new List<string>(new string[] { "Friends TimeLine", "Messages", "Search/Local", "Set Status", "Settings", "About/Feedback", "Exit" });
            RightMenu = new List<string>(new string[] { "@User TimeLine", "Reply @User", "Direct @User", "Quote", "Make Favorite", "Profile Page", "Stop Following", "Minimize" });

            if (!t.FavoritesWork) { RightMenu.Remove("Make Favorite"); }
            if (!t.DirectMessagesWork) { RightMenu.Remove("Direct @User"); }
            statList.LeftMenuItems = LeftMenu;
            statList.RightMenuItems = RightMenu;
        }

        private bool SetEverythingUp()
        {
            if(System.IO.File.Exists(ClientSettings.AppPath + "\\crash.txt"))
            {
                ChooseAccount errorForm = new ChooseAccount();
                errorForm.ShowDialog();
            }
            this.Show();
            bool ret = true;

            if (ClientSettings.CheckVersion)
            {
                lblLoading.Text = "Launching update checker";
                Application.DoEvents();
                Checker = new UpdateChecker();
                Checker.UpdateFound += new UpdateChecker.delUpdateFound(UpdateChecker_UpdateFound);
            }

            SetUpListControl();

            ResetDictionaries();

            CurrentlySelectedAccount = ClientSettings.AccountsList[0];

            GlobalEventHandler.AvatarHasChanged += new GlobalEventHandler.delAvatarHasChanged(GlobalEventHandler_AvatarHasChanged);
            GlobalEventHandler.Updated += new GlobalEventHandler.ArtWasUpdated(GlobalEventHandler_Updated);
            
            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                Notifyer = new NotificationHandler();
                Notifyer.LoadSettings();
                Notifyer.MessagesNotificationClicked += new NotificationHandler.delNotificationClicked(Notifyer_MessagesNotificationClicked);
            }
            
            return ret;
            
        }

        void GlobalEventHandler_Updated(string User)
        {
            statList.Redraw();
        }

        void GlobalEventHandler_AvatarHasChanged(string User, string NewURL)
        {
            Manager.UpdateImagesForUser(User, NewURL);
            statList.Redraw();
        }

        void Notifyer_MessagesNotificationClicked()
        {
            this.Show();
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
            SetConnectedMenus();
            Manager = new TimelineManagement();
            Manager.NoData += new TimelineManagement.delNullReturnedByAccount(Manager_NoData);
            Manager.ErrorCleared += new TimelineManagement.delNullReturnedByAccount(Manager_ErrorCleared);
            Manager.Progress += new TimelineManagement.delProgress(Manager_Progress);
            Manager.CompleteLoaded += new TimelineManagement.delComplete(Manager_CompleteLoaded);
            Manager.Startup(TwitterConnections);
            Manager.FriendsUpdated += new TimelineManagement.delFriendsUpdated(Manager_FriendsUpdated);
            Manager.MessagesUpdated += new TimelineManagement.delMessagesUpdated(Manager_MessagesUpdated);
            Manager.BothUpdated += new TimelineManagement.delBothUpdated(Manager_BothUpdated);
            foreach (Following f in FollowingDictionary.Values)
            {
                f.LoadFromTwitter();
            }
        }

        void Manager_ErrorCleared(Yedda.Twitter.Account t, Yedda.Twitter.ActionType Action)
        {

            if(LeftMenu.Contains("Errors"))
            {
                if (Yedda.Twitter.Failures.ContainsKey(t))
                {
                    if (Yedda.Twitter.Failures[t].ContainsKey(Action))
                    {
                        Yedda.Twitter.Failures[t][Action] = 0;
                    }
                }
                bool AllClear = true;
                foreach (Dictionary<Yedda.Twitter.ActionType,int> FailList in Yedda.Twitter.Failures.Values)
                {
                    foreach (int i in FailList.Values)
                    {
                        if (i > 0)
                        {
                            AllClear = false;
                        }
                    }
                }
                if (AllClear)
                {

                    //Remove the menu item.
                    LeftMenu.Remove("Errors");
                    lock (statList.LeftMenuItems)
                    {
                        statList.LeftMenuItems = LeftMenu;
                    }
                    statList.Redraw();
                }
            }
        }

        void Manager_NoData(Yedda.Twitter.Account t, Yedda.Twitter.ActionType Action)
        {
            if (!Yedda.Twitter.Failures.ContainsKey(t))
            {
                Yedda.Twitter.Failures.Add(t, new Dictionary<Yedda.Twitter.ActionType, int>());
            }
            if (!Yedda.Twitter.Failures[t].ContainsKey(Action))
            {
                Yedda.Twitter.Failures[t].Add(Action, 0);
            }
            Yedda.Twitter.Failures[t][Action]++;
            if (!LeftMenu.Contains("Errors"))
            {
                LeftMenu.Insert(LeftMenu.Count - 1, "Errors");
                lock (statList.LeftMenuItems)
                {
                    statList.LeftMenuItems = LeftMenu;
                }
                statList.Redraw();
            }
        }


        void Manager_CompleteLoaded()
        {
            if (InvokeRequired)
            {
                delNone d = new delNone(Manager_CompleteLoaded);
                this.Invoke(d);
            }
            else
            {
                lblLoading.Text = "Preparing UI";
                Application.DoEvents();
                statList.SwitchTolist("Friends_TimeLine");

                AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray());

                statList.SetSelectedIndexToZero();
                Application.DoEvents();
            }
        }

        void Manager_Progress(int percentage, string Status)
        {
            lblLoading.Text = Status;
            Application.DoEvents();
        }

        void Manager_BothUpdated(int Messagecount, int FreindsCount)
        {
            if (this.IsFocused())
            {
                if (statList.CurrentList() == "Messages")
                {
                    AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Messages].ToArray(), Messagecount);
                }
                else if (statList.CurrentList() == "Friends_TimeLine")
                {
                    AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray(), FreindsCount);
                }
            }
            else
            {
                if (Notifyer != null)
                {
                    Notifyer.NewBoth(Messagecount, FreindsCount);
                }
            }
        }

        void Manager_MessagesUpdated(int count)
        {
            if (this.IsFocused())
            {
                if (statList.CurrentList() == "Messages")
                {
                    AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Messages].ToArray(), count);
                    if (ClientSettings.BeepOnNew) { MessageBeep(0); }
                }
            }
            else
            {
                if (Notifyer != null)
                {
                    Notifyer.NewMessages(count);
                }
            }
            
        }

        void Manager_FriendsUpdated(int count)
        {
            if (this.IsFocused())
            {
                if (statList.CurrentList() == "Friends_TimeLine")
                {
                    AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray(), count);
                    if (ClientSettings.BeepOnNew) { MessageBeep(0); }
                }
            }
            else
            {
                if (Notifyer != null)
                {
                    Notifyer.NewFriendMessages(count);
                }
            }
        }

        private void SetStatus()
        {
            SetStatus("");
        }

        private void SetStatus(string ToUser)
        {

            SetStatus StatusForm = new SetStatus();
            if (CurrentlySelectedAccount == null)
            {
                StatusForm.AccountToSet = ClientSettings.AccountsList[0];
            }
            else
            {
                StatusForm.AccountToSet = CurrentlySelectedAccount;
            }
            this.statList.Visible = false;
            if (!string.IsNullOrEmpty(ToUser))
            {
                StatusForm.StatusText = ToUser + " ";
            }
            IsLoaded = false;
            if (StatusForm.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                this.statList.Visible = true;
                StatusForm.Hide();
                IsLoaded = false;
                string UpdateText = StatusForm.StatusText;
                if (UpdateText.Length > 140)
                {
                    //Truncate the text to the last available space, the add the URL.
                    string URL = Yedda.ShortText.shorten(UpdateText);
                    int trimLength = 5;
                    if (StatusForm.UseTwitPic)
                    {
                        trimLength = 30;
                    }
                    string NewText = UpdateText.Substring(0, UpdateText.LastIndexOf(" ",140 - (URL.Length + trimLength)));
                    UpdateText = NewText + " ... " + URL;
                }
                if (UpdateText != "Set Status")
                {
                    if (StatusForm.AccountToSet != null)
                    {
                        Yedda.Twitter t = GetMatchingConnection(StatusForm.AccountToSet);
                        if (StatusForm.GPSLocation != null)
                        {
                            try
                            {
                                t.SetLocation(StatusForm.GPSLocation);
                            }
                            catch { }
                        }
                        if (t.AllowTwitPic && StatusForm.UseTwitPic)
                        {
                            string retValue;
                            try
                            {
                                retValue = Yedda.TwitPic.SendStoredPic(StatusForm.AccountToSet.UserName, StatusForm.AccountToSet.Password, UpdateText, StatusForm.TwitPicFile);
                            }
                            catch (Exception ex)
                            {
                                Cursor.Current = Cursors.Default;
                                MessageBox.Show("Error sending the image to twitpic -- " + ex.ToString());
                            }
                        }
                        else
                        {
                            string retValue = t.Update(UpdateText, Yedda.Twitter.OutputFormatType.XML);
                            try
                            {
                                Library.status.DeserializeSingle(retValue, StatusForm.AccountToSet);
                            }
                            catch
                            {
                                MessageBox.Show("Error posting tweet:" + retValue);
                            }
                        }
                    }
                    Manager.RefreshFriendsTimeLine();
                }
            }
            Cursor.Current = Cursors.Default;
            IsLoaded = true;
            this.statList.Redraw();
            this.statList.Visible = true;
            StatusForm.Close();
            
        }

        private void SetUpListControl()
        {
            statList.ItemHeight = (ClientSettings.TextSize * ClientSettings.LinesOfText) + 5;
            statList.IsMaximized = ClientSettings.IsMaximized;
            statList.MenuItemSelected += new FingerUI.KListControl.delMenuItemSelected(statusList_MenuItemSelected);
            statList.WordClicked += new FingerUI.StatusItem.ClickedWordDelegate(statusList_WordClicked);
            statList.SelectedItemChanged += new EventHandler(statusList_SelectedItemChanged);
            statList.SwitchWindowState += new FingerUI.KListControl.delSwitchState(statusList_SwitchWindowState);
            statList.HookKey();
        }

        private void ShowAbout()
        {
            AboutForm a = new AboutForm();
            IsLoaded = false;
            a.ShowDialog();
            if (!string.IsNullOrEmpty(a.AskedToSeeUser))
            {
                SwitchToUserTimeLine(a.AskedToSeeUser);
            }
            a.Close();
            IsLoaded = true;
        }

        private void ShowProfile()
        {
            if (statList.SelectedItem == null) { return; }
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            
            System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
            string ProfileURL = GetMatchingConnection(selectedItem.Tweet.Account).GetProfileURL(selectedItem.Tweet.user.screen_name);
            if (ClientSettings.UseSkweezer)
            {
                ProfileURL = Yedda.Skweezer.GetSkweezerURL(ProfileURL);
            }
            pi.FileName = ProfileURL;
            pi.UseShellExecute = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
        }

        void statusList_MenuItemSelected(string ItemName)
        {
            switch (ItemName)
            {
                case "Errors":
                    Errors errForm = new Errors();
                    errForm.ShowDialog();
                    break;
                case "Search/Local":
                    TwitterSearch();
                    break;
                case "Public TimeLine":
                    ChangeCursor(Cursors.WaitCursor);
                    SwitchToList("Public_TimeLine");
                    statList.SetSelectedMenu("Public TimeLine");
                    statList.RightMenuItems = RightMenu;
                    statList.Redraw();
                    //GetTimeLineAsync();
                    break;
                case "Reconnect":
                case "Friends TimeLine":
                    ChangeCursor(Cursors.WaitCursor);
                    SwitchToList("Friends_TimeLine");
                    statList.SetSelectedMenu("Friends TimeLine");
                    statList.RightMenuItems = RightMenu;
                    AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray());
                    statList.Redraw();
                    Manager.RefreshFriendsTimeLine();
                    ChangeCursor(Cursors.Default);
                    //GetTimeLineAsync();
                    break;
                case "Messages":
                    ChangeCursor(Cursors.WaitCursor);
                    SwitchToList("Messages_TimeLine");
                    statList.SetSelectedMenu("Messages");
                    statList.RightMenuItems = RightMenu;
                    AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Messages].ToArray());
                    statList.Redraw();
                    Manager.RefreshMessagesTimeLine();
                    ChangeCursor(Cursors.Default);
                    //GetTimeLineAsync();
                    break;
                case "Favorites":
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
                    FingerUI.StatusItem statItem = (FingerUI.StatusItem)statList.SelectedItem;
                    if (statItem == null) { return; }
                    ShowUserID = statItem.Tweet.user.screen_name;
                    CurrentlySelectedAccount = statItem.Tweet.Account;
                    Yedda.Twitter Conn = GetMatchingConnection(CurrentlySelectedAccount);
                    SwitchToList("@User_TimeLine");
                    AddStatusesToList(Manager.GetUserTimeLine(Conn, ShowUserID));
                    ChangeCursor(Cursors.Default);


                    break;
                case "Reply @User":
                    SendReply();
                    break;
                case "Quote":
                    Quote();
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

        private void Quote()
        {
            if (statList.SelectedItem == null) { return; }
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string quote = "RT: \"" + selectedItem.Tweet.text + "\"";
            SetStatus(quote);
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
                if (ClientSettings.UseSkweezer)
                {
                    pi.FileName = Yedda.Skweezer.GetSkweezerURL(TextClicked);
                }
                else
                {
                    pi.FileName = TextClicked;
                }
                pi.UseShellExecute = true;
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
            }
            else
            {
                SwitchToUserTimeLine(TextClicked);
            }
        }

        private void SwitchToUserTimeLine(string TextClicked)
        {
            ChangeCursor(Cursors.WaitCursor);
            ShowUserID = TextClicked.Replace("@", "");
            FingerUI.StatusItem statItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (statItem == null) { return; }
            CurrentlySelectedAccount = statItem.Tweet.Account;
            Yedda.Twitter Conn = GetMatchingConnection(CurrentlySelectedAccount);
            SwitchToList("@User_TimeLine");
            AddStatusesToList(Manager.GetUserTimeLine(Conn, ShowUserID));
            ChangeCursor(Cursors.Default);
            return;
        }

        private void StopFollowingUser()
        {
            
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            Yedda.Twitter Conn = GetMatchingConnection(selectedItem.Tweet.Account);
            if (MessageBox.Show("Are you sure you want to stop following " + selectedItem.Tweet.user.screen_name + "?", "Stop Following", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                ChangeCursor(Cursors.WaitCursor);
                Conn.StopFollowingUser(selectedItem.Tweet.user.screen_name);
                FollowingDictionary[Conn].StopFollowing(selectedItem.Tweet.user);
                UpdateRightMenu();
                ChangeCursor(Cursors.Default);
            }
            
        }

        private void SwitchToDone()
        {
            lblLoading.Visible = false;
            lblTitle.Visible = false;
            statList.Visible = true;
            SwitchToList("Friends_TimeLine");
            IsLoaded = true;
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
            IsLoaded = false;
            if (f.ShowDialog() == DialogResult.Cancel)
            {
                IsLoaded = true;
                this.statList.Visible = true;
                f.Close(); 
                return; 
            }
            IsLoaded = true;
            this.statList.Visible = true;
            f.Hide();
            string SearchString = f.SearchText;
            f.Close();
            this.statList.Visible = true;
            
            ChangeCursor(Cursors.WaitCursor);
            statList.SetSelectedMenu("Search/Local");
            statList.RightMenuItems = RightMenu;
            Yedda.Twitter Conn = GetMatchingConnection(CurrentlySelectedAccount);
            SwitchToList("Search_TimeLine");
            this.statList.ClearVisible();
            AddStatusesToList(Manager.SearchTwitter(Conn, SearchString));
            ChangeCursor(Cursors.Default);
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

        private void SetWindowState(FormWindowState State)
        {
            if (InvokeRequired)
            {
                delSetWindowState d = new delSetWindowState(SetWindowState);
                this.Invoke(d, new object[] { State });
            }
            else
            {
                this.WindowState = State;
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            if (DetectDevice.DeviceType == DeviceType.Standard)
            {
                return;
            }
            inputPanel1.Enabled = false;
            if (!IsLoaded)
            {
                return;
            }

            base.OnActivated(e);

            if (ClientSettings.IsMaximized)
            {
                SetWindowState(FormWindowState.Maximized);
            }
            else
            {
                SetWindowState(FormWindowState.Normal);
            }

            if (statList.CurrentList() == "Friends_TimeLine")
            {
                AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray());
                statList.SetSelectedIndexToZero();
                statList.Visible = true;
            }
            else if (statList.CurrentList() == "Messages_TimeLine")
            {
                AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Messages].ToArray());
                statList.SetSelectedIndexToZero();
                statList.Visible = true;
            }

            SendToForground();

            this.Invalidate();
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
            /*
            statList.Clear();
             */
            foreach (Yedda.Twitter.Account a in ClientSettings.AccountsList)
            {
                a.Buffer.Clear();
            }
            ShowWindow(this.Handle, SW_MINIMIZED);
            GC.Collect();
        }

        [System.Runtime.InteropServices.DllImport("coredll.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        void SendToForground()
        {
            SetForegroundWindow(this.Handle);
        }

		#endregion�Methods�

    }

    
}