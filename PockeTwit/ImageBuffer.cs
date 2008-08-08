using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    public static class ImageBuffer
    {
        public delegate void ArtWasUpdated(string User);
        public static event ArtWasUpdated Updated;
        public static Bitmap FavoriteImage;
        public static Bitmap UnknownArt;
        private static Dictionary<string, Image> ImageDictionary = new Dictionary<string, Image>();
        static ImageBuffer()
        {
            FavoriteImage = new Bitmap(ClientSettings.AppPath + "\\asterisk_yellow.png");
            Bitmap DiskUnknown = new Bitmap(ClientSettings.AppPath + "\\unknownart-small.jpg");
            UnknownArt = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);
            Graphics g = Graphics.FromImage(UnknownArt);
            g.DrawImage(DiskUnknown, new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize), new Rectangle(0, 0, DiskUnknown.Width, DiskUnknown.Height), GraphicsUnit.Pixel);
            g.Dispose();
            AsyncArtGrabber.NewArtWasDownloaded += new AsyncArtGrabber.ArtIsReady(AsyncArtGrabber_NewArtWasDownloaded);
        }

        public static bool HasArt(string User)
        {
            if (ImageDictionary.ContainsKey(User))
            {
                return true;
            }
            string ArtPath = AsyncArtGrabber.DetermineCacheFileName(User);
            if (System.IO.File.Exists(ArtPath))
            {
                LoadArt(User);
                return true;
            }
            return false;
        }

        public static Image GetArt(string User)
        {
            if (!ImageDictionary.ContainsKey(User))
            {
                //How do we find art for a user by name alone?
                return UnknownArt;
            }
            return ImageDictionary[User];
        }

        public static Image GetArt(string User, string URL)
        {
            if (User == null) { return null; }
            if (!ImageDictionary.ContainsKey(User))
            {
                if (!LoadArt(User, URL))
                {
                    return UnknownArt;
                }
            }
            return ImageDictionary[User];
        }

        private static bool LoadArt(string User)
        {
            string ArtPath = AsyncArtGrabber.DetermineCacheFileName(User);
            Bitmap NewArt;
            try
            {
                NewArt = new Bitmap(ArtPath);
            }
            catch (System.IO.IOException)
            {
                return false;
            }
            ImageDictionary.Add(User, NewArt);
            return true;
        }

        private static bool LoadArt(string User, string URL)
        {
            string ArtPath = AsyncArtGrabber.CopyTempFile(User, URL);
            Bitmap NewArt;
            bool bFound = false;
            if (ArtPath != null)
            {
                NewArt = new Bitmap(ArtPath);
                bFound = true;
            }
            else
            {
                NewArt = UnknownArt;
            }
            ImageDictionary.Add(User, NewArt);
            return bFound;
        }

        private static void AsyncArtGrabber_NewArtWasDownloaded(string User, string Filename)
        {
            Bitmap NewArt = new Bitmap(Filename);
            ImageDictionary[User] = NewArt;
            if (Updated != null)
            {
                Updated(User);
            }
        }
    }
}
