using System.Collections.Generic;

namespace PointsServer.DApps.Dtos;

public class GetDAppListInput
{
    public string DappName { get; set; }
    public List<string> Categories { get; set; } = new();
}