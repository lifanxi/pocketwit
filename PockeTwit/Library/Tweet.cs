using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace PockeTwit.Library
{
    [Serializable]
    public class status
    {
        //public string created_at { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        //public string source { get; set; }
        //public bool truncated { get; set; }

        //public string in_reply_to_status_id { get; set; }
        public string in_reply_to_user_id { get; set; }

        public string favorited { get; set; }

        public User user { get; set; }

        public List<string> SplitLines { get; set; }
        public List<FingerUI.StatusItem.Clickable> Clickables { get; set; }

        
        public static status[] Deserialize(string response)
        {
            
            XmlSerializer s = new XmlSerializer(typeof(Library.status[]));
            Library.status[] statuses;
            if (string.IsNullOrEmpty(response))
            {
                statuses = new status[0];
            }
            else
            {
                using (System.IO.StringReader r = new System.IO.StringReader(response))
                {
                        statuses = (Library.status[])s.Deserialize(r);
                    
                }
            }
            return statuses;
        }
        public static string Serialize(status[] List)
        {
            if (List.Length == 0) { return null; }
            XmlSerializer s = new XmlSerializer(typeof(status[]));
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter w = new System.IO.StringWriter(sb))
            {
                s.Serialize(w, List);
            }
            return sb.ToString();
        }

    }

    [Serializable]
    public class User 
    {
        public string id { get; set; }
        //public string name { get; set; }
        public string screen_name { get; set; }
        //public string location { get; set; }
        //public string description { get; set; }
        public string profile_image_url { get; set; }
        //public string url { get; set; }
        //[XmlElement("protected")]
        //public bool is_protected { get; set; }
        //public int followers_count { get; set; }

        public static User FromId(string ID)
        {
            Yedda.Twitter Twitter = new Yedda.Twitter();
            Twitter.CurrentServer = ClientSettings.Server;
            string Response = Twitter.Show(ClientSettings.UserName, ClientSettings.Password, ID, Yedda.Twitter.OutputFormatType.XML);

            XmlSerializer s = new XmlSerializer(typeof(User));
            if (string.IsNullOrEmpty(Response))
            {
                return null;
            }
            using (System.IO.StringReader r = new System.IO.StringReader(Response))
            {
                return (User)s.Deserialize(r);
            }
        }
        
    }

}
