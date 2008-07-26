using System;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    class isgd
    {
        public static string ShortenURL(string URL)
        {
            string TotalURL = "http://is.gd/api.php?longurl=" + URL;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(TotalURL);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
