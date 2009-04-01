using System;

using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace LocalStorage
{
    class DataBaseUtility
    {
        private const string SQLFetchFromCache =
                        @"SELECT     statuses.id, statuses.fulltext, statuses.userid, statuses.[timestamp], 
                                     statuses.in_reply_to_id, statuses.favorited, statuses.clientSource, 
                                     statuses.accountSummary, statuses.statustypes, users.screenname, 
                                     users.fullname, users.description, users.avatarURL, statuses.statustypes
                          FROM       statuses INNER JOIN users ON statuses.userid = users.id";
        private const string SQLFetchFriends = "(statuses.statustypes == 0)";
        private const string SQLFetchMessages = "(statuses.statustypes & 1)";
        private const string SQLFetchFriendsAndMessages = " statuses.statustypes & 1 OR statuses.statustypes & 2 ";

        private const string SQLFetchDirects = "(statuses.statustypes & 2)";
        private const string SQLFetchSearches = "(statuses.statustypes & 4)";
        private const string SQLOrder = " ORDER BY statuses.[timestamp] DESC LIMIT @count ";

        private static string DBPath = ClientSettings.AppPath + "\\LocalStorage\\LocalCache.db";
        public static System.Data.SQLite.SQLiteConnection GetConnection()
        {
            if (!System.IO.File.Exists(DBPath)) { CreateDB(); }
            return new SQLiteConnection("Data Source=" + ClientSettings.AppPath + "\\LocalStorage\\LocalCache.db");
        }

        private static void CreateDB()
        {
            if (!System.IO.Directory.Exists(ClientSettings.AppPath + "\\LocalStorage"))
            {
                System.IO.Directory.CreateDirectory(ClientSettings.AppPath + "\\LocalStorage");
            }
            SQLiteConnection.CreateFile(DBPath);
            using(SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand comm = new SQLiteCommand(conn))
                {
                    comm.CommandText =
                        @"CREATE TABLE statuses (id VARCHAR(50) PRIMARY KEY,
                            fulltext NVARCHAR(255),
                            userid VARCHAR(50),
                            timestamp DATETIME,
                            in_reply_to_id VARCHAR(50),
                            favorited BIT,
                            clientSource VARCHAR(50),
                            accountSummary VARCHAR(50),
                            statustypes SMALLINT(2))
                                       ";
                    conn.Open();
                    SQLiteTransaction t = conn.BeginTransaction();
                    comm.ExecuteNonQuery();
                    comm.CommandText =
                        @"CREATE TABLE users (id VARCHAR(50) PRIMARY KEY,
                            screenname NVARCHAR(255),
                            fullname NVARCHAR(255),
                            description NVARCHAR(255),
                            avatarURL TEXT)
                                       ";
                    comm.ExecuteNonQuery();
                    t.Commit();
                    conn.Close();
                }
            }
        }



        public static List<PockeTwit.Library.status> GetList(PockeTwit.TimelineManagement.TimeLineType typeToGet, int Count)
        {
            List<PockeTwit.Library.status> cache = new List<PockeTwit.Library.status>();
            using (System.Data.SQLite.SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                string FetchQuery = SQLFetchFromCache;
                switch (typeToGet)
                {
                    case PockeTwit.TimelineManagement.TimeLineType.Friends:
                        FetchQuery = FetchQuery + " WHERE " + SQLFetchFriends + SQLOrder;
                        break;
                    case PockeTwit.TimelineManagement.TimeLineType.Messages:
                        FetchQuery = FetchQuery + " WHERE " + SQLFetchFriendsAndMessages + SQLOrder;
                        break;
                }
                
                using (System.Data.SQLite.SQLiteCommand comm = new System.Data.SQLite.SQLiteCommand(FetchQuery, conn))
                {
                    comm.Parameters.Add(new SQLiteParameter("@count", Count));
                    conn.Open();
                    using (System.Data.SQLite.SQLiteDataReader r = comm.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            PockeTwit.Library.status newStat = new PockeTwit.Library.status();
                            newStat.id = r.GetString(0);
                            newStat.text = r.GetString(1);
                            newStat.TypeofMessage = (PockeTwit.Library.StatusTypes)r.GetInt32(13);
                            newStat.createdAt = r.GetDateTime(3);
                            newStat.in_reply_to_status_id = r.GetString(4);
                            newStat.favorited = r.GetString(5);
                            newStat.source = r.GetString(6);
                            newStat.AccountSummary = r.GetString(7);

                            PockeTwit.Library.User u = new PockeTwit.Library.User();
                            u.id = r.GetString(2);
                            u.screen_name = r.GetString(9);
                            u.name = r.GetString(10);
                            u.description = r.GetString(11);
                            u.profile_image_url = r.GetString(12);

                            newStat.user = u;
                            cache.Add(newStat);
                        }
                    }
                    conn.Close();
                }
            }
            return cache;
        }

        public static void SaveItems(List<PockeTwit.Library.status> TempLine)
        {
            using (System.Data.SQLite.SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                System.Data.SQLite.SQLiteTransaction t = conn.BeginTransaction();
                foreach (PockeTwit.Library.status status in TempLine)
                {
                    status.Save(conn);
                }
                t.Commit();
                conn.Close();
            }
        }
    }
}
