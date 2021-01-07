using System;
using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace RavenCms.LoadTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            var pingPluginConfig = PingPluginConfig.CreateDefault(new[] { "RavenCms" });
            var pingPlugin = new PingPlugin(pingPluginConfig);

            NBomberRunner
                .RegisterScenarios(GetEntriesByTag())
                .WithWorkerPlugins(pingPlugin)
                .Run();
        }

        public static Scenario GetEntriesByTag()
        {
            var data = FeedData.FromJson<string>("tags.json");
            var tagFeed = Feed.CreateCircular("tagFeed", provider: data);

            var step = HttpStep.Create("step", tagFeed, context =>
            {
                string url = "http://localhost:5000/" + context.FeedItem;

                context.Logger.Debug("Data from feed: {FeedItem}", context.FeedItem);

                return Http.CreateRequest("GET", url)
                    .WithCheck(async response =>
                        response.IsSuccessStatusCode
                            ? Response.Ok()
                            : Response.Fail()
                    );
            });

            var scenario = ScenarioBuilder
                .CreateScenario("GetEntriesByTag", step)
                .WithoutWarmUp()
                .WithLoadSimulations(new[]
                {
                    Simulation
                        .InjectPerSec(rate: 1_000, during: TimeSpan.FromSeconds(20))
                });

            return scenario;
        }
    }
}
