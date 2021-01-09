using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.BulkInsert;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using RavenCms.Infrastructure;
using RavenCms.Models;
using RavenCms.Raven.Indexes;

namespace RavenCms.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IDocumentStore _store;

        public DemoController(IAsyncDocumentSession session, IDocumentStore store)
        {
            _session = session;
            _store = store;
        }

        [HttpGet("/products/")]
        public async Task<List<ProductForEmployee>> GetProducts(string employee)
        {
            var orders = await _session
                .Query<Products_ByEmployee.IndexEntry, Products_ByEmployee>()
                .Where(x => x.Employee == employee)
                .OfType<Order>()
                .ToListAsync()
                ;

            return orders.SelectMany(o => o.Lines, (order, line) =>
                new ProductForEmployee
                {
                    OrderId = order.Id,
                    ProductId = line.Product,
                    ProductName = line.ProductName

                }).ToList();
        }
    }

    public class ProductForEmployee
    {
        public string OrderId { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }
    }
}
