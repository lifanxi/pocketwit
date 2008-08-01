using System;

using System.Collections.Generic;
using System.Windows.Forms;

namespace PockeTwit
{
    static class Program
    {
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

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is System.Net.WebException)
            {
                System.Net.WebException ex = (System.Net.WebException)e.ExceptionObject;
                MessageBox.Show("Unable to connect to twitter.\r\nEither twitter is down or the network connection has been broken.", "Error");
                Application.Exit();
            }
        }

        
        

    }
}