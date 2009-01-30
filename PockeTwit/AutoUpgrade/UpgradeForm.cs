using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Net;
using System.Web;
using System.Windows.Forms;

namespace PockeTwit
{
    public partial class UpgradeForm : Form
    {

		#region Fields (1) 

        private UpgradeChecker.UpgradeInfo _NewVersion;
        private HttpWebRequest request;
        private HttpWebResponse response;
        private FileStream filestream;

        private int pbVal, maxVal;

        // Data buffer for stream operations
        private byte[] dataBuffer;
        private const int DataBlockSize = 65536;
		#endregion Fields 

		#region Constructors (1) 

        public UpgradeForm()
        {
            InitializeComponent();
            PockeTwit.Themes.FormColors.SetColors(this);
            if (ClientSettings.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

		#endregion Constructors 

		#region Properties (1) 

        public UpgradeChecker.UpgradeInfo NewVersion 
        {
            set
            {
                lblVersion.Text = value.webVersion.ToString();
                lblInfo.Text = value.UpgradeNotes;
                _NewVersion = value;
            }
        }

		#endregion Properties 

		#region Methods (2) 


		// Private Methods (2) 

        private void menuIgnore_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            NotificationHandler n = new NotificationHandler();
            n.BackupToDisk();
            n.ShutDown();

            System.IO.Directory.CreateDirectory(ClientSettings.AppPath + "\\Update");
            request = (HttpWebRequest)HttpWebRequest.Create(_NewVersion.DownloadURL);
            request.BeginGetResponse(new AsyncCallback(ResponseReceived), null);
            menuUpdate.Enabled = false;
            
            menuIgnore.Enabled = false;
            
            lblDownloading.Visible = true;
            progressDownload.Visible = true;
            lblInfo.Visible = false;
            lblVersion.Visible = false;
            label1.Visible = false;
            label2.Visible = false;

            Cursor.Current = Cursors.WaitCursor;
         
            
            
            /*
            System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
            pi.FileName = _NewVersion.DownloadURL;
            pi.UseShellExecute = true;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
            Application.Exit();
             */
        }

        public void SetProgressMax(object sender, EventArgs e)
        {
            progressDownload.Maximum = maxVal;
            Application.DoEvents();
        }
        public void UpdateProgressValue(object sender, EventArgs e)
        {
            progressDownload.Value = pbVal;
            Application.DoEvents();
        }

        void ResponseReceived(IAsyncResult res)
        {
            try
            {
                response = (HttpWebResponse)request.EndGetResponse(res);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
                return;
            }
            // Allocate data buffer
            dataBuffer = new byte[DataBlockSize];
            // Set up progrees bar
            maxVal = (int)response.ContentLength;
            progressDownload.Invoke(new EventHandler(SetProgressMax));
            
            // Open file stream to save received data
            filestream = new FileStream(ClientSettings.AppPath + "\\Update\\update.cab", FileMode.Create);
            // Request the first chunk
            response.GetResponseStream().BeginRead(dataBuffer, 0, DataBlockSize, new AsyncCallback(OnDataRead), this);

        }

        void OnDataRead(IAsyncResult res)
        {
            // How many bytes did we get this time
            int nBytes = response.GetResponseStream().EndRead(res);
            // Write buffer
            filestream.Write(dataBuffer, 0, nBytes);
            // Update progress bar using Invoke()
            pbVal += nBytes;
            
            progressDownload.Invoke(new EventHandler(UpdateProgressValue));
            
            // Are we done yet?
            if (nBytes > 0)
            {
                // No, keep reading
                response.GetResponseStream().BeginRead(dataBuffer, 0, DataBlockSize, new AsyncCallback(OnDataRead), this);
            }
            else
            {
                // Yes, perform cleanup and update UI.
                filestream.Close();
                filestream = null;
                DoneDownloading();
            }
        }

        void DoneDownloading()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = ClientSettings.AppPath + "\\Update\\update.cab";
            p.StartInfo.UseShellExecute = true;
            p.Start();
            Application.Exit();
        }

		#endregion Methods 

    }
}