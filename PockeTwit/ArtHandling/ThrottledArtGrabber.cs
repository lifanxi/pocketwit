using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

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
        public static Bitmap FavoriteImage;
        public static Bitmap UnknownArt;
        private static Queue<ArtRequest> Requests = new Queue<ArtRequest>();
        private static List<string> BadURLs = new List<string>();
        public static string CacheFolder;
        private static System.Threading.Thread WorkerThread;
        static ThrottledArtGrabber()
        {
            CacheFolder = ClientSettings.AppPath + "\\ArtCache";

            if (!System.IO.Directory.Exists(CacheFolder))
            {
                System.IO.Directory.CreateDirectory(CacheFolder);
            }
            if (!System.IO.Directory.Exists(CacheFolder + "\\Unknown"))
            {
                System.IO.Directory.CreateDirectory(CacheFolder+"\\Unknown");
            }
            FavoriteImage = new Bitmap(ClientSettings.AppPath + "\\asterisk_yellow.png");
            Bitmap DiskUnknown = new Bitmap(ClientSettings.AppPath + "\\unknownart-small.jpg");
            UnknownArt = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);
            Graphics g = Graphics.FromImage(UnknownArt);
            g.DrawImage(DiskUnknown, new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize), new Rectangle(0, 0, DiskUnknown.Width, DiskUnknown.Height), GraphicsUnit.Pixel);
            g.Dispose();
            if(!System.IO.File.Exists(CacheFolder+"\\Unknown"+"\\PockeTwitUnknownUser"))
            {
                UnknownArt.Save(CacheFolder+"\\Unknown"+"\\PockeTwitUnknownUser", System.Drawing.Imaging.ImageFormat.Bmp);
            }
            
        }

        public delegate void ArtIsReady(string Argument);
        public static event ArtIsReady NewArtWasDownloaded;
        
        public static Image GetArt(string user, string url)
        {
            string ArtName = DetermineCacheFileName(user, url);
            string ID = url;
            lock (BadURLs)
            {
                if (BadURLs.Contains(url))
                {
                    System.Diagnostics.Debug.WriteLine("Bad URL");
                    return UnknownArt;
                }
            }
                        
            if (!System.IO.File.Exists(ArtName + ".ID"))
            {
                ArtRequest r = new ArtRequest(user, url);
                QueueRequest(r);
                return UnknownArt;
            }
            else
            {
                
                string ID2;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(ArtName + ".ID"))
                {
                    ID2 = reader.ReadToEnd();
                }
                if (ID != ID2)
                {
                    ArtRequest r = new ArtRequest(user, url);
                    QueueRequest(r);
                    return UnknownArt;
                }
                else
                {
                    using (System.IO.FileStream s = new System.IO.FileStream(ArtName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        return new Bitmap(s);
                    }
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
                    //System.Diagnostics.Debug.WriteLine("Queue " + r.User);
                }
            }
            if (WorkerThread==null)
            {
                System.Diagnostics.Debug.WriteLine("New Queue, starting worker");
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
            while (Requests.Count > 0)
            {
                ArtRequest r;
                lock (Requests)
                {
                    r = Requests.Peek();
                }
                System.Diagnostics.Debug.WriteLine("Fetching " + r.User);
                FetchRequest(r);
                System.Diagnostics.Debug.WriteLine("Fetched " + r.User);
                lock (Requests)
                {
                    Requests.Dequeue();
                }
                if (NewArtWasDownloaded != null)
                {
                    NewArtWasDownloaded.Invoke(r.User);
                }
            }
            System.Diagnostics.Debug.WriteLine("Queue empty, closing shop.");
            WorkerThread = null;
        }

        private static void FetchRequest(ArtRequest r)
        {
            if (string.IsNullOrEmpty(r.URL))
            {
                r.URL = TryToFindURL(r.User);
            }
            string LocalFileName = DetermineCacheFileName(r.User, r.URL);
            System.Net.HttpWebResponse ArtResponse = null;
            try
            {
                System.Net.HttpWebRequest GetArt = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(r.URL);
                ArtResponse = (System.Net.HttpWebResponse)GetArt.GetResponse();
            }
            catch (Exception ex)
            {
                lock (BadURLs)
                {
                    BadURLs.Add(r.URL);
                    return;
                }
            }
            if (ArtResponse == null)
            {

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
                //ArtWriter = new System.IO.FileStream(LocalFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read);
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
                Bitmap original = new Bitmap(ArtWriter);
                Bitmap resized = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);
                Graphics g = Graphics.FromImage(resized);
                g.DrawImage(original, new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize), new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
                g.Dispose();
                resized.Save(LocalFileName, System.Drawing.Imaging.ImageFormat.Bmp);
                WriteID(LocalFileName, r.URL);
                ArtWriter.Close();
            }
            catch(Exception ex) 
            {
            }
        }

        public static string DetermineCacheFileName(string User, string URL)
        {
            string Folder = "Unknown";
            if (!string.IsNullOrEmpty(URL))
            {
                System.Uri U = new Uri(URL);
                Folder = U.Host;
            }
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[^\\w]");
            string UserFileName = r.Replace(User, "");


            string FileName = CacheFolder + "\\" + Folder + "\\" + UserFileName;
            return FileName;
        }


        public static string TryToFindURL(string User)
        {
            System.Diagnostics.Debug.WriteLine("Falling back to load user from screename");
            Library.User newUser = null;
            foreach (Yedda.Twitter.Account Account in ClientSettings.AccountsList)
            {
                newUser = Library.User.FromId(User, Account);
                if (newUser != null) { break; }
            }
            if (newUser == null) { return null; }
            return newUser.profile_image_url;
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
    }
}
