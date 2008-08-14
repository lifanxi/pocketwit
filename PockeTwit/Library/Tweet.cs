using System;

using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace PockeTwit.Library
{
    
    [Serializable]
    public class status : IComparable
    {
        [XmlIgnore]
        private static XmlSerializer statusSerializer = new XmlSerializer(typeof(Library.status[]));


		#region Properties (7) 

        [XmlIgnore]
        public List<FingerUI.StatusItem.Clickable> Clickables { get; set; }

        public string favorited { get; set; }


        [XmlIgnore]
        private DateTime createdAt;

        private string _created_at;
        public string created_at 
        {
            get { return _created_at; }
            set
            {
                _created_at = value;
                try
                {
                    createdAt = DateTime.ParseExact(created_at, "ddd MMM dd H:mm:ss K yyyy", null);
                }
                catch
                {
                    try
                    {
                        createdAt = DateTime.Parse(created_at);
                    }
                    catch
                    {
                        createdAt = new DateTime(2000, 1, 1);
                    }
                }
            }
        }
        public string id { get; set; }

        //public string source { get; set; }
        //public bool truncated { get; set; }
        //public string in_reply_to_status_id { get; set; }
        public string in_reply_to_user_id { get; set; }

        [XmlIgnore]
        public List<string> SplitLines { get; set; }

        public string text { get; set; }

        public User user { get; set; }

        [XmlIgnore]
        public Yedda.Twitter.Account Account { get; set; }

		#endregion Properties 

		#region Methods (3) 


		// Public Methods (3) 

        public static status[] Deserialize(string response)
        {
            return Deserialize(response, null);
        }
        public static status[] Deserialize(string response, Yedda.Twitter.Account Account)
        {
            
            
            Library.status[] statuses;
            if (string.IsNullOrEmpty(response))
            {
                statuses = new status[0];
            }
            else
            {
                using (System.IO.StringReader r = new System.IO.StringReader(response))
                {
                    statuses = (Library.status[])statusSerializer.Deserialize(r);                    
                }
            }
            foreach (Library.status stat in statuses)
            {
                stat.Account = Account;
            }
            return statuses;
        }

        public static status[] DeserializeFromAtom(string response)
        {
            return Deserialize(response, null);
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
                
                
                resultList.Add(newStat);
                
            }
            foreach (status stat in resultList)
            {
                stat.Account = Account;
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
                return otherStat.id.Equals(this.id);
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


		#endregion Methods 

    }

}
