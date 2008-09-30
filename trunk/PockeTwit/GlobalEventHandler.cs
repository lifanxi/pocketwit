using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit
{
    //Since imagebuffers are part of an account and there is no good way to bubble events to the UI,
    //  this static class allows me to create the events globally and subscribe in the UI.
    static class GlobalEventHandler
    {
        // Delegates (1) 

        public delegate void ArtWasUpdated(string User);
        public delegate void delAvatarHasChanged(string User, string NewURL);
        public delegate void delNoData(Yedda.Twitter.Account t, Yedda.Twitter.ActionType Action);

        // Events (1) 

        public static event ArtWasUpdated Updated;
        public static event delAvatarHasChanged AvatarHasChanged;
        public static event delNoData NoData;

        public static void CallArtWasUpdated(string User)
        {
            if (Updated != null)
            {
                Updated(User);
            }
        }
        public static void CallAvatarHasChanged(string User, string NewURL)
        {
            if (AvatarHasChanged != null)
            {
                AvatarHasChanged(User, NewURL);
            }
        }
        
    }
}
