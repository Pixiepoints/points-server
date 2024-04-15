namespace PointsServer.Grains.State.InvitationRelationships;

public class InvitationRelationshipsState
{
    public string Id { get; set; }
    public string Address { get; set; }
    public string OperatorAddress { get; set; }
    public string InviterAddress { get; set; }
    public string DappName { get; set; }
    public string Domain { get; set; }
    public long InviteTime { get; set; }
}