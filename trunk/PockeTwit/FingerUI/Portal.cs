using System;
using System.Drawing;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FingerUI
{
    class Portal : System.Windows.Forms.Control
    {
        #region GDI Imports
        [DllImport("coredll.dll")]
        static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        public enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062
        }
        #endregion

        private Bitmap _Rendered;
        public Graphics g;
        public Bitmap Rendered
        {
            get
            {
                return _Rendered;
            }
        }

        private List<StatusItem> Items = new List<StatusItem>();
        public readonly int MaxItems = 5;
        
        private int ItemHeight = (ClientSettings.TextSize * ClientSettings.LinesOfText) + 5;

        public Portal()
        {
            int maxWidth;
            Rectangle Screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            if (Screen.Width > Screen.Height) { maxWidth = Screen.Width; } else { maxWidth = Screen.Height; }
            _Rendered = new Bitmap(maxWidth, MaxItems * ItemHeight);
            g = Graphics.FromImage(_Rendered);
            PockeTwit.ThrottledArtGrabber.NewArtWasDownloaded += new PockeTwit.ThrottledArtGrabber.ArtIsReady(ThrottledArtGrabber_NewArtWasDownloaded);
        }

        delegate void delNewArt(string User);
        void ThrottledArtGrabber_NewArtWasDownloaded(string User)
        {
            if (InvokeRequired)
            {
                delNewArt d = new delNewArt(this.ThrottledArtGrabber_NewArtWasDownloaded);
                this.Invoke(d, User);
            }
            else
            {
                lock (Items)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        StatusItem s = (StatusItem)Items[i];
                        if (s.Tweet.user.screen_name.ToLower() == User)
                        {
                            Rectangle itemBounds = new Rectangle(0, ItemHeight * i, s.Bounds.Width, ItemHeight);
                            s.Render(g, itemBounds);
                        }
                        if (ClientSettings.ShowReplyImages)
                        {
                            if (!string.IsNullOrEmpty(s.Tweet.in_reply_to_user_id))
                            {
                                string ReplyTo = s.Tweet.SplitLines[0].Split(new char[] { ' ' })[0].TrimEnd(StatusItem.IgnoredAtChars).TrimStart('@').ToLower();
                                if (ReplyTo == User)
                                {
                                    Rectangle itemBounds = new Rectangle(0, ItemHeight * i, s.Bounds.Width, ItemHeight);
                                    s.Render(g, itemBounds);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetItemList(IEnumerable<KListControl.IKListItem> SetOfItems)
        {
            lock (Items)
            {
                Items.Clear();
                int i = 0;
                foreach (KListControl.IKListItem Item in SetOfItems)
                {
                    StatusItem SItem = (StatusItem)Item;
                    Items.Add(SItem);
                    i++;
                    if (i >= MaxItems)
                    {
                        break;
                    }
                }
                Render();
            }
        }
        public void SetItemList(IEnumerable<StatusItem> SetOfItems)
        {
            lock (Items)
            {
                Items.Clear();
                Items = new List<StatusItem>(SetOfItems);
                if (Items.Count > MaxItems)
                {
                    Items.RemoveRange(MaxItems, Items.Count - MaxItems);
                }
                Render();
            }
        }
        public void AddItemToStart(StatusItem Item)
        {
            lock (Items)
            {
                Items.Insert(0, Item);
                if (Items.Count > MaxItems)
                {
                    Items.RemoveAt(Items.Count - 1);
                    Items.TrimExcess();
                    RenderNewItemAtStart();
                }
                
            }
        }
        public void AddToEnd(StatusItem Item)
        {
            lock (Items)
            {
                Items.Add(Item);
                if (Items.Count > MaxItems)
                {
                    Items.RemoveAt(0);
                    Items.TrimExcess();
                    RenderNewItemAtEnd();
                }
            }
        }

        public void ReRenderItem(KListControl.IKListItem IItem)
        {
            StatusItem Item = (StatusItem)IItem;
            ReRenderItem(Item);
        }

        public void ReRenderItem(StatusItem Item)
        {
            lock (Items)
            {
                if (Items.Contains(Item))
                {
                    int i = Items.IndexOf(Item);
                    Rectangle itemBounds = new Rectangle(0, ItemHeight * i, Item.Bounds.Width, ItemHeight);
                    Item.Render(g, itemBounds);
                }
            }

        }

        private delegate void delRender();
        private void Render()
        {
            lock (Items)
            {
                for (int i = 0; i < MaxItems; i++)
                {
                    StatusItem Item = Items[i];
                    Rectangle ItemBounds = new Rectangle(0, i * ItemHeight, Item.Bounds.Width, ItemHeight);
                    using (Pen whitePen = new Pen(ClientSettings.ForeColor))
                    {
                        g.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Top, ItemBounds.Right, ItemBounds.Top);
                        g.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Bottom, ItemBounds.Right, ItemBounds.Bottom);
                        g.DrawLine(whitePen, ItemBounds.Right, ItemBounds.Top, ItemBounds.Right, ItemBounds.Bottom);
                    }
                    Item.Render(g, ItemBounds);
                }
            }
        }
        private void RenderNewItemAtStart()
        {
            //Copy all but last item from top down one.
            IntPtr gPtr = g.GetHdc();
            BitBlt(gPtr, 0, 0, _Rendered.Width, _Rendered.Height - ItemHeight, gPtr, 0, ItemHeight, TernaryRasterOperations.SRCCOPY);
            g.ReleaseHdc(gPtr);
            //Draw the first item.
            StatusItem Item = Items[0];
            Rectangle ItemBounds = new Rectangle(0, 0, Item.Bounds.Width, Item.Bounds.Height);
            using (Pen whitePen = new Pen(ClientSettings.ForeColor))
            {
                g.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Top, ItemBounds.Right, ItemBounds.Top);
                g.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Bottom, ItemBounds.Right, ItemBounds.Bottom);
                g.DrawLine(whitePen, ItemBounds.Right, ItemBounds.Top, ItemBounds.Right, ItemBounds.Bottom);
            }
            Item.Render(g, ItemBounds);
        }
        private void RenderNewItemAtEnd()
        {
            //Copy all but first item from top up one.
            IntPtr gPtr = g.GetHdc();
            BitBlt(gPtr, 0, 0, _Rendered.Width, _Rendered.Height - ItemHeight, gPtr, 0, ItemHeight, TernaryRasterOperations.SRCCOPY);
            g.ReleaseHdc(gPtr);
            //Draw the last item.
            StatusItem Item = Items[Items.Count-1];
            Rectangle ItemBounds = new Rectangle(0, (MaxItems-1)*ItemHeight, Item.Bounds.Width, Item.Bounds.Height);
            using (Pen whitePen = new Pen(ClientSettings.ForeColor))
            {
                g.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Top, ItemBounds.Right, ItemBounds.Top);
                g.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Bottom, ItemBounds.Right, ItemBounds.Bottom);
                g.DrawLine(whitePen, ItemBounds.Right, ItemBounds.Top, ItemBounds.Right, ItemBounds.Bottom);
            }
            Item.Render(g, ItemBounds);
        }
    }
}
