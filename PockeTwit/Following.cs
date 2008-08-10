using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit
{
    public  class Following
    {

        public Yedda.Twitter TwitterConnection { get; set; }

		#region Fields (2) 

        private  List<Library.User> FollowedUsers = new List<PockeTwit.Library.User>();
        private  bool OnceLoaded = false;

		#endregion Fields 

		#region Constructors (1) 

        public Following(Yedda.Twitter Connection )
        {
            TwitterConnection = Connection;
            GetCachedFollowers();

            //LoadFromTwitter();
            //GetFollowersFromTwitter();
        }

		#endregion Constructors 

		#region Methods (9) 


		// Public Methods (5) 

        public void AddUser(Library.User userToAdd)
        {
            FollowedUsers.Add(userToAdd);
            SaveUsers();
        }

        public bool IsFollowing(Library.User userToCheck)
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

        public  void LoadFromTwitter()
        {
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetFollowersFromTwitter);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "FetchFollowers";
            t.Start();
        }

        public  void Reset()
        {
            if (OnceLoaded)
            {
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetFollowersFromTwitter);
                System.Threading.Thread t = new System.Threading.Thread(ts);
                t.Name = "FetchFollowers";
                t.Start();
            }
        }

        public void StopFollowing(Library.User usertoStop)
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

        private  void GetCachedFollowers()
        {
            string location = ClientSettings.AppPath + "\\Following" + TwitterConnection.userName + TwitterConnection.CurrentServer.ToString() + ".xml";
            if (System.IO.File.Exists(location))
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(location))
                {
                    string Followers = r.ReadToEnd();
                    InterpretUsers(Followers);
                }
            }
        }

        private  void GetFollowersFromTwitter()
        {
            try
            {
                string response = this.TwitterConnection.GetFriends(Yedda.Twitter.OutputFormatType.XML);
                InterpretUsers(response);
                SaveUsers();
            }
            catch { }
            OnceLoaded = true;
        }

        private  void InterpretUsers(string response)
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

        private  void SaveUsers()
        {
            string location = ClientSettings.AppPath + "\\Following" + TwitterConnection.userName + TwitterConnection.CurrentServer.ToString() + ".xml";
            
            XmlSerializer s = new XmlSerializer(typeof(Library.User[]));
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(location))
            {
                s.Serialize(w, FollowedUsers.ToArray());
            }
        }


		#endregion Methods 

    }
}
