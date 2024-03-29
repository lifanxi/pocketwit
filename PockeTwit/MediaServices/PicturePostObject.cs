﻿using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit.MediaServices
{
    public class PicturePostObject: IDisposable, ICloneable
    {
        #region private properties

        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _filename = string.Empty;
        private string _message = string.Empty;
        private string _lat = string.Empty;
        private string _lon = string.Empty;
        private string _url = string.Empty;
        private string _contentType = string.Empty;
        private byte[] _pictureData = null;
        private System.IO.Stream _pictureStream = null;
        private bool _useAsync = true;

        #endregion

        #region constructor/deconstructor

        public PicturePostObject()
        {

        }

        ~PicturePostObject()
        {
            _pictureData = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _pictureData = null;
        }

        #endregion

        #endregion

        #region getters and setters

        /// <summary>
        /// Stream object to use to read data - will this improve performance?
        /// </summary>
        public System.IO.Stream PictureStream
        {
            get
            {
                if (_pictureStream != null)
                    return _pictureStream;
                else if (_pictureData != null)
                {
                    return new System.IO.MemoryStream(_pictureData);
                }
                else
                    return null;
            }
            set
            {
                _pictureStream = value;
            }
        }

        /// <summary>
        /// Byte data from a picture - may be null
        /// </summary>
        public byte[] PictureData
        {
            get
            {
                return _pictureData;
            }
            set
            {
                _pictureData = value;
            }
        }

        /// <summary>
        /// Get or set the User name
        /// </summary>
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _username = string.Empty;
                }
                else
                {
                    _username = value;
                }
            }
        }

        /// <summary>
        /// Get or set the Password
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _password = string.Empty;
                }
                else
                {
                    _password = value;
                }
            }
        }

        /// <summary>
        /// Get or set the Filename
        /// </summary>
        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _filename = string.Empty;
                }
                else
                {
                    _filename = value;
                }
            }
        }

        /// <summary>
        /// Get or set the Message
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _message = string.Empty;
                }
                else
                {
                    _message = value;
                }
            }
        }

        /// <summary>
        /// Do or do not use async. 
        /// </summary>
        public bool UseAsync
        {
            get
            {
                return _useAsync;
            }
            set
            {
                _useAsync = value;
            }
        }

        public string Lat
        {
            get
            {
                return _lat;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _lat = string.Empty;
                }
                else
                {
                    _lat = value;
                }
            }
        }

        public string Lon
        {
            get
            {
                return _lon;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _lon = string.Empty;
                }
                else
                {
                    _lon = value;
                }
            }
        }

        public string URL
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            //shallow clone, without the object data.
            //As long as no other objects are cloned no need to cast it to a PicturePostObject
            object clone = this.MemberwiseClone();

            //space for cloning objects.

            return clone;
        }

        #endregion
    }
}
