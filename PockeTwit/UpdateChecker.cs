using System;

using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;

namespace PockeTwit
{
    public static class UpdateChecker
    {
        private static double currentVersion = .07;
        private static string UpdateURL = "http://pocketwit.googlecode.com/svn/LatestRelease/Release.xml";
        private static string XMLResponse;
        private static UpdateInfo WebVersion;
        public delegate void delUpdateFound(UpdateInfo Info);
        public static event delUpdateFound UpdateFound;


        public struct UpdateInfo
        {
            public double webVersion;
            public string DownloadURL;
            public string UpdateNotes;
        }

        
        static UpdateChecker()
        {
            if (ClientSettings.CheckVersion)
            {
                CheckForUpdate();
            }
        }

        public static void CheckForUpdate()
        {
            System.Diagnostics.Debug.WriteLine("Autoupdate");
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(GetWebResponse);
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Name = "CheckUpdates";
            t.Start();
        }

        private static void GetWebResponse()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UpdateURL);

            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        XMLResponse = reader.ReadToEnd();
                    }
                }
            }
            catch{}
            XmlDocument UpdateInfoDoc = new XmlDocument();
            UpdateInfoDoc.LoadXml(XMLResponse);
            WebVersion = new UpdateInfo();
            WebVersion.webVersion = double.Parse(UpdateInfoDoc.SelectSingleNode("//version").InnerText);
            WebVersion.DownloadURL = UpdateInfoDoc.SelectSingleNode("//url").InnerText;
            WebVersion.UpdateNotes = UpdateInfoDoc.SelectSingleNode("//notes").InnerText;

            if (WebVersion.webVersion > currentVersion)
            {
                if (UpdateFound != null)
                {
                    UpdateFound(WebVersion);
                }
            }
        }
    

        
    }
}
