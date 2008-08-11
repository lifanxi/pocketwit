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
                AddUnique(items);
            }
        }

        private void AddUnique(IEnumerable<Library.status> items)
        {
            foreach (Library.status s in items)
            {
                if (!this.Contains(s))
                {
                    this.Add(s);
                }
            }
        }
        public void MergeIn(TimeLine otherLine)
        {
            this.AddUnique(otherLine);
            this.Sort();
            int overage = this.Count - ClientSettings.MaxTweets;
            if (overage > 0)
            {
                this.RemoveRange(ClientSettings.MaxTweets, overage);
            }
        }
    }
}
