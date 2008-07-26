using System;
using System.Collections.Generic;
using System.Text;

static class ClientSettings
{
    public static string UserName { get; set; }
    public static string Password { get; set; }
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
        }
        catch{}
        
    }
    public static void SaveSettings()
    {
        ConfigurationSettings.AppSettings["UserName"] = UserName;
        ConfigurationSettings.AppSettings["Password"] = Password;
        ConfigurationSettings.SaveConfig();
    }
}
