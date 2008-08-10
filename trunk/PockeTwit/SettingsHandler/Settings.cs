using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
public static class ClientSettings
{
    public class Account
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Yedda.Twitter.TwitterServer Server { get; set; }
    }


		#region Fields (13) 

    private static int _TextSize = 0;
    public static int AnimationInterval;
    public static string AppPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
    public static System.Drawing.Color BackColor = System.Drawing.Color.Black;
    public static System.Drawing.Color ErrorColor = System.Drawing.Color.Red;
    public static System.Drawing.Color ForeColor = System.Drawing.Color.LightGray;
    public static System.Drawing.Color LinkColor = System.Drawing.Color.LightBlue;
    public static int Margin = 5;
    public static int MaxTweets = 50;
    public static System.Drawing.Color SelectedBackColor = System.Drawing.Color.DarkSlateGray;
    public static System.Drawing.Color SelectedForeColor = System.Drawing.Color.White;
    public static int SmallArtSize = 60;
    public static int UpdateInterval;

		#endregion Fields 

		#region Constructors (1) 

    static ClientSettings()
    {
        TextSize = GetTextSize();
        LoadColors();
        LoadSettings();
    }

		#endregion Constructors 

		#region Properties (7) 

    public static bool BeepOnNew { get; set; }

    public static bool CheckVersion { get; set; }

    //public static string Password { get; set; }

    //public static Yedda.Twitter.TwitterServer Server { get; set; }

    public static bool ShowReplyImages { get; set; }

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

    public static List<Account> AccountsList { get; set; }
		#endregion Properties 

		#region Methods (4) 


		// Public Methods (2) 

    public static void LoadSettings()
    {
        ConfigurationSettings.LoadConfig();

        AccountsList = new List<Account>();
        Account LegacySettingsAccount = new Account();
        try
        {
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

            foreach (Account a in ConfigurationSettings.Accounts)
            {
                AccountsList.Add(a);
            }
        }
        catch{}
        
    }

    public static void SaveSettings()
    {
        ConfigurationSettings.AppSettings["CheckVersion"] = CheckVersion.ToString() ;
        ConfigurationSettings.AppSettings["BeepOnNew"] = BeepOnNew.ToString();
        ConfigurationSettings.AppSettings["AnimationInterval"] = AnimationInterval.ToString();
        ConfigurationSettings.AppSettings["UpdateInterval"] = UpdateInterval.ToString();
        ConfigurationSettings.AppSettings["MaxTweets"] = MaxTweets.ToString();
        ConfigurationSettings.AppSettings["ShowReplyImages"] = ShowReplyImages.ToString();
        ConfigurationSettings.Accounts = AccountsList;
        ConfigurationSettings.SaveConfig();
    }



		// Private Methods (2) 

    private static int GetTextSize()
    {
        using (System.Drawing.Bitmap b = new System.Drawing.Bitmap(100, 100))
        {
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
            {
                return (int)(g.MeasureString("H", new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 9, System.Drawing.FontStyle.Regular)).Height - 1);
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


                    System.Drawing.Color ColorChosen = System.Drawing.Color.FromArgb(int.Parse(ColorPair[1]), int.Parse(ColorPair[1]), int.Parse(ColorPair[1]));

                    FieldInfo fi = typeof(ClientSettings).GetField(ColorType);
                    fi.SetValue(null, ColorChosen);
                }
            }

        }
    }


		#endregion Methods 

}
