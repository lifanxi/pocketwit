using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Xml.Serialization;
using LocalStorage;

namespace PockeTwit.SpecialTimelines
{
    [Serializable]
    public static class SpecialTimeLinesRepository
    {
        #region TimeLineType enum

        public enum TimeLineType
        {
            UserGroup,
            SavedSearch
        }

        #endregion

        private static readonly Dictionary<string, UserGroupTimeLine> Items =
            new Dictionary<string, UserGroupTimeLine>();

        public static UserGroupTimeLine[] GetList()
        {
            var s = new List<UserGroupTimeLine>();
            lock (Items)
            {
                foreach (var item in Items.Values)
                {
                    s.Add(item);
                }
            }

            return s.ToArray();
        }

        public static void Add(UserGroupTimeLine newLine)
        {
            lock (Items)
            {
                if (!Items.ContainsKey(newLine.name))
                {
                    Items.Add(newLine.name, newLine);
                    NotificationHandler.AddSpecialTimeLineNotifications(newLine);
                }
            }
        }

        public static void Remove(UserGroupTimeLine oldLine)
        {
            lock (Items)
            {
                if (Items.ContainsKey(oldLine.name))
                {
                    Items.Remove(oldLine.name);
                    NotificationHandler.RemoveSpecialTimeLineNotifications(oldLine);
                }
            }
            using (var conn = DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    using (var comm = new SQLiteCommand(conn))
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
            lock (Items)
            {
                Items.Clear();
            }
            using (var conn = DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (var t = conn.BeginTransaction())
                {
                    using (var comm = new SQLiteCommand(conn))
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
            using (var conn = DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (var comm = new SQLiteCommand(conn))
                {
                    comm.CommandText =
                        "SELECT groupname, userid, exclusive, users.screenname FROM usersInGroups INNER JOIN users ON usersInGroups.userid = users.id";
                    using (var r = comm.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var groupName = r.GetString(0);
                            var userID = r.GetString(1);
                            var exclusive = r.GetBoolean(2);
                            var screenName = r.GetString(3);
                            var thisLine = new UserGroupTimeLine();
                            if (Items.ContainsKey(groupName))
                            {
                                thisLine = Items[groupName];
                            }
                            else
                            {
                                thisLine.name = groupName;
                                Add(thisLine);
                            }
                            thisLine.AddItem(userID, screenName, exclusive);
                        }
                    }
                }
            }
        }

        public static void Save()
        {
            if (Items.Count <= 0) return;
            using (var conn = DataBaseUtility.GetConnection())
            {
                lock (Items)
                {
                    conn.Open();
                    using (var t = conn.BeginTransaction())
                    {
                        foreach (var group in Items.Values)
                        {
                            using (var comm = new SQLiteCommand(conn))
                            {
                                comm.CommandText = "INSERT INTO groups (groupname) VALUES (@name);";
                                comm.Parameters.Add(new SQLiteParameter("@name", group.name));

                                comm.ExecuteNonQuery();

                                comm.CommandText = "DELETE FROM usersInGroups WHERE groupname=@groupname";
                                comm.Parameters.Add(new SQLiteParameter("@groupname", group.name));
                                comm.ExecuteNonQuery();
                                comm.Parameters.Clear();

                                foreach (var groupItem in group.Terms)
                                {
                                    comm.Parameters.Clear();
                                    comm.CommandText =
                                        "INSERT INTO usersInGroups (id, groupname, userid, exclusive) VALUES (@pairid, @name, @userid, @exclusive)";
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

        public static bool UserIsExcluded(string term)
        {
            lock (Items)
            {
                foreach (var t in Items.Values)
                {
                    foreach (var groupterm in t.Terms)
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

        internal static UserGroupTimeLine GetFromName(string listName)
        {
            UserGroupTimeLine ret = null;
            foreach (var t in GetList())
            {
                if (t.ListName == listName)
                {
                    ret = t;
                }
            }
            return ret;
        }


        public static void Export()
        {
            var fileName = ClientSettings.CacheDir + "\\GroupBackup.xml";
            lock (Items)
            {
                var l = new List<UserGroupTimeLine>();
                foreach (var item in Items.Values)
                {
                    l.Add(item);
                }
                var s = new XmlSerializer(typeof (UserGroupTimeLine[]));
                using (var w = new StreamWriter(fileName))
                {
                    s.Serialize(w, l.ToArray());
                }
            }
        }

        public static void Import()
        {
            var fileName = ClientSettings.CacheDir + "\\GroupBackup.xml";
            if (!File.Exists(fileName)) return;
            UserGroupTimeLine[] input;
            var s = new XmlSerializer(typeof (UserGroupTimeLine[]));

            using (var r = new StreamReader(fileName))
            {
                input = (UserGroupTimeLine[]) s.Deserialize(r);
            }

            lock (Items)
            {
                Items.Clear();
                foreach (var line in input)
                {
                    Items.Add(line.name, line);
                }
            }
            Save();
        }
    }
}