using PointsServer.Localization;
using Volo.Abp.Application.Services;

namespace PointsServer;

/* Inherit your application services from this class.
 */
public abstract class PointsServerAppService : ApplicationService
{
    protected PointsServerAppService()
    {
        LocalizationResource = typeof(PointsServerResource);
    }
}
