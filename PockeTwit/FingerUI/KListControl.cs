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
        private FullScreenTweet fsDisplay = new FullScreenTweet();
        private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();

        private class Velocity
        {
            public delegate void delStoppedMoving();
            public event delStoppedMoving StoppedMoving = delegate { };

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
                    bool bRaiseStop = false;
                    bRaiseStop = (_Y != value) && (value == 0);
                        
                    _Y = value;
                    if (bRaiseStop)
                    {
                        StoppedMoving();
                    }
                }
            }
            

        }
        
		#region Fields (23) 

        private bool menuwasClicked = false;
        private Portal SlidingPortal = new Portal();
        private Popup NotificationArea = new Popup();
        private Popup ErrorPopup = new Popup();
        private PockeTwit.Clickables ClickablesControl = new PockeTwit.Clickables();
        private bool HasMoved = false;
        private bool InFocus = false;
        // Background drawing
        
        public ItemList m_items = new ItemList();
        Dictionary<string, ItemList> ItemLists = new Dictionary<string, ItemList>();
        
        int m_itemWidth = 240;
        // Properties
        int m_maxVelocity = 45;
        Point m_mouseDown = new Point(-1, -1);
        Point m_mousePrev = new Point(-1, -1);
        Point m_offset = new Point();
        
        bool m_scrollBarMove = false;

        int m_selectedIndex = 0;
        Timer m_timer = new Timer();
        public bool Startup = true;
        private Velocity m_velocity = new Velocity();
        
        
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

        public KListControl()
        {
            animationTimer.Interval = ClientSettings.AnimationInterval;
            animationTimer.Tick += new EventHandler(animationTimer_Tick);

            NotificationArea.TextFont = ClientSettings.TextFont;
            NotificationArea.parentControl = this;

            ErrorPopup.TextFont = ClientSettings.TextFont;
            ErrorPopup.parentControl = this;
            ErrorPopup.AtTop = true;

            SelectedFont = ClientSettings.TextFont;
            m_timer.Interval = ClientSettings.AnimationInterval;
            m_timer.Tick += new EventHandler(m_timer_Tick);

            ClickablesControl.Visible = false;
            ClickablesControl.WordClicked += new StatusItem.ClickedWordDelegate(ClickablesControl_WordClicked);

            //Need to repaint when fetching state has changed.
            PockeTwit.GlobalEventHandler.TimeLineDone += new PockeTwit.GlobalEventHandler.delTimelineIsDone(GlobalEventHandler_TimeLineDone);
            PockeTwit.GlobalEventHandler.TimeLineFetching += new PockeTwit.GlobalEventHandler.delTimelineIsFetching(GlobalEventHandler_TimeLineFetching);

            SlidingPortal.NewImage += new Portal.delNewImage(SlidingPortal_NewImage);

            m_velocity.StoppedMoving += new Velocity.delStoppedMoving(m_velocity_StoppedMoving);

            fsDisplay.Visible = false;
            fsDisplay.Dock = DockStyle.Fill;
            this.Controls.Add(fsDisplay);

            PockeTwit.GlobalEventHandler.ShowErrorMessage += new PockeTwit.GlobalEventHandler.delshowErrorMessage(GlobalEventHandler_ShowErrorMessage);
        }

        void GlobalEventHandler_ShowErrorMessage(string Message)
        {
            ErrorPopup.ShowNotification(Message);
            ErrorPopup.Pause = 50;
        }

        void SlidingPortal_NewImage()
        {
            SlidingPortalOffset = YOffset - (itemsBeforePortal * ClientSettings.ItemHeight);
            SlidingPortal.WindowOffset = SlidingPortalOffset;
            Repaint();
        }

        void m_velocity_StoppedMoving()
        {
            RerenderPortal();
        }

        int itemsBeforePortal = 0;
        int previousItemsBeforePortal = 0;

        
        public void RerenderPortal()
        {
            if (!Capture && m_velocity.Y == 0 && m_velocity.X==0)
            {
                if (m_items.Count > SlidingPortal.MaxItems)
                {
                    RecalculatePortalPosition();
                }
            }
        }

        private void RecalculatePortalPosition()
        {
            lock (m_items)
            {
                int itemsBeforeScreen = YOffset / ClientSettings.ItemHeight;

                itemsBeforePortal = itemsBeforeScreen - (SlidingPortal.MaxItems / 2);
                int itemsAfterPortal = (m_items.Count - itemsBeforeScreen) - (SlidingPortal.MaxItems / 2) - 1;
                if (itemsAfterPortal < 0)
                {
                    itemsBeforePortal = itemsBeforePortal + itemsAfterPortal;
                }
                if (itemsBeforePortal < 0) { itemsBeforePortal = 0; }
                List<StatusItem> NewSet = new List<StatusItem>();
                int MaxSize = Math.Min(itemsBeforePortal + SlidingPortal.MaxItems, m_items.Count);
                for (int i = itemsBeforePortal; i < MaxSize; i++)
                {
                    NewSet.Add(m_items[i]);
                }
                //if (previousItemsBeforePortal != itemsBeforePortal | Force)
                //{
                previousItemsBeforePortal = itemsBeforePortal;
                SlidingPortal.SetItemList(NewSet);
                //}
            }
        }
        public void ResetFullScreenColors()
        {
            fsDisplay = new FullScreenTweet();
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
        
        private Point OldSize;

        private bool _Visible;
        public new bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (value && !_Visible)
                {
                    if (OldSize != new Point(this.Width, this.Height))
                    {
                        Application.DoEvents();
                        RerenderBySize();
                    }
                }
                OldSize = new Point(this.Width, this.Height);
                _Visible = value;
                base.Visible = value;
            }
        }

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
                if (XOffset < 0)
                {
                    return SideShown.Left;
                }
                else if (XOffset> 0)
                {
                    return SideShown.Right;
                }
                return SideShown.Middle;
            }
        }

        
        public bool IsMaximized { get; set; }
        
        
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
                //this.Redraw();
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
                return Math.Max(((m_items.Count * ClientSettings.ItemHeight)) - Bounds.Height, 0);
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
        public StatusItem SelectedItem
        {
            get
            {
                if (m_items.Count > 0)
                {
                    return m_items[m_selectedIndex];
                }
                
                return null;
            }
            set
            {
                m_items[m_selectedIndex].Selected = false;
                SlidingPortal.ReRenderItem(m_items[m_selectedIndex]);
                for(int i=0;i<m_items.Count;i++)
                {
                    StatusItem item = m_items[i];
                    if (item == value)
                    {
                        item.Selected = true;
                        m_selectedIndex = i;
                        PockeTwit.LastSelectedItems.SetLastSelected(CurrentList(), item.Tweet.id);
                        SlidingPortal.ReRenderItem(m_items[m_selectedIndex]);
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }
            }
        }

        public StatusItem this[int index]
        {
            get
            {
                if (m_items.Count <= index)
                {
                    return m_items[m_items.Count - 1];
                }
                return m_items[index];
            }
        }

        private int SlidingPortalOffset = 0;
        private int SlidingPortalCurrentMin;
        private int SlidingPortalCurrentEnd;
        private int SlidingPortalSpaces = 0;

        public int YOffset
        {
            get
            {
                return m_offset.Y;
            }
            set
            {
                m_offset.Y = value;

                SlidingPortalOffset = YOffset - (itemsBeforePortal * ClientSettings.ItemHeight);
                SlidingPortal.WindowOffset = SlidingPortalOffset;
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
        void animationTimer_Tick(object sender, EventArgs e)
        {
            if (!NotificationArea.isAnimating && !ErrorPopup.isAnimating)
            {
                animationTimer.Enabled = false;
            }
            this.Repaint();
        }

        public void startAnimation()
        {
            this.animationTimer.Enabled = true;
        }

        /*
        public void stopAnimation()
        {
            if (!NotificationArea.isAnimating && !ErrorPopup.isAnimating)
            {
                animationTimer.Enabled = false;
            }
        }
         */
		// Public Methods (16) 

        private string _CurrentList = null;
        public string CurrentList()
        {
            return _CurrentList;
        }
        public void SwitchTolist(string ListName)
        {
            string thisList = CurrentList();
            if (!string.IsNullOrEmpty(thisList))
            {
                if (SelectedItem != null)
                {
                    PockeTwit.LastSelectedItems.SetLastSelected(thisList, SelectedItem.Tweet.id);
                }
            }
            if (!ItemLists.ContainsKey(ListName))
            {
                ItemLists.Add(ListName, new ItemList());
            }
            _CurrentList = ListName;
            m_items = ItemLists[ListName];
            Reset();
        }

        public void JumpToLastSelected()
        {


            string jumpID = PockeTwit.LastSelectedItems.GetLastSelected(CurrentList());
            if (!string.IsNullOrEmpty(jumpID))
            {
                for (int i = 0; i < m_items.Count; i++)
                {
                    if (m_items[i].Tweet.id == jumpID)
                    {
                        m_selectedIndex = i;
                        SelectAndJump();
                        return;
                    }
                }
            }
            SetSelectedIndexToZero();
        }

        public void AddItem(string text, object value)
        {
            
            StatusItem item = new StatusItem(this, text, value);
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
                item.ParentGraphics = SlidingPortal._RenderedGraphics;
                item.Selected = false;
                item.Bounds = ItemBounds(0, item.Index);
                m_items.Add(item.Index, item);
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

        public void HookKey()
        {
            this.Parent.KeyDown += new KeyEventHandler(OnKeyDown);
            this.Parent.KeyPress += new KeyPressEventHandler(OnKeyPress);
        }
        
        public void JumpToItem(object Value)
        {
            for (int i = 0; i < this.Count; i++)
            {
                StatusItem item = this[i];
                if (item.Value.ToString() == Value.ToString())
                {
                    JumpToItem(item);
                }
            }
        }

        public void JumpToItem(StatusItem item)
        {
            Rectangle VisibleBounds = new Rectangle(0, YOffset, this.Width, this.Height);
            while (!VisibleBounds.Contains(item.Bounds))
            {
                if(item.Bounds.Top > VisibleBounds.Top)
                {
                    YOffset  = YOffset + ClientSettings.ItemHeight;
                }
                else
                {
                    YOffset = YOffset - ClientSettings.ItemHeight;
                }

                if (YOffset < 0) { YOffset = 0; }
                if (YOffset > (m_items.Values.Count - 1) * ClientSettings.ItemHeight) { YOffset = m_items.Values.Count * ClientSettings.ItemHeight; }

                VisibleBounds = new Rectangle(0, YOffset, this.Width, this.Height);
            }
            Invalidate();
        }

        public void Repaint()
        {
            if (InvokeRequired)
            {
                delClearMe d = new delClearMe(Repaint);
                this.Invoke(d, null);
            }
            else
            {
                this.Invalidate();
            }
        }

        public void Redraw()
        {
            if (!Startup)
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
        }

        public void RemoveItem(StatusItem item)
        {
            if (m_items.ContainsKey(item.Index))
            {
                m_items.Remove(item.Index);
            }
            Reset();
        }

        public void ResetHoriz()
        {
            XOffset = 0;
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
                    SlidingPortal.ReRenderItem(m_items[m_selectedIndex]);
                    m_selectedIndex = 0;
                    m_items[0].Selected = true;
                    SlidingPortal.ReRenderItem(m_items[m_selectedIndex]);
                    StatusItem s = (StatusItem)SelectedItem;
                    RightMenu.UserName = s.Tweet.user.screen_name;
                    PockeTwit.LastSelectedItems.SetLastSelected(CurrentList(), m_items[0].Tweet.id);
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
            this.Parent.KeyPress -= new KeyPressEventHandler(OnKeyPress);
        }

		// Protected Methods (11) 

        protected override void Dispose(bool disposing)
        {
            m_timer.Enabled = false;
            base.Dispose(disposing);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            InFocus = true;
        }


        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            OnKeyPress(null, e);
        }

        protected void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (LeftMenu.Contains("Back"))
            {
                if (e.KeyChar == (Char)Keys.Escape)
                {
                    this.MenuItemSelected("Back");
                }
            }
            e.Handled = true;
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
                this.Invalidate();
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
                            if (fsDisplay.Visible) { fsDisplay.Visible = false; }
                            else
                            {
                                ShowClickablesControl();
                            }
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
                            SetRightMenuUser();
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
                            SetRightMenuUser();
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
                if (fsDisplay.Visible) 
                {
                    fsDisplay.FontSize++;
                    return; 
                }
                if (CurrentlyViewing != SideShown.Right)
                {
                    m_velocity.X = (this.Width / 2);
                    XOffset =  XOffset + 3;
                    m_timer.Enabled = true;
                }
            }
            if (e.KeyCode == Keys.Left | e.KeyCode == Keys.F1)
            {
                if (fsDisplay.Visible) 
                {
                    fsDisplay.FontSize--;
                    return; 
                }
                if (CurrentlyViewing != SideShown.Left)
                {
                    m_velocity.X = -(this.Width / 2);
                    XOffset = XOffset - 3;
                    m_timer.Enabled = true;
                }
            }
            if (PockeTwit.DetectDevice.DeviceType == PockeTwit.DeviceType.Standard)
            {
                int KeyToCheck = (e.KeyValue - 48);
                if (0 <= KeyToCheck && KeyToCheck <= 9)
                {
                    if (CurrentlyViewing == SideShown.Left)
                    {
                        LeftMenu.SelectByNumber(KeyToCheck-1);
                        MenuItemSelected(LeftMenu.SelectedItem);
                    }
                    else if (CurrentlyViewing == SideShown.Right)
                    {
                        RightMenu.SelectByNumber(KeyToCheck-1);
                        MenuItemSelected(RightMenu.SelectedItem);
                    }
                }
            }
            this.Refresh();
            
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            fsDisplay.Visible = false;
            InFocus = false;
        }

        private long ticks = 0;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            HasMoved = false;
            //Fast scrolling on the right 10 pixels
            if (e.X > this.Width - PointerSize)
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
                    
                    float Percentage = (float)YOffset / MaxYOffset;

                    YOffset = MoveToPos;
                    
                    m_velocity.X = 0;
                    m_velocity.Y = 0;
                    Invalidate();
                    return;
                }
                Point currPos = new Point(e.X, e.Y);

                int distanceX = m_mousePrev.X - currPos.X;
                
                int distanceY = m_mousePrev.Y - currPos.Y;
                //if we're primarily moving vertically, ignore horizontal movement.
                //It makes it "stick" to the middle better!
                if (XOffset==0 & Math.Abs(distanceX) < Math.Abs(distanceY))
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

                XOffset = XOffset + distanceX;
                YOffset = YOffset + distanceY;
                //m_offset.Offset(distanceX, distanceY);
                ClipScrollPosition();

                m_mousePrev = currPos;

                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (ClickablesControl.Visible)
            {
                ClickablesControl.CheckForClicks(new Point(e.X, e.Y));
                Invalidate();
                return;
            }

            if (XOffset > 15)
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
                RerenderPortal();
                return;
            }
            // Did the click end on the same item it started on?
            bool sameX = Math.Abs(e.X - m_mouseDown.X) < m_itemWidth;
            bool sameY = Math.Abs(e.Y - m_mouseDown.Y) < ClientSettings.ItemHeight;

            if (sameY)
            {
                // Yes, so select that item or menuiten
                if (Math.Abs(e.X - m_mouseDown.X) < MaxVelocity)
                {
                    int OldSelected = m_selectedIndex;
                    SelectItemOrMenu(e);
                    //Check for double-click!
                    if (m_selectedIndex == OldSelected)
                    {
                        long NowTicks = DateTime.Now.Ticks;
                        if ((NowTicks - ticks) < new TimeSpan(0, 0, 0, 0, 500).Ticks)
                        {
                            ShowClickablesControl();
                            return;
                        }
                        ticks = NowTicks;
                    }
                    Invalidate();
                    if (menuwasClicked)
                    {
                        menuwasClicked = false;
                        return;
                    }
                }
            }
            else
            {
                m_timer.Enabled = true;
            }

            try
            {
                //Check if we're half-way to menu
                if (XOffset > 0 && XOffset <= this.Width)
                {
                    m_timer.Enabled = true;
                    if (XOffset > (this.Width * .6))
                    {
                        //Scroll to other side
                        m_velocity.X = (this.Width / 10);
                    }
                    else
                    {
                        m_velocity.X = -(this.Width / 10);
                        //Scroll back
                    }
                }

                if (XOffset < 0 && XOffset >= 0 - this.Width)
                {
                    m_timer.Enabled = true;
                    if (XOffset < (0 - (this.Width * .6)))
                    {
                        //Scroll to other side
                        m_velocity.X = -(this.Width / 10);
                    }
                    else
                    {
                        m_velocity.X = (this.Width / 10);
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
            if (HasMoved)
            {
                RerenderPortal();
            }
        }
        

        public void SnapBack()
        {
            if (CurrentlyViewing == SideShown.Right)
            {
                Capture = false;
                m_timer.Enabled = true;
                m_velocity.X = -(this.Width / 10);
            }
            else if (CurrentlyViewing == SideShown.Left)
            {
                Capture = false;
                m_timer.Enabled = true;
                m_velocity.X = (this.Width / 10);
            }
        }

        private void FillBuffer()
        {
            if (m_items.Count > 0)
            {
                /*
                FillImmediateBuffer();
                //I want to do this async, but I'm running into race conditions right now.
                FillBackBuffer(null);
                 */
                SlidingPortal.SetItemList(new List<StatusItem>(m_items.Values));
                SlidingPortalCurrentMin = 0;
                SlidingPortalCurrentEnd = SlidingPortal.MaxItems;
                //SlidingPortal.RenderImmediately();
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            if (NotificationArea.isAnimating || ErrorPopup.isAnimating)
            {
                animationTimer.Enabled = true;
            }
            using (Image flickerBuffer = new Bitmap(this.Width, this.Height))
            {

                using (Graphics flickerGraphics = Graphics.FromImage(flickerBuffer))
                {
                    flickerGraphics.Clear(ClientSettings.BackColor);
                    if (SlidingPortalOffset > SlidingPortal.BitmapHeight | SlidingPortalOffset < 0)
                    {
                        using (Brush sBrush = new SolidBrush(ClientSettings.ForeColor))
                        {
                            flickerGraphics.DrawString("Let me catch up...", ClientSettings.TextFont, sBrush, this.Bounds);
                        }
                    }
                    flickerGraphics.DrawImage(SlidingPortal.Rendered, 0 - XOffset, 0 - SlidingPortalOffset);
                    if (XOffset > 0)
                    {
                        DrawMenu(flickerGraphics, SideShown.Right);
                    }
                    else if (XOffset < 0)
                    {
                        DrawMenu(flickerGraphics, SideShown.Left);
                    }

                    DrawPointer(flickerGraphics);
                    if (PockeTwit.DetectDevice.DeviceType == PockeTwit.DeviceType.Professional && this.Width < this.Height)
                    {
                        if (XOffset > 15)
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
                        NotificationArea.ShowNotification("Updating...");
                    }
                    else
                    {
                        NotificationArea.HideNotification();
                    }

                    NotificationArea.DrawNotification(flickerGraphics, this.Bottom, this.Width);
                    ErrorPopup.DrawNotification(flickerGraphics, this.Bottom, this.Width);

                    if (ClickablesControl.Visible)
                    {
                        ClickablesControl.Render(flickerGraphics);
                    }
                    e.Graphics.DrawImage(flickerBuffer, 0, 0);
                }
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
                RerenderBySize();
            }
        }

        public void RerenderBySize()
        {
            base.Visible = false;
            Application.DoEvents();
            LeftMenu.Height = this.Height;
            LeftMenu.Width = this.Width;
            


            RightMenu.Height = this.Height;
            RightMenu.Width = this.Width;

            ClickablesControl.Top = this.Top + 20;
            ClickablesControl.Left = this.Left + 20;
            ClickablesControl.Width = this.Width - 40;
            ClickablesControl.Height = this.Height - 40;

            this.ItemWidth = this.Width;

            foreach (StatusItem item in m_items.Values)
            {
                item.Bounds = ItemBounds(0, item.Index);
                item.ResetTexts();
            }

            SlidingPortal.Clear();
            FillBuffer();
            SelectAndJump();
            this.Repaint();
            base.Visible = true;
        }



		// Private Methods (22) 

        private void CheckForClicks(Point point)
        {
            if (m_items.Count == 0) { return; }
            try
            {
                int itemNumber = FindIndex(point.X, point.Y).Y;
                if (itemNumber > m_items.Count-1) { return; }
                
                StatusItem s = (StatusItem)m_items[itemNumber];

                foreach (StatusItem.Clickable c in s.Tweet.Clickables)
                {
                    Rectangle itemRect = s.Bounds;
                    itemRect.Offset(-XOffset, -YOffset);
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


        void ClickablesControl_WordClicked(string TextClicked)
        {
            if (TextClicked == "Detailed View" | Yedda.ShortText.isShortTextURL(TextClicked))
            {
                //Show the full tweet somehow.
                StatusItem s = (StatusItem)SelectedItem;

                fsDisplay.Status = s.Tweet;
                fsDisplay.Render();
                fsDisplay.Visible = true;
                ClickablesControl.Visible = false;
                
                /*
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
                 */
            }
            else if (WordClicked != null)
            {
                WordClicked(TextClicked);
            }
        }

        private void ClipScrollPosition()
        {
            if (XOffset < MinXOffset)
            {
                XOffset = MinXOffset;
                m_velocity.X = 0;
            }
            else if (XOffset > MaxXOffset)
            {
                XOffset = MaxXOffset;
                m_velocity.X = 0;
            }
            if (YOffset < 0)
            {
                YOffset = 0;
                m_velocity.Y = 0;
            }
            else if (YOffset > MaxYOffset)
            {
                YOffset = MaxYOffset;
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
            
            foreach (StatusItem item in m_items.Values)
            {
                if (item is StatusItem)
                {
                    StatusItem sItem = (StatusItem)item;
                    sItem.ParentGraphics = SlidingPortal._RenderedGraphics;
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("coredll.dll", SetLastError = true)]
        static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);
        struct MEMORYSTATUS
        {
            public UInt32 dwLength;
            public UInt32 dwMemoryLoad;
            public UInt32 dwTotalPhys;
            public UInt32 dwAvailPhys;
            public UInt32 dwTotalPageFile;
            public UInt32 dwAvailPageFile;
            public UInt32 dwTotalVirtual;
            public UInt32 dwAvailVirtual;
        }

        private void DrawMenu(Graphics m_backBuffer, SideShown Side)
        {
            Bitmap MenuMap;
            if (Side == SideShown.Left)
            {
                MenuMap = LeftMenu.Rendered;
                if (MenuMap != null)
                {
                    m_backBuffer.DrawImage(MenuMap, (0 - this.Width) + Math.Abs(XOffset), 0);
                }
            }
            else if (Side == SideShown.Right)
            {
                MenuMap = RightMenu.Rendered;
                if (MenuMap != null)
                {
                    m_backBuffer.DrawImage(MenuMap, this.Width - XOffset, 0);
                }
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

        int PointerSize = ClientSettings.TextSize;
        int PointerHalf = ClientSettings.TextSize / 2;

        private void DrawPointer(Graphics g)
        {
            float Percentage = 0;
            if (YOffset > 0)
            {
                Percentage = (float)YOffset / MaxYOffset;
            }
            int Position = (int)Math.Round(Height * Percentage);
            using (SolidBrush SBrush = new SolidBrush(ClientSettings.ForeColor))
            {
                Point a = new Point(Width - PointerSize, Position);
                Point b = new Point(Width, Position - PointerHalf);
                Point c = new Point(Width, Position + PointerHalf);
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

            index.Y = ((y + YOffset - Bounds.Top) / (ClientSettings.ItemHeight));
            
            return index;
        }

        private string GetMenuItemForPoint(MouseEventArgs e)
        {
            Point X = new Point(e.X, e.Y);
            
            int LeftOfItem = this.Width - Math.Abs(XOffset);
            int MenuHeight;
            int TopOfItem;
            SideMenu MenuToCheck = null;
            if (XOffset > 0)
            {
                MenuToCheck = RightMenu;
                MenuHeight = RightMenu.ItemHeight;
                TopOfItem = RightMenu.TopOfMenu;
            }
            else if (XOffset < 0)
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
            int itemY = Bounds.Top + (ClientSettings.ItemHeight * y);

            return new Rectangle(Bounds.Left, itemY, ItemWidth, ClientSettings.ItemHeight);
            
        }

        private void m_timer_Tick(object sender, EventArgs e)
        {
            
            if (!Capture && (m_velocity.Y != 0 || m_velocity.X != 0))
            {
                XDirection dir = m_velocity.X > 0 ? XDirection.Right : XDirection.Left;
                XDirection currentPos = XOffset > 0 ? XDirection.Right : XDirection.Left;

                //m_offset.Offset(m_velocity.X, m_velocity.Y);
                XOffset = XOffset + m_velocity.X;
                YOffset = YOffset + m_velocity.Y;

                if (currentPos == XDirection.Right & dir == XDirection.Left)
                {
                    if (XOffset <= 0)
                    {
                        XOffset = 0;
                        m_velocity.X = 0;
                    }
                }
                else if (currentPos == XDirection.Left & dir == XDirection.Right)
                {
                    if (XOffset >= 0)
                    {
                        XOffset = 0;
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
                    m_timer.Enabled = false;
                    HasMoved = false;
                }

                Invalidate();
            }
        }

        private void Reset()
        {
            
            SlidingPortal._RenderedGraphics.Clear(ClientSettings.BackColor);
            string Message = "One moment please...";
            if (m_items.Count == 0) { Message = "There are no items to display"; }
            using (Brush sBrush = new SolidBrush(ClientSettings.ForeColor))
            {
                SlidingPortal._RenderedGraphics.DrawString(Message, ClientSettings.TextFont, sBrush, new RectangleF(0, 0, this.Width, this.Height));
            }
            m_timer.Enabled = false;
            if (m_items.Count > 0)
            {
                m_items[m_selectedIndex].Selected = false;
                SlidingPortal.ReRenderItem(m_items[m_selectedIndex]);
            }
            m_selectedIndex = 0;
            Capture = false;
            m_velocity.X = 0;
            m_velocity.Y = 0;
            YOffset = 0;
            FillBuffer();
            SetSelectedIndexToZero();
            Invalidate();
        }

        private void SetRightMenuUser()
        {
            if (CurrentlyViewing != SideShown.Left)
            {
                StatusItem s = (StatusItem)m_items[m_selectedIndex];
                RightMenu.UserName = s.Tweet.user.screen_name;
            }
        }

        private void SelectAndJump()
        {
            //m_items[m_selectedIndex].Render(m_backBuffer, m_items[m_selectedIndex].Bounds);
            StatusItem item = null;
            try
            {
                item = m_items[m_selectedIndex];
            }
            catch (KeyNotFoundException)
            {
                return;
            }
            item.Selected = true;
            PockeTwit.LastSelectedItems.SetLastSelected(CurrentList(), item.Tweet.id);
            SlidingPortal.ReRenderItem(m_items[m_selectedIndex]);
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, new EventArgs());
            }
            if (fsDisplay.Visible)
            {
                StatusItem s = (StatusItem)m_items[m_selectedIndex];
                fsDisplay.Status = s.Tweet;
                fsDisplay.Render();
            }
            JumpToItem(item);
            RerenderPortal();
        }

        private void SelectItemOrMenu(MouseEventArgs e)
        {
            if (e.X > this.Width-XOffset)
            {
                //MenuItem selected
                if(MenuItemSelected!=null)
                {
                    string ItemName = GetMenuItemForPoint(e);
                    if (!String.IsNullOrEmpty(ItemName))
                    {
                        MenuItemSelected(ItemName);
                        menuwasClicked = true;
                    }
                }
            }
            else if ((XOffset) < 0 && e.X<Math.Abs(XOffset))
            {
                if (MenuItemSelected != null)
                {
                    string ItemName = GetMenuItemForPoint(e);
                    if (!String.IsNullOrEmpty(ItemName))
                    {
                        MenuItemSelected(ItemName);
                        menuwasClicked = true;
                    }
                }
            }
            else
            {
                Point selectedIndex = FindIndex(e.X, e.Y);
                if (selectedIndex.Y!= m_selectedIndex)
                {
                    if (m_items.ContainsKey(selectedIndex.Y))
                    {
                        m_items[m_selectedIndex].Selected = false;
                        SlidingPortal.ReRenderItem(m_items[m_selectedIndex]);
                        m_selectedIndex = selectedIndex.Y;
                        m_items[m_selectedIndex].Selected = true;
                        PockeTwit.LastSelectedItems.SetLastSelected(CurrentList(), m_items[m_selectedIndex].Tweet.id);
                        SlidingPortal.ReRenderItem(m_items[m_selectedIndex]);
                        if (SelectedItemChanged != null)
                        {
                            SelectedItemChanged(this, new EventArgs());
                        }
                        if (fsDisplay.Visible)
                        {
                            StatusItem s = (StatusItem)m_items[m_selectedIndex];
                            fsDisplay.Status = s.Tweet;
                            fsDisplay.Render();
                        }
                        SetRightMenuUser();
                        m_velocity.X = 0;
                        m_velocity.Y = 0;
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

        }

        private void ShowClickablesControl()
        {
            StatusItem s = null;
            try
            {
                s = (StatusItem)m_items[m_selectedIndex];
            }
            catch (KeyNotFoundException) 
            {
                return;
            }
            if (s == null) { return; }
            ClickablesControl.Items = s.Tweet.Clickables;
            ClickablesControl.Visible = true;
            Invalidate();
        }

        private void UnselectCurrentItem()
        {
            if (m_selectedIndex >= 0)
            {
                m_items[m_selectedIndex].Selected = false;
                SlidingPortal.ReRenderItem(m_items[m_selectedIndex]);
            }
        }


		#endregion Methods 

		#region Nested Classes (1) 
        public class ItemList : Dictionary<int, StatusItem>
        {
        }
		#endregion Nested Classes 
        
    }
}
