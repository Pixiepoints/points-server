namespace PointsServer.Apply.Dtos;

public class ApplyCheckInput
{
    public string DappName { get; set; }
    public bool UseOwnerDomain { get; set; }
    public string Domain { get; set; }
    public string Address { get; set; }
    public string Describe { get; set; }
}