using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace FingerUI
{
    public class SideMenu : System.Windows.Forms.Control
    {
        public SideMenu(FingerUI.KListControl.SideShown Side)
        {
            _Side = Side;
        }
        public delegate void delClearMe();

        public Bitmap Rendered;

        private FingerUI.KListControl.SideShown _Side;
        private List<string> Items = new List<string>();
        private string _SelectedItem = null;
        public string SelectedItem
        {
            get 
            {
                lock (Items)
                {
                    if (Items.Count == 0)
                    {
                        return null;
                    }
                    if (string.IsNullOrEmpty(_SelectedItem))
                    {
                        return Items[0];
                    }
                    return _SelectedItem;
                }
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

        private string _UserName;
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                DrawMenu();
            }
        }

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

        private int _Width;
        public new int Width
        {
            get{return _Width;}
            set
            {
                _Width = value;
                Rendered = new Bitmap(_Height, _Width);
                DrawMenu();
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
                        int multiplyer = 4;
                        if (PockeTwit.DetectDevice.DeviceType == PockeTwit.DeviceType.Standard)
                        {
                            multiplyer = 3;
                        }
                        _ItemHeight = (this.Height - (ClientSettings.TextSize * multiplyer)) / Items.Count;
                        _TopOfMenu= ((this.Height / 2) - ((Items.Count * ItemHeight) / 2));
                    }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        public void InsertMenuItem(int index, string Item)
        {
            lock (Items)
            {
                Items.Insert(index, Item);
                SetMenuHeight();
            }
            DrawMenu();
        }

        public void ResetMenu(IEnumerable<string> NewItems)
        {
            lock (Items)
            {
                Items.Clear();
                Items.AddRange(NewItems);
                SetMenuHeight();
            }
            DrawMenu();
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
            DrawMenu();
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
            DrawMenu();
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
            DrawMenu();
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
            DrawMenu();
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
            DrawMenu();
        }

        private void DrawMenu()
        {
            Rendered = new Bitmap(_Width, _Height);
            using (Graphics m_backBuffer = Graphics.FromImage(Rendered))
            {
                int LeftPos = 0;
                int CurrentTop = TopOfMenu;
                foreach (string Item in this.GetItems())
                {
                    if (_Side == FingerUI.KListControl.SideShown.Left)
                    {
                        int TextWidth = (int)m_backBuffer.MeasureString(Item, ClientSettings.MenuFont).Width + ClientSettings.Margin;
                        LeftPos = _Width - (TextWidth + 5);
                    }
                    else
                    {
                        LeftPos = 6;
                    }
                    using (Pen whitePen = new Pen(ClientSettings.ForeColor))
                    {

                        Rectangle menuRect = new Rectangle(0, CurrentTop, _Width, ItemHeight);
                        Color BackColor;
                        Color MenuTextColor;

                        if (Item == SelectedItem)
                        {
                            BackColor = ClientSettings.SelectedBackColor;
                            MenuTextColor = ClientSettings.SelectedForeColor;
                        }
                        else
                        {
                            BackColor = ClientSettings.BackColor;
                            MenuTextColor = ClientSettings.ForeColor;


                        }
                        using (Brush sBrush = new SolidBrush(BackColor))
                        {
                            m_backBuffer.FillRectangle(sBrush, menuRect);
                        }

                        m_backBuffer.DrawLine(whitePen, menuRect.Left, menuRect.Top, menuRect.Right, menuRect.Top);
                        using (Brush sBrush = new SolidBrush(MenuTextColor))
                        {
                            StringFormat sFormat = new StringFormat();
                            sFormat.LineAlignment = StringAlignment.Center;
                            int TextTop = ((menuRect.Bottom - menuRect.Top) / 2) + menuRect.Top;
                            string DisplayItem = Item.Replace("@User", "@" + _UserName);
                            m_backBuffer.DrawString(DisplayItem, ClientSettings.MenuFont, sBrush, LeftPos, TextTop, sFormat);
                        }
                        m_backBuffer.DrawLine(whitePen, menuRect.Left, menuRect.Bottom, menuRect.Right, menuRect.Bottom);
                        if (_Side == FingerUI.KListControl.SideShown.Right)
                        {
                            m_backBuffer.DrawLine(whitePen, menuRect.Left, 0, menuRect.Left, this.Height);
                        }
                        else
                        {
                            m_backBuffer.DrawLine(whitePen, menuRect.Right-1, 0, menuRect.Right-1, this.Height);
                        }
                        CurrentTop = CurrentTop + ItemHeight;
                    }
                }
            }
        }

    }
}
