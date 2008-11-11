using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    public class TimelineManagement
    {
        #region Events
        public delegate void delFriendsUpdated(int count);
        public delegate void delMessagesUpdated(int count);
        public delegate void delBothUpdated(int Messagecount, int FreindsCount);
        public delegate void delProgress(int percentage, string Status);
        public delegate void delComplete();
        public delegate void delNullReturnedByAccount(Yedda.Twitter.Account t, Yedda.Twitter.ActionType Action);
        public event delFriendsUpdated FriendsUpdated;
        public event delMessagesUpdated MessagesUpdated;
        public event delProgress Progress;
        public event delComplete CompleteLoaded;
        public event delNullReturnedByAccount NoData;
        public event delNullReturnedByAccount ErrorCleared;

        #endregion
        public bool RunInBackground = true;
        
        public enum TimeLineType
        {
            Friends,
            Messages
        }
        public Dictionary<TimeLineType, TimeLine> TimeLines = new Dictionary<TimeLineType, TimeLine>();
        private Dictionary<Yedda.Twitter.Account, string> LastStatusID = new Dictionary<Yedda.Twitter.Account, string>();
        private Dictionary<Yedda.Twitter.Account, string> LastReplyID = new Dictionary<Yedda.Twitter.Account, string>();
        private Dictionary<Yedda.Twitter.Account, string> LastDirectID = new Dictionary<Yedda.Twitter.Account, string>();
        private System.Threading.Timer messagesTimerUpdate = new System.Threading.Timer(null, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        private System.Threading.Timer friendsTimerUpdate = new System.Threading.Timer(null, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        private List<Yedda.Twitter> TwitterConnections;
        private int HoldNewMessages = 0;
        private int HoldNewFriends = 0;
        public DateTime NextUpdate;

        public TimelineManagement()
        {
        }

        public void Startup(List<Yedda.Twitter> TwitterConnectionsToFollow)
        {
            Progress(0, "Starting");
            TimeLines.Add(TimeLineType.Friends, new TimeLine());
            TimeLines.Add(TimeLineType.Messages, new TimeLine());
            TwitterConnections = TwitterConnectionsToFollow;
            foreach (Yedda.Twitter t in TwitterConnections)
            {
                System.Diagnostics.Debug.WriteLine("Adding key: " + t.AccountInfo);
                LastStatusID.Add(t.AccountInfo, "");
                LastReplyID.Add(t.AccountInfo, "");
                LastDirectID.Add(t.AccountInfo, "");
            }
            Progress(0, "Loading Cache");
            LoadCachedtimeline(TimeLineType.Friends, "Friends");
            foreach (Library.status stat in TimeLines[TimeLineType.Friends])
            {
                LastStatusID[stat.Account] = stat.id;
            }

            LoadCachedtimeline(TimeLineType.Messages, "Messages");
            foreach (Library.status stat in TimeLines[TimeLineType.Messages])
            {
                if (stat.TypeofMessage == PockeTwit.Library.StatusTypes.Direct)
                {
                    LastDirectID[stat.Account] = stat.id;
                }
                else
                {
                    LastReplyID[stat.Account] = stat.id;
                }
            }

            if (TimeLines[TimeLineType.Friends].Count > 0)
            {
                CompleteLoaded();
            }
            else
            {
                Progress(0, "Fetching Friends TimeLine");
                GetFriendsTimeLine();
                Progress(0, "Fetching Messages TimeLine");
                GetMessagesTimeLine();
                CompleteLoaded();
            } 
            if (ClientSettings.UpdateInterval > 0)
            {
                friendsTimerUpdate_Tick(null);
                messagesTimerUpdate = new System.Threading.Timer(new System.Threading.TimerCallback(messagesTimerUpdate_Tick), null, ClientSettings.UpdateInterval, ClientSettings.UpdateInterval);
                friendsTimerUpdate = new System.Threading.Timer(new System.Threading.TimerCallback(friendsTimerUpdate_Tick), null, ClientSettings.UpdateInterval, ClientSettings.UpdateInterval);
                NextUpdate = DateTime.Now.Add(new TimeSpan(0,0,0,0,ClientSettings.UpdateInterval));
                System.Diagnostics.Debug.WriteLine("Next update in " + NextUpdate.ToString());
            }
        }

        void messagesTimerUpdate_Tick(object state)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(BackgroundMessagesUpdate));
            
            NextUpdate = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, ClientSettings.UpdateInterval));
            System.Diagnostics.Debug.WriteLine("Next update in " + NextUpdate.ToString());
        }
        void friendsTimerUpdate_Tick(object state)
        {
           System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(BackgroundFriendsUpdate));
            NextUpdate = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, ClientSettings.UpdateInterval));
            System.Diagnostics.Debug.WriteLine("Next update in " + NextUpdate.ToString());
        }
        private void BackgroundMessagesUpdate(object o)
        {
            GetMessagesTimeLine(true);
        }

        private void BackgroundFriendsUpdate(object o)
        {
            GetFriendsTimeLine(true);
        }

        public void RefreshFriendsTimeLine()
        {
            if (!GlobalEventHandler.FriendsUpdating)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GetFriendsTimeLine));
            }
         }

        public void RefreshMessagesTimeLine()
        {
            if (!GlobalEventHandler.MessagesUpdating)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GetMessagesTimeLine));
            }
        }

        private void LoadCachedtimeline(TimeLineType TimeType, string TimeLineName)
        {
            Library.status[] newstats = null;
            string cachePath = ClientSettings.AppPath + "\\" + TimeLineName + "Time.xml";
            if (System.IO.File.Exists(cachePath))
            {
                try
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(cachePath))
                    {
                        string s = r.ReadToEnd();
                        newstats = Library.status.Deserialize(s);
                    }
                }
                catch
                {
                    System.IO.File.Delete(cachePath);
                    MessageBox.Show("Error with cache. Clearing it.");
                }
            }
            TimeLine CachedLines = new TimeLine(newstats);
            TimeLines[TimeType] = CachedLines;
        }

        public Library.status[] SearchTwitter(Yedda.Twitter t, string SearchString)
        {
            string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Search, SearchString);
            if (string.IsNullOrEmpty(response))
            {
                NoData(t.AccountInfo, Yedda.Twitter.ActionType.Search);
                return null;
            }
            else
            {
                ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Search);
            }
            return Library.status.DeserializeFromAtom(response, t.AccountInfo);
        }

        public Library.status[] GetUserTimeLine(Yedda.Twitter t, string UserID)
        {
            string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Show, UserID);
            if (string.IsNullOrEmpty(response))
            {
                NoData(t.AccountInfo, Yedda.Twitter.ActionType.Show);
                return null;
            }
            else
            {
                ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Show);
            }
            return Library.status.Deserialize(response, t.AccountInfo);
        }
        private void GetMessagesTimeLine(object o)
        {
            GetMessagesTimeLine(true);
        }
        private void GetMessagesTimeLine()
        {
            GetMessagesTimeLine(true);
        }
        private void GetMessagesTimeLine(bool Notify)
        {
            GlobalEventHandler.NotifyTimeLineFetching(TimeLineType.Messages);
            List<Library.status> TempLine = new List<PockeTwit.Library.status>();
            lock(TwitterConnections){
                foreach (Yedda.Twitter t in TwitterConnections)
                {
                    if (t.AccountInfo.Enabled && t.AccountInfo.ServerURL.ServerType != Yedda.Twitter.TwitterServer.pingfm)
                    {
                        string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Replies);
                        if (!string.IsNullOrEmpty(response))
                        {
                            try
                            {
                                Library.status[] NewStats = Library.status.Deserialize(response, t.AccountInfo, PockeTwit.Library.StatusTypes.Reply);
                                TempLine.AddRange(NewStats);
                                if (NewStats.Length > 0)
                                {
                                    LastReplyID[t.AccountInfo] = NewStats[0].id;
                                }
                                ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Replies);
                            }
                            catch
                            {
                                NoData(t.AccountInfo, Yedda.Twitter.ActionType.Replies);
                            }
                        }
                        else
                        {
                            NoData(t.AccountInfo, Yedda.Twitter.ActionType.Replies);
                        }
                        ////I HATE DIRECT MESSAGES

                        if (t.DirectMessagesWork)
                        {
                            response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Direct_Messages);
                            if (!string.IsNullOrEmpty(response))
                            {
                                try
                                {
                                    Library.status[] NewStats = Library.status.FromDirectReplies(response, t.AccountInfo);
                                    TempLine.AddRange(NewStats);
                                    if (NewStats.Length > 0)
                                    {
                                        LastStatusID[t.AccountInfo] = NewStats[0].id;
                                    }
                                    ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Direct_Messages);
                                }
                                catch
                                {
                                    NoData(t.AccountInfo, Yedda.Twitter.ActionType.Direct_Messages);
                                }
                            }
                            else
                            {
                                NoData(t.AccountInfo, Yedda.Twitter.ActionType.Direct_Messages);
                            }
                        }
                    }
                }
            }
            int NewItems = TimeLines[TimeLineType.Messages].MergeIn(TempLine);
            if (MessagesUpdated != null && NewItems>0)
            {
                SaveStatuses(TimeLines[TimeLineType.Messages].ToArray(), "Messages");
                if (Notify)
                {
                    MessagesUpdated(NewItems);
                }
                else
                {
                    HoldNewMessages = NewItems;
                }
            }
            TempLine.Clear();
            TempLine.TrimExcess();
            GlobalEventHandler.NotifyTimeLineDone(TimeLineType.Messages);
            if (ClientSettings.UpdateInterval > 0)
            {
                messagesTimerUpdate.Change(ClientSettings.UpdateInterval, ClientSettings.UpdateInterval);
            }
        }
        private void GetFriendsTimeLine(object o)
        {
            GetFriendsTimeLine(true);
        }
        private void GetFriendsTimeLine()
        {
            GetFriendsTimeLine(true);
        }
        private void GetFriendsTimeLine(bool Notify)
        {
            GlobalEventHandler.NotifyTimeLineFetching(TimeLineType.Friends);
            List<Library.status> TempLine = new List<PockeTwit.Library.status>();
            foreach (Yedda.Twitter t in TwitterConnections)
            {
                if (t.AccountInfo.Enabled && t.AccountInfo.ServerURL.ServerType != Yedda.Twitter.TwitterServer.pingfm)
                {
                    string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Friends_Timeline);

                    if (!string.IsNullOrEmpty(response))
                    {
                        try
                        {
                            Library.status[] NewStats = Library.status.Deserialize(response, t.AccountInfo);
                            TempLine.AddRange(NewStats);
                            if (NewStats.Length > 0)
                            {
                                LastStatusID[t.AccountInfo] = NewStats[0].id;
                            }
                            ErrorCleared(t.AccountInfo, Yedda.Twitter.ActionType.Friends_Timeline);
                        }
                        catch
                        {
                            NoData(t.AccountInfo, Yedda.Twitter.ActionType.Friends_Timeline);
                        }
                    }
                    else
                    {
                        NoData(t.AccountInfo, Yedda.Twitter.ActionType.Friends_Timeline);
                    }
                }
            }
            int NewItems = 0;
            if (TempLine.Count > 0)
            {
                NewItems = TimeLines[TimeLineType.Friends].MergeIn(TempLine);
                SaveStatuses(TimeLines[TimeLineType.Friends].ToArray(), "Friends");
            }
            if (FriendsUpdated != null && NewItems>0)
            {
                if (Notify)
                {
                    FriendsUpdated(NewItems);
                }
                else
                {
                    HoldNewFriends = NewItems;
                }
            }
            TempLine.Clear();
            TempLine.TrimExcess();
            GlobalEventHandler.NotifyTimeLineDone(TimeLineType.Friends);
            if (ClientSettings.UpdateInterval > 0)
            {
                this.friendsTimerUpdate.Change(ClientSettings.UpdateInterval, ClientSettings.UpdateInterval);
            }
        }

        private void SaveStatuses(PockeTwit.Library.status[] statuses, string TimeLineName)
        {
            if (statuses.Length <= 20)
            {
                //No need to cache less than 20 tweets.  
                return;
            }

            try
            {
                string StatusString = Library.status.Serialize(statuses);
                using (System.IO.TextWriter w = new System.IO.StreamWriter(ClientSettings.AppPath + "\\" + TimeLineName + "Time.xml"))
                {
                    w.Write(StatusString);
                    w.Flush();
                    w.Close();  //Shouldn't need this in using, but I'm desperate   
                }
            }
            catch
            {
            }
        }

        private string FetchSpecificFromTwitter(Yedda.Twitter t, Yedda.Twitter.ActionType TimelineType)
        {
            return FetchSpecificFromTwitter(t, TimelineType, null);
        }
        private string FetchSpecificFromTwitter(Yedda.Twitter t, Yedda.Twitter.ActionType TimelineType, string AdditionalParameter)
        {
            string response = "";
            try
            {
                switch (TimelineType)
                {
                    case Yedda.Twitter.ActionType.Direct_Messages:
                        response = t.GetDirectTimeLineSince(LastDirectID[t.AccountInfo]);
                        break;
                    case Yedda.Twitter.ActionType.Friends_Timeline:
                        if (!t.BigTimeLines)
                        {
                            response = t.GetFriendsTimeline(Yedda.Twitter.OutputFormatType.XML);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(LastStatusID[t.AccountInfo]))
                            {
                                response = t.GetFriendsTimeLineMax(Yedda.Twitter.OutputFormatType.XML);
                            }
                            else
                            {
                                response = t.GetFriendsTimeLineSince(Yedda.Twitter.OutputFormatType.XML, LastStatusID[t.AccountInfo]);
                            }
                        }
                        break;
                    case Yedda.Twitter.ActionType.Public_Timeline:
                        response = t.GetPublicTimeline(Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Replies:
                        if (!t.BigTimeLines)
                        {
                            response = t.GetRepliesTimeLine(Yedda.Twitter.OutputFormatType.XML);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(LastReplyID[t.AccountInfo]))
                            {
                                response = t.GetRepliesTimeLine(Yedda.Twitter.OutputFormatType.XML);
                            }
                            else
                            {
                                response = t.GetRepliesTimeLineSince(Yedda.Twitter.OutputFormatType.XML, LastReplyID[t.AccountInfo]);
                            }
                        }
                        break;
                    case Yedda.Twitter.ActionType.User_Timeline:
                        response = t.GetUserTimeline(AdditionalParameter, Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Show:
                        response = t.GetUserTimeline(AdditionalParameter, Yedda.Twitter.OutputFormatType.XML);
                        break;
                    case Yedda.Twitter.ActionType.Favorites:
                        response = t.GetFavorites();
                        break;
                    case Yedda.Twitter.ActionType.Search:
                        response = t.SearchFor(AdditionalParameter);
                        break;
                }
            }
            catch (Exception ex)
            {
            }
            return response;
        }


        internal void UpdateImagesForUser(string User, string NewURL)
        {
            try
            {
                foreach (TimeLine t in this.TimeLines.Values)
                {
                    foreach (Library.status stat in t)
                    {
                        if (stat.user.screen_name == User)
                        {
                            if (string.IsNullOrEmpty(stat.user.profile_image_url))
                            {
                                stat.user.profile_image_url = NewURL;
                            }
                            else
                            {
                                Uri U1 = new Uri(stat.user.profile_image_url);
                                Uri U2 = new Uri(NewURL);
                                if (U1.Host == U2.Host)
                                {
                                    stat.user.profile_image_url = NewURL;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
