using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PockeTwit
{
    public static class AsyncArtGrabber
    {
       
        public delegate void GrabArtdelegate(string User, string URL);
        public delegate void ArtIsReady(string User, string FileName);
        public static event ArtIsReady NewArtWasDownloaded;
        
        public static string AppPath;
        public static string CacheFolder;
        private static string UnknownArtMed;
        private static string UnknownArtSmall;
        static AsyncArtGrabber()
        {
            AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            CacheFolder = AppPath + "\\ArtCache";

            UnknownArtMed = AppPath + "\\unknownart-med.jpg";
            UnknownArtSmall = AppPath + "\\unknownart-small.jpg";
            if (!System.IO.Directory.Exists(CacheFolder))
            {
                System.IO.Directory.CreateDirectory(CacheFolder);
            }
        }

        public static string GetArtFileName(string User, string URL)
        {
            string FileName = CopyTempFile(User, URL);
            
            return FileName;
        }
        public static string ConvertFileToUrl(string Filename)
        {
            return "file://" + Filename.Replace("\\", "/");
        }
        public static string DetermineCacheFileName(string User, string URL)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[^\\w]");

            string UserFileName = r.Replace(User, "");


            string FileName = CacheFolder + "\\" + UserFileName;
            return FileName;
        }

        private static void CheckCacheFolderExists(string Folder)
        {
            if (!System.IO.Directory.Exists(CacheFolder + "\\" + Folder))
            {
                System.IO.Directory.CreateDirectory(CacheFolder + "\\" + Folder);
            }
        }

        public static string CopyTempFile(string User, string URL)
        {
            string FileName = DetermineCacheFileName(User, URL);
            if (!System.IO.File.Exists(FileName))
            {
                System.IO.File.Copy(UnknownArtSmall, FileName);
                
                Thread t = new Thread(delegate() { GetArtFromURL(User, URL); });
                t.IsBackground = true;
                t.Priority = ThreadPriority.BelowNormal;
                t.Start();
            }
            return FileName;
        }
        private static void GetArtFromURL(string User, string URL)
        {
            
            string LocalFileName = DetermineCacheFileName(User, URL);
            
            System.Net.HttpWebRequest GetArt = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
            System.Net.HttpWebResponse ArtResponse = (System.Net.HttpWebResponse)GetArt.GetResponse();
            if (ArtResponse != null)
            {
                System.IO.Stream responseStream = ArtResponse.GetResponseStream();
                System.IO.FileStream ArtWriter = new System.IO.FileStream(LocalFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read);

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
                ArtWriter.Close();
                responseStream.Close();
                
                //Shrink it to the client size.
                Bitmap original = new Bitmap(LocalFileName);
                Bitmap resized = new Bitmap(ClientSettings.SmallArtSize, ClientSettings.SmallArtSize);
                Graphics g = Graphics.FromImage(resized);
                g.DrawImage(original, new Rectangle(0, 0, ClientSettings.SmallArtSize, ClientSettings.SmallArtSize), new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
                g.Dispose();
                resized.Save(LocalFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                if (NewArtWasDownloaded != null)
                {
                    NewArtWasDownloaded.Invoke(User, LocalFileName);
                }
                return;
            }
            return;
        }
    }
}
