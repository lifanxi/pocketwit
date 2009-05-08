using System;
using System.Collections.Generic;
using PockeTwit.SpecialTimelines;

namespace PockeTwit
{
    [Serializable]
    public class SpecialTimeLine
    {
        [Serializable]
        public class groupTerm
        {
            public string Term;
            public string Name;
            public bool Exclusive;
        }
        
        public string ListName
        {
            get
            {
                return "Grouped_TimeLine_" + name;
            }
        }

        public string name { get; set; }
        public groupTerm[] Terms { get; set; }

        
        public void AddItem(string Term, string ScreenName, bool Exclusive)
        {
            groupTerm newTerm = new groupTerm() { Term = Term, Name = ScreenName, Exclusive = Exclusive };
            if (Terms != null && Terms.Length > 0)
            {
                List<groupTerm> items = new List<groupTerm>(Terms);
                if (!items.Contains(newTerm))
                {
                    items.Add(newTerm);
                }
                Terms = items.ToArray();
            }
            else
            {
                Terms = new groupTerm[] { newTerm };
            }
        }
        public void RemoveItem(string Term)
        {
            List<groupTerm> items = new List<groupTerm>(Terms);
            groupTerm toRemove = new groupTerm();
            foreach (groupTerm t in items)
            {
                if (t.Term == Term)
                {
                    toRemove = t;
                }
            }
            if (items.Contains(toRemove))
            {
                items.Remove(toRemove);
            }
            Terms = items.ToArray();
            if(Terms.Length==0)
            {
                SpecialTimeLinesRepository.Remove(this);
            }
            SpecialTimeLinesRepository.Save();
        }
        

        public string GetConstraints()
        {
            if (Terms == null) 
            {
                SpecialTimeLinesRepository.Load();
            }
            if (Terms == null) 
            {
                return "";
            }
            string ret = "";
            List<string> UserList = new List<string>();
            foreach (groupTerm t in Terms)
            {
                UserList.Add("'"+t.Term+"'");
                
            }
            if (UserList.Count > 0)
            {
                ret = " AND statuses.userid IN(" + string.Join(",", UserList.ToArray()) + ") ";
            }

            return ret;
        }

        public override string ToString()
        {
            return name;
        }


    }
}