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
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(ErrorPath + "\\error.txt"))
            {
                Exception ex = (Exception)e.ExceptionObject;
                w.WriteLine(ex.Message);
                w.WriteLine("_________________");
                w.WriteLine(ex.StackTrace);

                if(ex.InnerException!=null)
                {
                    w.WriteLine();
                    w.WriteLine();
                    w.WriteLine("Inner exception:");
                    w.WriteLine(ex.InnerException.Message);
                    w.WriteLine("_______________________");
                    w.WriteLine(ex.InnerException.StackTrace);
                }
            }
            MessageBox.Show("An unexpected error has occured.  If this continues please contact @PockeTwitDev.", "Error");
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