using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace FingerUI
{
    /// <summary>
    /// A basic implementation of the IKListItem interface.
    /// </summary>
    public class KListItem : KListControl.IKListItem, IDisposable
    {

		#region Fields (9) 

        private Rectangle m_bounds;
        private KListControl m_parent;
        private bool m_selected = false;
        private static Bitmap m_selectionBitmap;
        private StringFormat m_stringFormat = new StringFormat();
        private string m_text;
        private object m_value;
        private int m_x = -1;
        private int m_y = -1;

		#endregion Fields 

		#region Constructors (2) 

        /// <summary>
        /// Initializes a new instance of the <see cref="KListItem"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        public KListItem(KListControl parent, string text, object value)
        {
            m_parent = parent;
            m_text = text;
            m_value = value;
            m_stringFormat.Alignment = StringAlignment.Near;
            m_stringFormat.LineAlignment = StringAlignment.Center;
        }

        /// <summary>
        /// Initializes the <see cref="KListItem"/> class.
        /// </summary>
        static KListItem()
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

		#endregion Constructors 

		#region Properties (7) 

        /// <summary>
        /// The unscrolled bounds for this item.
        /// </summary>
        public Rectangle Bounds { get { return m_bounds; } set { m_bounds = value; } }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public int Index { get { return m_y; } set { m_y = value; } }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public KListControl Parent { get { return m_parent; } set { m_parent = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KListItem"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected { get { return m_selected;  } set { m_selected = value; } }

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
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public int XIndex { get { return m_x; } set { m_x = value; } }

		#endregion Properties 

		#region Methods (2) 


		// Public Methods (2) 

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            m_parent = null;
        }

        /// <summary>
        /// Renders to the specified graphics.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="bounds">The bounds.</param>
        public virtual void Render(Graphics g, Rectangle bounds)
        {
            if (m_selected)
            {
                // Draw the selection image if available, otherwise just a gray box.
                if (m_selectionBitmap != null)
                {
                    g.DrawImage(m_selectionBitmap, bounds, new Rectangle(0, 0, m_selectionBitmap.Width, m_selectionBitmap.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    //g.DrawRectangle(new Pen(Color.Black), bounds);
                    Rectangle InnerBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                    InnerBounds.Offset(1, 1);
                    InnerBounds.Width--; InnerBounds.Height--;
                    using (SolidBrush FillColor = new SolidBrush(ClientSettings.BackColor))
                    {
                        g.FillRectangle(FillColor, InnerBounds);
                    }
                }
            }

            // Now draw the label.
            if (!string.IsNullOrEmpty(m_text))
            {
                Font TextFont = m_parent.Font;
                SolidBrush ColorBrush = new SolidBrush(ClientSettings.ForeColor);
                if (m_selected) 
                {
                    ColorBrush = new SolidBrush(ClientSettings.SelectedForeColor); 
                }
                Rectangle textBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                textBounds.Offset(1, 1);
                textBounds.Width--;
                textBounds.Height--;
                g.DrawString(m_text, TextFont, ColorBrush, textBounds, m_stringFormat);
                ColorBrush.Dispose();
            }
        }


		#endregion Methods 

    }
}
