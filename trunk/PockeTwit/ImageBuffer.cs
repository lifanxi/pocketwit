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
        private static Dictionary<string, Image> ImageDictionary = new Dictionary<string, Image>();
        static ImageBuffer()
        {
            FavoriteImage = new Bitmap(ClientSettings.AppPath + "\\asterisk_yellow.png");
            AsyncArtGrabber.NewArtWasDownloaded += new AsyncArtGrabber.ArtIsReady(AsyncArtGrabber_NewArtWasDownloaded);
        }

        public static Image GetArt(string User, string URL)
        {
            if (!ImageDictionary.ContainsKey(User))
            {
                System.Diagnostics.Debug.WriteLine("New item in dictionary -- " + User);
                LoadArt(User, URL);
            }
            return ImageDictionary[User];
        }

        private static void LoadArt(string User, string URL)
        {
            string ArtPath = AsyncArtGrabber.CopyTempFile(User, URL);
            Bitmap NewArt = new Bitmap(ArtPath);
            ImageDictionary.Add(User, NewArt);
        }

        private static void AsyncArtGrabber_NewArtWasDownloaded(string User, string Filename)
        {
            System.Diagnostics.Debug.WriteLine("New album art was fetched -- " + Filename);
            Bitmap NewArt = new Bitmap(Filename);
            ImageDictionary[User] = NewArt;
            if (Updated != null)
            {
                Updated(User);
            }
        }
    }
}
