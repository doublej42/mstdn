using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using Mastonet;
using jsonDb;

namespace mstdn
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Logger.Debug("Logging Started");
            //https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host
            using IHost host = CreateHostBuilder(args)
                .UseSerilog()
                .Build();

            // Application code should start here.
            var runner = ActivatorUtilities.CreateInstance<MstdnRunner>(host.Services);
            runner.RunAsync().Wait();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<JsonDb<JsonDbStruct>>();
                //services.AddTransient<HtmlProcessor>();
                //services.AddHostedService<SiteScannerService>();
            });


        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}


