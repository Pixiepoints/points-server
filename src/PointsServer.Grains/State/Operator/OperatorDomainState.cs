using PointsServer.Common;

namespace PointsServer.Grains.State.Operator;

[GenerateSerializer]
public class OperatorDomainState
{
    [Id(0)]
    public string Id { get; set; }
    [Id(1)]
    public string Address { get; set; }
    [Id(2)]
    public string InviterAddress { get; set; }
    [Id(3)]
    public OperatorRole Role { get; set; }
    [Id(4)]
    public ApplyStatus Status { get; set; }
    [Id(5)]
    public string Domain { get; set; }
    [Id(6)]
    public string Icon { get; set; }
    [Id(7)]
    public string DappName { get; set; }
    [Id(8)]
    public string Descibe { get; set; }
    [Id(9)]
    public long ApplyTime { get; set; }
}