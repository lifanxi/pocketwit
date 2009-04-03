using System;

using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace LocalStorage
{
    class DataBaseUtility
    {
        
        #region SQL Constants
        private const string SQLFetchFromCache =
                        @"SELECT     statuses.id, statuses.fulltext, statuses.userid, statuses.[timestamp], 
                                     statuses.in_reply_to_id, statuses.favorited, statuses.clientSource, 
                                     statuses.accountSummary, statuses.statustypes, users.screenname, 
                                     users.fullname, users.description, users.avatarURL, statuses.statustypes
                          FROM       statuses INNER JOIN users ON statuses.userid = users.id";
        private const string SQLCountFromCache =
                        @"SELECT     COUNT(id) AS newItems
                          FROM         statuses WHERE
                          (timestamp > 
                            (SELECT timestamp from statuses WHERE id=@id))";
        private const string SQLGetLastStatusID =
                        @"SELECT    statuses.id
                          FROM statuses 
                          WHERE statuses.accountSummary = @accountsummary";

        private const string SQLIgnoreGrouped =
            //@" INNER JOIN usersInGroups ON statuses.userid <> usersInGroups.userid ";
                        @" AND ((SELECT COUNT(id) FROM usersInGroups WHERE usersInGroups.userid=statuses.userid ) = 0)  ";
        
        private const string SQLFetchFriends = "(statuses.statustypes == 0)";
        private const string SQLFetchReplies = "(statuses.statustypes & 1)";
        
        private const string SQLFetchRepliesAndMessages = " statuses.statustypes & 1 OR statuses.statustypes & 2 ";

        private const string SQLFetchDirects = "(statuses.statustypes & 2)";
        private const string SQLFetchSearches = "(statuses.statustypes & 4)";
        private const string SQLOrder = " ORDER BY statuses.[timestamp] DESC ";
        private const string SQLLimit = " LIMIT @count ";
        #endregion

        private static string DBPath = ClientSettings.AppPath + "\\LocalStorage\\LocalCache.db";

        public static void CheckDBSchema()
        {
            try
            {
                using (SQLiteConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SQLiteCommand comm = new SQLiteCommand(conn))
                    {
                        
                        comm.CommandText = "SELECT value from DBProperties WHERE name='dbversion'";
                        string versionNum = (string)comm.ExecuteScalar();

                        if (versionNum == DBVersion) { return; }
                    }
                    conn.Close();
                }
            }
            catch { }

            DeleteDB();
        }
        private static void DeleteDB()
        {
            if (System.IO.File.Exists(DBPath))
            {
                System.IO.File.Delete(DBPath);
            }
        }
        
        public static System.Data.SQLite.SQLiteConnection GetConnection()
        {
            if (!System.IO.File.Exists(DBPath)) { CreateDB(); }
            return new SQLiteConnection("Data Source=" + ClientSettings.AppPath + "\\LocalStorage\\LocalCache.db");
        }

        //Update this number if you change the schema of the database -- it'll
        // force the client to recreate it.
        private const string DBVersion = "0004";
        private static void CreateDB()
        {
            if (!System.IO.Directory.Exists(ClientSettings.AppPath + "\\LocalStorage"))
            {
                System.IO.Directory.CreateDirectory(ClientSettings.AppPath + "\\LocalStorage");
            }
            SQLiteConnection.CreateFile(DBPath);
            try
            {
                using (SQLiteConnection conn = GetConnection())
                {
                    using (SQLiteCommand comm = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        SQLiteTransaction t = conn.BeginTransaction();

                        comm.CommandText =
                            @"CREATE TABLE DBProperties (name VARCHAR(50) PRIMARY KEY,
                            value NVARCHAR(255))
                            ";
                        comm.ExecuteNonQuery();

                        comm.CommandText =
                            @"INSERT INTO DBProperties (name,value) VALUES (@name,@value)";
                        comm.Parameters.Add(new SQLiteParameter("@name", "dbversion"));
                        comm.Parameters.Add(new SQLiteParameter("@value", DBVersion));
                        comm.ExecuteNonQuery();
                        comm.Parameters.Clear();

                        comm.CommandText =
                            @"CREATE TABLE statuses (id VARCHAR(50) PRIMARY KEY,
                            fulltext NVARCHAR(255),
                            userid VARCHAR(50),
                            timestamp DATETIME,
                            in_reply_to_id VARCHAR(50),
                            favorited BIT,
                            clientSource VARCHAR(50),
                            accountSummary VARCHAR(50),
                            statustypes SMALLINT(2),
                            UNIQUE (id) )
                                       ";
                        comm.ExecuteNonQuery();

                        comm.CommandText =
                            @"CREATE TABLE users (id VARCHAR(50) PRIMARY KEY,
                            screenname NVARCHAR(255),
                            fullname NVARCHAR(255),
                            description NVARCHAR(255),
                            avatarURL TEXT,
                            UNIQUE (id) )
                                       ";
                        comm.ExecuteNonQuery();

                        comm.CommandText =
                            @"CREATE TABLE groups (groupname NVARCHAR(50) PRIMARY KEY ON CONFLICT IGNORE)";
                        comm.ExecuteNonQuery();

                        comm.CommandText =
                            @"CREATE TABLE usersInGroups (id NVARCHAR(100) PRIMARY KEY ON CONFLICT IGNORE,
                            groupname NVARCHAR(50),
                            userid VARCHAR(50))";
                        comm.ExecuteNonQuery();


                        t.Commit();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static List<PockeTwit.Library.status> GetList(PockeTwit.TimelineManagement.TimeLineType typeToGet, int Count)
        {
            string Constraints = null;
            if (typeToGet == PockeTwit.TimelineManagement.TimeLineType.Friends)
            {
                Constraints = SQLIgnoreGrouped;
            }
            return GetList(typeToGet, Count, Constraints);
        }
        public static List<PockeTwit.Library.status> GetList(PockeTwit.TimelineManagement.TimeLineType typeToGet, int Count, string Constraints)
        {
            List<PockeTwit.Library.status> cache = new List<PockeTwit.Library.status>();
            using (System.Data.SQLite.SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                string FetchQuery = SQLFetchFromCache;
                
                FetchQuery = FetchQuery + " WHERE " +AddTypeWhereClause(typeToGet) + Constraints + SQLOrder + SQLLimit;
                
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

        private static string AddTypeWhereClause(PockeTwit.TimelineManagement.TimeLineType typeToGet)
        {
            switch (typeToGet)
            {
                case PockeTwit.TimelineManagement.TimeLineType.Friends:
                    return SQLFetchFriends;
                    break;
                case PockeTwit.TimelineManagement.TimeLineType.Messages:
                    return SQLFetchRepliesAndMessages;
                    break;
            }
            return null;
        }
        
        public static int GetItemsNewerThan(PockeTwit.TimelineManagement.TimeLineType typeToGet, string ID, string Constraints)
        {
            using (System.Data.SQLite.SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                string FetchQuery = SQLCountFromCache;
                FetchQuery = FetchQuery + " AND " + AddTypeWhereClause(typeToGet) + Constraints + SQLOrder;
                using (System.Data.SQLite.SQLiteCommand comm = new System.Data.SQLite.SQLiteCommand(FetchQuery, conn))
                {
                    comm.Parameters.Add(new SQLiteParameter("@id", ID));
                    conn.Open();
                    object o= comm.ExecuteScalar();
                    return System.Convert.ToInt32(o);
                }
            }
        }


        public static void CleanDB(int OlderThan)
        {
            DateTime SinceDate = DateTime.Now.AddDays(1);
            if (OlderThan > 0)
            {
                SinceDate = DateTime.Now.Subtract(new TimeSpan(OlderThan, 0, 0, 0));
            }
            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();
                using (SQLiteTransaction t = conn.BeginTransaction())
                {
                    using (SQLiteCommand comm = new SQLiteCommand(conn))
                    {
                        comm.CommandText = "DELETE FROM statuses WHERE timestamp<@SinceDay";
                        comm.Parameters.Add(new SQLiteParameter("@SinceDay", SinceDate));
                        comm.ExecuteNonQuery();
                        comm.Parameters.Clear();

                        /*
                        comm.CommandText = "DELETE FROM users";
                        comm.ExecuteNonQuery();
                         */
                    }
                    t.Commit();
                }
                conn.Close();
            }
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

        public static string GetLatestItem(Yedda.Twitter.Account account, PockeTwit.TimelineManagement.TimeLineType typeToGet)
        {
            string FetchQuery = SQLGetLastStatusID;
            switch (typeToGet)
            {
                case PockeTwit.TimelineManagement.TimeLineType.Friends:
                    FetchQuery = FetchQuery + " AND " + SQLFetchFriends + SQLOrder + SQLLimit;
                    break;
                case PockeTwit.TimelineManagement.TimeLineType.Replies:
                    FetchQuery = FetchQuery + " AND " + SQLFetchReplies + SQLOrder + SQLLimit;
                    break;
                case PockeTwit.TimelineManagement.TimeLineType.Direct:
                    FetchQuery = FetchQuery + " AND " + SQLFetchDirects + SQLOrder + SQLLimit;
                    break;
                case PockeTwit.TimelineManagement.TimeLineType.Messages:
                    FetchQuery = FetchQuery + " AND " + SQLFetchRepliesAndMessages + SQLOrder + SQLLimit;
                    break;
            }
                
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand comm = new SQLiteCommand(FetchQuery, conn))
                {
                    comm.Parameters.Add(new SQLiteParameter("@accountsummary", account.Summary));
                    comm.Parameters.Add(new SQLiteParameter("@count", 1));

                    conn.Open();
                    return (string)comm.ExecuteScalar();
                }
            }
        }
    }
}
