using System;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Reflection;

public class ConfigurationSettings
{

		#region Fields (1) 

    public static NameValueCollection AppSettings;

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

            if (File.Exists(ConfigFile))
            {
                oXml.Load(ConfigFile);
                XmlNodeList oList = oXml.SelectNodes("//appSettings/add");
                foreach (XmlNode oNode in oList)
                {
                    AppSettings.Add(oNode.Attributes["key"].Value, oNode.Attributes["value"].Value);
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


            Root.AppendChild(SettingsNode);
            oXml.AppendChild(Root);

            oXml.Save(ConfigFile);
        }
        catch(Exception ex) 
        {
        }
    }


		#endregion Methods 

}