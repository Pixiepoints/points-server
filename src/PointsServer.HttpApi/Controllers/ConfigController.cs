using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PointsServer.Config;
using Volo.Abp;

namespace PointsServer.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("ConfigController")]
[Route("api/app/config")]
public class ConfigController : PointsServerController
{
    private readonly IConfigAppService _configAppService;

    public ConfigController(IConfigAppService configAppService)
    {
        _configAppService = configAppService;
    }

    [HttpGet]
    public Dictionary<string, string> GetConfig()
    {
        return _configAppService.GetConfig();
    }
}