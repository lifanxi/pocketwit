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
        private class HistoryItem
        {
            public string Argument;
            public Yedda.Twitter.ActionType Action;
            public Yedda.Twitter.Account Account;
            public int SelectedItemIndex = -1;
            public int itemsOffset = -1;
        }

        private Stack<HistoryItem> History = new Stack<HistoryItem>();
        #region�Fields�(12)�
        private UpgradeChecker Checker;

        private Yedda.Twitter.Account CurrentlySelectedAccount;
        private List<Yedda.Twitter> TwitterConnections = new List<Yedda.Twitter>();
        private Dictionary<Yedda.Twitter, Following> FollowingDictionary = new Dictionary<Yedda.Twitter, Following>();
        private TimelineManagement Manager;
        private NotificationHandler Notifyer;
        private bool IsLoaded = false;
        private string ShowUserID;
        private bool StartBackground = false;
        private int displayedNewMessages;
        private int displayedNewUpdates;

        #region MenuItems
        #region LeftMenu
        FingerUI.SideMenuItem BackMenuItem;

        FingerUI.SideMenuItem FriendsTimeLineMenuItem;
        FingerUI.SideMenuItem MessagesMenuItem;
        
        FingerUI.SideMenuItem PublicMenuItem;
        FingerUI.SideMenuItem SearchMenuItem;
        FingerUI.SideMenuItem ViewFavoritesMenuItem;
        FingerUI.SideMenuItem MergedTimeLineMenuItem;

        FingerUI.SideMenuItem TimeLinesMenuItem;

        FingerUI.SideMenuItem PostUpdateMenuItem;
        FingerUI.SideMenuItem MapMenuItem;
        FingerUI.SideMenuItem SettingsMenuItem;
        FingerUI.SideMenuItem AboutMenuItem;
        FingerUI.SideMenuItem ExitMenuItem;
        #endregion
        #region RightMenu
        FingerUI.SideMenuItem ConversationMenuItem;
        
        FingerUI.SideMenuItem ReponsesMenuItem;

        FingerUI.SideMenuItem ReplyMenuItem;
        FingerUI.SideMenuItem DirectMenuItem;

        FingerUI.SideMenuItem EmailMenuItem;
        FingerUI.SideMenuItem QuoteMenuItem;
        FingerUI.SideMenuItem ToggleFavoriteMenuItem;
        FingerUI.SideMenuItem UserTimelineMenuItem;
        FingerUI.SideMenuItem ProfilePageMenuItem;
        FingerUI.SideMenuItem FollowMenuItem;
        FingerUI.SideMenuItem MinimizeMenuItem;
        #endregion
        #endregion MenuItems

        #endregion�Fields�

        #region�Constructors�(1)�
        public TweetList(bool InBackGround)
        {
            StartBackground = InBackGround;
            Microsoft.WindowsCE.Forms.MobileDevice.Hibernate += new EventHandler(MobileDevice_Hibernate);
            Program.StartUp = DateTime.Now;
            if (InBackGround)
            {
                this.Hide();
            }
            else
            {
                if (ClientSettings.IsMaximized)
                {
                    this.WindowState = FormWindowState.Maximized;
                }
            }
            InitializeComponent();

            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
            }
            if (UpgradeChecker.devBuild)
            {
                this.lblTitle.Text = "Launching PockeTwit Dev";
            }


            SizeF currentScreen = this.CurrentAutoScaleDimensions;
            if (currentScreen.Height == 192)
            {
                statList.MaxVelocity = 45;
            }
            else
            {
                statList.MaxVelocity = 45;
            }
            ClientSettings.TextHeight = currentScreen.Height;

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
                    statList.Clear();
                    if (Manager != null)
                    {
                        Manager.ShutDown();
                    }
                    this.Close();
                }

            }
            if (InBackGround)
            {
                timerStartup_Tick(null, new EventArgs());
            }
            else
            {
                timerStartup.Enabled = true;
            }
        }

        void MobileDevice_Hibernate(object sender, EventArgs e)
        {
            //MessageBox.Show("The device is running low on memory. You may want to close PockeTwit or other applications.");
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
            if (InvokeRequired)
            {
                delAddStatuses d = new delAddStatuses(AddStatusesToList);
                this.Invoke(d, new object[] { mergedstatuses, newItems });
            }
            else
            {
                int OldOffset = 0;
                int oldIndex = 0;

                
                OldOffset = statList.YOffset;
                oldIndex = statList.SelectedIndex;

                int oldCount = statList.Count;
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
                FingerUI.StatusItem currentItem = null;
                if (!ClientSettings.AutoScrollToTop)
                {
                    if (oldIndex >= 0 && newItems < oldCount)
                    {
                        try
                        {
                            statList.SelectedItem = statList[oldIndex + newItems];
                            currentItem = (FingerUI.StatusItem)statList.SelectedItem;
                            statList.YOffset = OldOffset + (newItems * ClientSettings.ItemHeight);
                        }
                        catch (KeyNotFoundException)
                        {
                            //What to do?
                        }
                    }
                    else
                    {
                        statList.JumpToLastSelected();
                        currentItem = (FingerUI.StatusItem)statList[0];
                    }
                }
                else
                {
                    statList.SelectedItem = statList[0];
                    statList.YOffset = 0;
                }
                if (currentItem != null)
                {
                    CurrentlySelectedAccount = currentItem.Tweet.Account;
                    UpdateRightMenu();
                }
                statList.Redraw();
                statList.RerenderPortal();
                statList.Repaint();
                GC.Collect();
                //This is the best time to save the addressbook.
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(AddressBook.Save));
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
            if (settings.ShowDialog() == DialogResult.Cancel)
            {
                this.statList.Visible = true;
                IsLoaded = true;
                return;
            }
            statList.BackColor = ClientSettings.BackColor;
            statList.ForeColor = ClientSettings.ForeColor;
            IsLoaded = true;
            this.statList.Visible = true;

            if (ClientSettings.CheckVersion)
            {
                Checker.CheckForUpgrade();
            }
            if (settings.NeedsReset)
            {
                MessageBox.Show("Your settings changes require that you restart the application.");
                statList.Clear();
                if (Manager != null)
                {
                    Manager.ShutDown();
                }
                this.Close();
            }
            if (settings.NeedsRerender)
            {
                PockeTwit.Themes.FormColors.SetColors(this);
                statList.ResetFullScreenColors();
                statList.RerenderBySize();
            }
            statList.Redraw();
            settings.Close();

        }

        private void ToggleFavorite()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (selectedItem == null) { return; }
            if (selectedItem.isFavorite)
            {
                DestroyFavorite();
                ToggleFavoriteMenuItem.Text = "Make Favorite";
            }
            else
            {
                CreateFavoriteAsync();
                ToggleFavoriteMenuItem.Text = "Remove Favorite";
            }
        }

        private void CreateFavorite(string ID, Yedda.Twitter.Account AccountInfo)
        {
            GetMatchingConnection(AccountInfo).SetFavorite(ID);
            UpdateRightMenu();
            statList.Repaint();
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
            statList.Repaint();
            ChangeCursor(Cursors.Default);
        }


        private void ToggleFollow()
        {
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (selectedItem == null) { return; }
            if (selectedItem.Tweet.user == null) { return; }
            Yedda.Twitter Conn = GetMatchingConnection(selectedItem.Tweet.Account);
            if (FollowingDictionary[Conn].IsFollowing(selectedItem.Tweet.user))
            {
                StopFollowingUser();
                FollowMenuItem.Text = "Follow";
            }
            else
            {
                FollowUser();
                FollowMenuItem.Text = "Stop Following";
            }
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
        private void StopFollowingUser()
        {
            if (statList.SelectedItem == null) { return; }
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


        private void SendDirectMessage()
        {
            if (statList.SelectedItem == null) { return; }
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string User = selectedItem.Tweet.user.screen_name;
            SetStatus("d " + User, selectedItem.Tweet.id);
        }

        private void SendReply()
        {
            if (statList.SelectedItem == null) { return; }
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string User = selectedItem.Tweet.user.screen_name;
            if (selectedItem.Tweet.isDirect)
            {
                if (MessageBox.Show("Are you sure you want to reply to a Direct Message?", "Repy to DM?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    SendDirectMessage();
                    return;
                }
            }
            SetStatus("@" + User, selectedItem.Tweet.id);
        }

        private void CreateLeftMenu()
        {
            BackMenuItem = new FingerUI.SideMenuItem(this.GoBackInHistory, "Back", statList.LeftMenu);
            BackMenuItem.CanHide = true;

            FriendsTimeLineMenuItem = new FingerUI.SideMenuItem(this.ShowFriendsTimeLine, "Friends Timeline", statList.LeftMenu);
            MessagesMenuItem = new FingerUI.SideMenuItem(this.ShowMessagesTimeLine, "Messages", statList.LeftMenu);
            PublicMenuItem = new FingerUI.SideMenuItem(this.ShowPublicTimeLine, "Public Timeline", statList.LeftMenu);
            SearchMenuItem = new FingerUI.SideMenuItem(this.TwitterSearch, "Search/Local", statList.LeftMenu);
            ViewFavoritesMenuItem = new FingerUI.SideMenuItem(this.ShowFavorites, "View Favorites", statList.LeftMenu);

            MergedTimeLineMenuItem = new FingerUI.SideMenuItem(ShowFriendsTimeLine, "Timeline", statList.LeftMenu);
            
            TimeLinesMenuItem = new FingerUI.SideMenuItem(null, "Other TimeLines ...", statList.LeftMenu);
            TimeLinesMenuItem.SubMenuItems.Add(SearchMenuItem);
            TimeLinesMenuItem.SubMenuItems.Add(PublicMenuItem);
            TimeLinesMenuItem.SubMenuItems.Add(ViewFavoritesMenuItem);
            
            
            PostUpdateMenuItem = new FingerUI.SideMenuItem(this.SetStatus, "Post Update", statList.LeftMenu);
            
            MapMenuItem = new FingerUI.SideMenuItem(this.MapList, "Map These", statList.LeftMenu);
            SettingsMenuItem = new FingerUI.SideMenuItem(this.ChangeSettings, "Settings", statList.LeftMenu);
            AboutMenuItem = new FingerUI.SideMenuItem(this.ShowAbout, "About/Feedback", statList.LeftMenu);
            MinimizeMenuItem = new FingerUI.SideMenuItem(this.Minimize, "Minimize", statList.RightMenu);
            ExitMenuItem = new FingerUI.SideMenuItem(this.ExitApplication, "Exit", statList.LeftMenu);
            
            if (ClientSettings.MergeMessages)
            {
                statList.LeftMenu.ResetMenu(new FingerUI.SideMenuItem[]{BackMenuItem, MergedTimeLineMenuItem, PostUpdateMenuItem, SearchMenuItem, MapMenuItem, SettingsMenuItem,
                AboutMenuItem, MinimizeMenuItem, ExitMenuItem});
            }
            else
            {
                statList.LeftMenu.ResetMenu(new FingerUI.SideMenuItem[]{BackMenuItem, FriendsTimeLineMenuItem, MessagesMenuItem, TimeLinesMenuItem, PostUpdateMenuItem, MapMenuItem, SettingsMenuItem,
                AboutMenuItem, MinimizeMenuItem, ExitMenuItem});
            }
            /*
            statList.LeftMenu.ResetMenu(new FingerUI.SideMenuItem[]{BackMenuItem, TimeLinesMenuItem, PostUpdateMenuItem, SearchMenuItem, MapMenuItem, SettingsMenuItem,
                AboutMenuItem, ExitMenuItem});
             */
        }

        private void CreateRightMenu()
        {
            // "Show Conversation", "Reply @User", "Direct @User", "Quote", 
            //   "Make Favorite", "@User TimeLine", "Profile Page", "Stop Following",
            // "Minimize" 
            ConversationMenuItem = new FingerUI.SideMenuItem(GetConversation, "Show Conversation", statList.RightMenu);
            ConversationMenuItem.CanHide = true;

            ReponsesMenuItem = new FingerUI.SideMenuItem(null, "Respond to @User...", statList.RightMenu);

            ReplyMenuItem = new FingerUI.SideMenuItem(SendReply, "Reply @User", statList.RightMenu);
            DirectMenuItem = new FingerUI.SideMenuItem(SendDirectMessage, "Direct @User", statList.RightMenu);

            ReponsesMenuItem.SubMenuItems.Add(ReplyMenuItem);
            ReponsesMenuItem.SubMenuItems.Add(DirectMenuItem);

            EmailMenuItem = new FingerUI.SideMenuItem(EmailThisItem, "Email Status", statList.RightMenu);
            QuoteMenuItem = new FingerUI.SideMenuItem(this.Quote, "Quote", statList.RightMenu);
            ToggleFavoriteMenuItem = new FingerUI.SideMenuItem(ToggleFavorite, "Make Favorite", statList.RightMenu);
            UserTimelineMenuItem = new FingerUI.SideMenuItem(ShowUserTimeLine, "@User Timeline", statList.RightMenu);
            ProfilePageMenuItem = new FingerUI.SideMenuItem(ShowProfile, "@User Profile", statList.RightMenu);
            FollowMenuItem = new FingerUI.SideMenuItem(ToggleFollow, "Follow @User", statList.RightMenu);


            statList.RightMenu.ResetMenu(new FingerUI.SideMenuItem[]{ConversationMenuItem, ReponsesMenuItem, QuoteMenuItem, EmailMenuItem, ToggleFavoriteMenuItem, 
                UserTimelineMenuItem, ProfilePageMenuItem, FollowMenuItem});
        }


        private void SetLeftMenu()
        {
            BackMenuItem.Visible = History.Count > 1;
            /*
            if (ClientSettings.MergeMessages)
            {
                LeftMenu = new List<string>(new string[] { "Back", "TimeLine", "Post Update", "Search/Local", "Map These", "Settings", "About/Feedback", "Exit" });
            }
            else
            {
                LeftMenu = new List<string>(new string[] { "Back", "Friends TimeLine", "Messages", "Post Update", "Search/Local", "Map These", "Settings", "About/Feedback", "Exit" });
            }
            if (History.Count <= 1)
            {
                LeftMenu.Remove("Back");
            }
            
            statList.LeftMenu.ResetMenu(LeftMenu);
             */
            switch (statList.CurrentList())
            {
                case "Friends_TimeLine":
                    FriendsTimeLineMenuItem.Text = "Refresh Friends Timeline" + newItemsText(displayedNewUpdates);
                    MessagesMenuItem.Text = "Messages" + newItemsText(displayedNewMessages);
                    MergedTimeLineMenuItem.Text = "Refresh TimeLine" + newItemsText(displayedNewUpdates);
                    break;
                case "Messages_TimeLine":
                    FriendsTimeLineMenuItem.Text = "Friends Timeline" + newItemsText(displayedNewUpdates);
                    MessagesMenuItem.Text = "Refresh Messages" + newItemsText(displayedNewMessages);
                    MergedTimeLineMenuItem.Text = "TimeLine" + newItemsText(displayedNewUpdates);
                    break;
                default:
                    MergedTimeLineMenuItem.Text = "TimeLine" + newItemsText(displayedNewUpdates);
                    MessagesMenuItem.Text = "Messages" + newItemsText(displayedNewMessages);
                    FriendsTimeLineMenuItem.Text = "Friends Timeline" + newItemsText(displayedNewUpdates);
                    break;
            }
        }
        private void UpdateRightMenu()
        {

            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (selectedItem == null) { return; }
            Yedda.Twitter conn = GetMatchingConnection(selectedItem.Tweet.Account);
            if (selectedItem != null)
            {
                statList.SetRightMenuUser();
                if (string.IsNullOrEmpty(selectedItem.Tweet.in_reply_to_status_id))
                {
                    ConversationMenuItem.Visible = false;
                }
                else
                {
                    ConversationMenuItem.Visible = true;
                }
                if (conn.FavoritesWork)
                {
                    if (selectedItem.isFavorite)
                    {
                        ToggleFavoriteMenuItem.Text = "Remove Favorite";
                    }
                    else
                    {
                        ToggleFavoriteMenuItem.Text = "Make Favorite";
                    }
                }

                if (string.IsNullOrEmpty(selectedItem.Tweet.user.id))
                {
                    FollowMenuItem.Visible = false;
                }
                else
                {
                    FollowMenuItem.Visible = true;
                    if (FollowingDictionary[conn].IsFollowing(selectedItem.Tweet.user))
                    {
                        FollowMenuItem.Text = "Stop Following";
                    }
                    else
                    {
                        FollowMenuItem.Text = "Follow";
                    }
                }
            }
        }

        private void SetConnectedMenus()
        {
            if (TwitterConnections.Count > 0)
            {
                SetConnectedMenus(TwitterConnections[0], null);
            }
        }
        private void SetConnectedMenus(Yedda.Twitter t, FingerUI.StatusItem item)
        {
            /*
            RightMenu = new List<string>(new string[] { "Show Conversation", "Reply @User", "Direct @User", "Quote", "Make Favorite", "@User TimeLine", "Profile Page", "Stop Following", "Minimize" });
            if (!t.FavoritesWork) { RightMenu.Remove("Make Favorite"); }
            if (!t.DirectMessagesWork) { RightMenu.Remove("Direct @User"); }
            
            if (item == null || string.IsNullOrEmpty(item.Tweet.in_reply_to_status_id))
            {
                RightMenu.Remove("Show Conversation");
            }
            statList.RightMenu.ResetMenu(RightMenu);
            SetLeftMenu();
             */
            SetLeftMenu();
            UpdateRightMenu();
        }

        private bool SetEverythingUp()
        {
            HistoryItem firstItem = new HistoryItem();
            firstItem.Action = Yedda.Twitter.ActionType.Friends_Timeline;
            History.Push(firstItem);
            if (System.IO.File.Exists(ClientSettings.AppPath + "\\crash.txt"))
            {
                CrashReport errorForm = new CrashReport();
                errorForm.ShowDialog();
            }
            if (!StartBackground)
            {
                this.Show();
            }
            bool ret = true;

            if (ClientSettings.CheckVersion)
            {
                Checker = new UpgradeChecker();
                Checker.UpgradeFound += new UpgradeChecker.delUpgradeFound(UpdateChecker_UpdateFound);
            }
            //lblLoading.Text = "Preparing";
            //Application.DoEvents();
            SetUpListControl();

            try
            {
                ResetDictionaries();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Corrupt settings-- " +ex.Message + "\r\nPlease reconfigure.");
                System.IO.File.Delete(ClientSettings.AppPath + "\\app.config");
                ClientSettings.AccountsList.Clear();
                SettingsHandler.MainSettings settings = new PockeTwit.SettingsHandler.MainSettings();
                if (settings.ShowDialog() == DialogResult.Cancel)
                {
                    statList.Clear();
                    Manager.ShutDown();
                    this.Close();
                }
                ResetDictionaries();
            }

            CurrentlySelectedAccount = ClientSettings.DefaultAccount;

            Notifyer = new NotificationHandler();
            NotificationHandler.LoadSettings();
            Notifyer.MessagesNotificationClicked += new NotificationHandler.delNotificationClicked(Notifyer_MessagesNotificationClicked);

            return ret;

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
                TwitterConn.AccountInfo.ServerURL = a.ServerURL;
                TwitterConn.AccountInfo.UserName = a.UserName;
                TwitterConn.AccountInfo.Password = a.Password;
                TwitterConn.AccountInfo.Enabled = a.Enabled;
                TwitterConnections.Add(TwitterConn);
                Following f = new Following(TwitterConn);
                FollowingDictionary.Add(TwitterConn, f);
            }
            SetConnectedMenus();
            Manager = new TimelineManagement();
            Manager.Progress += new TimelineManagement.delProgress(Manager_Progress);
            Manager.CompleteLoaded += new TimelineManagement.delComplete(Manager_CompleteLoaded);
            Manager.Startup(TwitterConnections);
            Manager.FriendsUpdated += new TimelineManagement.delFriendsUpdated(Manager_FriendsUpdated);
            Manager.MessagesUpdated += new TimelineManagement.delMessagesUpdated(Manager_MessagesUpdated);

            foreach (Following f in FollowingDictionary.Values)
            {
                f.LoadFromTwitter();
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
                statList.Startup = false;
                if (!StartBackground)
                {
                    statList.Visible = true;
                }
                //statList.SetSelectedIndexToZero();
                Program.Ready = DateTime.Now;
                Application.DoEvents();
            }
        }

        void Manager_Progress(int percentage, string Status)
        {
            lblLoading.Text = Status;
            Application.DoEvents();
        }

        void Manager_MessagesUpdated(int count)
        {
            if (statList.CurrentList() == "Messages_TimeLine")
            {
                AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Messages].ToArray(), count);
            }
            displayedNewMessages += count;
            MessagesMenuItem.Text = "Messages" + newItemsText(displayedNewMessages);
            Notifyer.NewMessages(count);
            setCaption(newItemsText(displayedNewUpdates) + " " + newItemsText(displayedNewMessages));
        }

        private string newItemsText(int count)
        {
            if (count > 0)
            {
                return " (" + count.ToString() + ")";
            }
            return "";
        }


        delegate void delText(string Text);
        private void setCaption(string text)
        {
            if (InvokeRequired)
            {
                delText d = new delText(setCaption);
                this.BeginInvoke(d, text);
            }
            else
            {
                this.Text = "PockeTwit" + text;
            }
        }

        void Manager_FriendsUpdated(int count)
        {
            if (statList.CurrentList() == "Friends_TimeLine")
            {
                AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray(), count);
            }
            displayedNewUpdates += count;
            FriendsTimeLineMenuItem.Text = "Friends Timeline" + newItemsText(displayedNewUpdates);
            Notifyer.NewFriendMessages(count);
            setCaption(newItemsText(displayedNewUpdates) + " " + newItemsText(displayedNewMessages));
        }

        private void MapList()
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            ProfileMap m = new ProfileMap();
            List<Library.User> users = new List<Library.User>();
            for (int i = 0; i < statList.m_items.Count; i++)
            {
                Library.User thisUser = statList.m_items[i].Tweet.user;
                if (thisUser.needsFetching)
                {
                    thisUser = Library.User.FromId(thisUser.screen_name, statList.m_items[i].Tweet.Account);
                    thisUser.needsFetching = false;
                }
                users.Add(thisUser);
            }
            m.Users = users;
            m.ShowDialog();
            if (m.Range > 0)
            {

                SearchForm f = new SearchForm();
                f.providedDistnce = m.Range.ToString();
                string secondLoc = Yedda.GoogleGeocoder.Geocode.GetAddress(m.CenterLocation.ToString());
                if (string.IsNullOrEmpty(secondLoc))
                {
                    secondLoc = m.CenterLocation.ToString();
                }

                f.providedLocation = secondLoc;

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

                ShowSearchResults(SearchString);
            }
            m.Close();
        }

        private void SetStatus()
        {
            SetStatus("", "");
        }

        private void SetStatus(string ToUser, string in_reply_to_status_id)
        {
            PostUpdate StatusForm = new PostUpdate(false);

            if (CurrentlySelectedAccount == null)
            {
                StatusForm.AccountToSet = ClientSettings.DefaultAccount;
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
            StatusForm.in_reply_to_status_id = in_reply_to_status_id;
            Manager.Pause();
            if (StatusForm.ShowDialog() == DialogResult.OK)
            {
                this.statList.Visible = true;
                StatusForm.Hide();
                IsLoaded = false;
                Manager.RefreshFriendsTimeLine();
            }
            else
            {
                this.statList.Visible = true;
                StatusForm.Hide();
                IsLoaded = false;
            }
            Manager.Start();
            this.Visible = true;
            IsLoaded = true;
            this.statList.Redraw();
            this.statList.Visible = true;
            StatusForm.Close();
        }

        private void SetUpListControl()
        {
            statList.IsMaximized = ClientSettings.IsMaximized;
            //statList.MenuItemSelected += new FingerUI.KListControl.delMenuItemSelected(statusList_MenuItemSelected);
            statList.WordClicked += new FingerUI.StatusItem.ClickedWordDelegate(statusList_WordClicked);
            statList.SelectedItemChanged += new EventHandler(statusList_SelectedItemChanged);
            statList.SwitchWindowState += new FingerUI.KListControl.delSwitchState(statusList_SwitchWindowState);
            statList.HookKey();
        }

        private void ShowAbout()
        {
            AboutForm a = new AboutForm();
            IsLoaded = false;
            statList.Visible = false;
            a.ShowDialog();

            this.Visible = true;
            
            statList.Visible = true;
            IsLoaded = true;
            string ReqedUser = a.AskedToSeeUser;
            a.Close();
            
            if (!string.IsNullOrEmpty(ReqedUser))
            {
                statList.IgnoreMouse = true;
                SwitchToUserTimeLine(a.AskedToSeeUser);
            }
        }
        private void EmailThisItem()
        {
            if (statList.SelectedItem == null) { return; }
            FingerUI.StatusItem selectedItem = statList.SelectedItem;
            Microsoft.WindowsMobile.PocketOutlook.OutlookSession sess = new Microsoft.WindowsMobile.PocketOutlook.OutlookSession();
            Microsoft.WindowsMobile.PocketOutlook.EmailAccountCollection accounts = sess.EmailAccounts;

            if (accounts.Count == 0)
            {
                MessageBox.Show("You don't have any email accounts set up on this phone.");
                return;
            }
            else if (accounts.Count>1)
            {
                EmailStatusForm f = new EmailStatusForm(selectedItem.Tweet.text);
                f.ShowDialog();
                f.Close();
                return;
            }

            Microsoft.WindowsMobile.PocketOutlook.EmailMessage m = new Microsoft.WindowsMobile.PocketOutlook.EmailMessage();
            m.BodyText = selectedItem.Tweet.text;
            Microsoft.WindowsMobile.PocketOutlook.MessagingApplication.DisplayComposeForm(accounts[0], m);
        }
        private void ShowProfile()
        {
            if (statList.SelectedItem == null) { return; }
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;

            ProfileView v = new ProfileView(selectedItem.Tweet.user);
            v.ShowDialog();
        }

        private void ExitApplication()
        {
            statList.Clear();
            this.Close();
        }

        private void GoBackInHistory()
        {
            
            if (History.Count > 0)
            {
                HistoryItem prev = null;
                try
                {
                    HistoryItem current = History.Pop();
                    prev = History.Pop();
                }
                catch
                {
                    return;
                }
                switch (prev.Action)
                {
                    case Yedda.Twitter.ActionType.Conversation:
                        GetConversation(prev);
                        break;
                    case Yedda.Twitter.ActionType.Friends_Timeline:
                        ShowFriendsTimeLine();
                        if (ClientSettings.MergeMessages)
                        {
                            statList.SetSelectedMenu(TimeLinesMenuItem);
                        }
                        else
                        {
                            statList.SetSelectedMenu(FriendsTimeLineMenuItem);
                        }
                        break;
                    case Yedda.Twitter.ActionType.Replies:
                        statList.SetSelectedMenu(MessagesMenuItem);
                        ShowMessagesTimeLine();
                        break;
                    case Yedda.Twitter.ActionType.Search:
                        statList.SetSelectedMenu(SearchMenuItem);
                        ShowSearchResults(prev.Argument);
                        break;
                    case Yedda.Twitter.ActionType.User_Timeline:
                        statList.SetSelectedMenu(UserTimelineMenuItem);
                        SwitchToUserTimeLine(prev.Argument);
                        break;
                }
                if (prev.SelectedItemIndex >= 0)
                {
                    try
                    {
                        statList.SelectedItem = statList[prev.SelectedItemIndex];
                    }
                    catch (KeyNotFoundException) { }
                }
                if (prev.itemsOffset >= 0)
                {
                    statList.YOffset = prev.itemsOffset;
                }
            }
        }
        private void ShowFavorites()
        {
            ChangeCursor(Cursors.WaitCursor);
            
            SwitchToList("Favorites_TimeLine");
            HistoryItem i = new HistoryItem();
            i.Action = Yedda.Twitter.ActionType.Favorites;
            History.Push(i);
            statList.SetSelectedMenu(ViewFavoritesMenuItem);
            AddStatusesToList(Manager.GetFavorites());
            ChangeCursor(Cursors.Default);
        }
        private void ShowPublicTimeLine()
        {
            ChangeCursor(Cursors.WaitCursor);
            
            SwitchToList("Public_TimeLine");
            HistoryItem i = new HistoryItem();
            i.Action = Yedda.Twitter.ActionType.Public_Timeline;
            History.Push(i);
            statList.SetSelectedMenu(PublicMenuItem);
            AddStatusesToList(Manager.GetPublicTimeLine());
            ChangeCursor(Cursors.Default);
        }

        private void ShowFriendsTimeLine()
        {
            ChangeCursor(Cursors.WaitCursor);
            bool Redraw = statList.CurrentList() != "Friends_TimeLine";
            SwitchToList("Friends_TimeLine");
            History.Clear();
            HistoryItem i = new HistoryItem();
            i.Action = Yedda.Twitter.ActionType.Friends_Timeline;
            History.Push(i);
            if (Redraw)
            {
                statList.SetSelectedMenu(FriendsTimeLineMenuItem);
                AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Friends].ToArray());
            }
            Manager.RefreshFriendsTimeLine();
            ChangeCursor(Cursors.Default);
        }

        private void ShowMessagesTimeLine()
        {
            ChangeCursor(Cursors.WaitCursor);
            bool Redraw = statList.CurrentList() != "Messages_TimeLine";
            SwitchToList("Messages_TimeLine");
            History.Clear();
            HistoryItem i = new HistoryItem();
            i.Action = Yedda.Twitter.ActionType.Replies;
            History.Push(i);
            //if (Redraw)
            //{
            statList.SetSelectedMenu(MessagesMenuItem);
            AddStatusesToList(Manager.TimeLines[TimelineManagement.TimeLineType.Messages].ToArray());
            //}
            Manager.RefreshMessagesTimeLine();
            ChangeCursor(Cursors.Default);
        }

        private void ShowUserTimeLine()
        {
            UpdateHistoryPosition();
            ChangeCursor(Cursors.WaitCursor);
            FingerUI.StatusItem statItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (statItem == null) { return; }
            ShowUserID = statItem.Tweet.user.screen_name;
            CurrentlySelectedAccount = statItem.Tweet.Account;
            Yedda.Twitter Conn = GetMatchingConnection(CurrentlySelectedAccount);
            SwitchToList("@User_TimeLine");
            HistoryItem i = new HistoryItem();
            i.Action = Yedda.Twitter.ActionType.User_Timeline;
            i.Account = CurrentlySelectedAccount;
            i.Argument = ShowUserID;
            History.Push(i);
            AddStatusesToList(Manager.GetUserTimeLine(Conn, ShowUserID));
            ChangeCursor(Cursors.Default);

            return;
        }

        private void GetConversation()
        {
            GetConversation(null);
        }
        private void GetConversation(HistoryItem history)
        {
            UpdateHistoryPosition();
            HistoryItem i = new HistoryItem();
            Library.status lastStatus;
            Yedda.Twitter Conn;

            if (history == null)
            {
                if (statList.SelectedItem == null) { return; }
                FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
                if (string.IsNullOrEmpty(selectedItem.Tweet.in_reply_to_status_id)) { return; }
                Conn = GetMatchingConnection(selectedItem.Tweet.Account);
                lastStatus = selectedItem.Tweet;

                i.Account = selectedItem.Tweet.Account;
                i.Action = Yedda.Twitter.ActionType.Conversation;
                i.Argument = lastStatus.id;
            }
            else
            {
                i = history;
                Conn = GetMatchingConnection(history.Account);
                try
                {
                    lastStatus = Library.status.DeserializeSingle(Conn.ShowSingleStatus(i.Argument), i.Account);
                }
                catch
                {
                    return;
                }
            }
            ChangeCursor(Cursors.WaitCursor);

            //List<Library.status> Conversation = GetConversationFROMTHEFUTURE(lastStatus);
            List<Library.status> Conversation = new List<PockeTwit.Library.status>();
            History.Push(i);

            while (!string.IsNullOrEmpty(lastStatus.in_reply_to_status_id))
            {
                Conversation.Add(lastStatus);
                try
                {
                    lastStatus = Library.status.DeserializeSingle(Conn.ShowSingleStatus(lastStatus.in_reply_to_status_id), Conn.AccountInfo);
                }
                catch
                {
                    lastStatus = null;
                    break;
                }
            }
            if (lastStatus != null)
            {
                Conversation.Add(lastStatus);
            }
            statList.SwitchTolist("Conversation");
            statList.ClearVisible();
            AddStatusesToList(Conversation.ToArray());
            ChangeCursor(Cursors.Default);
        }

        private List<PockeTwit.Library.status> GetConversationFROMTHEFUTURE(PockeTwit.Library.status lastStatus)
        {
            Yedda.Twitter Conn = GetMatchingConnection(lastStatus.Account);
            Library.status[] SearchResults = Manager.SearchTwitter(Conn, "@" + lastStatus.user.screen_name);
            List<Library.status> Results = new List<PockeTwit.Library.status>();
            foreach (Library.status s in SearchResults)
            {
                if (s.in_reply_to_status_id == lastStatus.id)
                {
                    Results.Add(s);
                }
            }

            if (Results.Count == 1)
            {
                Results.AddRange(GetConversationFROMTHEFUTURE(Results[0]));
            }
            return Results;
        }


        private void Quote()
        {
            if (statList.SelectedItem == null) { return; }
            FingerUI.StatusItem selectedItem = (FingerUI.StatusItem)statList.SelectedItem;
            string quote = "RT @" + selectedItem.Tweet.user.screen_name + ": \"" + selectedItem.Tweet.text + "\"";
            SetStatus(quote, selectedItem.Tweet.id);
        }

        void statusList_SelectedItemChanged(object sender, EventArgs e)
        {
            FingerUI.StatusItem statItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (statItem == null) { return; }
            CurrentlySelectedAccount = statItem.Tweet.Account;
            SetConnectedMenus(GetMatchingConnection(CurrentlySelectedAccount), statItem);
            UpdateRightMenu();
            UpdateHistoryPosition();
            int clickedNumber = statItem.Index + 1;
            if (statList.CurrentList() == "Messages_TimeLine")
            {
                if (clickedNumber <= displayedNewMessages)
                {
                    displayedNewMessages = displayedNewMessages - (displayedNewMessages - -statItem.Index);
                    MessagesMenuItem.Text = "Messages" + newItemsText(displayedNewMessages);
                }
            }
            else if (statList.CurrentList() == "Friends_TimeLine")
            {
                if (clickedNumber <= displayedNewUpdates)
                {
                    displayedNewUpdates = displayedNewUpdates - (displayedNewUpdates - statItem.Index);
                    FriendsTimeLineMenuItem.Text = "Friends Timeline" + newItemsText(displayedNewUpdates);
                }
            }
            setCaption(newItemsText(displayedNewUpdates) + " " + newItemsText(displayedNewMessages));
            SetLeftMenu();
        }

        private void UpdateHistoryPosition()
        {
            HistoryItem i = History.Peek();
            i.SelectedItemIndex = statList.SelectedIndex;
            i.itemsOffset = statList.YOffset;
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
                if (Yedda.PictureServiceFactory.Instance.FetchServiceAvailable(TextClicked))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Yedda.IPictureService p = Yedda.PictureServiceFactory.Instance.LocateFetchService(TextClicked);
                    p.FetchPicture(TextClicked);
                    p.DownloadFinish += new Yedda.DownloadFinishEventHandler(p_DownloadFinish);
                    p.ErrorOccured += new Yedda.ErrorOccuredEventHandler(p_ErrorOccured);
                    return;
                }
                

                System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
                if (ClientSettings.UseSkweezer)
                {
                    pi.FileName = Yedda.Skweezer.GetSkweezerURL(TextClicked);
                }
                else
                {
                    pi.FileName = TextClicked;
                }
                try
                {
                    pi.UseShellExecute = true;
                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
                }
                catch
                {
                    MessageBox.Show("There is no default web browser defined for the OS.");
                }
            }
            else if (TextClicked.StartsWith("#"))
            {
                ShowSearchResults("q=" + System.Web.HttpUtility.UrlEncode(TextClicked));
            }
            else if (TextClicked.StartsWith("@"))
            {
                SwitchToUserTimeLine(TextClicked);
            }
        }

        void p_ErrorOccured(object sender, Yedda.PictureServiceEventArgs eventArgs)
        {
            if (InvokeRequired)
            {
                delPictureDone d = new delPictureDone(p_ErrorOccured);
                this.Invoke(d, sender, eventArgs);
            }
            else
            {
                Yedda.IPictureService p = (Yedda.IPictureService)sender;
                p.DownloadFinish -= new Yedda.DownloadFinishEventHandler(p_DownloadFinish);
                p.ErrorOccured -= new Yedda.ErrorOccuredEventHandler(p_ErrorOccured);
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Unable to fetch picture.  You may want to try again.");
            }
        }

        delegate void delPictureDone(object sender, Yedda.PictureServiceEventArgs eventArgs);
        void p_DownloadFinish(object sender, Yedda.PictureServiceEventArgs eventArgs)
        {
            if (InvokeRequired)
            {
                delPictureDone d = new delPictureDone(p_DownloadFinish);
                this.Invoke(d, sender, eventArgs);
            }
            else
            {


                Yedda.IPictureService p = (Yedda.IPictureService)sender;
                p.DownloadFinish -= new Yedda.DownloadFinishEventHandler(p_DownloadFinish);
                p.ErrorOccured -= new Yedda.ErrorOccuredEventHandler(p_ErrorOccured);

                Cursor.Current = Cursors.Default;

                ImagePreview ip = new ImagePreview(eventArgs.ReturnMessage, eventArgs.PictureFileName);
                ip.ShowDialog();
            }
        }

        private void SwitchToUserTimeLine(string TextClicked)
        {
            UpdateHistoryPosition();
            ShowUserID = TextClicked.Replace("@", "");
            FingerUI.StatusItem statItem = (FingerUI.StatusItem)statList.SelectedItem;
            if (statItem == null) { return; }
            ChangeCursor(Cursors.WaitCursor);
            HistoryItem i = new HistoryItem();
            i.Argument = ShowUserID;
            i.Account = statItem.Tweet.Account;
            i.Action = Yedda.Twitter.ActionType.User_Timeline;
            History.Push(i);
            CurrentlySelectedAccount = statItem.Tweet.Account;
            Yedda.Twitter Conn = GetMatchingConnection(CurrentlySelectedAccount);
            SwitchToList("@User_TimeLine");
            AddStatusesToList(Manager.GetUserTimeLine(Conn, ShowUserID));
            ChangeCursor(Cursors.Default);
            return;
        }

        private void SwitchToDone()
        {

            //lblLoading.Visible = false;
            //lblTitle.Visible = false;

            GlobalEventHandler.setPid();

            if (!StartBackground)
            {
                statList.Visible = true;
            }
            statList.BringToFront();
            SwitchToList("Friends_TimeLine");
            IsLoaded = true;
            lblLoading.Text = "Please wait... re-rendering to fit orientation.";
            lblTitle.Text = "PockeTwit";
            
            StartBackground = false;
        }

        private void SwitchToList(string ListName)
        {
            if (statList.CurrentList() != ListName)
            {
                statList.SwitchTolist(ListName);
            }
            SetLeftMenu();
        }

        private void timerStartup_Tick(object sender, EventArgs e)
        {
            if (StartBackground) { this.Hide(); }
            CreateLeftMenu();
            CreateRightMenu();
            timerStartup.Enabled = false;
            timerStartup.Tick -= new EventHandler(timerStartup_Tick);
            if (!SetEverythingUp())
            {
                if (Notifyer != null) { Notifyer.ShutDown(); }
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

            ShowSearchResults(SearchString);
        }

        private void ShowSearchResults(string SearchString)
        {
            UpdateHistoryPosition();
            ChangeCursor(Cursors.WaitCursor);
            statList.SetSelectedMenu(SearchMenuItem);
            HistoryItem i = new HistoryItem();
            i.Action = Yedda.Twitter.ActionType.Search;
            i.Argument = SearchString;
            History.Push(i);

            Yedda.Twitter Conn = GetMatchingConnection(CurrentlySelectedAccount);
            SwitchToList("Search_TimeLine");
            this.statList.ClearVisible();
            AddStatusesToList(Manager.SearchTwitter(Conn, SearchString));
            ChangeCursor(Cursors.Default);
        }

        void UpdateChecker_UpdateFound(UpgradeChecker.UpgradeInfo Info)
        {
            UpgradeForm uf = new UpgradeForm();
            uf.NewVersion = Info;
            uf.ShowDialog();
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

        private bool isChangingingWindowState = false;

        protected override void OnActivated(EventArgs e)
        {
            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                inputPanel1.Enabled = false;
            }
            if (isChangingingWindowState) { return; }
            isChangingingWindowState = true;
            
            
            GlobalEventHandler.setPid();
            if (!IsLoaded)
            {
                isChangingingWindowState = false;
                return;
            }



            if (ClientSettings.IsMaximized)
            {
                SetWindowState(FormWindowState.Maximized);
            }
            else
            {
                SetWindowState(FormWindowState.Normal);
            }
            //How do we tell if it was launched in background and is not being activated by user?
            // This is occuring after the initial run/setup is complete.
            statList.Visible = true;
            if (DetectDevice.DeviceType == DeviceType.Standard)
            {
                statList.Focus();
            }
            isChangingingWindowState = false;
           
            SendToForground();

            this.Invalidate();
        }

        [System.Runtime.InteropServices.DllImport("coredll.dll")]
        static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_MINIMIZED = 6;

        void Minimize()
        {
            isChangingingWindowState = true;
            // The Taskbar must be enabled to be able to do a Smart Minimize
            statList.Visible = false;
            //this.FormBorderStyle = FormBorderStyle.FixedDialog;
            //statList.Visible = false;
            //this.WindowState = FormWindowState.Normal;
            //this.ControlBox = true;
            //this.MinimizeBox = true;
            //this.MaximizeBox = true;

            // Since there is no WindowState.Minimize, we have to P/Invoke ShowWindow
            /*
            statList.Clear();
             */
            ShowWindow(this.Handle, SW_MINIMIZED);
            isChangingingWindowState = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        [System.Runtime.InteropServices.DllImport("coredll.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        void SendToForground()
        {
            SetForegroundWindow(this.Handle);
        }

        protected override void OnClosed(EventArgs e)
        {
            Program.IgnoreDisposed = true;
            if (Manager != null)
            {
                Manager.ShutDown();
            }
            if (Notifyer != null)
            {
                Notifyer.ShutDown();
            }
            base.OnClosed(e);
        }

        #endregion�Methods�

        private void TweetList_LostFocus(object sender, EventArgs e)
        {
            if (!statList.Focused)
            {
                statList.Visible = false;
            }
        }

    }
}
