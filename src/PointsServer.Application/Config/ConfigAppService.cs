using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PointsServer.Options;
using Volo.Abp;
using Volo.Abp.Auditing;

namespace PointsServer.Config;

[DisableAuditing, RemoteService(false)]
public class ConfigAppService : PointsPlatformAppService, IConfigAppService
{
    private readonly CmsConfigOptions _options;

    public ConfigAppService(IOptionsSnapshot<CmsConfigOptions> options)
    {
        _options = options.Value;
    }

    public Dictionary<string, string> GetConfig()
    {
        return _options.ConfigMap;
    }
}