namespace PointsServer.Points.Dtos;

public class GetAllPointsListInput
{
    public string DappName { get; set; }
    public string LastId { get; set; }
    public long LastBlockHeight { get; set; }
    public int MaxResultCount { get; set; }
}