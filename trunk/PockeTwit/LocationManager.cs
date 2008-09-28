using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    class LocationManager
    {
        public delegate void delLocationReady(string Location);
        public event delLocationReady LocationReady;
        private GPS.GpsPosition position = null;
        private GPS.GpsDeviceState device = null;
        private GPS.Gps gps = null;
        
        public void GetGPS()
        {
            if (ClientSettings.UseGPS)
            {
                StartGPS();
            }
        }

        public void StartGPS()
        {
            gps = new PockeTwit.GPS.Gps();
            gps.DeviceStateChanged += new PockeTwit.GPS.DeviceStateChangedEventHandler(gps_DeviceStateChanged);
            gps.LocationChanged += new PockeTwit.GPS.LocationChangedEventHandler(gps_LocationChanged);

            gps.Open();
        }
        public void StopGPS()
        {
            gps.Close();
            gps.DeviceStateChanged -= new PockeTwit.GPS.DeviceStateChangedEventHandler(gps_DeviceStateChanged);
            gps.LocationChanged -= new PockeTwit.GPS.LocationChangedEventHandler(gps_LocationChanged);
            
        }

        void gps_LocationChanged(object sender, PockeTwit.GPS.LocationChangedEventArgs args)
        {
            if (args.Position == null) { return; }
            try
            {
                if (args.Position.LatitudeValid && args.Position.LongitudeValid)
                {
                    if (!Double.IsNaN(args.Position.Longitude) && !Double.IsNaN(args.Position.Latitude))
                    {
                        position = args.Position;
                        if (LocationReady != null)
                        {
                            LocationReady(position.Latitude.ToString() + "," + position.Longitude.ToString());
                        }
                    }
                }
            }
            catch (DivideByZeroException)
            {
            }
        }

        void gps_DeviceStateChanged(object sender, PockeTwit.GPS.DeviceStateChangedEventArgs args)
        {
            device = args.DeviceState;
        }
    }
}
