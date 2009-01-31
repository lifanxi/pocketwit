using System;

using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace PockeTwit
{
    class FirstRunChecker
    {
        private const string StorageRoot = @"\Software\Apps\JustForFun PockeTwit";

        FirstRunChecker()
        {
            
        }



        public static void CheckFirstRun()
        {
            RegistryKey r = Registry.LocalMachine.OpenSubKey(StorageRoot, false);
            if (r == null)
            {
                return;
            }
            byte[] b = (byte[])r.GetValue("FirstRun");
            if (b == null) { return; }
            if (b[0] > 0)
            {
                PerformUpdate();
            }
        }

        private static void PerformUpdate()
        {
            NotificationHandler.ReloadFromDisk();
            SetRunOnce();
        }

        public static void SetRunOnce()
        {
            Registry.LocalMachine.OpenSubKey(StorageRoot, true).SetValue("FirstRun", new byte[] { 0 });
        }
    }
}
