using Microsoft.Extensions.DependencyInjection;
using PointsServer.Grains;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace PointsServer.Silo;
[DependsOn(typeof(AbpAutofacModule),
    typeof(PointsServerGrainsModule),
    typeof(AbpAspNetCoreSerilogModule)
)]
public class PointsServerOrleansSiloModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHostedService<PointsServerHostedService>();
    }
}