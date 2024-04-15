using System.Collections.Generic;

namespace PointsServer.Options;

public class CmsConfigOptions
{
    public Dictionary<string, string> ConfigMap { get; set; } = new();
}