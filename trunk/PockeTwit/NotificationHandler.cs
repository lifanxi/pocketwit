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
        private int CurrentSpinner = 0;
        public delegate void delNotificationClicked();
        public event delNotificationClicked MessagesNotificationClicked;
        private christec.windowsce.forms.NotificationWithSoftKeys MessagesBubbler;
        [Flags]
        public enum Options
        {
            Sound = 1,
            Vibrate = 2,
            Flash = 4,
            Message = 8
        }
        public class NotificationInfoClass
        {
            public string Name;
            public string GUID;
            public Options Options;
            public string Sound;
            public TimelineManagement.TimeLineType Type;
            public SpecialTimeLine Group;

            public string  ListName
            {
                get
                {
                    if(Group!=null)
                    {
                        return Group.ListName;
                    }
                    if(Type==TimelineManagement.TimeLineType.Friends)
                    {
                        return "Friends_TimeLine";
                    }
                    return "Messages_TimeLine";
                }
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private static Dictionary<string,NotificationInfoClass> Notifications;

       
        public const string FriendsTweets = "{DF293090-5095-49ce-A626-AE6D6629437F}";
        public const string MessageTweets = "{B4D35E62-A83F-4add-B421-F7FC28E14310}";

        
        private static Microsoft.WindowsMobile.Status.RegistryState FriendsRegistryWatcher1;
        
        public NotificationHandler()
        {
            if (DetectDevice.DeviceType == DeviceType.Professional)
            {
                MessagesBubbler = new christec.windowsce.forms.NotificationWithSoftKeys();
                MessagesBubbler.Icon = Properties.Resources.MyIco;
                MessagesBubbler.Caption = "PockeTwit";
                MessagesBubbler.RightSoftKey = new christec.windowsce.forms.NotificationSoftKey(christec.windowsce.forms.SoftKeyType.Dismiss, "Dismiss");
                MessagesBubbler.LeftSoftKey = new christec.windowsce.forms.NotificationSoftKey(christec.windowsce.forms.SoftKeyType.StayOpen, "Show");
                MessagesBubbler.LeftSoftKeyClick += new EventHandler(MessagesBubbler_LeftSoftKeyClick);
                MessagesBubbler.Silent = true;   
            }
            LoadAll();
        }

        public static NotificationInfoClass[] GetList()
        {
            var ret = new List<NotificationInfoClass>();
            foreach (var notification in Notifications.Values)
            {
                ret.Add(notification);
            }
            return ret.ToArray();
        }
        public static NotificationInfoClass GetItem(string GUID)
        {
            return Notifications[GUID];
        }

        private void LoadAll()
        {
            Notifications = new Dictionary<string, NotificationInfoClass>();
            NotificationInfoClass Friends = new NotificationInfoClass {Name = "PockeTwit: Friends Update", Group = null, GUID = FriendsTweets, Type= TimelineManagement.TimeLineType.Friends};
            NotificationInfoClass Messages = new NotificationInfoClass{Name = "PockeTwit: Messages", Group = null, GUID = MessageTweets, Type=TimelineManagement.TimeLineType.Messages};

            Notifications.Add(FriendsTweets, Friends);
            Notifications.Add(MessageTweets, Messages);

            foreach (var line in SpecialTimeLines.GetList())
            {
                NotificationInfoClass c = new NotificationInfoClass
                {
                    Name = "PockeTwit: " + line.name,
                    Group = line,
                    GUID = line.name,
                    Type = TimelineManagement.TimeLineType.Friends
                };
                Notifications.Add(c.GUID, c);
            }

            foreach (var info in Notifications.Values)
            {
                RegistryKey infoKey = Registry.CurrentUser.CreateSubKey("\\ControlPanel\\Notifications\\" + info.GUID);
                LoadSettings(info, infoKey);
            }
        }

        public void ShutDown()
        {
            DismissBubbler();
        }


        void RegistryWatcher_Changed(object sender, Microsoft.WindowsMobile.Status.ChangeEventArgs args)
        {
            LoadAll();
        }

        

        void MessagesBubbler_LeftSoftKeyClick(object sender, EventArgs e)
        {
            DismissBubbler();
            if (MessagesNotificationClicked != null)
            {
                MessagesNotificationClicked();
            }
        }

        public void DismissBubbler()
        {
            MessagesBubbler.Visible = false;
        }



        public static void LoadSettings(NotificationInfoClass infoClass, RegistryKey key)
        {
            try
            {
                infoClass.Sound = (string)key.GetValue("Wave");
                if (infoClass.Sound == null)
                {
                    key.SetValue("Wave", "");
                }
                if (key.GetValue("Options") != null)
                {
                    infoClass.Options = (Options)key.GetValue("Options");
                }
                else
                {
                    key.SetValue("Options", 0);
                }

                
            }
            catch (NullReferenceException)
            {
            }
        }

        public static void SaveSettings(NotificationInfoClass InfoSet)
        {
            RegistryKey TheKey = Registry.CurrentUser.OpenSubKey("\\ControlPanel\\Notifications\\" + InfoSet.GUID, true);
            if (TheKey == null)
            {
                TheKey = Registry.CurrentUser.CreateSubKey("\\ControlPanel\\Notifications\\" + InfoSet.GUID);
            }
            if (InfoSet.Sound != null)
            {
                TheKey.SetValue("Wave", InfoSet.Sound);
            }
            
            TheKey.SetValue("Options", (int)InfoSet.Options);
        }

        private void ShowNotifications()
        {
            if (MessagesBubbler == null) { return; }
            if (GlobalEventHandler.isInForeground()) { return; }
            MessagesBubbler.Visible = false;
            
            if (!MessagesBubbler.Visible)
            {
                MessagesBubbler.Text = GetMessagesText();
                CurrentSpinner = 0;
                MessagesBubbler.Visible = true;
            }
        }

        public void NewItems()
        {
            foreach (var infoClass in Notifications.Values)
            {
                if(TimeLines.LastSelectedItems.GetUnreadItems(infoClass.ListName)>0)
                {
                    if ((infoClass.Options & Options.Vibrate) == Options.Vibrate)
                    {
                        VibrateStart();
                        if ((infoClass.Options & Options.Sound) == Options.Sound)
                        {
                            Sound s = new Sound(infoClass.Sound);
                            s.Play();
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                        VibrateStop();
                    }
                    else if ((infoClass.Options & Options.Sound) == Options.Sound)
                    {
                        Sound s = new Sound(infoClass.Sound);
                        s.Play();
                    }

                    if ((infoClass.Options & Options.Message) == Options.Message)
                    {
                        ShowNotifications();
                    }
                }
            }
        }
        
        
        private string GetMessagesText()
        {
            System.Text.StringBuilder HTMLString = new StringBuilder();
            HTMLString.Append("<html><body>");
            HTMLString.Append("New statuses are available.");
            HTMLString.Append("</body></html>");

            return HTMLString.ToString();
        }


        #region VibrateCode
        private void VibrateStop()
        {
            vib.SetLedStatus(1, Led.LedState.Off);
        }
        
        void VibrateStart() 
        {
            vib.SetLedStatus(1, Led.LedState.On);
        }
        #endregion
        private Led vib = new Led();
    }
}
