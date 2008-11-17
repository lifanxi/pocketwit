using System;

using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;

namespace PockeTwit
{
    public class UpdateChecker
    {

		#region Fields (4) 

        public static double currentVersion = .51;
        public static bool devBuild = false;

        private string UpdateURL = "http://pocketwit.googlecode.com/svn/LatestRelease/Release.xml";
        private UpdateInfo WebVersion;
        private string XMLResponse;

		#endregion Fields 

		#region Constructors (2) 

        public UpdateChecker(bool Auto)
        {
            if (Auto)
            {
                CheckForUpdate();
            }
        }

        public UpdateChecker()
        {
            if (ClientSettings.CheckVersion)
            {
                CheckForUpdate();
            }
        }

		#endregion Constructors 

		#region Delegates and Events (3) 


		// Delegates (1) 

        public delegate void delUpdateFound(UpdateInfo Info);


		// Events (2) 

        public event delUpdateFound CurrentVersion;

        public event delUpdateFound UpdateFound;


		#endregion Delegates and Events 

		#region Methods (2) 


		// Public Methods (1) 

        public void CheckForUpdate()
        {
            if (!devBuild)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GetWebResponse));
            }
        }



		// Private Methods (1) 

        private void GetWebResponse(object o)
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
            try
            {
                if (XMLResponse != null)
                {
                    UpdateInfoDoc.LoadXml(XMLResponse);
                    WebVersion = new UpdateInfo();
                    WebVersion.webVersion = double.Parse(UpdateInfoDoc.SelectSingleNode("//version").InnerText,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    WebVersion.DownloadURL = UpdateInfoDoc.SelectSingleNode("//url").InnerText;
                    WebVersion.UpdateNotes = UpdateInfoDoc.SelectSingleNode("//notes").InnerText;

                    if (WebVersion.webVersion > currentVersion)
                    {
                        if (UpdateFound != null)
                        {
                            UpdateFound(WebVersion);
                        }
                    }
                    else
                    {
                        if (CurrentVersion != null)
                        {
                            CurrentVersion(WebVersion);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("Update check complete");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Update check failed");
                }
            }
            catch
            {
            }
        }


		#endregion Methods 
        public struct UpdateInfo
        {
            public double webVersion;
            public string DownloadURL;
            public string UpdateNotes;
        }

    }
}
