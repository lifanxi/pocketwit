using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit
{
    public static class Following
    {

		#region Fields (2) 

        private static List<Library.User> FollowedUsers = new List<PockeTwit.Library.User>();
        private static bool OnceLoaded = false;

		#endregion Fields 

		#region Constructors (1) 

        static Following()
        {
            GetCachedFollowers();

            //LoadFromTwitter();
            //GetFollowersFromTwitter();
        }

		#endregion Constructors 

		#region Methods (9) 


		// Public Methods (5) 

        public static void AddUser(Library.User userToAdd)
        {
            FollowedUsers.Add(userToAdd);
            SaveUsers();
        }

        public static bool IsFollowing(Library.User userToCheck)
        {
            bool bFound = false;
            foreach (Library.User User in FollowedUsers)
            {
                if (User.screen_name == userToCheck.screen_name)
                {
                    bFound = true;
                    break;
                }
            }
            return bFound;
        }

        public static void LoadFromTwitter()
        {
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetFollowersFromTwitter);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "FetchFollowers";
            t.Start();
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

        public static void StopFollowing(Library.User usertoStop)
        {
            foreach(Library.User User in FollowedUsers)
            {
                if(User.screen_name == usertoStop.screen_name)
                {
                    FollowedUsers.Remove(User);
                    break;
                }
            }
            SaveUsers();
        }



		// Private Methods (4) 

        private static void GetCachedFollowers()
        {
            if (System.IO.File.Exists(ClientSettings.AppPath + "\\Following" + ClientSettings.UserName + ClientSettings.Server.ToString() + ".xml"))
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(ClientSettings.AppPath + "\\Following"+ ClientSettings.UserName +ClientSettings.Server.ToString()+".xml"))
                {
                    string Followers = r.ReadToEnd();
                    InterpretUsers(Followers);
                }
            }
        }

        private static void GetFollowersFromTwitter()
        {
            Yedda.Twitter twitter = new Yedda.Twitter();
            twitter.CurrentServer = ClientSettings.Server;
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
            XmlSerializer s = new XmlSerializer(typeof(Library.User[]));
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(ClientSettings.AppPath + "\\Following" + ClientSettings.UserName + ClientSettings.Server.ToString() + ".xml"))
            {
                s.Serialize(w, FollowedUsers.ToArray());
            }
        }


		#endregion Methods 

    }
}
