using System;
using System.Collections.Generic;
using System.Text;

static class ClientSettings
{
    public static string UserName { get; set; }
    public static string Password { get; set; }
    public static bool CheckVersion { get; set; }
    public static int CachedTweets { get; set; }

    public static int SmallArtSize = 60;
    public static string AppPath
    {
        get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase); }
    }

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
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["CachedTweets"]))
            {
                CachedTweets = int.Parse(ConfigurationSettings.AppSettings["CachedTweets"]);
            }
            else
            {
                CachedTweets = 50;
            }
        }
        catch{}
        
    }
    public static void SaveSettings()
    {
        ConfigurationSettings.AppSettings["UserName"] = UserName;
        ConfigurationSettings.AppSettings["Password"] = Password;
        ConfigurationSettings.AppSettings["CheckVersion"] = CheckVersion.ToString();
        ConfigurationSettings.AppSettings["CachedTweets"] = CachedTweets.ToString();
        ConfigurationSettings.SaveConfig();
    }
}
