using System;
using System.Collections.Generic;
using System.Text;

static class ClientSettings
{
    public static string UserName { get; set; }
    public static string Password { get; set; }
    public static bool CheckVersion { get; set; }
    public static int SmallArtSize = 60;
    public static System.Drawing.Color ForeColor = System.Drawing.Color.LightGray;
    public static System.Drawing.Color BackColor = System.Drawing.Color.Black;
    public static System.Drawing.Color LinkColor = System.Drawing.Color.LightBlue;
    public static System.Drawing.Color SelectedBackColor = System.Drawing.Color.DarkSlateGray;
    public static System.Drawing.Color SelectedForeColor = System.Drawing.Color.White;
    public static string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

    static ClientSettings()
    {
        LoadColors();
        LoadSettings();
    }

    private static void LoadColors()
    {
        if (System.IO.File.Exists(AppPath + "\\colors.txt"))
        {
        }
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
