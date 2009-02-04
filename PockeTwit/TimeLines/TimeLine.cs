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
            if (this.Count > 0)
            {
                this.LastReadID = this[0].id;
            }
        }

        private void AddUnique(IEnumerable<Library.status> items)
        {
            lock (InternalDictionary)
            {
                try
                {
                    if (items == null) { return; }
                    foreach (Library.status s in items)
                    {
                        if (!string.IsNullOrEmpty(s.id))
                        {
                            if (!InternalDictionary.ContainsKey(s.id))
                            {
                                InternalDictionary.Add(s.id, s);
                                this.Add(s);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }
        }
        
        public int MergeIn(IEnumerable<Library.status> otherLine)
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
            if (this.Count > 0)
            {
                this.LastReadID = this[0].id;
            }
            return NewItems;
        }

        private void TrimToFit()
        {
            this.Sort();
            int overage = this.Count - ClientSettings.MaxTweets;
            if (overage > 0)
            {
                for (int i = this.Count-overage; i < this.Count; i++)
                {
                    InternalDictionary.Remove(this[i].id);
                }
                this.RemoveRange(ClientSettings.MaxTweets, overage);
                this.TrimExcess();
            }
        }
    }
}
