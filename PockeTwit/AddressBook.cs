using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    static class AddressBook
    {
        private static List<string> _Names = new List<string>();

        public static string[] GetList()
        {
            return _Names.ToArray();
        }

        public static void AddName(string Name)
        {
            lock(_Names){
                if (!_Names.Contains(Name))
                {
                    _Names.Add(Name);
                }
            }
        }
    }
}
