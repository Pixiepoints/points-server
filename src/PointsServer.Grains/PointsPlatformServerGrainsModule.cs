using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace PointsServer.Grains;

[DependsOn(
    typeof(AbpAutoMapperModule),typeof(PointsServerApplicationContractsModule))]
public class PointsServerGrainsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<PointsServerGrainsModule>(); });
    }
}