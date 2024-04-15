using PointsServer.Common;
using Volo.Abp.EventBus;

namespace PointsServer.Apply.Etos;

[EventName("OperatorDomainCreateEto")]
public class OperatorDomainCreateEto
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