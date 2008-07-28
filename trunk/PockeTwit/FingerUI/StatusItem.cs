using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace FingerUI
{
    public class StatusItem : KListControl.IKListItem, IDisposable
    {
        public delegate void ClickedWordDelegate(string TextClicked);
        public event ClickedWordDelegate WordClicked;
        public class Clickable
        {
            public string Text;
            public RectangleF Location;
            
            public override bool Equals(object obj)
            {
                Clickable otherClick = (Clickable)obj;
                if (otherClick.Location.Top == this.Location.Top &&
                    otherClick.Location.Left == this.Location.Left)
                {
                    return true;
                }
                return false;
            }
            public void OffSet(int x, int y)
            {
                /*
                Location.Left = Location.Left + x;
                Location.Right = Location.Right - x;
                Location.Top = Location.Top + y;
                Location.Bottom = Location.Bottom - y;
                */
            }
        }

        public List<Clickable> Clickables = new List<Clickable>();
        private Font TextFont;

        public string Tweet { get; set; }
        public string User { get; set; }
        public string UserImageURL { get; set; }
        public string ID { get; set; }
        public string UserID { get; set; }
        public bool isBeingFollowed { get; set; }
        public bool isFavorite { get; set; }
        
        /// <summary>
        /// Initializes the <see cref="KListItem"/> class.
        /// </summary>
        static StatusItem()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "select.png");
            try
            {
                m_selectionBitmap = new Bitmap(path);
            }
            catch
            {
            }
            
        }

        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KListItem"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        public StatusItem(KListControl parent, string text, object value)
        {
            m_parent = parent;
            m_text = text;
            m_value = value;
            TextFont = m_parent.Font;
            //m_parent.MouseUp += new MouseEventHandler(m_parent_MouseUp);
        }

        void m_parent_MouseUp(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            Rectangle CurrentPosition = new Rectangle(currentOffset.X,currentOffset.Y, m_bounds.Width, m_bounds.Height);
            if (CurrentPosition.Contains(p))
            {
                System.Diagnostics.Debug.WriteLine("Parent mouseup");
                Clickable clicked = null;
                foreach (Clickable c in Clickables)
                {
                    Rectangle LocationRect = new Rectangle((int)c.Location.Left, (int)c.Location.Top, (int)c.Location.Width, (int)c.Location.Height);
                    LocationRect.Offset(currentOffset.X + ClientSettings.SmallArtSize + 5, currentOffset.Y);
                    if(LocationRect.Contains(p))
                    {
                        clicked = c;
                    }
                }
                if (clicked != null)
                {
                    if (WordClicked != null)
                    {
                        WordClicked(clicked.Text);
                    }
                }
            }
        }


        public StatusItem()
        {
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {

            m_parent.MouseUp -= new MouseEventHandler(m_parent_MouseUp);
            m_parent = null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KListItem"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected { get { return m_selected;  } set { m_selected = value; } }

        public bool Highlighted { get { return m_highlighted; } set { m_highlighted = value; } }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get { return m_text; } set { m_text = value; } }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get { return m_value; } set { m_value = value; } }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public KListControl Parent { get { return m_parent; } 
            set 
            {
                m_parent = value;
                TextFont = m_parent.Font;
                m_parent.MouseUp -= new MouseEventHandler(m_parent_MouseUp);
                m_parent.MouseUp += new MouseEventHandler(m_parent_MouseUp);
            }
        }

        /// <summary>
        /// The unscrolled bounds for this item.
        /// </summary>
        public Rectangle Bounds { get { return m_bounds; }
            set 
            {
                if (value.Width != m_bounds.Width)
                {
                    SplitLines = new List<string>(); 
                }
                m_bounds = value;
            }
        }

        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public int XIndex { get { return m_x; } set { m_x = value; } }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public int Index { get { return m_y; } set { m_y = value; } }

        /// <summary>
        /// Renders to the specified graphics.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="bounds">The bounds.</param>
        public virtual void Render(Graphics g, Rectangle bounds)
        {
            currentOffset = bounds;
            SolidBrush ForeBrush = new SolidBrush(m_parent.ForeColor);
            Rectangle textBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);

            textBounds = new Rectangle(bounds.X + (ClientSettings.SmallArtSize+5), bounds.Y, bounds.Width - (ClientSettings.SmallArtSize+5), bounds.Height);
            //Image AlbumArt = mpdclient.ArtBuffer.GetArt(Album, Artist, mpdclient.AsyncArtGrabber.ArtSize.Small);
            Image UserImage = PockeTwit.ImageBuffer.GetArt(User, UserImageURL);

            g.DrawImage(UserImage, bounds.X + 5, bounds.Y + 5);
        
            if (m_selected | m_highlighted) 
            {
                SolidBrush FillColor;

                if (m_selected)
                {
                    FillColor = new SolidBrush(m_parent.SelectedBackColor);
                    TextFont = m_parent.SelectedFont;
                    ForeBrush = new SolidBrush(m_parent.SelectedForeColor);
                }
                else
                {
                    FillColor = new SolidBrush(m_parent.HighLightBackColor);
                    TextFont = m_parent.HighlightedFont;
                    ForeBrush = new SolidBrush(m_parent.HighLightForeColor);
                }
                // Draw the selection image if available, otherwise just a gray box.
                if (m_selectionBitmap != null)
                {
                    g.DrawImage(m_selectionBitmap, bounds, new Rectangle(0, 0, m_selectionBitmap.Width, m_selectionBitmap.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    //g.DrawRectangle(new Pen(Color.Black), bounds);
                    Rectangle InnerBounds = new Rectangle(textBounds.Left, textBounds.Top, textBounds.Width, textBounds.Height);
                    InnerBounds.Offset(1, 1);
                    InnerBounds.Width--; InnerBounds.Height--;

                    g.FillRectangle(FillColor, InnerBounds);

                }
                FillColor.Dispose();
            }
            
            
            textBounds.Offset(5, 1);
            textBounds.Width = textBounds.Width-5;
            textBounds.Height--;

            m_stringFormat.Alignment = StringAlignment.Near;
            
            m_stringFormat.LineAlignment = StringAlignment.Near;
            BreakUpTheText(g, textBounds);
            int lineOffset = 0;
            foreach (string Line in SplitLines)
            {
                float Position = ((lineOffset * (TextFont.Size+4)) + textBounds.Top);
                
                g.DrawString(Line, TextFont, ForeBrush, textBounds.Left, Position, m_stringFormat);
                MakeClickable(Line, g, textBounds, (int)(lineOffset*(TextFont.Size+4)));
                lineOffset++;
            }
            ForeBrush.Dispose();
        }

        private void MakeClickable(string Line, Graphics g, Rectangle textBounds, int lineOffSet)
        {
            string[] Words = Line.Split(' ');
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < Words.Length; i++)
            {
                if (Words[i].StartsWith("@") | Words[i].StartsWith("http"))
                {
                    float startpos = g.MeasureString(b.ToString(), TextFont).Width;
                    SizeF WordSize = g.MeasureString(Words[i], TextFont);
                    Clickable c = new Clickable();
                    c.Location = new RectangleF(startpos, lineOffSet, WordSize.Width, WordSize.Height);
                    c.Text = Words[i];
                    using (Pen sPen = new Pen(Color.LightBlue))
                    {
                        g.DrawLine(sPen, (int)c.Location.Left + textBounds.Left, (int)c.Location.Bottom + textBounds.Top, (int)c.Location.Right + textBounds.Left, (int)c.Location.Bottom + textBounds.Top);
                    }
                    System.Diagnostics.Debug.WriteLine("Clickable found in " + this.User+ " -- " + c.Text);
                    if (!Clickables.Contains(c))
                    {
                        Clickables.Add(c);
                    }
                }
                b.Append(Words[i]+" ");
            }
            
        }

        private void BreakUpTheText(Graphics g, Rectangle textBounds)
        {
            SizeF size = g.MeasureString(this.Tweet, TextFont);
            string CurrentLine = this.Tweet;
            if (SplitLines.Count == 0)
            {
                bool SpaceSplit = false;
                if (this.Tweet.IndexOf(' ') > 0)
                {
                    SpaceSplit = true;
                }
                if (size.Width < textBounds.Width)
                {
                    SplitLines.Add(CurrentLine.TrimStart(new char[] { ' ' }));
                }
                while (size.Width > textBounds.Width)
                {
                    int lastBreak = 0;
                    int currentPos = 0;
                    string newString = "";
                    foreach (char c in CurrentLine)
                    {
                        newString = CurrentLine.Substring(0, currentPos);
                        if (g.MeasureString(newString, TextFont).Width > textBounds.Width)
                        {
                            if (!SpaceSplit | lastBreak == 0)
                            {
                                lastBreak = currentPos - 1;
                            }
                            newString = CurrentLine.Substring(0, lastBreak);
                            break;
                        }
                        if (c == ' ')
                        {
                            lastBreak = currentPos;
                        }
                        currentPos++;
                    }
                    SplitLines.Add(newString.TrimStart(new char[] { ' ' }));
                    if (SplitLines.Count >= 5) { break; }
                    if (lastBreak != 0)
                    {
                        CurrentLine = CurrentLine.Substring(lastBreak);
                    }
                    size = g.MeasureString(CurrentLine, TextFont);
                    if (size.Width < textBounds.Width)
                    {
                        SplitLines.Add(CurrentLine.TrimStart(new char[]{' '}));
                    }

                }
            }
        }

        private List<string> SplitLines = new List<string>();
        private StringFormat m_stringFormat = new StringFormat();
        private KListControl m_parent;
        private Rectangle m_bounds;
        private Rectangle currentOffset;
        private int m_x = -1;
        private int m_y = -1;

        private string m_text;
        private object m_value;
        private bool m_selected = false;
        private bool m_highlighted = false;

        private static Bitmap m_selectionBitmap;
    }
}
