﻿using System;
using System.Drawing;

using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FingerUI
{
    public class LowMemoryException : Exception { }

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

        private static List<Thread> _RenderThreads = new List<Thread>();
        public delegate void delNewImage();
        public event delNewImage NewImage = delegate { };

        private volatile bool cancelMyCurrentThread = false;
        
        public int WindowOffset;
        private Bitmap temp;
        private Bitmap _Rendered;
        public Graphics _RenderedGraphics;
        public Bitmap Rendered
        {
            get
            {
                return _Rendered;
            }
        }

        private System.Threading.Thread CurrentRenderJob;
        private System.Threading.Timer pauseBeforeStarting;
        private List<StatusItem> Items = new List<StatusItem>();
        public int MaxItems = ClientSettings.PortalSize;
        private const int PauseBeforeRerender = 50;
        private int _BitmapHeight = 0;
        public int BitmapHeight
        {
            get { return _BitmapHeight; }
        }
        
        private int ItemHeight = (ClientSettings.TextSize * ClientSettings.LinesOfText) + 5;
        private int maxWidth = 0;
        public Portal()
        {
            if (MaxItems < 15) { MaxItems = ClientSettings.MaxTweets; }
            SetBufferSize();
            PockeTwit.ThrottledArtGrabber.NewArtWasDownloaded += new PockeTwit.ThrottledArtGrabber.ArtIsReady(ThrottledArtGrabber_NewArtWasDownloaded);
            pauseBeforeStarting = new System.Threading.Timer(RenderBackgroundLowPriority, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        private void SetBufferSize()
        {
            Rectangle Screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            if (Screen.Width > Screen.Height) { maxWidth = Screen.Width; } else { maxWidth = Screen.Height; }
            

            //Try to create temporary bitmaps for everything we'll need so we can try it out.
            bool bGood = false;
            Bitmap TestMap = null;
            Bitmap SecondMap = null;

            Bitmap ScreenMap = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Bitmap AvatarMap = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);
            while (!bGood)
            {
                try
                {
                    TestMap = new Bitmap(maxWidth, MaxItems * ItemHeight);
                    SecondMap = new Bitmap(maxWidth, MaxItems * ItemHeight);
                    break;
                }
                catch (OutOfMemoryException ex)
                {
                    if (MaxItems == 5)
                    {
                        throw new LowMemoryException();
                    }
                    MaxItems = MaxItems - 5;
                    if (MaxItems < 5) { MaxItems = 5; }
                }
                finally
                {
                    if (TestMap != null)
                    {
                        TestMap.Dispose();
                    }
                    if (SecondMap != null)
                    {
                        SecondMap.Dispose();
                    }
                }
            }
            ScreenMap.Dispose();
            AvatarMap.Dispose();
            System.Diagnostics.Debug.WriteLine("Portal size:" + MaxItems);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (MaxItems > 20)
            {
                ClientSettings.PortalSize = MaxItems;
                ClientSettings.SaveSettings();
            }
            _BitmapHeight = MaxItems * ItemHeight;
            _Rendered = new Bitmap(maxWidth, _BitmapHeight);
            
            _RenderedGraphics = Graphics.FromImage(_Rendered);
            
        }

        delegate void delNewArt(string User);
        void ThrottledArtGrabber_NewArtWasDownloaded(string User)
        {
            //Don't bother if it's in the middle of rendering
            if (_RenderThreads.Count > 0)
            {
                return;
            }
            try
            {
                lock (Items)
                {
                    lock (_RenderThreads)
                    {
                        _RenderThreads.Add(System.Threading.Thread.CurrentThread);
                        for (int i = 0; i < Items.Count; i++)
                        {
                            StatusItem s = (StatusItem)Items[i];
                            if (s.Tweet.user.screen_name.ToLower() == User)
                            {
                                Rectangle itemBounds = new Rectangle(0, ItemHeight * i, s.Bounds.Width, ItemHeight);
                                s.Render(_RenderedGraphics, itemBounds);
                            }
                            if (ClientSettings.ShowReplyImages)
                            {
                                if (!string.IsNullOrEmpty(s.Tweet.in_reply_to_user_id))
                                {
                                    string ReplyTo = s.Tweet.SplitLines[0].Split(new char[] { ' ' })[0].TrimEnd(StatusItem.IgnoredAtChars).TrimStart('@').ToLower();
                                    if (ReplyTo == User)
                                    {
                                        Rectangle itemBounds = new Rectangle(0, ItemHeight * i, s.Bounds.Width, ItemHeight);
                                        s.Render(_RenderedGraphics, itemBounds);
                                    }
                                }
                            }
                        }
                        _RenderThreads.Remove(System.Threading.Thread.CurrentThread);
                    }
                }
            }
            catch
            {
                return;
            }
            NewImage();
        }

        public void SetItemList(List<StatusItem> SetOfItems)
        {
            StatusItem FirstNewItem = SetOfItems[0];
            int SpacesMoved = 0;
            try
            {
                if (Items.Contains(FirstNewItem) && SetOfItems.Count > SpacesMoved)
                {
                    //Items added to the end
                    SpacesMoved = Items.IndexOf(FirstNewItem);
                    StatusItem[] ItemsToAdd = new StatusItem[SpacesMoved];
                    Array.Copy(SetOfItems.ToArray(), SetOfItems.Count - SpacesMoved, ItemsToAdd, 0, SpacesMoved);
                    System.Diagnostics.Debug.WriteLine("Blitting " + SpacesMoved + " to the end of the image.");
                    AddItemsToEnd(ItemsToAdd);
                    return;
                }
                else
                {
                    StatusItem LastNewItem = SetOfItems[SetOfItems.Count - 1];
                    if (Items.Contains(LastNewItem))
                    {
                        //Items added to the start
                        SpacesMoved = MaxItems - (Items.IndexOf(LastNewItem) + 1);
                        StatusItem[] ItemsToAdd = new StatusItem[SpacesMoved];
                        Array.Copy(SetOfItems.ToArray(), 0, ItemsToAdd, 0, SpacesMoved);
                        System.Diagnostics.Debug.WriteLine("Blitting " + SpacesMoved + " to the start of the image.");
                        AddItemsToStart(ItemsToAdd);
                        return;
                    }
                }
            }
            catch 
            {
            }
            System.Diagnostics.Debug.WriteLine("Jumped " + SpacesMoved + " spaces");
            Items.Clear();
            Items = new List<StatusItem>(SetOfItems);
            if (Items.Count > MaxItems)
            {
                Items.RemoveRange(MaxItems, Items.Count - MaxItems);
            }
            Rerender();
        }

        public void Clear()
        {
            Items.Clear();
            _RenderedGraphics.Clear(ClientSettings.BackColor);
        }

        public void AddItemsToStart(StatusItem[] Items)
        {
            for (int i = Items.Length - 1; i >= 0; i--)
            {
                AddItemToStart(Items[i]);
            }
            NewImage();
        }
        public void AddItemToStart(StatusItem Item)
        {
            Items.Insert(0, Item);
            if (Items.Count > MaxItems)
            {
                Items.RemoveAt(Items.Count - 1);
                Items.TrimExcess();
                RenderNewItemAtStart();
            }
        }
        public void AddItemsToEnd(StatusItem[] Items)
        {
            foreach (StatusItem Item in Items)
            {
                AddItemToEnd(Item);
            }
            NewImage();
        }
        public void AddItemToEnd(StatusItem Item)
        {
            Items.Add(Item);
            if (Items.Count > MaxItems)
            {
                Items.RemoveAt(0);
                Items.TrimExcess();
                RenderNewItemAtEnd();
            }
        }

        public void ReRenderItem(StatusItem Item)
        {
            if (Items.Contains(Item))
            {
                int i = Items.IndexOf(Item);
                Rectangle itemBounds = new Rectangle(0, ItemHeight * i, Item.Bounds.Width, ItemHeight);
                Item.Render(_RenderedGraphics, itemBounds);
            }
        }

        public void Rerender()
        {
            //Tell the portal to rerender in 3 seconds (unless it's interrupted again)
            //pauseBeforeStarting.Change(PauseBeforeRerender, System.Threading.Timeout.Infinite);
            RenderBackgroundHighPriority(null);
        }
        
        private delegate void delRender();
        private void RenderBackgroundLowPriority(object state)
        {
            System.Diagnostics.Debug.WriteLine("RenderBackground called");
            Render(System.Threading.ThreadPriority.AboveNormal);
        }
        private void RenderBackgroundHighPriority(object state)
        {
            System.Diagnostics.Debug.WriteLine("RenderBackground called");
            Render(System.Threading.ThreadPriority.Highest);
        }
        
        private void Render(System.Threading.ThreadPriority p)
        {

            while (_RenderThreads.Count > 0)
            {
                Thread t = _RenderThreads[0];
                cancelMyCurrentThread = true;
                t.Join();
                _RenderThreads.Remove(t);
            }
            cancelMyCurrentThread = false;
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                System.Threading.Thread.CurrentThread.Name = "Renderer";
                System.Threading.Thread.CurrentThread.Priority = p;
                _RenderThreads.Add(System.Threading.Thread.CurrentThread);
            }
            try
            {
                using (temp = new Bitmap(maxWidth, ItemHeight * MaxItems))
                {
                    using (Graphics g = Graphics.FromImage(temp))
                    {
                        lock (Items)
                        {
                            int StartItem = Math.Max(WindowOffset / ItemHeight,0);
                            int EndItem = StartItem + 4;
                            if (EndItem > Items.Count)
                            {
                                EndItem = Items.Count;
                                StartItem = Math.Max(EndItem - 4,0);
                            }
                            System.Diagnostics.Debug.WriteLine("Prioritize items " + StartItem + " to " + EndItem);
                            // Onscreen items first
                            for (int i = StartItem; i < EndItem; i++)
                            {
                                if (!cancelMyCurrentThread)
                                {
                                    DrawSingleItem(i, g);
                                }
                            }
                            for (int i = 0; i < StartItem; i++)
                            {
                                if (!cancelMyCurrentThread)
                                {
                                    DrawSingleItem(i, g);
                                }
                            }
                            for (int i = EndItem; i < Items.Count; i++)
                            {
                                if (!cancelMyCurrentThread)
                                {
                                    DrawSingleItem(i, g);
                                }
                            }


                            if (cancelMyCurrentThread)
                            {
                                _RenderThreads.Remove(System.Threading.Thread.CurrentThread);
                                return;
                            }
                            System.Diagnostics.Debug.WriteLine("Done rendering background");
                            _RenderedGraphics.DrawImage(temp, 0, 0);
                            NewImage();
                            _RenderThreads.Remove(System.Threading.Thread.CurrentThread);
                        }

                    }
                }
            }
            catch (OutOfMemoryException)
            {
                PanicMode();
                Rerender();
                //throw new LowMemoryException();
            }
        }

        private void PanicMode()
        {
            _RenderedGraphics.Dispose();
            _Rendered.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            MaxItems = MaxItems - 5;
            SetBufferSize();
        }

        private void DrawSingleItem(int i, Graphics g)
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
        private void RenderNewItemAtStart()
        {
            //Copy all but last item from top down one.
            IntPtr gPtr = _RenderedGraphics.GetHdc();

            BitBlt(gPtr, 0, ItemHeight, _Rendered.Width, _Rendered.Height - ItemHeight, gPtr, 0, 0, TernaryRasterOperations.SRCCOPY);
            _RenderedGraphics.ReleaseHdc(gPtr);
            //Draw the first item.
            StatusItem Item = Items[0];
            Rectangle ItemBounds = new Rectangle(0, 0, Item.Bounds.Width, Item.Bounds.Height);
            using (Pen whitePen = new Pen(ClientSettings.ForeColor))
            {
                _RenderedGraphics.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Top, ItemBounds.Right, ItemBounds.Top);
                _RenderedGraphics.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Bottom, ItemBounds.Right, ItemBounds.Bottom);
                _RenderedGraphics.DrawLine(whitePen, ItemBounds.Right, ItemBounds.Top, ItemBounds.Right, ItemBounds.Bottom);
            }
            Item.Render(_RenderedGraphics, ItemBounds);
        }
        private void RenderNewItemAtEnd()
        {
            //Copy all but first item from top up one.
            IntPtr gPtr = _RenderedGraphics.GetHdc();
            BitBlt(gPtr, 0, 0, _Rendered.Width, _Rendered.Height - ItemHeight, gPtr, 0, ItemHeight, TernaryRasterOperations.SRCCOPY);
            _RenderedGraphics.ReleaseHdc(gPtr);
            //Draw the last item.
            StatusItem Item = Items[Items.Count - 1];
            Rectangle ItemBounds = new Rectangle(0, (MaxItems - 1) * ItemHeight, Item.Bounds.Width, Item.Bounds.Height);
            using (Pen whitePen = new Pen(ClientSettings.ForeColor))
            {
                _RenderedGraphics.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Top, ItemBounds.Right, ItemBounds.Top);
                _RenderedGraphics.DrawLine(whitePen, ItemBounds.Left, ItemBounds.Bottom, ItemBounds.Right, ItemBounds.Bottom);
                _RenderedGraphics.DrawLine(whitePen, ItemBounds.Right, ItemBounds.Top, ItemBounds.Right, ItemBounds.Bottom);
            }
            Item.Render(_RenderedGraphics, ItemBounds);
        }
    }
}
