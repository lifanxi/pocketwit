using System;
public abstract class WaitHandle : MarshalByRefObject, IDisposable
{
    // Fields
    protected static readonly IntPtr InvalidHandle = new IntPtr(-1);
    private const int WAIT_ABANDONED = 0x80;
    private const int WAIT_OBJECT_0 = 0;
    private const int WAIT_TIMEOUT = 0x102;
    private IntPtr waitHandle = InvalidHandle;

    // Methods
    internal bool CheckResultInternal(bool r)
    {
        if (!r && (this.waitHandle == InvalidHandle))
        {
            throw new ObjectDisposedException(null);
        }
        return r;
    }

    public virtual void Close()
    {
        throw new NotSupportedException();
    }

    protected virtual void Dispose(bool explicitDisposing)
    {
        if (this.waitHandle != InvalidHandle)
        {
            this.Close();
        }
    }

    internal virtual void SetHandleInternal(IntPtr handle)
    {
        this.waitHandle = handle;
    }

    void IDisposable.Dispose()
    {
        this.Dispose(true);
    }

    public virtual bool WaitOne()
    {
        throw new NotSupportedException();
    }

    public virtual bool WaitOne(int millisecondsTimeout, bool exitContext)
    {
        throw new NotSupportedException();
    }

    // Properties
    public virtual IntPtr Handle
    {
        get
        {
            return this.waitHandle;
        }
        set
        {
            this.waitHandle = value;
        }
    }
}
