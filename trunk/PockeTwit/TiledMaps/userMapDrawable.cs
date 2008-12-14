using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PockeTwit
{
    class userMapDrawable : TiledMaps.IGraphicsDrawable
    {
        public Library.User userToDraw;
        

        #region IGraphicsDrawable Members

        public void Draw(System.Drawing.Graphics graphics, System.Drawing.Rectangle destRect, System.Drawing.Rectangle sourceRect)
        {
            graphics.DrawImage(ThrottledArtGrabber.GetArt(this.userToDraw.screen_name, this.userToDraw.high_profile_image_url), destRect.X, destRect.Y);
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
