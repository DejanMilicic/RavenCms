using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RavenCms.Models
{
    public class EntriesPage
    {
        public Entry[] Entries { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int Total { get; set; }
    }
}
