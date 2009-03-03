﻿using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Yedda
{
    public class MobyPicture : IPictureService
    {
        private static volatile MobyPicture _instance;
        private static object syncRoot = new Object();

        private const string DEVELOPER_KEY = "";
        private const string API_URL = "http://api.mobypicture.com";

        private const string API_UPLOAD = "http://api.mobypicture.com/postMediaUrl";
        private const string API_UPLOAD_POST = "http://api.mobypicture.com/postMedia";
        private const string API_GET_THUMB = "http://api.mobypicture.com/getThumb";  //The extra / for directly sticking the image-id on.
        private const string API_SAVE_TO_PATH = "\\ArtCache\\www.mobypicture.com\\";
        private const string PT_DEFAULT_FILENAME = "image1.jpg";
        private const int PT_READ_BUFFER_SIZE = 512;
        private const bool PT_USE_DEFAULT_FILENAME = true;

        /// <summary>
        /// Private constructor for usage in singleton.
        /// </summary>
        private MobyPicture()
        {
        }

        /// <summary>
        /// Singleton constructor
        /// </summary>
        /// <returns></returns>
        public static MobyPicture Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new MobyPicture();
                        }
                    }
                }
                return _instance;
            }
        }
        
        #region IPictureService Members

        public string PostPicture(PicturePostObject postData)
        {
            #region Argument check

            //Check for empty path
            if (string.IsNullOrEmpty(postData.Filename))
            {
                return string.Empty;
            }

            //Check for empty credentials
            if (string.IsNullOrEmpty(postData.Username) ||
                string.IsNullOrEmpty(postData.Password))
            {
                return string.Empty;
            }

            #endregion

            using (System.IO.FileStream file = new FileStream(postData.Filename, FileMode.Open, FileAccess.Read))
            {
                byte[] incoming = new byte[file.Length];
                file.Read(incoming, 0, incoming.Length);
                postData.PictureData = incoming;

                XmlDocument uploadResult = UploadPicture(API_UPLOAD, postData);

                if (uploadResult.SelectSingleNode("rsp").Attributes["stat"].Value == "fail")
                {
                    string ErrorText = uploadResult.SelectSingleNode("//err").Attributes["msg"].Value;
                    throw new Exception(ErrorText);
                }
                else
                {
                    string URL = uploadResult.SelectSingleNode("//mediaurl").InnerText;
                    return URL;
                }
            }
        }

        public string FetchPicture(string pictureURL)
        {
            #region Argument check

            //Need a url to read from.
            if (string.IsNullOrEmpty(pictureURL))
            {
                return string.Empty;
            }

            #endregion

            string resultURL = string.Empty;

            resultURL = RetrievePicture(pictureURL);

            return resultURL;
        }

        #endregion

        private XmlDocument UploadPicture(string url, PicturePostObject ppo)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            string boundary = System.Guid.NewGuid().ToString();
            request.Credentials = new NetworkCredential(ppo.Username, ppo.Password);
            request.Headers.Add("Accept-Language", "cs,en-us;q=0.7,en;q=0.3");
            request.PreAuthenticate = true;
            request.ContentType = string.Format("multipart/form-data;boundary={0}", boundary);


            //request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Timeout = 20000;
            string header = string.Format("--{0}", boundary);
            string ender = "\r\n" + header + "\r\n";

            StringBuilder contents = new StringBuilder();

            contents.Append(CreateContentPartString(header, "u", ppo.Username));
            contents.Append(CreateContentPartString(header, "p", ppo.Password));
            contents.Append(CreateContentPartString(header, "k", DEVELOPER_KEY));
            //Don't send the picture to twitter just yet.
            contents.Append(CreateContentPartString(header, "s", "none"));

            if (!string.IsNullOrEmpty(ppo.Message))
            {
                contents.Append(CreateContentPartString(header, "message", ppo.Message));
            }

            contents.Append(CreateContentPartMedia(header));

            //Create the form message to send in bytes

            byte[] message = Encoding.UTF8.GetBytes(contents.ToString());
            byte[] footer = Encoding.UTF8.GetBytes(ender);
            request.ContentLength = message.Length + ppo.PictureData.Length + footer.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(message, 0, message.Length);
                requestStream.Write(ppo.PictureData, 0, ppo.PictureData.Length);
                requestStream.Write(footer, 0, footer.Length);
                requestStream.Flush();
                requestStream.Close();

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        XmlDocument responseXML = new XmlDocument();
                        responseXML.LoadXml(reader.ReadToEnd());
                        return responseXML;
                    }
                }

            }
            return null;
        }

        /// <summary>
        /// Use a imageId to retrieve and save a thumbnail to the device.
        /// </summary>
        /// <param name="imageId">Id for the image</param>
        /// <returns></returns>
        private string RetrievePicture(string pictureURL)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(API_GET_THUMB);
            myRequest.Method = "GET";
            String pictureFileName = String.Empty;

            using (HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    int totalSize = 0;
                    byte[] readBuffer = new byte[PT_READ_BUFFER_SIZE];
                    pictureFileName = GetPicturePath(pictureURL);

                    int responseSize = dataStream.Read(readBuffer, 0, PT_READ_BUFFER_SIZE);
                    while (responseSize > 0)
                    {
                        SavePicture(pictureFileName, readBuffer, responseSize);
                        try
                        {
                            totalSize += responseSize;
                            responseSize = dataStream.Read(readBuffer, 0, PT_READ_BUFFER_SIZE);
                        }
                        catch
                        {
                            responseSize = 0;
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                    dataStream.Close();
                }
                response.Close();
            }

            return pictureFileName;
        }

        /// <summary>
        /// Lookup the path and filename intended for the image. When it does not exist, create it.
        /// </summary>
        /// <param name="imageId">Image ID</param>
        /// <returns>Path to save the picture in.</returns>
        private static string GetPicturePath(string pictureURL)
        {
            #region argument check

            if (string.IsNullOrEmpty(pictureURL))
            {
                return ClientSettings.AppPath + API_SAVE_TO_PATH + "\\" + PT_DEFAULT_FILENAME;
            }

            #endregion

            String picturePath = String.Empty;

            int imageIdStartIndex = pictureURL.LastIndexOf('?') + 1;
            string imageId = pictureURL.Substring(imageIdStartIndex, pictureURL.Length - imageIdStartIndex);

            if (PT_USE_DEFAULT_FILENAME)
            {
                picturePath = ClientSettings.AppPath + API_SAVE_TO_PATH;
                if (!Directory.Exists(picturePath))
                {
                    Directory.CreateDirectory(picturePath);
                }
                picturePath = picturePath + PT_DEFAULT_FILENAME;
                if (File.Exists(picturePath))
                {
                    File.Delete(picturePath);
                }
            }
            else
            {
                string firstChar = imageId.Substring(0, 1);
                picturePath = ClientSettings.AppPath + API_SAVE_TO_PATH + firstChar + "\\";

                if (!System.IO.Directory.Exists(picturePath))
                {
                    System.IO.Directory.CreateDirectory(picturePath);
                }

                picturePath = picturePath + imageId + ".jpg";
            }
            return picturePath;
        }

        /// <summary>
        /// Save the picture data to disk.
        /// </summary>
        /// <param name="picturePath"></param>
        /// <param name="pictureData"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        private static bool SavePicture(String picturePath, byte[] pictureData, int bufferSize)
        {
            #region argument check
            if (String.IsNullOrEmpty(picturePath))
            {
                return false;
            }

            if (pictureData == null)
            {
                return false;
            }
            if (pictureData.Length == 0)
            {
                return false;
            }
            #endregion

            if (!File.Exists(picturePath))
            {
                using (FileStream pictureFile = File.Create(picturePath))
                {
                    pictureFile.Write(pictureData, 0, bufferSize);
                    pictureFile.Close();
                }
            }
            else
            {
                using (FileStream pictureFile = File.Open(picturePath, FileMode.Append, FileAccess.Write))
                {
                    pictureFile.Write(pictureData, 0, bufferSize);
                    pictureFile.Close();
                }
            }
            return true; ;
        }


        private string CreateContentPartString(string header, string dispositionName, string valueToSend)
        {
            StringBuilder contents = new StringBuilder();

            contents.Append(header);
            contents.Append("\r\n");
            contents.Append(String.Format("Content-Disposition: form-data;name=\"{0}\"\r\n", dispositionName));
            contents.Append("\r\n");
            contents.Append(valueToSend);
            contents.Append("\r\n");

            return contents.ToString();
        }

        private string CreateContentPartMedia(string header)
        {
            StringBuilder contents = new StringBuilder();

            contents.Append(header);
            contents.Append("\r\n");
            contents.Append(string.Format("Content-Disposition:form-data; name=\"i\";filename=\"image.jpg\"\r\n"));
            contents.Append("Content-Type: image/jpeg\r\n");
            contents.Append("\r\n");

            return contents.ToString();
        }
    }
}
