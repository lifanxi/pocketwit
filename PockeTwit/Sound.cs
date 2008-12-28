using System;

using System.IO;
using System.Runtime.InteropServices;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    public class Sound
    {
        private byte[] m_soundBytes;
        private string m_fileName;
        private const string m_waveDevice = "wav1:";

        private enum Flags
        {
            SND_SYNC = 0x0000,  /* play synchronously (default) */
            SND_ASYNC = 0x0001,  /* play asynchronously */
            SND_NODEFAULT = 0x0002,  /* silence (!default) if sound not found */
            SND_MEMORY = 0x0004,  /* pszSound points to a memory file */
            SND_LOOP = 0x0008,  /* loop the sound until next sndPlaySound */
            SND_NOSTOP = 0x0010,  /* don't stop any currently playing sound */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000, /* name is a registry alias */
            SND_ALIAS_ID = 0x00110000, /* alias is a predefined ID */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040004  /* name is resource name or atom */
        }

        [DllImport("coredll.dll", SetLastError = true)]
        static extern int SetSystemPowerState(string psState, int StateFlags, int Options);
        const int POWER_STATE_ON = 0x00010000;
        const int POWER_STATE_OFF = 0x00020000;
        const int POWER_STATE_SUSPEND = 0x00200000;
        const int POWER_FORCE = 4096;
        const int POWER_STATE_RESET = 0x00800000;

        public enum CEDEVICE_POWER_STATE : int
        {
          PwrDeviceUnspecified = -1,
          //Full On: full power,  full functionality
          D0 = 0,
          /// <summary>
          /// Low Power On: fully functional at low power/performance
          /// </summary>
          D1 = 1,
          /// <summary>
          /// Standby: partially powered with automatic wake
          /// </summary>
          D2 = 2,
          /// <summary>
          /// Sleep: partially powered with device initiated wake
          /// </summary>
          D3 = 3,
          /// <summary>
          /// Off: unpowered
          /// </summary>
          D4 = 4,
          PwrDeviceMaximum
        }

        public enum DevicePowerFlags
        {
          None = 0,
          /// <summary>
          /// Specifies the name of the device whose power should be maintained at or above the DeviceState level.
          /// </summary>
          POWER_NAME = 0x00000001,
          /// <summary>
          /// Indicates that the requirement should be enforced even during a system suspend.
          /// </summary>
          POWER_FORCE = 0x00001000,
          POWER_DUMPDW = 0x00002000
        }

        [DllImport("CoreDll.DLL", EntryPoint = "PlaySoundW", SetLastError = true)]
        private extern static int WCE_PlaySound(string szSound, IntPtr hMod, int flags);

        [DllImport("CoreDll.DLL", EntryPoint = "PlaySoundW", SetLastError = true)]
        private extern static int WCE_PlaySoundBytes(byte[] szSound, IntPtr hMod, int flags);

        [DllImport("CoreDLL", SetLastError = true)]
        public static extern IntPtr SetDevicePower(string pDevice, DevicePowerFlags DeviceFlags, CEDEVICE_POWER_STATE DevicePowerState);

        [DllImport("CoreDLL")]
        public static extern int GetDevicePower(string device, DevicePowerFlags flags, out CEDEVICE_POWER_STATE PowerState);

        /// <summary>
        /// Construct the Sound object to play sound data from the specified file.
        /// </summary>
        public Sound(string fileName)
        {
            m_fileName = fileName;
        }

        /// <summary>
        /// Construct the Sound object to play sound data from the specified stream.
        /// </summary>
        public Sound(Stream stream)
        {
            // read the data from the stream
            m_soundBytes = new byte[stream.Length];
            stream.Read(m_soundBytes, 0, (int)stream.Length);
        }

        /// <summary>
        /// Play the sound
        /// </summary>
        public void Play()
        {
          // if a file name has been registered, call WCE_PlaySound,
          //  otherwise call WCE_PlaySoundBytes
          //SetSystemPowerState(null, POWER_STATE_ON, POWER_FORCE);

          // Check to see if the wave device is powered before playing a sound
          CEDEVICE_POWER_STATE CurrAudioState = CEDEVICE_POWER_STATE.PwrDeviceUnspecified;

          GetDevicePower(m_waveDevice, DevicePowerFlags.POWER_NAME, out CurrAudioState);

          // If not powered, power up the wave device
          if (CurrAudioState != CEDEVICE_POWER_STATE.D0)
          {
                SetDevicePower(m_waveDevice, DevicePowerFlags.POWER_NAME | DevicePowerFlags.POWER_FORCE, CEDEVICE_POWER_STATE.D0);
                System.Windows.Forms.MessageBox.Show("Wakup wav device");
                
          }

          if (m_fileName != null)
            WCE_PlaySound(m_fileName, IntPtr.Zero, (int)(Flags.SND_FILENAME));
          else
            WCE_PlaySoundBytes(m_soundBytes, IntPtr.Zero, (int)(Flags.SND_MEMORY));


          if (CurrAudioState != CEDEVICE_POWER_STATE.PwrDeviceUnspecified)
          {
            // Change the wave device power state back to what it was
              System.Windows.Forms.MessageBox.Show("sleep wav device");
            SetDevicePower(m_waveDevice, DevicePowerFlags.POWER_NAME | DevicePowerFlags.POWER_FORCE, CurrAudioState);
          }

        }
    }
}
