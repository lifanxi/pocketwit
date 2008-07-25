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
        public string Tweet { get; set; }
        public string User { get; set; }
        public string UserImageURL { get; set; }
        
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
        public KListControl Parent { get { return m_parent; } set { m_parent = value; } }

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
            Font TextFont = m_parent.Font;
            SolidBrush ForeBrush = new SolidBrush(m_parent.ForeColor);
            Rectangle textBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);

            textBounds = new Rectangle(bounds.X + 35, bounds.Y, bounds.Width - (35), bounds.Height);
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
            
            
            textBounds.Offset(1, 1);
            textBounds.Width--;
            textBounds.Height--;

            m_stringFormat.Alignment = StringAlignment.Near;
            
            m_stringFormat.LineAlignment = StringAlignment.Near;
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
                    SplitLines.Add(CurrentLine);
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
                            if (!SpaceSplit)
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
                    SplitLines.Add(newString);
                    CurrentLine = CurrentLine.Substring(lastBreak);
                    size = g.MeasureString(CurrentLine, TextFont);
                    if (size.Width < textBounds.Width)
                    {
                        SplitLines.Add(CurrentLine);
                    }
                }
            }
            int lineOffset = 0;
            foreach (string Line in SplitLines)
            {
                float Position = ((lineOffset * (TextFont.Size+4)) + textBounds.Top);
                g.DrawString(Line, TextFont, ForeBrush, textBounds.Left, Position, m_stringFormat);
                lineOffset++;
            }
            ForeBrush.Dispose();
        }

        private List<string> SplitLines = new List<string>();
        private StringFormat m_stringFormat = new StringFormat();
        private KListControl m_parent;
        private Rectangle m_bounds;
        private int m_x = -1;
        private int m_y = -1;

        private string m_text;
        private object m_value;
        private bool m_selected = false;
        private bool m_highlighted = false;

        private static Bitmap m_selectionBitmap;
    }
}
