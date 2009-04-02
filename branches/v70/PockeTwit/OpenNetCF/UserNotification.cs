using System.Runtime.InteropServices;
using System;

[Flags]
public enum NotificationAction
{
    Dialog = 4,
    Led = 1,
    Private = 0x20,
    Repeat = 0x10,
    Sound = 8,
    Vibrate = 2
}

public enum NotificationType
{
    ClassicTime = 4,
    Event = 1,
    Period = 3,
    Time = 2
}

public enum NotificationStatus
{
    Inactive,
    Signalled
}

[StructLayout(LayoutKind.Sequential)]
public class UserNotification
{
    private int ActionFlags;
    [MarshalAs(UnmanagedType.LPWStr)]
    private string pwszDialogTitle;
    [MarshalAs(UnmanagedType.LPWStr)]
    private string pwszDialogText;
    [MarshalAs(UnmanagedType.LPWStr)]
    private string pwszSound;
    private int nMaxSound;
    private int dwReserved;
    public NotificationAction Action
    {
        get
        {
            return (NotificationAction) this.ActionFlags;
        }
        set
        {
            this.ActionFlags = (int) value;
        }
    }
    public string Title
    {
        get
        {
            return this.pwszDialogTitle;
        }
        set
        {
            this.pwszDialogTitle = value;
        }
    }
    public string Text
    {
        get
        {
            return this.pwszDialogText;
        }
        set
        {
            this.pwszDialogText = value;
        }
    }
    public string Sound
    {
        get
        {
            return this.pwszSound;
        }
        set
        {
            this.pwszSound = value;
        }
    }
    internal int MaxSound
    {
        get
        {
            return this.nMaxSound;
        }
        set
        {
            this.nMaxSound = value;
        }
    }
}
