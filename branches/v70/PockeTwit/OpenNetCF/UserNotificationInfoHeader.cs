using System.Runtime.InteropServices;
[StructLayout(LayoutKind.Sequential)]
public class UserNotificationInfoHeader
{
    public int hNotification;
    public int dwStatus;
    public UserNotificationTrigger pcent;
    public UserNotification pceun;
    public int Handle
    {
        get
        {
            return this.hNotification;
        }
        set
        {
            this.hNotification = value;
        }
    }
    public NotificationStatus Status
    {
        get
        {
            return (NotificationStatus) this.dwStatus;
        }
        set
        {
            this.dwStatus = (int) value;
        }
    }
    public UserNotificationTrigger UserNotificationTrigger
    {
        get
        {
            return this.pcent;
        }
        set
        {
            this.pcent = value;
        }
    }
    public UserNotification UserNotification
    {
        get
        {
            return this.pceun;
        }
        set
        {
            this.pceun = value;
        }
    }
}

 
