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
        public event delNotificationClicked FriendsNotificationClicked;
        public event delNotificationClicked MessagesNotificationClicked;
        private Timer t_StopVibrate = new Timer();
        private christec.windowsce.forms.NotificationWithSoftKeys FriendsBubbler;
        private christec.windowsce.forms.NotificationWithSoftKeys MessagesBubbler;
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
                FriendsBubbler = new christec.windowsce.forms.NotificationWithSoftKeys();
                FriendsBubbler.LeftSoftKey = new christec.windowsce.forms.NotificationSoftKey(christec.windowsce.forms.SoftKeyType.Dismiss, "Dismiss");
                FriendsBubbler.RightSoftKey = new christec.windowsce.forms.NotificationSoftKey(christec.windowsce.forms.SoftKeyType.StayOpen, "Show");
                FriendsBubbler.RightSoftKeyClick += new EventHandler(FriendsBubbler_RightSoftKeyClick);
                FriendsBubbler.Silent = true;

                MessagesBubbler = new christec.windowsce.forms.NotificationWithSoftKeys();
                MessagesBubbler.LeftSoftKey = new christec.windowsce.forms.NotificationSoftKey(christec.windowsce.forms.SoftKeyType.Dismiss, "Dismiss");
                MessagesBubbler.RightSoftKey = new christec.windowsce.forms.NotificationSoftKey(christec.windowsce.forms.SoftKeyType.StayOpen, "Show");
                MessagesBubbler.RightSoftKeyClick += new EventHandler(MessagesBubbler_RightSoftKeyClick);
                MessagesBubbler.Silent = true;

            }
            t_StopVibrate.Tick += new EventHandler(t_StopVibrate_Tick);
        }

        void MessagesBubbler_RightSoftKeyClick(object sender, EventArgs e)
        {
            MessagesBubbler.Visible = false;
            if (MessagesNotificationClicked != null)
            {
                MessagesNotificationClicked();
            }
        }

        void FriendsBubbler_RightSoftKeyClick(object sender, EventArgs e)
        {
            MessagesBubbler.Visible = false;
            if (FriendsNotificationClicked != null)
            {
                FriendsNotificationClicked();
            }
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
            LoadSettings();
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
                HTMLString.Append("</body></html>");
                FriendsBubbler.Text = HTMLString.ToString();
                FriendsBubbler.Caption = "PockeTwit";
                FriendsBubbler.Visible = true;
            }
        }

        public void NewMessages(int Count)
        {
            LoadSettings();
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
                HTMLString.Append("</body></html>");
                MessagesBubbler.Text = HTMLString.ToString();
                MessagesBubbler.Caption = "PockeTwit";
                MessagesBubbler.Visible = true;
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
