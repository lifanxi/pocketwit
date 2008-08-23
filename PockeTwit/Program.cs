using System;

using System.Collections.Generic;
using System.Windows.Forms;

namespace PockeTwit
{
    static class Program
    {

		#region Methods (2) 

        static void SendError(string e)
        {
            Microsoft.WindowsMobile.PocketOutlook.EmailMessage m = new Microsoft.WindowsMobile.PocketOutlook.EmailMessage();
            m.BodyText = e;
            m.To.Add(new Microsoft.WindowsMobile.PocketOutlook.Recipient("pocketwitdev@gmail.com"));
            m.Subject = "Crash Report";
            Microsoft.WindowsMobile.PocketOutlook.OutlookSession s = new Microsoft.WindowsMobile.PocketOutlook.OutlookSession();
            s.EmailAccounts[0].Send(m);
        }
		// Private Methods (2) 

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string ErrorPath = ClientSettings.AppPath;
            Exception ex = (Exception)e.ExceptionObject;

            System.Text.StringBuilder b = new System.Text.StringBuilder();
            b.Append(ex.Message);
            b.Append("\r\n");
            b.Append("_________________");
            b.Append("\r\n");
            b.Append(ex.StackTrace);
            b.Append("\r\n");
            if (ex.InnerException != null)
            {
                b.Append("\r\n");
                b.Append("\r\n");
                b.Append("Inner exception:");
                b.Append("\r\n");
                b.Append(ex.InnerException.Message);
                b.Append("\r\n");
                b.Append("_______________________");
                b.Append("\r\n");
                b.Append(ex.InnerException.StackTrace);
                b.Append("\r\n");
            }
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(ErrorPath + "\\error.txt"))
            {
                w.Write(b.ToString());
            }
            DialogResult res;
            res = MessageBox.Show("An unexpected error has occured.  May I send an email containing the crash report to the developer?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            if(res == DialogResult.Yes)
            {
                SendError(b.ToString());
            }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            
            
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            ClientSettings.LoadSettings();
            Application.Run(new TweetList());
        }


		#endregion Methods 

        }
}