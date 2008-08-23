using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class SearchForm : Form
    {
        private delegate void delEnableDistance();

        public GPS.GpsPosition position = null;
        private GPS.GpsDeviceState device = null;
        private GPS.Gps gps = null;
        

		#region Constructors (1) 

        public SearchForm()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;

            if (ClientSettings.UseGPS)
            {
                gps = new PockeTwit.GPS.Gps();
                gps.DeviceStateChanged += new PockeTwit.GPS.DeviceStateChangedEventHandler(gps_DeviceStateChanged);
                gps.LocationChanged += new PockeTwit.GPS.LocationChangedEventHandler(gps_LocationChanged);
                gps.Open();
            }
        }

        private void EnableDistance()
        {
            if (InvokeRequired)
            {
                delEnableDistance d = new delEnableDistance(EnableDistance);
                this.Invoke(d, null);
            }
            else
            {
                lblWithin.Visible = true;
                cmbDistance.Visible = true;
                cmbMeasurement.Visible = true;
                cmbDistance.Enabled = true;
                cmbMeasurement.Enabled = true;
            }
        }
        void gps_LocationChanged(object sender, PockeTwit.GPS.LocationChangedEventArgs args)
        {
            if (args.Position == null) { return; }
            try
            {
                if (args.Position.LatitudeValid && args.Position.LongitudeValid)
                {
                    position = args.Position;
                    EnableDistance();
                }
            }
            catch (DivideByZeroException ex)
            {
            }
        }

        void gps_DeviceStateChanged(object sender, PockeTwit.GPS.DeviceStateChangedEventArgs args)
        {
            this.device = args.DeviceState;
        }

		#endregion Constructors 

		#region Properties (1) 

        public string SearchText{get;set;}

		#endregion Properties 

		#region Methods (2) 


		// Private Methods (2) 

        private void menuCancel_Click(object sender, EventArgs e)
        {
            if (ClientSettings.UseGPS)
            {
                gps.Close();
            }
            this.DialogResult = DialogResult.Cancel;

        }

        private void menuSearch_Click(object sender, EventArgs e)
        {
            StringBuilder b = new StringBuilder();
            if(!string.IsNullOrEmpty(txtSearch.Text))
            {
                b.Append("q=");
                b.Append(txtSearch.Text);
                if(!string.IsNullOrEmpty((string)cmbDistance.SelectedValue))
                {
                    b.Append("&");
                }
            }
            if (!string.IsNullOrEmpty(cmbDistance.Text))
            {
                
                b.Append("geocode=" + position.Latitude.ToString() + "," + position.Longitude.ToString());
                b.Append("," + cmbDistance.Text);
                switch(cmbMeasurement.Text)
                {
                    case "Miles":
                        b.Append("mi");
                        break;
                    case "Kilometers":
                        b.Append("km");
                        break;
                }
                
            }
            this.SearchText = b.ToString();
            if (ClientSettings.UseGPS)
            {
                gps.Close();
            }
            this.DialogResult = DialogResult.OK;
        }


		#endregion Methods 

    }
}