using System;

using System.Data.SQLite;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit
{
    public class SpecialTimeLine
    {
        public string name { get; set; }
        public string[] Terms { get; set; }

        public void AddItem(string Term)
        {
            if (Terms != null && Terms.Length > 0)
            {
                List<string> items = new List<string>(Terms);
                if (!items.Contains(Term))
                {
                    items.Add(Term);
                }
                Terms = items.ToArray();
            }
            else
            {
                Terms = new string[]{ Term };
            }
            
        }

        public string GetConstraints()
        {
            string ret = "";
            List<string> UserList = new List<string>();
            foreach (string t in Terms)
            {
                UserList.Add("'"+t+"'");
                
            }
            if (UserList.Count > 0)
            {
                ret = " AND statuses.userid IN(" + string.Join(",", UserList.ToArray()) + ") ";
            }

            return ret;
        }
    }

    public static class SpecialTimeLines
    {
        private static string configPath = ClientSettings.AppPath + "\\savedTimelines.xml";
        private static Dictionary<string, SpecialTimeLine> _Items = new Dictionary<string, SpecialTimeLine>();

        public static SpecialTimeLine[] GetList()
        {
            List<SpecialTimeLine> s = new List<SpecialTimeLine>();
            lock (_Items)
            {
                foreach (SpecialTimeLine item in _Items.Values)
                {
                    s.Add(item);
                }
            }
            return s.ToArray();
        }
        public static void Add(SpecialTimeLine newLine)
        {
            lock (_Items)
            {
                if (!_Items.ContainsKey(newLine.name))
                {
                    _Items.Add(newLine.name, newLine);
                }
            }
        }
        public static void Remove(SpecialTimeLine oldLine)
        {
            lock (_Items)
            {
                if(_Items.ContainsKey(oldLine.name))
                {
                    _Items.Remove(oldLine.name);
                }
            }
        }
        public static void Clear()
        {
            lock (_Items)
            {
                _Items.Clear();
            }
        }

        public static void Load()
        {
            /*
            if(!System.IO.File.Exists(configPath)){return;}
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(List<SpecialTimeLine>));
                lock (_Items)
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(configPath))
                    {
                        _Items = (List<SpecialTimeLine>)s.Deserialize(r);
                        return;
                    }
                }
            }
            catch 
            {
                System.IO.File.Delete(configPath);
            }
            */
            using (SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (SQLiteCommand comm = new SQLiteCommand(conn))
                {
                    comm.CommandText = "SELECT groupname, userid FROM usersInGroups";
                    using (SQLiteDataReader r = comm.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string groupName = r.GetString(0);
                            string userID = r.GetString(1);
                            SpecialTimeLine thisLine = new SpecialTimeLine();
                            if (_Items.ContainsKey(groupName))
                            {
                                thisLine = _Items[groupName];
                            }
                            else
                            {
                                thisLine.name = groupName;
                                Add(thisLine);
                            }
                            thisLine.AddItem(userID);
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
                            foreach (SpecialTimeLine group in _Items.Values)
                            {
                                using (SQLiteCommand comm = new SQLiteCommand(conn))
                                {
                                    comm.CommandText = "INSERT INTO groups (groupname) VALUES (@name);";
                                    comm.Parameters.Add(new SQLiteParameter("@name", group.name));

                                    comm.ExecuteNonQuery();
                                    
                                    foreach (string userid in group.Terms)
                                    {
                                        comm.Parameters.Clear();
                                        comm.CommandText = "INSERT INTO usersInGroups (id, groupname, userid) VALUES (@pairid, @name, @userid)";
                                        comm.Parameters.Add(new SQLiteParameter("@pairid", group.name+userid));
                                        comm.Parameters.Add(new SQLiteParameter("@name", group.name));
                                        comm.Parameters.Add(new SQLiteParameter("@userid", userid));
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
    }
}
