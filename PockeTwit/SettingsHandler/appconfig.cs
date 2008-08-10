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
                    a.Server = (Yedda.Twitter.TwitterServer)Enum.Parse(typeof(Yedda.Twitter.TwitterServer), oNode.Attributes["server"].Value, true);
                    Accounts.Add(a);
                }
            }
        }
        catch (Exception ex)
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

                XmlAttribute serverAtt = oXml.CreateAttribute("server");
                serverAtt.Value = Account.Server.ToString();

                AccountNode.Attributes.Append(userAtt);
                AccountNode.Attributes.Append(passAtt);
                AccountNode.Attributes.Append(serverAtt);
                AccountsNode.AppendChild(AccountNode);
            }

            Root.AppendChild(SettingsNode);
            Root.AppendChild(AccountsNode);
            oXml.AppendChild(Root);

            oXml.Save(ConfigFile);
        }
        catch(Exception ex) 
        {
        }
    }


		#endregion Methods 

}