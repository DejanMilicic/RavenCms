﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents.Indexes;
using RavenCms.Models;

namespace RavenCms.Raven.Indexes
{
    public class Products_ByEmployee : AbstractIndexCreationTask<Order, Products_ByEmployee.IndexEntry>
    {
        public class IndexEntry
        {
            public string Employee { get; set; }

            public string Product { get; set; }
        }

        public Products_ByEmployee()
        {
            Map = orders => from order in orders
                from orderLine in order.Lines
                select new
                {
                    Employee = order.Employee,
                    Product = orderLine.Product
                };
        }
    }
}