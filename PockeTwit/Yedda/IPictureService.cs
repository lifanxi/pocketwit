using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Yedda
{
    public class PictureServiceEventArgs : EventArgs
    {
        private PictureServiceErrorLevel _pictureServiceErrorLevel = PictureServiceErrorLevel.OK; 
        private string _returnMessage = string.Empty;
        private string _errorMessage = string.Empty;
        private string _pictureFileName = string.Empty;
        
        // Constructor
        public PictureServiceEventArgs(PictureServiceErrorLevel pictureServiceErrorLevel, string returnMessage, string errorMessage)
        {
            _pictureServiceErrorLevel = pictureServiceErrorLevel;
            _returnMessage = returnMessage;
            _errorMessage = errorMessage;
        }
        public PictureServiceEventArgs(PictureServiceErrorLevel pictureServiceErrorLevel, string returnMessage, string errorMessage, string pictureFileName)
        {
            _pictureServiceErrorLevel = pictureServiceErrorLevel;
            _returnMessage = returnMessage;
            _errorMessage = errorMessage;
            _pictureFileName = pictureFileName;
        }

        public PictureServiceErrorLevel ErrorLevel
        {
            get { return _pictureServiceErrorLevel; }
        }
        public string ReturnMessage
        {
            get { return _returnMessage; }
        }
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }
        public string PictureFileName
        {
            get { return _pictureFileName; }
        }
    }

    public enum PictureServiceErrorLevel
    {
        OK = 0,
        NotReady = 10,
        UnAvailable = 20,
        Failed = 99

        //Allways room te expand.
    }

    public delegate void UploadFinishEventHandler(object sender, PictureServiceEventArgs eventArgs);
    public delegate void DownloadFinishEventHandler(object sender, PictureServiceEventArgs eventArgs);
    public delegate void ErrorOccuredEventHandler(object sender, PictureServiceEventArgs eventArgs);
    public delegate void MessageReadyEventHandler(object sender, PictureServiceEventArgs eventArgs);


    /// <summary>
    /// Interface for multiple picture services
    /// </summary>
    public interface IPictureService
    {
        event UploadFinishEventHandler UploadFinish;
        event DownloadFinishEventHandler DownloadFinish;
        event ErrorOccuredEventHandler ErrorOccured;
        event MessageReadyEventHandler MessageReady;

        /// <summary>
        /// Send a picture to a twitter picture framework
        /// </summary>
        /// <param name="postData">Postdata</param>
        /// <returns>Returned URL from server</returns>
        void PostPicture(PicturePostObject postData);

        /// <summary>
        /// Retrieve a picture from a picture service. 
        /// </summary>
        /// <param name="pictureURL">pictureURL</param>
        /// <returns>Local path for downloaded picture.</returns>
        void FetchPicture(string pictureURL);

        bool HasEventHandlersSet { get; set; }


    }
}