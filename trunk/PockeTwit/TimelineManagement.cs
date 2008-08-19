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
        public event delFriendsUpdated FriendsUpdated;
        public event delMessagesUpdated MessagesUpdated;
        #endregion

        
        
        public enum TimeLineType
        {
            Friends,
            Messages
        }
        public Dictionary<TimeLineType, TimeLine> TimeLines = new Dictionary<TimeLineType, TimeLine>();
        private Dictionary<Yedda.Twitter, string> LastStatusID = new Dictionary<Yedda.Twitter, string>();
        private System.Threading.Timer timerUpdate;
        private List<Yedda.Twitter> TwitterConnections = new List<Yedda.Twitter>();
        


        public TimelineManagement(List<Yedda.Twitter> TwitterConnectionsToFollow)
        {
            TimeLines.Add(TimeLineType.Friends, new TimeLine());
            TimeLines.Add(TimeLineType.Messages, new TimeLine());
            TwitterConnections = TwitterConnectionsToFollow;
            foreach (Yedda.Twitter t in TwitterConnections)
            {
                LastStatusID.Add(t, "");
            }
            LoadCachedtimeline();
            GetFriendsTimeLine();
            GetMessagesTimeLine();
            timerUpdate = new System.Threading.Timer(new System.Threading.TimerCallback(timerUpdate_Tick), null, ClientSettings.UpdateInterval, ClientSettings.UpdateInterval);
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
            GetFriendsTimeLine();
            GetMessagesTimeLine();
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
                    }
                }
            }
            int NewItems = TimeLines[TimeLineType.Messages].MergeIn(new TimeLine(TempLine));
            if (MessagesUpdated != null && NewItems>0)
            {
                MessagesUpdated(NewItems);
            }
        }

        private void GetFriendsTimeLine()
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
            int NewItems = TimeLines[TimeLineType.Friends].MergeIn(new TimeLine(TempLine));
            if (FriendsUpdated != null && NewItems>0)
            {
                FriendsUpdated(NewItems);
            }
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
                        response = t.GetRepliesTimeLine(Yedda.Twitter.OutputFormatType.XML);
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
