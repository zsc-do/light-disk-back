using lightDiskBack.Controllers.login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lightDiskBack.Dto
{
    public class GithubUser
    {
        public int id { get; set; }
        public string login { get; set; }
        public string avatar_url { get; set; }
}
}
