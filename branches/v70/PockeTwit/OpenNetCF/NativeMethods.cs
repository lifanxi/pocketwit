using System.Runtime.InteropServices;
using System;
using System.Text;

[Flags]
internal enum PowerEventType
{
    PBT_OEMBASE = 0x10000,
    PBT_POWERINFOCHANGE = 8,
    PBT_POWERSTATUSCHANGE = 4,
    PBT_RESUME = 2,
    PBT_SUSPENDKEYPRESSED = 0x100,
    PBT_TRANSITION = 1
}

[Flags]
internal enum PowerStateFlags
{
    Boot = 0x80000,
    CriticalOff = 0x40000,
    Idle = 0x100000,
    Off = 0x20000,
    On = 0x10000,
    PasswordProtected = 0x10000000,
    Reset = 0x800000,
    Suspend = 0x200000
}

public enum TimeZoneState
{
    Unknown,
    Standard,
    Daylight
}

public enum ProcessorArchitecture : short
{
    Alpha = 2,
    Alpha64 = 7,
    ARM = 5,
    IA64 = 6,
    Intel = 0,
    MIPS = 1,
    PPC = 3,
    SHX = 4,
    Unknown = -1
}


public enum ProcessorType
{
    Alpha_21064 = 0x5248,
    ARM_7TDMI = 0x11171,
    ARM720 = 0x720,
    ARM820 = 0x820,
    ARM920 = 0x920,
    Hitachi_SH3 = 0x2713,
    Hitachi_SH3E = 0x2714,
    Hitachi_SH4 = 0x2715,
    Intel_386 = 0x182,
    Intel_486 = 0x1e6,
    Intel_IA64 = 0x898,
    Intel_Pentium = 0x24a,
    Intel_PentiumII = 0x2ae,
    MIPS_R4000 = 0xfa0,
    Motorola_821 = 0x335,
    PPC_403 = 0x193,
    PPC_601 = 0x259,
    PPC_603 = 0x25b,
    PPC_604 = 0x25c,
    PPC_620 = 620,
    SHx_SH3 = 0x67,
    SHx_SH4 = 0x68,
    StrongARM = 0xa11
}

 


[StructLayout(LayoutKind.Sequential)]
public struct SystemInfo
{
    public ProcessorArchitecture ProcessorArchitecture;
    internal ushort wReserved;
    public int PageSize;
    public int MinimumApplicationAddress;
    public int MaximumApplicationAddress;
    public int ActiveProcessorMask;
    public int NumberOfProcessors;
    public ProcessorType ProcessorType;
    public int AllocationGranularity;
    public short ProcessorLevel;
    public short ProcessorRevision;
}

 

public enum StdIoStream
{
    Input,
    Output,
    ErrorOutput
}
public enum NotificationEvent
{
    None,
    TimeChange,
    SyncEnd,
    OnACPower,
    OffACPower,
    NetConnect,
    NetDisconnect,
    DeviceChange,
    IRDiscovered,
    RS232Detected,
    RestoreEnd,
    Wakeup,
    TimeZoneChange,
    MachineNameChange,
    RndisFNDetected,
    InternetProxyChange
}

 


internal static class NativeMethods
{
    // Fields
    internal const uint BatteryLifeUnknown = uint.MaxValue;
    public const byte BatteryPercentageUnknown = 0xff;
    public const int ERROR_INSUFFICIENT_BUFFER = 0x7a;
    public const int ERROR_NOT_SUPPORTED = 50;
    internal const int ERROR_SUCCESS = 0;
    internal const int FILE_ANY_ACCESS = 0;
    internal const int FILE_DEVICE_HAL = 0x101;
    internal static int IOCTL_HAL_GET_DEVICEID = 0x1010054;
    internal const int IOCTL_HAL_REBOOT = 0x101003c;
    internal const int METHOD_BUFFERED = 0;
    internal const int POWER_FORCE = 0x1000;

    public const int ERROR_ALREADY_EXISTS = 0xb7;
    public const int EVENT_ALL_ACCESS = 3;
    public const int INFINITE = -1;
    public const int WAIT_FAILED = -1;
    public const int WAIT_TIMEOUT = 0x102;


    // Methods
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern int CeGetThreadPriority(IntPtr hThread);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern int CeGetThreadQuantum(IntPtr hThread);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool CeSetThreadPriority(IntPtr hThread, int nPriority);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool CeSetThreadPriority(uint hThread, int nPriority);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool CeSetThreadQuantum(IntPtr hThread, int dwTime);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool CeSetThreadQuantum(uint hThread, int dwTime);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool InitialOwner, string MutexName);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern IntPtr CreateSemaphore(IntPtr lpSemaphoreAttributes, int lInitialCount, int lMaximumCount, string lpName);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern void DeleteCriticalSection(IntPtr lpCriticalSection);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern void EnterCriticalSection(IntPtr lpCriticalSection);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool EventModify(IntPtr hEvent, EVENT ef);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern void InitializeCriticalSection(IntPtr lpCriticalSection);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern void LeaveCriticalSection(IntPtr lpCriticalSection);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern int MsgWaitForMultipleObjectsEx(uint nCount, IntPtr[] lpHandles, uint dwMilliseconds, uint dwWakeMask, uint dwFlags);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern IntPtr OpenEvent(int dwDesiredAccess, bool bInheritHandle, string lpName);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern IntPtr OpenSemaphore(int desiredAccess, bool inheritHandle, string name);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool ReleaseMutex(IntPtr hMutex);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool ReleaseSemaphore(IntPtr handle, int lReleaseCount, out int previousCount);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern uint ResumeThread(IntPtr hThread);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern uint ResumeThread(uint hThread);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern uint SuspendThread(IntPtr hThread);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern uint SuspendThread(uint hThread);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool TerminateThread(IntPtr hThread, int dwExitCode);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern bool TerminateThread(uint hThread, int dwExitCode);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern int WaitForMultipleObjects(int nCount, IntPtr[] lpHandles, bool fWaitAll, int dwMilliseconds);
    [DllImport("coredll.dll", SetLastError = true)]
    public static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);


    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern bool CeClearUserNotification(int hNotification);
    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern bool CeGetUserNotification(int hNotification, uint cBufferSize, ref int pcBytesNeeded, IntPtr pBuffer);
    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern bool CeGetUserNotificationHandles(int[] rghNotifications, int cHandles, ref int pcHandlesNeeded);
    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern bool CeGetUserNotificationPreferences(IntPtr hWndParent, UserNotification lpNotification);
    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern bool CeHandleAppNotifications(string appName);
    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern bool CeRunAppAtEvent(string pwszAppName, int lWhichEvent);
    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern bool CeRunAppAtTime(string pwszAppName, ref SYSTEMTIME lpTime);
    [DllImport("coredll.dll", EntryPoint = "CeRunAppAtTime", SetLastError = true)]
    internal static extern bool CeRunAppAtTimeCancel(string pwszAppName, byte[] lpTime);
    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern int CeSetUserNotification(int hNotification, string pwszAppName, ref SYSTEMTIME lpTime, UserNotification lpUserNotification);
    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern int CeSetUserNotificationEx(int hNotification, UserNotificationTrigger lpTrigger, UserNotification lpUserNotification);
    [DllImport("coredll.dll", EntryPoint = "NLedGetDeviceInfo", SetLastError = true)]
    internal static extern bool NLedGetDeviceCount(short nID, ref Led.NLED_COUNT_INFO pOutput);
    [DllImport("coredll.dll", EntryPoint = "NLedGetDeviceInfo", SetLastError = true)]
    internal static extern bool NLedGetDeviceSupports(short nID, ref Led.NLED_SUPPORTS_INFO pOutput);
    [DllImport("coredll.dll", SetLastError = true)]
    internal static extern bool NLedSetDevice(short nID, ref Led.NLED_SETTINGS_INFO pOutput);

    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern bool CeRunAppAtEvent(string pwszAppName, NotificationEvent lWhichEvent);
    [DllImport("citydb.dll", SetLastError=true)]
    internal static extern void ClockFreeAllTimeZoneData();
    [DllImport("citydb.dll", SetLastError=true)]
    internal static extern int ClockGetNumTimezones();
    [DllImport("citydb.dll", SetLastError=true)]
    internal static extern IntPtr ClockGetTimeZoneData(int nOffset);
    [DllImport("citydb.dll", SetLastError=true)]
    internal static extern IntPtr ClockGetTimeZoneDataByOffset(int nOffset, out int tzIndex);
    [DllImport("citydb.dll", SetLastError=true)]
    internal static extern void ClockLoadAllTimeZoneData();
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern bool GetLocalTime(byte[] st);
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern int GetStdioPath(StdIoStream id, StringBuilder pwszBuf, int lpdwLength);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern void GetSystemInfo(out SystemInfo pSI);
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern bool GetSystemMemoryDivision(ref int lpdwStorePages, ref int lpdwRamPages, ref int lpdwPageSize);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern int GetSystemPowerState(string pBuffer, uint dwBufChars, ref uint pdwFlags);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern bool GetSystemTime(byte[] st);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern TimeZoneState GetTimeZoneInformation(byte[] tzice);
    [DllImport("coredll.dll")]
    internal static extern void GlobalMemoryStatus(out MemoryStatus msce);
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern void GwesPowerOff();
    [DllImport("citydb.dll", SetLastError=true)]
    internal static extern void InitCityDb();
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern bool KernelIoControl(int dwIoControlCode, byte[] inBuf, int inBufSize, byte[] outBuf, int outBufSize, ref int bytesReturned);
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern bool KernelIoControl(int dwIoControlCode, IntPtr inBuf, int inBufSize, IntPtr outBuf, int outBufSize, ref int bytesReturned);
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern IntPtr RequestPowerNotifications(IntPtr hMsgQ, PowerEventType Flags);
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern void SetCleanRebootFlag();
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern bool SetLocalTime(byte[] st);
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern int SetStdioPath(StdIoStream id, string pwszPath);
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern bool SetSystemMemoryDivision(int dwStorePages);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern int SetSystemPowerState(IntPtr psState, PowerStateFlags flags, uint Options);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern int SetSystemPowerState(string psState, PowerStateFlags flags, uint Options);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern bool SetSystemTime(byte[] st);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern bool SetTimeZoneInformation(byte[] tzice);
    [DllImport("coredll.dll", SetLastError=true)]
    public static extern bool StopPowerNotifications(IntPtr hNotifHandle);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern void SystemIdleTimerReset();
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern bool SystemParametersInfo(SystemParametersInfoAction action, int size, byte[] buffer, SystemParametersInfoFlags winini);
    [DllImport("coredll.dll", SetLastError=true)]
    internal static extern void TouchCalibrate();
    [DllImport("citydb.dll", SetLastError=true)]
    internal static extern void UninitCityDb();

    // Nested Types
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryStatus
    {
        internal uint dwLength;
        public int MemoryLoad;
        public int TotalPhysical;
        public int AvailablePhysical;
        public int TotalPageFile;
        public int AvailablePageFile;
        public int TotalVirtual;
        public int AvailableVirtual;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MsgQueueInfo
    {
        public int dwSize;
        public int dwFlags;
        public int dwMaxMessages;
        public int cbMaxMessage;
        public int dwCurrentMessages;
        public int dwMaxQueueMessages;
        public short wNumReaders;
        public short wNumWriters;
    }

    public enum SystemParametersInfoAction
    {
        GetBatteryIdleTimeout = 0xfc,
        GetExternalIdleTimeout = 0xfe,
        GetFontSmoothingContrast = 0x200c,
        GetMouse = 3,
        GetOemInfo = 0x102,
        GetPlatformType = 0x101,
        GetScreenSaveTimeout = 14,
        GetShowSounds = 0x38,
        GetWakeupIdleTimeout = 0x100,
        GetWheelScrollLines = 0x68,
        GetWorkArea = 0x30,
        SetBatteryIdleTimeout = 0xfb,
        SetDeskPattern = 0x15,
        SetDeskWallpaper = 20,
        SetExternalIdleTimeout = 0xfd,
        SetFontSmoothingContrast = 0x200d,
        SetMouse = 4,
        SetScreenSaveTimeout = 15,
        SetShowSounds = 0x39,
        SetWakeupIdleTimeout = 0xff,
        SetWheelScrollLines = 0x69,
        SetWorkArea = 0x2f
    }

    public enum SystemParametersInfoFlags
    {
        None,
        UpdateIniFile,
        SendChange
    }
    public enum EVENT
    {
        PULSE = 1,
        RESET = 2,
        SET = 3
    }

}

 
