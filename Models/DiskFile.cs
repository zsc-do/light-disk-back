using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lightDiskBack.Models
{
    public class DiskFile
    {

        public int diskFileId { get; set; }


        public String diskFileSize { get; set; }


        public String diskFileUrl { get; set; }

        public String diskFileType { get; set; }

        public List<WpFile> wpFileList { get; set; } = new List<WpFile>();


    }
}
