using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace PockeTwit
{
    class ArtRequest
    {
        public ArtRequest(string user, string url)
        {
            User = user.ToLower();
            URL = url;
        }
        public string URL;
        public string User;

        public override bool Equals(object obj)
        {
            ArtRequest other = (ArtRequest)obj;
            return (other.URL == URL && other.User == User);
        }
    }
    static class ThrottledArtGrabber
    {
        public static bool running = true;
        private static System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[^\\w]", System.Text.RegularExpressions.RegexOptions.Compiled);
        public static Bitmap FavoriteImage;
        public static Bitmap UnknownArt;
        public static Bitmap DefaultArt;
        public static TiledMaps.WinCEImagingBitmap mapMarkerImage;
        private static Queue<ArtRequest> Requests = new Queue<ArtRequest>();
        private static List<string> BadURLs = new List<string>();
        public static string CacheFolder;
        private static System.Threading.Thread WorkerThread;
        static ThrottledArtGrabber()
        {
            SetupCacheDir();
            mapMarkerImage = new TiledMaps.WinCEImagingBitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PockeTwit.Marker.png"));
            FavoriteImage = new Bitmap(ClientSettings.AppPath + "\\asterisk_yellow.png");            
        }

        public static void ResetCacheDir()
        {
            System.IO.Directory.Delete(CacheFolder, true);
            SetupCacheDir();
        }

        private static void SetupCacheDir()
        {
            if (string.IsNullOrEmpty(ClientSettings.CacheDir))
            {
                CacheFolder = ClientSettings.AppPath + "\\ArtCache";
            }
            else
            {
                CacheFolder = ClientSettings.CacheDir + "\\ArtCache";
            }

            if (!System.IO.Directory.Exists(CacheFolder))
            {
                System.IO.Directory.CreateDirectory(CacheFolder);
            }
            if (!System.IO.Directory.Exists(CacheFolder + "\\Unknown"))
            {
                System.IO.Directory.CreateDirectory(CacheFolder + "\\Unknown");
            }
            UnknownArt = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);
            DefaultArt = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);


            
            Bitmap DiskUnknown = new Bitmap(ClientSettings.AppPath + "\\unknownart-small.jpg");
            Bitmap DiskDefault = new Bitmap(ClientSettings.AppPath + "\\default_profile_bigger.png");
            using (Graphics g = Graphics.FromImage(UnknownArt))
            {
                g.DrawImage(DiskUnknown, new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize), new Rectangle(0, 0, DiskUnknown.Width, DiskUnknown.Height), GraphicsUnit.Pixel);
            }

            using (Graphics g = Graphics.FromImage(DefaultArt))
            {
                g.DrawImage(DiskDefault, new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize), new Rectangle(0, 0, DiskDefault.Width, DiskDefault.Height), GraphicsUnit.Pixel);
            }
            DiskUnknown.Dispose();
            DiskDefault.Dispose();

            if (!System.IO.File.Exists(CacheFolder + "\\Unknown" + "\\PockeTwitUnknownUser"))
            {
                UnknownArt.Save(CacheFolder + "\\Unknown" + "\\PockeTwitUnknownUser", System.Drawing.Imaging.ImageFormat.Bmp);
            }

            

        }

        public delegate void ArtIsReady(string Argument);
        public static event ArtIsReady NewArtWasDownloaded;
        
        public static Image GetArt(string user, string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                //Don't re-queue -- we won't be able to get it for now.
                return new Bitmap(UnknownArt);
            }
            if (url == "http://static.twitter.com/images/default_profile_normal.png")
            {
                return new Bitmap(DefaultArt);
            }
            string ID = url.Replace("_bigger","").Replace("_normal","").Replace("https","").Replace("http","").ToLower() ;
            string ArtName = DetermineCacheFileName(user, url);
            lock (BadURLs)
            {
                if (BadURLs.Contains(url))
                {
                    return new Bitmap(UnknownArt);
                }
            }
                        
            if (!System.IO.File.Exists(ArtName + ".ID"))
            {
                //No existing ID, we'll queue a fetch
                ArtRequest r = new ArtRequest(user, url);
                QueueRequest(r);

                //Should we write ID now to prevent multiple requests?
                return new Bitmap(UnknownArt);
            }
            else
            {
                try
                {
                    string ID2;
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(ArtName + ".ID"))
                    {
                        ID2 = reader.ReadToEnd().ToLower();
                    }
                    if (ID != ID2)
                    {
                        ArtRequest r = new ArtRequest(user, url);
                        QueueRequest(r);
                        return new Bitmap(UnknownArt);
                    }
                    else
                    {
                        try
                        {
                            using (System.IO.FileStream s = new System.IO.FileStream(ArtName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                            {
                                return new Bitmap(s);
                            }
                        }
                        catch
                        {
                            return new Bitmap(UnknownArt);
                        }
                    }
                }
                catch
                {
                    return new Bitmap(UnknownArt);
                }
            }
        }

        private static void QueueRequest(ArtRequest r)
        {
            lock (Requests)
            {
                if (!Requests.Contains(r))
                {
                    Requests.Enqueue(r);
                }
            }
            if (WorkerThread==null)
            {
                WorkerThread = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessQueue));
                WorkerThread.Name = "AvatarFetcher";
                WorkerThread.Start();
            }
        
        }
        public static bool HasArt(string user)
        {
            string u = user.ToLower();
            string LocalFileName = DetermineCacheFileName(user, "");
            return System.IO.File.Exists(LocalFileName);
        }
        private static void ProcessQueue()
        {
            while (Requests.Count > 0 && running)
            {
                ArtRequest r;
                lock (Requests)
                {
                    r = Requests.Peek();
                }
                FetchRequest(r);
                lock (Requests)
                {
                    Requests.Dequeue();
                }
                if (NewArtWasDownloaded != null)
                {
                    NewArtWasDownloaded.Invoke(r.User);
                }
            }
            WorkerThread = null;
        }

        private static void AddBadURL(string URL)
        {
            if (URL == "http://static.twitter.com/images/default_profile_normal.png")
            {
                return;
            }
            lock (BadURLs)
            {
                BadURLs.Add(URL);
            }
        }

        private static void FetchRequest(ArtRequest r)
        {
            System.Diagnostics.Debug.WriteLine("Processing " + r.User);
            if (string.IsNullOrEmpty(r.URL))
            {
                return;
            }
            string LocalFileName = DetermineCacheFileName(r.User, r.URL);
            System.Net.HttpWebResponse ArtResponse = null;
            try
            {
                System.Net.HttpWebRequest GetArt = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(r.URL);
                GetArt.Timeout = 20000;
                ArtResponse = (System.Net.HttpWebResponse)GetArt.GetResponse();
            }
            catch (Exception ex)
            {
                lock (BadURLs)
                {
                    AddBadURL(r.URL);
                    return;
                }
            }
            if (ArtResponse == null)
            {
                lock (BadURLs)
                {
                    AddBadURL(r.URL);
                    return;
                }
            }
            System.IO.Stream responseStream = null;
            System.IO.MemoryStream ArtWriter = null;
            try
            {
                responseStream = ArtResponse.GetResponseStream();

                if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(LocalFileName)))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(LocalFileName));
                }
                ArtWriter = new System.IO.MemoryStream();
                int count = 0;
                byte[] buffer = new byte[8192];
                do
                {
                    count = responseStream.Read(buffer, 0, buffer.Length);
                    if (count != 0)
                    {
                        ArtWriter.Write(buffer, 0, count);
                    }
                }
                while (count != 0);
                responseStream.Close();

                ArtWriter.Seek(0, System.IO.SeekOrigin.Begin);
                using (Bitmap original = new Bitmap(ArtWriter))
                {
                    using (Bitmap resized = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize))
                    {
                        Graphics g = Graphics.FromImage(resized);
                        g.DrawImage(original, new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize), new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
                        g.Dispose();
                        resized.Save(LocalFileName, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
                WriteID(LocalFileName, r.URL.Replace("_bigger","").Replace("_normal","").Replace("https","").Replace("http","").ToLower());
                ArtWriter.Close();
            }
            catch(Exception ex) 
            {
                lock (BadURLs)
                {
                    AddBadURL(r.URL);
                    DeleteID(LocalFileName);
                    return;
                }
            }
        }

        public static string DetermineCacheFileName(string User, string URL)
        {
            string Folder = "Unknown";
            
            if (!string.IsNullOrEmpty(URL))
            {
                Folder = URL.Substring(7, URL.IndexOf('/', 8));
            }
            
            string UserFileName = User.Replace("/","").Replace("\\","").Replace("?","").Replace("!","");
            char SubFolder = UserFileName[0];

            string FileName = CacheFolder + "\\" + Folder + "\\" + "\\" + SubFolder + "\\" + UserFileName;
            
            return FileName;
        }


        private static void WriteID(string ArtPath, string ID)
        {
            System.IO.FileStream fs = new System.IO.FileStream(ArtPath + ".ID", System.IO.FileMode.Create, System.IO.FileAccess.Write);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
            {
                sw.Write(ID);
                sw.Flush();
                sw.Close();
            }
        }
        private static void DeleteID(string ArtPath)
        {
            try
            {
                System.IO.File.Delete(ArtPath + ".ID");
            }
            catch { }
        }
    }
}
