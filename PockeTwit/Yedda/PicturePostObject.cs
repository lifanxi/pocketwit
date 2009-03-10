using System;

using System.Collections.Generic;
using System.Text;

namespace Yedda
{
    public class PicturePostObject: IDisposable, ICloneable
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        //private GPSObject _GPSObject = null;
        private string _filename = string.Empty;
        private string _message = string.Empty;
        private byte[] _pictureData = null;

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

        /// <summary>
        /// Byte data from a picture
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
        /// Get or set the GPS data
        /// </summary>
        //public GPSObject GPS
        //{
        //    get
        //    {
        //        return _GPSObject;
        //    }
        //    set
        //    {
        //        _GPSObject = value;
        //    }
        //}

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
