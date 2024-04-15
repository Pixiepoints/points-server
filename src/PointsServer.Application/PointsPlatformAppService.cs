using PointsServer.Localization;
using Volo.Abp.Application.Services;

namespace PointsServer;

public class PointsPlatformAppService : ApplicationService
{
    protected PointsPlatformAppService()
    {
        LocalizationResource = typeof(PointsServerResource);
    }
}