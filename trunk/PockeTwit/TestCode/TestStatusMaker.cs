using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit.TestCode
{
    static class TestStatusMaker
    {
        public static Library.status[] GenerateTestStatuses(int NumberOfItems)
        {
            Library.status[] ret = new PockeTwit.Library.status[NumberOfItems];

            for (int i = 0; i < NumberOfItems; i++)
            {
                DateTime t = DateTime.Now.Subtract(new TimeSpan(0, 0, NumberOfItems - i));
                ret[i] = new PockeTwit.Library.status();
                ret[i].created_at = t.ToString();
                ret[i].text = "Test status " + i;
                ret[i].user = new PockeTwit.Library.User();
                ret[i].user.name = "Test user" + i;
            }
            return ret;
        }
    }
}
