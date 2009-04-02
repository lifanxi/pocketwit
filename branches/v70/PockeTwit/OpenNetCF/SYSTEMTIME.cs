using System.Runtime.InteropServices;
using System;
[StructLayout(LayoutKind.Sequential)]
public struct SYSTEMTIME
{
    public short wYear;
    public short wMonth;
    public short wDayOfWeek;
    public short wDay;
    public short wHour;
    public short wMinute;
    public short wSecond;
    public short wMillisecond;
    public static SYSTEMTIME FromDateTime(DateTime dt)
    {
        SYSTEMTIME systemtime = new SYSTEMTIME();
        systemtime.wYear = (short)dt.Year;
        systemtime.wMonth = (short)dt.Month;
        systemtime.wDayOfWeek = (short)dt.DayOfWeek;
        systemtime.wDay = (short)dt.Day;
        systemtime.wHour = (short)dt.Hour;
        systemtime.wMinute = (short)dt.Minute;
        systemtime.wSecond = (short)dt.Second;
        systemtime.wMillisecond = (short)dt.Millisecond;
        return systemtime;
    }

    public DateTime ToDateTime()
    {
        if ((((this.wYear == 0) && (this.wMonth == 0)) && ((this.wDay == 0) && (this.wHour == 0))) && ((this.wMinute == 0) && (this.wSecond == 0)))
        {
            return DateTime.MinValue;
        }
        return new DateTime(this.wYear, this.wMonth, this.wDay, this.wHour, this.wMinute, this.wSecond, this.wMillisecond);
    }
}


