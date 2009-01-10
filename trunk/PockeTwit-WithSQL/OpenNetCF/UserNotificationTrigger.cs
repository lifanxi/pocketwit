using System.Runtime.InteropServices;
using System;
[StructLayout(LayoutKind.Sequential)]
public class UserNotificationTrigger
{
    internal int dwSize = 0x34;
    private int dwType;
    private int dwEvent;
    [MarshalAs(UnmanagedType.LPWStr)]
    private string lpszApplication = "";
    [MarshalAs(UnmanagedType.LPWStr)]
    private string lpszArguments;
    internal SYSTEMTIME stStartTime = new SYSTEMTIME();
    internal SYSTEMTIME stEndTime = new SYSTEMTIME();
    public NotificationType Type
    {
        get
        {
            return (NotificationType) this.dwType;
        }
        set
        {
            this.dwType = (int) value;
        }
    }
    public NotificationEvent Event
    {
        get
        {
            return (NotificationEvent) this.dwEvent;
        }
        set
        {
            this.dwEvent = (int) value;
        }
    }
    public string Application
    {
        get
        {
            return this.lpszApplication;
        }
        set
        {
            this.lpszApplication = value;
        }
    }
    public string Arguments
    {
        get
        {
            return this.lpszArguments;
        }
        set
        {
            this.lpszArguments = value;
        }
    }
    public DateTime StartTime
    {
        get
        {
            return this.stStartTime.ToDateTime();
        }
        set
        {
            this.stStartTime = SYSTEMTIME.FromDateTime(value);
        }
    }
    public DateTime EndTime
    {
        get
        {
            return this.stEndTime.ToDateTime();
        }
        set
        {
            this.stEndTime = SYSTEMTIME.FromDateTime(value);
        }
    }
}
