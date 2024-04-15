using PointsServer.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace PointsServer.Controllers;

public abstract class PointsServerController : AbpControllerBase
{
    protected PointsServerController()
    {
        LocalizationResource = typeof(PointsServerResource);
    }
}