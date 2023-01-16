using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lightDiskBack.Models
{
    public class ShareFile
    {
        public int shareFileId { get; set; }

        public string shareBatchNum { get; set; }

        public string shareFilePath { get; set; }


        public string wpFileName { get; set; }

        public string wpIsFolder { get; set; }


        public int diskFileId { get; set; }


        public List<ShareFile> ChildFolder { get; set; } = new List<ShareFile>();
    }
}
