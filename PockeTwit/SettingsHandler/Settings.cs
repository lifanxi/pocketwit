using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Drawing;
public static class ClientSettings
{


		#region Fields (13) 

    private static int _TextSize = 0;
    public static int AnimationInterval;
    public static string AppPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
    public static System.Drawing.Color BackColor = System.Drawing.Color.Black;
    public static System.Drawing.Color ErrorColor = System.Drawing.Color.Red;
    public static System.Drawing.Color ForeColor = System.Drawing.Color.LightGray;
    public static System.Drawing.Color LinkColor = System.Drawing.Color.LightBlue;
    public static System.Drawing.Color SmallTextColor = System.Drawing.Color.Gray;
    public static System.Drawing.Color SelectedSmallTextColor = System.Drawing.Color.Gray;
    public static int Margin = 5;
    public static int MaxTweets = 50;
    public static System.Drawing.Color SelectedBackColor = System.Drawing.Color.DarkSlateGray;
    public static System.Drawing.Color SelectedForeColor = System.Drawing.Color.White;
    public static System.Drawing.Font MenuFont = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
    public static Font SmallFont = new Font(FontFamily.GenericSansSerif, 6, FontStyle.Regular);
    public static int SmallArtSize = 65;
    public static int UpdateInterval;

    public static int LinesOfText
    {
        get
        {
            if (ShowExtra)
            {
                return 6;
            }
            else
            {
                return 5;
            }
        }
    }
		#endregion Fields 

		#region Constructors (1) 

    static ClientSettings()
    {
        GetTextSizes();
        LoadColors();
        LoadSettings();
    }

		#endregion Constructors 

		#region Properties (7) 

    public static bool ShowAvatars { get; set; }

    public static bool UseGPS { get; set; }

    public static bool IsMaximized { get; set; }

    public static bool BeepOnNew { get; set; }

    public static bool CheckVersion { get; set; }

    public static string DistancePreference { get; set; }

    public static bool ShowReplyImages { get; set; }

    public static bool _ShowExtra = true;
    public static bool ShowExtra
    {
        get { return _ShowExtra; }
        set { _ShowExtra = value; }
    }
    public static int SmallTextSize
    { get; set; }
    public static int TextSize 
    {
        get
        {
            return _TextSize;
        }
        set
        {
            _TextSize = value;
            SmallArtSize = _TextSize * 5;
        }
    }

    //public static string UserName { get; set; }

    public static List<Yedda.Twitter.Account> AccountsList { get; set; }
		#endregion Properties 

		#region Methods (4) 


		// Public Methods (2) 

    public static void LoadSettings()
    {
        ConfigurationSettings.LoadConfig();

        AccountsList = new List<Yedda.Twitter.Account>();
        Yedda.Twitter.Account LegacySettingsAccount = new Yedda.Twitter.Account();
        try
        {
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["ShowAvatars"]))
            {
                ShowAvatars = bool.Parse(ConfigurationSettings.AppSettings["ShowAvatars"]);
            }
            else
            {
                ShowAvatars = true;
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["DistancePreference"]))
            {
                DistancePreference = ConfigurationSettings.AppSettings["DistancePreference"];
            }
            else
            {
                DistancePreference = "Miles";
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["UseGPS"]))
            {
                UseGPS = bool.Parse(ConfigurationSettings.AppSettings["UseGPS"]);
            }
            else
            {
                UseGPS = true;
            }

            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["IsMaximized"]))
            {
                IsMaximized = bool.Parse(ConfigurationSettings.AppSettings["IsMaximized"]);
            }
            else
            {
                IsMaximized = true;
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["UserName"]))
            {
                LegacySettingsAccount.UserName = ConfigurationSettings.AppSettings["UserName"];
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["Password"]))
            {
                LegacySettingsAccount.Password = ConfigurationSettings.AppSettings["Password"];
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
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["ShowExtra"]))
            {
                ShowExtra = bool.Parse(ConfigurationSettings.AppSettings["ShowExtra"]);
            }

            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["Server"]))
            {
                LegacySettingsAccount.Server = (Yedda.Twitter.TwitterServer)Enum.Parse(typeof(Yedda.Twitter.TwitterServer), ConfigurationSettings.AppSettings["Server"], true);
            }

            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["AnimationInterval"]))
            {
                AnimationInterval = int.Parse(ConfigurationSettings.AppSettings["AnimationInterval"]);
            }
            else
            {
                AnimationInterval = 15;
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["UpdateInterval"]))
            {
                UpdateInterval = int.Parse(ConfigurationSettings.AppSettings["UpdateInterval"]);
            }
            else
            {
                UpdateInterval = 90000;
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["MaxTweets"]))
            {
                MaxTweets= int.Parse(ConfigurationSettings.AppSettings["MaxTweets"]);
            }
            else
            {
                MaxTweets = 50;
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["ShowReplyImages"]))
            {
                ShowReplyImages = bool.Parse(ConfigurationSettings.AppSettings["ShowReplyImages"]);
            }
            else
            {
                ShowReplyImages = false;
            }
            if (!string.IsNullOrEmpty(LegacySettingsAccount.UserName))
            {
                AccountsList.Add(LegacySettingsAccount);
            }

            foreach (Yedda.Twitter.Account a in ConfigurationSettings.Accounts)
            {
                AccountsList.Add(a);
            }
        }
        catch(Exception e)
        {}
        
    }

    public static void SaveSettings()
    {
        ConfigurationSettings.AppSettings["ShowAvatars"] = ShowAvatars.ToString();
        ConfigurationSettings.AppSettings["UseGPS"] = UseGPS.ToString();
        ConfigurationSettings.AppSettings["IsMaximized"] = IsMaximized.ToString();
        ConfigurationSettings.AppSettings["CheckVersion"] = CheckVersion.ToString();
        ConfigurationSettings.AppSettings["BeepOnNew"] = BeepOnNew.ToString();
        ConfigurationSettings.AppSettings["AnimationInterval"] = AnimationInterval.ToString();
        ConfigurationSettings.AppSettings["UpdateInterval"] = UpdateInterval.ToString();
        ConfigurationSettings.AppSettings["MaxTweets"] = MaxTweets.ToString();
        ConfigurationSettings.AppSettings["ShowReplyImages"] = ShowReplyImages.ToString();
        ConfigurationSettings.AppSettings["DistancePreference"] = DistancePreference;
        if (ConfigurationSettings.AppSettings["UserName"] != null)
        {
            ConfigurationSettings.AppSettings.Remove("UserName");
        }
        if (ConfigurationSettings.AppSettings["Password"] != null)
        {
            ConfigurationSettings.AppSettings.Remove("Password");
        }
        ConfigurationSettings.AppSettings["ShowExtra"] = ShowExtra.ToString();
        ConfigurationSettings.Accounts = AccountsList;
        ConfigurationSettings.SaveConfig();
    }



		// Private Methods (2) 

    private static void GetTextSizes()
    {
        using (System.Drawing.Bitmap b = new System.Drawing.Bitmap(100, 100))
        {
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
            {
                TextSize = (int)(g.MeasureString("H", new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 9, System.Drawing.FontStyle.Regular)).Height - 1);
                SmallTextSize = (int)(g.MeasureString("H", new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 5, System.Drawing.FontStyle.Regular)).Height - 1);
            }
        }
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


                    System.Drawing.Color ColorChosen = System.Drawing.Color.FromArgb(int.Parse(ColorPair[1]), int.Parse(ColorPair[2]), int.Parse(ColorPair[3]));

                    FieldInfo fi = typeof(ClientSettings).GetField(ColorType);
                    fi.SetValue(null, ColorChosen);
                }
            }

        }
    }


		#endregion Methods 

}
