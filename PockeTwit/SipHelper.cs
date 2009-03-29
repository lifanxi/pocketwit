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

        public static void DisableCompletion(TextBox textBox)
        {
            textBox.Capture = true;
            IntPtr m_hWnd = textBox.Handle;
            textBox.Capture = false;
            SIPINFO info = new SIPINFO();
            SHSipInfo(SPI_GETSIPINFO, 0, info, 0);
            m_dwSipFlasg = info.fdwFlags;
            info.fdwFlags |= SIPF_DISABLECOMPLETION;
            SHSipInfo(SPI_SETSIPINFO, 0, info, 0);
            SHSipPreference(m_hWnd, SIPSTATE.SIP_FORCEDOWN);
            SHSipPreference(m_hWnd, SIPSTATE.SIP_UP);
        }


        public static void EnableCompletion(TextBox textBox)
        {
            textBox.Capture = true;
            IntPtr m_hWnd = textBox.Handle;
            textBox.Capture = false;
            SIPINFO info = new SIPINFO();
            SHSipInfo(SPI_GETSIPINFO, 0, info, 0);
            info.fdwFlags &= ~SIPF_DISABLECOMPLETION;
            SHSipInfo(SPI_SETSIPINFO, 0, info, 0);
            SHSipPreference(m_hWnd, SIPSTATE.SIP_FORCEDOWN);
            SHSipPreference(m_hWnd, SIPSTATE.SIP_UP);
        }

        #region p/invoke

        private const int SPI_SETSIPINFO = 224;
        private const int SPI_GETSIPINFO = 225;
        private const int SIPF_DISABLECOMPLETION = 0x08;


        [DllImport("coredll.dll")]
        private static extern IntPtr GetCapture();

        [DllImport("aygshell.dll")]
        private static extern int SHSipInfo(
            uint uiAction,
            uint uiParam,
            SIPINFO pvParam,
            uint fWinIni);


        [DllImport("aygshell.dll")]
        private static extern int SHSipPreference(
            IntPtr hwnd, SIPSTATE st);

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

        private enum SIPSTATE
        {
            SIP_UP = 0,
            SIP_DOWN,
            SIP_FORCEDOWN,
            SIP_UNCHANGED,
            SIP_INPUTDIALOG
        }

        #endregion
    }
}
