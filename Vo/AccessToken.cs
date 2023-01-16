using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lightDiskBack.Vo
{
    public class AccessToken
    {
        public string access_token { get; set; }

        public string scope { get; set; }
        public string token_type { get; set; }
    }
}
