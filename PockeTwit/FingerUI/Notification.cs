using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace FingerUI
{
    class NotificationPopup
    {
        public Font TextFont;
        private int Visibility = 0;


        public void DrawNotification(Graphics g, int Bottom, int Width)
        {
            Width = Width - (ClientSettings.Margin*2);
            int Left = ClientSettings.Margin;
            using (Brush ForeBrush = new SolidBrush(ClientSettings.ForeColor))
            {
                using (Pen p = new Pen(ClientSettings.ForeColor))
                {
                    
                    Rectangle boxPos = new Rectangle(Left, Bottom - (ClientSettings.TextSize + (ClientSettings.Margin * 2)), Width, ClientSettings.TextSize + (ClientSettings.Margin * 2));
                    Rectangle textPos = new Rectangle(Left+ClientSettings.Margin, Bottom - (ClientSettings.TextSize + ClientSettings.Margin), Width- ClientSettings.Margin, ClientSettings.TextSize + ClientSettings.Margin);
                    Gradient.GradientFill.Fill(g, boxPos, ClientSettings.SelectedBackGradColor, ClientSettings.SelectedBackColor, Gradient.GradientFill.FillDirection.TopToBottom);
                    g.DrawRectangle(p, boxPos);
                    g.DrawString("Fetching", TextFont, ForeBrush, textPos);
                }
            }
        }
    }
}
