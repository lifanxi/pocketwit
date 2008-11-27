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
            _Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            _Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        }
        public delegate void delClearMe();

        private Bitmap _Rendered = null;
        public Bitmap Rendered
        {
            get
            {
                if (IsDirty)
                {
                    DrawMenu();
                }
                return _Rendered;
            }
        }

        private bool IsDirty = true;
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
                IsDirty=true;
            }
        }

        private int _Height;
        public new int Height
        {
            get { return _Height; }
            set
            {
                if (value != _Height)
                {
                    _Height = value;
                    SetMenuHeight();
                    if (_Width > 0 && _Height > 0)
                    {
                        if (_Rendered != null)
                        {
                            _Rendered.Dispose();
                            GC.Collect();
                        }
                        _Rendered = new Bitmap(_Width, _Height);
                    }
                }
            }
        }

        private int _Width;
        public new int Width
        {
            get{return _Width;}
            set
            {
                if (value != _Width)
                {
                    _Width = value;
                    if (_Width > 0 && _Height > 0)
                    {
                        if (_Rendered != null)
                        {
                            _Rendered.Dispose();
                            GC.Collect();
                        }
                        _Rendered = new Bitmap(_Width, _Height);
                    }
                    IsDirty = true;
                }
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
            IsDirty=true;
        }

        public void ResetMenu(IEnumerable<string> NewItems)
        {
            lock (Items)
            {
                Items.Clear();
                Items.AddRange(NewItems);
                SetMenuHeight();
            }
            IsDirty=true;
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
            IsDirty=true;
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
            IsDirty=true;
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
                if (PrevSelected < Items.Count - 1)
                {
                    _SelectedItem = Items[PrevSelected + 1];
                }
                else
                {
                    _SelectedItem = Items[0];
                }
            }
            IsDirty=true;
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
                else
                {
                    _SelectedItem = Items[Items.Count- 1];
                }
            }
            IsDirty=true;
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
            IsDirty=true;
        }

        private void DrawMenu()
        {
            if (_Rendered == null) 
            {
                _Rendered = new Bitmap(_Width, _Height);
            }
            using (Graphics m_backBuffer = Graphics.FromImage(_Rendered))
            {
                m_backBuffer.Clear(ClientSettings.BackColor);
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
                        Color GradColor;

                        if (Item == SelectedItem)
                        {
                            BackColor = ClientSettings.SelectedBackColor;
                            GradColor = ClientSettings.SelectedBackGradColor;
                            MenuTextColor = ClientSettings.SelectedForeColor;
                        }
                        else
                        {
                            BackColor = ClientSettings.BackColor;
                            GradColor = ClientSettings.BackGradColor;
                            MenuTextColor = ClientSettings.ForeColor;
                        }
                        Gradient.GradientFill.Fill(m_backBuffer, menuRect, BackColor, GradColor, Gradient.GradientFill.FillDirection.TopToBottom);
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
            IsDirty = false;
        }

    }
}
