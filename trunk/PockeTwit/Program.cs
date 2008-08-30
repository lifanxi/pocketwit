using System;

using System.Collections.Generic;
using System.Windows.Forms;

namespace PockeTwit
{
    static class Program
    {

		#region Methods (2) 

        
		// Private Methods (2) 

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string ErrorPath = ClientSettings.AppPath;
            Exception ex = (Exception)e.ExceptionObject;

            System.Text.StringBuilder b = new System.Text.StringBuilder();
            b.Append("From v" + UpdateChecker.currentVersion.ToString());
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
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(ErrorPath + "\\error.txt"))
            {
                w.Write(b.ToString());
            }

            ChooseAccount form = new ChooseAccount(b.ToString());
            form.ShowDialog();

            DialogResult res;
            
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