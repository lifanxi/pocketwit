using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit
{
    public static class Following
    {
        private static List<Library.User> FollowedUsers = new List<PockeTwit.Library.User>();

        private static bool OnceLoaded = false;

        static Following()
        {
            GetCachedFollowers();

            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetFollowersFromTwitter);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "FetchFollowers";
            t.Start();
            //GetFollowersFromTwitter();
        }

        public static void Reset()
        {
            if (OnceLoaded)
            {
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetFollowersFromTwitter);
                System.Threading.Thread t = new System.Threading.Thread(ts);
                t.Name = "FetchFollowers";
                t.Start();
            }
        }

        private static void GetCachedFollowers()
        {
            string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (System.IO.File.Exists(AppPath + "\\Following.xml"))
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(AppPath + "\\Following.xml"))
                {
                    string Followers = r.ReadToEnd();
                    InterpretUsers(Followers);
                }
            }
        }

        private static void GetFollowersFromTwitter()
        {
            Yedda.Twitter twitter = new Yedda.Twitter();
            try
            {
                string response = twitter.GetFriends(ClientSettings.UserName, ClientSettings.Password, Yedda.Twitter.OutputFormatType.XML);
                InterpretUsers(response);
                SaveUsers();
            }
            catch { }
            OnceLoaded = true;
        }

        private static void InterpretUsers(string response)
        {
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

        private static void SaveUsers()
        {
            string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            XmlSerializer s = new XmlSerializer(typeof(Library.User[]));
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(AppPath + "\\Following.xml"))
            {
                s.Serialize(w, FollowedUsers.ToArray());
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
