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
            ClientSettings.LoadSettings();

            if (string.IsNullOrEmpty(ClientSettings.UserName) | string.IsNullOrEmpty(ClientSettings.Password))
            {
                // SHow Settings page first
                SettingsForm settings = new SettingsForm();
                if (settings.ShowDialog() == DialogResult.Cancel) { return; }
            }

            Application.Run(new TweetList());
        }
    }
}