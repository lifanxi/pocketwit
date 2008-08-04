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
        
        public static string CacheFolder;
        //private static string UnknownArtSmall;
        static AsyncArtGrabber()
        {
            CacheFolder = ClientSettings.AppPath + "\\ArtCache";

            //UnknownArtSmall = ClientSettings.AppPath + "\\unknownart-small.jpg";
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
        public static string DetermineCacheFileName(string User)
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
            string FileName = DetermineCacheFileName(User);
            if (!System.IO.File.Exists(FileName))
            {
                Thread t = new Thread(delegate() { GetArtFromURL(User, URL); });
                t.IsBackground = true;
                t.Priority = ThreadPriority.BelowNormal;
                t.Start();
                return null;
            }
            return FileName;
        }
        private static void GetArtFromURL(string User, string URL)
        {
            
            string LocalFileName = DetermineCacheFileName(User);
            System.Net.HttpWebResponse ArtResponse = null;
            try
            {
                System.Net.HttpWebRequest GetArt = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                ArtResponse = (System.Net.HttpWebResponse)GetArt.GetResponse();
            }
            catch { }
            if (ArtResponse == null)
            {
                DeleteTempArt(User, URL);
                return;
            }
            System.IO.Stream responseStream;
            System.IO.FileStream ArtWriter;
            try
            {
                responseStream = ArtResponse.GetResponseStream();
            
                ArtWriter = new System.IO.FileStream(LocalFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read);

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
            }
            catch
            {
                DeleteTempArt(User, URL);
                return;
            }
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

        private static void DeleteTempArt(string User, string URL)
        {
            string LocalFileName = DetermineCacheFileName(User);
            if (System.IO.File.Exists(LocalFileName))
            {
                System.IO.File.Delete(LocalFileName);
            }

        }
    }
}
