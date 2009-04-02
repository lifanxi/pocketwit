using System;

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PockeTwit
{
    public enum GroupingType
    {
        user=0,
        searchterm=1
    }
    public struct GroupingItem
    {
        public GroupingType GroupType { get; set; }
        public string Term;
    }
    public class SpecialTimeLine
    {
        public string name { get; set; }
        public GroupingItem[] Terms { get; set; }

        public void AddItem(GroupingItem Term)
        {
            List<GroupingItem> items = new List<GroupingItem>(Terms);
            if (!items.Contains(Term))
            {
                items.Add(Term);
            }
            Terms = items.ToArray();
        }

        public string GetConstraints()
        {
            string ret = "";
            List<string> UserList = new List<string>();
            foreach (GroupingItem t in Terms)
            {
                switch (t.GroupType)
                {
                    case GroupingType.user:
                        UserList.Add("'"+t.Term+"'");
                        break;
                }
            }
            if (UserList.Count > 0)
            {
                ret = " AND users.screenname IN(" + string.Join(",", UserList.ToArray()) + ") ";
            }

            return ret;
        }
    }

    public static class SpecialTimeLines
    {
        private static string configPath = ClientSettings.AppPath + "\\savedTimelines.xml";
        private static List<SpecialTimeLine> _Items = new List<SpecialTimeLine>();

        public static SpecialTimeLine[] GetList()
        {
            return _Items.ToArray();
        }
        public static void Add(SpecialTimeLine newLine)
        {
            lock (_Items)
            {
                _Items.Add(newLine);
            }
        }
        public static void Remove(SpecialTimeLine oldLine)
        {
            lock (_Items)
            {
                _Items.Remove(oldLine);
            }
        }
        public static void Clear()
        {
            lock (_Items)
            {
                _Items.Clear();
            }
        }

        public static void Load()
        {
            if(!System.IO.File.Exists(configPath)){return;}
            XmlSerializer s = new XmlSerializer(typeof(List<SpecialTimeLine>));
            lock (_Items)
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(configPath))
                {
                    _Items = (List<SpecialTimeLine>)s.Deserialize(r);
                }
            }
        }
        public static void Save()
        {
            XmlSerializer s = new XmlSerializer(typeof(List<SpecialTimeLine>));
            lock (_Items)
            {
                using (System.IO.StreamWriter w = new System.IO.StreamWriter(configPath))
                {
                    s.Serialize(w, _Items);
                }
            }
        }
    }
}
