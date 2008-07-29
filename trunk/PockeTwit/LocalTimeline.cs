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
            string LocationPath = ClientSettings.AppPath + "\\FriendsTime.xml";
            string timestring = null;
            using (System.IO.StreamReader r = new System.IO.StreamReader(LocationPath))
            {
                timestring = r.ReadToEnd();
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
            string LocationPath = ClientSettings.AppPath + "\\FriendsTime.xml";
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
                response = twitter.GetFriendsTimelineSinceID(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML, _TimeLine[0].id);
            }
            else
            {
                response = twitter.GetFriendsTimeline(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML);
            }

            Library.status[] NewLine = null;

            if (!string.IsNullOrEmpty(response) && !string.IsNullOrEmpty(response.Trim()))
            {
                NewLine = DeserializeTimeline(response);
                if (NewLine.Length > 0)
                {
                    Library.status[] MergeLine = new Library.status[_TimeLine.Length + NewLine.Length];
                    int i = 0;
                    foreach (Library.status stat in NewLine)
                    {
                        MergeLine[i] = stat;
                        i++;
                    }
                    foreach (Library.status stat in _TimeLine)
                    {
                        MergeLine[i] = stat;
                        i++;
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
