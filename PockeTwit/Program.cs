using System;

using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using FingerUI;
using Microsoft.WindowsCE.Forms;

namespace PockeTwit
{
    static class Program
    {
        public static bool IgnoreDisposed = false;

		#region Methods (2) 

		// Private Methods (2) 
       
        [MTAThread]
        static void Main(string[] Args)
        {
            // suppress ObjectDisposedExceptions unless it's a devbuild
            IgnoreDisposed = !UpgradeChecker.devBuild;
            bool bBackGround = false;
            if (Args.Length > 0)
            {
                string Arg =  Args[0];

                if (Arg == "/BackGround")
                {
                    bBackGround = true;
                }

                if (Arg == "/QuickPost")
                {
                    ClientSettings.LoadSettings();
                    if (ClientSettings.AccountsList.Count == 0)
                    {
                        PockeTwit.Localization.LocalizedMessageBox.Show("You must configure PockeTwit before using QuickPost.", "PockeTwit QuickPost");
                        return;
                    }
                    PostUpdate PostForm = new PostUpdate(true);
                    PostForm.AccountToSet = ClientSettings.DefaultAccount;
                    Application.Run(PostForm);
                    PostForm.Close();
                    return;
                }
            }
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            ClientSettings.LoadSettings();
            Application.Run(new TweetList(bBackGround, Args));
            LocalStorage.DataBaseUtility.CleanDB(10);

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            string ErrorPath = ClientSettings.AppPath;
            Exception ex = (Exception)e.ExceptionObject;
            if (ex is ObjectDisposedException && IgnoreDisposed)
            {
                return;
            }
            if (ex is LowMemoryException)
            {
                PockeTwit.Localization.LocalizedMessageBox.Show("You do not currently have enough graphics memory to run PockeTwit.  Please close some applications or soft-reset and try again.", "Low Memory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            // There's a special place in hell for this, but
            // the exception can't be trapped and it doesn't do any harm
            // No point crashing because of it
            // This is caused by the LargeIntervalTimer in OpenNetCF.
            // We can't change the code ourselves. Best thing in the long run
            // would be to implement the functionality ourselves.
            // only suppress in release builds though
            if (!UpgradeChecker.devBuild && ex.Message.Equals("Cannot Cancel Notification Handler"))
            {
                return;
            }
            System.Text.StringBuilder b = new System.Text.StringBuilder();
            b.Append("From v" + UpgradeChecker.currentVersion.ToString());
            if(UpgradeChecker.devBuild)
                b.Append(" dev build " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision);
            b.Append("\r\n");
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
            b.Append("_________________");
            b.Append("\r\n");
            try
            {
                b.Append(string.Format("OEM Device Name: {0}\r\n", DetectDevice.GetOEMName()));
                b.Append(string.Format("OS Version: {0} ({1})\r\n", Environment.OSVersion.ToString(), Microsoft.WindowsCE.Forms.SystemSettings.Platform.ToString()));
                b.Append(string.Format(".NET CLR Version: {0}\r\n", Environment.Version.ToString()));
            }
            catch (Exception)
            {
                b.Append("Exception loading device information.");
            }
            b.Append("\r\n");
            b.Append(ClientSettings.AppPath);
            b.Append("\r\n");
            b.Append("_________________");

            using (System.IO.StreamWriter w = new System.IO.StreamWriter(ErrorPath + "\\crash.txt"))
            {
                w.Write(b.ToString());
            }
            PockeTwit.Localization.LocalizedMessageBox.Show("An unexpected error has occured and PockeTwit must shut down.\n\nYou will have an opportunity to submit a crash report to the developer on the next run.", "PockeTwit");

        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		#endregion Methods 

        }
}
