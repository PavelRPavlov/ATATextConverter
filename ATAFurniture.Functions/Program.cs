using ATAFurniture.Functions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddConfiguration(
            new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<Program>()
            .Build());
    })
    .ConfigureServices((appBuilder, services) =>
    {
        services
            .AddOptions<CosmosDbConfiguration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                var context = configuration.GetSection("CosmosDbConfiguration");
                context.Bind(settings);
            });
    })
    .Build();

host.Run();