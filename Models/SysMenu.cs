using System;
using System.Collections.Generic;
using System.Collections;
using lightDiskBack.Models;

namespace lightDiskBack.Models
{
    public class SysMenu
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int? ParentId { get; set; }
        public string MenuUrl { get; set; }
        public string MenuType { get; set; }
        public string MenuPerms { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string DelFlag { get; set; }

        public ArrayList ChildMenu{ get; set; }

        public List<SysRole> SysRoles { get; set; } = new List<SysRole>();

    }
}
