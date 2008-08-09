using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    public class SideMenuFunctions
    {
        public static List<string> ReplaceMenuItem(List<string> List, string Original, string New)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i] == Original)
                {
                    List[i] = New;
                }
            }
            return List;
        }
    }
}
