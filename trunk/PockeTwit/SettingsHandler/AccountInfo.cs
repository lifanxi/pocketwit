using System;

using System.Collections.Generic;
using System.Text;

namespace PockeTwit.SettingsHandler
{
    struct AccountInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Yedda.Twitter.TwitterServer Service { get; set; }
    }
}
