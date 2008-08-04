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
        [Serializable]
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
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        //public List<Clickable> Clickables = new List<Clickable>();
        private Font TextFont;

        private Graphics _ParentGraphics;
        public Graphics ParentGraphics 
        {
            set
            {
                _ParentGraphics = value;
            }
        }

        private PockeTwit.Library.status _Tweet;
        public PockeTwit.Library.status Tweet 
        {
            get { return _Tweet; }
            set
            {
                _Tweet = value;
                if (string.IsNullOrEmpty(value.favorited))
                {
                    m_highlighted = false;
                }
                else
                {
                    m_highlighted = bool.Parse(value.favorited);
                }
                if (Tweet.Clickables == null)
                {
                    Tweet.Clickables = new List<Clickable>();
                }
                if (Tweet.SplitLines == null)
                {
                    Tweet.SplitLines = new List<string>();
                }
                
            }

        }

        public bool isBeingFollowed
        {
            get
            {
                return (PockeTwit.Following.IsFollowing(Tweet.user));
            }
        }
        public bool isFavorite
        {
            get 
            {
                if(string.IsNullOrEmpty(Tweet.favorited))
                {
                    return false;
                }
                return bool.Parse(Tweet.favorited);
            }
            set
            {
                Tweet.favorited = value.ToString();
                this.Highlighted = value;
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
        }

        public StatusItem()
        {
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {

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
            }
        }

        /// <summary>
        /// The unscrolled bounds for this item.
        /// </summary>
        public Rectangle Bounds { get { return m_bounds; }
            set 
            {
                if (m_bounds.Width!=0 && value.Width != m_bounds.Width)
                {
                    Tweet.SplitLines = new List<string>(); 
                }
                m_bounds = value;
                Rectangle textBounds = new Rectangle(ClientSettings.SmallArtSize + ClientSettings.Margin, 0, m_bounds.Width - (ClientSettings.SmallArtSize + (ClientSettings.Margin*2)), m_bounds.Height);
                BreakUpTheText(_ParentGraphics, textBounds);
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
            
            Rectangle textBounds = new Rectangle(bounds.X + (ClientSettings.SmallArtSize + ClientSettings.Margin), bounds.Y, bounds.Width - (ClientSettings.SmallArtSize + (ClientSettings.Margin*2)), bounds.Height);
            //Image AlbumArt = mpdclient.ArtBuffer.GetArt(Album, Artist, mpdclient.AsyncArtGrabber.ArtSize.Small);
        
            if (m_selected) 
            {
                SolidBrush FillColor;

                FillColor = new SolidBrush(m_parent.SelectedBackColor);
                TextFont = m_parent.SelectedFont;
                ForeBrush = new SolidBrush(m_parent.SelectedForeColor);
            
                //g.DrawRectangle(new Pen(Color.Black), bounds);
                //Rectangle InnerBounds = new Rectangle(textBounds.Left, textBounds.Top, textBounds.Width+5, textBounds.Height);
                Rectangle InnerBounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width , bounds.Height);
                InnerBounds.Offset(1, 1);
                InnerBounds.Width--; InnerBounds.Height--;

                g.FillRectangle(FillColor, InnerBounds);

                FillColor.Dispose();
            }

            Image UserImage = PockeTwit.ImageBuffer.GetArt(Tweet.user.screen_name, Tweet.user.profile_image_url);

            g.DrawImage(UserImage, bounds.X + ClientSettings.Margin, bounds.Y + ClientSettings.Margin);


            if (m_highlighted)
            {
                System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
                ia.SetColorKey(PockeTwit.ImageBuffer.FavoriteImage.GetPixel(0, 0), PockeTwit.ImageBuffer.FavoriteImage.GetPixel(0, 0));
                g.DrawImage(PockeTwit.ImageBuffer.FavoriteImage, 
                    new Rectangle(bounds.X+5, bounds.Y+5, 7, 7),0,0,7,7,GraphicsUnit.Pixel, ia);
            }
            
            textBounds.Offset(5, 1);
            textBounds.Width = textBounds.Width-5;
            textBounds.Height--;

            m_stringFormat.Alignment = StringAlignment.Near;
            
            m_stringFormat.LineAlignment = StringAlignment.Near;
            BreakUpTheText(g, textBounds);
            int lineOffset = 0;
            foreach (string Line in Tweet.SplitLines)
            {
                float Position = ((lineOffset * (ClientSettings.TextSize)) + textBounds.Top);
                
                g.DrawString(Line, TextFont, ForeBrush, textBounds.Left, Position, m_stringFormat);
                MakeClickable(Line, g, textBounds);
                lineOffset++;
            }
            ForeBrush.Dispose();
        }

        //texbounds is the area we're allowed to draw within
        //lineOffset is how many lines we've already drawn in these bounds
        private void MakeClickable(string Line, Graphics g, Rectangle textBounds)
        {
            
            using (Pen sPen = new Pen(ClientSettings.LinkColor))
            {
                foreach (Clickable c in Tweet.Clickables)
                {
                    g.DrawLine(sPen, (int)c.Location.Left + textBounds.Left, (int)c.Location.Bottom + textBounds.Top,
                        (int)c.Location.Right + textBounds.Left, (int)c.Location.Bottom + textBounds.Top);
                }
            }
        }

        #region Parsing Routines
        private void BreakUpTheText(Graphics g, Rectangle textBounds)
        {
            if (Tweet.SplitLines.Count == 0)
            {
                string CurrentLine = System.Web.HttpUtility.HtmlDecode(this.Tweet.text);
                FirstClickableRun(CurrentLine);
                SizeF size = g.MeasureString(CurrentLine, TextFont);

                bool SpaceSplit = false;
                if (this.Tweet.text.IndexOf(' ') > 0)
                {
                    SpaceSplit = true;
                }
                if (size.Width < textBounds.Width)
                {
                    string line = CurrentLine.TrimStart(new char[] { ' ' });
                    Tweet.SplitLines.Add(line);
                    FindClickables(line, g, 0);
                }
                int LineOffset = 1;
                while (size.Width > textBounds.Width)
                {
                    int lastBreak = 0;
                    int currentPos = 0;
                    StringBuilder newString = new StringBuilder();
                    foreach (char c in CurrentLine)
                    {
                        newString.Append(c);    
                        if (g.MeasureString(newString.ToString(), TextFont).Width > textBounds.Width)
                        {
                            if (!SpaceSplit | lastBreak == 0)
                            {
                                lastBreak = currentPos - 1;
                            }
                            newString = new StringBuilder(CurrentLine.Substring(0, lastBreak));
                            break;
                        }
                        if (c == ' ')
                        {
                            lastBreak = currentPos;
                        }
                        currentPos++;
                    }
                    string line = newString.ToString().TrimStart(new char[] { ' ' });
                    Tweet.SplitLines.Add(line);
                    FindClickables(line, g, LineOffset-1);
                    if (Tweet.SplitLines.Count >= 5) { break; }
                    if (lastBreak != 0)
                    {
                        CurrentLine = CurrentLine.Substring(lastBreak);
                    }
                    size = g.MeasureString(CurrentLine, TextFont);
                    if (size.Width < textBounds.Width)
                    {
                        line = CurrentLine.TrimStart(new char[] { ' ' });
                        Tweet.SplitLines.Add(line);
                        FindClickables(line,g,LineOffset);
                    }
                    LineOffset++;
                }
            }
        }

        private void FirstClickableRun(string text)
        {
            string[] words = text.Split(new char[] { ' ' });
            foreach (string word in words)
            {
                if ((word.StartsWith("http") | word.StartsWith("@")) && word.Length>1)
                {
                    Clickable c = new Clickable();
                    c.Text = word;
                    Tweet.Clickables.Add(c);
                }
            }
        }
        private void FindClickables(string Line, Graphics g, int lineOffSet)
        {
            System.Diagnostics.Debug.WriteLine("Find clickables in " + Tweet.id);
            string[] Words = Line.Split(' ');
            StringBuilder LineBeforeThisWord = new StringBuilder();
            float Position = ((lineOffSet * (ClientSettings.TextSize)));
            for (int i = 0; i < Words.Length; i++)
            {
                string WordToCheck = Words[i];
                List<Clickable> OriginalClicks = new List<Clickable>(Tweet.Clickables);
                foreach(Clickable c in OriginalClicks)
                {
                    if (i == Words.Length-1)
                    {
                        if (c.Text.StartsWith(WordToCheck))
                        {
                            float startpos = g.MeasureString(LineBeforeThisWord.ToString(), TextFont).Width;
                            //Find the size of the word
                            SizeF WordSize = g.MeasureString(Words[i], TextFont);
                            //A structure containing info we need to know about the word.
                            c.Location = new RectangleF(startpos, Position, WordSize.Width, WordSize.Height);
                            
                            string SecondPart = c.Text.Substring(WordToCheck.Length);

                            if (!string.IsNullOrEmpty(SecondPart))
                            {
                                Clickable wrapClick = new Clickable();
                                wrapClick.Text = c.Text;
                                //Find the size of the word
                                WordSize = g.MeasureString(SecondPart, TextFont);
                                //A structure containing info we need to know about the word.
                                float NextPosition = (((lineOffSet + 1) * (ClientSettings.TextSize)));
                                wrapClick.Location = new RectangleF(0F, NextPosition, WordSize.Width, WordSize.Height);
                                Tweet.Clickables.Add(wrapClick);
                            }
                        }
                        System.Diagnostics.Debug.WriteLine("Last word");
                    }
                    else if (WordToCheck == c.Text)
                    {
                        //Find out how far to the right this word will appear
                        float startpos = g.MeasureString(LineBeforeThisWord.ToString(), TextFont).Width;
                        //Find the size of the word
                        SizeF WordSize = g.MeasureString(Words[i], TextFont);
                        //A structure containing info we need to know about the word.
                        c.Location = new RectangleF(startpos, Position, WordSize.Width, WordSize.Height);
                        c.Text = WordToCheck;
                    }
                }
                LineBeforeThisWord.Append(WordToCheck + " ");
            }
        }
        #endregion

        //private List<string> SplitLines = new List<string>();
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

    }
}
