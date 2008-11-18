using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace PockeTwit.Utility
{
    public class Memory
    {
        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr LocalAlloc(int uFlags, int uBytes);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr LocalReAlloc(IntPtr hMem, int uBytes, int fuFlags);

        private const int LMEM_FIXED = 0;
        private const int LMEM_MOVEABLE = 2;
        private const int LMEM_ZEROINIT = 0x40;
        private const int LPTR = (LMEM_FIXED | LMEM_ZEROINIT);

        // Allocates a block of memory using LocalAlloc

        public static IntPtr AllocHLocal(int cb)
        {
            return LocalAlloc(LPTR, cb);
        }

        // Frees memory allocated by AllocHLocal

        public static void FreeHLocal(IntPtr hlocal)
        {
            if (!hlocal.Equals(IntPtr.Zero))
            {
                if (!IntPtr.Zero.Equals(LocalFree(hlocal)))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                hlocal = IntPtr.Zero;
            }
        }

        // Resizes a block of memory previously allocated with AllocHLocal

        public static IntPtr ReAllocHLocal(IntPtr pv, int cb)
        {
            IntPtr newMem = LocalReAlloc(pv, cb, LMEM_MOVEABLE);
            if (newMem.Equals(IntPtr.Zero))
            {
                throw new OutOfMemoryException();
            }
            return newMem;
        }

        // Copies the contents of a managed string to unmanaged memory

        public static IntPtr StringToHLocalUni(string s)
        {
            if (s == null)
            {
                return IntPtr.Zero;
            }
            else
            {
                int nc = s.Length;
                int len = 2 * (1 + nc);
                IntPtr hLocal = AllocHLocal(len);
                if (hLocal.Equals(IntPtr.Zero))
                {
                    throw new OutOfMemoryException();
                }
                else
                {
                    Marshal.Copy(s.ToCharArray(), 0, hLocal, s.Length);
                    return hLocal;
                }
            }
        }
    }

}
