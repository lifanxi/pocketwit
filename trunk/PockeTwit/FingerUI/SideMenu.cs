using System;

using System.Collections.Generic;
using System.Text;

namespace FingerUI
{
    public class SideMenu : System.Windows.Forms.Control
    {
        public delegate void delClearMe();

        private List<string> Items = new List<string>();
        private string _SelectedItem = null;
        public string SelectedItem
        {
            get 
            {
                if (string.IsNullOrEmpty(_SelectedItem))
                {
                    return Items[0];
                }
                return _SelectedItem; 
            }
            set
            {
                lock (Items)
                {
                    if (Items.Contains(value))
                    {
                        _SelectedItem = value;
                    }
                    else
                    {
                        _SelectedItem = null;
                    }
                }
            }
        }
        public int Count
        {
            get { return Items.Count; }
        }
        private int _ItemHeight = 0;
        private int _TopOfMenu = 0;
        public int ItemHeight { get { return _ItemHeight; } }
        public int TopOfMenu { get { return _TopOfMenu; } }

        private int _Height;
        public new int Height
        {
            get { return _Height; }
            set
            {
                _Height = value;
                SetMenuHeight();
            }
        }

        private void SetMenuHeight()
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(SetMenuHeight);
                this.Invoke(d, null);
            }
            else
            {
                if (Items.Count > 0)
                {
                    try
                    {
                        _ItemHeight = (this.Height - (ClientSettings.TextSize * 5)) / Items.Count;
                        _TopOfMenu= ((this.Height / 2) - ((Items.Count * ItemHeight) / 2));
                    }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        public void Clear()
        {
            lock (Items) { Items.Clear(); }
        }
        public void AddItems(IEnumerable<string> NewItems)
        {
            lock (Items)
            {
                foreach (string Item in NewItems)
                {
                    if (!Items.Contains(Item))
                    {
                        Items.Add(Item);
                    }
                }
                SetMenuHeight();
            }
        }
        public void RemoveItem(string OldItem)
        {
            lock (Items)
            {
                if (Items.Contains(OldItem))
                {
                    Items.Remove(OldItem);
                }
                SetMenuHeight();
            }
        }
        public string[] GetItems()
        {
            lock (Items)
            {
                return Items.ToArray();
            }
        }

        public bool Contains(string ItemToSeek)
        {
            lock (Items)
            {
                return Items.Contains(ItemToSeek);
            }
        }
        public void SelectDown()
        {
            lock (Items)
            {
                int PrevSelected = Items.IndexOf(SelectedItem);
                if(PrevSelected<Items.Count-1)
                {
                    _SelectedItem = Items[PrevSelected + 1];
                }
            }
        }
        public void SelectUp()
        {
            lock (Items)
            {
                int PrevSelected = Items.IndexOf(SelectedItem);
                if (PrevSelected > 0)
                {
                    _SelectedItem = Items[PrevSelected - 1];
                }
            }
        }

        public void ReplaceItem(string Original, string New)
        {
            lock (Items)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] == Original)
                    {
                        Items[i] = New;
                    }
                }
            }
        }

    }
}
