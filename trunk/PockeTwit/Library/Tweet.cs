using System;

using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace PockeTwit.Library
{

    public enum StatusTypes
    {
        Normal,
        Reply,
        Direct,
        SearchResult
    }

    [Serializable]
    public class status : IComparable
    {
        [XmlIgnore]
        private static XmlSerializer statusSerializer = new XmlSerializer(typeof(Library.status[]));
        private static XmlSerializer singleSerializer = new XmlSerializer(typeof(Library.status));

        public StatusTypes TypeofMessage = StatusTypes.Normal;

		#region Properties (7) 
        [XmlIgnore]
        public bool Clipped = false;
        [XmlIgnore]
        public List<string> SplitLines { get; set; }
        [XmlIgnore]
        public List<FingerUI.StatusItem.Clickable> Clickables { get; set; }

        public string in_reply_to_status_id { get; set; }

        public string favorited { get; set; }
        [XmlIgnore]
        public string TimeStamp
        {
            get
            {
                TimeSpan Difference = DateTime.Now - createdAt;
                double Diff;
                string Span = "";
                if (Difference.TotalDays > 1)
                {
                    Diff = Math.Round(Difference.TotalDays);
                    if (Diff > 1) { Span = "days"; } else { Span = "day"; }
                }
                else if (Difference.TotalHours > 1)
                {
                    Diff = Math.Round(Difference.TotalHours);
                    if (Diff > 1) { Span = "hours"; } else { Span = "hour"; }
                }
                else
                {
                    Diff = Math.Round(Difference.TotalMinutes);
                    Span = "min";
                }
                return "about " + Diff.ToString() + " " + Span +  " ago.";
            }
        }
        private DateTime createdAt;
        private string _created_at;
        public string created_at 
        {
            get { return _created_at; }
            set
            {
                IFormatProvider format = new System.Globalization.CultureInfo(1033);
                _created_at = value;
                try
                {
                    createdAt = DateTime.ParseExact(created_at, "ddd MMM dd HH:mm:ss K yyyy", format, System.Globalization.DateTimeStyles.AssumeUniversal);
                }
                catch(Exception ex)
                {
                    try
                    {
                        createdAt = DateTime.ParseExact(created_at, "ddd MMM dd H:mm:ss K yyyy", format, System.Globalization.DateTimeStyles.AssumeUniversal);
                    }
                    catch (Exception exx)
                    {
                        try
                        {
                            createdAt = DateTime.Parse(created_at, format, System.Globalization.DateTimeStyles.AssumeUniversal);
                        }
                        catch
                        {
                            createdAt = new DateTime(2000, 1, 1);
                        }
                    }
                }
            }
        }
        public string id { get; set; }

        public string source { get; set; }
        //public bool truncated { get; set; }
        //public string in_reply_to_status_id { get; set; }
        public string in_reply_to_user_id { get; set; }
        public bool isDirect { get; set; }

        public string location { get; set; }
        public string text { get;set;}
        [XmlIgnore]
        public string DisplayText
        {
            get
            {
                if (ClientSettings.IncludeUserName)
                {
                    return this.user.screen_name + ": " + text;
                }
                else
                {
                    return text;
                }
            }
        }

        public User user { get; set; }

        private Yedda.Twitter.Account _Account;
        public Yedda.Twitter.Account Account {
            get
            {
                if (_Account == null)
                {
                    _Account = ClientSettings.DefaultAccount;
                }
                return _Account;
            }
            set
            {
                _Account = value;
            }
        }

		#endregion Properties 

		#region Methods (3) 


		// Public Methods (3) 

        public static status[] Deserialize(string response)
        {
            return Deserialize(response, null, StatusTypes.Normal);
        }
        public static status[] Deserialize(string response, Yedda.Twitter.Account Account)
        {
            return Deserialize(response, Account, StatusTypes.Normal);
        }
        public static status DeserializeSingle(string response, Yedda.Twitter.Account Account)
        {
            status s = null;
            if (Account == null || (Account.ServerURL.ServerType != Yedda.Twitter.TwitterServer.brightkite && Account.ServerURL.ServerType!= Yedda.Twitter.TwitterServer.pingfm))
            {
                using (System.IO.StringReader r = new System.IO.StringReader(response))
                {
                    s = (Library.status)singleSerializer.Deserialize(r);
                }
                if (s.text == null)
                {
                    throw new Exception("Unable to deserialize the response");
                }
            }
            return s;
        }
        public static status[] Deserialize(string response, Yedda.Twitter.Account Account, StatusTypes TypeOfMessage)
        {
            Library.status[] statuses = null;
            
            
            
            if (string.IsNullOrEmpty(response))
            {
                statuses = new status[0];
            }
            else
            {
                if (Account == null || Account.ServerURL.ServerType != Yedda.Twitter.TwitterServer.brightkite)
                {
                    using (System.IO.StringReader r = new System.IO.StringReader(response))
                    {
                        statuses = (Library.status[])statusSerializer.Deserialize(r);
                    }
                }
                else if (Account.ServerURL.ServerType == Yedda.Twitter.TwitterServer.brightkite)
                {
                    statuses = FromBrightKite(response);
                }
            }
            if (Account != null)
            {
                foreach (Library.status stat in statuses)
                {
                    stat.Account = Account;
                    stat.TypeofMessage = TypeOfMessage;
                }
            }
            return statuses;
        }


        public static status[] DeserializeFromAtom(string response)
        {
            return DeserializeFromAtom(response, null);
        }
        public static status[] DeserializeFromAtom(string response, Yedda.Twitter.Account Account)
        {
            List<status> resultList = new List<status>();
            
            XmlDocument results = new XmlDocument();
            
            results.LoadXml(response);
            XmlNamespaceManager nm = new XmlNamespaceManager(results.NameTable);
            nm.AddNamespace("google", "http://base.google.com/ns/1.0");
            nm.AddNamespace("openSearch", "http://a9.com/-/spec/opensearch/1.1/");
            nm.AddNamespace("s", "http://www.w3.org/2005/Atom");
            XmlNodeList entries = results.SelectNodes("//s:entry", nm);
            System.Diagnostics.Debug.WriteLine(entries.Count);
            foreach (XmlNode entry in entries)
            {
                status newStat = new status();
                newStat.text = entry.SelectSingleNode("s:title",nm).InnerText;
                newStat.id = entry.SelectSingleNode("s:id", nm).InnerText;
                newStat.created_at = entry.SelectSingleNode("s:published", nm).InnerText;
                string userName = entry.SelectSingleNode("s:author/s:name",nm).InnerText;
                newStat.created_at = entry.SelectSingleNode("s:published", nm).InnerText;
                string userscreenName = userName.Split(new char[]{' '})[0];
                newStat.user = new User();
                newStat.user.screen_name = userscreenName;
                newStat.user.profile_image_url = entry.SelectSingleNode("s:link[@type=\"image/png\"]", nm).Attributes["href"].Value;
                
                resultList.Add(newStat);
                
            }
            foreach (status stat in resultList)
            {
                stat.TypeofMessage = StatusTypes.SearchResult;
                stat.Account = Account;
            }
            return resultList.ToArray();
        }

        public static status[] FromDirectReplies(string response, Yedda.Twitter.Account Account)
        {
            List<status> resultList = new List<status>();

            XmlDocument results = new XmlDocument();

            results.LoadXml(response);
            XmlNodeList entries = results.SelectNodes("//direct_message");
            foreach (XmlNode entry in entries)
            {
                status newStat = new status();
                newStat.text = entry.SelectSingleNode("text").InnerText;
                newStat.id = entry.SelectSingleNode("id").InnerText;
                newStat.created_at = entry.SelectSingleNode("created_at").InnerText;
                string userName = entry.SelectSingleNode("sender/screen_name").InnerText;
                newStat.user = new User();
                newStat.user.screen_name = userName;

                resultList.Add(newStat);

            }
            foreach (status stat in resultList)
            {
                stat.TypeofMessage = StatusTypes.Direct;
                stat.Account = Account;
            }
            return resultList.ToArray();
        }

        public static status[] FromBrightKite(string response)
        {
            List<status> resultList = new List<status>();

            XmlDocument results = new XmlDocument();

            results.LoadXml(response);
            XmlNodeList entries = results.SelectNodes("//note");
            foreach (XmlNode entry in entries)
            {
                status newStat = new status();
                newStat.text = entry.SelectSingleNode("body").InnerText;
                newStat.id = entry.SelectSingleNode("id").InnerText;
                newStat.created_at = entry.SelectSingleNode("created_at").InnerText;
                newStat.location = entry.SelectSingleNode("place/display_location").InnerText;
                string userName = entry.SelectSingleNode("creator/login").InnerText;
                string avURL = entry.SelectSingleNode("creator/small_avatar_url").InnerText;
                newStat.user = new User();
                newStat.user.screen_name = userName;
                newStat.user.profile_image_url = "http://brightkite.com/" + avURL;
                resultList.Add(newStat);

            }
            return resultList.ToArray();
        }

        public static string Serialize(status[] List)
        {
            if (List.Length == 0) { return null; }
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter w = new System.IO.StringWriter(sb))
            {
                statusSerializer.Serialize(w, List);
            }
            return sb.ToString();
        }


		#endregion Methods 

        public override bool Equals(object obj)
        {
            try
            {
                status otherStat = (status)obj;
                return (otherStat.id.Equals(this.id) && otherStat.Account.Equals(this.Account));
            }
            catch
            {
                return false;
            }
        }
    
        #region IComparable Members

        public int CompareTo(object obj)
        {
            status otherStat = (status)obj;
            return otherStat.createdAt.CompareTo(this.createdAt);
        }

        #endregion
    }

    [Serializable]
    public class User 
    {

		#region Properties (2) 

        //public string location { get; set; }
        //public string description { get; set; }
        public string profile_image_url { get; set; }

        private string _high_profile_image_url;
        public string high_profile_image_url
        {
            get
            {
                if (string.IsNullOrEmpty(_high_profile_image_url))
                {
                    if (!string.IsNullOrEmpty(profile_image_url))
                    {
                        if (profile_image_url.IndexOf("s3.amazonaws.com/twitter_production") > 0)
                        {
                            _high_profile_image_url = profile_image_url.Replace("_normal", "_bigger");
                        }
                        else
                        {
                            _high_profile_image_url = profile_image_url;
                        }
                    }
                    else
                    {
                        _high_profile_image_url = "";
                    }
                }
                return _high_profile_image_url;
            }
        }
        //public string id { get; set; }
        //public string name { get; set; }
        public string screen_name { get; set; }

		#endregion Properties 

		#region Methods (1) 


		// Public Methods (1) 

        //public string url { get; set; }
        //[XmlElement("protected")]
        //public bool is_protected { get; set; }
        //public int followers_count { get; set; }
        public static User FromId(string ID, Yedda.Twitter.Account Account)
        {
            Yedda.Twitter t = new Yedda.Twitter();
            t.AccountInfo = Account;
            string Response = null;
            try
            {
                Response = t.Show(ID, Yedda.Twitter.OutputFormatType.XML);
            }
            catch
            {
                User toReturn = new User();
                toReturn.screen_name = "PockeTwitUnknownUser";
                return toReturn;
            }

            try
            {
                XmlSerializer s = new XmlSerializer(typeof(User));
                if (string.IsNullOrEmpty(Response))
                {
                    User toReturn = new User();
                    toReturn.screen_name = "PockeTwitUnknownUser";
                    return toReturn;
                }
                using (System.IO.StringReader r = new System.IO.StringReader(Response))
                {
                    return (User)s.Deserialize(r);
                }
            }
            catch
            {
                User toReturn = new User();
                toReturn.screen_name = "PockeTwitUnknownUser";
                return toReturn;
            }
        }


		#endregion Methods 

    }

}
