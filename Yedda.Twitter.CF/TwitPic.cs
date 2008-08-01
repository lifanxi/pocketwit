using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Yedda
{
    public class TwitPic
    {
        protected const string TwitPicBaseURL = "http://twitpic.com/api/{0}";
        

        public enum ActionType
        {
            uploadAndPost,
            upload
        }

        protected string GetActionTypeString(ActionType actionType)
        {
            return actionType.ToString();
        }

        protected string ExecutePostCommand(string url, string userName, string password, string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                request.Credentials = new NetworkCredential(userName, password);
                request.PreAuthenticate = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                request.Timeout = 20000;

                /*
                if (!string.IsNullOrEmpty(TwitterClient))
                {
                    request.Headers.Add("X-Twitter-Client", TwitterClient);
                }

                if (!string.IsNullOrEmpty(TwitterClientVersion))
                {
                    request.Headers.Add("X-Twitter-Version", TwitterClientVersion);
                }

                if (!string.IsNullOrEmpty(TwitterClientUrl))
                {
                    request.Headers.Add("X-Twitter-URL", TwitterClientUrl);
                }


                if (!string.IsNullOrEmpty(Source))
                {
                    data += "&source=" + HttpUtility.UrlEncode(Source);
                }
                 */

                byte[] bytes = Encoding.UTF8.GetBytes(data);

                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Flush();
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            return null;
        }
    }
}
