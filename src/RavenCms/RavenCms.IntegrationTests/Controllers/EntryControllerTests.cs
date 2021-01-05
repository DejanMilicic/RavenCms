using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RavenCms.IntegrationTests.Infrastructure;
using RavenCms.Models;
using Xunit;

namespace RavenCms.IntegrationTests.Controllers
{
    public class EntryControllerTests : Fixture
    {
        public EntryControllerTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        private class CreateEntryModel
        {
            public CreateEntryModel(string[] tags)
            {
                Tags = tags;
            }

            public string[] Tags { get; }
        }

        [Fact]
        public async Task EntryPost_CreatesOneEntry_WithExpectedContent()
        {
            var newEntryModel = new CreateEntryModel(new [] { "tag1", "tag2", "tag3" });
            var newEntryModelContent = JsonSerializer.Serialize(newEntryModel);

            var stringContent = new StringContent(newEntryModelContent, Encoding.UTF8, MediaTypeNames.Application.Json); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+
            var result = HttpClient.PostAsync("/entry", stringContent).Result;

            var session = Store.OpenSession();

            WaitForIndexing(Store);
            WaitForUserToContinueTheTest(Store);

            List<Entry> entries = session.Query<Entry>().ToList();

            entries.Count.Should().Be(1);

            var entry = entries.Single();
            entry.Tags.Should().BeEquivalentTo(newEntryModel.Tags);
        }
    }
}
