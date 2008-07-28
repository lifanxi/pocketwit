using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit
{
    public static class Following
    {
        private static List<Library.User> FollowedUsers = new List<PockeTwit.Library.User>();

        static Following()
        {
            Yedda.Twitter twitter = new Yedda.Twitter();
            string response = twitter.GetFriends(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML);
            XmlSerializer s = new XmlSerializer(typeof(Library.User[]));
            Library.User[] friends;
            if (string.IsNullOrEmpty(response))
            {
                friends = new PockeTwit.Library.User[0];
            }
            else
            {
                using (System.IO.StringReader r = new System.IO.StringReader(response))
                {
                    friends = (Library.User[])s.Deserialize(r);
                    FollowedUsers = new List<PockeTwit.Library.User>(friends);
                }
            }
        }

        public static bool IsFollowing(Library.User userToCheck)
        {
            bool bFound = false;
            foreach (Library.User User in FollowedUsers)
            {
                if (User.id == userToCheck.id)
                {
                    bFound = true;
                    break;
                }
            }
            return bFound;
        }
    }
}
