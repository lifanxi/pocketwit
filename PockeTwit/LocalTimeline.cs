using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit
{
    class LocalTimeline
    {
        private Library.status[] _TimeLine = new PockeTwit.Library.status[0];
        private Yedda.Twitter twitter = new Yedda.Twitter();
        
        public Library.status[] TimeLine
        {
            get
            {
                return _TimeLine;
            }
        }

        public void LoadFromCache()
        {
            string LocationPath = ClientSettings.AppPath + "\\" + ClientSettings.UserName + "FriendsTime.xml";
            string timestring = null;
            if (System.IO.File.Exists(LocationPath))
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(LocationPath))
                {
                    timestring = r.ReadToEnd();
                }
            }
            if (string.IsNullOrEmpty(timestring))
            {
                _TimeLine = new PockeTwit.Library.status[0];
            }
            else
            {
                _TimeLine = DeserializeTimeline(timestring);
            }
        }

        private Library.status[] DeserializeTimeline(string timestring)
        {
            XmlSerializer s = new XmlSerializer(typeof(Library.status[]));

            if (string.IsNullOrEmpty(timestring))
            {
                return new PockeTwit.Library.status[0];
            }
            else
            {
                using (System.IO.StringReader r = new System.IO.StringReader(timestring))
                {
                    return (Library.status[])s.Deserialize(r);
                }
            }
        }
        public void SaveToCache()
        {
            string LocationPath = ClientSettings.AppPath + "\\" + ClientSettings.UserName + "FriendsTime.xml";
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(LocationPath))
            {
                XmlSerializer s = new XmlSerializer(typeof(Library.status[]));

                s.Serialize(w, _TimeLine);
            }
        }

        public bool CheckForUpdates()
        {
            string response = null;
            if (_TimeLine.Length > 0)
            {
                response = twitter.GetFriendsTimelineSinceID(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML, _TimeLine[0].id, ClientSettings.CachedTweets.ToString());
            }
            else
            {
                response = twitter.GetFriendsTimelineCount(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML, ClientSettings.CachedTweets.ToString());
            }

            Library.status[] NewLine = null;

            if (!string.IsNullOrEmpty(response) && !string.IsNullOrEmpty(response.Trim()))
            {
                NewLine = DeserializeTimeline(response);
                if (NewLine.Length > 0)
                {
                    int ListLenght;
                    ListLenght = (_TimeLine.Length + NewLine.Length > ClientSettings.CachedTweets) ? ClientSettings.CachedTweets : _TimeLine.Length + NewLine.Length;
                    Library.status[] MergeLine = new Library.status[ListLenght];
                    int i = 0;
                    foreach (Library.status stat in NewLine)
                    {
                        if (stat != null)
                        {
                            if (i >= ClientSettings.CachedTweets) 
                            {
                                break; 
                            }
                            MergeLine[i] = stat;
                            i++;
                        }
                    }
                    foreach (Library.status stat in _TimeLine)
                    {
                        if (stat != null)
                        {
                            if (i >= ClientSettings.CachedTweets) 
                            {
                                break; 
                            }
                            MergeLine[i] = stat;
                            i++;
                        }
                    }

                    _TimeLine = MergeLine;
                    SaveToCache();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
        }

    }
}
