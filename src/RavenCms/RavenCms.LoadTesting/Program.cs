using System;
using System.Threading.Tasks;
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
            /*
            // first, you need to create a step
            var stepEmpty = Step.Create("step", async context =>
            {
                // you can define and execute any logic here,
                // for example: send http request, SQL query etc
                // NBomber will measure how much time it takes to execute your logic

                await Task.Delay(TimeSpan.FromSeconds(1));
                return Response.Ok();
            });

            // second, we add our step to the scenario
            var scenario = ScenarioBuilder.CreateScenario("hello_world", stepEmpty);

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
            */

            var step = HttpStep.Create("fetch_html_page", context =>
                Http.CreateRequest("GET", "https://nbomber.com")
            );

            var scenario = ScenarioBuilder
                .CreateScenario("nbomber_web_site", step)
                .WithLoadSimulations(new[]
                {
                    Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30))
                });

            var pingPluginConfig = PingPluginConfig.CreateDefault(new[] {"nbomber.com"});
            var pingPlugin = new PingPlugin(pingPluginConfig);

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithWorkerPlugins(pingPlugin)
                .Run();
        }
    }
}
