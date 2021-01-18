using System.Linq;
using Raven.Client.Documents.Indexes;
using RavenCms.Models;

namespace RavenCms.Raven.Indexes
{
    public class Products_ByEmployee : AbstractIndexCreationTask<Order, Products_ByEmployee.IndexEntry>
    {
        public class IndexEntry
        {
            public string Id { get; set; }

            public string Employee { get; set; }

            public string Product { get; set; }
        }

        public Products_ByEmployee()
        {
            Map = orders => from order in orders
                from orderLine in order.Lines
                select new IndexEntry
                {
                    Id = order.Id,
                    Employee = order.Employee,
                    Product = orderLine.Product
                };

            Stores.Add(x => x.Product, FieldStorage.Yes);
        }
    }
}
