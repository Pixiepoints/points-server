using System;

namespace PointsServer.Points.Dtos;

public class GetPointsListInput
{
    public string DappName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public long SkipCount { get; set; }
    public long MaxResultCount { get; set; }
}