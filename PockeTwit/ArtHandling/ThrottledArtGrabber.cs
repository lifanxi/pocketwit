using System;
using System.Data.SQLite;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace PockeTwit
{
    class ArtRequest
    {
        public ArtRequest(string url)
        {
            URL = url;
        }
        public string URL;

        public override bool Equals(object obj)
        {
            ArtRequest other = (ArtRequest)obj;
            return (other.URL == URL);
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
        private static System.Threading.Thread WorkerThread;
        static ThrottledArtGrabber()
        {
            Setup();
            mapMarkerImage = new TiledMaps.WinCEImagingBitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PockeTwit.Marker.png"));
            FavoriteImage = new Bitmap(ClientSettings.AppPath + "\\asterisk_yellow.png");            
        }

        private static void Setup()
        {
            
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
        }

        public delegate void ArtIsReady(string Argument);
        public static event ArtIsReady NewArtWasDownloaded;
        
        public static Image GetArt(string url)
        {
            if(string.IsNullOrEmpty(url) | string.IsNullOrEmpty((url)))
            {
                //Don't re-queue -- we won't be able to get it for now.
                return new Bitmap(UnknownArt);
            }
            if (url == "http://static.twitter.com/images/default_profile_normal.png")
            {
                return new Bitmap(DefaultArt);
            }
            lock (BadURLs)
            {
                if (BadURLs.Contains(url))
                {
                    return new Bitmap(UnknownArt);
                }
            }

            
            try
            {
                if(!HasArt(url))
                {
                    ArtRequest r = new ArtRequest(url);
                    QueueRequest(r);
                    return new Bitmap(UnknownArt);
                }
                else
                {
                    return GetBitmapFromDB(url);
                }
            }
            catch
            {
                return new Bitmap(UnknownArt);
            }
        }

        private static string CleanURL(string URL)
        {
            if(string.IsNullOrEmpty(URL))
            {
                return null;
            }
            return URL.ToLower().Replace("http://", "").Replace("https://", "").Replace("bigger", "").Replace("normal","");
        }

        private static Image GetBitmapFromDB(string url)
        {
            int bufferSize = 100;                  
            byte[] outbyte = new byte[bufferSize];
            long retval;                            
            long startIndex = 0;
            
            using (SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                MemoryStream s = new MemoryStream();
                using (SQLiteCommand comm = new SQLiteCommand(conn))
                {
                    comm.CommandText =
                        "SELECT avatar FROM avatarCache WHERE url=@url;";
                    comm.Parameters.Add(new SQLiteParameter("@url", url));

                    using(SQLiteDataReader r = comm.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            
                            BinaryWriter writer = new BinaryWriter(s);

                            retval = r.GetBytes(0, startIndex, outbyte, 0, bufferSize);
                            while (retval == bufferSize)
                            {
                                writer.Write(outbyte);
                                writer.Flush();

                                // Reposition the start index to the end of the last buffer and fill the buffer.
                                startIndex += bufferSize;
                                retval = r.GetBytes(0, startIndex, outbyte, 0, bufferSize);
                            }

                            writer.Write(outbyte, 0, (int)retval);
                            writer.Flush();
                            
                        }
                    }
                    
                    return new Bitmap(s);
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
        public static bool HasArt(string url)
        {
            using (SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (SQLiteCommand comm = new SQLiteCommand(conn))
                {
                    comm.CommandText =
                        "SELECT url FROM avatarCache WHERE url=@url;";
                    comm.Parameters.Add(new SQLiteParameter("@url", url));

                    return comm.ExecuteScalar()!=null;       
                }
            }
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
                    NewArtWasDownloaded.Invoke(r.URL);
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
            if (string.IsNullOrEmpty(r.URL))
            {
                return;
            }
            System.Net.HttpWebResponse ArtResponse = null;
            try
            {
                System.Net.HttpWebRequest GetArt = (System.Net.HttpWebRequest) System.Net.HttpWebRequest.Create(r.URL);
                GetArt.Timeout = 20000;
                ArtResponse = (System.Net.HttpWebResponse) GetArt.GetResponse();
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
            System.IO.MemoryStream ArtWriter = new System.IO.MemoryStream();
            try
            {
                responseStream = ArtResponse.GetResponseStream();

                int count = 0;
                byte[] buffer = new byte[8192];
                do
                {
                    count = responseStream.Read(buffer, 0, buffer.Length);
                    if (count != 0)
                    {
                        ArtWriter.Write(buffer, 0, count);
                    }
                } while (count != 0);
                responseStream.Close();

                ArtWriter.Seek(0, System.IO.SeekOrigin.Begin);
                using (Bitmap original = new Bitmap(ArtWriter))
                {
                    using (Bitmap resized = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize))
                    {
                        Graphics g = Graphics.FromImage(resized);
                        g.DrawImage(original,
                                    new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize),
                                    new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
                        g.Dispose();

                        byte[] blobdata = BmpToBytes_MemStream(resized);
                        using (System.Data.SQLite.SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
                        {
                            conn.Open();
                            using (System.Data.SQLite.SQLiteTransaction t = conn.BeginTransaction())
                            {

                                using (System.Data.SQLite.SQLiteCommand comm = new SQLiteCommand(conn))
                                {
                                    comm.CommandText =
                                        "INSERT INTO avatarCache (avatar, url) VALUES (@avatar, @url);";
                                    comm.Parameters.Add(new SQLiteParameter("@avatar", blobdata));
                                    comm.Parameters.Add(new SQLiteParameter("@url", r.URL));
                                    try
                                    {
                                        comm.ExecuteNonQuery();
                                    }
                                    catch(System.Data.SQLite.SQLiteException ex)
                                    {
                                    }
                                }

                                t.Commit();

                            }

                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
            }
            catch (Exception ex)
            {

                lock (BadURLs)
                {
                    AddBadURL(r.URL);
                    return;
                }
            }
            finally
            {
                ArtWriter.Close();
                NewArtWasDownloaded(r.URL);
            }
        }

        public static void ClearAvatars()
        {
            using(System.Data.SQLite.SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                using(System.Data.SQLite.SQLiteTransaction t = conn.BeginTransaction())
                {
                    using(System.Data.SQLite.SQLiteCommand comm = new SQLiteCommand(conn))
                    {
                        comm.CommandText = "DELETE FROM avatarCache";
                        comm.ExecuteNonQuery();

                        comm.CommandText = "VACUUM";
                        comm.ExecuteNonQuery();
                    }
                    t.Commit();
                }
            }
        }

        public static void ClearUnlinkedAvatars()
        {
            using (System.Data.SQLite.SQLiteConnection conn = LocalStorage.DataBaseUtility.GetConnection())
            {
                conn.Open();
                using (System.Data.SQLite.SQLiteTransaction t = conn.BeginTransaction())
                {
                    using (System.Data.SQLite.SQLiteCommand comm = new SQLiteCommand(conn))
                    {
                        comm.CommandText = @"DELETE FROM avatarCache WHERE url NOT IN (
                                                SELECT DISTINCT avatarURL FROM users
                                                );";
                        comm.ExecuteNonQuery();

                        comm.CommandText = "VACUUM";
                        comm.ExecuteNonQuery();
                    }
                    t.Commit();
                }
            }
        }

        private static byte[] BmpToBytes_MemStream(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            // Save to memory using the Jpeg format
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            // read to end
            byte[] bmpBytes = ms.GetBuffer();
            bmp.Dispose();
            ms.Close();

            return bmpBytes;
        }
        
    }
}
