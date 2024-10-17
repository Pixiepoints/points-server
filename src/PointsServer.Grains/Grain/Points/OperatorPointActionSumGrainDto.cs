using PointsServer.Common;

namespace PointsServer.Grains.Grain.Points;

[GenerateSerializer]
public class OperatorPointActionSumGrainDto
{
    [Id(1)]
    public string Id { get; set; }
    [Id(2)]
    public string Domain { get; set; }
    [Id(3)]
    public string Address { get; set; }
    [Id(4)]
    public OperatorRole Role { get; set; }
    [Id(5)]
    public string DappName { get; set; }
    [Id(6)]
    public string RecordAction { get; set; }
    [Id(7)]
    public decimal Amount { get; set; }
    [Id(8)]
    public string PointSymbol { get; set; }
    [Id(9)]
    public long CreateTime { get; set; }
    [Id(10)]
    public long UpdateTime { get; set; }
}