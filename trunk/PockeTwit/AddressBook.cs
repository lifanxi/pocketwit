﻿using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    static class AddressBook
    {
        private static string location = ClientSettings.AppPath + "\\AddressBook.xml";
        private static List<string> _Names = new List<string>();

        static AddressBook()
        {
            Load();
        }

        public static string[] GetList()
        {
            return _Names.ToArray();
        }

        public static string[] GetList(string startsWith)
        {
            return _Names.FindAll(delegate(string s)
            {
                return s.StartsWith(startsWith);
            }).ToArray();
        }

        public static void AddName(string Name)
        {
            lock(_Names){
                if (!_Names.Contains(Name))
                {
                    _Names.Add(Name);
                    _Names.Sort();
                    Save();
                }
            }
        }

        private static void Load()
        {
            if (System.IO.File.Exists(location))
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(location))
                {
                    string Addresses = r.ReadToEnd();
                    if (!string.IsNullOrEmpty(Addresses))
                    {
                        System.Xml.XmlDocument d = new System.Xml.XmlDocument();
                        d.LoadXml(Addresses);

                        System.Xml.XmlNodeList l = d.SelectNodes("//name");
                        foreach (System.Xml.XmlNode n in l)
                        {
                            if (!_Names.Contains(n.InnerText))
                            {
                                _Names.Add(n.InnerText);
                            }
                        }
                    }
                }
            }
        }
        private static void Save()
        {
            System.Xml.XmlDocument d = new System.Xml.XmlDocument();
            System.Xml.XmlElement root = d.CreateElement("usernames");
            d.AppendChild(root);

            foreach (string ID in _Names)
            {
                System.Xml.XmlElement idElement = d.CreateElement("name");
                idElement.InnerText = ID;
                root.AppendChild(idElement);
            }

            d.Save(location);
        }

        
    }
}
