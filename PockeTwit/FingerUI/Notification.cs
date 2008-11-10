using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace FingerUI
{
    class NotificationPopup
    {
        public Font TextFont;
        private int AnimationStep = 0;
        private int _AnimationPixels = -1;
        private int AnimationPixels
        {
            get
            {
                if (_AnimationPixels < 0)
                {
                    if (ClientSettings.TextHeight == 192)
                    {
                        _AnimationPixels = 4;
                    }
                    else
                    {
                        _AnimationPixels = 1;
                    }
                }
                return _AnimationPixels;
            }
        }
        private bool Visibility = false;
        private int MaxHeight = ClientSettings.TextSize + (ClientSettings.Margin * 2);
        public KListControl parentControl;
        private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
        public NotificationPopup()
        {
            animationTimer.Interval = ClientSettings.AnimationInterval;
            animationTimer.Tick += new EventHandler(animationTimer_Tick);
            
        }

        void animationTimer_Tick(object sender, EventArgs e)
        {
            parentControl.Invalidate();
        }
        
        public void ShowNotification()
        {
            animationTimer.Enabled = true;
            Visibility = true;
        }

        public void HideNotification()
        {
            Visibility = false;
        }
        public void DrawNotification(Graphics g, int Bottom, int Width)
        {
            if (Visibility)
            {
                if (AnimationStep < MaxHeight) { AnimationStep= AnimationStep+AnimationPixels; }
            }
            else
            {
                if (AnimationStep > 0) { AnimationStep = AnimationStep - AnimationPixels; }
                else { animationTimer.Enabled = false; }

            }
            if (AnimationStep > 0)
            {
                Width = Width - (ClientSettings.Margin * 2);
                int Left = ClientSettings.Margin;
                using (Brush ForeBrush = new SolidBrush(ClientSettings.ForeColor))
                {
                    using (Pen p = new Pen(ClientSettings.ForeColor))
                    {

                        Rectangle boxPos = new Rectangle(Left, Bottom - AnimationStep, Width, ClientSettings.TextSize + (ClientSettings.Margin * 2));
                        Rectangle textPos = new Rectangle(Left + ClientSettings.Margin, (Bottom - AnimationStep)+ClientSettings.Margin, Width - ClientSettings.Margin, ClientSettings.TextSize + ClientSettings.Margin);
                        Gradient.GradientFill.Fill(g, boxPos, ClientSettings.SelectedBackGradColor, ClientSettings.SelectedBackColor, Gradient.GradientFill.FillDirection.TopToBottom);
                        g.DrawRectangle(p, boxPos);
                        g.DrawString("Fetching", TextFont, ForeBrush, textPos);
                    }
                }
            }
        }
    }
}
