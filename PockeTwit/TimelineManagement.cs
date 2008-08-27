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
        public event delFriendsUpdated FriendsUpdated;
        public event delMessagesUpdated MessagesUpdated;
        public event delBothUpdated BothUpdated;
        public event delProgress Progress;
        public event delComplete CompleteLoaded;
        #endregion

        
        
        public enum TimeLineType
        {
            Friends,
            Messages
        }
        public Dictionary<TimeLineType, TimeLine> TimeLines = new Dictionary<TimeLineType, TimeLine>();
        private Dictionary<Yedda.Twitter, string> LastStatusID = new Dictionary<Yedda.Twitter, string>();
        private Dictionary<Yedda.Twitter, string> LastReplyID = new Dictionary<Yedda.Twitter, string>();
        private Dictionary<Yedda.Twitter, string> LastDirectID = new Dictionary<Yedda.Twitter, string>();
        private System.Threading.Timer timerUpdate;
        private List<Yedda.Twitter> TwitterConnections;
        private int HoldNewMessages = 0;
        private int HoldNewFriends = 0;


        public TimelineManagement(List<Yedda.Twitter> TwitterConnectionsToFollow)
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
                LastStatusID.Add(t, "");
                LastReplyID.Add(t, "");
                LastDirectID.Add(t, "");
            }
            Progress(0, "Loading Cache");
            LoadCachedtimeline();
            Progress(0, "Fetching Friends TimeLine");
            GetFriendsTimeLine();
            Progress(0, "Fetching Messages TimeLine");
            GetMessagesTimeLine();
            CompleteLoaded();
            if (ClientSettings.UpdateInterval > 0)
            {
                timerUpdate = new System.Threading.Timer(new System.Threading.TimerCallback(timerUpdate_Tick), null, ClientSettings.UpdateInterval, ClientSettings.UpdateInterval);
            }
        }

        void timerUpdate_Tick(object state)
        {
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(BackgroundUpdate);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "BackgroundUpdate";
            t.IsBackground = true;
            t.Start();
        }
    
        private void BackgroundUpdate()
        {
            GetFriendsTimeLine(false);
            GetMessagesTimeLine(false);
            if (HoldNewMessages > 0 && HoldNewFriends > 0)
            {
                if (BothUpdated != null)
                {
                    BothUpdated(HoldNewMessages, HoldNewFriends);
                }
            }
            else if (HoldNewMessages > 0)
            {
                if (MessagesUpdated != null)
                {
                    MessagesUpdated(HoldNewMessages);
                }
            }
            else if (HoldNewFriends > 0)
            {
                if (FriendsUpdated != null)
                {
                    FriendsUpdated(HoldNewFriends);
                }
            }
            HoldNewFriends = 0;
            HoldNewMessages = 0;
        }

        public void RefreshFriendsTimeLine()
        {
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetFriendsTimeLine);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "BackgroundUpdate";
            t.IsBackground = true;
            t.Start();
        }

        public void RefreshMessagesTimeLine()
        {
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetMessagesTimeLine);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "BackgroundUpdate";
            t.IsBackground = true;
            t.Start();
        }

        private void LoadCachedtimeline()
        {
            List<Library.status> Loaded = new List<PockeTwit.Library.status>();
            foreach (Yedda.Twitter t in TwitterConnections)
            {
                if (t.AccountInfo.Enabled)
                {
                    string cachePath = ClientSettings.AppPath + "\\" + t.AccountInfo.UserName + t.AccountInfo.ServerURL.Name + "FriendsTime.xml";
                    if (System.IO.File.Exists(cachePath))
                    {
                        using (System.IO.StreamReader r = new System.IO.StreamReader(cachePath))
                        {
                            string s = r.ReadToEnd();
                            Library.status[] newstats = Library.status.Deserialize(s);
                            Loaded.AddRange(newstats);
                            LastStatusID[t] = newstats[0].id;
                        }
                    }
                }
            }
            TimeLine CachedLines = new TimeLine(Loaded);
            TimeLines[TimeLineType.Friends] = CachedLines;
        }

        public Library.status[] SearchTwitter(Yedda.Twitter t, string SearchString)
        {
            string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Search, SearchString);
            if (string.IsNullOrEmpty(response))
            {
                return null;
            }
            return Library.status.DeserializeFromAtom(response, t.AccountInfo);
        }

        public Library.status[] GetUserTimeLine(Yedda.Twitter t, string UserID)
        {
            string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Show, UserID);
            if (string.IsNullOrEmpty(response))
            {
                return null;
            }
            return Library.status.Deserialize(response, t.AccountInfo);
        }

        private void GetMessagesTimeLine()
        {
            GetMessagesTimeLine(true);
        }
        private void GetMessagesTimeLine(bool Notify)
        {
            List<Library.status> TempLine = new List<PockeTwit.Library.status>();
            foreach (Yedda.Twitter t in TwitterConnections)
            {
                if (t.AccountInfo.Enabled)
                {
                    string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Replies);
                    if (!string.IsNullOrEmpty(response))
                    {
                        Library.status[] NewStats = Library.status.Deserialize(response, t.AccountInfo);
                        TempLine.AddRange(NewStats);
                        if (NewStats.Length > 0)
                        {
                            LastReplyID[t] = NewStats[0].id;
                        }
                    }
                    ////I HATE DIRECT MESSAGES
                    /*
                    if (t.DirectMessagesWork)
                    {
                        response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Direct_Messages);
                        if (!string.IsNullOrEmpty(response))
                        {
                            Library.status[] NewStats = Library.status.Deserialize(response, t.AccountInfo);
                            TempLine.AddRange(NewStats);
                            if (NewStats.Length > 0)
                            {
                                LastStatusID[t] = NewStats[0].id;
                            }
                        }
                    }
                     */
                }
            }
            int NewItems = TimeLines[TimeLineType.Messages].MergeIn(TempLine);
            if (MessagesUpdated != null && NewItems>0)
            {
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
        }

        private void GetFriendsTimeLine()
        {
            GetFriendsTimeLine(true);
        }
        private void GetFriendsTimeLine(bool Notify)
        {
            List<Library.status> TempLine = new List<PockeTwit.Library.status>();
            foreach (Yedda.Twitter t in TwitterConnections)
            {
                if (t.AccountInfo.Enabled)
                {
                    string response = FetchSpecificFromTwitter(t, Yedda.Twitter.ActionType.Friends_Timeline);

                    if (!string.IsNullOrEmpty(response))
                    {
                        Library.status[] NewStats = Library.status.Deserialize(response, t.AccountInfo);
                        TempLine.AddRange(NewStats);
                        if (NewStats.Length > 0)
                        {
                            SaveStatuses(NewStats, t);
                            LastStatusID[t] = NewStats[0].id;
                        }
                    }
                }
            }
            int NewItems = TimeLines[TimeLineType.Friends].MergeIn(TempLine);
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
        }

        private void SaveStatuses(PockeTwit.Library.status[] statuses, Yedda.Twitter t)
        {
            if (statuses.Length <= 20)
            {
                //No need to cache less than 20 tweets.  
                return;
            }
            string StatusString = Library.status.Serialize(statuses);

            using (System.IO.TextWriter w = new System.IO.StreamWriter(ClientSettings.AppPath + "\\" + t.AccountInfo.UserName + t.AccountInfo.ServerURL.Name + "FriendsTime.xml"))
            {
                w.Write(StatusString);
                w.Flush();
                w.Close();  //Shouldn't need this in using, but I'm desperate   
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
                        response = t.GetDirectTimeLineSince(LastDirectID[t]);
                        break;
                    case Yedda.Twitter.ActionType.Friends_Timeline:
                        if (!t.BigTimeLines)
                        {
                            response = t.GetFriendsTimeline(Yedda.Twitter.OutputFormatType.XML);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(LastStatusID[t]))
                            {
                                response = t.GetFriendsTimeLineMax(Yedda.Twitter.OutputFormatType.XML);
                            }
                            else
                            {
                                response = t.GetFriendsTimeLineSince(Yedda.Twitter.OutputFormatType.XML, LastStatusID[t]);
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
                            if (string.IsNullOrEmpty(LastReplyID[t]))
                            {
                                response = t.GetRepliesTimeLine(Yedda.Twitter.OutputFormatType.XML);
                            }
                            else
                            {
                                response = t.GetRepliesTimeLineSince(Yedda.Twitter.OutputFormatType.XML, LastReplyID[t]);
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
                //statList.Warning = ex.Message;
                //CurrentlyConnected = false;
            }
            return response;
        }

    }
}
