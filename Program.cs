using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steeltoe.Bootstrap.Autoconfig;
using System.Linq;
using System.Threading.Tasks;
using Steeltoe.Extensions.Configuration.Placeholder;

namespace DemoApp
{
    class Program
    {
        public static async Task Main()
        {
            var logFactory = LoggerFactory.Create(c => c.AddSimpleConsole(c =>
            {
                c.SingleLine = true;
            }));
            var logger = logFactory.CreateLogger("DemoApp");

            var host = Host.CreateDefaultBuilder()
                .AddSteeltoe(logFactory)
                .ConfigureServices(services =>
            {
                var svc = services.BuildServiceProvider();
                var config = (IConfigurationRoot)svc.GetRequiredService<IConfiguration>();
                logger.LogInformation($"Config providers: ");
                foreach (var configurationProvider in config.Providers)
                {
                    logger.LogInformation($"  - {configurationProvider}");
                    if(configurationProvider is PlaceholderResolverProvider placeholder)
                    {
                        foreach (var subProvider in placeholder.Providers)
                        {
                            logger.LogInformation($"    - {subProvider}");
                        }
                    }
                }
                logger.LogInformation($"DI Container: ");
                foreach (var registration in services.Where(x => !x.ServiceType.Namespace.StartsWith("Microsoft")).OrderBy(x => x.ServiceType.ToString()))
                {
                    logger.LogInformation($"  - {registration.ServiceType}");
                    var implementations = svc.GetServices(registration.ServiceType);
                    foreach (var impl in implementations)
                    {
                        logger.LogInformation($"    - {impl}");
                    }
                }
            }).Build();

            await Task.Delay(500);



        }
    } 
}