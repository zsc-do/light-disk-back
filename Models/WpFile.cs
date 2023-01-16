using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace lightDiskBack.Models
{
    public class WpFile
    {
        public int fileId { get; set; }

        public String fileName { get; set; }


        public String isFolder { get; set; }


        public string filePath { get; set; }

        public String delFlag{ get; set; }

        public int userId { get; set; }

        public SysUser SysUser { get; set; }


        public int diskFileId { get; set; }

        public DiskFile diskFile { get; set; }


        public List<WpFile> ChildFolder { get; set; } = new List<WpFile>();
    }
}
