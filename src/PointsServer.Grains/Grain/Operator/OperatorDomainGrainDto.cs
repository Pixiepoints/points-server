using PointsServer.Common;

namespace PointsServer.Grains.Grain.Operator;

public class OperatorDomainGrainDto
{
    public string Id { get; set; }
    public string Address { get; set; }
    public string InviterAddress { get; set; }
    public OperatorRole Role { get; set; }
    public ApplyStatus Status { get; set; }
    public string Domain { get; set; }
    public string Icon { get; set; }
    public string DappName { get; set; }
    public string Descibe { get; set; }
    public long ApplyTime { get; set; }
}