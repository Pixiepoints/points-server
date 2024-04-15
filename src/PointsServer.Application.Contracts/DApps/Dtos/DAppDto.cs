namespace PointsServer.DApps.Dtos;

public class DAppDto
{
    public string DappName { get; set; }
    public string DappId { get; set; }
    public string Icon { get; set; }
    public string Category { get; set; }
    public string Link { get; set; }
    public string SecondLevelDomain { get; set; }
    public string FirstLevelDomain { get; set; }
    public bool SupportsApply { get; set; }
}