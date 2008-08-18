using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    public class TimeLine : List<Library.status>
    {
        private string LastReadID = "";

        private Dictionary<string, Library.status> InternalDictionary = new Dictionary<string, PockeTwit.Library.status>();
        public TimeLine()
        {
        }
        public TimeLine(IEnumerable<Library.status> items)
        {

            this.Clear();
            InternalDictionary.Clear();
            if (items != null)
            {
                AddUnique(items);
            }
            TrimToFit();
        }

        private void AddUnique(IEnumerable<Library.status> items)
        {
            foreach (Library.status s in items)
            {
                if (!InternalDictionary.ContainsKey(s.id))
                {
                    InternalDictionary.Add(s.id, s);
                    this.Add(s);
                }
            }
        }
        public int MergeIn(TimeLine otherLine)
        {
            int NewItems = 0;
            this.AddUnique(otherLine);
            TrimToFit();
            if (string.IsNullOrEmpty(LastReadID))
            {
                NewItems = this.Count;
            }
            else
            {
                foreach (Library.status stat in this)
                {
                    if (stat.id == LastReadID)
                    {
                        break;
                    }
                    NewItems++;
                }
            }
            this.LastReadID = this[0].id;
            return NewItems;
        }

        private void TrimToFit()
        {
            this.Sort();
            int overage = this.Count - ClientSettings.MaxTweets;
            if (overage > 0)
            {
                for (int i = overage; i < this.Count; i++)
                {
                    InternalDictionary.Remove(this[i].id);
                }
                this.RemoveRange(ClientSettings.MaxTweets, overage);
                this.TrimExcess();
            }
        }
    }
}
