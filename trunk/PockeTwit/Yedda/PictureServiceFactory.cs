using System;

using System.Collections.Generic;
using System.Text;
using Yedda;
using System.Collections;

namespace Yedda
{
    class PictureServiceFactory
    {
        #region private properties
        private static volatile PictureServiceFactory _instance;
        private static object syncRoot = new Object();
        
        private ArrayList serviceList;
        
        #endregion

         #region constructor
        /// <summary>
        /// Private constructor for usage in singleton.
        /// </summary>
        private PictureServiceFactory()
        {
            SetupServices();
        }

        /// <summary>
        /// Singleton constructor
        /// </summary>
        /// <returns></returns>
        public static PictureServiceFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new PictureServiceFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region setup

        private void SetupServices()
        {
            serviceList = new ArrayList();
            //Adding services hardcoded, maybe something like reflection could be used?
            serviceList.Add(TwitPic.Instance);
            serviceList.Add(MobyPicture.Instance);

            foreach (IPictureService service in serviceList)
            {
                service.ReadBufferSize = 512;
                service.RootPath = ClientSettings.AppPath;
                service.DefaultFileName = "image1.jpg";
                service.DefaultFilePath = "ArtCache";
                service.UseDefaultFileName = true;
                service.UseDefaultFilePath = true;
            }

        }

        #endregion

        #region public methods

        public IPictureService GetServiceByName(string ServiceName)
        {
            foreach (IPictureService service in serviceList)
            {
                if (service.ServiceName == ServiceName)
                {
                    return service;
                }
            }
            return null;
        }

        public bool FetchServiceAvailable(string URL)
        {
            foreach (IPictureService service in serviceList)
            {
                if (service.CanFetchUrl(URL))
                {
                    return true;
                }
            }
            return false;
        }

        public IPictureService LocateFetchService(string URL)
        {
            foreach (IPictureService service in serviceList)
            {
                if (service.CanFetchUrl(URL))
                {
                    return service;
                }
            }
            return null;
        }

        public ArrayList GetServiceNames()
        {
            ArrayList servicesList = new ArrayList();

            foreach (IPictureService service in serviceList)
            {
                servicesList.Add(service.ServiceName);
            }
            return servicesList;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Set all the event handlers for the chosen picture service.
        /// Aparently after posting, event set are lost so these have to be set again.
        /// </summary>
        /// <param name="pictureService">Picture service on which the event handlers should be set.</param>
        private void SetPictureEventHandlers(IPictureService service)
        {
            if (!service.HasEventHandlersSet)
            {
                service.DownloadFinish += new DownloadFinishEventHandler(service_DownloadFinish);
                service.ErrorOccured += new ErrorOccuredEventHandler(service_ErrorOccured);
            }
        }

        private void service_ErrorOccured(object sender, PictureServiceEventArgs eventArgs)
        {
            
        }

        /// <summary>
        /// Event handling for when the download is finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void service_DownloadFinish(object sender, PictureServiceEventArgs eventArgs)
        {
            

        }

     



        #endregion
    }
}
