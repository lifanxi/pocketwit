using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace PockeTwit.Library
{
    [Serializable]
    public class status
    {
        public string created_at { get; set; }
        public long id { get; set; }
        public string text { get; set; }
        public string source { get; set; }
        public bool truncated { get; set; }

        public string in_reply_to_status_id { get; set; }
        public string in_reply_to_user_id { get; set; }

        public string favorited { get; set; }

        public User user { get; set; }
    }

    [Serializable]
    public class User
    {
        public long id { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string profile_image_url { get; set; }
        public string url { get; set; }
        [XmlElement("protected")]
        public bool is_protected { get; set; }
        public int followers_count { get; set; }
    }

   
}
