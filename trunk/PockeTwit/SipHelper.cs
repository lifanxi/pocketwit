using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DisableCompletion
{
    /// <summary>
    /// Summary description for SIPHelper.
    /// </summary>
    public class SIPHelper
    {

        private static int m_dwSipFlasg;

        public static void DisableCompletion()
        {
            SIPINFO info = new SIPINFO();
            SHSipInfo(SPI_GETSIPINFO, 0, info, 0);
            m_dwSipFlasg = info.fdwFlags;
            info.fdwFlags |= SIPF_DISABLECOMPLETION;
            SHSipInfo(SPI_SETSIPINFO, 0, info, 0);
            SHSipInfo(SPI_SETCOMPLETIONINFO, 0, info, 0);
        }


        public static void EnableCompletion()
        {
            SIPINFO info = new SIPINFO();
            SHSipInfo(SPI_GETSIPINFO, 0, info, 0);
            info.fdwFlags &= ~SIPF_DISABLECOMPLETION;
            SHSipInfo(SPI_SETSIPINFO, 0, info, 0);
            SHSipInfo(SPI_SETCOMPLETIONINFO, 0, info, 0);
        }

        #region p/invoke

        private const int SPI_SETCOMPLETIONINFO = 223;
        private const int SPI_SETSIPINFO = 224;
        private const int SPI_GETSIPINFO = 225;
        private const int SIPF_DISABLECOMPLETION = 0x08;


        [DllImport("aygshell.dll")]
        private static extern int SHSipInfo(
            uint uiAction,
            uint uiParam,
            SIPINFO pvParam,
            uint fWinIni);


        private class SIPINFO
        {
            public int cbSize;
            public int fdwFlags;
            //RECT    rcVisibleDesktop;
            public int deskLeft;
            public int deskTop;
            public int deskRight;
            public int deskBottom;
            //RECT    rcSipRect;
            public int sipLeft;
            public int sipTop;
            public int sipRight;
            public int sipBottom;
            public int dwImDataSize;
            public IntPtr pvImData;
        }

        #endregion
    }
}
