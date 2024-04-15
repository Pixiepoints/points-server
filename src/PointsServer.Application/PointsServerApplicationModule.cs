using Microsoft.Extensions.DependencyInjection;
using PointsServer.DApps;
using PointsServer.Grains;
using PointsServer.Options;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace PointsServer;

[DependsOn(
    typeof(PointsServerDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(PointsServerApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(PointsServerGrainsModule),
    typeof(AbpSettingManagementApplicationModule)
)]
public class PointsServerApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<PointsServerApplicationModule>(); });

        var configuration = context.Services.GetConfiguration();
        context.Services.AddSingleton<IDAppService, DAppService>();

        Configure<DappOption>(configuration.GetSection("Dapp"));
        Configure<PointsRulesOption>(configuration.GetSection("PointsRules"));
        Configure<ApplyConfirmOptions>(configuration.GetSection("ApplyConfirm"));
        Configure<InternalWhiteListOptions>(configuration.GetSection("InternalWhiteList"));
        context.Services.AddHttpClient();
    }
}