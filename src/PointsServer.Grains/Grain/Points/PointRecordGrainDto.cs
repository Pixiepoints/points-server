using PointsServer.Common;

namespace PointsServer.Grains.Grain.Points;

public class PointRecordGrainDto
{
    public string Id { get; set; }
    public string Address { get; set; }
    public OperatorRole Role { get; set; }
    public string Domain { get; set; }
    public string DappName { get; set; }
    public string RecordAction { get; set; }
    public decimal Amount { get; set; }
    public string PointSymbol { get; set; }
    public long RecordTime { get; set; }
}