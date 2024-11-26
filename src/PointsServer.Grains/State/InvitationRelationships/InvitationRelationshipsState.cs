namespace PointsServer.Grains.State.InvitationRelationships;

[GenerateSerializer]
public class InvitationRelationshipsState
{
    [Id(0)]
    public string Id { get; set; }
    [Id(1)]
    public string Address { get; set; }
    [Id(2)]
    public string OperatorAddress { get; set; }
    [Id(3)]
    public string InviterAddress { get; set; }
    [Id(4)]
    public string DappName { get; set; }
    [Id(5)]
    public string Domain { get; set; }
    [Id(6)]
    public long InviteTime { get; set; }
}