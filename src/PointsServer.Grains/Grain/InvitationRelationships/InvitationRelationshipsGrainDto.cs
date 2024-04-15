namespace PointsServer.Grains.Grain.InvitationRelationships;

public class InvitationRelationshipsGrainDto
{
    public string Id { get; set; }
    public string Address { get; set; }
    public string OperatorAddress { get; set; }
    public string InviterAddress { get; set; }
    public string DappName { get; set; }
    public string Domain { get; set; }
    public DateTime InviteTime { get; set; }
}