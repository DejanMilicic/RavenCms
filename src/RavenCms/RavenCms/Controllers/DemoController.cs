﻿using System.Collections.Generic;
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
        private readonly IDocumentSession _s;
        private readonly IDocumentStore _store;

        public DemoController(IAsyncDocumentSession session, IDocumentStore store, IDocumentSession s)
        {
            _session = session;
            _store = store;
            _s = s;
        }

        [HttpGet("/products/")]
        public async Task<List<ProductForEmployee>> GetProducts(string employee)
        {
            var orders = await _session
                    .Query<Products_ByEmployee.IndexEntry, Products_ByEmployee>()
                    .Where(x => x.Employee == employee)
                    .OfType<Order>()
                    .ToListAsync();

            _s.Load<dynamic>(orders.SelectMany(x => x.Lines).Select(x => x.Product));

            return orders
                .SelectMany(o => o.Lines, (order, line) =>
                {
                    var product = _s.Load<dynamic>(line.Product);
                    int warranty = product.WarrantyLength != null ? product.WarrantyLength : 0;
                    return new ProductForEmployee
                    {
                        OrderId = order.Id,
                        ProductId = line.Product,
                        ProductName = line.ProductName,
                        ProductWarranty = warranty
                    };
                }
            ).ToList();
        }

        [HttpGet("/products/{skip}/{take}")]
        public async Task<List<Products_ByEmployee.IndexEntry>> GetProductsPaged(string employee, int skip, int take)
        {
            return await _session
                .Query<Products_ByEmployee.IndexEntry, Products_ByEmployee>()
                .Where(x => x.Employee == employee)
                .ProjectInto<Products_ByEmployee.IndexEntry>()
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }

    public class ProductForEmployee
    {
        public string OrderId { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public int ProductWarranty { get; set; }
    }
}
