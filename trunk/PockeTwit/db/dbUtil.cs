using System;

using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace PockeTwit.db
{
    class dbUtil
    {
        public static SQLiteConnection GetDBConnection()
        {
            return new SQLiteConnection("Data Source=cache.db");
        }
    }
}
