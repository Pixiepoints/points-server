using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace PointsServer.ContractEventHandler.Core
{
    [DependsOn(
        typeof(AbpAutoMapperModule)
    )]
    public class PointsServerContractEventHandlerCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<PointsServerContractEventHandlerCoreModule>();
            });
        }
    }
}