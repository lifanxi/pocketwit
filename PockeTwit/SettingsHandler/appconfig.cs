using System;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Reflection;

public class ConfigurationSettings
{

		#region Fields (1) 

    public static NameValueCollection AppSettings;
    public static System.Collections.Generic.List<Yedda.Twitter.Account> Accounts;
		#endregion Fields 

		#region Methods (2) 


		// Public Methods (2) 

    public static void LoadConfig()
    {
        try
        {

            string ConfigFile = Path.Combine(ClientSettings.AppPath, "App.config");

            XmlDocument oXml = new XmlDocument();
            AppSettings = new NameValueCollection();
            Accounts = new System.Collections.Generic.List<Yedda.Twitter.Account>();

            if (File.Exists(ConfigFile))
            {
                oXml.Load(ConfigFile);
                XmlNodeList oList = oXml.SelectNodes("//appSettings/add");
                foreach (XmlNode oNode in oList)
                {
                    AppSettings.Add(oNode.Attributes["key"].Value, oNode.Attributes["value"].Value);
                }

                oList = oXml.SelectNodes("//accounts/add");
                foreach (XmlNode oNode in oList)
                {
                    Yedda.Twitter.Account a = new Yedda.Twitter.Account();
                    a.UserName = oNode.Attributes["user"].Value;
                    a.Password = oNode.Attributes["password"].Value;
                    try
                    {
                        a.OAuth_token = oNode.Attributes["oauth_token"].Value;
                        a.OAuth_token_secret = oNode.Attributes["oauth_token_secret"].Value;
                    }
                    catch (Exception) { }

                    try
                    {
                        a.OAuth_token_secure = oNode.Attributes["oauth_token_secure"].Value;
                        a.OAuth_token_secret_secure = oNode.Attributes["oauth_token_secret_secure"].Value;
                    }
                    catch (Exception) { }

                    //if tokens are not saved secure yet, create them
                    if (string.IsNullOrEmpty(a.OAuth_token_secure))
                    {
                        a.OAuth_token_secure = ICSettings.Encryption.Encrypt(a.OAuth_token);
                        a.OAuth_token_secret_secure = ICSettings.Encryption.Encrypt(a.OAuth_token_secret);
                    }
                    //if tokens aren't read from config (which is good), create tokens from secrets.
                    if (string.IsNullOrEmpty(a.OAuth_token))
                    {
                        a.OAuth_token = ICSettings.Encryption.Decrypt(a.OAuth_token_secure);
                        a.OAuth_token_secret = ICSettings.Encryption.Decrypt(a.OAuth_token_secret_secure);
                    }


                    if (oNode.Attributes["servername"] != null)
                    {
                        string ServerName = oNode.Attributes["servername"].Value;
                        a.ServerURL = Yedda.Servers.ServerList[ServerName];
                    }
                    if (oNode.Attributes["server"] != null)
                    {
                        a.Server = (Yedda.Twitter.TwitterServer)Enum.Parse(typeof(Yedda.Twitter.TwitterServer), oNode.Attributes["server"].Value, true);
                    }
                    a.Enabled = bool.Parse(oNode.Attributes["enabled"].Value);
                    Accounts.Add(a);
                }
            }
        }
        catch
        {
        }
    }

    public static void SaveConfig()
    {
        try
        {
            string ConfigFile = Path.Combine(ClientSettings.AppPath, "App.config");

            XmlDocument oXml = new XmlDocument();
            XmlNode Root = oXml.CreateNode(XmlNodeType.Element, "configuration", "");
            XmlNode SettingsNode = oXml.CreateNode(XmlNodeType.Element, "appSettings", "");
            foreach(string AppSettingName in AppSettings.Keys)
            {
                XmlNode SettingNode = oXml.CreateNode(XmlNodeType.Element, "add", "");
                XmlAttribute keyatt = oXml.CreateAttribute("key");
                keyatt.Value = AppSettingName;

                XmlAttribute valueatt = oXml.CreateAttribute("value");
                valueatt.Value = AppSettings[AppSettingName];
                SettingNode.Attributes.Append(keyatt);
                SettingNode.Attributes.Append(valueatt);
                SettingsNode.AppendChild(SettingNode);
            }

            XmlNode AccountsNode = oXml.CreateNode(XmlNodeType.Element, "accounts", "");
            foreach (Yedda.Twitter.Account Account in Accounts)
            {
                XmlNode AccountNode = oXml.CreateNode(XmlNodeType.Element, "add", "");
                XmlAttribute userAtt = oXml.CreateAttribute("user");
                userAtt.Value = Account.UserName;

                XmlAttribute passAtt = oXml.CreateAttribute("password");
                passAtt.Value = Account.Password;

                XmlAttribute oauthTokenAtt = oXml.CreateAttribute("oauth_token");
                oauthTokenAtt.Value = string.Empty;

                XmlAttribute oauthTokenSecretAtt = oXml.CreateAttribute("oauth_token_secret");
                oauthTokenSecretAtt.Value = string.Empty;

                XmlAttribute oauthTokenSecureAtt = oXml.CreateAttribute("oauth_token_secure");
                oauthTokenSecureAtt.Value = ICSettings.Encryption.Encrypt(Account.OAuth_token);

                XmlAttribute oauthTokenSecretSecureAtt = oXml.CreateAttribute("oauth_token_secret_secure");
                oauthTokenSecretSecureAtt.Value = ICSettings.Encryption.Encrypt(Account.OAuth_token_secret);

                XmlAttribute serverNameAtt = oXml.CreateAttribute("servername");
                serverNameAtt.Value = Account.ServerURL.Name;

                XmlAttribute enabledAtt = oXml.CreateAttribute("enabled");
                enabledAtt.Value = Account.Enabled.ToString();

                AccountNode.Attributes.Append(userAtt);
                AccountNode.Attributes.Append(passAtt);
                AccountNode.Attributes.Append(oauthTokenAtt);
                AccountNode.Attributes.Append(oauthTokenSecretAtt);
                AccountNode.Attributes.Append(oauthTokenSecureAtt);
                AccountNode.Attributes.Append(oauthTokenSecretSecureAtt);
                AccountNode.Attributes.Append(serverNameAtt);
                AccountNode.Attributes.Append(enabledAtt);
                AccountsNode.AppendChild(AccountNode);
            }

            Root.AppendChild(SettingsNode);
            Root.AppendChild(AccountsNode);
            oXml.AppendChild(Root);

            oXml.Save(ConfigFile);
        }
        catch(Exception ex) 
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }


		#endregion Methods 

}