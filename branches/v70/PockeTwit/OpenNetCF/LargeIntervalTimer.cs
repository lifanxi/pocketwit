using System;
using System.Threading;
public class LargeIntervalTimer
{
    // Fields
    private bool m_enabled;
    private string m_eventName = Guid.NewGuid().ToString();
    private DateTime m_firstTime = DateTime.MinValue;
    private TimeSpan m_interval = new TimeSpan(0, 0, 60);
    private bool m_oneShot;
    private EventWaitHandle m_quitHandle;
    private bool m_useFirstTime;
    private EventWaitHandle m_waitHandle;

    // Events
    public event EventHandler Tick;

    // Methods
    public LargeIntervalTimer()
    {
        this.m_waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, this.m_eventName);
        this.m_quitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
    }

    ~LargeIntervalTimer()
    {
        this.m_waitHandle.Close();
        this.Enabled = false;
    }

    private void InternalThreadProc()
    {
        do
        {
            if (this.m_useFirstTime)
            {
                Notify.RunAppAtTime(string.Format(@"\\.\Notifications\NamedEvents\{0}", this.m_eventName), this.m_firstTime);
                this.m_useFirstTime = false;
            }
            else
            {
                Notify.RunAppAtTime(string.Format(@"\\.\Notifications\NamedEvents\{0}", this.m_eventName), DateTime.Now.Add(this.m_interval));
                this.m_firstTime = DateTime.MinValue;
            }
            if (EventWaitHandle.WaitAny(new WaitHandle[] { this.m_waitHandle, this.m_quitHandle }) == 0)
            {
                if (this.Tick != null)
                {
                    this.Tick(this, null);
                }
            }
            else
            {
                return;
            }
        }
        while (!this.m_oneShot);
        this.m_enabled = false;
    }

    // Properties
    public bool Enabled
    {
        get
        {
            return this.m_enabled;
        }
        set
        {
            if ((!this.m_enabled || !value) && (this.m_enabled || value))
            {
                this.m_enabled = value;
                if (this.m_enabled)
                {
                    new Thread(new ThreadStart(this.InternalThreadProc)).Start();
                }
                else
                {
                    this.m_quitHandle.Set();
                }
            }
        }
    }

    public DateTime FirstEventTime
    {
        get
        {
            return this.m_firstTime;
        }
        set
        {
            if (value.CompareTo(DateTime.Now) <= 0)
            {
                this.m_firstTime = DateTime.MinValue;
                this.m_useFirstTime = false;
            }
            this.m_firstTime = value;
            this.m_useFirstTime = true;
        }
    }

    public TimeSpan Interval
    {
        get
        {
            return this.m_interval;
        }
        set
        {
            this.m_interval = new TimeSpan(value.Days, value.Hours, value.Minutes, value.Seconds);
        }
    }

    public bool OneShot
    {
        get
        {
            return this.m_oneShot;
        }
        set
        {
            this.m_oneShot = value;
        }
    }
}

 