using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    class TimeLine : List<Library.status>
    {
        private Dictionary<string, Library.status> InternalDictionary = new Dictionary<string, PockeTwit.Library.status>();
        public TimeLine(IEnumerable<Library.status> items)
        {

            this.Clear();
            InternalDictionary.Clear();
            if (items != null)
            {
                AddUnique(items);
            }
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
        public void MergeIn(TimeLine otherLine)
        {
            this.AddUnique(otherLine);
            this.Sort();
            int overage = this.Count - ClientSettings.MaxTweets;
            if (overage > 0)
            {
                for (int i = overage; i < this.Count; i++)
                {
                    InternalDictionary.Remove(this[i].id);
                }
                this.RemoveRange(ClientSettings.MaxTweets, overage);
            }
        }
    }
}
