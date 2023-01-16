using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lightDiskBack.Models
{
    public class SysUser : IdentityUser<int>
    {
        public List<SysRole> Role { get; set; } = new List<SysRole>();

        public List<WpFile> wpFileList { get; set; } = new List<WpFile>();


        public Storage storage { get; set; }

        public string githubId { get; set; }


    }
}
