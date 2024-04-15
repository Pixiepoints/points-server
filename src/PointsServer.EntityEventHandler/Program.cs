using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PointsServer.EntityEventHandler.Extensions;
using Serilog;

namespace PointsServer.EntityEventHandler
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Starting PointsServer.EntityEventHandler.");
                await CreateHostBuilder(args).RunConsoleAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(build => { build.AddJsonFile("appsettings.secrets.json", optional: true); })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplication<PointsServerEntityEventHandlerModule>();
                })
                .ConfigureAppConfiguration((h, c) => c.AddJsonFile("apollo.appsettings.json"))
                .UseApollo()
                .UseAutofac()
                .UseSerilog();
    }
}