namespace PointsServer.Grains.Grain.Worker;

[GenerateSerializer]
public class WorkerOptionGrainDto
{
    [Id(0)]
    public string Id { get; set; }
    [Id(1)]
    public string Type { get; set; }
    [Id(2)]
    public string ChainId { get; set; }
    [Id(3)]
    public long LatestExecuteTime { get; set; }
}