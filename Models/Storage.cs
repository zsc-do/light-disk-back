using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace lightDiskBack.Models
{
    public class Storage
    {
        public int storageId { get; set; }

        public string storageSize { get; set; }

        public string simpleSize { get; set; }

        public string memberSize { get; set; }

        public int userId { get; set; }

        public string isMember { get; set; }

        public SysUser SysUser { get; set; }

    }
}
