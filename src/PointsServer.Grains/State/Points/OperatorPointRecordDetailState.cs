using PointsServer.Common;

namespace PointsServer.Grains.State.Points;

[GenerateSerializer]
public class OperatorPointRecordDetailState
{
    [Id(0)]
    public string Id { get; set; }
    [Id(1)]
    public string Address { get; set; }
    [Id(2)]
    public OperatorRole Role { get; set; }
    [Id(3)]
    public string Domain { get; set; }
    [Id(4)]
    public string DappName { get; set; }
    [Id(5)]
    public string RecordAction { get; set; }
    [Id(6)]
    public decimal Amount { get; set; }
    [Id(7)]
    public string PointSymbol { get; set; }
    [Id(8)]
    public long RecordTime { get; set; }
}