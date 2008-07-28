using System;

using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;

namespace PockeTwit
{
    static class UpdateChecker
    {
        private static int currentVersion = 7;
        private static string UpdateURL = "http://pocketwit.googlecode.com/svn/LatestRelease/Release.xml";
        private static string XMLResponse;
        private static UpdateInfo WebVersion;


        public struct UpdateInfo
        {
            public int webVersion;
            public string DownloadURL;
            public string UpdateNotes;
        }

        
        static UpdateChecker()
        {
            if (ClientSettings.CheckVersion)
            {
                GetWebResponse();
            }
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
            WebVersion.webVersion = int.Parse(UpdateInfoDoc.SelectSingleNode("//version").InnerText);
            WebVersion.DownloadURL = UpdateInfoDoc.SelectSingleNode("//url").InnerText;
            WebVersion.UpdateNotes = UpdateInfoDoc.SelectSingleNode("//notes").InnerText;
        }
    

        private static bool UpdateIsAvailable()
        {
            return WebVersion.webVersion > currentVersion;
        }
    }
}
