using PointsServer.MongoDB;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace PointsServer.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(PointsServerMongoDbModule),
    typeof(PointsServerApplicationContractsModule)
    )]
public class PointsServerDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
