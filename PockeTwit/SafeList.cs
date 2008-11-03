using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    public class SafeList<TValue> : IList<TValue>
    {
        private readonly object syncRoot = new object();
        private List<TValue> d = new List<TValue>();

        public SafeList()
        {
        }
        public SafeList(IEnumerable<TValue> Collection)
        {
            d.AddRange(Collection);
        }

        public void Add(TValue Value)
        {
            lock (syncRoot)
            {
                d.Add(Value);
            }
        }
        public void AddRange(IEnumerable<TValue> Collection)
        {
            lock (syncRoot)
            {
                d.AddRange(Collection);
            }
        }
        public void Clear()
        {
            lock (syncRoot)
            {
                d.Clear();
            }
        }
        public bool Contains(TValue Value)
        {
            lock (syncRoot)
            {
                return d.Contains(Value);
            }
        }
        public int Count
        {
            get { return d.Count; }
        }
        public List<TValue> GetRange(int index, int count)
        {
            return d.GetRange(index, count);
        }
        public void Insert(int index, TValue Value)
        {
            lock (syncRoot)
            {
                d.Insert(index, Value);
            }
        }
        public void InsertRange(int index, IEnumerable<TValue> Collection)
        {
            lock (syncRoot)
            {
                d.InsertRange(index, Collection);
            }
        }
        public void Remove(TValue Value)
        {
            lock (syncRoot)
            {
                d.Remove(Value);
            }
        }
        public void RemoveAt(int index)
        {
            lock (syncRoot)
            {
                d.RemoveAt(index);
            }
        }
        public void RemoveRange(int index, int count)
        {
            lock (syncRoot)
            {
                d.RemoveRange(index, count);
            }
        }
        public void Sort()
        {
            lock (syncRoot)
            {
                d.Sort();
            }
        }
        public void Reverse()
        {
            lock (syncRoot)
            {
                d.Reverse();
            }
        }
        public TValue[] ToArrary()
        {
            return d.ToArray();
        }
        public void TrimExcess()
        {
            d.TrimExcess();
        }

        
        public TValue this[int index]
        {
            get
            {
                return d[index];
            }
            set
            {
                lock (syncRoot)
                {
                    d[index] = value;
                }
            }
        }
        
        public void ForEach(Action<TValue> action)
        {
            d.ForEach(action);
        }

        

        #region IEnumerable<TValue> Members

        
        #endregion

        #region IList<TValue> Members

        public int IndexOf(TValue item)
        {
            return d.IndexOf(item);
        }

        #endregion

        #region ICollection<TValue> Members


        public void CopyTo(TValue[] array, int arrayIndex)
        {
            d.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<TValue>.Remove(TValue item)
        {
            return d.Remove(item);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return d.GetEnumerator();
        }

        #endregion

        #region IEnumerable<TValue> Members

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return d.GetEnumerator();
        }

        #endregion
    }
}
