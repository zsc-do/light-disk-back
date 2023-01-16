using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lightDiskBack.Models
{
    public class Share
    {

        public int shareId { get; set; }

        public DateTime endTime { get; set; }


        public string extractionCode { get; set; }

        public string shareBatchNum { get; set; }

        public string shareStatus { get; set; }

        public int userId { get; set; }


    }
}
