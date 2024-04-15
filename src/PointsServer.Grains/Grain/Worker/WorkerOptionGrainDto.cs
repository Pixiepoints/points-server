namespace PointsServer.Grains.Grain.Worker;

public class WorkerOptionGrainDto
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string ChainId { get; set; }
    public long LatestExecuteTime { get; set; }
}