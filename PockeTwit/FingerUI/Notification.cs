using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace FingerUI
{
    class NotificationPopup
    {
        public bool AtTop = false;
        private string _DisplayText;
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
        public int Pause = -1;
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
        
        
        public void ShowNotification(string Text)
        {
            if (!Visibility)
            {
                _DisplayText = Text;
                animationTimer.Enabled = true;
                Visibility = true;
            }
            else
            {
                if (Text != _DisplayText)
                {
                    _DisplayText = Text;
                }
            }
        }

        public void HideNotification()
        {
            if (Pause <= 0)
            {
                Visibility = false;
            }
        }
        public void DrawNotification(Graphics g, int Bottom, int Width)
        {
            if (Visibility)
            {
                if (AnimationStep < MaxHeight) { AnimationStep = AnimationStep+AnimationPixels; }
                if (AnimationStep >= MaxHeight) 
                { 
                    AnimationStep = MaxHeight;
                    if (Pause == 0)
                    {
                        Visibility = false;
                    }
                    else { Pause--; }
                }
            }
            else
            {
                if (AnimationStep > 0) 
                {
                    if (Pause > 0)
                    {
                        Pause--;
                    }
                    else
                    {
                        AnimationStep = AnimationStep - AnimationPixels;
                    }
                }
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
                        Rectangle boxPos;
                        Rectangle textPos;
                        if (AtTop)
                        {
                            boxPos = new Rectangle(Left, AnimationStep - (MaxHeight+3), Width, ClientSettings.TextSize + (ClientSettings.Margin * 2));
                            textPos = new Rectangle(Left + ClientSettings.Margin, (AnimationStep - (MaxHeight+3)) + ClientSettings.Margin, Width - ClientSettings.Margin, ClientSettings.TextSize + ClientSettings.Margin);
                        }
                        else
                        {
                            boxPos = new Rectangle(Left, Bottom - AnimationStep, Width, ClientSettings.TextSize + (ClientSettings.Margin * 2));
                            textPos = new Rectangle(Left + ClientSettings.Margin, (Bottom - AnimationStep) + ClientSettings.Margin, Width - ClientSettings.Margin, ClientSettings.TextSize + ClientSettings.Margin);
                        }
                        Gradient.GradientFill.Fill(g, boxPos, ClientSettings.SelectedBackColor, ClientSettings.SelectedBackGradColor, Gradient.GradientFill.FillDirection.TopToBottom);
                        g.DrawRectangle(p, boxPos);
                        g.DrawString(_DisplayText, TextFont, ForeBrush, textPos);
                    }
                }
            }
        }
    }
}
