using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SystemEnvironment = System.Environment;
using Serilog;

namespace ATAFurniture.Server;

public class Program
{
    private static readonly string Environment
        = SystemEnvironment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    
    public static IConfiguration Configuration { get; private set; } = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();
    
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();
        
        try
        {
            Log.Information("Starting...");

            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseConfiguration(Configuration)
                    .UseSentry();
                webBuilder.UseStartup<Startup>();
            });
}