using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PockeTwit
{
    class userMapDrawable : TiledMaps.IGraphicsDrawable
    {
        public Library.User userToDraw;
        public char charToUse;
        public bool IsOpened = false;
        public Bitmap markerImage = null;

        private Brush B = new SolidBrush(Color.Black);
        #region IGraphicsDrawable Members

        public void Draw(System.Drawing.Graphics graphics, System.Drawing.Rectangle destRect, System.Drawing.Rectangle sourceRect)
        {
            if (IsOpened)
            {
                graphics.DrawImage(ThrottledArtGrabber.GetArt(this.userToDraw.screen_name, this.userToDraw.high_profile_image_url), destRect.X, destRect.Y);
            }
            else
            {
                if (markerImage != null)
                {
                    //graphics.DrawImage(markerImage, destRect.X, destRect.Y);
                    //graphics.DrawString(charToUse.ToString(), ClientSettings.SmallFont, B, destRect);
                    //For now until I fix it.
                    graphics.DrawImage(ThrottledArtGrabber.GetArt(this.userToDraw.screen_name, this.userToDraw.high_profile_image_url), destRect.X, destRect.Y);
                }
                else
                {
                    graphics.DrawImage(ThrottledArtGrabber.GetArt(this.userToDraw.screen_name, this.userToDraw.high_profile_image_url), destRect.X, destRect.Y);
                }
            }
        }

        #endregion

        #region IMapDrawable Members

        public int Width
        {
            get 
            {
                return ClientSettings.SmallArtSize + (ClientSettings.Margin * 2);
            }
        }

        public int Height
        {
            get 
            {
                return ClientSettings.SmallArtSize + (ClientSettings.Margin * 2);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
