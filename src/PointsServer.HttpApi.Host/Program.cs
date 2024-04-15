using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PointsServer.Extension;
using Serilog;

namespace PointsServer
{
    public class Program
    {
        public async static Task<int> Main(string[] args)
        {
            ThreadPool.SetMinThreads(300, 300);
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Starting PointsServer.HttpApi.Host");

                var builder = WebApplication.CreateBuilder(args);
                builder.Configuration.AddJsonFile("apollo.appsettings.json");
                builder.Host.AddAppSettingsSecretsJson()
                    .UseApollo()
                    .UseAutofac()
                    .UseSerilog();

                await builder.AddApplicationAsync<PointsServerHttpApiHostModule>();
                var app = builder.Build();
                await app.InitializeApplicationAsync();
                await app.RunAsync();
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
    }
}