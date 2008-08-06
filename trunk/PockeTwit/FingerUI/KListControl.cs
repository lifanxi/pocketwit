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
        /// <summary>
        /// Interface for items contained within the list.
        /// </summary>
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

        public delegate void delClearMe();
        public delegate void delAddItem(StatusItem item);

        public delegate void delMenuItemSelected(string ItemName);
        public event delMenuItemSelected MenuItemSelected;
        public event StatusItem.ClickedWordDelegate WordClicked;

        public delegate void delSwitchState(bool IsMaximized);
        public event delSwitchState SwitchWindowState;

        public bool IsMaximized { get; set; }
        private bool HasMoved = false;
        

        private string LastItemSelected = null;

        public List<string> RightMenuItems = new List<string>();
        public List<string> LeftMenuItems = new List<string>();

        public string Warning { get; set; }

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

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"></see> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            CleanupBackBuffer();

            m_timer.Enabled = false;

            PockeTwit.ImageBuffer.Updated -= new PockeTwit.ImageBuffer.ArtWasUpdated(ImageBuffer_Updated);
            base.Dispose(disposing);
        }

        
        public Color SelectedForeColor { get; set; }
        public Color SelectedBackColor { get; set; }
        public Color HighLightForeColor { get; set; }
        public Color HighLightBackColor { get; set; }
        public Font SelectedFont { get; set; }
        public Font HighlightedFont { get; set; }


        /// <summary>
        /// Occurs when the selected item changes.
        /// </summary>
        public event EventHandler SelectedItemChanged;

        /// <summary>
        /// Occurs when the selected item is clicked on (after already being selected).
        /// </summary>
        public event EventHandler SelectedItemClicked;

        /// <summary>
        /// Gets the <see cref="Scroller.KListControl.IKListItem"/> at the specified index.
        /// </summary>
        public IKListItem this[int index]
        {
            get
            {
                return m_items[index];
            }
        }
        
                /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public IKListItem SelectedItem
        {
            get
            {
                return m_selectedItem;
            }
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_items.Count;
            }
        }

        /// <summary>
        /// Gets or sets the maximum scroll velocity.
        /// </summary>
        /// <value>The maximum velocity.</value>
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

        /// <summary>
        /// Gets or sets the height of items in the control.
        /// </summary>
        /// <value>The height of the items.</value>
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

        /// <summary>
        /// Gets or sets the height of items in the control.
        /// </summary>
        /// <value>The height of the items.</value>
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

        
        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="text">The text for the item.</param>
        /// <param name="value">A value related to the item.</param>
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


        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(IKListItem item)
        {
            item.Parent = this;
            item.Selected = false;
            item.Bounds = ItemBounds(0, item.Index);
            m_items.Add(item.Index, item);
            //Reset();
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RemoveItem(IKListItem item)
        {
            if (m_items.ContainsKey(item.Index))
            {
                m_items.Remove(item.Index);
            }
            Reset();
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
        

        /// <summary>
        /// Clears the list.
        /// </summary>
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

        /// <summary>
        /// Invalidates the item (when visible).
        /// </summary>
        /// <param name="item">The item.</param>
        public void Invalidate(IKListItem item)
        {
            Rectangle itemBounds = item.Bounds;
            itemBounds.Offset(-m_offset.X, -m_offset.Y);
            if (Bounds.IntersectsWith(itemBounds))
            {
                Invalidate(itemBounds);
            }
        }

        /// <summary>
        /// Begins updates - suspending layout recalculation.
        /// </summary>
        public void BeginUpdate()
        {
            m_updating = true;
        }

        /// <summary>
        /// Ends updates - re-enabling layout recalculation.
        /// </summary>
        public void EndUpdate()
        {
            m_updating = false;
            Reset();
        }

        /// <summary>
        /// Called when the control is resized.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.ItemWidth = this.Width;
            foreach (IKListItem item in m_items.Values)
            {
                item.Bounds = ItemBounds(0, item.Index);
            }
            CreateBackBuffer();
            Reset();
        }

        enum XDirection
        {
            Left, Right
        }

        /// <summary>
        /// Handles the Tick event of the m_timer control.
        /// </summary>
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

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing
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
                if (this.Width < this.Height)
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

                e.Graphics.DrawImage(m_backBufferBitmap, 0, 0);
            }
            else
            {
                base.OnPaint(e);
            }
        }

        private void DrawLeftMenu(Graphics m_backBuffer)
        {
            int TopOfItem = (this.Height / 2) - ((LeftMenuItems.Count / 2) * 30);
            int LeftOfItem = ((0 - this.Width) + Math.Abs(m_offset.X))+50;

            //int LeftOfItem = this.Width - Math.Abs(m_offset.X);
            foreach (string MenuItem in LeftMenuItems)
            {
                int TextWidth = (int)m_backBuffer.MeasureString(MenuItem, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)).Width + ClientSettings.Margin;

                using (Pen whitePen = new Pen(ForeColor))
                {

                    Rectangle menuRect = new Rectangle(LeftOfItem + 1, TopOfItem, ItemWidth-50, 30);

                    Color BackColor = this.BackColor;
                    if (MenuItem == LastItemSelected)
                    {
                    //    BackColor = this.SelectedBackColor;
                        LastItemSelected = null;
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
                    TopOfItem = TopOfItem + 30;
                }
            }
        }

        private void DrawRightMenu(Graphics graphics)
        {
            int TopOfItem = (this.Height / 2) - ((RightMenuItems.Count / 2) * 30);
            int LeftOfItem = this.Width - Math.Abs(m_offset.X);
            foreach (string MenuItem in RightMenuItems)
            {
                using (Pen whitePen = new Pen(ForeColor))
                {
                    Rectangle menuRect = new Rectangle(LeftOfItem + 1, TopOfItem, ItemWidth, 30);

                    Color BackColor = this.BackColor;
                    if (MenuItem == LastItemSelected)
                    {
                    //   BackColor = this.SelectedBackColor;
                        LastItemSelected = null;
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
                    TopOfItem = TopOfItem + 30;
                }
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

        /// <summary>
        /// Called when the user clicks on the control with the mouse.
        /// </summary>
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

        /// <summary>
        /// Called when the user moves the mouse over the control.
        /// </summary>
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

        /// <summary>
        /// Called when the user releases a mouse button.
        /// </summary>
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

        public void HookKey()
        {
            this.Parent.KeyDown += new KeyEventHandler(OnKeyDown);
        }

        void Parent_KeyDown( KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
        public void UnHookKey()
        {
            this.Parent.KeyDown -= new KeyEventHandler(OnKeyDown);
        }

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                try
                {
                    if (m_selectedIndex.Y >0 )
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
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                try
                {
                    if (m_selectedIndex.Y < m_items.Count-1)
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
            if (e.KeyCode == Keys.Right)
            {
                m_velocity.X = 15;
                m_offset.X = m_offset.X + 3;
                m_timer.Enabled = true;
            }
            if (e.KeyCode == Keys.Left)
            {
                m_velocity.X = -15;
                m_offset.X = m_offset.X - 3;
                m_timer.Enabled = true;
            }
            Invalidate();
        }

        private void SelectAndJump()
        {
            m_selectedItem.Selected = false;
            IKListItem item;
            item = m_items[m_selectedIndex.Y];
            item.Selected = true;
            JumpToItem(item);
        }

        private void UnselectCurrentItem()
        {
            if (m_selectedIndex.Y >= 0)
            {
                IKListItem item = m_items[m_selectedIndex.Y];
                item.Selected = false;
            }
        }
       

        private void SelectItemOrMenu(MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SelectItemOrMenu");
            if (e.X > this.Width-m_offset.X)
            {
                //MenuItem selected
                if(MenuItemSelected!=null)
                {
                    string ItemName = GetMenuItemForPoint(e);
                    if (!String.IsNullOrEmpty(ItemName))
                    {
                        MenuItemSelected(ItemName);
                        LastItemSelected = ItemName;
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
                        LastItemSelected = ItemName;
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

        private string GetMenuItemForPoint(MouseEventArgs e)
        {
            Point X = new Point(e.X, e.Y);
            int TopOfItem = (this.Height / 2) - ((RightMenuItems.Count / 2) * 30);
            int LeftOfItem = this.Width - Math.Abs(m_offset.X);
            if (m_offset.X > 0)
            {
                foreach (string MenuItem in RightMenuItems)
                {
                    Rectangle menuRect = new Rectangle(LeftOfItem, TopOfItem, ItemWidth, 30);
                    TopOfItem = TopOfItem + 30;
                    if (menuRect.Contains(X))
                    {
                        Invalidate(menuRect);
                        return MenuItem;
                    }
                }

            }
            else if (m_offset.X < 0)
            {
                TopOfItem = (this.Height / 2) - ((LeftMenuItems.Count / 2) * 30);
                LeftOfItem = 0;
                foreach (string MenuItem in LeftMenuItems)
                {
                    Rectangle menuRect = new Rectangle(LeftOfItem, TopOfItem, Math.Abs(m_offset.X), 30);
                    TopOfItem = TopOfItem + 30;
                    if (menuRect.Contains(X))
                    {
                        Invalidate(menuRect);
                        return MenuItem;
                    }
                }
            }
            return null;
        }

        public void ResetHoriz()
        {
            m_offset.X = 0;
        }

        /// <summary>
        /// Resets the drawing of the list.
        /// </summary>
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
                m_selectedIndex = new Point(-1, -1);
                Capture = false;
                m_velocity.X = 0;
                m_velocity.Y = 0;
                //m_offset.X = 0;
                m_offset.Y = 0;

                Invalidate();

                if (SelectedItemChanged != null)
                {
                    SelectedItemChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Cleans up the background paint buffer.
        /// </summary>
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

        /// <summary>
        /// Creates the background paint buffer.
        /// </summary>
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

        /// <summary>
        /// Clips the scroll position.
        /// </summary>
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

        /// <summary>
        /// Clips the velocity.
        /// </summary>
        private void ClipVelocity()
        {
            m_velocity.X = Math.Min(m_velocity.X, m_maxVelocity);
            m_velocity.X = Math.Max(m_velocity.X, -m_maxVelocity);

            m_velocity.Y = Math.Min(m_velocity.Y, m_maxVelocity);
            m_velocity.Y = Math.Max(m_velocity.Y, -m_maxVelocity);
        }

        /// <summary>
        /// Finds the bounds for the specified item.
        /// </summary>
        /// <param name="x">The item x index.</param>
        /// <param name="y">The item y index.</param>
        /// <returns>The item bounds.</returns>
        private Rectangle ItemBounds(int x, int y)
        {
            int itemY = Bounds.Top + (m_itemHeight * y);

            return new Rectangle(Bounds.Left, itemY, ItemWidth, ItemHeight);
            
        }

        /// <summary>
        /// Finds the index for the specified y offset.
        /// </summary>
        /// <param name="x">The x offset.</param>
        /// <param name="y">The y offset.</param>
        /// <returns></returns>
        private Point FindIndex(int x, int y)
        {
            Point index = new Point(0, 0);

            index.Y = ((y + m_offset.Y - Bounds.Top) / (m_itemHeight));
            
            return index;
        }

        /// <summary>
        /// Gets the maximum x offset.
        /// </summary>
        /// <value>The maximum x offset.</value>
        private int MaxXOffset
        {
            get
            {
                //return Math.Max(((m_items.Count * ItemWidth)) - Bounds.Width, 0);
                return this.Width-50;
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


        /// <summary>
        /// Gets the maximum y offset.
        /// </summary>
        /// <value>The maximum y offset.</value>
        private int MaxYOffset
        {
            get
            {
                return Math.Max(((m_items.Count * ItemHeight)) - Bounds.Height, 0);
            }
        }

        // The items!
        class ItemList : Dictionary<int, IKListItem>
        {
        }
        
        ItemList m_items = new ItemList();

        

        // Properties
        int m_maxVelocity = 15;
        int m_itemHeight = 40;
        int m_itemWidth = 240;
        bool m_updating = false;
        bool m_scrollBarMove = false;
        List<FingerUI.KListControl.IKListItem> OnScreenItems = new List<IKListItem>();

        // Background drawing
        Bitmap m_backBufferBitmap;
        Graphics m_backBuffer;
        Dictionary<string, Rectangle> m_AlbumCacheLocations = new Dictionary<string, Rectangle>();

        // Motion variables
        Point m_selectedIndex = new Point(-1, -1);
        IKListItem m_selectedItem = null;
        Point m_velocity = new Point(0, 0);
        Point m_mouseDown = new Point(-1, -1);
        Point m_mousePrev = new Point(-1, -1);
        Timer m_timer = new Timer();
        int m_timerCount = 0;
        Point m_offset = new Point();

    }
}
