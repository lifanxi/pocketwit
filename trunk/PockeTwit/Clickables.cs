using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public class Clickables
    {

		#region Fields (2) 

        private int _CurrentlyFocused = 0;
        private List<string> TextItems = new List<string>(new string[]{"Exit"});

		#endregion Fields 

		#region Constructors (1) 

        public Clickables()
        {
            
        }

		#endregion Constructors 

		#region Properties (7) 

        public int Height { get; set; }

        public List<FingerUI.StatusItem.Clickable> Items
        {
            set
            {
                TextItems = new List<string>();
                TextItems.Add("Detailed View");
                foreach (FingerUI.StatusItem.Clickable c in value)
                {
                    if (!TextItems.Contains(c.Text))
                    {
                        TextItems.Add(c.Text);
                    }
                }
                TextItems.Add("Exit");
            }
        }

        public int Left { get; set; }


        public int Top { get; set; }

        public bool Visible { get; set; }

        public int Width { get; set; }

		#endregion Properties 

		#region Delegates and Events (1) 


		// Events (1) 

        public event FingerUI.StatusItem.ClickedWordDelegate WordClicked;


		#endregion Delegates and Events 

		#region Methods (2) 


		// Public Methods (2) 

        public void CheckForClicks(Point p)
        {
            int ItemHeight = (ClientSettings.TextSize * 2);
            int TopOfItem = ((this.Height / 2) - ((TextItems.Count * ItemHeight) / 2));
            foreach (string Item in TextItems)
            {
                Rectangle r = new Rectangle(this.Left, TopOfItem, this.Width, ItemHeight);
                if (r.Contains(p))
                {
                    this.Visible = false;
                    if (TextItems[_CurrentlyFocused] == "Exit")
                    {
                        _CurrentlyFocused = 0;
                        return;
                    }
                    if (WordClicked != null)
                    {
                        WordClicked(Item);
                        _CurrentlyFocused = 0;
                    }
                }
                TopOfItem = TopOfItem + ItemHeight;
            }
        }

        public void KeyDown(KeyEventArgs e)
        {
            lock (TextItems)
            {
                if (e.KeyCode == Keys.Down)
                {
                    if (_CurrentlyFocused < TextItems.Count - 1)
                    {
                        _CurrentlyFocused++;
                    }
                    else
                    {
                        _CurrentlyFocused = 0;
                    }
                }
                if (e.KeyCode == Keys.Up)
                {
                    if (_CurrentlyFocused > 0)
                    {
                        _CurrentlyFocused--;
                    }
                    else
                    {
                        _CurrentlyFocused = TextItems.Count - 1;
                    }
                }
                if (e.KeyCode == Keys.Enter)
                {
                    this.Visible = false;

                    if (TextItems[_CurrentlyFocused] == "Exit")
                    {
                        _CurrentlyFocused = 0;
                        return;
                    }
                    if (WordClicked != null)
                    {
                        WordClicked(TextItems[_CurrentlyFocused]);
                        _CurrentlyFocused = 0;
                    }
                }
            }
        }

        public void Render(Graphics g)
        {
            int ItemHeight = (ClientSettings.TextSize * 2);
            int TopOfItem = ((this.Height / 2) - ((TextItems.Count * ItemHeight) / 2));

            Region originalClip = g.Clip;
            g.Clip = new Region(new Rectangle(this.Left, this.Top, this.Width+1, this.Height+1));
            int i = 0;
            using (Pen whitePen = new Pen(ClientSettings.ForeColor))
            {
                foreach (string Item in TextItems)
                {
                    Rectangle r = new Rectangle(this.Left, TopOfItem, this.Width, ItemHeight);
                    int TextTop = ((r.Bottom - r.Top) / 2) + r.Top;
                    Color BackColor;
                    if (i == _CurrentlyFocused)
                    {
                        BackColor = ClientSettings.SelectedBackColor;
                    }
                    else
                    {
                        BackColor = ClientSettings.BackColor;
                    }
                    using (Brush b = new SolidBrush(BackColor))
                    {
                        g.FillRectangle(b, r);
                        g.DrawRectangle(whitePen, r);
                        StringFormat sFormat = new StringFormat();
                        sFormat.LineAlignment = StringAlignment.Center;
                        using (Brush c = new SolidBrush(ClientSettings.ForeColor))
                        {
                            
                            g.DrawString(Item, new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold), c, r.Left + 4, TextTop, sFormat);
                        }
                    }
                    TopOfItem = TopOfItem + ItemHeight;
                    i++;
                }
            }
            g.Clip = originalClip;
        }


		#endregion Methods 

    }
}
