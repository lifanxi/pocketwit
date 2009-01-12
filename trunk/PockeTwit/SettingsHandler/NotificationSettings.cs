using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PockeTwit.SettingsHandler
{
    public partial class NotificationSettings : Form
    {
        public NotificationSettings()
        {
            InitializeComponent();
        }

        private void cmbNotificationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            NotificationHandler.NotificationInfo currentInfo;
            switch (cmbNotificationType.Text)
            {
                case "Friends":
                    currentInfo = NotificationHandler.Friends;
                    break;
                case "Messages":
                    currentInfo = NotificationHandler.Messages;
                    break;
            }

            
        }
    }
}