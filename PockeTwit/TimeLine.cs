using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    class TimeLine : List<Library.status>
    {
        public TimeLine(IEnumerable<Library.status> items)
        {

            this.Clear();
            if (items != null)
            {
                this.AddRange(items);
            }
        }
        public void MergeIn(TimeLine otherLine)
        {
            this.AddRange(otherLine);
            this.Sort();
            int overage = this.Count - ClientSettings.MaxTweets;
            this.RemoveRange(ClientSettings.MaxTweets, overage);
        }
    }
}
