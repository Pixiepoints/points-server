using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace PointsServer;

[Dependency(ReplaceServices = true)]
public class PointsServerBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "PointsServer";
}