using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace PointsServer.EntityEventHandler.Core
{
    [DependsOn(typeof(AbpAutoMapperModule),
        typeof(PointsServerApplicationModule),
        typeof(PointsServerApplicationContractsModule))]
    public class PointsServerEntityEventHandlerCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<PointsServerEntityEventHandlerCoreModule>();
            });
        }
    }
}