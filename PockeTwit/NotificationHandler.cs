using System;
using Microsoft.Win32;
using Microsoft.WindowsCE.Forms;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    class NotificationHandler
    {
        public delegate void delNotificationClicked();
        public event delNotificationClicked NotificationClicked;
        private Timer t_StopVibrate = new Timer();
        private Notification Bubbler;
        [Flags]
        private enum Options
        {
            Sound = 1,
            Vibrate = 2,
            Flash = 4,
            Message = 8
        }
        private struct NotificationInfo
        {
            public Options Options;
            public string Sound;
        }

        public const string FriendsTweets = "{DF293090-5095-49ce-A626-AE6D6629437F}";
        public const string MessageTweets = "{B4D35E62-A83F-4add-B421-F7FC28E14310}";

        private NotificationInfo Friends = new NotificationInfo();
        private NotificationInfo Messages = new NotificationInfo();

        public NotificationHandler()
        {
            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                Bubbler = new Notification();
                Bubbler.ResponseSubmitted += new ResponseSubmittedEventHandler(Bubbler_ResponseSubmitted);
            }
            t_StopVibrate.Tick += new EventHandler(t_StopVibrate_Tick);
        }

        void t_StopVibrate_Tick(object sender, EventArgs e)
        {
            VibrateStop();
        }

        public void LoadSettings()
        {
            RegistryKey FriendsKey = Registry.CurrentUser.OpenSubKey("\\ControlPanel\\Notifications\\" + FriendsTweets);
            RegistryKey MessageKey = Registry.CurrentUser.OpenSubKey("\\ControlPanel\\Notifications\\" + MessageTweets);
            if (FriendsKey == null)
            {
                return;
            }
            Friends.Sound = (string)FriendsKey.GetValue("Wave");
            Messages.Sound = (string)MessageKey.GetValue("Wave");
            Friends.Options = (Options)FriendsKey.GetValue("Options");
            Messages.Options = (Options)MessageKey.GetValue("Options");
        }

        public void NewFriendMessages(int Count)
        {
            if ((Friends.Options & Options.Sound) == Options.Sound)
            {
                Sound s = new Sound(Friends.Sound);
                s.Play();
            }
            if ((Friends.Options & Options.Vibrate) == Options.Vibrate)
            {
                t_StopVibrate.Interval = 1000;
                t_StopVibrate.Enabled = true;
                VibrateStart();
            }
            if ((Friends.Options & Options.Message) == Options.Message)
            {
                System.Text.StringBuilder HTMLString = new StringBuilder();
                HTMLString.Append("<html><body>");
                HTMLString.Append(Count.ToString() + " new friend updates are available!");
                HTMLString.Append("<form method=\'GET\' action=notify>");
                HTMLString.Append("<p align=\"right\">");
                HTMLString.Append("<input type=button name='Show' value='Show'>");
                HTMLString.Append("</p>");
                HTMLString.Append("</form>");
                HTMLString.Append("</body></html>");
                Bubbler.Text = HTMLString.ToString();

                Bubbler.Caption = "PockeTwit";
                Bubbler.Visible = true;

            }
        }

        public void NewMessages(int Count)
        {
            if ((Messages.Options & Options.Sound) == Options.Sound)
            {
                Sound s = new Sound(Messages.Sound);
                s.Play();
            }
            if ((Messages.Options & Options.Vibrate) == Options.Vibrate)
            {
                t_StopVibrate.Interval = 1000;
                t_StopVibrate.Enabled = true;
                VibrateStart();
            }
            if ((Messages.Options & Options.Message) == Options.Message)
            {
                System.Text.StringBuilder HTMLString = new StringBuilder();
                HTMLString.Append("<html><body>");
                HTMLString.Append(Count.ToString() + " new messages are available!");
                HTMLString.Append("<form method=\'GET\' action=notify>");
                HTMLString.Append("<p align=\"right\">");
                HTMLString.Append("<input type=button name='Show' value='Show'>");
                HTMLString.Append("</p>");
                HTMLString.Append("</form>");
                HTMLString.Append("</body></html>");
                Bubbler.Text = HTMLString.ToString();
                
                Bubbler.Caption = "PockeTwit";
                Bubbler.Visible = true;
                
            }
        }

        void Bubbler_ResponseSubmitted(object sender, ResponseSubmittedEventArgs e)
        {
            Bubbler.Visible = false;
            if (NotificationClicked != null)
            {
                NotificationClicked();
            }
        }

        #region VibrateCode
        [System.Runtime.InteropServices.DllImport("aygshell.dll")]
        private static extern int Vibrate(int cvn, IntPtr rgvn, bool fRepeat, uint dwTimeout);

        [System.Runtime.InteropServices.DllImport("aygshell.dll")]
        public static extern int VibrateStop();

        const uint INFINITE = (uint)0xffffffffL;

        void VibrateStart() 
        {
            Vibrate(0, IntPtr.Zero, true, INFINITE);
        }
        #endregion
    }
}
