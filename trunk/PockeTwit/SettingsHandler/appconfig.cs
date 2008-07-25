using System;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Reflection;

public class ConfigurationSettings
{

    public static NameValueCollection AppSettings;

    public static void LoadConfig()
    {
        try
        {

            string AppPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "SettingsHandler");
            string ConfigFile = Path.Combine(AppPath, "App.config");

            XmlDocument oXml = new XmlDocument();

            if (File.Exists(ConfigFile))
            {
                oXml.Load(ConfigFile);
            }
            else
            {
                oXml.Load(Path.Combine(AppPath, "defaultapp.config"));
            }


            //XmlNodeList oList = oXml.GetElementsByTagName("appSettings");
            XmlNodeList oList = oXml.SelectNodes("//appSettings/add");

            AppSettings = new NameValueCollection();

            foreach (XmlNode oNode in oList)
            {
                AppSettings.Add(oNode.Attributes["key"].Value, oNode.Attributes["value"].Value);
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

            string AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string ConfigFile = Path.Combine(AppPath, "App.config");

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


            Root.AppendChild(SettingsNode);
            oXml.AppendChild(Root);

            /*
            if (File.Exists(ConfigFile))
            {
                oXml.Load(ConfigFile);
            }
            else
            {
                oXml.Load(Path.Combine(AppPath, "defaultapp.config"));
            }

            XmlNodeList oList = oXml.GetElementsByTagName("appSettings");

            foreach (XmlNode oNode in oList)
            {
                foreach (XmlNode oKey in oNode.ChildNodes)
                {
                    string Key = oKey.Attributes["key"].Value;
                    string Setting = AppSettings[Key];
                    oKey.Attributes["value"].Value = Setting;
                }
            }
            */
            oXml.Save(ConfigFile);
        }
        catch(Exception ex) 
        {
        }
    }

}