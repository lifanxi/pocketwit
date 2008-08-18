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
                    System.Diagnostics.Debug.WriteLine("Trim " + i.ToString() + ": " + this[i].id);
                }
                this.RemoveRange(ClientSettings.MaxTweets, overage);
                this.TrimExcess();
            }
        }

        public void OutputDebug()
        {
            foreach (Library.status s in this)
            {
                System.Diagnostics.Debug.WriteLine(s.text);
            }
        }

        public void OutputDebugTimes()
        {
            foreach (Library.status s in this)
            {
                System.Diagnostics.Debug.WriteLine(s.created_at);
            }
        }
        public Library.status FindByDate(string created_at)
        {
            foreach (Library.status s in this)
            {
                if (s.created_at == created_at)
                {
                    return s;
                }
            }
            return null;
        }
        public Library.status FindStartingWith(string startingWith)
        {
            foreach (Library.status s in this)
            {
                if (s.text.StartsWith(startingWith))
                {
                    return s;
                }
            }
            return null;
        }
    }
}
