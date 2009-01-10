using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit
{
    public  class Following
    {

        public Yedda.Twitter TwitterConnection { get; set; }
        public delegate void delFollowers(Yedda.Twitter ConnectionDone);
        public event delFollowers FollowersDone;

		#region Fields (2) 

        private  List<Library.User> FollowedUsers = new List<PockeTwit.Library.User>();
        private  bool OnceLoaded = false;

		#endregion Fields 

		#region Constructors (1) 

        public Following(Yedda.Twitter Connection )
        {
            TwitterConnection = Connection;
            GetCachedFollowers();
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
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GetFollowersFromTwitter));
        }

        public  void Reset()
        {
            if (OnceLoaded)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GetFollowersFromTwitter));
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
            string location = ClientSettings.AppPath + "\\Following" + TwitterConnection.AccountInfo.UserName + TwitterConnection.AccountInfo.ServerURL.Name + ".xml";
            try
            {
                if (System.IO.File.Exists(location))
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(location))
                    {
                        string Followers = r.ReadToEnd();
                        InterpretUsers(Followers);
                    }
                }
            }
            catch
            {
                if (!string.IsNullOrEmpty(location))
                {
                    System.IO.File.Delete(location);
                }
            }
            
        }

        private  void GetFollowersFromTwitter(object o)
        {
            try
            {
                string response = this.TwitterConnection.GetFriends(Yedda.Twitter.OutputFormatType.XML);
                InterpretUsers(response);
                SaveUsers();
            }
            catch 
            {
            }
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
            string location = ClientSettings.AppPath + "\\Following" + TwitterConnection.AccountInfo.UserName + TwitterConnection.AccountInfo.ServerURL.Name + ".xml";
            
            XmlSerializer s = new XmlSerializer(typeof(Library.User[]));
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(location))
            {
                s.Serialize(w, FollowedUsers.ToArray());
            }
        }


		#endregion Methods 

    }
}
