using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
public static class Notify
{
    // Methods
    public static bool ClearUserNotification(int handle)
    {
        return NativeMethods.CeClearUserNotification(handle);
    }

    public static UserNotificationInfoHeader GetUserNotification(int handle)
    {
        int pcBytesNeeded = 0;
        NativeMethods.CeGetUserNotification(handle, 0, ref pcBytesNeeded, IntPtr.Zero);
        IntPtr pBuffer = Marshal.AllocHGlobal(pcBytesNeeded);
        if (!NativeMethods.CeGetUserNotification(handle, (uint) pcBytesNeeded, ref pcBytesNeeded, pBuffer))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Error getting UserNotification");
        }
        UserNotificationInfoHeader header = new UserNotificationInfoHeader();
        header.hNotification = Marshal.ReadInt32(pBuffer, 0);
        header.dwStatus = Marshal.ReadInt32(pBuffer, 4);
        IntPtr ptr = (IntPtr) Marshal.ReadInt32(pBuffer, 8);
        IntPtr ptr3 = (IntPtr) Marshal.ReadInt32(pBuffer, 12);
        header.pcent = new UserNotificationTrigger();
        if (ptr != IntPtr.Zero)
        {
            header.pcent.dwSize = Marshal.ReadInt32(ptr);
            header.pcent.Type = (NotificationType) Marshal.ReadInt32(ptr, 4);
            header.pcent.Event = (NotificationEvent) Marshal.ReadInt32(ptr, 8);
            header.pcent.Application = Marshal.PtrToStringUni((IntPtr) Marshal.ReadInt32(ptr, 12));
            header.pcent.Arguments = Marshal.PtrToStringUni((IntPtr) Marshal.ReadInt32(ptr, 0x10));
            header.pcent.stStartTime = (SYSTEMTIME) Marshal.PtrToStructure((IntPtr) (ptr.ToInt32() + 20), typeof(SYSTEMTIME));
            header.pcent.stEndTime = (SYSTEMTIME) Marshal.PtrToStructure((IntPtr) (ptr.ToInt32() + 0x24), typeof(SYSTEMTIME));
        }
        header.pceun = new UserNotification();
        if (ptr3 != IntPtr.Zero)
        {
            header.pceun.Action = (NotificationAction) Marshal.ReadInt32(ptr3, 0);
            header.pceun.Title = Marshal.PtrToStringUni((IntPtr) Marshal.ReadInt32(ptr3, 4));
            header.pceun.Text = Marshal.PtrToStringUni((IntPtr) Marshal.ReadInt32(ptr3, 8));
            header.pceun.Sound = Marshal.PtrToStringUni((IntPtr) Marshal.ReadInt32(ptr3, 12));
            header.pceun.MaxSound = Marshal.ReadInt32(ptr3, 0x10);
        }
        Marshal.FreeHGlobal(pBuffer);
        return header;
    }

    public static int[] GetUserNotificationHandles()
    {
        int pcHandlesNeeded = 0;
        if (!NativeMethods.CeGetUserNotificationHandles(null, 0, ref pcHandlesNeeded))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Error retrieving handles");
        }
        int[] rghNotifications = new int[pcHandlesNeeded];
        if (!NativeMethods.CeGetUserNotificationHandles(rghNotifications, pcHandlesNeeded, ref pcHandlesNeeded))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Error retrieving handles");
        }
        return rghNotifications;
    }

    public static UserNotification GetUserNotificationPreferences(IntPtr hWnd)
    {
        UserNotification template = new UserNotification();
        return GetUserNotificationPreferences(hWnd, template);
    }

    public static UserNotification GetUserNotificationPreferences(IntPtr hWnd, UserNotification template)
    {
        template.MaxSound = 260;
        template.Sound = new string('\0', 260);
        if (!NativeMethods.CeGetUserNotificationPreferences(hWnd, template))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not get user preferences");
        }
        return template;
    }

    public static void HandleAppNotifications(string application)
    {
        if (!NativeMethods.CeHandleAppNotifications(application))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Error clearing Application Notifications");
        }
    }

    public static void RunAppAtEvent(string appName, NotificationEvent whichEvent)
    {
        if (!NativeMethods.CeRunAppAtEvent(appName, (int) whichEvent))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot Set Notification Handler");
        }
    }

    public static void RunAppAtTime(string appName, DateTime time)
    {
        SYSTEMTIME lpTime = new SYSTEMTIME();
        if (time != DateTime.MinValue)
        {
            lpTime = new SYSTEMTIME();
            lpTime = SYSTEMTIME.FromDateTime(time);
            if (!NativeMethods.CeRunAppAtTime(appName, ref lpTime))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot Set Notification Handler");
            }
        }
        else if (!NativeMethods.CeRunAppAtTimeCancel(appName, null))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot Cancel Notification Handler");
        }
    }

    public static void SetNamedEventAtTime(string eventName, DateTime eventTime)
    {
        RunAppAtTime(string.Format(@"\\.\Notifications\NamedEvents\{0}", eventName), eventTime);
    }

    public static void SetNamedEventAtTime(string eventName, TimeSpan timeFromNow)
    {
        RunAppAtTime(string.Format(@"\\.\Notifications\NamedEvents\{0}", eventName), DateTime.Now.Add(timeFromNow));
    }

    public static int SetUserNotification(UserNotificationTrigger trigger, UserNotification notification)
    {
        int num = NativeMethods.CeSetUserNotificationEx(0, trigger, notification);
        if (num == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Error setting UserNotification");
        }
        return num;
    }

    public static int SetUserNotification(int handle, UserNotificationTrigger trigger, UserNotification notification)
    {
        int num = NativeMethods.CeSetUserNotificationEx(handle, trigger, notification);
        if (num == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Error setting UserNotification");
        }
        return num;
    }

    public static int SetUserNotification(string application, DateTime time, UserNotification notify)
    {
        return SetUserNotification(0, application, time, notify);
    }

    public static int SetUserNotification(int handle, string application, DateTime time, UserNotification notify)
    {
        SYSTEMTIME lpTime = SYSTEMTIME.FromDateTime(time);
        int num = NativeMethods.CeSetUserNotification(handle, application, ref lpTime, notify);
        if (num == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Error setting UserNotification");
        }
        return num;
    }
}

