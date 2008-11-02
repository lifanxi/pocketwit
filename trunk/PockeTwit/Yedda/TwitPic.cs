using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Yedda
{
    public static class TwitPic
    {
        
        private static string ExecutePostCommand(string url, string userName, string password, byte[] photo, string message)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {

                string boundary = System.Guid.NewGuid().ToString();
                request.Credentials = new NetworkCredential(userName, password);
                request.Headers.Add("Accept-Language", "cs,en-us;q=0.7,en;q=0.3");
                request.PreAuthenticate = true;
                request.ContentType = string.Format("multipart/form-data;boundary={0}", boundary);
                

                //request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                request.Timeout = 20000;
                string header = string.Format("--{0}", boundary);
                string footer = header + "--";

                StringBuilder contents = new StringBuilder();

                
                contents.Append(header);
                contents.Append("\r\n");
                contents.Append("Content-Disposition: form-data;name=\"username\"\r\n");
                contents.Append("\r\n");
                contents.Append(userName);
                contents.Append("\r\n");

                contents.Append(header);
                contents.Append("\r\n");
                contents.Append("Content-Disposition: form-data;name=\"password\"\r\n");
                contents.Append("\r\n");
                contents.Append(password);
                contents.Append("\r\n");

                contents.Append(header);
                contents.Append("\r\n");
                contents.Append("Content-Disposition: form-data;name=\"source\"\r\n");
                contents.Append("\r\n");
                contents.Append("pocketwit");
                contents.Append("\r\n");

                if (!string.IsNullOrEmpty(message))
                {
                    contents.Append(header);
                    contents.Append("\r\n");
                    contents.Append("Content-Disposition: form-data;name=\"message\"\r\n");
                    contents.Append("\r\n");
                    contents.Append(message);
                    contents.Append("\r\n");
                }
                
                contents.Append(header);
                contents.Append("\r\n");
                contents.Append(string.Format("Content-Disposition:form-data; name=\"media\";filename=\"image.jpg\"\r\n"));
                contents.Append("Content-Type: image/jpeg\r\n");
                contents.Append("\r\n");
                
                string End = "\r\n" + header + "\r\n";
                byte[] bytes = Encoding.UTF8.GetBytes(contents.ToString());
                byte[] footBytes = Encoding.UTF8.GetBytes(End);
                request.ContentLength = bytes.Length + photo.Length + footBytes.Length ;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Write(photo, 0, photo.Length);
                    requestStream.Write(footBytes, 0, footBytes.Length);
                    requestStream.Flush();
                    requestStream.Close();
                    
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    
                }
            }

            return null;
        }

        public static string SendStoredPic(string userName, string password, string Message, string Path)
        {

            using(System.IO.FileStream f = new FileStream(Path,FileMode.Open, FileAccess.Read))
            {
                byte[] incoming = new byte[f.Length];
                f.Read(incoming, 0, incoming.Length);
                string ret = ExecutePostCommand("http://twitpic.com/api/uploadAndPost", userName, password, incoming, Message);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ret);
                if (doc.SelectSingleNode("rsp").Attributes["status"].Value == "fail")
                {
                    string ErrorText = doc.SelectSingleNode("//err").Attributes["msg"].Value;
                    throw new Exception(ErrorText);
                }
                else
                {
                    string URL = doc.SelectSingleNode("//mediaurl").InnerText;
                    return URL;
                }
                throw new Exception("Error communicating with twitpic.  Please try again.");
            }
        }

        public static string JustUpload(string userName, string password, string Path)
        {
            using (System.IO.FileStream f = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                byte[] incoming = new byte[f.Length];
                f.Read(incoming, 0, incoming.Length);
                string ret = ExecutePostCommand("http://twitpic.com/api/upload", userName, password, incoming, null);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ret);
                if (doc.SelectSingleNode("rsp").Attributes["stat"].Value == "fail")
                {
                    string ErrorText = doc.SelectSingleNode("//err").Attributes["msg"].Value;
                    throw new Exception(ErrorText);
                }
                else
                {
                    string URL = doc.SelectSingleNode("//mediaurl").InnerText;
                    return URL;
                }
                throw new Exception("Error communicating with twitpic.  Please try again.");
            }
        }
    }
}
