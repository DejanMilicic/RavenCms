using System.Linq;
using Raven.Client.Documents.Indexes;
using RavenCms.Models;

namespace RavenCms.Raven.Indexes
{
    public class Entries_ByTag : AbstractIndexCreationTask<Entry>
    {
        public class Result
        {
            public string[] Tag { get; set; }
        }

        public Entries_ByTag()
        {
            Map = entries => entries
                .Select(entry => new Result
                {
                    Tag = entry.Tags.ToArray(),
                });
        }
    }
}
