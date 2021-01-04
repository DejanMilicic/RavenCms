using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.BulkInsert;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using RavenCms.Infrastructure;
using RavenCms.Models;
using RavenCms.Raven.Indexes;

namespace RavenCms.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntryController : ControllerBase
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IDocumentStore _store;

        public EntryController(IAsyncDocumentSession session, IDocumentStore store)
        {
            _session = session;
            _store = store;
        }

        [HttpGet("/seed")]
        public async Task<string> Seed()
        {
            DetailedDatabaseStatistics stats = _store.Maintenance.Send(new GetDetailedStatisticsOperation());
            if (stats.CountOfDocuments > 0)
                return "Database is already seeded";

            Faker<Entry> generator = new Faker<Entry>()
                .StrictMode(true)
                .Ignore(e => e.Id)
                .RuleFor(e => e.Tags, f => Helper.GetRandomTags());

            List<Entry> entries = generator.Generate(20);

            await using BulkInsertOperation bulkInsert = _store.BulkInsert();
            foreach (Entry entry in entries)
            {
                await bulkInsert.StoreAsync(entry);
            }

            return "Database was empty, new data seeded";
        }

        [HttpGet("/{tag}")]
        public async IAsyncEnumerable<Entry> GetAllTaggedWith(string tag)
        {
            var query = _session.Query<Entries_ByTag.Result, Entries_ByTag>()
                .Where(x => x.Tag.ContainsAny(new string[]{tag}))
                .OfType<Entry>();

            await using var stream = await _session.Advanced.StreamAsync(query);

            while (await stream.MoveNextAsync())
                yield return stream.Current.Document;
        }
    }
}
