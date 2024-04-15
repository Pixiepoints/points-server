using System.Collections.Generic;

namespace PointsServer.Config;

public interface IConfigAppService
{
    Dictionary<string, string> GetConfig();
}