using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace FingerUI
{
    public class KListControl : UserControl
    {
        private class Velocity
        {
            private int _X = 0;
            public int X
            {
                get
                {
                    return _X;
                }
                set
                {
                    _X = value;
                }
            }
            private int _Y = 0;
            public int Y
            {
                get
                {
                    return _Y;
                }
                set
                {
                    _Y = value;
                }
            }
            

        }
        
		#region Fields (23) 
        private NotificationPopup NotificationArea = new NotificationPopup(); 
        private Font HighlightedFont;
        private PockeTwit.Clickables ClickablesControl = new PockeTwit.Clickables();
        private bool HasMoved = false;
        private bool InFocus = false;
        Graphics m_backBuffer;
        Bitmap flickerBuffer;
        // Background drawing
        Bitmap m_backBufferBitmap;
        int m_itemHeight = 40;
        ItemList m_items = new ItemList();
        Dictionary<string, ItemList> ItemLists = new Dictionary<string, ItemList>();
        int m_itemWidth = 240;
        // Properties
        int m_maxVelocity = 15;
        Point m_mouseDown = new Point(-1, -1);
        Point m_mousePrev = new Point(-1, -1);
        Point m_offset = new Point();
        bool m_scrollBarMove = false;

        int m_selectedIndex = 0;
        System.Threading.Timer m_timer;
        bool m_updating = false;
        private Velocity m_velocity = new Velocity();
        
        List<FingerUI.KListControl.IKListItem> OnScreenItems = new List<IKListItem>();

        public SideMenu LeftMenu = new SideMenu(SideShown.Left);
        public SideMenu RightMenu = new SideMenu(SideShown.Right);
		#endregion Fields 

		#region Enums (2) 

        enum XDirection
        {
            Left, Right
        }
        public enum SideShown
        {
            Left,
            Middle,
            Right
        }

		#endregion Enums 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="KListControl"/> class.
        /// </summary>
        public KListControl()
        {
            NotificationArea.TextFont = this.Font;
            NotificationArea.parentControl = this;
            CreateBackBuffer();
            SelectedFont = this.Font;
            HighlightedFont = this.Font;
            //m_timer.Interval = ClientSettings.AnimationInterval;
            //m_timer.Tick += new EventHandler(m_timer_Tick);

            ClickablesControl.Visible = false;
            ClickablesControl.WordClicked += new StatusItem.ClickedWordDelegate(ClickablesControl_WordClicked);

            //Need to repaint when fetching state has changed.
            PockeTwit.GlobalEventHandler.TimeLineDone += new PockeTwit.GlobalEventHandler.delTimelineIsDone(GlobalEventHandler_TimeLineDone);
            PockeTwit.GlobalEventHandler.TimeLineFetching += new PockeTwit.GlobalEventHandler.delTimelineIsFetching(GlobalEventHandler_TimeLineFetching);
            PockeTwit.ThrottledArtGrabber.NewArtWasDownloaded += new PockeTwit.ThrottledArtGrabber.ArtIsReady(ThrottledArtGrabber_NewArtWasDownloaded);
            m_timer = new System.Threading.Timer(new System.Threading.TimerCallback(m_timer_Tick), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }


        void ThrottledArtGrabber_NewArtWasDownloaded(string User)
        {
            if (InvokeRequired)
            {
                delMenuItemSelected d = new delMenuItemSelected(this.ThrottledArtGrabber_NewArtWasDownloaded);
                this.Invoke(d, User);
            }
            else
            {
                for (int i = 0; i < m_items.Count; i++)
                {
                    StatusItem s = (StatusItem)m_items[i];
                    if (s.Tweet.user.screen_name.ToLower() == User)
                    {
                        s.Render(m_backBuffer);
                        Invalidate();
                    }
                    if (ClientSettings.ShowReplyImages)
                    {
                        if (!string.IsNullOrEmpty(s.Tweet.in_reply_to_user_id))
                        {
                            string ReplyTo = s.Tweet.SplitLines[0].Split(new char[] { ' ' })[0].TrimEnd(StatusItem.IgnoredAtChars).TrimStart('@').ToLower();
                            if (ReplyTo == User)
                            {
                                s.Render(m_backBuffer);
                                Invalidate();
                            }
                        }
                    }
                }
            }
        }

        void GlobalEventHandler_TimeLineFetching(PockeTwit.TimelineManagement.TimeLineType TType)
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(this.Invalidate);
                this.Invoke(d);
            }
            else
            {
                this.Invalidate();
            }
        }

        void GlobalEventHandler_TimeLineDone(PockeTwit.TimelineManagement.TimeLineType TType)
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(this.Invalidate);
                this.Invoke(d);
            }
            else
            {
                this.Invalidate();
            }
        }


		#endregion Constructors 

		#region Properties (19) 
        public int Count
        {
            get
            {
                return m_items.Count;
            }
        }

        private SideShown CurrentlyViewing
        {
            get
            {
                if (m_offset.X < 0)
                {
                    return SideShown.Left;
                }
                else if (m_offset.X> 0)
                {
                    return SideShown.Right;
                }
                return SideShown.Middle;
            }
        }

        
        public bool IsMaximized { get; set; }
        
        public int ItemHeight
        {
            get
            {
                // In horizontal mode, we just use the full bounds, other modes use m_itemHeight.
                return  m_itemHeight;
            }
            set
            {
                m_itemHeight = value;
                CreateBackBuffer();
                Reset();
            }
        }

        public int ItemWidth
        {
            get
            {
                // In vertical mode, we just use the full bounds, other modes use m_itemWidth.
                return m_itemWidth;
            }
            set
            {
                m_itemWidth = value;
                Reset();
            }
        }

        public int MaxVelocity
        {
            get
            {
                return m_maxVelocity;
            }
            set
            {
                m_maxVelocity = value;
            }
        }

        private int MaxXOffset
        {
            get
            {
                //return Math.Max(((m_items.Count * ItemWidth)) - Bounds.Width, 0);
                return this.Width-50;
            }
        }

        private int MaxYOffset
        {
            get
            {
                return Math.Max(((m_items.Count * ItemHeight)) - Bounds.Height, 0);
            }
        }

        private int MinXOffset
        {
            get
            {
                return 0 - (this.Width - 50);
            }
        }

        
        public Font SelectedFont { get; set; }

        public int SelectedIndex
        {
            get
            {
                return m_selectedIndex;
            }
        }
        public IKListItem SelectedItem
        {
            get
            {
                lock (m_items)
                {
                    if (m_items.Count > 0)
                    {
                        return (IKListItem)m_items[m_selectedIndex];
                    }
                }
                return null;
            }
            set
            {
                m_items[m_selectedIndex].Selected = false;
                m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
                for(int i=0;i<m_items.Count;i++)
                {
                    IKListItem item = m_items[i];
                    if (item == value)
                    {
                        item.Selected = true;
                        m_selectedIndex = i;
                        m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }
            }
        }

        public IKListItem this[int index]
        {
            get
            {
                return m_items[index];
            }
        }

        
        public int YOffset
        {
            get
            {
                return m_offset.Y;
            }
            set
            {
                m_offset.Y = value;
            }
        }
        public int XOffset
        {
            get
            {
                return m_offset.X;
            }
            set
            {
                m_offset.X = value;
            }
        }

		#endregion Properties 

		#region Delegates and Events (9) 


		// Delegates (4) 

        public delegate void delAddItem(StatusItem item);
        public delegate void delClearMe();
        public delegate void delMenuItemSelected(string ItemName);
        public delegate void delSwitchState(bool IsMaximized);


		// Events (5) 

        public event delMenuItemSelected MenuItemSelected;

        public event EventHandler SelectedItemChanged;

        public event EventHandler SelectedItemClicked;

        public event delSwitchState SwitchWindowState;

        public event StatusItem.ClickedWordDelegate WordClicked;


		#endregion Delegates and Events 

		#region Methods (49) 


		// Public Methods (16) 

        private string _CurrentList = null;
        public string CurrentList()
        {
            return _CurrentList;
        }
        public void SwitchTolist(string ListName)
        {
            if (!ItemLists.ContainsKey(ListName))
            {
                ItemLists.Add(ListName, new ItemList());
            }
            _CurrentList = ListName;
            m_items = ItemLists[ListName];
            Reset();
        }

        public void AddItem(string text, object value)
        {
            
            KListItem item = new KListItem(this, text, value);
            item.Index = m_items.Count;
            AddItem(item);
        }

        public void AddItem(StatusItem item)
        {
            if (InvokeRequired)
            {
                delAddItem d = new delAddItem(AddItem);
                this.Invoke(d, new object[] { item });
            }
            else
            {
                item.Parent = this;
                item.Index = m_items.Count;
                item.ParentGraphics = m_backBuffer;
                AddItem((IKListItem)item);
            }
        }

        public void AddItem(IKListItem item)
        {
            item.Parent = this;
            item.Selected = false;
            item.Bounds = ItemBounds(0, item.Index);
            m_items.Add(item.Index, item);
            //Reset();
        }

        public void BeginUpdate()
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(BeginUpdate);
                this.Invoke(d, null);
            }
            else
            {
                m_updating = true;
            }
        }

        public void Clear()
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(Clear);
                this.Invoke(d, null);
            }
            else
            {
                foreach (ItemList items in this.ItemLists.Values)
                {
                    items.Clear();
                }
                Reset();
            }
        }
        public void ClearVisible()
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(ClearVisible);
                this.Invoke(d, null);
            }
            else
            {
                m_items.Clear();
                Reset();
            }
        }

        public void EndUpdate()
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(EndUpdate);
                this.Invoke(d, null);
            }
            else
            {
                m_updating = false;
                Reset();
            }
        }

        public void HookKey()
        {
            this.Parent.KeyDown += new KeyEventHandler(OnKeyDown);
        }

        public void Invalidate(IKListItem item)
        {
            Rectangle itemBounds = item.Bounds;
            itemBounds.Offset(-m_offset.X, -m_offset.Y);
            if (Bounds.IntersectsWith(itemBounds))
            {
                Invalidate(itemBounds);
            }
        }

        public void JumpToItem(object Value)
        {
            for (int i = 0; i < this.Count; i++)
            {
                IKListItem item = this[i];
                if (item.Value.ToString() == Value.ToString())
                {
                    JumpToItem(item);
                }
            }
        }

        public void JumpToItem(IKListItem item)
        {
            Rectangle VisibleBounds = new Rectangle(0, m_offset.Y, this.Width, this.Height);
            while (!VisibleBounds.Contains(item.Bounds))
            {
                if(item.Bounds.Top > VisibleBounds.Top)
                {
                    this.m_offset.Y  = this.m_offset.Y + ItemHeight;
                }
                else
                {
                    this.m_offset.Y = this.m_offset.Y - ItemHeight;
                }

                if(m_offset.Y<0){m_offset.Y=0;}
                if(m_offset.Y>(m_items.Values.Count-1)*ItemHeight){m_offset.Y=m_items.Values.Count*ItemHeight;}

                VisibleBounds = new Rectangle(0, m_offset.Y, this.Width, this.Height);
                Invalidate();
            }
        }

        public void Redraw()
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(Redraw);
                this.Invoke(d, null);
            }
            else
            {
                FillBuffer();
                this.Invalidate();
            }
        }

        public void RemoveItem(IKListItem item)
        {
            if (m_items.ContainsKey(item.Index))
            {
                m_items.Remove(item.Index);
            }
            Reset();
        }

        public void ResetHoriz()
        {
            m_offset.X = 0;
        }

        public void SetSelectedIndexToZero()
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(SetSelectedIndexToZero);
                this.Invoke(d, null);
            }
            else
            {
                if (m_items != null && m_items.Count>0)
                {
                    m_items[m_selectedIndex].Selected = false;
                    m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
                    m_selectedIndex = 0;
                    m_items[0].Selected = true;
                    m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
                    //FillBuffer();
                }
            }
        }

        public void SetSelectedMenu(string RequestedMenuItem)
        {
            if (RightMenu.Contains(RequestedMenuItem))
            {
                RightMenu.SelectedItem = RequestedMenuItem;
            }
            if (LeftMenu.Contains(RequestedMenuItem))
            {
                LeftMenu.SelectedItem = RequestedMenuItem;
            }
        }

        public void UnHookKey()
        {
            this.Parent.KeyDown -= new KeyEventHandler(OnKeyDown);
        }

		// Protected Methods (11) 

        protected override void Dispose(bool disposing)
        {
            CleanupBackBuffer();
            
            //m_timer.Enabled = false;
            base.Dispose(disposing);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            InFocus = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (InFocus)
            {
                OnKeyDown(null, e);
            }
        }

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (ClickablesControl.Visible)
            {
                ClickablesControl.KeyDown(e);
                Invalidate();
                return;
            }
            if (e.KeyCode == (Keys.LButton | Keys.MButton | Keys.Back))
            {
                switch (CurrentlyViewing)
                {
                    case SideShown.Left:
                        {
                            MenuItemSelected(LeftMenu.SelectedItem);
                            break;
                        }
                    case SideShown.Right:
                        {
                            MenuItemSelected(RightMenu.SelectedItem);
                            break;
                        }
                    case SideShown.Middle:
                        {
                            ShowClickablesControl();
                            break;
                        }
                }
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                if (CurrentlyViewing == SideShown.Middle)
                {
                    try
                    {
                        if (m_selectedIndex > 0)
                        {
                            UnselectCurrentItem();
                            m_selectedIndex--;
                            SelectAndJump();
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    if (CurrentlyViewing == SideShown.Right)
                    {
                        RightMenu.SelectUp();
                    }
                    if (CurrentlyViewing == SideShown.Left)
                    {
                        LeftMenu.SelectUp();
                    }
                }
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                if (CurrentlyViewing == SideShown.Middle)
                {
                    try
                    {
                        if (m_selectedIndex < m_items.Count - 1)
                        {
                            UnselectCurrentItem();
                            m_selectedIndex++;
                            SelectAndJump();
                        }
                    }
                    catch
                    {
                    }
                }
                if (CurrentlyViewing == SideShown.Right)
                {
                    RightMenu.SelectDown();
                }
                if (CurrentlyViewing == SideShown.Left)
                {
                    LeftMenu.SelectDown();
                }
            }
            if (e.KeyCode == Keys.Right | e.KeyCode == Keys.F2) 
            {
                if (CurrentlyViewing != SideShown.Right)
                {
                    SetRightMenuUser();
                    m_velocity.X = 15;
                    m_offset.X = m_offset.X + 3;
                    m_timer.Change(ClientSettings.AnimationInterval, ClientSettings.AnimationInterval);
                    //m_timer.Enabled = true;
                }
            }
            if (e.KeyCode == Keys.Left | e.KeyCode == Keys.F1)
            {
                if (CurrentlyViewing != SideShown.Left)
                {
                    m_velocity.X = -15;
                    m_offset.X = m_offset.X - 3;
                    //m_timer.Enabled = true;
                    m_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
            }
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            InFocus = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            HasMoved = false;
            //Fast scrolling on the right 10 pixels
            if (e.X > this.Width - 10)
            {
                m_scrollBarMove = true;
                return;
            }
            
            base.OnMouseDown(e);

            Capture = true;

            m_mouseDown.X = e.X;
            m_mouseDown.Y = e.Y;
            m_mousePrev = m_mouseDown;
            
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            
            base.OnMouseMove(e);
            
            if (e.Button == MouseButtons.Left)
            {
                //Fast scroll
                if (m_scrollBarMove)
                {
                    float ScrollPos = (float)e.Y/this.Height;
                    int MoveToPos = (int)Math.Round(MaxYOffset * ScrollPos);
                    
                    float Percentage = (float)m_offset.Y / MaxYOffset;

                    m_offset.Y = MoveToPos;
                    
                    m_velocity.X = 0;
                    m_velocity.Y = 0;
                    Invalidate();
                    return;
                }
                Point currPos = new Point(e.X, e.Y);

                int distanceX = m_mousePrev.X - currPos.X;
                if (distanceX > 3 & m_offset.X == 0) { SetRightMenuUser(); }
                
                int distanceY = m_mousePrev.Y - currPos.Y;
                //if we're primarily moving vertically, ignore horizontal movement.
                //It makes it "stick" to the middle better!
                if (m_offset.X==0 & Math.Abs(distanceX) < Math.Abs(distanceY))
                {
                    distanceX = 0;
                }

                m_velocity.X = distanceX / 2;
                m_velocity.Y = distanceY / 2;

                if (distanceX != 0 || distanceY != 0)
                {
                    HasMoved = true;
                }
                

                ClipVelocity();

                m_offset.Offset(distanceX, distanceY);
                ClipScrollPosition();

                m_mousePrev = currPos;

                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (m_offset.X > 15)
            {
                Rectangle cLocation = new Rectangle(this.Width - 15, 5, 10, 10);
                if (cLocation.Contains(new Point(e.X, e.Y)))
                {
                    IsMaximized = !IsMaximized;
                    if (SwitchWindowState != null)
                    {
                        SwitchWindowState(IsMaximized);
                    }
                }
            }

            //If we're fast-scrolling. stop
            if (m_scrollBarMove)
            {
                m_scrollBarMove = false;
                return;
            }
            // Did the click end on the same item it started on?
            bool sameX = Math.Abs(e.X - m_mouseDown.X) < m_itemWidth;
            bool sameY = Math.Abs(e.Y - m_mouseDown.Y) < m_itemHeight;

            if (sameY)
            {
                // Yes, so select that item or menuiten
                SelectItemOrMenu(e);
            }
            else
            {
                m_timer.Change(ClientSettings.AnimationInterval, ClientSettings.AnimationInterval);
                //m_timer.Enabled = true;
            }

            try
            {
                //Check if we're half-way to menu
                if (m_offset.X > 0 && m_offset.X <= this.Width)
                {
                    m_timer.Change(ClientSettings.AnimationInterval, ClientSettings.AnimationInterval);
                    //m_timer.Enabled = true;
                    if (m_offset.X > (this.Width * .6))
                    {
                        //Scroll to other side
                        m_velocity.X = 7;
                    }
                    else
                    {
                        m_velocity.X = -7;
                        //Scroll back
                    }
                }

                if (m_offset.X < 0 && m_offset.X >= 0 - this.Width)
                {
                    m_timer.Change(ClientSettings.AnimationInterval, ClientSettings.AnimationInterval);
                    //m_timer.Enabled = true;
                    if (m_offset.X < (0 - (this.Width * .6)))
                    {
                        //Scroll to other side
                        m_velocity.X = -7;
                    }
                    else
                    {
                        m_velocity.X = 7;
                        //Scroll back
                    }
                }

                m_mouseDown.Y = -1;
                Capture = false;

                Invalidate();
            }
            catch (ObjectDisposedException)
            { }
            if (!HasMoved)
            {
                CheckForClicks(new Point(e.X, e.Y));
            }
            HasMoved = false;
        }

        private void FillBuffer()
        {
            if (m_items.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("FillBuffer called with " + m_items.Count);
                lock (m_items)
                {
                    for (int i = 0; i < m_items.Count; i++)
                    {
                        IKListItem item = m_items[i];
                        Rectangle itemRect = item.Bounds;
                        using (Pen whitePen = new Pen(ClientSettings.ForeColor))
                        {
                            m_backBuffer.DrawLine(whitePen, itemRect.Left, itemRect.Top, itemRect.Right, itemRect.Top);
                            m_backBuffer.DrawLine(whitePen, itemRect.Left, itemRect.Bottom, itemRect.Right, itemRect.Bottom);
                            m_backBuffer.DrawLine(whitePen, itemRect.Right, itemRect.Top, itemRect.Right, itemRect.Bottom);
                        }
                        item.Render(m_backBuffer, itemRect);
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Graphics flickerGraphics = Graphics.FromImage(flickerBuffer))
            {
                OnScreenItems.Clear();
                flickerGraphics.Clear(ClientSettings.BackColor);
                flickerGraphics.DrawImage(m_backBufferBitmap, 0 - m_offset.X, 0 - m_offset.Y);
                if (m_offset.X > 0)
                {
                    DrawMenu(flickerGraphics, SideShown.Right);
                }
                else if (m_offset.X < 0)
                {
                    DrawMenu(flickerGraphics, SideShown.Left);
                }

                DrawPointer(flickerGraphics);
                if (PockeTwit.DetectDevice.DeviceType == PockeTwit.DeviceType.Professional && this.Width < this.Height)
                {
                    if (m_offset.X > 15)
                    {
                        if (IsMaximized)
                        {
                            DrawMaxWindowSwitcher(flickerGraphics);
                        }
                        else
                        {
                            DrawStandardWindowSwitcher(flickerGraphics);
                        }
                    }
                }

                if ((CurrentList() == "Friends_TimeLine" && PockeTwit.GlobalEventHandler.FriendsUpdating) || (CurrentList() == "Messages_TimeLine" && PockeTwit.GlobalEventHandler.MessagesUpdating))
                {
                    NotificationArea.ShowNotification();
                }
                else
                {
                    NotificationArea.HideNotification();
                }

                NotificationArea.DrawNotification(flickerGraphics, this.Bottom, this.Width);


                if (ClickablesControl.Visible)
                {
                    ClickablesControl.Render(flickerGraphics);
                }
                e.Graphics.DrawImage(flickerBuffer, 0, 0);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.Visible)
            {
                ClickablesControl.Top = this.Top + 20;
                ClickablesControl.Left = this.Left + 20;
                ClickablesControl.Width = this.Width - 40;
                ClickablesControl.Height = this.Height - 40;

                this.ItemWidth = this.Width;

                foreach (IKListItem item in m_items.Values)
                {
                    item.Bounds = ItemBounds(0, item.Index);
                }
                CreateBackBuffer();
                LeftMenu.Height = this.Height;
                LeftMenu.Width = this.Width;



                RightMenu.Height = this.Height;
                RightMenu.Width = this.Width;
                Reset();
            }
        }



		// Private Methods (22) 

        private void CheckForClicks(Point point)
        {
            try
            {
                int itemNumber = FindIndex(point.X, point.Y).Y;
                StatusItem s = (StatusItem)m_items[itemNumber];

                foreach (StatusItem.Clickable c in s.Tweet.Clickables)
                {
                    Rectangle itemRect = s.Bounds;
                    itemRect.Offset(-m_offset.X, -m_offset.Y);
                    Rectangle cRect = new Rectangle(((int)c.Location.X + itemRect.Left) + (ClientSettings.SmallArtSize + 10), (int)c.Location.Y + itemRect.Top, (int)c.Location.Width, (int)c.Location.Height);
                    if (cRect.Contains(point))
                    {
                        if (WordClicked != null)
                        {
                            WordClicked(c.Text);
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                //Oops, we're closing shop.
            }
        }

        private void CleanupBackBuffer()
        {
            if (m_backBufferBitmap != null)
            {
                m_backBufferBitmap.Dispose();
                m_backBufferBitmap = null;
                m_backBuffer.Dispose();
                m_backBuffer = null;
            }
            if (flickerBuffer != null)
            {
                flickerBuffer.Dispose();
                flickerBuffer = null;
            }
        }


        void ClickablesControl_WordClicked(string TextClicked)
        {
            if (TextClicked == "Full Text" | Yedda.ShortText.isShortTextURL(TextClicked))
            {
                //Show the full tweet somehow.
                StatusItem s = (StatusItem)SelectedItem;
                string fullText = null;
                if (Yedda.ShortText.isShortTextURL(s.Tweet.text))
                {
                    string[] splitup = s.Tweet.text.Split(new char[] { ' ' });
                    fullText = Yedda.ShortText.getFullText(splitup[splitup.Length - 1]);
                }
                else
                {
                    fullText = s.Tweet.text;   
                }
                MessageBox.Show(fullText, s.Tweet.user.screen_name);
            }
            else if (WordClicked != null)
            {
                WordClicked(TextClicked);
            }
        }

        private void ClipScrollPosition()
        {
            if (m_offset.X < MinXOffset)
            {
                m_offset.X = MinXOffset;
                m_velocity.X = 0;
            }
            else if (m_offset.X > MaxXOffset)
            {
                m_offset.X = MaxXOffset;
                m_velocity.X = 0;
            }
            if (m_offset.Y < 0)
            {
                m_offset.Y = 0;
                m_velocity.Y = 0;
            }
            else if (m_offset.Y > MaxYOffset)
            {
                m_offset.Y = MaxYOffset;
                m_velocity.Y = 0;
            }
        }

        private void ClipVelocity()
        {
            m_velocity.X = Math.Min(m_velocity.X, m_maxVelocity);
            m_velocity.X = Math.Max(m_velocity.X, -m_maxVelocity);

            m_velocity.Y = Math.Min(m_velocity.Y, m_maxVelocity);
            m_velocity.Y = Math.Max(m_velocity.Y, -m_maxVelocity);
        }

        private void CreateBackBuffer()
        {
            CleanupBackBuffer();
            flickerBuffer = new Bitmap(this.Width, this.Height);
            m_backBufferBitmap = new Bitmap(this.Width, this.ItemHeight*ClientSettings.MaxTweets);
            m_backBuffer = Graphics.FromImage(m_backBufferBitmap);
            foreach (IKListItem item in m_items.Values)
            {
                if (item is StatusItem)
                {
                    StatusItem sItem = (StatusItem)item;
                    sItem.ParentGraphics = m_backBuffer;
                }
            }
        }

        private void DrawItems()
        {
            if (m_backBuffer != null)
            {
                m_backBuffer.Clear(ClientSettings.BackColor);

                
                ItemList.Enumerator yEnumerator = m_items.GetEnumerator();
                bool moreY = yEnumerator.MoveNext();
                
                while (moreY)
                {
                    IKListItem item = yEnumerator.Current.Value;
                    if (item != null)
                    {
                        Rectangle itemRect = item.Bounds;
                        itemRect.Offset(-m_offset.X, -m_offset.Y);
                        
                        //Draw borders

                        using (Pen whitePen = new Pen(ClientSettings.ForeColor))
                        {
                            m_backBuffer.DrawLine(whitePen, itemRect.Left, itemRect.Top, itemRect.Right, itemRect.Top);
                            m_backBuffer.DrawLine(whitePen, itemRect.Left, itemRect.Bottom, itemRect.Right, itemRect.Bottom);
                            m_backBuffer.DrawLine(whitePen, itemRect.Right, itemRect.Top, itemRect.Right, itemRect.Bottom);
                        }
                        item.Render(m_backBuffer, itemRect);
                    }

                    moreY = yEnumerator.MoveNext();
                }

                DrawPointer(m_backBuffer);
            }
            
        }

        private void DrawMenu(Graphics m_backBuffer, SideShown Side)
        {
            Bitmap MenuMap;
            if (Side == SideShown.Left)
            {
                MenuMap = LeftMenu.Rendered;
                m_backBuffer.DrawImage(MenuMap, (0 - this.Width) + Math.Abs(m_offset.X), 0);
            }
            else if (Side == SideShown.Right)
            {
                MenuMap = RightMenu.Rendered;
                m_backBuffer.DrawImage(MenuMap, this.Width - m_offset.X, 0);
            }
            
        }

        private void DrawMaxWindowSwitcher(Graphics g)
        {
            using (Pen sPen = new Pen(ClientSettings.ForeColor))
            {
                Rectangle cLocation = new Rectangle(this.Width - 15, 5, 10, 10);
                Rectangle cInterior = new Rectangle(this.Width - 13, 7, 6, 6);
                g.DrawRectangle(sPen, cLocation);
                sPen.Width = 1;
                g.DrawLine(sPen, cInterior.Left, cInterior.Bottom, cInterior.Right, cInterior.Bottom);
            }
        }

        private void DrawPointer(Graphics g)
        {
            float Percentage = 0;
            if (m_offset.Y > 0)
            {
                Percentage = (float)m_offset.Y / MaxYOffset;
            }
            int Position = (int)Math.Round(Height * Percentage);
            using (SolidBrush SBrush = new SolidBrush(ClientSettings.ForeColor))
            {
                Point a = new Point(Width - 10, Position);
                Point b = new Point(Width, Position - 5);
                Point c = new Point(Width, Position + 5);
                Point[] Triangle = new Point[]{a,b,c};
                g.FillPolygon(SBrush, Triangle);
            }
            

        }


        private void DrawStandardWindowSwitcher(Graphics g)
        {
            using(Pen sPen = new Pen(ClientSettings.ForeColor))
            {
                sPen.Width = 2;
                Rectangle cLocation = new Rectangle(this.Width - 15, 5, 10, 10);
                Rectangle cInterior = new Rectangle(this.Width - 13, 7, 6, 6);
                g.DrawRectangle(sPen, cLocation);
                sPen.Width = 1;
                g.DrawRectangle(sPen, cInterior);
            }
        }

        private Point FindIndex(int x, int y)
        {
            Point index = new Point(0, 0);

            index.Y = ((y + m_offset.Y - Bounds.Top) / (m_itemHeight));
            
            return index;
        }

        private string GetMenuItemForPoint(MouseEventArgs e)
        {
            Point X = new Point(e.X, e.Y);
            
            int LeftOfItem = this.Width - Math.Abs(m_offset.X);
            int MenuHeight;
            int TopOfItem;
            SideMenu MenuToCheck = null;
            if (m_offset.X > 0)
            {
                MenuToCheck = RightMenu;
                MenuHeight = RightMenu.ItemHeight;
                TopOfItem = RightMenu.TopOfMenu;
            }
            else if (m_offset.X < 0)
            {
                MenuToCheck = LeftMenu;
            }
            MenuHeight = MenuToCheck.ItemHeight;
            TopOfItem = MenuToCheck.TopOfMenu;
            foreach (string MenuItem in MenuToCheck.GetItems())
            {
                Rectangle menuRect = new Rectangle(LeftOfItem, TopOfItem, ItemWidth, MenuHeight);
                TopOfItem = TopOfItem + MenuHeight;
                if (menuRect.Contains(X))
                {
                    Invalidate(menuRect);
                    return MenuItem;
                }
            }
            return null;
        }

        private Rectangle ItemBounds(int x, int y)
        {
            int itemY = Bounds.Top + (m_itemHeight * y);

            return new Rectangle(Bounds.Left, itemY, ItemWidth, ItemHeight);
            
        }

        private void m_timer_Tick(object sender)
        {
            
            if (!Capture && (m_velocity.Y != 0 || m_velocity.X != 0))
            {
                XDirection dir = m_velocity.X > 0 ? XDirection.Right : XDirection.Left;
                XDirection currentPos = m_offset.X > 0 ? XDirection.Right : XDirection.Left;

                m_offset.Offset(m_velocity.X, m_velocity.Y);

                if (currentPos == XDirection.Right & dir == XDirection.Left)
                {
                    if (m_offset.X <= 0)
                    {
                        m_offset.X = 0;
                        m_velocity.X = 0;
                    }
                }
                else if (currentPos == XDirection.Left & dir == XDirection.Right)
                {
                    if (m_offset.X >= 0)
                    {
                        m_offset.X = 0;
                        m_velocity.X = 0;
                    }
                }


                

                ClipScrollPosition();
                
                // Slow down
                if (m_velocity.Y < 0)
                {
                    m_velocity.Y++;
                }
                else if (m_velocity.Y > 0)
                {
                    m_velocity.Y--;
                }
                if (m_velocity.Y == 0 && m_velocity.X == 0)
                {
                    //m_timer.Enabled = false;
                    m_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    HasMoved = false;
                }

                Invalidate();
            }
        }

        private void Reset()
        {
            if (!m_updating)
            {
                m_backBuffer.Clear(ClientSettings.BackColor);
                m_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                //m_timer.Enabled = false;
                if (m_items.Count > 0)
                {
                    m_items[m_selectedIndex].Selected = false;
                    m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
                }
                m_selectedIndex = 0;
                Capture = false;
                m_velocity.X = 0;
                m_velocity.Y = 0;
                m_offset.Y = 0;
                FillBuffer();
                SetSelectedIndexToZero();
                Invalidate();
            }
        }

        private void SetRightMenuUser()
        {
            System.Diagnostics.Debug.WriteLine("RightMenuSet called");
            if (CurrentlyViewing != SideShown.Left)
            {
                StatusItem s = (StatusItem)m_items[m_selectedIndex];
                RightMenu.UserName = s.Tweet.user.screen_name;
            }
        }

        private void SelectAndJump()
        {
            //m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
            IKListItem item = m_items[m_selectedIndex];
            item.Selected = true;
            m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, new EventArgs());
            }
            JumpToItem(item);
        }

        private void SelectItemOrMenu(MouseEventArgs e)
        {
            if (e.X > this.Width-m_offset.X)
            {
                //MenuItem selected
                if(MenuItemSelected!=null)
                {
                    string ItemName = GetMenuItemForPoint(e);
                    if (!String.IsNullOrEmpty(ItemName))
                    {
                        MenuItemSelected(ItemName);
                    }
                }
            }
            else if ((m_offset.X) < 0 && e.X<Math.Abs(m_offset.X))
            {
                if (MenuItemSelected != null)
                {
                    string ItemName = GetMenuItemForPoint(e);
                    if (!String.IsNullOrEmpty(ItemName))
                    {
                        MenuItemSelected(ItemName);
                    }
                }
            }
            else
            {
                Point selectedIndex = FindIndex(e.X, e.Y);
                if (selectedIndex .Y!= m_selectedIndex)
                {
                    if (m_items.ContainsKey(selectedIndex.Y))
                    {
                        m_items[m_selectedIndex].Selected = false;
                        m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
                        m_selectedIndex = selectedIndex.Y;
                        m_items[m_selectedIndex].Selected = true;
                        m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
                        if (SelectedItemChanged != null)
                        {
                            SelectedItemChanged(this, new EventArgs());
                        }
                        if (CurrentlyViewing == SideShown.Right)
                        {
                            SetRightMenuUser();
                        }
                    }
                }
                else
                {
                    if (SelectedItemClicked != null)
                    {
                        SelectedItemClicked(this, new EventArgs());
                    }
                }
            }
            m_velocity.X = 0;
            m_velocity.Y = 0;
        }

        private void ShowClickablesControl()
        {
            StatusItem s = (StatusItem)m_items[m_selectedIndex];
            if (s == null) { return; }
            ClickablesControl.Items = s.Tweet.Clickables;
            if (s.Tweet.Clipped)
            {
                ClickablesControl.ShowClipped = true;
            }
            ClickablesControl.Visible = true;
        }

        private void UnselectCurrentItem()
        {
            if (m_selectedIndex >= 0)
            {
                IKListItem item = m_items[m_selectedIndex];
                item.Selected = false;
                m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
            }
        }


		#endregion Methods 

		#region Nested Classes (1) 
        class ItemList : Dictionary<int, IKListItem>
        {
        }
		#endregion Nested Classes 
        public interface IKListItem
        {
            /// <summary>
            /// Gets or sets the parent.
            /// </summary>
            /// <value>The parent.</value>
            KListControl Parent { get; set; }

            /// <summary>
            /// The unscrolled bounds for this item.
            /// </summary>
            Rectangle Bounds { get; set; }

            
            /// <summary>
            /// Gets or sets the Y.
            /// </summary>
            /// <value>The Y.</value>
            int Index { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="IKListItem"/> is selected.
            /// </summary>
            /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
            bool Selected { get; set; }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            /// <value>The text.</value>
            string Text { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            object Value { get; set; }

            /// <summary>
            /// Renders the specified graphics object.
            /// </summary>
            /// <param name="g">The graphics.</param>
            /// <param name="bounds">The bounds.</param>
            void Render(Graphics g, Rectangle bounds);
        }

    }
}
