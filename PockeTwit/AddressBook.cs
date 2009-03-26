using System;

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

        public static int Count()
        {
            return _Names.Count;
        }

        public static void AddName(string Name)
        {
            string nameToAdd = Name.ToLower();
            lock(_Names){
                if (!_Names.Contains(nameToAdd))
                {
                    _Names.Add(nameToAdd);
                }
            }
        }

        public static void Load()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Load));
        }

        private static void Load(object o)
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
        //Has to match System.Threading.WaitCallback to be called from threadpool
        public static void Save(object o)
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
            try
            {
                d.Save(location);
            }
            catch
            {
                //If it fails it's most likely because it's already open by another thread
            }
        }

        public static void Clear()
        {
            lock (_Names)
            {
                _Names.Clear();
            }
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Save));
        }
        
    }
}
