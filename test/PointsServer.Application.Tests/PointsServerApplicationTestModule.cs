using Microsoft.Extensions.DependencyInjection;
using Moq;
using PointsServer.EntityEventHandler.Core;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.MongoDB;
using Volo.Abp.AutoMapper;
using Volo.Abp.EventBus;
using Volo.Abp.Identity;
using Volo.Abp.Identity.MongoDB;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace PointsServer;

[DependsOn(
    typeof(AbpEventBusModule),
    typeof(PointsServerApplicationModule),
    typeof(PointsServerApplicationContractsModule),
    typeof(PointsServerOrleansTestBaseModule),
    typeof(PointsServerDomainTestModule)
)]
public class PointsServerApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<PointsServerApplicationModule>(); });
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<PointsServerEntityEventHandlerCoreModule>(); });

        context.Services.AddSingleton(new Mock<IMongoDbContextProvider<IAuditLoggingMongoDbContext>>().Object);
        context.Services.AddSingleton<IAuditLogRepository, MongoAuditLogRepository>();
        context.Services.AddSingleton<IIdentityUserRepository, MongoIdentityUserRepository>();
    }
}