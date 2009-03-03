using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Yedda
{
    /// <summary>
    /// Interface for multiple picture services
    /// </summary>
    public interface IPictureService
    {
        /// <summary>
        /// Send a picture to a twitter picture framework
        /// </summary>
        /// <param name="postData">Postdata</param>
        /// <returns>Returned URL from server</returns>
        string PostPicture(PicturePostObject postData);

        /// <summary>
        /// Retrieve a picture from a picture service. 
        /// </summary>
        /// <param name="pictureURL">pictureURL</param>
        /// <returns>Local path for downloaded picture.</returns>
        string FetchPicture(string pictureURL);
        
    }
}