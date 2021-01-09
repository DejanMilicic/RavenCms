using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RavenCms.Models
{
    public class Employee
    {
        public string Id { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public List<string> Notes { get; set; }
        
        public string ReportsTo { get; set; }
        
        public string Title { get; set; }
    }
}
