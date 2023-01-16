using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lightDiskBack.Models
{
    public class SysRole : IdentityRole<int>
    {

        public List<SysUser> User { get; set; } = new List<SysUser>();

        public List<SysMenu> SysMenus { get; set; } = new List<SysMenu>();

    }
}
