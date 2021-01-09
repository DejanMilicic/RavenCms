using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RavenCms.Models
{
    public class Order
    {
        public string Id { get; set; }

        public string Company { get; set; }

        public string Employee { get; set; }
        
        public float Freight { get; set; }
        
        public List<Line> Lines { get; set; }
        
        public DateTimeOffset OrderedAt { get; set; }
        
        public DateTimeOffset RequireAt { get; set; }
        
        public string ShipVia { get; set; }


        public class Line
        {
            public float Discount { get; set; }

            public float PricePerUnit { get; set; }
            
            public string Product { get; set; }
            
            public string ProductName { get; set; }
            
            public int Quantity { get; set; }
        }
    }
}
