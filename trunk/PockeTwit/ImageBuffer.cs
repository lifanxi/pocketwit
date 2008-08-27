using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    public static class ImageBuffer
    {
        class ImageInfo
        {
            public Image Image;
            public DateTime LastRequested;
        }
		#region Fields (3) 

        public static Bitmap FavoriteImage;
        private static Dictionary<string, ImageInfo> ImageDictionary = new Dictionary<string, ImageInfo>();
        public static Bitmap UnknownArt;
        private static System.Threading.Timer timerUpdate;

		#endregion Fields 

		#region Constructors (1) 

        static ImageBuffer()
        {
            FavoriteImage = new Bitmap(ClientSettings.AppPath + "\\asterisk_yellow.png");
            Bitmap DiskUnknown = new Bitmap(ClientSettings.AppPath + "\\unknownart-small.jpg");
            UnknownArt = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);
            Graphics g = Graphics.FromImage(UnknownArt);
            g.DrawImage(DiskUnknown, new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize), new Rectangle(0, 0, DiskUnknown.Width, DiskUnknown.Height), GraphicsUnit.Pixel);
            g.Dispose();
            AsyncArtGrabber.NewArtWasDownloaded += new AsyncArtGrabber.ArtIsReady(AsyncArtGrabber_NewArtWasDownloaded);
            timerUpdate = new System.Threading.Timer(new System.Threading.TimerCallback(Trim), null, 30000, 30000);
        }


		#endregion Constructors 

		#region Delegates and Events (2) 


		// Delegates (1) 

        public delegate void ArtWasUpdated(string User);


		// Events (1) 

        public static event ArtWasUpdated Updated;


		#endregion Delegates and Events 

		#region Methods (6) 


		// Public Methods (3) 

        public static void Clear()
        {
            ImageDictionary.Clear();
        }

        public static Image GetArt(string User)
        {
            if (!ImageDictionary.ContainsKey(User))
            {
                //How do we find art for a user by name alone?
                return UnknownArt;
            }
            ImageDictionary[User].LastRequested = DateTime.Now;
            return ImageDictionary[User].Image;
        }

        public static Image GetArt(string User, string URL)
        {
            if (User == null) { return null; }
            if (!ImageDictionary.ContainsKey(User))
            {
                if (!LoadArt(User, URL))
                {
                    if (string.IsNullOrEmpty(URL))
                    {
                        System.Diagnostics.Debug.WriteLine("Falling back to load user from screename");
                        Library.User newUser = null;
                        foreach (Yedda.Twitter.Account Account in ClientSettings.AccountsList)
                        {
                            newUser = Library.User.FromId(User, Account);
                            if (newUser != null) { break; }
                        }
                        if (newUser == null) { return UnknownArt; }
                        URL = newUser.profile_image_url;
                        LoadArt(User, URL);
                    }
                    return UnknownArt;
                }
            }
            ImageDictionary[User].LastRequested = DateTime.Now;
            return ImageDictionary[User].Image;
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

        public static void Trim(object state)
        {
            DateTime runTime = DateTime.Now;
            List<string> Keys = new List<string>(ImageDictionary.Keys);

            foreach (string infoKey in Keys)
            {
                ImageInfo info = ImageDictionary[infoKey];
                if (runTime.Ticks - info.LastRequested.Ticks > 30000)
                {
                    System.Diagnostics.Debug.WriteLine("Removing " + infoKey + " from imagebuffer");
                    ImageDictionary.Remove(infoKey);
                }
            }
        }

		// Private Methods (3) 

        private static void AsyncArtGrabber_NewArtWasDownloaded(string User, string Filename)
        {
            if (System.IO.File.Exists(Filename))
            {
                try
                {
                    Bitmap NewArt = new Bitmap(Filename);
                    ImageInfo newInfo = new ImageInfo();
                    newInfo.Image = NewArt;
                    newInfo.LastRequested = DateTime.Now;
                    ImageDictionary[User] = newInfo;
                    if (Updated != null)
                    {
                        Updated(User);
                    }
                }
                catch
                {
                    //Try again next time.
                    System.IO.File.Delete(Filename);
                }
            }
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
            ImageInfo newInfo = new ImageInfo();
            newInfo.LastRequested = DateTime.Now;
            newInfo.Image = NewArt;
            ImageDictionary.Add(User, newInfo);
            return true;
        }

        private static bool LoadArt(string User, string URL)
        {
            string ArtPath = AsyncArtGrabber.CopyTempFile(User, URL);
            Bitmap NewArt;
            bool bFound = false;
            try
            {
                if (ArtPath != null)
                {
                    NewArt = new Bitmap(ArtPath);
                    bFound = true;
                }
                else
                {
                    NewArt = UnknownArt;
                }
                if (ImageDictionary.ContainsKey(User))
                {
                    ImageDictionary.Remove(User);
                }
                ImageInfo newInfo = new ImageInfo();
                newInfo.LastRequested = DateTime.Now;
                newInfo.Image = NewArt;
                ImageDictionary.Add(User, newInfo);
                return bFound;
            }
            catch
            {
                return false;
            }
        }


		#endregion Methods 

    }
}
