using System;

using System.Data.SQLite;

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit.SpecialTimelines
{
    [Serializable]
    public static class SpecialTimeLinesRepository
    {
        public enum TimeLineType
        {
            UserGroup
        }

        private static Dictionary<string, UserGroupTimeLine> _Items = new Dictionary<string, UserGroupTimeLine>();

        public static UserGroupTimeLine[] GetList()
        {
            List<UserGroupTimeLine> s = new List<UserGroupTimeLine>();
            lock (_Items)
            {
                foreach (UserGroupTimeLine item in _Items.Values)
                {
                    s.Add(item);
                }
            }

            return s.ToArray();
        }
        public static void Add(UserGroupTimeLine newLine)
        {
            lock (_Items)
            {
                if (!_Items.ContainsKey(newLine.name))
                {
                    _Items.Add(newLine.name, newLine);
                    NotificationHandler.AddSpecialTimeLineNotifications(newLine);
                }
            }
        }
        public static void Remove(UserGroupTimeLine oldLine)
        {
            lock (_Items)
            {
                if(_Items.ContainsKey(oldLine.name))
                {
                    _Items.Remove(oldLine.name);
                    NotificationHandler.RemoveSpecialTimeLineNotifications(oldLine);
                }
            }
            using (SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (SQLiteTransaction t = conn.BeginTransaction())
                {
                    using (SQLiteCommand comm = new SQLiteCommand(conn))
                    {
                        comm.CommandText = "DELETE FROM usersInGroups WHERE groupname=@groupname;";
                        comm.Parameters.Add(new SQLiteParameter("@groupname", oldLine.name));
                        comm.ExecuteNonQuery();

                        comm.CommandText = "DELETE FROM groups WHERE groupname=@groupname;";
                        comm.ExecuteNonQuery();
                    }
                    t.Commit();
                }
            }
        }
        public static void Clear()
        {
            lock (_Items)
            {
                _Items.Clear();
            }
            using (SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (SQLiteTransaction t = conn.BeginTransaction())
                {
                    using (SQLiteCommand comm = new SQLiteCommand(conn))
                    {
                        comm.CommandText = "DELETE FROM usersInGroups;";
                        comm.ExecuteNonQuery();

                        comm.CommandText = "DELETE FROM groups;";
                        comm.ExecuteNonQuery();
                    }
                    t.Commit();
                }
            }

        }

        public static void Load()
        {
            
            using (SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (SQLiteCommand comm = new SQLiteCommand(conn))
                {
                    comm.CommandText = "SELECT groupname, userid, exclusive, users.screenname FROM usersInGroups INNER JOIN users ON usersInGroups.userid = users.id";
                    using (SQLiteDataReader r = comm.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string groupName = r.GetString(0);
                            string userID = r.GetString(1);
                            bool exclusive = r.GetBoolean(2);
                            string screenName = r.GetString(3);
                            UserGroupTimeLine thisLine = new UserGroupTimeLine();
                            if (_Items.ContainsKey(groupName))
                            {
                                thisLine = _Items[groupName];
                            }
                            else
                            {
                                thisLine.name = groupName;
                                Add(thisLine);
                            }
                            thisLine.AddItem(userID,screenName, exclusive);
                        }
                    }
                }
            }
        }
        public static void Save()
        {
            
            if (_Items.Count > 0)
            {
                using (SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
                {
                    lock (_Items)
                    {
                        conn.Open();
                        using (SQLiteTransaction t = conn.BeginTransaction())
                        {
                            foreach (UserGroupTimeLine group in _Items.Values)
                            {
                                using (SQLiteCommand comm = new SQLiteCommand(conn))
                                {
                                    comm.CommandText = "INSERT INTO groups (groupname) VALUES (@name);";
                                    comm.Parameters.Add(new SQLiteParameter("@name", group.name));

                                    comm.ExecuteNonQuery();

                                    comm.CommandText = "DELETE FROM usersInGroups WHERE groupname=@groupname";
                                    comm.Parameters.Add(new SQLiteParameter("@groupname", group.name));
                                    comm.ExecuteNonQuery();
                                    comm.Parameters.Clear();

                                    foreach (UserGroupTimeLine.groupTerm groupItem in group.Terms)
                                    {
                                        comm.Parameters.Clear();
                                        comm.CommandText = "INSERT INTO usersInGroups (id, groupname, userid, exclusive) VALUES (@pairid, @name, @userid, @exclusive)";
                                        comm.Parameters.Add(new SQLiteParameter("@pairid", group.name + groupItem.Term));
                                        comm.Parameters.Add(new SQLiteParameter("@name", group.name));
                                        comm.Parameters.Add(new SQLiteParameter("@userid", groupItem.Term));
                                        comm.Parameters.Add(new SQLiteParameter("@exclusive", groupItem.Exclusive));
                                        comm.ExecuteNonQuery();

                                    }
                                }
                            }
                            t.Commit();
                        }
                    }
                }
            }
        }

        public static bool UserIsExcluded(string term)
        {
            lock (_Items)
            {
                foreach (UserGroupTimeLine t in _Items.Values)
                {
                    foreach (UserGroupTimeLine.groupTerm groupterm in t.Terms)
                    {
                        if (groupterm.Term == term && groupterm.Exclusive)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal static UserGroupTimeLine GetFromName(string ListName)
        {
            UserGroupTimeLine ret = null;
            foreach (UserGroupTimeLine t in GetList())
            {
                if (t.ListName == ListName)
                {
                    ret = t;
                }
            }
            return ret;
        }


        public static void Export()
        {
            string FileName = ClientSettings.CacheDir + "\\GroupBackup.xml";
            lock (_Items)
            {
                List<UserGroupTimeLine> l = new List<UserGroupTimeLine>();
                foreach (var item in _Items.Values)
                {
                    l.Add(item);
                }
                System.Xml.Serialization.XmlSerializer s = new XmlSerializer(typeof(UserGroupTimeLine[]));
                StringBuilder b = new StringBuilder();
                using (System.IO.StreamWriter w = new StreamWriter(FileName))
                {
                    s.Serialize(w, l.ToArray());
                }
            }
        }

        public static void Import()
        {
            string FileName = ClientSettings.CacheDir + "\\GroupBackup.xml";
            if (!System.IO.File.Exists(FileName)) return;
            UserGroupTimeLine[] Input;
            System.Xml.Serialization.XmlSerializer s = new XmlSerializer(typeof (UserGroupTimeLine[]));

            using (System.IO.StreamReader r = new StreamReader(FileName))
            {
                Input = (UserGroupTimeLine[]) s.Deserialize(r);
            }

            lock (_Items)
            {
                _Items.Clear();
                foreach (var line in Input)
                {
                    _Items.Add(line.name, line);
                }
            }
            Save();
        }
    }
}