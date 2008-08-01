using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
static class ClientSettings
{
    public static string UserName { get; set; }
    public static string Password { get; set; }
    public static bool CheckVersion { get; set; }
    public static bool BeepOnNew { get; set; }
    public static Yedda.Twitter.TwitterServer Server { get; set; }
    public static int AnimationInterval = 50;

    public static int SmallArtSize = 60;
    
    public static System.Drawing.Color ForeColor = System.Drawing.Color.LightGray;
    public static System.Drawing.Color BackColor = System.Drawing.Color.Black;
    public static System.Drawing.Color LinkColor = System.Drawing.Color.LightBlue;
    public static System.Drawing.Color SelectedBackColor = System.Drawing.Color.DarkSlateGray;
    public static System.Drawing.Color SelectedForeColor = System.Drawing.Color.White;
    public static System.Drawing.Color ErrorColor = System.Drawing.Color.Red;
    public static string AppPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

    static ClientSettings()
    {
        LoadColors();
        LoadSettings();
    }

    private static void LoadColors()
    {
        if (System.IO.File.Exists(AppPath + "\\colors.txt"))
        {
            using (System.IO.StreamReader r = new System.IO.StreamReader(AppPath + "\\colors.txt"))
            {
                while(!r.EndOfStream)
                {
                    string[] ColorPair = r.ReadLine().Split(new char[]{':'});
                    string ColorType = ColorPair[0];


                    System.Drawing.Color ColorChosen = System.Drawing.Color.FromArgb(int.Parse(ColorPair[1]), int.Parse(ColorPair[1]), int.Parse(ColorPair[1]));

                    FieldInfo fi = typeof(ClientSettings).GetField(ColorType);
                    fi.SetValue(null, ColorChosen);
                }
            }

        }
    }

    public static void LoadSettings()
    {
        ConfigurationSettings.LoadConfig();

        try
        {
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["UserName"]))
            {
                UserName = ConfigurationSettings.AppSettings["UserName"];
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["Password"]))
            {
                Password = ConfigurationSettings.AppSettings["Password"];
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["CheckVersion"]))
            {
                CheckVersion = bool.Parse(ConfigurationSettings.AppSettings["CheckVersion"]);
            }
            else
            {
                CheckVersion = true;
            }

            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["BeepOnNew"]))
            {
                BeepOnNew = bool.Parse(ConfigurationSettings.AppSettings["BeepOnNew"]);
            }
            else
            {
                BeepOnNew = false;
            }

            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["Server"]))
            {
                Server = (Yedda.Twitter.TwitterServer)Enum.Parse(typeof(Yedda.Twitter.TwitterServer),ConfigurationSettings.AppSettings["Server"],true);
            }
            else
            {
                Server = Yedda.Twitter.TwitterServer.twitter;
            }

            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["AnimationInterval"]))
            {
                AnimationInterval = int.Parse(ConfigurationSettings.AppSettings["AnimationInterval"]);
            }
            else
            {
                AnimationInterval = 50;
            }
        }
        catch{}
        
    }
    public static void SaveSettings()
    {
        ConfigurationSettings.AppSettings["UserName"] = UserName;
        ConfigurationSettings.AppSettings["Password"] = Password;
        ConfigurationSettings.AppSettings["CheckVersion"] = CheckVersion.ToString() ;
        ConfigurationSettings.AppSettings["BeepOnNew"] = BeepOnNew.ToString();
        ConfigurationSettings.AppSettings["Server"] = Server.ToString();
        ConfigurationSettings.AppSettings["AnimationInterval"] = AnimationInterval.ToString();
        ConfigurationSettings.SaveConfig();
    }
}
