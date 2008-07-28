using System;
using System.Collections.Generic;
using System.Text;

static class ClientSettings
{
    public static string UserName { get; set; }
    public static string Password { get; set; }
    public static bool CheckVersion { get; set; }
    public static int SmallArtSize = 60;
    static ClientSettings()
    {
        LoadSettings();
    }

    public static void LoadSettings()
    {
        ConfigurationSettings.LoadConfig();

        try
        {
            UserName = ConfigurationSettings.AppSettings["UserName"];
            Password = ConfigurationSettings.AppSettings["Password"];
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["CheckVersion"]))
            {
                CheckVersion = bool.Parse(ConfigurationSettings.AppSettings["CheckVersion"]);
            }
            else
            {
                CheckVersion = true;
            }
        }
        catch{}
        
    }
    public static void SaveSettings()
    {
        ConfigurationSettings.AppSettings["UserName"] = UserName;
        ConfigurationSettings.AppSettings["Password"] = Password;
        ConfigurationSettings.AppSettings["CheckVersion"] = CheckVersion.ToString() ;
        ConfigurationSettings.SaveConfig();
    }
}
