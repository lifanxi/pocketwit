using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    public class SideMenuFunctions
    {

		#region Methods (1) 


		// Public Methods (1) 

        public static SafeList<string> ReplaceMenuItem(SafeList<string> List, string Original, string New)
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


		#endregion Methods 

    }
}
