using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace FingerUI
{
    /// <summary>
    /// A Kinetic list control.
    /// </summary>
    public class KListControl : UserControl
    {

		#region Fields (23) 

        private PockeTwit.Clickables ClickablesControl = new PockeTwit.Clickables();
        private bool HasMoved = false;
        private bool InFocus = false;
        Graphics m_backBuffer;
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
        
        Point m_selectedIndex = new Point(0,0);
        IKListItem m_selectedItem = null;
        Timer m_timer = new Timer();
        bool m_updating = false;
        Point m_velocity = new Point(0, 0);
        private int LeftMenuItemFocusedIndex = 0;
        private int RightMenuItemFocusedIndex = 0;
        List<FingerUI.KListControl.IKListItem> OnScreenItems = new List<IKListItem>();
        public List<string> LeftMenuItems = new List<string>();
        public List<string> RightMenuItems = new List<string>();
		#endregion Fields 

		#region Enums (2) 

        enum XDirection
        {
            Left, Right
        }
        private enum SideShown
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
            CreateBackBuffer();
            SelectedFont = this.Font;
            HighlightedFont = this.Font;
            m_timer.Interval = ClientSettings.AnimationInterval;
            m_timer.Tick += new EventHandler(m_timer_Tick);
            PockeTwit.ImageBuffer.Updated += new PockeTwit.ImageBuffer.ArtWasUpdated(ImageBuffer_Updated);

            ClickablesControl.Visible = false;
            ClickablesControl.WordClicked += new StatusItem.ClickedWordDelegate(ClickablesControl_WordClicked);
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

        public Color HighLightBackColor { get; set; }

        public Font HighlightedFont { get; set; }

        public Color HighLightForeColor { get; set; }

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
                if (this.LeftMenuItems.Count > 0)
                {
                    return 0 - (this.Width - 50);
                }
                return 0;
            }
        }

        public Color SelectedBackColor { get; set; }

        public Font SelectedFont { get; set; }

        public Color SelectedForeColor { get; set; }

        public IKListItem SelectedItem
        {
            get
            {
                if (m_items.Count > 0)
                {
                    return (IKListItem)m_items[m_selectedIndex.Y];
                }
                return null;
            }
        }

        public IKListItem this[int index]
        {
            get
            {
                return m_items[index];
            }
        }

        public string Warning { get; set; }

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

        public void SwitchTolist(string ListName)
        {
            if (!ItemLists.ContainsKey(ListName))
            {
                ItemLists.Add(ListName, new ItemList());
            }
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
            m_updating = true;
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
                m_items.Clear();
                Reset();
            }
        }

        public void EndUpdate()
        {
            m_updating = false;
            Reset();
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
                    m_selectedIndex.Y = 0;
                    m_selectedItem = m_items[0];
                    m_items[0].Selected = true;
                }
            }
        }

        public void SetSelectedMenu(string RequestedMenuItem)
        {
            int i = 0;
            foreach (string MenuItem in RightMenuItems)
            {
                if (MenuItem == RequestedMenuItem)
                {
                    RightMenuItemFocusedIndex = i;
                    this.Redraw();
                    return;
                }
                i++;
            }
            i=0;
            foreach (string MenuItem in LeftMenuItems)
            {
                if (MenuItem == RequestedMenuItem)
                {
                    LeftMenuItemFocusedIndex = i;
                    this.Redraw();
                    return;
                }
                i++;
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

            m_timer.Enabled = false;

            PockeTwit.ImageBuffer.Updated -= new PockeTwit.ImageBuffer.ArtWasUpdated(ImageBuffer_Updated);
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
                            MenuItemSelected(LeftMenuItems[LeftMenuItemFocusedIndex]);
                            break;
                        }
                    case SideShown.Right:
                        {
                            MenuItemSelected(RightMenuItems[RightMenuItemFocusedIndex]);
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
                        if (m_selectedIndex.Y > 0)
                        {
                            UnselectCurrentItem();
                            m_selectedIndex.Y = m_selectedIndex.Y - 1;
                            m_selectedItem = m_items[m_selectedIndex.Y];
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
                        if (RightMenuItemFocusedIndex >0)
                        {
                            RightMenuItemFocusedIndex--;
                        }
                    }
                    if (CurrentlyViewing == SideShown.Left)
                    {
                        if (LeftMenuItemFocusedIndex >0)
                        {
                            LeftMenuItemFocusedIndex--;
                        }
                    }
                }
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                if (CurrentlyViewing == SideShown.Middle)
                {
                    try
                    {
                        if (m_selectedIndex.Y < m_items.Count - 1)
                        {
                            UnselectCurrentItem();
                            m_selectedIndex.Y = m_selectedIndex.Y + 1;
                            m_selectedItem = m_items[m_selectedIndex.Y];
                            SelectAndJump();
                        }
                    }
                    catch
                    {
                    }
                }
                if (CurrentlyViewing == SideShown.Right)
                {
                    if (RightMenuItemFocusedIndex < RightMenuItems.Count-1)
                    {
                        RightMenuItemFocusedIndex++;
                    }
                }
                if (CurrentlyViewing == SideShown.Left)
                {
                    if (LeftMenuItemFocusedIndex < LeftMenuItems.Count-1)
                    {
                        LeftMenuItemFocusedIndex++;
                    }
                }
            }
            if (e.KeyCode == Keys.Right)
            {
                if (CurrentlyViewing != SideShown.Right)
                {
                    //RightMenuItemFocusedIndex = 0;
                    m_velocity.X = 15;
                    m_offset.X = m_offset.X + 3;
                    m_timer.Enabled = true;
                }
            }
            if (e.KeyCode == Keys.Left)
            {
                if (CurrentlyViewing != SideShown.Left)
                {
                    //LeftMenuItemFocusedIndex = 0;
                    m_velocity.X = -15;
                    m_offset.X = m_offset.X - 3;
                    m_timer.Enabled = true;
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

                //Don't allow us to drag left if there's no right menu
                int distanceX = 0;
                if (RightMenuItems.Count > 0)
                {
                    distanceX = m_mousePrev.X - currPos.X;
                }
                
                
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
                m_timer.Enabled = true;
            }

            try
            {
                //Check if we're half-way to menu
                if (m_offset.X > 0 && m_offset.X <= this.Width)
                {
                    m_timer.Enabled = true;
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
                    m_timer.Enabled = true;
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

        protected override void OnPaint(PaintEventArgs e)
        {
            OnScreenItems.Clear();
            if (m_backBuffer != null)
            {
                m_backBuffer.Clear(BackColor);
                
                Point startIndex = FindIndex(Bounds.Left, Bounds.Top);

                ItemList.Enumerator yEnumerator = m_items.GetEnumerator();
                bool moreY = yEnumerator.MoveNext();
                while (moreY && yEnumerator.Current.Key < startIndex.Y)
                {
                    moreY = yEnumerator.MoveNext();
                }

                
                while (moreY)
                {
                    IKListItem item = yEnumerator.Current.Value;
                    if (item != null)
                    {
                        Rectangle itemRect = item.Bounds;
                        itemRect.Offset(-m_offset.X, -m_offset.Y);
                        if (Bounds.IntersectsWith(itemRect))
                        {
                            //Draw borders
                            OnScreenItems.Add(item);
                            using (Pen whitePen = new Pen(ForeColor))
                            {
                                m_backBuffer.DrawLine(whitePen, itemRect.Left, itemRect.Top, itemRect.Right, itemRect.Top);
                                m_backBuffer.DrawLine(whitePen, itemRect.Left, itemRect.Bottom, itemRect.Right, itemRect.Bottom);
                                m_backBuffer.DrawLine(whitePen, itemRect.Right, itemRect.Top, itemRect.Right, itemRect.Bottom);
                            }
                            item.Render(m_backBuffer, itemRect);
                        }
                        else
                        {
                            break;
                        }
                    }

                    moreY = yEnumerator.MoveNext();
                }

                if (m_offset.X > 0)
                {
                    DrawRightMenu(m_backBuffer);
                }
                else if (m_offset.X < 0)
                {
                    DrawLeftMenu(m_backBuffer);
                }

                DrawPointer(m_backBuffer);
                if (PockeTwit.DetectDevice.DeviceType == PockeTwit.DeviceType.Professional &&  this.Width < this.Height)
                {
                    if (m_offset.X > 15)
                    {
                        if (IsMaximized)
                        {
                            DrawMaxWindowSwitcher(m_backBuffer);
                        }
                        else
                        {
                            DrawStandardWindowSwitcher(m_backBuffer);
                        }
                    }
                }
                

                if (!string.IsNullOrEmpty(Warning))
                {
                    using(Brush redBrush = new SolidBrush(ClientSettings.ErrorColor))
                    {
                        using (Font WarningFont = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold))
                        {
                            m_backBuffer.DrawString(Warning, WarningFont, redBrush, 0, 0);
                        }
                    }
                }

                if (ClickablesControl.Visible)
                {
                    ClickablesControl.Render(m_backBuffer);
                }

                e.Graphics.DrawImage(m_backBufferBitmap, 0, 0);
            }
            else
            {
                base.OnPaint(e);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

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
            Reset();
        }



		// Private Methods (22) 

        private void CheckForClicks(Point point)
        {
            foreach (IKListItem Item in OnScreenItems)
            {
                if (Item is StatusItem)
                {
                    StatusItem s = (StatusItem)Item;
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
        }

        void ClickablesControl_WordClicked(string TextClicked)
        {
            if (TextClicked == "Full Text")
            {
                //Show the full tweet somehow.
                StatusItem s = (StatusItem)SelectedItem;
                MessageBox.Show(s.Tweet.text, s.Tweet.user.screen_name);
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

            m_backBufferBitmap = new Bitmap(Bounds.Width, Bounds.Height);
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
                m_backBuffer.Clear(BackColor);

                
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

                        using (Pen whitePen = new Pen(ForeColor))
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

        private void DrawLeftMenu(Graphics m_backBuffer)
        {
            int MenuHeight = (this.Height - (ClientSettings.TextSize * 5)) / LeftMenuItems.Count;


            int TopOfItem = ((this.Height / 2) - ((LeftMenuItems.Count * MenuHeight) / 2));
            int LeftOfItem = ((0 - this.Width) + Math.Abs(m_offset.X))+50;

            //int LeftOfItem = this.Width - Math.Abs(m_offset.X);
            int i = 0;
            foreach (string MenuItem in LeftMenuItems)
            {
                int TextWidth = (int)m_backBuffer.MeasureString(MenuItem, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)).Width + ClientSettings.Margin;

                using (Pen whitePen = new Pen(ForeColor))
                {

                    Rectangle menuRect = new Rectangle(LeftOfItem + 1, TopOfItem, ItemWidth - 50, MenuHeight);

                    Color BackColor;
                    if (i == LeftMenuItemFocusedIndex && CurrentlyViewing == SideShown.Left)
                    {
                        BackColor = ClientSettings.SelectedBackColor;
                    }
                    else
                    {
                        BackColor = ClientSettings.BackColor;
                    }
                    using (Brush sBrush = new SolidBrush(BackColor))
                    {
                        m_backBuffer.FillRectangle(sBrush, menuRect);
                    }

                    m_backBuffer.DrawLine(whitePen, menuRect.Left, menuRect.Top, menuRect.Right, menuRect.Top);
                    using (Brush sBrush = new SolidBrush(ForeColor))
                    {
                        StringFormat sFormat = new StringFormat();
                        //sFormat.Alignment = StringAlignment.Center;
                        sFormat.LineAlignment = StringAlignment.Center;
                        int TextTop = ((menuRect.Bottom - menuRect.Top) / 2) + menuRect.Top;
                        int LeftPos = menuRect.Right - TextWidth;
                        //m_backBuffer.DrawString(MenuItem, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold), sBrush, menuRect.X + 5, TextTop, sFormat);
                        m_backBuffer.DrawString(MenuItem, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold), sBrush, LeftPos, TextTop, sFormat);
                    }
                    m_backBuffer.DrawLine(whitePen, menuRect.Left, menuRect.Bottom, menuRect.Right, menuRect.Bottom);
                    m_backBuffer.DrawLine(whitePen, menuRect.Right, 0, menuRect.Right, this.Height);
                    TopOfItem = TopOfItem + MenuHeight;
                }
                i++;
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
            using (SolidBrush SBrush = new SolidBrush(ForeColor))
            {
                Point a = new Point(Width - 10, Position);
                Point b = new Point(Width, Position - 5);
                Point c = new Point(Width, Position + 5);
                Point[] Triangle = new Point[]{a,b,c};
                g.FillPolygon(SBrush, Triangle);
            }
            

        }

        private void DrawRightMenu(Graphics graphics)
        {
            int MenuHeight = (this.Height-(ClientSettings.TextSize*5)) / RightMenuItems.Count;


            int TopOfItem = ((this.Height / 2) - ((RightMenuItems.Count * MenuHeight) / 2));
            int LeftOfItem = this.Width - Math.Abs(m_offset.X);
            int i = 0;
            foreach (string MenuItem in RightMenuItems)
            {
                using (Pen whitePen = new Pen(ForeColor))
                {
                    Rectangle menuRect = new Rectangle(LeftOfItem + 1, TopOfItem, ItemWidth, MenuHeight);
                    Color BackColor;
                    if (i == RightMenuItemFocusedIndex && CurrentlyViewing == SideShown.Right)
                    {
                        BackColor = ClientSettings.SelectedBackColor;
                    }
                    else
                    {
                        BackColor = ClientSettings.BackColor;
                    }
                    using (Brush sBrush = new SolidBrush(BackColor))
                    {
                        m_backBuffer.FillRectangle(sBrush, menuRect);
                    }

                    m_backBuffer.DrawLine(whitePen, menuRect.Left, menuRect.Top, menuRect.Right, menuRect.Top);
                    using (Brush sBrush = new SolidBrush(ForeColor))
                    {
                        StringFormat sFormat = new StringFormat();
                        //sFormat.Alignment = StringAlignment.Center;
                        sFormat.LineAlignment = StringAlignment.Center;
                        int TextTop = ((menuRect.Bottom - menuRect.Top) / 2) + menuRect.Top;
                        StatusItem SelectedStatus = (StatusItem)SelectedItem;
                        string DisplayItem = MenuItem;
                        if(SelectedStatus !=null)
                        {
                            DisplayItem = MenuItem.Replace("@User", "@"+SelectedStatus.Tweet.user.screen_name);
                        }
                        m_backBuffer.DrawString(DisplayItem, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold), sBrush, menuRect.X + 5, TextTop, sFormat);
                    }
                    m_backBuffer.DrawLine(whitePen, menuRect.Left, menuRect.Bottom, menuRect.Right, menuRect.Bottom);
                    m_backBuffer.DrawLine(whitePen, menuRect.Left, 0, menuRect.Left, this.Height);
                    TopOfItem = TopOfItem + MenuHeight;
                }
                i++;
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
            if (m_offset.X > 0)
            {
                MenuHeight = (this.Height - (ClientSettings.TextSize * 5)) / RightMenuItems.Count;
                int TopOfItem = ((this.Height / 2) - ((RightMenuItems.Count * MenuHeight) / 2));
                foreach (string MenuItem in RightMenuItems)
                {
                    Rectangle menuRect = new Rectangle(LeftOfItem, TopOfItem, ItemWidth, MenuHeight);
                    TopOfItem = TopOfItem + MenuHeight;
                    if (menuRect.Contains(X))
                    {
                        Invalidate(menuRect);
                        return MenuItem;
                    }
                }

            }
            else if (m_offset.X < 0)
            {
                MenuHeight = (this.Height - (ClientSettings.TextSize * 5)) / LeftMenuItems.Count;
                int TopOfItem = ((this.Height / 2) - ((LeftMenuItems.Count * MenuHeight) / 2));
                LeftOfItem = 0;
                foreach (string MenuItem in LeftMenuItems)
                {
                    Rectangle menuRect = new Rectangle(LeftOfItem, TopOfItem, Math.Abs(m_offset.X), MenuHeight);
                    TopOfItem = TopOfItem + MenuHeight;
                    if (menuRect.Contains(X))
                    {
                        Invalidate(menuRect);
                        return MenuItem;
                    }
                }
            }
            return null;
        }

        void ImageBuffer_Updated(string User)
        {
            if(InvokeRequired)
            {
                delClearMe d = new delClearMe(Refresh);
                this.Invoke(d, null);
            }
            else
            {
                this.Refresh();
            }
        }

        private Rectangle ItemBounds(int x, int y)
        {
            int itemY = Bounds.Top + (m_itemHeight * y);

            return new Rectangle(Bounds.Left, itemY, ItemWidth, ItemHeight);
            
        }

        private void m_timer_Tick(object sender, EventArgs e)
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
                    m_timer.Enabled = false;
                    HasMoved = false;
                }

                Invalidate();
            }
        }

        private void Reset()
        {
            if (!m_updating)
            {
                m_timer.Enabled = false;
                if (m_selectedItem != null)
                {
                    m_selectedItem.Selected = false;
                    m_selectedItem = null;
                }
                m_selectedIndex = new Point(0, 0);
                Capture = false;
                m_velocity.X = 0;
                m_velocity.Y = 0;
                //m_offset.X = 0;
                m_offset.Y = 0;

                SetSelectedIndexToZero();
                Invalidate();

                if (SelectedItemChanged != null)
                {
                    SelectedItemChanged(this, new EventArgs());
                }
                
            }
        }

        private void SelectAndJump()
        {
            m_selectedItem.Selected = false;
            IKListItem item;
            item = m_items[m_selectedIndex.Y];
            item.Selected = true;
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
                if (selectedIndex != m_selectedIndex)
                {
                    if (m_items.ContainsKey(selectedIndex.Y))
                    {
                        if (m_selectedItem != null)
                        {
                            m_selectedItem.Selected = false;
                        }
                        m_selectedIndex = selectedIndex;
                        m_selectedItem = m_items[selectedIndex.Y];
                        m_selectedItem.Selected = true;

                        if (SelectedItemChanged != null)
                        {
                            SelectedItemChanged(this, new EventArgs());
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
            StatusItem s = (StatusItem)m_selectedItem;
            if (s == null) { return; }
            ClickablesControl.Items = s.Tweet.Clickables;
            if (s.Clipped)
            {
                ClickablesControl.ShowClipped = true;
            }
            ClickablesControl.Visible = true;
        }

        private void UnselectCurrentItem()
        {
            if (m_selectedIndex.Y >= 0)
            {
                IKListItem item = m_items[m_selectedIndex.Y];
                item.Selected = false;
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
